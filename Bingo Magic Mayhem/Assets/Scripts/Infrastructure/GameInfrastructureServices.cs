using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// SDK-free composition root for the first production infrastructure pass.
    /// Future UGS implementations should satisfy the same facade contracts.
    /// </summary>
    public sealed class GameInfrastructureServices
    {
        public ServiceEnvironment Environment { get; }
        public InfrastructureCompositionSnapshot Composition { get; }
        public ILocalDurableStateStore DurableState { get; }
        public IActionJournal ActionJournal { get; }
        public IIdentityFacade Identity { get; }
        public IAnalyticsFacade Analytics { get; }
        public IRemoteConfigFacade RemoteConfig { get; }
        public IProfileSettingsCloudSync ProfileSettingsCloudSync { get; }
        public IInfrastructureDiagnosticsFacade Diagnostics { get; }

        private GameInfrastructureServices(
            ServiceEnvironment environment,
            InfrastructureCompositionSnapshot composition,
            ILocalDurableStateStore durableState,
            IActionJournal actionJournal,
            IIdentityFacade identity,
            IAnalyticsFacade analytics,
            IRemoteConfigFacade remoteConfig,
            IProfileSettingsCloudSync profileSettingsCloudSync,
            IInfrastructureDiagnosticsFacade diagnostics)
        {
            Environment = environment;
            Composition = composition ?? throw new ArgumentNullException(nameof(composition));
            DurableState = durableState;
            ActionJournal = actionJournal;
            Identity = identity;
            Analytics = analytics;
            RemoteConfig = remoteConfig;
            ProfileSettingsCloudSync = profileSettingsCloudSync;
            Diagnostics = diagnostics;
        }

        public static GameInfrastructureServices CreateLocal(
            ServiceEnvironment environment = ServiceEnvironment.Prototype,
            IEnumerable<RemoteConfigEntry> configDefaults = null,
            string storageRoot = null)
        {
            return CreateConfigured(
                environment,
                new InfrastructureCompositionOptions(),
                configDefaults,
                storageRoot);
        }

        public static GameInfrastructureServices CreateConfigured(
            ServiceEnvironment environment = ServiceEnvironment.Prototype,
            InfrastructureCompositionOptions compositionOptions = null,
            IEnumerable<RemoteConfigEntry> configDefaults = null,
            string storageRoot = null)
        {
            string root = string.IsNullOrWhiteSpace(storageRoot)
                ? Path.Combine(Application.persistentDataPath, "bmm_infrastructure")
                : storageRoot;

            LocalStateMigrationRegistry migrations = new LocalStateMigrationRegistry();
            migrations.RegisterState(ProfileSettingsPersistence.StateName, ProfileSettingsState.CurrentSchemaVersion);
            migrations.RegisterMigration(ProfileSettingsPersistence.StateName, 1, ProfileSettingsPersistence.MigrateV1ToV2);

            JsonFileDurableStateStore durableState = new JsonFileDurableStateStore(Path.Combine(root, "state"), migrations);
            JsonLinesActionJournal actionJournal = new JsonLinesActionJournal(Path.Combine(root, "journal", "actions.jsonl"));
            IRemoteConfigFacade remoteConfig = new LocalRemoteConfigFacade(MergeRemoteConfigDefaults(configDefaults));
            InfrastructureCompositionOptions options = compositionOptions ?? new InfrastructureCompositionOptions();
            CompositionResolution composition = ResolveComposition(durableState, actionJournal, options);
            IIdentityFacade identity = composition.Identity;
            IAnalyticsFacade analytics = composition.Analytics;
            IProfileSettingsCloudSync profileSettingsCloudSync = composition.ProfileCloudSync;
            IInfrastructureDiagnosticsFacade diagnostics = new InfrastructureDiagnosticsFacade(
                environment,
                Path.Combine(root, "diagnostics"),
                durableState,
                actionJournal,
                identity,
                profileSettingsCloudSync,
                remoteConfig,
                composition.Snapshot);

            durableState.StateRecovered += stateName => actionJournal.RecordAction(
                identity.Current?.PlayerId ?? "uninitialized_local_guest",
                "persistence",
                "snapshot_recovered",
                JsonUtility.ToJson(new SnapshotRecoveredPayload { StateName = stateName }),
                status: JournalActionStatus.AppliedLocal);

            durableState.StateMigrated += migration => actionJournal.RecordAction(
                identity.Current?.PlayerId ?? "uninitialized_local_guest",
                "persistence",
                "snapshot_migrated",
                JsonUtility.ToJson(new SnapshotMigratedPayload
                {
                    StateName = migration.StateName,
                    FromVersion = migration.FromVersion,
                    ToVersion = migration.ToVersion
                }),
                status: JournalActionStatus.AppliedLocal);

            return new GameInfrastructureServices(
                environment,
                composition.Snapshot,
                durableState,
                actionJournal,
                identity,
                analytics,
                remoteConfig,
                profileSettingsCloudSync,
                diagnostics);
        }

        private static CompositionResolution ResolveComposition(
            JsonFileDurableStateStore durableState,
            JsonLinesActionJournal actionJournal,
            InfrastructureCompositionOptions options)
        {
            InfrastructureCompositionSnapshot snapshot = new InfrastructureCompositionSnapshot
            {
                DesiredIdentityProvider = Describe(options.Identity),
                DesiredAnalyticsProvider = Describe(options.Analytics),
                DesiredProfileCloudSyncProvider = Describe(options.ProfileCloudSync),
                ActiveIdentityProvider = "local_guest",
                ActiveAnalyticsProvider = "local_journal",
                ActiveProfileCloudSyncProvider = "disabled_local",
                UgsAdaptersCompiled = IsUgsAdapterCompiled(),
                UsesLocalFallback = false,
                FallbackReason = ""
            };

            IIdentityFacade identity = new LocalIdentityFacade(durableState);
            IAnalyticsFacade analytics = new LocalAnalyticsFacade(actionJournal, identity);
            IProfileSettingsCloudSync profileCloudSync = new DisabledProfileSettingsCloudSync();

#if BMM_UGS_ADAPTERS
            UgsAdapterRuntimePolicySnapshot runtimePolicy = UgsAdapterRuntimePolicy.Capture(true, options?.UgsRuntimeOptions);

            if (options.Identity == IdentityProviderPreference.UgsAnonymous && runtimePolicy.AllowsAuthentication)
            {
                identity = new UgsIdentityFacade(options.UgsRuntimeOptions);
                snapshot.ActiveIdentityProvider = "ugs_anonymous";
            }

            if (options.Analytics == AnalyticsProviderPreference.UgsAnalytics && runtimePolicy.AllowsAnalytics)
            {
                analytics = new UgsAnalyticsFacade(options.UgsRuntimeOptions);
                snapshot.ActiveAnalyticsProvider = "ugs_analytics";
            }

            if (options.ProfileCloudSync == ProfileCloudSyncPreference.UgsCloudSave && runtimePolicy.AllowsCloudSave)
            {
                profileCloudSync = new UgsCloudSaveProfileSettingsSync();
                snapshot.ActiveProfileCloudSyncProvider = "ugs_cloud_save";
            }
#endif

            snapshot.UsesLocalFallback =
                snapshot.ActiveIdentityProvider != snapshot.DesiredIdentityProvider ||
                snapshot.ActiveAnalyticsProvider != snapshot.DesiredAnalyticsProvider ||
                snapshot.ActiveProfileCloudSyncProvider != snapshot.DesiredProfileCloudSyncProvider;

            if (snapshot.UsesLocalFallback)
            {
                snapshot.FallbackReason = snapshot.UgsAdaptersCompiled
                    ? "UGS-backed providers were requested, but runtime approval gates keep local-first providers active."
                    : "UGS-backed providers were requested, but the compile gate is absent so local-first providers remain active.";
            }

            return new CompositionResolution
            {
                Snapshot = snapshot,
                Identity = identity,
                Analytics = analytics,
                ProfileCloudSync = profileCloudSync
            };
        }

        private static string Describe(IdentityProviderPreference preference)
        {
            return preference == IdentityProviderPreference.UgsAnonymous ? "ugs_anonymous" : "local_guest";
        }

        private static string Describe(AnalyticsProviderPreference preference)
        {
            return preference == AnalyticsProviderPreference.UgsAnalytics ? "ugs_analytics" : "local_journal";
        }

        private static string Describe(ProfileCloudSyncPreference preference)
        {
            return preference == ProfileCloudSyncPreference.UgsCloudSave ? "ugs_cloud_save" : "disabled_local";
        }

        private static bool IsUgsAdapterCompiled()
        {
#if BMM_UGS_ADAPTERS
            return true;
#else
            return false;
#endif
        }

        private sealed class CompositionResolution
        {
            public InfrastructureCompositionSnapshot Snapshot;
            public IIdentityFacade Identity;
            public IAnalyticsFacade Analytics;
            public IProfileSettingsCloudSync ProfileCloudSync;
        }

        private static IEnumerable<RemoteConfigEntry> MergeRemoteConfigDefaults(IEnumerable<RemoteConfigEntry> configDefaults)
        {
            Dictionary<string, RemoteConfigEntry> entries = new Dictionary<string, RemoteConfigEntry>(StringComparer.Ordinal);
            foreach (RemoteConfigEntry entry in RemoteConfigSafetyDiagnostics.CreateLocalSafetyDefaults())
            {
                entries[entry.Key] = entry;
            }

            if (configDefaults != null)
            {
                foreach (RemoteConfigEntry entry in configDefaults)
                {
                    if (entry != null && !string.IsNullOrWhiteSpace(entry.Key))
                    {
                        entries[entry.Key.Trim()] = new RemoteConfigEntry(entry.Key.Trim(), entry.Value ?? "");
                    }
                }
            }

            List<RemoteConfigEntry> merged = new List<RemoteConfigEntry>(entries.Values);
            merged.Sort((left, right) => string.CompareOrdinal(left.Key, right.Key));
            return merged;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await Identity.InitializeAsync(cancellationToken);
            await RemoteConfig.LoadAsync(cancellationToken);

            Analytics.Track(
                PrototypeAnalyticsEvents.InfrastructureInitialized,
                JsonUtility.ToJson(new InfrastructureInitializedPayload
                {
                    Environment = Environment.ToString().ToLowerInvariant(),
                    IdentityProvider = Identity.Current.Provider,
                    ConfigSource = RemoteConfig.Source,
                    ConfigRevision = RemoteConfig.Revision
                }));
        }

        [Serializable]
        private sealed class InfrastructureInitializedPayload
        {
            public string Environment = "";
            public string IdentityProvider = "";
            public string ConfigSource = "";
            public string ConfigRevision = "";
        }

        [Serializable]
        private sealed class SnapshotRecoveredPayload
        {
            public string StateName = "";
        }

        [Serializable]
        private sealed class SnapshotMigratedPayload
        {
            public string StateName = "";
            public int FromVersion;
            public int ToVersion;
        }
    }
}

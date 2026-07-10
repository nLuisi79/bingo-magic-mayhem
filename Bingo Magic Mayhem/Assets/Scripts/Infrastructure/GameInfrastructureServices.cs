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
        public ILocalDurableStateStore DurableState { get; }
        public IActionJournal ActionJournal { get; }
        public IIdentityFacade Identity { get; }
        public IAnalyticsFacade Analytics { get; }
        public IRemoteConfigFacade RemoteConfig { get; }
        public IInfrastructureDiagnosticsFacade Diagnostics { get; }

        private GameInfrastructureServices(
            ServiceEnvironment environment,
            ILocalDurableStateStore durableState,
            IActionJournal actionJournal,
            IIdentityFacade identity,
            IAnalyticsFacade analytics,
            IRemoteConfigFacade remoteConfig,
            IInfrastructureDiagnosticsFacade diagnostics)
        {
            Environment = environment;
            DurableState = durableState;
            ActionJournal = actionJournal;
            Identity = identity;
            Analytics = analytics;
            RemoteConfig = remoteConfig;
            Diagnostics = diagnostics;
        }

        public static GameInfrastructureServices CreateLocal(
            ServiceEnvironment environment = ServiceEnvironment.Prototype,
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
            IIdentityFacade identity = new LocalIdentityFacade(durableState);
            IRemoteConfigFacade remoteConfig = new LocalRemoteConfigFacade(configDefaults);
            IAnalyticsFacade analytics = new LocalAnalyticsFacade(actionJournal, identity);
            IInfrastructureDiagnosticsFacade diagnostics = new InfrastructureDiagnosticsFacade(
                environment,
                Path.Combine(root, "diagnostics"),
                durableState,
                actionJournal,
                identity);

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
                durableState,
                actionJournal,
                identity,
                analytics,
                remoteConfig,
                diagnostics);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await Identity.InitializeAsync(cancellationToken);
            await RemoteConfig.LoadAsync(cancellationToken);

            Analytics.Track(
                "infrastructure_initialized",
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

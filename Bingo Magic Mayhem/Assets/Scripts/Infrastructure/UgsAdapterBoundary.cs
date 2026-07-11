using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if BMM_UGS_ADAPTERS
using Unity.RemoteConfig;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
#endif

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// UGS package entries may exist in the manifest while runtime adapters remain
    /// disabled. Define BMM_UGS_ADAPTERS only after package resolution, consent,
    /// environment, and project-link checks are complete.
    /// </summary>
    public static class UgsAdapterBoundary
    {
        public const string RequiredScriptingDefine = "BMM_UGS_ADAPTERS";
        public const bool EnabledByDefault = false;
        public const string RequiredEnvironment = "development";
    }

    [Serializable]
    public sealed class UgsAdapterRuntimeOptions
    {
        public bool ProjectLinked;
        public bool EnvironmentApproved;
        public bool ConsentApproved;
        public bool AuthenticationApproved;
        public bool AnalyticsApproved;
        public bool CloudSaveApproved;
        public bool ServicesInitialized;
    }

    [Serializable]
    public sealed class UgsAdapterRuntimePolicySnapshot
    {
        public string PolicyVersion = "";
        public bool AdapterCompiled;
        public bool ProjectLinked;
        public bool EnvironmentApproved;
        public bool ConsentApproved;
        public bool AuthenticationApproved;
        public bool AnalyticsApproved;
        public bool CloudSaveApproved;
        public bool ServicesInitialized;
        public bool AllowsAuthentication;
        public bool AllowsAnalytics;
        public bool AllowsCloudSave;
        public string Reason = "";
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    public static class UgsAdapterRuntimePolicy
    {
        public const string PolicyVersion = "ugs_adapter_runtime_policy_v0.1";

        public static UgsAdapterRuntimePolicySnapshot Capture(
            bool adapterCompiled,
            UgsAdapterRuntimeOptions options = null)
        {
            options ??= new UgsAdapterRuntimeOptions();

            bool baseRequirementsMet = adapterCompiled
                                       && options.ProjectLinked
                                       && options.EnvironmentApproved
                                       && options.ServicesInitialized;

            UgsAdapterRuntimePolicySnapshot snapshot = new UgsAdapterRuntimePolicySnapshot
            {
                PolicyVersion = PolicyVersion,
                AdapterCompiled = adapterCompiled,
                ProjectLinked = options.ProjectLinked,
                EnvironmentApproved = options.EnvironmentApproved,
                ConsentApproved = options.ConsentApproved,
                AuthenticationApproved = options.AuthenticationApproved,
                AnalyticsApproved = options.AnalyticsApproved,
                CloudSaveApproved = options.CloudSaveApproved,
                ServicesInitialized = options.ServicesInitialized,
                AllowsAuthentication = baseRequirementsMet && options.AuthenticationApproved,
                AllowsAnalytics = baseRequirementsMet && options.ConsentApproved && options.AnalyticsApproved,
                AllowsCloudSave = baseRequirementsMet && options.ConsentApproved && options.CloudSaveApproved,
                Reason = "Compiled UGS adapters still require explicit runtime approval gates before Authentication, Analytics, or Cloud Save may execute live calls."
            };

            AddCheck(
                snapshot,
                "Adapter compile gate",
                adapterCompiled ? BackendPreflightStatus.Warning : BackendPreflightStatus.Blocked,
                adapterCompiled
                    ? "BMM_UGS_ADAPTERS is compiled; runtime approvals still decide whether live UGS calls are allowed."
                    : "BMM_UGS_ADAPTERS is absent; live UGS calls remain blocked.");

            AddCheck(
                snapshot,
                "Project link",
                options.ProjectLinked ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                options.ProjectLinked
                    ? "Unity project link is marked ready for future adapter testing."
                    : "Project link approval is still required before live UGS calls may run.");

            AddCheck(
                snapshot,
                "Environment approval",
                options.EnvironmentApproved ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                options.EnvironmentApproved
                    ? "Development environment approval is recorded for future adapter testing."
                    : "The required development environment is not yet approved for live adapter use.");

            AddCheck(
                snapshot,
                "Services initialization",
                options.ServicesInitialized ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                options.ServicesInitialized
                    ? "Unity Services initialization is marked ready."
                    : "Unity Services initialization remains blocked until an approved adapter composition enables it.");

            AddCheck(
                snapshot,
                "Authentication runtime approval",
                snapshot.AllowsAuthentication ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                snapshot.AllowsAuthentication
                    ? "Anonymous Authentication is explicitly approved for runtime use."
                    : "Authentication stays blocked until compile, project-link, environment, initialization, and auth approval gates all pass.");

            AddCheck(
                snapshot,
                "Analytics runtime approval",
                snapshot.AllowsAnalytics ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                snapshot.AllowsAnalytics
                    ? "Analytics is explicitly approved for runtime use with consent."
                    : "Analytics stays blocked until compile, project-link, environment, initialization, consent, and analytics approval gates all pass.");

            AddCheck(
                snapshot,
                "Cloud Save runtime approval",
                snapshot.AllowsCloudSave ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                snapshot.AllowsCloudSave
                    ? "Cloud Save is explicitly approved for runtime use with consent."
                    : "Cloud Save stays blocked until compile, project-link, environment, initialization, consent, and Cloud Save approval gates all pass.");

            return snapshot;
        }

        private static void AddCheck(
            UgsAdapterRuntimePolicySnapshot snapshot,
            string name,
            BackendPreflightStatus status,
            string detail)
        {
            snapshot.Checks.Add(new BackendPreflightCheck
            {
                Name = name ?? "",
                Status = status,
                Detail = detail ?? ""
            });
        }
    }

    public static class UgsPreflightDiagnostics
    {
        private const string PackageState = "resolved_lockfile_cache";

        public static BackendPreflightSnapshot Capture(ServiceEnvironment environment)
        {
            BackendPreflightSnapshot snapshot = new BackendPreflightSnapshot
            {
                Environment = environment.ToString().ToLowerInvariant(),
                PackageState = PackageState,
                AdapterDefine = IsAdapterDefineEnabled() ? "enabled" : "absent",
                LiveCloudCallsEnabled = false
            };

            AddCheck(
                snapshot,
                "UGS packages",
                BackendPreflightStatus.Pass,
                "Core, Authentication, Cloud Save, Remote Config, and Analytics are resolved in the lockfile/cache.");

            AddCheck(
                snapshot,
                "Adapter define",
                IsAdapterDefineEnabled() ? BackendPreflightStatus.Warning : BackendPreflightStatus.Pass,
                IsAdapterDefineEnabled()
                    ? "BMM_UGS_ADAPTERS is enabled; verify project link, consent, and offline fallback before testing."
                    : "BMM_UGS_ADAPTERS is absent, so runtime remains on the local-first path.");

            AddCheck(
                snapshot,
                "Project environment",
                BackendPreflightStatus.Blocked,
                "Development environment link is not enabled by the local composition root.");

            AddCheck(
                snapshot,
                "Analytics consent",
                BackendPreflightStatus.Blocked,
                "Live Analytics collection remains disabled until consent/privacy handling is approved.");

            AddCheck(
                snapshot,
                "Cloud Save conflict policy",
                BackendPreflightStatus.Blocked,
                "Cloud profile/settings sync remains disabled until write-lock and offline reconciliation policy is tested.");

            AddCheck(
                snapshot,
                "Gameplay/economy sync",
                BackendPreflightStatus.Pass,
                "Economy, Cloud Code, IAP, Leaderboards, and gameplay reward sync are still out of scope.");

            return snapshot;
        }

        private static void AddCheck(
            BackendPreflightSnapshot snapshot,
            string name,
            BackendPreflightStatus status,
            string detail)
        {
            snapshot.Checks.Add(new BackendPreflightCheck
            {
                Name = name ?? "",
                Status = status,
                Detail = detail ?? ""
            });

            if (status == BackendPreflightStatus.Blocked)
            {
                snapshot.BlockedCount++;
            }
            else if (status == BackendPreflightStatus.Warning)
            {
                snapshot.WarningCount++;
            }
        }

        private static bool IsAdapterDefineEnabled()
        {
#if BMM_UGS_ADAPTERS
            return true;
#else
            return false;
#endif
        }
    }

    public static class CloudProfileSyncDiagnostics
    {
        public const string ProfileSettingsCloudKey = "bmm.profile_settings.v2";
        public const string ServiceName = "ugs_cloud_save_profile_settings";
        public const string ConflictPolicyVersion = "profile_cloud_conflict_policy_v0.1";

        public static CloudProfileSyncStatus Capture(bool adapterCompiled)
        {
            CloudProfileConflictPolicySnapshot conflictPolicy = CaptureConflictPolicy();
            CloudProfileSyncStatus status = new CloudProfileSyncStatus
            {
                Service = ServiceName,
                CloudKey = ProfileSettingsCloudKey,
                LiveSyncEnabled = false,
                AdapterCompiled = adapterCompiled,
                ProjectLinked = false,
                EnvironmentApproved = false,
                ConsentApproved = false,
                ConflictPolicyApproved = false,
                OfflineFallbackTested = false,
                CanUpload = false,
                CanDownload = false,
                Reason = "Disabled: local profile/settings snapshots remain authoritative until project link, consent/privacy, conflict policy, and offline fallback are approved.",
                ConflictPolicy = conflictPolicy
            };

            AddCheck(
                status,
                "Cloud Save package",
                BackendPreflightStatus.Pass,
                "Cloud Save is resolved in the lockfile/cache, but no live sync client is constructed by the local composition root.");

            AddCheck(
                status,
                "Adapter compile gate",
                adapterCompiled ? BackendPreflightStatus.Warning : BackendPreflightStatus.Pass,
                adapterCompiled
                    ? "BMM_UGS_ADAPTERS is compiled; profile/settings sync still reports upload/download blocked until enablement gates pass."
                    : "BMM_UGS_ADAPTERS is absent, so profile/settings sync uses the disabled local facade.");

            AddCheck(
                status,
                "Project link",
                BackendPreflightStatus.Blocked,
                "No Unity project/environment link is accepted by this scaffold.");

            AddCheck(
                status,
                "Consent/privacy",
                BackendPreflightStatus.Blocked,
                "Profile/settings cloud writes require approved consent and privacy handling.");

            AddCheck(
                status,
                "Conflict policy",
                BackendPreflightStatus.Blocked,
                "Upload/download remains blocked until last-write, merge, and device conflict rules are approved and tested.");

            AddCheck(
                status,
                "Offline fallback",
                BackendPreflightStatus.Blocked,
                "Offline-first retry, stale remote data, and recovery behavior must be verified before enabling Cloud Save.");

            AddCheck(
                status,
                "Gameplay/economy scope",
                BackendPreflightStatus.Pass,
                "Only profile/settings is represented here; gameplay, economy, rewards, room progression, and album state are out of scope.");

            return status;
        }

        public static CloudProfileConflictPolicySnapshot CaptureConflictPolicy()
        {
            CloudProfileConflictPolicySnapshot policy = new CloudProfileConflictPolicySnapshot
            {
                PolicyVersion = ConflictPolicyVersion,
                StateName = ProfileSettingsPersistence.StateName,
                CloudKey = ProfileSettingsCloudKey,
                ConflictMode = "blocked_until_product_policy",
                LocalSnapshotAuthoritative = true,
                AllowsUpload = false,
                AllowsDownload = false,
                AllowsAutomaticMerge = false,
                AllowsRemoteOverwrite = false,
                AllowsGameplayStateSync = false,
                RequiredApprovalCount = 5,
                ApprovedGateCount = 0,
                BlockedGateCount = 5,
                LocalWinsRule = "OPEN: do not prefer local snapshot over cloud until timestamp and device-authority rules are approved.",
                RemoteWinsRule = "OPEN: do not overwrite local snapshot from cloud until recovery and user-visible fallback rules are approved.",
                SameTimestampRule = "OPEN: equal timestamps/device collisions require a deterministic rule before sync can run.",
                StaleRemoteRule = "OPEN: stale remote data handling must be approved before download is enabled.",
                OfflineRetryRule = "OPEN: queued retry/backoff and duplicate-write idempotency must be approved before upload is enabled.",
                BackupRecoveryRule = "OPEN: local backup recovery remains the only active recovery path until cloud restore behavior is tested.",
                NextRequiredApproval = "Approve profile/settings conflict policy before enabling Cloud Save upload or download."
            };

            AddPolicyCheck(
                policy,
                "Project/environment link",
                BackendPreflightStatus.Blocked,
                "Cloud Save sync cannot run until the intended Unity project and development environment are linked.");

            AddPolicyCheck(
                policy,
                "Consent/privacy",
                BackendPreflightStatus.Blocked,
                "Cross-device profile/settings storage needs approved consent, privacy copy, and data deletion expectations.");

            AddPolicyCheck(
                policy,
                "Timestamp authority",
                BackendPreflightStatus.Blocked,
                "No final rule exists for client timestamp, server timestamp, or device id tie-breaking.");

            AddPolicyCheck(
                policy,
                "Merge/overwrite behavior",
                BackendPreflightStatus.Blocked,
                "No automatic merge or remote overwrite is permitted until product approves local-vs-cloud outcomes.");

            AddPolicyCheck(
                policy,
                "Offline retry/idempotency",
                BackendPreflightStatus.Blocked,
                "No upload retry queue is active until duplicate write and stale retry behavior are tested.");

            AddPolicyCheck(
                policy,
                "Gameplay/economy isolation",
                BackendPreflightStatus.Pass,
                "This policy only covers profile/settings; gameplay, economy, rewards, album, jackpot, progression, Coven, and monetization state stay out of scope.");

            return policy;
        }

        private static void AddCheck(
            CloudProfileSyncStatus status,
            string name,
            BackendPreflightStatus checkStatus,
            string detail)
        {
            status.Checks.Add(new BackendPreflightCheck
            {
                Name = name ?? "",
                Status = checkStatus,
                Detail = detail ?? ""
            });
        }

        private static void AddPolicyCheck(
            CloudProfileConflictPolicySnapshot policy,
            string name,
            BackendPreflightStatus checkStatus,
            string detail)
        {
            policy.Checks.Add(new BackendPreflightCheck
            {
                Name = name ?? "",
                Status = checkStatus,
                Detail = detail ?? ""
            });
        }
    }

    public sealed class DisabledProfileSettingsCloudSync : IProfileSettingsCloudSync
    {
        private readonly CloudProfileSyncStatus status;

        public DisabledProfileSettingsCloudSync()
        {
            status = CloudProfileSyncDiagnostics.Capture(false);
        }

        public CloudProfileSyncStatus Status => status;

        public Task<CloudProfileSyncStatus> RefreshStatusAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(status);
        }

        public Task<bool> TryUploadAsync(ProfileSettingsState state, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(false);
        }

        public Task<ProfileSettingsState> TryDownloadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<ProfileSettingsState>(null);
        }
    }

#if BMM_UGS_ADAPTERS
    /// <summary>
    /// Live UGS adapters are intentionally compile-gated. They preserve the local
    /// facades but are not constructed by GameInfrastructureServices.CreateLocal.
    /// </summary>
    public sealed class UgsIdentityFacade : IIdentityFacade
    {
        private readonly UgsAdapterRuntimeOptions runtimeOptions;

        public UgsIdentityFacade(UgsAdapterRuntimeOptions runtimeOptions = null)
        {
            this.runtimeOptions = runtimeOptions ?? new UgsAdapterRuntimeOptions();
        }

        public IdentitySession Current { get; private set; }

        public async Task<IdentitySession> InitializeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            UgsAdapterRuntimePolicySnapshot policy = UgsAdapterRuntimePolicy.Capture(true, runtimeOptions);
            if (!policy.AllowsAuthentication)
            {
                throw new InvalidOperationException(policy.Reason);
            }

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Current = new IdentitySession
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                Provider = "unity_authentication_anonymous",
                IsCloudAuthenticated = true,
                IsAnonymous = true
            };
            return Current;
        }
    }

    public sealed class UgsAnalyticsFacade : IAnalyticsFacade
    {
        private readonly UgsAdapterRuntimeOptions runtimeOptions;

        public UgsAnalyticsFacade(UgsAdapterRuntimeOptions runtimeOptions = null)
        {
            this.runtimeOptions = runtimeOptions ?? new UgsAdapterRuntimeOptions();
        }

        public void Track(string eventName, string safePayloadJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("An analytics event name is required.", nameof(eventName));
            }

            UgsAdapterRuntimePolicySnapshot policy = UgsAdapterRuntimePolicy.Capture(true, runtimeOptions);
            if (!policy.AllowsAnalytics)
            {
                throw new InvalidOperationException(policy.Reason);
            }

            // Event schemas must be created in the Dashboard before live recording.
            AnalyticsService.Instance.RecordEvent(eventName.Trim());
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            UgsAdapterRuntimePolicySnapshot policy = UgsAdapterRuntimePolicy.Capture(true, runtimeOptions);
            if (!policy.AllowsAnalytics)
            {
                throw new InvalidOperationException(policy.Reason);
            }

            AnalyticsService.Instance.Flush();
            return Task.CompletedTask;
        }
    }

    public sealed class UgsRemoteConfigFacade : IRemoteConfigFacade
    {
        public string Source => "ugs_remote_config";
        public string Revision => RemoteConfigService.Instance.appConfig.configAssignmentHash;

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
        }

        public bool HasKey(string key) => RemoteConfigService.Instance.appConfig.HasKey(key);
        public string GetString(string key, string fallback = "") => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetString(key) : fallback;
        public int GetInt(string key, int fallback = 0) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetInt(key) : fallback;
        public float GetFloat(string key, float fallback = 0f) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetFloat(key) : fallback;
        public bool GetBool(string key, bool fallback = false) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetBool(key) : fallback;
        public IReadOnlyList<RemoteConfigEntry> GetAllEntries() => new List<RemoteConfigEntry>();
    }

    public sealed class UgsCloudSaveProfileSettingsSync : IProfileSettingsCloudSync
    {
        private const string ProfileSettingsKey = CloudProfileSyncDiagnostics.ProfileSettingsCloudKey;

        public CloudProfileSyncStatus Status => CloudProfileSyncDiagnostics.Capture(true);

        public Task<CloudProfileSyncStatus> RefreshStatusAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Status);
        }

        public Task<bool> TryUploadAsync(ProfileSettingsState state, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(false);
        }

        public Task<ProfileSettingsState> TryDownloadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<ProfileSettingsState>(null);
        }

        public async Task SaveAsync(ProfileSettingsState state, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { ProfileSettingsKey, JsonUtility.ToJson(state) }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        public async Task<ProfileSettingsState> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, Unity.Services.CloudSave.Models.Data.Player.PlayerData> data =
                await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { ProfileSettingsKey });
            if (!data.TryGetValue(ProfileSettingsKey, out Unity.Services.CloudSave.Models.Data.Player.PlayerData value))
            {
                return null;
            }

            return JsonUtility.FromJson<ProfileSettingsState>(value.Value.GetAs<string>());
        }
    }
#endif
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    public enum ServiceEnvironment
    {
        Local,
        Prototype,
        Beta
    }

    public enum JournalRecordKind
    {
        Action,
        StatusTransition
    }

    public enum JournalActionStatus
    {
        Pending,
        AppliedLocal,
        Synced,
        Rejected,
        Compensated
    }

    [Serializable]
    public sealed class IdentitySession
    {
        public string PlayerId = "";
        public string Provider = "local_guest";
        public bool IsCloudAuthenticated;
        public bool IsAnonymous = true;
    }

    [Serializable]
    public sealed class IdentitySafetySnapshot
    {
        public string PolicyVersion = "";
        public string Provider = "";
        public bool IsCloudAuthenticated;
        public bool IsAnonymous = true;
        public bool AdapterCompiled;
        public bool ProjectLinked;
        public bool EnvironmentApproved;
        public bool ConsentApproved;
        public bool AllowsCloudSignIn;
        public bool AllowsAccountLink;
        public bool AllowsRecovery;
        public bool RemoteFlagsRequestLiveAuth;
        public bool LiveRuntimeChangeAllowed;
        public string Reason = "";
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    [Serializable]
    public sealed class ActionJournalRecord
    {
        public string ActionId = "";
        public string PlayerId = "";
        public long Sequence;
        public string CreatedAtUtc = "";
        public string Source = "";
        public string Type = "";
        public string PayloadJson = "{}";
        public JournalRecordKind RecordKind;
        public JournalActionStatus Status;
        public string IdempotencyKey = "";
    }

    [Serializable]
    public sealed class RemoteConfigEntry
    {
        public string Key = "";
        public string Value = "";

        public RemoteConfigEntry()
        {
        }

        public RemoteConfigEntry(string key, string value)
        {
            Key = key ?? "";
            Value = value ?? "";
        }
    }

    [Serializable]
    public sealed class RemoteConfigSafetyEntry
    {
        public string Key = "";
        public string Value = "";
        public bool IsKnownInfrastructureKey;
        public bool IsPresent;
        public bool IsRiskyEnabled;
    }

    [Serializable]
    public sealed class RemoteConfigSafetySnapshot
    {
        public string PolicyVersion = "";
        public string Source = "";
        public string Revision = "";
        public int RequiredKeyCount;
        public int PresentRequiredKeyCount;
        public int MissingRequiredKeyCount;
        public int UnknownKeyCount;
        public int RiskyEnabledKeyCount;
        public bool UgsAdaptersEnabled;
        public bool CloudProfileSyncEnabled;
        public bool JournalUploadEnabled;
        public bool AnalyticsUploadEnabled;
        public bool DiagnosticsExportEnabled;
        public bool LiveRuntimeChangeAllowed;
        public string Reason = "";
        public List<RemoteConfigSafetyEntry> Entries = new List<RemoteConfigSafetyEntry>();
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    [Serializable]
    public sealed class AnalyticsSafetySnapshot
    {
        public string PolicyVersion = "";
        public bool AdapterCompiled;
        public bool ConsentApproved;
        public bool LiveUploadEnabled;
        public bool AllowsRuntimeUpload;
        public bool RemoteFlagsRequestUpload;
        public int TotalAnalyticsRecordCount;
        public int AllowlistedAnalyticsRecordCount;
        public int BlockedAnalyticsRecordCount;
        public string Reason = "";
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    public enum BackendPreflightStatus
    {
        Pass,
        Warning,
        Blocked
    }

    [Serializable]
    public sealed class BackendPreflightCheck
    {
        public string Name = "";
        public BackendPreflightStatus Status;
        public string Detail = "";
    }

    [Serializable]
    public sealed class BackendPreflightSnapshot
    {
        public string Environment = "";
        public string PackageState = "";
        public string AdapterDefine = "";
        public bool LiveCloudCallsEnabled;
        public int BlockedCount;
        public int WarningCount;
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    [Serializable]
    public sealed class JournalPolicySourceSummary
    {
        public string Event = "";
        public int RecordCount;
        public int FutureUploadEligibleCount;
        public int ActiveUploadEligibleCount;
        public int BlockedSensitiveCount;
        public int BlockedUnapprovedCount;
    }

    [Serializable]
    public sealed class JournalSyncPolicySnapshot
    {
        public string PolicyVersion = "";
        public bool LiveUploadsEnabled;
        public int RetainLocalRecordCount;
        public int ExportSummaryAllowedRecordCount;
        public int FutureUploadEligibleRecordCount;
        public int ActiveUploadEligibleRecordCount;
        public int BlockedSensitiveRecordCount;
        public int BlockedUnapprovedRecordCount;
        public int PendingRecordCount;
        public int AppliedLocalRecordCount;
        public int SyncedRecordCount;
        public int RejectedRecordCount;
        public int CompensatedRecordCount;
        public List<JournalPolicySourceSummary> SourceSummaries = new List<JournalPolicySourceSummary>();
    }

    [Serializable]
    public sealed class CloudProfileSyncStatus
    {
        public string Service = "ugs_cloud_save_profile_settings";
        public string CloudKey = "";
        public bool LiveSyncEnabled;
        public bool AdapterCompiled;
        public bool ProjectLinked;
        public bool EnvironmentApproved;
        public bool ConsentApproved;
        public bool ConflictPolicyApproved;
        public bool OfflineFallbackTested;
        public bool CanUpload;
        public bool CanDownload;
        public string Reason = "";
        public CloudProfileConflictPolicySnapshot ConflictPolicy = new CloudProfileConflictPolicySnapshot();
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    [Serializable]
    public sealed class CloudProfileConflictPolicySnapshot
    {
        public string PolicyVersion = "";
        public string StateName = "";
        public string CloudKey = "";
        public string ConflictMode = "blocked_until_product_policy";
        public bool LocalSnapshotAuthoritative = true;
        public bool AllowsUpload;
        public bool AllowsDownload;
        public bool AllowsAutomaticMerge;
        public bool AllowsRemoteOverwrite;
        public bool AllowsGameplayStateSync;
        public int RequiredApprovalCount;
        public int ApprovedGateCount;
        public int BlockedGateCount;
        public string LocalWinsRule = "open";
        public string RemoteWinsRule = "open";
        public string SameTimestampRule = "open";
        public string StaleRemoteRule = "open";
        public string OfflineRetryRule = "open";
        public string BackupRecoveryRule = "open";
        public string NextRequiredApproval = "";
        public List<BackendPreflightCheck> Checks = new List<BackendPreflightCheck>();
    }

    public interface ILocalDurableStateStore
    {
        bool TryLoad<T>(string stateName, out T value) where T : class;
        void Save<T>(string stateName, T value) where T : class;
    }

    public interface IActionJournal
    {
        ActionJournalRecord RecordAction(
            string playerId,
            string source,
            string type,
            string payloadJson = "{}",
            string idempotencyKey = "",
            JournalActionStatus status = JournalActionStatus.Pending,
            string actionId = "");

        ActionJournalRecord RecordStatus(
            string actionId,
            string playerId,
            string source,
            string type,
            JournalActionStatus status,
            string payloadJson = "{}");

        IReadOnlyList<ActionJournalRecord> ReadAll();
    }

    public interface IIdentityFacade
    {
        IdentitySession Current { get; }
        Task<IdentitySession> InitializeAsync(CancellationToken cancellationToken = default);
    }

    public interface IAnalyticsFacade
    {
        void Track(string eventName, string safePayloadJson = "{}");
        Task FlushAsync(CancellationToken cancellationToken = default);
    }

    public interface IRemoteConfigFacade
    {
        string Source { get; }
        string Revision { get; }
        Task LoadAsync(CancellationToken cancellationToken = default);
        bool HasKey(string key);
        string GetString(string key, string fallback = "");
        int GetInt(string key, int fallback = 0);
        float GetFloat(string key, float fallback = 0f);
        bool GetBool(string key, bool fallback = false);
        IReadOnlyList<RemoteConfigEntry> GetAllEntries();
    }

    public interface IProfileSettingsCloudSync
    {
        CloudProfileSyncStatus Status { get; }
        Task<CloudProfileSyncStatus> RefreshStatusAsync(CancellationToken cancellationToken = default);
        Task<bool> TryUploadAsync(ProfileSettingsState state, CancellationToken cancellationToken = default);
        Task<ProfileSettingsState> TryDownloadAsync(CancellationToken cancellationToken = default);
    }
}

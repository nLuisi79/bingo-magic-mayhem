using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    [Serializable]
    public sealed class SnapshotDiagnosticsEntry
    {
        public string StateName = "";
        public int SchemaVersion;
        public long Bytes;
        public bool HasBackup;
        public string Health = "unknown";
    }

    [Serializable]
    public sealed class DiagnosticsEventCount
    {
        public string Event = "";
        public int Count;
    }

    [Serializable]
    public sealed class InfrastructureDiagnosticsSnapshot
    {
        public string Environment = "";
        public string RedactedPlayerId = "";
        public string IdentityProvider = "";
        public int SnapshotCount;
        public List<SnapshotDiagnosticsEntry> Snapshots = new List<SnapshotDiagnosticsEntry>();
        public int JournalRecordCount;
        public int PendingActionRecordCount;
        public long JournalBytes;
        public long LastJournalSequence;
        public string LastRecoveredState = "none";
        public string LastRecoveryAtUtc = "";
        public string LastMigratedState = "none";
        public string LastMigration = "";
        public BackendPreflightSnapshot BackendPreflight = new BackendPreflightSnapshot();
        public IdentitySafetySnapshot IdentitySafety = new IdentitySafetySnapshot();
        public JournalSyncPolicySnapshot JournalSyncPolicy = new JournalSyncPolicySnapshot();
        public JournalRetentionPolicySnapshot JournalRetentionPolicy = new JournalRetentionPolicySnapshot();
        public AnalyticsSafetySnapshot AnalyticsSafety = new AnalyticsSafetySnapshot();
        public DiagnosticsExportSafetySnapshot ExportSafety = new DiagnosticsExportSafetySnapshot();
        public CloudProfileSyncStatus ProfileCloudSync = new CloudProfileSyncStatus();
        public RemoteConfigSafetySnapshot RemoteConfigSafety = new RemoteConfigSafetySnapshot();
        public InfrastructureCompositionSnapshot Composition = new InfrastructureCompositionSnapshot();
        public List<DiagnosticsEventCount> EventCounts = new List<DiagnosticsEventCount>();
        public string CapturedAtUtc = "";
    }

    public interface IInfrastructureDiagnosticsFacade
    {
        InfrastructureDiagnosticsSnapshot Capture();
        string ExportSafeSummary();
    }

    /// <summary>
    /// Produces redacted operational diagnostics only. Journal payloads, action ids,
    /// idempotency keys, messages, tokens, and full player ids are never exported.
    /// </summary>
    public sealed class InfrastructureDiagnosticsFacade : IInfrastructureDiagnosticsFacade
    {
        private readonly ServiceEnvironment environment;
        private readonly string exportDirectory;
        private readonly JsonFileDurableStateStore stateStore;
        private readonly JsonLinesActionJournal journal;
        private readonly IIdentityFacade identity;
        private readonly IProfileSettingsCloudSync profileSettingsCloudSync;
        private readonly IRemoteConfigFacade remoteConfig;
        private readonly InfrastructureCompositionSnapshot composition;

        public InfrastructureDiagnosticsFacade(
            ServiceEnvironment environment,
            string exportDirectory,
            JsonFileDurableStateStore stateStore,
            JsonLinesActionJournal journal,
            IIdentityFacade identity,
            IProfileSettingsCloudSync profileSettingsCloudSync,
            IRemoteConfigFacade remoteConfig,
            InfrastructureCompositionSnapshot composition)
        {
            this.environment = environment;
            this.exportDirectory = exportDirectory ?? throw new ArgumentNullException(nameof(exportDirectory));
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            this.profileSettingsCloudSync = profileSettingsCloudSync ?? throw new ArgumentNullException(nameof(profileSettingsCloudSync));
            this.remoteConfig = remoteConfig ?? throw new ArgumentNullException(nameof(remoteConfig));
            this.composition = composition ?? throw new ArgumentNullException(nameof(composition));
        }

        public InfrastructureDiagnosticsSnapshot Capture()
        {
            IReadOnlyList<ActionJournalRecord> records = journal.ReadAll();
            RemoteConfigSafetySnapshot remoteConfigSafety = RemoteConfigSafetyDiagnostics.Capture(remoteConfig);
            JournalRetentionPolicySnapshot retentionPolicy = JournalPolicyDiagnostics.CaptureRetentionPolicy(records);
            InfrastructureDiagnosticsSnapshot snapshot = new InfrastructureDiagnosticsSnapshot
            {
                Environment = environment.ToString().ToLowerInvariant(),
                RedactedPlayerId = Redact(identity.Current?.PlayerId),
                IdentityProvider = identity.Current?.Provider ?? "uninitialized",
                Snapshots = stateStore.CaptureSnapshotDiagnostics(),
                JournalRecordCount = records.Count,
                JournalBytes = journal.GetFileSizeBytes(),
                LastRecoveredState = stateStore.LastRecoveredState,
                LastRecoveryAtUtc = stateStore.LastRecoveryAtUtc,
                LastMigratedState = stateStore.LastMigratedState,
                LastMigration = stateStore.LastMigration,
                BackendPreflight = UgsPreflightDiagnostics.Capture(environment),
                IdentitySafety = IdentitySafetyDiagnostics.Capture(identity, remoteConfigSafety),
                JournalSyncPolicy = JournalPolicyDiagnostics.Capture(records),
                JournalRetentionPolicy = retentionPolicy,
                AnalyticsSafety = AnalyticsSafetyDiagnostics.Capture(records, remoteConfigSafety),
                ExportSafety = ExportSafetyDiagnostics.Capture(remoteConfigSafety, retentionPolicy),
                ProfileCloudSync = profileSettingsCloudSync.Status,
                RemoteConfigSafety = remoteConfigSafety,
                Composition = composition,
                CapturedAtUtc = DateTime.UtcNow.ToString("O")
            };
            snapshot.SnapshotCount = snapshot.Snapshots.Count;

            Dictionary<string, int> eventCounts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (ActionJournalRecord record in records)
            {
                snapshot.LastJournalSequence = Math.Max(snapshot.LastJournalSequence, record.Sequence);
                if (record.RecordKind == JournalRecordKind.Action && record.Status == JournalActionStatus.Pending)
                {
                    snapshot.PendingActionRecordCount++;
                }

                string key = (record.Source ?? "unknown") + "/" + (record.Type ?? "unknown");
                eventCounts[key] = eventCounts.TryGetValue(key, out int count) ? count + 1 : 1;
            }

            foreach (KeyValuePair<string, int> pair in eventCounts)
            {
                snapshot.EventCounts.Add(new DiagnosticsEventCount { Event = pair.Key, Count = pair.Value });
            }

            snapshot.EventCounts.Sort((left, right) => string.CompareOrdinal(left.Event, right.Event));
            return snapshot;
        }

        public string ExportSafeSummary()
        {
            Directory.CreateDirectory(exportDirectory);
            string path = Path.Combine(
                exportDirectory,
                "bmm_diagnostics_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".json");
            File.WriteAllText(path, JsonUtility.ToJson(Capture(), true), new UTF8Encoding(false));
            return path;
        }

        private static string Redact(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return "uninitialized";
            }

            if (playerId.Length <= 10)
            {
                return "***";
            }

            return playerId.Substring(0, 6) + "..." + playerId.Substring(playerId.Length - 4);
        }
    }

    public static class ExportSafetyDiagnostics
    {
        public const string PolicyVersion = "diagnostics_export_safety_v0.1";

        public static DiagnosticsExportSafetySnapshot Capture(
            RemoteConfigSafetySnapshot remoteConfigSafety,
            JournalRetentionPolicySnapshot retentionPolicy)
        {
            DiagnosticsExportSafetySnapshot snapshot = new DiagnosticsExportSafetySnapshot
            {
                PolicyVersion = PolicyVersion,
                LocalFileExportEnabled = true,
                PayloadFreeOnly = true,
                SensitivePayloadRedactionRequired = true,
                ExternalShareEnabled = false,
                InAppShareEnabled = false,
                ClipboardCopyEnabled = false,
                RemoteConfigRequestsExportEnabled = remoteConfigSafety == null || remoteConfigSafety.DiagnosticsExportEnabled,
                RemoteConfigCanDisableLocalExport = false,
                ExportBlockedRecordCount = retentionPolicy?.ExportBlockedRecordCount ?? 0,
                Reason = "Diagnostics export is limited to local payload-free files for support verification. In-app share, clipboard forwarding, and external Beta share flows remain blocked until privacy, support, and moderation rules are approved."
            };

            AddCheck(
                snapshot,
                "Local file export",
                BackendPreflightStatus.Pass,
                "Diagnostics can be exported only as a local redacted JSON file in this build.");

            AddCheck(
                snapshot,
                "Payload-free contract",
                BackendPreflightStatus.Pass,
                "Exported diagnostics omit journal payloads, messages, tokens, credentials, idempotency keys, and full player ids.");

            AddCheck(
                snapshot,
                "External share flow",
                BackendPreflightStatus.Blocked,
                "No in-app share, upload, email, or social handoff path is enabled for diagnostics exports.");

            AddCheck(
                snapshot,
                "Clipboard/quick copy",
                BackendPreflightStatus.Blocked,
                "Clipboard and one-tap copy flows are disabled so support-safe export review stays explicit.");

            AddCheck(
                snapshot,
                "Remote Config authority",
                snapshot.RemoteConfigRequestsExportEnabled ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.RemoteConfigRequestsExportEnabled
                    ? "Remote Config currently allows the local support export path to remain visible."
                    : "Remote Config requests export disabled, but the local support export remains code-authoritative until a Beta support workflow is approved.");

            AddCheck(
                snapshot,
                "Sensitive row redaction",
                snapshot.ExportBlockedRecordCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.ExportBlockedRecordCount == 0
                    ? "Current retained journal rows do not raise additional redaction warnings beyond the payload-free export contract."
                    : "One or more retained journal rows include sensitive markers and require payload-free export handling.");

            return snapshot;
        }

        private static void AddCheck(
            DiagnosticsExportSafetySnapshot snapshot,
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

    public static class JournalPolicyDiagnostics
    {
        private const string PolicyVersion = "local_journal_policy_v0.1";
        private const string RetentionPolicyVersion = "journal_retention_policy_v0.1";
        private static readonly bool LiveUploadsEnabled = false;

        private static readonly HashSet<string> FutureUploadAllowlist = new HashSet<string>(StringComparer.Ordinal)
        {
            "analytics/infrastructure_initialized",
            "persistence/snapshot_recovered",
            "persistence/snapshot_migrated",
            "profile_settings/profile_settings_initialized",
            "profile_settings/profile_settings_migrated",
            "profile_settings/avatar_selected",
            "profile_settings/frame_selected",
            "profile_settings/dauber_selected",
            "profile_settings/display_name_changed",
            "profile_settings/sound_preference_changed",
            "profile_settings/notification_preference_changed"
        };

        private static readonly string[] SensitivePayloadMarkers =
        {
            "\"message\"",
            "\"displayname\"",
            "\"display_name\"",
            "\"email\"",
            "\"password\"",
            "\"secret\"",
            "\"token\"",
            "\"access_token\"",
            "\"receipt\"",
            "\"credential\"",
            "\"idempotency\""
        };

        public static JournalSyncPolicySnapshot Capture(IReadOnlyList<ActionJournalRecord> records)
        {
            JournalSyncPolicySnapshot snapshot = new JournalSyncPolicySnapshot
            {
                PolicyVersion = PolicyVersion,
                LiveUploadsEnabled = LiveUploadsEnabled
            };
            Dictionary<string, JournalPolicySourceSummary> sourceSummaries =
                new Dictionary<string, JournalPolicySourceSummary>(StringComparer.Ordinal);

            foreach (ActionJournalRecord record in records)
            {
                snapshot.RetainLocalRecordCount++;
                snapshot.ExportSummaryAllowedRecordCount++;
                CountStatus(snapshot, record.Status);

                string eventKey = BuildEventKey(record);
                if (!sourceSummaries.TryGetValue(eventKey, out JournalPolicySourceSummary sourceSummary))
                {
                    sourceSummary = new JournalPolicySourceSummary { Event = eventKey };
                    sourceSummaries[eventKey] = sourceSummary;
                }

                sourceSummary.RecordCount++;
                if (ContainsSensitivePayloadMarker(record.PayloadJson))
                {
                    snapshot.BlockedSensitiveRecordCount++;
                    sourceSummary.BlockedSensitiveCount++;
                    continue;
                }

                if (!FutureUploadAllowlist.Contains(eventKey))
                {
                    snapshot.BlockedUnapprovedRecordCount++;
                    sourceSummary.BlockedUnapprovedCount++;
                    continue;
                }

                snapshot.FutureUploadEligibleRecordCount++;
                sourceSummary.FutureUploadEligibleCount++;
                if (LiveUploadsEnabled)
                {
                    snapshot.ActiveUploadEligibleRecordCount++;
                    sourceSummary.ActiveUploadEligibleCount++;
                }
            }

            foreach (KeyValuePair<string, JournalPolicySourceSummary> pair in sourceSummaries)
            {
                snapshot.SourceSummaries.Add(pair.Value);
            }

            snapshot.SourceSummaries.Sort((left, right) => string.CompareOrdinal(left.Event, right.Event));
            return snapshot;
        }

        public static JournalRetentionPolicySnapshot CaptureRetentionPolicy(IReadOnlyList<ActionJournalRecord> records)
        {
            JournalRetentionPolicySnapshot snapshot = new JournalRetentionPolicySnapshot
            {
                PolicyVersion = RetentionPolicyVersion,
                RetentionEnabled = false,
                ArchiveEnabled = false,
                CompactionEnabled = false,
                DeleteEnabled = false,
                ExportAllowed = true,
                SensitivePayloadRedactionRequired = true,
                Reason = "Journal retention, archive, compaction, and deletion remain disabled until privacy, support, and recovery policies are approved."
            };

            foreach (ActionJournalRecord record in records)
            {
                snapshot.TotalRecordCount++;
                snapshot.RetainedRecordCount++;

                if (ContainsSensitivePayloadMarker(record.PayloadJson))
                {
                    snapshot.ExportBlockedRecordCount++;
                    continue;
                }

                if (record.Status == JournalActionStatus.Synced ||
                    record.Status == JournalActionStatus.Rejected ||
                    record.Status == JournalActionStatus.Compensated)
                {
                    snapshot.ArchiveCandidateCount++;
                    snapshot.CompactionCandidateCount++;
                    snapshot.DeleteCandidateCount++;
                }
            }

            AddRetentionCheck(
                snapshot,
                "Retention gate",
                BackendPreflightStatus.Blocked,
                "No timed retention window is approved, so all journal rows remain retained locally.");

            AddRetentionCheck(
                snapshot,
                "Archive/compaction gate",
                BackendPreflightStatus.Blocked,
                "Archive and compaction candidates are counted for planning only; no archive or rewrite path is active.");

            AddRetentionCheck(
                snapshot,
                "Delete/clear gate",
                BackendPreflightStatus.Blocked,
                "Delete and clear operations remain disabled until support and recovery policies are approved.");

            AddRetentionCheck(
                snapshot,
                "Export redaction",
                snapshot.ExportBlockedRecordCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.ExportBlockedRecordCount == 0
                    ? "Current journal rows do not trigger export blocking by sensitive payload markers."
                    : "One or more retained journal rows require payload-free export redaction.");

            return snapshot;
        }

        private static void CountStatus(JournalSyncPolicySnapshot snapshot, JournalActionStatus status)
        {
            switch (status)
            {
                case JournalActionStatus.Pending:
                    snapshot.PendingRecordCount++;
                    break;
                case JournalActionStatus.AppliedLocal:
                    snapshot.AppliedLocalRecordCount++;
                    break;
                case JournalActionStatus.Synced:
                    snapshot.SyncedRecordCount++;
                    break;
                case JournalActionStatus.Rejected:
                    snapshot.RejectedRecordCount++;
                    break;
                case JournalActionStatus.Compensated:
                    snapshot.CompensatedRecordCount++;
                    break;
            }
        }

        private static string BuildEventKey(ActionJournalRecord record)
        {
            return (record.Source ?? "unknown") + "/" + (record.Type ?? "unknown");
        }

        private static bool ContainsSensitivePayloadMarker(string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                return false;
            }

            string lowered = payloadJson.ToLowerInvariant();
            foreach (string marker in SensitivePayloadMarkers)
            {
                if (lowered.Contains(marker))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddRetentionCheck(
            JournalRetentionPolicySnapshot snapshot,
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
}

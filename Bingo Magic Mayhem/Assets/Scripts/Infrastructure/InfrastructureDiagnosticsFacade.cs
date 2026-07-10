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
        public JournalSyncPolicySnapshot JournalSyncPolicy = new JournalSyncPolicySnapshot();
        public CloudProfileSyncStatus ProfileCloudSync = new CloudProfileSyncStatus();
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

        public InfrastructureDiagnosticsFacade(
            ServiceEnvironment environment,
            string exportDirectory,
            JsonFileDurableStateStore stateStore,
            JsonLinesActionJournal journal,
            IIdentityFacade identity,
            IProfileSettingsCloudSync profileSettingsCloudSync)
        {
            this.environment = environment;
            this.exportDirectory = exportDirectory ?? throw new ArgumentNullException(nameof(exportDirectory));
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            this.profileSettingsCloudSync = profileSettingsCloudSync ?? throw new ArgumentNullException(nameof(profileSettingsCloudSync));
        }

        public InfrastructureDiagnosticsSnapshot Capture()
        {
            IReadOnlyList<ActionJournalRecord> records = journal.ReadAll();
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
                JournalSyncPolicy = JournalPolicyDiagnostics.Capture(records),
                ProfileCloudSync = profileSettingsCloudSync.Status,
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

    public static class JournalPolicyDiagnostics
    {
        private const string PolicyVersion = "local_journal_policy_v0.1";
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
    }
}

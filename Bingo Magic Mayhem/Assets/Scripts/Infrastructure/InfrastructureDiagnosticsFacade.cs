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

        public InfrastructureDiagnosticsFacade(
            ServiceEnvironment environment,
            string exportDirectory,
            JsonFileDurableStateStore stateStore,
            JsonLinesActionJournal journal,
            IIdentityFacade identity)
        {
            this.environment = environment;
            this.exportDirectory = exportDirectory ?? throw new ArgumentNullException(nameof(exportDirectory));
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
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
}

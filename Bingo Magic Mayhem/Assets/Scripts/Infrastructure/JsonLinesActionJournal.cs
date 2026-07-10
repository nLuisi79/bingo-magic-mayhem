using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Append-only local journal. State changes are represented by new records;
    /// existing lines are never rewritten.
    /// </summary>
    public sealed class JsonLinesActionJournal : IActionJournal
    {
        private readonly string journalPath;
        private readonly object fileLock = new object();
        private long nextSequence;

        public JsonLinesActionJournal(string journalPath)
        {
            if (string.IsNullOrWhiteSpace(journalPath))
            {
                throw new ArgumentException("A journal path is required.", nameof(journalPath));
            }

            this.journalPath = journalPath;
            string directory = Path.GetDirectoryName(journalPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            nextSequence = FindNextSequence();
        }

        public ActionJournalRecord RecordAction(
            string playerId,
            string source,
            string type,
            string payloadJson = "{}",
            string idempotencyKey = "",
            JournalActionStatus status = JournalActionStatus.Pending,
            string actionId = "")
        {
            return Append(new ActionJournalRecord
            {
                ActionId = string.IsNullOrWhiteSpace(actionId) ? Guid.NewGuid().ToString("N") : actionId,
                PlayerId = playerId ?? "",
                Source = RequireLabel(source, nameof(source)),
                Type = RequireLabel(type, nameof(type)),
                PayloadJson = NormalizePayload(payloadJson),
                RecordKind = JournalRecordKind.Action,
                Status = status,
                IdempotencyKey = idempotencyKey ?? ""
            });
        }

        public ActionJournalRecord RecordStatus(
            string actionId,
            string playerId,
            string source,
            string type,
            JournalActionStatus status,
            string payloadJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                throw new ArgumentException("A status transition must reference an action id.", nameof(actionId));
            }

            return Append(new ActionJournalRecord
            {
                ActionId = actionId,
                PlayerId = playerId ?? "",
                Source = RequireLabel(source, nameof(source)),
                Type = RequireLabel(type, nameof(type)),
                PayloadJson = NormalizePayload(payloadJson),
                RecordKind = JournalRecordKind.StatusTransition,
                Status = status
            });
        }

        public IReadOnlyList<ActionJournalRecord> ReadAll()
        {
            List<ActionJournalRecord> records = new List<ActionJournalRecord>();
            lock (fileLock)
            {
                if (!File.Exists(journalPath))
                {
                    return records;
                }

                foreach (string line in File.ReadLines(journalPath, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    try
                    {
                        ActionJournalRecord record = JsonUtility.FromJson<ActionJournalRecord>(line);
                        if (record != null && record.Sequence > 0)
                        {
                            records.Add(record);
                        }
                    }
                    catch (ArgumentException exception)
                    {
                        Debug.LogWarning($"Skipped a malformed local journal row: {exception.Message}");
                    }
                }
            }

            return records;
        }

        public long GetFileSizeBytes()
        {
            lock (fileLock)
            {
                return File.Exists(journalPath) ? new FileInfo(journalPath).Length : 0L;
            }
        }

        private ActionJournalRecord Append(ActionJournalRecord record)
        {
            lock (fileLock)
            {
                record.Sequence = nextSequence++;
                record.CreatedAtUtc = DateTime.UtcNow.ToString("O");
                string line = JsonUtility.ToJson(record) + Environment.NewLine;
                byte[] bytes = new UTF8Encoding(false).GetBytes(line);

                using FileStream stream = new FileStream(journalPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
                return record;
            }
        }

        private long FindNextSequence()
        {
            long greatest = 0;
            if (!File.Exists(journalPath))
            {
                return 1;
            }

            foreach (string line in File.ReadLines(journalPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    ActionJournalRecord record = JsonUtility.FromJson<ActionJournalRecord>(line);
                    if (record != null)
                    {
                        greatest = Math.Max(greatest, record.Sequence);
                    }
                }
                catch (ArgumentException)
                {
                    // Preserve the append-only file and continue after malformed rows.
                }
            }

            return greatest + 1;
        }

        private static string RequireLabel(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Journal source and type labels are required.", parameterName);
            }

            return value.Trim();
        }

        private static string NormalizePayload(string payloadJson)
        {
            return string.IsNullOrWhiteSpace(payloadJson) ? "{}" : payloadJson.Trim();
        }
    }
}

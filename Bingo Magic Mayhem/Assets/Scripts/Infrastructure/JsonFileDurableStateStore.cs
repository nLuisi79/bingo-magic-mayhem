using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Stores versioned local snapshots outside PlayerPrefs. A primary snapshot is
    /// replaced atomically where the platform supports it and a last-known-good
    /// backup is retained for recovery.
    /// </summary>
    public sealed class JsonFileDurableStateStore : ILocalDurableStateStore
    {
        private const int CurrentEnvelopeVersion = 1;
        private readonly string rootDirectory;
        private readonly object fileLock = new object();
        private readonly LocalStateMigrationRegistry migrationRegistry;

        public event Action<string> StateRecovered;
        public event Action<SnapshotMigrationInfo> StateMigrated;

        public string LastRecoveredState { get; private set; } = "none";
        public string LastRecoveryAtUtc { get; private set; } = "";
        public string LastMigratedState { get; private set; } = "none";
        public string LastMigration { get; private set; } = "";

        public JsonFileDurableStateStore(string rootDirectory, LocalStateMigrationRegistry migrationRegistry = null)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("A durable-state directory is required.", nameof(rootDirectory));
            }

            this.rootDirectory = rootDirectory;
            this.migrationRegistry = migrationRegistry ?? new LocalStateMigrationRegistry();
            Directory.CreateDirectory(rootDirectory);
        }

        public bool TryLoad<T>(string stateName, out T value) where T : class
        {
            string primaryPath = GetStatePath(stateName);
            string backupPath = GetBackupPath(primaryPath);

            lock (fileLock)
            {
                SnapshotReadOutcome primaryOutcome = TryRead(stateName, primaryPath, out value, out SnapshotMigrationInfo primaryMigration);
                if (primaryOutcome == SnapshotReadOutcome.Success)
                {
                    ApplySuccessfulMigration(stateName, value, primaryMigration, persistMigratedSnapshot: true);
                    return true;
                }

                if (primaryOutcome == SnapshotReadOutcome.Unsupported)
                {
                    value = null;
                    return false;
                }

                SnapshotReadOutcome backupOutcome = TryRead(stateName, backupPath, out value, out SnapshotMigrationInfo backupMigration);
                if (backupOutcome == SnapshotReadOutcome.Success)
                {
                    Debug.LogWarning($"Recovered local state '{stateName}' from its last-known-good backup.");
                    LastRecoveredState = stateName;
                    LastRecoveryAtUtc = DateTime.UtcNow.ToString("O");
                    StateRecovered?.Invoke(stateName);
                    ApplySuccessfulMigration(stateName, value, backupMigration, persistMigratedSnapshot: false);
                    return true;
                }
            }

            value = null;
            return false;
        }

        public void Save<T>(string stateName, T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string primaryPath = GetStatePath(stateName);
            string backupPath = GetBackupPath(primaryPath);
            string temporaryPath = primaryPath + ".tmp";
            StateEnvelope envelope = new StateEnvelope
            {
                SchemaVersion = CurrentEnvelopeVersion,
                StateSchemaVersion = GetStateSchemaVersion(stateName, value),
                SavedAtUtc = DateTime.UtcNow.ToString("O"),
                PayloadJson = JsonUtility.ToJson(value)
            };
            string json = JsonUtility.ToJson(envelope);

            lock (fileLock)
            {
                Directory.CreateDirectory(rootDirectory);
                WriteAndFlush(temporaryPath, json);
                ReplacePrimary(temporaryPath, primaryPath, backupPath);
            }
        }

        public List<SnapshotDiagnosticsEntry> CaptureSnapshotDiagnostics()
        {
            List<SnapshotDiagnosticsEntry> entries = new List<SnapshotDiagnosticsEntry>();
            lock (fileLock)
            {
                if (!Directory.Exists(rootDirectory))
                {
                    return entries;
                }

                foreach (string path in Directory.GetFiles(rootDirectory, "*.snapshot.json", SearchOption.TopDirectoryOnly))
                {
                    string fileName = Path.GetFileName(path);
                    string stateName = fileName.Substring(0, fileName.Length - ".snapshot.json".Length);
                    SnapshotDiagnosticsEntry entry = new SnapshotDiagnosticsEntry
                    {
                        StateName = stateName,
                        Bytes = new FileInfo(path).Length,
                        HasBackup = File.Exists(GetBackupPath(path)),
                        Health = "malformed"
                    };

                    try
                    {
                        StateEnvelope envelope = JsonUtility.FromJson<StateEnvelope>(File.ReadAllText(path, Encoding.UTF8));
                        if (envelope != null && envelope.SchemaVersion > 0 && !string.IsNullOrWhiteSpace(envelope.PayloadJson))
                        {
                            entry.SchemaVersion = ResolveStoredStateVersion(envelope);
                            int supportedVersion = migrationRegistry.GetCurrentVersion(stateName, entry.SchemaVersion);
                            entry.Health = entry.SchemaVersion > supportedVersion ? "newer_than_client" : "healthy";
                        }
                    }
                    catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
                    {
                        entry.Health = "unreadable";
                    }

                    entries.Add(entry);
                }
            }

            entries.Sort((left, right) => string.CompareOrdinal(left.StateName, right.StateName));
            return entries;
        }

        private SnapshotReadOutcome TryRead<T>(
            string stateName,
            string path,
            out T value,
            out SnapshotMigrationInfo migrationInfo) where T : class
        {
            value = null;
            migrationInfo = null;
            if (!File.Exists(path))
            {
                return SnapshotReadOutcome.Missing;
            }

            try
            {
                StateEnvelope envelope = JsonUtility.FromJson<StateEnvelope>(File.ReadAllText(path, Encoding.UTF8));
                if (envelope == null || envelope.SchemaVersion <= 0 || string.IsNullOrWhiteSpace(envelope.PayloadJson))
                {
                    return SnapshotReadOutcome.Unreadable;
                }

                int storedStateVersion = ResolveStoredStateVersion(envelope);
                string payload = migrationRegistry.Migrate(stateName, storedStateVersion, envelope.PayloadJson, out migrationInfo);
                value = JsonUtility.FromJson<T>(payload);
                return value != null ? SnapshotReadOutcome.Success : SnapshotReadOutcome.Unreadable;
            }
            catch (InvalidDataException exception)
            {
                Debug.LogError($"Cannot load local state '{stateName}': {exception.Message}");
                return SnapshotReadOutcome.Unsupported;
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
            {
                Debug.LogWarning($"Could not read local state '{path}': {exception.Message}");
                return SnapshotReadOutcome.Unreadable;
            }
        }

        private void ApplySuccessfulMigration<T>(
            string stateName,
            T value,
            SnapshotMigrationInfo migrationInfo,
            bool persistMigratedSnapshot) where T : class
        {
            if (migrationInfo == null || !migrationInfo.WasMigrated)
            {
                return;
            }

            if (persistMigratedSnapshot)
            {
                Save(stateName, value);
            }

            LastMigratedState = stateName;
            LastMigration = migrationInfo.FromVersion + " -> " + migrationInfo.ToVersion;
            StateMigrated?.Invoke(migrationInfo);
        }

        private int GetStateSchemaVersion<T>(string stateName, T value) where T : class
        {
            int fallback = value is IVersionedLocalState versioned ? versioned.SchemaVersion : 1;
            return migrationRegistry.GetCurrentVersion(stateName, fallback);
        }

        private static int ResolveStoredStateVersion(StateEnvelope envelope)
        {
            if (envelope.StateSchemaVersion > 0)
            {
                return envelope.StateSchemaVersion;
            }

            StateVersionProbe probe = JsonUtility.FromJson<StateVersionProbe>(envelope.PayloadJson);
            return probe != null && probe.SchemaVersion > 0 ? probe.SchemaVersion : 1;
        }

        private string GetStatePath(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName) ||
                !string.Equals(Path.GetFileName(stateName), stateName, StringComparison.Ordinal) ||
                stateName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("State names must be simple file-safe names.", nameof(stateName));
            }

            return Path.Combine(rootDirectory, stateName + ".snapshot.json");
        }

        private static string GetBackupPath(string primaryPath)
        {
            return primaryPath + ".bak";
        }

        private static void WriteAndFlush(string path, string contents)
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(contents);
            using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush(true);
        }

        private static void ReplacePrimary(string temporaryPath, string primaryPath, string backupPath)
        {
            if (!File.Exists(primaryPath))
            {
                File.Move(temporaryPath, primaryPath);
                return;
            }

            try
            {
                File.Replace(temporaryPath, primaryPath, backupPath);
            }
            catch (PlatformNotSupportedException)
            {
                ReplaceWithBackupFallback(temporaryPath, primaryPath, backupPath);
            }
            catch (IOException)
            {
                ReplaceWithBackupFallback(temporaryPath, primaryPath, backupPath);
            }
        }

        private static void ReplaceWithBackupFallback(string temporaryPath, string primaryPath, string backupPath)
        {
            File.Copy(primaryPath, backupPath, true);
            File.Delete(primaryPath);
            File.Move(temporaryPath, primaryPath);
        }

        [Serializable]
        private sealed class StateEnvelope
        {
            public int SchemaVersion;
            public int StateSchemaVersion;
            public string SavedAtUtc = "";
            public string PayloadJson = "";
        }

        [Serializable]
        private sealed class StateVersionProbe
        {
            public int SchemaVersion = 1;
        }

        private enum SnapshotReadOutcome
        {
            Missing,
            Success,
            Unreadable,
            Unsupported
        }
    }
}

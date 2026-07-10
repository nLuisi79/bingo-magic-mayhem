using System;
using System.Collections.Generic;
using System.IO;

namespace BingoMagicMayhem.Infrastructure
{
    public interface IVersionedLocalState
    {
        int SchemaVersion { get; }
    }

    public sealed class SnapshotMigrationInfo
    {
        public string StateName = "";
        public int FromVersion;
        public int ToVersion;
        public bool WasMigrated;
    }

    /// <summary>
    /// Explicit, ordered snapshot migrations. A state can only advance one version
    /// at a time, preventing silent skips when an intermediate transform is absent.
    /// </summary>
    public sealed class LocalStateMigrationRegistry
    {
        private readonly Dictionary<string, StateMigrationDefinition> definitions =
            new Dictionary<string, StateMigrationDefinition>(StringComparer.Ordinal);

        public void RegisterState(string stateName, int currentVersion)
        {
            ValidateStateName(stateName);
            if (currentVersion < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(currentVersion));
            }

            if (definitions.TryGetValue(stateName, out StateMigrationDefinition existing))
            {
                existing.CurrentVersion = currentVersion;
                return;
            }

            definitions[stateName] = new StateMigrationDefinition { CurrentVersion = currentVersion };
        }

        public void RegisterMigration(string stateName, int fromVersion, Func<string, string> migration)
        {
            ValidateStateName(stateName);
            if (fromVersion < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(fromVersion));
            }

            if (migration == null)
            {
                throw new ArgumentNullException(nameof(migration));
            }

            if (!definitions.TryGetValue(stateName, out StateMigrationDefinition definition))
            {
                throw new InvalidOperationException($"Register state '{stateName}' before adding migrations.");
            }

            definition.Steps[fromVersion] = migration;
        }

        public int GetCurrentVersion(string stateName, int fallbackVersion)
        {
            return definitions.TryGetValue(stateName, out StateMigrationDefinition definition)
                ? definition.CurrentVersion
                : Math.Max(1, fallbackVersion);
        }

        public string Migrate(string stateName, int storedVersion, string payloadJson, out SnapshotMigrationInfo info)
        {
            int safeStoredVersion = Math.Max(1, storedVersion);
            info = new SnapshotMigrationInfo
            {
                StateName = stateName,
                FromVersion = safeStoredVersion,
                ToVersion = safeStoredVersion
            };

            if (!definitions.TryGetValue(stateName, out StateMigrationDefinition definition))
            {
                return payloadJson;
            }

            if (safeStoredVersion > definition.CurrentVersion)
            {
                throw new InvalidDataException(
                    $"Snapshot '{stateName}' uses schema {safeStoredVersion}, newer than supported schema {definition.CurrentVersion}.");
            }

            string migrated = payloadJson;
            int version = safeStoredVersion;
            while (version < definition.CurrentVersion)
            {
                if (!definition.Steps.TryGetValue(version, out Func<string, string> step))
                {
                    throw new InvalidDataException(
                        $"Snapshot '{stateName}' has no migration from schema {version} to {version + 1}.");
                }

                migrated = step(migrated);
                if (string.IsNullOrWhiteSpace(migrated))
                {
                    throw new InvalidDataException(
                        $"Snapshot '{stateName}' migration from schema {version} returned an empty payload.");
                }

                version++;
            }

            info.ToVersion = version;
            info.WasMigrated = version != safeStoredVersion;
            return migrated;
        }

        private static void ValidateStateName(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
            {
                throw new ArgumentException("A state name is required.", nameof(stateName));
            }
        }

        private sealed class StateMigrationDefinition
        {
            public int CurrentVersion;
            public readonly Dictionary<int, Func<string, string>> Steps =
                new Dictionary<int, Func<string, string>>();
        }
    }
}

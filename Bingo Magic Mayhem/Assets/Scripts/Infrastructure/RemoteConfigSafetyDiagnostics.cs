using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Infrastructure
{
    public static class RemoteConfigSafetyDiagnostics
    {
        public const string PolicyVersion = "infra_remote_config_safety_v0.1";
        public const string UgsAdaptersEnabledKey = "infra_ugs_adapters_enabled";
        public const string CloudProfileSyncEnabledKey = "infra_cloud_profile_sync_enabled";
        public const string JournalUploadEnabledKey = "infra_journal_upload_enabled";
        public const string DiagnosticsExportEnabledKey = "infra_diagnostics_export_enabled";

        private static readonly string[] RequiredKeys =
        {
            UgsAdaptersEnabledKey,
            CloudProfileSyncEnabledKey,
            JournalUploadEnabledKey,
            DiagnosticsExportEnabledKey
        };

        private static readonly HashSet<string> KnownInfrastructureKeys =
            new HashSet<string>(RequiredKeys, StringComparer.Ordinal);

        public static IEnumerable<RemoteConfigEntry> CreateLocalSafetyDefaults()
        {
            return new[]
            {
                new RemoteConfigEntry(UgsAdaptersEnabledKey, "false"),
                new RemoteConfigEntry(CloudProfileSyncEnabledKey, "false"),
                new RemoteConfigEntry(JournalUploadEnabledKey, "false"),
                new RemoteConfigEntry(DiagnosticsExportEnabledKey, "true")
            };
        }

        public static RemoteConfigSafetySnapshot Capture(IRemoteConfigFacade remoteConfig)
        {
            if (remoteConfig == null)
            {
                throw new ArgumentNullException(nameof(remoteConfig));
            }

            RemoteConfigSafetySnapshot snapshot = new RemoteConfigSafetySnapshot
            {
                PolicyVersion = PolicyVersion,
                Source = remoteConfig.Source,
                Revision = remoteConfig.Revision,
                RequiredKeyCount = RequiredKeys.Length,
                UgsAdaptersEnabled = remoteConfig.GetBool(UgsAdaptersEnabledKey, false),
                CloudProfileSyncEnabled = remoteConfig.GetBool(CloudProfileSyncEnabledKey, false),
                JournalUploadEnabled = remoteConfig.GetBool(JournalUploadEnabledKey, false),
                DiagnosticsExportEnabled = remoteConfig.GetBool(DiagnosticsExportEnabledKey, true),
                LiveRuntimeChangeAllowed = false,
                Reason = "Remote Config is diagnostics-only for infrastructure flags; live UGS, Cloud Save sync, and journal upload remain disabled by code gates."
            };

            foreach (string key in RequiredKeys)
            {
                bool present = remoteConfig.HasKey(key);
                bool enabled = remoteConfig.GetBool(key, false);
                snapshot.Entries.Add(new RemoteConfigSafetyEntry
                {
                    Key = key,
                    Value = present ? remoteConfig.GetString(key, "") : "<missing>",
                    IsKnownInfrastructureKey = true,
                    IsPresent = present,
                    IsRiskyEnabled = IsRiskyEnableKey(key) && enabled
                });

                if (present)
                {
                    snapshot.PresentRequiredKeyCount++;
                }
                else
                {
                    snapshot.MissingRequiredKeyCount++;
                }
            }

            foreach (RemoteConfigEntry entry in remoteConfig.GetAllEntries())
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key))
                {
                    continue;
                }

                string key = entry.Key.Trim();
                if (KnownInfrastructureKeys.Contains(key))
                {
                    continue;
                }

                snapshot.UnknownKeyCount++;
                snapshot.Entries.Add(new RemoteConfigSafetyEntry
                {
                    Key = key,
                    Value = "<redacted_unknown>",
                    IsKnownInfrastructureKey = false,
                    IsPresent = true,
                    IsRiskyEnabled = false
                });
            }

            foreach (RemoteConfigSafetyEntry entry in snapshot.Entries)
            {
                if (entry.IsRiskyEnabled)
                {
                    snapshot.RiskyEnabledKeyCount++;
                }
            }

            AddCheck(
                snapshot,
                "Required infrastructure keys",
                snapshot.MissingRequiredKeyCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.MissingRequiredKeyCount == 0
                    ? "All required infrastructure Remote Config keys are present."
                    : "One or more infrastructure Remote Config keys are missing and using safe local fallbacks.");

            AddCheck(
                snapshot,
                "Unknown keys",
                snapshot.UnknownKeyCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.UnknownKeyCount == 0
                    ? "No unknown Remote Config keys are visible to the local diagnostics surface."
                    : "Unknown Remote Config keys are visible; confirm they are not gameplay/economy tuning before live fetch is enabled.");

            AddCheck(
                snapshot,
                "Risky enable flags",
                snapshot.RiskyEnabledKeyCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Blocked,
                snapshot.RiskyEnabledKeyCount == 0
                    ? "UGS adapters, Cloud Save profile sync, and journal upload flags are off."
                    : "One or more live/cloud infrastructure flags are true, but runtime remains code-gated off until approved.");

            AddCheck(
                snapshot,
                "Diagnostics export flag",
                snapshot.DiagnosticsExportEnabled ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.DiagnosticsExportEnabled
                    ? "Safe diagnostics export remains available locally."
                    : "Diagnostics export is disabled by config and should be verified before Beta support workflows.");

            return snapshot;
        }

        private static bool IsRiskyEnableKey(string key)
        {
            return key == UgsAdaptersEnabledKey ||
                   key == CloudProfileSyncEnabledKey ||
                   key == JournalUploadEnabledKey;
        }

        private static void AddCheck(
            RemoteConfigSafetySnapshot snapshot,
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

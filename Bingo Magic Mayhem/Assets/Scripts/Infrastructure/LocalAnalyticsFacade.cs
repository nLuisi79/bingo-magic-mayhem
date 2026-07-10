using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Consent-safe local analytics seam. Events stay in the local journal until
    /// a future analytics adapter and upload policy are approved.
    /// </summary>
    public sealed class LocalAnalyticsFacade : IAnalyticsFacade
    {
        private readonly IActionJournal journal;
        private readonly IIdentityFacade identity;

        public LocalAnalyticsFacade(IActionJournal journal, IIdentityFacade identity)
        {
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        public void Track(string eventName, string safePayloadJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("An analytics event name is required.", nameof(eventName));
            }

            journal.RecordAction(
                identity.Current?.PlayerId ?? "uninitialized_local_guest",
                "analytics",
                eventName.Trim(),
                safePayloadJson,
                status: JournalActionStatus.AppliedLocal);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }

    public static class AnalyticsSafetyDiagnostics
    {
        public const string PolicyVersion = "analytics_safety_v0.1";

        private static readonly HashSet<string> AllowlistedAnalyticsEvents =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "analytics/infrastructure_initialized"
            };

        public static AnalyticsSafetySnapshot Capture(
            IReadOnlyList<ActionJournalRecord> records,
            RemoteConfigSafetySnapshot remoteConfigSafety)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            bool remoteFlagsRequestUpload = remoteConfigSafety != null &&
                                            (remoteConfigSafety.AnalyticsUploadEnabled || remoteConfigSafety.JournalUploadEnabled);

            AnalyticsSafetySnapshot snapshot = new AnalyticsSafetySnapshot
            {
                PolicyVersion = PolicyVersion,
                AdapterCompiled = IsAdapterDefineEnabled(),
                ConsentApproved = false,
                LiveUploadEnabled = false,
                AllowsRuntimeUpload = false,
                RemoteFlagsRequestUpload = remoteFlagsRequestUpload,
                Reason = "Analytics remains local-only until consent/privacy, event approval, and live upload policy are explicitly approved."
            };

            foreach (ActionJournalRecord record in records)
            {
                if (record.RecordKind != JournalRecordKind.Action || !string.Equals(record.Source, "analytics", StringComparison.Ordinal))
                {
                    continue;
                }

                snapshot.TotalAnalyticsRecordCount++;
                string eventKey = BuildEventKey(record);
                if (AllowlistedAnalyticsEvents.Contains(eventKey))
                {
                    snapshot.AllowlistedAnalyticsRecordCount++;
                }
                else
                {
                    snapshot.BlockedAnalyticsRecordCount++;
                }
            }

            AddCheck(
                snapshot,
                "Local analytics sink",
                BackendPreflightStatus.Pass,
                "Analytics events are recorded only into the local journal in this build.");

            AddCheck(
                snapshot,
                "Consent/privacy",
                BackendPreflightStatus.Blocked,
                "Live analytics upload remains blocked until consent, privacy, and deletion handling are approved.");

            AddCheck(
                snapshot,
                "Event allowlist",
                snapshot.BlockedAnalyticsRecordCount == 0 ? BackendPreflightStatus.Pass : BackendPreflightStatus.Warning,
                snapshot.BlockedAnalyticsRecordCount == 0
                    ? "Observed analytics events are within the current infrastructure allowlist."
                    : "One or more analytics events are outside the current infrastructure allowlist and would remain local-only.");

            AddCheck(
                snapshot,
                "Remote Config bypass",
                remoteFlagsRequestUpload ? BackendPreflightStatus.Blocked : BackendPreflightStatus.Pass,
                remoteFlagsRequestUpload
                    ? "Remote Config may request analytics/journal upload, but runtime upload remains code-gated off."
                    : "Remote Config is not requesting analytics upload behavior.");

            AddCheck(
                snapshot,
                "Runtime upload",
                BackendPreflightStatus.Blocked,
                "No live analytics upload path is active in the local composition root.");

            return snapshot;
        }

        private static string BuildEventKey(ActionJournalRecord record)
        {
            return (record.Source ?? "unknown") + "/" + (record.Type ?? "unknown");
        }

        private static void AddCheck(
            AnalyticsSafetySnapshot snapshot,
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

        private static bool IsAdapterDefineEnabled()
        {
#if BMM_UGS_ADAPTERS
            return true;
#else
            return false;
#endif
        }
    }
}

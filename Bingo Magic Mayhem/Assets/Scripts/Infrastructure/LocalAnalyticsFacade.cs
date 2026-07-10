using System;
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
}

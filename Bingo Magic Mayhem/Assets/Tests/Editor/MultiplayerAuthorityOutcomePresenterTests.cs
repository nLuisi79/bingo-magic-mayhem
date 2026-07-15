using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerAuthorityOutcomePresenterTests
{
    [Test]
    public void BuildClaimOutcome_AcceptedJackpotClaim_QueuesCelebrationAndHandoff()
    {
        MatchClaimResolution resolution = new MatchClaimResolution
        {
            ClaimType = MatchClaimType.JackpotState,
            Result = MatchClaimResolutionKind.Accepted,
            Reason = "Jackpot state verified."
        };

        MultiplayerAuthorityOutcomeModel outcome = MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(resolution);

        Assert.That(outcome.FeedbackKind, Is.EqualTo(MultiplayerAuthorityFeedbackKind.Success));
        Assert.That(outcome.Headline, Is.EqualTo("Claim accepted."));
        Assert.That(outcome.ShouldCelebrate, Is.True);
        Assert.That(outcome.ShouldQueueJackpotHandoff, Is.True);
    }

    [Test]
    public void BuildClaimOutcome_DuplicateClaim_ReturnsInformationalOutcome()
    {
        MatchClaimResolution resolution = new MatchClaimResolution
        {
            ClaimType = MatchClaimType.Bingo,
            Result = MatchClaimResolutionKind.Duplicate,
            Reason = "Claim idempotency key has already been resolved."
        };

        MultiplayerAuthorityOutcomeModel outcome = MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(resolution);

        Assert.That(outcome.FeedbackKind, Is.EqualTo(MultiplayerAuthorityFeedbackKind.Informational));
        Assert.That(outcome.Headline, Is.EqualTo("Duplicate claim ignored."));
        Assert.That(outcome.ShouldCelebrate, Is.False);
        Assert.That(outcome.ShouldQueueJackpotHandoff, Is.False);
    }

    [Test]
    public void BuildRoundEndOutcome_WithWheelspinEntitlement_QueuesHandoff()
    {
        MatchEndEvent endEvent = new MatchEndEvent
        {
            EndReason = "Max balls reached."
        };
        endEvent.WheelspinEntitledPlayerIds.Add("host_1");

        MultiplayerAuthorityOutcomeModel outcome = MultiplayerAuthorityOutcomePresenter.BuildRoundEndOutcome(endEvent);

        Assert.That(outcome.FeedbackKind, Is.EqualTo(MultiplayerAuthorityFeedbackKind.Success));
        Assert.That(outcome.Headline, Is.EqualTo("Wheelspin handoff queued."));
        Assert.That(outcome.Detail, Is.EqualTo("Max balls reached."));
        Assert.That(outcome.ShouldQueueJackpotHandoff, Is.True);
    }
}

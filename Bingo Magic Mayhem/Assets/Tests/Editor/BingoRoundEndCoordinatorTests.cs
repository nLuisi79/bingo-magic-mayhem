using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class BingoRoundEndCoordinatorTests
{
    [Test]
    public void TryBeginCountdown_BlocksWhenDecisionIsMissingInactiveOrPreviewAlreadyShown()
    {
        BingoRoundEndCoordinator coordinator = new BingoRoundEndCoordinator();

        bool missing = coordinator.TryBeginCountdown(null, rewardPreviewShown: false);
        bool inactive = coordinator.TryBeginCountdown(
            new BingoRoundEndDecision(BingoRoundEndReasonKind.None, "", ""),
            rewardPreviewShown: false);
        bool previewShown = coordinator.TryBeginCountdown(
            BingoRoundEndRules.CreateFinalBallDecision("done"),
            rewardPreviewShown: true);

        Assert.That(missing, Is.False);
        Assert.That(inactive, Is.False);
        Assert.That(previewShown, Is.False);
        Assert.That(coordinator.CountdownActive, Is.False);
    }

    [Test]
    public void TryBeginCountdown_AllowsOneActiveCountdownUntilCompletedOrCanceled()
    {
        BingoRoundEndCoordinator coordinator = new BingoRoundEndCoordinator();
        BingoRoundEndDecision decision = BingoRoundEndRules.CreateFinalBallDecision("Max balls reached.");

        bool started = coordinator.TryBeginCountdown(decision, rewardPreviewShown: false);
        bool secondStart = coordinator.TryBeginCountdown(decision, rewardPreviewShown: false);
        coordinator.CancelCountdown();
        bool restarted = coordinator.TryBeginCountdown(decision, rewardPreviewShown: false);
        coordinator.CompleteCountdown();

        Assert.That(started, Is.True);
        Assert.That(secondStart, Is.False);
        Assert.That(restarted, Is.True);
        Assert.That(coordinator.CountdownActive, Is.False);
    }
}

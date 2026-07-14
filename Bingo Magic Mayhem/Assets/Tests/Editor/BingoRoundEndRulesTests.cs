using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class BingoRoundEndRulesTests
{
    [Test]
    public void EndDecisions_ReturnExpectedKindsAndMessages()
    {
        BingoRoundEndDecision finalBall = BingoRoundEndRules.CreateFinalBallDecision("Max balls reached.");
        BingoRoundEndDecision blackout = BingoRoundEndRules.CreateBlackoutCompleteDecision();
        BingoRoundEndDecision roomPool = BingoRoundEndRules.CreateRoomPoolExhaustedDecision("Room bingo pool exhausted.");
        BingoRoundEndDecision coverage = BingoRoundEndRules.CreateStandardCoverageDecision("Coverage reached.");

        Assert.That(finalBall.Kind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
        Assert.That(finalBall.CountdownMessage, Is.EqualTo("Final ball called. Daub anything you missed."));
        Assert.That(blackout.Kind, Is.EqualTo(BingoRoundEndReasonKind.BlackoutComplete));
        Assert.That(blackout.Reason, Is.EqualTo("Every card reached blackout."));
        Assert.That(roomPool.Kind, Is.EqualTo(BingoRoundEndReasonKind.RoomPoolExhausted));
        Assert.That(roomPool.CountdownMessage, Is.EqualTo("Room bingo pool exhausted."));
        Assert.That(coverage.Kind, Is.EqualTo(BingoRoundEndReasonKind.StandardCoverage));
        Assert.That(coverage.ShouldStartCountdown, Is.True);
    }

    [Test]
    public void ResolvePostProgressDecision_PrioritizesBlackoutThenRoomPoolThenCoverage()
    {
        BingoRoundEndDecision blackout = BingoRoundEndRules.ResolvePostProgressDecision(
            blackoutRoom: true,
            allCardsReachedJackpot: true,
            roomPoolExhausted: true,
            roomPoolReason: "pool",
            standardCoverageReached: true,
            standardCoverageReason: "coverage");
        BingoRoundEndDecision roomPool = BingoRoundEndRules.ResolvePostProgressDecision(
            blackoutRoom: false,
            allCardsReachedJackpot: false,
            roomPoolExhausted: true,
            roomPoolReason: "pool",
            standardCoverageReached: true,
            standardCoverageReason: "coverage");
        BingoRoundEndDecision coverage = BingoRoundEndRules.ResolvePostProgressDecision(
            blackoutRoom: false,
            allCardsReachedJackpot: false,
            roomPoolExhausted: false,
            roomPoolReason: "pool",
            standardCoverageReached: true,
            standardCoverageReason: "coverage");
        BingoRoundEndDecision none = BingoRoundEndRules.ResolvePostProgressDecision(
            blackoutRoom: false,
            allCardsReachedJackpot: false,
            roomPoolExhausted: false,
            roomPoolReason: "pool",
            standardCoverageReached: false,
            standardCoverageReason: "coverage");

        Assert.That(blackout.Kind, Is.EqualTo(BingoRoundEndReasonKind.BlackoutComplete));
        Assert.That(roomPool.Kind, Is.EqualTo(BingoRoundEndReasonKind.RoomPoolExhausted));
        Assert.That(coverage.Kind, Is.EqualTo(BingoRoundEndReasonKind.StandardCoverage));
        Assert.That(none.Kind, Is.EqualTo(BingoRoundEndReasonKind.None));
        Assert.That(none.ShouldStartCountdown, Is.False);
    }

    [Test]
    public void StandardCoverageLimit_UsesExpectedThresholdsPerCardSelection()
    {
        Assert.That(BingoRoundEndRules.GetStandardCoverageLimit(1), Is.EqualTo(17));
        Assert.That(BingoRoundEndRules.GetStandardCoverageLimit(2), Is.EqualTo(16));
        Assert.That(BingoRoundEndRules.GetStandardCoverageLimit(4), Is.EqualTo(15));
        Assert.That(BingoRoundEndRules.GetStandardCoverageLimit(6), Is.EqualTo(14));
    }

    [Test]
    public void StandardCoverageLimitReached_IgnoresBlackoutRooms()
    {
        bool reached = BingoRoundEndRules.IsStandardCoverageLimitReached(
            blackoutRoom: true,
            playerCardCount: 4,
            totalMarkedPlayableSquares: 80,
            selectedCardCount: 4);

        Assert.That(reached, Is.False);
    }

    [Test]
    public void StandardCoverageLimitReached_UsesAverageMarkedSquaresAcrossCards()
    {
        bool notReached = BingoRoundEndRules.IsStandardCoverageLimitReached(
            blackoutRoom: false,
            playerCardCount: 4,
            totalMarkedPlayableSquares: 59,
            selectedCardCount: 4);
        bool reached = BingoRoundEndRules.IsStandardCoverageLimitReached(
            blackoutRoom: false,
            playerCardCount: 4,
            totalMarkedPlayableSquares: 60,
            selectedCardCount: 4);

        Assert.That(notReached, Is.False);
        Assert.That(reached, Is.True);
    }
}

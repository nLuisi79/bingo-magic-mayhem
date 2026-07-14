using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class PrototypeMultiplayerAuthorityBridgeTests
{
    [Test]
    public void Bridge_BeginsRound_RecordsObservedCall_AndPublishesRoundEnd()
    {
        LocalMultiplayerSessionFacade session = new LocalMultiplayerSessionFacade();
        LocalAuthoritativeMatchSimulator simulator = new LocalAuthoritativeMatchSimulator(session);
        PrototypeMultiplayerAuthorityBridge bridge = new PrototypeMultiplayerAuthorityBridge(simulator);

        bridge.BeginLocalAuthoritativeRound(
            "host_1",
            "Host",
            realmIndex: 0,
            roomIndex: 1,
            selectedCardCount: 4,
            manaBetPerCard: 25,
            roundSeed: 777,
            maxCallCount: 40,
            autoCallIntervalSeconds: 2f,
            otherParticipants: new[]
            {
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_1", DisplayName = "Guest", Ready = true }
            });

        MatchCallEvent callEvent = bridge.RecordPrototypeCall(22, 1234);
        MatchClaimResolution claim = bridge.SubmitPrototypeBingoClaim(
            "guest_1",
            cardIndex: 0,
            claimCallIndex: 0,
            markedCellKeys: new[] { "0:0", "0:1", "0:2", "0:3", "0:4" },
            claimedNumbers: new[] { 22 },
            idempotencyKey: "claim_1");
        MatchEndEvent end = bridge.PublishPrototypeRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "guest_1" });
        LocalAuthoritativeMatchSummary summary = bridge.BuildSummary();

        Assert.That(callEvent, Is.Not.Null);
        Assert.That(callEvent.CalledNumber, Is.EqualTo(22));
        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(claim.ValidatedNumberCount, Is.EqualTo(1));
        Assert.That(end.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
        Assert.That(summary.ParticipantCount, Is.EqualTo(2));
        Assert.That(summary.CallCount, Is.EqualTo(1));
        Assert.That(summary.ClaimCount, Is.EqualTo(1));
        Assert.That(summary.EndEventCount, Is.EqualTo(1));
        Assert.That(summary.MatchState, Is.EqualTo(MatchAuthorityLifecycleState.Ended));
    }
}

using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class PrototypeMultiplayerGameplayBridgeTests
{
    [Test]
    public void BeginRound_RecordClaimAndEnd_FlowThroughGameplayBridge()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");

        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent observedCall = bridge.TryRecordObservedCall(true, 22);
        MatchClaimResolution claim = bridge.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 });
        bool published = bridge.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        Assert.That(observedCall, Is.Not.Null);
        Assert.That(claim, Is.Not.Null);
        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(controller.SessionFacade.CallLog.Count, Is.EqualTo(1));
        Assert.That(controller.SessionFacade.ClaimLog.Count, Is.EqualTo(1));
        Assert.That(controller.SessionFacade.MatchEndLog.Count, Is.EqualTo(1));
        Assert.That(controller.SessionFacade.MatchEndLog[0].WheelspinEntitledPlayerIds.Count, Is.EqualTo(1));
        Assert.That(published, Is.True);
    }

    [Test]
    public void TryPublishRoundEnd_DoesNothingWhenAlreadyPublished()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        bool published = bridge.TryPublishRoundEnd(
            true,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        Assert.That(published, Is.False);
        Assert.That(controller.SessionFacade.MatchEndLog.Count, Is.EqualTo(0));
    }

    [Test]
    public void TryRecordObservedCall_WhenRoundInactive_ReturnsNullAndDoesNothing()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");

        MatchCallEvent callEvent = bridge.TryRecordObservedCall(false, 22);

        Assert.That(callEvent, Is.Null);
        Assert.That(controller.SessionFacade.CallLog.Count, Is.EqualTo(0));
    }

    [Test]
    public void TrySubmitBingoClaim_WhenRepeatedWithSameAuthorityKey_ReturnsDuplicateResolution()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        bridge.TryRecordObservedCall(true, 22);

        MatchClaimResolution firstClaim = bridge.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 });
        MatchClaimResolution duplicateClaim = bridge.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 });

        Assert.That(firstClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(duplicateClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Duplicate));
        Assert.That(duplicateClaim.Reason, Does.Contain("idempotency"));
    }

    [Test]
    public void TrySubmitJackpotStateClaim_AfterRoundEnd_ReturnsRoundClosedResolution()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        bridge.TryRecordObservedCall(true, 22);
        bridge.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: false);

        MatchClaimResolution resolution = bridge.TrySubmitJackpotStateClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            claimedNumbers: new[] { 22 });

        Assert.That(resolution, Is.Not.Null);
        Assert.That(resolution.Result, Is.EqualTo(MatchClaimResolutionKind.RoundClosed));
    }
}

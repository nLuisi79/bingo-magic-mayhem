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
        MatchCallEvent observedCall = bridge.TryEmitAuthoritativeCall(true);
        MatchClaimResolution claim = bridge.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { observedCall.CalledNumber });
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
    public void TryEmitAuthoritativeCall_WhenRoundInactive_ReturnsNullAndDoesNothing()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");

        MatchCallEvent callEvent = bridge.TryEmitAuthoritativeCall(false);

        Assert.That(callEvent, Is.Null);
        Assert.That(controller.SessionFacade.CallLog.Count, Is.EqualTo(0));
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

    [Test]
    public void TrySubmitBingoClaim_GuestPlayer_RoutesClaimThroughAuthorityBoundary()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        controller.EnsureLocalLobby(
            "host_1",
            "Host",
            0,
            0,
            2,
            10,
            new[]
            {
                new LocalAuthoritativeMatchParticipant
                {
                    PlayerId = "guest_1",
                    DisplayName = "Guest",
                    Ready = true
                }
            });
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent observedCall = bridge.TryEmitAuthoritativeCall(true);

        MatchClaimResolution claim = bridge.TrySubmitBingoClaim(
            true,
            "guest_1",
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { observedCall.CalledNumber });

        Assert.That(claim, Is.Not.Null);
        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(claim.PlayerId, Is.EqualTo("guest_1"));
        Assert.That(controller.SessionFacade.ClaimLog[0].PlayerId, Is.EqualTo("guest_1"));
    }

    [Test]
    public void TryPublishRoundEnd_WithGuestWheelspinEntitlement_PreservesExplicitRecipient()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        controller.EnsureLocalLobby(
            "host_1",
            "Host",
            0,
            0,
            2,
            10,
            new[]
            {
                new LocalAuthoritativeMatchParticipant
                {
                    PlayerId = "guest_1",
                    DisplayName = "Guest",
                    Ready = true
                }
            });
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        bool published = bridge.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "guest_1" });

        Assert.That(published, Is.True);
        Assert.That(controller.SessionFacade.MatchEndLog.Count, Is.EqualTo(1));
        Assert.That(controller.SessionFacade.MatchEndLog[0].WheelspinEntitledPlayerIds, Is.EquivalentTo(new[] { "guest_1" }));
    }

    [Test]
    public void TryResolvePostRoundHandoff_AfterWheelspinEnd_CompletesPendingState()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerGameplayBridge bridge = new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1");
        bridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        bridge.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        MatchEndEvent resolved = bridge.TryResolvePostRoundHandoff("host_1");

        Assert.That(resolved, Is.Not.Null);
        Assert.That(resolved.HandoffState, Is.EqualTo(MultiplayerPostRoundHandoffState.JackpotCompleted));
        Assert.That(resolved.ResolvedWheelspinPlayerId, Is.EqualTo("host_1"));
    }
}

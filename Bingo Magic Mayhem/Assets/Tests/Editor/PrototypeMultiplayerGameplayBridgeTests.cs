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
        bridge.TryRecordObservedCall(true, 22);
        bridge.TrySubmitBingoClaim(
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
}

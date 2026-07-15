using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class PrototypeMultiplayerPresentationFacadeTests
{
    [Test]
    public void BeginRound_RecordClaimAndEnd_FlowThroughPresentationFacade()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent observedCall = facade.TryRecordObservedCall(true, 22);
        MatchClaimResolution claim = facade.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 });
        bool published = facade.TryPublishRoundEnd(
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
    public void SyncLobbyAndBuildState_ReturnsUpdatedLobbyPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        PrototypeMultiplayerPresentationState state = facade.SyncLobbyAndBuildState("Host", 0, 1, 4, 25);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Lobby, Is.Not.Null);
        Assert.That(state.LobbyStatus, Is.Not.Null);
        Assert.That(state.Lobby.HasRoom, Is.True);
        Assert.That(state.LobbyStatus.ReadinessSummaryLabel, Is.EqualTo("1/1 ready"));
    }

    [Test]
    public void BuildLobbyDisplayModel_ReadsControllerPresentationState()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 1, 4, 25);
        MultiplayerLobbyDisplayModel model = facade.BuildLobbyDisplayModel();

        Assert.That(model.HasRoom, Is.True);
        Assert.That(model.RoomCodeLabel, Does.StartWith("Room Code "));
        Assert.That(model.ActionLabel, Is.EqualTo("Waiting for more players"));
    }

    [Test]
    public void BuildPresentationState_ReflectsRoundLifecycleAcrossLobbyCallClaimAndEnd()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 1, 4, 25);
        PrototypeMultiplayerPresentationState lobbyState = facade.BuildPresentationState();

        facade.BeginAuthoritativeRound("Host", 0, 1, 4, 25, 99, 30, 1f);
        facade.TryRecordObservedCall(true, 22);
        facade.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 });
        facade.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        PrototypeMultiplayerPresentationState endState = facade.BuildPresentationState();

        Assert.That(lobbyState, Is.Not.Null);
        Assert.That(lobbyState.Lobby, Is.Not.Null);
        Assert.That(lobbyState.LobbyStatus, Is.Not.Null);
        Assert.That(lobbyState.Session, Is.Not.Null);
        Assert.That(lobbyState.GameplayRound, Is.Not.Null);
        Assert.That(lobbyState.Lobby.HasRoom, Is.True);
        Assert.That(lobbyState.Session.RoomStateLabel, Does.Contain("Lobby"));
        Assert.That(lobbyState.LobbyStatus.DetailSummaryLabel, Does.Contain("Waiting"));

        Assert.That(endState, Is.Not.Null);
        Assert.That(endState.Session, Is.Not.Null);
        Assert.That(endState.GameplayRound, Is.Not.Null);
        Assert.That(endState.MatchSummary, Is.Not.Null);
        Assert.That(endState.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchEnded));
        Assert.That(endState.MatchSummary.MatchState, Is.EqualTo(MatchAuthorityLifecycleState.Ended));
        Assert.That(endState.MatchSummary.CallCount, Is.EqualTo(1));
        Assert.That(endState.MatchSummary.ClaimCount, Is.EqualTo(1));
        Assert.That(endState.MatchSummary.EndEventCount, Is.EqualTo(1));
        Assert.That(endState.GameplayRound.PostRoundSummaryLabel, Does.Contain("Max balls reached."));
    }

    [Test]
    public void TryPublishRoundEndAndBuildState_ReturnsPublishFlagAndEndedPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 1, 4, 25, 99, 30, 1f);
        PrototypeMultiplayerRoundEndPublishResult result = facade.TryPublishRoundEndAndBuildState(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: false);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.WasPublished, Is.True);
        Assert.That(result.PresentationState, Is.Not.Null);
        Assert.That(result.PresentationState.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchEnded));
        Assert.That(result.PresentationState.GameplayRound.PostRoundSummaryLabel, Does.Contain("no wheelspin entitlements"));
    }
}

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
        MatchCallEvent observedCall = facade.TryEmitAuthoritativeCall(true);
        MatchClaimResolution claim = facade.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { observedCall.CalledNumber });
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
    public void SyncLobbyAndBuildState_RequestOverload_ReturnsUpdatedLobbyPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        PrototypeMultiplayerPresentationState state = facade.SyncLobbyAndBuildState(
            new PrototypeMultiplayerLobbySyncRequest("Host", 0, 1, 4, 25));

        Assert.That(state, Is.Not.Null);
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
        MatchCallEvent callEvent = facade.TryEmitAuthoritativeCall(true);
        facade.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { callEvent.CalledNumber });
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
        Assert.That(result.LatestMatchEnd, Is.Not.Null);
    }

    [Test]
    public void BuildMirroredPresentationState_AfterHostRoundFlow_ReflectsGuestVisibleLifecycle()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 1, 4, 25);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        PrototypeMultiplayerPresentationState mirroredLobby = facade.BuildMirroredPresentationState();

        facade.BeginAuthoritativeRound("Host", 0, 1, 4, 25, 99, 30, 1f);
        MatchCallEvent callEvent = facade.TryEmitAuthoritativeCall(true);
        facade.TrySubmitBingoClaim(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { callEvent.CalledNumber });
        PrototypeMultiplayerPresentationState mirroredRound = facade.BuildMirroredPresentationState();

        facade.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);
        PrototypeMultiplayerPresentationState mirroredEnd = facade.BuildMirroredPresentationState();

        Assert.That(mirroredLobby, Is.Not.Null);
        Assert.That(mirroredLobby.Session.RoomStateLabel, Does.Contain("Lobby"));
        Assert.That(mirroredLobby.LobbyStatus.ReadinessSummaryLabel, Is.EqualTo("2/2 ready"));

        Assert.That(mirroredRound, Is.Not.Null);
        Assert.That(mirroredRound.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchInProgress));
        Assert.That(mirroredRound.GameplayRound.AuthoritySummaryLabel, Does.Contain("Call authority live."));
        Assert.That(mirroredRound.GameplayRound.ClaimSummaryLabel, Does.Contain("Accepted"));
        Assert.That(mirroredRound.MatchSummary, Is.Null);

        Assert.That(mirroredEnd, Is.Not.Null);
        Assert.That(mirroredEnd.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchEnded));
        Assert.That(mirroredEnd.GameplayRound.PostRoundSummaryLabel, Does.Contain("1 player is queued for jackpot wheelspin handoff"));
        Assert.That(mirroredEnd.GameplayRound.PostRoundSummaryLabel, Does.Contain("Max balls reached."));
    }

    [Test]
    public void GetCurrentAuthoritativeCallIndex_AfterTwoCalls_ReturnsLatestAuthorityIndex()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        facade.TryEmitAuthoritativeCall(true);
        facade.TryEmitAuthoritativeCall(true);

        Assert.That(facade.GetCurrentAuthoritativeCallIndex(), Is.EqualTo(1));
    }

    [Test]
    public void TrySubmitBingoClaimAndBuildResult_ReturnsResolutionAndUpdatedPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent callEvent = facade.TryEmitAuthoritativeCall(true);

        PrototypeMultiplayerClaimSubmitResult result = facade.TrySubmitBingoClaimAndBuildResult(
            true,
            cardIndex: 0,
            claimCallIndex: 0,
            newBingoCount: 1,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { callEvent.CalledNumber });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Resolution, Is.Not.Null);
        Assert.That(result.Resolution.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(result.PresentationState, Is.Not.Null);
        Assert.That(result.PresentationState.GameplayRound.ClaimSummaryLabel, Does.Contain("Accepted"));
        Assert.That(result.Outcome, Is.Not.Null);
        Assert.That(result.Outcome.Headline, Is.EqualTo("Claim accepted."));
    }

    [Test]
    public void TryEmitAuthoritativeCallAndBuildResult_ReturnsCallAndUpdatedPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        PrototypeMultiplayerCallResult result = facade.TryEmitAuthoritativeCallAndBuildResult(true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CallEvent, Is.Not.Null);
        Assert.That(result.CallEvent.CalledNumber, Is.InRange(1, 75));
        Assert.That(result.PresentationState, Is.Not.Null);
        Assert.That(result.PresentationState.Session.LatestCallLabel, Does.Contain("Latest call #1"));
        Assert.That(result.PresentationState.GameplayRound.AuthoritySummaryLabel, Does.Contain("Call authority live."));
    }

    [Test]
    public void BeginAuthoritativeRoundAndBuildState_RequestOverload_StartsRoundAndReturnsPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        PrototypeMultiplayerPresentationState state = facade.BeginAuthoritativeRoundAndBuildState(
            new PrototypeMultiplayerRoundStartRequest("Host", 0, 0, 2, 10, 99, 30, 1f));

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchInProgress));
        Assert.That(state.GameplayRound.AuthoritySummaryLabel, Does.Contain("Awaiting first number"));
    }

    [Test]
    public void TryEmitAuthoritativeCallAndBuildResult_RequestOverload_ReturnsCallAndUpdatedPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        PrototypeMultiplayerCallResult result = facade.TryEmitAuthoritativeCallAndBuildResult(
            new PrototypeMultiplayerAuthoritativeCallRequest(true));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CallEvent, Is.Not.Null);
        Assert.That(result.PresentationState.Session.LatestCallLabel, Does.Contain("Latest call #1"));
    }

    [Test]
    public void TryRecordObservedCallAndBuildState_RequestOverload_ReturnsUpdatedPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        PrototypeMultiplayerPresentationState state = facade.TryRecordObservedCallAndBuildState(
            new PrototypeMultiplayerObservedCallRequest(true, 22));

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Session.LatestCallLabel, Does.Contain("22"));
        Assert.That(state.GameplayRound.AuthoritySummaryLabel, Does.Contain("Call authority live."));
    }

    [Test]
    public void TrySubmitBingoClaimAndBuildResult_RequestOverload_ReturnsOutcomeAndPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent callEvent = facade.TryEmitAuthoritativeCall(true);

        PrototypeMultiplayerClaimSubmitResult result = facade.TrySubmitBingoClaimAndBuildResult(
            new PrototypeMultiplayerBingoClaimRequest(
                true,
                cardIndex: 0,
                claimCallIndex: 0,
                newBingoCount: 1,
                markedCellKeys: new[] { "0:0" },
                claimedNumbers: new[] { callEvent.CalledNumber }));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Resolution.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(result.Outcome, Is.Not.Null);
        Assert.That(result.Outcome.ShouldCelebrate, Is.True);
        Assert.That(result.PresentationState.GameplayRound.ClaimSummaryLabel, Does.Contain("Accepted"));
    }

    [Test]
    public void TrySubmitBingoClaimAndBuildResult_RequestOverload_SupportsGuestClaimIdentity()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 0, 2, 10);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        MatchCallEvent callEvent = facade.TryEmitAuthoritativeCall(true);

        PrototypeMultiplayerClaimSubmitResult result = facade.TrySubmitBingoClaimAndBuildResult(
            new PrototypeMultiplayerBingoClaimRequest(
                true,
                cardIndex: 0,
                claimCallIndex: 0,
                newBingoCount: 1,
                markedCellKeys: new[] { "0:0" },
                claimedNumbers: new[] { callEvent.CalledNumber },
                playerId: "guest_1"));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Resolution, Is.Not.Null);
        Assert.That(result.Resolution.PlayerId, Is.EqualTo("guest_1"));
        Assert.That(result.Resolution.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
    }

    [Test]
    public void TryPublishRoundEndAndBuildState_RequestOverload_ReturnsOutcomeAndLatestMatchEnd()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        PrototypeMultiplayerRoundEndPublishResult result = facade.TryPublishRoundEndAndBuildState(
            new PrototypeMultiplayerRoundEndRequest(
                alreadyPublished: false,
                BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
                hostEarnedWheelspin: true));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.WasPublished, Is.True);
        Assert.That(result.LatestMatchEnd, Is.Not.Null);
        Assert.That(result.Outcome, Is.Not.Null);
        Assert.That(result.Outcome.ShouldQueueJackpotHandoff, Is.True);
    }

    [Test]
    public void TryPublishRoundEndAndBuildState_RequestOverload_PreservesGuestWheelspinOwnership()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 0, 2, 10);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        PrototypeMultiplayerRoundEndPublishResult result = facade.TryPublishRoundEndAndBuildState(
            new PrototypeMultiplayerRoundEndRequest(
                alreadyPublished: false,
                BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
                hostEarnedWheelspin: false,
                wheelspinEntitledPlayerIds: new[] { "guest_1" }));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.WasPublished, Is.True);
        Assert.That(result.LatestMatchEnd, Is.Not.Null);
        Assert.That(result.LatestMatchEnd.WheelspinEntitledPlayerIds, Is.EquivalentTo(new[] { "guest_1" }));
        Assert.That(result.PresentationState.GameplayRound.PostRoundSummaryLabel, Does.Contain("1 player is queued"));
    }

    [Test]
    public void ResolvePostRoundHandoffAndBuildState_CompletesPendingJackpotPresentation()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        facade.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        PrototypeMultiplayerPostRoundHandoffResult result = facade.ResolvePostRoundHandoffAndBuildState("host_1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.EndEvent, Is.Not.Null);
        Assert.That(result.EndEvent.HandoffState, Is.EqualTo(MultiplayerPostRoundHandoffState.JackpotCompleted));
        Assert.That(result.PresentationState, Is.Not.Null);
        Assert.That(result.PresentationState.GameplayRound.PostRoundSummaryLabel, Does.Contain("completed"));
        Assert.That(result.PresentationState.UiSurface, Is.Not.Null);
        Assert.That(result.PresentationState.UiSurface.CanReturnToLobby, Is.True);
        Assert.That(result.PresentationState.UiSurface.HasPendingJackpotHandoff, Is.False);
    }

    [Test]
    public void BuildPresentationState_ExposesUiSurfaceReplayGateDuringPendingJackpotHandoff()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        facade.TryPublishRoundEnd(
            false,
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: true);

        PrototypeMultiplayerPresentationState state = facade.BuildPresentationState();

        Assert.That(state, Is.Not.Null);
        Assert.That(state.UiSurface, Is.Not.Null);
        Assert.That(state.UiSurface.HasPendingJackpotHandoff, Is.True);
        Assert.That(state.UiSurface.IsReplayBlockedByPostRoundHandoff, Is.True);
        Assert.That(state.UiSurface.CanReturnToLobby, Is.False);
        Assert.That(state.UiSurface.ReplayActionLabel, Does.Contain("Resolve jackpot handoff"));
    }

    [Test]
    public void BuildPresentationState_ExposesUiSurfaceReconnectPressureForDisconnectedGuest()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        PrototypeMultiplayerPresentationFacade facade = new PrototypeMultiplayerPresentationFacade(
            controller,
            new PrototypeMultiplayerGameplayBridge(controller, controller, "host_1"));

        facade.SyncLobby("Host", 0, 0, 2, 10);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        controller.SetParticipantConnection("guest_1", false);

        PrototypeMultiplayerPresentationState state = facade.BuildPresentationState();

        Assert.That(state, Is.Not.Null);
        Assert.That(state.UiSurface, Is.Not.Null);
        Assert.That(state.UiSurface.IsWaitingForReconnect, Is.True);
        Assert.That(state.UiSurface.DisconnectedParticipantCount, Is.EqualTo(1));
        Assert.That(state.UiSurface.ConnectionSummaryLabel, Does.Contain("1 disconnected"));
        Assert.That(state.UiSurface.ReplayActionLabel, Does.Contain("reconnect"));
    }
}

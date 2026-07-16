using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class PrototypeMultiplayerRuntimeProviderTests
{
    [Test]
    public void CreateRuntime_LocalMode_ReturnsUsableLocalRuntime()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();

        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Local, "host_1");

        Assert.That(runtime, Is.Not.Null);
        Assert.That(runtime.BackendMode, Is.EqualTo(PrototypeMultiplayerBackendMode.Local));
        Assert.That(runtime.RoomSessionService, Is.Not.Null);
        Assert.That(runtime.MatchAuthorityService, Is.Not.Null);
        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerRoomSessionController>());
        Assert.That(runtime.MatchAuthorityService, Is.SameAs(runtime.RoomSessionService));
        Assert.That(runtime.SyncAdapter, Is.InstanceOf<LocalInMemoryMultiplayerRoomSessionSyncAdapter>());
        Assert.That(runtime.PresentationFacade, Is.Not.Null);
    }

    [Test]
    public void CreateRuntime_UgsMode_ReturnsExplicitAdapterBoundary()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();

        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        Assert.That(runtime, Is.Not.Null);
        Assert.That(runtime.BackendMode, Is.EqualTo(PrototypeMultiplayerBackendMode.Ugs));
        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerUgsRuntimeAdapter>());
        Assert.That(runtime.RoomSessionService, Is.SameAs(runtime.MatchAuthorityService));
        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerUgsRuntimeAdapter>());
        Assert.That(runtime.SyncAdapter, Is.InstanceOf<PrototypeMultiplayerUgsRoomSessionSyncAdapter>());
    }

    [Test]
    public void CreateRuntime_UgsMode_UsesLocalFallbackBehaviorThroughAdapter()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();
        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        PrototypeMultiplayerPresentationState roundState =
            runtime.PresentationFacade.BeginAuthoritativeRoundAndBuildState("Host", 0, 0, 2, 10, 99, 30, 1f);
        PrototypeMultiplayerCallResult callResult =
            runtime.PresentationFacade.TryEmitAuthoritativeCallAndBuildResult(true);
        PrototypeMultiplayerPresentationState callState = callResult.PresentationState;

        Assert.That(roundState, Is.Not.Null);
        Assert.That(callResult, Is.Not.Null);
        Assert.That(callResult.CallEvent, Is.Not.Null);
        Assert.That(callState, Is.Not.Null);
        Assert.That(callState.GameplayRound.AuthoritySummaryLabel, Does.Contain("Call authority live."));
        Assert.That(runtime.RoomSessionService.CurrentRoom, Is.Not.Null);
        Assert.That(callState.Session.MatchId, Is.Not.Empty);
        Assert.That(callState.MatchSummary.CallCount, Is.EqualTo(1));
    }

    [Test]
    public void CreateRuntime_UgsMode_BuildMirroredPresentationState_ExposesGuestVisibleFallbackFlow()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();
        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        runtime.PresentationFacade.SyncLobby("Host", 0, 0, 2, 10);
        runtime.RoomSessionService.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        runtime.PresentationFacade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        PrototypeMultiplayerCallResult callResult =
            runtime.PresentationFacade.TryEmitAuthoritativeCallAndBuildResult(true);
        runtime.PresentationFacade.TryPublishRoundEnd(
            false,
            BingoMagicMayhem.Rounds.BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            hostEarnedWheelspin: false);

        PrototypeMultiplayerPresentationState mirroredState = runtime.PresentationFacade.BuildMirroredPresentationState();

        Assert.That(callResult, Is.Not.Null);
        Assert.That(callResult.CallEvent, Is.Not.Null);
        Assert.That(mirroredState, Is.Not.Null);
        Assert.That(mirroredState.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchEnded));
        Assert.That(mirroredState.GameplayRound.PostRoundSummaryLabel, Does.Contain("no wheelspin entitlements"));
        Assert.That(runtime.SyncAdapter.Mirror.MirroredMatchEnds.Count, Is.EqualTo(1));
    }
}

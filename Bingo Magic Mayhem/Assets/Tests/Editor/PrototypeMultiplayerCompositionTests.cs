using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class PrototypeMultiplayerCompositionTests
{
    [Test]
    public void CreateLocalControllerDependencies_ReusesProvidedFacadeAndSyncAdapter()
    {
        LocalMultiplayerSessionFacade sessionFacade = new LocalMultiplayerSessionFacade();
        LocalInMemoryMultiplayerRoomSessionSyncAdapter syncAdapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();

        PrototypeMultiplayerControllerDependencies dependencies =
            PrototypeMultiplayerComposition.CreateLocalControllerDependencies(sessionFacade, syncAdapter);

        Assert.That(dependencies.SessionFacade, Is.SameAs(sessionFacade));
        Assert.That(dependencies.SyncAdapter, Is.SameAs(syncAdapter));
        Assert.That(dependencies.MatchSimulator, Is.Not.Null);
        Assert.That(dependencies.AuthorityBridge, Is.Not.Null);
    }

    [Test]
    public void CreateLocalRuntime_WiresSharedGameplayAndControllerSeams()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");

        PrototypeMultiplayerPresentationState state =
            runtime.PresentationFacade.BeginAuthoritativeRoundAndBuildState("Host", 0, 0, 2, 10, 99, 30, 1f);

        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerRoomSessionController>());
        Assert.That(runtime.MatchAuthorityService, Is.SameAs(runtime.RoomSessionService));
        Assert.That(runtime.SyncAdapter, Is.InstanceOf<LocalInMemoryMultiplayerRoomSessionSyncAdapter>());
        Assert.That(runtime.PresentationFacade, Is.Not.Null);
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Session, Is.Not.Null);
        Assert.That(runtime.RoomSessionService.CurrentRoom, Is.Not.Null);
        Assert.That(runtime.RoomSessionService.CurrentRoom.HostPlayerId, Is.EqualTo("host_1"));
        Assert.That(state.Session.MatchId, Is.Not.Empty);
    }

    [Test]
    public void CreateUgsStubRuntime_WiresExplicitSyncAdapterBoundaryIntoFallbackController()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateUgsStubRuntime("host_1");

        PrototypeMultiplayerPresentationState state =
            runtime.PresentationFacade.BeginAuthoritativeRoundAndBuildState("Host", 0, 0, 2, 10, 99, 30, 1f);

        Assert.That(runtime.BackendMode, Is.EqualTo(PrototypeMultiplayerBackendMode.Ugs));
        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerUgsRuntimeAdapter>());
        Assert.That(runtime.MatchAuthorityService, Is.SameAs(runtime.RoomSessionService));
        Assert.That(runtime.SyncAdapter, Is.InstanceOf<PrototypeMultiplayerUgsRoomSessionSyncAdapter>());
        Assert.That(runtime.SyncAdapter.LatestMatchStart, Is.Not.Null);
        Assert.That(runtime.PresentationFacade, Is.Not.Null);
        Assert.That(state, Is.Not.Null);
        Assert.That(state.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchInProgress));
    }

    [Test]
    public void CreateLocalRuntime_BuildMirroredPresentationState_UsesSyncAdapterMirrorForGuestView()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        runtime.RoomSessionService.AddOrUpdateLocalParticipant("guest_1", "Guest", true);

        runtime.PresentationFacade.SyncLobby("Host", 0, 0, 2, 10);
        runtime.PresentationFacade.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        runtime.PresentationFacade.TryRecordObservedCall(true, 22);

        PrototypeMultiplayerPresentationState mirroredState = runtime.PresentationFacade.BuildMirroredPresentationState();

        Assert.That(mirroredState, Is.Not.Null);
        Assert.That(mirroredState.Session, Is.Not.Null);
        Assert.That(mirroredState.Session.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchInProgress));
        Assert.That(mirroredState.Session.LatestCallLabel, Is.EqualTo("Latest call #1: 22"));
        Assert.That(mirroredState.Snapshot.CallLog.Count, Is.EqualTo(1));
        Assert.That(mirroredState.MatchSummary, Is.Null);
    }
}

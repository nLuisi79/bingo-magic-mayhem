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

        runtime.GameplayBridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        Assert.That(runtime.RoomSessionService, Is.SameAs(runtime.Controller));
        Assert.That(runtime.MatchAuthorityService, Is.SameAs(runtime.Controller));
        Assert.That(runtime.Controller, Is.Not.Null);
        Assert.That(runtime.GameplayBridge, Is.Not.Null);
        Assert.That(runtime.Controller.SessionFacade.CurrentRoom, Is.Not.Null);
        Assert.That(runtime.Controller.SessionFacade.CurrentMatch, Is.Not.Null);
        Assert.That(runtime.Controller.SessionFacade.CurrentRoom.HostPlayerId, Is.EqualTo("host_1"));
    }

    [Test]
    public void CreateUgsStubRuntime_WiresExplicitSyncAdapterBoundaryIntoFallbackController()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateUgsStubRuntime("host_1");

        runtime.GameplayBridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);

        Assert.That(runtime.BackendMode, Is.EqualTo(PrototypeMultiplayerBackendMode.Ugs));
        Assert.That(runtime.Controller.SyncAdapter, Is.InstanceOf<PrototypeMultiplayerUgsRoomSessionSyncAdapter>());
        Assert.That(runtime.Controller.SyncAdapter.LatestMatchStart, Is.Not.Null);
    }
}

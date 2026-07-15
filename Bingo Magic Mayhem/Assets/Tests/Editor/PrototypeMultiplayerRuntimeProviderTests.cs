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
        PrototypeMultiplayerPresentationState callState =
            runtime.PresentationFacade.TryRecordObservedCallAndBuildState(true, 22);

        Assert.That(roundState, Is.Not.Null);
        Assert.That(callState, Is.Not.Null);
        Assert.That(callState.GameplayRound.AuthoritySummaryLabel, Does.Contain("Call authority live."));
        Assert.That(runtime.RoomSessionService.CurrentRoom, Is.Not.Null);
        Assert.That(callState.Session.MatchId, Is.Not.Empty);
        Assert.That(callState.MatchSummary.CallCount, Is.EqualTo(1));
    }
}

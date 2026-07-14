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
        Assert.That(runtime.Controller, Is.Not.Null);
        Assert.That(runtime.GameplayBridge, Is.Not.Null);
    }

    [Test]
    public void CreateRuntime_UgsMode_ReturnsExplicitAdapterBoundary()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();

        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        Assert.That(runtime, Is.Not.Null);
        Assert.That(runtime.BackendMode, Is.EqualTo(PrototypeMultiplayerBackendMode.Ugs));
        Assert.That(runtime.Controller, Is.Not.Null);
        Assert.That(runtime.RoomSessionService, Is.Not.SameAs(runtime.Controller));
        Assert.That(runtime.MatchAuthorityService, Is.Not.SameAs(runtime.Controller));
        Assert.That(runtime.RoomSessionService, Is.SameAs(runtime.MatchAuthorityService));
        Assert.That(runtime.RoomSessionService, Is.InstanceOf<PrototypeMultiplayerUgsRuntimeAdapter>());
    }

    [Test]
    public void CreateRuntime_UgsMode_UsesLocalFallbackBehaviorThroughAdapter()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();
        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        runtime.GameplayBridge.BeginAuthoritativeRound("Host", 0, 0, 2, 10, 99, 30, 1f);
        runtime.GameplayBridge.TryRecordObservedCall(true, 22);

        Assert.That(runtime.Controller.SessionFacade.CurrentRoom, Is.Not.Null);
        Assert.That(runtime.Controller.SessionFacade.CurrentMatch, Is.Not.Null);
        Assert.That(runtime.Controller.SessionFacade.CallLog.Count, Is.EqualTo(1));
    }
}

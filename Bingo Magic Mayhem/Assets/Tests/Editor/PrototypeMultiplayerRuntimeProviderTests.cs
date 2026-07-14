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
        Assert.That(runtime.RoomSessionService, Is.Not.Null);
        Assert.That(runtime.MatchAuthorityService, Is.Not.Null);
        Assert.That(runtime.Controller, Is.Not.Null);
        Assert.That(runtime.GameplayBridge, Is.Not.Null);
    }

    [Test]
    public void CreateRuntime_UgsMode_CurrentlyFallsBackToLocalRuntimeSeam()
    {
        IPrototypeMultiplayerRuntimeProvider provider = new PrototypeMultiplayerRuntimeProvider();

        PrototypeMultiplayerRuntime runtime = provider.CreateRuntime(PrototypeMultiplayerBackendMode.Ugs, "host_1");

        Assert.That(runtime, Is.Not.Null);
        Assert.That(runtime.Controller, Is.Not.Null);
        Assert.That(runtime.RoomSessionService, Is.SameAs(runtime.Controller));
        Assert.That(runtime.MatchAuthorityService, Is.SameAs(runtime.Controller));
    }
}

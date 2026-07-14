using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerRoomSessionServiceContractTests
{
    [Test]
    public void LocalRuntime_ExposesRoomSessionServiceContract()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        IMultiplayerRoomSessionService service = runtime.RoomSessionService;

        MultiplayerRoomSnapshot room = service.EnsureHostReady("host_1", "Host", 1, 2, 4, 25);
        service.AddOrUpdateLocalParticipant("guest_1", "Guest", true);

        Assert.That(service, Is.Not.Null);
        Assert.That(room, Is.Not.Null);
        Assert.That(service.CurrentRoom, Is.Not.Null);
        Assert.That(service.CurrentRoom.Participants.Count, Is.EqualTo(2));
        Assert.That(service.CurrentRoom.HostPlayerId, Is.EqualTo("host_1"));
    }
}

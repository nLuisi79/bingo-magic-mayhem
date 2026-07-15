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

    [Test]
    public void LocalRuntime_RoomSessionContract_ExposesConnectionAndReplayLifecycle()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        IMultiplayerRoomSessionService service = runtime.RoomSessionService;
        IMultiplayerMatchAuthorityService authorityService = runtime.MatchAuthorityService;

        service.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        service.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        service.SetParticipantConnection("guest_1", false);

        Assert.That(service.CurrentRoom.Participants[1].IsConnected, Is.False);

        service.SetParticipantConnection("guest_1", true);
        service.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, 99, 30, 1f);
        authorityService.PublishRoundEnd(
            BingoMagicMayhem.Rounds.BingoRoundEndRules.CreateFinalBallDecision("Replay ready."),
            new[] { "host_1" });

        MultiplayerRoomSnapshot room = service.ReturnCurrentRoomToLobby();

        Assert.That(room.State, Is.EqualTo(MultiplayerRoomLifecycleState.Lobby));
        Assert.That(service.CurrentRoom.Participants[0].IsReady, Is.True);
        Assert.That(service.CurrentRoom.Participants[1].IsReady, Is.False);
    }
}

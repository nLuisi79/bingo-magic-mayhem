using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerLobbyPresenterTests
{
    [Test]
    public void Build_WithoutRoom_ReturnsCreateRoomState()
    {
        MultiplayerLobbyDisplayModel model = MultiplayerLobbyPresenter.Build(null);

        Assert.That(model.HasRoom, Is.False);
        Assert.That(model.ActionLabel, Is.EqualTo("Create local room"));
        Assert.That(model.CanStart, Is.False);
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.NoRoom));
    }

    [Test]
    public void Build_WithReadyLobby_ReturnsStartMatchState()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        controller.EnsureLocalLobby(
            "host_1",
            "Host",
            realmIndex: 0,
            roomIndex: 1,
            selectedCardCount: 4,
            manaBetPerCard: 25,
            localParticipants: new[]
            {
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_1", DisplayName = "Guest", Ready = true }
            });

        MultiplayerLobbyDisplayModel model = controller.BuildLobbyDisplayModel();

        Assert.That(model.HasRoom, Is.True);
        Assert.That(model.RoomCodeLabel, Does.StartWith("Room Code "));
        Assert.That(model.ReadinessLabel, Is.EqualTo("2/2 ready"));
        Assert.That(model.ActionLabel, Is.EqualTo("Start local match"));
        Assert.That(model.CanStart, Is.True);
        Assert.That(model.DetailLabel, Does.Contain("Host may start"));
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.ReadyToStart));
        Assert.That(model.ParticipantSummary, Does.Contain("Host (Host)"));
    }

    [Test]
    public void Build_WithOnlyHost_WaitsForMorePlayers()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        controller.EnsureHostReady("host_1", "Host", 0, 1, 4, 25);

        MultiplayerLobbyDisplayModel model = controller.BuildLobbyDisplayModel();

        Assert.That(model.ActionLabel, Is.EqualTo("Waiting for more players"));
        Assert.That(model.DetailLabel, Does.Contain("at least one more player"));
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.WaitingForPlayers));
        Assert.That(model.CanStart, Is.False);
    }
}

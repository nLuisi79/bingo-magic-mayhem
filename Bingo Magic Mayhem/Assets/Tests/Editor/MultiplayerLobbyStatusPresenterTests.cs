using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerLobbyStatusPresenterTests
{
    [Test]
    public void Build_WithoutLobbyModel_ReturnsSafeFallbackLabels()
    {
        MultiplayerLobbyStatusDisplayModel model = MultiplayerLobbyStatusPresenter.Build(null);

        Assert.That(model.TitleSummaryLabel, Is.EqualTo("Local Multiplayer  |  No room"));
        Assert.That(model.ReadinessSummaryLabel, Is.EqualTo("0/0 ready"));
        Assert.That(model.DetailSummaryLabel, Does.Contain("Create local room"));
        Assert.That(model.CanStart, Is.False);
    }

    [Test]
    public void Build_WithLobbyModel_SummarizesLobbyStrings()
    {
        MultiplayerLobbyDisplayModel lobbyModel = new MultiplayerLobbyDisplayModel(
            title: "Local Multiplayer",
            roomCodeLabel: "Room Code ABC123",
            readinessLabel: "2/2 ready",
            stateLabel: "Ready to start",
            participantSummary: "Host (Host) - Ready  |  Guest - Ready",
            detailLabel: "Host may start when everyone is ready.",
            actionLabel: "Start local match",
            gameplayFlowState: MultiplayerGameplayFlowState.ReadyToStart,
            canStart: true,
            hasRoom: true);

        MultiplayerLobbyStatusDisplayModel model = MultiplayerLobbyStatusPresenter.Build(lobbyModel);

        Assert.That(model.TitleSummaryLabel, Is.EqualTo("Local Multiplayer  |  Ready to start"));
        Assert.That(model.ReadinessSummaryLabel, Is.EqualTo("2/2 ready"));
        Assert.That(model.DetailSummaryLabel, Is.EqualTo("Start local match — Host may start when everyone is ready."));
        Assert.That(model.ActionLabel, Is.EqualTo("Start local match"));
        Assert.That(model.DetailLabel, Is.EqualTo("Host may start when everyone is ready."));
        Assert.That(model.ParticipantSummaryLabel, Does.Contain("Host (Host)"));
        Assert.That(model.CanStart, Is.True);
    }
}

using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class MultiplayerGameplayRoundPresenterTests
{
    [Test]
    public void Build_WithoutSession_ReturnsSafeFallbackLabels()
    {
        MultiplayerGameplayRoundDisplayModel model = MultiplayerGameplayRoundPresenter.Build(null);

        Assert.That(model.RoomSummaryLabel, Is.EqualTo("No multiplayer room."));
        Assert.That(model.AuthoritySummaryLabel, Is.EqualTo("Authority unavailable."));
        Assert.That(model.ClaimSummaryLabel, Is.EqualTo("No claim state."));
        Assert.That(model.PostRoundSummaryLabel, Is.EqualTo("No post-round state."));
    }

    [Test]
    public void Build_RoundInProgress_SummarizesAuthorityAndClaimState()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 2, 10);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);
        facade.SetParticipantReady("guest_1", true);
        MatchAuthorityState match = facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 0,
            SelectedCardCount = 2,
            ManaBetPerCard = 10,
            RoundSeed = 42,
            MaxCallCount = 30,
            AutoCallIntervalSeconds = 1f
        });
        facade.RegisterObservedCall(22, 1000);
        facade.ResolveClaim(new MatchClaimAttempt
        {
            MatchId = match.MatchId,
            PlayerId = "guest_1",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "claim_1",
            ClaimedNumbers = { 22 }
        });

        MultiplayerRoomSessionDisplayModel sessionModel = MultiplayerRoomSessionPresenter.Build(facade);
        MultiplayerGameplayRoundDisplayModel model = MultiplayerGameplayRoundPresenter.Build(sessionModel);

        Assert.That(model.RoomSummaryLabel, Is.EqualTo("Match in progress  |  2/2 ready"));
        Assert.That(model.AuthoritySummaryLabel, Does.Contain("Authority live."));
        Assert.That(model.AuthoritySummaryLabel, Does.Contain("Call authority live."));
        Assert.That(model.ClaimSummaryLabel, Does.Contain("Last claim Accepted"));
        Assert.That(model.PostRoundSummaryLabel, Does.Contain("No jackpot handoff pending."));
    }

    [Test]
    public void Build_RoundEnded_SummarizesPostRoundHandoff()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 2, 10);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);
        facade.SetParticipantReady("guest_1", true);
        facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 0,
            SelectedCardCount = 2,
            ManaBetPerCard = 10,
            RoundSeed = 42,
            MaxCallCount = 30,
            AutoCallIntervalSeconds = 1f
        });
        facade.PublishMatchEnd(BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."), new[] { "guest_1" });

        MultiplayerRoomSessionDisplayModel sessionModel = MultiplayerRoomSessionPresenter.Build(facade);
        MultiplayerGameplayRoundDisplayModel model = MultiplayerGameplayRoundPresenter.Build(sessionModel);

        Assert.That(model.RoomSummaryLabel, Is.EqualTo("Room closed  |  2/2 ready"));
        Assert.That(model.PostRoundSummaryLabel, Does.Contain("queued for jackpot wheelspin handoff"));
        Assert.That(model.PostRoundSummaryLabel, Does.Contain("Ended: Max balls reached."));
    }
}

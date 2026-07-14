using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class MultiplayerRoomSessionPresenterTests
{
    [Test]
    public void Build_WithoutRoom_ReturnsEmptySafeModel()
    {
        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(null);

        Assert.That(model.RoomStateLabel, Is.EqualTo("No room"));
        Assert.That(model.MatchStateLabel, Is.EqualTo("No match"));
        Assert.That(model.CanStartMatchLocally, Is.False);
        Assert.That(model.AuthorityStatusLabel, Is.EqualTo("No authority session."));
        Assert.That(model.HostStartHint, Is.EqualTo("Create a room to begin multiplayer."));
        Assert.That(model.CallAuthorityLabel, Is.EqualTo("Call authority offline."));
        Assert.That(model.LatestCallLabel, Is.EqualTo("No calls yet."));
        Assert.That(model.ClaimPresentationState, Is.EqualTo(MultiplayerClaimPresentationState.None));
        Assert.That(model.ClaimStatusLabel, Is.EqualTo("No claims resolved."));
        Assert.That(model.ClaimResolutionReasonLabel, Is.EqualTo("No claim resolution reason."));
        Assert.That(model.PostRoundSequenceState, Is.EqualTo(MultiplayerPostRoundSequenceState.None));
        Assert.That(model.JackpotHandoffLabel, Is.EqualTo("No jackpot handoff pending."));
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.NoRoom));
        Assert.That(model.Participants.Count, Is.EqualTo(0));
    }

    [Test]
    public void Build_LobbyRoom_SummarizesReadinessAndAllowsStartWhenEveryoneReady()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 1, 2, 4, 25);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);
        facade.SetParticipantReady("guest_1", true);

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.RoomStateLabel, Is.EqualTo("Lobby open"));
        Assert.That(model.MatchStateLabel, Is.EqualTo("No match"));
        Assert.That(model.HostDisplayName, Is.EqualTo("Host"));
        Assert.That(model.ReadinessSummary, Is.EqualTo("2/2 ready"));
        Assert.That(model.CanStartMatchLocally, Is.True);
        Assert.That(model.AuthorityStatusLabel, Is.EqualTo("Authority armed. Host can start the round."));
        Assert.That(model.HostStartHint, Is.EqualTo("All connected players are ready. Host may start."));
        Assert.That(model.CallAuthorityLabel, Is.EqualTo("Call authority offline."));
        Assert.That(model.ClaimPresentationState, Is.EqualTo(MultiplayerClaimPresentationState.None));
        Assert.That(model.ClaimStatusLabel, Is.EqualTo("No claims resolved."));
        Assert.That(model.PostRoundSequenceState, Is.EqualTo(MultiplayerPostRoundSequenceState.None));
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.ReadyToStart));
        Assert.That(model.PendingReadyParticipantCount, Is.EqualTo(0));
        Assert.That(model.Participants.Count, Is.EqualTo(2));
        Assert.That(model.Participants[0].PresenceLabel, Is.EqualTo("Ready"));
    }

    [Test]
    public void Build_LobbyRoom_WithUnreadyGuest_ShowsStartBlocker()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 1, 2, 4, 25);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);
        facade.SetParticipantReady("guest_1", false);

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.CanStartMatchLocally, Is.False);
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.WaitingForReadyPlayers));
        Assert.That(model.PendingReadyParticipantCount, Is.EqualTo(1));
        Assert.That(model.AuthorityStatusLabel, Does.Contain("Waiting for 1 player to ready"));
        Assert.That(model.HostStartHint, Is.EqualTo("One connected player still needs to ready up."));
    }

    [Test]
    public void Build_AfterAuthorityActivity_SummarizesMatchAndLastEvent()
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
        facade.PublishMatchEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "guest_1" });

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.RoomStateLabel, Is.EqualTo("Room closed"));
        Assert.That(model.MatchStateLabel, Is.EqualTo("Round ended"));
        Assert.That(model.HasActiveMatch, Is.False);
        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchEnded));
        Assert.That(model.AuthorityStatusLabel, Is.EqualTo("Authority complete. Round end has been recorded."));
        Assert.That(model.CallAuthorityLabel, Is.EqualTo("Call authority stopped. Round has ended."));
        Assert.That(model.LatestCallLabel, Is.EqualTo("Latest call #1: 22"));
        Assert.That(model.ClaimPresentationState, Is.EqualTo(MultiplayerClaimPresentationState.Accepted));
        Assert.That(model.ClaimStatusLabel, Is.EqualTo("Last claim Accepted: guest_1 at call 1"));
        Assert.That(model.ClaimResolutionReasonLabel, Is.EqualTo("Accepted by local authority."));
        Assert.That(model.PostRoundSequenceState, Is.EqualTo(MultiplayerPostRoundSequenceState.JackpotPending));
        Assert.That(model.JackpotHandoffLabel, Is.EqualTo("Round ended. 1 player is queued for jackpot wheelspin handoff."));
        Assert.That(model.WheelspinEntitledPlayerCount, Is.EqualTo(1));
        Assert.That(model.ActivitySummary, Is.EqualTo("Calls 1  |  Claims 1  |  Ends 1"));
        Assert.That(model.LastEventSummary, Is.EqualTo("Ended: Max balls reached."));
    }

    [Test]
    public void Build_MatchInProgress_ExposesCallAuthorityAndClaimState()
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

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.GameplayFlowState, Is.EqualTo(MultiplayerGameplayFlowState.MatchInProgress));
        Assert.That(model.CallAuthorityLabel, Is.EqualTo("Call authority live. Latest authoritative call index 0."));
        Assert.That(model.LatestCallLabel, Is.EqualTo("Latest call #1: 22"));
        Assert.That(model.ClaimPresentationState, Is.EqualTo(MultiplayerClaimPresentationState.Accepted));
        Assert.That(model.ClaimStatusLabel, Is.EqualTo("Last claim Accepted: guest_1 at call 1"));
        Assert.That(model.ClaimResolutionReasonLabel, Is.EqualTo("Accepted by local authority."));
        Assert.That(model.PostRoundSequenceState, Is.EqualTo(MultiplayerPostRoundSequenceState.RoundActive));
        Assert.That(model.JackpotHandoffLabel, Is.EqualTo("No jackpot handoff pending."));
        Assert.That(model.WheelspinEntitledPlayerCount, Is.EqualTo(0));
    }

    [Test]
    public void Build_RejectedClaim_ExposesRejectedClaimPresentationState()
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
            IdempotencyKey = "claim_rejected"
        });

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.ClaimPresentationState, Is.EqualTo(MultiplayerClaimPresentationState.Rejected));
        Assert.That(model.ClaimStatusLabel, Is.EqualTo("Last claim Rejected: guest_1 at call 0"));
        Assert.That(model.ClaimResolutionReasonLabel, Does.Contain("did not include any numbers"));
    }

    [Test]
    public void Build_RoundEndedWithoutWheelspin_ExposesJackpotNotEligibleState()
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
        facade.PublishMatchEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new string[0]);

        MultiplayerRoomSessionDisplayModel model = MultiplayerRoomSessionPresenter.Build(facade);

        Assert.That(model.PostRoundSequenceState, Is.EqualTo(MultiplayerPostRoundSequenceState.JackpotNotEligible));
        Assert.That(model.JackpotHandoffLabel, Is.EqualTo("Round ended with no wheelspin entitlements queued."));
        Assert.That(model.WheelspinEntitledPlayerCount, Is.EqualTo(0));
    }
}

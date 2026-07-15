using System;
using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class LocalMultiplayerSessionFacadeTests
{
    [Test]
    public void CreateRoom_StartMatch_AndEmitCall_ProduceAuthoritativeLocalState()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 1, 4, 25);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);
        facade.SetParticipantReady("guest_1", true);

        MatchAuthorityState match = facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 1,
            SelectedCardCount = 4,
            ManaBetPerCard = 25,
            RoundSeed = 12345,
            MaxCallCount = 45,
            AutoCallIntervalSeconds = 2f
        });
        MatchCallEvent callEvent = facade.EmitNextCall();

        Assert.That(facade.CurrentRoom.State, Is.EqualTo(MultiplayerRoomLifecycleState.MatchInProgress));
        Assert.That(match.State, Is.EqualTo(MatchAuthorityLifecycleState.InRound));
        Assert.That(callEvent, Is.Not.Null);
        Assert.That(callEvent.CalledNumber, Is.InRange(1, 75));
        Assert.That(callEvent.CallIndex, Is.EqualTo(0));
        Assert.That(facade.CallLog.Count, Is.EqualTo(1));
        Assert.That(facade.CurrentMatch.CurrentCallIndex, Is.EqualTo(0));
    }

    [Test]
    public void ResolveClaim_RejectsMissingNumbers_AndAcceptsCalledNumbers()
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
            RoundSeed = 77,
            MaxCallCount = 20,
            AutoCallIntervalSeconds = 1.5f
        });

        facade.EmitNextCall();

        MatchClaimResolution missingNumbersClaim = facade.ResolveClaim(new MatchClaimAttempt
        {
            MatchId = match.MatchId,
            PlayerId = "guest_1",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "missing_numbers"
        });

        int calledNumber = facade.CallLog[0].CalledNumber;
        MatchClaimResolution acceptedClaim = facade.ResolveClaim(new MatchClaimAttempt
        {
            MatchId = match.MatchId,
            PlayerId = "guest_1",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "accepted",
            ClaimedNumbers = { calledNumber }
        });

        Assert.That(missingNumbersClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Rejected));
        Assert.That(missingNumbersClaim.Reason, Does.Contain("did not include any numbers"));
        Assert.That(acceptedClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(acceptedClaim.AcceptedCallIndex, Is.EqualTo(0));
        Assert.That(acceptedClaim.ValidatedNumberCount, Is.EqualTo(1));
    }

    [Test]
    public void ResolveClaim_RejectsUncalledNumbers_AndUnknownPlayers()
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
            RoundSeed = 78,
            MaxCallCount = 20,
            AutoCallIntervalSeconds = 1.5f
        });

        facade.RegisterObservedCall(22, 1000);

        MatchClaimResolution unknownPlayerClaim = facade.ResolveClaim(new MatchClaimAttempt
        {
            MatchId = match.MatchId,
            PlayerId = "ghost_player",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "ghost",
            ClaimedNumbers = { 22 }
        });

        MatchClaimResolution uncalledNumberClaim = facade.ResolveClaim(new MatchClaimAttempt
        {
            MatchId = match.MatchId,
            PlayerId = "guest_1",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "uncalled",
            ClaimedNumbers = { 22, 44 }
        });

        Assert.That(unknownPlayerClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Rejected));
        Assert.That(unknownPlayerClaim.Reason, Does.Contain("not present"));
        Assert.That(uncalledNumberClaim.Result, Is.EqualTo(MatchClaimResolutionKind.Rejected));
        Assert.That(uncalledNumberClaim.Reason, Does.Contain("uncalled number 44"));
    }

    [Test]
    public void PublishMatchEnd_ClosesMatchAndCarriesWheelspinEntitlements()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 1, 5);
        facade.SetParticipantReady("host_1", true);
        MatchAuthorityState match = facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 0,
            SelectedCardCount = 1,
            ManaBetPerCard = 5,
            RoundSeed = 9,
            MaxCallCount = 10,
            AutoCallIntervalSeconds = 1f
        });

        MatchEndEvent endEvent = facade.PublishMatchEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "host_1" });

        Assert.That(endEvent.MatchId, Is.EqualTo(match.MatchId));
        Assert.That(endEvent.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
        Assert.That(endEvent.WheelspinEntitledPlayerIds.Count, Is.EqualTo(1));
        Assert.That(facade.CurrentMatch.State, Is.EqualTo(MatchAuthorityLifecycleState.Ended));
        Assert.That(facade.CurrentRoom.State, Is.EqualTo(MultiplayerRoomLifecycleState.Closed));
    }

    [Test]
    public void RegisterObservedCall_TracksExplicitHostCallsAndRejectsDuplicates()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 1, 5);
        facade.SetParticipantReady("host_1", true);
        facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 0,
            SelectedCardCount = 1,
            ManaBetPerCard = 5,
            RoundSeed = 11,
            MaxCallCount = 10,
            AutoCallIntervalSeconds = 1f
        });

        MatchCallEvent first = facade.RegisterObservedCall(12, 1000);
        MatchCallEvent duplicate = facade.RegisterObservedCall(12, 2000);

        Assert.That(first, Is.Not.Null);
        Assert.That(first.CalledNumber, Is.EqualTo(12));
        Assert.That(first.CallIndex, Is.EqualTo(0));
        Assert.That(first.EmittedUtcTicks, Is.EqualTo(1000));
        Assert.That(duplicate, Is.Null);
        Assert.That(facade.CallLog.Count, Is.EqualTo(1));
    }

    [Test]
    public void AddLocalParticipant_ExistingPlayerReconnectsAndUpdatesDisplayName()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 1, 5);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantConnection("guest_1", false);

        MultiplayerRoomSnapshot room = facade.AddLocalParticipant("guest_1", "Guest Rejoined");

        Assert.That(room.Participants.Count, Is.EqualTo(2));
        Assert.That(room.Participants[1].DisplayName, Is.EqualTo("Guest Rejoined"));
        Assert.That(room.Participants[1].IsConnected, Is.True);
    }

    [Test]
    public void ReturnCurrentRoomToLobby_AfterRoundEnd_ReopensRoomAndClearsAuthorityLogs()
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
            RoundSeed = 17,
            MaxCallCount = 20,
            AutoCallIntervalSeconds = 1f
        });
        facade.RegisterObservedCall(22, 1000);
        facade.PublishMatchEnd(
            BingoMagicMayhem.Rounds.BingoRoundEndRules.CreateFinalBallDecision("Replay ready."),
            new[] { "host_1" });

        MultiplayerRoomSnapshot room = facade.ReturnCurrentRoomToLobby();

        Assert.That(room.State, Is.EqualTo(MultiplayerRoomLifecycleState.Lobby));
        Assert.That(facade.CurrentMatch, Is.Null);
        Assert.That(facade.CallLog.Count, Is.EqualTo(0));
        Assert.That(facade.ClaimLog.Count, Is.EqualTo(0));
        Assert.That(facade.MatchEndLog.Count, Is.EqualTo(0));
        Assert.That(room.Participants[0].IsReady, Is.True);
        Assert.That(room.Participants[1].IsReady, Is.False);
    }

    [Test]
    public void StartAuthoritativeMatch_WithUnreadyConnectedParticipant_Throws()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 2, 10);
        facade.AddLocalParticipant("guest_1", "Guest");
        facade.SetParticipantReady("host_1", true);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
            {
                RealmIndex = 0,
                RoomIndex = 0,
                SelectedCardCount = 2,
                ManaBetPerCard = 10,
                RoundSeed = 77,
                MaxCallCount = 20,
                AutoCallIntervalSeconds = 1f
            }));

        Assert.That(exception.Message, Does.Contain("must be ready"));
    }

    [Test]
    public void PublishMatchEnd_WhenAlreadyEnded_ReturnsExistingEndEventWithoutDuplication()
    {
        LocalMultiplayerSessionFacade facade = new LocalMultiplayerSessionFacade();
        facade.CreateRoom("host_1", "Host", 0, 0, 1, 5);
        facade.SetParticipantReady("host_1", true);
        facade.StartAuthoritativeMatch(new AuthoritativeMatchStartRequest
        {
            RealmIndex = 0,
            RoomIndex = 0,
            SelectedCardCount = 1,
            ManaBetPerCard = 5,
            RoundSeed = 9,
            MaxCallCount = 10,
            AutoCallIntervalSeconds = 1f
        });

        MatchEndEvent first = facade.PublishMatchEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "host_1" });
        MatchEndEvent second = facade.PublishMatchEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Different duplicate reason."),
            new[] { "host_1" });

        Assert.That(second, Is.SameAs(first));
        Assert.That(facade.MatchEndLog.Count, Is.EqualTo(1));
    }
}

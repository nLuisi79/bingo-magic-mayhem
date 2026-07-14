using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class LocalAuthoritativeMatchSimulatorTests
{
    [Test]
    public void CreateRoomAndStartMatch_ReadiesParticipantsAndStartsAuthorityState()
    {
        LocalMultiplayerSessionFacade session = new LocalMultiplayerSessionFacade();
        LocalAuthoritativeMatchSimulator simulator = new LocalAuthoritativeMatchSimulator(session);

        MultiplayerRoomSnapshot room = simulator.CreateRoomAndStartMatch(
            "host_1",
            "Host",
            new[]
            {
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_1", DisplayName = "Guest 1", Ready = true },
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_2", DisplayName = "Guest 2", Ready = false }
            },
            new AuthoritativeMatchStartRequest
            {
                RealmIndex = 1,
                RoomIndex = 2,
                SelectedCardCount = 4,
                ManaBetPerCard = 25,
                RoundSeed = 9001,
                MaxCallCount = 40,
                AutoCallIntervalSeconds = 2f
            });

        Assert.That(room.Participants.Count, Is.EqualTo(3));
        Assert.That(session.CurrentMatch, Is.Not.Null);
        Assert.That(session.CurrentMatch.State, Is.EqualTo(MatchAuthorityLifecycleState.InRound));
        Assert.That(session.CurrentRoom.Participants[0].IsReady, Is.True);
        Assert.That(session.CurrentRoom.Participants[1].IsReady, Is.True);
        Assert.That(session.CurrentRoom.Participants[2].IsReady, Is.False);
    }

    [Test]
    public void EmitClaimAndEndMatch_BuildSummaryReflectsAuthoritativeFlow()
    {
        LocalMultiplayerSessionFacade session = new LocalMultiplayerSessionFacade();
        LocalAuthoritativeMatchSimulator simulator = new LocalAuthoritativeMatchSimulator(session);
        simulator.CreateRoomAndStartMatch(
            "host_1",
            "Host",
            new[]
            {
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_1", DisplayName = "Guest 1", Ready = true }
            },
            new AuthoritativeMatchStartRequest
            {
                RealmIndex = 0,
                RoomIndex = 0,
                SelectedCardCount = 2,
                ManaBetPerCard = 10,
                RoundSeed = 42,
                MaxCallCount = 30,
                AutoCallIntervalSeconds = 1f
            });

        MatchCallEvent call = simulator.EmitNextHostCall();
        MatchClaimResolution claim = simulator.SubmitClaim(
            "guest_1",
            MatchClaimType.Bingo,
            cardIndex: 0,
            claimCallIndex: call.CallIndex,
            idempotencyKey: "claim_1",
            markedCellKeys: new[] { "0:0", "0:1", "0:2" },
            claimedNumbers: new[] { call.CalledNumber });
        MatchEndEvent end = simulator.EndMatch(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "guest_1" });
        LocalAuthoritativeMatchSummary summary = simulator.BuildSummary();

        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(claim.ValidatedNumberCount, Is.EqualTo(1));
        Assert.That(end.WheelspinEntitledPlayerIds.Count, Is.EqualTo(1));
        Assert.That(summary.ParticipantCount, Is.EqualTo(2));
        Assert.That(summary.ReadyParticipantCount, Is.EqualTo(2));
        Assert.That(summary.CallCount, Is.EqualTo(1));
        Assert.That(summary.ClaimCount, Is.EqualTo(1));
        Assert.That(summary.EndEventCount, Is.EqualTo(1));
        Assert.That(summary.MatchState, Is.EqualTo(MatchAuthorityLifecycleState.Ended));
        Assert.That(summary.LastEndedReason, Is.EqualTo("Max balls reached."));
        Assert.That(summary.LastCall, Is.Not.Null);
        Assert.That(summary.LastClaimResolution, Is.Not.Null);
        Assert.That(summary.LastMatchEnd, Is.Not.Null);
    }
}

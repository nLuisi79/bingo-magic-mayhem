using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class PrototypeMultiplayerRoomSessionControllerTests
{
    [Test]
    public void EnsureHostReady_CreatesLobbyAndReadiesHost()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();

        MultiplayerRoomSnapshot room = controller.EnsureHostReady(
            "host_1",
            "Host",
            realmIndex: 1,
            roomIndex: 2,
            selectedCardCount: 4,
            manaBetPerCard: 25);
        MultiplayerRoomSessionDisplayModel model = controller.BuildRoomSessionDisplayModel();

        Assert.That(room, Is.Not.Null);
        Assert.That(model.RoomStateLabel, Is.EqualTo("Lobby open"));
        Assert.That(model.ReadinessSummary, Is.EqualTo("1/1 ready"));
        Assert.That(model.CanStartMatchLocally, Is.True);
    }

    [Test]
    public void EnsureLocalLobby_AddsParticipantsAndBuildsLobbyDisplay()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();

        controller.EnsureLocalLobby(
            "host_1",
            "Host",
            realmIndex: 1,
            roomIndex: 2,
            selectedCardCount: 4,
            manaBetPerCard: 25,
            localParticipants: new[]
            {
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_1", DisplayName = "Guest 1", Ready = true },
                new LocalAuthoritativeMatchParticipant { PlayerId = "guest_2", DisplayName = "Guest 2", Ready = false }
            });
        MultiplayerLobbyDisplayModel lobbyModel = controller.BuildLobbyDisplayModel();

        Assert.That(lobbyModel.HasRoom, Is.True);
        Assert.That(lobbyModel.ReadinessLabel, Is.EqualTo("2/3 ready"));
        Assert.That(lobbyModel.StateLabel, Is.EqualTo("Lobby open"));
        Assert.That(lobbyModel.ParticipantSummary, Does.Contain("Guest 2 - Waiting"));
    }

    [Test]
    public void BeginRound_RecordClaimAndEnd_PropagateThroughController()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        controller.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);

        controller.BeginLocalAuthoritativeRound(
            "host_1",
            "Host",
            realmIndex: 0,
            roomIndex: 0,
            selectedCardCount: 2,
            manaBetPerCard: 10,
            roundSeed: 99,
            maxCallCount: 30,
            autoCallIntervalSeconds: 1f);
        controller.RecordObservedCall(22, 1000);
        MatchClaimResolution claim = controller.SubmitBingoClaim(
            "host_1",
            cardIndex: 0,
            claimCallIndex: 0,
            markedCellKeys: new[] { "0:0", "0:1" },
            claimedNumbers: new[] { 22 },
            idempotencyKey: "claim_1");
        MatchEndEvent end = controller.PublishRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "host_1" });
        LocalAuthoritativeMatchSummary summary = controller.BuildMatchSummary();

        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(end.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
        Assert.That(summary.CallCount, Is.EqualTo(1));
        Assert.That(summary.ClaimCount, Is.EqualTo(1));
        Assert.That(summary.EndEventCount, Is.EqualTo(1));
    }

    [Test]
    public void Controller_PublishesLifecyclePayloadsThroughSyncAdapter()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController(
            new LocalMultiplayerSessionFacade(),
            adapter);

        controller.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        controller.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        controller.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, 99, 30, 1f);
        controller.RecordObservedCall(22, 1000);
        controller.SubmitBingoClaim(
            "host_1",
            cardIndex: 0,
            claimCallIndex: 0,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 },
            idempotencyKey: "claim_1");
        controller.PublishRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "host_1" });

        Assert.That(adapter.RoomSyncLog.Count, Is.GreaterThanOrEqualTo(3));
        Assert.That(adapter.ReadinessLog.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(adapter.MatchStartLog.Count, Is.EqualTo(1));
        Assert.That(adapter.CallBroadcastLog.Count, Is.EqualTo(1));
        Assert.That(adapter.ClaimSubmitLog.Count, Is.EqualTo(1));
        Assert.That(adapter.ClaimResolutionLog.Count, Is.EqualTo(1));
        Assert.That(adapter.MatchEndLog.Count, Is.EqualTo(1));
        Assert.That(adapter.LatestMatchEnd.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
    }

    [Test]
    public void Controller_CanApplyReceivedPayloadsIntoMirror()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController(
            new LocalMultiplayerSessionFacade(),
            adapter);

        controller.ApplyReceivedRoomSync(new MultiplayerRoomSyncPayload
        {
            RoomId = "room_remote",
            HostPlayerId = "host_remote",
            RoomState = MultiplayerRoomLifecycleState.Lobby,
            Participants =
            {
                new MultiplayerParticipantSyncPayload { PlayerId = "host_remote", DisplayName = "Host", IsHost = true, IsReady = true }
            }
        });
        controller.ApplyReceivedMatchStart(new MultiplayerMatchStartPayload
        {
            RoomId = "room_remote",
            MatchId = "match_remote",
            HostPlayerId = "host_remote"
        });
        controller.ApplyReceivedCallBroadcast(new MultiplayerCallBroadcastPayload
        {
            MatchId = "match_remote",
            CallIndex = 0,
            CalledNumber = 22,
            EmittedUtcTicks = 1000
        });

        Assert.That(adapter.Mirror.MirroredRoom.RoomId, Is.EqualTo("room_remote"));
        Assert.That(adapter.Mirror.MirroredMatch.MatchId, Is.EqualTo("match_remote"));
        Assert.That(adapter.Mirror.MirroredCalls.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.AppliedEventLog.Count, Is.EqualTo(3));
    }
}

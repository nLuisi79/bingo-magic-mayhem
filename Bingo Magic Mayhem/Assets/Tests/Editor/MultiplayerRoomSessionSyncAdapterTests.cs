using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerRoomSessionSyncAdapterTests
{
    [Test]
    public void LocalInMemoryAdapter_PublishesAndStoresLatestPayloads()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();

        adapter.PublishRoomSync(new MultiplayerRoomSyncPayload { RoomId = "room_1" });
        adapter.PublishReadinessUpdate(new MultiplayerReadinessUpdatePayload { PlayerId = "host_1", IsReady = true });
        adapter.PublishMatchStart(new MultiplayerMatchStartPayload { MatchId = "match_1" });
        adapter.PublishCallBroadcast(new MultiplayerCallBroadcastPayload { MatchId = "match_1", CalledNumber = 22 });
        adapter.PublishClaimSubmit(new MultiplayerClaimSubmitPayload { MatchId = "match_1", PlayerId = "host_1" });
        adapter.PublishClaimResolution(new MultiplayerClaimResolutionPayload { MatchId = "match_1", PlayerId = "host_1" });
        adapter.PublishMatchEnd(new MultiplayerMatchEndPayload { MatchId = "match_1" });

        Assert.That(adapter.LatestRoomSync.RoomId, Is.EqualTo("room_1"));
        Assert.That(adapter.LatestReadinessUpdate.PlayerId, Is.EqualTo("host_1"));
        Assert.That(adapter.LatestMatchStart.MatchId, Is.EqualTo("match_1"));
        Assert.That(adapter.LatestCallBroadcast.CalledNumber, Is.EqualTo(22));
        Assert.That(adapter.LatestClaimSubmit.PlayerId, Is.EqualTo("host_1"));
        Assert.That(adapter.LatestClaimResolution.PlayerId, Is.EqualTo("host_1"));
        Assert.That(adapter.LatestMatchEnd.MatchId, Is.EqualTo("match_1"));
        Assert.That(adapter.RoomSyncLog.Count, Is.EqualTo(1));
        Assert.That(adapter.ReadinessLog.Count, Is.EqualTo(1));
        Assert.That(adapter.MatchStartLog.Count, Is.EqualTo(1));
        Assert.That(adapter.CallBroadcastLog.Count, Is.EqualTo(1));
        Assert.That(adapter.ClaimSubmitLog.Count, Is.EqualTo(1));
        Assert.That(adapter.ClaimResolutionLog.Count, Is.EqualTo(1));
        Assert.That(adapter.MatchEndLog.Count, Is.EqualTo(1));
    }

    [Test]
    public void LocalInMemoryAdapter_ResetClearsState()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();
        adapter.PublishRoomSync(new MultiplayerRoomSyncPayload { RoomId = "room_1" });

        adapter.Reset();

        Assert.That(adapter.LatestRoomSync, Is.Null);
        Assert.That(adapter.RoomSyncLog.Count, Is.EqualTo(0));
    }

    [Test]
    public void LocalInMemoryAdapter_AppliesReceivedPayloadsIntoMirroredSessionState()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();

        adapter.ApplyReceivedRoomSync(new MultiplayerRoomSyncPayload
        {
            RoomId = "room_1",
            RoomCode = "L0001",
            HostPlayerId = "host_1",
            RoomState = MultiplayerRoomLifecycleState.Lobby,
            RealmIndex = 1,
            RoomIndex = 2,
            SelectedCardCount = 4,
            ManaBetPerCard = 25,
            Participants =
            {
                new MultiplayerParticipantSyncPayload { PlayerId = "host_1", DisplayName = "Host", IsHost = true, IsReady = true },
                new MultiplayerParticipantSyncPayload { PlayerId = "guest_1", DisplayName = "Guest", IsReady = false }
            }
        });
        adapter.ApplyReceivedReadinessUpdate(new MultiplayerReadinessUpdatePayload
        {
            RoomId = "room_1",
            PlayerId = "guest_1",
            IsReady = true
        });
        adapter.ApplyReceivedMatchStart(new MultiplayerMatchStartPayload
        {
            RoomId = "room_1",
            MatchId = "match_1",
            HostPlayerId = "host_1",
            RealmIndex = 1,
            RoomIndex = 2,
            SelectedCardCount = 4,
            ManaBetPerCard = 25,
            RoundSeed = 88,
            MaxCallCount = 60,
            AutoCallIntervalSeconds = 1.5f
        });
        adapter.ApplyReceivedCallBroadcast(new MultiplayerCallBroadcastPayload
        {
            MatchId = "match_1",
            CallIndex = 0,
            CalledNumber = 22,
            EmittedUtcTicks = 1000
        });
        adapter.ApplyReceivedClaimSubmit(new MultiplayerClaimSubmitPayload
        {
            MatchId = "match_1",
            PlayerId = "host_1",
            ClaimType = MatchClaimType.Bingo,
            ClaimCallIndex = 0,
            IdempotencyKey = "claim_1",
            ClaimedNumbers = { 22 }
        });
        adapter.ApplyReceivedClaimResolution(new MultiplayerClaimResolutionPayload
        {
            MatchId = "match_1",
            PlayerId = "host_1",
            ClaimType = MatchClaimType.Bingo,
            Result = MatchClaimResolutionKind.Accepted,
            AcceptedCallIndex = 0,
            ValidatedNumberCount = 1,
            Reason = "Accepted"
        });
        adapter.ApplyReceivedMatchEnd(new MultiplayerMatchEndPayload
        {
            MatchId = "match_1",
            EndReasonKind = BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.FinalBall,
            EndReason = "Max balls reached.",
            FinalCallIndex = 0,
            WheelspinEntitledPlayerIds = { "host_1" }
        });

        Assert.That(adapter.Mirror.MirroredRoom, Is.Not.Null);
        Assert.That(adapter.Mirror.MirroredRoom.RoomId, Is.EqualTo("room_1"));
        Assert.That(adapter.Mirror.MirroredRoom.State, Is.EqualTo(MultiplayerRoomLifecycleState.Closed));
        Assert.That(adapter.Mirror.MirroredRoom.Participants.Count, Is.EqualTo(2));
        Assert.That(adapter.Mirror.MirroredRoom.Participants[1].IsReady, Is.True);
        Assert.That(adapter.Mirror.MirroredMatch, Is.Not.Null);
        Assert.That(adapter.Mirror.MirroredMatch.MatchId, Is.EqualTo("match_1"));
        Assert.That(adapter.Mirror.MirroredMatch.State, Is.EqualTo(MatchAuthorityLifecycleState.Ended));
        Assert.That(adapter.Mirror.MirroredCalls.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.MirroredClaimSubmissions.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.MirroredClaimResolutions.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.MirroredMatchEnds.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.AppliedEventLog.Count, Is.EqualTo(7));
        Assert.That(adapter.Mirror.AppliedEventLog[0].SequenceId, Is.EqualTo(1));
        Assert.That(adapter.Mirror.AppliedEventLog[6].Kind, Is.EqualTo(MultiplayerSessionSyncEventKind.MatchEnd));
    }

    [Test]
    public void LocalInMemoryAdapter_IgnoresDuplicateReceivedPayloads()
    {
        LocalInMemoryMultiplayerRoomSessionSyncAdapter adapter = new LocalInMemoryMultiplayerRoomSessionSyncAdapter();
        MultiplayerCallBroadcastPayload duplicateCall = new MultiplayerCallBroadcastPayload
        {
            MatchId = "match_1",
            CallIndex = 0,
            CalledNumber = 22,
            EmittedUtcTicks = 1000
        };

        adapter.ApplyReceivedCallBroadcast(duplicateCall);
        adapter.ApplyReceivedCallBroadcast(duplicateCall);

        Assert.That(adapter.Mirror.MirroredCalls.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.AppliedEventLog.Count, Is.EqualTo(1));
        Assert.That(adapter.Mirror.DuplicateIgnoredCount, Is.EqualTo(1));
    }
}

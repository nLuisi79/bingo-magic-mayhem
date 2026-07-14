using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class MultiplayerTransportPayloadFactoryTests
{
    [Test]
    public void BuildRoomSyncPayload_CopiesRoomAndParticipantState()
    {
        LocalMultiplayerSessionFacade session = new LocalMultiplayerSessionFacade();
        session.CreateRoom("host_1", "Host", realmIndex: 2, roomIndex: 4, selectedCardCount: 6, manaBetPerCard: 30);
        session.SetParticipantReady("host_1", true);
        session.AddLocalParticipant("guest_1", "Guest 1");

        MultiplayerRoomSyncPayload payload = MultiplayerTransportPayloadFactory.BuildRoomSyncPayload(session.CurrentRoom);

        Assert.That(payload.RoomId, Is.EqualTo(session.CurrentRoom.RoomId));
        Assert.That(payload.RoomCode, Is.EqualTo(session.CurrentRoom.RoomCode));
        Assert.That(payload.RealmIndex, Is.EqualTo(2));
        Assert.That(payload.RoomIndex, Is.EqualTo(4));
        Assert.That(payload.SelectedCardCount, Is.EqualTo(6));
        Assert.That(payload.ManaBetPerCard, Is.EqualTo(30));
        Assert.That(payload.Participants.Count, Is.EqualTo(2));
        Assert.That(payload.Participants[0].PlayerId, Is.EqualTo("host_1"));
        Assert.That(payload.Participants[0].IsReady, Is.True);
        Assert.That(payload.Participants[1].DisplayName, Is.EqualTo("Guest 1"));
    }

    [Test]
    public void BuildReadinessUpdatePayload_UsesParticipantStateAndTimestamp()
    {
        LocalMultiplayerSessionFacade session = new LocalMultiplayerSessionFacade();
        session.CreateRoom("host_1", "Host", 0, 0, 2, 10);
        session.SetParticipantReady("host_1", true);

        MultiplayerReadinessUpdatePayload payload = MultiplayerTransportPayloadFactory.BuildReadinessUpdatePayload(
            session.CurrentRoom,
            "host_1",
            updatedUtcTicks: 12345);

        Assert.That(payload.RoomId, Is.EqualTo(session.CurrentRoom.RoomId));
        Assert.That(payload.PlayerId, Is.EqualTo("host_1"));
        Assert.That(payload.IsReady, Is.True);
        Assert.That(payload.UpdatedUtcTicks, Is.EqualTo(12345));
    }

    [Test]
    public void BuildMatchAndClaimPayloads_CopyAuthorityState()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        controller.EnsureHostReady("host_1", "Host", 1, 3, 4, 25);
        controller.BeginLocalAuthoritativeRound("host_1", "Host", 1, 3, 4, 25, roundSeed: 88, maxCallCount: 60, autoCallIntervalSeconds: 1.5f);

        MultiplayerMatchStartPayload startPayload = controller.BuildMatchStartPayload();
        MultiplayerClaimSubmitPayload claimPayload = controller.BuildBingoClaimSubmitPayload(
            "host_1",
            cardIndex: 1,
            claimCallIndex: 4,
            markedCellKeys: new[] { "0:0", "0:1", "0:2" },
            claimedNumbers: new[] { 5, 10, 15 },
            idempotencyKey: "claim_123");

        Assert.That(startPayload.RoomId, Is.EqualTo(controller.SessionFacade.CurrentRoom.RoomId));
        Assert.That(startPayload.MatchId, Is.EqualTo(controller.SessionFacade.CurrentMatch.MatchId));
        Assert.That(startPayload.RoundSeed, Is.EqualTo(88));
        Assert.That(startPayload.MaxCallCount, Is.EqualTo(60));
        Assert.That(startPayload.AutoCallIntervalSeconds, Is.EqualTo(1.5f));
        Assert.That(claimPayload.MatchId, Is.EqualTo(controller.SessionFacade.CurrentMatch.MatchId));
        Assert.That(claimPayload.ClaimType, Is.EqualTo(MatchClaimType.Bingo));
        Assert.That(claimPayload.MarkedCellKeys.Count, Is.EqualTo(3));
        Assert.That(claimPayload.ClaimedNumbers.Count, Is.EqualTo(3));
        Assert.That(claimPayload.IdempotencyKey, Is.EqualTo("claim_123"));
    }

    [Test]
    public void BuildCallResolutionAndEndPayloads_CopyLifecycleEvents()
    {
        PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController();
        controller.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        controller.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, roundSeed: 77, maxCallCount: 30, autoCallIntervalSeconds: 1f);

        MatchCallEvent callEvent = controller.RecordObservedCall(22, emittedUtcTicks: 1000);
        MatchClaimResolution resolution = controller.SubmitBingoClaim(
            "host_1",
            cardIndex: 0,
            claimCallIndex: 0,
            markedCellKeys: new[] { "0:0" },
            claimedNumbers: new[] { 22 },
            idempotencyKey: "claim_1");
        MatchEndEvent endEvent = controller.PublishRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Authority test end."),
            new[] { "host_1" });

        MultiplayerCallBroadcastPayload callPayload = controller.BuildCallBroadcastPayload(callEvent);
        MultiplayerClaimResolutionPayload resolutionPayload = controller.BuildClaimResolutionPayload(resolution);
        MultiplayerMatchEndPayload endPayload = controller.BuildMatchEndPayload(endEvent);

        Assert.That(callPayload.MatchId, Is.EqualTo(controller.SessionFacade.CurrentMatch.MatchId));
        Assert.That(callPayload.CalledNumber, Is.EqualTo(22));
        Assert.That(callPayload.EmittedUtcTicks, Is.EqualTo(1000));
        Assert.That(resolutionPayload.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(resolutionPayload.ValidatedNumberCount, Is.EqualTo(1));
        Assert.That(endPayload.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
        Assert.That(endPayload.FinalCallIndex, Is.EqualTo(0));
        Assert.That(endPayload.WheelspinEntitledPlayerIds, Does.Contain("host_1"));
    }
}

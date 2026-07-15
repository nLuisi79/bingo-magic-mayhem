using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class MultiplayerMatchAuthorityServiceContractTests
{
    [Test]
    public void LocalRuntime_ExposesMatchAuthorityServiceContract()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        IMultiplayerRoomSessionService roomSessionService = runtime.RoomSessionService;
        IMultiplayerMatchAuthorityService authorityService = runtime.MatchAuthorityService;

        roomSessionService.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        roomSessionService.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, 99, 30, 1f);

        MatchCallEvent callEvent = authorityService.RecordObservedCall(22, 1000);
        MatchClaimResolution claim = authorityService.SubmitBingoClaim(
            "host_1",
            0,
            0,
            new[] { "0:0" },
            new[] { 22 },
            "claim_1");
        MatchEndEvent endEvent = authorityService.PublishRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Max balls reached."),
            new[] { "host_1" });

        Assert.That(callEvent, Is.Not.Null);
        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Accepted));
        Assert.That(endEvent.EndReasonKind, Is.EqualTo(BingoRoundEndReasonKind.FinalBall));
    }

    [Test]
    public void LocalRuntime_MatchAuthorityContract_RejectsClaimBeforeAnyObservedCall()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        IMultiplayerRoomSessionService roomSessionService = runtime.RoomSessionService;
        IMultiplayerMatchAuthorityService authorityService = runtime.MatchAuthorityService;

        roomSessionService.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        roomSessionService.AddOrUpdateLocalParticipant("guest_1", "Guest", true);
        roomSessionService.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, 99, 30, 1f);

        MatchClaimResolution claim = authorityService.SubmitBingoClaim(
            "guest_1",
            0,
            0,
            new[] { "0:0" },
            new[] { 22 },
            "claim_before_call");

        Assert.That(claim.Result, Is.EqualTo(MatchClaimResolutionKind.Rejected));
        Assert.That(claim.Reason, Does.Contain("has not happened yet"));
    }

    [Test]
    public void LocalRuntime_MatchAuthorityContract_ClosesFurtherAuthorityActionsAfterRoundEnd()
    {
        PrototypeMultiplayerRuntime runtime = PrototypeMultiplayerComposition.CreateLocalRuntime("host_1");
        IMultiplayerRoomSessionService roomSessionService = runtime.RoomSessionService;
        IMultiplayerMatchAuthorityService authorityService = runtime.MatchAuthorityService;

        roomSessionService.EnsureHostReady("host_1", "Host", 0, 0, 2, 10);
        roomSessionService.BeginLocalAuthoritativeRound("host_1", "Host", 0, 0, 2, 10, 99, 30, 1f);
        authorityService.RecordObservedCall(22, 1000);
        authorityService.PublishRoundEnd(
            BingoRoundEndRules.CreateFinalBallDecision("Round complete."),
            new[] { "host_1" });

        MatchCallEvent afterEndCall = authorityService.RecordObservedCall(44, 2000);
        MatchClaimResolution afterEndClaim = authorityService.SubmitJackpotStateClaim(
            "host_1",
            0,
            0,
            new[] { 22 },
            "jackpot_after_end");

        Assert.That(afterEndCall, Is.Null);
        Assert.That(afterEndClaim.Result, Is.EqualTo(MatchClaimResolutionKind.RoundClosed));
        Assert.That(afterEndClaim.Reason, Does.Contain("not active"));
    }
}

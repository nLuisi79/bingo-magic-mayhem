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
}

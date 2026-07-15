namespace BingoMagicMayhem.Multiplayer
{
    public enum MultiplayerAuthorityFeedbackKind
    {
        None = 0,
        Informational = 1,
        Success = 2,
        Warning = 3
    }

    public sealed class MultiplayerAuthorityOutcomeModel
    {
        public MultiplayerAuthorityOutcomeModel(
            MultiplayerAuthorityFeedbackKind feedbackKind,
            string headline,
            string detail,
            bool shouldCelebrate,
            bool shouldQueueJackpotHandoff)
        {
            FeedbackKind = feedbackKind;
            Headline = headline ?? "";
            Detail = detail ?? "";
            ShouldCelebrate = shouldCelebrate;
            ShouldQueueJackpotHandoff = shouldQueueJackpotHandoff;
        }

        public MultiplayerAuthorityFeedbackKind FeedbackKind { get; }
        public string Headline { get; }
        public string Detail { get; }
        public bool ShouldCelebrate { get; }
        public bool ShouldQueueJackpotHandoff { get; }
    }

    public static class MultiplayerAuthorityOutcomePresenter
    {
        public static MultiplayerAuthorityOutcomeModel BuildClaimOutcome(MatchClaimResolution resolution)
        {
            if (resolution == null)
            {
                return new MultiplayerAuthorityOutcomeModel(
                    MultiplayerAuthorityFeedbackKind.None,
                    "No authority result.",
                    "",
                    false,
                    false);
            }

            switch (resolution.Result)
            {
                case MatchClaimResolutionKind.Accepted:
                    return new MultiplayerAuthorityOutcomeModel(
                        MultiplayerAuthorityFeedbackKind.Success,
                        "Claim accepted.",
                        BuildDetail(resolution, "Authority accepted the claim."),
                        true,
                        resolution.ClaimType == MatchClaimType.JackpotState);
                case MatchClaimResolutionKind.Duplicate:
                    return new MultiplayerAuthorityOutcomeModel(
                        MultiplayerAuthorityFeedbackKind.Informational,
                        "Duplicate claim ignored.",
                        BuildDetail(resolution, "Authority already resolved this claim."),
                        false,
                        false);
                case MatchClaimResolutionKind.RoundClosed:
                    return new MultiplayerAuthorityOutcomeModel(
                        MultiplayerAuthorityFeedbackKind.Warning,
                        "Round already closed.",
                        BuildDetail(resolution, "Claim arrived after round end."),
                        false,
                        false);
                case MatchClaimResolutionKind.Rejected:
                default:
                    return new MultiplayerAuthorityOutcomeModel(
                        MultiplayerAuthorityFeedbackKind.Warning,
                        "Claim rejected.",
                        BuildDetail(resolution, "Authority rejected the claim."),
                        false,
                        false);
            }
        }

        public static MultiplayerAuthorityOutcomeModel BuildRoundEndOutcome(MatchEndEvent endEvent)
        {
            if (endEvent == null)
            {
                return new MultiplayerAuthorityOutcomeModel(
                    MultiplayerAuthorityFeedbackKind.None,
                    "No round-end result.",
                    "",
                    false,
                    false);
            }

            bool hasWheelspin = endEvent.WheelspinEntitledPlayerIds.Count > 0;
            return new MultiplayerAuthorityOutcomeModel(
                hasWheelspin ? MultiplayerAuthorityFeedbackKind.Success : MultiplayerAuthorityFeedbackKind.Informational,
                hasWheelspin ? "Wheelspin handoff queued." : "Round ended.",
                string.IsNullOrEmpty(endEvent.EndReason) ? "Authority ended the round." : endEvent.EndReason,
                false,
                hasWheelspin);
        }

        private static string BuildDetail(MatchClaimResolution resolution, string fallback)
        {
            if (resolution == null)
            {
                return fallback ?? "";
            }

            return string.IsNullOrEmpty(resolution.Reason) ? (fallback ?? "") : resolution.Reason;
        }
    }
}

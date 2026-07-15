namespace BingoMagicMayhem.Multiplayer
{
    public sealed class MultiplayerGameplayRoundDisplayModel
    {
        public MultiplayerGameplayRoundDisplayModel(
            string roomSummaryLabel,
            string authoritySummaryLabel,
            string claimSummaryLabel,
            string postRoundSummaryLabel)
        {
            RoomSummaryLabel = roomSummaryLabel ?? "";
            AuthoritySummaryLabel = authoritySummaryLabel ?? "";
            ClaimSummaryLabel = claimSummaryLabel ?? "";
            PostRoundSummaryLabel = postRoundSummaryLabel ?? "";
        }

        public string RoomSummaryLabel { get; }
        public string AuthoritySummaryLabel { get; }
        public string ClaimSummaryLabel { get; }
        public string PostRoundSummaryLabel { get; }
    }

    public static class MultiplayerGameplayRoundPresenter
    {
        public static MultiplayerGameplayRoundDisplayModel Build(MultiplayerRoomSessionDisplayModel sessionModel)
        {
            if (sessionModel == null)
            {
                return new MultiplayerGameplayRoundDisplayModel(
                    "No multiplayer room.",
                    "Authority unavailable.",
                    "No claim state.",
                    "No post-round state.");
            }

            return new MultiplayerGameplayRoundDisplayModel(
                $"{sessionModel.RoomStateLabel}  |  {sessionModel.ReadinessSummary}",
                $"{sessionModel.AuthorityStatusLabel}  |  {sessionModel.CallAuthorityLabel}",
                $"{sessionModel.ClaimStatusLabel}  |  {sessionModel.ClaimResolutionReasonLabel}",
                $"{sessionModel.JackpotHandoffLabel}  |  {sessionModel.LastEventSummary}");
        }
    }
}

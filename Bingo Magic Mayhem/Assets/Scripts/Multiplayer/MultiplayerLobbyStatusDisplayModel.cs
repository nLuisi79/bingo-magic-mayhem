namespace BingoMagicMayhem.Multiplayer
{
    public sealed class MultiplayerLobbyStatusDisplayModel
    {
        public MultiplayerLobbyStatusDisplayModel(
            string titleSummaryLabel,
            string readinessSummaryLabel,
            string detailSummaryLabel,
            string participantSummaryLabel,
            string actionLabel,
            string detailLabel,
            bool canStart)
        {
            TitleSummaryLabel = titleSummaryLabel ?? "";
            ReadinessSummaryLabel = readinessSummaryLabel ?? "";
            DetailSummaryLabel = detailSummaryLabel ?? "";
            ParticipantSummaryLabel = participantSummaryLabel ?? "";
            ActionLabel = actionLabel ?? "";
            DetailLabel = detailLabel ?? "";
            CanStart = canStart;
        }

        public string TitleSummaryLabel { get; }
        public string ReadinessSummaryLabel { get; }
        public string DetailSummaryLabel { get; }
        public string ParticipantSummaryLabel { get; }
        public string ActionLabel { get; }
        public string DetailLabel { get; }
        public bool CanStart { get; }
    }

    public static class MultiplayerLobbyStatusPresenter
    {
        public static MultiplayerLobbyStatusDisplayModel Build(MultiplayerLobbyDisplayModel lobbyModel)
        {
            if (lobbyModel == null)
            {
                return new MultiplayerLobbyStatusDisplayModel(
                    "Local Multiplayer  |  No room",
                    "0/0 ready",
                    "Create local room — Create a room to begin multiplayer.",
                    "No participants joined.",
                    "Create local room",
                    "Create a room to begin multiplayer.",
                    false);
            }

            return new MultiplayerLobbyStatusDisplayModel(
                $"{lobbyModel.Title}  |  {lobbyModel.StateLabel}",
                lobbyModel.ReadinessLabel,
                $"{lobbyModel.ActionLabel} — {lobbyModel.DetailLabel}",
                lobbyModel.ParticipantSummary,
                lobbyModel.ActionLabel,
                lobbyModel.DetailLabel,
                lobbyModel.CanStart);
        }
    }
}

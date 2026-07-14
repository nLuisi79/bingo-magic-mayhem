namespace BingoMagicMayhem.Multiplayer
{
    public sealed class MultiplayerLobbyDisplayModel
    {
        public MultiplayerLobbyDisplayModel(
            string title,
            string roomCodeLabel,
            string readinessLabel,
            string stateLabel,
            string participantSummary,
            string detailLabel,
            string actionLabel,
            MultiplayerGameplayFlowState gameplayFlowState,
            bool canStart,
            bool hasRoom)
        {
            Title = title ?? "";
            RoomCodeLabel = roomCodeLabel ?? "";
            ReadinessLabel = readinessLabel ?? "";
            StateLabel = stateLabel ?? "";
            ParticipantSummary = participantSummary ?? "";
            DetailLabel = detailLabel ?? "";
            ActionLabel = actionLabel ?? "";
            GameplayFlowState = gameplayFlowState;
            CanStart = canStart;
            HasRoom = hasRoom;
        }

        public string Title { get; }
        public string RoomCodeLabel { get; }
        public string ReadinessLabel { get; }
        public string StateLabel { get; }
        public string ParticipantSummary { get; }
        public string DetailLabel { get; }
        public string ActionLabel { get; }
        public MultiplayerGameplayFlowState GameplayFlowState { get; }
        public bool CanStart { get; }
        public bool HasRoom { get; }
    }
}

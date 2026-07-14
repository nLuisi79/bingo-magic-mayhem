using System.Text;

namespace BingoMagicMayhem.Multiplayer
{
    public static class MultiplayerLobbyPresenter
    {
        public static MultiplayerLobbyDisplayModel Build(MultiplayerRoomSessionDisplayModel sessionModel)
        {
            if (sessionModel == null || !sessionModel.HasRoom)
            {
                return new MultiplayerLobbyDisplayModel(
                    "Local Multiplayer",
                "No room code",
                "0/0 ready",
                "No room",
                "No participants joined.",
                "Create a room to begin multiplayer.",
                "Create local room",
                MultiplayerGameplayFlowState.NoRoom,
                false,
                false);
        }

            return new MultiplayerLobbyDisplayModel(
                "Local Multiplayer",
                string.IsNullOrEmpty(sessionModel.RoomCode) ? "No room code" : $"Room Code {sessionModel.RoomCode}",
                sessionModel.ReadinessSummary,
                sessionModel.RoomStateLabel,
                BuildParticipantSummary(sessionModel),
                string.IsNullOrEmpty(sessionModel.HostStartHint) ? sessionModel.AuthorityStatusLabel : sessionModel.HostStartHint,
                BuildActionLabel(sessionModel),
                sessionModel.GameplayFlowState,
                sessionModel.CanStartMatchLocally,
                true);
        }

        private static string BuildActionLabel(MultiplayerRoomSessionDisplayModel sessionModel)
        {
            switch (sessionModel.GameplayFlowState)
            {
                case MultiplayerGameplayFlowState.ReadyToStart:
                    return "Start local match";
                case MultiplayerGameplayFlowState.MatchInProgress:
                    return "Match already running";
                case MultiplayerGameplayFlowState.MatchEnded:
                    return "Round finished";
                case MultiplayerGameplayFlowState.WaitingForPlayers:
                    return "Waiting for more players";
                default:
                    return "Waiting for ready players";
            }
        }

        private static string BuildParticipantSummary(MultiplayerRoomSessionDisplayModel sessionModel)
        {
            if (sessionModel.Participants == null || sessionModel.Participants.Count == 0)
            {
                return "No participants joined.";
            }

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < sessionModel.Participants.Count; index++)
            {
                MultiplayerRoomParticipantDisplayModel participant = sessionModel.Participants[index];
                if (participant == null)
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append("  |  ");
                }

                builder.Append(participant.DisplayName);
                if (participant.IsHost)
                {
                    builder.Append(" (Host)");
                }

                builder.Append(" - ");
                builder.Append(participant.PresenceLabel);
            }

            return builder.ToString();
        }
    }
}

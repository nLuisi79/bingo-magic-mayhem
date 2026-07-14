using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    public enum MultiplayerGameplayFlowState
    {
        NoRoom = 0,
        WaitingForPlayers = 1,
        WaitingForReadyPlayers = 2,
        ReadyToStart = 3,
        MatchInProgress = 4,
        MatchEnded = 5
    }

    public enum MultiplayerClaimPresentationState
    {
        None = 0,
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Duplicate = 4,
        RoundClosed = 5
    }

    public enum MultiplayerPostRoundSequenceState
    {
        None = 0,
        RoundActive = 1,
        RoundEnded = 2,
        JackpotPending = 3,
        JackpotNotEligible = 4
    }

    public sealed class MultiplayerRoomParticipantDisplayModel
    {
        public MultiplayerRoomParticipantDisplayModel(
            string playerId,
            string displayName,
            string presenceLabel,
            bool isHost,
            bool isReady,
            bool isConnected)
        {
            PlayerId = playerId ?? "";
            DisplayName = displayName ?? "";
            PresenceLabel = presenceLabel ?? "";
            IsHost = isHost;
            IsReady = isReady;
            IsConnected = isConnected;
        }

        public string PlayerId { get; }
        public string DisplayName { get; }
        public string PresenceLabel { get; }
        public bool IsHost { get; }
        public bool IsReady { get; }
        public bool IsConnected { get; }
    }

    public sealed class MultiplayerRoomSessionDisplayModel
    {
        public MultiplayerRoomSessionDisplayModel(
            string roomId,
            string roomCode,
            string roomStateLabel,
            string matchId,
            string matchStateLabel,
            string hostDisplayName,
            string selectionSummary,
            string readinessSummary,
            string activitySummary,
            string lastEventSummary,
            string authorityStatusLabel,
            string hostStartHint,
            string callAuthorityLabel,
            string latestCallLabel,
            MultiplayerClaimPresentationState claimPresentationState,
            string claimStatusLabel,
            string claimResolutionReasonLabel,
            MultiplayerPostRoundSequenceState postRoundSequenceState,
            string jackpotHandoffLabel,
            int wheelspinEntitledPlayerCount,
            MultiplayerGameplayFlowState gameplayFlowState,
            int connectedParticipantCount,
            int readyParticipantCount,
            int pendingReadyParticipantCount,
            bool canStartMatchLocally,
            bool hasActiveMatch,
            IReadOnlyList<MultiplayerRoomParticipantDisplayModel> participants)
        {
            RoomId = roomId ?? "";
            RoomCode = roomCode ?? "";
            RoomStateLabel = roomStateLabel ?? "";
            MatchId = matchId ?? "";
            MatchStateLabel = matchStateLabel ?? "";
            HostDisplayName = hostDisplayName ?? "";
            SelectionSummary = selectionSummary ?? "";
            ReadinessSummary = readinessSummary ?? "";
            ActivitySummary = activitySummary ?? "";
            LastEventSummary = lastEventSummary ?? "";
            AuthorityStatusLabel = authorityStatusLabel ?? "";
            HostStartHint = hostStartHint ?? "";
            CallAuthorityLabel = callAuthorityLabel ?? "";
            LatestCallLabel = latestCallLabel ?? "";
            ClaimPresentationState = claimPresentationState;
            ClaimStatusLabel = claimStatusLabel ?? "";
            ClaimResolutionReasonLabel = claimResolutionReasonLabel ?? "";
            PostRoundSequenceState = postRoundSequenceState;
            JackpotHandoffLabel = jackpotHandoffLabel ?? "";
            WheelspinEntitledPlayerCount = wheelspinEntitledPlayerCount;
            GameplayFlowState = gameplayFlowState;
            ConnectedParticipantCount = connectedParticipantCount;
            ReadyParticipantCount = readyParticipantCount;
            PendingReadyParticipantCount = pendingReadyParticipantCount;
            CanStartMatchLocally = canStartMatchLocally;
            HasActiveMatch = hasActiveMatch;
            Participants = participants ?? new List<MultiplayerRoomParticipantDisplayModel>();
        }

        public string RoomId { get; }
        public string RoomCode { get; }
        public string RoomStateLabel { get; }
        public string MatchId { get; }
        public string MatchStateLabel { get; }
        public string HostDisplayName { get; }
        public string SelectionSummary { get; }
        public string ReadinessSummary { get; }
        public string ActivitySummary { get; }
        public string LastEventSummary { get; }
        public string AuthorityStatusLabel { get; }
        public string HostStartHint { get; }
        public string CallAuthorityLabel { get; }
        public string LatestCallLabel { get; }
        public MultiplayerClaimPresentationState ClaimPresentationState { get; }
        public string ClaimStatusLabel { get; }
        public string ClaimResolutionReasonLabel { get; }
        public MultiplayerPostRoundSequenceState PostRoundSequenceState { get; }
        public string JackpotHandoffLabel { get; }
        public int WheelspinEntitledPlayerCount { get; }
        public MultiplayerGameplayFlowState GameplayFlowState { get; }
        public int ConnectedParticipantCount { get; }
        public int ReadyParticipantCount { get; }
        public int PendingReadyParticipantCount { get; }
        public bool CanStartMatchLocally { get; }
        public bool HasActiveMatch { get; }
        public bool HasRoom => !string.IsNullOrEmpty(RoomId);
        public IReadOnlyList<MultiplayerRoomParticipantDisplayModel> Participants { get; }
    }
}

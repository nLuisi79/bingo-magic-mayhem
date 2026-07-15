using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Builds a UI-safe room/session read model from the local multiplayer facade.
    /// This keeps view binding decoupled from the mutable authority/session types.
    /// </summary>
    public static class MultiplayerRoomSessionPresenter
    {
        public static MultiplayerRoomSessionDisplayModel Build(IMultiplayerSessionFacade sessionFacade)
        {
            return Build(MultiplayerRoomSessionSnapshotFactory.Build(sessionFacade));
        }

        public static MultiplayerRoomSessionDisplayModel Build(MultiplayerRoomSessionSnapshot snapshot)
        {
            if (snapshot == null || snapshot.Room == null || string.IsNullOrEmpty(snapshot.Room.RoomId))
            {
                return new MultiplayerRoomSessionDisplayModel(
                    "",
                    "",
                    "No room",
                    "",
                    "No match",
                    "",
                "No room selection",
                "0/0 ready",
                "Calls 0  |  Claims 0  |  Ends 0",
                "No room activity recorded.",
                "No authority session.",
                "Create a room to begin multiplayer.",
                "Call authority offline.",
                "No calls yet.",
                MultiplayerClaimPresentationState.None,
                "No claims resolved.",
                "No claim resolution reason.",
                MultiplayerPostRoundSequenceState.None,
                "No jackpot handoff pending.",
                0,
                MultiplayerGameplayFlowState.NoRoom,
                0,
                0,
                0,
                false,
                false,
                new List<MultiplayerRoomParticipantDisplayModel>());
            }

            MultiplayerRoomSnapshot room = snapshot.Room;
            MatchAuthorityState match = snapshot.Match;
            List<MultiplayerRoomParticipantDisplayModel> participants = BuildParticipants(room);
            int connectedCount = CountConnectedParticipants(room);
            int readyCount = CountConnectedReadyParticipants(room);
            int pendingReadyCount = CountPendingReadyParticipants(room);
            string hostDisplayName = ResolveHostDisplayName(room);
            bool hasActiveMatch = match != null && match.State == MatchAuthorityLifecycleState.InRound;
            bool canStartMatchLocally = room.State == MultiplayerRoomLifecycleState.Lobby
                && connectedCount > 1
                && readyCount == connectedCount;
            MultiplayerGameplayFlowState gameplayFlowState = BuildGameplayFlowState(room, match, connectedCount, readyCount, pendingReadyCount);
            MatchCallEvent lastCall = snapshot.CallLog.Count > 0 ? snapshot.CallLog[snapshot.CallLog.Count - 1] : null;
            MatchClaimResolution lastClaim = snapshot.ClaimLog.Count > 0 ? snapshot.ClaimLog[snapshot.ClaimLog.Count - 1] : null;
            MatchEndEvent lastEnd = snapshot.MatchEndLog.Count > 0 ? snapshot.MatchEndLog[snapshot.MatchEndLog.Count - 1] : null;
            MultiplayerClaimPresentationState claimPresentationState = BuildClaimPresentationState(lastClaim);
            MultiplayerPostRoundSequenceState postRoundSequenceState = BuildPostRoundSequenceState(gameplayFlowState, lastEnd);

            return new MultiplayerRoomSessionDisplayModel(
                room.RoomId,
                room.RoomCode,
                BuildRoomStateLabel(room.State),
                match?.MatchId ?? "",
                BuildMatchStateLabel(match?.State ?? MatchAuthorityLifecycleState.None),
                hostDisplayName,
                $"Realm {room.SelectedRealmIndex + 1}  |  Room {room.SelectedRoomIndex + 1}  |  Cards {room.SelectedCardCount}  |  Bet {room.ManaBetPerCard}",
                $"{readyCount}/{connectedCount} ready",
                $"Calls {snapshot.CallLog.Count}  |  Claims {snapshot.ClaimLog.Count}  |  Ends {snapshot.MatchEndLog.Count}",
                BuildLastEventSummary(snapshot),
                BuildAuthorityStatusLabel(gameplayFlowState, pendingReadyCount, match),
                BuildHostStartHint(gameplayFlowState, pendingReadyCount),
                BuildCallAuthorityLabel(gameplayFlowState, lastCall),
                BuildLatestCallLabel(lastCall),
                claimPresentationState,
                BuildClaimStatusLabel(lastClaim),
                BuildClaimResolutionReasonLabel(lastClaim),
                postRoundSequenceState,
                BuildJackpotHandoffLabel(lastEnd),
                lastEnd?.WheelspinEntitledPlayerIds.Count ?? 0,
                gameplayFlowState,
                connectedCount,
                readyCount,
                pendingReadyCount,
                canStartMatchLocally,
                hasActiveMatch,
                participants);
        }

        private static List<MultiplayerRoomParticipantDisplayModel> BuildParticipants(MultiplayerRoomSnapshot room)
        {
            List<MultiplayerRoomParticipantDisplayModel> participants = new List<MultiplayerRoomParticipantDisplayModel>();
            if (room == null)
            {
                return participants;
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant == null)
                {
                    continue;
                }

                participants.Add(new MultiplayerRoomParticipantDisplayModel(
                    participant.PlayerId,
                    participant.DisplayName,
                    BuildPresenceLabel(participant.IsReady, participant.IsConnected),
                    participant.IsHost,
                    participant.IsReady,
                    participant.IsConnected));
            }

            return participants;
        }

        private static string BuildLastEventSummary(MultiplayerRoomSessionSnapshot snapshot)
        {
            if (snapshot.MatchEndLog.Count > 0)
            {
                MatchEndEvent end = snapshot.MatchEndLog[snapshot.MatchEndLog.Count - 1];
                return $"Ended: {end.EndReason}";
            }

            if (snapshot.ClaimLog.Count > 0)
            {
                MatchClaimResolution claim = snapshot.ClaimLog[snapshot.ClaimLog.Count - 1];
                return $"Last claim: {claim.PlayerId} {claim.Result}";
            }

            if (snapshot.CallLog.Count > 0)
            {
                MatchCallEvent call = snapshot.CallLog[snapshot.CallLog.Count - 1];
                return $"Last call: {call.CalledNumber}";
            }

            return "Room created. No authority events yet.";
        }

        private static string ResolveHostDisplayName(MultiplayerRoomSnapshot room)
        {
            if (room == null)
            {
                return "";
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant != null && participant.IsHost)
                {
                    return participant.DisplayName ?? "";
                }
            }

            return "";
        }

        private static int CountConnectedReadyParticipants(MultiplayerRoomSnapshot room)
        {
            int readyCount = 0;
            if (room == null)
            {
                return readyCount;
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant != null && participant.IsConnected && participant.IsReady)
                {
                    readyCount++;
                }
            }

            return readyCount;
        }

        private static int CountConnectedParticipants(MultiplayerRoomSnapshot room)
        {
            int connectedCount = 0;
            if (room == null)
            {
                return connectedCount;
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant != null && participant.IsConnected)
                {
                    connectedCount++;
                }
            }

            return connectedCount;
        }

        private static int CountPendingReadyParticipants(MultiplayerRoomSnapshot room)
        {
            int pendingCount = 0;
            if (room == null)
            {
                return pendingCount;
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant != null && participant.IsConnected && !participant.IsReady)
                {
                    pendingCount++;
                }
            }

            return pendingCount;
        }

        private static MultiplayerGameplayFlowState BuildGameplayFlowState(
            MultiplayerRoomSnapshot room,
            MatchAuthorityState match,
            int connectedCount,
            int readyCount,
            int pendingReadyCount)
        {
            if (room == null)
            {
                return MultiplayerGameplayFlowState.NoRoom;
            }

            if (match != null && match.State == MatchAuthorityLifecycleState.InRound)
            {
                return MultiplayerGameplayFlowState.MatchInProgress;
            }

            if (match != null && match.State == MatchAuthorityLifecycleState.Ended)
            {
                return MultiplayerGameplayFlowState.MatchEnded;
            }

            if (connectedCount <= 1)
            {
                return MultiplayerGameplayFlowState.WaitingForPlayers;
            }

            if (readyCount == connectedCount && connectedCount > 1)
            {
                return MultiplayerGameplayFlowState.ReadyToStart;
            }

            if (pendingReadyCount > 0)
            {
                return MultiplayerGameplayFlowState.WaitingForReadyPlayers;
            }

            return MultiplayerGameplayFlowState.WaitingForPlayers;
        }

        private static string BuildAuthorityStatusLabel(
            MultiplayerGameplayFlowState gameplayFlowState,
            int pendingReadyCount,
            MatchAuthorityState match)
        {
            switch (gameplayFlowState)
            {
                case MultiplayerGameplayFlowState.WaitingForPlayers:
                    return "Host lobby open. Waiting for more players to join.";
                case MultiplayerGameplayFlowState.WaitingForReadyPlayers:
                    return pendingReadyCount == 1
                        ? "Authority idle. Waiting for 1 player to ready."
                        : $"Authority idle. Waiting for {pendingReadyCount} players to ready.";
                case MultiplayerGameplayFlowState.ReadyToStart:
                    return "Authority armed. Host can start the round.";
                case MultiplayerGameplayFlowState.MatchInProgress:
                    return $"Authority live. Round active in {match?.MatchId ?? "current match"}.";
                case MultiplayerGameplayFlowState.MatchEnded:
                    return "Authority complete. Round end has been recorded.";
                default:
                    return "No authority session.";
            }
        }

        private static string BuildHostStartHint(MultiplayerGameplayFlowState gameplayFlowState, int pendingReadyCount)
        {
            switch (gameplayFlowState)
            {
                case MultiplayerGameplayFlowState.WaitingForPlayers:
                    return "Invite or add at least one more player before starting.";
                case MultiplayerGameplayFlowState.WaitingForReadyPlayers:
                    return pendingReadyCount == 1
                        ? "One connected player still needs to ready up."
                        : $"{pendingReadyCount} connected players still need to ready up.";
                case MultiplayerGameplayFlowState.ReadyToStart:
                    return "All connected players are ready. Host may start.";
                case MultiplayerGameplayFlowState.MatchInProgress:
                    return "Round already started. Calls and claims are authority-controlled.";
                case MultiplayerGameplayFlowState.MatchEnded:
                    return "Round complete. Resolve post-round flow before starting again.";
                default:
                    return "Create a room to begin multiplayer.";
            }
        }

        private static string BuildCallAuthorityLabel(MultiplayerGameplayFlowState gameplayFlowState, MatchCallEvent lastCall)
        {
            switch (gameplayFlowState)
            {
                case MultiplayerGameplayFlowState.MatchInProgress:
                    return lastCall == null
                        ? "Call authority live. Awaiting first number."
                        : $"Call authority live. Latest authoritative call index {lastCall.CallIndex}.";
                case MultiplayerGameplayFlowState.MatchEnded:
                    return "Call authority stopped. Round has ended.";
                default:
                    return "Call authority offline.";
            }
        }

        private static string BuildLatestCallLabel(MatchCallEvent lastCall)
        {
            if (lastCall == null)
            {
                return "No calls yet.";
            }

            return $"Latest call #{lastCall.CallIndex + 1}: {lastCall.CalledNumber}";
        }

        private static string BuildClaimStatusLabel(MatchClaimResolution lastClaim)
        {
            if (lastClaim == null)
            {
                return "No claims resolved.";
            }

            return $"Last claim {lastClaim.Result}: {lastClaim.PlayerId} at call {lastClaim.AcceptedCallIndex + 1}";
        }

        private static string BuildClaimResolutionReasonLabel(MatchClaimResolution lastClaim)
        {
            if (lastClaim == null)
            {
                return "No claim resolution reason.";
            }

            return string.IsNullOrEmpty(lastClaim.Reason)
                ? "No claim resolution reason."
                : lastClaim.Reason;
        }

        private static MultiplayerClaimPresentationState BuildClaimPresentationState(MatchClaimResolution lastClaim)
        {
            if (lastClaim == null)
            {
                return MultiplayerClaimPresentationState.None;
            }

            switch (lastClaim.Result)
            {
                case MatchClaimResolutionKind.Accepted:
                    return MultiplayerClaimPresentationState.Accepted;
                case MatchClaimResolutionKind.Duplicate:
                    return MultiplayerClaimPresentationState.Duplicate;
                case MatchClaimResolutionKind.RoundClosed:
                    return MultiplayerClaimPresentationState.RoundClosed;
                case MatchClaimResolutionKind.Rejected:
                    return MultiplayerClaimPresentationState.Rejected;
                default:
                    return MultiplayerClaimPresentationState.Pending;
            }
        }

        private static MultiplayerPostRoundSequenceState BuildPostRoundSequenceState(
            MultiplayerGameplayFlowState gameplayFlowState,
            MatchEndEvent lastEnd)
        {
            if (gameplayFlowState == MultiplayerGameplayFlowState.MatchInProgress)
            {
                return MultiplayerPostRoundSequenceState.RoundActive;
            }

            if (lastEnd == null)
            {
                return MultiplayerPostRoundSequenceState.None;
            }

            if (lastEnd.WheelspinEntitledPlayerIds.Count > 0)
            {
                return MultiplayerPostRoundSequenceState.JackpotPending;
            }

            return MultiplayerPostRoundSequenceState.JackpotNotEligible;
        }

        private static string BuildJackpotHandoffLabel(MatchEndEvent lastEnd)
        {
            if (lastEnd == null)
            {
                return "No jackpot handoff pending.";
            }

            int entitledCount = lastEnd.WheelspinEntitledPlayerIds.Count;
            if (entitledCount <= 0)
            {
                return "Round ended with no wheelspin entitlements queued.";
            }

            return entitledCount == 1
                ? "Round ended. 1 player is queued for jackpot wheelspin handoff."
                : $"Round ended. {entitledCount} players are queued for jackpot wheelspin handoff.";
        }

        private static string BuildPresenceLabel(bool isReady, bool isConnected)
        {
            if (!isConnected)
            {
                return "Disconnected";
            }

            return isReady ? "Ready" : "Waiting";
        }

        private static string BuildRoomStateLabel(MultiplayerRoomLifecycleState state)
        {
            switch (state)
            {
                case MultiplayerRoomLifecycleState.Lobby:
                    return "Lobby open";
                case MultiplayerRoomLifecycleState.MatchInProgress:
                    return "Match in progress";
                case MultiplayerRoomLifecycleState.Closed:
                    return "Room closed";
                default:
                    return "No room";
            }
        }

        private static string BuildMatchStateLabel(MatchAuthorityLifecycleState state)
        {
            switch (state)
            {
                case MatchAuthorityLifecycleState.Lobby:
                    return "Lobby";
                case MatchAuthorityLifecycleState.InRound:
                    return "Round active";
                case MatchAuthorityLifecycleState.Ended:
                    return "Round ended";
                default:
                    return "No match";
            }
        }
    }
}

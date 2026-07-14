using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Central translator for converting local room/match authority state into the
    /// transport-neutral payload contracts that a future UGS-backed room/session layer
    /// can publish or consume. This keeps sync payload construction out of UI/gameplay
    /// roots and behind one backend swap seam.
    /// </summary>
    public static class MultiplayerTransportPayloadFactory
    {
        public static MultiplayerRoomSyncPayload BuildRoomSyncPayload(MultiplayerRoomSnapshot room)
        {
            MultiplayerRoomSyncPayload payload = new MultiplayerRoomSyncPayload();
            if (room == null)
            {
                return payload;
            }

            payload.RoomId = room.RoomId ?? "";
            payload.RoomCode = room.RoomCode ?? "";
            payload.HostPlayerId = room.HostPlayerId ?? "";
            payload.RoomState = room.State;
            payload.RealmIndex = room.SelectedRealmIndex;
            payload.RoomIndex = room.SelectedRoomIndex;
            payload.SelectedCardCount = room.SelectedCardCount;
            payload.ManaBetPerCard = room.ManaBetPerCard;

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant == null)
                {
                    continue;
                }

                payload.Participants.Add(BuildParticipantSyncPayload(participant));
            }

            return payload;
        }

        public static MultiplayerParticipantSyncPayload BuildParticipantSyncPayload(MultiplayerParticipantSnapshot participant)
        {
            MultiplayerParticipantSyncPayload payload = new MultiplayerParticipantSyncPayload();
            if (participant == null)
            {
                return payload;
            }

            payload.PlayerId = participant.PlayerId ?? "";
            payload.DisplayName = participant.DisplayName ?? "";
            payload.IsHost = participant.IsHost;
            payload.IsReady = participant.IsReady;
            payload.IsConnected = participant.IsConnected;
            return payload;
        }

        public static MultiplayerReadinessUpdatePayload BuildReadinessUpdatePayload(MultiplayerRoomSnapshot room, string playerId, long updatedUtcTicks = 0)
        {
            MultiplayerReadinessUpdatePayload payload = new MultiplayerReadinessUpdatePayload
            {
                RoomId = room?.RoomId ?? "",
                PlayerId = playerId ?? "",
                UpdatedUtcTicks = updatedUtcTicks > 0 ? updatedUtcTicks : DateTime.UtcNow.Ticks
            };

            if (room == null || string.IsNullOrEmpty(playerId))
            {
                return payload;
            }

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant != null && string.Equals(participant.PlayerId, playerId, StringComparison.Ordinal))
                {
                    payload.IsReady = participant.IsReady;
                    break;
                }
            }

            return payload;
        }

        public static MultiplayerMatchStartPayload BuildMatchStartPayload(MatchAuthorityState match)
        {
            MultiplayerMatchStartPayload payload = new MultiplayerMatchStartPayload();
            if (match == null)
            {
                return payload;
            }

            payload.RoomId = match.RoomId ?? "";
            payload.MatchId = match.MatchId ?? "";
            payload.HostPlayerId = match.HostPlayerId ?? "";
            payload.RealmIndex = match.RealmIndex;
            payload.RoomIndex = match.RoomIndex;
            payload.SelectedCardCount = match.SelectedCardCount;
            payload.ManaBetPerCard = match.ManaBetPerCard;
            payload.RoundSeed = match.RoundSeed;
            payload.MaxCallCount = match.MaxCallCount;
            payload.AutoCallIntervalSeconds = match.AutoCallIntervalSeconds;
            return payload;
        }

        public static MultiplayerCallBroadcastPayload BuildCallBroadcastPayload(MatchCallEvent callEvent)
        {
            MultiplayerCallBroadcastPayload payload = new MultiplayerCallBroadcastPayload();
            if (callEvent == null)
            {
                return payload;
            }

            payload.MatchId = callEvent.MatchId ?? "";
            payload.CallIndex = callEvent.CallIndex;
            payload.CalledNumber = callEvent.CalledNumber;
            payload.EmittedUtcTicks = callEvent.EmittedUtcTicks;
            return payload;
        }

        public static MultiplayerClaimSubmitPayload BuildClaimSubmitPayload(
            string matchId,
            string playerId,
            MatchClaimType claimType,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            MultiplayerClaimSubmitPayload payload = new MultiplayerClaimSubmitPayload
            {
                MatchId = matchId ?? "",
                PlayerId = playerId ?? "",
                ClaimType = claimType,
                CardIndex = cardIndex,
                ClaimCallIndex = claimCallIndex,
                IdempotencyKey = idempotencyKey ?? ""
            };

            AppendRange(payload.MarkedCellKeys, markedCellKeys);
            AppendRange(payload.ClaimedNumbers, claimedNumbers);
            return payload;
        }

        public static MultiplayerClaimSubmitPayload BuildClaimSubmitPayload(MatchClaimAttempt attempt)
        {
            if (attempt == null)
            {
                return new MultiplayerClaimSubmitPayload();
            }

            return BuildClaimSubmitPayload(
                attempt.MatchId,
                attempt.PlayerId,
                attempt.ClaimType,
                attempt.CardIndex,
                attempt.ClaimCallIndex,
                attempt.MarkedCellKeys,
                attempt.ClaimedNumbers,
                attempt.IdempotencyKey);
        }

        public static MultiplayerClaimResolutionPayload BuildClaimResolutionPayload(MatchClaimResolution resolution)
        {
            MultiplayerClaimResolutionPayload payload = new MultiplayerClaimResolutionPayload();
            if (resolution == null)
            {
                return payload;
            }

            payload.MatchId = resolution.MatchId ?? "";
            payload.PlayerId = resolution.PlayerId ?? "";
            payload.ClaimType = resolution.ClaimType;
            payload.Result = resolution.Result;
            payload.AcceptedCallIndex = resolution.AcceptedCallIndex;
            payload.ValidatedNumberCount = resolution.ValidatedNumberCount;
            payload.Reason = resolution.Reason ?? "";
            return payload;
        }

        public static MultiplayerMatchEndPayload BuildMatchEndPayload(MatchEndEvent endEvent)
        {
            MultiplayerMatchEndPayload payload = new MultiplayerMatchEndPayload();
            if (endEvent == null)
            {
                return payload;
            }

            payload.MatchId = endEvent.MatchId ?? "";
            payload.EndReasonKind = endEvent.EndReasonKind;
            payload.EndReason = endEvent.EndReason ?? "";
            payload.FinalCallIndex = endEvent.FinalCallIndex;
            AppendRange(payload.WheelspinEntitledPlayerIds, endEvent.WheelspinEntitledPlayerIds);
            return payload;
        }

        private static void AppendRange<T>(List<T> target, IReadOnlyList<T> values)
        {
            if (target == null || values == null)
            {
                return;
            }

            for (int index = 0; index < values.Count; index++)
            {
                target.Add(values[index]);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Non-live UGS-shaped runtime adapter. This is an explicit boundary seam for future
    /// UGS-backed multiplayer wiring, while safely delegating to the local prototype
    /// runtime until real service activation is approved.
    /// </summary>
    public sealed class PrototypeMultiplayerUgsRuntimeAdapter : IMultiplayerRoomSessionService, IMultiplayerMatchAuthorityService
    {
        private readonly PrototypeMultiplayerRuntime localFallbackRuntime;

        public PrototypeMultiplayerUgsRuntimeAdapter(PrototypeMultiplayerRuntime localFallbackRuntime)
        {
            this.localFallbackRuntime = localFallbackRuntime ?? throw new ArgumentNullException(nameof(localFallbackRuntime));
        }

        public PrototypeMultiplayerRuntime LocalFallbackRuntime => localFallbackRuntime;
        public MultiplayerRoomSnapshot CurrentRoom => localFallbackRuntime.RoomSessionService.CurrentRoom;

        public MultiplayerRoomSnapshot EnsureLocalRoom(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            return localFallbackRuntime.RoomSessionService.EnsureLocalRoom(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
        }

        public MultiplayerRoomSnapshot AddOrUpdateLocalParticipant(string playerId, string displayName, bool isReady)
        {
            return localFallbackRuntime.RoomSessionService.AddOrUpdateLocalParticipant(playerId, displayName, isReady);
        }

        public MultiplayerRoomSnapshot SetParticipantConnection(string playerId, bool isConnected)
        {
            return localFallbackRuntime.RoomSessionService.SetParticipantConnection(playerId, isConnected);
        }

        public MultiplayerRoomSnapshot EnsureLocalLobby(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            IReadOnlyList<LocalAuthoritativeMatchParticipant> localParticipants = null)
        {
            return localFallbackRuntime.RoomSessionService.EnsureLocalLobby(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                localParticipants);
        }

        public MultiplayerRoomSnapshot SetParticipantReady(string playerId, bool isReady)
        {
            return localFallbackRuntime.RoomSessionService.SetParticipantReady(playerId, isReady);
        }

        public MultiplayerRoomSnapshot ReturnCurrentRoomToLobby()
        {
            return localFallbackRuntime.RoomSessionService.ReturnCurrentRoomToLobby();
        }

        public MultiplayerRoomSnapshot EnsureHostReady(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            return localFallbackRuntime.RoomSessionService.EnsureHostReady(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
        }

        public MultiplayerRoomSnapshot BeginLocalAuthoritativeRound(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds,
            IReadOnlyList<LocalAuthoritativeMatchParticipant> otherParticipants = null)
        {
            return localFallbackRuntime.RoomSessionService.BeginLocalAuthoritativeRound(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds,
                otherParticipants);
        }

        public MatchCallEvent RecordObservedCall(int calledNumber, long emittedUtcTicks = 0)
        {
            return localFallbackRuntime.MatchAuthorityService.RecordObservedCall(calledNumber, emittedUtcTicks);
        }

        public MatchClaimResolution SubmitBingoClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return localFallbackRuntime.MatchAuthorityService.SubmitBingoClaim(
                playerId,
                cardIndex,
                claimCallIndex,
                markedCellKeys,
                claimedNumbers,
                idempotencyKey);
        }

        public MatchClaimResolution SubmitJackpotStateClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return localFallbackRuntime.MatchAuthorityService.SubmitJackpotStateClaim(
                playerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                idempotencyKey);
        }

        public MatchEndEvent PublishRoundEnd(
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            return localFallbackRuntime.MatchAuthorityService.PublishRoundEnd(decision, wheelspinEntitledPlayerIds);
        }
    }
}

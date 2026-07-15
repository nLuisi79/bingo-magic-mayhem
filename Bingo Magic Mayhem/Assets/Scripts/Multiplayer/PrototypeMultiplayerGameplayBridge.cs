using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Prototype-specific orchestration seam between BingoPrototype gameplay events and
    /// the isolated multiplayer controller. This keeps prototype UI/gameplay roots from
    /// assembling transport/authority calls directly.
    /// </summary>
    public sealed class PrototypeMultiplayerGameplayBridge
    {
        private readonly IMultiplayerRoomSessionService roomSessionService;
        private readonly IMultiplayerMatchAuthorityService matchAuthorityService;
        private readonly string hostPlayerId;

        public PrototypeMultiplayerGameplayBridge(
            IMultiplayerRoomSessionService roomSessionService,
            IMultiplayerMatchAuthorityService matchAuthorityService,
            string hostPlayerId)
        {
            this.roomSessionService = roomSessionService ?? throw new ArgumentNullException(nameof(roomSessionService));
            this.matchAuthorityService = matchAuthorityService ?? throw new ArgumentNullException(nameof(matchAuthorityService));
            this.hostPlayerId = string.IsNullOrEmpty(hostPlayerId) ? "prototype_local_host" : hostPlayerId;
        }

        public void SyncLobby(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            roomSessionService.EnsureHostReady(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
        }

        public void BeginAuthoritativeRound(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            SyncLobby(hostDisplayName, realmIndex, roomIndex, selectedCardCount, manaBetPerCard);
            roomSessionService.BeginLocalAuthoritativeRound(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds);
        }

        public MatchCallEvent TryRecordObservedCall(bool roundIsActive, int calledNumber)
        {
            if (!roundIsActive)
            {
                return null;
            }

            return matchAuthorityService.RecordObservedCall(calledNumber);
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            if (!roundIsActive)
            {
                return null;
            }

            return matchAuthorityService.SubmitBingoClaim(
                hostPlayerId,
                cardIndex,
                claimCallIndex,
                markedCellKeys,
                claimedNumbers,
                BuildBingoClaimIdempotencyKey(cardIndex, newBingoCount, claimCallIndex));
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            if (!roundIsActive)
            {
                return null;
            }

            return matchAuthorityService.SubmitJackpotStateClaim(
                hostPlayerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                BuildJackpotClaimIdempotencyKey(cardIndex, claimCallIndex));
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            if (alreadyPublished)
            {
                return false;
            }

            List<string> wheelspinEntitledPlayerIds = new List<string>();
            if (hostEarnedWheelspin)
            {
                wheelspinEntitledPlayerIds.Add(hostPlayerId);
            }

            matchAuthorityService.PublishRoundEnd(
                decision ?? new BingoMagicMayhem.Rounds.BingoRoundEndDecision(BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None, "", ""),
                wheelspinEntitledPlayerIds);
            return true;
        }

        private static string BuildBingoClaimIdempotencyKey(int cardIndex, int newBingoCount, int claimCallIndex)
        {
            return $"prototype_bingo:{cardIndex}:{newBingoCount}:{claimCallIndex}";
        }

        private static string BuildJackpotClaimIdempotencyKey(int cardIndex, int claimCallIndex)
        {
            return $"prototype_jackpot:{cardIndex}:{claimCallIndex}";
        }
    }
}

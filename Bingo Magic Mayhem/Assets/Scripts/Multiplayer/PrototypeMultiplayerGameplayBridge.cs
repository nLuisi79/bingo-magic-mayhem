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

        public MatchCallEvent TryEmitAuthoritativeCall(bool roundIsActive, long emittedUtcTicks = 0)
        {
            if (!roundIsActive)
            {
                return null;
            }

            return matchAuthorityService.EmitNextAuthoritativeCall(emittedUtcTicks);
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            return TrySubmitBingoClaim(
                roundIsActive,
                hostPlayerId,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            if (!roundIsActive)
            {
                return null;
            }

            string resolvedPlayerId = ResolvePlayerId(playerId);
            return matchAuthorityService.SubmitBingoClaim(
                resolvedPlayerId,
                cardIndex,
                claimCallIndex,
                markedCellKeys,
                claimedNumbers,
                string.IsNullOrEmpty(idempotencyKey)
                    ? BuildBingoClaimIdempotencyKey(resolvedPlayerId, cardIndex, newBingoCount, claimCallIndex)
                    : idempotencyKey);
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            return TrySubmitJackpotStateClaim(
                roundIsActive,
                hostPlayerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            if (!roundIsActive)
            {
                return null;
            }

            string resolvedPlayerId = ResolvePlayerId(playerId);
            return matchAuthorityService.SubmitJackpotStateClaim(
                resolvedPlayerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                string.IsNullOrEmpty(idempotencyKey)
                    ? BuildJackpotClaimIdempotencyKey(resolvedPlayerId, cardIndex, claimCallIndex)
                    : idempotencyKey);
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            IReadOnlyList<string> explicitEntitledPlayerIds = hostEarnedWheelspin
                ? new[] { hostPlayerId }
                : Array.Empty<string>();

            return TryPublishRoundEnd(alreadyPublished, decision, explicitEntitledPlayerIds);
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            if (alreadyPublished)
            {
                return false;
            }

            List<string> resolvedEntitledPlayerIds = new List<string>();
            if (wheelspinEntitledPlayerIds != null)
            {
                for (int index = 0; index < wheelspinEntitledPlayerIds.Count; index++)
                {
                    string resolvedPlayerId = ResolvePlayerId(wheelspinEntitledPlayerIds[index]);
                    if (!resolvedEntitledPlayerIds.Contains(resolvedPlayerId))
                    {
                        resolvedEntitledPlayerIds.Add(resolvedPlayerId);
                    }
                }
            }

            matchAuthorityService.PublishRoundEnd(
                decision ?? new BingoMagicMayhem.Rounds.BingoRoundEndDecision(BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None, "", ""),
                resolvedEntitledPlayerIds);
            return true;
        }

        public MatchEndEvent TryResolvePostRoundHandoff(string resolvedPlayerId)
        {
            if (string.IsNullOrEmpty(resolvedPlayerId))
            {
                return null;
            }

            return matchAuthorityService.ResolvePostRoundHandoff(resolvedPlayerId);
        }

        private string ResolvePlayerId(string playerId)
        {
            return string.IsNullOrEmpty(playerId) ? hostPlayerId : playerId;
        }

        private static string BuildBingoClaimIdempotencyKey(string playerId, int cardIndex, int newBingoCount, int claimCallIndex)
        {
            return $"prototype_bingo:{playerId}:{cardIndex}:{newBingoCount}:{claimCallIndex}";
        }

        private static string BuildJackpotClaimIdempotencyKey(string playerId, int cardIndex, int claimCallIndex)
        {
            return $"prototype_jackpot:{playerId}:{cardIndex}:{claimCallIndex}";
        }
    }
}

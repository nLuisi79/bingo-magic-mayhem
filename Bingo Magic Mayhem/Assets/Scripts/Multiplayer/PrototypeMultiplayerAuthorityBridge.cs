using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Prototype-facing adapter that mirrors current gameplay lifecycle events into the
    /// local authoritative multiplayer simulator without changing existing prototype flow.
    /// </summary>
    public sealed class PrototypeMultiplayerAuthorityBridge
    {
        private readonly LocalAuthoritativeMatchSimulator simulator;

        public PrototypeMultiplayerAuthorityBridge(LocalAuthoritativeMatchSimulator simulator)
        {
            this.simulator = simulator ?? throw new ArgumentNullException(nameof(simulator));
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
            return simulator.CreateRoomAndStartMatch(
                hostPlayerId,
                hostDisplayName,
                otherParticipants,
                new AuthoritativeMatchStartRequest
                {
                    RealmIndex = realmIndex,
                    RoomIndex = roomIndex,
                    SelectedCardCount = selectedCardCount,
                    ManaBetPerCard = manaBetPerCard,
                    RoundSeed = roundSeed,
                    MaxCallCount = maxCallCount,
                    AutoCallIntervalSeconds = autoCallIntervalSeconds
                });
        }

        public MatchCallEvent RecordPrototypeCall(int calledNumber, long emittedUtcTicks = 0)
        {
            return simulator.RecordObservedHostCall(calledNumber, emittedUtcTicks);
        }

        public MatchClaimResolution SubmitPrototypeBingoClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return simulator.SubmitClaim(
                playerId,
                MatchClaimType.Bingo,
                cardIndex,
                claimCallIndex,
                idempotencyKey,
                markedCellKeys,
                claimedNumbers);
        }

        public MatchClaimResolution SubmitPrototypeJackpotStateClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return simulator.SubmitClaim(
                playerId,
                MatchClaimType.JackpotState,
                cardIndex,
                claimCallIndex,
                idempotencyKey,
                claimedNumbers: claimedNumbers);
        }

        public MatchEndEvent PublishPrototypeRoundEnd(BingoMagicMayhem.Rounds.BingoRoundEndDecision decision, IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            return simulator.EndMatch(decision, wheelspinEntitledPlayerIds);
        }

        public LocalAuthoritativeMatchSummary BuildSummary()
        {
            return simulator.BuildSummary();
        }
    }
}

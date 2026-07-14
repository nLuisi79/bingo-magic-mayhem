using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Narrow controller/factory layer that owns local multiplayer room/session setup
    /// for the prototype. This keeps construction and room/session mutations out of
    /// gameplay/UI roots so the backend can later swap behind one seam.
    /// </summary>
    public sealed class PrototypeMultiplayerRoomSessionController : IMultiplayerRoomSessionService, IMultiplayerMatchAuthorityService
    {
        private readonly LocalMultiplayerSessionFacade sessionFacade;
        private readonly LocalAuthoritativeMatchSimulator matchSimulator;
        private readonly PrototypeMultiplayerAuthorityBridge authorityBridge;
        private readonly IMultiplayerRoomSessionSyncAdapter syncAdapter;

        public PrototypeMultiplayerRoomSessionController()
            : this(PrototypeMultiplayerComposition.CreateLocalControllerDependencies())
        {
        }

        public PrototypeMultiplayerRoomSessionController(LocalMultiplayerSessionFacade sessionFacade)
            : this(PrototypeMultiplayerComposition.CreateLocalControllerDependencies(sessionFacade))
        {
        }

        public PrototypeMultiplayerRoomSessionController(LocalMultiplayerSessionFacade sessionFacade, IMultiplayerRoomSessionSyncAdapter syncAdapter)
            : this(PrototypeMultiplayerComposition.CreateLocalControllerDependencies(sessionFacade, syncAdapter))
        {
        }

        public PrototypeMultiplayerRoomSessionController(PrototypeMultiplayerControllerDependencies dependencies)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            sessionFacade = dependencies.SessionFacade;
            matchSimulator = dependencies.MatchSimulator;
            authorityBridge = dependencies.AuthorityBridge;
            syncAdapter = dependencies.SyncAdapter;
        }

        public IMultiplayerSessionFacade SessionFacade => sessionFacade;
        public LocalAuthoritativeMatchSimulator MatchSimulator => matchSimulator;
        public PrototypeMultiplayerAuthorityBridge AuthorityBridge => authorityBridge;
        public IMultiplayerRoomSessionSyncAdapter SyncAdapter => syncAdapter;
        public MultiplayerRoomSnapshot CurrentRoom => sessionFacade.CurrentRoom;

        public MultiplayerRoomSnapshot EnsureLocalRoom(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            MultiplayerRoomSnapshot room = sessionFacade.CurrentRoom;
            if (room == null
                || room.State == MultiplayerRoomLifecycleState.Closed
                || !string.Equals(room.HostPlayerId, hostPlayerId, StringComparison.Ordinal)
                || room.SelectedRealmIndex != realmIndex
                || room.SelectedRoomIndex != roomIndex
                || room.SelectedCardCount != selectedCardCount
                || room.ManaBetPerCard != manaBetPerCard)
            {
                room = sessionFacade.CreateRoom(
                    hostPlayerId,
                    hostDisplayName,
                    realmIndex,
                    roomIndex,
                    selectedCardCount,
                    manaBetPerCard);
            }

            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            return room;
        }

        public MultiplayerRoomSnapshot AddOrUpdateLocalParticipant(string playerId, string displayName, bool isReady)
        {
            sessionFacade.AddLocalParticipant(playerId, displayName);
            MultiplayerRoomSnapshot room = sessionFacade.SetParticipantReady(playerId, isReady);
            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            syncAdapter.PublishReadinessUpdate(BuildReadinessUpdatePayload(playerId));
            return room;
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
            MultiplayerRoomSnapshot room = EnsureHostReady(
                hostPlayerId,
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);

            if (localParticipants != null)
            {
                for (int index = 0; index < localParticipants.Count; index++)
                {
                    LocalAuthoritativeMatchParticipant participant = localParticipants[index];
                    if (participant == null || string.IsNullOrEmpty(participant.PlayerId))
                    {
                        continue;
                    }

                    AddOrUpdateLocalParticipant(participant.PlayerId, participant.DisplayName, participant.Ready);
                }
            }

            return room;
        }

        public MultiplayerRoomSnapshot SetParticipantReady(string playerId, bool isReady)
        {
            MultiplayerRoomSnapshot room = sessionFacade.SetParticipantReady(playerId, isReady);
            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            syncAdapter.PublishReadinessUpdate(BuildReadinessUpdatePayload(playerId));
            return room;
        }

        public MultiplayerRoomSnapshot EnsureHostReady(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            EnsureLocalRoom(hostPlayerId, hostDisplayName, realmIndex, roomIndex, selectedCardCount, manaBetPerCard);
            MultiplayerRoomSnapshot room = sessionFacade.SetParticipantReady(hostPlayerId, true);
            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            syncAdapter.PublishReadinessUpdate(BuildReadinessUpdatePayload(hostPlayerId));
            return room;
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
            MultiplayerRoomSnapshot room = authorityBridge.BeginLocalAuthoritativeRound(
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
            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            syncAdapter.PublishMatchStart(BuildMatchStartPayload());
            return room;
        }

        public MatchCallEvent RecordObservedCall(int calledNumber, long emittedUtcTicks = 0)
        {
            MatchCallEvent callEvent = authorityBridge.RecordPrototypeCall(calledNumber, emittedUtcTicks);
            if (callEvent != null)
            {
                syncAdapter.PublishCallBroadcast(BuildCallBroadcastPayload(callEvent));
            }

            return callEvent;
        }

        public MultiplayerRoomSyncPayload BuildRoomSyncPayload()
        {
            return MultiplayerTransportPayloadFactory.BuildRoomSyncPayload(sessionFacade.CurrentRoom);
        }

        public MultiplayerReadinessUpdatePayload BuildReadinessUpdatePayload(string playerId, long updatedUtcTicks = 0)
        {
            return MultiplayerTransportPayloadFactory.BuildReadinessUpdatePayload(sessionFacade.CurrentRoom, playerId, updatedUtcTicks);
        }

        public MultiplayerMatchStartPayload BuildMatchStartPayload()
        {
            return MultiplayerTransportPayloadFactory.BuildMatchStartPayload(sessionFacade.CurrentMatch);
        }

        public MultiplayerCallBroadcastPayload BuildCallBroadcastPayload(MatchCallEvent callEvent)
        {
            return MultiplayerTransportPayloadFactory.BuildCallBroadcastPayload(callEvent);
        }

        public MatchClaimResolution SubmitBingoClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            syncAdapter.PublishClaimSubmit(BuildBingoClaimSubmitPayload(
                playerId,
                cardIndex,
                claimCallIndex,
                markedCellKeys,
                claimedNumbers,
                idempotencyKey));

            MatchClaimResolution resolution = authorityBridge.SubmitPrototypeBingoClaim(
                playerId,
                cardIndex,
                claimCallIndex,
                markedCellKeys,
                claimedNumbers,
                idempotencyKey);
            syncAdapter.PublishClaimResolution(BuildClaimResolutionPayload(resolution));
            return resolution;
        }

        public MultiplayerClaimSubmitPayload BuildBingoClaimSubmitPayload(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return MultiplayerTransportPayloadFactory.BuildClaimSubmitPayload(
                sessionFacade.CurrentMatch?.MatchId ?? "",
                playerId,
                MatchClaimType.Bingo,
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
            syncAdapter.PublishClaimSubmit(BuildJackpotStateClaimSubmitPayload(
                playerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                idempotencyKey));

            MatchClaimResolution resolution = authorityBridge.SubmitPrototypeJackpotStateClaim(
                playerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                idempotencyKey);
            syncAdapter.PublishClaimResolution(BuildClaimResolutionPayload(resolution));
            return resolution;
        }

        public MultiplayerClaimSubmitPayload BuildJackpotStateClaimSubmitPayload(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey)
        {
            return MultiplayerTransportPayloadFactory.BuildClaimSubmitPayload(
                sessionFacade.CurrentMatch?.MatchId ?? "",
                playerId,
                MatchClaimType.JackpotState,
                cardIndex,
                claimCallIndex,
                null,
                claimedNumbers,
                idempotencyKey);
        }

        public MultiplayerClaimResolutionPayload BuildClaimResolutionPayload(MatchClaimResolution resolution)
        {
            return MultiplayerTransportPayloadFactory.BuildClaimResolutionPayload(resolution);
        }

        public MatchEndEvent PublishRoundEnd(BingoMagicMayhem.Rounds.BingoRoundEndDecision decision, IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            MatchEndEvent endEvent = authorityBridge.PublishPrototypeRoundEnd(decision, wheelspinEntitledPlayerIds);
            syncAdapter.PublishRoomSync(BuildRoomSyncPayload());
            syncAdapter.PublishMatchEnd(BuildMatchEndPayload(endEvent));
            return endEvent;
        }

        public MultiplayerMatchEndPayload BuildMatchEndPayload(MatchEndEvent endEvent)
        {
            return MultiplayerTransportPayloadFactory.BuildMatchEndPayload(endEvent);
        }

        public void ApplyReceivedRoomSync(MultiplayerRoomSyncPayload payload)
        {
            syncAdapter.ApplyReceivedRoomSync(payload);
        }

        public void ApplyReceivedReadinessUpdate(MultiplayerReadinessUpdatePayload payload)
        {
            syncAdapter.ApplyReceivedReadinessUpdate(payload);
        }

        public void ApplyReceivedMatchStart(MultiplayerMatchStartPayload payload)
        {
            syncAdapter.ApplyReceivedMatchStart(payload);
        }

        public void ApplyReceivedCallBroadcast(MultiplayerCallBroadcastPayload payload)
        {
            syncAdapter.ApplyReceivedCallBroadcast(payload);
        }

        public void ApplyReceivedClaimSubmit(MultiplayerClaimSubmitPayload payload)
        {
            syncAdapter.ApplyReceivedClaimSubmit(payload);
        }

        public void ApplyReceivedClaimResolution(MultiplayerClaimResolutionPayload payload)
        {
            syncAdapter.ApplyReceivedClaimResolution(payload);
        }

        public void ApplyReceivedMatchEnd(MultiplayerMatchEndPayload payload)
        {
            syncAdapter.ApplyReceivedMatchEnd(payload);
        }

        public MultiplayerRoomSessionDisplayModel BuildRoomSessionDisplayModel()
        {
            return MultiplayerRoomSessionPresenter.Build(sessionFacade);
        }

        public MultiplayerLobbyDisplayModel BuildLobbyDisplayModel()
        {
            return MultiplayerLobbyPresenter.Build(BuildRoomSessionDisplayModel());
        }

        public LocalAuthoritativeMatchSummary BuildMatchSummary()
        {
            return matchSimulator.BuildSummary();
        }
    }
}

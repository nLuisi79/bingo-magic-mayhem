using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Presentation-facing seam for prototype multiplayer. This consolidates the
    /// UI/gameplay bridge actions and display-model reads so large UI roots do not
    /// couple themselves to runtime construction details.
    /// </summary>
    public sealed class PrototypeMultiplayerPresentationState
    {
        public PrototypeMultiplayerPresentationState(
            MultiplayerLobbyDisplayModel lobby,
            MultiplayerLobbyStatusDisplayModel lobbyStatus,
            MultiplayerRoomSessionDisplayModel session,
            MultiplayerGameplayRoundDisplayModel gameplayRound,
            MultiplayerRoomSessionSnapshot snapshot,
            LocalAuthoritativeMatchSummary matchSummary)
        {
            Lobby = lobby;
            LobbyStatus = lobbyStatus;
            Session = session;
            GameplayRound = gameplayRound;
            Snapshot = snapshot;
            MatchSummary = matchSummary;
        }

        public MultiplayerLobbyDisplayModel Lobby { get; }
        public MultiplayerLobbyStatusDisplayModel LobbyStatus { get; }
        public MultiplayerRoomSessionDisplayModel Session { get; }
        public MultiplayerGameplayRoundDisplayModel GameplayRound { get; }
        public MultiplayerRoomSessionSnapshot Snapshot { get; }
        public LocalAuthoritativeMatchSummary MatchSummary { get; }
    }

    public sealed class PrototypeMultiplayerRoundEndPublishResult
    {
        public PrototypeMultiplayerRoundEndPublishResult(bool wasPublished, PrototypeMultiplayerPresentationState presentationState)
        {
            WasPublished = wasPublished;
            PresentationState = presentationState;
        }

        public bool WasPublished { get; }
        public PrototypeMultiplayerPresentationState PresentationState { get; }
    }

    /// <summary>
    /// Local-only relay helper that advances published room/session sync payloads into
    /// the receive/apply mirror so guest-facing presentation can be exercised without a
    /// live transport layer.
    /// </summary>
    public sealed class PrototypeMultiplayerMirrorRelay
    {
        private readonly IMultiplayerRoomSessionSyncAdapter syncAdapter;
        private int relayedRoomSyncCount;
        private int relayedReadinessCount;
        private int relayedMatchStartCount;
        private int relayedCallBroadcastCount;
        private int relayedClaimSubmitCount;
        private int relayedClaimResolutionCount;
        private int relayedMatchEndCount;

        public PrototypeMultiplayerMirrorRelay(IMultiplayerRoomSessionSyncAdapter syncAdapter)
        {
            this.syncAdapter = syncAdapter ?? throw new ArgumentNullException(nameof(syncAdapter));
        }

        public void RelayUnseenPublishedEvents()
        {
            RelayRange(syncAdapter.RoomSyncLog, ref relayedRoomSyncCount, syncAdapter.ApplyReceivedRoomSync);
            RelayRange(syncAdapter.ReadinessLog, ref relayedReadinessCount, syncAdapter.ApplyReceivedReadinessUpdate);
            RelayRange(syncAdapter.MatchStartLog, ref relayedMatchStartCount, syncAdapter.ApplyReceivedMatchStart);
            RelayRange(syncAdapter.CallBroadcastLog, ref relayedCallBroadcastCount, syncAdapter.ApplyReceivedCallBroadcast);
            RelayRange(syncAdapter.ClaimSubmitLog, ref relayedClaimSubmitCount, syncAdapter.ApplyReceivedClaimSubmit);
            RelayRange(syncAdapter.ClaimResolutionLog, ref relayedClaimResolutionCount, syncAdapter.ApplyReceivedClaimResolution);
            RelayRange(syncAdapter.MatchEndLog, ref relayedMatchEndCount, syncAdapter.ApplyReceivedMatchEnd);
        }

        public void ResetProgress()
        {
            relayedRoomSyncCount = 0;
            relayedReadinessCount = 0;
            relayedMatchStartCount = 0;
            relayedCallBroadcastCount = 0;
            relayedClaimSubmitCount = 0;
            relayedClaimResolutionCount = 0;
            relayedMatchEndCount = 0;
        }

        private static void RelayRange<TPayload>(
            IReadOnlyList<TPayload> source,
            ref int relayedCount,
            Action<TPayload> receiver)
        {
            if (source == null || receiver == null)
            {
                return;
            }

            int safeRelayedCount = Math.Max(0, relayedCount);
            for (int index = safeRelayedCount; index < source.Count; index++)
            {
                receiver(source[index]);
            }

            relayedCount = source.Count;
        }
    }

    public sealed class PrototypeMultiplayerPresentationFacade
    {
        private readonly PrototypeMultiplayerRoomSessionController controller;
        private readonly PrototypeMultiplayerGameplayBridge gameplayBridge;
        private readonly PrototypeMultiplayerMirrorRelay mirrorRelay;

        public PrototypeMultiplayerPresentationFacade(
            PrototypeMultiplayerRoomSessionController controller,
            PrototypeMultiplayerGameplayBridge gameplayBridge)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.gameplayBridge = gameplayBridge ?? throw new ArgumentNullException(nameof(gameplayBridge));
            mirrorRelay = new PrototypeMultiplayerMirrorRelay(this.controller.SyncAdapter);
        }

        public PrototypeMultiplayerRoomSessionController Controller => controller;
        public PrototypeMultiplayerGameplayBridge GameplayBridge => gameplayBridge;

        public void SyncLobby(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            gameplayBridge.SyncLobby(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
        }

        public PrototypeMultiplayerPresentationState SyncLobbyAndBuildState(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            SyncLobby(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
            return BuildPresentationState();
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
            gameplayBridge.BeginAuthoritativeRound(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds);
        }

        public PrototypeMultiplayerPresentationState BeginAuthoritativeRoundAndBuildState(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            BeginAuthoritativeRound(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds);
            return BuildPresentationState();
        }

        public MatchCallEvent TryRecordObservedCall(bool roundIsActive, int calledNumber)
        {
            return gameplayBridge.TryRecordObservedCall(roundIsActive, calledNumber);
        }

        public MatchCallEvent TryEmitAuthoritativeCall(bool roundIsActive, long emittedUtcTicks = 0)
        {
            return gameplayBridge.TryEmitAuthoritativeCall(roundIsActive, emittedUtcTicks);
        }

        public PrototypeMultiplayerPresentationState TryEmitAuthoritativeCallAndBuildState(bool roundIsActive, long emittedUtcTicks = 0)
        {
            TryEmitAuthoritativeCall(roundIsActive, emittedUtcTicks);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerPresentationState TryRecordObservedCallAndBuildState(bool roundIsActive, int calledNumber)
        {
            TryRecordObservedCall(roundIsActive, calledNumber);
            return BuildPresentationState();
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            return gameplayBridge.TrySubmitBingoClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
        }

        public PrototypeMultiplayerPresentationState TrySubmitBingoClaimAndBuildState(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            TrySubmitBingoClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
            return BuildPresentationState();
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            return gameplayBridge.TrySubmitJackpotStateClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
        }

        public PrototypeMultiplayerPresentationState TrySubmitJackpotStateClaimAndBuildState(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            TrySubmitJackpotStateClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
            return BuildPresentationState();
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            return gameplayBridge.TryPublishRoundEnd(
                alreadyPublished,
                decision,
                hostEarnedWheelspin);
        }

        public PrototypeMultiplayerRoundEndPublishResult TryPublishRoundEndAndBuildState(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            bool wasPublished = TryPublishRoundEnd(
                alreadyPublished,
                decision,
                hostEarnedWheelspin);
            return new PrototypeMultiplayerRoundEndPublishResult(wasPublished, BuildPresentationState());
        }

        public MultiplayerLobbyDisplayModel BuildLobbyDisplayModel()
        {
            return controller.BuildLobbyDisplayModel();
        }

        public MultiplayerRoomSessionDisplayModel BuildRoomSessionDisplayModel()
        {
            return controller.BuildRoomSessionDisplayModel();
        }

        public MultiplayerRoomSessionSnapshot BuildSessionSnapshot()
        {
            return controller.BuildSessionSnapshot();
        }

        public int GetCurrentAuthoritativeCallIndex()
        {
            return controller.GetCurrentAuthoritativeCallIndex();
        }

        public void SyncMirroredStateFromPublishedEvents()
        {
            mirrorRelay.RelayUnseenPublishedEvents();
        }

        public MultiplayerRoomSessionSnapshot BuildMirroredSessionSnapshot()
        {
            SyncMirroredStateFromPublishedEvents();
            return MultiplayerRoomSessionSnapshotFactory.Build(controller.SyncAdapter.Mirror);
        }

        public LocalAuthoritativeMatchSummary BuildMatchSummary()
        {
            return controller.BuildMatchSummary();
        }

        public PrototypeMultiplayerPresentationState BuildPresentationState()
        {
            return BuildPresentationStateFromSnapshot(BuildSessionSnapshot(), BuildMatchSummary());
        }

        public PrototypeMultiplayerPresentationState BuildMirroredPresentationState()
        {
            return BuildPresentationStateFromSnapshot(BuildMirroredSessionSnapshot(), null);
        }

        private static PrototypeMultiplayerPresentationState BuildPresentationStateFromSnapshot(
            MultiplayerRoomSessionSnapshot snapshot,
            LocalAuthoritativeMatchSummary matchSummary)
        {
            MultiplayerRoomSessionDisplayModel sessionDisplayModel = MultiplayerRoomSessionPresenter.Build(snapshot);
            MultiplayerLobbyDisplayModel lobbyDisplayModel = MultiplayerLobbyPresenter.Build(sessionDisplayModel);
            return new PrototypeMultiplayerPresentationState(
                lobbyDisplayModel,
                MultiplayerLobbyStatusPresenter.Build(lobbyDisplayModel),
                sessionDisplayModel,
                MultiplayerGameplayRoundPresenter.Build(sessionDisplayModel),
                snapshot,
                matchSummary);
        }
    }
}

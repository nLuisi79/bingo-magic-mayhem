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

    public sealed class PrototypeMultiplayerPresentationFacade
    {
        private readonly PrototypeMultiplayerRoomSessionController controller;
        private readonly PrototypeMultiplayerGameplayBridge gameplayBridge;

        public PrototypeMultiplayerPresentationFacade(
            PrototypeMultiplayerRoomSessionController controller,
            PrototypeMultiplayerGameplayBridge gameplayBridge)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.gameplayBridge = gameplayBridge ?? throw new ArgumentNullException(nameof(gameplayBridge));
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

        public LocalAuthoritativeMatchSummary BuildMatchSummary()
        {
            return controller.BuildMatchSummary();
        }

        public PrototypeMultiplayerPresentationState BuildPresentationState()
        {
            MultiplayerRoomSessionSnapshot snapshot = BuildSessionSnapshot();
            MultiplayerRoomSessionDisplayModel sessionDisplayModel = MultiplayerRoomSessionPresenter.Build(snapshot);
            MultiplayerLobbyDisplayModel lobbyDisplayModel = MultiplayerLobbyPresenter.Build(sessionDisplayModel);
            return new PrototypeMultiplayerPresentationState(
                lobbyDisplayModel,
                MultiplayerLobbyStatusPresenter.Build(lobbyDisplayModel),
                sessionDisplayModel,
                MultiplayerGameplayRoundPresenter.Build(sessionDisplayModel),
                snapshot,
                BuildMatchSummary());
        }
    }
}

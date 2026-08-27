using System.Collections.Generic;
using BingoMagicMayhem.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Thin room-entry presentation shell for runtime-generated lobby content.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryPanelView : MonoBehaviour
    {
        private readonly List<RoomEntryCardOptionView> cardOptionViews = new List<RoomEntryCardOptionView>();
        private Button backButton;
        private RoomEntryBetRowView betRowView;
        private RoomEntryHeaderView headerView;
        private RoomEntryRestorePanelView restorePanelView;
        private RoomEntryJackpotView jackpotView;
        private RoomEntryPanelAnimator animator;
        private Text pickTitleText;
        private Text helperNoteText;

        public void ResetRuntimeBindings()
        {
            cardOptionViews.Clear();
            backButton = null;
            betRowView = null;
            headerView = null;
            restorePanelView = null;
            jackpotView = null;
            animator = null;
            pickTitleText = null;
            helperNoteText = null;
        }

        public void RegisterCardOption(RoomEntryCardOptionView view)
        {
            if (view != null)
            {
                if (!cardOptionViews.Contains(view))
                {
                    cardOptionViews.Add(view);
                }
            }
        }

        public void BindBetRow(RoomEntryBetRowView view)
        {
            betRowView = view;
        }

        public void BindScreenChrome(Button back, Text title, Text helperNote)
        {
            backButton = back;
            pickTitleText = title;
            helperNoteText = helperNote;
        }

        public void BindHeader(RoomEntryHeaderView view)
        {
            headerView = view;
        }

        public void BindRestorePanel(RoomEntryRestorePanelView view)
        {
            restorePanelView = view;
        }

        public void BindJackpotPanel(RoomEntryJackpotView view)
        {
            jackpotView = view;
        }

        public void BindAnimator(RoomEntryPanelAnimator viewAnimator)
        {
            animator = viewAnimator;
        }

        public void SetBetSummary(string summary)
        {
            betRowView?.SetSummary(summary);
        }

        public void ApplyHeader(RoomEntryHeaderDisplayModel displayModel)
        {
            headerView?.Apply(displayModel);
        }

        public void ApplyRestorePanel(RoomEntryRestorePanelDisplayModel displayModel)
        {
            restorePanelView?.Apply(displayModel);
        }

        public void ApplyJackpotPanel(RoomEntryJackpotDisplayModel displayModel)
        {
            jackpotView?.Apply(displayModel);
        }

        public void ApplyScreenChrome(RoomEntryScreenChromeDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (pickTitleText != null)
            {
                pickTitleText.text = displayModel.PickTitleText;
            }

            if (helperNoteText != null)
            {
                helperNoteText.gameObject.SetActive(displayModel.ShowHelperNote);
                helperNoteText.text = displayModel.HelperNoteText;
            }

            if (backButton != null)
            {
                backButton.gameObject.SetActive(displayModel.ShowBackButton);
            }
        }

        public void ShowIdleState()
        {
            animator?.ShowIdleState();
        }
    }

    public sealed class RoomEntryScreenChromeDisplayModel
    {
        public RoomEntryScreenChromeDisplayModel(
            string pickTitleText,
            string helperNoteText,
            bool showHelperNote,
            bool showBackButton)
        {
            PickTitleText = pickTitleText;
            HelperNoteText = helperNoteText;
            ShowHelperNote = showHelperNote;
            ShowBackButton = showBackButton;
        }

        public string PickTitleText { get; }
        public string HelperNoteText { get; }
        public bool ShowHelperNote { get; }
        public bool ShowBackButton { get; }
    }

    public static class PrototypeRoomEntryDisplayFactory
    {
        public static RoomEntryScreenChromeDisplayModel BuildScreenChrome(bool isBlackoutRoom)
        {
            return new RoomEntryScreenChromeDisplayModel(
                isBlackoutRoom ? "Pick Cards for Blackout" : "Pick Cards to Play",
                "Daub every playable square on a card to earn one wheelspin.",
                isBlackoutRoom,
                true);
        }

        public static RoomEntryHeaderDisplayModel BuildHeader(
            string roomName,
            string realmName,
            string potionName,
            string rankText,
            string modeLabel,
            string modeIconText,
            bool isSpecialRoom)
        {
            return new RoomEntryHeaderDisplayModel(
                roomName,
                realmName,
                $"Potion: {potionName}",
                rankText,
                modeLabel.ToUpperInvariant(),
                modeIconText,
                isSpecialRoom);
        }

        public static RoomEntryRestorePanelDisplayModel BuildRestorePanel(
            string potionName,
            string restoreStatusText,
            string ingredientProgressText,
            string restoreRewardText,
            bool activeRoomRestored,
            bool canRestore)
        {
            return new RoomEntryRestorePanelDisplayModel(
                potionName,
                "Collect ingredients while you play bingo.",
                restoreStatusText,
                "Ingredients",
                ingredientProgressText,
                $"Restore Reward\n{restoreRewardText}",
                activeRoomRestored ? "Restored" : "Restore",
                canRestore);
        }

        public static RoomEntryCardOptionDisplayModel BuildCardOption(
            int cardCount,
            int selectedCardCount,
            int manaBetPerCard,
            bool canAfford,
            bool blockedByHandoff,
            bool blockedByReconnect,
            string flavorText,
            string ingredientText,
            string chanceText)
        {
            return RoomEntryCardOptionPresenter.Build(
                cardCount,
                selectedCardCount,
                manaBetPerCard,
                canAfford,
                blockedByHandoff,
                blockedByReconnect,
                flavorText,
                ingredientText,
                chanceText);
        }
    }

    public sealed class PrototypeMultiplayerStatusDisplayModel
    {
        public PrototypeMultiplayerStatusDisplayModel(
            string lobbyTitle,
            string lobbyReadiness,
            Color lobbyReadinessColor,
            string lobbyDetail,
            string lobbyParticipants,
            string gameplayRoom,
            string gameplayAuthority,
            string gameplayClaim,
            string gameplayPostRound)
        {
            LobbyTitle = lobbyTitle ?? "";
            LobbyReadiness = lobbyReadiness ?? "";
            LobbyReadinessColor = lobbyReadinessColor;
            LobbyDetail = lobbyDetail ?? "";
            LobbyParticipants = lobbyParticipants ?? "";
            GameplayRoom = gameplayRoom ?? "";
            GameplayAuthority = gameplayAuthority ?? "";
            GameplayClaim = gameplayClaim ?? "";
            GameplayPostRound = gameplayPostRound ?? "";
        }

        public string LobbyTitle { get; }
        public string LobbyReadiness { get; }
        public Color LobbyReadinessColor { get; }
        public string LobbyDetail { get; }
        public string LobbyParticipants { get; }
        public string GameplayRoom { get; }
        public string GameplayAuthority { get; }
        public string GameplayClaim { get; }
        public string GameplayPostRound { get; }
    }

    public static class PrototypeMultiplayerStatusPresenter
    {
        public static PrototypeMultiplayerStatusDisplayModel Build(PrototypeMultiplayerPresentationState state)
        {
            MultiplayerLobbyStatusDisplayModel lobby = state?.LobbyStatus;
            MultiplayerGameplayRoundDisplayModel gameplay = state?.GameplayRound;
            MultiplayerRoomSessionDisplayModel session = state?.Session;
            PrototypeMultiplayerUiSurfaceModel uiSurface = state?.UiSurface;

            if (lobby == null && gameplay == null)
            {
                return new PrototypeMultiplayerStatusDisplayModel(
                    "",
                    "",
                    new Color(1f, 0.9f, 0.32f),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "");
            }

            Color readinessColor = lobby != null && lobby.CanStart
                ? new Color(0.55f, 1f, 0.85f)
                : new Color(1f, 0.9f, 0.32f);
            if (uiSurface != null && uiSurface.IsReplayBlockedByPostRoundHandoff)
            {
                readinessColor = new Color(1f, 0.72f, 0.32f);
            }
            else if (uiSurface != null && uiSurface.IsWaitingForReconnect)
            {
                readinessColor = new Color(1f, 0.82f, 0.46f);
            }

            string lobbyTitle = uiSurface?.TitleLabel ?? lobby?.TitleSummaryLabel ?? "";
            string lobbyDetail = uiSurface != null
                ? $"{uiSurface.LifecycleSummaryLabel}  |  {uiSurface.ReplayActionLabel}"
                : $"{lobby?.ActionLabel ?? ""} - {lobby?.DetailLabel ?? ""}";
            string lobbyParticipants = uiSurface?.ConnectionSummaryLabel ?? lobby?.ParticipantSummaryLabel ?? "";
            string gameplayRoom = uiSurface != null && session != null
                ? $"{uiSurface.LifecycleSummaryLabel}  |  {session.SelectionSummary}"
                : gameplay?.RoomSummaryLabel ?? "";
            string gameplayAuthority = uiSurface != null && uiSurface.IsWaitingForReconnect
                ? $"{gameplay?.AuthoritySummaryLabel ?? ""}  |  {uiSurface.ConnectionSummaryLabel}"
                : gameplay?.AuthoritySummaryLabel ?? "";
            string gameplayPostRound = uiSurface != null
                ? $"{uiSurface.PostRoundActionLabel}  |  {uiSurface.ReplayActionLabel}"
                : gameplay?.PostRoundSummaryLabel ?? "";

            return new PrototypeMultiplayerStatusDisplayModel(
                lobbyTitle,
                lobby?.ReadinessSummaryLabel ?? "",
                readinessColor,
                lobbyDetail,
                lobbyParticipants,
                gameplayRoom,
                gameplayAuthority,
                gameplay?.ClaimSummaryLabel ?? "",
                gameplayPostRound);
        }
    }

    [DisallowMultipleComponent]
    public sealed class PrototypeMultiplayerStatusView : MonoBehaviour
    {
        [SerializeField] private Text lobbyTitleText;
        [SerializeField] private Text lobbyReadinessText;
        [SerializeField] private Text lobbyDetailText;
        [SerializeField] private Text lobbyParticipantsText;
        [SerializeField] private Text gameplayRoomText;
        [SerializeField] private Text gameplayAuthorityText;
        [SerializeField] private Text gameplayClaimText;
        [SerializeField] private Text gameplayPostRoundText;

        public void ResetRuntimeBindings()
        {
            lobbyTitleText = null;
            lobbyReadinessText = null;
            lobbyDetailText = null;
            lobbyParticipantsText = null;
            gameplayRoomText = null;
            gameplayAuthorityText = null;
            gameplayClaimText = null;
            gameplayPostRoundText = null;
        }

        public void Initialize(
            Text lobbyTitle,
            Text lobbyReadiness,
            Text lobbyDetail,
            Text lobbyParticipants,
            Text gameplayRoom,
            Text gameplayAuthority,
            Text gameplayClaim,
            Text gameplayPostRound)
        {
            lobbyTitleText = lobbyTitle;
            lobbyReadinessText = lobbyReadiness;
            lobbyDetailText = lobbyDetail;
            lobbyParticipantsText = lobbyParticipants;
            gameplayRoomText = gameplayRoom;
            gameplayAuthorityText = gameplayAuthority;
            gameplayClaimText = gameplayClaim;
            gameplayPostRoundText = gameplayPostRound;
        }

        public void Apply(PrototypeMultiplayerStatusDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (lobbyTitleText != null) lobbyTitleText.text = displayModel.LobbyTitle;
            if (lobbyReadinessText != null)
            {
                lobbyReadinessText.text = displayModel.LobbyReadiness;
                lobbyReadinessText.color = displayModel.LobbyReadinessColor;
            }

            if (lobbyDetailText != null) lobbyDetailText.text = displayModel.LobbyDetail;
            if (lobbyParticipantsText != null) lobbyParticipantsText.text = displayModel.LobbyParticipants;
            if (gameplayRoomText != null) gameplayRoomText.text = displayModel.GameplayRoom;
            if (gameplayAuthorityText != null) gameplayAuthorityText.text = displayModel.GameplayAuthority;
            if (gameplayClaimText != null) gameplayClaimText.text = displayModel.GameplayClaim;
            if (gameplayPostRoundText != null) gameplayPostRoundText.text = displayModel.GameplayPostRound;
        }
    }

    public static class PrototypeMultiplayerRoomEntryCoordinator
    {
        public static PrototypeMultiplayerPresentationState SyncLobbyAndBuildState(
            PrototypeMultiplayerPresentationFacade facade,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.SyncLobbyAndBuildState(
                new PrototypeMultiplayerLobbySyncRequest(
                    hostDisplayName,
                    realmIndex,
                    roomIndex,
                    selectedCardCount,
                    manaBetPerCard));
        }

        public static PrototypeMultiplayerPresentationState BeginAuthoritativeRoundAndBuildState(
            PrototypeMultiplayerPresentationFacade facade,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.BeginAuthoritativeRoundAndBuildState(
                new PrototypeMultiplayerRoundStartRequest(
                    hostDisplayName,
                    realmIndex,
                    roomIndex,
                    selectedCardCount,
                    manaBetPerCard,
                    roundSeed,
                    maxCallCount,
                    autoCallIntervalSeconds));
        }
    }

    public static class PrototypeMultiplayerGameplayCoordinator
    {
        public static PrototypeMultiplayerCallResult TryEmitAuthoritativeCallAndBuildResult(
            PrototypeMultiplayerPresentationFacade facade,
            bool roundIsActive,
            long emittedUtcTicks = 0)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.TryEmitAuthoritativeCallAndBuildResult(
                new PrototypeMultiplayerAuthoritativeCallRequest(
                    roundIsActive,
                    emittedUtcTicks));
        }

        public static PrototypeMultiplayerPresentationState TryRecordObservedCallAndBuildState(
            PrototypeMultiplayerPresentationFacade facade,
            bool roundIsActive,
            int calledNumber)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.TryRecordObservedCallAndBuildState(
                new PrototypeMultiplayerObservedCallRequest(
                    roundIsActive,
                    calledNumber));
        }

        public static PrototypeMultiplayerClaimSubmitResult TrySubmitBingoClaimAndBuildResult(
            PrototypeMultiplayerPresentationFacade facade,
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.TrySubmitBingoClaimAndBuildResult(
                new PrototypeMultiplayerBingoClaimRequest(
                    roundIsActive,
                    cardIndex,
                    claimCallIndex,
                    newBingoCount,
                    markedCellKeys,
                    claimedNumbers));
        }

        public static PrototypeMultiplayerClaimSubmitResult TrySubmitJackpotStateClaimAndBuildResult(
            PrototypeMultiplayerPresentationFacade facade,
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.TrySubmitJackpotStateClaimAndBuildResult(
                new PrototypeMultiplayerJackpotClaimRequest(
                    roundIsActive,
                    cardIndex,
                    claimCallIndex,
                    claimedNumbers));
        }

        public static PrototypeMultiplayerRoundEndPublishResult TryPublishRoundEndAndBuildState(
            PrototypeMultiplayerPresentationFacade facade,
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            if (facade == null)
            {
                return null;
            }

            return facade.TryPublishRoundEndAndBuildState(
                new PrototypeMultiplayerRoundEndRequest(
                    alreadyPublished,
                    decision,
                    hostEarnedWheelspin));
        }
    }

    public static class PrototypeMultiplayerPresentationBinding
    {
        public static void Apply(
            PrototypeMultiplayerPresentationState state,
            PrototypeMultiplayerStatusView statusView,
            RoomEntryPanelView roomEntryPanelView,
            string currentPotText,
            string jackpotSpinText,
            int pendingJackpotSpins)
        {
            statusView?.Apply(PrototypeMultiplayerStatusPresenter.Build(state));
            roomEntryPanelView?.ApplyJackpotPanel(BuildJackpotPanel(state, currentPotText, jackpotSpinText, pendingJackpotSpins));
        }

        public static RoomEntryJackpotDisplayModel BuildJackpotPanel(
            PrototypeMultiplayerPresentationState state,
            string currentPotText,
            string jackpotSpinText,
            int pendingJackpotSpins)
        {
            PrototypeMultiplayerUiSurfaceModel uiSurface = state?.UiSurface;
            return RoomEntryJackpotPresenter.Build(
                currentPotText,
                jackpotSpinText,
                uiSurface?.PostRoundActionLabel,
                pendingJackpotSpins,
                uiSurface?.CanResolvePostRoundHandoff ?? false);
        }
    }
}

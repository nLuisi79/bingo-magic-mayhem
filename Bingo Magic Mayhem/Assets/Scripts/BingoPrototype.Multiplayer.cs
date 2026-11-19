using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoMagicMayhem.Cosmetics;
using BingoMagicMayhem.Infrastructure;
using BingoMagicMayhem.Jackpot;
using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using BingoMagicMayhem.UI.Common;
using BingoMagicMayhem.UI.Bazaar;
using BingoMagicMayhem.UI.Collections;
using BingoMagicMayhem.UI.Coven;
using BingoMagicMayhem.UI.Den;
using BingoMagicMayhem.UI.Friends;
using BingoMagicMayhem.UI.Inbox;
using BingoMagicMayhem.UI.Leaderboard;
using BingoMagicMayhem.UI.Market;
using BingoMagicMayhem.UI.Rewards;
using BingoMagicMayhem.UI.RoomEntry;
using BingoMagicMayhem.UI.Trail;
using BingoMagicMayhem.UI.Navigation;
using BingoMagicMayhem.UI.Profile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Prototype multiplayer presentation and the authoritative-round mirror. Values stay prototype-only.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private MultiplayerLobbyDisplayModel BuildPrototypeMultiplayerLobbyDisplayModel()
    {
        if (prototypeMultiplayerPresentationState == null)
        {
            RefreshPrototypeMultiplayerPresentationState();
        }

        return prototypeMultiplayerPresentationState?.Lobby;
    }

    private void RefreshPrototypeMultiplayerPresentationState()
    {
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime?.PresentationFacade?.BuildPresentationState());
    }

    private void ApplyPrototypeMultiplayerPresentationState(PrototypeMultiplayerPresentationState state)
    {
        prototypeMultiplayerPresentationState = state;
        RefreshGameplayMultiplayerStatus();
    }

    private void RefreshLobbyMultiplayerStatus()
    {
        MultiplayerLobbyStatusDisplayModel model = prototypeMultiplayerPresentationState?.LobbyStatus;
        if (model == null)
        {
            BuildPrototypeMultiplayerLobbyDisplayModel();
            model = prototypeMultiplayerPresentationState?.LobbyStatus;
        }

        if (model == null)
        {
            return;
        }

        if (lobbyMultiplayerTitleText != null)
        {
            lobbyMultiplayerTitleText.text = model.TitleSummaryLabel;
        }

        if (lobbyMultiplayerReadinessText != null)
        {
            lobbyMultiplayerReadinessText.text = model.ReadinessSummaryLabel;
            lobbyMultiplayerReadinessText.color = model.CanStart
                ? new Color(0.55f, 1f, 0.85f)
                : new Color(1f, 0.9f, 0.32f);
        }

        if (lobbyMultiplayerDetailText != null)
        {
            lobbyMultiplayerDetailText.text = $"{model.ActionLabel} — {model.DetailLabel}";
        }

        if (lobbyMultiplayerParticipantsText != null)
        {
            lobbyMultiplayerParticipantsText.text = model.ParticipantSummaryLabel;
        }
    }

    private void RefreshGameplayMultiplayerStatus()
    {
        MultiplayerGameplayRoundDisplayModel gameplayModel = prototypeMultiplayerPresentationState?.GameplayRound;
        if (gameplayModel == null)
        {
            return;
        }

        if (gameplayMultiplayerRoomText != null)
        {
            gameplayMultiplayerRoomText.text = gameplayModel.RoomSummaryLabel;
        }

        if (gameplayMultiplayerAuthorityText != null)
        {
            gameplayMultiplayerAuthorityText.text = gameplayModel.AuthoritySummaryLabel;
        }

        if (gameplayMultiplayerClaimText != null)
        {
            gameplayMultiplayerClaimText.text = gameplayModel.ClaimSummaryLabel;
        }

        if (gameplayMultiplayerPostRoundText != null)
        {
            gameplayMultiplayerPostRoundText.text = gameplayModel.PostRoundSummaryLabel;
        }
    }

    private void SyncPrototypeMultiplayerLobbyState()
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime.PresentationFacade.SyncLobbyAndBuildState(
                GetPrototypeMultiplayerHostDisplayName(),
                RealmContentCatalog.ActivePrototypeRealmIndex,
                RealmContentCatalog.ActivePrototypeRoomIndex,
                selectedCardCount,
                manaBetPerCard));
    }

    private void BeginPrototypeAuthoritativeRoundMirror()
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        prototypeRoundEndPublished = false;
        activePrototypeRoundEndDecision = null;
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime.PresentationFacade.BeginAuthoritativeRoundAndBuildState(
                GetPrototypeMultiplayerHostDisplayName(),
                RealmContentCatalog.ActivePrototypeRealmIndex,
                RealmContentCatalog.ActivePrototypeRoomIndex,
                selectedCardCount,
                manaBetPerCard,
                System.Environment.TickCount,
                GetActiveMaxBallCalls(),
                roomRules.AutoCallInterval));
    }

    private void MirrorPrototypeObservedCall(int calledNumber)
    {
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime?.PresentationFacade?.TryRecordObservedCallAndBuildState(roundFlow.IsActive, calledNumber));
    }

    private void MirrorPrototypeBingoClaim(int cardIndex, int newBingoCount)
    {
        PrototypeMultiplayerClaimSubmitResult claimResult =
            prototypeMultiplayerRuntime?.PresentationFacade?.TrySubmitBingoClaimAndBuildResult(
                roundFlow.IsActive,
                cardIndex,
                GetPrototypeClaimCallIndex(),
                newBingoCount,
                BuildMarkedCellKeys(cardIndex),
                BuildClaimedNumbers(cardIndex));
        ApplyPrototypeMultiplayerPresentationState(claimResult?.PresentationState);
        ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(claimResult?.Resolution));
    }

    private void MirrorPrototypeJackpotStateClaim(int cardIndex)
    {
        PrototypeMultiplayerClaimSubmitResult claimResult =
            prototypeMultiplayerRuntime?.PresentationFacade?.TrySubmitJackpotStateClaimAndBuildResult(
                roundFlow.IsActive,
                cardIndex,
                GetPrototypeClaimCallIndex(),
                BuildClaimedNumbers(cardIndex));
        ApplyPrototypeMultiplayerPresentationState(claimResult?.PresentationState);
        ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(claimResult?.Resolution));
    }

    private void ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomeModel outcome)
    {
        if (outcome == null)
        {
            return;
        }

        if (statusText != null && !string.IsNullOrEmpty(outcome.Headline))
        {
            statusText.text = string.IsNullOrEmpty(outcome.Detail)
                ? outcome.Headline
                : $"{outcome.Headline} {outcome.Detail}";
        }

        if (outcome.ShouldCelebrate)
        {
            ShowPowerUpBurst(outcome.Headline.ToUpperInvariant());
        }
    }

    private void ApplyPrototypeRoundEndOutcome(MultiplayerAuthorityOutcomeModel outcome)
    {
        if (outcome == null)
        {
            return;
        }

        if (statusText != null && !string.IsNullOrEmpty(outcome.Detail))
        {
            statusText.text = outcome.Detail;
        }

        if (outcome.ShouldQueueJackpotHandoff)
        {
            ShowPowerUpBurst(outcome.Headline.ToUpperInvariant());
        }
    }

    private void PublishPrototypeRoundEnd(string reason)
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        BingoRoundEndDecision decision = activePrototypeRoundEndDecision
            ?? new BingoRoundEndDecision(BingoRoundEndReasonKind.None, reason, "");
        PrototypeMultiplayerRoundEndPublishResult publishResult =
            prototypeMultiplayerRuntime.PresentationFacade.TryPublishRoundEndAndBuildState(
                prototypeRoundEndPublished,
                decision,
                lastJackpotEarnedCardIndex >= 0);
        prototypeRoundEndPublished = publishResult.WasPublished;
        ApplyPrototypeMultiplayerPresentationState(publishResult.PresentationState);
        ApplyPrototypeRoundEndOutcome(MultiplayerAuthorityOutcomePresenter.BuildRoundEndOutcome(
            publishResult.LatestMatchEnd));
    }

    private string GetPrototypeMultiplayerHostDisplayName()
    {
        return string.IsNullOrWhiteSpace(profileSettingsState.DisplayName) ? "Host" : profileSettingsState.DisplayName;
    }

    private int GetPrototypeClaimCallIndex()
    {
        int authoritativeCallIndex =
            prototypeMultiplayerRuntime?.PresentationFacade?.GetCurrentAuthoritativeCallIndex() ?? -1;
        if (authoritativeCallIndex >= 0)
        {
            return authoritativeCallIndex;
        }

        return Mathf.Max(0, roundFlow.History.Count - 1);
    }

    private List<string> BuildMarkedCellKeys(int cardIndex)
    {
        List<string> keys = new List<string>();
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return keys;
        }

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (playerCards.IsMarked(cardIndex, row, column))
                {
                    keys.Add($"{cardIndex}:{PlayerCardSet.GetCellKey(row, column)}");
                }
            }
        }

        return keys;
    }

    private List<int> BuildClaimedNumbers(int cardIndex)
    {
        List<int> claimedNumbers = new List<int>();
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return claimedNumbers;
        }

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (playerCards.IsMarked(cardIndex, row, column))
                {
                    int number = playerCards.GetNumber(cardIndex, row, column);
                    if (number > 0)
                    {
                        claimedNumbers.Add(number);
                    }
                }
            }
        }

        return claimedNumbers;
    }
}

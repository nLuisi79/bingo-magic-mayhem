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

// Round flow: auto-caller, calling numbers, round completion and end countdowns, room bingo pool and restore.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void TickAutoCaller()
    {
        if (roundEndCoordinator.CountdownActive)
        {
            return;
        }

        if (timerText != null)
        {
            timerText.text = roundFlow.IsActive ? $"{Mathf.Max(0f, roundFlow.NextCallTimer):0.0}s" : "Stopped";
        }

        if (!roundFlow.TickAutoCallTimer(Time.deltaTime))
        {
            return;
        }

        CallNumber();
    }

    private void CallNumber()
    {
        float calledAtTime = Time.time;
        int calledNumber;
        if (prototypeMultiplayerRuntime?.PresentationFacade != null)
        {
            PrototypeMultiplayerCallResult callResult =
                prototypeMultiplayerRuntime.PresentationFacade.TryEmitAuthoritativeCallAndBuildResult(roundFlow.IsActive);
            MatchCallEvent authoritativeCall = callResult?.CallEvent;
            if (authoritativeCall == null
                || !roundFlow.RegisterAuthoritativeCall(authoritativeCall.CalledNumber, calledAtTime))
            {
                StartFinalBallCountdown("No more numbers to call.");
                return;
            }

            calledNumber = authoritativeCall.CalledNumber;
            ApplyPrototypeMultiplayerPresentationState(callResult.PresentationState);
        }
        else if (!roundFlow.TryCallNext(calledAtTime, out calledNumber))
        {
            StartFinalBallCountdown("No more numbers to call.");
            return;
        }

        if (powerUps.ClairvoyanceActive)
        {
            StartCoroutine(TriggerClairvoyanceAlert(calledNumber));
        }

        if (calledNumberText != null)
        {
            calledNumberText.fontSize = 46;
            calledNumberText.text = $"{GetBingoLetter(calledNumber)}-{calledNumber}";
        }

        if (ballsLeftText != null)
        {
            ballsLeftText.text = Mathf.Max(0, GetActiveMaxBallCalls() - roundFlow.History.Count).ToString();
        }

        RefreshCalledBallDisplays();

        if (!IsBlackoutRoom())
        {
            MaybeSimulatedPlayersClaimBingo();
        }

        if (statusText != null)
        {
            statusText.text = HasNumberOnAnyCard(calledNumber)
                ? "Called number is on one of your cards. Daub quickly for bonus XP."
                : "No match across your visible cards. Stay ready.";
        }

        BingoRoundEndDecision callDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            false,
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted. You claimed {GetTotalBingos()}, simulated players claimed {rewards.SimulatedBingosClaimed}.",
            IsStandardCoverageLimitReached(),
            "Standard room coverage limit reached.");
        if (TryStartResolvedRoundEndCountdown(callDecision))
        {
            return;
        }

        if (roundFlow.IsMaxCallLimitReached(GetActiveMaxBallCalls()))
        {
            StartFinalBallCountdown("Max balls reached.");
        }
    }

    private void CompleteRound(string reason)
    {
        finalBallCountdownRoutine = null;
        roundEndCoordinator.CompleteCountdown();
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        roundFlow.StopRound();
        PublishPrototypeRoundEnd(reason);

        if (rewardPreviewShown)
        {
            return;
        }

        rewardPreviewShown = true;
        if (!powerUps.ClairvoyanceActive && HasMissedDaubs())
        {
            allCalledVisible = true;
            RefreshCalledBallDisplays();
        }

        RewardPreview preview = RewardPreview.Build(playerCards, rewards, cellRewards, selectedCardCount, manaBetPerCard, reason, fortuneDoublePrizeActive, placedSigilBonusBingos);
        pendingRewardPreview = preview;
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.RoundCompleted,
            PrototypeAnalyticsPayloadFactory.CreateRoundCompletedJson(
                selectedCardCount,
                manaBetPerCard,
                inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom),
                reason,
                preview));
        if (statusText != null)
        {
            statusText.text = preview.GetStatusLine();
        }

        ShowRewardPreview(preview);
    }

    private void StartFinalBallCountdown(string reason)
    {
        StartRoundEndCountdown(BingoRoundEndRules.CreateFinalBallDecision(reason));
    }

    private void StartBlackoutCompletionCountdown()
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        ShowPowerUpBurst("BLACKOUT COMPLETE");
        StartRoundEndCountdown(BingoRoundEndRules.CreateBlackoutCompleteDecision());
    }

    private void StartRoomPoolCountdown(string reason)
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        StartRoundEndCountdown(BingoRoundEndRules.CreateRoomPoolExhaustedDecision(reason));
    }

    private bool TryStartResolvedRoundEndCountdown(BingoRoundEndDecision decision)
    {
        if (decision == null || !decision.ShouldStartCountdown)
        {
            return false;
        }

        if (decision.Kind == BingoRoundEndReasonKind.BlackoutComplete)
        {
            RefreshRoomPoolTexts();
            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }

            ShowPowerUpBurst("BLACKOUT COMPLETE");
        }
        else if (decision.Kind == BingoRoundEndReasonKind.RoomPoolExhausted)
        {
            RefreshRoomPoolTexts();
            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }
        }

        StartRoundEndCountdown(decision);
        return true;
    }

    private void StartRoundEndCountdown(BingoRoundEndDecision decision)
    {
        if (!roundEndCoordinator.TryBeginCountdown(decision, rewardPreviewShown))
        {
            return;
        }

        activePrototypeRoundEndDecision = decision;
        finalBallCountdownRoutine = StartCoroutine(RoundEndCountdownRoutine(decision));
    }

    private IEnumerator RoundEndCountdownRoutine(BingoRoundEndDecision decision)
    {
        for (int seconds = 5; seconds >= 1; seconds--)
        {
            if (rewardPreviewShown)
            {
                finalBallCountdownRoutine = null;
                roundEndCoordinator.CancelCountdown();
                yield break;
            }

            string latestCallLabel = roundFlow.History.Count > 0
                ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}"
                : "";
            BingoRoundCountdownDisplayState countdownDisplay = BingoRoundCountdownPresenter.Build(seconds, latestCallLabel, decision.CountdownMessage);

            if (timerText != null)
            {
                timerText.text = countdownDisplay.TimerLabel;
            }

            if (calledNumberText != null)
            {
                calledNumberText.fontSize = 30;
                calledNumberText.text = countdownDisplay.CalledNumberLabel;
            }

            if (ballsLeftText != null)
            {
                ballsLeftText.text = countdownDisplay.BallsLeftLabel;
            }

            if (gameplayPowerUpBurstText != null)
            {
                gameplayPowerUpBurstText.text = countdownDisplay.BurstLabel;
                gameplayPowerUpBurstText.color = new Color(0.72f, 0.95f, 1f, 1f);
                gameplayPowerUpBurstText.gameObject.SetActive(true);
            }

            if (statusText != null)
            {
                statusText.text = countdownDisplay.StatusLabel;
            }

            yield return new WaitForSeconds(1f);
        }

        finalBallCountdownRoutine = null;
        roundEndCoordinator.CompleteCountdown();
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        if (!rewardPreviewShown)
        {
            CompleteRound(decision.Reason);
        }
    }

    private void StopFinalBallCountdown()
    {
        if (finalBallCountdownRoutine != null)
        {
            StopCoroutine(finalBallCountdownRoutine);
        }

        finalBallCountdownRoutine = null;
        roundEndCoordinator.CancelCountdown();
    }

    private void StopRoundAndClearCalledHistory()
    {
        roundFlow.ResetSession();
    }

    private int GetActiveMaxBallCalls()
    {
        if (IsBlackoutRoom())
        {
            return roomRules.GetBlackoutMaxBallCalls(selectedCardCount, manaBetPerCard);
        }

        int baseMaxCalls = roomRules.GetMaxBallCalls(selectedCardCount, manaBetPerCard);
        return Mathf.Max(24, baseMaxCalls - GetPowerUpAssistedCallPenalty());
    }

    private int GetPowerUpAssistedCallPenalty()
    {
        if (selectedCardCount <= 0 || powerUpAssistedMarks <= 0)
        {
            return 0;
        }

        int averageAssistedMarks = Mathf.CeilToInt((float)powerUpAssistedMarks / selectedCardCount);
        return Mathf.Clamp(averageAssistedMarks * 2, 0, 18);
    }

    private bool IsStandardCoverageLimitReached()
    {
        int totalMarked = 0;
        for (int index = 0; index < playerCards.Count; index++)
        {
            totalMarked += playerCards.CountMarkedPlayable(index);
        }

        return BingoRoundEndRules.IsStandardCoverageLimitReached(
            IsBlackoutRoom(),
            playerCards.Count,
            totalMarked,
            selectedCardCount);
    }

    private bool TryStartStandardCoverageCountdown(string reason)
    {
        if (!IsBlackoutRoom() && IsStandardCoverageLimitReached())
        {
            StartRoundEndCountdown(BingoRoundEndRules.CreateStandardCoverageDecision(reason));
            return true;
        }

        return false;
    }

    private void RestoreActiveRoom()
    {
        if (!IsRoomUnlocked(RealmContentCatalog.ActivePrototypeRoomIndex))
        {
            return;
        }

        if (!inventory.TryRestoreActiveRoom())
        {
            return;
        }

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.RoomRestored,
            PrototypeAnalyticsPayloadFactory.CreateRoomRestoredJson(
                selectedCardCount,
                manaBetPerCard,
                RealmContentCatalog.ActivePrototypeRoom.Progression.RestoreManaReward));

        int restoredRealmIndex = RealmContentCatalog.ActivePrototypeRealmIndex;
        if (IsRealmComplete(restoredRealmIndex))
        {
            // A newly completed realm always begins at the restored-open state.
            DurablePrefs.DeleteKey(WorldMapRestoredClosedSavePrefix + restoredRealmIndex);
            DurablePrefs.Save();
            worldMapRestoringRealmIndex = restoredRealmIndex;
            BuildWorldMapUi();
            return;
        }

        BuildRealmMapUi();
    }

    private void MaybeSimulatedPlayersClaimBingo()
    {
        if (!rewards.ShouldSimulatedPlayerClaim(roundFlow.History.Count))
        {
            return;
        }

        int simulatedClaimCount = roundFlow.History.Count >= 32 ? 2 : 1;
        ConsumeRoomBingos(simulatedClaimCount, true);
        rewards.ScheduleNextSimulatedClaim();
    }

    private void ConsumeRoomBingos(int count, bool simulated)
    {
        rewards.ConsumeRoomBingos(count, simulated);
        RefreshRoomPoolTexts();
    }

    private void RefreshRoomPoolTexts()
    {
        if (roomBingoCountText != null)
        {
            roomBingoCountText.text = IsBlackoutRoom()
                ? $"{GetTotalBingos()}/{selectedCardCount}"
                : $"{rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}";
        }

        if (roundSummaryText != null)
        {
            roundSummaryText.text = GetRoomPoolSummaryText();
        }
    }
}

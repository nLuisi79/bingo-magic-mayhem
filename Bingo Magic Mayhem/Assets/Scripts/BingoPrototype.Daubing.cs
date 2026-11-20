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

// Daubing and Bingo checks: daub handling and XP, Clairvoyance alerts, winning lines, called-ball history and missed daubs.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void ShowCard(int cardIndex)
    {
        SaveCurrentCard();
        currentCardIndex = Mathf.Clamp(cardIndex, 0, selectedCardCount - 1);
        LoadCurrentCard();
        BuildUi();
    }

    private void DaubVisibleCardCell(int cardIndex, int row, int column)
    {
        if (row == 2 && column == 2)
        {
            statusText.text = "FREE space is already yours.";
            return;
        }

        int value = playerCards.GetNumber(cardIndex, row, column);
        if (playerCards.IsMarked(cardIndex, row, column))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} is already daubed on Card {cardIndex + 1}.";
            return;
        }

        if (wildSigilModeActive)
        {
            wildSigilModeActive = false;
            ApplyPowerUpMarks("Wild Sigil", new List<Vector3Int> { new Vector3Int(cardIndex, row, column) });
            if (roundFlow.IsActive && !rewardPreviewShown)
            {
                SelectNextBankPowerUp("Next power-up charging");
            }

            return;
        }

        string key = $"{cardIndex}:{row}:{column}";
        if (!roundFlow.IsNumberCalled(value))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} has not been called yet.";
            if (visibleCardCells.TryGetValue(key, out Button wrongButton))
            {
                StartCoroutine(FlashCell(wrongButton.transform, new Color(0.95f, 0.22f, 0.26f), new Color(0.95f, 0.97f, 1f)));
            }
            return;
        }

        int previousBingoCount = playerCards.GetBingoCount(cardIndex);
        int daubXp = GetDaubXp(value, key);
        string xpReason = powerUps.IsCellAlerted(key) ? "Clairvoyance daub" : "Daub";

        playerCards.Mark(cardIndex, row, column);
        powerUps.ClearCellAlert(key);
        string collectedRewardText = "";
        if (cellRewards.TryCollect(key, out CellReward collectedReward))
        {
            string displayQuantity = CellRewardTracker.GetDisplayQuantity(collectedReward);
            collectedRewardText = $" Collected {collectedReward.Name} {displayQuantity}.";
            if (collectedReward.Kind == CellRewardKind.Crystals)
            {
                ShowPowerUpBurst($"CRYSTALS {displayQuantity}");
            }
        }
        string placedPowerUpText = TriggerPlacedPowerUpSigil(key, cardIndex);
        UpdateCardBingoProgress(cardIndex);
        AddXp(daubXp, xpReason);
        TrackPowerUpReleaseFromDaub();

        if (visibleCardCells.TryGetValue(key, out Button button))
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetVisibleCellColor(true, playerCards.IsWinningCell(cardIndex, row, column));
            }

            RefreshVisibleCellLabel(button.transform, cardIndex, row, column);
            RemoveRewardBadge(button.transform);
            RemovePlacedPowerUpBadge(button.transform);
            StartCoroutine(PulseCell(button.transform));
        }

        RefreshVisibleCardWinningCells(cardIndex);

        int newBingoCount = playerCards.GetBingoCount(cardIndex);
        if (newBingoCount > previousBingoCount)
        {
            MirrorPrototypeBingoClaim(cardIndex, newBingoCount);
            int bingoDelta = newBingoCount - previousBingoCount;
            int claimedCount = IsBlackoutRoom() ? 1 : bingoDelta;
            int bingoBonus = claimedCount * GetScaledXp(roomRules.BingoClaimXp);
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.BingoClaimed,
                PrototypeAnalyticsPayloadFactory.CreateBingoClaimedJson(
                    selectedCardCount,
                    manaBetPerCard,
                    inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom),
                    cardIndex + 1,
                    bingoDelta,
                    claimedCount,
                    rewards.RoomBingosClaimed,
                    roomRules.RoomBingoPool,
                    IsBlackoutRoom(),
                    "daub"));
            if (!IsBlackoutRoom())
            {
                ConsumeRoomBingos(claimedCount, false);
            }
            else
            {
                RefreshRoomPoolTexts();
            }

            AddXp(bingoBonus, "Auto-Claim Bingo");

            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }

            if (newBingoCount >= BingoRoomRules.JackpotBingosPerCard)
            {
                string earnedLabel = IsBlackoutRoom() ? "BLACKOUT WHEELSPIN EARNED" : "JACKPOT WHEELSPIN EARNED";
                string reachedLabel = IsBlackoutRoom() ? "completed blackout" : "reached 5/5";
                lastJackpotEarnedCardIndex = cardIndex;
                roundFlow.MarkJackpotState();
                MirrorPrototypeJackpotStateClaim(cardIndex);
                int jackpotStateXp = GetScaledXp(roomRules.JackpotStateXp);
                AddXp(jackpotStateXp, "Jackpot State");
                BingoRoundEndDecision jackpotDecision = BingoRoundEndRules.ResolvePostProgressDecision(
                    IsBlackoutRoom(),
                    AllCardsReachedJackpot(),
                    IsPrototypeBingoPoolExhausted(),
                    $"Room bingo pool exhausted. Jackpot reached on Card {cardIndex + 1}.",
                    false,
                    "");
                if (TryStartResolvedRoundEndCountdown(jackpotDecision))
                {
                    return;
                }

                if (bingoBannerText != null)
                {
                    bingoBannerText.text = GetBingoBannerText();
                }

                statusText.text = $"{earnedLabel}! Card {cardIndex + 1} {reachedLabel}. +{daubXp + bingoBonus + jackpotStateXp} XP.{collectedRewardText}{placedPowerUpText}";
                return;
            }

            BingoRoundEndDecision bingoProgressDecision = BingoRoundEndRules.ResolvePostProgressDecision(
                IsBlackoutRoom(),
                false,
                IsPrototypeBingoPoolExhausted(),
                $"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
                IsStandardCoverageLimitReached(),
                $"Standard room coverage limit reached after Card {cardIndex + 1} bingo.");
            if (TryStartResolvedRoundEndCountdown(bingoProgressDecision))
            {
                return;
            }

            statusText.text = IsBlackoutRoom()
                ? $"Card {cardIndex + 1} blackout complete. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}"
                : $"Card {cardIndex + 1} auto-claimed {newBingoCount}/{BingoRoomRules.JackpotBingosPerCard}. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}";
            return;
        }

        statusText.text = IsBlackoutRoom()
            ? $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. Blackout {playerCards.CountMarkedPlayable(cardIndex)}/{BingoRoomRules.BlackoutPlayableSquares}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}"
            : $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}";
        BingoRoundEndDecision daubDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            false,
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
            IsStandardCoverageLimitReached(),
            $"Standard room coverage limit reached after {GetBingoLetter(value)}-{value}.");
        TryStartResolvedRoundEndCountdown(daubDecision);
    }

    private string TriggerPlacedPowerUpSigil(string key, int cardIndex)
    {
        if (!placedPowerUpSigils.TryGetValue(key, out string powerUpName))
        {
            return "";
        }

        placedPowerUpSigils.Remove(key);

        if (powerUpName == "Fortune Sigil")
        {
            fortuneDoublePrizeActive = true;
            ShowPowerUpBurst("FORTUNE x2 ACTIVE");
            SetGameplayPowerUpEventText("Fortune x2 triggered");
            RefreshPowerUpDisplays();
            return " Fortune Sigil triggered: round mana winnings are x2.";
        }

        if (powerUpName == "Wild Sigil")
        {
            wildSigilModeActive = true;
            ShowPowerUpBurst("WILD MODE: TAP 1 CELL");
            SetGameplayPowerUpEventText("Wild triggered");
            RefreshPowerUpDisplays();
            return " Wild Sigil triggered: tap any open square.";
        }

        if (powerUpName == "Arcane Spark")
        {
            ApplyRandomMarksAcrossCards(powerUpName, GetArcaneSparkMarkCount());
            ShowPowerUpBurst($"ARCANE SPARK +{GetArcaneSparkMarkCount()}");
            SetGameplayPowerUpEventText("Spark triggered");
            return " Arcane Spark triggered: lightning daubs released.";
        }

        if (powerUpName == "Presto Sigil")
        {
            if (!IsBlackoutRoom())
            {
                placedSigilBonusBingos++;
                ConsumeRoomBingos(1, false);
                AddXp(GetScaledXp(roomRules.BingoClaimXp), "Presto Bonus Bingo");
                if (bingoBannerText != null)
                {
                    bingoBannerText.text = GetBingoBannerText();
                }
            }

            ShowPowerUpBurst(IsBlackoutRoom() ? "PRESTO HIT" : "PRESTO +1 BINGO");
            RefreshPowerUpDisplays();
            return $" Presto Sigil triggered on Card {cardIndex + 1}: +1 bonus bingo.";
        }

        if (powerUpName == "Pandora Sigil")
        {
            List<string> granted = inventory.OpenPandoraPowerUps();
            string grantedText = granted.Count > 0 ? GetCompactPowerUpGrantSummary(granted) : "no power-ups";
            ShowPowerUpBurst($"PANDORA +{granted.Count} REWARDS");
            SetGameplayPowerUpEventText("Pandora opened");
            RefreshPowerUpDisplays();
            return $" Pandora Sigil opened: +{grantedText}.";
        }

        return $" {powerUpName} triggered.";
    }

    private int GetDaubXp(int value, string visibleKey)
    {
        if (!roundFlow.TryGetCalledAtTime(value, out float calledAt))
        {
            return GetScaledXp(roomRules.NormalDaubXp);
        }

        float daubDelay = Time.time - calledAt;
        return GetScaledXp(roomRules.GetDaubXp(daubDelay, powerUps.IsCellAlerted(visibleKey)));
    }

    private int GetScaledXp(int baseAmount)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseAmount * GetActiveProgression().XpMultiplier));
    }

    private void AddXp(int amount, string reason)
    {
        if (amount <= 0)
        {
            return;
        }

        rewards.AddXp(amount);
        if (xpText != null)
        {
            xpText.text = BuildGameplayRankText();
        }
    }

    private IEnumerator TriggerClairvoyanceAlert(int calledNumber)
    {
        yield return new WaitForSeconds(roomRules.GetClairvoyanceAlertDelay(selectedCardCount));

        if (!roundFlow.IsActive || !powerUps.ClairvoyanceActive || !roundFlow.IsNumberCalled(calledNumber))
        {
            yield break;
        }

        bool alerted = false;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (row == 2 && column == 2)
                    {
                        continue;
                    }

                    if (playerCards.GetNumber(cardIndex, row, column) != calledNumber || playerCards.IsMarked(cardIndex, row, column))
                    {
                        continue;
                    }

                    string visibleKey = $"{cardIndex}:{row}:{column}";
                    if (!visibleCardCells.TryGetValue(visibleKey, out Button button))
                    {
                        continue;
                    }

                    powerUps.AlertCell(visibleKey);
                    StartCoroutine(FlashClairvoyanceCell(button.transform, cardIndex, row, column));
                    alerted = true;
                }
            }
        }

        if (alerted && statusText != null)
        {
            statusText.text = "Clairvoyance reveals a called number. Daub it to keep earning XP.";
        }
    }

    private string GetBingoBannerText()
    {
        List<string> parts = new List<string>();
        if (lastJackpotEarnedCardIndex >= 0)
        {
            parts.Add(IsBlackoutRoom()
                ? $"BLACKOUT WHEELSPIN EARNED: CARD {lastJackpotEarnedCardIndex + 1}"
                : $"WHEELSPIN EARNED: CARD {lastJackpotEarnedCardIndex + 1}");
        }

        for (int index = 0; index < playerCards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = playerCards.CountMarkedPlayable(index);
                if (marked <= 0)
                {
                    continue;
                }

                parts.Add(marked >= BingoRoomRules.BlackoutPlayableSquares
                    ? $"Card {index + 1}: BLACKOUT"
                    : $"Card {index + 1}: {marked}/{BingoRoomRules.BlackoutPlayableSquares}");
                continue;
            }

            int count = playerCards.GetBingoCount(index);
            if (count <= 0)
            {
                continue;
            }

            parts.Add(count >= BingoRoomRules.JackpotBingosPerCard
                ? $"Card {index + 1}: JACKPOT"
                : $"Card {index + 1}: {count}/{BingoRoomRules.JackpotBingosPerCard}");
        }

        return parts.Count == 0 ? "" : string.Join("  |  ", parts);
    }

    private bool AllCardsReachedJackpot()
    {
        return playerCards.AllCardsReachedJackpot();
    }

    private bool IsPrototypeBingoPoolExhausted()
    {
        return rewards.IsRoomPoolExhausted;
    }

    private bool HasNumberOnAnyCard(int calledNumber)
    {
        return playerCards.HasNumberOnAnyCard(calledNumber);
    }

    private void UpdateCardBingoProgress(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return;
        }

        if (IsBlackoutRoom())
        {
            playerCards.RefreshBlackoutProgress(cardIndex);
        }
        else
        {
            playerCards.RefreshBingoProgress(cardIndex);
        }

        if (visibleCardBingoTexts.TryGetValue(cardIndex, out Text bingoText))
        {
            bingoText.text = GetCardBingoText(cardIndex);
        }

        if (roundSummaryText != null)
        {
            roundSummaryText.text = GetRoomPoolSummaryText();
        }
    }

    private void RefreshVisibleCardWinningCells(int cardIndex)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                string visibleKey = $"{cardIndex}:{row}:{column}";
                if (!visibleCardCells.TryGetValue(visibleKey, out Button button))
                {
                    continue;
                }

                bool isMarked = playerCards.IsMarked(cardIndex, row, column);
                bool isWinningCell = playerCards.IsWinningCell(cardIndex, row, column);
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = GetVisibleCellColor(isMarked, isWinningCell);
                }

                Text label = GetCellNumberLabel(button.transform);
                if (label != null)
                {
                    int value = playerCards.GetNumber(cardIndex, row, column);
                    label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
                    label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
                }
            }
        }
    }

    private void HighlightWinningCard(int cardIndex)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (!playerCards.IsMarked(cardIndex, row, column))
                {
                    continue;
                }

                string key = $"{cardIndex}:{row}:{column}";
                if (!visibleCardCells.TryGetValue(key, out Button button))
                {
                    continue;
                }

                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(1f, 0.72f, 0.12f);
                }

                Text label = GetCellNumberLabel(button.transform);
                if (label != null)
                {
                    RefreshVisibleCellLabel(button.transform, cardIndex, row, column);
                }

                StartCoroutine(PulseCell(button.transform));
            }
        }
    }

    private IEnumerator PulseCell(Transform cellTransform)
    {
        Image image = cellTransform.GetComponent<Image>();
        Color settleColor = image != null ? image.color : new Color(0.19f, 0.78f, 0.45f);
        Color flashColor = new Color(1f, 0.78f, 0.18f);
        Vector3 baseScale = cellTransform.localScale;
        Vector3 pulseScale = baseScale * 1.22f;

        if (image != null)
        {
            image.color = flashColor;
        }

        float elapsed = 0f;
        const float pulseUpDuration = 0.14f;
        while (elapsed < pulseUpDuration)
        {
            elapsed += Time.deltaTime;
            cellTransform.localScale = Vector3.Lerp(baseScale, pulseScale, elapsed / pulseUpDuration);
            yield return null;
        }

        elapsed = 0f;
        const float settleDuration = 0.22f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            cellTransform.localScale = Vector3.Lerp(pulseScale, baseScale, elapsed / settleDuration);
            if (image != null)
            {
                image.color = Color.Lerp(flashColor, settleColor, elapsed / settleDuration);
            }
            yield return null;
        }

        cellTransform.localScale = baseScale;
        if (image != null)
        {
            image.color = settleColor;
        }
    }

    private void ToggleCell(int row, int column)
    {
        if (row == 2 && column == 2)
        {
            statusText.text = "FREE space is already yours.";
            return;
        }

        int value = numbers[row, column];
        if (marked[row, column])
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} is already daubed.";
            return;
        }

        if (!roundFlow.IsNumberCalled(value))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} has not been called yet.";
            StartCoroutine(FlashCell(cells[row, column].transform, new Color(0.95f, 0.22f, 0.26f), new Color(0.95f, 0.97f, 1f)));
            return;
        }

        marked[row, column] = true;
        SaveCurrentCard();
        UpdateCell(row, column);
        StartCoroutine(PulseCell(cells[row, column].transform));

        if (!roundFlow.HasJackpotState && HasWinningLine())
        {
            roundFlow.MarkJackpotState();
            roundFlow.StopRound();
            statusText.text = $"BINGO on Card {currentCardIndex + 1}! Reward preview: {selectedCardCount * manaBetPerCard} Mana entry cleared.";
            return;
        }

        statusText.text = $"{GetBingoLetter(value)}-{value} daubed. Keep going.";
    }

    private void UpdateCell(int row, int column)
    {
        Image image = cells[row, column].GetComponent<Image>();
        image.color = marked[row, column] ? new Color(0.19f, 0.78f, 0.45f) : new Color(0.95f, 0.97f, 1f);

        Text label = cells[row, column].GetComponentInChildren<Text>();
        label.color = marked[row, column] ? Color.white : new Color(0.08f, 0.1f, 0.16f);
    }

    private bool HasWinningLine()
    {
        for (int index = 0; index < BoardSize; index++)
        {
            if (IsRowMarked(index) || IsColumnMarked(index))
            {
                return true;
            }
        }

        return IsDiagonalMarked(true) || IsDiagonalMarked(false);
    }

    private bool HasWinningLine(int cardIndex)
    {
        bool[,] marks = playerCards.Marks[cardIndex];

        for (int index = 0; index < BoardSize; index++)
        {
            bool rowComplete = true;
            bool columnComplete = true;

            for (int offset = 0; offset < BoardSize; offset++)
            {
                rowComplete &= marks[index, offset];
                columnComplete &= marks[offset, index];
            }

            if (rowComplete || columnComplete)
            {
                return true;
            }
        }

        bool leftToRight = true;
        bool rightToLeft = true;
        for (int index = 0; index < BoardSize; index++)
        {
            leftToRight &= marks[index, index];
            rightToLeft &= marks[index, BoardSize - 1 - index];
        }

        return leftToRight || rightToLeft;
    }

    private bool IsRowMarked(int row)
    {
        for (int column = 0; column < BoardSize; column++)
        {
            if (!marked[row, column])
            {
                return false;
            }
        }

        return true;
    }

    private bool IsColumnMarked(int column)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            if (!marked[row, column])
            {
                return false;
            }
        }

        return true;
    }

    private bool IsDiagonalMarked(bool leftToRight)
    {
        for (int index = 0; index < BoardSize; index++)
        {
            int column = leftToRight ? index : BoardSize - 1 - index;
            if (!marked[index, column])
            {
                return false;
            }
        }

        return true;
    }

    private string GetBingoLetter(int number)
    {
        if (number <= 15) return "B";
        if (number <= 30) return "I";
        if (number <= 45) return "N";
        if (number <= 60) return "G";
        return "O";
    }

    private string GetCalledHistoryText()
    {
        int count = Mathf.Min(roundFlow.History.Count, 8);
        List<string> recentCalls = new List<string>();

        for (int index = 0; index < count; index++)
        {
            int number = roundFlow.History[index];
            recentCalls.Add($"{GetBingoLetter(number)}-{number}");
        }

        return string.Join("  ", recentCalls);
    }

    private string GetCalledHistoryPanelText()
    {
        if (roundFlow.History.Count == 0)
        {
            return allCalledVisible ? "No calls yet." : "Hidden";
        }

        if (!allCalledVisible)
        {
            return "Tap Show to reveal the full call history.";
        }

        string missedSummary = GetMissedDaubSummaryText();
        if (missedSummary.Length == 0)
        {
            return GetAllCalledHistoryText();
        }

        return $"<color=#ffcc33><b>MISSED DAUBS</b></color>\n{missedSummary}\n\nALL CALLS\n{GetAllCalledHistoryText()}";
    }

    private string GetAllCalledHistoryText()
    {
        List<string> calls = new List<string>();
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            int number = roundFlow.History[index];
            string call = $"{GetBingoLetter(number)}-{number}";
            calls.Add(IsMissedDaubNumber(number) ? $"<color=#ffcc33><b>{call}</b></color>" : call);
        }

        return string.Join("  ", calls);
    }

    private int GetCalledHistoryPanelFontSize()
    {
        if (!allCalledVisible)
        {
            return 18;
        }

        return HasMissedDaubs() ? 14 : 15;
    }

    private void ToggleAllCalledVisible()
    {
        allCalledVisible = !allCalledVisible;
        BuildUi();
    }

    private void RefreshCalledBallDisplays()
    {
        if (calledNumberText != null)
        {
            calledNumberText.fontSize = 46;
            calledNumberText.text = roundFlow.History.Count > 0 ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}" : "Starting";
        }

        if (calledHistoryText != null)
        {
            calledHistoryText.text = GetCalledHistoryPanelText();
            calledHistoryText.fontSize = GetCalledHistoryPanelFontSize();
        }

        for (int index = 0; index < callQueueTexts.Count; index++)
        {
            callQueueTexts[index].text = GetCallQueueLabel(index);
        }
    }

    private string GetCallQueueLabel(int index)
    {
        int historyIndex = index + 1;
        if (roundFlow.History.Count <= historyIndex)
        {
            return "-";
        }

        int number = roundFlow.History[historyIndex];
        return $"{GetBingoLetter(number)}-{number}";
    }

    private bool HasMissedDaubs()
    {
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            if (IsMissedDaubNumber(roundFlow.History[index]))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMissedDaubNumber(int number)
    {
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (row == 2 && column == 2)
                    {
                        continue;
                    }

                    if (playerCards.GetNumber(cardIndex, row, column) == number && !playerCards.IsMarked(cardIndex, row, column))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private string GetMissedDaubSummaryText()
    {
        List<string> missed = new List<string>();
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            int number = roundFlow.History[index];
            if (!IsMissedDaubNumber(number))
            {
                continue;
            }

            missed.Add($"{GetBingoLetter(number)}-{number} {GetMissedDaubCardText(number)}");
            if (missed.Count >= 6)
            {
                break;
            }
        }

        if (missed.Count == 0)
        {
            return "";
        }

        string suffix = CountMissedDaubNumbers() > missed.Count ? "\nMore missed calls highlighted below." : "";
        return string.Join("\n", missed) + suffix;
    }

    private string GetMissedDaubCardText(int number)
    {
        List<string> cards = new List<string>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            if (CardHasUnmarkedCalledNumber(cardIndex, number))
            {
                cards.Add((cardIndex + 1).ToString());
            }
        }

        return cards.Count == 0 ? "" : $"Card {string.Join(",", cards)}";
    }

    private bool CardHasUnmarkedCalledNumber(int cardIndex, int number)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (row == 2 && column == 2)
                {
                    continue;
                }

                if (playerCards.GetNumber(cardIndex, row, column) == number && !playerCards.IsMarked(cardIndex, row, column))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CountMissedDaubNumbers()
    {
        int count = 0;
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            if (IsMissedDaubNumber(roundFlow.History[index]))
            {
                count++;
            }
        }

        return count;
    }
}

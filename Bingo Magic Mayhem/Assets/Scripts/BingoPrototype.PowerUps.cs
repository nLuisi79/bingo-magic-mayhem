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

// Gameplay power-ups: Clairvoyance activation, the power-up bank, effects, sigils and marking.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void ActivatePrototypeClairvoyance()
    {
        if (!inventory.TryActivateClairvoyance())
        {
            return;
        }

        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        BuildLobbyUi();
    }

    private void ActivatePrototypeClairvoyanceFromInventory()
    {
        if (!inventory.TryActivateClairvoyance())
        {
            return;
        }

        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        RefreshPowerUpDisplays();
        BuildPowerUpInventoryUi();
    }

    private string GetPowerUpBankButtonText()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "BANK\n--";
        }

        if (!IsPowerUpBankReady())
        {
            return $"{GetPowerUpBadge(bankPowerUpName)} {GetShortPowerUpName(bankPowerUpName)}\nCharge {bankPowerUpCharge}/{GetPowerUpBankThreshold()}";
        }

        if (inventory.GetInventoryRewardCount(bankPowerUpName) > 0)
        {
            return $"{GetPowerUpBadge(bankPowerUpName)} READY\nTap";
        }

        return $"BUY {PlayerInventoryState.GameplayPowerUpCrystalCost}<>\nTap";
    }

    private string GetPowerUpBankStockText()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "No eligible power-up";
        }

        int stock = inventory.GetInventoryRewardCount(bankPowerUpName);
        return stock > 0 ? $"{bankPowerUpName} stock x{stock}" : $"{bankPowerUpName} stock 0";
    }

    private Color GetPowerUpBankButtonColor()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return new Color(0.24f, 0.22f, 0.28f);
        }

        if (!IsPowerUpBankReady())
        {
            return new Color(0.24f, 0.22f, 0.28f);
        }

        return inventory.GetInventoryRewardCount(bankPowerUpName) > 0
            ? new Color(0.35f, 0.12f, 0.62f)
            : new Color(0.08f, 0.42f, 0.62f);
    }

    private bool IsPowerUpBankReady()
    {
        return roundFlow.IsActive
            && !string.IsNullOrEmpty(bankPowerUpName)
            && bankPowerUpCharge >= GetPowerUpBankThreshold();
    }

    private int GetPowerUpBankThreshold()
    {
        return Mathf.Max(1, selectedCardCount);
    }

    private void SelectNextBankPowerUp(string message)
    {
        bankPowerUpName = DrawNextBankPowerUp();
        bankPowerUpCharge = 0;
        SetGameplayPowerUpEventText(string.IsNullOrEmpty(bankPowerUpName) ? "No eligible bank power-up" : $"{message}: {GetShortPowerUpName(bankPowerUpName)}");
        RefreshPowerUpDisplays();
    }

    private string DrawNextBankPowerUp()
    {
        if (bankPowerUpBag.Count == 0)
        {
            RefillBankPowerUpBag();
        }

        if (bankPowerUpBag.Count == 0)
        {
            return "";
        }

        string next = bankPowerUpBag[0];
        bankPowerUpBag.RemoveAt(0);
        return next;
    }

    private void RefillBankPowerUpBag()
    {
        bankPowerUpBag.Clear();
        List<string> eligible = inventory.GetEligibleGameplayPowerUps(selectedCardCount, manaBetPerCard, GetActiveProgression().Level);
        if (selectedCardCount >= 2 && Random.value < WildSigilBankOfferChance)
        {
            eligible.Add("Wild Sigil");
        }

        while (eligible.Count > 0)
        {
            int index = Random.Range(0, eligible.Count);
            bankPowerUpBag.Add(eligible[index]);
            eligible.RemoveAt(index);
        }
    }

    private void TrackPowerUpReleaseFromDaub()
    {
        if (!roundFlow.IsActive || string.IsNullOrEmpty(bankPowerUpName) || IsPowerUpBankReady())
        {
            return;
        }

        bankPowerUpCharge = Mathf.Min(GetPowerUpBankThreshold(), bankPowerUpCharge + 1);
        if (IsPowerUpBankReady())
        {
            SetGameplayPowerUpEventText(GetPowerUpBankChargedMessage());
            RefreshPowerUpDisplays();
            if (CanAutoReleasePowerUpBank())
            {
                StartCoroutine(UsePowerUpBankAfterDaub());
            }
            return;
        }

        SetGameplayPowerUpEventText($"{GetShortPowerUpName(bankPowerUpName)} charging {bankPowerUpCharge}/{GetPowerUpBankThreshold()}");
        RefreshPowerUpDisplays();
    }

    private IEnumerator UsePowerUpBankAfterDaub()
    {
        yield return null;
        UsePowerUpBank();
    }

    private void TogglePowerUpAutoDrop()
    {
        inventory.SetAutoDropPowerUps(!inventory.AutoDropPowerUps);
        SetGameplayPowerUpEventText(inventory.AutoDropPowerUps ? "Auto-drop enabled" : "Auto-drop off");
        RefreshPowerUpDisplays();
        if (CanAutoReleasePowerUpBank())
        {
            StartCoroutine(UsePowerUpBankAfterDaub());
        }
    }

    private bool CanAutoReleasePowerUpBank()
    {
        return inventory.AutoDropPowerUps
            && IsPowerUpBankReady()
            && !string.IsNullOrEmpty(bankPowerUpName)
            && inventory.GetInventoryRewardCount(bankPowerUpName) > 0;
    }

    private string GetPowerUpBankChargedMessage()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "No power-up selected";
        }

        if (inventory.GetInventoryRewardCount(bankPowerUpName) > 0)
        {
            return inventory.AutoDropPowerUps ? $"{GetShortPowerUpName(bankPowerUpName)} auto releasing" : $"{GetShortPowerUpName(bankPowerUpName)} charged";
        }

        return $"Out of stock. Tap to buy for {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals.";
    }

    private void UsePowerUpBank()
    {
        if (!IsPowerUpBankReady())
        {
            return;
        }

        string powerUpName = bankPowerUpName;
        bool usedInventory = inventory.GetInventoryRewardCount(powerUpName) > 0;
        if (usedInventory)
        {
            if (!inventory.TryConsumeInventoryReward(powerUpName, 1))
            {
                SetGameplayPowerUpEventText("Inventory use failed");
                RefreshPowerUpDisplays();
                return;
            }
        }
        else if (!inventory.TrySpendCrystals(PlayerInventoryState.GameplayPowerUpCrystalCost))
        {
            SetGameplayPowerUpEventText("Need 10 crystals");
            RefreshPowerUpDisplays();
            return;
        }

        SetGameplayPowerUpEventText(usedInventory ? $"Used {powerUpName} from inventory" : $"Bought {powerUpName} for {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals");
        ShowPowerUpBurst($"{GetPowerUpBadge(powerUpName)} {GetShortPowerUpName(powerUpName).ToUpperInvariant()}");
        RefreshPowerUpDisplays();
        bool selectNext = ApplyPowerUpEffect(powerUpName);
        if (!usedInventory && statusText != null)
        {
            statusText.text += $" Spent {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals.";
        }

        if (selectNext && roundFlow.IsActive && !rewardPreviewShown)
        {
            SelectNextBankPowerUp("Next power-up charging");
        }
        else
        {
            RefreshPowerUpDisplays();
        }
    }

    private bool ApplyPowerUpEffect(string powerUpName)
    {
        if (powerUpName == "Single Sigil")
        {
            ApplyRandomMarksPerCard(powerUpName, 1);
            return true;
        }

        if (powerUpName == "Multi Sigil")
        {
            ApplyRandomMarksPerCard(powerUpName, 2);
            return true;
        }

        if (powerUpName == "Arcane Spark")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Fortune Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Wild Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Presto Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Pandora Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        return true;
    }

    private void PlacePowerUpSigilOnCards(string powerUpName)
    {
        int placedCount = 0;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            if (!TryGetRandomPowerUpDropCell(cardIndex, out Vector2Int cell))
            {
                continue;
            }

            string key = $"{cardIndex}:{cell.x}:{cell.y}";
            placedPowerUpSigils[key] = powerUpName;
            placedCount++;
            RefreshPlacedPowerUpBadge(cardIndex, cell.x, cell.y);
        }

        string shortName = GetShortPowerUpName(powerUpName);
        SetGameplayPowerUpEventText(placedCount > 0 ? $"{shortName} dropped" : $"{shortName} found no square");
        ShowPowerUpBurst(placedCount > 0 ? $"{GetPowerUpBadge(powerUpName)} DROPPED x{placedCount}" : $"{GetPowerUpBadge(powerUpName)} NO OPEN CELL");
        if (statusText != null)
        {
            statusText.text = placedCount > 0
                ? $"{powerUpName} dropped onto {placedCount} card{(placedCount == 1 ? "" : "s")}. Daub its called square to trigger it."
                : $"{powerUpName} found no open square to drop onto.";
        }
    }

    private bool TryGetRandomPowerUpDropCell(int cardIndex, out Vector2Int cell)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        List<Vector2Int> calledCandidates = new List<Vector2Int>();
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                string key = $"{cardIndex}:{row}:{column}";
                if (IsMarkablePowerUpSquare(cardIndex, row, column) && !placedPowerUpSigils.ContainsKey(key))
                {
                    Vector2Int candidate = new Vector2Int(row, column);
                    candidates.Add(candidate);
                    int value = playerCards.GetNumber(cardIndex, row, column);
                    if (roundFlow.IsNumberCalled(value))
                    {
                        calledCandidates.Add(candidate);
                    }
                }
            }
        }

        if (calledCandidates.Count > 0)
        {
            cell = calledCandidates[Random.Range(0, calledCandidates.Count)];
            return true;
        }

        if (candidates.Count == 0)
        {
            cell = Vector2Int.zero;
            return false;
        }

        cell = candidates[Random.Range(0, candidates.Count)];
        return true;
    }

    private void RefreshPlacedPowerUpBadge(int cardIndex, int row, int column)
    {
        string key = $"{cardIndex}:{row}:{column}";
        if (!visibleCardCells.TryGetValue(key, out Button button))
        {
            return;
        }

        RemovePlacedPowerUpBadge(button.transform);
        if (!playerCards.IsMarked(cardIndex, row, column) && placedPowerUpSigils.TryGetValue(key, out string powerUpName))
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            float size = rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) : CellSize;
            CreatePlacedPowerUpBadge(button.transform, GetPowerUpCellInitials(powerUpName), size);
        }
    }

    private int GetArcaneSparkMarkCount()
    {
        if (selectedCardCount <= 1) return 2;
        if (selectedCardCount == 2) return 5;
        if (selectedCardCount == 4) return 10;
        return 15;
    }

    private void ApplyRandomMarksPerCard(string powerUpName, int marksPerCard)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            AddRandomCardTargets(targets, cardIndex, marksPerCard);
        }

        ApplyPowerUpMarks(powerUpName, targets);
    }

    private void ApplyRandomMarksAcrossCards(string powerUpName, int markCount)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (IsMarkablePowerUpSquare(cardIndex, row, column))
                    {
                        candidates.Add(new Vector3Int(cardIndex, row, column));
                    }
                }
            }
        }

        List<Vector3Int> targets = new List<Vector3Int>();
        while (targets.Count < markCount && candidates.Count > 0)
        {
            int index = Random.Range(0, candidates.Count);
            targets.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        ApplyPowerUpMarks(powerUpName, targets);
    }

    private void AddRandomCardTargets(List<Vector3Int> targets, int cardIndex, int count)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (IsMarkablePowerUpSquare(cardIndex, row, column))
                {
                    candidates.Add(new Vector2Int(row, column));
                }
            }
        }

        while (count > 0 && candidates.Count > 0)
        {
            int index = Random.Range(0, candidates.Count);
            Vector2Int cell = candidates[index];
            candidates.RemoveAt(index);
            targets.Add(new Vector3Int(cardIndex, cell.x, cell.y));
            count--;
        }
    }

    private bool IsMarkablePowerUpSquare(int cardIndex, int row, int column)
    {
        return cardIndex >= 0
            && cardIndex < playerCards.Count
            && !(row == 2 && column == 2)
            && !playerCards.IsMarked(cardIndex, row, column);
    }

    private void ApplyPrestoSigil()
    {
        List<Vector3Int> targets = GetBestPrestoTargets();
        if (targets.Count == 0)
        {
            ApplyRandomMarksAcrossCards("Presto Sigil", 1);
            return;
        }

        ApplyPowerUpMarks("Presto Sigil", targets);
    }

    private List<Vector3Int> GetBestPrestoTargets()
    {
        List<Vector3Int> bestTargets = new List<Vector3Int>();
        int bestMarked = -1;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                EvaluatePrestoLine(cardIndex, GetLineTargets(cardIndex, true, row), ref bestMarked, ref bestTargets);
            }

            for (int column = 0; column < BoardSize; column++)
            {
                EvaluatePrestoLine(cardIndex, GetLineTargets(cardIndex, false, column), ref bestMarked, ref bestTargets);
            }

            EvaluatePrestoLine(cardIndex, GetDiagonalTargets(cardIndex, true), ref bestMarked, ref bestTargets);
            EvaluatePrestoLine(cardIndex, GetDiagonalTargets(cardIndex, false), ref bestMarked, ref bestTargets);
        }

        return bestTargets;
    }

    private List<Vector3Int> GetLineTargets(int cardIndex, bool rowLine, int index)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int offset = 0; offset < BoardSize; offset++)
        {
            int row = rowLine ? index : offset;
            int column = rowLine ? offset : index;
            targets.Add(new Vector3Int(cardIndex, row, column));
        }

        return targets;
    }

    private List<Vector3Int> GetDiagonalTargets(int cardIndex, bool leftToRight)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int index = 0; index < BoardSize; index++)
        {
            targets.Add(new Vector3Int(cardIndex, index, leftToRight ? index : BoardSize - 1 - index));
        }

        return targets;
    }

    private void EvaluatePrestoLine(int cardIndex, List<Vector3Int> line, ref int bestMarked, ref List<Vector3Int> bestTargets)
    {
        int markedCount = 0;
        List<Vector3Int> unmarked = new List<Vector3Int>();
        for (int index = 0; index < line.Count; index++)
        {
            Vector3Int cell = line[index];
            if (playerCards.IsMarked(cardIndex, cell.y, cell.z))
            {
                markedCount++;
                continue;
            }

            if (!(cell.y == 2 && cell.z == 2))
            {
                unmarked.Add(cell);
            }
        }

        if (unmarked.Count == 0)
        {
            return;
        }

        if (markedCount > bestMarked)
        {
            bestMarked = markedCount;
            bestTargets = unmarked;
        }
    }

    private void ApplyPowerUpMarks(string powerUpName, List<Vector3Int> targets)
    {
        if (targets.Count == 0)
        {
            if (statusText != null)
            {
                statusText.text = $"{powerUpName} found no valid squares.";
            }
            RefreshPowerUpDisplays();
            return;
        }

        int totalNewBingos = 0;
        int markedSquares = 0;
        List<string> collectedRewards = new List<string>();
        for (int index = 0; index < targets.Count; index++)
        {
            Vector3Int target = targets[index];
            if (!IsMarkablePowerUpSquare(target.x, target.y, target.z))
            {
                continue;
            }

            int previousBingoCount = playerCards.GetBingoCount(target.x);
            playerCards.Mark(target.x, target.y, target.z);
            markedSquares++;
            powerUpAssistedMarks++;

            string key = $"{target.x}:{target.y}:{target.z}";
            powerUpMarkedCellLabels[key] = GetPowerUpCellInitials(powerUpName);
            placedPowerUpSigils.Remove(key);
            powerUps.ClearCellAlert(key);
            if (cellRewards.TryCollect(key, out CellReward collectedReward))
            {
                string displayQuantity = CellRewardTracker.GetDisplayQuantity(collectedReward);
                collectedRewards.Add($"{collectedReward.Name} {displayQuantity}");
                if (collectedReward.Kind == CellRewardKind.Crystals)
                {
                    ShowPowerUpBurst($"CRYSTALS {displayQuantity}");
                }
            }

            UpdateCardBingoProgress(target.x);
            int newBingoCount = playerCards.GetBingoCount(target.x);
            int bingoDelta = Mathf.Max(0, newBingoCount - previousBingoCount);
            totalNewBingos += IsBlackoutRoom() && bingoDelta > 0 ? 1 : bingoDelta;
            RefreshPowerUpMarkedCell(target.x, target.y, target.z);

            if (newBingoCount >= BingoRoomRules.JackpotBingosPerCard)
            {
                lastJackpotEarnedCardIndex = target.x;
                roundFlow.MarkJackpotState();
            }
        }

        if (totalNewBingos > 0)
        {
            if (!IsBlackoutRoom())
            {
                ConsumeRoomBingos(totalNewBingos, false);
            }
            else
            {
                RefreshRoomPoolTexts();
            }

            AddXp(totalNewBingos * GetScaledXp(roomRules.BingoClaimXp), $"{powerUpName} Bingo");
        }

        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        string rewardText = collectedRewards.Count > 0 ? $" Collected {string.Join(", ", collectedRewards)}." : "";
        if (statusText != null)
        {
            statusText.text = $"{powerUpName} marked {markedSquares} square{(markedSquares == 1 ? "" : "s")}.{rewardText}";
        }

        ShowPowerUpBurst($"{GetPowerUpBadge(powerUpName)} MARKED {markedSquares}");
        RefreshPowerUpDisplays();
        CompleteRoundIfPowerUpEndedGame(powerUpName);
    }

    private void RefreshPowerUpMarkedCell(int cardIndex, int row, int column)
    {
        string key = $"{cardIndex}:{row}:{column}";
        if (!visibleCardCells.TryGetValue(key, out Button button))
        {
            return;
        }

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = GetVisibleCellColor(true, playerCards.IsWinningCell(cardIndex, row, column));
        }

        RefreshVisibleCellLabel(button.transform, cardIndex, row, column);

        RemoveRewardBadge(button.transform);
        RemovePlacedPowerUpBadge(button.transform);
        StartCoroutine(PulseCell(button.transform));
        RefreshVisibleCardWinningCells(cardIndex);
    }

    private void RefreshVisibleCellLabel(Transform cellTransform, int cardIndex, int row, int column)
    {
        Text label = GetCellNumberLabel(cellTransform);
        if (label == null)
        {
            return;
        }

        bool isMarked = cardIndex >= 0 && cardIndex < playerCards.Count && playerCards.IsMarked(cardIndex, row, column);
        int value = cardIndex >= 0 && cardIndex < playerCards.Count ? playerCards.GetNumber(cardIndex, row, column) : 0;
        label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
        label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
    }

    private void CompleteRoundIfPowerUpEndedGame(string powerUpName)
    {
        BingoRoundEndDecision powerUpDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            AllCardsReachedJackpot(),
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted by {powerUpName} at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
            IsStandardCoverageLimitReached(),
            $"Standard room coverage limit reached after {powerUpName}.");
        TryStartResolvedRoundEndCountdown(powerUpDecision);
    }

    private void SetGameplayPowerUpEventText(string message)
    {
        if (gameplayPowerUpEventText != null)
        {
            gameplayPowerUpEventText.text = message;
        }
    }

    private void ShowPowerUpBurst(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        SetGameplayPowerUpEventText(message);
        if (gameplayPowerUpBurstText == null)
        {
            return;
        }

        if (powerUpBurstRoutine != null)
        {
            StopCoroutine(powerUpBurstRoutine);
        }

        powerUpBurstRoutine = StartCoroutine(ShowPowerUpBurstRoutine(message));
    }

    private IEnumerator ShowPowerUpBurstRoutine(string message)
    {
        gameplayPowerUpBurstText.text = message;
        gameplayPowerUpBurstText.color = new Color(0.72f, 0.95f, 1f, 1f);
        gameplayPowerUpBurstText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        powerUpBurstRoutine = null;
    }

    private string GetPowerUpBadge(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "S1";
        if (powerUpName == "Multi Sigil") return "S+";
        if (powerUpName == "Arcane Spark") return "SPRK";
        if (powerUpName == "Fortune Sigil") return "x2";
        if (powerUpName == "Wild Sigil") return "WILD";
        if (powerUpName == "Presto Sigil") return "BINGO";
        if (powerUpName == "Pandora Sigil") return "BOX";
        return "PU";
    }

    private string GetPowerUpCellInitials(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "S1";
        if (powerUpName == "Multi Sigil") return "M+";
        if (powerUpName == "Arcane Spark") return "SP";
        if (powerUpName == "Fortune Sigil") return "x2";
        if (powerUpName == "Wild Sigil") return "W";
        if (powerUpName == "Presto Sigil") return "P";
        if (powerUpName == "Pandora Sigil") return "BX";
        return "PU";
    }

    private string GetCompactPowerUpGrantSummary(List<string> grantedPowerUps)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        for (int index = 0; index < grantedPowerUps.Count; index++)
        {
            string name = grantedPowerUps[index];
            counts[name] = counts.ContainsKey(name) ? counts[name] + 1 : 1;
        }

        List<string> summary = new List<string>();
        foreach (KeyValuePair<string, int> grant in counts)
        {
            summary.Add($"{GetShortPowerUpName(grant.Key)} x{grant.Value}");
        }

        return string.Join(", ", summary);
    }

    private string GetShortPowerUpName(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "Single";
        if (powerUpName == "Multi Sigil") return "Multi";
        if (powerUpName == "Arcane Spark") return "Spark";
        if (powerUpName == "Fortune Sigil") return "Fortune";
        if (powerUpName == "Wild Sigil") return "Wild";
        if (powerUpName == "Presto Sigil") return "Presto";
        if (powerUpName == "Pandora Sigil") return "Pandora";
        return powerUpName;
    }

    private void GrantPrototypeJackpotSpin()
    {
        GrantPrototypeJackpotSpin(false);
    }

    private void GrantPrototypeJackpotSpin(bool returnToMap)
    {
        inventory.AddPrototypeJackpotSpin();
        RefreshDevSettingsUi(returnToMap);
    }
}

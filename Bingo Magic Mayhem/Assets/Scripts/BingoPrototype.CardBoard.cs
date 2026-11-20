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

// Visible bingo cards: board/cell construction, reward and power-up badges, cell colors/labels and progress text.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildVisibleCards()
    {
        BuildVisibleCards(contentRoot, 1180);
    }

    private void BuildVisibleCards(Transform parent, float availableWidth)
    {
        visibleCardCells.Clear();
        visibleCardBingoTexts.Clear();

        int cardsToShow = Mathf.Max(1, selectedCardCount);
        int columns = cardsToShow <= 1 ? 1 : cardsToShow <= 4 ? 2 : 3;
        int rows = Mathf.CeilToInt((float)cardsToShow / columns);
        float boardGap = cardsToShow <= 2 ? 7f : 4f;
        float cardChromeHeight = cardsToShow <= 2 ? 104f : 90f;
        float layoutSpacingX = cardsToShow <= 2 ? 20f : 14f;
        float layoutSpacingY = cardsToShow <= 2 ? 12f : 10f;
        float maxAreaHeight = cardsToShow <= 2 ? 650f : 680f;
        float maxCardWidth = ((availableWidth - (layoutSpacingX * (columns - 1))) / columns) - 18f;
        float maxCardHeight = ((maxAreaHeight - (layoutSpacingY * (rows - 1))) / rows);
        float maxBoardWidth = Mathf.Min(maxCardWidth, maxCardHeight - cardChromeHeight);
        float fitCellSize = Mathf.Floor((maxBoardWidth - ((BoardSize - 1) * boardGap)) / BoardSize);
        float maxCellSize = cardsToShow <= 1 ? 108f : cardsToShow <= 2 ? 88f : cardsToShow <= 4 ? 54f : 44f;
        float minCellSize = cardsToShow <= 2 ? 58f : cardsToShow <= 4 ? 46f : 36f;
        float boardCellSize = Mathf.Clamp(fitCellSize, minCellSize, maxCellSize);
        float cardWidth = BoardSize * boardCellSize + (BoardSize - 1) * boardGap;
        float cardHeight = cardChromeHeight + cardWidth;
        float cardsAreaHeight = Mathf.Min(maxAreaHeight, rows * cardHeight + (rows - 1) * layoutSpacingY);

        GameObject cardsArea = CreatePanelInParent(parent, "VisibleCards", new Color(0f, 0f, 0f, 0f), availableWidth, cardsAreaHeight);
        GridLayoutGroup cardsLayout = cardsArea.AddComponent<GridLayoutGroup>();
        cardsLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cardsLayout.constraintCount = Mathf.Max(1, columns);
        cardsLayout.cellSize = new Vector2(cardWidth + 18f, cardHeight);
        cardsLayout.spacing = new Vector2(layoutSpacingX, layoutSpacingY);
        cardsLayout.childAlignment = TextAnchor.MiddleCenter;

        for (int cardIndex = 0; cardIndex < cardsToShow; cardIndex++)
        {
            CreateCardBoard(cardsArea.transform, cardIndex, boardCellSize, boardGap, cardWidth, cardHeight);
        }
    }

    private void CreateCardBoard(Transform parent, int cardIndex, float boardCellSize, float boardGap, float cardWidth, float cardHeight)
    {
        GameObject cardPanel = new GameObject($"Card_{cardIndex + 1}");
        cardPanel.transform.SetParent(parent, false);

        Image panelImage = cardPanel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.17f, 0.25f);

        VerticalLayoutGroup layout = cardPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 6, 8);
        layout.spacing = 4;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        LayoutElement panelLayout = cardPanel.AddComponent<LayoutElement>();
        panelLayout.preferredWidth = cardWidth + 18f;
        panelLayout.preferredHeight = cardHeight;
        panelLayout.flexibleWidth = 0;

        CreateCardHeaderRow(cardPanel.transform, cardWidth, boardCellSize, boardGap);

        GameObject grid = new GameObject($"Card_{cardIndex + 1}_Grid");
        grid.transform.SetParent(cardPanel.transform, false);

        LayoutElement gridLayoutElement = grid.AddComponent<LayoutElement>();
        gridLayoutElement.preferredWidth = cardWidth;
        gridLayoutElement.preferredHeight = cardWidth;
        gridLayoutElement.flexibleWidth = 0;

        GridLayoutGroup gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = BoardSize;
        gridLayout.cellSize = new Vector2(boardCellSize, boardCellSize);
        gridLayout.spacing = new Vector2(boardGap, boardGap);
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                CreateVisibleCardCell(grid.transform, cardIndex, row, column, boardCellSize);
            }
        }

        Text bingoText = CreateTextInParent(cardPanel.transform, GetCardBingoText(cardIndex), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), cardWidth, 22);
        visibleCardBingoTexts[cardIndex] = bingoText;
        CreatePotionBadge(cardPanel.transform, playerCards.GetPotionMultiplier(cardIndex), boardCellSize);
    }

    private void CreatePotionBadge(Transform parent, int multiplier, float boardCellSize)
    {
        GameObject badge = new GameObject($"Potion_x{multiplier}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.sizeDelta = new Vector2(Mathf.Max(72f, boardCellSize * 1.25f), Mathf.Max(30f, boardCellSize * 0.42f));
        rect.anchoredPosition = new Vector2(-8f, 8f);

        LayoutElement layoutElement = badge.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.35f, 0.1f, 0.62f, 0.95f);

        Text text = CreateChildText(badge.transform);
        text.text = $"POTION x{multiplier}";
        text.color = new Color(1f, 0.9f, 0.32f);
        text.fontSize = Mathf.Max(10, Mathf.RoundToInt(boardCellSize * 0.18f));
        text.fontStyle = FontStyle.Bold;
    }

    private void CreateCardHeaderRow(Transform parent, float cardWidth, float boardCellSize, float boardGap)
    {
        GameObject header = new GameObject("BingoHeader");
        header.transform.SetParent(parent, false);

        LayoutElement headerLayoutElement = header.AddComponent<LayoutElement>();
        float headerHeight = Mathf.Clamp(boardCellSize * 0.62f, 38f, 52f);
        headerLayoutElement.preferredWidth = cardWidth;
        headerLayoutElement.preferredHeight = headerHeight;
        headerLayoutElement.flexibleWidth = 0;

        GridLayoutGroup headerLayout = header.AddComponent<GridLayoutGroup>();
        headerLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        headerLayout.constraintCount = BoardSize;
        headerLayout.cellSize = new Vector2(boardCellSize, headerHeight - 2f);
        headerLayout.spacing = new Vector2(boardGap, 0f);
        headerLayout.childAlignment = TextAnchor.MiddleCenter;

        string[] letters = { "B", "I", "N", "G", "O" };
        Color[] colors =
        {
            new Color(0.78f, 0.12f, 0.1f),
            new Color(0.94f, 0.54f, 0.06f),
            new Color(0.18f, 0.64f, 0.12f),
            new Color(0.05f, 0.35f, 0.78f),
            new Color(0.5f, 0.12f, 0.74f)
        };

        for (int index = 0; index < letters.Length; index++)
        {
            GameObject cell = new GameObject($"Header_{letters[index]}");
            cell.transform.SetParent(header.transform, false);
            Image image = cell.AddComponent<Image>();
            image.color = colors[index];

            Text text = CreateChildText(cell.transform);
            text.text = letters[index];
            text.fontSize = Mathf.RoundToInt(Mathf.Clamp(boardCellSize * 0.36f, 18f, 30f));
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
        }
    }

    private void CreateVisibleCardCell(Transform parent, int cardIndex, int row, int column, float boardCellSize)
    {
        GameObject cellObject = new GameObject($"Card_{cardIndex + 1}_Cell_{row}_{column}");
        cellObject.transform.SetParent(parent, false);

        bool isMarked = playerCards.IsMarked(cardIndex, row, column);
        int value = playerCards.GetNumber(cardIndex, row, column);
        bool isWinningCell = playerCards.IsWinningCell(cardIndex, row, column);

        Image image = cellObject.AddComponent<Image>();
        image.color = GetVisibleCellColor(isMarked, isWinningCell);

        Button button = cellObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => DaubVisibleCardCell(cardIndex, row, column));

        Text label = CreateChildText(cellObject.transform);
        string visibleKey = $"{cardIndex}:{row}:{column}";
        label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
        label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
        label.fontSize = Mathf.RoundToInt(boardCellSize * GetVisibleCellLabelScale(row, column, isMarked, label.text));
        label.fontStyle = FontStyle.Bold;

        if (!isMarked && cellRewards.TryGetReward(visibleKey, out CellReward reward))
        {
            CreateRewardBadge(cellObject.transform, reward.Badge, boardCellSize);
        }

        if (!isMarked && placedPowerUpSigils.TryGetValue(visibleKey, out string placedPowerUpName))
        {
            CreatePlacedPowerUpBadge(cellObject.transform, GetPowerUpCellInitials(placedPowerUpName), boardCellSize);
        }

        visibleCardCells[visibleKey] = button;
    }

    private void CreateRewardBadge(Transform parent, string label, float boardCellSize)
    {
        GameObject badge = new GameObject($"Reward_{label}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(boardCellSize * 0.58f, boardCellSize * 0.28f);
        rect.anchoredPosition = new Vector2(-2f, -2f);

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.38f, 0.1f, 0.62f);

        Text text = CreateChildText(badge.transform);
        text.text = label;
        text.color = new Color(0.88f, 0.96f, 1f);
        text.fontSize = Mathf.Max(8, Mathf.RoundToInt(boardCellSize * 0.18f));
        text.fontStyle = FontStyle.Bold;
    }

    private void CreatePlacedPowerUpBadge(Transform parent, string label, float boardCellSize)
    {
        GameObject badge = new GameObject($"PowerUpDrop_{label}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(boardCellSize * 0.08f, boardCellSize * 0.08f);
        rect.offsetMax = new Vector2(-boardCellSize * 0.08f, -boardCellSize * 0.08f);

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.28f, 0.05f, 0.58f, 0.24f);

        Text text = CreateChildText(badge.transform);
        text.text = label;
        text.color = new Color(0.52f, 0.92f, 1f, 0.82f);
        text.fontSize = Mathf.Max(14, Mathf.RoundToInt(boardCellSize * 0.28f));
        text.fontStyle = FontStyle.Bold;
        badge.transform.SetAsFirstSibling();
    }

    private Text GetCellNumberLabel(Transform cellTransform)
    {
        Transform labelTransform = cellTransform.Find("Label");
        return labelTransform != null ? labelTransform.GetComponent<Text>() : null;
    }

    private void RemoveRewardBadge(Transform cellTransform)
    {
        for (int index = cellTransform.childCount - 1; index >= 0; index--)
        {
            Transform child = cellTransform.GetChild(index);
            if (child.name.StartsWith("Reward_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void RemovePlacedPowerUpBadge(Transform cellTransform)
    {
        for (int index = cellTransform.childCount - 1; index >= 0; index--)
        {
            Transform child = cellTransform.GetChild(index);
            if (child.name.StartsWith("PowerUpDrop_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private string GetCellKey(int row, int column)
    {
        return PlayerCardSet.GetCellKey(row, column);
    }

    private Color GetVisibleCellColor(bool isMarked, bool isWinningCell)
    {
        if (isWinningCell)
        {
            return new Color(1f, 0.72f, 0.12f);
        }

        return isMarked ? new Color(0.08f, 0.52f, 0.32f) : new Color(0.95f, 0.97f, 1f);
    }

    private string GetVisibleCellLabelText(int cardIndex, int row, int column, int value, bool isMarked)
    {
        if (row == 2 && column == 2)
        {
            return "FREE";
        }

        if (!isMarked)
        {
            return value.ToString();
        }

        string visibleKey = $"{cardIndex}:{row}:{column}";
        return powerUpMarkedCellLabels.TryGetValue(visibleKey, out string powerUpLabel) ? powerUpLabel : "";
    }

    private Color GetVisibleCellLabelColor(int cardIndex, int row, int column, bool isMarked)
    {
        if (!isMarked)
        {
            return new Color(0.08f, 0.1f, 0.16f);
        }

        if (row == 2 && column == 2)
        {
            return Color.white;
        }

        string visibleKey = $"{cardIndex}:{row}:{column}";
        return powerUpMarkedCellLabels.ContainsKey(visibleKey) ? new Color(0.72f, 0.95f, 1f) : Color.white;
    }

    private float GetVisibleCellLabelScale(int row, int column, bool isMarked, string labelText)
    {
        if (row == 2 && column == 2)
        {
            return 0.38f;
        }

        if (!isMarked)
        {
            return 0.5f;
        }

        return string.IsNullOrEmpty(labelText) ? 0.1f : 0.46f;
    }

    private string GetCardBingoText(int cardIndex)
    {
        if (IsBlackoutRoom())
        {
            int marked = playerCards.CountMarkedPlayable(cardIndex);
            string blackoutProgress = GetBlackoutProgressMeter(marked);
            string blackoutText = marked >= BingoRoomRules.BlackoutPlayableSquares
                ? $"BLACKOUT EARNED {blackoutProgress}"
                : $"Blackout {blackoutProgress} {marked}/{BingoRoomRules.BlackoutPlayableSquares}";
            return $"{blackoutText}  |  Potion x{playerCards.GetPotionMultiplier(cardIndex)}";
        }

        int count = playerCards.GetBingoCount(cardIndex);
        string jackpotProgress = GetJackpotProgressMeter(count);
        string bingoText = count >= BingoRoomRules.JackpotBingosPerCard
            ? $"JACKPOT EARNED {jackpotProgress}"
            : $"Jackpot {jackpotProgress} {count}/{BingoRoomRules.JackpotBingosPerCard}";
        return $"{bingoText}  |  Potion x{playerCards.GetPotionMultiplier(cardIndex)}";
    }

    private string GetBlackoutProgressMeter(int markedCount)
    {
        int filled = Mathf.Clamp(Mathf.FloorToInt(markedCount / 4f), 0, 6);
        return $"[{new string('#', filled)}{new string('-', 6 - filled)}]";
    }

    private string GetJackpotProgressMeter(int bingoCount)
    {
        int filled = Mathf.Clamp(bingoCount, 0, BingoRoomRules.JackpotBingosPerCard);
        return $"[{new string('#', filled)}{new string('-', BingoRoomRules.JackpotBingosPerCard - filled)}]";
    }

    private string GetRoundSummaryText()
    {
        if (playerCards.Count == 0)
        {
            return "No cards active.";
        }

        List<string> lines = new List<string>();
        for (int index = 0; index < playerCards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = playerCards.CountMarkedPlayable(index);
                string blackoutLabel = marked >= BingoRoomRules.BlackoutPlayableSquares ? "BLACKOUT" : $"{marked}/{BingoRoomRules.BlackoutPlayableSquares}";
                lines.Add($"Card {index + 1}: {blackoutLabel}");
                continue;
            }

            int count = playerCards.GetBingoCount(index);
            string label = count >= BingoRoomRules.JackpotBingosPerCard ? "JACKPOT" : $"{count}/{BingoRoomRules.JackpotBingosPerCard}";
            lines.Add($"Card {index + 1}: {label}");
        }

        return string.Join("\n", lines);
    }

    private bool IsBlackoutRoom()
    {
        return RealmContentCatalog.ActivePrototypeRoom.IsSpecial
            && RealmContentCatalog.ActivePrototypeRoom.ModeLabel == "Blackout";
    }

    private RoomProgressionProfile GetActiveProgression()
    {
        return RealmContentCatalog.ActivePrototypeRoom.Progression;
    }

    private string FormatPercent(float value)
    {
        return $"{Mathf.RoundToInt(value * 100f)}%";
    }

    private string GetGameplayClairvoyanceText()
    {
        return inventory.HasActiveClairvoyance()
            ? $"CLAIRVOYANCE\n{inventory.GetActiveClairvoyanceTimeText()}"
            : "CLAIRVOYANCE\nOFF";
    }
}

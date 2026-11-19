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

// End-of-round reward preview, card reveal and reward redemption.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void ShowRewardPreview(RewardPreview preview)
    {
        if (statusText == null || statusText.transform.parent == null)
        {
            return;
        }

        Transform root = statusText.transform.parent;
        GameObject panel = CreateAnchoredPanel(root, "RewardPreview", new Color(0.96f, 0.86f, 0.62f, 0.98f), 980, 500, 0f, -162f);
        Color ink = new Color(0.15f, 0.08f, 0.24f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 42, FontStyle.Bold, ink, 900, 58, 0f, 210f);
        string roundResultText = IsBlackoutRoom()
            ? $"{preview.JackpotCards} {(preview.JackpotCards == 1 ? "BLACKOUT" : "BLACKOUTS")}"
            : $"{preview.PlayerBingos} {(preview.PlayerBingos == 1 ? "BINGO" : "BINGOS")}";
        Text roundResultLabel = CreateAnchoredText(panel.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 360, 46, 0f, 160f);

        CreateRoundSummaryStat(panel.transform, "MANA EARNED", preview.ManaReward.ToString("N0"), "*", -312f, 100f);
        CreateRoundSummaryStat(panel.transform, "XP EARNED", preview.PlayerXp.ToString("N0"), "XP", 0f, 100f);
        CreateRoundSummaryStat(panel.transform, preview.EndLevel > preview.StartLevel ? "LEVEL UP" : $"LEVEL {preview.EndLevel}", BuildRoundRankSummary(preview), "", 312f, 100f);

        Text collectedRewardsTitle = CreateAnchoredText(panel.transform, string.Empty, 22, FontStyle.Bold, ink, 820, 30, 0f, 36f);
        RoundCollectedRewardsSectionView collectedRewardsSectionView = panel.GetComponent<RoundCollectedRewardsSectionView>();
        if (collectedRewardsSectionView == null)
        {
            collectedRewardsSectionView = panel.AddComponent<RoundCollectedRewardsSectionView>();
        }

        collectedRewardsSectionView.ResetRuntimeBindings();
        collectedRewardsSectionView.Initialize(collectedRewardsTitle);
        collectedRewardsSectionView.Apply(new RoundCollectedRewardsSectionDisplayModel
        {
            TitleText = "COLLECTED REWARDS"
        });
        BuildRoundCollectedRewards(panel.transform, preview);

        BuildRoundIngredientProgress(panel.transform, preview);

        Button redeem = CreateAnchoredButton(panel.transform, "Collect", 22, 260, 50, new Color(0.12f, 0.55f, 0.08f), 0f, -220f);
        RewardPreviewPanelView view = panel.GetComponent<RewardPreviewPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<RewardPreviewPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, roundResultLabel, collectedRewardsTitle, null, redeem);
        view.ContinueRequested += RedeemRewardsAndReturnHome;
        view.Apply(new RewardPreviewPanelDisplayModel
        {
            TitleText = "ROUND COMPLETE",
            RoundResultText = roundResultText,
            CollectedRewardsTitleText = string.Empty,
            PotionProgressTitleText = string.Empty,
            ContinueButtonText = "Collect"
        });
    }

    private void CreateRoundSummaryStat(Transform parent, string label, string value, string icon, float x, float y)
    {
        GameObject tile = CreateAnchoredPanel(parent, label, new Color(0.99f, 0.91f, 0.72f), 280, 94, x, y);
        Color ink = new Color(0.15f, 0.08f, 0.24f);
        Text labelText = CreateAnchoredText(tile.transform, label, 17, FontStyle.Bold, ink, 240, 24, 0f, 28f);
        string body = string.IsNullOrEmpty(icon) ? value : $"{icon}  {value}";
        Text valueText = CreateAnchoredText(tile.transform, body, value.Length > 12 ? 21 : 34, FontStyle.Bold, ink, 250, 46, 0f, -12f);
        RoundSummaryStatView view = tile.GetComponent<RoundSummaryStatView>();
        if (view == null)
        {
            view = tile.AddComponent<RoundSummaryStatView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(labelText, valueText);
        view.Apply(new RoundSummaryStatDisplayModel
        {
            LabelText = label,
            ValueText = body
        });
    }

    private string BuildRoundRankSummary(RewardPreview preview)
    {
        return $"Level {preview.EndLevel}\n{preview.LevelProgressText}";
    }

    private void ShowCardReveal(IReadOnlyList<AwardedAlbumCardRecord> cards)
    {
        if (statusText == null || statusText.transform.parent == null)
        {
            ContinueAfterCardReveal();
            return;
        }

        Transform root = statusText.transform.parent;
        Transform existing = root.Find("CardReveal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        GameObject panel = CreateAnchoredPanel(root, "CardReveal", new Color(0.035f, 0.018f, 0.07f, 0.985f), 1320, 680, 0f, -84f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 44, FontStyle.Bold, gold, 980, 58, 0f, 286f);
        Text subtitleText = CreateAnchoredText(panel.transform, string.Empty, 19, FontStyle.Bold, new Color(0.86f, 0.84f, 0.94f), 920, 30, 0f, 238f);
        Text overflowText = CreateAnchoredText(panel.transform, string.Empty, 20, FontStyle.Bold, Color.white, 360, 34, 0f, -182f);
        overflowText.gameObject.SetActive(false);
        Button continueButton = CreateAnchoredButton(panel.transform, "Continue", 24, 280, 56, new Color(0.12f, 0.55f, 0.08f), 0f, -266f);
        CardRevealPanelView panelView = panel.GetComponent<CardRevealPanelView>();
        if (panelView == null)
        {
            panelView = panel.AddComponent<CardRevealPanelView>();
        }

        panelView.ResetRuntimeBindings();
        panelView.Initialize(titleText, subtitleText, overflowText, continueButton);
        panelView.ContinueRequested += ContinueAfterCardReveal;

        int visibleCount = Mathf.Min(cards.Count, 5);
        panelView.Apply(new CardRevealPanelDisplayModel
        {
            TitleText = "ADDED TO GRIMOIRE",
            SubtitleText = "Open the Library to find NEW cards marked in their potion set.",
            OverflowText = cards.Count > visibleCount ? $"+{cards.Count - visibleCount} more card wins" : string.Empty,
            ContinueButtonText = "Continue"
        });

        float startX = -(visibleCount - 1) * 126f;
        for (int index = 0; index < visibleCount; index++)
        {
            AwardedAlbumCardRecord card = cards[index];
            float x = startX + index * 252f;
            CreateCardRevealTile(panel.transform, card, x, 38f);
        }
    }

    private void CreateCardRevealTile(Transform parent, AwardedAlbumCardRecord card, float x, float y)
    {
        Color cardColor = GetRevealCardColor(card.RarityLabel);
        GameObject tile = CreateAnchoredPanel(parent, $"CardReveal_{card.CardName}", cardColor, 210, 300, x, y);
        Text starsText = CreateAnchoredText(tile.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 180, 40, 0f, 104f);
        Text cardNameText = CreateAnchoredText(tile.transform, string.Empty, 24, FontStyle.Bold, Color.white, 176, 86, 0f, 32f);
        Text rarityText = CreateAnchoredText(tile.transform, string.Empty, 18, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 160, 28, 0f, -58f);
        Text potionNameText = CreateAnchoredText(tile.transform, string.Empty, 13, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 174, 46, 0f, -102f);
        Text statusText = CreateAnchoredText(tile.transform, string.Empty, 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 160, 26, 0f, 132f);
        statusText.gameObject.SetActive(false);

        CardRevealTileView view = tile.GetComponent<CardRevealTileView>();
        if (view == null)
        {
            view = tile.AddComponent<CardRevealTileView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(starsText, cardNameText, rarityText, potionNameText, statusText);
        view.Apply(new CardRevealTileDisplayModel
        {
            StarsText = BuildStarText(card.Stars),
            CardName = card.CardName,
            RarityText = card.RarityLabel,
            PotionName = card.PotionName,
            StatusText = card.IsNew ? string.Empty : (card.Copies > 1 ? $"Duplicate x{card.Copies}" : string.Empty)
        });

        if (card.IsNew)
        {
            CreateNewBadge(tile.transform, 58f, 132f, 82, 30, 14);
        }
    }

    private Color GetRevealCardColor(string rarityLabel)
    {
        if (rarityLabel == "Gilded")
        {
            return new Color(0.22f, 0.38f, 0.82f);
        }

        if (rarityLabel == "Ancient")
        {
            return new Color(0.36f, 0.24f, 0.58f);
        }

        return new Color(0.58f, 0.42f, 0.22f);
    }

    private void BuildRoundCollectedRewards(Transform parent, RewardPreview preview)
    {
        Dictionary<string, int> rewardCounts = new Dictionary<string, int>();

        for (int index = 0; index < preview.CollectedCellRewards.Count; index++)
        {
            CellReward reward = preview.CollectedCellRewards[index];
            string rewardName = reward.Name;
            int quantity = reward.Quantity;
            rewardCounts[rewardName] = rewardCounts.ContainsKey(rewardName) ? rewardCounts[rewardName] + quantity : quantity;
        }

        List<string> rewardsList = new List<string>();
        foreach (KeyValuePair<string, int> reward in rewardCounts)
        {
            rewardsList.Add(FormatRoundRewardCount(reward.Key, reward.Value));
        }

        if (rewardsList.Count == 0)
        {
            rewardsList.Add("No extra rewards");
        }

        int maxTiles = Mathf.Min(6, rewardsList.Count);
        float startX = maxTiles > 5 ? -375f : -360f;
        for (int index = 0; index < maxTiles; index++)
        {
            float x = startX + index * (maxTiles > 5 ? 150f : 180f);
            GameObject tile = CreateAnchoredPanel(parent, $"RewardTile_{index}", new Color(0.99f, 0.91f, 0.72f), maxTiles > 5 ? 136 : 156, 88, x, -30f);
            Text iconText = CreateAnchoredText(tile.transform, GetRewardTileIcon(rewardsList[index]), 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 120, 30, 0f, 18f);
            Text bodyText = CreateAnchoredText(tile.transform, rewardsList[index], 13, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), maxTiles > 5 ? 118 : 134, 44, 0f, -20f);
            RoundCollectedRewardTileView view = tile.GetComponent<RoundCollectedRewardTileView>();
            if (view == null)
            {
                view = tile.AddComponent<RoundCollectedRewardTileView>();
            }

            view.ResetRuntimeBindings();
            view.Initialize(iconText, bodyText);
            view.Apply(new RoundCollectedRewardTileDisplayModel
            {
                IconText = GetRewardTileIcon(rewardsList[index]),
                BodyText = rewardsList[index]
            });
        }
    }

    private string FormatRoundRewardCount(string rewardName, int quantity)
    {
        if (rewardName.Contains("Clairvoyance"))
        {
            return $"{rewardName} {quantity}m";
        }

        return $"{rewardName} x{quantity}";
    }

    private void BuildRoundIngredientProgress(Transform parent, RewardPreview preview)
    {
        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        Text titleText = CreateAnchoredText(parent, string.Empty, 22, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), 820, 30, 0f, -106f);
        Text potionNameText = CreateAnchoredText(parent, string.Empty, 18, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 360, 26, -260f, -140f);
        Text summaryText = CreateAnchoredText(parent, string.Empty, 16, FontStyle.Bold, new Color(0.16f, 0.38f, 0.08f), 420, 24, -230f, -166f);
        RoundIngredientProgressSectionView sectionView = parent.GetComponent<RoundIngredientProgressSectionView>();
        if (sectionView == null)
        {
            sectionView = parent.gameObject.AddComponent<RoundIngredientProgressSectionView>();
        }

        sectionView.ResetRuntimeBindings();
        sectionView.Initialize(titleText, potionNameText, summaryText);
        sectionView.Apply(new RoundIngredientProgressSectionDisplayModel
        {
            TitleText = "POTION PROGRESS",
            PotionNameText = RealmContentCatalog.ActivePrototypeRoom.PotionName,
            SummaryText = preview.IngredientDrops.Count == 0 ? "No ingredients added this round" : $"{preview.IngredientDrops.Count} ingredient types added this round"
        });

        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            int earned = GetIngredientDropQuantity(preview, requirement.Name);
            float x = -6f + index * 132f;
            GameObject tile = CreateAnchoredPanel(parent, $"IngredientTile_{index}", earned > 0 ? new Color(1f, 0.9f, 0.58f) : new Color(0.91f, 0.82f, 0.62f), 112, 78, x, -154f);
            Text quantityText = CreateAnchoredText(tile.transform, earned > 0 ? $"+{earned}" : "-", 21, FontStyle.Bold, earned > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.45f, 0.4f, 0.36f), 80, 28, 0f, 16f);
            Text nameText = CreateAnchoredText(tile.transform, GetShortRoundIngredientName(requirement.Name), 12, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), 96, 36, 0f, -18f);
            RoundIngredientProgressTileView view = tile.GetComponent<RoundIngredientProgressTileView>();
            if (view == null)
            {
                view = tile.AddComponent<RoundIngredientProgressTileView>();
            }

            view.ResetRuntimeBindings();
            view.Initialize(quantityText, nameText);
            view.Apply(new RoundIngredientProgressTileDisplayModel
            {
                QuantityText = earned > 0 ? $"+{earned}" : "-",
                NameText = GetShortRoundIngredientName(requirement.Name)
            });
        }
    }

    private int GetIngredientDropQuantity(RewardPreview preview, string ingredientName)
    {
        for (int index = 0; index < preview.IngredientDrops.Count; index++)
        {
            if (preview.IngredientDrops[index].Name == ingredientName)
            {
                return preview.IngredientDrops[index].Quantity;
            }
        }

        return 0;
    }

    private string GetRewardTileIcon(string reward)
    {
        if (reward.Contains("Crystal")) return "<>";
        if (reward.Contains("Clairvoyance")) return "EYE";
        if (reward.Contains("Pandora")) return "BOX";
        if (reward.Contains("Card")) return "CARD";
        if (reward.Contains("Orb")) return "ORB";
        if (reward.Contains("Fortune")) return "x2";
        if (reward.Contains("Single")) return "S1";
        if (reward.Contains("Multi")) return "S+";
        if (reward.Contains("Arcane")) return "SPRK";
        if (reward.Contains("Wild")) return "WILD";
        if (reward.Contains("Presto")) return "BINGO";
        return "+";
    }

    private string GetShortRoundIngredientName(string name)
    {
        string[] parts = name.Split(' ');
        return parts.Length <= 2 ? name : $"{parts[0]}\n{parts[1]}";
    }

    private Button CreateAnchoredButton(Transform parent, string label, int fontSize, float width, float height, Color color, float x, float y)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        Text text = CreateChildText(buttonObject.transform);
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;

        return button;
    }

    private void RedeemRewardsAndReturnHome()
    {
        bool shouldShowJackpotSpins = false;
        if (pendingRewardPreview != null)
        {
            shouldShowJackpotSpins = pendingRewardPreview.JackpotSpinsEarned > 0;
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.RoundRewardsCollected,
                PrototypeAnalyticsPayloadFactory.CreateRoundRewardsCollectedJson(pendingRewardPreview));
            inventory.Redeem(pendingRewardPreview);
            pendingRewardPreview = null;
        }

        rewardPreviewShown = false;
        if (inventory.LastAwardedAlbumCards.Count > 0)
        {
            cardRevealShouldShowJackpotSpins = shouldShowJackpotSpins;
            ShowCardReveal(inventory.LastAwardedAlbumCards);
            return;
        }

        if (shouldShowJackpotSpins && inventory.PendingJackpotSpins > 0)
        {
            BuildJackpotSpinUi();
            return;
        }

        BuildLobbyUi();
    }

    private void ContinueAfterCardReveal()
    {
        if (cardRevealShouldShowJackpotSpins && inventory.PendingJackpotSpins > 0)
        {
            cardRevealShouldShowJackpotSpins = false;
            BuildJackpotSpinUi();
            return;
        }

        cardRevealShouldShowJackpotSpins = false;
        BuildLobbyUi();
    }
}

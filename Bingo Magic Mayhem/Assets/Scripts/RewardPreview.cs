using System.Collections.Generic;
using UnityEngine;

public sealed class RewardPreview
{
    public readonly List<IngredientDrop> IngredientDrops = new List<IngredientDrop>();
    public readonly List<CellReward> CollectedCellRewards = new List<CellReward>();

    public string EndReason { get; private set; }
    public int EntryMana { get; private set; }
    public int ManaReward { get; private set; }
    public int BingoManaReward { get; private set; }
    public int PlayerXp { get; private set; }
    public int PlayerBingos { get; private set; }
    public int SimulatedBingos { get; private set; }
    public int RoomBingos { get; private set; }
    public int BestCardBingos { get; private set; }
    public int JackpotCards { get; private set; }
    public int JackpotSpinsEarned { get; private set; }
    public int StartLevel { get; private set; }
    public int EndLevel { get; private set; }
    public string LevelProgressText { get; private set; }
    public string CellRewardSummary { get; private set; }
    public string CardBingoSummary { get; private set; }
    public string PotionIngredientSummary { get; private set; }

    public static RewardPreview Build(
        PlayerCardSet cards,
        BingoRewardTracker rewards,
        CellRewardTracker cellRewards,
        int selectedCardCount,
        int manaBetPerCard,
        string endReason,
        bool fortuneDoublePrizeActive = false,
        int bonusBingos = 0)
    {
        RewardPreview preview = new RewardPreview
        {
            EndReason = endReason,
            EntryMana = selectedCardCount * manaBetPerCard,
            PlayerXp = rewards.RoundXp,
            PlayerBingos = IsBlackoutRoom() ? 0 : cards.GetTotalBingos() + bonusBingos,
            SimulatedBingos = rewards.SimulatedBingosClaimed,
            RoomBingos = rewards.RoomBingosClaimed,
            StartLevel = rewards.RoundStartLevel,
            EndLevel = rewards.CurrentLevel,
            LevelProgressText = rewards.GetLevelProgressText(),
            CellRewardSummary = cellRewards.GetCollectedSummary(),
            CardBingoSummary = BuildCardBingoSummary(cards)
        };

        for (int index = 0; index < cellRewards.CollectedRewards.Count; index++)
        {
            preview.CollectedCellRewards.Add(cellRewards.CollectedRewards[index]);
        }

        for (int index = 0; index < cards.Count; index++)
        {
            int cardBingos = cards.GetBingoCount(index);
            preview.BestCardBingos = Mathf.Max(preview.BestCardBingos, cardBingos);
            if (cardBingos >= BingoRoomRules.JackpotBingosPerCard)
            {
                preview.JackpotCards++;
            }
        }

        preview.BuildIngredientDrops(cards);
        preview.PotionIngredientSummary = preview.BuildPotionIngredientSummary(cards);
        preview.JackpotSpinsEarned = preview.JackpotCards;
        if (IsBlackoutRoom())
        {
            preview.PlayerBingos = preview.JackpotCards;
        }

        preview.CalculateManaRewards(manaBetPerCard, fortuneDoublePrizeActive);
        return preview;
    }

    public string GetStatusLine()
    {
        string resultLabel = IsBlackoutRoom()
            ? $"{PlayerBingos} {(PlayerBingos == 1 ? "blackout" : "blackouts")}"
            : $"{PlayerBingos} {(PlayerBingos == 1 ? "bingo" : "bingos")}";
        return $"{EndReason} Reward preview: +{ManaReward} Mana, {PlayerXp} XP, {resultLabel}. Ingredients: {GetIngredientSummary()}. {CellRewardSummary}.";
    }

    public string GetPanelTitle()
    {
        return JackpotCards > 0 ? "ROUND COMPLETE - WHEELSPIN EARNED" : "ROUND COMPLETE";
    }

    public string GetPanelBody()
    {
        string resultName = IsBlackoutRoom() ? "Blackouts" : "Bingos";
        string roomResultLine = IsBlackoutRoom()
            ? $"Blackout cards: {PlayerBingos}"
            : $"Room bingos: {RoomBingos}  (Sim {SimulatedBingos})";
        return $"{EndReason}\n"
            + $"Entry spent: {EntryMana} Mana\n"
            + $"Mana won: +{ManaReward}  ({resultName} +{BingoManaReward})\n"
            + $"XP earned: +{PlayerXp}  |  {GetLevelSummary()}\n"
            + $"Your {resultName.ToLowerInvariant()}: {PlayerBingos}  |  Cards: {CardBingoSummary}\n"
            + $"Jackpot wheelspins: +{JackpotSpinsEarned}  ({GetWheelspinRuleText()})\n"
            + $"{roomResultLine}\n"
            + $"Ingredients: {GetIngredientSummary()}\n"
            + $"Potion cards: {PotionIngredientSummary}\n"
            + $"Cell rewards: {GetCellRewardPanelSummary()}";
    }

    private string GetWheelspinRuleText()
    {
        return IsBlackoutRoom() ? "1 per blackout card" : "1 per 5/5 card";
    }

    private static bool IsBlackoutRoom()
    {
        return RealmContentCatalog.ActivePrototypeRoom.IsSpecial
            && RealmContentCatalog.ActivePrototypeRoom.ModeLabel == "Blackout";
    }

    private string GetLevelSummary()
    {
        return EndLevel > StartLevel
            ? $"LEVEL UP! {StartLevel} -> {EndLevel}"
            : $"Level {EndLevel}  {LevelProgressText}";
    }

    private void CalculateManaRewards(int manaBetPerCard, bool fortuneDoublePrizeActive)
    {
        if (PlayerBingos <= 0)
        {
            BingoManaReward = 0;
            ManaReward = 0;
            return;
        }

        BingoManaReward = PlayerBingos * manaBetPerCard;
        ManaReward = fortuneDoublePrizeActive ? BingoManaReward * 2 : BingoManaReward;
    }

    private void BuildIngredientDrops(PlayerCardSet cards)
    {
        IReadOnlyList<IngredientRequirement> roomIngredients = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        BuildPotionIngredientDrops(cards, roomIngredients);
    }

    private void BuildPotionIngredientDrops(PlayerCardSet cards, IReadOnlyList<IngredientRequirement> roomIngredients)
    {
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
        {
            int bingos = cards.GetBingoCount(cardIndex);
            if (bingos <= 0)
            {
                continue;
            }

            int multiplier = cards.GetPotionMultiplier(cardIndex);
            TryAddIngredientDrop(roomIngredients[0], multiplier);

            if (bingos >= 2)
            {
                TryAddIngredientDrop(roomIngredients[1], multiplier);
            }

            if (bingos >= 3)
            {
                TryAddIngredientDrop(roomIngredients[2], multiplier);
            }

            if (bingos >= 4)
            {
                TryAddIngredientDrop(roomIngredients[3], multiplier);
            }

            if (bingos >= BingoRoomRules.JackpotBingosPerCard)
            {
                TryAddIngredientDrop(roomIngredients[4], 1);
            }
        }
    }

    private void TryAddIngredientDrop(IngredientRequirement ingredient, int quantity)
    {
        float baseChance = RealmContentCatalog.ActivePrototypeRoom.Progression.IngredientDropChance;
        float rarityChance = GetIngredientRarityChance(ingredient.Rarity);
        if (Random.value > baseChance * rarityChance)
        {
            return;
        }

        AddIngredientDrop(ingredient.Name, quantity);
    }

    private float GetIngredientRarityChance(string rarity)
    {
        if (rarity == "Common")
        {
            return 0.75f;
        }

        if (rarity == "Uncommon")
        {
            return 0.32f;
        }

        if (rarity == "Rare")
        {
            return 0.18f;
        }

        if (rarity == "Key Ingredient")
        {
            return 0.10f;
        }

        return 0.5f;
    }

    private void AddIngredientDrop(string name, int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        for (int index = 0; index < IngredientDrops.Count; index++)
        {
            if (IngredientDrops[index].Name == name)
            {
                IngredientDrops[index] = new IngredientDrop(name, IngredientDrops[index].Quantity + quantity);
                return;
            }
        }

        IngredientDrops.Add(new IngredientDrop(name, quantity));
    }

    private string GetIngredientSummary()
    {
        if (IngredientDrops.Count == 0)
        {
            return "none this round";
        }

        List<string> parts = new List<string>();
        for (int index = 0; index < IngredientDrops.Count; index++)
        {
            IngredientDrop drop = IngredientDrops[index];
            parts.Add($"{drop.Name} +{drop.Quantity}");
        }

        return string.Join(", ", parts);
    }

    private string GetCellRewardPanelSummary()
    {
        return CellRewardSummary == "Cell rewards: none" ? "none" : CellRewardSummary;
    }

    private string BuildPotionIngredientSummary(PlayerCardSet cards)
    {
        List<string> parts = new List<string>();
        for (int index = 0; index < cards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = cards.CountMarkedPlayable(index);
                if (marked <= 0)
                {
                    continue;
                }

                parts.Add($"C{index + 1} x{cards.GetPotionMultiplier(index)} -> {marked}/24");
                continue;
            }

            int bingos = cards.GetBingoCount(index);
            if (bingos <= 0)
            {
                continue;
            }

            parts.Add($"C{index + 1} x{cards.GetPotionMultiplier(index)} -> {bingos}/5");
        }

        return parts.Count == 0 ? "no bingo cards" : string.Join(", ", parts);
    }

    private static string BuildCardBingoSummary(PlayerCardSet cards)
    {
        if (cards.Count == 0)
        {
            return "none";
        }

        List<string> parts = new List<string>();
        for (int index = 0; index < cards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = cards.CountMarkedPlayable(index);
                string blackoutValue = marked >= BingoRoomRules.BlackoutPlayableSquares ? "BLACKOUT" : $"{marked}/24";
                parts.Add($"C{index + 1} {blackoutValue}");
                continue;
            }

            int count = cards.GetBingoCount(index);
            string value = count >= BingoRoomRules.JackpotBingosPerCard ? "JACKPOT" : $"{count}/5";
            parts.Add($"C{index + 1} {value}");
        }

        return string.Join(", ", parts);
    }
}

public sealed class IngredientDrop
{
    public IngredientDrop(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    public string Name { get; private set; }
    public int Quantity { get; private set; }
}

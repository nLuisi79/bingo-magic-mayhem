using System.Collections.Generic;
using UnityEngine;

public sealed class CellRewardTracker
{
    private readonly Dictionary<string, CellReward> availableRewards = new Dictionary<string, CellReward>();
    private readonly List<CellReward> collectedRewards = new List<CellReward>();

    public IReadOnlyList<CellReward> CollectedRewards => collectedRewards;

    public void GenerateForCards(PlayerCardSet cards, int selectedCardCount, int manaBetPerCard, RoomProgressionProfile progression)
    {
        availableRewards.Clear();
        collectedRewards.Clear();

        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
        {
            int rewardSlots = GetRewardSlotCount(selectedCardCount);
            for (int slot = 0; slot < rewardSlots; slot++)
            {
                if (Random.value > progression.CellRewardChance)
                {
                    continue;
                }

                CellReward reward = CellReward.Roll(selectedCardCount, manaBetPerCard, progression.Level);
                AddReward(cardIndex, reward, slot);
            }
        }
    }

    public bool TryGetReward(string cellKey, out CellReward reward)
    {
        return availableRewards.TryGetValue(cellKey, out reward);
    }

    public bool TryCollect(string cellKey, out CellReward reward)
    {
        if (!availableRewards.TryGetValue(cellKey, out reward))
        {
            return false;
        }

        availableRewards.Remove(cellKey);
        collectedRewards.Add(reward);
        return true;
    }

    public string GetCollectedSummary()
    {
        if (collectedRewards.Count == 0)
        {
            return "Cell rewards: none";
        }

        Dictionary<string, int> totals = new Dictionary<string, int>();
        for (int index = 0; index < collectedRewards.Count; index++)
        {
            CellReward reward = collectedRewards[index];
            string name = reward.Name;
            int quantity = reward.Kind == CellRewardKind.ClairvoyanceMinutes ? 1 : reward.Quantity;
            totals[name] = totals.TryGetValue(name, out int current) ? current + quantity : quantity;
        }

        List<string> parts = new List<string>();
        foreach (KeyValuePair<string, int> total in totals)
        {
            parts.Add($"{total.Key} x{total.Value}");
        }

        return "Cell rewards: " + string.Join(", ", parts);
    }

    public static string GetDisplayQuantity(CellReward reward)
    {
        if (reward.Kind == CellRewardKind.ClairvoyanceMinutes)
        {
            return $"{reward.Quantity}m";
        }

        return $"x{reward.Quantity}";
    }

    private int GetRewardSlotCount(int selectedCardCount)
    {
        if (selectedCardCount <= 1)
        {
            return 2;
        }

        return selectedCardCount >= 4 ? 3 : 2;
    }

    private void AddReward(int cardIndex, CellReward reward, int slot)
    {
        int row = (slot * 2 + cardIndex) % BingoRoomRules.BoardSize;
        int column = (slot * 3 + cardIndex + 1) % BingoRoomRules.BoardSize;
        if (row == 2 && column == 2)
        {
            column = (column + 1) % BingoRoomRules.BoardSize;
        }

        availableRewards[$"{cardIndex}:{row}:{column}"] = reward;
    }
}

public sealed class CellReward
{
    public static readonly List<string> InventoryRewardNames = new List<string>
    {
        "Regular Card",
        "Gilded Card",
        "Ancient Card",
        "Special Card",
        "Pandora Sigil",
        "Single Sigil",
        "Multi Sigil",
        "Arcane Spark",
        "Fortune Sigil",
        "Wild Sigil",
        "Presto Sigil",
        "Club Orbs"
    };

    private static readonly List<CellReward> Catalog = new List<CellReward>
    {
        new CellReward("Crystals", "CRY", CellRewardKind.Crystals, 3, 0, 1, 1, 16f),
        new CellReward("Club Orbs", "ORB", CellRewardKind.Inventory, 1, 0, 1, 1, 14f),
        new CellReward("Regular Card", "CARD", CellRewardKind.Inventory, 1, 0, 1, 1, 13f),
        new CellReward("Clairvoyance 15m", "EYE", CellRewardKind.ClairvoyanceMinutes, 15, 0, 1, 1, 11f),
        new CellReward("Single Sigil", "S1", CellRewardKind.Inventory, 1, 0, 1, 1, 10f),
        new CellReward("Pandora Sigil", "BOX", CellRewardKind.Inventory, 1, 0, 1, 2, 10f),
        new CellReward("Multi Sigil", "S+", CellRewardKind.Inventory, 1, 50, 2, 2, 7f),
        new CellReward("Fortune Sigil", "x2", CellRewardKind.Inventory, 1, 75, 2, 2, 5f),
        new CellReward("Arcane Spark", "SPRK", CellRewardKind.Inventory, 1, 100, 3, 3, 4f),
        new CellReward("Gilded Card", "GILD", CellRewardKind.Inventory, 1, 100, 3, 3, 3f),
        new CellReward("Ancient Card", "ANCT", CellRewardKind.Inventory, 1, 150, 3, 4, 1.4f),
        new CellReward("Wild Sigil", "WILD", CellRewardKind.Inventory, 1, 200, 4, 4, 0.55f),
        new CellReward("Presto Sigil", "BINGO", CellRewardKind.Inventory, 1, 250, 4, 4, 0.4f)
    };

    private CellReward(string name, string badge, CellRewardKind kind, int quantity, int minimumBet, int minimumCards, int minimumRoomLevel, float weight)
    {
        Name = name;
        Badge = badge;
        Kind = kind;
        Quantity = quantity;
        MinimumBet = minimumBet;
        MinimumCards = minimumCards;
        MinimumRoomLevel = minimumRoomLevel;
        Weight = weight;
    }

    public string Name { get; }
    public string Badge { get; }
    public CellRewardKind Kind { get; }
    public int Quantity { get; }
    public int MinimumBet { get; }
    public int MinimumCards { get; }
    public int MinimumRoomLevel { get; }
    public float Weight { get; }

    public static CellReward Roll(int selectedCardCount, int manaBetPerCard, int roomLevel)
    {
        float totalWeight = 0f;
        for (int index = 0; index < Catalog.Count; index++)
        {
            if (!Catalog[index].IsEligible(selectedCardCount, manaBetPerCard, roomLevel))
            {
                continue;
            }

            totalWeight += Catalog[index].Weight;
        }

        float roll = Random.value * totalWeight;
        for (int index = 0; index < Catalog.Count; index++)
        {
            CellReward reward = Catalog[index];
            if (!reward.IsEligible(selectedCardCount, manaBetPerCard, roomLevel))
            {
                continue;
            }

            roll -= reward.Weight;
            if (roll <= 0f)
            {
                return reward;
            }
        }

        return Catalog[0];
    }

    private bool IsEligible(int selectedCardCount, int manaBetPerCard, int roomLevel)
    {
        return selectedCardCount >= MinimumCards
            && manaBetPerCard >= MinimumBet
            && roomLevel >= MinimumRoomLevel;
    }
}

public enum CellRewardKind
{
    Inventory,
    Crystals,
    ClairvoyanceMinutes
}

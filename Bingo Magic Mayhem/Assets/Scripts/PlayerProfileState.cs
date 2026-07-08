using System.Collections.Generic;

public sealed class PlayerProfileState
{
    public PlayerProfileState(
        int mana,
        int crystals,
        int powerUpCount,
        int clairvoyanceMinutes,
        string activeClairvoyanceText,
        int pendingJackpotSpins,
        int manaCauldronAmount,
        int manaCauldronCapacity,
        int level,
        string rankTitle,
        string levelProgressText,
        string nextRankHintText,
        IReadOnlyList<ProfileInventoryLine> inventoryLines)
    {
        Mana = mana;
        Crystals = crystals;
        PowerUpCount = powerUpCount;
        ClairvoyanceMinutes = clairvoyanceMinutes;
        ActiveClairvoyanceText = activeClairvoyanceText;
        PendingJackpotSpins = pendingJackpotSpins;
        ManaCauldronAmount = manaCauldronAmount;
        ManaCauldronCapacity = manaCauldronCapacity;
        Level = level;
        RankTitle = rankTitle;
        LevelProgressText = levelProgressText;
        NextRankHintText = nextRankHintText;
        InventoryLines = inventoryLines;
    }

    public int Mana { get; private set; }
    public int Crystals { get; private set; }
    public int PowerUpCount { get; private set; }
    public int ClairvoyanceMinutes { get; private set; }
    public string ActiveClairvoyanceText { get; private set; }
    public int PendingJackpotSpins { get; private set; }
    public int ManaCauldronAmount { get; private set; }
    public int ManaCauldronCapacity { get; private set; }
    public int Level { get; private set; }
    public string RankTitle { get; private set; }
    public string LevelProgressText { get; private set; }
    public string NextRankHintText { get; private set; }
    public IReadOnlyList<ProfileInventoryLine> InventoryLines { get; private set; }

    public static PlayerProfileState FromPrototype(PlayerInventoryState inventory, BingoRewardTracker rewards)
    {
        List<ProfileInventoryLine> lines = new List<ProfileInventoryLine>
        {
            new ProfileInventoryLine("Clairvoyance", $"{inventory.ClairvoyanceMinutes}m stock, {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "inactive")}"),
            new ProfileInventoryLine("Mana Cauldron", $"{inventory.ManaCauldronAmount}/{inventory.ManaCauldronCapacity}"),
            new ProfileInventoryLine("Cards", $"Regular {inventory.GetInventoryRewardCount("Regular Card")} | Gilded {inventory.GetInventoryRewardCount("Gilded Card")} | Ancient {inventory.GetInventoryRewardCount("Ancient Card")} | Special {inventory.GetInventoryRewardCount("Special Card")}"),
            new ProfileInventoryLine("Pandora Sigil", inventory.GetInventoryRewardCount("Pandora Sigil").ToString()),
            new ProfileInventoryLine("Club Orbs", inventory.GetInventoryRewardCount("Club Orbs").ToString()),
            new ProfileInventoryLine("Single Sigil", inventory.GetInventoryRewardCount("Single Sigil").ToString()),
            new ProfileInventoryLine("Multi Sigil", inventory.GetInventoryRewardCount("Multi Sigil").ToString()),
            new ProfileInventoryLine("Arcane Spark", inventory.GetInventoryRewardCount("Arcane Spark").ToString()),
            new ProfileInventoryLine("Fortune Sigil", inventory.GetInventoryRewardCount("Fortune Sigil").ToString()),
            new ProfileInventoryLine("Wild Sigil", inventory.GetInventoryRewardCount("Wild Sigil").ToString()),
            new ProfileInventoryLine("Presto Sigil", inventory.GetInventoryRewardCount("Presto Sigil").ToString())
        };

        return new PlayerProfileState(
            inventory.Mana,
            inventory.Crystals,
            inventory.GetPowerUpInventoryCount(),
            inventory.ClairvoyanceMinutes,
            inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF",
            inventory.PendingJackpotSpins,
            inventory.ManaCauldronAmount,
            inventory.ManaCauldronCapacity,
            rewards.CurrentLevel,
            rewards.GetRankTitle(),
            rewards.GetLevelProgressText(),
            rewards.GetNextRankHintText(),
            lines);
    }
}

public readonly struct ProfileInventoryLine
{
    public ProfileInventoryLine(string label, string value)
    {
        Label = label;
        Value = value;
    }

    public string Label { get; }
    public string Value { get; }
}

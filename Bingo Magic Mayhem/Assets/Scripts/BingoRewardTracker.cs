using UnityEngine;

public sealed class BingoRewardTracker
{
    private const string PlayerXpKey = "BMM.Rewards.PlayerXp";
    private const int BaseLevelXp = 100;
    private const int LevelXpStep = 50;

    public int PlayerXp { get; private set; }
    public int RoundXp { get; private set; }
    public int RoundStartLevel { get; private set; } = 1;
    public int RoomBingosClaimed { get; private set; }
    public int SimulatedBingosClaimed { get; private set; }
    public int NextSimulatedBingoCall { get; private set; } = 14;

    private readonly BingoRoomRules rules;

    public BingoRewardTracker(BingoRoomRules rules)
    {
        this.rules = rules;
    }

    public void Load()
    {
        PlayerXp = PlayerPrefs.GetInt(PlayerXpKey, PlayerXp);
        RoundStartLevel = CurrentLevel;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerXpKey, PlayerXp);
        PlayerPrefs.Save();
    }

    public void ResetSavedXp()
    {
        PlayerXp = 0;
        RoundXp = 0;
        RoundStartLevel = 1;
        PlayerPrefs.DeleteKey(PlayerXpKey);
        PlayerPrefs.Save();
    }

    public void ResetRound()
    {
        RoundXp = 0;
        RoundStartLevel = CurrentLevel;
        RoomBingosClaimed = 0;
        SimulatedBingosClaimed = 0;
        NextSimulatedBingoCall = 14;
    }

    public void AddXp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        PlayerXp += amount;
        RoundXp += amount;
        Save();
    }

    public int ConsumeRoomBingos(int count, bool simulated)
    {
        if (count <= 0 || IsRoomPoolExhausted)
        {
            return 0;
        }

        int accepted = Mathf.Min(count, rules.RoomBingoPool - RoomBingosClaimed);
        RoomBingosClaimed += accepted;
        if (simulated)
        {
            SimulatedBingosClaimed += accepted;
        }

        return accepted;
    }

    public bool ShouldSimulatedPlayerClaim(int calledCount)
    {
        return calledCount >= NextSimulatedBingoCall && !IsRoomPoolExhausted;
    }

    public void ScheduleNextSimulatedClaim()
    {
        NextSimulatedBingoCall += SimulatedBingosClaimed < 6 ? 5 : 7;
    }

    public bool IsRoomPoolExhausted => RoomBingosClaimed >= rules.RoomBingoPool;

    public int CurrentLevel => GetLevelForXp(PlayerXp);

    public int LevelStartXp => GetTotalXpForLevel(CurrentLevel);

    public int NextLevelXp => GetTotalXpForLevel(CurrentLevel + 1);

    public int XpIntoCurrentLevel => PlayerXp - LevelStartXp;

    public int XpForCurrentLevel => NextLevelXp - LevelStartXp;

    public bool LeveledUpThisRound => CurrentLevel > RoundStartLevel;

    public string GetLevelText()
    {
        return $"Level {CurrentLevel}";
    }

    public string GetRankTitle()
    {
        return GetRankTitleForLevel(CurrentLevel);
    }

    public string GetRankSummaryText()
    {
        return $"{GetRankTitle()} | Level {CurrentLevel}";
    }

    public string GetLevelProgressText()
    {
        return $"{XpIntoCurrentLevel}/{XpForCurrentLevel} XP";
    }

    public string GetNextRankHintText()
    {
        int nextThreshold = GetNextRankThreshold(CurrentLevel);
        return nextThreshold > CurrentLevel ? $"Next rank at Level {nextThreshold}" : "Top rank reached";
    }

    private static int GetLevelForXp(int xp)
    {
        int level = 1;
        int required = BaseLevelXp;
        int remaining = Mathf.Max(0, xp);

        while (remaining >= required)
        {
            remaining -= required;
            level++;
            required += LevelXpStep;
        }

        return level;
    }

    private static int GetTotalXpForLevel(int level)
    {
        int total = 0;
        int required = BaseLevelXp;
        for (int currentLevel = 1; currentLevel < level; currentLevel++)
        {
            total += required;
            required += LevelXpStep;
        }

        return total;
    }

    private static string GetRankTitleForLevel(int level)
    {
        if (level >= 1000) return "Sorcerer Supreme";
        if (level >= 950) return "Ascendant";
        if (level >= 775) return "Paragon";
        if (level >= 625) return "Grand Archmage";
        if (level >= 500) return "Archmage";
        if (level >= 425) return "Spellmaster";
        if (level >= 350) return "Wizard";
        if (level >= 275) return "Enchanter";
        if (level >= 200) return "Mystic";
        if (level >= 140) return "Thaumaturge";
        if (level >= 90) return "Mage";
        if (level >= 50) return "Spellbinder";
        if (level >= 20) return "Apprentice";
        return "Novice";
    }

    private static int GetNextRankThreshold(int level)
    {
        if (level < 20) return 20;
        if (level < 50) return 50;
        if (level < 90) return 90;
        if (level < 140) return 140;
        if (level < 200) return 200;
        if (level < 275) return 275;
        if (level < 350) return 350;
        if (level < 425) return 425;
        if (level < 500) return 500;
        if (level < 625) return 625;
        if (level < 775) return 775;
        if (level < 950) return 950;
        if (level < 1000) return 1000;
        return level;
    }
}

using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class PlayerInventoryState
{
    private const string SavePrefix = "BMM.Inventory.";
    private const string FreebieCodeRedeemedPrefix = SavePrefix + "FreebieCodeRedeemed.";
    private const int DefaultMana = 5420;
    private const int DefaultCrystals = 50;
    private const int DefaultClairvoyanceMinutes = 0;
    private const int DefaultSingleSigils = 3;
    private const int DefaultMultiSigils = 1;
    private const int DefaultPandoraSigils = 1;
    private const int DefaultManaCauldronAmount = 80;
    private const int DefaultManaCauldronCapacity = 120;
    private const int BaseManaCauldronRefillAmount = 40;
    private const int ManaCauldronRefillPerRestoredRoom = 20;
    private const int ManaCauldronRefillSeconds = 3600;
    private const int ClairvoyanceActivationMinutes = 15;
    private const int AlbumSeasonDays = 90;
    private const int BookOfShadowsSetDays = 30;
    private const int DailyBonusBaseMana = 300;
    private const int DailyBonusManaPerStreak = 50;
    private const int DailyBonusManaPerRestoredRoom = 25;
    private const int DailyBonusBaseCrystals = 5;
    private const int PrototypeDailyHelpRequestLimit = 7;
    private static readonly int[] DailyBonusStreakSaveCosts = { 0, 50, 100, 200, 300 };
    private static readonly PrototypeRewardGrant[] DailySpinPrototypeRewards =
    {
        new PrototypeRewardGrant("Daily Spin", 25, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 30, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 35, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 45, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 50, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 60, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 70, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 75, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 80, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 90, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 100, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 120, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 150, 0, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 0, 5, 0, 0, new List<PrototypeRewardItem>()),
        new PrototypeRewardGrant("Daily Spin", 0, 0, 0, 0, new List<PrototypeRewardItem> { new PrototypeRewardItem("Single Sigil", 1) }),
        new PrototypeRewardGrant("Daily Spin", 0, 0, 0, 0, new List<PrototypeRewardItem> { new PrototypeRewardItem("Multi Sigil", 1) })
    };
    private static readonly DateTime PrototypeGlobalAlbumSeasonStartUtc = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
    public const int GameplayPowerUpCrystalCost = 10;
    public static readonly GameplayPowerUpTuning[] GameplayPowerUps =
    {
        new GameplayPowerUpTuning("Single Sigil", 1, 0, 1, 16, true),
        new GameplayPowerUpTuning("Multi Sigil", 1, 0, 1, 12, true),
        new GameplayPowerUpTuning("Arcane Spark", 1, 0, 1, 8, true),
        new GameplayPowerUpTuning("Fortune Sigil", 1, 0, 1, 9, true),
        new GameplayPowerUpTuning("Wild Sigil", 2, 0, 1, 1, true),
        new GameplayPowerUpTuning("Presto Sigil", 1, 0, 1, 2, true),
        new GameplayPowerUpTuning("Pandora Sigil", 1, 0, 1, 5, false)
    };

    private readonly Dictionary<string, int> ingredients = new Dictionary<string, int>
    {
        { "Gilded Azalea Petals", 0 },
        { "Sunwarm Dewdrops", 0 },
        { "Honeyglow Pollen", 0 },
        { "Amberroot Shavings", 0 },
        { "Arboretum Heartseed", 0 }
    };

    private readonly Dictionary<string, int> cellRewards = new Dictionary<string, int>
    {
        { "Single Sigil", DefaultSingleSigils },
        { "Multi Sigil", DefaultMultiSigils },
        { "Pandora Sigil", DefaultPandoraSigils }
    };

    private readonly Dictionary<string, int> grimoireCardCopies = new Dictionary<string, int>();
    private readonly Dictionary<string, int> bookOfShadowsCardCopies = new Dictionary<string, int>();
    private readonly List<AwardedAlbumCardRecord> lastAwardedAlbumCards = new List<AwardedAlbumCardRecord>();

    public int Mana { get; private set; } = DefaultMana;
    public int Crystals { get; private set; } = DefaultCrystals;
    public int ClairvoyanceMinutes { get; private set; } = DefaultClairvoyanceMinutes;
    public float ActiveClairvoyanceSeconds { get; private set; }
    public long ActiveClairvoyanceExpiresAtTicks { get; private set; }
    public int PendingJackpotSpins { get; private set; }
    public int ManaCauldronAmount { get; private set; } = DefaultManaCauldronAmount;
    public int ManaCauldronCapacity { get; private set; } = DefaultManaCauldronCapacity;
    public long ManaCauldronNextFillAtTicks { get; private set; }
    public long AlbumSeasonStartedAtTicks { get; private set; } = PrototypeGlobalAlbumSeasonStartUtc.Ticks;
    public bool BookOfShadowsPurchased { get; private set; } = true;
    public int JokerWildCards { get; private set; }
    public int DailyBonusStreak { get; private set; }
    public string LastDailyBonusClaimDate { get; private set; } = "";
    public int DailyBonusStreakSaveUses { get; private set; }
    public string LastDailySpinClaimDate { get; private set; } = "";
    public string LastSocialHelpRequestDate { get; private set; } = "";
    public int SocialHelpRequestsUsedToday { get; private set; }
    public int CurrentRoomJackpotPot { get; private set; } = RoomProgressionProfile.LevelOne.MinimumJackpotPot;
    public bool ActiveRoomRestored { get; private set; }
    public bool AutoDropPowerUps { get; private set; }
    public IReadOnlyList<AwardedAlbumCardRecord> LastAwardedAlbumCards => lastAwardedAlbumCards;

    private RoomProgressionProfile ActiveProgression => RealmContentCatalog.ActivePrototypeRoom.Progression;

    public struct GameplayPowerUpTuning
    {
        public readonly string Name;
        public readonly int MinCards;
        public readonly int MinBet;
        public readonly int MinLevel;
        public readonly int BankWeight;
        public readonly bool CanDropFromPandora;

        public GameplayPowerUpTuning(string name, int minCards, int minBet, int minLevel, int bankWeight, bool canDropFromPandora)
        {
            Name = name;
            MinCards = minCards;
            MinBet = minBet;
            MinLevel = minLevel;
            BankWeight = bankWeight;
            CanDropFromPandora = canDropFromPandora;
        }
    }

    public bool IsRoomRestored(RoomDefinition room)
    {
        return PlayerPrefs.GetInt(GetRoomRestoreKey(room), 0) == 1;
    }

    public int GetRestoredRoomCount()
    {
        int restoredCount = 0;
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            if (IsRoomRestored(room))
            {
                restoredCount++;
            }
        }

        return restoredCount;
    }

    public int GetManaCauldronHourlyRefillAmount()
    {
        int scaledAmount = BaseManaCauldronRefillAmount + GetRestoredRoomCount() * ManaCauldronRefillPerRestoredRoom;
        return Mathf.Clamp(scaledAmount, 0, ManaCauldronCapacity);
    }

    public int GetSavedManaBetForRoom(RoomDefinition room)
    {
        int savedBet = PlayerPrefs.GetInt(GetRoomManaBetKey(room), room.Progression.MinManaBet);
        return Mathf.Clamp(savedBet, room.Progression.MinManaBet, room.Progression.MaxManaBet);
    }

    public int GetSavedManaBetForActiveRoom()
    {
        return GetSavedManaBetForRoom(RealmContentCatalog.ActivePrototypeRoom);
    }

    public void SaveManaBetForActiveRoom(int manaBetPerCard)
    {
        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        int clampedBet = Mathf.Clamp(manaBetPerCard, room.Progression.MinManaBet, room.Progression.MaxManaBet);
        PlayerPrefs.SetInt(GetRoomManaBetKey(room), clampedBet);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        Mana = PlayerPrefs.GetInt(SavePrefix + "Mana", Mana);
        Crystals = PlayerPrefs.GetInt(SavePrefix + "Crystals", Crystals);
        ClairvoyanceMinutes = PlayerPrefs.GetInt(SavePrefix + "ClairvoyanceMinutes", ClairvoyanceMinutes);
        ActiveClairvoyanceSeconds = PlayerPrefs.GetFloat(SavePrefix + "ActiveClairvoyanceSeconds", ActiveClairvoyanceSeconds);
        if (!long.TryParse(PlayerPrefs.GetString(SavePrefix + "ActiveClairvoyanceExpiresAtTicks", "0"), out long savedExpiryTicks))
        {
            savedExpiryTicks = 0L;
        }

        ActiveClairvoyanceExpiresAtTicks = savedExpiryTicks;
        SyncTimedPowerUpsFromClock();
        PendingJackpotSpins = PlayerPrefs.GetInt(SavePrefix + "PendingJackpotSpins", PendingJackpotSpins);
        ManaCauldronCapacity = Mathf.Max(DefaultManaCauldronCapacity, PlayerPrefs.GetInt(SavePrefix + "ManaCauldronCapacity", ManaCauldronCapacity));
        ManaCauldronAmount = Mathf.Clamp(PlayerPrefs.GetInt(SavePrefix + "ManaCauldronAmount", ManaCauldronAmount), 0, ManaCauldronCapacity);
        if (!long.TryParse(PlayerPrefs.GetString(SavePrefix + "ManaCauldronNextFillAtTicks", "0"), out long cauldronTicks))
        {
            cauldronTicks = 0L;
        }

        ManaCauldronNextFillAtTicks = cauldronTicks;
        SyncManaCauldronFromClock();
        AlbumSeasonStartedAtTicks = PrototypeGlobalAlbumSeasonStartUtc.Ticks;
        BookOfShadowsPurchased = PlayerPrefs.GetInt(SavePrefix + "BookOfShadowsPurchased", BookOfShadowsPurchased ? 1 : 0) == 1;
        JokerWildCards = PlayerPrefs.GetInt(SavePrefix + "JokerWildCards", JokerWildCards);
        DailyBonusStreak = PlayerPrefs.GetInt(SavePrefix + "DailyBonusStreak", DailyBonusStreak);
        LastDailyBonusClaimDate = PlayerPrefs.GetString(SavePrefix + "LastDailyBonusClaimDate", LastDailyBonusClaimDate);
        DailyBonusStreakSaveUses = PlayerPrefs.GetInt(SavePrefix + "DailyBonusStreakSaveUses", DailyBonusStreakSaveUses);
        LastDailySpinClaimDate = PlayerPrefs.GetString(SavePrefix + "LastDailySpinClaimDate", LastDailySpinClaimDate);
        LastSocialHelpRequestDate = PlayerPrefs.GetString(SavePrefix + "LastSocialHelpRequestDate", LastSocialHelpRequestDate);
        SocialHelpRequestsUsedToday = PlayerPrefs.GetInt(SavePrefix + "SocialHelpRequestsUsedToday", SocialHelpRequestsUsedToday);
        SyncSocialHelpRequestDate();
        AutoDropPowerUps = PlayerPrefs.GetInt(SavePrefix + "AutoDropPowerUps", AutoDropPowerUps ? 1 : 0) == 1;
        LoadActiveRoomJackpotPot();
        ActiveRoomRestored = IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom);

        LoadRoomIngredients();

        LoadCellRewards();
        LoadAlbumCards();
    }

    public void RefreshActiveRoomState()
    {
        ActiveRoomRestored = IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom);
        LoadActiveRoomJackpotPot();
        LoadRoomIngredients();
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SavePrefix + "Mana", Mana);
        PlayerPrefs.SetInt(SavePrefix + "Crystals", Crystals);
        PlayerPrefs.SetInt(SavePrefix + "ClairvoyanceMinutes", ClairvoyanceMinutes);
        PlayerPrefs.SetFloat(SavePrefix + "ActiveClairvoyanceSeconds", ActiveClairvoyanceSeconds);
        PlayerPrefs.SetString(SavePrefix + "ActiveClairvoyanceExpiresAtTicks", ActiveClairvoyanceExpiresAtTicks.ToString());
        PlayerPrefs.SetInt(SavePrefix + "PendingJackpotSpins", PendingJackpotSpins);
        PlayerPrefs.SetInt(SavePrefix + "ManaCauldronAmount", ManaCauldronAmount);
        PlayerPrefs.SetInt(SavePrefix + "ManaCauldronCapacity", ManaCauldronCapacity);
        PlayerPrefs.SetString(SavePrefix + "ManaCauldronNextFillAtTicks", ManaCauldronNextFillAtTicks.ToString());
        PlayerPrefs.SetInt(SavePrefix + "BookOfShadowsPurchased", BookOfShadowsPurchased ? 1 : 0);
        PlayerPrefs.SetInt(SavePrefix + "JokerWildCards", JokerWildCards);
        PlayerPrefs.SetInt(SavePrefix + "DailyBonusStreak", DailyBonusStreak);
        PlayerPrefs.SetString(SavePrefix + "LastDailyBonusClaimDate", LastDailyBonusClaimDate);
        PlayerPrefs.SetInt(SavePrefix + "DailyBonusStreakSaveUses", DailyBonusStreakSaveUses);
        PlayerPrefs.SetString(SavePrefix + "LastDailySpinClaimDate", LastDailySpinClaimDate);
        PlayerPrefs.SetString(SavePrefix + "LastSocialHelpRequestDate", LastSocialHelpRequestDate);
        PlayerPrefs.SetInt(SavePrefix + "SocialHelpRequestsUsedToday", SocialHelpRequestsUsedToday);
        PlayerPrefs.SetInt(SavePrefix + "AutoDropPowerUps", AutoDropPowerUps ? 1 : 0);
        PlayerPrefs.SetInt(GetRoomJackpotPotKey(RealmContentCatalog.ActivePrototypeRoom), Mathf.Max(ActiveProgression.MinimumJackpotPot, CurrentRoomJackpotPot));
        PlayerPrefs.SetInt(GetRoomRestoreKey(RealmContentCatalog.ActivePrototypeRoom), ActiveRoomRestored ? 1 : 0);

        SaveRoomIngredients();

        SaveCellRewards();
        SaveAlbumCards();

        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        Mana = DefaultMana;
        Crystals = DefaultCrystals;
        ClairvoyanceMinutes = DefaultClairvoyanceMinutes;
        ActiveClairvoyanceSeconds = 0f;
        ActiveClairvoyanceExpiresAtTicks = 0L;
        PendingJackpotSpins = 0;
        ManaCauldronAmount = DefaultManaCauldronAmount;
        ManaCauldronCapacity = DefaultManaCauldronCapacity;
        ManaCauldronNextFillAtTicks = 0L;
        AlbumSeasonStartedAtTicks = PrototypeGlobalAlbumSeasonStartUtc.Ticks;
        BookOfShadowsPurchased = true;
        JokerWildCards = 0;
        DailyBonusStreak = 0;
        LastDailyBonusClaimDate = "";
        DailyBonusStreakSaveUses = 0;
        LastDailySpinClaimDate = "";
        LastSocialHelpRequestDate = "";
        SocialHelpRequestsUsedToday = 0;
        AutoDropPowerUps = false;
        CurrentRoomJackpotPot = ActiveProgression.MinimumJackpotPot;
        ActiveRoomRestored = false;

        ingredients.Clear();
        ingredients["Gilded Azalea Petals"] = 0;
        ingredients["Sunwarm Dewdrops"] = 0;
        ingredients["Honeyglow Pollen"] = 0;
        ingredients["Amberroot Shavings"] = 0;
        ingredients["Arboretum Heartseed"] = 0;
        cellRewards.Clear();
        grimoireCardCopies.Clear();
        bookOfShadowsCardCopies.Clear();
        SeedStarterPowerUps();

        ClearSavedKeys();
        Save();
    }

    public void ResetRoomProgressKeepingInventory()
    {
        ClearRoomRestoreKeys();
        ClearRoomIngredientKeys();
        ClearRoomJackpotPotKeys();
        ClearRoomManaBetKeys();

        ingredients.Clear();
        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        for (int index = 0; index < requirements.Count; index++)
        {
            ingredients[requirements[index].Name] = 0;
        }

        CurrentRoomJackpotPot = ActiveProgression.MinimumJackpotPot;
        ActiveRoomRestored = false;
        Save();
    }

    public void UnlockRealmForTesting(int realmIndex)
    {
        if (realmIndex <= 0 || realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return;
        }

        IReadOnlyList<RoomDefinition> previousRooms = RealmContentCatalog.AllRealms[realmIndex - 1].Rooms;
        if (previousRooms.Count == 0)
        {
            return;
        }

        PlayerPrefs.SetInt(GetRoomRestoreKey(previousRooms[previousRooms.Count - 1]), 1);
        ActiveRoomRestored = IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom);
        PlayerPrefs.Save();
    }

    public bool TrySpendMana(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (Mana < amount)
        {
            return false;
        }

        Mana -= amount;
        Save();
        return true;
    }

    public void ContributeRoomJackpotFromSpend(int manaSpent)
    {
        if (manaSpent <= 0)
        {
            return;
        }

        int contribution = Mathf.Max(1, Mathf.RoundToInt(manaSpent * ActiveProgression.JackpotContributionRate));
        CurrentRoomJackpotPot = Mathf.Max(ActiveProgression.MinimumJackpotPot, CurrentRoomJackpotPot + contribution);
        Save();
    }

    public bool CanAffordMana(int amount)
    {
        return amount <= Mana;
    }

    public bool TrySpendCrystals(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (Crystals < amount)
        {
            return false;
        }

        Crystals -= amount;
        Save();
        return true;
    }

    public bool CanCollectManaCauldron()
    {
        return ManaCauldronAmount > 0;
    }

    public int CollectManaCauldron()
    {
        int collected = ManaCauldronAmount;
        if (collected <= 0)
        {
            return 0;
        }

        ManaCauldronAmount = 0;
        ManaCauldronNextFillAtTicks = DateTime.UtcNow.AddSeconds(ManaCauldronRefillSeconds).Ticks;
        AddMana(collected);
        Save();
        return collected;
    }

    public bool CanClaimDailyBonus()
    {
        return LastDailyBonusClaimDate != GetTodayClaimDateKey();
    }

    public bool HasMissedDailyBonusStreak()
    {
        if (DailyBonusStreak <= 0 || string.IsNullOrEmpty(LastDailyBonusClaimDate))
        {
            return false;
        }

        if (!DateTime.TryParse(LastDailyBonusClaimDate, out DateTime lastClaimDate))
        {
            return false;
        }

        return lastClaimDate.Date < DateTime.UtcNow.Date.AddDays(-1);
    }

    public int GetDailyBonusStreakSaveCost()
    {
        int costIndex = Mathf.Clamp(DailyBonusStreakSaveUses, 0, DailyBonusStreakSaveCosts.Length - 1);
        return DailyBonusStreakSaveCosts[costIndex];
    }

    public bool CanUseDailyBonusStreakSave()
    {
        return HasMissedDailyBonusStreak() && Crystals >= GetDailyBonusStreakSaveCost();
    }

    public string GetDailyBonusStreakSaveSummary()
    {
        if (!HasMissedDailyBonusStreak())
        {
            return "Streak protected";
        }

        int cost = GetDailyBonusStreakSaveCost();
        return cost <= 0 ? "Save this streak free" : $"Save this streak for {cost} Crystals";
    }

    public bool TryUseDailyBonusStreakSave()
    {
        if (!HasMissedDailyBonusStreak())
        {
            return false;
        }

        int cost = GetDailyBonusStreakSaveCost();
        if (Crystals < cost)
        {
            return false;
        }

        Crystals -= cost;
        DailyBonusStreakSaveUses++;
        LastDailyBonusClaimDate = DateTime.UtcNow.Date.AddDays(-1).ToString("yyyy-MM-dd");
        Save();
        return true;
    }

    public int GetDailyBonusDisplayDay()
    {
        int effectiveStreak = CanClaimDailyBonus() ? DailyBonusStreak + 1 : Mathf.Max(1, DailyBonusStreak);
        return ((effectiveStreak - 1) % 7) + 1;
    }

    public int GetNextDailyBonusChestDay()
    {
        int[] milestones = { 7, 14, 30, 60, 100, 180, 365 };
        for (int index = 0; index < milestones.Length; index++)
        {
            if (DailyBonusStreak < milestones[index])
            {
                return milestones[index];
            }
        }

        int overage = DailyBonusStreak - 365;
        return 365 + ((overage / 365) + 1) * 365;
    }

    public string GetDailyBonusClaimStateText()
    {
        if (CanClaimDailyBonus())
        {
            return $"Ready: Day {GetDailyBonusDisplayDay()}";
        }

        return "Claimed today";
    }

    public DailyBonusRewardDefinition GetDailyBonusRewardForDay(int day)
    {
        int restoredRooms = GetRestoredRoomCount();
        int mana = DailyBonusBaseMana + restoredRooms * DailyBonusManaPerRestoredRoom;
        switch (Mathf.Clamp(day, 1, 7))
        {
            case 1:
                return new DailyBonusRewardDefinition(day, mana, 0, "", 0, "Mana");
            case 2:
                return new DailyBonusRewardDefinition(day, 0, 20, "", 0, "Crystals");
            case 3:
                return new DailyBonusRewardDefinition(day, 0, 0, "Single Sigil", 1, "Power-Up");
            case 4:
                return new DailyBonusRewardDefinition(day, 0, 0, "Clairvoyance", 15, "Timed Boost");
            case 5:
                return new DailyBonusRewardDefinition(day, 0, 0, "Regular Card", 1, "Card");
            case 6:
                return new DailyBonusRewardDefinition(day, 0, 0, "Pandora Sigil", 1, "Curiosity");
            default:
                return new DailyBonusRewardDefinition(day, mana + 250, DailyBonusBaseCrystals + 20, "Pandora Sigil", 1, "Daily Chest");
        }
    }

    public DailyBonusRewardDefinition GetNextDailyBonusReward()
    {
        return GetDailyBonusRewardForDay(GetDailyBonusDisplayDay());
    }

    public DailyBonusRewardDefinition ClaimDailyBonus()
    {
        if (!CanClaimDailyBonus())
        {
            return DailyBonusRewardDefinition.Empty;
        }

        DateTime today = DateTime.UtcNow.Date;
        if (DateTime.TryParse(LastDailyBonusClaimDate, out DateTime lastClaimDate) && lastClaimDate.Date == today.AddDays(-1))
        {
            DailyBonusStreak++;
        }
        else
        {
            DailyBonusStreak = 1;
        }

        DailyBonusRewardDefinition reward = GetDailyBonusRewardForDay(((DailyBonusStreak - 1) % 7) + 1);
        ApplyDailyBonusReward(reward);
        LastDailyBonusClaimDate = GetTodayClaimDateKey();
        Save();
        return reward;
    }

    public bool CanClaimDailySpin()
    {
        return LastDailySpinClaimDate != GetTodayClaimDateKey();
    }

    public string GetDailySpinClaimStateText()
    {
        return CanClaimDailySpin() ? "free spin ready" : "spun today";
    }

    public IReadOnlyList<PrototypeRewardGrant> GetDailySpinPrototypeRewards()
    {
        return DailySpinPrototypeRewards;
    }

    public PrototypeRewardGrant ClaimDailySpin()
    {
        if (!CanClaimDailySpin())
        {
            return new PrototypeRewardGrant("Daily Spin", 0, 0, 0, 0, new List<PrototypeRewardItem>());
        }

        int rewardIndex = UnityEngine.Random.Range(0, DailySpinPrototypeRewards.Length);
        PrototypeRewardGrant reward = DailySpinPrototypeRewards[rewardIndex];
        ApplyRewardGrant(reward);
        LastDailySpinClaimDate = GetTodayClaimDateKey();
        Save();
        return reward;
    }

    public void DevResetDailySpinClaim()
    {
        LastDailySpinClaimDate = "";
        PlayerPrefs.DeleteKey(SavePrefix + "LastDailySpinClaimDate");
        PlayerPrefs.Save();
    }

    public string GetManaCauldronCountdownText()
    {
        SyncManaCauldronFromClock();
        if (ManaCauldronAmount >= ManaCauldronCapacity)
        {
            return "Full - ready to collect";
        }

        if (ManaCauldronNextFillAtTicks <= 0L)
        {
            return "Refill pending";
        }

        return $"Next fill in {GetManaCauldronRemainingTimeText()}";
    }

    public string GetManaCauldronRemainingTimeText()
    {
        if (ManaCauldronNextFillAtTicks <= 0L)
        {
            return "--";
        }

        TimeSpan remaining = new DateTime(ManaCauldronNextFillAtTicks, DateTimeKind.Utc) - DateTime.UtcNow;
        if (remaining.TotalSeconds <= 0)
        {
            return "now";
        }

        int hours = Mathf.FloorToInt((float)remaining.TotalHours);
        int minutes = remaining.Minutes;
        int seconds = remaining.Seconds;
        if (hours > 0)
        {
            return $"{hours}h {minutes:00}m";
        }

        return $"{minutes:00}m {seconds:00}s";
    }

    public bool TickManaCauldron()
    {
        int previousAmount = ManaCauldronAmount;
        long previousTicks = ManaCauldronNextFillAtTicks;
        SyncManaCauldronFromClock();
        return previousAmount != ManaCauldronAmount || previousTicks != ManaCauldronNextFillAtTicks;
    }

    public int GetAlbumSeasonDay()
    {
        double elapsedDays = (DateTime.UtcNow - new DateTime(AlbumSeasonStartedAtTicks, DateTimeKind.Utc)).TotalDays;
        return Mathf.Clamp(Mathf.FloorToInt((float)elapsedDays) + 1, 1, AlbumSeasonDays);
    }

    public int GetAlbumSeasonDaysRemaining()
    {
        double elapsedDays = (DateTime.UtcNow - new DateTime(AlbumSeasonStartedAtTicks, DateTimeKind.Utc)).TotalDays;
        return Mathf.Clamp(Mathf.CeilToInt((float)(AlbumSeasonDays - elapsedDays)), 0, AlbumSeasonDays);
    }

    public int GetActiveBookOfShadowsSetNumber()
    {
        int seasonDay = GetAlbumSeasonDay();
        return Mathf.Clamp(((seasonDay - 1) / BookOfShadowsSetDays) + 1, 1, 3);
    }

    public int GetBookOfShadowsSetDaysRemaining()
    {
        int activeSet = GetActiveBookOfShadowsSetNumber();
        DateTime setEnd = new DateTime(AlbumSeasonStartedAtTicks, DateTimeKind.Utc).AddDays(activeSet * BookOfShadowsSetDays);
        TimeSpan remaining = setEnd - DateTime.UtcNow;
        return Mathf.Clamp(Mathf.CeilToInt((float)remaining.TotalDays), 0, BookOfShadowsSetDays);
    }

    public string GetAlbumSeasonSummaryText()
    {
        return $"Season day {GetAlbumSeasonDay()}/{AlbumSeasonDays} | {GetAlbumSeasonDaysRemaining()} days left";
    }

    public string GetBookOfShadowsSeasonSummaryText()
    {
        return $"Set {GetActiveBookOfShadowsSetNumber()}/3 | {GetBookOfShadowsSetDaysRemaining()} days left";
    }

    public void Redeem(RewardPreview preview)
    {
        lastAwardedAlbumCards.Clear();
        AddMana(preview.ManaReward);
        AddIngredientDrops(preview.IngredientDrops);
        AddCellRewardCounts(preview.CollectedCellRewards);
        AddJackpotSpins(preview.JackpotSpinsEarned);
        Save();
    }

    public bool TryConsumeJackpotSpin()
    {
        if (PendingJackpotSpins <= 0)
        {
            return false;
        }

        PendingJackpotSpins--;
        Save();
        return true;
    }

    public void AddPrototypeJackpotSpin()
    {
        AddJackpotSpins(1);
        Save();
    }

    public void GrantJackpotMana(int amount)
    {
        AddMana(amount);
        Save();
    }

    public void GrantJackpotSpecialMana(int amount)
    {
        AddMana(amount);
        ResetRoomJackpotPot();
        Save();
    }

    public void ResetRoomJackpotPot()
    {
        CurrentRoomJackpotPot = ActiveProgression.MinimumJackpotPot;
    }

    public string GetJackpotSpinText()
    {
        return $"{PendingJackpotSpins} pending";
    }

    public string GetCurrentJackpotPotText()
    {
        return CurrentRoomJackpotPot.ToString("N0");
    }

    public int GetWheelStandardValue(float potMultiplier)
    {
        return RoundWheelValue(CurrentRoomJackpotPot * potMultiplier);
    }

    public int GetJackpotValue()
    {
        return RoundWheelValue(CurrentRoomJackpotPot);
    }

    public int GetEpicValue()
    {
        return RoundWheelValue(CurrentRoomJackpotPot * 2f);
    }

    public int GetLegendaryValue()
    {
        return RoundWheelValue(CurrentRoomJackpotPot * 3f);
    }

    public string GetManaText()
    {
        return Mana.ToString("N0");
    }

    public string GetCrystalText()
    {
        return Crystals.ToString("N0");
    }

    public string GetPowerUpText()
    {
        return GetPowerUpInventoryCount().ToString();
    }

    public string GetClairvoyanceRoomStatusText()
    {
        return HasActiveClairvoyance() ? GetActiveClairvoyanceTimeText() : "OFF";
    }

    public string GetPowerUpInventoryText()
    {
        return $"Mana: {GetManaText()}  |  Crystals: {GetCrystalText()}\n"
            + $"Mana Cauldron: {ManaCauldronAmount}/{ManaCauldronCapacity}\n"
            + $"Cards: Regular {GetInventoryRewardCount("Regular Card")}  Gilded {GetInventoryRewardCount("Gilded Card")}  Ancient {GetInventoryRewardCount("Ancient Card")}  Special {GetInventoryRewardCount("Special Card")}\n"
            + $"Club Orbs: {GetInventoryRewardCount("Club Orbs")}  |  Pandora Chests: {GetInventoryRewardCount("Pandora Sigil")}\n"
            + $"Clairvoyance stock: {ClairvoyanceMinutes}m  |  Active: {(HasActiveClairvoyance() ? GetActiveClairvoyanceTimeText() : "OFF")}\n"
            + $"Single Sigil: {GetInventoryRewardCount("Single Sigil")}\n"
            + $"Multi Sigil: {GetInventoryRewardCount("Multi Sigil")}\n"
            + $"Arcane Spark: {GetInventoryRewardCount("Arcane Spark")}\n"
            + $"Fortune Sigil: {GetInventoryRewardCount("Fortune Sigil")}\n"
            + $"Wild Sigil: {GetInventoryRewardCount("Wild Sigil")}\n"
            + $"Presto Sigil: {GetInventoryRewardCount("Presto Sigil")}";
    }

    public bool HasActiveClairvoyance()
    {
        return ActiveClairvoyanceSeconds > 0.01f;
    }

    public bool CanActivateClairvoyance()
    {
        return ClairvoyanceMinutes >= ClairvoyanceActivationMinutes;
    }

    public bool TryActivateClairvoyance()
    {
        if (!CanActivateClairvoyance())
        {
            return false;
        }

        ClairvoyanceMinutes -= ClairvoyanceActivationMinutes;
        ActiveClairvoyanceSeconds += ClairvoyanceActivationMinutes * 60f;
        ActiveClairvoyanceExpiresAtTicks = DateTime.UtcNow.AddSeconds(ActiveClairvoyanceSeconds).Ticks;
        Save();
        return true;
    }

    public bool TickPowerUps(float deltaSeconds)
    {
        float previousSeconds = ActiveClairvoyanceSeconds;
        SyncTimedPowerUpsFromClock();
        bool cauldronChanged = TickManaCauldron();
        return previousSeconds > 0f || ActiveClairvoyanceSeconds > 0f || cauldronChanged;
    }

    private void SyncManaCauldronFromClock()
    {
        if (ManaCauldronAmount >= ManaCauldronCapacity)
        {
            ManaCauldronNextFillAtTicks = 0L;
            return;
        }

        if (ManaCauldronNextFillAtTicks <= 0L)
        {
            return;
        }

        if (DateTime.UtcNow.Ticks < ManaCauldronNextFillAtTicks)
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        DateTime nextFillAt = new DateTime(ManaCauldronNextFillAtTicks, DateTimeKind.Utc);
        int fillsToApply = Mathf.Max(1, Mathf.FloorToInt((float)((now - nextFillAt).TotalSeconds / ManaCauldronRefillSeconds)) + 1);
        int refillAmount = GetManaCauldronHourlyRefillAmount() * fillsToApply;
        ManaCauldronAmount = Mathf.Min(ManaCauldronCapacity, ManaCauldronAmount + refillAmount);
        ManaCauldronNextFillAtTicks = ManaCauldronAmount >= ManaCauldronCapacity
            ? 0L
            : nextFillAt.AddSeconds(ManaCauldronRefillSeconds * fillsToApply).Ticks;
    }

    private void SyncTimedPowerUpsFromClock()
    {
        if (ActiveClairvoyanceExpiresAtTicks <= 0L)
        {
            ActiveClairvoyanceSeconds = 0f;
            return;
        }

        double remainingSeconds = new DateTime(ActiveClairvoyanceExpiresAtTicks, DateTimeKind.Utc)
            .Subtract(DateTime.UtcNow)
            .TotalSeconds;
        ActiveClairvoyanceSeconds = Mathf.Max(0f, (float)remainingSeconds);
        if (ActiveClairvoyanceSeconds <= 0f)
        {
            ActiveClairvoyanceExpiresAtTicks = 0L;
        }
    }

    public string GetActiveClairvoyanceTimeText()
    {
        int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, ActiveClairvoyanceSeconds));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    public void AddClairvoyanceMinutes(int minutes)
    {
        if (minutes <= 0)
        {
            return;
        }

        ClairvoyanceMinutes += minutes;
        Save();
    }

    public void DevGrantMana(int amount)
    {
        AddMana(amount);
        Save();
    }

    public void DevGrantCrystals(int amount)
    {
        AddCrystals(amount);
        Save();
    }

    public void AddCrystalsForPrototype(int amount)
    {
        AddCrystals(amount);
        Save();
    }

    public string GetIngredientProgressText()
    {
        return GetIngredientProgressText(RealmContentCatalog.ActivePrototypeRoom);
    }

    public string GetIngredientProgressText(RoomDefinition room)
    {
        IReadOnlyList<IngredientRequirement> requirements = room.Ingredients;
        return $"{GetShortIngredientName(requirements[0].Name)} {GetIngredientCountForRoom(requirements[0])}/{requirements[0].Required}\n"
            + $"{GetShortIngredientName(requirements[1].Name)} {GetIngredientCountForRoom(requirements[1])}/{requirements[1].Required}\n"
            + $"{GetShortIngredientName(requirements[2].Name)} {GetIngredientCountForRoom(requirements[2])}/{requirements[2].Required}";
    }

    public string GetFullIngredientProgressText()
    {
        return GetFullIngredientProgressText(RealmContentCatalog.ActivePrototypeRoom);
    }

    public string GetFullIngredientProgressText(RoomDefinition room)
    {
        IReadOnlyList<IngredientRequirement> requirements = room.Ingredients;
        List<string> lines = new List<string>();
        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            lines.Add($"{GetShortIngredientName(requirement.Name)} {GetIngredientCountForRoom(requirement)}/{requirement.Required}");
        }

        return string.Join("\n", lines);
    }

    public string GetRestoreStatusText()
    {
        if (ActiveRoomRestored)
        {
            return "Potion Restored";
        }

        return CanRestoreActiveRoom() ? "Potion Ready" : "Ingredients Needed";
    }

    public string GetRestoreRewardText()
    {
        return $"+{ActiveProgression.RestoreManaReward} Mana\n+1 Card Pack";
    }

    public bool CanRestoreActiveRoom()
    {
        return CanRestoreRoom(RealmContentCatalog.ActivePrototypeRoom);
    }

    public bool CanRestoreRoom(RoomDefinition room)
    {
        if (IsRoomRestored(room))
        {
            return false;
        }

        IReadOnlyList<IngredientRequirement> requirements = room.Ingredients;
        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            if (GetIngredientCountForRoom(requirement) < requirement.Required)
            {
                return false;
            }
        }

        return true;
    }

    public string GetRoomRestoreStatusText(RoomDefinition room)
    {
        if (IsRoomRestored(room))
        {
            return "Restored";
        }

        return CanRestoreRoom(room) ? "Potion Ready" : "In Progress";
    }

    public bool TryRestoreActiveRoom()
    {
        if (!CanRestoreActiveRoom())
        {
            return false;
        }

        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            ingredients[requirement.Name] = GetIngredientCount(requirement.Name) - requirement.Required;
        }

        ActiveRoomRestored = true;
        PlayerPrefs.SetInt(GetRoomRestoreKey(RealmContentCatalog.ActivePrototypeRoom), 1);
        AddMana(ActiveProgression.RestoreManaReward);
        AddCellRewardCount("Regular Card", 1);
        Save();
        return true;
    }

    public string GetCellRewardInventoryText()
    {
        if (cellRewards.Count == 0)
        {
            return "Cards 0\nChests 0\nPower-Ups 0";
        }

        int cardCount = GetCellRewardCount("Regular Card") + GetCellRewardCount("Gilded Card") + GetCellRewardCount("Ancient Card");
        int powerUpCount = GetCellRewardCount("Single Sigil") + GetCellRewardCount("Multi Sigil") + GetCellRewardCount("Arcane Spark")
            + GetCellRewardCount("Fortune Sigil") + GetCellRewardCount("Wild Sigil") + GetCellRewardCount("Presto Sigil") + GetCellRewardCount("Pandora Sigil");
        return $"Cards {cardCount}\nPandora {GetCellRewardCount("Pandora Sigil")}\nPower-Ups {powerUpCount}";
    }

    public int GetPowerUpInventoryCount()
    {
        int clairvoyanceCharges = Mathf.CeilToInt(ClairvoyanceMinutes / 15f);
        return clairvoyanceCharges
            + GetSigilInventoryCount()
            + GetCellRewardCount("Pandora Sigil");
    }

    public int GetInventoryRewardCount(string name)
    {
        return GetCellRewardCount(name);
    }

    public int GetOwnedGrimoireCardCount()
    {
        int owned = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            if (GetGrimoireCardCopies(cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetOwnedGrimoireCardCountByTier(AlbumCardTier tier)
    {
        int owned = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].Tier == tier && GetGrimoireCardCopies(cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetOwnedGrimoireCardCountByStars(int stars)
    {
        int owned = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].Stars == stars && GetGrimoireCardCopies(cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetOwnedGrimoireEntryCardCount(GrimoireEntryDefinition entry)
    {
        int owned = 0;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            if (GetGrimoireCardCopies(entry.Cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetGrimoireCardCopies(string cardId)
    {
        return grimoireCardCopies.TryGetValue(cardId, out int copies) ? copies : 0;
    }

    public bool IsGrimoireCardUnseen(string cardId)
    {
        return PlayerPrefs.GetInt(GetGrimoireCardUnseenKey(cardId), 0) == 1;
    }

    public bool HasUnseenGrimoireEntryCards(GrimoireEntryDefinition entry)
    {
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            if (IsGrimoireCardUnseen(entry.Cards[index].Id))
            {
                return true;
            }
        }

        return false;
    }

    public void MarkGrimoireEntryCardsSeen(GrimoireEntryDefinition entry)
    {
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            PlayerPrefs.DeleteKey(GetGrimoireCardUnseenKey(entry.Cards[index].Id));
        }

        PlayerPrefs.Save();
    }

    public bool IsGrimoireEntryComplete(GrimoireEntryDefinition entry)
    {
        return GetOwnedGrimoireEntryCardCount(entry) >= entry.Cards.Count;
    }

    public bool IsGrimoireAlbumComplete()
    {
        return GetOwnedGrimoireCardCount() >= CardAlbumCatalog.TotalCards;
    }

    public bool IsGrimoireEntryRewardClaimed(GrimoireEntryDefinition entry)
    {
        return PlayerPrefs.GetInt(GetGrimoireEntryRewardClaimedKey(entry), 0) == 1;
    }

    public bool CanClaimGrimoireEntryReward(GrimoireEntryDefinition entry)
    {
        return IsGrimoireEntryComplete(entry) && !IsGrimoireEntryRewardClaimed(entry);
    }

    public bool TryClaimGrimoireEntryReward(GrimoireEntryDefinition entry)
    {
        if (!CanClaimGrimoireEntryReward(entry))
        {
            return false;
        }

        ApplyAlbumReward(entry.Reward);
        PlayerPrefs.SetInt(GetGrimoireEntryRewardClaimedKey(entry), 1);
        Save();
        return true;
    }

    public bool IsGrimoireCompletionRewardClaimed()
    {
        return PlayerPrefs.GetInt(GetGrimoireCompletionRewardClaimedKey(), 0) == 1;
    }

    public bool CanClaimGrimoireCompletionReward()
    {
        return IsGrimoireAlbumComplete() && !IsGrimoireCompletionRewardClaimed();
    }

    public bool TryClaimGrimoireCompletionReward()
    {
        if (!CanClaimGrimoireCompletionReward())
        {
            return false;
        }

        ApplyAlbumReward(CardAlbumCatalog.GrimoireOneCompletionReward);
        ConvertGrimoireDuplicatesToJokerWilds();
        PlayerPrefs.SetInt(GetGrimoireCompletionRewardClaimedKey(), 1);
        Save();
        return true;
    }

    public int GetGrimoireDuplicateCount()
    {
        int duplicates = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            duplicates += Mathf.Max(0, GetGrimoireCardCopies(cards[index].Id) - 1);
        }

        return duplicates;
    }

    public int GetOwnedBookOfShadowsCardCount()
    {
        int owned = 0;
        IReadOnlyList<BookOfShadowsCardDefinition> cards = CardAlbumCatalog.AllBookOfShadowsCards;
        for (int index = 0; index < cards.Count; index++)
        {
            if (GetBookOfShadowsCardCopies(cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetOwnedBookOfShadowsSetCardCount(BookOfShadowsSetDefinition set)
    {
        int owned = 0;
        for (int entryIndex = 0; entryIndex < set.Entries.Count; entryIndex++)
        {
            owned += GetOwnedBookOfShadowsEntryCardCount(set.Entries[entryIndex]);
        }

        return owned;
    }

    public int GetOwnedBookOfShadowsEntryCardCount(BookOfShadowsEntryDefinition entry)
    {
        int owned = 0;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            if (GetBookOfShadowsCardCopies(entry.Cards[index].Id) > 0)
            {
                owned++;
            }
        }

        return owned;
    }

    public int GetBookOfShadowsCardCopies(string cardId)
    {
        return bookOfShadowsCardCopies.TryGetValue(cardId, out int copies) ? copies : 0;
    }

    public bool IsBookOfShadowsCardUnseen(string cardId)
    {
        return PlayerPrefs.GetInt(GetBookOfShadowsCardUnseenKey(cardId), 0) == 1;
    }

    public bool HasUnseenBookOfShadowsEntryCards(BookOfShadowsEntryDefinition entry)
    {
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            if (IsBookOfShadowsCardUnseen(entry.Cards[index].Id))
            {
                return true;
            }
        }

        return false;
    }

    public void MarkBookOfShadowsEntryCardsSeen(BookOfShadowsEntryDefinition entry)
    {
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            PlayerPrefs.DeleteKey(GetBookOfShadowsCardUnseenKey(entry.Cards[index].Id));
        }

        PlayerPrefs.Save();
    }

    public bool IsBookOfShadowsEntryComplete(BookOfShadowsEntryDefinition entry)
    {
        return GetOwnedBookOfShadowsEntryCardCount(entry) >= entry.Cards.Count;
    }

    public bool IsBookOfShadowsSetComplete(BookOfShadowsSetDefinition set)
    {
        return GetOwnedBookOfShadowsSetCardCount(set) >= CardAlbumCatalog.CountBookOfShadowsInSet(set);
    }

    public bool IsBookOfShadowsEntryRewardClaimed(BookOfShadowsEntryDefinition entry)
    {
        return PlayerPrefs.GetInt(GetBookOfShadowsEntryRewardClaimedKey(entry), 0) == 1;
    }

    public bool CanClaimBookOfShadowsEntryReward(BookOfShadowsEntryDefinition entry)
    {
        return BookOfShadowsPurchased && IsBookOfShadowsEntryComplete(entry) && !IsBookOfShadowsEntryRewardClaimed(entry);
    }

    public bool TryClaimBookOfShadowsEntryReward(BookOfShadowsEntryDefinition entry)
    {
        if (!CanClaimBookOfShadowsEntryReward(entry))
        {
            return false;
        }

        ApplyAlbumReward(entry.Reward);
        PlayerPrefs.SetInt(GetBookOfShadowsEntryRewardClaimedKey(entry), 1);
        Save();
        return true;
    }

    public bool IsBookOfShadowsCompletionRewardClaimed()
    {
        return PlayerPrefs.GetInt(GetBookOfShadowsCompletionRewardClaimedKey(), 0) == 1;
    }

    public bool CanClaimBookOfShadowsCompletionReward()
    {
        if (!BookOfShadowsPurchased || IsBookOfShadowsCompletionRewardClaimed())
        {
            return false;
        }

        IReadOnlyList<BookOfShadowsSetDefinition> sets = CardAlbumCatalog.BookOfShadowsSets;
        for (int index = 0; index < sets.Count; index++)
        {
            if (!IsBookOfShadowsSetComplete(sets[index]))
            {
                return false;
            }
        }

        return true;
    }

    public bool TryClaimBookOfShadowsCompletionReward()
    {
        if (!CanClaimBookOfShadowsCompletionReward())
        {
            return false;
        }

        ApplyAlbumReward(CardAlbumCatalog.BookOfShadowsCompletionReward);
        PlayerPrefs.SetInt(GetBookOfShadowsCompletionRewardClaimedKey(), 1);
        Save();
        return true;
    }

    public string GetAlbumCompletionHookText()
    {
        string grimoireStatus = IsGrimoireAlbumComplete()
            ? $"Grimoire complete. Reward: {CardAlbumCatalog.BuildCompactRewardLine(CardAlbumCatalog.GrimoireOneCompletionReward)}. Duplicate Jokers ready: {GetGrimoireDuplicateCount()}."
            : $"Grimoire reward: {CardAlbumCatalog.BuildCompactRewardLine(CardAlbumCatalog.GrimoireOneCompletionReward)}. Duplicates: {GetGrimoireDuplicateCount()}.";
        string shadowsStatus = BookOfShadowsPurchased
            ? $"Book of Shadows reward: {CardAlbumCatalog.BuildCompactRewardLine(CardAlbumCatalog.BookOfShadowsCompletionReward)}."
            : "Book of Shadows locked until purchased.";
        return grimoireStatus + "\n" + shadowsStatus;
    }

    public int GetIngredientInventoryCount(string name)
    {
        return GetIngredientCount(name);
    }

    public void AddIngredientForPrototype(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name) || amount <= 0)
        {
            return;
        }

        ingredients[name] = GetIngredientCount(name) + amount;
        Save();
    }

    public bool TryGiftIngredient(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name) || amount <= 0)
        {
            return false;
        }

        int current = GetIngredientCount(name);
        if (current < amount)
        {
            return false;
        }

        ingredients[name] = current - amount;
        Save();
        return true;
    }

    public int GetDailyHelpRequestLimit()
    {
        return PrototypeDailyHelpRequestLimit;
    }

    public int GetDailyHelpRequestsUsedToday()
    {
        SyncSocialHelpRequestDate();
        return SocialHelpRequestsUsedToday;
    }

    public bool CanSendSocialHelpRequest()
    {
        return GetDailyHelpRequestsUsedToday() < GetDailyHelpRequestLimit();
    }

    public bool TrySendSocialHelpRequest()
    {
        SyncSocialHelpRequestDate();
        if (SocialHelpRequestsUsedToday >= PrototypeDailyHelpRequestLimit)
        {
            return false;
        }

        SocialHelpRequestsUsedToday++;
        LastSocialHelpRequestDate = GetTodayClaimDateKey();
        Save();
        return true;
    }

    public bool TryUseJokerWildForGrimoireCard(string cardId)
    {
        if (string.IsNullOrWhiteSpace(cardId) || JokerWildCards <= 0)
        {
            return false;
        }

        JokerWildCards--;
        int previousCopies = GetGrimoireCardCopies(cardId);
        grimoireCardCopies[cardId] = previousCopies + 1;
        if (previousCopies == 0)
        {
            PlayerPrefs.SetInt(GetGrimoireCardUnseenKey(cardId), 1);
        }

        Save();
        return true;
    }

    public bool TryGiftGrimoireRegularDuplicate(string cardId)
    {
        if (!CanGiftGrimoireRegularDuplicate(cardId))
        {
            return false;
        }

        grimoireCardCopies[cardId] = GetGrimoireCardCopies(cardId) - 1;
        Save();
        return true;
    }

    public bool CanGiftGrimoireRegularDuplicate(string cardId)
    {
        if (!CardAlbumCatalog.TryGetGrimoireCardById(cardId, out AlbumCardDefinition card))
        {
            return false;
        }

        return card.Tier == AlbumCardTier.Regular && GetGrimoireCardCopies(cardId) > 1;
    }

    public bool AddSpecificGrimoireCardForPrototype(string cardId)
    {
        if (!CardAlbumCatalog.TryGetGrimoireCardById(cardId, out _))
        {
            return false;
        }

        AddGrimoireCardCopyWithoutSave(cardId);
        Save();
        return true;
    }

    public void SetAutoDropPowerUps(bool enabled)
    {
        AutoDropPowerUps = enabled;
        Save();
    }

    public string RollEligibleGameplayPowerUp(int selectedCardCount, int manaBetPerCard, int roomLevel)
    {
        List<string> available = new List<string>();
        for (int index = 0; index < GameplayPowerUps.Length; index++)
        {
            GameplayPowerUpTuning powerUp = GameplayPowerUps[index];
            AddEligibleGameplayPowerUp(available, powerUp.Name, powerUp.MinCards, powerUp.MinBet, powerUp.MinLevel, selectedCardCount, manaBetPerCard, roomLevel, powerUp.BankWeight);
        }

        if (available.Count == 0)
        {
            return "";
        }

        return available[UnityEngine.Random.Range(0, available.Count)];
    }

    public List<string> GetEligibleGameplayPowerUps(int selectedCardCount, int manaBetPerCard, int roomLevel)
    {
        List<string> available = new List<string>();
        for (int index = 0; index < GameplayPowerUps.Length; index++)
        {
            GameplayPowerUpTuning powerUp = GameplayPowerUps[index];
            if (powerUp.Name == "Wild Sigil")
            {
                continue;
            }

            if (selectedCardCount >= powerUp.MinCards && manaBetPerCard >= powerUp.MinBet && roomLevel >= powerUp.MinLevel)
            {
                available.Add(powerUp.Name);
            }
        }

        return available;
    }

    public void AddInventoryReward(string name, int amount)
    {
        AddCellRewardCount(name, amount);
        Save();
    }

    public void UnlockBookOfShadowsForPrototype()
    {
        BookOfShadowsPurchased = true;
        Save();
    }

    public void AwardBookOfShadowsCardForPrototype(int amount)
    {
        if (!BookOfShadowsPurchased || amount <= 0)
        {
            return;
        }

        for (int index = 0; index < amount; index++)
        {
            AwardBookOfShadowsCard();
        }

        Save();
    }

    public List<string> OpenPandoraPowerUps()
    {
        int rewardCount = UnityEngine.Random.Range(4, 6);
        List<string> pool = new List<string>();
        for (int index = 0; index < GameplayPowerUps.Length; index++)
        {
            if (GameplayPowerUps[index].CanDropFromPandora)
            {
                pool.Add(GameplayPowerUps[index].Name);
            }
        }

        List<string> granted = new List<string>();
        for (int index = 0; index < rewardCount; index++)
        {
            if (UnityEngine.Random.value < 0.22f)
            {
                int crystalAmount = UnityEngine.Random.Range(2, 6);
                AddCrystals(crystalAmount);
                granted.Add($"Crystals +{crystalAmount}");
                continue;
            }

            string rewardName = pool[UnityEngine.Random.Range(0, pool.Count)];
            AddCellRewardCount(rewardName, 1);
            granted.Add(rewardName);
        }

        Save();
        return granted;
    }

    public bool TryConsumeInventoryReward(string name, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        int current = GetCellRewardCount(name);
        if (current < amount)
        {
            return false;
        }

        cellRewards[name] = current - amount;
        Save();
        return true;
    }

    private int GetSigilInventoryCount()
    {
        return GetCellRewardCount("Single Sigil")
            + GetCellRewardCount("Multi Sigil")
            + GetCellRewardCount("Arcane Spark")
            + GetCellRewardCount("Fortune Sigil")
            + GetCellRewardCount("Wild Sigil")
            + GetCellRewardCount("Presto Sigil");
    }

    private void AddEligibleGameplayPowerUp(List<string> available, string name, int minCards, int minBet, int minLevel, int selectedCardCount, int manaBetPerCard, int roomLevel, int weight)
    {
        if (selectedCardCount < minCards || manaBetPerCard < minBet || roomLevel < minLevel)
        {
            return;
        }

        for (int index = 0; index < weight; index++)
        {
            available.Add(name);
        }
    }

    private void AddIngredientDrops(IReadOnlyList<IngredientDrop> drops)
    {
        for (int index = 0; index < drops.Count; index++)
        {
            IngredientDrop drop = drops[index];
            ingredients[drop.Name] = GetIngredientCount(drop.Name) + drop.Quantity;
        }
    }

    private void ApplyAlbumReward(AlbumRewardDefinition reward)
    {
        AddMana(reward.Mana);
        AddCrystals(reward.Crystals);
        AddClairvoyanceMinutesWithoutSave(reward.ClairvoyanceHours * 60);
        AwardPowerUpBundle(reward.PowerUps);
    }

    private void ApplyDailyBonusReward(DailyBonusRewardDefinition reward)
    {
        ApplyRewardGrant(reward.ToRewardGrant());
    }

    public void ApplyRewardGrant(PrototypeRewardGrant reward)
    {
        AddMana(reward.Mana);
        AddCrystals(reward.Crystals);

        if (reward.ClairvoyanceMinutes > 0)
        {
            AddClairvoyanceMinutesWithoutSave(reward.ClairvoyanceMinutes);
        }

        if (reward.JackpotSpins > 0)
        {
            AddJackpotSpins(reward.JackpotSpins);
        }

        for (int index = 0; index < reward.Items.Count; index++)
        {
            PrototypeRewardItem item = reward.Items[index];
            AddInventoryRewardWithoutSave(item.Name, item.Quantity);
        }

        Save();
    }

    public bool TryRedeemSocialFreebieCode(string code, PrototypeRewardGrant reward, DateTime expiresAtUtc, out string resultMessage)
    {
        string normalizedCode = NormalizeFreebieCode(code);
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            resultMessage = "Freebie link is missing a code.";
            return false;
        }

        if (expiresAtUtc <= DateTime.MinValue)
        {
            resultMessage = "Freebie link is missing a valid expiration.";
            return false;
        }

        if (DateTime.UtcNow >= expiresAtUtc)
        {
            resultMessage = "This social freebie link has expired.";
            return false;
        }

        string redeemedKey = FreebieCodeRedeemedPrefix + normalizedCode;
        if (PlayerPrefs.GetInt(redeemedKey, 0) == 1)
        {
            resultMessage = "This social freebie link was already redeemed.";
            return false;
        }

        ApplyRewardGrant(reward);
        PlayerPrefs.SetInt(redeemedKey, 1);
        PlayerPrefs.Save();
        resultMessage = "Social freebie redeemed.";
        return true;
    }

    private static string NormalizeFreebieCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "";
        }

        string trimmed = code.Trim().ToLowerInvariant();
        char[] chars = new char[trimmed.Length];
        int count = 0;
        for (int index = 0; index < trimmed.Length; index++)
        {
            char c = trimmed[index];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == '_')
            {
                chars[count] = c;
                count++;
            }
        }

        return new string(chars, 0, count);
    }

    private void AwardPowerUpBundle(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        List<string> pool = new List<string>();
        for (int index = 0; index < GameplayPowerUps.Length; index++)
        {
            if (GameplayPowerUps[index].Name == "Wild Sigil")
            {
                continue;
            }

            int weight = Mathf.Max(1, GameplayPowerUps[index].BankWeight);
            for (int weightIndex = 0; weightIndex < weight; weightIndex++)
            {
                pool.Add(GameplayPowerUps[index].Name);
            }
        }

        if (pool.Count == 0)
        {
            return;
        }

        for (int index = 0; index < amount; index++)
        {
            string rewardName = pool[UnityEngine.Random.Range(0, pool.Count)];
            AddCellRewardCount(rewardName, 1);
        }
    }

    private void AddClairvoyanceMinutesWithoutSave(int minutes)
    {
        if (minutes > 0)
        {
            ClairvoyanceMinutes += minutes;
        }
    }

    private void ConvertGrimoireDuplicatesToJokerWilds()
    {
        int converted = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            string cardId = cards[index].Id;
            int copies = GetGrimoireCardCopies(cardId);
            if (copies <= 1)
            {
                continue;
            }

            converted += copies - 1;
            grimoireCardCopies[cardId] = 1;
        }

        JokerWildCards += converted;
    }

    private static string GetTodayClaimDateKey()
    {
        return DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    }

    private void SyncSocialHelpRequestDate()
    {
        string today = GetTodayClaimDateKey();
        if (LastSocialHelpRequestDate == today)
        {
            return;
        }

        LastSocialHelpRequestDate = today;
        SocialHelpRequestsUsedToday = 0;
    }

    private void AddMana(int amount)
    {
        if (amount > 0)
        {
            Mana += amount;
        }
    }

    private void AddJackpotSpins(int amount)
    {
        if (amount > 0)
        {
            PendingJackpotSpins += amount;
        }
    }

    private void AddCellRewardCounts(IReadOnlyList<CellReward> rewards)
    {
        for (int index = 0; index < rewards.Count; index++)
        {
            CellReward reward = rewards[index];
            if (reward.Kind == CellRewardKind.Crystals)
            {
                AddCrystals(reward.Quantity);
                continue;
            }

            if (reward.Kind == CellRewardKind.ClairvoyanceMinutes)
            {
                AddClairvoyanceMinutes(reward.Quantity);
                continue;
            }

            AddCellRewardCount(reward.Name, reward.Quantity);
        }
    }

    private void AddCellRewardCount(string name, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (TryRouteAlbumCardReward(name, amount))
        {
            return;
        }

        cellRewards[name] = GetCellRewardCount(name) + amount;
    }

    private void AddInventoryRewardWithoutSave(string name, int amount)
    {
        if (string.IsNullOrEmpty(name) || amount <= 0)
        {
            return;
        }

        if (name == "Clairvoyance")
        {
            AddClairvoyanceMinutesWithoutSave(amount);
            return;
        }

        AddCellRewardCount(name, amount);
    }

    private bool TryRouteAlbumCardReward(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        if (name.StartsWith(CardAlbumCatalog.SpecificGrimoireCardRewardPrefix))
        {
            string cardId = name.Substring(CardAlbumCatalog.SpecificGrimoireCardRewardPrefix.Length);
            if (!CardAlbumCatalog.TryGetGrimoireCardById(cardId, out _))
            {
                return false;
            }

            for (int index = 0; index < amount; index++)
            {
                AddGrimoireCardCopyWithoutSave(cardId);
            }

            return true;
        }

        AlbumCardTier tier;
        if (name == "Regular Card")
        {
            tier = AlbumCardTier.Regular;
        }
        else if (name == "Gilded Card")
        {
            tier = AlbumCardTier.Gilded;
        }
        else if (name == "Ancient Card")
        {
            tier = AlbumCardTier.Ancient;
        }
        else
        {
            return false;
        }

        for (int index = 0; index < amount; index++)
        {
            AwardGrimoireCard(tier);
        }

        return true;
    }

    private void AddGrimoireCardCopyWithoutSave(string cardId)
    {
        int previousCopies = GetGrimoireCardCopies(cardId);
        grimoireCardCopies[cardId] = previousCopies + 1;
        if (previousCopies == 0)
        {
            PlayerPrefs.SetInt(GetGrimoireCardUnseenKey(cardId), 1);
        }
    }

    private void AwardGrimoireCard(AlbumCardTier tier)
    {
        List<AlbumCardDefinition> pool = new List<AlbumCardDefinition>();
        List<AlbumCardDefinition> missing = new List<AlbumCardDefinition>();
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier != tier)
            {
                continue;
            }

            pool.Add(card);
            if (GetGrimoireCardCopies(card.Id) == 0)
            {
                missing.Add(card);
            }
        }

        if (pool.Count == 0)
        {
            return;
        }

        AlbumCardDefinition awarded = missing.Count > 0
            ? missing[UnityEngine.Random.Range(0, missing.Count)]
            : pool[UnityEngine.Random.Range(0, pool.Count)];
        int previousCopies = GetGrimoireCardCopies(awarded.Id);
        grimoireCardCopies[awarded.Id] = previousCopies + 1;
        bool isNew = previousCopies == 0;
        if (isNew)
        {
            PlayerPrefs.SetInt(GetGrimoireCardUnseenKey(awarded.Id), 1);
        }

        lastAwardedAlbumCards.Add(AwardedAlbumCardRecord.FromGrimoire(awarded, isNew, grimoireCardCopies[awarded.Id]));
    }

    private void AwardBookOfShadowsCard()
    {
        IReadOnlyList<BookOfShadowsSetDefinition> sets = CardAlbumCatalog.BookOfShadowsSets;
        int activeSetIndex = Mathf.Clamp(GetActiveBookOfShadowsSetNumber() - 1, 0, sets.Count - 1);
        BookOfShadowsSetDefinition activeSet = sets[activeSetIndex];
        List<BookOfShadowsCardDefinition> pool = new List<BookOfShadowsCardDefinition>();
        List<BookOfShadowsCardDefinition> missing = new List<BookOfShadowsCardDefinition>();

        for (int entryIndex = 0; entryIndex < activeSet.Entries.Count; entryIndex++)
        {
            IReadOnlyList<BookOfShadowsCardDefinition> cards = activeSet.Entries[entryIndex].Cards;
            for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
            {
                BookOfShadowsCardDefinition card = cards[cardIndex];
                pool.Add(card);
                if (GetBookOfShadowsCardCopies(card.Id) == 0)
                {
                    missing.Add(card);
                }
            }
        }

        if (pool.Count == 0)
        {
            return;
        }

        BookOfShadowsCardDefinition awarded = missing.Count > 0
            ? missing[UnityEngine.Random.Range(0, missing.Count)]
            : pool[UnityEngine.Random.Range(0, pool.Count)];
        int previousCopies = GetBookOfShadowsCardCopies(awarded.Id);
        bookOfShadowsCardCopies[awarded.Id] = previousCopies + 1;
        bool isNew = previousCopies == 0;
        if (isNew)
        {
            PlayerPrefs.SetInt(GetBookOfShadowsCardUnseenKey(awarded.Id), 1);
        }

        lastAwardedAlbumCards.Add(AwardedAlbumCardRecord.FromBookOfShadows(awarded, isNew, bookOfShadowsCardCopies[awarded.Id]));
    }

    private void AddCrystals(int amount)
    {
        if (amount > 0)
        {
            Crystals += amount;
        }
    }

    private void SeedStarterPowerUps()
    {
        cellRewards["Single Sigil"] = DefaultSingleSigils;
        cellRewards["Multi Sigil"] = DefaultMultiSigils;
        cellRewards["Pandora Sigil"] = DefaultPandoraSigils;
    }

    private int GetIngredientCount(string name)
    {
        return ingredients.TryGetValue(name, out int count) ? count : 0;
    }

    private int GetIngredientCountForRoom(IngredientRequirement requirement)
    {
        return PlayerPrefs.GetInt(SavePrefix + "Ingredient." + requirement.Name, GetIngredientCount(requirement.Name));
    }

    private int GetCellRewardCount(string name)
    {
        return cellRewards.TryGetValue(name, out int count) ? count : 0;
    }

    private void LoadIngredient(string name)
    {
        ingredients[name] = PlayerPrefs.GetInt(SavePrefix + "Ingredient." + name, GetIngredientCount(name));
    }

    private void LoadRoomIngredients()
    {
        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        for (int index = 0; index < requirements.Count; index++)
        {
            LoadIngredient(requirements[index].Name);
        }
    }

    private void SaveIngredient(string name)
    {
        PlayerPrefs.SetInt(SavePrefix + "Ingredient." + name, GetIngredientCount(name));
    }

    private void SaveRoomIngredients()
    {
        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        for (int index = 0; index < requirements.Count; index++)
        {
            SaveIngredient(requirements[index].Name);
        }
    }

    private void LoadCellRewards()
    {
        for (int index = 0; index < CellReward.InventoryRewardNames.Count; index++)
        {
            LoadCellReward(CellReward.InventoryRewardNames[index]);
        }
    }

    private void LoadCellReward(string name)
    {
        if (PlayerPrefs.HasKey(SavePrefix + "CellReward." + name))
        {
            cellRewards[name] = PlayerPrefs.GetInt(SavePrefix + "CellReward." + name, 0);
        }
    }

    private void SaveCellRewards()
    {
        for (int index = 0; index < CellReward.InventoryRewardNames.Count; index++)
        {
            SaveCellReward(CellReward.InventoryRewardNames[index]);
        }
    }

    private void SaveCellReward(string name)
    {
        PlayerPrefs.SetInt(SavePrefix + "CellReward." + name, GetCellRewardCount(name));
    }

    private void LoadAlbumCards()
    {
        grimoireCardCopies.Clear();
        IReadOnlyList<AlbumCardDefinition> grimoireCards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < grimoireCards.Count; index++)
        {
            string cardId = grimoireCards[index].Id;
            int copies = PlayerPrefs.GetInt(GetGrimoireCardKey(cardId), 0);
            if (copies > 0)
            {
                grimoireCardCopies[cardId] = copies;
            }
        }

        bookOfShadowsCardCopies.Clear();
        IReadOnlyList<BookOfShadowsCardDefinition> shadowsCards = CardAlbumCatalog.AllBookOfShadowsCards;
        for (int index = 0; index < shadowsCards.Count; index++)
        {
            string cardId = shadowsCards[index].Id;
            int copies = PlayerPrefs.GetInt(GetBookOfShadowsCardKey(cardId), 0);
            if (copies > 0)
            {
                bookOfShadowsCardCopies[cardId] = copies;
            }
        }
    }

    private void SaveAlbumCards()
    {
        IReadOnlyList<AlbumCardDefinition> grimoireCards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < grimoireCards.Count; index++)
        {
            string cardId = grimoireCards[index].Id;
            PlayerPrefs.SetInt(GetGrimoireCardKey(cardId), GetGrimoireCardCopies(cardId));
        }

        IReadOnlyList<BookOfShadowsCardDefinition> shadowsCards = CardAlbumCatalog.AllBookOfShadowsCards;
        for (int index = 0; index < shadowsCards.Count; index++)
        {
            string cardId = shadowsCards[index].Id;
            PlayerPrefs.SetInt(GetBookOfShadowsCardKey(cardId), GetBookOfShadowsCardCopies(cardId));
        }
    }

    private void ClearSavedKeys()
    {
        PlayerPrefs.DeleteKey(SavePrefix + "Mana");
        PlayerPrefs.DeleteKey(SavePrefix + "Crystals");
        PlayerPrefs.DeleteKey(SavePrefix + "ActivePowerUps");
        PlayerPrefs.DeleteKey(SavePrefix + "ClairvoyanceMinutes");
        PlayerPrefs.DeleteKey(SavePrefix + "ActiveClairvoyanceSeconds");
        PlayerPrefs.DeleteKey(SavePrefix + "ActiveClairvoyanceExpiresAtTicks");
        PlayerPrefs.DeleteKey(SavePrefix + "PendingJackpotSpins");
        PlayerPrefs.DeleteKey(SavePrefix + "ManaCauldronAmount");
        PlayerPrefs.DeleteKey(SavePrefix + "ManaCauldronCapacity");
        PlayerPrefs.DeleteKey(SavePrefix + "ManaCauldronNextFillAtTicks");
        PlayerPrefs.DeleteKey(SavePrefix + "BookOfShadowsPurchased");
        PlayerPrefs.DeleteKey(SavePrefix + "JokerWildCards");
        PlayerPrefs.DeleteKey(SavePrefix + "DailyBonusStreak");
        PlayerPrefs.DeleteKey(SavePrefix + "LastDailyBonusClaimDate");
        PlayerPrefs.DeleteKey(SavePrefix + "DailyBonusStreakSaveUses");
        PlayerPrefs.DeleteKey(SavePrefix + "LastDailySpinClaimDate");
        PlayerPrefs.DeleteKey(SavePrefix + "LastSocialHelpRequestDate");
        PlayerPrefs.DeleteKey(SavePrefix + "SocialHelpRequestsUsedToday");
        PlayerPrefs.DeleteKey(SavePrefix + "AutoDropPowerUps");
        PlayerPrefs.DeleteKey(SavePrefix + "RoomJackpotPot");
        ClearRoomJackpotPotKeys();
        ClearRoomRestoreKeys();
        ClearRoomManaBetKeys();

        ClearRoomIngredientKeys();

        ClearCellRewardKeys();
        ClearAlbumCardKeys();
        ClearAlbumRewardClaimKeys();
    }

    private void ClearAlbumCardKeys()
    {
        IReadOnlyList<AlbumCardDefinition> grimoireCards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < grimoireCards.Count; index++)
        {
            PlayerPrefs.DeleteKey(GetGrimoireCardKey(grimoireCards[index].Id));
            PlayerPrefs.DeleteKey(GetGrimoireCardUnseenKey(grimoireCards[index].Id));
        }

        IReadOnlyList<BookOfShadowsCardDefinition> shadowsCards = CardAlbumCatalog.AllBookOfShadowsCards;
        for (int index = 0; index < shadowsCards.Count; index++)
        {
            PlayerPrefs.DeleteKey(GetBookOfShadowsCardKey(shadowsCards[index].Id));
            PlayerPrefs.DeleteKey(GetBookOfShadowsCardUnseenKey(shadowsCards[index].Id));
        }
    }

    private void ClearAlbumRewardClaimKeys()
    {
        IReadOnlyList<GrimoireEntryDefinition> grimoireEntries = CardAlbumCatalog.GrimoireOneEntries;
        for (int index = 0; index < grimoireEntries.Count; index++)
        {
            PlayerPrefs.DeleteKey(GetGrimoireEntryRewardClaimedKey(grimoireEntries[index]));
        }

        PlayerPrefs.DeleteKey(GetGrimoireCompletionRewardClaimedKey());

        IReadOnlyList<BookOfShadowsSetDefinition> shadowSets = CardAlbumCatalog.BookOfShadowsSets;
        for (int setIndex = 0; setIndex < shadowSets.Count; setIndex++)
        {
            IReadOnlyList<BookOfShadowsEntryDefinition> entries = shadowSets[setIndex].Entries;
            for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
            {
                PlayerPrefs.DeleteKey(GetBookOfShadowsEntryRewardClaimedKey(entries[entryIndex]));
            }
        }

        PlayerPrefs.DeleteKey(GetBookOfShadowsCompletionRewardClaimedKey());
    }

    private static string GetGrimoireCardKey(string cardId)
    {
        return SavePrefix + "Album.Grimoire." + cardId;
    }

    private static string GetBookOfShadowsCardKey(string cardId)
    {
        return SavePrefix + "Album.BookOfShadows." + cardId;
    }

    private static string GetGrimoireCardUnseenKey(string cardId)
    {
        return SavePrefix + "Album.Grimoire.Unseen." + cardId;
    }

    private static string GetBookOfShadowsCardUnseenKey(string cardId)
    {
        return SavePrefix + "Album.BookOfShadows.Unseen." + cardId;
    }

    private static string GetGrimoireEntryRewardClaimedKey(GrimoireEntryDefinition entry)
    {
        return SavePrefix + $"Album.Grimoire.EntryRewardClaimed.{entry.GrimoireNumber}.{entry.EntryNumber}";
    }

    private static string GetGrimoireCompletionRewardClaimedKey()
    {
        return SavePrefix + "Album.Grimoire.CompletionRewardClaimed.1";
    }

    private static string GetBookOfShadowsEntryRewardClaimedKey(BookOfShadowsEntryDefinition entry)
    {
        return SavePrefix + $"Album.BookOfShadows.EntryRewardClaimed.{entry.SetNumber}.{entry.EntryNumber}";
    }

    private static string GetBookOfShadowsCompletionRewardClaimedKey()
    {
        return SavePrefix + "Album.BookOfShadows.CompletionRewardClaimed.1";
    }

    private void ClearRoomIngredientKeys()
    {
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            IReadOnlyList<IngredientRequirement> requirements = room.Ingredients;
            for (int index = 0; index < requirements.Count; index++)
            {
                PlayerPrefs.DeleteKey(SavePrefix + "Ingredient." + requirements[index].Name);
            }
        }
    }

    private void ClearCellRewardKeys()
    {
        PlayerPrefs.DeleteKey(SavePrefix + "CellReward.Card");
        PlayerPrefs.DeleteKey(SavePrefix + "CellReward.Double Prize");
        PlayerPrefs.DeleteKey(SavePrefix + "CellReward.Chest");
        for (int index = 0; index < CellReward.InventoryRewardNames.Count; index++)
        {
            PlayerPrefs.DeleteKey(SavePrefix + "CellReward." + CellReward.InventoryRewardNames[index]);
        }
    }

    private void ClearRoomRestoreKeys()
    {
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            PlayerPrefs.DeleteKey(GetRoomRestoreKey(room));
        }
    }

    private void ClearRoomJackpotPotKeys()
    {
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            PlayerPrefs.DeleteKey(GetRoomJackpotPotKey(room));
        }
    }

    private void ClearRoomManaBetKeys()
    {
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            PlayerPrefs.DeleteKey(GetRoomManaBetKey(room));
        }
    }

    private void LoadActiveRoomJackpotPot()
    {
        int minimumPot = ActiveProgression.MinimumJackpotPot;
        CurrentRoomJackpotPot = Mathf.Max(minimumPot, PlayerPrefs.GetInt(GetRoomJackpotPotKey(RealmContentCatalog.ActivePrototypeRoom), minimumPot));
    }

    private static string GetShortIngredientName(string name)
    {
        if (name == "Gilded Azalea Petals") return "Azalea";
        if (name == "Sunwarm Dewdrops") return "Dewdrops";
        if (name == "Honeyglow Pollen") return "Pollen";
        if (name == "Amberroot Shavings") return "Amberroot";
        if (name == "Arboretum Heartseed") return "Heartseed";
        return name;
    }

    private static string GetRoomRestoreKey(RoomDefinition room)
    {
        return SavePrefix + "Restored." + room.Name;
    }

    private static string GetRoomJackpotPotKey(RoomDefinition room)
    {
        return SavePrefix + "RoomJackpotPot." + room.Name;
    }

    private static string GetRoomManaBetKey(RoomDefinition room)
    {
        return SavePrefix + "RoomManaBet." + room.Name;
    }

    private static int RoundWheelValue(float value)
    {
        if (value >= 1000f)
        {
            return Mathf.RoundToInt(value / 100f) * 100;
        }

        if (value >= 250f)
        {
            return Mathf.RoundToInt(value / 25f) * 25;
        }

        return Mathf.RoundToInt(value / 5f) * 5;
    }
}

public sealed class AwardedAlbumCardRecord
{
    private AwardedAlbumCardRecord(string albumName, string cardName, string potionName, string rarityLabel, int stars, bool isNew, int copies)
    {
        AlbumName = albumName;
        CardName = cardName;
        PotionName = potionName;
        RarityLabel = rarityLabel;
        Stars = stars;
        IsNew = isNew;
        Copies = copies;
    }

    public string AlbumName { get; }
    public string CardName { get; }
    public string PotionName { get; }
    public string RarityLabel { get; }
    public int Stars { get; }
    public bool IsNew { get; }
    public int Copies { get; }

    public static AwardedAlbumCardRecord FromGrimoire(AlbumCardDefinition card, bool isNew, int copies)
    {
        return new AwardedAlbumCardRecord("Grimoire", card.CardName, card.PotionName, card.Tier.ToString(), card.Stars, isNew, copies);
    }

    public static AwardedAlbumCardRecord FromBookOfShadows(BookOfShadowsCardDefinition card, bool isNew, int copies)
    {
        return new AwardedAlbumCardRecord("Book of Shadows", card.CardName, card.PotionName, "Regular", card.Stars, isNew, copies);
    }
}

public readonly struct DailyBonusRewardDefinition
{
    public static readonly DailyBonusRewardDefinition Empty = new DailyBonusRewardDefinition(0, 0, 0, "", 0, "");

    public DailyBonusRewardDefinition(int day, int mana, int crystals, string itemName, int itemQuantity, string label)
    {
        Day = day;
        Mana = mana;
        Crystals = crystals;
        ItemName = itemName;
        ItemQuantity = itemQuantity;
        Label = label;
    }

    public int Day { get; }
    public int Mana { get; }
    public int Crystals { get; }
    public string ItemName { get; }
    public int ItemQuantity { get; }
    public string Label { get; }

    public string Summary
    {
        get
        {
            List<string> parts = new List<string>();
            if (Mana > 0)
            {
                parts.Add($"{Mana:N0} Mana");
            }

            if (Crystals > 0)
            {
                parts.Add($"{Crystals:N0} Crystals");
            }

            if (!string.IsNullOrEmpty(ItemName) && ItemQuantity > 0)
            {
                string quantityText = ItemName == "Clairvoyance" ? $"{ItemQuantity}m" : $"x{ItemQuantity}";
                parts.Add($"{ItemName} {quantityText}");
            }

            return parts.Count == 0 ? "No reward" : string.Join(" + ", parts);
        }
    }

    public PrototypeRewardGrant ToRewardGrant()
    {
        List<PrototypeRewardItem> items = new List<PrototypeRewardItem>();
        int clairvoyanceMinutes = 0;
        if (!string.IsNullOrEmpty(ItemName) && ItemQuantity > 0)
        {
            if (ItemName == "Clairvoyance")
            {
                clairvoyanceMinutes = ItemQuantity;
            }
            else
            {
                items.Add(new PrototypeRewardItem(ItemName, ItemQuantity));
            }
        }

        return new PrototypeRewardGrant(Label, Mana, Crystals, clairvoyanceMinutes, 0, items);
    }
}

public readonly struct PrototypeRewardGrant
{
    public PrototypeRewardGrant(string source, int mana, int crystals, int clairvoyanceMinutes, int jackpotSpins, IReadOnlyList<PrototypeRewardItem> items)
    {
        Source = source;
        Mana = mana;
        Crystals = crystals;
        ClairvoyanceMinutes = clairvoyanceMinutes;
        JackpotSpins = jackpotSpins;
        Items = items ?? new List<PrototypeRewardItem>();
    }

    public string Source { get; }
    public int Mana { get; }
    public int Crystals { get; }
    public int ClairvoyanceMinutes { get; }
    public int JackpotSpins { get; }
    public IReadOnlyList<PrototypeRewardItem> Items { get; }
}

public readonly struct PrototypeRewardItem
{
    public PrototypeRewardItem(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    public string Name { get; }
    public int Quantity { get; }
}

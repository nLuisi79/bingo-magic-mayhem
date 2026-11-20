using System;
using UnityEngine;

// Prototype economy and progression tuning in one place instead of scattered constants.
// Defaults below are the values the prototype has always used; Resources/config/economy_tuning.json overrides any of
// them without a code change, and the same shape can later be served from Remote Config.
// TODO(product): every value here is a Beta draft, not approved tuning. See Docs/09_OPEN_DECISIONS.md and
// Docs/12_BETA_1_PRODUCTION_READINESS.md ("Config And Tuning").
[Serializable]
public sealed class EconomyTuning
{
    public const string ResourcePath = "config/economy_tuning";

    public StartingInventorySettings StartingInventory = new StartingInventorySettings();
    public ManaCauldronSettings ManaCauldron = new ManaCauldronSettings();
    public DailyBonusSettings DailyBonus = new DailyBonusSettings();
    public DailySpinRewardEntry[] DailySpinRewards =
    {
        DailySpinRewardEntry.ManaOnly(25), DailySpinRewardEntry.ManaOnly(30), DailySpinRewardEntry.ManaOnly(35), DailySpinRewardEntry.ManaOnly(45),
        DailySpinRewardEntry.ManaOnly(50), DailySpinRewardEntry.ManaOnly(60), DailySpinRewardEntry.ManaOnly(70), DailySpinRewardEntry.ManaOnly(75),
        DailySpinRewardEntry.ManaOnly(80), DailySpinRewardEntry.ManaOnly(90), DailySpinRewardEntry.ManaOnly(100), DailySpinRewardEntry.ManaOnly(120),
        DailySpinRewardEntry.ManaOnly(150), new DailySpinRewardEntry { Crystals = 5 },
        new DailySpinRewardEntry { ItemName = "Single Sigil", ItemQuantity = 1 },
        new DailySpinRewardEntry { ItemName = "Multi Sigil", ItemQuantity = 1 },
    };
    public JackpotWheelSettings JackpotWheel = new JackpotWheelSettings();
    public IngredientDropSettings IngredientDrops = new IngredientDropSettings();
    public IngredientMultiplierSettings IngredientMultiplier = new IngredientMultiplierSettings();
    public RoomProgressionSettings RoomProgression = new RoomProgressionSettings();
    public SocialSettings Social = new SocialSettings();
    public PowerUpSettings PowerUps = new PowerUpSettings();

    private static EconomyTuning current;

    // Loaded on first use. If that first use happens somewhere Unity forbids Resources.Load (a MonoBehaviour constructor
    // or field initializer), the built-in defaults are returned for that call only and the file is loaded next time.
    public static EconomyTuning Current
    {
        get
        {
            if (current != null)
            {
                return current;
            }

            EconomyTuning loaded = Load(out bool loadAllowed);
            if (loadAllowed)
            {
                current = loaded;
            }

            return loaded;
        }
    }

    // How many reads fell back to defaults because they happened during object construction (should stay 0).
    public static int DeferredLoadCount { get; private set; }

    // Test hook: replace (or clear, with null) the active tuning.
    public static void OverrideForTests(EconomyTuning tuning)
    {
        current = tuning;
    }

    public static EconomyTuning Load()
    {
        return Load(out _);
    }

    private static EconomyTuning Load(out bool loadAllowed)
    {
        loadAllowed = true;
        EconomyTuning tuning = new EconomyTuning();
        TextAsset asset;
        try
        {
            asset = Resources.Load<TextAsset>(ResourcePath);
        }
        catch (UnityException exception)
        {
            loadAllowed = false;
            DeferredLoadCount++;
            Debug.LogWarning($"{ResourcePath}.json was requested during object construction; using built-in defaults for this read. Move the read to Awake/Start. {exception.Message}");
            return tuning;
        }

        if (asset == null || string.IsNullOrWhiteSpace(asset.text))
        {
            return tuning;
        }

        try
        {
            JsonUtility.FromJsonOverwrite(asset.text, tuning);
        }
        catch (ArgumentException exception)
        {
            Debug.LogWarning($"Ignoring malformed {ResourcePath}.json; using built-in defaults. {exception.Message}");
            return new EconomyTuning();
        }

        return tuning;
    }

    [Serializable]
    public sealed class StartingInventorySettings
    {
        public int Mana = 5420;
        public int Crystals = 50;
        public int ClairvoyanceMinutes = 0;
        public int SingleSigils = 3;
        public int MultiSigils = 1;
        public int PandoraSigils = 1;
    }

    [Serializable]
    public sealed class ManaCauldronSettings
    {
        public int StartingAmount = 80;
        public int Capacity = 120;
        public int BaseRefill = 40;
        public int RefillPerRestoredRoom = 20;
        public int RefillSeconds = 3600;
    }

    [Serializable]
    public sealed class DailyBonusSettings
    {
        // "Scaled mana" = BaseMana + restored rooms x ManaPerRestoredRoom; days with AddScaledMana add it to Mana.
        public int BaseMana = 300;
        public int ManaPerRestoredRoom = 25;
        public int[] StreakSaveCrystalCosts = { 0, 50, 100, 200, 300 };
        public DailyBonusDayEntry[] Days =
        {
            new DailyBonusDayEntry { AddScaledMana = true, Label = "Mana" },
            new DailyBonusDayEntry { Crystals = 20, Label = "Crystals" },
            new DailyBonusDayEntry { ItemName = "Single Sigil", ItemQuantity = 1, Label = "Power-Up" },
            new DailyBonusDayEntry { ItemName = "Clairvoyance", ItemQuantity = 15, Label = "Timed Boost" },
            new DailyBonusDayEntry { ItemName = "Regular Card", ItemQuantity = 1, Label = "Card" },
            new DailyBonusDayEntry { ItemName = "Pandora Sigil", ItemQuantity = 1, Label = "Curiosity" },
            new DailyBonusDayEntry { AddScaledMana = true, Mana = 250, Crystals = 25, ItemName = "Pandora Sigil", ItemQuantity = 1, Label = "Daily Chest" },
        };
    }

    [Serializable]
    public sealed class DailyBonusDayEntry
    {
        public bool AddScaledMana;
        public int Mana;
        public int Crystals;
        public string ItemName = "";
        public int ItemQuantity;
        public string Label = "";
    }

    [Serializable]
    public sealed class DailySpinRewardEntry
    {
        public int Mana;
        public int Crystals;
        public string ItemName = "";
        public int ItemQuantity;

        public static DailySpinRewardEntry ManaOnly(int amount)
        {
            return new DailySpinRewardEntry { Mana = amount };
        }
    }

    [Serializable]
    public sealed class JackpotWheelSettings
    {
        // Relative odds per segment out of their total (segments 0-7 standard, 8 Jackpot, 9 Epic, 10 Legendary).
        public int[] SegmentWeights = { 18, 14, 14, 12, 11, 10, 8, 7, 3, 2, 1 };
        // Standard segment payouts as a fraction of the room's jackpot pot.
        public float[] StandardMultipliers = { 0.2f, 0.3f, 0.4f, 0.6f, 0.8f, 1f, 1.25f, 1.5f };
    }

    [Serializable]
    public sealed class IngredientDropSettings
    {
        public float CommonWeight = 0.75f;
        public float UncommonWeight = 0.32f;
        public float RareWeight = 0.18f;
        public float KeyIngredientWeight = 0.10f;
        public float OtherWeight = 0.5f;
    }

    // Locked rule (LOCKED_DECISIONS.md, Ingredients and Potion Restoration): the card potion multiplier follows the mana
    // bet. Past DoubleAbove of the room's max bet -> 2x, past TripleAbove -> 3x, the max bet -> MaxMultiplier (4x).
    // TODO(product): "of possible bet" is read as a fraction of the room's max bet; confirm vs. position in the min-max range.
    [Serializable]
    public sealed class IngredientMultiplierSettings
    {
        public float DoubleAboveBetFraction = 0.5f;
        public float TripleAboveBetFraction = 0.75f;
        public int MaxMultiplier = 4;
    }

    [Serializable]
    public sealed class RoomProgressionSettings
    {
        // Per-room values derive from realm number (1-based) and room number (1-4, room 4 = special).
        public int MinBetBase = 25;
        public int MinBetPerRealm = 25;
        public int StandardMaxBetMultiplier = 6;
        public int SpecialMaxBetMultiplier = 8;
        public int BetStep = 25;
        public int LargeBetStep = 50;
        public int LargeBetStepFromRealm = 4;
        public int MinimumPotBase = 250;
        public int MinimumPotPerRealm = 250;
        public int MinimumPotPerRoom = 125;
        public float JackpotContributionBase = 0.10f;
        public float JackpotContributionPerRealm = 0.01f;
        public float JackpotContributionSpecialBonus = 0.03f;
        public float XpMultiplierPerLevel = 0.08f;
        public float StandardNoBingoIngredientChance = 0.022f;
        public float SpecialNoBingoIngredientChance = 0.016f;
        public float StandardCellRewardChance = 0.75f;
        public float SpecialCellRewardChance = 0.60f;
        public int RestoreManaBase = 1000;
        public int RestoreManaPerRealm = 500;
        public int RestoreManaPerRoom = 250;
    }

    [Serializable]
    public sealed class SocialSettings
    {
        public int DailyHelpRequestLimit = 7;
        public int FriendManaAmount = 10;
        public int FriendDailyManaLimit = 3;
        public int FreebieExpirationDays = 14;
    }

    [Serializable]
    public sealed class PowerUpSettings
    {
        public int CrystalCost = 10;
        public float WildSigilOfferChance = 1f / 15f;
        public int ClairvoyanceActivationMinutes = 15;
    }
}

using System.Collections.Generic;

public sealed class RealmDefinition
{
    public RealmDefinition(string name, IReadOnlyList<RoomDefinition> rooms)
    {
        Name = name;
        Rooms = rooms;
    }

    public string Name { get; private set; }
    public IReadOnlyList<RoomDefinition> Rooms { get; private set; }
}

public sealed class RoomDefinition
{
    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients)
        : this(name, potionName, ingredients, BingoRoomMode.Standard, "Standard", RoomProgressionProfile.LevelOne)
    {
    }

    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, BingoRoomMode mode, string modeLabel)
        : this(name, potionName, ingredients, mode, modeLabel, RoomProgressionProfile.LevelOne)
    {
    }

    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, RoomProgressionProfile progression)
        : this(name, potionName, ingredients, BingoRoomMode.Standard, "Standard", progression)
    {
    }

    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, BingoRoomMode mode, string modeLabel, RoomProgressionProfile progression)
    {
        Name = name;
        PotionName = potionName;
        Ingredients = ingredients;
        Mode = mode;
        ModeLabel = modeLabel;
        Progression = progression;
    }

    public string Name { get; private set; }
    public string PotionName { get; private set; }
    public IReadOnlyList<IngredientRequirement> Ingredients { get; private set; }
    public BingoRoomMode Mode { get; private set; }
    public string ModeLabel { get; private set; }
    public RoomProgressionProfile Progression { get; private set; }
    public bool IsSpecial => Mode == BingoRoomMode.Special;
}

public sealed class RoomProgressionProfile
{
    private static RoomProgressionProfile levelOne;

    // Lazy so that touching this type never loads the tuning Resource from a static initializer.
    public static RoomProgressionProfile LevelOne => levelOne ??= Create(1, 1, 0);

    public RoomProgressionProfile(
        int level,
        int minManaBet,
        int maxManaBet,
        int betStep,
        int minimumJackpotPot,
        float jackpotContributionRate,
        float xpMultiplier,
        float noBingoIngredientDropChance,
        float cellRewardChance,
        int restoreManaReward)
    {
        Level = level;
        MinManaBet = minManaBet;
        MaxManaBet = maxManaBet;
        BetStep = betStep;
        MinimumJackpotPot = minimumJackpotPot;
        JackpotContributionRate = jackpotContributionRate;
        XpMultiplier = xpMultiplier;
        NoBingoIngredientDropChance = noBingoIngredientDropChance;
        CellRewardChance = cellRewardChance;
        RestoreManaReward = restoreManaReward;
    }

    public int Level { get; private set; }
    public int MinManaBet { get; private set; }
    public int MaxManaBet { get; private set; }
    public int BetStep { get; private set; }
    public int MinimumJackpotPot { get; private set; }
    public float JackpotContributionRate { get; private set; }
    public float XpMultiplier { get; private set; }
    public float NoBingoIngredientDropChance { get; private set; }
    public float CellRewardChance { get; private set; }
    public int RestoreManaReward { get; private set; }

    // realmIndex is the 1-based realm number; constants come from EconomyTuning.RoomProgression.
    public static RoomProgressionProfile Create(int realmIndex, int roomIndex, int roomOffset)
    {
        EconomyTuning.RoomProgressionSettings tuning = EconomyTuning.Current.RoomProgression;
        int level = ((realmIndex - 1) * 4) + roomIndex;
        bool special = roomIndex == 4;
        int minBet = tuning.MinBetBase + ((realmIndex - 1) * tuning.MinBetPerRealm);
        int maxBet = minBet * (special ? tuning.SpecialMaxBetMultiplier : tuning.StandardMaxBetMultiplier);
        int betStep = realmIndex >= tuning.LargeBetStepFromRealm ? tuning.LargeBetStep : tuning.BetStep;
        int minimumPot = tuning.MinimumPotBase + ((realmIndex - 1) * tuning.MinimumPotPerRealm) + (roomOffset * tuning.MinimumPotPerRoom);
        float contribution = tuning.JackpotContributionBase + (realmIndex * tuning.JackpotContributionPerRealm) + (special ? tuning.JackpotContributionSpecialBonus : 0f);
        float xp = 1f + ((level - 1) * tuning.XpMultiplierPerLevel);
        // A Bingo always earns an ingredient. This only controls the rare
        // consolation ingredient roll for cards that finish without a Bingo.
        float noBingoIngredient = special ? tuning.SpecialNoBingoIngredientChance : tuning.StandardNoBingoIngredientChance;
        float cellReward = special ? tuning.SpecialCellRewardChance : tuning.StandardCellRewardChance;
        int restoreReward = tuning.RestoreManaBase + ((realmIndex - 1) * tuning.RestoreManaPerRealm) + (roomOffset * tuning.RestoreManaPerRoom);
        return new RoomProgressionProfile(level, minBet, maxBet, betStep, minimumPot, contribution, xp, noBingoIngredient, cellReward, restoreReward);
    }
}

public sealed class IngredientRequirement
{
    public IngredientRequirement(string name, int required, string rarity, string description)
    {
        Name = name;
        Required = required;
        Rarity = rarity;
        Description = description;
    }

    public string Name { get; private set; }
    public int Required { get; private set; }
    public string Rarity { get; private set; }
    public string Description { get; private set; }
}

public static class RealmContentCatalog
{
    public static int ActivePrototypeRealmIndex { get; private set; }
    public static int ActivePrototypeRoomIndex { get; private set; }

    // Realm, room, potion and ingredient names and amounts follow the BMM Potions Google Sheet, the current source of truth.
    // Rarities follow the sheet's Rarity column (filled 2026-11-20). Room modes and progression are per-position (room 4 =
    // Blackout). PotionsSheetCatalogTests checks this file against the sheet snapshot.
    public static readonly IReadOnlyList<RealmDefinition> AllRealms = new List<RealmDefinition>
    {
        new RealmDefinition("Sunpetal Conservatory",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Gilded Azalea Arboretum",
                    "Azalea Sunmend Essence",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Gilded Azalea Petals", 5, "Common", ""),
                        new IngredientRequirement("Sunwarm Dewdrops", 4, "Common", ""),
                        new IngredientRequirement("Honeyglow Pollen", 4, "Common", ""),
                        new IngredientRequirement("Amberroot Shavings", 6, "Uncommon", ""),
                        new IngredientRequirement("Golden Heartseed", 6, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(1, 1, 0)),
                new RoomDefinition(
                    "Thorn-Tangled Terrace",
                    "Thornweave Taming Elixir",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Velvet Thorn Tips", 8, "Common", ""),
                        new IngredientRequirement("Terrace Moss Tufts", 9, "Common", ""),
                        new IngredientRequirement("Rosegold Sap Drops", 6, "Common", ""),
                        new IngredientRequirement("Thornheart Bud", 8, "Uncommon", ""),
                        new IngredientRequirement("Bramblelace Fibers", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(1, 2, 1)),
                new RoomDefinition(
                    "Pollenspire Fountain",
                    "Pollenspire Renewal Tonic",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Spire Pollen Grains", 7, "Common", ""),
                        new IngredientRequirement("Fountain Dewdrops", 5, "Common", ""),
                        new IngredientRequirement("Nectarflow Ribbons", 6, "Common", ""),
                        new IngredientRequirement("Lilygold Filaments", 7, "Uncommon", ""),
                        new IngredientRequirement("Pollen Dust", 4, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(1, 3, 2)),
                new RoomDefinition(
                    "Solstice Bloom Glasshouse",
                    "Solstice Revival Tincture",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Sunlit Glass Slivers", 7, "Common", ""),
                        new IngredientRequirement("Golden Hour Motes", 8, "Common", ""),
                        new IngredientRequirement("Prismvine Twine", 8, "Uncommon", ""),
                        new IngredientRequirement("Dawnmirror Fragments", 5, "Uncommon", ""),
                        new IngredientRequirement("Golden Stamen", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(1, 4, 3)),
            }),
        new RealmDefinition("Floating Lotus Lagoon",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Dewdrop Marina",
                    "Marina Mist Tonic",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Dewdrop Pearl", 7, "Common", ""),
                        new IngredientRequirement("Marina Reed", 6, "Common", ""),
                        new IngredientRequirement("Mistpetal Drop", 6, "Common", ""),
                        new IngredientRequirement("Driftwood Fragment", 5, "Uncommon", ""),
                        new IngredientRequirement("Silver Koi Scale", 5, "Uncommon", ""),
                        new IngredientRequirement("Pearlmist Shell", 4, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 1, 0)),
                new RoomDefinition(
                    "Waterlily Canal",
                    "Waterlily Flow Draught",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Waterlily Silk", 7, "Common", ""),
                        new IngredientRequirement("Canal Current Drop", 7, "Common", ""),
                        new IngredientRequirement("Lotusstem Reed", 6, "Common", ""),
                        new IngredientRequirement("Ripplemint Leaf", 6, "Uncommon", ""),
                        new IngredientRequirement("Blue Koi Scale", 5, "Uncommon", ""),
                        new IngredientRequirement("Canal Pearl", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 2, 1)),
                new RoomDefinition(
                    "Opalwater Glasswalk",
                    "Opalwater Clarity Potion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Opalwater Essence", 8, "Common", ""),
                        new IngredientRequirement("Glasswalk Shard", 7, "Common", ""),
                        new IngredientRequirement("Prism Dew", 7, "Common", ""),
                        new IngredientRequirement("Lotuslight Dust", 6, "Common", ""),
                        new IngredientRequirement("Clearpath Crystal", 6, "Uncommon", ""),
                        new IngredientRequirement("Opalstone", 5, "Uncommon", ""),
                        new IngredientRequirement("Mirrored Strand", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 3, 2)),
                new RoomDefinition(
                    "Glasswater Grotto",
                    "Grotto Glow Elixir",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Glasswater Shard", 8, "Common", ""),
                        new IngredientRequirement("Grotto Glowcap", 8, "Common", ""),
                        new IngredientRequirement("Opalwater Essence", 7, "Uncommon", ""),
                        new IngredientRequirement("Glimmerfin Scale", 7, "Uncommon", ""),
                        new IngredientRequirement("Floating Lotus Bloom", 6, "Uncommon", ""),
                        new IngredientRequirement("Stillwater Teardrop", 6, "Rare", ""),
                        new IngredientRequirement("Pearlglow Shard", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(2, 4, 3)),
            }),
        new RealmDefinition("Everbloom Sanctuary",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Eternal Bloomspout Spring",
                    "Gardener’s Grace Mending Balm",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Graceyard Soil Cakes", 9, "Common", ""),
                        new IngredientRequirement("Moonlit Ripple Lace", 6, "Common", ""),
                        new IngredientRequirement("Terracotta Mender’s Clay", 8, "Common", ""),
                        new IngredientRequirement("Rootwax Binding Cord", 8, "Uncommon", ""),
                        new IngredientRequirement("Keepsake Clover", 4, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 1, 0)),
                new RoomDefinition(
                    "Luminara Garden",
                    "Luminara Flow Essence",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Luminara Waterglass", 8, "Common", ""),
                        new IngredientRequirement("Silver Basin Coins", 6, "Common", ""),
                        new IngredientRequirement("Polished Trowel Charms", 7, "Common", ""),
                        new IngredientRequirement("Bluebell Chime Shells", 7, "Uncommon", ""),
                        new IngredientRequirement("Luminous Springcore", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 2, 1)),
                new RoomDefinition(
                    "Primrose Petal Sanctuary",
                    "Blossomveil Awakening Tonic",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Veilpetal Blossoms", 7, "Common", ""),
                        new IngredientRequirement("Roselight Essence", 4, "Common", ""),
                        new IngredientRequirement("Marblemoss Threads", 5, "Common", ""),
                        new IngredientRequirement("Petalsheen Dust", 7, "Uncommon", ""),
                        new IngredientRequirement("Heartbloom Petals", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 3, 2)),
                new RoomDefinition(
                    "Everbloom Shrine",
                    "Heartbloom Revival Mist",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Frosted Petal Plates", 5, "Common", ""),
                        new IngredientRequirement("Sanctuary Lightwick", 8, "Common", ""),
                        new IngredientRequirement("Violet Geode Nectar", 7, "Uncommon", ""),
                        new IngredientRequirement("Halofern Ribbons", 6, "Uncommon", ""),
                        new IngredientRequirement("Gilded Bloom Sigils", 7, "Uncommon", ""),
                        new IngredientRequirement("Everheart Bud", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(3, 4, 3)),
            }),
        new RealmDefinition("Glimmerwisp Forest",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Lanternleaf Path",
                    "Lanternleaf Wayfinder Essence",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Lanternleaf Sprig", 8, "Common", ""),
                        new IngredientRequirement("Wispglow Dew", 7, "Common", ""),
                        new IngredientRequirement("Pathmoss Thread", 7, "Common", ""),
                        new IngredientRequirement("Fernlight Dust", 6, "Uncommon", ""),
                        new IngredientRequirement("Glowroot Chip", 6, "Uncommon", ""),
                        new IngredientRequirement("Silvermoth Wing", 6, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(4, 1, 0)),
                new RoomDefinition(
                    "Glimmerfern Grove",
                    "Glimmerfern Vitality Serum",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Glimmerfern Frond", 8, "Common", ""),
                        new IngredientRequirement("Emerald Fernsap", 7, "Common", ""),
                        new IngredientRequirement("Groveglow Nectar", 7, "Common", ""),
                        new IngredientRequirement("Curling Fernshoot", 6, "Uncommon", ""),
                        new IngredientRequirement("Verdant Pulse Seed", 6, "Uncommon", ""),
                        new IngredientRequirement("Softspore Pollen", 6, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(4, 2, 1)),
                new RoomDefinition(
                    "Glowcap Thicket",
                    "Glowcap Bramble Balm",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Glowcap Mushroom", 9, "Common", ""),
                        new IngredientRequirement("Thicketvine Curl", 8, "Common", ""),
                        new IngredientRequirement("Bramblelight Thorn", 7, "Common", ""),
                        new IngredientRequirement("Sporeglow Powder", 7, "Common", ""),
                        new IngredientRequirement("Tanglecap Resin", 6, "Uncommon", ""),
                        new IngredientRequirement("Mushroom Veilcap", 6, "Uncommon", ""),
                        new IngredientRequirement("Briarsoft Moss", 6, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(4, 3, 2)),
                new RoomDefinition(
                    "Wispwillow Hollow",
                    "Hollowbloom Distillate",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Wispwillow Bark", 9, "Common", ""),
                        new IngredientRequirement("Hollowlight Seed", 9, "Common", ""),
                        new IngredientRequirement("Willowwisp Sap", 8, "Uncommon", ""),
                        new IngredientRequirement("Hollowbloom Petal", 8, "Uncommon", ""),
                        new IngredientRequirement("Ancient Rootdrop", 7, "Uncommon", ""),
                        new IngredientRequirement("Wispnest Fiber", 7, "Rare", ""),
                        new IngredientRequirement("Heartwood Dew", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(4, 4, 3)),
            }),
        new RealmDefinition("Cerulean Moon Monastery",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Bluebell Luminary Alcove",
                    "Bluebell Luminary Dew",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Bluebell Glowdrop", 9, "Common", ""),
                        new IngredientRequirement("Luminary Wick", 8, "Common", ""),
                        new IngredientRequirement("Moonmoss", 8, "Common", ""),
                        new IngredientRequirement("Cerulean Pollen", 7, "Uncommon", ""),
                        new IngredientRequirement("Quietlight Pearl", 7, "Uncommon", ""),
                        new IngredientRequirement("Silverbell Petal", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(5, 1, 0)),
                new RoomDefinition(
                    "Cerulean Lantern Courtyard",
                    "Cerulean Lantern Infusion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Cerulean Oil", 9, "Common", ""),
                        new IngredientRequirement("Starpetal", 8, "Common", ""),
                        new IngredientRequirement("Blueflame Glass", 8, "Common", ""),
                        new IngredientRequirement("Moonstone Thread", 7, "Uncommon", ""),
                        new IngredientRequirement("Lanternseed Husk", 7, "Uncommon", ""),
                        new IngredientRequirement("Silverstep Dust", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(5, 2, 1)),
                new RoomDefinition(
                    "Skyglass Scriptorium",
                    "Skyglass Scroll Reagent",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Skyglass Shard", 10, "Common", ""),
                        new IngredientRequirement("Starink Vial", 9, "Common", ""),
                        new IngredientRequirement("Scrollfiber", 8, "Common", ""),
                        new IngredientRequirement("Quillmoon Feather", 8, "Common", ""),
                        new IngredientRequirement("Glyphdust", 7, "Uncommon", ""),
                        new IngredientRequirement("Sealblue Powder", 7, "Uncommon", ""),
                        new IngredientRequirement("Silverseal Wax", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(5, 3, 2)),
                new RoomDefinition(
                    "Starwell Bell Tower",
                    "Starwell Chime Revival",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Starwell Drop", 10, "Common", ""),
                        new IngredientRequirement("Bellchime Crystal", 10, "Common", ""),
                        new IngredientRequirement("Moonmetal", 9, "Uncommon", ""),
                        new IngredientRequirement("Celestial Cord", 9, "Uncommon", ""),
                        new IngredientRequirement("Echo Pearl", 8, "Uncommon", ""),
                        new IngredientRequirement("Chime Fragment", 8, "Rare", ""),
                        new IngredientRequirement("Cerulean Resonance Dust", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(5, 4, 3)),
            }),
        new RealmDefinition("Emberglass Grove",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Amberleaf Arbor",
                    "Amberleaf Sunbrew",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Amberleaf Cluster", 10, "Common", ""),
                        new IngredientRequirement("Sunamber Dew", 9, "Common", ""),
                        new IngredientRequirement("Glassbark Sliver", 9, "Common", ""),
                        new IngredientRequirement("Rosegold Pollen", 8, "Uncommon", ""),
                        new IngredientRequirement("Kindleseed Husk", 8, "Uncommon", ""),
                        new IngredientRequirement("Warmroot Sap", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(6, 1, 0)),
                new RoomDefinition(
                    "Cinderglass Path",
                    "Cinderglass Mending Salve",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Cinderglass Shard", 10, "Common", ""),
                        new IngredientRequirement("Clearflame Drop", 9, "Common", ""),
                        new IngredientRequirement("Pathglow Pebble", 9, "Common", ""),
                        new IngredientRequirement("Ashlace Thread", 8, "Uncommon", ""),
                        new IngredientRequirement("Emberpolish Resin", 8, "Uncommon", ""),
                        new IngredientRequirement("Smokeveil Dust", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(6, 2, 1)),
                new RoomDefinition(
                    "Sparkstone Garden",
                    "Sparkstone Bloom Catalyst",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Sparkstone Pebble", 11, "Common", ""),
                        new IngredientRequirement("Flamepetal Bloom", 10, "Common", ""),
                        new IngredientRequirement("Amber Emberseed", 9, "Common", ""),
                        new IngredientRequirement("Glowglass Nectar", 9, "Common", ""),
                        new IngredientRequirement("Coppershine Moss", 8, "Uncommon", ""),
                        new IngredientRequirement("Sunflare Pollen", 8, "Uncommon", ""),
                        new IngredientRequirement("Warmthistle Thorn", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(6, 3, 2)),
                new RoomDefinition(
                    "Embervein Rootway",
                    "Embervein Rekindling Remedy",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Embervein Root", 11, "Common", ""),
                        new IngredientRequirement("Heartwood Ember", 11, "Common", ""),
                        new IngredientRequirement("Rootglass Thread", 10, "Uncommon", ""),
                        new IngredientRequirement("Deepwarm Resin", 10, "Uncommon", ""),
                        new IngredientRequirement("Crimson Sapstone", 9, "Uncommon", ""),
                        new IngredientRequirement("Emberline Crystal", 9, "Rare", ""),
                        new IngredientRequirement("Glowcoal Fragment", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(6, 4, 3)),
            }),
        new RealmDefinition("Howling Hollows Manor",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Mosswrought Gateway",
                    "Rustbreaker Revival",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Rusted Iron Filigree", 11, "Common", ""),
                        new IngredientRequirement("Mosswrought Fragment", 10, "Common", ""),
                        new IngredientRequirement("Gatevine Oil", 10, "Common", ""),
                        new IngredientRequirement("Tarnishroot Flake", 9, "Uncommon", ""),
                        new IngredientRequirement("Ironlace Charm", 9, "Uncommon", ""),
                        new IngredientRequirement("Weathered Iron Dust", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(7, 1, 0)),
                new RoomDefinition(
                    "Hollowmoss Topiary Grounds",
                    "Topiary Teardrop Nectar",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Topiary Teardrop", 11, "Common", ""),
                        new IngredientRequirement("Sculpted Mossleaf", 10, "Common", ""),
                        new IngredientRequirement("Hedgecurl Sprig", 10, "Common", ""),
                        new IngredientRequirement("Keeper’s Thread", 9, "Uncommon", ""),
                        new IngredientRequirement("Velvetmoss Pearl", 9, "Uncommon", ""),
                        new IngredientRequirement("Trimmed Thornbud", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(7, 2, 1)),
                new RoomDefinition(
                    "Hollowheart Hall",
                    "Rootbind Restorative",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Rootsnare Strand", 12, "Common", ""),
                        new IngredientRequirement("Hearthwood Splinter", 11, "Common", ""),
                        new IngredientRequirement("Binding Vinecurl", 11, "Common", ""),
                        new IngredientRequirement("Hallglow Ember", 10, "Common", ""),
                        new IngredientRequirement("Twisted Root Resin", 10, "Uncommon", ""),
                        new IngredientRequirement("Heartwood Dust", 9, "Uncommon", ""),
                        new IngredientRequirement("Velvetshade Moss", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(7, 3, 2)),
                new RoomDefinition(
                    "Elderhollow Labyrinth",
                    "Wayward Soul’s Elixir",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Wayward Soul Drop", 12, "Common", ""),
                        new IngredientRequirement("Elderroot Needle", 12, "Common", ""),
                        new IngredientRequirement("Labyrinth Ivy Thread", 11, "Uncommon", ""),
                        new IngredientRequirement("Shadowmoss Pearl", 11, "Uncommon", ""),
                        new IngredientRequirement("Pathmark Shard", 10, "Uncommon", ""),
                        new IngredientRequirement("Lanternless Dew", 10, "Rare", ""),
                        new IngredientRequirement("Labyrinth Seed", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(7, 4, 3)),
            }),
        new RealmDefinition("Crimson Moon Canyon",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Rubyrock Pass",
                    "Rockheart Tincture",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Rockheart Stone Fragment", 12, "Common", ""),
                        new IngredientRequirement("Rubyrock Splinter", 11, "Common", ""),
                        new IngredientRequirement("Crimson Mineral Dew", 11, "Common", ""),
                        new IngredientRequirement("Glowvein Fragment", 10, "Uncommon", ""),
                        new IngredientRequirement("Canyonroot Resin", 10, "Uncommon", ""),
                        new IngredientRequirement("Scarlet Quartz Dust", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(8, 1, 0)),
                new RoomDefinition(
                    "Coppercliff Crossing",
                    "Bridgemender Brew",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Copperstone Fragment", 12, "Common", ""),
                        new IngredientRequirement("Bridgevine Thread", 12, "Common", ""),
                        new IngredientRequirement("Copperheart Sap", 11, "Common", ""),
                        new IngredientRequirement("Spanstone Pebble", 11, "Uncommon", ""),
                        new IngredientRequirement("Rivethorn Bud", 10, "Uncommon", ""),
                        new IngredientRequirement("Copper Glowdrop", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(8, 2, 1)),
                new RoomDefinition(
                    "Stardust Ravine",
                    "Twilight Trail Catalyst",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Twilight Dust", 13, "Common", ""),
                        new IngredientRequirement("Starseed", 12, "Common", ""),
                        new IngredientRequirement("Trailglimmer Sand", 12, "Common", ""),
                        new IngredientRequirement("Nightglass Flake", 11, "Uncommon", ""),
                        new IngredientRequirement("Falling Star Petal", 11, "Uncommon", ""),
                        new IngredientRequirement("Duskwind Thread", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(8, 3, 2)),
                new RoomDefinition(
                    "Moonflare Mesa",
                    "Starlight Prism Potion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Starlight Prism Shard", 13, "Common", ""),
                        new IngredientRequirement("Moonlit Flaredrop", 13, "Common", ""),
                        new IngredientRequirement("Crimsonlight Petal", 12, "Uncommon", ""),
                        new IngredientRequirement("Glowstone", 12, "Uncommon", ""),
                        new IngredientRequirement("Prismroot Strand", 11, "Uncommon", ""),
                        new IngredientRequirement("Skyfire Dust", 10, "Rare", ""),
                        new IngredientRequirement("Lunar Quartz Bloom", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(8, 4, 3)),
            }),
        new RealmDefinition("Arcane Aerie Heights",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Arcane Aerie Gate",
                    "Skybound Counter Infusion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Skybound Vapor", 13, "Common", ""),
                        new IngredientRequirement("Countercloud Drop", 12, "Common", ""),
                        new IngredientRequirement("Skycrag Stonechip", 12, "Common", ""),
                        new IngredientRequirement("Gateweight Pebble", 11, "Uncommon", ""),
                        new IngredientRequirement("Anchorroot Dust", 10, "Uncommon", ""),
                        new IngredientRequirement("Cloudweight Resin", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(9, 1, 0)),
                new RoomDefinition(
                    "The Aerie Emporium",
                    "Emporium Settling Serum",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Merchant Teardrop", 13, "Common", ""),
                        new IngredientRequirement("Settling Sap", 12, "Common", ""),
                        new IngredientRequirement("Counterfloat Pearl", 10, "Common", ""),
                        new IngredientRequirement("Driftglass Bead", 11, "Uncommon", ""),
                        new IngredientRequirement("Weightwell Wax", 11, "Uncommon", ""),
                        new IngredientRequirement("Settling Resin", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(9, 2, 1)),
                new RoomDefinition(
                    "Gryphon Lantern Shrine",
                    "Weighted Wing Antitoxin",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Weighted Feather", 14, "Common", ""),
                        new IngredientRequirement("Gryphon Talon Shard", 13, "Common", ""),
                        new IngredientRequirement("Antigrav Venom Drop", 13, "Common", ""),
                        new IngredientRequirement("Shrinebase Dust", 12, "Common", ""),
                        new IngredientRequirement("Perchstone", 12, "Uncommon", ""),
                        new IngredientRequirement("Skyward Toxin Pearl", 11, "Uncommon", ""),
                        new IngredientRequirement("Wingbind Resin", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(9, 3, 2)),
                new RoomDefinition(
                    "Aerie Anchor Hall",
                    "Anchor’s Embrace Elixir",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Anchorheart Stone Fragment", 14, "Common", ""),
                        new IngredientRequirement("Aerie Lodestone", 14, "Common", ""),
                        new IngredientRequirement("Anchor Shard", 13, "Uncommon", ""),
                        new IngredientRequirement("Gravitywell Bloom", 11, "Uncommon", ""),
                        new IngredientRequirement("Settling Stone Dust", 12, "Uncommon", ""),
                        new IngredientRequirement("Downward Star Resin", 12, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(9, 4, 3)),
            }),
        new RealmDefinition("Crystalline Chanterelle Colony",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Sparkleborough Settlement",
                    "Spore-Spine Awakening Brew",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Lumenroot Extract", 14, "Common", ""),
                        new IngredientRequirement("Phosphor-Spore Dust", 13, "Common", ""),
                        new IngredientRequirement("Shardneedle Sliver", 13, "Common", ""),
                        new IngredientRequirement("Stiff-Stalk Elm Sap", 12, "Common", ""),
                        new IngredientRequirement("Spore-Cap Pebble", 12, "Uncommon", ""),
                        new IngredientRequirement("Mycelium Pod", 11, "Uncommon", ""),
                        new IngredientRequirement("Glint-Shell Snail Horn", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 1, 0)),
                new RoomDefinition(
                    "Lostleaf Library",
                    "Shard-Sparked Arid Serum",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Arid-Bloom Petal", 14, "Common", ""),
                        new IngredientRequirement("Sun-Baked Ink Stone", 14, "Common", ""),
                        new IngredientRequirement("Royal Papyrus Shred", 13, "Common", ""),
                        new IngredientRequirement("Memoryglass Quill Tip", 13, "Common", ""),
                        new IngredientRequirement("Crown-Shard Cluster", 12, "Uncommon", ""),
                        new IngredientRequirement("Salt-Crust Conch", 12, "Uncommon", ""),
                        new IngredientRequirement("Dried Reed Ring", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 2, 1)),
                new RoomDefinition(
                    "Milkweed Municipal Hall",
                    "Withering Strand Spritz",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Emerald Chameleon Venom", 15, "Common", ""),
                        new IngredientRequirement("Silk-Core Milkweed Pod", 14, "Common", ""),
                        new IngredientRequirement("Starlight Tendril Spool", 14, "Common", ""),
                        new IngredientRequirement("Gryphon-Nest Twig", 13, "Common", ""),
                        new IngredientRequirement("Dew-Locked Amber Drop", 13, "Uncommon", ""),
                        new IngredientRequirement("Weaver-Spider Egg Casing", 12, "Uncommon", ""),
                        new IngredientRequirement("Braid-Root Crown", 12, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 3, 2)),
                new RoomDefinition(
                    "Luminous Loopwater Lagoon",
                    "Infinity Cascade Catalyst",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Upcurrent Droplet", 15, "Common", ""),
                        new IngredientRequirement("Whispering Lagoon Dew", 15, "Common", ""),
                        new IngredientRequirement("Prism Polarity Beads", 14, "Uncommon", ""),
                        new IngredientRequirement("Cloud-Silk Cocoon", 14, "Uncommon", ""),
                        new IngredientRequirement("Fountain-Core Seed", 13, "Uncommon", ""),
                        new IngredientRequirement("Reverse-Tide Mist", 13, "Rare", ""),
                        new IngredientRequirement("Echoing Ripple Orb", 12, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(10, 4, 3)),
            }),
    };

    // Everfrost Vale is the 11th realm in the BMM Potions sheet, outside the current ten-realm set. Kept for when it is added.
    public static readonly RealmDefinition EverfrostVale = new RealmDefinition("Everfrost Vale",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Shimmerflurry Orchard",
                    "Shimmerroot Revival Infusion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Shimmerroot Strand", 14, "Common", ""),
                        new IngredientRequirement("Frostfruit Pearl", 13, "Common", ""),
                        new IngredientRequirement("Orchard Chilldrop", 13, "Common", ""),
                        new IngredientRequirement("Snowcrisp Bark", 12, "Common", ""),
                        new IngredientRequirement("Flurryblossom Pollen", 12, "Uncommon", ""),
                        new IngredientRequirement("Slushbane Resin", 11, "Uncommon", ""),
                        new IngredientRequirement("Everfrost Sap", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(11, 1, 0)),
                new RoomDefinition(
                    "Crystalline Court",
                    "Crystalline Frostform Catalyst",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Crystalline Mane Shard", 14, "Common", ""),
                        new IngredientRequirement("Frostform Dust", 14, "Common", ""),
                        new IngredientRequirement("Ground Frostturn Pearl", 13, "Common", ""),
                        new IngredientRequirement("Icecreature Chip", 13, "Common", ""),
                        new IngredientRequirement("Refreezing Spark", 12, "Uncommon", ""),
                        new IngredientRequirement("Mirrorfrost Flake", 12, "Uncommon", ""),
                        new IngredientRequirement("Snowturn Resin", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(11, 2, 1)),
                new RoomDefinition(
                    "Grand Glacier",
                    "Showstopper Stabilizing Salve",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Showstopper Snowflake", 15, "Common", ""),
                        new IngredientRequirement("Stabilizing Frostwax", 14, "Common", ""),
                        new IngredientRequirement("Canopy Thread", 14, "Common", ""),
                        new IngredientRequirement("Arctic Trim Fragment", 13, "Common", ""),
                        new IngredientRequirement("Snowring Powder", 13, "Uncommon", ""),
                        new IngredientRequirement("Curtainfrost Pearl", 12, "Uncommon", ""),
                        new IngredientRequirement("Slushseal Resin", 12, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(11, 3, 2)),
                new RoomDefinition(
                    "Winter Wonder Garden",
                    "Frosted Formweave Reagent",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Frosted Formthread", 15, "Common", ""),
                        new IngredientRequirement("Alabaster Snowdust", 15, "Common", ""),
                        new IngredientRequirement("Alabaster Pearl", 14, "Uncommon", ""),
                        new IngredientRequirement("Icebloom Filament", 14, "Uncommon", ""),
                        new IngredientRequirement("Sunwane Crystal", 13, "Uncommon", ""),
                        new IngredientRequirement("Winterweave Resin", 13, "Rare", ""),
                        new IngredientRequirement("Everchill Seed", 12, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(11, 4, 3)),
            });

    public static readonly RealmDefinition SunpetalConservatory = AllRealms[0];

    public static RealmDefinition ActivePrototypeRealm => AllRealms[ActivePrototypeRealmIndex];
    public static RoomDefinition ActivePrototypeRoom => ActivePrototypeRealm.Rooms[ActivePrototypeRoomIndex];
    public static IEnumerable<RoomDefinition> AllRooms
    {
        get
        {
            for (int realmIndex = 0; realmIndex < AllRealms.Count; realmIndex++)
            {
                IReadOnlyList<RoomDefinition> rooms = AllRealms[realmIndex].Rooms;
                for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
                {
                    yield return rooms[roomIndex];
                }
            }
        }
    }

    public static void SetActivePrototypeRealm(int realmIndex)
    {
        if (realmIndex < 0 || realmIndex >= AllRealms.Count)
        {
            return;
        }

        ActivePrototypeRealmIndex = realmIndex;
        ActivePrototypeRoomIndex = 0;
    }

    public static void SetActivePrototypeRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= ActivePrototypeRealm.Rooms.Count)
        {
            return;
        }

        ActivePrototypeRoomIndex = roomIndex;
    }

    public static void SetActivePrototypeRoom(int realmIndex, int roomIndex)
    {
        SetActivePrototypeRealm(realmIndex);
        SetActivePrototypeRoom(roomIndex);
    }
}

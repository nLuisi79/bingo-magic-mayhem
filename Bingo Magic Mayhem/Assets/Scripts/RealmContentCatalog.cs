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
    public static readonly RoomProgressionProfile LevelOne = Create(1, 1, 0);

    public RoomProgressionProfile(
        int level,
        int minManaBet,
        int maxManaBet,
        int betStep,
        int minimumJackpotPot,
        float jackpotContributionRate,
        float xpMultiplier,
        float ingredientDropChance,
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
        IngredientDropChance = ingredientDropChance;
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
    public float IngredientDropChance { get; private set; }
    public float CellRewardChance { get; private set; }
    public int RestoreManaReward { get; private set; }

    public static RoomProgressionProfile Create(int realmIndex, int roomIndex, int roomOffset)
    {
        int level = ((realmIndex - 1) * 4) + roomIndex;
        bool special = roomIndex == 4;
        int minBet = 25 + ((realmIndex - 1) * 25);
        int maxBet = minBet * (special ? 8 : 6);
        int betStep = realmIndex >= 4 ? 50 : 25;
        int minimumPot = 250 + ((realmIndex - 1) * 250) + (roomOffset * 125);
        float contribution = 0.10f + (realmIndex * 0.01f) + (special ? 0.03f : 0f);
        float xp = 1f + ((level - 1) * 0.08f);
        float ingredient = special ? 0.16f : 0.22f;
        float cellReward = special ? 0.60f : 0.75f;
        int restoreReward = 1000 + ((realmIndex - 1) * 500) + (roomOffset * 250);
        return new RoomProgressionProfile(level, minBet, maxBet, betStep, minimumPot, contribution, xp, ingredient, cellReward, restoreReward);
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
                        new IngredientRequirement("Gilded Azalea Petals", 6, "Common", ""),
                        new IngredientRequirement("Sunwarm Dewdrops", 5, "Common", ""),
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
                        new IngredientRequirement("Terrace Moss Tufts", 7, "Common", ""),
                        new IngredientRequirement("Rosegold Sap Drops", 7, "Common", ""),
                        new IngredientRequirement("Thornheart Bud", 8, "Uncommon", ""),
                        new IngredientRequirement("Bramblelace Fibers", 4, "Key Ingredient", ""),
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
                        new IngredientRequirement("Fountain Dewdrops", 6, "Common", ""),
                        new IngredientRequirement("Nectarflow Ribbons", 7, "Common", ""),
                        new IngredientRequirement("Lilygold Filaments", 7, "Uncommon", ""),
                        new IngredientRequirement("Pollen Dust", 3, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(1, 3, 2)),
                new RoomDefinition(
                    "Solstice Bloom Glasshouse",
                    "Solstice Revival Tincture",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Sunlit Glass Slivers", 8, "Common", ""),
                        new IngredientRequirement("Golden Hour Motes", 8, "Common", ""),
                        new IngredientRequirement("Prismvine Twine", 8, "Uncommon", ""),
                        new IngredientRequirement("Dawnmirror Fragments", 5, "Uncommon", ""),
                        new IngredientRequirement("Golden Stamen", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(1, 4, 3)),
            }),
        new RealmDefinition("Everbloom Sanctuary",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Heartbloom petals",
                    "Blossomveil Awakening Tonic",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Veilpetal Blossoms", 8, "Common", ""),
                        new IngredientRequirement("Roselight Essence", 5, "Common", ""),
                        new IngredientRequirement("Marblemoss Threads", 5, "Common", ""),
                        new IngredientRequirement("Petalsheen Dust", 7, "Uncommon", ""),
                        new IngredientRequirement("Heartbloom Petals", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 1, 0)),
                new RoomDefinition(
                    "Gardener's Grace Yard",
                    "Gardener’s Grace Mending Balm",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Graceyard Soil Cakes", 8, "Common", ""),
                        new IngredientRequirement("Polished Trowel Charms", 6, "Common", ""),
                        new IngredientRequirement("Terracotta Mender’s Clay", 8, "Common", ""),
                        new IngredientRequirement("Rootwax Binding Cord", 8, "Uncommon", ""),
                        new IngredientRequirement("Keepsake Clover", 3, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 2, 1)),
                new RoomDefinition(
                    "Luminara Fountain",
                    "Luminara Flow Essence",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Luminara Waterglass", 7, "Common", ""),
                        new IngredientRequirement("Silver Basin Coins", 7, "Common", ""),
                        new IngredientRequirement("Moonlit Ripple Lace", 8, "Common", ""),
                        new IngredientRequirement("Bluebell Chime Shells", 7, "Uncommon", ""),
                        new IngredientRequirement("Luminous Springcore", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(2, 3, 2)),
                new RoomDefinition(
                    "Everbloom Shrine",
                    "Heartbloom Revival Mist",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Frosted Petal Plates", 6, "Common", ""),
                        new IngredientRequirement("Sanctuary Lightwick", 7, "Common", ""),
                        new IngredientRequirement("Violet Geode Nectar", 7, "Uncommon", ""),
                        new IngredientRequirement("Halofern Ribbons", 7, "Uncommon", ""),
                        new IngredientRequirement("Gilded Bloom Sigils", 7, "Uncommon", ""),
                        new IngredientRequirement("Everheart Bud", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(2, 4, 3)),
            }),
        new RealmDefinition("Floating Lotus Lagoon",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Dewdrop Marina",
                    "Marina Mist Tonic",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Dewdrop Pearl", 8, "Common", ""),
                        new IngredientRequirement("Marina Reed", 6, "Common", ""),
                        new IngredientRequirement("Mistpetal Drop", 7, "Common", ""),
                        new IngredientRequirement("Driftwood Fragment", 6, "Uncommon", ""),
                        new IngredientRequirement("Silver Koi Scale", 5, "Uncommon", ""),
                        new IngredientRequirement("Pearlmist Shell", 3, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 1, 0)),
                new RoomDefinition(
                    "Waterlily Canal",
                    "Waterlily Flow Draught",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Waterlily Silk", 7, "Common", ""),
                        new IngredientRequirement("Canal Current Drop", 8, "Common", ""),
                        new IngredientRequirement("Lotusstem Reed", 7, "Common", ""),
                        new IngredientRequirement("Ripplemint Leaf", 6, "Uncommon", ""),
                        new IngredientRequirement("Blue Koi Scale", 5, "Uncommon", ""),
                        new IngredientRequirement("Canal Pearl", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 2, 1)),
                new RoomDefinition(
                    "Opalwater Glasswalk",
                    "Opalwater Clarity Potion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Opalwater Essence", 9, "Common", ""),
                        new IngredientRequirement("Glasswalk Shard", 7, "Common", ""),
                        new IngredientRequirement("Prism Dew", 8, "Common", ""),
                        new IngredientRequirement("Lotuslight Dust", 7, "Common", ""),
                        new IngredientRequirement("Clearpath Crystal", 6, "Uncommon", ""),
                        new IngredientRequirement("Opalstone", 5, "Uncommon", ""),
                        new IngredientRequirement("Mirrored Strand", 5, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(3, 3, 2)),
                new RoomDefinition(
                    "Glasswater Grotto",
                    "Grotto Glow Elixir",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Glasswater Shard", 8, "Common", ""),
                        new IngredientRequirement("Grotto Glowcap", 9, "Common", ""),
                        new IngredientRequirement("Opalwater Essence", 8, "Uncommon", ""),
                        new IngredientRequirement("Glimmerfin Scale", 7, "Uncommon", ""),
                        new IngredientRequirement("Floating Lotus Bloom", 6, "Uncommon", ""),
                        new IngredientRequirement("Stillwater Teardrop", 6, "Rare", ""),
                        new IngredientRequirement("Pearlglow Shard", 4, "Key Ingredient", ""),
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
                        new IngredientRequirement("Lanternleaf Sprig", 9, "Common", ""),
                        new IngredientRequirement("Wispglow Dew", 7, "Common", ""),
                        new IngredientRequirement("Pathmoss Thread", 8, "Common", ""),
                        new IngredientRequirement("Fernlight Dust", 7, "Uncommon", ""),
                        new IngredientRequirement("Glowroot Chip", 6, "Uncommon", ""),
                        new IngredientRequirement("Silvermoth Wing", 5, "Key Ingredient", ""),
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
                        new IngredientRequirement("Emerald Fernsap", 8, "Common", ""),
                        new IngredientRequirement("Groveglow Nectar", 8, "Common", ""),
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
                        new IngredientRequirement("Bramblelight Thorn", 8, "Common", ""),
                        new IngredientRequirement("Sporeglow Powder", 8, "Common", ""),
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
                        new IngredientRequirement("Wispwillow Bark", 8, "Common", ""),
                        new IngredientRequirement("Hollowlight Seed", 9, "Common", ""),
                        new IngredientRequirement("Willowwisp Sap", 9, "Uncommon", ""),
                        new IngredientRequirement("Hollowbloom Petal", 8, "Uncommon", ""),
                        new IngredientRequirement("Ancient Rootdrop", 7, "Uncommon", ""),
                        new IngredientRequirement("Wispnest Fiber", 7, "Rare", ""),
                        new IngredientRequirement("Heartwood Dew", 6, "Key Ingredient", ""),
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
                        new IngredientRequirement("Bluebell Glowdrop", 10, "Common", ""),
                        new IngredientRequirement("Luminary Wick", 8, "Common", ""),
                        new IngredientRequirement("Moonmoss", 9, "Common", ""),
                        new IngredientRequirement("Cerulean Pollen", 8, "Uncommon", ""),
                        new IngredientRequirement("Quietlight Pearl", 7, "Uncommon", ""),
                        new IngredientRequirement("Silverbell Petal", 6, "Key Ingredient", ""),
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
                        new IngredientRequirement("Starpetal", 9, "Common", ""),
                        new IngredientRequirement("Blueflame Glass", 9, "Common", ""),
                        new IngredientRequirement("Moonstone Thread", 7, "Uncommon", ""),
                        new IngredientRequirement("Lanternseed Husk", 7, "Uncommon", ""),
                        new IngredientRequirement("Silverstep Dust", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(5, 2, 1)),
                new RoomDefinition(
                    "Script Dust",
                    "Skyglass Scroll Reagent",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Skyglass Shard", 10, "Common", ""),
                        new IngredientRequirement("Starink Vial", 9, "Common", ""),
                        new IngredientRequirement("Scrollfiber", 9, "Common", ""),
                        new IngredientRequirement("Quillmoon Feather", 9, "Common", ""),
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
                        new IngredientRequirement("Starwell Drop", 9, "Common", ""),
                        new IngredientRequirement("Bellchime Crystal", 10, "Common", ""),
                        new IngredientRequirement("Moonmetal", 10, "Uncommon", ""),
                        new IngredientRequirement("Celestial Cord", 9, "Uncommon", ""),
                        new IngredientRequirement("Echo Pearl", 8, "Uncommon", ""),
                        new IngredientRequirement("Chime Fragment", 8, "Rare", ""),
                        new IngredientRequirement("Cerulean Resonance Dust", 7, "Key Ingredient", ""),
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
                        new IngredientRequirement("Glassbark Sliver", 10, "Common", ""),
                        new IngredientRequirement("Rosegold Pollen", 9, "Uncommon", ""),
                        new IngredientRequirement("Kindleseed Husk", 8, "Uncommon", ""),
                        new IngredientRequirement("Warmroot Sap", 7, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(6, 1, 0)),
                new RoomDefinition(
                    "Cinderglass Path",
                    "Cinderglass Mending Salve",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Cinderglass Shard", 9, "Common", ""),
                        new IngredientRequirement("Clearflame Drop", 10, "Common", ""),
                        new IngredientRequirement("Pathglow Pebble", 10, "Common", ""),
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
                        new IngredientRequirement("Sparkstone Pebble", 10, "Common", ""),
                        new IngredientRequirement("Flamepetal Bloom", 9, "Common", ""),
                        new IngredientRequirement("Amber Emberseed", 10, "Common", ""),
                        new IngredientRequirement("Glowglass Nectar", 10, "Common", ""),
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
                        new IngredientRequirement("Embervein Root", 9, "Common", ""),
                        new IngredientRequirement("Heartwood Ember", 10, "Common", ""),
                        new IngredientRequirement("Rootglass Thread", 10, "Uncommon", ""),
                        new IngredientRequirement("Deepwarm Resin", 10, "Uncommon", ""),
                        new IngredientRequirement("Crimson Sapstone", 9, "Uncommon", ""),
                        new IngredientRequirement("Emberline Crystal", 9, "Rare", ""),
                        new IngredientRequirement("Glowcoal Fragment", 8, "Key Ingredient", ""),
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
                        new IngredientRequirement("Gatevine Oil", 11, "Common", ""),
                        new IngredientRequirement("Tarnishroot Flake", 10, "Uncommon", ""),
                        new IngredientRequirement("Ironlace Charm", 9, "Uncommon", ""),
                        new IngredientRequirement("Weathered Iron Dust", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(7, 1, 0)),
                new RoomDefinition(
                    "Hollowmoss Topiary Grounds",
                    "Topiary Teardrop Nectar",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Topiary Teardrop", 10, "Common", ""),
                        new IngredientRequirement("Sculpted Mossleaf", 11, "Common", ""),
                        new IngredientRequirement("Hedgecurl Sprig", 11, "Common", ""),
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
                        new IngredientRequirement("Rootsnare Strand", 11, "Common", ""),
                        new IngredientRequirement("Hearthwood Splinter", 10, "Common", ""),
                        new IngredientRequirement("Binding Vinecurl", 11, "Common", ""),
                        new IngredientRequirement("Hallglow Ember", 11, "Common", ""),
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
                        new IngredientRequirement("Wayward Soul Drop", 10, "Common", ""),
                        new IngredientRequirement("Elderroot Needle", 11, "Common", ""),
                        new IngredientRequirement("Labyrinth Ivy Thread", 11, "Uncommon", ""),
                        new IngredientRequirement("Shadowmoss Pearl", 11, "Uncommon", ""),
                        new IngredientRequirement("Pathmark Shard", 10, "Uncommon", ""),
                        new IngredientRequirement("Lanternless Dew", 10, "Rare", ""),
                        new IngredientRequirement("Labyrinth Seed", 9, "Key Ingredient", ""),
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
                        new IngredientRequirement("Rockheart Stone Fragment", 11, "Common", ""),
                        new IngredientRequirement("Rubyrock Splinter", 10, "Common", ""),
                        new IngredientRequirement("Crimson Mineral Dew", 11, "Common", ""),
                        new IngredientRequirement("Glowvein Fragment", 11, "Uncommon", ""),
                        new IngredientRequirement("Canyonroot Resin", 10, "Uncommon", ""),
                        new IngredientRequirement("Scarlet Quartz Dust", 8, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(8, 1, 0)),
                new RoomDefinition(
                    "Coppercliff Crossing",
                    "Bridgemender Brew",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Copperstone Fragment", 10, "Common", ""),
                        new IngredientRequirement("Bridgevine Thread", 11, "Common", ""),
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
                        new IngredientRequirement("Twilight Dust", 11, "Common", ""),
                        new IngredientRequirement("Starseed", 11, "Common", ""),
                        new IngredientRequirement("Trailglimmer Sand", 10, "Common", ""),
                        new IngredientRequirement("Nightglass Flake", 11, "Uncommon", ""),
                        new IngredientRequirement("Falling Star Petal", 11, "Uncommon", ""),
                        new IngredientRequirement("Duskwind Thread", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(8, 3, 2)),
                new RoomDefinition(
                    "Moonflare Mesa",
                    "Starlight Prism Potion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Starlight Prism Shard", 10, "Common", ""),
                        new IngredientRequirement("Moonlit Flaredrop", 11, "Common", ""),
                        new IngredientRequirement("Crimsonlight Petal", 11, "Uncommon", ""),
                        new IngredientRequirement("Glowstone", 11, "Uncommon", ""),
                        new IngredientRequirement("Prismroot Strand", 11, "Uncommon", ""),
                        new IngredientRequirement("Skyfire Dust", 10, "Rare", ""),
                        new IngredientRequirement("Lunar Quartz Bloom", 9, "Key Ingredient", ""),
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
                        new IngredientRequirement("Skybound Vapor", 11, "Common", ""),
                        new IngredientRequirement("Countercloud Drop", 10, "Common", ""),
                        new IngredientRequirement("Skycrag Stonechip", 11, "Common", ""),
                        new IngredientRequirement("Gateweight Pebble", 11, "Uncommon", ""),
                        new IngredientRequirement("Anchorroot Dust", 10, "Uncommon", ""),
                        new IngredientRequirement("Cloudweight Resin", 9, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(9, 1, 0)),
                new RoomDefinition(
                    "The Aerie Emporium",
                    "Emporium Settling Serum",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Merchant Teardrop", 10, "Common", ""),
                        new IngredientRequirement("Settling Sap", 11, "Common", ""),
                        new IngredientRequirement("Counterfloat Pearl", 11, "Common", ""),
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
                        new IngredientRequirement("Weighted Feather", 11, "Common", ""),
                        new IngredientRequirement("Gryphon Talon Shard", 10, "Common", ""),
                        new IngredientRequirement("Antigrav Venom Drop", 11, "Common", ""),
                        new IngredientRequirement("Shrinebase Dust", 11, "Common", ""),
                        new IngredientRequirement("Perchstone", 11, "Uncommon", ""),
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
                        new IngredientRequirement("Anchorheart Stone Fragment", 11, "Common", ""),
                        new IngredientRequirement("Aerie Lodestone", 10, "Common", ""),
                        new IngredientRequirement("Anchor Shard", 11, "Uncommon", ""),
                        new IngredientRequirement("Gravitywell Bloom", 11, "Uncommon", ""),
                        new IngredientRequirement("Settling Stone Dust", 11, "Uncommon", ""),
                        new IngredientRequirement("Downward Star Resin", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(9, 4, 3)),
            }),
        new RealmDefinition("Everfrost Vale",
            new List<RoomDefinition>
            {
                new RoomDefinition(
                    "Shimmerflurry Orchard",
                    "Shimmerroot Revival Infusion",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Shimmerroot Strand", 10, "Common", ""),
                        new IngredientRequirement("Frostfruit Pearl", 11, "Common", ""),
                        new IngredientRequirement("Orchard Chilldrop", 11, "Common", ""),
                        new IngredientRequirement("Snowcrisp Bark", 10, "Common", ""),
                        new IngredientRequirement("Flurryblossom Pollen", 11, "Uncommon", ""),
                        new IngredientRequirement("Slushbane Resin", 11, "Uncommon", ""),
                        new IngredientRequirement("Everfrost Sap", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 1, 0)),
                new RoomDefinition(
                    "Crystalline Creatures Carousel",
                    "Crystalline Frostform Catalyst",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Crystalline Mane Shard", 11, "Common", ""),
                        new IngredientRequirement("Frostform Dust", 11, "Common", ""),
                        new IngredientRequirement("Ground Frostturn Pearl", 10, "Common", ""),
                        new IngredientRequirement("Icecreature Chip", 11, "Common", ""),
                        new IngredientRequirement("Refreezing Spark", 11, "Uncommon", ""),
                        new IngredientRequirement("Mirrorfrost Flake", 11, "Uncommon", ""),
                        new IngredientRequirement("Snowturn Resin", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 2, 1)),
                new RoomDefinition(
                    "Grand Glacial Bigtop",
                    "Showstopper Stabilizing Salve",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Showstopper Snowflake", 11, "Common", ""),
                        new IngredientRequirement("Stabilizing Frostwax", 10, "Common", ""),
                        new IngredientRequirement("Canopy Thread", 11, "Common", ""),
                        new IngredientRequirement("Arctic Trim Fragment", 11, "Common", ""),
                        new IngredientRequirement("Snowring Powder", 11, "Uncommon", ""),
                        new IngredientRequirement("Curtainfrost Pearl", 11, "Uncommon", ""),
                        new IngredientRequirement("Slushseal Resin", 11, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Standard,
                    "Standard",
                    RoomProgressionProfile.Create(10, 3, 2)),
                new RoomDefinition(
                    "Alabaster Wonder Garden",
                    "Frosted Formweave Reagent",
                    new List<IngredientRequirement>
                    {
                        new IngredientRequirement("Frosted Formthread", 10, "Common", ""),
                        new IngredientRequirement("Alabaster Snowdust", 11, "Common", ""),
                        new IngredientRequirement("Alabaster Pearl", 11, "Uncommon", ""),
                        new IngredientRequirement("Icebloom Filament", 11, "Uncommon", ""),
                        new IngredientRequirement("Sunwane Crystal", 11, "Uncommon", ""),
                        new IngredientRequirement("Winterweave Resin", 11, "Rare", ""),
                        new IngredientRequirement("Everchill Seed", 10, "Key Ingredient", ""),
                    },
                    BingoRoomMode.Special,
                    "Blackout",
                    RoomProgressionProfile.Create(10, 4, 3)),
            }),
    };

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

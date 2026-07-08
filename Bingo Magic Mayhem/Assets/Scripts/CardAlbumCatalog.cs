using System.Collections.Generic;

public enum AlbumCardTier
{
    Regular,
    Gilded,
    Ancient
}

public sealed class AlbumRewardDefinition
{
    public AlbumRewardDefinition(int mana, int crystals, int powerUps, int clairvoyanceHours, string note = "")
    {
        Mana = mana;
        Crystals = crystals;
        PowerUps = powerUps;
        ClairvoyanceHours = clairvoyanceHours;
        Note = note;
    }

    public int Mana { get; }
    public int Crystals { get; }
    public int PowerUps { get; }
    public int ClairvoyanceHours { get; }
    public string Note { get; }
}

public sealed class AlbumCardDefinition
{
    public AlbumCardDefinition(int grimoireNumber, int entryNumber, string potionName, int slotNumber, string cardName, int stars, AlbumCardTier tier)
    {
        GrimoireNumber = grimoireNumber;
        EntryNumber = entryNumber;
        PotionName = potionName;
        SlotNumber = slotNumber;
        CardName = cardName;
        Stars = stars;
        Tier = tier;
        Id = $"grimoire-{grimoireNumber:00}-entry-{entryNumber:00}-card-{slotNumber:00}";
        DisplayName = cardName;
    }

    public int GrimoireNumber { get; }
    public int EntryNumber { get; }
    public string PotionName { get; }
    public int SlotNumber { get; }
    public string CardName { get; }
    public int Stars { get; }
    public AlbumCardTier Tier { get; }
    public string Id { get; }
    public string DisplayName { get; }
}

public sealed class GrimoireEntryDefinition
{
    public GrimoireEntryDefinition(int grimoireNumber, int entryNumber, string potionName, AlbumRewardDefinition reward, IReadOnlyList<AlbumCardDefinition> cards)
    {
        GrimoireNumber = grimoireNumber;
        EntryNumber = entryNumber;
        PotionName = potionName;
        Reward = reward;
        Cards = cards;
    }

    public int GrimoireNumber { get; }
    public int EntryNumber { get; }
    public string PotionName { get; }
    public AlbumRewardDefinition Reward { get; }
    public IReadOnlyList<AlbumCardDefinition> Cards { get; }
}

public sealed class BookOfShadowsCardDefinition
{
    public BookOfShadowsCardDefinition(int setNumber, int entryNumber, string potionName, int slotNumber, string cardName, int stars)
    {
        SetNumber = setNumber;
        EntryNumber = entryNumber;
        PotionName = potionName;
        SlotNumber = slotNumber;
        CardName = cardName;
        Stars = stars;
        Id = $"book-shadows-{setNumber:00}-entry-{entryNumber:00}-card-{slotNumber:00}";
        DisplayName = cardName;
    }

    public int SetNumber { get; }
    public int EntryNumber { get; }
    public string PotionName { get; }
    public int SlotNumber { get; }
    public string CardName { get; }
    public int Stars { get; }
    public string Id { get; }
    public string DisplayName { get; }
}

public sealed class BookOfShadowsEntryDefinition
{
    public BookOfShadowsEntryDefinition(int setNumber, int entryNumber, string potionName, AlbumRewardDefinition reward, IReadOnlyList<BookOfShadowsCardDefinition> cards)
    {
        SetNumber = setNumber;
        EntryNumber = entryNumber;
        PotionName = potionName;
        Reward = reward;
        Cards = cards;
    }

    public int SetNumber { get; }
    public int EntryNumber { get; }
    public string PotionName { get; }
    public AlbumRewardDefinition Reward { get; }
    public IReadOnlyList<BookOfShadowsCardDefinition> Cards { get; }
}

public sealed class BookOfShadowsSetDefinition
{
    public BookOfShadowsSetDefinition(int setNumber, int startDay, int endDay, IReadOnlyList<BookOfShadowsEntryDefinition> entries)
    {
        SetNumber = setNumber;
        StartDay = startDay;
        EndDay = endDay;
        Entries = entries;
    }

    public int SetNumber { get; }
    public int StartDay { get; }
    public int EndDay { get; }
    public IReadOnlyList<BookOfShadowsEntryDefinition> Entries { get; }
}

public static class CardAlbumCatalog
{
    public const string SpecificGrimoireCardRewardPrefix = "Grimoire Card:";
    public static readonly AlbumRewardDefinition GrimoireOneCompletionReward = new AlbumRewardDefinition(300000, 3000, 3000, 175, "VIP highest");
    public static readonly AlbumRewardDefinition BookOfShadowsCompletionReward = new AlbumRewardDefinition(30000, 1500, 1500, 95, "VIP highest");
    private static readonly int[] GrimoireOneSlotStars = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 3, 4, 5, 4, 5 };

    private static readonly AlbumCardTier[] GrimoireOneSlotTiers =
    {
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Regular,
        AlbumCardTier.Ancient,
        AlbumCardTier.Ancient,
        AlbumCardTier.Ancient,
        AlbumCardTier.Gilded,
        AlbumCardTier.Gilded
    };

    private static readonly string[][] GrimoireOneRows =
    {
        new[] { "Sovereign Balm of Elysium", "Elysian Goldleaf", "Sovereign Sunmilk", "Halo-Silk Threads", "Crownstar Resin", "Seraphic Dewdrops", "Thronebloom Petals", "Aurelian Heartseed", "Paradise Myrrh Dust", "Blessed Laurel Ash", "Eternal Gracewax", "", "", "", "", "" },
        new[] { "Ambrosian Nectar of Olympus", "Ambrosian Honeydrops", "Olympian Cloudmilk", "Suncrest Fig Nectar", "Ichor-Silk Filaments", "Godgrain Pollen", "Laurel-Crown Petals", "Aurelian Peachstone", "Hearthgold Cinnamon Bark", "Nectarveil Dew", "Immortal Chalice Glaze", "", "", "", "", "" },
        new[] { "Pyre-Born Elixir of Awakening", "Phoenix Ember Ash", "Dawnfire Resin", "Cinderheart Seeds", "Molten Marigold Petals", "Kindling Starflakes", "Ashen Goldsalt", "Flamewake Dewdrops", "Ignis Root Shavings", "Pyre-Silk Threads", "Awakener's Spark Essence", "", "", "", "", "" },
        new[] { "Storm-Crowned Essence of Evocation", "Thunderhead Crownshards", "Lightning Vein Sap", "Tempest Laurel Leaves", "Cloudiron Filings", "Galeglass Beads", "Raincall Pearl Drops", "Evoker's Blueflame Wick", "Stormsigil Ink", "Skyfang Crystal Splinters", "Crowned Tempest Core", "", "", "", "", "" },
        new[] { "Hestian Hearth-Shield", "Hestian Emberstone", "Hearthveil Ash", "Sanctuary Cinnamon Bark", "Warding Flamewick", "Golden Threshold Salt", "Cottage-Rose Petals", "Iron Kettle Filings", "Honeyed Oathwax", "Lanternheart Glowdrops", "", "", "", "Everflame Binding Cord", "", "" },
        new[] { "Lotus Chalice of Purification", "Moonlotus Petals", "Silver Chalice Dew", "Pearlwater Essence", "White Sandalwood Dust", "Lotusroot Filaments", "Serenity Reed Ash", "Opaline Rainbeads", "Veilwash Salt Crystals", "Dawnwater Lily Pollen", "", "", "", "Purity Flame Reflection", "", "" },
        new[] { "Oracle's Moonwater Elixir", "Oracle Moonwater Drops", "Seer's Pearl Fragments", "Lunar Veil Threads", "Reflection Pool Glass", "Omenreed Ink", "Crescent Lotus Pollen", "Starlit Scrying Salt", "Nightbloom Sage Ash", "Prophet's Chalice Dew", "", "", "", "Third-Eye Moonstone Core", "", "" },
        new[] { "Golden Thread Fatebrew", "Golden Fate Threads", "Spinner's Loom Dust", "Destiny Silk Filaments", "Kismet Honeydrops", "Hourglass Saffron Grains", "Fortune Laurel Leaves", "Crossroads Goldsalt", "Fatespindle Pearl Beads", "Oathline Ink", "", "", "", "Threadheart Starseed", "", "" },
        new[] { "Dragonheart Distillate", "Dragonheart Emberstone", "", "Wyrmfire Resin", "Ruby Scale Shavings", "Flamevein Saffron", "Hoardgold Dust", "Cavernsmoke Salts", "Inferno Thorn Tips", "Elderfang Pearl Chips", "", "", "Molten Clawmark Ink", "Heartflame Essence", "", "" },
        new[] { "Colossus Root Serum", "Colossus Root Knots", "", "Titanbark Shavings", "Stonevein Sap Drops", "Eldergranite Dust", "Ironmoss Tufts", "Deepsoil Heartseed", "Mountain Marrow Salts", "Rootbound Amber Resin", "", "", "Giantstep Pebbles", "Worldroot Binding Cord", "", "" },
        new[] { "Aurelian Lionheart Essence", "Aurelian Mane Filaments", "", "Lionheart Sunstone Core", "Courage-Crown Petals", "Regal Saffron Threads", "Pridegold Dust", "Roarwave Amber Drops", "Sunlit Laurel Leaves", "Nobleclaw Resin", "", "", "Honorbound Oathwax", "Sovereign Heartflame", "", "" },
        new[] { "Atlas's Core Concoction", "Atlas Corestone", "", "Skyburden Granite Dust", "Titan Shoulder Salts", "Celestial Axis Shavings", "Worldweight Amber Drops", "Heavenbrace Iron Filings", "Orbital Root Threads", "Firmament Pearl Chips", "", "", "Burdenbound Oathwax", "Axis Heart Essence", "", "" },
        new[] { "Hissing Crown Venom", "Serpent Crown Fangs", "", "Viperjade Venom Drops", "Hisslace Filaments", "Emerald Scale Shavings", "Crowncoil Resin", "Nightfang Pepper Seeds", "Venomveil Dewdrops", "Basilisk Goldsalt", "", "", "", "Royal Antidote Pearl Dust", "", "Crowned Serpent Essence" },
        new[] { "Philosopher's Bloom Brew", "Philosopher's Bloom Petals", "", "Aurumroot Shavings", "Wisdomglass Dewdrops", "Transmutation Pollen", "Stoneheart Nectar", "Sagefire Ash", "Mercurial Vine Threads", "Rosegold Crucible Dust", "", "", "", "Elixir Pearl Fragments", "", "Prima Materia Spark" },
        new[] { "Orthos Equilibrium Extract", "Twinfang Root Tips", "", "Equinox Pearl Drops", "Guardian Hound Fur Tufts", "Balance Scale Dust", "Sunmoon Laurel Leaves", "Mirrorbone Salts", "Harmony Reed Threads", "Threshold Amber Resin", "", "", "", "Stillfire Ash", "", "Equilibrium Heartcore" },
        new[] { "Golden Seam Infusion", "Golden Seam Threads", "", "Mender's Sunwax", "Kintsugi Pearl Dust", "Silkneedle Thorn Tips", "Restoration Loom Fibers", "Aurelian Binding Resin", "Fracturelight Dewdrops", "Patchwork Laurel Leaves", "", "", "", "Mosaic Heartseed", "", "Seamglow Essence" },
        new[] { "Elysian Slumber Serum", "Elysian Poppy Petals", "", "Dreamcloud Milk Drops", "Moonmoss Tufts", "Slumberglass Beads", "Lavender Star Pollen", "Nightveil Silk Threads", "Pillowthorn Down", "Lullaby Pearl Dust", "", "", "", "Restwell Honeydrops", "", "Serene Mooncore Essence" },
        new[] { "Morphean Dreamtide Moonbrew", "Morphean Moonwater Drops", "", "Dreamtide Pearl Fragments", "Somnus Seafoam Wisps", "Lunar Kelp Ribbons", "Nightcurrent Salt Crystals", "Oneiro Lotus Pollen", "Tideglass Beads", "Moon-Shell Dust", "", "", "", "Sleepsong Coral Threads", "", "Morphean Tidemoon Core" },
        new[] { "Astral Aegis Catalyst", "Astral Aegis Shards", "", "Starmetal Filings", "", "Nebula Pearl Drops", "Voidward Salt Crystals", "Celestial Laurel Leaves", "Cometglass Beads", "Aetherbind Threads", "", "", "Starforge Ember Ash", "Guardian Constellation Ink", "", "Aegis Core Essence" },
        new[] { "Starpath Recall Restoration", "Starpath Compass Shards", "", "Memoryglass Dewdrops", "", "Northstar Laurel Leaves", "Astral Breadcrumb Seeds", "Wayfinder Pearl Dust", "Recall Rune Ink", "Comet-Tail Threads", "", "", "Moonlit Milestone Stones", "Homeward Aether Salt", "", "Returnstar Core Essence" },
        new[] { "Elder Oath Balm", "Elder Oathwax", "", "Vowroot Shavings", "", "Ancestral Laurel Ash", "Promise Pearl Dust", "Honorbound Cord Fibers", "Elder Sigil Ink", "Lineage Amber Drops", "", "", "Truthstone Salts", "Guardian Bark Threads", "", "Oathheart Essence" },
        new[] { "Symbiotic Soul Salve", "Soulvine Tendrils", "", "Twinheart Dewdrops", "", "Symbiont Moss Tufts", "Empathy Pearl Dust", "Shared Breath Seeds", "Harmony Sap Drops", "Bondweave Silk Threads", "", "", "Mirrorroot Shavings", "Lifeline Amber Resin", "", "Soulpair Essence" },
        new[] { "Loyalheart Ember Elixir", "Loyalheart Emberstone", "", "Steadfast Flamewick", "", "Companion Ashleaf Petals", "Oathfire Honeydrops", "Kindred Spark Seeds", "Guardian Hearthsalt", "Trustforge Amber Resin", "", "", "Devotion Silk Threads", "Braveheart Laurel Leaves", "", "Everloyal Ember Essence" },
        new[] { "Ancestral Rootbond Remedy", "Ancestral Root Threads", "", "Lineage Soil Cakes", "", "Grandmother Bark Shavings", "", "Bloodline Amber Drops", "", "Heritage Moss Tufts", "Kinship Heartseed", "", "Rootmemory Pearl Dust", "Elderbranch Ash", "Generational Binding Cord", "Rootbond Essence" },
        new[] { "Ambervein Tears of Resurgence", "Ambervein Tear Drops", "", "Resurgence Bark Shavings", "", "Sunken Memory Resin", "", "Golden Griefmoss Tufts", "", "Revival Heartseed", "Honeyed Rootsalts", "", "Dawnsap Filaments", "Phoenixleaf Ash", "Aurelian Dew Pearls", "Resurgent Amber Essence" },
        new[] { "Lunar Infinity Catalyst", "Lunar Infinity Shards", "", "Moonloop Pearl Drops", "", "Eclipseglass Beads", "", "Crescent Spiral Threads", "", "Cyclebloom Pollen", "Starlit Tidal Salts", "", "Waxing Moonmilk Drops", "Waning Veil Ash", "Orbital Moonstone Core", "Infinite Lunar Essence" },
        new[] { "Helios Solstice Sovereign", "Helios Sunwheel Shards", "", "Solstice Crownfire Drops", "", "Solar Laurel Leaves", "", "Aurelian Sunmilk", "", "Dawncrest Saffron Threads", "Noonday Goldsalt", "", "Sunflare Glass Beads", "Radiance Oathwax", "Horizon Amber Resin", "Sovereign Solar Essence" },
        new[] { "Arcane Vitalis Infusion", "Vitalis Aether Drops", "", "Arcane Heartleaf Petals", "", "Lifeglyph Ink", "", "Prismroot Shavings", "", "Mana Pearl Dust", "Vital Spark Seeds", "", "Aethervein Silk Threads", "Renewal Glass Beads", "Energizing Laurel Ash", "Arcane Vitalis Essence" },
        new[] { "Elderroot Omni-Realm Renewal", "Elderroot Crown Knots", "", "Omni-Realm Soil Cakes", "", "Worldbridge Sap Drops", "", "Ancient Grove Pearl Dust", "", "Realmroot Binding Fibers", "", "Portalbloom Pollen", "Deep Time Amber Resin", "Sanctuary Moss Tufts", "Continuum Bark Shavings", "Omni-Renewal Essence" },
        new[] { "Omniverse Tethering Tonic", "Omniverse Anchor Shards", "", "Tetherline Aether Threads", "", "Dimensional Knot Resin", "", "Continuum Pearl Drops", "", "Portalwake Glass Beads", "", "Gravitywell Salts", "Realmchain Gold Filings", "Aether Compass Ink", "Multiversal Binding Cord", "Tethercore Essence" },
        new[] { "Astral Symphony Concoction", "Astral Harp Strings", "", "Celestial Chime Shards", "", "Starlit Resonance Drops", "", "Nebula Song Pearl Dust", "", "Moonlit Lyrewood Shavings", "", "Constellation Reed Threads", "Echo-Orb Glass Beads", "Aether Choir Salt Crystals", "Harmony Sigil Ink", "Symphonic Starcore Essence" },
        new[] { "Quintessence Matrix Elixir", "Quintessence Prism Shards", "", "Aetherglass Filaments", "", "Matrix Core Seeds", "", "Celestial Mercury Drops", "", "Voidlace Powder", "", "Elemental Convergence Salts", "Astral Lattice Threads", "Philosopher's Bloom Pollen", "Resonance Pearl Dust", "Omni-Spark Essence" }
    };

    private static readonly AlbumRewardDefinition[] GrimoireOneRewards =
    {
        new AlbumRewardDefinition(400, 25, 0, 6),
        new AlbumRewardDefinition(600, 25, 0, 6),
        new AlbumRewardDefinition(800, 25, 0, 6),
        new AlbumRewardDefinition(1000, 50, 0, 12),
        new AlbumRewardDefinition(1100, 50, 0, 12),
        new AlbumRewardDefinition(1200, 50, 0, 12),
        new AlbumRewardDefinition(1300, 50, 0, 12),
        new AlbumRewardDefinition(1400, 50, 0, 12),
        new AlbumRewardDefinition(1500, 50, 0, 12),
        new AlbumRewardDefinition(1600, 50, 50, 12),
        new AlbumRewardDefinition(1700, 0, 50, 12),
        new AlbumRewardDefinition(1800, 0, 50, 12),
        new AlbumRewardDefinition(1900, 0, 50, 12),
        new AlbumRewardDefinition(2000, 0, 50, 12),
        new AlbumRewardDefinition(2100, 0, 50, 12),
        new AlbumRewardDefinition(2200, 0, 50, 12),
        new AlbumRewardDefinition(2250, 150, 0, 24),
        new AlbumRewardDefinition(2300, 150, 0, 24),
        new AlbumRewardDefinition(2350, 150, 0, 24),
        new AlbumRewardDefinition(2400, 150, 0, 24),
        new AlbumRewardDefinition(2450, 150, 0, 24),
        new AlbumRewardDefinition(2500, 150, 0, 24),
        new AlbumRewardDefinition(2550, 150, 0, 24),
        new AlbumRewardDefinition(2600, 0, 150, 24),
        new AlbumRewardDefinition(2650, 0, 150, 24),
        new AlbumRewardDefinition(2700, 0, 150, 24),
        new AlbumRewardDefinition(2750, 0, 150, 24),
        new AlbumRewardDefinition(2800, 0, 150, 24),
        new AlbumRewardDefinition(2850, 0, 150, 24),
        new AlbumRewardDefinition(2900, 300, 0, 36),
        new AlbumRewardDefinition(2950, 0, 300, 36),
        new AlbumRewardDefinition(3000, 400, 400, 48)
    };

    private static readonly int[] BookOfShadowsSlotStars = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 };

    private static readonly string[][][] BookOfShadowsRows =
    {
        new[]
        {
            new[] { "Warlock Binding Brew", "Warlock's Binding Cord", "Blackthorn Knot Tips", "Witchsalt Circle Crystals", "Oathbreaker Ash", "Iron Sigil Filings", "Moonless Ink Drops", "Hawthorn Thread Fibers", "Shadowwax Sealings", "Crowcall Feather Barbs", "Binding Circle Essence" },
            new[] { "Saltcircle Ward Tonic", "Saltcircle Crystals", "Threshold Chalk Dust", "Moon-Blessed Brine Drops", "Protective Sage Ash", "Hearthline Rosemary Sprigs", "Iron Boundary Filings", "Circlewax Sealings", "Four-Corner Pearl Dust", "Warding Bell Chime Shards", "Sanctuary Circle Essence" },
            new[] { "Iron Pin Uncrossing Potion", "Iron Uncrossing Pins", "Crossroad Salt Crystals", "Jinxroot Shavings", "Red Thread Snippings", "Sharpthorn Needle Tips", "Unhexing Sage Ash", "Rustless Iron Filings", "Knotbreak Brine Drops", "Mirrorback Pearl Dust", "Uncrossing Spark Essence" },
            new[] { "Hexmark Revealing Ink", "Hexmark Ink Drops", "Revealer's Quill Barbs", "Moonlit Soot Dust", "Truthglass Shavings", "Witchlight Pearl Dust", "Sigilroot Fibers", "Crow-Eye Dewdrops", "Unmasking Salt Crystals", "Obsidian Inkbinder Wax", "Revelation Glyph Essence" },
            new[] { "Mirrorback Mischief Draught", "Mirrorback Glass Shards", "Trickster Thread Snippings", "Laughing Soot Dust", "Reversal Pearl Dust", "Silver Foxglove Petals", "Mischief Knot Resin", "Glint-Eye Dewdrops", "Backfire Salt Crystals", "Jesterbell Chime Chips", "Mirrorback Essence" },
            new[] { "Crow's Fate-Blocking Balm", "Crow Omen Feathers", "Fateblock Wax Sealings", "Ravenpath Salt Crystals", "Moonless Laurel Ash", "Crossroads Crowberries", "Shadow Thread Snippings", "Omen-Eye Pearl Dust", "Warning Bell Chime Chips", "Blackthorn Wing Tips", "Fate-Blocking Crow Essence" },
            new[] { "Candlelight Nightshade Antidote", "Candlelight Wick Threads", "Nightshade Antidote Drops", "Glowroot Shavings", "Black Candlewax Flecks", "Moonlit Belladonna Petals", "Lanternsalt Crystals", "Sootveil Ash", "Dawnmoth Wing Dust", "Flameglass Beads", "Candleflame Remedy Essence" },
            new[] { "Threshold Lockbrew", "Threshold Keyshards", "Doorline Salt Crystals", "Lockroot Shavings", "Iron Hinge Filings", "Black Candlewax Sealings", "Boundary Chalk Dust", "Nightbolt Resin", "Latchline Cord Fibers", "Doorwarden Charm Bells", "Threshold Seal Essence" }
        },
        new[]
        {
            new[] { "Warlock's Knot Unraveling Draught", "Warlock Knot Fibers", "Unraveling Thread Snips", "Knotbreaker Iron Filings", "Charmknot Thorn Hooks", "Undoing Brine Drops", "Loosened Sigil Ink", "Tangle-Sage Ash", "Freehand Chalk Dust", "Released Cord Wax", "Unraveling Draught Essence" },
            new[] { "Enemy Disentanglement Serum", "Enemy Thread Snippings", "Disentangling Vine Fibers", "Cord-Cutter Iron Filings", "Separation Salt Crystals", "Boundary Pearl Dust", "Release Root Shavings", "Unhooking Thorn Tips", "Clearpath Brine Drops", "Freedom Sigil Ink", "Disentanglement Essence" },
            new[] { "Glamour-Snare Dissolver", "Glamourveil Petals", "Snarethorn Needle Tips", "Trueface Pearl Dust", "Illusionglass Shards", "Unmasking Brine Drops", "Vanishing Silk Fibers", "Silver Sage Ash", "Charmbreaker Salt Crystals", "Clear-Sight Ink Drops", "Glamour Dissolution Essence" },
            new[] { "Whisperhex Silencing Tonic", "Whisperhex Echo Threads", "Silence Bell Chime Chips", "Murmur-Moth Wing Dust", "Hushroot Shavings", "Stillvoice Pearl Dust", "Hushwax Throat Seals", "Throat-Sage Ash", "Quietus Salt Crystals", "Unspoken Sigil Ink", "Whisper-Silencing Essence" },
            new[] { "Shadowtag Cleanser", "Shadowtag Soot Marks", "Cleansewater Brine Drops", "Moonchalk Dust", "Tracker's Thread Snippings", "Unmarked Pearl Dust", "Veilroot Shavings", "Lantern-Sage Ash", "Boundary Salt Crystals", "Identity Sigil Ink", "Shadowtag Cleansing Essence" },
            new[] { "Lady Luck Restoration Libation", "Lady Luck Clover Petals", "Fortune Cup Dewdrops", "Lucky Penny Goldflakes", "Chancewheel Pearl Dust", "Rabbitfoot Fern Tufts", "Jinxbreak Salt Crystals", "Wishbone Twig Shavings", "Fortune Ribbon Snippings", "Golden Dice Pips", "Restored Luck Essence" },
            new[] { "Essence of Evil Eye", "Evil Eye Glass Beads", "Envyroot Shavings", "Mirrorlash Pearl Dust", "Blue Ward Salt Crystals", "Watcher's Soot Specks", "Protective Rue Ash", "Gaze-Breaker Thorn Tips", "Unseeing Ink Drops", "Nazar Ribbon Snippings", "Evil Eye Deflection Essence" },
            new[] { "The Witch's Knot Concoction", "Witch's Knot Cord Fibers", "Triple-Knot Thread Snippings", "Knotcharmer Briar Hooks", "Moonwax Sealings", "Knotwise Sage Ash", "Intent-Bound Brine Drops", "Knotkeeper Rosemary Sprigs", "Iron Thread Filings", "Oathloop Pearl Dust", "Witch-Knot Binding Essence" }
        },
        new[]
        {
            new[] { "Nightmare Bramble Draught", "Nightmare Bramble Thorns", "Dreamsnare Vine Fibers", "Moonless Poppy Petals", "Bramblewake Salt Crystals", "Fearroot Shavings", "Dreamdoor Wax Seals", "Ravenmoth Wing Dust", "Thornshadow Ash", "Lucid Pearl Dust", "Nightmare-Breaking Essence" },
            new[] { "Veil-Tear Mending Tonic", "Veil-Tear Silk Threads", "Liminal Needlethorn Tips", "Mistglass Shards", "Seambound Moonwax", "Threshold Pearl Dust", "Riftmoss Tufts", "Boundary Brine Drops", "Mender's Sigil Ink", "Hushveil Ash", "Veil-Mending Essence" },
            new[] { "Omni Sage Ash Clarifier", "Omni Sage Ash", "Clarifying Smoke Wisps", "Truth-Sage Leaf Crumbles", "Haze-Cutter Salt Crystals", "Discernment Pearl Dust", "Signalroot Shavings", "Smokevein Charcoal Specks", "Oracle Brine Drops", "Clear-Sight Sigil Ink", "Omni-Clarity Essence" },
            new[] { "Lingering Apparition Antidote", "Apparition Residue Drops", "Spiritveil Salt Crystals", "Ghostlight Pearl Dust", "Graveyard Sage Ash", "Ectomist Wisps", "Farewell Bell Chime Chips", "Resting Mark Chalk", "Afterimage Glass Shards", "Resting Root Shavings", "Apparition Antidote Essence" },
            new[] { "Apparitional Inhabitant Isolation Mist", "Apparitional Mist Wisps", "Isolation Circle Salt", "Spiritglass Beads", "Containment Sage Ash", "Room-Boundary Chalk Dust", "Ectoweb Silk Threads", "Quiet Bell Chime Chips", "Threshold Wax Sealings", "Stillwater Pearl Dust", "Isolation Mist Essence" },
            new[] { "Seekless Scrying Serum", "Seekless Mirror Shards", "Scryveil Ink Drops", "Hidden Pool Dewdrops", "Obscuring Smoke Wisps", "Privacy Pearl Dust", "Watcherless Salt Crystals", "Moonshadow Silk Threads", "Blindspot Sage Ash", "Untraceable Rune Chalk", "Seekless Scrying Essence" },
            new[] { "Blinding Beacon Resistance Balm", "Blinding Beacon Shards", "Shadeveil Wax Sealings", "Clear-Sight Pearl Dust", "Sunfilter Sage Ash", "Dimming Glass Beads", "Lanternshade Silk Threads", "Glarebreak Salt Crystals", "False-Light Ink Drops", "Eyelid Moonmoss Tufts", "Beacon Resistance Essence" },
            new[] { "Reliquary Resurgence Liniment", "Reliquary Dust", "Relicbone Pearl Chips", "Sanctified Oil Drops", "Heirloom Amber Resin", "Resurgence Brass Filings", "Votive Candlewax Flecks", "Ancestral Sigil Ink", "Awakening Laurel Ash", "Memory Thread Fibers", "Relic Resurgence Essence" }
        }
    };

    private static readonly AlbumRewardDefinition[][] BookOfShadowsRewards =
    {
        new[]
        {
            new AlbumRewardDefinition(2650, 150, 0, 24),
            new AlbumRewardDefinition(2700, 0, 150, 24),
            new AlbumRewardDefinition(2750, 150, 0, 24),
            new AlbumRewardDefinition(2800, 0, 150, 24),
            new AlbumRewardDefinition(2850, 150, 0, 24),
            new AlbumRewardDefinition(2900, 0, 300, 36),
            new AlbumRewardDefinition(2950, 300, 0, 36),
            new AlbumRewardDefinition(3000, 400, 400, 48)
        },
        new[]
        {
            new AlbumRewardDefinition(2650, 150, 0, 24),
            new AlbumRewardDefinition(2700, 0, 150, 24),
            new AlbumRewardDefinition(2750, 150, 0, 24),
            new AlbumRewardDefinition(2800, 0, 150, 24),
            new AlbumRewardDefinition(2850, 150, 0, 24),
            new AlbumRewardDefinition(2900, 0, 300, 36),
            new AlbumRewardDefinition(2950, 300, 0, 36),
            new AlbumRewardDefinition(3000, 400, 400, 48)
        },
        new[]
        {
            new AlbumRewardDefinition(2650, 150, 0, 24),
            new AlbumRewardDefinition(2700, 0, 150, 24),
            new AlbumRewardDefinition(2750, 150, 0, 24),
            new AlbumRewardDefinition(2800, 0, 150, 24),
            new AlbumRewardDefinition(2850, 150, 0, 24),
            new AlbumRewardDefinition(2900, 0, 300, 36),
            new AlbumRewardDefinition(2950, 300, 0, 36),
            new AlbumRewardDefinition(3000, 400, 400, 48)
        }
    };

    private static readonly List<GrimoireEntryDefinition> grimoireOneEntries = BuildGrimoireOneEntries();
    private static readonly List<BookOfShadowsSetDefinition> bookOfShadowsSets = BuildBookOfShadowsSets();
    private static readonly List<AlbumCardDefinition> allCards = BuildAllCards();
    private static readonly List<BookOfShadowsCardDefinition> allBookOfShadowsCards = BuildAllBookOfShadowsCards();

    public static IReadOnlyList<GrimoireEntryDefinition> GrimoireOneEntries => grimoireOneEntries;

    public static IReadOnlyList<BookOfShadowsSetDefinition> BookOfShadowsSets => bookOfShadowsSets;

    public static IReadOnlyList<AlbumCardDefinition> AllCards => allCards;

    public static IReadOnlyList<BookOfShadowsCardDefinition> AllBookOfShadowsCards => allBookOfShadowsCards;

    public static int TotalCards => allCards.Count;

    public static int BookOfShadowsTotalCards => allBookOfShadowsCards.Count;

    public static bool TryGetGrimoireCardById(string cardId, out AlbumCardDefinition card)
    {
        for (int index = 0; index < allCards.Count; index++)
        {
            if (allCards[index].Id == cardId)
            {
                card = allCards[index];
                return true;
            }
        }

        card = null;
        return false;
    }

    public static string GetGrimoireCardDisplayName(string cardId)
    {
        return TryGetGrimoireCardById(cardId, out AlbumCardDefinition card) ? card.CardName : cardId;
    }

    public static int CountByTier(AlbumCardTier tier)
    {
        int count = 0;
        for (int index = 0; index < allCards.Count; index++)
        {
            if (allCards[index].Tier == tier)
            {
                count++;
            }
        }

        return count;
    }

    public static int CountByStars(int stars)
    {
        int count = 0;
        for (int index = 0; index < allCards.Count; index++)
        {
            if (allCards[index].Stars == stars)
            {
                count++;
            }
        }

        return count;
    }

    public static int CountOwnedByStars(int regularOwned, int gildedOwned, int ancientOwned, int stars)
    {
        int owned = 0;
        int remainingRegular = regularOwned;
        int remainingGilded = gildedOwned;
        int remainingAncient = ancientOwned;

        for (int index = 0; index < allCards.Count; index++)
        {
            AlbumCardDefinition card = allCards[index];
            if (card.Tier == AlbumCardTier.Regular && remainingRegular > 0)
            {
                remainingRegular--;
                if (card.Stars == stars)
                {
                    owned++;
                }
            }
            else if (card.Tier == AlbumCardTier.Gilded && remainingGilded > 0)
            {
                remainingGilded--;
                if (card.Stars == stars)
                {
                    owned++;
                }
            }
            else if (card.Tier == AlbumCardTier.Ancient && remainingAncient > 0)
            {
                remainingAncient--;
                if (card.Stars == stars)
                {
                    owned++;
                }
            }
        }

        return owned;
    }

    public static int CountOwnedInEntry(GrimoireEntryDefinition entry, int regularOwned, int gildedOwned, int ancientOwned)
    {
        int owned = 0;
        int remainingRegular = regularOwned;
        int remainingGilded = gildedOwned;
        int remainingAncient = ancientOwned;

        for (int index = 0; index < allCards.Count; index++)
        {
            AlbumCardDefinition card = allCards[index];
            bool cardIsOwned = false;
            if (card.Tier == AlbumCardTier.Regular && remainingRegular > 0)
            {
                remainingRegular--;
                cardIsOwned = true;
            }
            else if (card.Tier == AlbumCardTier.Gilded && remainingGilded > 0)
            {
                remainingGilded--;
                cardIsOwned = true;
            }
            else if (card.Tier == AlbumCardTier.Ancient && remainingAncient > 0)
            {
                remainingAncient--;
                cardIsOwned = true;
            }

            if (cardIsOwned && card.EntryNumber == entry.EntryNumber)
            {
                owned++;
            }
        }

        return owned;
    }

    public static int CountBookOfShadowsByStars(int stars)
    {
        int count = 0;
        for (int index = 0; index < allBookOfShadowsCards.Count; index++)
        {
            if (allBookOfShadowsCards[index].Stars == stars)
            {
                count++;
            }
        }

        return count;
    }

    public static string BuildBookOfShadowsStarLine()
    {
        List<string> parts = new List<string>();
        for (int stars = 1; stars <= 5; stars++)
        {
            parts.Add($"{stars}* x{CountBookOfShadowsByStars(stars)}");
        }

        return string.Join("  ", parts);
    }

    public static int CountBookOfShadowsInSet(BookOfShadowsSetDefinition set)
    {
        int count = 0;
        for (int index = 0; index < set.Entries.Count; index++)
        {
            count += set.Entries[index].Cards.Count;
        }

        return count;
    }

    public static string BuildEntryCardLine(GrimoireEntryDefinition entry)
    {
        List<string> parts = new List<string>();
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            AlbumCardDefinition card = entry.Cards[index];
            parts.Add($"{card.SlotNumber}:{card.Stars}*{GetTierSuffix(card.Tier)}");
        }

        return string.Join("  ", parts);
    }

    public static string BuildRewardLine(AlbumRewardDefinition reward)
    {
        List<string> parts = new List<string>();
        if (reward.Mana > 0)
        {
            parts.Add($"{reward.Mana:n0} Mana");
        }

        if (reward.Crystals > 0)
        {
            parts.Add($"{reward.Crystals:n0} Crystals");
        }

        if (reward.PowerUps > 0)
        {
            parts.Add($"{reward.PowerUps:n0} Power-Ups");
        }

        if (reward.ClairvoyanceHours > 0)
        {
            parts.Add($"{reward.ClairvoyanceHours}h Clairvoyance");
        }

        if (!string.IsNullOrEmpty(reward.Note))
        {
            parts.Add(reward.Note);
        }

        return parts.Count == 0 ? "Reward pending" : string.Join("  |  ", parts);
    }

    public static string BuildCompactRewardLine(AlbumRewardDefinition reward)
    {
        List<string> parts = new List<string>();
        if (reward.Mana > 0)
        {
            parts.Add($"{reward.Mana:n0} M");
        }

        if (reward.Crystals > 0)
        {
            parts.Add($"{reward.Crystals:n0} C");
        }

        if (reward.PowerUps > 0)
        {
            parts.Add($"{reward.PowerUps:n0} PU");
        }

        if (reward.ClairvoyanceHours > 0)
        {
            parts.Add($"{reward.ClairvoyanceHours}h Eye");
        }

        return parts.Count == 0 ? "Reward pending" : string.Join(" | ", parts);
    }

    private static List<GrimoireEntryDefinition> BuildGrimoireOneEntries()
    {
        List<GrimoireEntryDefinition> entries = new List<GrimoireEntryDefinition>();
        for (int rowIndex = 0; rowIndex < GrimoireOneRows.Length; rowIndex++)
        {
            string[] row = GrimoireOneRows[rowIndex];
            string potionName = row[0];
            List<AlbumCardDefinition> cards = new List<AlbumCardDefinition>();
            for (int slotIndex = 0; slotIndex < GrimoireOneSlotStars.Length; slotIndex++)
            {
                string cardName = row[slotIndex + 1];
                if (string.IsNullOrEmpty(cardName))
                {
                    continue;
                }

                cards.Add(new AlbumCardDefinition(
                    1,
                    rowIndex + 1,
                    potionName,
                    slotIndex + 1,
                    cardName,
                    GrimoireOneSlotStars[slotIndex],
                    GrimoireOneSlotTiers[slotIndex]));
            }

            entries.Add(new GrimoireEntryDefinition(1, rowIndex + 1, potionName, GrimoireOneRewards[rowIndex], cards));
        }

        return entries;
    }

    private static List<BookOfShadowsSetDefinition> BuildBookOfShadowsSets()
    {
        List<BookOfShadowsSetDefinition> sets = new List<BookOfShadowsSetDefinition>();
        for (int setIndex = 0; setIndex < BookOfShadowsRows.Length; setIndex++)
        {
            List<BookOfShadowsEntryDefinition> entries = new List<BookOfShadowsEntryDefinition>();
            string[][] setRows = BookOfShadowsRows[setIndex];
            for (int rowIndex = 0; rowIndex < setRows.Length; rowIndex++)
            {
                string[] row = setRows[rowIndex];
                string potionName = row[0];
                List<BookOfShadowsCardDefinition> cards = new List<BookOfShadowsCardDefinition>();
                for (int slotIndex = 0; slotIndex < BookOfShadowsSlotStars.Length; slotIndex++)
                {
                    cards.Add(new BookOfShadowsCardDefinition(
                        setIndex + 1,
                        rowIndex + 1,
                        potionName,
                        slotIndex + 1,
                        row[slotIndex + 1],
                        BookOfShadowsSlotStars[slotIndex]));
                }

                entries.Add(new BookOfShadowsEntryDefinition(setIndex + 1, rowIndex + 1, potionName, BookOfShadowsRewards[setIndex][rowIndex], cards));
            }

            sets.Add(new BookOfShadowsSetDefinition(setIndex + 1, setIndex * 30 + 1, (setIndex + 1) * 30, entries));
        }

        return sets;
    }

    private static List<AlbumCardDefinition> BuildAllCards()
    {
        List<AlbumCardDefinition> cards = new List<AlbumCardDefinition>();
        for (int entryIndex = 0; entryIndex < grimoireOneEntries.Count; entryIndex++)
        {
            IReadOnlyList<AlbumCardDefinition> entryCards = grimoireOneEntries[entryIndex].Cards;
            for (int cardIndex = 0; cardIndex < entryCards.Count; cardIndex++)
            {
                cards.Add(entryCards[cardIndex]);
            }
        }

        return cards;
    }

    private static List<BookOfShadowsCardDefinition> BuildAllBookOfShadowsCards()
    {
        List<BookOfShadowsCardDefinition> cards = new List<BookOfShadowsCardDefinition>();
        for (int setIndex = 0; setIndex < bookOfShadowsSets.Count; setIndex++)
        {
            IReadOnlyList<BookOfShadowsEntryDefinition> entries = bookOfShadowsSets[setIndex].Entries;
            for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
            {
                IReadOnlyList<BookOfShadowsCardDefinition> entryCards = entries[entryIndex].Cards;
                for (int cardIndex = 0; cardIndex < entryCards.Count; cardIndex++)
                {
                    cards.Add(entryCards[cardIndex]);
                }
            }
        }

        return cards;
    }

    private static string GetTierSuffix(AlbumCardTier tier)
    {
        if (tier == AlbumCardTier.Gilded)
        {
            return "G";
        }

        if (tier == AlbumCardTier.Ancient)
        {
            return "A";
        }

        return "";
    }
}

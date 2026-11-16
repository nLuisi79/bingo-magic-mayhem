using System.Collections.Generic;
using UnityEngine;

// Per-realm data for the illustrated realm-home (realm map) screen. Shared chrome — header, arrows, counters,
// label plate, medallion ring, lock and badge — is identical for every realm and lives in realm_home/shared.
// Each realm supplies its own backgrounds and potions in realm_home/<folder>, plus where its rooms sit.
// Positions are stage units (1600 x 900, origin at center), i.e. the 1920 x 1080 gridded mockup's pixel
// coordinates divided by 1.2.
public sealed class RealmHomeRoomLayout
{
    public RealmHomeRoomLayout(
        string displayName,
        string potionArtName,
        Vector2 nodePosition,
        float badgeOffsetX,
        Vector2 headerPotionSize,
        float headerPotionOffsetX,
        Vector2 medallionPotionSize,
        float medallionPotionY)
    {
        DisplayName = displayName;
        PotionArtName = potionArtName;
        NodePosition = nodePosition;
        BadgeOffsetX = badgeOffsetX;
        HeaderPotionSize = headerPotionSize;
        HeaderPotionOffsetX = headerPotionOffsetX;
        MedallionPotionSize = medallionPotionSize;
        MedallionPotionY = medallionPotionY;
    }

    // Label text; "\n" splits the two lines.
    public string DisplayName { get; private set; }
    // Potion art is "<PotionArtName>-potion" / "<PotionArtName>-shadow" in the realm's ui/potions folder.
    public string PotionArtName { get; private set; }
    // Center of the medallion + label pair.
    public Vector2 NodePosition { get; private set; }
    public float BadgeOffsetX { get; private set; }
    // Potion sizes are the art's true proportions; the import squares these textures, so they are drawn unstretched by size.
    public Vector2 HeaderPotionSize { get; private set; }
    public float HeaderPotionOffsetX { get; private set; }
    public Vector2 MedallionPotionSize { get; private set; }
    public float MedallionPotionY { get; private set; }
}

public sealed class RealmHomeLayout
{
    public const string SharedUiResourceRoot = "prototype/ui/realm_home/shared/";

    public RealmHomeLayout(string artFolder, IReadOnlyList<RealmHomeRoomLayout> rooms)
    {
        ArtRoot = "prototype/ui/realm_home/" + artFolder + "/";
        BackgroundResourcePrefix = ArtRoot + artFolder + "-realm-";
        Rooms = rooms;
    }

    public string ArtRoot { get; private set; }
    // Backgrounds are "<prefix>broken0" .. "broken3" and "<prefix>restored".
    public string BackgroundResourcePrefix { get; private set; }
    public string UiResourceRoot => SharedUiResourceRoot;
    public string HeaderResourceRoot => SharedUiResourceRoot + "realm_header/";
    public string RoomLabelResourceRoot => SharedUiResourceRoot + "room_label/";
    public string PotionResourceRoot => ArtRoot + "ui/potions/";
    public IReadOnlyList<RealmHomeRoomLayout> Rooms { get; private set; }
}

public static class RealmHomeLayoutCatalog
{
    private const float HeaderPotionHeight = 84f;
    private const float MedallionPotionHeight = 115f;

    // Realms whose placements have been measured from a gridded mockup. Keyed by realm name so the layout follows
    // the realm wherever it sits in the realm order.
    private static readonly Dictionary<string, RealmHomeLayout> measuredLayouts = new Dictionary<string, RealmHomeLayout>
    {
        ["Sunpetal Conservatory"] = new RealmHomeLayout("sunpetal", new List<RealmHomeRoomLayout>
        {
            new RealmHomeRoomLayout("Gilded Azalea\nArboretum", "sunpetal-conservatory", new Vector2(-616f, 13f), 71f,
                new Vector2(59f, HeaderPotionHeight), -147f, new Vector2(81f, MedallionPotionHeight), 16f),
            new RealmHomeRoomLayout("Thorn-Tangled\nTerrace", "thorn-tangled", new Vector2(577f, 53f), 62f,
                new Vector2(64f, HeaderPotionHeight), -54f, new Vector2(87f, MedallionPotionHeight), 16f),
            new RealmHomeRoomLayout("Pollenspire\nFountain", "pollenspire", new Vector2(-463f, -312f), 63f,
                new Vector2(36f, HeaderPotionHeight), 43f, new Vector2(49f, MedallionPotionHeight), 23f),
            new RealmHomeRoomLayout("Solstice Bloom\nGlasshouse", "solstice-bloom", new Vector2(519f, -347f), 60f,
                new Vector2(61f, HeaderPotionHeight), 149f, new Vector2(84f, MedallionPotionHeight), 15f),
        }),
    };

    // Art folders follow the production asset tree (rooms unity ready/NN_<folder>).
    private static readonly Dictionary<string, string> artFolders = new Dictionary<string, string>
    {
        ["Sunpetal Conservatory"] = "sunpetal",
        ["Floating Lotus Lagoon"] = "lotus",
        ["Everbloom Sanctuary"] = "everbloom",
        ["Glimmerwisp Forest"] = "glimmerwisp",
        ["Cerulean Moon Monastery"] = "cerulean",
        ["Emberglass Grove"] = "emberglass",
        ["Howling Hollows Manor"] = "hollows",
        ["Crimson Moon Canyon"] = "crimson",
        ["Arcane Aerie Heights"] = "aerie",
        ["Crystalline Chanterelle Colony"] = "chanterelle",
        ["Everfrost Vale"] = "everfrost",
    };

    private static readonly Dictionary<string, RealmHomeLayout> defaultLayouts = new Dictionary<string, RealmHomeLayout>();

    // Every realm gets the illustrated screen. Realms without a measured mockup reuse Sunpetal's room placements and
    // show whatever realm art exists; missing backgrounds and potions are skipped rather than drawn as placeholders.
    public static RealmHomeLayout ForRealm(int realmIndex)
    {
        if (realmIndex < 0 || realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return null;
        }

        RealmDefinition realm = RealmContentCatalog.AllRealms[realmIndex];
        if (measuredLayouts.TryGetValue(realm.Name, out RealmHomeLayout measured))
        {
            return measured;
        }

        if (!defaultLayouts.TryGetValue(realm.Name, out RealmHomeLayout layout))
        {
            layout = CreateDefaultLayout(realm);
            defaultLayouts[realm.Name] = layout;
        }

        return layout;
    }

    private static RealmHomeLayout CreateDefaultLayout(RealmDefinition realm)
    {
        IReadOnlyList<RealmHomeRoomLayout> template = measuredLayouts["Sunpetal Conservatory"].Rooms;
        List<RealmHomeRoomLayout> rooms = new List<RealmHomeRoomLayout>();
        for (int index = 0; index < realm.Rooms.Count && index < template.Count; index++)
        {
            RealmHomeRoomLayout placement = template[index];
            string roomName = realm.Rooms[index].Name;
            rooms.Add(new RealmHomeRoomLayout(
                SplitIntoTwoLines(roomName),
                ToArtName(roomName),
                placement.NodePosition,
                placement.BadgeOffsetX,
                placement.HeaderPotionSize,
                placement.HeaderPotionOffsetX,
                placement.MedallionPotionSize,
                placement.MedallionPotionY));
        }

        string folder = artFolders.TryGetValue(realm.Name, out string mappedFolder) ? mappedFolder : ToArtName(realm.Name);
        return new RealmHomeLayout(folder, rooms);
    }

    // Breaks a room name at the space closest to its middle, matching the two-line labels.
    private static string SplitIntoTwoLines(string name)
    {
        int middle = name.Length / 2;
        int best = -1;
        for (int index = 0; index < name.Length; index++)
        {
            if (name[index] == ' ' && (best < 0 || Mathf.Abs(index - middle) < Mathf.Abs(best - middle)))
            {
                best = index;
            }
        }

        return best < 0 ? name : name.Substring(0, best) + "\n" + name.Substring(best + 1);
    }

    private static string ToArtName(string name)
    {
        return name.ToLowerInvariant().Replace(' ', '-').Replace("'", string.Empty);
    }
}

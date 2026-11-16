using System.Collections.Generic;
using UnityEngine;

// Per-realm data for the illustrated realm-home (realm map) screen. Shared chrome — header, arrows, counters,
// label plate and medallion geometry — lives in BingoPrototype; each realm supplies its art folder and how its
// rooms sit on its background. Positions are stage units (1600 x 900, origin at center), i.e. the 1920 x 1080
// gridded mockup's pixel coordinates divided by 1.2.
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
    public RealmHomeLayout(string artFolder, IReadOnlyList<RealmHomeRoomLayout> rooms)
        : this(artFolder, artFolder + "-realm-", rooms)
    {
    }

    public RealmHomeLayout(string artFolder, string backgroundFilePrefix, IReadOnlyList<RealmHomeRoomLayout> rooms)
    {
        ArtRoot = "prototype/ui/realm_home/" + artFolder + "/";
        BackgroundResourcePrefix = ArtRoot + backgroundFilePrefix;
        Rooms = rooms;
    }

    public string ArtRoot { get; private set; }
    // Backgrounds are "<prefix>broken0" .. "broken3" and "<prefix>restored".
    public string BackgroundResourcePrefix { get; private set; }
    public string UiResourceRoot => ArtRoot + "ui/";
    public string HeaderResourceRoot => UiResourceRoot + "realm_header/";
    public string RoomLabelResourceRoot => UiResourceRoot + "room_label/";
    public string PotionResourceRoot => UiResourceRoot + "potions/";
    public IReadOnlyList<RealmHomeRoomLayout> Rooms { get; private set; }
}

public static class RealmHomeLayoutCatalog
{
    private const float HeaderPotionHeight = 84f;
    private const float MedallionPotionHeight = 115f;

    private static readonly Dictionary<int, RealmHomeLayout> layoutsByRealmIndex = new Dictionary<int, RealmHomeLayout>
    {
        [0] = new RealmHomeLayout("sunpetal", new List<RealmHomeRoomLayout>
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

    // Realms without an entry fall back to the generic realm-map screen.
    public static RealmHomeLayout ForRealm(int realmIndex)
    {
        return layoutsByRealmIndex.TryGetValue(realmIndex, out RealmHomeLayout layout) ? layout : null;
    }
}

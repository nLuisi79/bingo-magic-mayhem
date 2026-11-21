using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoMagicMayhem.Cosmetics;
using BingoMagicMayhem.Infrastructure;
using BingoMagicMayhem.Jackpot;
using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using BingoMagicMayhem.UI.Common;
using BingoMagicMayhem.UI.Bazaar;
using BingoMagicMayhem.UI.Collections;
using BingoMagicMayhem.UI.Coven;
using BingoMagicMayhem.UI.Den;
using BingoMagicMayhem.UI.Friends;
using BingoMagicMayhem.UI.Inbox;
using BingoMagicMayhem.UI.Leaderboard;
using BingoMagicMayhem.UI.Market;
using BingoMagicMayhem.UI.Rewards;
using BingoMagicMayhem.UI.RoomEntry;
using BingoMagicMayhem.UI.Trail;
using BingoMagicMayhem.UI.Navigation;
using BingoMagicMayhem.UI.Profile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Realm Home screen: room nodes, badges, nine-slice title/label frames, background art and realm/room unlock checks.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildRealmMapUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        StopRoundAndClearCalledHistory();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.1f, 0.22f, 0.17f, 0.98f), 0, 0);
        layout.enabled = false;
        activeStageSafePadding = 0f;
        RealmHomeLayout realmHome = RealmHomeLayoutCatalog.ForRealm(RealmContentCatalog.ActivePrototypeRealmIndex);
        activeStageFillsViewport = realmHome != null;

        if (realmHome != null)
        {
            CreateRealmHomeBackground(realmHome);
            CreateRealmHomeMapUi(realmHome);
            ApplyStageScale(true);
            return;
        }

        GameObject accountBar = CreateAnchoredPanel(contentRoot, "RealmAccountBar", new Color(0.09f, 0.04f, 0.15f), 720, 74, 395f, 386f);
        lobbyManaText = CreateLobbyResourceTile(accountBar.transform, "Mana", inventory.GetManaText(), "*", 220, -245f);
        lobbyCrystalText = CreateLobbyResourceTile(accountBar.transform, "Crystals", inventory.GetCrystalText(), "<>", 220, 0f);
        CreateLobbyResourceTile(accountBar.transform, "Power-Ups", inventory.GetPowerUpText(), "+", 240, 255f);

        Button map = CreateAnchoredButton(contentRoot, "Map", 18, 82, 54, new Color(0.35f, 0.12f, 0.62f), -732f, 386f);
        map.onClick.AddListener(() => BeginLoadingTransition(BuildWorldMapUi));

        GameObject titlePanel = CreateAnchoredPanel(contentRoot, "RealmTitle", new Color(0.08f, 0.28f, 0.12f), 860, 150, -260f, 284f);
        CreateAnchoredText(titlePanel.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 58, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 820, 72, 0f, 14f);
        CreateAnchoredText(titlePanel.transform, $"Realm {RealmContentCatalog.ActivePrototypeRealmIndex + 1} / {RealmContentCatalog.AllRealms.Count}", 20, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 760, 32, 0f, -52f);

        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        CreateRealmRoomNode(rooms[0], 0, -390f, 100f);
        CreateRealmRoomNode(rooms[1], 1, 430f, 125f);
        CreateRealmRoomNode(rooms[2], 2, -520f, -245f);
        CreateRealmRoomNode(rooms[3], 3, 390f, -255f);

        CreateAnchoredText(contentRoot, "Restore each room's potion to unlock the next room path.", 24, FontStyle.Bold, new Color(0.9f, 0.95f, 0.86f), 980, 40, 0f, -410f);

        ApplyStageScale(true);
    }

    private void CreateRealmHomeMapUi(RealmHomeLayout realmHome)
    {
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        int realmIndex = RealmContentCatalog.ActivePrototypeRealmIndex;
        CreateRealmHomeTopNavigation();

        // Shared chrome positions come from the 1920 x 1080 gridded Sunpetal mockup, converted to the
        // 1600 x 900 stage (divide by 1.2). Per-realm placements live in RealmHomeLayoutCatalog.
        Image mapArrow = CreateAnchoredImage(contentRoot, realmHome.UiResourceRoot + "nav-left", 75f, 75f, -751f, 354f);
        mapArrow.name = "Return to world map";
        Button mapButton = mapArrow.gameObject.AddComponent<Button>();
        mapButton.targetGraphic = mapArrow;
        mapButton.onClick.AddListener(() => BeginLoadingTransition(BuildWorldMapUi));

        int nextRealmIndex = realmIndex + 1;
        Image nextArrow = CreateAnchoredImage(contentRoot, realmHome.UiResourceRoot + "nav-right", 75f, 75f, -131f, 354f);
        nextArrow.name = "Next realm";
        Button nextButton = nextArrow.gameObject.AddComponent<Button>();
        nextButton.targetGraphic = nextArrow;
        nextButton.interactable = nextRealmIndex < RealmContentCatalog.AllRealms.Count && IsRealmUnlocked(nextRealmIndex);
        ColorBlock nextArrowColors = nextButton.colors;
        // The mockup keeps the arrow solid while the next realm is locked; it just ignores taps.
        nextArrowColors.disabledColor = Color.white;
        nextButton.colors = nextArrowColors;
        nextButton.onClick.AddListener(() => EnterRealm(nextRealmIndex));

        GameObject header = CreateRealmHomeNineTileFrame(
            contentRoot,
            "Realm home header",
            realmHome.HeaderResourceRoot,
            578f,
            168f,
            -443f,
            355f);
        Text realmTitle = CreateAnchoredText(header.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 38, FontStyle.Normal, new Color(0.05f, 0.16f, 0.07f), 570f, 52f, 6f, -41f);
        ApplyWorldMapDisplayTextTreatment(realmTitle, WorldMapProfileFontResource, new Vector2(1f, -1f));

        const float headerPotionY = 21f;
        int roomCount = Mathf.Min(rooms.Count, realmHome.Rooms.Count);
        for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
        {
            RealmHomeRoomLayout roomLayout = realmHome.Rooms[roomIndex];
            string potionState = inventory.IsRoomRestored(rooms[roomIndex]) ? "potion" : "shadow";
            string potionResource = GetRealmHomePotionResource(realmHome, roomLayout, potionState);
            if (!RealmHomeArtExists(potionResource))
            {
                continue;
            }

            Image potion = CreateAnchoredImage(
                header.transform,
                potionResource,
                roomLayout.HeaderPotionSize.x,
                roomLayout.HeaderPotionSize.y,
                roomLayout.HeaderPotionOffsetX,
                headerPotionY);
            // The import rescales these textures to power-of-two sizes, so draw at the art's true proportions instead.
            potion.preserveAspect = false;
            potion.raycastTarget = false;
        }

        for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
        {
            CreateRealmHomeRoomNode(realmHome, rooms[roomIndex], roomIndex);
        }
    }

    private void CreateRealmHomeTopNavigation()
    {
        // The shared world-map counters are laid out in their own frame; scale them down to the mockup's pill size.
        GameObject counterRoot = CreateAnchoredPanel(contentRoot, "Realm home top counters", Color.clear, 1f, 1f, 108f, -4f);
        counterRoot.transform.localScale = new Vector3(0.846f, 0.846f, 1f);
        counterRoot.GetComponent<Image>().raycastTarget = false;
        CreateTopCounter(counterRoot.transform, "Mana", "Nav unity ready/profile inventory/nav_profileMana_count", 8f);
        CreateTopCounter(counterRoot.transform, "Crystal", "Nav unity ready/profile inventory/nav_profileCrystal_count", 8f);
        CreateTopCounter(counterRoot.transform, "Power", "Nav unity ready/profile inventory/nav_profilePowerUp_count", 8f);

        Image home = CreateMapImage(contentRoot, "Realm home Home", "Nav unity ready/top nav/nav_worldHome_default", 78f, 78f, 738f, 396f);
        Button homeButton = home.gameObject.AddComponent<Button>();
        homeButton.targetGraphic = home;
        homeButton.onClick.AddListener(BuildPlayerDenUi);
    }

    private void CreateRealmHomeRoomNode(RealmHomeLayout realmHome, RoomDefinition room, int roomIndex)
    {
        RealmHomeRoomLayout roomLayout = realmHome.Rooms[roomIndex];
        bool available = IsRoomUnlocked(roomIndex);
        // The node is centered on the medallion + label pair; the medallion sits at the node's left.
        const float medallionX = -102f;
        GameObject node = CreateAnchoredPanel(contentRoot, $"RealmHomeRoom_{roomIndex + 1}", Color.clear, 305f, 110f, roomLayout.NodePosition.x, roomLayout.NodePosition.y);
        Image nodeImage = node.GetComponent<Image>();
        Button nodeButton = node.AddComponent<Button>();
        nodeButton.targetGraphic = nodeImage;
        nodeButton.interactable = available;
        if (available)
        {
            nodeButton.onClick.AddListener(() => EnterRoom(roomIndex));
        }

        // Names too wide for the stock plate add the right fill slice; only names too long for that also add the left one.
        // The left edge stays tucked behind the medallion either way.
        const int roomTitleFontSize = 25;
        const float labelNaturalWidth = 288f;
        const float titleNaturalWidth = 178f;
        const float titleLeftX = -36f;
        const float titleFitLimit = 165f;
        string roomName = roomLayout.DisplayName;
        float nameWidth = MeasureWidestTextLine(Resources.Load<Font>(WorldMapDisplayFontResource), roomTitleFontSize, roomName);
        float fillWidth = GetRealmHomeLabelFillWidth(labelNaturalWidth);
        int fillColumns = nameWidth <= titleFitLimit ? 0 : nameWidth <= titleFitLimit + fillWidth ? 1 : 2;
        float labelGrowth = fillColumns * fillWidth;
        float labelX = 8f + labelGrowth / 2f;
        GameObject label = fillColumns > 0
            ? CreateRealmHomeWideLabelFrame(node.transform, $"Room {roomIndex + 1} label", realmHome.RoomLabelResourceRoot, labelNaturalWidth, 81f, labelX, 0f, fillColumns == 2)
            : CreateRealmHomeNineTileFrame(node.transform, $"Room {roomIndex + 1} label", realmHome.RoomLabelResourceRoot, labelNaturalWidth, 81f, labelX, 0f);
        float titleWidth = titleNaturalWidth + labelGrowth;
        float titleCenterX = titleLeftX + titleWidth / 2f - labelX;
        Text roomTitle = CreateAnchoredText(label.transform, roomName, roomTitleFontSize, FontStyle.Normal, new Color(1f, 0.86f, 0.3f), titleWidth, 66f, titleCenterX, -1f);
        ApplyWorldMapDisplayTextTreatment(roomTitle, WorldMapDisplayFontResource, new Vector2(1f, -1f));
        roomTitle.alignment = TextAnchor.MiddleLeft;
        // Two name lines sit 23 stage units apart (tighter than the mockup's 26); derive the multiplier from the font's own line height.
        const float roomTitleLineGap = 23f;
        if (roomTitle.font != null && roomTitle.font.lineHeight > 0 && roomTitle.font.fontSize > 0)
        {
            float naturalLineGap = roomTitle.font.lineHeight * (float)roomTitle.fontSize / roomTitle.font.fontSize;
            roomTitle.lineSpacing = roomTitleLineGap / naturalLineGap;
        }

        Image frame = CreateAnchoredImage(node.transform, realmHome.UiResourceRoot + "room-frame", 101f, 101f, medallionX, 0f);
        frame.raycastTarget = false;
        bool restored = inventory.IsRoomRestored(room);
        string medallionResource = restored
            ? GetRealmHomePotionResource(realmHome, roomLayout, "potion")
            : available
                ? GetRealmHomePotionResource(realmHome, roomLayout, "shadow")
                : realmHome.UiResourceRoot + "room-lock";
        float medallionWidth;
        float medallionHeight;
        float medallionY;
        if (restored)
        {
            medallionWidth = roomLayout.MedallionPotionSize.x;
            medallionHeight = roomLayout.MedallionPotionSize.y;
            medallionY = roomLayout.MedallionPotionY;
        }
        else if (available)
        {
            // The mockup draws the shadow silhouette smaller than the full potion so it sits inside the ring.
            const float shadowScale = 0.76f;
            medallionWidth = roomLayout.MedallionPotionSize.x * shadowScale;
            medallionHeight = roomLayout.MedallionPotionSize.y * shadowScale;
            medallionY = 11f;
        }
        else
        {
            medallionWidth = 60f;
            medallionHeight = 60f;
            medallionY = 0f;
        }

        // Realms whose potion art isn't delivered yet show an empty ring rather than a placeholder block.
        if (RealmHomeArtExists(medallionResource))
        {
            Image medallion = CreateAnchoredImage(node.transform, medallionResource, medallionWidth, medallionHeight, medallionX, medallionY);
            medallion.preserveAspect = false;
            medallion.raycastTarget = false;
        }

        // Unlocked rooms show ingredient progress toward their potion. Restoring spends the requirement,
        // so a restored room shows the leftover ingredients counting toward its next brew. Locked rooms show no badge.
        if (!available)
        {
            return;
        }

        Image countBadge = CreateAnchoredImage(node.transform, realmHome.UiResourceRoot + "potion-count", 103f, 45f, roomLayout.BadgeOffsetX, -60f);
        countBadge.preserveAspect = false;
        countBadge.raycastTarget = false;
        Text count = CreateAnchoredText(countBadge.transform, inventory.GetCombinedIngredientProgressText(room), 24, FontStyle.Normal, new Color(0.28f, 0.12f, 0.02f), 78f, 36f, 0f, -1f);
        ApplyWorldMapDisplayTextTreatment(count, WorldMapDisplayFontResource, new Vector2(1f, -1f));
        count.fontSize = GetRealmHomeBadgeFontSize(count.font, 78f);
        count.horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    private static float MeasureWidestTextLine(Font font, int fontSize, string text)
    {
        if (font == null || string.IsNullOrEmpty(text))
        {
            return 0f;
        }

        font.RequestCharactersInTexture(text, fontSize);
        float widest = 0f;
        foreach (string line in text.Split('\n'))
        {
            float width = 0f;
            foreach (char character in line)
            {
                if (font.GetCharacterInfo(character, out CharacterInfo info, fontSize))
                {
                    width += info.advance;
                }
            }

            widest = Mathf.Max(widest, width);
        }

        return widest;
    }

    // Every badge uses one size, chosen so the widest expected count ("99/99") fits with the mockup's side padding.
    private int GetRealmHomeBadgeFontSize(Font font, float maxWidth)
    {
        const string widestCount = "99/99";
        const int maxSize = 24;
        if (font == null)
        {
            return maxSize;
        }

        font.RequestCharactersInTexture(widestCount, maxSize);
        float width = 0f;
        foreach (char character in widestCount)
        {
            if (font.GetCharacterInfo(character, out CharacterInfo info, maxSize))
            {
                width += info.advance;
            }
        }

        if (width <= maxWidth)
        {
            return maxSize;
        }

        return Mathf.Max(12, Mathf.FloorToInt(maxSize * maxWidth / width));
    }

    private GameObject CreateRealmHomeNineTileFrame(
        Transform parent,
        string name,
        string resourceRoot,
        float width,
        float height,
        float x,
        float y)
    {
        GameObject frame = CreateAnchoredPanel(parent, name, Color.clear, width, height, x, y);
        frame.GetComponent<Image>().raycastTarget = false;
        float tileStepX = width / 3f;
        float tileStepY = height / 3f;
        float tileWidth = tileStepX + 2f;
        float tileHeight = tileStepY + 2f;
        string[] columns = { "a", "b", "c" };

        for (int column = 0; column < columns.Length; column++)
        {
            for (int row = 0; row < 3; row++)
            {
                Image tile = CreateAnchoredImage(
                    frame.transform,
                    resourceRoot + "tile-" + columns[column] + (row + 1),
                    tileWidth,
                    tileHeight,
                    (column - 1) * tileStepX,
                    (1 - row) * tileStepY);
                tile.preserveAspect = false;
                tile.raycastTarget = false;
                // Default Repeat wrapping bleeds each tile's opposite edge into the joins, drawing a visible seam line.
                if (tile.sprite != null)
                {
                    tile.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                }
            }
        }

        return frame;
    }

    // Source-pixel geometry of the room-label art: 1623 x 458 tiles, plus 746-wide fill slices that are cropped
    // to the visible border (row 1 starts at the border, row 3 ends 17 px lower than the tiles' border top).
    private const float LabelTileSourceWidth = 1623f;
    private const float LabelTileSourceHeight = 458f;
    private const float LabelFillSourceWidth = 746f;
    private static readonly float[] LabelFillSourceHeights = { 366f, 459f, 412f };
    private const float LabelFillRow3SourceOffset = 17f;

    private float GetRealmHomeLabelFillWidth(float naturalWidth)
    {
        return naturalWidth / 3f * LabelFillSourceWidth / LabelTileSourceWidth;
    }

    // Builds a | b | tr | c (or a | tl | b | tr | c for very long names) at natural scale,
    // so a long room name gets a wider plate without stretching any art.
    private GameObject CreateRealmHomeWideLabelFrame(Transform parent, string name, string labelResourceRoot, float naturalWidth, float height, float x, float y, bool includeLeftFill)
    {
        float tileStepX = naturalWidth / 3f;
        float fillWidth = GetRealmHomeLabelFillWidth(naturalWidth);
        float totalWidth = naturalWidth + (includeLeftFill ? 2f : 1f) * fillWidth;
        float tileStepY = height / 3f;
        float sourceToStage = tileStepY / LabelTileSourceHeight;

        GameObject frame = CreateAnchoredPanel(parent, name, Color.clear, totalWidth, height, x, y);
        frame.GetComponent<Image>().raycastTarget = false;

        float left = -totalWidth / 2f;
        string[] columns = includeLeftFill
            ? new[] { "tile-a", "fill-tl", "tile-b", "fill-tr", "tile-c" }
            : new[] { "tile-a", "tile-b", "fill-tr", "tile-c" };
        float[] widths = includeLeftFill
            ? new[] { tileStepX, fillWidth, tileStepX, fillWidth, tileStepX }
            : new[] { tileStepX, tileStepX, fillWidth, tileStepX };
        for (int column = 0; column < columns.Length; column++)
        {
            float centerX = left + widths[column] / 2f;
            bool isFill = columns[column].StartsWith("fill-");
            for (int row = 0; row < 3; row++)
            {
                float rowTop = height / 2f - row * tileStepY;
                float tileHeight = tileStepY;
                float centerY = rowTop - tileStepY / 2f;
                if (isFill)
                {
                    tileHeight = LabelFillSourceHeights[row] * sourceToStage;
                    if (row == 0)
                    {
                        centerY = rowTop - tileStepY + tileHeight / 2f;
                    }
                    else if (row == 2)
                    {
                        centerY = rowTop - LabelFillRow3SourceOffset * sourceToStage - tileHeight / 2f;
                    }
                }

                Image tile = CreateAnchoredImage(
                    frame.transform,
                    labelResourceRoot + columns[column] + (row + 1),
                    widths[column] + 2f,
                    tileHeight + 2f,
                    centerX,
                    centerY);
                tile.preserveAspect = false;
                tile.raycastTarget = false;
                if (tile.sprite != null)
                {
                    tile.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                }
            }

            left += widths[column];
        }

        return frame;
    }

    private string GetRealmHomePotionResource(RealmHomeLayout realmHome, RealmHomeRoomLayout roomLayout, string state)
    {
        return realmHome.PotionResourceRoot + roomLayout.PotionArtName + "-" + state;
    }

    private static bool RealmHomeArtExists(string resourcePath)
    {
        return Resources.Load<Texture2D>(resourcePath) != null;
    }

    private void CreateRealmHomeBackground(RealmHomeLayout realmHome)
    {
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        int restoredRoomCount = 0;
        for (int index = 0; index < rooms.Count; index++)
        {
            if (inventory.IsRoomRestored(rooms[index]))
            {
                restoredRoomCount++;
            }
        }

        string stateAssetName = restoredRoomCount >= rooms.Count
            ? "restored"
            : "broken" + Mathf.Clamp(restoredRoomCount, 0, 3);
        string backgroundResource = realmHome.BackgroundResourcePrefix + stateAssetName;
        if (!RealmHomeArtExists(backgroundResource))
        {
            // No background delivered for this realm/state yet: keep the stage's plain backdrop.
            GameObject placeholder = CreateAnchoredPanel(contentRoot, RealmHomeBackgroundObjectPrefix + stateAssetName, Color.clear, StageWidth, StageHeight, 0f, 0f);
            placeholder.GetComponent<Image>().raycastTarget = false;
            placeholder.transform.SetAsFirstSibling();
            return;
        }

        // Optional wide "bleed" art (see Docs/REALM_HOME_WIDE_ART_SPEC.md): same scene painted out to 2200x1350 stage
        // units with the original 1600x900 composition untouched in the centre, so rooms stay aligned and wide phones /
        // tablets show real scenery instead of the dimmed side bands. Falls back to the 16:9 art when not delivered.
        string wideResource = backgroundResource + StageFit.WideArtResourceSuffix;
        bool hasWideArt = RealmHomeArtExists(wideResource);
        Image background = CreateAnchoredImage(
            contentRoot,
            hasWideArt ? wideResource : backgroundResource,
            hasWideArt ? StageFit.WideArtSize.x : StageWidth,
            hasWideArt ? StageFit.WideArtSize.y : StageHeight,
            0f,
            0f);
        background.name = RealmHomeBackgroundObjectPrefix + stateAssetName;
        background.preserveAspect = false;
        background.raycastTarget = false;
        background.transform.SetAsFirstSibling();

        // On screens wider or taller than 16:9 the stage is fit (so rooms stay aligned with the art); a dimmed,
        // enlarged copy of the same art fills the leftover bands instead of plain bars. Sized in ApplyStageScale.
        // With wide art this only shows on screens beyond the wide-art envelope. Uses the 16:9 art, which matches the
        // cover sizing in ApplyStageScale.
        Image ambient = CreateAnchoredImage(transform, backgroundResource, StageWidth, StageHeight, 0f, 0f);
        ambient.name = "Realm home ambient backdrop";
        ambient.preserveAspect = false;
        ambient.raycastTarget = false;
        ambient.color = new Color(0.45f, 0.45f, 0.5f, 1f);
        ambient.transform.SetAsFirstSibling();
        stageAmbientBackdrop = ambient.rectTransform;
    }

    private void CreateRealmSelectorRow()
    {
        float startX = -502f;
        for (int index = 0; index < RealmContentCatalog.AllRealms.Count; index++)
        {
            bool active = index == RealmContentCatalog.ActivePrototypeRealmIndex;
            bool unlocked = IsRealmUnlocked(index);
            Color color = active
                ? new Color(0.12f, 0.55f, 0.08f)
                : unlocked ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.18f, 0.08f, 0.28f);
            Button realmButton = CreateAnchoredButton(contentRoot, $"R{index + 1}", 17, 72, 42, color, startX + (index * 82f), 174f);
            int realmIndex = index;
            realmButton.onClick.AddListener(() => SelectRealmForMap(realmIndex));
        }
    }

    private void CreateRealmRoomNode(RoomDefinition room, int roomIndex, float x, float y)
    {
        bool available = IsRoomUnlocked(roomIndex);
        string status = GetRoomMapStatus(room, roomIndex, available);
        Color nodeColor = available ? new Color(0.18f, 0.36f, 0.14f) : new Color(0.12f, 0.14f, 0.12f);
        GameObject node = CreateAnchoredPanel(contentRoot, $"MapRoom_{roomIndex + 1}", nodeColor, 540, 150, x, y);
        CreateAnchoredText(node.transform, room.Name, 28, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 500, 42, 0f, 48f);
        CreateAnchoredText(node.transform, status, 20, FontStyle.Bold, available ? new Color(0.8f, 1f, 0.55f) : new Color(0.7f, 0.7f, 0.68f), 210, 30, -146f, 8f);
        CreateAnchoredText(node.transform, GetRoomMapIngredientText(room), 17, FontStyle.Bold, Color.white, 320, 56, 78f, -20f);

        Button button = CreateAnchoredButton(node.transform, available ? "Enter" : "Locked", 18, 132, 40, available ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.25f, 0.25f, 0.25f), -158f, -46f);
        button.interactable = available;
        if (available)
        {
            button.onClick.AddListener(() => EnterRoom(roomIndex));
        }
    }

    private bool IsRoomUnlocked(int roomIndex)
    {
        return GetRoomProgressState(RealmContentCatalog.ActivePrototypeRealmIndex, roomIndex) != RoomProgressState.Locked;
    }

    // Normal progression plus any Prototype Settings > Room Controls override (RoomAccessRules).
    private RoomProgressState GetRoomProgressState(int realmIndex, int roomIndex)
    {
        if (realmIndex < 0 || realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return RoomProgressState.Locked;
        }

        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.AllRealms[realmIndex].Rooms;
        if (roomIndex < 0 || roomIndex >= rooms.Count)
        {
            return RoomProgressState.Locked;
        }

        RoomDefinition room = rooms[roomIndex];
        return RoomAccessRules.ResolveRoom(
            inventory.IsRoomRestored(room),
            inventory.GetRoomStateOverride(room),
            IsRealmUnlocked(realmIndex),
            roomIndex == 0,
            roomIndex > 0 && inventory.IsRoomRestored(rooms[roomIndex - 1]));
    }

    private string GetRoomMapStatus(RoomDefinition room, int roomIndex, bool available)
    {
        if (!IsRealmUnlocked(RealmContentCatalog.ActivePrototypeRealmIndex))
        {
            return "Realm Locked";
        }

        if (inventory.IsRoomRestored(room))
        {
            return "Restored";
        }

        if (!available)
        {
            return "Locked";
        }

        if (inventory.CanRestoreRoom(room))
        {
            return "Potion Ready";
        }

        return roomIndex == RealmContentCatalog.ActivePrototypeRoomIndex ? "Selected" : "Unlocked";
    }

    private void EnterRoom(int roomIndex)
    {
        if (!IsRoomUnlocked(roomIndex))
        {
            return;
        }

        BeginLoadingTransition(() =>
        {
            RealmContentCatalog.SetActivePrototypeRoom(roomIndex);
            inventory.RefreshActiveRoomState();
            ApplySavedManaBetForActiveRoom();
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.RoomEntered,
                PrototypeAnalyticsPayloadFactory.CreateRoomEnteredJson(
                    selectedCardCount,
                    manaBetPerCard,
                    inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom)));
            BuildLobbyUi();
        });
    }

    private bool IsRealmUnlocked(int realmIndex)
    {
        if (realmIndex <= 0)
        {
            return true;
        }

        if (realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return false;
        }

        return RoomAccessRules.IsRealmUnlocked(false, IsRealmComplete(realmIndex - 1), HasForcedAccessibleRoom(realmIndex));
    }

    private bool HasForcedAccessibleRoom(int realmIndex)
    {
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.AllRealms[realmIndex].Rooms;
        for (int index = 0; index < rooms.Count; index++)
        {
            RoomStateOverride roomOverride = inventory.GetRoomStateOverride(rooms[index]);
            if (roomOverride == RoomStateOverride.Open || roomOverride == RoomStateOverride.Complete)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsRealmComplete(int realmIndex)
    {
        if (realmIndex < 0 || realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return false;
        }

        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.AllRealms[realmIndex].Rooms;
        for (int index = 0; index < rooms.Count; index++)
        {
            if (!inventory.IsRoomRestored(rooms[index]))
            {
                return false;
            }
        }

        return true;
    }

    private void EnterRealm(int realmIndex)
    {
        if (!IsRealmUnlocked(realmIndex))
        {
            return;
        }

        RealmContentCatalog.SetActivePrototypeRealm(realmIndex);
        inventory.RefreshActiveRoomState();
        ApplySavedManaBetForActiveRoom();
        BuildRealmMapUi();
    }

    private void ChangeActiveRealm(int direction)
    {
        int targetRealm = RealmContentCatalog.ActivePrototypeRealmIndex + direction;
        if (targetRealm < 0 || targetRealm >= RealmContentCatalog.AllRealms.Count)
        {
            return;
        }

        RealmContentCatalog.SetActivePrototypeRealm(targetRealm);
        inventory.RefreshActiveRoomState();
        ApplySavedManaBetForActiveRoom();
        BuildRealmMapUi();
    }

    private void SelectRealmForMap(int realmIndex)
    {
        if (realmIndex < 0 || realmIndex >= RealmContentCatalog.AllRealms.Count)
        {
            return;
        }

        RealmContentCatalog.SetActivePrototypeRealm(realmIndex);
        inventory.RefreshActiveRoomState();
        ApplySavedManaBetForActiveRoom();
        BuildRealmMapUi();
    }

    private void CreateRoomModeBadge(Transform parent, RoomDefinition room, float x, float y, float width, float height)
    {
        Color badgeColor = room.IsSpecial ? new Color(0.38f, 0.08f, 0.58f) : new Color(0.18f, 0.11f, 0.34f);
        GameObject badge = CreateAnchoredPanel(parent, $"{room.ModeLabel}ModeBadge", badgeColor, width, height, x, y);
        CreateAnchoredText(badge.transform, room.ModeLabel.ToUpperInvariant(), 13, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 10, 18, 0f, 20f);
        CreateModeIcon(badge.transform, room, width - 22, 28, 0f, -10f);
    }

    private void CreateModeIcon(Transform parent, RoomDefinition room, float width, float height, float x, float y)
    {
        GameObject icon = CreateAnchoredPanel(parent, $"{room.ModeLabel}ModeIcon", new Color(0.96f, 0.86f, 0.62f), width, height, x, y);
        if (room.IsSpecial)
        {
            CreateAnchoredText(icon.transform, "25", 17, FontStyle.Bold, new Color(0.35f, 0.06f, 0.52f), width - 8, height - 6, 0f, 0f);
            return;
        }

        CreateAnchoredText(icon.transform, "*****", 15, FontStyle.Bold, new Color(0.9f, 0.18f, 1f), width - 8, height - 6, 0f, 0f);
    }

    private string GetRoomMapIngredientText(RoomDefinition room)
    {
        return inventory.GetIngredientProgressText(room);
    }
}

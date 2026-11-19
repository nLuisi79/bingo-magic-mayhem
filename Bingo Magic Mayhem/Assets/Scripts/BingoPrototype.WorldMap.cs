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

// World Map screen: realm placement, visual states, HUD, slide/loading transitions and the restore transition.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private sealed class WorldMapRealmPlacement
    {
        public WorldMapRealmPlacement(string folder, string assetPrefix, float x, float y, float frameWidth, float frameHeight, float doorWidth, float doorHeight)
        {
            Folder = folder;
            AssetPrefix = assetPrefix;
            X = x;
            Y = y;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            DoorWidth = doorWidth;
            DoorHeight = doorHeight;
        }

        public string Folder { get; private set; }
        public string AssetPrefix { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float FrameWidth { get; private set; }
        public float FrameHeight { get; private set; }
        public float DoorWidth { get; private set; }
        public float DoorHeight { get; private set; }
    }

    private enum WorldMapRealmVisualState
    {
        Locked,
        BrokenClosed,
        Open,
        Restoring,
        RestoredOpen,
        RestoredClosed
    }

    private WorldMapRealmVisualState GetWorldMapRealmVisualState(int realmIndex)
    {
        if (realmIndex == worldMapPreviewRealmIndex)
        {
            return worldMapPreviewRealmState;
        }

        if (realmIndex == worldMapRestoringRealmIndex)
        {
            return WorldMapRealmVisualState.Restoring;
        }

        if (!IsRealmUnlocked(realmIndex))
        {
            return WorldMapRealmVisualState.Locked;
        }

        if (IsRealmComplete(realmIndex))
        {
            return IsWorldMapRealmRestoredClosed(realmIndex)
                ? WorldMapRealmVisualState.RestoredClosed
                : WorldMapRealmVisualState.RestoredOpen;
        }

        return HasWorldMapRealmBeenOpened(realmIndex)
            ? WorldMapRealmVisualState.Open
            : WorldMapRealmVisualState.BrokenClosed;
    }

    private bool IsWorldMapRealmRestoredClosed(int realmIndex)
    {
        return DurablePrefs.GetInt(WorldMapRestoredClosedSavePrefix + realmIndex, 0) == 1;
    }

    private bool HasWorldMapRealmBeenOpened(int realmIndex)
    {
        return DurablePrefs.GetInt(WorldMapOpenedSavePrefix + realmIndex, 0) == 1;
    }

    private void MarkWorldMapRealmOpened(int realmIndex)
    {
        DurablePrefs.SetInt(WorldMapOpenedSavePrefix + realmIndex, 1);
        DurablePrefs.Save();
    }

    private void MarkPreviousRealmRestoredClosedAfterRoundStart()
    {
        int currentRealmIndex = RealmContentCatalog.ActivePrototypeRealmIndex;
        int previousRealmIndex = currentRealmIndex - 1;
        if (previousRealmIndex < 0 || !IsRealmComplete(previousRealmIndex))
        {
            return;
        }

        DurablePrefs.SetInt(WorldMapRestoredClosedSavePrefix + previousRealmIndex, 1);
        DurablePrefs.Save();
    }

    private IEnumerator CompleteWorldMapRestoreTransition(int realmIndex)
    {
        yield return new WaitForSecondsRealtime(0.35f);

        if (activeWorldMapPortalSwirl != null)
        {
            const float fadeDuration = 0.7f;
            Color originalColor = activeWorldMapPortalSwirl.color;
            for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.unscaledDeltaTime)
            {
                Color fadedColor = originalColor;
                fadedColor.a = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
                activeWorldMapPortalSwirl.color = fadedColor;
                yield return null;
            }
        }

        if (worldMapRestoringRealmIndex == realmIndex)
        {
            worldMapRestoringRealmIndex = -1;
            worldMapRestoreTransitionRoutine = null;
            BuildWorldMapUi();
        }
    }

    private void BuildWorldMapUi()
    {
        activeWorldMapPortalSwirl = null;
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        VerticalLayoutGroup layout = PrepareStage(1920f, 1080f, Color.black, 0, 0);
        layout.enabled = false;
        activeStageSafePadding = 0f;
        Image rootBackground = GetComponent<Image>();
        if (rootBackground != null)
        {
            rootBackground.color = Color.black;
        }

        // The viewport is exactly the first 1920-pixel page of the long mock.
        GameObject viewport = CreateAnchoredLayer(contentRoot, "World Map Viewport", 1920f, 1080f);
        Image viewportGraphic = viewport.AddComponent<Image>();
        viewportGraphic.color = Color.clear;
        viewport.AddComponent<RectMask2D>();
        worldMapContentRect = CreateAnchoredLayer(viewport.transform, "World Map Content", WorldMapWidth, WorldMapHeight).GetComponent<RectTransform>();
        worldMapFirstRealmIndex = 0;
        Transform worldLayer = worldMapContentRect.transform;

        CreateMapImage(worldLayer, "worldPlatformBG", "Nav unity ready/home BG - platform/worldPlatformBG", WorldMapWidth, WorldMapHeight, 957.5f, 0f);
        CreateMapImage(worldLayer, "platform base 1", "Nav unity ready/home BG - platform/worlds10Platform_base1", WorldMapWidth, 281f, 957.5f, -399.5f);
        CreateMapImage(worldLayer, "platform base 2", "Nav unity ready/home BG - platform/worlds10Platform_base2", WorldMapWidth, 244f, 957.5f, -248f);
        CreateMapImage(worldLayer, "platform base 3", "Nav unity ready/home BG - platform/worlds10Platform_base3", WorldMapWidth, 183f, 957.5f, -194.5f);
        CreateMapImage(worldLayer, "platform base 4", "Nav unity ready/home BG - platform/worlds10Platform_base4", 3831f, 140f, 957.5f, -127f);

        for (int index = 0; index < WorldMapRealms.Length; index++)
        {
            CreateWorldMapRealm(worldLayer, WorldMapRealms[index], index);
        }

        if (showWorldMapReferenceGrid)
        {
            CreateWorldMapReferenceGrid(worldLayer);
        }

        CreateWorldMapHud(contentRoot);
        ApplyStageScale(true);

        if (worldMapRestoringRealmIndex >= 0)
        {
            if (worldMapRestoreTransitionRoutine != null)
            {
                StopCoroutine(worldMapRestoreTransitionRoutine);
            }

            worldMapRestoreTransitionRoutine = StartCoroutine(CompleteWorldMapRestoreTransition(worldMapRestoringRealmIndex));
        }
    }

    private void CreateWorldMapRealm(Transform parent, WorldMapRealmPlacement realm, int index)
    {
        bool isSunpetal = index == 0;
        bool isFloatingLotus = index == 1;
        bool isEverbloom = index == 2;
        bool isGlimmerwisp = index == 3;
        bool isCerulean = index == 4;
        bool isEmberglass = index == 5;
        bool isHowlingHollows = index == 6;
        bool isCrimsonMoon = index == 7;
        bool isArcaneAerie = index == 8;
        bool isChanterelle = index == 9;
        WorldMapRealmVisualState visualState = GetWorldMapRealmVisualState(index);
        bool usesRestoredFrame = visualState == WorldMapRealmVisualState.Restoring
            || visualState == WorldMapRealmVisualState.RestoredOpen
            || visualState == WorldMapRealmVisualState.RestoredClosed;
        string brokenBasePath = "rooms unity ready/" + realm.Folder + "/broken/" + realm.AssetPrefix;
        string restoredBasePath = "rooms unity ready/" + realm.Folder + "/restored/" + realm.AssetPrefix;
        string frameAssetPath = usesRestoredFrame ? restoredBasePath + "_frame_restored" : brokenBasePath + "_frame_broken";
        string doorAssetPath = visualState switch
        {
            WorldMapRealmVisualState.Open => brokenBasePath + "_door_broken_doorRealm",
            WorldMapRealmVisualState.Restoring => brokenBasePath + "_door_broken_portalSwirl",
            WorldMapRealmVisualState.RestoredOpen => restoredBasePath + "_door_restored_doorRealm",
            WorldMapRealmVisualState.RestoredClosed => restoredBasePath + "_door_restored_doorClosed",
            _ => brokenBasePath + "_door_broken_doorRealm_doorClosed"
        };
        float frameX = realm.X;
        float frameY = realm.Y;
        float frameWidth = realm.FrameWidth;
        float frameHeight = realm.FrameHeight;
        float doorX = realm.X;
        float doorY = realm.Y;
        float doorWidth = realm.DoorWidth;
        float doorHeight = realm.DoorHeight;

        if (isSunpetal)
        {
            // Photoshop bounds: frame [-930, -523, 274, -164], door [-836, -626, 233, -80].
            frameX = -726.5f;
            frameY = 55f;
            frameWidth = 407f;
            frameHeight = 438f;
            doorX = -731f;
            doorY = 76.5f;
            doorWidth = 210f;
            doorHeight = 313f;

            if (usesRestoredFrame)
            {
                // All restored Sunpetal frame states use bounds [-931, -530, 275, -121].
                frameX = -730.5f;
                frameY = 77f;
                frameWidth = 401f;
                frameHeight = 396f;

                if (visualState == WorldMapRealmVisualState.Restoring)
                {
                    // Portal swirl bounds: [-836, -623, 209, -99].
                    doorX = -729.5f;
                    doorY = 55f;
                    doorWidth = 213f;
                    doorHeight = 308f;
                }
                else if (visualState == WorldMapRealmVisualState.RestoredClosed)
                {
                    // Restored closed-door bounds: [-836, -623, 209, -99].
                    doorX = -729.5f;
                    doorY = 55f;
                    doorWidth = 213f;
                    doorHeight = 308f;
                }
                else
                {
                    // Restored open-realm bounds: [-836, -623, 209, -99].
                    doorX = -729.5f;
                    doorY = 55f;
                    doorWidth = 213f;
                    doorHeight = 308f;
                }
            }
        }

        if (isFloatingLotus)
        {
            // Photoshop bounds: frame [-551, -156, 275, -180], door [-458, -251, 211, -94].
            frameX = -353.5f;
            frameY = 47.5f;
            frameWidth = 395f;
            frameHeight = 455f;
            doorX = -354.5f;
            doorY = 58.5f;
            doorWidth = 207f;
            doorHeight = 305f;
        }

        if (isEverbloom)
        {
            if (!usesRestoredFrame)
            {
                // Broken frame bounds: [-178, 240, 312, -175]. Door vertical bounds: [188, -118].
                frameX = 31f;
                frameY = 68.5f;
                frameWidth = 418f;
                frameHeight = 487f;
                doorX = 46.5f;
                doorY = 35f;
                doorWidth = 191f;
                doorHeight = 306f;
            }
            else
            {
                // Restored frame keeps its established placement; its door shares the measured vertical bounds.
                frameX = 55f;
                frameY = 78f;
                frameWidth = 372f;
                frameHeight = 398f;
                doorX = 45.5f;
                doorY = 35f;
                doorWidth = 209f;
                doorHeight = 306f;
            }
        }

        if (isGlimmerwisp)
        {
            // Photoshop bounds: frame [243, 599, 269, -112], door [333, 501, 202, -84].
            frameX = 421f;
            frameY = 78.5f;
            frameWidth = 356f;
            frameHeight = 381f;
            doorX = 417f;
            doorY = 59f;
            doorWidth = 168f;
            doorHeight = 286f;
        }

        if (isCerulean)
        {
            // Photoshop bounds: frame [631, 979, 284, -105], door [704, 908, 207, -85].
            frameX = 805f;
            frameY = 89.5f;
            frameWidth = 348f;
            frameHeight = 389f;
            doorX = 806f;
            doorY = 61f;
            doorWidth = 204f;
            doorHeight = 292f;
        }

        if (isEmberglass)
        {
            // Photoshop page-two bounds: frame [-918, -593, 290, -117], door [-845, -669, 210, -87].
            frameX = WorldMapSecondPageOriginX - 755.5f;
            frameY = 86.5f;
            frameWidth = 325f;
            frameHeight = 407f;
            doorX = WorldMapSecondPageOriginX - 757f;
            doorY = 61.5f;
            doorWidth = 176f;
            doorHeight = 297f;
        }

        if (isHowlingHollows)
        {
            // Photoshop page-two bounds: frame [-579, -220, 269, -177], door [-456, -344, 112, -84].
            frameX = WorldMapSecondPageOriginX - 399.5f;
            frameY = 46f;
            frameWidth = 359f;
            frameHeight = 446f;
            doorX = WorldMapSecondPageOriginX - 400f;
            doorY = 14f;
            doorWidth = 112f;
            doorHeight = 196f;
        }

        if (isCrimsonMoon)
        {
            // Photoshop page-two bounds: frame [-197, 192, 279, -133], door [-100, 90, 194, -79].
            frameX = WorldMapSecondPageOriginX - 2.5f;
            frameY = 73f;
            frameWidth = 389f;
            frameHeight = 412f;
            doorX = WorldMapSecondPageOriginX - 5f;
            doorY = 57.5f;
            doorWidth = 190f;
            doorHeight = 273f;
        }

        if (isArcaneAerie)
        {
            // Photoshop page-two bounds: frame [203, 548, 279, -100], door [300, 450, 187, -64].
            frameX = WorldMapSecondPageOriginX + 375.5f;
            frameY = 89.5f;
            frameWidth = 345f;
            frameHeight = 379f;
            doorX = WorldMapSecondPageOriginX + 375f;
            doorY = 61.5f;
            doorWidth = 150f;
            doorHeight = 251f;
        }

        if (isChanterelle)
        {
            // Photoshop page-two bounds: frame [569, 900, 279, -123], door [629, 842, 226, -85].
            frameX = WorldMapSecondPageOriginX + 734.5f;
            frameY = 78f;
            frameWidth = 331f;
            frameHeight = 402f;
            doorX = WorldMapSecondPageOriginX + 735.5f;
            doorY = 70.5f;
            doorWidth = 213f;
            doorHeight = 311f;
        }

        Image door = CreateMapImage(parent, realm.Folder + " door", doorAssetPath, doorWidth, doorHeight, doorX, doorY);
        if (visualState == WorldMapRealmVisualState.Locked)
        {
            // Keep locked realms recognizable while clearly unavailable.
            door.color = new Color(0.42f, 0.42f, 0.42f, 1f);
        }

        if (visualState == WorldMapRealmVisualState.Restoring)
        {
            activeWorldMapPortalSwirl = door;
        }

        Image frame = CreateMapImage(parent, realm.Folder + " frame", frameAssetPath, frameWidth, frameHeight, frameX, frameY);
        if (visualState == WorldMapRealmVisualState.Locked)
        {
            frame.color = new Color(0.42f, 0.42f, 0.42f, 1f);
        }
        else
        {
            CreateWorldMapRealmButton(door, index);
            CreateWorldMapRealmButton(frame, index);
        }

        // Plates and bottle groups follow the measured frame centre, not the old default realm centre.
        float plateX = frameX;
        Image titlePlate = CreateMapImage(parent, realm.Folder + " title plate", "rooms unity ready/title_bottle/title plate short", 290f, 82f, plateX, -133f);
        CreateWorldMapRealmButton(titlePlate, index);
        Text realmTitle = CreateAnchoredText(parent, WorldMapRealmLabels[index], 25, FontStyle.Normal, new Color(1f, 0.84f, 0.35f), 252f, 62f, plateX, -133f);
        ApplyWorldMapDisplayTextTreatment(realmTitle);
        realmTitle.lineSpacing = 0.78f;
        CreateWorldMapRealmButton(realmTitle, index);

        float[] potionOffsets = { -108f, -46f, 19f, 78f };
        Color realmPotionColor = WorldMapRealmPotionColors[Mathf.Clamp(index, 0, WorldMapRealmPotionColors.Length - 1)];
        IReadOnlyList<RoomDefinition> realmRooms = RealmContentCatalog.AllRealms[index].Rooms;

        for (int potionIndex = 0; potionIndex < potionOffsets.Length; potionIndex++)
        {
            float potionX = plateX + potionOffsets[potionIndex];
            bool roomRestored = potionIndex < realmRooms.Count && inventory.IsRoomRestored(realmRooms[potionIndex]);
            if (roomRestored)
            {
                Image potionInner = CreateMapImage(parent, realm.Folder + " potion inner " + potionIndex, "rooms unity ready/title_bottle/potion_inner", 44f, 69f, potionX, -235f);
                potionInner.color = realmPotionColor;
            }

            CreateMapImage(parent, realm.Folder + " potion outer " + potionIndex, "rooms unity ready/title_bottle/potion_outer", 44f, 69f, potionX, -235f);
        }
    }

    private void CreateWorldMapReferenceGrid(Transform parent)
    {
        const float minorLineThickness = 2f;
        const float axisLineThickness = 4f;
        Color gridColor = new Color(0f, 0.95f, 1f, 0.8f);
        Color axisColor = new Color(1f, 0.08f, 0.08f, 0.95f);

        // World positions use the same centre-origin conversion as the Photoshop guide.
        for (float x = -900f; x <= 2900f; x += WorldMapGridStep)
        {
            CreateWorldMapGuideLine(parent, "Grid vertical " + x, minorLineThickness, WorldMapHeight, x, 0f, gridColor);
        }

        for (float y = -500f; y <= 500f; y += WorldMapGridStep)
        {
            CreateWorldMapGuideLine(parent, "Grid horizontal " + y, WorldMapWidth, minorLineThickness, 957.5f, y, gridColor);
        }

        CreateWorldMapGuideLine(parent, "Grid X axis", axisLineThickness, WorldMapHeight, 0f, 0f, axisColor);
        CreateWorldMapGuideLine(parent, "Grid Y axis", WorldMapWidth, axisLineThickness, 957.5f, 0f, axisColor);
    }

    private void CreateWorldMapGuideLine(Transform parent, string name, float width, float height, float x, float y, Color color)
    {
        GameObject line = CreateAnchoredPanel(parent, name, color, width, height, x, y);
        line.GetComponent<Image>().raycastTarget = false;
    }

    private void CreateWorldMapHud(Transform parent)
    {
        const float realmNavUpOffset = 10f;

        // Photoshop bounds: left -931, right -738, top 504, bottom 322.
        CreateMapImage(parent, "Avatar", "Nav unity ready/profile level/nav_worldHome_avatar_witch_001", 193f, 182f, -834.5f, 413f);
        // Photoshop bounds: left -931, right -737, top 516, bottom 323.
        CreateMapImage(parent, "Avatar Frame", "Nav unity ready/profile level/nav_worldHome_frame_basic_gold", 194f, 193f, -834f, 419.5f);
        // Photoshop bounds: left -770, right -681, top 435, bottom 343.
        Image levelBadge = CreateMapImage(parent, "Level Badge", "Nav unity ready/profile level/nav_profileLevelBG", 89f, 92f, -725.5f, 389f);
        Text levelNumber = CreateAnchoredText(parent, "26", 34, FontStyle.Normal, Color.white, 59f, 44f, -725.5f, 389f);
        ApplyWorldMapDisplayTextTreatment(levelNumber, WorldMapProfileFontResource, addShadow: false);
        // Photoshop bounds: left -740, right -320, top 412, bottom 360.
        CreateMapImage(parent, "XP bar left", "Nav unity ready/profile level/nav_profileLevelPill_left", 108f, 52f, -686f, 386f);
        CreateMapImage(parent, "XP bar center", "Nav unity ready/profile level/nav_profileLevelPill_center", 204f, 52f, -530f, 386f);
        CreateMapImage(parent, "XP bar right", "Nav unity ready/profile level/nav_profileLevelPill_right", 108f, 52f, -374f, 386f);
        CreateMapImage(parent, "XP bar fill", "Nav unity ready/profile level/nav_profileLevelPill_fill", 129f, 52f, -696.5f, 386f);
        levelBadge.transform.SetAsLastSibling();
        levelNumber.transform.SetAsLastSibling();
        Text experienceText = CreateAnchoredText(parent, "12,495 / 19,800", 24, FontStyle.Normal, Color.white, 204f, 44f, -530f, 386f);
        ApplyWorldMapDisplayTextTreatment(experienceText, addShadow: false);

        CreateTopCounter(parent, "Mana", "Nav unity ready/profile inventory/nav_profileMana_count");
        CreateTopCounter(parent, "Crystal", "Nav unity ready/profile inventory/nav_profileCrystal_count");
        CreateTopCounter(parent, "Power", "Nav unity ready/profile inventory/nav_profilePowerUp_count");
        Image home = CreateMapImage(parent, "Home", "Nav unity ready/top nav/nav_worldHome_default", 76f, 76f, 790f, 478f);
        Button homeButton = home.gameObject.AddComponent<Button>();
        homeButton.targetGraphic = home;
        homeButton.onClick.AddListener(BuildPlayerDenUi);

        Image settings = CreateMapImage(parent, "Settings", "Nav unity ready/top nav/nav_worldSettings_default", 76f, 76f, 900f, 478f);
        Button settingsButton = settings.gameObject.AddComponent<Button>();
        settingsButton.targetGraphic = settings;
        settingsButton.onClick.AddListener(() => BuildDevSettingsUi(true));

        CreateMapImage(parent, "Realm banner", "Nav unity ready/bottom Nav/realm banner", 804f, 56f, 1f, -324f);
        CreateMapImage(parent, "Realm nav", "Nav unity ready/bottom Nav/nav_worldHome_realmNavProgress", 1648f, 127f, -3f, -442.5f + realmNavUpOffset);
        // Photoshop bounds: left -715, right 715, top -433, bottom -442.
        CreateMapImage(parent, "Realm nav line", "Nav unity ready/bottom Nav/nav_worldHome_realmNavProgressRealms", 1430f, 9f, 0f, -427.5f + realmNavUpOffset);
        Image leftArrow = CreateMapImage(parent, "Left arrow", "Nav unity ready/bottom Nav/nav_worldHome_realmNavProgress_arrowleft", 42f, 67f, -771f, -442.5f);
        Image rightArrow = CreateMapImage(parent, "Right arrow", "Nav unity ready/bottom Nav/nav_worldHome_realmNavProgress_arrowright", 42f, 67f, 767f, -442.5f);
        CreateWorldMapArrowButton(leftArrow, -1);
        CreateWorldMapArrowButton(rightArrow, 1);

        int activeRealmIndex = Mathf.Clamp(RealmContentCatalog.ActivePrototypeRealmIndex, 0, WorldMapRealms.Length - 1);
        float activeMarkerX = Mathf.Lerp(-645.5f, 627.5f, activeRealmIndex / (float)(WorldMapRealms.Length - 1));
        CreateWorldMapActiveMarkerGlow(parent, activeMarkerX, -428.5f + realmNavUpOffset);

        for (int index = 0; index < WorldMapRealms.Length; index++)
        {
            // First marker bounds [-659, -632, -425, -452]; last [614, 641, -425, -452].
            float markerX = Mathf.Lerp(-645.5f, 627.5f, index / (float)(WorldMapRealms.Length - 1));
            bool unlocked = IsRealmUnlocked(index);
            string markerAssetPath = unlocked
                ? "Nav unity ready/bottom Nav/progressNavMarkerScaled"
                : "Nav unity ready/bottom Nav/lock-realm";
            float markerSize = unlocked ? 27f : 48f;
            Image marker = CreateMapImage(parent, "Realm marker " + (index + 1), markerAssetPath, markerSize, markerSize, markerX, -428.5f + realmNavUpOffset);
            if (unlocked)
            {
                CreateWorldMapRealmButton(marker, index);
            }

            Color labelColor = unlocked ? new Color(1f, 0.84f, 0.22f) : new Color(0.62f, 0.53f, 0.28f);
            Text label = CreateAnchoredText(parent, WorldMapRealmLabels[index], 15, FontStyle.Bold, labelColor, 132f, 36f, markerX, -470f);
            label.lineSpacing = 0.8f;
            if (unlocked)
            {
                CreateWorldMapRealmButton(label, index);
            }
        }

        // Photoshop bounds: left -728, right -364, top 480, bottom 432.
        // This is intentionally a fixed prototype identity until profile-name entry is implemented.
        Text profileName = CreateAnchoredText(parent, "Mayfair Mabel", 48, FontStyle.Normal, Color.white, 364f, 48f, -546f, 456f);
        ApplyWorldMapDisplayTextTreatment(profileName, WorldMapProfileFontResource, addShadow: false);
        WorldMapTextGradient profileNameGradient = profileName.gameObject.AddComponent<WorldMapTextGradient>();
        profileNameGradient.SetColors(new Color(1f, 0.96f, 0.7f), new Color(0.84f, 0.5f, 0.12f));
    }

    private void ApplyWorldMapDisplayTextTreatment(Text text, string fontResource = WorldMapDisplayFontResource, Vector2? shadowDistance = null, bool addShadow = true)
    {
        Font displayFont = Resources.Load<Font>(fontResource);
        if (displayFont != null)
        {
            text.font = displayFont;
            text.fontStyle = FontStyle.Normal;
        }

        text.verticalOverflow = VerticalWrapMode.Overflow;
        if (addShadow)
        {
            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.08f, 0.025f, 0.02f, 0.9f);
            shadow.effectDistance = shadowDistance ?? new Vector2(2f, -2f);
        }
    }

    private void CreateWorldMapArrowButton(Image arrowImage, int direction)
    {
        Button button = arrowImage.gameObject.AddComponent<Button>();
        button.targetGraphic = arrowImage;
        button.onClick.AddListener(() => SlideWorldMap(direction));
    }

    private void CreateWorldMapRealmButton(Graphic graphic, int realmIndex)
    {
        graphic.raycastTarget = true;
        Button button = graphic.gameObject.AddComponent<Button>();
        button.targetGraphic = graphic;
        button.onClick.AddListener(() => OpenWorldMapRealm(realmIndex));
    }

    private void OpenWorldMapRealm(int realmIndex)
    {
        if (!IsRealmUnlocked(realmIndex))
        {
            return;
        }

        BeginLoadingTransition(() =>
        {
            MarkWorldMapRealmOpened(realmIndex);
            // Enter the realm home first; a selected room then opens its room-entry lobby.
            RealmContentCatalog.SetActivePrototypeRealm(realmIndex);
            inventory.RefreshActiveRoomState();
            ApplySavedManaBetForActiveRoom();
            BuildRealmMapUi();
        });
    }

    private void BeginLoadingTransition(System.Action destination)
    {
        if (loadingTransitionRoutine != null)
        {
            return;
        }

        loadingTransitionRoutine = StartCoroutine(PlayLoadingTransition(destination));
    }

    private IEnumerator PlayLoadingTransition(System.Action destination)
    {
        GameObject loadingOverlay = CreateLoadingOverlay(out CanvasGroup canvasGroup, out LoadingFillView fill);
        yield return null;

        float elapsed = 0f;
        while (elapsed < 0.55f)
        {
            elapsed += Time.unscaledDeltaTime;
            fill.SetProgress(Mathf.Lerp(0f, 0.82f, elapsed / 0.55f));
            yield return null;
        }

        destination?.Invoke();
        yield return null;

        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.unscaledDeltaTime;
            fill.SetProgress(Mathf.Lerp(0.82f, 1f, elapsed / 0.2f));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.18f)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - (elapsed / 0.18f);
            yield return null;
        }

        Destroy(loadingOverlay);
        loadingTransitionRoutine = null;
    }

    private GameObject CreateLoadingOverlay(out CanvasGroup canvasGroup, out LoadingFillView fill)
    {
        GameObject overlay = new GameObject(
            "Loading Transition",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(CanvasGroup));
        Canvas canvas = overlay.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = overlay.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasGroup = overlay.GetComponent<CanvasGroup>();

        Image background = CreateAnchoredImage(overlay.transform, LoadingScreenBackgroundResource, 1920f, 1080f, 0f, 0f);
        background.name = "Loading Background";
        background.preserveAspect = false;
        background.raycastTarget = true;

        Image glow = CreateLoadingRoundedImage(overlay.transform, "Loading Bar Glow", 924f, 100f, 0f, -285f, 50f, new Color(1f, 0f, 0.94f, 0.22f));
        glow.raycastTarget = false;
        Image frame = CreateLoadingRoundedImage(overlay.transform, "Loading Bar Frame", 900f, 76f, 0f, -285f, 38f, new Color(1f, 0.82f, 1f));
        frame.raycastTarget = false;
        Image inset = CreateLoadingRoundedImage(frame.transform, "Loading Bar Inset", 876f, 52f, 0f, 0f, 26f, new Color(0.12f, 0.015f, 0.2f));
        inset.raycastTarget = false;

        Image fillImage = CreateLoadingRoundedImage(frame.transform, "Loading Bar Fill", 42f, 42f, -399f, 0f, 21f, new Color(1f, 0.1f, 0.92f));
        fillImage.raycastTarget = false;
        fill = new LoadingFillView(fillImage, width => GetLoadingRoundedRectSprite(width, 42, 21));
        fill.SetProgress(0f);
        return overlay;
    }

    private Image CreateLoadingRoundedImage(Transform parent, string name, float width, float height, float x, float y, float radius, Color color)
    {
        GameObject imageObject = CreateAnchoredPanel(parent, name, color, width, height, x, y);
        Image image = imageObject.GetComponent<Image>();
        image.sprite = GetLoadingRoundedRectSprite(Mathf.RoundToInt(width), Mathf.RoundToInt(height), Mathf.RoundToInt(radius));
        image.type = Image.Type.Simple;
        return image;
    }

    private Sprite GetLoadingRoundedRectSprite(int width, int height, int radius)
    {
        string key = width + "x" + height + "x" + radius;
        if (loadingRoundedRectSprites.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.name = "LoadingRoundedRect_" + key;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[width * height];
        float minX = radius;
        float maxX = width - 1f - radius;
        float minY = radius;
        float maxY = height - 1f - radius;
        if (maxX < minX)
        {
            minX = maxX = (width - 1f) * 0.5f;
        }

        if (maxY < minY)
        {
            minY = maxY = (height - 1f) * 0.5f;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nearestX = Mathf.Clamp(x, minX, maxX);
                float nearestY = Mathf.Clamp(y, minY, maxY);
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(nearestX, nearestY));
                float alpha = radius <= 0 ? 1f : Mathf.Clamp01(radius + 0.5f - distance);
                pixels[(y * width) + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        sprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 1f);
        loadingRoundedRectSprites.Add(key, sprite);
        return sprite;
    }

    private void SlideWorldMap(int direction)
    {
        if (worldMapContentRect == null)
        {
            return;
        }

        int maximumFirstIndex = WorldMapRealms.Length - 5;
        int nextFirstIndex = Mathf.Clamp(worldMapFirstRealmIndex + direction, 0, maximumFirstIndex);
        if (nextFirstIndex == worldMapFirstRealmIndex)
        {
            return;
        }

        if (worldMapSlideRoutine != null)
        {
            StopCoroutine(worldMapSlideRoutine);
        }

        worldMapSlideRoutine = StartCoroutine(SlideWorldMapRoutine(nextFirstIndex));
    }

    private IEnumerator SlideWorldMapRoutine(int nextFirstIndex)
    {
        Vector2 start = worldMapContentRect.anchoredPosition;
        float worldOffset = WorldMapRealms[nextFirstIndex].X - WorldMapRealms[0].X;
        Vector2 target = new Vector2(-worldOffset, start.y);
        const float duration = 0.28f;

        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            worldMapContentRect.anchoredPosition = Vector2.LerpUnclamped(start, target, progress);
            yield return null;
        }

        worldMapContentRect.anchoredPosition = target;
        worldMapFirstRealmIndex = nextFirstIndex;
        worldMapSlideRoutine = null;
    }

    private void CreateTopCounter(Transform parent, string name, string iconPath, float pillRightExtension = 0f)
    {
        float pillLeftX;
        float pillCenterX;
        float pillRightX;
        float pillRightWidth;
        float iconX;
        float iconY;
        float iconWidth;
        float iconHeight;
        float backgroundX;
        float addX;
        string countText;

        switch (name)
        {
            case "Mana":
                // Measured bounds: icon [20, 63, 497, 450], background [7, 76, 508, 439].
                pillLeftX = 22.5f;
                pillCenterX = 97f;
                pillRightX = 168f;
                pillRightWidth = 40f;
                iconX = 31.5f;
                iconY = 473.5f;
                iconWidth = 43f;
                iconHeight = 47f;
                backgroundX = 31.5f;
                addX = 166f;
                countText = inventory.GetManaText();
                break;
            case "Crystal":
                // Measured bounds: icon [270, 297, 504, 446], background [249, 318, 508, 439].
                pillLeftX = 264.5f;
                pillCenterX = 339f;
                pillRightX = 410f;
                pillRightWidth = 40f;
                iconX = 273.5f;
                iconY = 475f;
                iconWidth = 27f;
                iconHeight = 58f;
                backgroundX = 273.5f;
                addX = 408f;
                countText = inventory.GetCrystalText();
                break;
            case "Power":
                // Measured bounds: icon [481, 525, 497, 450], background [469, 538, 508, 439].
                pillLeftX = 484.5f;
                pillCenterX = 559f;
                pillRightX = 630f;
                pillRightWidth = 40f;
                iconX = 493f;
                iconY = 473.5f;
                iconWidth = 44f;
                iconHeight = 47f;
                backgroundX = 493.5f;
                addX = 628f;
                countText = inventory.GetPowerUpText();
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(name), name, "Unknown world-map profile counter.");
        }

        const float pillY = 474.5f;
        float pillCenterWidth = 102f;
        const float pillHeight = 59f;
        float countX = pillCenterX + 10f;
        if (pillRightExtension > 0f)
        {
            // Lengthen the pill past the add button; the center piece overlaps both ends slightly to hide seams.
            pillCenterX += pillRightExtension / 2f;
            pillCenterWidth += pillRightExtension + 4f;
            pillRightX += pillRightExtension;
        }
        CreateMapImage(parent, name + " pill left", "Nav unity ready/profile inventory/nav_profileInventoryPill_countPillLeft", 47f, pillHeight, pillLeftX, pillY);
        CreateMapImage(parent, name + " pill center", "Nav unity ready/profile inventory/nav_profileInventoryPill_countPillCenter", pillCenterWidth, pillHeight, pillCenterX, pillY);
        CreateMapImage(parent, name + " pill right", "Nav unity ready/profile inventory/nav_profileInventoryPill_countPillRight", pillRightWidth, pillHeight, pillRightX, pillY);
        CreateMapImage(parent, name + " icon background", "Nav unity ready/profile inventory/nav_profileInventory_countBG", 69f, 69f, backgroundX, 473.5f);
        CreateMapImage(parent, name + " icon", iconPath, iconWidth, iconHeight, iconX, iconY);
        int countFontSize = countText.Length >= 6 ? 24 : 28;
        Text count = CreateAnchoredText(parent, countText, countFontSize, FontStyle.Normal, Color.white, 86f, 46f, countX, pillY);
        ApplyWorldMapDisplayTextTreatment(count, addShadow: false);
        // Long balances must not wrap a digit onto a hidden line.
        count.horizontalOverflow = HorizontalWrapMode.Overflow;
        CreateMapImage(parent, name + " add", "Nav unity ready/profile inventory/nav_profileInventory_countPillAdd", 36f, 36f, addX, 475f);
    }

    private Image CreateMapImage(Transform parent, string name, string relativePath, float width, float height, float x, float y)
    {
        Image image = CreateAnchoredImage(parent, WorldMapResourceRoot + relativePath, width, height, x, y);
        image.preserveAspect = false;
        return image;
    }

    private void CreateWorldMapActiveMarkerGlow(Transform parent, float x, float y)
    {
        GameObject glowObject = CreateAnchoredPanel(parent, "Active realm marker glow", Color.white, 72f, 72f, x, y);
        Image glow = glowObject.GetComponent<Image>();
        glow.sprite = GetWorldMapActiveMarkerGlowSprite();
        glow.preserveAspect = true;
        glow.raycastTarget = false;
    }

    private Sprite GetWorldMapActiveMarkerGlowSprite()
    {
        if (worldMapActiveMarkerGlowSprite != null)
        {
            return worldMapActiveMarkerGlowSprite;
        }

        const int textureSize = 128;
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        texture.name = "WorldMapActiveMarkerGlow";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        Color[] pixels = new Color[textureSize * textureSize];
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float normalizedX = ((x + 0.5f) / textureSize * 2f) - 1f;
                float normalizedY = ((y + 0.5f) / textureSize * 2f) - 1f;
                float radius = Mathf.Sqrt((normalizedX * normalizedX) + (normalizedY * normalizedY));
                // Keep this close to the marker so it reads as a soft shadow, not a second icon.
                float alpha = Mathf.Exp(-4.5f * radius * radius) * 0.3f;
                pixels[(y * textureSize) + x] = new Color(0.8f, 0.16f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        worldMapActiveMarkerGlowSprite = Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0.5f));
        return worldMapActiveMarkerGlowSprite;
    }
}

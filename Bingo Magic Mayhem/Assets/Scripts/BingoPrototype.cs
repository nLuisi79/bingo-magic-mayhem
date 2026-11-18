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

[RequireComponent(typeof(RectTransform))]
public partial class BingoPrototype : MonoBehaviour
{
    private const int BoardSize = 5;
    private const float CellSize = 76f;
    private const float CellGap = 8f;
    private const float BoardWidth = BoardSize * CellSize + (BoardSize - 1) * CellGap;
    private const float StageWidth = 1600f;
    private const float StageHeight = 900f;
    private const float LobbyStageWidth = StageWidth;
    private const float LobbyStageHeight = StageHeight;
    private const float SafePadding = 32f;
    // Mirrors the supplied long mock: cyan grid every 100 world units, red axes at 0,0.
    // Keep this enabled while transferring Photoshop measurements; disable for the final presentation.
    private bool showWorldMapReferenceGrid = false;
    private const float WorldMapGridStep = 100f;
    private const float WorldMapWidth = 3835f;
    private const float WorldMapHeight = 1080f;
    private const float WorldMapSecondPageOriginX = 1917.5f;
    private static float WildSigilBankOfferChance => EconomyTuning.Current.PowerUps.WildSigilOfferChance;
    private const string PrototypeSocialFreebieCode = "social-spark";
    private const string PrototypeInstagramUrl = "https://www.instagram.com/bingomagicmayhem";
    private const string PrototypeFacebookUrl = "https://www.facebook.com/bingomagicmayhem";
    private const string PrototypeXUrl = "https://x.com/bingomagicmayhem";
    private static int SocialFreebieExpirationDays => EconomyTuning.Current.Social.FreebieExpirationDays;
    private static int PrototypeFriendManaAmount => EconomyTuning.Current.Social.FriendManaAmount;
    private static int PrototypeFriendDailyManaLimit => EconomyTuning.Current.Social.FriendDailyManaLimit;
    private const char PrototypeFriendSaveSeparator = '|';
    private const string PrototypeCovenDiscoverySaveKey = "BMM.Prototype.CovenDiscovery.Requests";
    private const string PrototypeLocalHostPlayerId = "prototype_local_host";
    private static readonly string[] LockedRankLadder =
    {
        "Aura TBD: Novice",
        "Aura TBD: Apprentice",
        "Aura TBD: Spellbinder",
        "Aura TBD: Mage",
        "Aura TBD: Thaumaturge",
        "Aura TBD: Mystic",
        "Aura TBD: Enchanter",
        "Aura TBD: Wizard",
        "Aura TBD: Spellmaster",
        "Aura TBD: Archmage",
        "Aura TBD: Grand Archmage",
        "Aura TBD: Paragon",
        "Aura TBD: Ascendant",
        "Aura TBD: Sorcerer Supreme"
    };

    private const string RealmBannerTextResource = "prototype/ui/nav/realm_banner_text_v001";
    private const string NavArrowLeftResource = "prototype/ui/nav/nav_arrow_left_v001";
    private const string NavArrowRightResource = "prototype/ui/nav/nav_arrow_right_v001";
    private const string ProfileAvatarResource = "cosmetics/avatars/avatar_moon_witch_v001";
    private const string ProfileFrameResource = "cosmetics/frames/frame_plain_gold_v001";
    private const string WorldMapResourceRoot = "prototype/ui/world_map/unity_files/";
    private const string WorldMapDisplayFontResource = "prototype/fonts/EBGaramond-SemiBold";
    private const string WorldMapProfileFontResource = "prototype/fonts/EBGaramond-ExtraBold";
    private const string RealmHomeBackgroundObjectPrefix = "Realm home background ";
    private const string LoadingScreenBackgroundResource = "prototype/ui/loading/loading";
    private const string WorldMapOpenedSavePrefix = "BingoMagicMayhem.WorldMap.Opened.";
    private const string WorldMapRestoredClosedSavePrefix = "BingoMagicMayhem.WorldMap.RestoredClosed.";

    private static readonly WorldMapRealmPlacement[] WorldMapRealms =
    {
        new WorldMapRealmPlacement("01_sunpetal", "worldMap_sunpetal", -729.5f, 78.5f, 401f, 393f, 208.8f, 307.69f),
        new WorldMapRealmPlacement("02_lotus", "worldMap_floatingLotus", -354f, 38f, 429f, 567.9f, 225f, 357.71f),
        new WorldMapRealmPlacement("03_everbloom", "worldMap_everbloom", 20.5f, 81.5f, 395.49f, 397.59f, 217.88f, 301.93f),
        new WorldMapRealmPlacement("04_glimmerwisp", "worldMap_glimmerwisp", 389.5f, 78f, 386.12f, 384f, 386.12f, 384f),
        new WorldMapRealmPlacement("05_cerulean", "worldMap_cerulean", 723f, 89f, 341.5f, 391.85f, 341.5f, 391.85f),
        new WorldMapRealmPlacement("06_emberglass", "worldMap_emberglass", 1183.5f, 85f, 331.85f, 424.95f, 179.27f, 309.59f),
        new WorldMapRealmPlacement("07_hollows", "worldMap_hollows", 1520.5f, 52.5f, 382.07f, 508.9f, 119.2f, 219.25f),
        new WorldMapRealmPlacement("08_crimson", "worldMap_crimson", 1902f, 75.5f, 378.77f, 472.38f, 178.4f, 327.67f),
        new WorldMapRealmPlacement("09_aerie", "worldMap_aerie", 2238f, 87.5f, 378.51f, 388.98f, 171.72f, 262.31f),
        new WorldMapRealmPlacement("10_chanterelle", "worldMap_chanterelle", 2627f, 79f, 318.88f, 430.37f, 205.96f, 333.32f)
    };

    private static readonly string[] WorldMapRealmLabels =
    {
        "Sunpetal\nConservatory",
        "Floating Lotus\nLagoon",
        "Everbloom\nSanctuary",
        "Glimmerwisp\nForest",
        "Cerulean Moon\nMonastery",
        "Emberglass\nGrove",
        "Howling Hollows\nManor",
        "Crimson Moon\nCanyon",
        "Arcane Aerie\nHeights",
        "Crystalline\nChanterelle"
    };

    // Filled bottles use the restored door's signature gem color for their realm.
    private static readonly Color[] WorldMapRealmPotionColors =
    {
        new Color(0.16f, 0.72f, 1f),  // Sunpetal: sky blue
        new Color(0.53f, 0.28f, 0.92f), // Floating Lotus: violet
        new Color(0.96f, 0.33f, 0.63f), // Everbloom: rose pink
        new Color(0.94f, 0.12f, 0.58f), // Glimmerwisp: fuchsia
        new Color(0.18f, 0.62f, 1f),  // Cerulean: sapphire blue
        new Color(0.56f, 0.28f, 0.48f), // Emberglass: plum
        new Color(1f, 0.56f, 0.12f),  // Howling Hollows: amber
        new Color(0.88f, 0.4f, 0.66f), // Crimson Moon: orchid
        new Color(0.62f, 0.35f, 0.9f), // Arcane Aerie: amethyst
        new Color(0.84f, 0.94f, 1f)   // Crystalline Chanterelle: icy crystal
    };


    private readonly int[,] numbers = new int[BoardSize, BoardSize];
    private readonly bool[,] marked = new bool[BoardSize, BoardSize];
    private readonly Button[,] cells = new Button[BoardSize, BoardSize];
    private readonly PlayerCardSet playerCards = new PlayerCardSet();
    private readonly Dictionary<string, Button> visibleCardCells = new Dictionary<string, Button>();
    private readonly Dictionary<int, Text> visibleCardBingoTexts = new Dictionary<int, Text>();
    private readonly List<Text> callQueueTexts = new List<Text>();
    private readonly List<RectTransform> jackpotWheelSegmentRects = new List<RectTransform>();
    private readonly Dictionary<string, string> powerUpMarkedCellLabels = new Dictionary<string, string>();
    private readonly Dictionary<string, string> placedPowerUpSigils = new Dictionary<string, string>();
    private readonly BingoRoomRules roomRules = new BingoRoomRules();
    private BingoRoundFlowController roundFlow;
    private readonly PowerUpRuntimeState powerUps = new PowerUpRuntimeState();
    private readonly CellRewardTracker cellRewards = new CellRewardTracker();
    private readonly PlayerInventoryState inventory = new PlayerInventoryState();
    private readonly CovenState coven = new CovenState();
    private BingoRewardTracker rewards;
    private readonly BingoRoundEndCoordinator roundEndCoordinator = new BingoRoundEndCoordinator();
    private readonly IPrototypeMultiplayerRuntimeProvider prototypeMultiplayerRuntimeProvider = new PrototypeMultiplayerRuntimeProvider();
    private PrototypeMultiplayerRuntime prototypeMultiplayerRuntime;
    private PrototypeMultiplayerPresentationState prototypeMultiplayerPresentationState;
    private RewardPreview pendingRewardPreview;
    private BingoRoundEndDecision activePrototypeRoundEndDecision;
    private bool prototypeRoundEndPublished;

    private Font uiFont;
    private Transform contentRoot;
    private RectTransform stageRect;
    // Dimmed, cover-scaled copy of a full-bleed screen's background that fills the bands left on non-16:9 screens.
    private RectTransform stageAmbientBackdrop;
    private Rect lastSafeArea;
    private float activeStageSafePadding = SafePadding;
    private RectTransform worldMapContentRect;
    private int worldMapPreviewRealmIndex = -1;
    private WorldMapRealmVisualState worldMapPreviewRealmState = WorldMapRealmVisualState.RestoredOpen;
    private int worldMapRestoringRealmIndex = -1;
    private Image activeWorldMapPortalSwirl;
    private Sprite worldMapActiveMarkerGlowSprite;
    private Coroutine worldMapRestoreTransitionRoutine;
    private Coroutine worldMapSlideRoutine;
    private int worldMapFirstRealmIndex;
    private Vector2 lastRootSize;
    private float activeStageWidth = StageWidth;
    private float activeStageHeight = StageHeight;
    private bool activeStageFillsViewport;
    private Text roomText;
    private Text calledNumberText;
    private Text calledHistoryText;
    private Text statusText;
    private Text timerText;
    private Text ballsLeftText;
    private Text roomBingoCountText;
    private Text bingoBannerText;
    private Text roundSummaryText;
    private Text xpText;
    private Text gameplayManaText;
    private Text gameplayCrystalText;
    private Text lobbyPowerUpText;
    private Text lobbyManaText;
    private Text lobbyCrystalText;
    private Text lobbyMultiplayerTitleText;
    private Text lobbyMultiplayerReadinessText;
    private Text lobbyMultiplayerDetailText;
    private Text lobbyMultiplayerParticipantsText;
    private Text gameplayMultiplayerRoomText;
    private Text gameplayMultiplayerAuthorityText;
    private Text gameplayMultiplayerClaimText;
    private Text gameplayMultiplayerPostRoundText;
    private Text powerUpInventoryText;
    private Text lobbyClairvoyanceStatusText;
    private Text denManaCauldronText;
    private Text cauldronAmountText;
    private Text cauldronStatusText;
    private Button cauldronCollectButton;
    private RoomEntryPanelView roomEntryPanelView;
    private PlayerProfileModalView playerProfileModalView;
    private PrototypeInboxCategory inboxActiveCategory = PrototypeInboxCategory.Gifts;
    private string lastInboxClaimSummary = "";
    private string lastLibraryCardGiftSummary = "";
    private string playerProfileActiveTab = PlayerProfileTabIds.Profile;
    private string prototypeLeaderboardTab = "Friends";
    private bool prototypeSoundEnabled = true;
    private bool prototypeNotificationsEnabled = true;
    private string lastInfrastructureDiagnosticsExport = "";
    private readonly HashSet<int> prototypeOracleSelectedCards = new HashSet<int>();
    private readonly FriendsState friendsState = new FriendsState();
    private readonly HashSet<string> prototypeRequestedCovenNames = new HashSet<string>();
    private int prototypeCardGiftsSentToday;
    private int prototypeCardGiftsReceivedToday;
    private string lastFriendStatusSummary = "";
    private InputField prototypeFriendMessageInput;
    private Text prototypeFriendMessageStatusText;
    private InputField profileDisplayNameInput;
    private Text profileDisplayNameStatusText;
    private string lastProfileDisplayNameStatus = "";
    private string lastOracleReadingSummary = "";
    private int selectedPrototypeAvatarIndex;
    private int selectedPrototypeFrameIndex;
    private int selectedPrototypeDauberIndex;
    private ProfileSettingsPersistence profileSettingsPersistence;
    private ProfileSettingsState profileSettingsState = ProfileSettingsState.CreateDefault();
    private string lastDailySpinSummary = "";
    private int lastDailySpinRewardIndex = -1;
    private bool dailySpinAnimating;
    private int dailySpinAnimationSequence;
    private float dailySpinWheelRotation;
    private bool devSettingsReturnToDen;
    private Text gameplayClairvoyanceText;
    private Text gameplayPowerUpReadyText;
    private Text gameplayPowerUpEventText;
    private Text gameplayPowerUpBurstText;
    private Button gameplayPowerUpReadyButton;
    private Text gameplayPowerUpStockText;
    private Button gameplayPowerUpAutoButton;
    private string bankPowerUpName = "";
    private readonly List<string> bankPowerUpBag = new List<string>();
    private int bankPowerUpCharge;
    private bool fortuneDoublePrizeActive;
    private bool wildSigilModeActive;
    private int placedSigilBonusBingos;
    private int powerUpAssistedMarks;
    private int selectedCardCount = 4;
    private int currentCardIndex;
    private int manaBetPerCard = 25;
    private int selectedGrimoireEntryIndex;
    private int selectedGrimoireIndexPage;
    private int selectedBookOfShadowsEntryIndex;
    private string jackpotSpinResultText = "";
    private readonly JackpotWheelFlowController jackpotWheelFlow = new JackpotWheelFlowController();
    private bool rewardPreviewShown;
    private bool allCalledVisible;
    private int lastJackpotEarnedCardIndex = -1;
    private bool cardRevealShouldShowJackpotSpins;
    private float powerUpSaveAccumulator;
    private Coroutine powerUpBurstRoutine;
    private Coroutine finalBallCountdownRoutine;
    private Coroutine loadingTransitionRoutine;
    private readonly Dictionary<string, Sprite> loadingRoundedRectSprites = new Dictionary<string, Sprite>();

    private sealed class LoadingFillView
    {
        private const float FillHeight = 42f;
        private const float FillWidth = 840f;
        private const float FillLeft = -420f;

        private readonly Image fill;
        private readonly System.Func<int, Sprite> spriteForWidth;

        public LoadingFillView(Image fill, System.Func<int, Sprite> spriteForWidth)
        {
            this.fill = fill;
            this.spriteForWidth = spriteForWidth;
        }

        public void SetProgress(float progress)
        {
            float width = FillWidth * Mathf.Clamp01(progress);
            bool visible = width > 0.01f;
            fill.enabled = visible;
            if (!visible)
            {
                return;
            }

            width = Mathf.Max(FillHeight, width);
            int pixelWidth = Mathf.CeilToInt(width);
            fill.sprite = spriteForWidth(pixelWidth);
            fill.rectTransform.sizeDelta = new Vector2(pixelWidth, FillHeight);
            fill.rectTransform.anchoredPosition = new Vector2(FillLeft + (pixelWidth * 0.5f), 0f);
        }
    }

    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeAvatars = CosmeticCatalog.Avatars;
    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeAvatarFrames = CosmeticCatalog.Frames;
    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeDaubers = CosmeticCatalog.Daubers;
    private static readonly ICosmeticSpriteResolver CosmeticSpriteResolver = new ResourcesCosmeticSpriteResolver();

    private async void Start()
    {
        EnsureRuntimeState();
        await InitializeInfrastructureAsync();
        inventory.Load();
        inventory.ReconcileSequentialRoomRestores();
        coven.Load();
        friendsState.Load();
        LoadPrototypeCovenDiscoveryState();
        LoadPrototypeTrailState();
        Application.deepLinkActivated += HandlePrototypeDeepLink;
        if (!string.IsNullOrWhiteSpace(Application.absoluteURL))
        {
            HandlePrototypeDeepLink(Application.absoluteURL);
        }

        ApplySavedManaBetForActiveRoom();
        rewards.Load();
        EnsureUiHost();
        BuildWorldMapUi();
    }

    private async Task InitializeInfrastructureAsync()
    {
        try
        {
            GameInfrastructureServices services = await GameInfrastructureRuntime.InitializeAsync();
            profileSettingsPersistence = new ProfileSettingsPersistence(
                services.DurableState,
                services.ActionJournal,
                services.Identity);
            ApplyProfileSettings(profileSettingsPersistence.Load());
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"Local infrastructure initialization failed; profile settings will use session defaults. {exception}");
            ApplyProfileSettings(ProfileSettingsState.CreateDefault());
        }
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= HandlePrototypeDeepLink;
    }

    private void LateUpdate()
    {
        EnsureRuntimeState();
        ApplyStageScale();
        TickInventoryPowerUps();
        TickAutoCaller();
    }

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

    private void EnsureRuntimeState()
    {
        if (roundFlow == null)
        {
            roundFlow = new BingoRoundFlowController(roomRules);
        }

        if (rewards == null)
        {
            rewards = new BingoRewardTracker(roomRules);
        }

        if (prototypeMultiplayerRuntime == null)
        {
            prototypeMultiplayerRuntime = prototypeMultiplayerRuntimeProvider.CreateRuntime(
                PrototypeMultiplayerBackendMode.Local,
                PrototypeLocalHostPlayerId);
        }
    }

    private void EnsureUiHost()
    {
        StretchToParent();

        if (GetComponentInParent<Canvas>() == null)
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            ConfigureCanvasScaler(scaler);
        }

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            CanvasScaler scaler = parentCanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = parentCanvas.gameObject.AddComponent<CanvasScaler>();
            }

            ConfigureCanvasScaler(scaler);
        }

        if (parentCanvas != null && parentCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            parentCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        if (GetComponent<Image>() == null)
        {
            gameObject.AddComponent<Image>();
        }
    }

    private void StretchToParent()
    {
        RectTransform rect = EnsureRootRectTransform();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private RectTransform EnsureRootRectTransform()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = gameObject.AddComponent<RectTransform>();
        }

        return rect;
    }

    private void ConfigureCanvasScaler(CanvasScaler scaler)
    {
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void BuildCallPool()
    {
        roundFlow.ResetSession();
        powerUps.ResetRound();
        rewards.ResetRound();
    }

    private void GenerateCard()
    {
        GenerateCards(selectedCardCount);
    }

    private void GenerateCards(int count)
    {
        playerCards.Generate(count);
        currentCardIndex = 0;
        LoadCurrentCard();
    }

    private void FillCard(int[,] targetNumbers, bool[,] targetMarks)
    {
        int[] minByColumn = { 1, 16, 31, 46, 61 };

        for (int column = 0; column < BoardSize; column++)
        {
            List<int> available = new List<int>();
            for (int value = minByColumn[column]; value < minByColumn[column] + 15; value++)
            {
                available.Add(value);
            }

            for (int row = 0; row < BoardSize; row++)
            {
                int index = Random.Range(0, available.Count);
                targetNumbers[row, column] = available[index];
                available.RemoveAt(index);
                targetMarks[row, column] = false;
            }
        }

        targetMarks[2, 2] = true;
    }

    private void LoadCurrentCard()
    {
        if (!playerCards.HasAnyCards)
        {
            FillCard(numbers, marked);
            return;
        }

        int safeIndex = Mathf.Clamp(currentCardIndex, 0, playerCards.Count - 1);
        int[,] sourceNumbers = playerCards.Numbers[safeIndex];
        bool[,] sourceMarks = playerCards.Marks[safeIndex];

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                numbers[row, column] = sourceNumbers[row, column];
                marked[row, column] = sourceMarks[row, column];
            }
        }
    }

    private void SaveCurrentCard()
    {
        if (!playerCards.HasAnyCards)
        {
            return;
        }

        int safeIndex = Mathf.Clamp(currentCardIndex, 0, playerCards.Count - 1);
        bool[,] targetMarks = playerCards.Marks[safeIndex];

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                targetMarks[row, column] = marked[row, column];
            }
        }
    }

    private void BuildUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.08f, 0.06f, 0.13f, 0.98f), 8, 5);

        BuildFixedGameplayLayout();

        ApplyStageScale(true);
    }

    private void BuildFixedGameplayLayout()
    {
        GameObject root = CreatePanel("GameplayFixedRoot", new Color(0f, 0f, 0f, 0f), 884, 1584);

        BuildFixedHeader(root.transform);
        BuildFixedLeftRail(root.transform);
        BuildFixedCenterCards(root.transform);
        BuildFixedRightRail(root.transform);

        statusText = CreateAnchoredText(root.transform, "Auto-calls are live. Daub called numbers on any visible card.", 20, FontStyle.Bold, new Color(0.9f, 0.94f, 1f), 1120, 34, 0f, -404f);
        gameplayPowerUpBurstText = CreateAnchoredText(root.transform, "", 42, FontStyle.Bold, new Color(0.72f, 0.95f, 1f), 930, 78, 0f, 276f);
        gameplayPowerUpBurstText.gameObject.SetActive(false);
    }

    private void BuildFixedHeader(Transform parent)
    {
        CreateAnchoredPanel(parent, "HeaderBand", new Color(0.09f, 0.04f, 0.15f), 1540, 94, 0f, 386f);

        GameObject roomPanel = CreateAnchoredPanel(parent, "RoomTitle", new Color(0.15f, 0.08f, 0.22f), 470, 74, -485f, 386f);
        CreateAnchoredText(roomPanel.transform, RealmContentCatalog.ActivePrototypeRoom.Name, 27, FontStyle.Bold, new Color(1f, 0.91f, 0.52f), 440, 38, 0f, 12f);
        CreateAnchoredText(roomPanel.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 20, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 440, 24, 0f, -22f);

        gameplayManaText = CreateFixedCurrency(parent, "Mana", inventory.GetManaText(), -130f, 386f);
        gameplayCrystalText = CreateFixedCurrency(parent, "Crystals", inventory.GetCrystalText(), 110f, 386f);
        xpText = CreateAnchoredText(parent, BuildGameplayRankText(), 15, FontStyle.Bold, new Color(0.75f, 0.95f, 1f), 210, 58, 320f, 386f);
        gameplayMultiplayerRoomText = CreateAnchoredText(parent, string.Empty, 13, FontStyle.Bold, new Color(0.82f, 0.9f, 1f), 430, 18, -10f, 350f);
        gameplayMultiplayerAuthorityText = CreateAnchoredText(parent, string.Empty, 12, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 430, 18, -10f, 330f);

        GameObject balls = CreateAnchoredPanel(parent, "BallsLeft", new Color(0.15f, 0.08f, 0.22f), 150, 74, 565f, 386f);
        CreateAnchoredText(balls.transform, "BALLS LEFT", 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 130, 22, 0f, 18f);
        ballsLeftText = CreateAnchoredText(balls.transform, Mathf.Max(0, GetActiveMaxBallCalls() - roundFlow.History.Count).ToString(), 38, FontStyle.Bold, Color.white, 130, 42, 0f, -12f);
    }

    private string BuildGameplayRankText()
    {
        return $"Level {rewards.CurrentLevel}\n{rewards.GetLevelProgressText()}";
    }

    private void CreateFixedTile(Transform parent, string title, string detail, float width, float height, float x, float y, Color color)
    {
        GameObject tile = CreateAnchoredPanel(parent, title, color, width, height, x, y);
        CreateAnchoredText(tile.transform, title, 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 14, 24, 0f, 12f);
        CreateAnchoredText(tile.transform, detail, 18, FontStyle.Bold, Color.white, width - 14, 28, 0f, -16f);
    }

    private Text CreateFixedCurrency(Transform parent, string label, string value, float x, float y)
    {
        GameObject panel = CreateAnchoredPanel(parent, label, new Color(0.12f, 0.08f, 0.16f), 210, 70, x, y);
        CreateAnchoredText(panel.transform, label == "Mana" ? "*" : "<>", 28, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 46, -74f, 0f);
        Text valueText = CreateAnchoredText(panel.transform, value, 31, FontStyle.Bold, Color.white, 116, 46, 0f, 0f);
        CreateAnchoredText(panel.transform, "+", 28, FontStyle.Bold, new Color(0.3f, 1f, 0.22f), 34, 46, 78f, 0f);
        return valueText;
    }

    private void BuildFixedLeftRail(Transform parent)
    {
        GameObject rail = CreateAnchoredPanel(parent, "LeftGameplayRail", new Color(0.16f, 0.07f, 0.25f), 220, 720, -660f, -35f);
        CreateAnchoredText(rail.transform, IsBlackoutRoom() ? "BLACKOUTS" : "ROOM BINGOS", 22, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 34, 0f, 300f);
        string countText = IsBlackoutRoom() ? $"{GetTotalBingos()}/{selectedCardCount}" : $"{rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}";
        roomBingoCountText = CreateAnchoredText(rail.transform, countText, 52, FontStyle.Bold, Color.white, 190, 78, 0f, 242f);
        CreateAnchoredText(rail.transform, "AUTO-CLAIM", 17, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 190, 30, 0f, 184f);
        CreateAnchoredText(rail.transform, "POWER-UP", 13, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 20, 0f, 152f);
        Button ready = CreateAnchoredButton(rail.transform, GetPowerUpBankButtonText(), 13, 154, 46, GetPowerUpBankButtonColor(), 0f, 116f);
        ready.interactable = IsPowerUpBankReady();
        ready.onClick.AddListener(UsePowerUpBank);
        gameplayPowerUpReadyButton = ready;
        gameplayPowerUpReadyText = ready.GetComponentInChildren<Text>();
        gameplayPowerUpStockText = CreateAnchoredText(rail.transform, GetPowerUpBankStockText(), 11, FontStyle.Bold, new Color(0.82f, 0.86f, 1f), 180, 20, 0f, 82f);
        gameplayPowerUpEventText = CreateAnchoredText(rail.transform, fortuneDoublePrizeActive ? "Fortune x2 active" : "Bank charging", 11, FontStyle.Bold, new Color(0.82f, 0.86f, 1f), 180, 20, 0f, 60f);
        gameplayPowerUpAutoButton = CreateAnchoredButton(rail.transform, inventory.AutoDropPowerUps ? "Auto ON" : "Auto OFF", 13, 116, 30, inventory.AutoDropPowerUps ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.32f, 0.12f, 0.62f), 0f, 30f);
        gameplayPowerUpAutoButton.onClick.AddListener(TogglePowerUpAutoDrop);
        bingoBannerText = CreateAnchoredText(rail.transform, GetBingoBannerText().Length > 0 ? GetBingoBannerText() : IsBlackoutRoom() ? "No blackouts yet" : "No bingos yet", 14, FontStyle.Bold, new Color(0.9f, 0.9f, 1f), 190, 48, 0f, -14f);
        roundSummaryText = CreateAnchoredText(rail.transform, GetRoomPoolSummaryText(), 11, FontStyle.Bold, new Color(0.82f, 0.86f, 1f), 190, 48, 0f, -60f);
        gameplayMultiplayerClaimText = CreateAnchoredText(rail.transform, string.Empty, 11, FontStyle.Bold, new Color(0.86f, 0.9f, 1f), 190, 38, 0f, -102f);
        gameplayMultiplayerPostRoundText = CreateAnchoredText(rail.transform, string.Empty, 11, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 190, 38, 0f, -138f);
        CreateAnchoredText(rail.transform, "ALL CALLED\nNUMBERS", 17, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 48, 0f, -110f);
        Button toggle = CreateAnchoredButton(rail.transform, allCalledVisible ? "Hide" : "Show", 15, 104, 30, new Color(0.32f, 0.12f, 0.62f), 0f, -150f);
        toggle.onClick.AddListener(ToggleAllCalledVisible);
        calledHistoryText = CreateAnchoredText(rail.transform, GetCalledHistoryPanelText(), GetCalledHistoryPanelFontSize(), FontStyle.Bold, Color.white, 198, 238, 0f, -282f);
        RefreshGameplayMultiplayerStatus();
    }

    private void BuildFixedCenterCards(Transform parent)
    {
        GameObject center = CreateAnchoredPanel(parent, "CenterGameplayCards", new Color(0f, 0f, 0f, 0f), 1010, 720, 0f, -35f);
        VerticalLayoutGroup layout = center.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        BuildVisibleCards(center.transform, 970);
    }

    private void BuildFixedRightRail(Transform parent)
    {
        callQueueTexts.Clear();
        GameObject rail = CreateAnchoredPanel(parent, "RightGameplayRail", new Color(0.88f, 0.8f, 0.62f), 272, 720, 660f, -35f);
        CreateAnchoredText(rail.transform, "NEXT CALL", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 28, 0f, 315f);
        calledNumberText = CreateAnchoredText(rail.transform, roundFlow.History.Count > 0 ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}" : "Starting", 46, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 62, 0f, 270f);
        timerText = CreateAnchoredText(rail.transform, "--", 24, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 36, 0f, 222f);
        CreateAnchoredImage(rail.transform, "Clairvoyance", 104, 82, 0f, 154f);
        gameplayClairvoyanceText = CreateAnchoredText(rail.transform, GetGameplayClairvoyanceText(), 14, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 34, 0f, 96f);
        CreateAnchoredText(rail.transform, "CALL QUEUE", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 32, 0f, 68f);

        for (int index = 0; index < 6; index++)
        {
            string label = GetCallQueueLabel(index);
            float y = 8f - (index * 78f);
            GameObject call = CreateAnchoredPanel(rail.transform, $"Queue_{index}", GetQueueColor(index), 92, 70, 0f, y);
            callQueueTexts.Add(CreateAnchoredText(call.transform, label, 24, FontStyle.Bold, Color.white, 84, 42, 0f, 0f));
        }

        RefreshCalledBallDisplays();
    }

    private Color GetQueueColor(int index)
    {
        Color[] colors =
        {
            new Color(0.26f, 0.1f, 0.58f),
            new Color(0.05f, 0.18f, 0.65f),
            new Color(0.05f, 0.48f, 0.78f),
            new Color(0.64f, 0.04f, 0.28f),
            new Color(0.08f, 0.48f, 0.16f),
            new Color(0.26f, 0.1f, 0.58f)
        };

        return colors[Mathf.Clamp(index, 0, colors.Length - 1)];
    }

    private void BuildGameplayHeader()
    {
        GameObject header = CreatePanel("GameplayHeader", new Color(0.09f, 0.04f, 0.15f), 86, 1540);
        HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(14, 14, 12, 12);
        layout.spacing = 16;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        GameObject roomPanel = CreatePanelInParent(header.transform, "RoomTitle", new Color(0.15f, 0.08f, 0.22f), 470, 80);
        VerticalLayoutGroup roomLayout = roomPanel.AddComponent<VerticalLayoutGroup>();
        roomLayout.padding = new RectOffset(10, 10, 8, 8);
        roomLayout.spacing = 2;
        roomLayout.childAlignment = TextAnchor.MiddleCenter;
        roomLayout.childControlWidth = false;
        roomLayout.childControlHeight = false;
        roomLayout.childForceExpandWidth = false;
        roomLayout.childForceExpandHeight = false;
        CreateTextInParent(roomPanel.transform, RealmContentCatalog.ActivePrototypeRoom.Name, 30, FontStyle.Bold, new Color(1f, 0.91f, 0.52f), 450, 42);
        CreateTextInParent(roomPanel.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 20, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 450, 24);

        gameplayManaText = CreateHeaderCurrency(header.transform, "Mana", inventory.GetManaText());
        gameplayCrystalText = CreateHeaderCurrency(header.transform, "Crystals", inventory.GetCrystalText());

        GameObject balls = CreatePanelInParent(header.transform, "BallsLeft", new Color(0.15f, 0.08f, 0.22f), 160, 80);
        VerticalLayoutGroup ballsLayout = balls.AddComponent<VerticalLayoutGroup>();
        ballsLayout.padding = new RectOffset(8, 8, 8, 8);
        ballsLayout.spacing = 0;
        ballsLayout.childAlignment = TextAnchor.MiddleCenter;
        ballsLayout.childControlWidth = false;
        ballsLayout.childControlHeight = false;
        ballsLayout.childForceExpandWidth = false;
        ballsLayout.childForceExpandHeight = false;
        CreateTextInParent(balls.transform, "BALLS LEFT", 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 140, 24);
        CreateTextInParent(balls.transform, Mathf.Max(0, 75 - roundFlow.CalledCount).ToString(), 38, FontStyle.Bold, Color.white, 140, 42);
    }

    private void CreateHeaderTile(Transform parent, string title, string detail, float width, Color color)
    {
        GameObject tile = CreatePanelInParent(parent, title, color, width, 80);
        VerticalLayoutGroup layout = tile.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 2;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(tile.transform, title, 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 18, 24);
        CreateTextInParent(tile.transform, detail, 19, FontStyle.Bold, Color.white, width - 18, 34);
    }

    private Text CreateHeaderCurrency(Transform parent, string label, string value)
    {
        GameObject panel = CreatePanelInParent(parent, label, new Color(0.12f, 0.08f, 0.16f), 210, 70);
        HorizontalLayoutGroup layout = panel.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 10, 10);
        layout.spacing = 12;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(panel.transform, label == "Mana" ? "*" : "<>", 28, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 46);
        Text valueText = CreateTextInParent(panel.transform, value, 31, FontStyle.Bold, Color.white, 116, 46);
        CreateTextInParent(panel.transform, "+", 28, FontStyle.Bold, new Color(0.3f, 1f, 0.22f), 34, 46);
        return valueText;
    }

    private void BuildGameplayBody()
    {
        GameObject body = CreatePanel("GameplayBody", new Color(0f, 0f, 0f, 0f), 760, 1540);
        HorizontalLayoutGroup layout = body.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        BuildLeftGameplayRail(body.transform);
        BuildCenterGameplayCards(body.transform);
        BuildRightGameplayRail(body.transform);
    }

    private void BuildLeftGameplayRail(Transform parent)
    {
        GameObject rail = CreatePanelInParent(parent, "LeftGameplayRail", new Color(0.16f, 0.07f, 0.25f), 220, 760);
        VerticalLayoutGroup layout = rail.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(14, 14, 14, 14);
        layout.spacing = 14;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(rail.transform, IsBlackoutRoom() ? "BLACKOUTS" : "BINGOS", 26, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 34);
        CreateTextInParent(rail.transform, GetTotalBingos().ToString(), 60, FontStyle.Bold, Color.white, 190, 78);
        CreateTextInParent(rail.transform, "AUTO-CLAIM", 17, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 190, 30);
        CreateTextInParent(rail.transform, GetBingoBannerText().Length > 0 ? GetBingoBannerText() : IsBlackoutRoom() ? "No blackouts yet" : "No bingos yet", 14, FontStyle.Bold, new Color(0.9f, 0.9f, 1f), 190, 82);
        CreateTextInParent(rail.transform, "ALL CALLED\nNUMBERS", 18, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 52);
        calledHistoryText = CreateTextInParent(rail.transform, roundFlow.History.Count > 0 ? GetCalledHistoryText() : "none", 16, FontStyle.Normal, Color.white, 190, 170);
    }

    private void BuildCenterGameplayCards(Transform parent)
    {
        GameObject center = CreatePanelInParent(parent, "CenterGameplayCards", new Color(0f, 0f, 0f, 0f), 1010, 760);
        VerticalLayoutGroup layout = center.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        bingoBannerText = CreateTextInParent(center.transform, GetBingoBannerText(), 25, FontStyle.Bold, new Color(1f, 0.84f, 0.18f), 970, 32);
        BuildVisibleCards(center.transform, 970);
        roundSummaryText = CreateTextInParent(center.transform, GetRoundSummaryText(), 17, FontStyle.Bold, new Color(0.9f, 0.92f, 1f), 970, 42);
    }

    private void BuildRightGameplayRail(Transform parent)
    {
        GameObject rail = CreatePanelInParent(parent, "RightGameplayRail", new Color(0.88f, 0.8f, 0.62f), 272, 760);
        VerticalLayoutGroup layout = rail.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 14, 14);
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(rail.transform, "NEXT CALL", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 28);
        calledNumberText = CreateTextInParent(rail.transform, roundFlow.History.Count > 0 ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}" : "Starting", 46, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 62);
        timerText = CreateTextInParent(rail.transform, "--", 24, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 36);
        CreateTextInParent(rail.transform, "CALL QUEUE", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 32);

        for (int index = 0; index < 6; index++)
        {
            string label = roundFlow.History.Count > index ? $"{GetBingoLetter(roundFlow.History[index])}\n{roundFlow.History[index]}" : "-";
            CreateTextInParent(rail.transform, label, 30, FontStyle.Bold, Color.white, 96, 76);
        }
    }

    private int GetTotalBingos()
    {
        if (IsBlackoutRoom())
        {
            int total = 0;
            for (int index = 0; index < playerCards.Count; index++)
            {
                if (playerCards.CountMarkedPlayable(index) >= BingoRoomRules.BlackoutPlayableSquares)
                {
                    total++;
                }
            }

            return total;
        }

        return playerCards.GetTotalBingos() + placedSigilBonusBingos;
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

        Image background = CreateAnchoredImage(
            contentRoot,
            backgroundResource,
            StageWidth,
            StageHeight,
            0f,
            0f);
        background.name = RealmHomeBackgroundObjectPrefix + stateAssetName;
        background.preserveAspect = false;
        background.raycastTarget = false;
        background.transform.SetAsFirstSibling();

        // On screens wider or taller than 16:9 the stage is fit (so rooms stay aligned with the art); a dimmed,
        // enlarged copy of the same art fills the leftover bands instead of plain bars. Sized in ApplyStageScale.
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
        if (!IsRealmUnlocked(RealmContentCatalog.ActivePrototypeRealmIndex))
        {
            return false;
        }

        if (roomIndex <= 0)
        {
            return true;
        }

        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        return inventory.IsRoomRestored(rooms[roomIndex])
            || inventory.IsRoomRestored(rooms[roomIndex - 1]);
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

        return IsRealmComplete(realmIndex - 1);
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

    private void BuildLobbyUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        StopRoundAndClearCalledHistory();
        ClampManaBetToActiveRoom();

        VerticalLayoutGroup layout = PrepareStage(LobbyStageWidth, LobbyStageHeight, new Color(0.12f, 0.25f, 0.18f, 0.98f), 0, 0);
        layout.enabled = false;
        roomEntryPanelView = contentRoot.gameObject.GetComponent<RoomEntryPanelView>();
        if (roomEntryPanelView == null)
        {
            roomEntryPanelView = contentRoot.gameObject.AddComponent<RoomEntryPanelView>();
        }

        roomEntryPanelView.ResetRuntimeBindings();
        RoomEntryPanelAnimator roomEntryAnimator = contentRoot.gameObject.GetComponent<RoomEntryPanelAnimator>();
        if (roomEntryAnimator == null)
        {
            roomEntryAnimator = contentRoot.gameObject.AddComponent<RoomEntryPanelAnimator>();
        }

        roomEntryPanelView.BindAnimator(roomEntryAnimator);

        BuildLobbyHeader();

        Button back = CreateAnchoredButton(contentRoot, "Back", 18, 112, 54, new Color(0.35f, 0.12f, 0.62f), -710f, 382f);
        back.onClick.AddListener(() => BeginLoadingTransition(BuildRealmMapUi));
        BuildLobbyClairvoyanceRail();

        BuildLobbyRestorePanel();
        BuildLobbyJackpotPanel();

        string pickTitle = IsBlackoutRoom() ? "Pick Cards for Blackout" : "Pick Cards to Play";
        CreateAnchoredText(contentRoot, pickTitle, 44, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 1180, 58, 0f, -78f);
        if (IsBlackoutRoom())
        {
            CreateAnchoredText(contentRoot, "Daub every playable square on a card to earn one wheelspin.", 22, FontStyle.Bold, new Color(0.86f, 0.9f, 1f), 980, 32, 0f, -120f);
        }

        CreateCardOptionAnchored(1, -570f, -252f);
        CreateCardOptionAnchored(2, -190f, -252f);
        CreateCardOptionAnchored(4, 190f, -252f);
        CreateCardOptionAnchored(6, 570f, -252f);

        GameObject betRow = CreateAnchoredPanel(contentRoot, "LobbyBetRow", new Color(0.97f, 0.89f, 0.68f), 700, 78, 0f, -405f);
        Button minus = CreateAnchoredButton(betRow.transform, "-", 30, 72, 58, new Color(0.35f, 0.12f, 0.62f), -230f, 0f);
        Button plus = CreateAnchoredButton(betRow.transform, "+", 30, 72, 58, new Color(0.35f, 0.12f, 0.62f), 230f, 0f);
        roomText = CreateAnchoredText(betRow.transform, "", 24, FontStyle.Bold, new Color(0.15f, 0.1f, 0.32f), 300, 56, 0f, 0f);

        RoomEntryBetRowView betRowView = betRow.AddComponent<RoomEntryBetRowView>();
        betRowView.ResetRuntimeBindings();
        betRowView.Initialize(minus, plus, roomText);
        betRowView.DecreaseRequested += () => AdjustLobbyBet(-GetActiveProgression().BetStep);
        betRowView.IncreaseRequested += () => AdjustLobbyBet(GetActiveProgression().BetStep);
        roomEntryPanelView.BindBetRow(betRowView);
        RefreshLobbyBetText();
        SyncPrototypeMultiplayerLobbyState();
        RefreshLobbyMultiplayerStatus();

        ApplyStageScale(true);
        roomEntryPanelView.ShowIdleState();
    }

    private void BuildLobbyJackpotPanel()
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, "JackpotWheelspin", new Color(0.35f, 0.08f, 0.48f), 300, 178, 610f, 118f);
        Color textColor = Color.white;
        Text titleText = CreateAnchoredText(tile.transform, "Jackpot\nWheelspin", 22, FontStyle.Bold, textColor, 274f, 62f, 0f, 42f);
        Text detailText = CreateAnchoredText(tile.transform, $"Pot {inventory.GetCurrentJackpotPotText()}\n{inventory.GetJackpotSpinText()}", 30, FontStyle.Bold, textColor, 274f, 78f, 0f, -34f);

        Button spinPendingButton = CreateAnchoredButton(contentRoot, "Spin Pending", 16, 178, 34, new Color(0.12f, 0.55f, 0.08f), 610f, 36f);
        Text spinPendingText = spinPendingButton.GetComponentInChildren<Text>();

        RoomEntryJackpotView jackpotView = tile.AddComponent<RoomEntryJackpotView>();
        jackpotView.ResetRuntimeBindings();
        jackpotView.Initialize(titleText, detailText, spinPendingButton, spinPendingText);
        jackpotView.SpinPendingRequested += BuildJackpotSpinUi;
        roomEntryPanelView?.BindJackpotPanel(jackpotView);
        roomEntryPanelView?.ApplyJackpotPanel(BuildRoomEntryJackpotDisplayModel());
    }

    private RoomEntryJackpotDisplayModel BuildRoomEntryJackpotDisplayModel()
    {
        return new RoomEntryJackpotDisplayModel(
            "Jackpot\nWheelspin",
            $"Pot {inventory.GetCurrentJackpotPotText()}\n{inventory.GetJackpotSpinText()}",
            inventory.PendingJackpotSpins > 0,
            "Spin Pending");
    }

    private void BuildLobbyClairvoyanceRail()
    {
        GameObject rail = CreateAnchoredPanel(contentRoot, "LobbyClairvoyanceRail", new Color(0.16f, 0.07f, 0.25f), 150, 132, -710f, -20f);
        Button activate = rail.AddComponent<Button>();
        activate.targetGraphic = rail.GetComponent<Image>();
        activate.interactable = inventory.CanActivateClairvoyance() && !inventory.HasActiveClairvoyance();
        activate.onClick.AddListener(ActivatePrototypeClairvoyance);

        Text title = CreateAnchoredText(rail.transform, "EYE", 26, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 120, 36, 0f, 34f);
        Text label = CreateAnchoredText(rail.transform, "Clairvoyance", 13, FontStyle.Bold, Color.white, 130, 22, 0f, 4f);
        lobbyClairvoyanceStatusText = CreateAnchoredText(rail.transform, GetLobbyClairvoyanceActionText(), 17, FontStyle.Bold, inventory.HasActiveClairvoyance() ? new Color(0.55f, 1f, 0.85f) : new Color(0.78f, 0.74f, 0.88f), 140, 38, 0f, -34f);
        title.raycastTarget = false;
        label.raycastTarget = false;
        lobbyClairvoyanceStatusText.raycastTarget = false;
    }

    private string GetLobbyClairvoyanceActionText()
    {
        if (inventory.HasActiveClairvoyance())
        {
            return inventory.GetActiveClairvoyanceTimeText();
        }

        return inventory.CanActivateClairvoyance()
            ? "TAP TO ACTIVATE"
            : $"{inventory.ClairvoyanceMinutes}m / NEED 15m";
    }

    private void BuildLobbyHeader()
    {
        GameObject header = CreateAnchoredPanel(contentRoot, "LobbyHeader", new Color(0f, 0f, 0f, 0f), 1500, 184, 0f, 322f);

        CreateAnchoredPanel(header.transform, "LobbyTopBand", new Color(0.09f, 0.04f, 0.15f), 1500, 72, 0f, 52f);
        lobbyManaText = CreateLobbyResourceTile(header.transform, "Mana", inventory.GetManaText(), "*", 220, 190f, 52f);
        lobbyCrystalText = CreateLobbyResourceTile(header.transform, "Crystals", inventory.GetCrystalText(), "<>", 230, 430f, 52f);
        lobbyPowerUpText = CreateLobbyResourceTile(header.transform, "Power-Ups", inventory.GetPowerUpText(), "+", 230, 680f, 52f, true);

        GameObject titlePanel = CreateAnchoredPanel(header.transform, "LobbyRoomTitle", new Color(0.15f, 0.08f, 0.22f), 900, 104, -280f, -44f);
        Text roomNameText = CreateAnchoredText(titlePanel.transform, RealmContentCatalog.ActivePrototypeRoom.Name, 42, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 860, 50, 0f, 20f);
        Text realmNameText = CreateAnchoredText(titlePanel.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 22, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 820, 30, 0f, -16f);
        Text potionLabelText = CreateAnchoredText(titlePanel.transform, $"Potion: {RealmContentCatalog.ActivePrototypeRoom.PotionName}", 19, FontStyle.Bold, Color.white, 780, 28, 0f, -44f);
        lobbyMultiplayerTitleText = CreateAnchoredText(titlePanel.transform, string.Empty, 16, FontStyle.Bold, new Color(0.75f, 0.95f, 1f), 540, 22, -150f, -72f);
        lobbyMultiplayerReadinessText = CreateAnchoredText(titlePanel.transform, string.Empty, 14, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 260, 20, 276f, -72f);
        lobbyMultiplayerDetailText = CreateAnchoredText(titlePanel.transform, string.Empty, 12, FontStyle.Bold, new Color(0.86f, 0.9f, 1f), 860, 20, 0f, -92f);
        lobbyMultiplayerParticipantsText = CreateAnchoredText(titlePanel.transform, string.Empty, 11, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 860, 18, 0f, -112f);

        GameObject modeBadge = CreateAnchoredPanel(titlePanel.transform, $"{RealmContentCatalog.ActivePrototypeRoom.ModeLabel}ModeBadge", new Color(0.18f, 0.11f, 0.34f), 126f, 74f, 334f, -12f);
        Text modeBadgeText = CreateAnchoredText(modeBadge.transform, RealmContentCatalog.ActivePrototypeRoom.ModeLabel.ToUpperInvariant(), 13, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 116f, 18f, 0f, 20f);
        GameObject modeIcon = CreateAnchoredPanel(modeBadge.transform, $"{RealmContentCatalog.ActivePrototypeRoom.ModeLabel}ModeIcon", new Color(0.96f, 0.86f, 0.62f), 104f, 28f, 0f, -10f);
        Text modeIconText = CreateAnchoredText(modeIcon.transform, GetRoomModeIconText(RealmContentCatalog.ActivePrototypeRoom), 15, FontStyle.Bold, new Color(0.9f, 0.18f, 1f), 96f, 22f, 0f, 0f);

        Text rankText = CreateAnchoredText(header.transform, BuildGameplayRankText(), 14, FontStyle.Bold, new Color(0.75f, 0.95f, 1f), 220, 58, 12f, 52f);

        RoomEntryHeaderView headerView = header.AddComponent<RoomEntryHeaderView>();
        headerView.ResetRuntimeBindings();
        headerView.Initialize(
            roomNameText,
            realmNameText,
            potionLabelText,
            rankText,
            modeBadge.GetComponent<Image>(),
            modeBadgeText,
            modeIconText);
        roomEntryPanelView?.BindHeader(headerView);
        roomEntryPanelView?.ApplyHeader(BuildRoomEntryHeaderDisplayModel());
    }

    private RoomEntryHeaderDisplayModel BuildRoomEntryHeaderDisplayModel()
    {
        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        return new RoomEntryHeaderDisplayModel(
            room.Name,
            RealmContentCatalog.ActivePrototypeRealm.Name,
            $"Potion: {room.PotionName}",
            BuildGameplayRankText(),
            room.ModeLabel.ToUpperInvariant(),
            GetRoomModeIconText(room),
            room.IsSpecial);
    }

    private RoomEntryRestorePanelDisplayModel BuildRoomEntryRestorePanelDisplayModel()
    {
        return new RoomEntryRestorePanelDisplayModel(
            RealmContentCatalog.ActivePrototypeRoom.PotionName,
            "Collect ingredients while you play bingo.",
            inventory.GetRestoreStatusText(),
            "Ingredients",
            inventory.GetFullIngredientProgressText(),
            $"Restore Reward\n{inventory.GetRestoreRewardText()}",
            inventory.ActiveRoomRestored ? "Restored" : "Restore",
            inventory.CanRestoreActiveRoom());
    }

    private static string GetRoomModeIconText(RoomDefinition room)
    {
        return room.IsSpecial ? "25" : "*****";
    }

    private Text CreateLobbyResourceTile(Transform parent, string label, string value, string icon, float width, float x)
    {
        return CreateLobbyResourceTile(parent, label, value, icon, width, x, 0f, false);
    }

    private Text CreateLobbyResourceTile(Transform parent, string label, string value, string icon, float width, float x, float y)
    {
        return CreateLobbyResourceTile(parent, label, value, icon, width, x, y, false);
    }

    private Text CreateLobbyResourceTile(Transform parent, string label, string value, string icon, float width, float x, float y, bool clickActivatesClairvoyance)
    {
        GameObject panel = CreateAnchoredPanel(parent, label, new Color(0.12f, 0.08f, 0.16f), width, 58, x, y);
        string display = string.IsNullOrEmpty(value) ? icon : $"{icon}  {value}";
        Text valueText = CreateAnchoredText(panel.transform, display, string.IsNullOrEmpty(value) ? 26 : 23, FontStyle.Bold, Color.white, width - 18, 36, 0f, string.IsNullOrEmpty(label) ? 0f : 8f);
        if (!string.IsNullOrEmpty(label))
        {
            CreateAnchoredText(panel.transform, label, 13, FontStyle.Bold, new Color(0.84f, 0.88f, 1f), width - 18, 18, 0f, -17f);
        }

        if (clickActivatesClairvoyance)
        {
            Button button = panel.AddComponent<Button>();
            button.targetGraphic = panel.GetComponent<Image>();
            button.onClick.AddListener(BuildPlayerDenUi);
        }

        return valueText;
    }

    private void BuildPlayerDenUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        StopRoundAndClearCalledHistory();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.07f, 0.035f, 0.1f, 0.98f), 0, 0);
        layout.enabled = false;

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.86f, 0.82f, 0.95f);
        Color deepPurple = new Color(0.13f, 0.06f, 0.2f);
        Color wood = new Color(0.24f, 0.12f, 0.08f);

        CreateAnchoredPanel(contentRoot, "DenBackWall", new Color(0.12f, 0.07f, 0.16f), 1500, 820, 0f, -12f);
        CreateAnchoredPanel(contentRoot, "DenFloor", new Color(0.17f, 0.09f, 0.07f), 1500, 210, 0f, -346f);

        CreateAnchoredPanel(contentRoot, "DenTopBand", new Color(0.06f, 0.03f, 0.1f), 1450, 76, 0f, 382f);
        Text brandText = CreateAnchoredText(contentRoot, string.Empty, 32, FontStyle.Bold, gold, 310, 68, -10f, 382f);
        Text manaValueText = CreateLobbyResourceTile(contentRoot, "Mana", profile.Mana.ToString("N0"), "*", 230, 356f, 382f);
        Text crystalsValueText = CreateLobbyResourceTile(contentRoot, "Crystals", profile.Crystals.ToString("N0"), "<>", 230, 606f, 382f);
        Button rankButton = CreateAnchoredButton(contentRoot, string.Empty, 14, 300, 58, new Color(0.09f, 0.04f, 0.15f), -600f, 382f);
        Button map = CreateAnchoredButton(contentRoot, string.Empty, 18, 104, 50, new Color(0.35f, 0.12f, 0.62f), -474f, 382f);
        Button room = CreateAnchoredButton(contentRoot, string.Empty, 18, 104, 50, new Color(0.35f, 0.12f, 0.62f), -354f, 382f);
        Button settings = CreateAnchoredButton(contentRoot, string.Empty, 16, 72, 50, new Color(0.18f, 0.08f, 0.28f), 700f, 382f);

        PlayerDenTopBandView topBandView = contentRoot.gameObject.GetComponent<PlayerDenTopBandView>();
        if (topBandView == null)
        {
            topBandView = contentRoot.gameObject.AddComponent<PlayerDenTopBandView>();
        }

        topBandView.ResetRuntimeBindings();
        topBandView.Initialize(brandText, manaValueText, crystalsValueText, rankButton, map, room, settings);
        topBandView.RankRequested += OpenPlayerProfileSummaryRequested;
        topBandView.MapRequested += BuildWorldMapUi;
        topBandView.RoomRequested += BuildLobbyUi;
        topBandView.SettingsRequested += OpenDenSettingsRequested;
        topBandView.Apply(new PlayerDenTopBandDisplayModel
        {
            BrandText = "BINGO\nMAGIC MAYHEM",
            ManaText = profile.Mana.ToString("N0"),
            CrystalsText = profile.Crystals.ToString("N0"),
            RankButtonText = $"Profile\nL{profile.Level} | Aura TBD",
            MapButtonText = "Map",
            RoomButtonText = "Room",
            SettingsButtonText = "SET"
        });

        GameObject titlePanel = CreateAnchoredPanel(contentRoot, "DenTitle", new Color(0.15f, 0.08f, 0.22f), 720, 82, 0f, 286f);
        Text denTitleText = CreateAnchoredText(titlePanel.transform, string.Empty, 40, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 660, 48, 0f, 12f);
        Text denSubtitleText = CreateAnchoredText(titlePanel.transform, string.Empty, 16, FontStyle.Bold, muted, 680, 26, 0f, -24f);
        PlayerDenTitleView denTitleView = titlePanel.GetComponent<PlayerDenTitleView>();
        if (denTitleView == null)
        {
            denTitleView = titlePanel.AddComponent<PlayerDenTitleView>();
        }

        denTitleView.ResetRuntimeBindings();
        denTitleView.Initialize(denTitleText, denSubtitleText);
        denTitleView.Apply(new PlayerDenTitleDisplayModel
        {
            TitleText = "PLAYER'S DEN",
            SubtitleText = "Inventory, claims, and account hub | Rank now derives from Aura Strength"
        });

        GameObject library = CreateDenDoorTile("Library\nGrimoire", "Albums and cards", -530f, 150f, 280, 150, new Color(0.18f, 0.1f, 0.16f));
        BindPlayerDenDoorInteraction(library, BuildLibraryGrimoireUi);
        GameObject bazaar = CreateDenDoorTile("Bewitchment\nBazaar", "Oracle Alley", 0f, 138f, 320, 142, new Color(0.18f, 0.07f, 0.26f));
        BindPlayerDenDoorInteraction(bazaar, BuildBewitchmentBazaarUi);
        GameObject apothecary = CreateDenDoorTile("Apothecary", "Potion workshop", 358f, 145f, 260, 132, new Color(0.12f, 0.18f, 0.11f));
        BindPlayerDenDoorInteraction(apothecary, BuildApothecaryUi);
        GameObject relicWall = CreateDenDoorTile("Relic Wall", "Badges later", 626f, 128f, 220, 210, new Color(0.16f, 0.1f, 0.08f));
        BindPlayerDenDoorInteraction(relicWall, BuildRelicWallUi);
        GameObject cabinet = CreateDenDoorTile("Cabinet of\nCuriosities", "Inventory stash", 536f, -176f, 300, 172, wood);
        BindPlayerDenDoorInteraction(cabinet, BuildPowerUpInventoryUi);

        Button cauldron = CreateAnchoredButton(contentRoot, GetDenManaCauldronButtonText(), 22, 420, 184, new Color(0.16f, 0.08f, 0.2f), 0f, -118f);
        denManaCauldronText = cauldron.GetComponentInChildren<Text>();
        Text cauldronSummaryText = CreateAnchoredText(contentRoot, $"Restored rooms: {inventory.GetRestoredRoomCount()} | +{inventory.GetManaCauldronHourlyRefillAmount()}/hour", 18, FontStyle.Bold, muted, 520, 28, 0f, -232f);

        PlayerDenCauldronView cauldronView = cauldron.gameObject.GetComponent<PlayerDenCauldronView>();
        if (cauldronView == null)
        {
            cauldronView = cauldron.gameObject.AddComponent<PlayerDenCauldronView>();
        }

        cauldronView.ResetRuntimeBindings();
        cauldronView.Initialize(cauldron, denManaCauldronText, cauldronSummaryText);
        cauldronView.Requested += OpenManaCauldronRequested;
        cauldronView.Apply(BuildPlayerDenCauldronDisplayModel());

        PlayerDenFeatureTileView trailTile = CreateDenFeatureTile(
            "DenEnchantedTrailTile",
            deepPurple,
            370,
            268,
            -568f,
            -174f,
            "ENCHANTED\nTRAIL",
            "Task path and free rewards",
            "Prototype shell",
            "Tap to view",
            29,
            gold,
            muted);
        trailTile.Selected += BuildEnchantedTrailUi;

        GameObject covenTile = CreateDenDoorTile("Coven\nCircle", "Orbs and sharing", -292f, -172f, 176, 148, new Color(0.08f, 0.05f, 0.1f));
        BindPlayerDenDoorInteraction(covenTile, BuildCovenCircleUi);

        PlayerDenActionTileView freebies = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Freebies",
            SubtitleText = "social link"
        }, -620f);
        freebies.Selected += BuildSocialFreebiesUi;
        PlayerDenActionTileView friends = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Friends",
            SubtitleText = $"{friendsState.Friends.Count} friends"
        }, -440f);
        friends.Selected += BuildFriendsUi;
        PlayerDenActionTileView leaders = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Leaders",
            SubtitleText = "friends | team | global"
        }, -260f);
        leaders.Selected += BuildLeaderboardUi;
        PlayerDenActionTileView market = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Mayhem\nMarket",
            SubtitleText = "IAP shell"
        }, -80f);
        market.Selected += BuildMayhemMarketUi;
        PlayerDenActionTileView dailyBonus = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Daily Bonus",
            SubtitleText = inventory.GetDailyBonusClaimStateText()
        }, 250f);
        dailyBonus.Selected += BuildDailyBonusUi;
        PlayerDenActionTileView dailySpin = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Daily Spin",
            SubtitleText = inventory.GetDailySpinClaimStateText()
        }, 430f);
        dailySpin.Selected += BuildDailySpinUi;
        PlayerDenActionTileView inbox = CreateDenBottomButton(new PlayerDenActionTileDisplayModel
        {
            TitleText = "Inbox",
            SubtitleText = GetDenInboxStatusText()
        }, 610f);
        inbox.Selected += BuildInboxUi;

        ApplyStageScale(true);
    }

    private PlayerDenCauldronDisplayModel BuildPlayerDenCauldronDisplayModel()
    {
        return new PlayerDenCauldronDisplayModel
        {
            ButtonText = GetDenManaCauldronButtonText(),
            SummaryText = $"Restored rooms: {inventory.GetRestoredRoomCount()} | +{inventory.GetManaCauldronHourlyRefillAmount()}/hour"
        };
    }

    private void BindUiInteraction(Button button, System.Action action)
    {
        if (button == null || action == null)
        {
            return;
        }

        UIInteractionHandler handler = button.gameObject.GetComponent<UIInteractionHandler>();
        if (handler == null)
        {
            handler = button.gameObject.AddComponent<UIInteractionHandler>();
        }

        handler.Initialize(button);
        handler.Requested += action;
    }

    private void BindPlayerDenDoorInteraction(GameObject tile, System.Action action)
    {
        if (tile == null || action == null)
        {
            return;
        }

        Button button = tile.GetComponent<Button>();
        if (button == null)
        {
            button = tile.AddComponent<Button>();
        }

        PlayerDenDoorView doorView = tile.GetComponent<PlayerDenDoorView>();
        if (doorView == null)
        {
            doorView = tile.AddComponent<PlayerDenDoorView>();
        }

        button.targetGraphic = tile.GetComponent<Image>();
        doorView.Selected += action;
    }

    private void OpenPlayerProfileSummaryRequested()
    {
        BuildPlayerProfileUi(PlayerProfileTabIds.Profile);
    }

    private void OpenDenSettingsRequested()
    {
        BuildDevSettingsUi(false, true);
    }

    private void OpenManaCauldronRequested()
    {
        BuildManaCauldronUi();
    }

    private void CreateInventoryRows(Transform parent, IReadOnlyList<ProfileInventoryLine> lines)
    {
        const int columns = 2;
        float[] xPositions = { -190f, 190f };
        for (int index = 0; index < lines.Count; index++)
        {
            int row = index / columns;
            int column = index % columns;
            float y = 156f - row * 70f;
            ProfileInventoryLine line = lines[index];
            GameObject rowPanel = CreateAnchoredPanel(parent, $"Inventory_{line.Label}", new Color(0.99f, 0.94f, 0.78f), 330, 54, xPositions[column], y);
            Text labelText = CreateAnchoredText(rowPanel.transform, line.Label, 17, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 300, 22, 0f, 10f);
            Text valueText = CreateAnchoredText(rowPanel.transform, line.Value, 15, FontStyle.Bold, new Color(0.18f, 0.14f, 0.28f), 300, 22, 0f, -13f);
            UILabelValueCardView rowView = rowPanel.GetComponent<UILabelValueCardView>();
            if (rowView == null)
            {
                rowView = rowPanel.AddComponent<UILabelValueCardView>();
            }

            rowView.ResetRuntimeBindings();
            rowView.Initialize(labelText, valueText);
            rowView.Apply(line.Label, line.Value);
        }
    }

    private void BuildPlayerProfileUi(string tab)
    {
        playerProfileActiveTab = string.IsNullOrWhiteSpace(tab) ? PlayerProfileTabIds.Profile : tab;
        RemovePlayerProfileModal();

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        GameObject panel = CreateAnchoredPanel(contentRoot, "PlayerProfileModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1080, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        PlayerProfileModalDisplayModel modalDisplayModel = BuildPlayerProfileModalDisplayModel(profile);
        UIModalChromeView modalChromeView = CreateModalChrome(
            panel,
            modalDisplayModel.Title,
            modalDisplayModel.Subtitle,
            modalDisplayModel.CloseButtonText,
            gold,
            muted,
            new Color(0.35f, 0.12f, 0.62f),
            820f,
            274f,
            820f,
            28f,
            232f,
            18,
            220f,
            48f,
            0f,
            -286f,
            20);

        PlayerProfileTabButtonView profileTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Profile, -220f, 184f);
        PlayerProfileTabButtonView avatarsTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Avatars, 0f, 184f);
        PlayerProfileTabButtonView settingsTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Settings, 220f, 184f);

        BuildPlayerProfileTabContent(panel.transform, profile);

        playerProfileModalView = panel.AddComponent<PlayerProfileModalView>();
        playerProfileModalView.ResetRuntimeBindings();
        playerProfileModalView.Initialize(modalChromeView);
        playerProfileModalView.RegisterTab(profileTabView);
        playerProfileModalView.RegisterTab(avatarsTabView);
        playerProfileModalView.RegisterTab(settingsTabView);
        playerProfileModalView.SetActiveTab(playerProfileActiveTab);
        playerProfileModalView.Apply(modalDisplayModel);
        playerProfileModalView.CloseRequested += RemovePlayerProfileModal;
        playerProfileModalView.TabRequested += BuildPlayerProfileUi;
    }

    private PlayerProfileTabButtonView CreatePlayerProfileTabButton(Transform parent, string tab, float x, float y)
    {
        bool active = playerProfileActiveTab == tab;
        Button button = CreateAnchoredButton(parent, tab, 18, 190, 42, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        UITabButtonBinder binder = button.gameObject.AddComponent<UITabButtonBinder>();
        binder.SetSelected(active);
        PlayerProfileTabButtonView tabView = button.gameObject.AddComponent<PlayerProfileTabButtonView>();
        tabView.ResetRuntimeBindings();
        tabView.Initialize(binder, tab);
        return tabView;
    }

    private void BuildPlayerProfileTabContent(Transform parent, PlayerProfileState profile)
    {
        if (playerProfileActiveTab == PlayerProfileTabIds.Avatars)
        {
            BuildPlayerAvatarTab(parent, profile);
            return;
        }

        if (playerProfileActiveTab == PlayerProfileTabIds.Settings)
        {
            BuildPlayerSettingsTab(parent);
            return;
        }

        BuildPlayerProfileSummaryTab(parent, profile);
    }

    private void BuildPlayerProfileSummaryTab(Transform parent, PlayerProfileState profile)
    {
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color purple = new Color(0.18f, 0.08f, 0.32f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        GameObject identity = CreateProfileSectionPanel(parent, "ProfileIdentity", cream, 320, 334, -330f, -26f);
        GameObject summaryAvatar = CreateAnchoredPanel(identity.transform, "AvatarFrame", GetPrototypeFrameColor(), 150, 150, 0f, 70f);
        if (!TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 126f, 126f))
        {
            CreateAnchoredText(summaryAvatar.transform, GetPrototypeAvatarIcon(), 46, FontStyle.Bold, Color.white, 130, 80, 0f, 14f);
        }
        TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 150f, 150f);
        CreateAnchoredText(identity.transform, "Player Name", 16, FontStyle.Bold, purple, 260, 24, 0f, -8f);
        Text displayNameText = CreateAnchoredText(identity.transform, profileSettingsState.DisplayName, 30, FontStyle.Bold, purple, 280, 40, 0f, -42f);
        Text levelAndRankText = CreateAnchoredText(identity.transform, $"Level {profile.Level}\nRank: Aura TBD", 18, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 260, 58, 0f, -102f);

        GameObject progress = CreateProfileSectionPanel(parent, "ProfileProgress", new Color(0.12f, 0.06f, 0.18f), 640, 334, 190f, -26f);
        CreateProfileSectionTitle(progress.transform, "ACCOUNT SUMMARY", 24, gold, 560f, 132f);
        Text manaValueText = CreateProfileSummaryRow(progress.transform, "Mana", profile.Mana.ToString("N0"), -188f, 82f);
        Text crystalsValueText = CreateProfileSummaryRow(progress.transform, "Crystals", profile.Crystals.ToString("N0"), 188f, 82f);
        Text albumValueText = CreateProfileSummaryRow(progress.transform, "Album", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} Grimoire", -188f, 18f);
        Text bookValueText = CreateProfileSummaryRow(progress.transform, "Book", inventory.BookOfShadowsPurchased ? $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked", 188f, 18f);
        Text roomsValueText = CreateProfileSummaryRow(progress.transform, "Rooms", $"{inventory.GetRestoredRoomCount()} restored", -188f, -46f);
        Text auraValueText = CreateProfileSummaryRow(progress.transform, "Aura", "Strength formula TBD", 188f, -46f);
        Text auraRankNoteText = CreateAnchoredText(progress.transform, profile.AuraRankNoteText, 15, FontStyle.Bold, muted, 560, 42, 0f, -116f);

        Button rank = CreateAnchoredButton(parent, "Aura Ranks", 18, 210, 42, new Color(0.35f, 0.12f, 0.62f), -330f, -218f);
        PlayerProfileSummaryTabView summaryView = parent.gameObject.AddComponent<PlayerProfileSummaryTabView>();
        summaryView.ResetRuntimeBindings();
        summaryView.Initialize(
            displayNameText,
            levelAndRankText,
            auraRankNoteText,
            manaValueText,
            crystalsValueText,
            albumValueText,
            bookValueText,
            roomsValueText,
            auraValueText,
            rank);
        summaryView.AuraRanksRequested += BuildRankInfoUi;
        summaryView.Apply(BuildPlayerProfileSummaryDisplayModel(profile));
    }

    private Text CreateProfileSummaryRow(Transform parent, string label, string value, float x, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"ProfileSummary_{label}", 286, 48, x, y);
        Text labelText = CreateAnchoredText(row.transform, label, 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 250, 18, 0f, 10f);
        Text valueText = CreateAnchoredText(row.transform, value, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 22, 0f, -10f);
        PlayerProfileSummaryRowView rowView = row.GetComponent<PlayerProfileSummaryRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<PlayerProfileSummaryRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, valueText);
        rowView.Apply(label, value);
        return rowView.ValueText;
    }

    private void BuildPlayerAvatarTab(Transform parent, PlayerProfileState profile)
    {
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        GameObject preview = CreateProfileSectionPanel(parent, "AvatarPreview", new Color(0.12f, 0.06f, 0.18f), 360, 360, -300f, -30f);
        GameObject previewAvatar = CreateAnchoredPanel(preview.transform, "AvatarFramePreview", GetPrototypeFrameColor(), 190, 190, 0f, 58f);
        if (!TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 160f, 160f))
        {
            CreateAnchoredText(previewAvatar.transform, GetPrototypeAvatarIcon(), 58, FontStyle.Bold, Color.white, 160, 100, 0f, 16f);
        }
        TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 190f, 190f);
        Text avatarNameText = CreateAnchoredText(preview.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, 25, FontStyle.Bold, gold, 300, 34, 0f, -54f);
        Text cosmeticsSummaryText = CreateAnchoredText(preview.transform, $"{PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName}\n{PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName} dauber", 17, FontStyle.Bold, muted, 300, 58, 0f, -106f);

        GameObject controls = CreateProfileSectionPanel(parent, "AvatarControls", new Color(0.96f, 0.86f, 0.62f), 560, 360, 190f, -30f);
        CreateProfileSectionTitle(controls.transform, "COSMETICS", 26, new Color(0.18f, 0.08f, 0.32f), 480f, 132f);
        (Text avatarValueText, Button avatarNextButton) = CreateCosmeticSelectorRow(controls.transform, "Avatar", PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, -6f, 70f, CyclePrototypeAvatar);
        (Text frameValueText, Button frameNextButton) = CreateCosmeticSelectorRow(controls.transform, "Frame", PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName, -6f, 6f, CyclePrototypeFrame);
        (Text dauberValueText, Button dauberNextButton) = CreateCosmeticSelectorRow(controls.transform, "Dauber", PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName, -6f, -58f, CyclePrototypeDauber);
        Text footnoteText = CreateAnchoredText(controls.transform, "Avatar unlock rules, rank cosmetics, and custom avatar behavior are placeholders only.", 14, FontStyle.Bold, new Color(0.34f, 0.26f, 0.34f), 480, 42, 0f, -128f);

        PlayerAvatarTabView avatarTabView = parent.gameObject.AddComponent<PlayerAvatarTabView>();
        avatarTabView.ResetRuntimeBindings();
        avatarTabView.Initialize(
            avatarNameText,
            cosmeticsSummaryText,
            avatarValueText,
            frameValueText,
            dauberValueText,
            footnoteText,
            avatarNextButton,
            frameNextButton,
            dauberNextButton);
        avatarTabView.AvatarNextRequested += CyclePrototypeAvatar;
        avatarTabView.FrameNextRequested += CyclePrototypeFrame;
        avatarTabView.DauberNextRequested += CyclePrototypeDauber;
        avatarTabView.Apply(BuildPlayerAvatarTabDisplayModel());
    }

    private (Text valueText, Button nextButton) CreateCosmeticSelectorRow(Transform parent, string label, string value, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateProfileOptionRowPanel(parent, $"Cosmetic_{label}", 480f, 48f, x, y);
        Text labelText = CreateProfileOptionRowLabel(row.transform, label, 16, new Color(1f, 0.9f, 0.32f), 118, 22, -164f);
        Text valueText = CreateProfileOptionRowValue(row.transform, value, 16, Color.white, 210, 24, -18f);
        Button next = CreateAnchoredButton(row.transform, "Next", 13, 82, 28, new Color(0.35f, 0.12f, 0.62f), 184f, 0f);
        ProfileSelectorRowView rowView = row.GetComponent<ProfileSelectorRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<ProfileSelectorRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, valueText, next);
        rowView.Apply(new ProfileSelectorRowDisplayModel
        {
            LabelText = label,
            ValueText = value,
            ActionButtonText = "Next"
        });
        rowView.ActionRequested += () => action?.Invoke();
        return (valueText, next);
    }

    private bool TryCreateCosmeticSprite(Transform parent, string assetKey, string objectName, float width, float height)
    {
        Sprite sprite = CosmeticSpriteResolver.Load(assetKey);
        if (sprite == null)
        {
            return false;
        }

        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;

        Image image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return true;
    }

    private void BuildPlayerSettingsTab(Transform parent)
    {
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        GameObject account = CreateProfileSectionPanel(parent, "ProfileAccountSettings", new Color(0.12f, 0.06f, 0.18f), 460, 360, -250f, -30f);
        CreateProfileSectionHeader(account.transform, "ACCOUNT", 25, gold, 380f, 132f);
        profileDisplayNameInput = CreateAnchoredInputField(account.transform, "ProfileDisplayNameInput", "Display name", 360, 44, 0f, 84f);
        profileDisplayNameInput.text = profileSettingsState.DisplayName;
        profileDisplayNameInput.characterLimit = ProfileDisplayNameValidator.BetaMaximumLength;
        profileDisplayNameInput.lineType = InputField.LineType.SingleLine;
        profileDisplayNameInput.textComponent.alignment = TextAnchor.MiddleLeft;
        Button saveName = CreateAnchoredButton(account.transform, "Save Display Name", 16, 220, 38, new Color(0.12f, 0.55f, 0.08f), 0f, 38f);
        profileDisplayNameStatusText = CreateAnchoredText(account.transform, lastProfileDisplayNameStatus, 12, FontStyle.Bold, muted, 380, 34, 0f, -2f);
        CreateProfileSectionHeader(account.transform, "LOGIN PLACEHOLDERS", 14, gold, 380f, -42f, 22f);
        CreateCompactLoginOptionButton(account.transform, "Google", -125f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Facebook", 0f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Apple", 125f, -78f);
        CreateProfileSectionNote(account.transform, "Name validation is local Beta/test only. Login, uniqueness, and moderation require backend approval.", 12, muted, 390, 48, 0f, -132f);

        GameObject preferences = CreateProfileSectionPanel(parent, "ProfilePreferences", new Color(0.96f, 0.86f, 0.62f), 460, 360, 250f, -30f);
        CreateProfileSectionHeader(preferences.transform, "PREFERENCES", 25, new Color(0.18f, 0.08f, 0.32f), 380f, 132f);
        (Button soundToggleButton, Text soundToggleText) = CreatePreferenceToggle(preferences.transform, "Sound", prototypeSoundEnabled, 58f, TogglePrototypeSound);
        (Button notificationsToggleButton, Text notificationsToggleText) = CreatePreferenceToggle(preferences.transform, "Notifications", prototypeNotificationsEnabled, -8f, TogglePrototypeNotifications);
        CreateProfileSectionNote(preferences.transform, "Cosmetics and preferences save locally. Name editing and platform permissions wire up later.", 14, new Color(0.34f, 0.26f, 0.34f), 380, 58, 0f, -112f);

        PlayerSettingsTabView settingsTabView = parent.gameObject.AddComponent<PlayerSettingsTabView>();
        settingsTabView.ResetRuntimeBindings();
        settingsTabView.Initialize(
            profileDisplayNameInput,
            profileDisplayNameStatusText,
            saveName,
            soundToggleButton,
            soundToggleText,
            notificationsToggleButton,
            notificationsToggleText);
        settingsTabView.SaveNameRequested += SavePrototypeDisplayName;
        settingsTabView.SoundToggleRequested += TogglePrototypeSound;
        settingsTabView.NotificationsToggleRequested += TogglePrototypeNotifications;
        settingsTabView.Apply(BuildPlayerSettingsTabDisplayModel());
    }

    private void CreateCompactLoginOptionButton(Transform parent, string provider, float x, float y)
    {
        Button button = CreateAnchoredButton(parent, provider, 13, 110, 32, new Color(0.18f, 0.16f, 0.28f), x, y);
        ProfileLoginOptionView view = button.gameObject.GetComponent<ProfileLoginOptionView>();
        if (view == null)
        {
            view = button.gameObject.AddComponent<ProfileLoginOptionView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(button);
        view.Apply(new ProfileLoginOptionDisplayModel
        {
            ProviderName = provider,
            IsAvailable = false
        });
    }

    private GameObject CreateProfileSectionPanel(Transform parent, string name, Color color, float width, float height, float x, float y)
    {
        return CreateAnchoredPanel(parent, name, color, width, height, x, y);
    }

    private Text CreateProfileSectionTitle(Transform parent, string text, int fontSize, Color color, float width, float y, float height = 34f)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, (int)width, (int)height, 0f, y);
    }

    private Text CreateProfileSectionHeader(Transform parent, string text, int fontSize, Color color, float width, float y, float height = 34f)
    {
        Text title = CreateProfileSectionTitle(parent, string.Empty, fontSize, color, width, y, height);
        ProfileSectionHeaderView view = title.gameObject.GetComponent<ProfileSectionHeaderView>();
        if (view == null)
        {
            view = title.gameObject.AddComponent<ProfileSectionHeaderView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(title);
        view.Apply(new ProfileSectionHeaderDisplayModel
        {
            TitleText = text
        });
        return title;
    }

    private Text CreateProfileSectionNote(Transform parent, string text, int fontSize, Color color, int width, int height, float x, float y)
    {
        Text note = CreateAnchoredText(parent, string.Empty, fontSize, FontStyle.Bold, color, width, height, x, y);
        ProfileSectionNoteView view = note.gameObject.GetComponent<ProfileSectionNoteView>();
        if (view == null)
        {
            view = note.gameObject.AddComponent<ProfileSectionNoteView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(note);
        view.Apply(new ProfileSectionNoteDisplayModel
        {
            NoteText = text
        });
        return note;
    }

    private void SavePrototypeDisplayName()
    {
        DisplayNameValidationResult result = ProfileDisplayNameValidator.ValidateBeta(profileDisplayNameInput?.text);
        lastProfileDisplayNameStatus = result.Message;
        if (profileDisplayNameStatusText != null)
        {
            profileDisplayNameStatusText.text = result.Message;
            profileDisplayNameStatusText.color = result.IsValid
                ? new Color(0.55f, 1f, 0.55f)
                : new Color(1f, 0.58f, 0.58f);
        }

        if (!result.IsValid)
        {
            return;
        }

        profileSettingsState.DisplayName = result.NormalizedName;
        if (profileDisplayNameInput != null)
        {
            profileDisplayNameInput.text = result.NormalizedName;
        }

        PersistProfileSettings("display_name_changed");
    }

    private (Button button, Text valueText) CreatePreferenceToggle(Transform parent, string label, bool enabled, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateProfileOptionRowPanel(parent, $"Preference_{label}", 360f, 50f, 0f, y);
        Text labelText = CreateProfileOptionRowLabel(row.transform, label, 18, Color.white, 170, 24, -72f);
        Button toggle = CreateAnchoredButton(row.transform, enabled ? "ON" : "OFF", 16, 92, 30, enabled ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.42f, 0.12f, 0.18f), 112f, 0f);
        ProfileToggleRowView rowView = row.GetComponent<ProfileToggleRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<ProfileToggleRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, toggle);
        rowView.Apply(new ProfileToggleRowDisplayModel
        {
            LabelText = label,
            ToggleText = enabled ? "ON" : "OFF"
        });
        rowView.ToggleRequested += () => action?.Invoke();
        Text valueText = toggle.GetComponentInChildren<Text>();
        return (toggle, valueText);
    }

    private GameObject CreateProfileOptionRowPanel(Transform parent, string name, float width, float height, float x, float y)
    {
        return CreateAnchoredPanel(parent, name, new Color(0.18f, 0.13f, 0.2f), width, height, x, y);
    }

    private Text CreateProfileOptionRowLabel(Transform parent, string text, int fontSize, Color color, int width, int height, float x)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, 0f);
    }

    private Text CreateProfileOptionRowValue(Transform parent, string text, int fontSize, Color color, int width, int height, float x)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, 0f);
    }

    private Button CreateDenReturnFooter(Transform parent, float x, float y, UnityEngine.Events.UnityAction action)
    {
        Button close = CreateAnchoredButton(parent, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), x, y);
        UIFooterActionView footerView = close.gameObject.GetComponent<UIFooterActionView>();
        if (footerView == null)
        {
            footerView = close.gameObject.AddComponent<UIFooterActionView>();
        }

        footerView.ResetRuntimeBindings();
        footerView.Initialize(close);
        if (action != null)
        {
            footerView.Requested += () => action();
        }

        footerView.Apply(new UIFooterActionDisplayModel
        {
            ButtonText = "Back to Den",
            Interactable = close.interactable
        });

        return close;
    }

    private void CreateDualFooterNavigation(
        Transform parent,
        string backLabel,
        float y,
        UnityEngine.Events.UnityAction backAction,
        UnityEngine.Events.UnityAction closeAction,
        string closeLabel = "Back to Den")
    {
        Button back = CreateAnchoredButton(parent, backLabel, 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, y);
        Button close = CreateAnchoredButton(parent, closeLabel, 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, y);
        UIDualFooterNavigationView footerView = parent.gameObject.GetComponent<UIDualFooterNavigationView>();
        if (footerView == null)
        {
            footerView = parent.gameObject.AddComponent<UIDualFooterNavigationView>();
        }

        footerView.ResetRuntimeBindings();
        footerView.Initialize(back, close);
        if (backAction != null)
        {
            footerView.BackRequested += () => backAction();
        }

        if (closeAction != null)
        {
            footerView.CloseRequested += () => closeAction();
        }

        footerView.Apply(new UIDualFooterNavigationDisplayModel
        {
            BackButtonText = backLabel,
            BackButtonInteractable = back.interactable,
            CloseButtonText = closeLabel,
            CloseButtonInteractable = close.interactable
        });
    }

    private GameObject CreateModalShell(string name, float width, float height, Color color)
    {
        return CreateAnchoredPanel(contentRoot, name, color, width, height, 0f, -18f);
    }

    private Text CreateModalTitle(Transform parent, string text, Color color, float width = 820f, float y = 274f)
    {
        return CreateAnchoredText(parent, text, 44, FontStyle.Bold, color, width, 58, 0f, y);
    }

    private Text CreateModalSubtitle(Transform parent, string text, Color color, float width = 980f, float height = 34f, float y = 232f, int fontSize = 15)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, 0f, y);
    }

    private Text CreateModalSectionHeader(Transform parent, string text, Color color, float width, float x, float y, int fontSize = 23, float height = 32f)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, y);
    }

    private UIModalChromeView CreateModalChrome(
        GameObject panel,
        string title,
        string subtitle,
        string closeLabel,
        Color titleColor,
        Color subtitleColor,
        Color closeButtonColor,
        float titleWidth = 820f,
        float titleY = 274f,
        float subtitleWidth = 980f,
        float subtitleHeight = 34f,
        float subtitleY = 232f,
        int subtitleFontSize = 15,
        float closeWidth = 220f,
        float closeHeight = 48f,
        float closeX = 0f,
        float closeY = -286f,
        int closeFontSize = 20)
    {
        Text titleText = CreateModalTitle(panel.transform, title, titleColor, titleWidth, titleY);
        Text subtitleText = CreateModalSubtitle(panel.transform, subtitle, subtitleColor, subtitleWidth, subtitleHeight, subtitleY, subtitleFontSize);
        Button closeButton = CreateAnchoredButton(panel.transform, closeLabel, closeFontSize, closeWidth, closeHeight, closeButtonColor, closeX, closeY);
        Text closeButtonText = closeButton.GetComponentInChildren<Text>();

        UIModalController modalController = panel.GetComponent<UIModalController>();
        if (modalController == null)
        {
            modalController = panel.AddComponent<UIModalController>();
        }

        closeButton.onClick.RemoveListener(modalController.RequestClose);
        closeButton.onClick.AddListener(modalController.RequestClose);

        UIModalChromeView chromeView = panel.GetComponent<UIModalChromeView>();
        if (chromeView == null)
        {
            chromeView = panel.AddComponent<UIModalChromeView>();
        }

        chromeView.ResetRuntimeBindings();
        chromeView.Initialize(titleText, subtitleText, closeButton, closeButtonText, modalController);
        chromeView.Apply(title, subtitle, closeLabel);
        return chromeView;
    }

    private GameObject CreateModalSectionPanel(Transform parent, string name, float width, float height, float x, float y, Color? colorOverride = null)
    {
        return CreateAnchoredPanel(parent, name, colorOverride ?? new Color(0.1f, 0.045f, 0.14f), width, height, x, y);
    }

    private GameObject CreateModalInfoCard(Transform parent, string name, float width, float height, float x, float y, Color? colorOverride = null)
    {
        return CreateAnchoredPanel(parent, name, colorOverride ?? new Color(0.96f, 0.86f, 0.62f), width, height, x, y);
    }

    private PlayerProfileSummaryDisplayModel BuildPlayerProfileSummaryDisplayModel(PlayerProfileState profile)
    {
        return new PlayerProfileSummaryDisplayModel(
            profileSettingsState.DisplayName,
            $"Level {profile.Level}\nRank: Aura TBD",
            profile.AuraRankNoteText,
            profile.Mana.ToString("N0"),
            profile.Crystals.ToString("N0"),
            $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} Grimoire",
            inventory.BookOfShadowsPurchased ? $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked",
            $"{inventory.GetRestoredRoomCount()} restored",
            "Strength formula TBD");
    }

    private PlayerProfileModalDisplayModel BuildPlayerProfileModalDisplayModel(PlayerProfileState profile)
    {
        return new PlayerProfileModalDisplayModel(
            "PLAYER PROFILE",
            $"Level {profile.Level}  |  {profile.LevelProgressText}  |  {profile.AuraStrengthText}",
            "Back to Den");
    }

    private PlayerAvatarTabDisplayModel BuildPlayerAvatarTabDisplayModel()
    {
        return new PlayerAvatarTabDisplayModel(
            PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName,
            $"{PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName}\n{PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName} dauber",
            PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName,
            PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName,
            PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName,
            "Avatar unlock rules, rank cosmetics, and custom avatar behavior are placeholders only.");
    }

    private PlayerSettingsTabDisplayModel BuildPlayerSettingsTabDisplayModel()
    {
        return new PlayerSettingsTabDisplayModel(
            profileSettingsState.DisplayName,
            lastProfileDisplayNameStatus,
            prototypeSoundEnabled,
            prototypeNotificationsEnabled);
    }

    private string GetPrototypeAvatarIcon()
    {
        if (selectedPrototypeAvatarIndex == 1) return "SUN";
        if (selectedPrototypeAvatarIndex == 2) return "SEER";
        if (selectedPrototypeAvatarIndex == 3) return "STAR";
        return "MOON";
    }

    private Color GetPrototypeFrameColor()
    {
        if (selectedPrototypeFrameIndex == 1) return new Color(0.35f, 0.12f, 0.62f);
        if (selectedPrototypeFrameIndex == 2) return new Color(0.12f, 0.42f, 0.18f);
        if (selectedPrototypeFrameIndex == 3) return new Color(0.12f, 0.25f, 0.52f);
        return new Color(0.78f, 0.48f, 0.06f);
    }

    private void CyclePrototypeAvatar()
    {
        selectedPrototypeAvatarIndex = (selectedPrototypeAvatarIndex + 1) % PrototypeAvatars.Count;
        PersistProfileSettings("avatar_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void CyclePrototypeFrame()
    {
        selectedPrototypeFrameIndex = (selectedPrototypeFrameIndex + 1) % PrototypeAvatarFrames.Count;
        PersistProfileSettings("frame_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void CyclePrototypeDauber()
    {
        selectedPrototypeDauberIndex = (selectedPrototypeDauberIndex + 1) % PrototypeDaubers.Count;
        PersistProfileSettings("dauber_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void TogglePrototypeSound()
    {
        prototypeSoundEnabled = !prototypeSoundEnabled;
        PersistProfileSettings("sound_preference_changed");
        BuildPlayerProfileUi(PlayerProfileTabIds.Settings);
    }

    private void TogglePrototypeNotifications()
    {
        prototypeNotificationsEnabled = !prototypeNotificationsEnabled;
        PersistProfileSettings("notification_preference_changed");
        BuildPlayerProfileUi(PlayerProfileTabIds.Settings);
    }

    private void ApplyProfileSettings(ProfileSettingsState state)
    {
        profileSettingsState = state ?? ProfileSettingsState.CreateDefault();
        selectedPrototypeAvatarIndex = GetCosmeticIndex(profileSettingsState.AvatarId, PrototypeAvatars);
        selectedPrototypeFrameIndex = GetCosmeticIndex(profileSettingsState.FrameId, PrototypeAvatarFrames);
        selectedPrototypeDauberIndex = GetCosmeticIndex(profileSettingsState.DauberId, PrototypeDaubers);
        prototypeSoundEnabled = profileSettingsState.SoundEnabled;
        prototypeNotificationsEnabled = profileSettingsState.NotificationsEnabled;
    }

    private void PersistProfileSettings(string actionType)
    {
        profileSettingsState = new ProfileSettingsState
        {
            DisplayName = profileSettingsState.DisplayName,
            AvatarId = PrototypeAvatars[Mathf.Clamp(selectedPrototypeAvatarIndex, 0, PrototypeAvatars.Count - 1)].Id,
            FrameId = PrototypeAvatarFrames[Mathf.Clamp(selectedPrototypeFrameIndex, 0, PrototypeAvatarFrames.Count - 1)].Id,
            DauberId = PrototypeDaubers[Mathf.Clamp(selectedPrototypeDauberIndex, 0, PrototypeDaubers.Count - 1)].Id,
            SoundEnabled = prototypeSoundEnabled,
            NotificationsEnabled = prototypeNotificationsEnabled
        };

        if (profileSettingsPersistence != null)
        {
            profileSettingsPersistence.Save(profileSettingsState, actionType);
        }
    }

    private static int GetCosmeticIndex(string id, IReadOnlyList<CosmeticDefinition> definitions)
    {
        for (int index = 0; index < definitions.Count; index++)
        {
            if (string.Equals(definitions[index].Id, id, System.StringComparison.Ordinal))
            {
                return index;
            }
        }

        return 0;
    }

    private void RemovePlayerProfileModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("PlayerProfileModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        playerProfileModalView = null;
    }

    private void BuildRankInfoUi()
    {
        RemoveRankInfoModal();

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        GameObject panel = CreateModalShell("RankInfoModal", 960, 660, new Color(0.055f, 0.025f, 0.095f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.86f, 0.82f, 0.95f);
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color ink = new Color(0.18f, 0.08f, 0.32f);

        CreateModalTitle(panel.transform, "AURA RANKS", gold, 760f);
        CreateAnchoredText(panel.transform, $"Level {profile.Level}  |  {profile.LevelProgressText}", 30, FontStyle.Bold, Color.white, 760, 46, 0f, 218f);
        CreateAnchoredText(panel.transform, $"{profile.AuraStrengthText}  |  Rank thresholds TBD", 20, FontStyle.Bold, muted, 760, 34, 0f, 178f);

        GameObject note = CreateAnchoredPanel(panel.transform, "RankBenefitsNote", new Color(0.12f, 0.06f, 0.2f), 820, 60, 0f, 126f);
        CreateAnchoredText(note.transform, "Rank is now Aura-derived. Level is only one Aura input.", 18, FontStyle.Bold, muted, 780, 24, 0f, 10f);
        CreateAnchoredText(note.transform, "Purchases may contribute only as a small capped support signal.", 14, FontStyle.Bold, muted, 780, 22, 0f, -14f);

        CreateAnchoredText(panel.transform, "RANK TITLES - AURA THRESHOLDS TBD", 22, FontStyle.Bold, gold, 760, 30, 0f, 76f);
        for (int index = 0; index < LockedRankLadder.Length; index++)
        {
            int col = index < 7 ? 0 : 1;
            int row = col == 0 ? index : index - 7;
            float x = col == 0 ? -220f : 220f;
            float y = 32f - row * 42f;
            GameObject rankRow = CreateAnchoredPanel(panel.transform, $"RankInfo_{index}", cream, 360, 36, x, y);
            CreateAnchoredText(rankRow.transform, LockedRankLadder[index], 16, FontStyle.Bold, ink, 330, 24, 0f, 0f);
        }

        CreateDenReturnFooter(panel.transform, 0f, -286f, RemoveRankInfoModal);
    }

    private void CreateDenFeatureTile(string title, string subtitle, float x, float y)
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, $"DenFeature_{title}", new Color(0.15f, 0.08f, 0.22f), 280, 86, x, y);
        CreateAnchoredText(tile.transform, title, 23, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 250, 28, 0f, 18f);
        CreateAnchoredText(tile.transform, subtitle, 14, FontStyle.Bold, new Color(0.85f, 0.82f, 0.95f), 250, 28, 0f, -18f);
    }

    private GameObject CreateDenDoorTile(string title, string subtitle, float x, float y, float width, float height, Color color)
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, $"DenDoor_{title}", color, width, height, x, y);
        Button button = tile.GetComponent<Button>();
        if (button == null)
        {
            button = tile.AddComponent<Button>();
        }

        button.targetGraphic = tile.GetComponent<Image>();

        Text titleText = CreateAnchoredText(tile.transform, string.Empty, 28, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, height * 0.48f, 0f, height * 0.14f);
        Text subtitleText = CreateAnchoredText(tile.transform, string.Empty, 15, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), width - 26, 28, 0f, -height * 0.28f);
        PlayerDenDoorView doorView = tile.GetComponent<PlayerDenDoorView>();
        if (doorView == null)
        {
            doorView = tile.AddComponent<PlayerDenDoorView>();
        }

        doorView.ResetRuntimeBindings();
        doorView.Initialize(titleText, subtitleText, button);
        doorView.Apply(new PlayerDenDoorDisplayModel
        {
            TitleText = title,
            SubtitleText = subtitle
        });
        return tile;
    }

    private void BuildApothecaryUi()
    {
        RemoveApothecaryModal();
        GameObject panel = CreateModalShell("ApothecaryModal", 1120, 660, new Color(0.045f, 0.035f, 0.075f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "APOTHECARY",
            "Potion-only prototype surface. Crafting costs, recipes, upgrades, and reward tuning are not active in this pass.",
            "Back to Den",
            gold,
            muted,
            new Color(0.35f, 0.12f, 0.62f));
        chromeView.CloseRequested += RemoveApothecaryModal;

        CreateApothecarySection(panel.transform, "Active Potion", -344f, 82f, 360, 252, new Color(0.11f, 0.07f, 0.16f));
        CreateAnchoredText(panel.transform, room.PotionName, 26, FontStyle.Bold, gold, 310, 64, -344f, 126f);
        CreateAnchoredText(panel.transform, room.Name, 16, FontStyle.Bold, muted, 300, 24, -344f, 74f);
        // The full ingredient list lives in the Potion Ingredients panel; repeating it here overflowed into the row below.
        CreateAnchoredText(panel.transform, inventory.GetRestoreStatusText(), 24, FontStyle.Bold, inventory.CanRestoreActiveRoom() ? new Color(0.46f, 0.9f, 0.34f) : Color.white, 300, 32, -344f, 18f);

        CreateApothecarySection(panel.transform, "Potion Ingredients", 84f, 82f, 568, 252, new Color(0.1f, 0.055f, 0.13f));
        CreateAnchoredText(panel.transform, inventory.GetFullIngredientProgressText(), 19, FontStyle.Bold, Color.white, 500, 156, 84f, 64f);
        CreateAnchoredText(panel.transform, "Ingredient collection still comes from room play and ingredient detail flows.", 14, FontStyle.Bold, muted, 500, 36, 84f, -20f);

        // The lower row stops above the shared Back button (y -286), leaving a band for the footer note.
        CreateApothecarySection(panel.transform, "Completion Reward", -280f, -145f, 430, 170, new Color(0.13f, 0.075f, 0.11f));
        CreateAnchoredText(panel.transform, inventory.GetRestoreRewardText(), 28, FontStyle.Bold, gold, 360, 76, -280f, -135f);
        CreateAnchoredText(panel.transform, "Reward values remain prototype placeholders until potion rewards are approved.", 14, FontStyle.Bold, muted, 380, 40, -280f, -200f);

        CreateApothecarySection(panel.transform, "Potion Workbench", 292f, -145f, 430, 170, new Color(0.11f, 0.07f, 0.14f));
        CreateApothecaryWorkbenchSlot(panel.transform, "Recipe", "Locked", 166f, -140f);
        CreateApothecaryWorkbenchSlot(panel.transform, "Ingredient", "Later", 292f, -140f);
        CreateApothecaryWorkbenchSlot(panel.transform, "Output", "Later", 418f, -140f);
        CreateAnchoredText(panel.transform, "Mixing and crafting stay configurable until potion rules are approved.", 14, FontStyle.Bold, muted, 380, 36, 292f, -208f);

        CreateAnchoredText(panel.transform, "Sigils, Clairvoyance, Pandora, and other boost inventory stay in Cabinet of Curiosities.", 14, FontStyle.Bold, muted, 900, 24, 0f, -246f);
    }

    private void CreateApothecarySection(Transform parent, string title, float x, float y, float width, float height, Color color)
    {
        GameObject section = CreateAnchoredPanel(parent, $"Apothecary_{title}", color, width, height, x, y);
        CreateAnchoredText(section.transform, title, 18, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 28, 24, 0f, height * 0.5f - 28f);
    }

    private void CreateApothecaryWorkbenchSlot(Transform parent, string title, string state, float x, float y)
    {
        GameObject slot = CreateAnchoredPanel(parent, $"ApothecarySlot_{title}", new Color(0.22f, 0.18f, 0.22f), 104, 82, x, y);
        CreateAnchoredText(slot.transform, title, 13, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 92, 18, 0f, 20f);
        CreateAnchoredText(slot.transform, state, 16, FontStyle.Bold, new Color(0.76f, 0.72f, 0.82f), 92, 28, 0f, -12f);
    }

    private void RemoveApothecaryModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("ApothecaryModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildRelicWallUi()
    {
        RemoveRelicWallModal();
        GameObject panel = CreateModalShell("RelicWallModal", 1120, 660, new Color(0.055f, 0.025f, 0.095f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "RELIC WALL",
            "Badge display shell for rank, collection, realm, and event accomplishments. Rewards and achievement rules are not active yet.",
            "Back to Den",
            gold,
            muted,
            new Color(0.35f, 0.12f, 0.62f));
        chromeView.CloseRequested += RemoveRelicWallModal;

        CreateRelicSection(panel.transform, "Identity", -360f, 66f);
        CreateRelicBadge(panel.transform, "Aura Rank", "TBD", true, -470f, 66f);
        CreateRelicBadge(panel.transform, "Avatar Frame", PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName, true, -360f, 66f);
        CreateRelicBadge(panel.transform, "Dauber", PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName, true, -250f, 66f);

        CreateRelicSection(panel.transform, "Collections", 0f, 66f);
        CreateRelicBadge(panel.transform, "Grimoire", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards}", inventory.GetOwnedGrimoireCardCount() > 0, -110f, 66f);
        CreateRelicBadge(panel.transform, "Book Shadows", inventory.BookOfShadowsPurchased ? "Active" : "Locked", inventory.BookOfShadowsPurchased, 0f, 66f);
        CreateRelicBadge(panel.transform, "Duplicates", $"{inventory.GetGrimoireDuplicateCount()} extra", inventory.GetGrimoireDuplicateCount() > 0, 110f, 66f);

        CreateRelicSection(panel.transform, "Realms", 360f, 66f);
        CreateRelicBadge(panel.transform, "Rooms", $"{inventory.GetRestoredRoomCount()} restored", inventory.GetRestoredRoomCount() > 0, 250f, 66f);
        CreateRelicBadge(panel.transform, "Realm Path", $"R{RealmContentCatalog.ActivePrototypeRealmIndex + 1}", true, 360f, 66f);
        CreateRelicBadge(panel.transform, "Realm Complete", "Later", false, 470f, 66f);

        CreateRelicSection(panel.transform, "Social & Events", -180f, -164f);
        CreateRelicBadge(panel.transform, "Coven", coven.IsJoined ? "Joined" : "Join later", coven.IsJoined, -290f, -164f);
        CreateRelicBadge(panel.transform, "Daily Spin", inventory.CanClaimDailySpin() ? "Ready" : "Spun", !inventory.CanClaimDailySpin(), -180f, -164f);
        CreateRelicBadge(panel.transform, "Trail", "Later", false, -70f, -164f);

        CreateRelicSection(panel.transform, "Prestige", 250f, -164f);
        CreateRelicBadge(panel.transform, "Rank Gifts", "Later", false, 140f, -164f);
        CreateRelicBadge(panel.transform, "Event Crown", "Later", false, 250f, -164f);
        CreateRelicBadge(panel.transform, "Beta Badge", "Later", false, 360f, -164f);

        CreateAnchoredText(panel.transform, "Relic Wall is display-only in this pass. Badge criteria, rewards, rarity, and event rules remain open decisions.", 15, FontStyle.Bold, muted, 900, 34, 0f, -238f);
    }

    private void CreateRelicSection(Transform parent, string title, float x, float y)
    {
        CreateAnchoredText(parent, title, 21, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 330, 30, x, y + 92f);
    }

    private void CreateRelicBadge(Transform parent, string title, string detail, bool earned, float x, float y)
    {
        Color badgeColor = earned ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.22f, 0.2f, 0.24f);
        Color textColor = earned ? Color.white : new Color(0.68f, 0.64f, 0.72f);
        Color accentColor = earned ? new Color(1f, 0.9f, 0.32f) : new Color(0.42f, 0.38f, 0.46f);
        GameObject badge = CreateAnchoredPanel(parent, $"RelicBadge_{title}", badgeColor, 96, 112, x, y);
        Text accentText = CreateAnchoredText(badge.transform, earned ? "*" : "-", 34, FontStyle.Bold, accentColor, 70, 36, 0f, 26f);
        Text titleText = CreateAnchoredText(badge.transform, title, 12, FontStyle.Bold, textColor, 82, 30, 0f, -10f);
        Text detailText = CreateAnchoredText(badge.transform, detail, 10, FontStyle.Bold, textColor, 82, 24, 0f, -40f);
        UIBadgeCardView badgeView = badge.AddComponent<UIBadgeCardView>();
        badgeView.ResetRuntimeBindings();
        badgeView.Initialize(accentText, titleText, detailText);
        badgeView.Apply(earned ? "*" : "-", title, detail);
    }

    private void RemoveRelicWallModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("RelicWallModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private PlayerDenActionTileView CreateDenBottomButton(PlayerDenActionTileDisplayModel displayModel, float x)
    {
        string tileName = displayModel == null || string.IsNullOrEmpty(displayModel.TitleText) ? "DenBottom_Action" : $"DenBottom_{displayModel.TitleText}";
        GameObject tile = CreateAnchoredPanel(contentRoot, tileName, new Color(0.13f, 0.07f, 0.18f), 154, 86, x, -378f);
        Button button = tile.GetComponent<Button>();
        if (button == null)
        {
            button = tile.AddComponent<Button>();
        }

        button.targetGraphic = tile.GetComponent<Image>();

        Text titleText = CreateAnchoredText(tile.transform, string.Empty, 19, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 132, 28, 0f, 18f);
        Text subtitleText = CreateAnchoredText(tile.transform, string.Empty, 12, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 132, 24, 0f, -18f);
        PlayerDenActionTileView cardView = tile.GetComponent<PlayerDenActionTileView>();
        if (cardView == null)
        {
            cardView = tile.AddComponent<PlayerDenActionTileView>();
        }

        cardView.ResetRuntimeBindings();
        cardView.Initialize(titleText, subtitleText, button);
        cardView.Apply(displayModel);
        return cardView;
    }

    private PlayerDenFeatureTileView CreateDenFeatureTile(string name, Color tileColor, float width, float height, float x, float y, string title, string subtitle, string detail, string hint, int titleFontSize, Color titleColor, Color mutedColor)
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, name, tileColor, width, height, x, y);
        Button button = tile.GetComponent<Button>();
        if (button == null)
        {
            button = tile.AddComponent<Button>();
        }

        Text titleText = CreateAnchoredText(tile.transform, title, titleFontSize, FontStyle.Bold, titleColor, 330, 76, 0f, 62f);
        Text subtitleText = CreateAnchoredText(tile.transform, subtitle, 16, FontStyle.Bold, mutedColor, 320, 28, 0f, 2f);
        Text detailText = CreateAnchoredText(tile.transform, detail, 18, FontStyle.Bold, Color.white, 300, 32, 0f, -62f);
        Text hintText = CreateAnchoredText(tile.transform, hint, 13, FontStyle.Bold, mutedColor, 300, 24, 0f, -104f);

        PlayerDenFeatureTileView tileView = tile.GetComponent<PlayerDenFeatureTileView>();
        if (tileView == null)
        {
            tileView = tile.AddComponent<PlayerDenFeatureTileView>();
        }

        tileView.ResetRuntimeBindings();
        tileView.Initialize(titleText, subtitleText, detailText, hintText, button);
        tileView.Apply(title, subtitle, detail, hint);
        return tileView;
    }

    private void BuildPowerUpInventoryUi()
    {
        RemovePowerUpInventoryModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "PowerUpInventoryModal", new Color(0.09f, 0.035f, 0.14f, 0.985f), 1200, 680, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.85f, 0.82f, 0.95f);
        int totalSigils = inventory.GetInventoryRewardCount("Single Sigil")
            + inventory.GetInventoryRewardCount("Multi Sigil")
            + inventory.GetInventoryRewardCount("Arcane Spark")
            + inventory.GetInventoryRewardCount("Fortune Sigil")
            + inventory.GetInventoryRewardCount("Wild Sigil")
            + inventory.GetInventoryRewardCount("Presto Sigil");
        int totalCards = inventory.GetInventoryRewardCount("Regular Card")
            + inventory.GetInventoryRewardCount("Gilded Card")
            + inventory.GetInventoryRewardCount("Ancient Card")
            + inventory.GetInventoryRewardCount("Special Card");
        CreateAnchoredText(panel.transform, "CABINET OF CURIOSITIES", 40, FontStyle.Bold, gold, 1060, 54, 0f, 288f);
        CreateAnchoredText(panel.transform, "Storage for playable sigils, timed boosts, and Pandora. Cards live in Library; orbs live with Coven systems.", 17, FontStyle.Bold, muted, 1040, 28, 0f, 250f);

        CreateAnchoredPanel(panel.transform, "CabinetTotals", new Color(0.14f, 0.07f, 0.2f), 1060, 78, 0f, 194f);
        CreateCabinetStat(panel.transform, "Power-Ups", inventory.GetPowerUpText(), -330f, 194f);
        CreateCabinetStat(panel.transform, "Playable Sigils", totalSigils.ToString(), -110f, 194f);
        CreateCabinetStat(panel.transform, "Clairvoyance", inventory.ClairvoyanceMinutes + "m", 110f, 194f);
        CreateCabinetStat(panel.transform, "Pandora", inventory.GetInventoryRewardCount("Pandora Sigil").ToString(), 330f, 194f);

        CreateCabinetSection(panel.transform, "Timed Boost", -398f, 82f, 320, 190, new Color(0.13f, 0.06f, 0.22f));
        GameObject timedBoostCard = CreateAnchoredPanel(panel.transform, "CabinetTimedBoostCard", new Color(0f, 0f, 0f, 0f), 280, 96, -398f, 92f);
        Text timedBoostTitleText = CreateAnchoredText(timedBoostCard.transform, "Clairvoyance", 24, FontStyle.Bold, gold, 280, 30, 0f, 24f);
        Text timedBoostNoteText = CreateAnchoredText(timedBoostCard.transform, $"{inventory.ClairvoyanceMinutes}m stock\nActive: {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}", 20, FontStyle.Bold, Color.white, 280, 58, 0f, -24f);
        UITitleNoteRowView timedBoostView = timedBoostCard.GetComponent<UITitleNoteRowView>();
        if (timedBoostView == null)
        {
            timedBoostView = timedBoostCard.AddComponent<UITitleNoteRowView>();
        }

        timedBoostView.ResetRuntimeBindings();
        timedBoostView.Initialize(timedBoostTitleText, timedBoostNoteText);
        timedBoostView.Apply("Clairvoyance", $"{inventory.ClairvoyanceMinutes}m stock\nActive: {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}");
        Button clairvoyance = CreateAnchoredButton(panel.transform, inventory.CanActivateClairvoyance() ? "Activate 15m" : "No Stock", 17, 220, 38, inventory.CanActivateClairvoyance() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), -398f, 4f);
        clairvoyance.interactable = inventory.CanActivateClairvoyance();
        clairvoyance.onClick.AddListener(ActivatePrototypeClairvoyanceFromInventory);

        CreateCabinetSection(panel.transform, "Playable Sigils", 68f, 48f, 560, 258, new Color(0.12f, 0.05f, 0.19f));
        CreateCabinetInventoryRow(panel.transform, "Single Sigil", "Random daub on each active card", inventory.GetInventoryRewardCount("Single Sigil"), -118f, 118f);
        CreateCabinetInventoryRow(panel.transform, "Multi Sigil", "Two random daubs on each active card", inventory.GetInventoryRewardCount("Multi Sigil"), 254f, 118f);
        CreateCabinetInventoryRow(panel.transform, "Arcane Spark", "Lightning daubs across active cards", inventory.GetInventoryRewardCount("Arcane Spark"), -118f, 58f);
        CreateCabinetInventoryRow(panel.transform, "Fortune Sigil", "Doubles active card winnings", inventory.GetInventoryRewardCount("Fortune Sigil"), 254f, 58f);
        CreateCabinetInventoryRow(panel.transform, "Wild Sigil", "Premium choose-any-square sigil", inventory.GetInventoryRewardCount("Wild Sigil"), -118f, -2f);
        CreateCabinetInventoryRow(panel.transform, "Presto Sigil", "Turns its daub into an extra bingo", inventory.GetInventoryRewardCount("Presto Sigil"), 254f, -2f);

        CreateCabinetSection(panel.transform, "Curiosities", -398f, -196f, 320, 164, new Color(0.17f, 0.08f, 0.07f));
        CreateCabinetInventoryRow(panel.transform, "Pandora Sigil", "Chest that opens into power-ups", inventory.GetInventoryRewardCount("Pandora Sigil"), -398f, -178f);
        GameObject curiositiesNote = CreateAnchoredPanel(panel.transform, "CuriositiesNote", new Color(0f, 0f, 0f, 0f), 280, 48, -398f, -236f);
        Text curiositiesTitleText = CreateAnchoredText(curiositiesNote.transform, "Pandora", 15, FontStyle.Bold, gold, 280, 18, 0f, 10f);
        Text curiositiesBodyText = CreateAnchoredText(curiositiesNote.transform, "Stored here and opens during gameplay when loaded from the power-up bank.", 12, FontStyle.Bold, muted, 280, 30, 0f, -12f);
        UITitleNoteRowView curiositiesView = curiositiesNote.GetComponent<UITitleNoteRowView>();
        if (curiositiesView == null)
        {
            curiositiesView = curiositiesNote.AddComponent<UITitleNoteRowView>();
        }

        curiositiesView.ResetRuntimeBindings();
        curiositiesView.Initialize(curiositiesTitleText, curiositiesBodyText);
        curiositiesView.Apply("Pandora", "Stored here and opens during gameplay when loaded from the power-up bank.");

        CreateCabinetSection(panel.transform, "Other Destinations", 162f, -196f, 670, 164, new Color(0.13f, 0.08f, 0.15f));
        CreateCabinetDestinationRow(panel.transform, "Cards", "Open Library Grimoire", totalCards, 12f, -178f);
        CreateCabinetDestinationRow(panel.transform, "Club Orbs", "Open Coven Circle", inventory.GetInventoryRewardCount("Club Orbs"), 370f, -178f);
        CreateAnchoredText(panel.transform, "Cards now live in Library. Club Orbs will move to Covens when that view is built.", 15, FontStyle.Bold, muted, 600, 36, 162f, -236f);

        powerUpInventoryText = CreateAnchoredText(panel.transform, "", 1, FontStyle.Normal, Color.clear, 1, 1, 0f, 0f);

        CreateDenReturnFooter(panel.transform, 0f, -292f, BuildPlayerDenUi);
    }

    private void CreateCabinetStat(Transform parent, string label, string value, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CabinetStat_{label}", new Color(0f, 0f, 0f, 0f), 210, 54, x, y);
        Text valueText = CreateAnchoredText(row.transform, value, 24, FontStyle.Bold, Color.white, 210, 30, 0f, 10f);
        Text labelText = CreateAnchoredText(row.transform, label, 13, FontStyle.Bold, new Color(0.84f, 0.88f, 1f), 210, 18, 0f, -18f);
        LibraryStatView view = row.GetComponent<LibraryStatView>();
        if (view == null)
        {
            view = row.AddComponent<LibraryStatView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(valueText, labelText);
        view.Apply(new LibraryStatDisplayModel
        {
            LabelText = label,
            ValueText = value
        });
    }

    private void CreateCabinetSection(Transform parent, string title, float x, float y, float width, float height, Color color)
    {
        GameObject section = CreateAnchoredPanel(parent, $"Cabinet_{title}", color, width, height, x, y);
        Text titleText = CreateAnchoredText(section.transform, title.ToUpperInvariant(), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, 24, 0f, height * 0.5f - 20f);
        LibrarySectionView view = section.GetComponent<LibrarySectionView>();
        if (view == null)
        {
            view = section.AddComponent<LibrarySectionView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText);
        view.Apply(new LibrarySectionDisplayModel
        {
            TitleText = title.ToUpperInvariant()
        });
    }

    private void CreateCabinetInventoryRow(Transform parent, string title, string note, int count, float x, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"CabinetRow_{title}", 340, 46, x, y);
        Text titleText = CreateAnchoredText(row.transform, title, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 180, 22, -58f, 8f);
        Text noteText = CreateAnchoredText(row.transform, note, 11, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 210, 18, -42f, -13f);
        Text countText = CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 70, 34, 126f, 0f);
        UITitleNoteCountRowView rowView = row.AddComponent<UITitleNoteCountRowView>();
        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, noteText, countText);
        rowView.Apply(title, note, "x" + count);
    }

    private void CreateCabinetDestinationRow(Transform parent, string title, string note, int count, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CabinetRoute_{title}", new Color(0.18f, 0.13f, 0.2f), 340, 46, x, y);
        Text titleText = CreateAnchoredText(row.transform, title, 17, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 180, 22, -58f, 8f);
        Text noteText = CreateAnchoredText(row.transform, note, 11, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 210, 18, -42f, -13f);
        Text countText = CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, Color.white, 70, 34, 126f, 0f);
        UITitleNoteCountRowView rowView = row.GetComponent<UITitleNoteCountRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<UITitleNoteCountRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, noteText, countText);
        rowView.Apply(title, note, "x" + count);
    }

    private void RemovePowerUpInventoryModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("PowerUpInventoryModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveRankInfoModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("RankInfoModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildLobbyRestorePanel()
    {
        GameObject panel = CreateAnchoredPanel(contentRoot, "RestorePanel", new Color(0.96f, 0.86f, 0.62f), 980, 168, -245f, 118f);
        Color textColor = new Color(0.15f, 0.09f, 0.22f);
        Text potionNameText = CreateAnchoredText(panel.transform, RealmContentCatalog.ActivePrototypeRoom.PotionName, 22, FontStyle.Bold, textColor, 280, 30, -338f, 54f);
        Text hintText = CreateAnchoredText(panel.transform, "Collect ingredients while you play bingo.", 18, FontStyle.Bold, textColor, 300, 30, -338f, 22f);
        Text statusText = CreateAnchoredText(panel.transform, inventory.GetRestoreStatusText(), 21, FontStyle.Bold, inventory.CanRestoreActiveRoom() ? new Color(0.06f, 0.45f, 0.08f) : textColor, 280, 30, -338f, -12f);

        Button restore = CreateAnchoredButton(panel.transform, inventory.ActiveRoomRestored ? "Restored" : "Restore", 20, 220, 44, inventory.CanRestoreActiveRoom() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.35f, 0.12f, 0.62f), -338f, -56f);
        Text restoreButtonText = restore.GetComponentInChildren<Text>();

        Text ingredientsHeaderText = CreateAnchoredText(panel.transform, "Ingredients", 19, FontStyle.Bold, textColor, 360, 26, 22f, 58f);
        Text ingredientsBodyText = CreateAnchoredText(panel.transform, inventory.GetFullIngredientProgressText(), 16, FontStyle.Bold, textColor, 420, 112, 22f, -14f);
        Text restoreRewardText = CreateAnchoredText(panel.transform, $"Restore Reward\n{inventory.GetRestoreRewardText()}", 18, FontStyle.Bold, textColor, 180, 118, 388f, 0f);

        RoomEntryRestorePanelView restorePanelView = panel.AddComponent<RoomEntryRestorePanelView>();
        restorePanelView.ResetRuntimeBindings();
        restorePanelView.Initialize(
            potionNameText,
            hintText,
            statusText,
            ingredientsHeaderText,
            ingredientsBodyText,
            restoreRewardText,
            restore,
            restoreButtonText);
        restorePanelView.RestoreRequested += RestoreActiveRoom;
        roomEntryPanelView?.BindRestorePanel(restorePanelView);
        roomEntryPanelView?.ApplyRestorePanel(BuildRoomEntryRestorePanelDisplayModel());
    }

    private void CreateLobbyInfoTile(string title, string detail, float width, float height, float x, float y, Color color)
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, title, color, width, height, x, y);
        Color textColor = color.grayscale > 0.55f ? new Color(0.15f, 0.09f, 0.22f) : Color.white;
        CreateAnchoredText(tile.transform, title, 22, FontStyle.Bold, textColor, width - 26, 62, 0f, 42f);
        CreateAnchoredText(tile.transform, detail, 30, FontStyle.Bold, textColor, width - 26, 78, 0f, -34f);
    }

    private void CreateCardOptionAnchored(int cardCount, float x, float y)
    {
        int entryCost = cardCount * manaBetPerCard;
        bool canAfford = inventory.CanAffordMana(entryCost);
        Color fill = !canAfford
            ? new Color(0.5f, 0.48f, 0.44f)
            : cardCount == selectedCardCount ? new Color(1f, 0.86f, 0.46f) : new Color(0.96f, 0.88f, 0.68f);

        GameObject tile = CreateAnchoredPanel(contentRoot, $"{cardCount} Card Option", fill, 330, 244, x, y);
        Color textColor = new Color(0.14f, 0.08f, 0.24f);
        Text titleText = CreateAnchoredText(tile.transform, $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}", 34, FontStyle.Bold, textColor, 300, 42, 0f, 86f);
        Text flavorText = CreateAnchoredText(tile.transform, GetCardOptionFlavor(cardCount), 18, FontStyle.Bold, textColor, 290, 28, 0f, 48f);
        Text ingredientText = CreateAnchoredText(tile.transform, GetCardOptionIngredientText(cardCount), 18, FontStyle.Bold, textColor, 290, 30, 0f, 12f);
        Text chanceText = CreateAnchoredText(tile.transform, GetCardOptionChanceText(cardCount), 16, FontStyle.Bold, textColor, 290, 48, 0f, -28f);

        string buttonLabel = canAfford ? $"Play {entryCost} Mana" : $"Need {entryCost} Mana";
        Button play = CreateAnchoredButton(tile.transform, buttonLabel, 20, 260, 48, canAfford ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.36f, 0.36f, 0.36f), 0f, -88f);
        Text playText = play.GetComponentInChildren<Text>();

        RoomEntryCardOptionView optionView = tile.AddComponent<RoomEntryCardOptionView>();
        optionView.ResetRuntimeBindings();
        optionView.Initialize(
            tile.GetComponent<Image>(),
            titleText,
            flavorText,
            ingredientText,
            chanceText,
            play,
            playText);
        optionView.Apply(
            new RoomEntryCardOptionDisplayModel(
                cardCount,
                $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}",
                GetCardOptionFlavor(cardCount),
                GetCardOptionIngredientText(cardCount),
                GetCardOptionChanceText(cardCount),
                buttonLabel,
                canAfford,
                fill,
                canAfford ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.36f, 0.36f, 0.36f)));
        optionView.PlayRequested += BeginRoundFromLobby;
        roomEntryPanelView?.RegisterCardOption(optionView);
    }

    private string GetCardOptionFlavor(int cardCount)
    {
        if (IsBlackoutRoom())
        {
            return cardCount == 1 ? "One blackout chance" : $"{cardCount} blackout chances";
        }

        switch (cardCount)
        {
            case 1:
                return "Great for casual play";
            case 2:
                return "More chances, more fun";
            case 4:
                return "Best value for rewards";
            case 6:
                return "Maximum rewards";
            default:
                return "Choose cards to play";
        }
    }

    private string GetCardOptionIngredientText(int cardCount)
    {
        if (IsBlackoutRoom())
        {
            return $"Each blackout earns 1 wheelspin  |  {roomRules.GetBlackoutCallRuleText(cardCount, manaBetPerCard)}";
        }

        return $"Bingo ingredients guaranteed  |  {FormatPercent(GetActiveProgression().NoBingoIngredientDropChance)} no-bingo bonus";
    }

    private string GetCardOptionChanceText(int cardCount)
    {
        if (IsBlackoutRoom())
        {
            return $"Rewards {FormatPercent(GetActiveProgression().CellRewardChance)}  |  Potion x per blackout card";
        }

        return $"Special Card {GetSpecialCardChance(cardCount)}%  |  Rewards {FormatPercent(GetActiveProgression().CellRewardChance)}";
    }

    private int GetMaxIngredientTypesForCardCount(int cardCount)
    {
        if (cardCount <= 1) return 2;
        if (cardCount <= 2) return 3;
        if (cardCount <= 4) return 4;
        return 5;
    }

    private int GetSpecialCardChance(int cardCount)
    {
        if (cardCount <= 1) return 5;
        if (cardCount <= 2) return 8;
        if (cardCount <= 4) return 12;
        return 18;
    }

    private void BuildGameplayRail(Transform parent)
    {
        GameObject rail = CreatePanelInParent(parent, "GameplayRail", new Color(0.18f, 0.09f, 0.32f), 140, 540);
        VerticalLayoutGroup layout = rail.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(rail.transform, "CALLED BALL", 16, FontStyle.Bold, Color.white, 116, 26);
        calledNumberText = CreateTextInParent(rail.transform, roundFlow.History.Count > 0 ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}" : "Starting", 29, FontStyle.Bold, new Color(1f, 0.84f, 0.18f), 116, 48);
        timerText = CreateTextInParent(rail.transform, "Next call: --", 15, FontStyle.Bold, new Color(0.74f, 0.92f, 1f), 116, 26);
        CreateTextInParent(rail.transform, "RECENT", 15, FontStyle.Bold, Color.white, 116, 24);
        calledHistoryText = CreateTextInParent(rail.transform, roundFlow.History.Count > 0 ? GetCalledHistoryText() : "none", 13, FontStyle.Normal, new Color(0.84f, 0.88f, 1f), 116, 86);
        CreateTextInParent(rail.transform, "CARD VIEW\nAll Visible", 15, FontStyle.Bold, Color.white, 116, 58);
    }

    private void BuildGameplayCenter(Transform parent)
    {
        GameObject center = CreatePanelInParent(parent, "GameplayCenter", new Color(0f, 0f, 0f, 0f), 910, 540);
        VerticalLayoutGroup layout = center.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        bingoBannerText = CreateTextInParent(center.transform, "", 27, FontStyle.Bold, new Color(1f, 0.84f, 0.18f), 890, 34);
        BuildVisibleCards(center.transform, 890);
    }

    private void BuildGameplaySummary(Transform parent)
    {
        GameObject summary = CreatePanelInParent(parent, "GameplaySummary", new Color(0.22f, 0.1f, 0.34f), 140, 540);
        VerticalLayoutGroup layout = summary.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(summary.transform, "JACKPOT", 17, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 116, 30);
        CreateTextInParent(summary.transform, "5 Bingos\nany card", 16, FontStyle.Bold, Color.white, 116, 54);
        CreateTextInParent(summary.transform, "STATUS", 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 116, 28);
        roundSummaryText = CreateTextInParent(summary.transform, GetRoundSummaryText(), 14, FontStyle.Bold, Color.white, 116, 150);
        CreateTextInParent(summary.transform, "Rewards\nIngredients\nChance", 14, FontStyle.Bold, new Color(0.88f, 0.84f, 1f), 116, 88);
    }

    private VerticalLayoutGroup PrepareStage(float width, float height, Color stageColor, int padding, int spacing)
    {
        activeStageWidth = width;
        activeStageHeight = height;
        activeStageSafePadding = SafePadding;
        activeStageFillsViewport = false;
        lastRootSize = Vector2.zero;
        stageAmbientBackdrop = null;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (LayoutGroup group in GetComponents<LayoutGroup>())
        {
            group.enabled = false;
            Destroy(group);
        }

        Image background = GetComponent<Image>();
        if (background != null)
        {
            background.color = new Color(0.08f, 0.09f, 0.12f, 1f);
        }

        RectTransform rootRect = EnsureRootRectTransform();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject stage = new GameObject("Stage");
        stage.transform.SetParent(transform, false);

        stageRect = stage.AddComponent<RectTransform>();
        stageRect.anchorMin = new Vector2(0.5f, 0.5f);
        stageRect.anchorMax = new Vector2(0.5f, 0.5f);
        stageRect.pivot = new Vector2(0.5f, 0.5f);
        stageRect.anchoredPosition = Vector2.zero;
        stageRect.sizeDelta = new Vector2(width, height);

        Image stageImage = stage.AddComponent<Image>();
        stageImage.color = stageColor;

        contentRoot = stage.transform;

        VerticalLayoutGroup layout = stage.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(padding, padding, padding, padding);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        return layout;
    }

    private void ApplyStageScale(bool force = false)
    {
        if (stageRect == null)
        {
            return;
        }

        RectTransform rootRect = EnsureRootRectTransform();
        Vector2 rootSize = rootRect.rect.size;
        Rect safeArea = Screen.safeArea;
        if (!force && rootSize == lastRootSize && safeArea == lastSafeArea)
        {
            return;
        }

        lastRootSize = rootSize;
        lastSafeArea = safeArea;

        // Keep every screen inside the device safe area (notches, rounded corners, home indicator).
        GetSafeAreaInsets(rootSize, safeArea, out float insetLeft, out float insetRight, out float insetBottom, out float insetTop);
        bool fillsViewport = activeStageFillsViewport || IsRealmHomeStage();
        float safePadding = fillsViewport ? 0f : activeStageSafePadding;
        float availableWidth = Mathf.Max(1f, rootSize.x - insetLeft - insetRight - safePadding * 2f);
        float availableHeight = Mathf.Max(1f, rootSize.y - insetBottom - insetTop - safePadding * 2f);
        float widthScale = availableWidth / activeStageWidth;
        float heightScale = availableHeight / activeStageHeight;

        // Full-bleed screens fit the whole stage so artwork-anchored UI (room medallions) stays visible and aligned;
        // the ambient backdrop covers any leftover bands. Other screens keep the original size limits.
        float scale = Mathf.Min(widthScale, heightScale);
        scale = fillsViewport ? Mathf.Max(0.55f, scale) : Mathf.Clamp(scale, 0.55f, 1.15f);

        stageRect.localScale = new Vector3(scale, scale, 1f);
        stageRect.anchoredPosition = new Vector2((insetLeft - insetRight) * 0.5f, (insetBottom - insetTop) * 0.5f);

        if (stageAmbientBackdrop != null)
        {
            float coverScale = Mathf.Max(rootSize.x / activeStageWidth, rootSize.y / activeStageHeight);
            stageAmbientBackdrop.sizeDelta = new Vector2(activeStageWidth, activeStageHeight) * coverScale;
        }
    }

    private static void GetSafeAreaInsets(Vector2 rootSize, Rect safeArea, out float left, out float right, out float bottom, out float top)
    {
        left = right = bottom = top = 0f;
        if (Screen.width <= 0 || Screen.height <= 0 || safeArea.width <= 0f || safeArea.height <= 0f)
        {
            return;
        }

        float toCanvasX = rootSize.x / Screen.width;
        float toCanvasY = rootSize.y / Screen.height;
        left = Mathf.Max(0f, safeArea.xMin) * toCanvasX;
        right = Mathf.Max(0f, Screen.width - safeArea.xMax) * toCanvasX;
        bottom = Mathf.Max(0f, safeArea.yMin) * toCanvasY;
        top = Mathf.Max(0f, Screen.height - safeArea.yMax) * toCanvasY;
    }

    private bool IsRealmHomeStage()
    {
        if (stageRect == null)
        {
            return false;
        }

        for (int index = 0; index < stageRect.childCount; index++)
        {
            if (stageRect.GetChild(index).name.StartsWith(RealmHomeBackgroundObjectPrefix))
            {
                return true;
            }
        }

        return false;
    }

    private Text CreateText(string label, int size, FontStyle style, Color color, float height)
    {
        return CreateText(label, size, style, color, height, 560);
    }

    private Text CreateText(string label, int size, FontStyle style, Color color, float height, float width)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(contentRoot, false);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;

        return text;
    }

    private GameObject CreatePanel(string name, Color color, float height)
    {
        return CreatePanel(name, color, height, BoardWidth);
    }

    private GameObject CreatePanel(string name, Color color, float height, float width)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(contentRoot, false);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;
        layoutElement.flexibleWidth = 0;

        return panel;
    }

    private GameObject CreatePanelInParent(Transform parent, string name, Color color, float width, float height)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;
        layoutElement.flexibleWidth = 0;

        return panel;
    }

    private GameObject CreateAnchoredLayer(Transform parent, string name, float width, float height)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent, false);

        RectTransform rect = layer.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;

        return layer;
    }

    private GameObject CreateAnchoredPanel(Transform parent, string name, Color color, float width, float height, float x, float y)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        return panel;
    }

    private Text CreateAnchoredText(Transform parent, string label, int size, FontStyle style, Color color, float width, float height, float x, float y)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        return text;
    }

    private InputField CreateAnchoredInputField(Transform parent, string name, string placeholder, float width, float height, float x, float y)
    {
        GameObject inputObject = CreateAnchoredPanel(parent, name, new Color(0.96f, 0.86f, 0.62f), width, height, x, y);
        InputField input = inputObject.AddComponent<InputField>();
        input.characterLimit = 160;
        input.lineType = InputField.LineType.MultiLineSubmit;
        input.caretColor = new Color(0.18f, 0.08f, 0.32f);
        input.selectionColor = new Color(0.35f, 0.12f, 0.62f, 0.35f);

        Text messageText = CreateAnchoredText(inputObject.transform, "", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), width - 28f, height - 22f, 0f, 0f);
        messageText.alignment = TextAnchor.UpperLeft;
        messageText.verticalOverflow = VerticalWrapMode.Truncate;

        Text placeholderText = CreateAnchoredText(inputObject.transform, placeholder, 16, FontStyle.Italic, new Color(0.36f, 0.28f, 0.42f, 0.72f), width - 28f, height - 22f, 0f, 0f);
        placeholderText.alignment = TextAnchor.UpperLeft;
        placeholderText.verticalOverflow = VerticalWrapMode.Truncate;

        input.textComponent = messageText;
        input.placeholder = placeholderText;
        input.targetGraphic = inputObject.GetComponent<Image>();

        return input;
    }

    private Image CreateAnchoredImage(Transform parent, string resourceName, float width, float height, float x, float y)
    {
        GameObject imageObject = new GameObject(resourceName);
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = imageObject.AddComponent<Image>();
        Texture2D texture = Resources.Load<Texture2D>(resourceName);
        if (texture != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.preserveAspect = true;
            image.color = Color.white;
        }
        else
        {
            image.color = new Color(0.15f, 0.3f, 0.8f);
        }

        return image;
    }

    private void CreateInfoTile(Transform parent, string title, string detail, float width, Color color)
    {
        GameObject tile = new GameObject(title);
        tile.transform.SetParent(parent, false);

        Image image = tile.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = tile.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = 132;
        layoutElement.flexibleWidth = 0;

        VerticalLayoutGroup layout = tile.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 6;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(tile.transform, title, 20, FontStyle.Bold, Color.white, width - 24, 36);
        CreateTextInParent(tile.transform, detail, 19, FontStyle.Bold, Color.white, width - 24, 70);
    }

    private void CreateCardOption(Transform parent, int cardCount, string ingredientText, string chanceText)
    {
        int entryCost = cardCount * manaBetPerCard;
        bool canAfford = inventory.CanAffordMana(entryCost);
        Color fill = !canAfford
            ? new Color(0.5f, 0.48f, 0.44f)
            : cardCount == selectedCardCount ? new Color(1f, 0.86f, 0.46f) : new Color(0.96f, 0.88f, 0.68f);
        GameObject tile = new GameObject($"{cardCount} Card Option");
        tile.transform.SetParent(parent, false);

        Image image = tile.AddComponent<Image>();
        image.color = fill;

        LayoutElement layoutElement = tile.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 250;
        layoutElement.preferredHeight = 205;
        layoutElement.flexibleWidth = 0;

        VerticalLayoutGroup layout = tile.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 10, 10);
        layout.spacing = 7;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(tile.transform, $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}", 25, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 30);
        CreateTextInParent(tile.transform, ingredientText, 18, FontStyle.Bold, new Color(0.14f, 0.08f, 0.24f), 220, 30);
        CreateTextInParent(tile.transform, chanceText, 15, FontStyle.Bold, new Color(0.14f, 0.08f, 0.24f), 220, 38);

        Button play = CreateButton(tile.transform, canAfford ? $"Play {entryCost} Mana" : $"Need {entryCost} Mana", 20, 48, 210, canAfford ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.36f, 0.36f, 0.36f));
        play.onClick.AddListener(() => BeginRoundFromLobby(cardCount));
    }

    private Text CreateTextInParent(Transform parent, string label, int size, FontStyle style, Color color, float width, float height)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;
        layoutElement.flexibleWidth = 0;

        return text;
    }

    private void CreateHeaderCell(Transform parent, string letter)
    {
        GameObject headerObject = new GameObject($"Header_{letter}");
        headerObject.transform.SetParent(parent, false);
        headerObject.AddComponent<RectTransform>();

        Text text = CreateChildText(headerObject.transform);
        text.text = letter;
        text.fontSize = 28;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(1f, 0.87f, 0.32f);
    }

    private void BuildVisibleCards()
    {
        BuildVisibleCards(contentRoot, 1180);
    }

    private void BuildVisibleCards(Transform parent, float availableWidth)
    {
        visibleCardCells.Clear();
        visibleCardBingoTexts.Clear();

        int cardsToShow = Mathf.Max(1, selectedCardCount);
        int columns = cardsToShow <= 1 ? 1 : cardsToShow <= 4 ? 2 : 3;
        int rows = Mathf.CeilToInt((float)cardsToShow / columns);
        float boardGap = cardsToShow <= 2 ? 7f : 4f;
        float cardChromeHeight = cardsToShow <= 2 ? 104f : 90f;
        float layoutSpacingX = cardsToShow <= 2 ? 20f : 14f;
        float layoutSpacingY = cardsToShow <= 2 ? 12f : 10f;
        float maxAreaHeight = cardsToShow <= 2 ? 650f : 680f;
        float maxCardWidth = ((availableWidth - (layoutSpacingX * (columns - 1))) / columns) - 18f;
        float maxCardHeight = ((maxAreaHeight - (layoutSpacingY * (rows - 1))) / rows);
        float maxBoardWidth = Mathf.Min(maxCardWidth, maxCardHeight - cardChromeHeight);
        float fitCellSize = Mathf.Floor((maxBoardWidth - ((BoardSize - 1) * boardGap)) / BoardSize);
        float maxCellSize = cardsToShow <= 1 ? 108f : cardsToShow <= 2 ? 88f : cardsToShow <= 4 ? 54f : 44f;
        float minCellSize = cardsToShow <= 2 ? 58f : cardsToShow <= 4 ? 46f : 36f;
        float boardCellSize = Mathf.Clamp(fitCellSize, minCellSize, maxCellSize);
        float cardWidth = BoardSize * boardCellSize + (BoardSize - 1) * boardGap;
        float cardHeight = cardChromeHeight + cardWidth;
        float cardsAreaHeight = Mathf.Min(maxAreaHeight, rows * cardHeight + (rows - 1) * layoutSpacingY);

        GameObject cardsArea = CreatePanelInParent(parent, "VisibleCards", new Color(0f, 0f, 0f, 0f), availableWidth, cardsAreaHeight);
        GridLayoutGroup cardsLayout = cardsArea.AddComponent<GridLayoutGroup>();
        cardsLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cardsLayout.constraintCount = Mathf.Max(1, columns);
        cardsLayout.cellSize = new Vector2(cardWidth + 18f, cardHeight);
        cardsLayout.spacing = new Vector2(layoutSpacingX, layoutSpacingY);
        cardsLayout.childAlignment = TextAnchor.MiddleCenter;

        for (int cardIndex = 0; cardIndex < cardsToShow; cardIndex++)
        {
            CreateCardBoard(cardsArea.transform, cardIndex, boardCellSize, boardGap, cardWidth, cardHeight);
        }
    }

    private void CreateCardBoard(Transform parent, int cardIndex, float boardCellSize, float boardGap, float cardWidth, float cardHeight)
    {
        GameObject cardPanel = new GameObject($"Card_{cardIndex + 1}");
        cardPanel.transform.SetParent(parent, false);

        Image panelImage = cardPanel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.17f, 0.25f);

        VerticalLayoutGroup layout = cardPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 6, 8);
        layout.spacing = 4;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        LayoutElement panelLayout = cardPanel.AddComponent<LayoutElement>();
        panelLayout.preferredWidth = cardWidth + 18f;
        panelLayout.preferredHeight = cardHeight;
        panelLayout.flexibleWidth = 0;

        CreateCardHeaderRow(cardPanel.transform, cardWidth, boardCellSize, boardGap);

        GameObject grid = new GameObject($"Card_{cardIndex + 1}_Grid");
        grid.transform.SetParent(cardPanel.transform, false);

        LayoutElement gridLayoutElement = grid.AddComponent<LayoutElement>();
        gridLayoutElement.preferredWidth = cardWidth;
        gridLayoutElement.preferredHeight = cardWidth;
        gridLayoutElement.flexibleWidth = 0;

        GridLayoutGroup gridLayout = grid.AddComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = BoardSize;
        gridLayout.cellSize = new Vector2(boardCellSize, boardCellSize);
        gridLayout.spacing = new Vector2(boardGap, boardGap);
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                CreateVisibleCardCell(grid.transform, cardIndex, row, column, boardCellSize);
            }
        }

        Text bingoText = CreateTextInParent(cardPanel.transform, GetCardBingoText(cardIndex), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), cardWidth, 22);
        visibleCardBingoTexts[cardIndex] = bingoText;
        CreatePotionBadge(cardPanel.transform, playerCards.GetPotionMultiplier(cardIndex), boardCellSize);
    }

    private void CreatePotionBadge(Transform parent, int multiplier, float boardCellSize)
    {
        GameObject badge = new GameObject($"Potion_x{multiplier}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.sizeDelta = new Vector2(Mathf.Max(72f, boardCellSize * 1.25f), Mathf.Max(30f, boardCellSize * 0.42f));
        rect.anchoredPosition = new Vector2(-8f, 8f);

        LayoutElement layoutElement = badge.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.35f, 0.1f, 0.62f, 0.95f);

        Text text = CreateChildText(badge.transform);
        text.text = $"POTION x{multiplier}";
        text.color = new Color(1f, 0.9f, 0.32f);
        text.fontSize = Mathf.Max(10, Mathf.RoundToInt(boardCellSize * 0.18f));
        text.fontStyle = FontStyle.Bold;
    }

    private void CreateCardHeaderRow(Transform parent, float cardWidth, float boardCellSize, float boardGap)
    {
        GameObject header = new GameObject("BingoHeader");
        header.transform.SetParent(parent, false);

        LayoutElement headerLayoutElement = header.AddComponent<LayoutElement>();
        float headerHeight = Mathf.Clamp(boardCellSize * 0.62f, 38f, 52f);
        headerLayoutElement.preferredWidth = cardWidth;
        headerLayoutElement.preferredHeight = headerHeight;
        headerLayoutElement.flexibleWidth = 0;

        GridLayoutGroup headerLayout = header.AddComponent<GridLayoutGroup>();
        headerLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        headerLayout.constraintCount = BoardSize;
        headerLayout.cellSize = new Vector2(boardCellSize, headerHeight - 2f);
        headerLayout.spacing = new Vector2(boardGap, 0f);
        headerLayout.childAlignment = TextAnchor.MiddleCenter;

        string[] letters = { "B", "I", "N", "G", "O" };
        Color[] colors =
        {
            new Color(0.78f, 0.12f, 0.1f),
            new Color(0.94f, 0.54f, 0.06f),
            new Color(0.18f, 0.64f, 0.12f),
            new Color(0.05f, 0.35f, 0.78f),
            new Color(0.5f, 0.12f, 0.74f)
        };

        for (int index = 0; index < letters.Length; index++)
        {
            GameObject cell = new GameObject($"Header_{letters[index]}");
            cell.transform.SetParent(header.transform, false);
            Image image = cell.AddComponent<Image>();
            image.color = colors[index];

            Text text = CreateChildText(cell.transform);
            text.text = letters[index];
            text.fontSize = Mathf.RoundToInt(Mathf.Clamp(boardCellSize * 0.36f, 18f, 30f));
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
        }
    }

    private void CreateVisibleCardCell(Transform parent, int cardIndex, int row, int column, float boardCellSize)
    {
        GameObject cellObject = new GameObject($"Card_{cardIndex + 1}_Cell_{row}_{column}");
        cellObject.transform.SetParent(parent, false);

        bool isMarked = playerCards.IsMarked(cardIndex, row, column);
        int value = playerCards.GetNumber(cardIndex, row, column);
        bool isWinningCell = playerCards.IsWinningCell(cardIndex, row, column);

        Image image = cellObject.AddComponent<Image>();
        image.color = GetVisibleCellColor(isMarked, isWinningCell);

        Button button = cellObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => DaubVisibleCardCell(cardIndex, row, column));

        Text label = CreateChildText(cellObject.transform);
        string visibleKey = $"{cardIndex}:{row}:{column}";
        label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
        label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
        label.fontSize = Mathf.RoundToInt(boardCellSize * GetVisibleCellLabelScale(row, column, isMarked, label.text));
        label.fontStyle = FontStyle.Bold;

        if (!isMarked && cellRewards.TryGetReward(visibleKey, out CellReward reward))
        {
            CreateRewardBadge(cellObject.transform, reward.Badge, boardCellSize);
        }

        if (!isMarked && placedPowerUpSigils.TryGetValue(visibleKey, out string placedPowerUpName))
        {
            CreatePlacedPowerUpBadge(cellObject.transform, GetPowerUpCellInitials(placedPowerUpName), boardCellSize);
        }

        visibleCardCells[visibleKey] = button;
    }

    private void CreateRewardBadge(Transform parent, string label, float boardCellSize)
    {
        GameObject badge = new GameObject($"Reward_{label}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(boardCellSize * 0.58f, boardCellSize * 0.28f);
        rect.anchoredPosition = new Vector2(-2f, -2f);

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.38f, 0.1f, 0.62f);

        Text text = CreateChildText(badge.transform);
        text.text = label;
        text.color = new Color(0.88f, 0.96f, 1f);
        text.fontSize = Mathf.Max(8, Mathf.RoundToInt(boardCellSize * 0.18f));
        text.fontStyle = FontStyle.Bold;
    }

    private void CreatePlacedPowerUpBadge(Transform parent, string label, float boardCellSize)
    {
        GameObject badge = new GameObject($"PowerUpDrop_{label}");
        badge.transform.SetParent(parent, false);

        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(boardCellSize * 0.08f, boardCellSize * 0.08f);
        rect.offsetMax = new Vector2(-boardCellSize * 0.08f, -boardCellSize * 0.08f);

        Image image = badge.AddComponent<Image>();
        image.color = new Color(0.28f, 0.05f, 0.58f, 0.24f);

        Text text = CreateChildText(badge.transform);
        text.text = label;
        text.color = new Color(0.52f, 0.92f, 1f, 0.82f);
        text.fontSize = Mathf.Max(14, Mathf.RoundToInt(boardCellSize * 0.28f));
        text.fontStyle = FontStyle.Bold;
        badge.transform.SetAsFirstSibling();
    }

    private Text GetCellNumberLabel(Transform cellTransform)
    {
        Transform labelTransform = cellTransform.Find("Label");
        return labelTransform != null ? labelTransform.GetComponent<Text>() : null;
    }

    private void RemoveRewardBadge(Transform cellTransform)
    {
        for (int index = cellTransform.childCount - 1; index >= 0; index--)
        {
            Transform child = cellTransform.GetChild(index);
            if (child.name.StartsWith("Reward_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void RemovePlacedPowerUpBadge(Transform cellTransform)
    {
        for (int index = cellTransform.childCount - 1; index >= 0; index--)
        {
            Transform child = cellTransform.GetChild(index);
            if (child.name.StartsWith("PowerUpDrop_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private string GetCellKey(int row, int column)
    {
        return PlayerCardSet.GetCellKey(row, column);
    }

    private Color GetVisibleCellColor(bool isMarked, bool isWinningCell)
    {
        if (isWinningCell)
        {
            return new Color(1f, 0.72f, 0.12f);
        }

        return isMarked ? new Color(0.08f, 0.52f, 0.32f) : new Color(0.95f, 0.97f, 1f);
    }

    private string GetVisibleCellLabelText(int cardIndex, int row, int column, int value, bool isMarked)
    {
        if (row == 2 && column == 2)
        {
            return "FREE";
        }

        if (!isMarked)
        {
            return value.ToString();
        }

        string visibleKey = $"{cardIndex}:{row}:{column}";
        return powerUpMarkedCellLabels.TryGetValue(visibleKey, out string powerUpLabel) ? powerUpLabel : "";
    }

    private Color GetVisibleCellLabelColor(int cardIndex, int row, int column, bool isMarked)
    {
        if (!isMarked)
        {
            return new Color(0.08f, 0.1f, 0.16f);
        }

        if (row == 2 && column == 2)
        {
            return Color.white;
        }

        string visibleKey = $"{cardIndex}:{row}:{column}";
        return powerUpMarkedCellLabels.ContainsKey(visibleKey) ? new Color(0.72f, 0.95f, 1f) : Color.white;
    }

    private float GetVisibleCellLabelScale(int row, int column, bool isMarked, string labelText)
    {
        if (row == 2 && column == 2)
        {
            return 0.38f;
        }

        if (!isMarked)
        {
            return 0.5f;
        }

        return string.IsNullOrEmpty(labelText) ? 0.1f : 0.46f;
    }

    private string GetCardBingoText(int cardIndex)
    {
        if (IsBlackoutRoom())
        {
            int marked = playerCards.CountMarkedPlayable(cardIndex);
            string blackoutProgress = GetBlackoutProgressMeter(marked);
            string blackoutText = marked >= BingoRoomRules.BlackoutPlayableSquares
                ? $"BLACKOUT EARNED {blackoutProgress}"
                : $"Blackout {blackoutProgress} {marked}/{BingoRoomRules.BlackoutPlayableSquares}";
            return $"{blackoutText}  |  Potion x{playerCards.GetPotionMultiplier(cardIndex)}";
        }

        int count = playerCards.GetBingoCount(cardIndex);
        string jackpotProgress = GetJackpotProgressMeter(count);
        string bingoText = count >= BingoRoomRules.JackpotBingosPerCard
            ? $"JACKPOT EARNED {jackpotProgress}"
            : $"Jackpot {jackpotProgress} {count}/{BingoRoomRules.JackpotBingosPerCard}";
        return $"{bingoText}  |  Potion x{playerCards.GetPotionMultiplier(cardIndex)}";
    }

    private string GetBlackoutProgressMeter(int markedCount)
    {
        int filled = Mathf.Clamp(Mathf.FloorToInt(markedCount / 4f), 0, 6);
        return $"[{new string('#', filled)}{new string('-', 6 - filled)}]";
    }

    private string GetJackpotProgressMeter(int bingoCount)
    {
        int filled = Mathf.Clamp(bingoCount, 0, BingoRoomRules.JackpotBingosPerCard);
        return $"[{new string('#', filled)}{new string('-', BingoRoomRules.JackpotBingosPerCard - filled)}]";
    }

    private string GetRoundSummaryText()
    {
        if (playerCards.Count == 0)
        {
            return "No cards active.";
        }

        List<string> lines = new List<string>();
        for (int index = 0; index < playerCards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = playerCards.CountMarkedPlayable(index);
                string blackoutLabel = marked >= BingoRoomRules.BlackoutPlayableSquares ? "BLACKOUT" : $"{marked}/{BingoRoomRules.BlackoutPlayableSquares}";
                lines.Add($"Card {index + 1}: {blackoutLabel}");
                continue;
            }

            int count = playerCards.GetBingoCount(index);
            string label = count >= BingoRoomRules.JackpotBingosPerCard ? "JACKPOT" : $"{count}/{BingoRoomRules.JackpotBingosPerCard}";
            lines.Add($"Card {index + 1}: {label}");
        }

        return string.Join("\n", lines);
    }

    private bool IsBlackoutRoom()
    {
        return RealmContentCatalog.ActivePrototypeRoom.IsSpecial
            && RealmContentCatalog.ActivePrototypeRoom.ModeLabel == "Blackout";
    }

    private RoomProgressionProfile GetActiveProgression()
    {
        return RealmContentCatalog.ActivePrototypeRoom.Progression;
    }

    private string FormatPercent(float value)
    {
        return $"{Mathf.RoundToInt(value * 100f)}%";
    }

    private string GetGameplayClairvoyanceText()
    {
        return inventory.HasActiveClairvoyance()
            ? $"CLAIRVOYANCE\n{inventory.GetActiveClairvoyanceTimeText()}"
            : "CLAIRVOYANCE\nOFF";
    }

    private void TickInventoryPowerUps()
    {
        if (!inventory.IsLoaded)
        {
            return;
        }

        bool wasActive = inventory.HasActiveClairvoyance();
        bool ticking = inventory.TickPowerUps(Time.deltaTime);
        bool cauldronVisible = denManaCauldronText != null || cauldronStatusText != null || cauldronAmountText != null;
        if (!ticking && !wasActive && !cauldronVisible)
        {
            return;
        }

        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        powerUpSaveAccumulator += Time.deltaTime;
        if (powerUpSaveAccumulator >= 1f || wasActive != inventory.HasActiveClairvoyance())
        {
            powerUpSaveAccumulator = 0f;
            inventory.Save();
        }

        RefreshPowerUpDisplays();
    }

    private void RefreshPowerUpDisplays()
    {
        if (gameplayManaText != null)
        {
            gameplayManaText.text = inventory.GetManaText();
        }

        if (gameplayCrystalText != null)
        {
            gameplayCrystalText.text = inventory.GetCrystalText();
        }

        if (lobbyManaText != null)
        {
            lobbyManaText.text = $"*  {inventory.GetManaText()}";
        }

        if (lobbyCrystalText != null)
        {
            lobbyCrystalText.text = $"<>  {inventory.GetCrystalText()}";
        }

        if (lobbyPowerUpText != null)
        {
            lobbyPowerUpText.text = $"+  {inventory.GetPowerUpText()}";
        }

        if (powerUpInventoryText != null)
        {
            powerUpInventoryText.text = inventory.GetPowerUpInventoryText();
        }

        if (lobbyClairvoyanceStatusText != null)
        {
            lobbyClairvoyanceStatusText.text = GetLobbyClairvoyanceActionText();
            lobbyClairvoyanceStatusText.color = inventory.HasActiveClairvoyance() ? new Color(0.55f, 1f, 0.85f) : new Color(0.78f, 0.74f, 0.88f);
        }

        if (gameplayClairvoyanceText != null)
        {
            gameplayClairvoyanceText.text = GetGameplayClairvoyanceText();
        }

        if (denManaCauldronText != null)
        {
            denManaCauldronText.text = GetDenManaCauldronButtonText();
        }

        if (cauldronAmountText != null)
        {
            cauldronAmountText.text = $"{inventory.ManaCauldronAmount} / {inventory.ManaCauldronCapacity}";
        }

        if (cauldronStatusText != null)
        {
            cauldronStatusText.text = GetManaCauldronDetailStatusText();
        }

        if (cauldronCollectButton != null)
        {
            cauldronCollectButton.interactable = inventory.ManaCauldronAmount > 0;
            Image image = cauldronCollectButton.GetComponent<Image>();
            if (image != null)
            {
                image.color = inventory.ManaCauldronAmount > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f);
            }

            Text text = cauldronCollectButton.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = inventory.ManaCauldronAmount > 0 ? $"Collect {inventory.ManaCauldronAmount}" : "Empty";
            }
        }

        if (gameplayPowerUpReadyText != null)
        {
            gameplayPowerUpReadyText.text = GetPowerUpBankButtonText();
        }

        if (gameplayPowerUpReadyButton != null)
        {
            gameplayPowerUpReadyButton.interactable = IsPowerUpBankReady();
            Image image = gameplayPowerUpReadyButton.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetPowerUpBankButtonColor();
            }
        }

        if (gameplayPowerUpStockText != null)
        {
            gameplayPowerUpStockText.text = GetPowerUpBankStockText();
        }

        if (gameplayPowerUpAutoButton != null)
        {
            Text autoText = gameplayPowerUpAutoButton.GetComponentInChildren<Text>();
            if (autoText != null)
            {
                autoText.text = inventory.AutoDropPowerUps ? "Auto ON" : "Auto OFF";
            }

            Image autoImage = gameplayPowerUpAutoButton.GetComponent<Image>();
            if (autoImage != null)
            {
                autoImage.color = inventory.AutoDropPowerUps ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.32f, 0.12f, 0.62f);
            }
        }
    }

    private string GetRoomPoolSummaryText()
    {
        if (IsBlackoutRoom())
        {
            return $"Blackouts {GetTotalBingos()}/{selectedCardCount}\nCalls {roundFlow.History.Count}/{GetActiveMaxBallCalls()}";
        }

        return $"Mine {GetTotalBingos()}  |  Sim {rewards.SimulatedBingosClaimed}\nRoom pool {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}";
    }

    private Button CreateCell(Transform parent, int row, int column)
    {
        GameObject cellObject = new GameObject($"Cell_{row}_{column}");
        cellObject.transform.SetParent(parent, false);

        Image image = cellObject.AddComponent<Image>();
        image.color = marked[row, column] ? new Color(0.19f, 0.78f, 0.45f) : new Color(0.95f, 0.97f, 1f);

        Button button = cellObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => ToggleCell(row, column));

        Text label = CreateChildText(cellObject.transform);
        label.text = row == 2 && column == 2 ? "FREE" : numbers[row, column].ToString();
        label.color = marked[row, column] ? Color.white : new Color(0.08f, 0.1f, 0.16f);
        label.fontSize = row == 2 && column == 2 ? 18 : 26;
        label.fontStyle = FontStyle.Bold;

        return button;
    }

    private Text CreateChildText(Transform parent)
    {
        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text text = textObject.AddComponent<Text>();
        text.font = uiFont;
        text.alignment = TextAnchor.MiddleCenter;

        return text;
    }

    private Button CreateButton(Transform parent, string label, int fontSize, float height, float width, Color color)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;
        layoutElement.flexibleWidth = 0;

        if (!string.IsNullOrEmpty(label))
        {
            Text text = CreateChildText(buttonObject.transform);
            text.text = label;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }

        return button;
    }

    private void ResetGame()
    {
        pendingRewardPreview = null;
        lastJackpotEarnedCardIndex = -1;
        StopFinalBallCountdown();
        BuildCallPool();
        GenerateCard();
        BuildUi();
    }

    private void SelectCardCount(int cardCount)
    {
        selectedCardCount = cardCount;
        ClampManaBetToActiveRoom();
        BuildLobbyUi();
    }

    private void BeginRoundFromLobby(int cardCount)
    {
        selectedCardCount = cardCount;
        ClampManaBetToActiveRoom();
        int entryCost = selectedCardCount * manaBetPerCard;
        if (!inventory.TrySpendMana(entryCost))
        {
            roomText.text = $"Need {entryCost} Mana";
            return;
        }

        inventory.ContributeRoomJackpotFromSpend(entryCost);
        MarkPreviousRealmRestoredClosedAfterRoundStart();
        pendingRewardPreview = null;
        lastJackpotEarnedCardIndex = -1;
        StopFinalBallCountdown();
        BuildCallPool();
        roundFlow.BeginRound();
        rewardPreviewShown = false;
        allCalledVisible = false;
        powerUpMarkedCellLabels.Clear();
        placedPowerUpSigils.Clear();
        bankPowerUpBag.Clear();
        bankPowerUpCharge = 0;
        fortuneDoublePrizeActive = false;
        wildSigilModeActive = false;
        placedSigilBonusBingos = 0;
        powerUpAssistedMarks = 0;
        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        powerUps.ResetRound();
        rewards.ResetRound();
        GenerateCards(selectedCardCount);
        cellRewards.GenerateForCards(playerCards, selectedCardCount, manaBetPerCard, GetActiveProgression());
        BeginPrototypeAuthoritativeRoundMirror();
        SelectNextBankPowerUp("Bank ready to charge.");
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.RoundStarted,
            PrototypeAnalyticsPayloadFactory.CreateRoundStartedJson(
                selectedCardCount,
                manaBetPerCard,
                inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom),
                entryCost));
        BuildUi();
    }

    private void AdjustLobbyBet(int delta)
    {
        RoomProgressionProfile progression = GetActiveProgression();
        manaBetPerCard = Mathf.Clamp(manaBetPerCard + delta, progression.MinManaBet, progression.MaxManaBet);
        inventory.SaveManaBetForActiveRoom(manaBetPerCard);
        BuildLobbyUi();
    }

    private void ClampManaBetToActiveRoom()
    {
        RoomProgressionProfile progression = GetActiveProgression();
        manaBetPerCard = Mathf.Clamp(manaBetPerCard, progression.MinManaBet, progression.MaxManaBet);
    }

    private void ApplySavedManaBetForActiveRoom()
    {
        manaBetPerCard = inventory.GetSavedManaBetForActiveRoom();
        ClampManaBetToActiveRoom();
    }

    private void RefreshLobbyBetText()
    {
        RoomProgressionProfile progression = GetActiveProgression();
        string summary = $"{manaBetPerCard} Mana / Card\nRange {progression.MinManaBet}-{progression.MaxManaBet}";
        if (roomText != null)
        {
            roomText.text = summary;
        }

        roomEntryPanelView?.SetBetSummary(summary);
    }

    private MultiplayerLobbyDisplayModel BuildPrototypeMultiplayerLobbyDisplayModel()
    {
        if (prototypeMultiplayerPresentationState == null)
        {
            RefreshPrototypeMultiplayerPresentationState();
        }

        return prototypeMultiplayerPresentationState?.Lobby;
    }

    private void RefreshPrototypeMultiplayerPresentationState()
    {
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime?.PresentationFacade?.BuildPresentationState());
    }

    private void ApplyPrototypeMultiplayerPresentationState(PrototypeMultiplayerPresentationState state)
    {
        prototypeMultiplayerPresentationState = state;
        RefreshGameplayMultiplayerStatus();
    }

    private void RefreshLobbyMultiplayerStatus()
    {
        MultiplayerLobbyStatusDisplayModel model = prototypeMultiplayerPresentationState?.LobbyStatus;
        if (model == null)
        {
            BuildPrototypeMultiplayerLobbyDisplayModel();
            model = prototypeMultiplayerPresentationState?.LobbyStatus;
        }

        if (model == null)
        {
            return;
        }

        if (lobbyMultiplayerTitleText != null)
        {
            lobbyMultiplayerTitleText.text = model.TitleSummaryLabel;
        }

        if (lobbyMultiplayerReadinessText != null)
        {
            lobbyMultiplayerReadinessText.text = model.ReadinessSummaryLabel;
            lobbyMultiplayerReadinessText.color = model.CanStart
                ? new Color(0.55f, 1f, 0.85f)
                : new Color(1f, 0.9f, 0.32f);
        }

        if (lobbyMultiplayerDetailText != null)
        {
            lobbyMultiplayerDetailText.text = $"{model.ActionLabel} — {model.DetailLabel}";
        }

        if (lobbyMultiplayerParticipantsText != null)
        {
            lobbyMultiplayerParticipantsText.text = model.ParticipantSummaryLabel;
        }
    }

    private void RefreshGameplayMultiplayerStatus()
    {
        MultiplayerGameplayRoundDisplayModel gameplayModel = prototypeMultiplayerPresentationState?.GameplayRound;
        if (gameplayModel == null)
        {
            return;
        }

        if (gameplayMultiplayerRoomText != null)
        {
            gameplayMultiplayerRoomText.text = gameplayModel.RoomSummaryLabel;
        }

        if (gameplayMultiplayerAuthorityText != null)
        {
            gameplayMultiplayerAuthorityText.text = gameplayModel.AuthoritySummaryLabel;
        }

        if (gameplayMultiplayerClaimText != null)
        {
            gameplayMultiplayerClaimText.text = gameplayModel.ClaimSummaryLabel;
        }

        if (gameplayMultiplayerPostRoundText != null)
        {
            gameplayMultiplayerPostRoundText.text = gameplayModel.PostRoundSummaryLabel;
        }
    }

    private void ShowCard(int cardIndex)
    {
        SaveCurrentCard();
        currentCardIndex = Mathf.Clamp(cardIndex, 0, selectedCardCount - 1);
        LoadCurrentCard();
        BuildUi();
    }

    private void DaubVisibleCardCell(int cardIndex, int row, int column)
    {
        if (row == 2 && column == 2)
        {
            statusText.text = "FREE space is already yours.";
            return;
        }

        int value = playerCards.GetNumber(cardIndex, row, column);
        if (playerCards.IsMarked(cardIndex, row, column))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} is already daubed on Card {cardIndex + 1}.";
            return;
        }

        if (wildSigilModeActive)
        {
            wildSigilModeActive = false;
            ApplyPowerUpMarks("Wild Sigil", new List<Vector3Int> { new Vector3Int(cardIndex, row, column) });
            if (roundFlow.IsActive && !rewardPreviewShown)
            {
                SelectNextBankPowerUp("Next power-up charging");
            }

            return;
        }

        string key = $"{cardIndex}:{row}:{column}";
        if (!roundFlow.IsNumberCalled(value))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} has not been called yet.";
            if (visibleCardCells.TryGetValue(key, out Button wrongButton))
            {
                StartCoroutine(FlashCell(wrongButton.transform, new Color(0.95f, 0.22f, 0.26f), new Color(0.95f, 0.97f, 1f)));
            }
            return;
        }

        int previousBingoCount = playerCards.GetBingoCount(cardIndex);
        int daubXp = GetDaubXp(value, key);
        string xpReason = powerUps.IsCellAlerted(key) ? "Clairvoyance daub" : "Daub";

        playerCards.Mark(cardIndex, row, column);
        powerUps.ClearCellAlert(key);
        string collectedRewardText = "";
        if (cellRewards.TryCollect(key, out CellReward collectedReward))
        {
            string displayQuantity = CellRewardTracker.GetDisplayQuantity(collectedReward);
            collectedRewardText = $" Collected {collectedReward.Name} {displayQuantity}.";
            if (collectedReward.Kind == CellRewardKind.Crystals)
            {
                ShowPowerUpBurst($"CRYSTALS {displayQuantity}");
            }
        }
        string placedPowerUpText = TriggerPlacedPowerUpSigil(key, cardIndex);
        UpdateCardBingoProgress(cardIndex);
        AddXp(daubXp, xpReason);
        TrackPowerUpReleaseFromDaub();

        if (visibleCardCells.TryGetValue(key, out Button button))
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetVisibleCellColor(true, playerCards.IsWinningCell(cardIndex, row, column));
            }

            RefreshVisibleCellLabel(button.transform, cardIndex, row, column);
            RemoveRewardBadge(button.transform);
            RemovePlacedPowerUpBadge(button.transform);
            StartCoroutine(PulseCell(button.transform));
        }

        RefreshVisibleCardWinningCells(cardIndex);

        int newBingoCount = playerCards.GetBingoCount(cardIndex);
        if (newBingoCount > previousBingoCount)
        {
            MirrorPrototypeBingoClaim(cardIndex, newBingoCount);
            int bingoDelta = newBingoCount - previousBingoCount;
            int claimedCount = IsBlackoutRoom() ? 1 : bingoDelta;
            int bingoBonus = claimedCount * GetScaledXp(roomRules.BingoClaimXp);
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.BingoClaimed,
                PrototypeAnalyticsPayloadFactory.CreateBingoClaimedJson(
                    selectedCardCount,
                    manaBetPerCard,
                    inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom),
                    cardIndex + 1,
                    bingoDelta,
                    claimedCount,
                    rewards.RoomBingosClaimed,
                    roomRules.RoomBingoPool,
                    IsBlackoutRoom(),
                    "daub"));
            if (!IsBlackoutRoom())
            {
                ConsumeRoomBingos(claimedCount, false);
            }
            else
            {
                RefreshRoomPoolTexts();
            }

            AddXp(bingoBonus, "Auto-Claim Bingo");

            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }

            if (newBingoCount >= BingoRoomRules.JackpotBingosPerCard)
            {
                string earnedLabel = IsBlackoutRoom() ? "BLACKOUT WHEELSPIN EARNED" : "JACKPOT WHEELSPIN EARNED";
                string reachedLabel = IsBlackoutRoom() ? "completed blackout" : "reached 5/5";
                lastJackpotEarnedCardIndex = cardIndex;
                roundFlow.MarkJackpotState();
                MirrorPrototypeJackpotStateClaim(cardIndex);
                int jackpotStateXp = GetScaledXp(roomRules.JackpotStateXp);
                AddXp(jackpotStateXp, "Jackpot State");
                BingoRoundEndDecision jackpotDecision = BingoRoundEndRules.ResolvePostProgressDecision(
                    IsBlackoutRoom(),
                    AllCardsReachedJackpot(),
                    IsPrototypeBingoPoolExhausted(),
                    $"Room bingo pool exhausted. Jackpot reached on Card {cardIndex + 1}.",
                    false,
                    "");
                if (TryStartResolvedRoundEndCountdown(jackpotDecision))
                {
                    return;
                }

                if (bingoBannerText != null)
                {
                    bingoBannerText.text = GetBingoBannerText();
                }

                statusText.text = $"{earnedLabel}! Card {cardIndex + 1} {reachedLabel}. +{daubXp + bingoBonus + jackpotStateXp} XP.{collectedRewardText}{placedPowerUpText}";
                return;
            }

            BingoRoundEndDecision bingoProgressDecision = BingoRoundEndRules.ResolvePostProgressDecision(
                IsBlackoutRoom(),
                false,
                IsPrototypeBingoPoolExhausted(),
                $"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
                IsStandardCoverageLimitReached(),
                $"Standard room coverage limit reached after Card {cardIndex + 1} bingo.");
            if (TryStartResolvedRoundEndCountdown(bingoProgressDecision))
            {
                return;
            }

            statusText.text = IsBlackoutRoom()
                ? $"Card {cardIndex + 1} blackout complete. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}"
                : $"Card {cardIndex + 1} auto-claimed {newBingoCount}/{BingoRoomRules.JackpotBingosPerCard}. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}";
            return;
        }

        statusText.text = IsBlackoutRoom()
            ? $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. Blackout {playerCards.CountMarkedPlayable(cardIndex)}/{BingoRoomRules.BlackoutPlayableSquares}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}"
            : $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}";
        BingoRoundEndDecision daubDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            false,
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
            IsStandardCoverageLimitReached(),
            $"Standard room coverage limit reached after {GetBingoLetter(value)}-{value}.");
        TryStartResolvedRoundEndCountdown(daubDecision);
    }

    private string TriggerPlacedPowerUpSigil(string key, int cardIndex)
    {
        if (!placedPowerUpSigils.TryGetValue(key, out string powerUpName))
        {
            return "";
        }

        placedPowerUpSigils.Remove(key);

        if (powerUpName == "Fortune Sigil")
        {
            fortuneDoublePrizeActive = true;
            ShowPowerUpBurst("FORTUNE x2 ACTIVE");
            SetGameplayPowerUpEventText("Fortune x2 triggered");
            RefreshPowerUpDisplays();
            return " Fortune Sigil triggered: round mana winnings are x2.";
        }

        if (powerUpName == "Wild Sigil")
        {
            wildSigilModeActive = true;
            ShowPowerUpBurst("WILD MODE: TAP 1 CELL");
            SetGameplayPowerUpEventText("Wild triggered");
            RefreshPowerUpDisplays();
            return " Wild Sigil triggered: tap any open square.";
        }

        if (powerUpName == "Arcane Spark")
        {
            ApplyRandomMarksAcrossCards(powerUpName, GetArcaneSparkMarkCount());
            ShowPowerUpBurst($"ARCANE SPARK +{GetArcaneSparkMarkCount()}");
            SetGameplayPowerUpEventText("Spark triggered");
            return " Arcane Spark triggered: lightning daubs released.";
        }

        if (powerUpName == "Presto Sigil")
        {
            if (!IsBlackoutRoom())
            {
                placedSigilBonusBingos++;
                ConsumeRoomBingos(1, false);
                AddXp(GetScaledXp(roomRules.BingoClaimXp), "Presto Bonus Bingo");
                if (bingoBannerText != null)
                {
                    bingoBannerText.text = GetBingoBannerText();
                }
            }

            ShowPowerUpBurst(IsBlackoutRoom() ? "PRESTO HIT" : "PRESTO +1 BINGO");
            RefreshPowerUpDisplays();
            return $" Presto Sigil triggered on Card {cardIndex + 1}: +1 bonus bingo.";
        }

        if (powerUpName == "Pandora Sigil")
        {
            List<string> granted = inventory.OpenPandoraPowerUps();
            string grantedText = granted.Count > 0 ? GetCompactPowerUpGrantSummary(granted) : "no power-ups";
            ShowPowerUpBurst($"PANDORA +{granted.Count} REWARDS");
            SetGameplayPowerUpEventText("Pandora opened");
            RefreshPowerUpDisplays();
            return $" Pandora Sigil opened: +{grantedText}.";
        }

        return $" {powerUpName} triggered.";
    }

    private int GetDaubXp(int value, string visibleKey)
    {
        if (!roundFlow.TryGetCalledAtTime(value, out float calledAt))
        {
            return GetScaledXp(roomRules.NormalDaubXp);
        }

        float daubDelay = Time.time - calledAt;
        return GetScaledXp(roomRules.GetDaubXp(daubDelay, powerUps.IsCellAlerted(visibleKey)));
    }

    private int GetScaledXp(int baseAmount)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseAmount * GetActiveProgression().XpMultiplier));
    }

    private void AddXp(int amount, string reason)
    {
        if (amount <= 0)
        {
            return;
        }

        rewards.AddXp(amount);
        if (xpText != null)
        {
            xpText.text = BuildGameplayRankText();
        }
    }

    private void TickAutoCaller()
    {
        if (roundEndCoordinator.CountdownActive)
        {
            return;
        }

        if (timerText != null)
        {
            timerText.text = roundFlow.IsActive ? $"{Mathf.Max(0f, roundFlow.NextCallTimer):0.0}s" : "Stopped";
        }

        if (!roundFlow.TickAutoCallTimer(Time.deltaTime))
        {
            return;
        }

        CallNumber();
    }

    private void CallNumber()
    {
        float calledAtTime = Time.time;
        int calledNumber;
        if (prototypeMultiplayerRuntime?.PresentationFacade != null)
        {
            PrototypeMultiplayerCallResult callResult =
                prototypeMultiplayerRuntime.PresentationFacade.TryEmitAuthoritativeCallAndBuildResult(roundFlow.IsActive);
            MatchCallEvent authoritativeCall = callResult?.CallEvent;
            if (authoritativeCall == null
                || !roundFlow.RegisterAuthoritativeCall(authoritativeCall.CalledNumber, calledAtTime))
            {
                StartFinalBallCountdown("No more numbers to call.");
                return;
            }

            calledNumber = authoritativeCall.CalledNumber;
            ApplyPrototypeMultiplayerPresentationState(callResult.PresentationState);
        }
        else if (!roundFlow.TryCallNext(calledAtTime, out calledNumber))
        {
            StartFinalBallCountdown("No more numbers to call.");
            return;
        }

        if (powerUps.ClairvoyanceActive)
        {
            StartCoroutine(TriggerClairvoyanceAlert(calledNumber));
        }

        if (calledNumberText != null)
        {
            calledNumberText.fontSize = 46;
            calledNumberText.text = $"{GetBingoLetter(calledNumber)}-{calledNumber}";
        }

        if (ballsLeftText != null)
        {
            ballsLeftText.text = Mathf.Max(0, GetActiveMaxBallCalls() - roundFlow.History.Count).ToString();
        }

        RefreshCalledBallDisplays();

        if (!IsBlackoutRoom())
        {
            MaybeSimulatedPlayersClaimBingo();
        }

        if (statusText != null)
        {
            statusText.text = HasNumberOnAnyCard(calledNumber)
                ? "Called number is on one of your cards. Daub quickly for bonus XP."
                : "No match across your visible cards. Stay ready.";
        }

        BingoRoundEndDecision callDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            false,
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted. You claimed {GetTotalBingos()}, simulated players claimed {rewards.SimulatedBingosClaimed}.",
            IsStandardCoverageLimitReached(),
            "Standard room coverage limit reached.");
        if (TryStartResolvedRoundEndCountdown(callDecision))
        {
            return;
        }

        if (roundFlow.IsMaxCallLimitReached(GetActiveMaxBallCalls()))
        {
            StartFinalBallCountdown("Max balls reached.");
        }
    }

    private void CompleteRound(string reason)
    {
        finalBallCountdownRoutine = null;
        roundEndCoordinator.CompleteCountdown();
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        roundFlow.StopRound();
        PublishPrototypeRoundEnd(reason);

        if (rewardPreviewShown)
        {
            return;
        }

        rewardPreviewShown = true;
        if (!powerUps.ClairvoyanceActive && HasMissedDaubs())
        {
            allCalledVisible = true;
            RefreshCalledBallDisplays();
        }

        RewardPreview preview = RewardPreview.Build(playerCards, rewards, cellRewards, selectedCardCount, manaBetPerCard, reason, fortuneDoublePrizeActive, placedSigilBonusBingos);
        pendingRewardPreview = preview;
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.RoundCompleted,
            PrototypeAnalyticsPayloadFactory.CreateRoundCompletedJson(
                selectedCardCount,
                manaBetPerCard,
                inventory.IsRoomRestored(RealmContentCatalog.ActivePrototypeRoom),
                reason,
                preview));
        if (statusText != null)
        {
            statusText.text = preview.GetStatusLine();
        }

        ShowRewardPreview(preview);
    }

    private void StartFinalBallCountdown(string reason)
    {
        StartRoundEndCountdown(BingoRoundEndRules.CreateFinalBallDecision(reason));
    }

    private void StartBlackoutCompletionCountdown()
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        ShowPowerUpBurst("BLACKOUT COMPLETE");
        StartRoundEndCountdown(BingoRoundEndRules.CreateBlackoutCompleteDecision());
    }

    private void StartRoomPoolCountdown(string reason)
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        StartRoundEndCountdown(BingoRoundEndRules.CreateRoomPoolExhaustedDecision(reason));
    }

    private bool TryStartResolvedRoundEndCountdown(BingoRoundEndDecision decision)
    {
        if (decision == null || !decision.ShouldStartCountdown)
        {
            return false;
        }

        if (decision.Kind == BingoRoundEndReasonKind.BlackoutComplete)
        {
            RefreshRoomPoolTexts();
            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }

            ShowPowerUpBurst("BLACKOUT COMPLETE");
        }
        else if (decision.Kind == BingoRoundEndReasonKind.RoomPoolExhausted)
        {
            RefreshRoomPoolTexts();
            if (bingoBannerText != null)
            {
                bingoBannerText.text = GetBingoBannerText();
            }
        }

        StartRoundEndCountdown(decision);
        return true;
    }

    private void StartRoundEndCountdown(BingoRoundEndDecision decision)
    {
        if (!roundEndCoordinator.TryBeginCountdown(decision, rewardPreviewShown))
        {
            return;
        }

        activePrototypeRoundEndDecision = decision;
        finalBallCountdownRoutine = StartCoroutine(RoundEndCountdownRoutine(decision));
    }

    private IEnumerator RoundEndCountdownRoutine(BingoRoundEndDecision decision)
    {
        for (int seconds = 5; seconds >= 1; seconds--)
        {
            if (rewardPreviewShown)
            {
                finalBallCountdownRoutine = null;
                roundEndCoordinator.CancelCountdown();
                yield break;
            }

            string latestCallLabel = roundFlow.History.Count > 0
                ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}"
                : "";
            BingoRoundCountdownDisplayState countdownDisplay = BingoRoundCountdownPresenter.Build(seconds, latestCallLabel, decision.CountdownMessage);

            if (timerText != null)
            {
                timerText.text = countdownDisplay.TimerLabel;
            }

            if (calledNumberText != null)
            {
                calledNumberText.fontSize = 30;
                calledNumberText.text = countdownDisplay.CalledNumberLabel;
            }

            if (ballsLeftText != null)
            {
                ballsLeftText.text = countdownDisplay.BallsLeftLabel;
            }

            if (gameplayPowerUpBurstText != null)
            {
                gameplayPowerUpBurstText.text = countdownDisplay.BurstLabel;
                gameplayPowerUpBurstText.color = new Color(0.72f, 0.95f, 1f, 1f);
                gameplayPowerUpBurstText.gameObject.SetActive(true);
            }

            if (statusText != null)
            {
                statusText.text = countdownDisplay.StatusLabel;
            }

            yield return new WaitForSeconds(1f);
        }

        finalBallCountdownRoutine = null;
        roundEndCoordinator.CompleteCountdown();
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        if (!rewardPreviewShown)
        {
            CompleteRound(decision.Reason);
        }
    }

    private void StopFinalBallCountdown()
    {
        if (finalBallCountdownRoutine != null)
        {
            StopCoroutine(finalBallCountdownRoutine);
        }

        finalBallCountdownRoutine = null;
        roundEndCoordinator.CancelCountdown();
    }

    private void StopRoundAndClearCalledHistory()
    {
        roundFlow.ResetSession();
    }

    private int GetActiveMaxBallCalls()
    {
        if (IsBlackoutRoom())
        {
            return roomRules.GetBlackoutMaxBallCalls(selectedCardCount, manaBetPerCard);
        }

        int baseMaxCalls = roomRules.GetMaxBallCalls(selectedCardCount, manaBetPerCard);
        return Mathf.Max(24, baseMaxCalls - GetPowerUpAssistedCallPenalty());
    }

    private int GetPowerUpAssistedCallPenalty()
    {
        if (selectedCardCount <= 0 || powerUpAssistedMarks <= 0)
        {
            return 0;
        }

        int averageAssistedMarks = Mathf.CeilToInt((float)powerUpAssistedMarks / selectedCardCount);
        return Mathf.Clamp(averageAssistedMarks * 2, 0, 18);
    }

    private void SyncPrototypeMultiplayerLobbyState()
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime.PresentationFacade.SyncLobbyAndBuildState(
                GetPrototypeMultiplayerHostDisplayName(),
                RealmContentCatalog.ActivePrototypeRealmIndex,
                RealmContentCatalog.ActivePrototypeRoomIndex,
                selectedCardCount,
                manaBetPerCard));
    }

    private void BeginPrototypeAuthoritativeRoundMirror()
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        prototypeRoundEndPublished = false;
        activePrototypeRoundEndDecision = null;
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime.PresentationFacade.BeginAuthoritativeRoundAndBuildState(
                GetPrototypeMultiplayerHostDisplayName(),
                RealmContentCatalog.ActivePrototypeRealmIndex,
                RealmContentCatalog.ActivePrototypeRoomIndex,
                selectedCardCount,
                manaBetPerCard,
                System.Environment.TickCount,
                GetActiveMaxBallCalls(),
                roomRules.AutoCallInterval));
    }

    private void MirrorPrototypeObservedCall(int calledNumber)
    {
        ApplyPrototypeMultiplayerPresentationState(
            prototypeMultiplayerRuntime?.PresentationFacade?.TryRecordObservedCallAndBuildState(roundFlow.IsActive, calledNumber));
    }

    private void MirrorPrototypeBingoClaim(int cardIndex, int newBingoCount)
    {
        PrototypeMultiplayerClaimSubmitResult claimResult =
            prototypeMultiplayerRuntime?.PresentationFacade?.TrySubmitBingoClaimAndBuildResult(
                roundFlow.IsActive,
                cardIndex,
                GetPrototypeClaimCallIndex(),
                newBingoCount,
                BuildMarkedCellKeys(cardIndex),
                BuildClaimedNumbers(cardIndex));
        ApplyPrototypeMultiplayerPresentationState(claimResult?.PresentationState);
        ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(claimResult?.Resolution));
    }

    private void MirrorPrototypeJackpotStateClaim(int cardIndex)
    {
        PrototypeMultiplayerClaimSubmitResult claimResult =
            prototypeMultiplayerRuntime?.PresentationFacade?.TrySubmitJackpotStateClaimAndBuildResult(
                roundFlow.IsActive,
                cardIndex,
                GetPrototypeClaimCallIndex(),
                BuildClaimedNumbers(cardIndex));
        ApplyPrototypeMultiplayerPresentationState(claimResult?.PresentationState);
        ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(claimResult?.Resolution));
    }

    private void ApplyPrototypeAuthorityClaimOutcome(MultiplayerAuthorityOutcomeModel outcome)
    {
        if (outcome == null)
        {
            return;
        }

        if (statusText != null && !string.IsNullOrEmpty(outcome.Headline))
        {
            statusText.text = string.IsNullOrEmpty(outcome.Detail)
                ? outcome.Headline
                : $"{outcome.Headline} {outcome.Detail}";
        }

        if (outcome.ShouldCelebrate)
        {
            ShowPowerUpBurst(outcome.Headline.ToUpperInvariant());
        }
    }

    private void ApplyPrototypeRoundEndOutcome(MultiplayerAuthorityOutcomeModel outcome)
    {
        if (outcome == null)
        {
            return;
        }

        if (statusText != null && !string.IsNullOrEmpty(outcome.Detail))
        {
            statusText.text = outcome.Detail;
        }

        if (outcome.ShouldQueueJackpotHandoff)
        {
            ShowPowerUpBurst(outcome.Headline.ToUpperInvariant());
        }
    }

    private void PublishPrototypeRoundEnd(string reason)
    {
        if (prototypeMultiplayerRuntime?.PresentationFacade == null)
        {
            return;
        }

        BingoRoundEndDecision decision = activePrototypeRoundEndDecision
            ?? new BingoRoundEndDecision(BingoRoundEndReasonKind.None, reason, "");
        PrototypeMultiplayerRoundEndPublishResult publishResult =
            prototypeMultiplayerRuntime.PresentationFacade.TryPublishRoundEndAndBuildState(
                prototypeRoundEndPublished,
                decision,
                lastJackpotEarnedCardIndex >= 0);
        prototypeRoundEndPublished = publishResult.WasPublished;
        ApplyPrototypeMultiplayerPresentationState(publishResult.PresentationState);
        ApplyPrototypeRoundEndOutcome(MultiplayerAuthorityOutcomePresenter.BuildRoundEndOutcome(
            publishResult.LatestMatchEnd));
    }

    private string GetPrototypeMultiplayerHostDisplayName()
    {
        return string.IsNullOrWhiteSpace(profileSettingsState.DisplayName) ? "Host" : profileSettingsState.DisplayName;
    }

    private int GetPrototypeClaimCallIndex()
    {
        int authoritativeCallIndex =
            prototypeMultiplayerRuntime?.PresentationFacade?.GetCurrentAuthoritativeCallIndex() ?? -1;
        if (authoritativeCallIndex >= 0)
        {
            return authoritativeCallIndex;
        }

        return Mathf.Max(0, roundFlow.History.Count - 1);
    }

    private List<string> BuildMarkedCellKeys(int cardIndex)
    {
        List<string> keys = new List<string>();
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return keys;
        }

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (playerCards.IsMarked(cardIndex, row, column))
                {
                    keys.Add($"{cardIndex}:{PlayerCardSet.GetCellKey(row, column)}");
                }
            }
        }

        return keys;
    }

    private List<int> BuildClaimedNumbers(int cardIndex)
    {
        List<int> claimedNumbers = new List<int>();
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return claimedNumbers;
        }

        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (playerCards.IsMarked(cardIndex, row, column))
                {
                    int number = playerCards.GetNumber(cardIndex, row, column);
                    if (number > 0)
                    {
                        claimedNumbers.Add(number);
                    }
                }
            }
        }

        return claimedNumbers;
    }

    private bool IsStandardCoverageLimitReached()
    {
        int totalMarked = 0;
        for (int index = 0; index < playerCards.Count; index++)
        {
            totalMarked += playerCards.CountMarkedPlayable(index);
        }

        return BingoRoundEndRules.IsStandardCoverageLimitReached(
            IsBlackoutRoom(),
            playerCards.Count,
            totalMarked,
            selectedCardCount);
    }

    private bool TryStartStandardCoverageCountdown(string reason)
    {
        if (!IsBlackoutRoom() && IsStandardCoverageLimitReached())
        {
            StartRoundEndCountdown(BingoRoundEndRules.CreateStandardCoverageDecision(reason));
            return true;
        }

        return false;
    }

    private void ShowRewardPreview(RewardPreview preview)
    {
        if (statusText == null || statusText.transform.parent == null)
        {
            return;
        }

        Transform root = statusText.transform.parent;
        GameObject panel = CreateAnchoredPanel(root, "RewardPreview", new Color(0.96f, 0.86f, 0.62f, 0.98f), 980, 500, 0f, -162f);
        Color ink = new Color(0.15f, 0.08f, 0.24f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 42, FontStyle.Bold, ink, 900, 58, 0f, 210f);
        string roundResultText = IsBlackoutRoom()
            ? $"{preview.JackpotCards} {(preview.JackpotCards == 1 ? "BLACKOUT" : "BLACKOUTS")}"
            : $"{preview.PlayerBingos} {(preview.PlayerBingos == 1 ? "BINGO" : "BINGOS")}";
        Text roundResultLabel = CreateAnchoredText(panel.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 360, 46, 0f, 160f);

        CreateRoundSummaryStat(panel.transform, "MANA EARNED", preview.ManaReward.ToString("N0"), "*", -312f, 100f);
        CreateRoundSummaryStat(panel.transform, "XP EARNED", preview.PlayerXp.ToString("N0"), "XP", 0f, 100f);
        CreateRoundSummaryStat(panel.transform, preview.EndLevel > preview.StartLevel ? "LEVEL UP" : $"LEVEL {preview.EndLevel}", BuildRoundRankSummary(preview), "", 312f, 100f);

        Text collectedRewardsTitle = CreateAnchoredText(panel.transform, string.Empty, 22, FontStyle.Bold, ink, 820, 30, 0f, 36f);
        RoundCollectedRewardsSectionView collectedRewardsSectionView = panel.GetComponent<RoundCollectedRewardsSectionView>();
        if (collectedRewardsSectionView == null)
        {
            collectedRewardsSectionView = panel.AddComponent<RoundCollectedRewardsSectionView>();
        }

        collectedRewardsSectionView.ResetRuntimeBindings();
        collectedRewardsSectionView.Initialize(collectedRewardsTitle);
        collectedRewardsSectionView.Apply(new RoundCollectedRewardsSectionDisplayModel
        {
            TitleText = "COLLECTED REWARDS"
        });
        BuildRoundCollectedRewards(panel.transform, preview);

        BuildRoundIngredientProgress(panel.transform, preview);

        Button redeem = CreateAnchoredButton(panel.transform, "Collect", 22, 260, 50, new Color(0.12f, 0.55f, 0.08f), 0f, -220f);
        RewardPreviewPanelView view = panel.GetComponent<RewardPreviewPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<RewardPreviewPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, roundResultLabel, collectedRewardsTitle, null, redeem);
        view.ContinueRequested += RedeemRewardsAndReturnHome;
        view.Apply(new RewardPreviewPanelDisplayModel
        {
            TitleText = "ROUND COMPLETE",
            RoundResultText = roundResultText,
            CollectedRewardsTitleText = string.Empty,
            PotionProgressTitleText = string.Empty,
            ContinueButtonText = "Collect"
        });
    }

    private void CreateRoundSummaryStat(Transform parent, string label, string value, string icon, float x, float y)
    {
        GameObject tile = CreateAnchoredPanel(parent, label, new Color(0.99f, 0.91f, 0.72f), 280, 94, x, y);
        Color ink = new Color(0.15f, 0.08f, 0.24f);
        Text labelText = CreateAnchoredText(tile.transform, label, 17, FontStyle.Bold, ink, 240, 24, 0f, 28f);
        string body = string.IsNullOrEmpty(icon) ? value : $"{icon}  {value}";
        Text valueText = CreateAnchoredText(tile.transform, body, value.Length > 12 ? 21 : 34, FontStyle.Bold, ink, 250, 46, 0f, -12f);
        RoundSummaryStatView view = tile.GetComponent<RoundSummaryStatView>();
        if (view == null)
        {
            view = tile.AddComponent<RoundSummaryStatView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(labelText, valueText);
        view.Apply(new RoundSummaryStatDisplayModel
        {
            LabelText = label,
            ValueText = body
        });
    }

    private string BuildRoundRankSummary(RewardPreview preview)
    {
        return $"Level {preview.EndLevel}\n{preview.LevelProgressText}";
    }

    private void ShowCardReveal(IReadOnlyList<AwardedAlbumCardRecord> cards)
    {
        if (statusText == null || statusText.transform.parent == null)
        {
            ContinueAfterCardReveal();
            return;
        }

        Transform root = statusText.transform.parent;
        Transform existing = root.Find("CardReveal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        GameObject panel = CreateAnchoredPanel(root, "CardReveal", new Color(0.035f, 0.018f, 0.07f, 0.985f), 1320, 680, 0f, -84f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 44, FontStyle.Bold, gold, 980, 58, 0f, 286f);
        Text subtitleText = CreateAnchoredText(panel.transform, string.Empty, 19, FontStyle.Bold, new Color(0.86f, 0.84f, 0.94f), 920, 30, 0f, 238f);
        Text overflowText = CreateAnchoredText(panel.transform, string.Empty, 20, FontStyle.Bold, Color.white, 360, 34, 0f, -182f);
        overflowText.gameObject.SetActive(false);
        Button continueButton = CreateAnchoredButton(panel.transform, "Continue", 24, 280, 56, new Color(0.12f, 0.55f, 0.08f), 0f, -266f);
        CardRevealPanelView panelView = panel.GetComponent<CardRevealPanelView>();
        if (panelView == null)
        {
            panelView = panel.AddComponent<CardRevealPanelView>();
        }

        panelView.ResetRuntimeBindings();
        panelView.Initialize(titleText, subtitleText, overflowText, continueButton);
        panelView.ContinueRequested += ContinueAfterCardReveal;

        int visibleCount = Mathf.Min(cards.Count, 5);
        panelView.Apply(new CardRevealPanelDisplayModel
        {
            TitleText = "ADDED TO GRIMOIRE",
            SubtitleText = "Open the Library to find NEW cards marked in their potion set.",
            OverflowText = cards.Count > visibleCount ? $"+{cards.Count - visibleCount} more card wins" : string.Empty,
            ContinueButtonText = "Continue"
        });

        float startX = -(visibleCount - 1) * 126f;
        for (int index = 0; index < visibleCount; index++)
        {
            AwardedAlbumCardRecord card = cards[index];
            float x = startX + index * 252f;
            CreateCardRevealTile(panel.transform, card, x, 38f);
        }
    }

    private void CreateCardRevealTile(Transform parent, AwardedAlbumCardRecord card, float x, float y)
    {
        Color cardColor = GetRevealCardColor(card.RarityLabel);
        GameObject tile = CreateAnchoredPanel(parent, $"CardReveal_{card.CardName}", cardColor, 210, 300, x, y);
        Text starsText = CreateAnchoredText(tile.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 180, 40, 0f, 104f);
        Text cardNameText = CreateAnchoredText(tile.transform, string.Empty, 24, FontStyle.Bold, Color.white, 176, 86, 0f, 32f);
        Text rarityText = CreateAnchoredText(tile.transform, string.Empty, 18, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 160, 28, 0f, -58f);
        Text potionNameText = CreateAnchoredText(tile.transform, string.Empty, 13, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 174, 46, 0f, -102f);
        Text statusText = CreateAnchoredText(tile.transform, string.Empty, 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 160, 26, 0f, 132f);
        statusText.gameObject.SetActive(false);

        CardRevealTileView view = tile.GetComponent<CardRevealTileView>();
        if (view == null)
        {
            view = tile.AddComponent<CardRevealTileView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(starsText, cardNameText, rarityText, potionNameText, statusText);
        view.Apply(new CardRevealTileDisplayModel
        {
            StarsText = BuildStarText(card.Stars),
            CardName = card.CardName,
            RarityText = card.RarityLabel,
            PotionName = card.PotionName,
            StatusText = card.IsNew ? string.Empty : (card.Copies > 1 ? $"Duplicate x{card.Copies}" : string.Empty)
        });

        if (card.IsNew)
        {
            CreateNewBadge(tile.transform, 58f, 132f, 82, 30, 14);
        }
    }

    private Color GetRevealCardColor(string rarityLabel)
    {
        if (rarityLabel == "Gilded")
        {
            return new Color(0.22f, 0.38f, 0.82f);
        }

        if (rarityLabel == "Ancient")
        {
            return new Color(0.36f, 0.24f, 0.58f);
        }

        return new Color(0.58f, 0.42f, 0.22f);
    }

    private void BuildRoundCollectedRewards(Transform parent, RewardPreview preview)
    {
        Dictionary<string, int> rewardCounts = new Dictionary<string, int>();

        for (int index = 0; index < preview.CollectedCellRewards.Count; index++)
        {
            CellReward reward = preview.CollectedCellRewards[index];
            string rewardName = reward.Name;
            int quantity = reward.Quantity;
            rewardCounts[rewardName] = rewardCounts.ContainsKey(rewardName) ? rewardCounts[rewardName] + quantity : quantity;
        }

        List<string> rewardsList = new List<string>();
        foreach (KeyValuePair<string, int> reward in rewardCounts)
        {
            rewardsList.Add(FormatRoundRewardCount(reward.Key, reward.Value));
        }

        if (rewardsList.Count == 0)
        {
            rewardsList.Add("No extra rewards");
        }

        int maxTiles = Mathf.Min(6, rewardsList.Count);
        float startX = maxTiles > 5 ? -375f : -360f;
        for (int index = 0; index < maxTiles; index++)
        {
            float x = startX + index * (maxTiles > 5 ? 150f : 180f);
            GameObject tile = CreateAnchoredPanel(parent, $"RewardTile_{index}", new Color(0.99f, 0.91f, 0.72f), maxTiles > 5 ? 136 : 156, 88, x, -30f);
            Text iconText = CreateAnchoredText(tile.transform, GetRewardTileIcon(rewardsList[index]), 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 120, 30, 0f, 18f);
            Text bodyText = CreateAnchoredText(tile.transform, rewardsList[index], 13, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), maxTiles > 5 ? 118 : 134, 44, 0f, -20f);
            RoundCollectedRewardTileView view = tile.GetComponent<RoundCollectedRewardTileView>();
            if (view == null)
            {
                view = tile.AddComponent<RoundCollectedRewardTileView>();
            }

            view.ResetRuntimeBindings();
            view.Initialize(iconText, bodyText);
            view.Apply(new RoundCollectedRewardTileDisplayModel
            {
                IconText = GetRewardTileIcon(rewardsList[index]),
                BodyText = rewardsList[index]
            });
        }
    }

    private string FormatRoundRewardCount(string rewardName, int quantity)
    {
        if (rewardName.Contains("Clairvoyance"))
        {
            return $"{rewardName} {quantity}m";
        }

        return $"{rewardName} x{quantity}";
    }

    private void BuildRoundIngredientProgress(Transform parent, RewardPreview preview)
    {
        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        Text titleText = CreateAnchoredText(parent, string.Empty, 22, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), 820, 30, 0f, -106f);
        Text potionNameText = CreateAnchoredText(parent, string.Empty, 18, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 360, 26, -260f, -140f);
        Text summaryText = CreateAnchoredText(parent, string.Empty, 16, FontStyle.Bold, new Color(0.16f, 0.38f, 0.08f), 420, 24, -230f, -166f);
        RoundIngredientProgressSectionView sectionView = parent.GetComponent<RoundIngredientProgressSectionView>();
        if (sectionView == null)
        {
            sectionView = parent.gameObject.AddComponent<RoundIngredientProgressSectionView>();
        }

        sectionView.ResetRuntimeBindings();
        sectionView.Initialize(titleText, potionNameText, summaryText);
        sectionView.Apply(new RoundIngredientProgressSectionDisplayModel
        {
            TitleText = "POTION PROGRESS",
            PotionNameText = RealmContentCatalog.ActivePrototypeRoom.PotionName,
            SummaryText = preview.IngredientDrops.Count == 0 ? "No ingredients added this round" : $"{preview.IngredientDrops.Count} ingredient types added this round"
        });

        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            int earned = GetIngredientDropQuantity(preview, requirement.Name);
            float x = -6f + index * 132f;
            GameObject tile = CreateAnchoredPanel(parent, $"IngredientTile_{index}", earned > 0 ? new Color(1f, 0.9f, 0.58f) : new Color(0.91f, 0.82f, 0.62f), 112, 78, x, -154f);
            Text quantityText = CreateAnchoredText(tile.transform, earned > 0 ? $"+{earned}" : "-", 21, FontStyle.Bold, earned > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.45f, 0.4f, 0.36f), 80, 28, 0f, 16f);
            Text nameText = CreateAnchoredText(tile.transform, GetShortRoundIngredientName(requirement.Name), 12, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), 96, 36, 0f, -18f);
            RoundIngredientProgressTileView view = tile.GetComponent<RoundIngredientProgressTileView>();
            if (view == null)
            {
                view = tile.AddComponent<RoundIngredientProgressTileView>();
            }

            view.ResetRuntimeBindings();
            view.Initialize(quantityText, nameText);
            view.Apply(new RoundIngredientProgressTileDisplayModel
            {
                QuantityText = earned > 0 ? $"+{earned}" : "-",
                NameText = GetShortRoundIngredientName(requirement.Name)
            });
        }
    }

    private int GetIngredientDropQuantity(RewardPreview preview, string ingredientName)
    {
        for (int index = 0; index < preview.IngredientDrops.Count; index++)
        {
            if (preview.IngredientDrops[index].Name == ingredientName)
            {
                return preview.IngredientDrops[index].Quantity;
            }
        }

        return 0;
    }

    private string GetRewardTileIcon(string reward)
    {
        if (reward.Contains("Crystal")) return "<>";
        if (reward.Contains("Clairvoyance")) return "EYE";
        if (reward.Contains("Pandora")) return "BOX";
        if (reward.Contains("Card")) return "CARD";
        if (reward.Contains("Orb")) return "ORB";
        if (reward.Contains("Fortune")) return "x2";
        if (reward.Contains("Single")) return "S1";
        if (reward.Contains("Multi")) return "S+";
        if (reward.Contains("Arcane")) return "SPRK";
        if (reward.Contains("Wild")) return "WILD";
        if (reward.Contains("Presto")) return "BINGO";
        return "+";
    }

    private string GetShortRoundIngredientName(string name)
    {
        string[] parts = name.Split(' ');
        return parts.Length <= 2 ? name : $"{parts[0]}\n{parts[1]}";
    }

    private Button CreateAnchoredButton(Transform parent, string label, int fontSize, float width, float height, Color color, float x, float y)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        Text text = CreateChildText(buttonObject.transform);
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;

        return button;
    }

    private void RedeemRewardsAndReturnHome()
    {
        bool shouldShowJackpotSpins = false;
        if (pendingRewardPreview != null)
        {
            shouldShowJackpotSpins = pendingRewardPreview.JackpotSpinsEarned > 0;
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.RoundRewardsCollected,
                PrototypeAnalyticsPayloadFactory.CreateRoundRewardsCollectedJson(pendingRewardPreview));
            inventory.Redeem(pendingRewardPreview);
            pendingRewardPreview = null;
        }

        rewardPreviewShown = false;
        if (inventory.LastAwardedAlbumCards.Count > 0)
        {
            cardRevealShouldShowJackpotSpins = shouldShowJackpotSpins;
            ShowCardReveal(inventory.LastAwardedAlbumCards);
            return;
        }

        if (shouldShowJackpotSpins && inventory.PendingJackpotSpins > 0)
        {
            BuildJackpotSpinUi();
            return;
        }

        BuildLobbyUi();
    }

    private void ContinueAfterCardReveal()
    {
        if (cardRevealShouldShowJackpotSpins && inventory.PendingJackpotSpins > 0)
        {
            cardRevealShouldShowJackpotSpins = false;
            BuildJackpotSpinUi();
            return;
        }

        cardRevealShouldShowJackpotSpins = false;
        BuildLobbyUi();
    }

    private void ActivatePrototypeClairvoyance()
    {
        if (!inventory.TryActivateClairvoyance())
        {
            return;
        }

        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        BuildLobbyUi();
    }

    private void ActivatePrototypeClairvoyanceFromInventory()
    {
        if (!inventory.TryActivateClairvoyance())
        {
            return;
        }

        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        RefreshPowerUpDisplays();
        BuildPowerUpInventoryUi();
    }

    private string GetPowerUpBankButtonText()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "BANK\n--";
        }

        if (!IsPowerUpBankReady())
        {
            return $"{GetPowerUpBadge(bankPowerUpName)} {GetShortPowerUpName(bankPowerUpName)}\nCharge {bankPowerUpCharge}/{GetPowerUpBankThreshold()}";
        }

        if (inventory.GetInventoryRewardCount(bankPowerUpName) > 0)
        {
            return $"{GetPowerUpBadge(bankPowerUpName)} READY\nTap";
        }

        return $"BUY {PlayerInventoryState.GameplayPowerUpCrystalCost}<>\nTap";
    }

    private string GetPowerUpBankStockText()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "No eligible power-up";
        }

        int stock = inventory.GetInventoryRewardCount(bankPowerUpName);
        return stock > 0 ? $"{bankPowerUpName} stock x{stock}" : $"{bankPowerUpName} stock 0";
    }

    private Color GetPowerUpBankButtonColor()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return new Color(0.24f, 0.22f, 0.28f);
        }

        if (!IsPowerUpBankReady())
        {
            return new Color(0.24f, 0.22f, 0.28f);
        }

        return inventory.GetInventoryRewardCount(bankPowerUpName) > 0
            ? new Color(0.35f, 0.12f, 0.62f)
            : new Color(0.08f, 0.42f, 0.62f);
    }

    private bool IsPowerUpBankReady()
    {
        return roundFlow.IsActive
            && !string.IsNullOrEmpty(bankPowerUpName)
            && bankPowerUpCharge >= GetPowerUpBankThreshold();
    }

    private int GetPowerUpBankThreshold()
    {
        return Mathf.Max(1, selectedCardCount);
    }

    private void SelectNextBankPowerUp(string message)
    {
        bankPowerUpName = DrawNextBankPowerUp();
        bankPowerUpCharge = 0;
        SetGameplayPowerUpEventText(string.IsNullOrEmpty(bankPowerUpName) ? "No eligible bank power-up" : $"{message}: {GetShortPowerUpName(bankPowerUpName)}");
        RefreshPowerUpDisplays();
    }

    private string DrawNextBankPowerUp()
    {
        if (bankPowerUpBag.Count == 0)
        {
            RefillBankPowerUpBag();
        }

        if (bankPowerUpBag.Count == 0)
        {
            return "";
        }

        string next = bankPowerUpBag[0];
        bankPowerUpBag.RemoveAt(0);
        return next;
    }

    private void RefillBankPowerUpBag()
    {
        bankPowerUpBag.Clear();
        List<string> eligible = inventory.GetEligibleGameplayPowerUps(selectedCardCount, manaBetPerCard, GetActiveProgression().Level);
        if (selectedCardCount >= 2 && Random.value < WildSigilBankOfferChance)
        {
            eligible.Add("Wild Sigil");
        }

        while (eligible.Count > 0)
        {
            int index = Random.Range(0, eligible.Count);
            bankPowerUpBag.Add(eligible[index]);
            eligible.RemoveAt(index);
        }
    }

    private void TrackPowerUpReleaseFromDaub()
    {
        if (!roundFlow.IsActive || string.IsNullOrEmpty(bankPowerUpName) || IsPowerUpBankReady())
        {
            return;
        }

        bankPowerUpCharge = Mathf.Min(GetPowerUpBankThreshold(), bankPowerUpCharge + 1);
        if (IsPowerUpBankReady())
        {
            SetGameplayPowerUpEventText(GetPowerUpBankChargedMessage());
            RefreshPowerUpDisplays();
            if (CanAutoReleasePowerUpBank())
            {
                StartCoroutine(UsePowerUpBankAfterDaub());
            }
            return;
        }

        SetGameplayPowerUpEventText($"{GetShortPowerUpName(bankPowerUpName)} charging {bankPowerUpCharge}/{GetPowerUpBankThreshold()}");
        RefreshPowerUpDisplays();
    }

    private IEnumerator UsePowerUpBankAfterDaub()
    {
        yield return null;
        UsePowerUpBank();
    }

    private void TogglePowerUpAutoDrop()
    {
        inventory.SetAutoDropPowerUps(!inventory.AutoDropPowerUps);
        SetGameplayPowerUpEventText(inventory.AutoDropPowerUps ? "Auto-drop enabled" : "Auto-drop off");
        RefreshPowerUpDisplays();
        if (CanAutoReleasePowerUpBank())
        {
            StartCoroutine(UsePowerUpBankAfterDaub());
        }
    }

    private bool CanAutoReleasePowerUpBank()
    {
        return inventory.AutoDropPowerUps
            && IsPowerUpBankReady()
            && !string.IsNullOrEmpty(bankPowerUpName)
            && inventory.GetInventoryRewardCount(bankPowerUpName) > 0;
    }

    private string GetPowerUpBankChargedMessage()
    {
        if (string.IsNullOrEmpty(bankPowerUpName))
        {
            return "No power-up selected";
        }

        if (inventory.GetInventoryRewardCount(bankPowerUpName) > 0)
        {
            return inventory.AutoDropPowerUps ? $"{GetShortPowerUpName(bankPowerUpName)} auto releasing" : $"{GetShortPowerUpName(bankPowerUpName)} charged";
        }

        return $"Out of stock. Tap to buy for {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals.";
    }

    private void UsePowerUpBank()
    {
        if (!IsPowerUpBankReady())
        {
            return;
        }

        string powerUpName = bankPowerUpName;
        bool usedInventory = inventory.GetInventoryRewardCount(powerUpName) > 0;
        if (usedInventory)
        {
            if (!inventory.TryConsumeInventoryReward(powerUpName, 1))
            {
                SetGameplayPowerUpEventText("Inventory use failed");
                RefreshPowerUpDisplays();
                return;
            }
        }
        else if (!inventory.TrySpendCrystals(PlayerInventoryState.GameplayPowerUpCrystalCost))
        {
            SetGameplayPowerUpEventText("Need 10 crystals");
            RefreshPowerUpDisplays();
            return;
        }

        SetGameplayPowerUpEventText(usedInventory ? $"Used {powerUpName} from inventory" : $"Bought {powerUpName} for {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals");
        ShowPowerUpBurst($"{GetPowerUpBadge(powerUpName)} {GetShortPowerUpName(powerUpName).ToUpperInvariant()}");
        RefreshPowerUpDisplays();
        bool selectNext = ApplyPowerUpEffect(powerUpName);
        if (!usedInventory && statusText != null)
        {
            statusText.text += $" Spent {PlayerInventoryState.GameplayPowerUpCrystalCost} crystals.";
        }

        if (selectNext && roundFlow.IsActive && !rewardPreviewShown)
        {
            SelectNextBankPowerUp("Next power-up charging");
        }
        else
        {
            RefreshPowerUpDisplays();
        }
    }

    private bool ApplyPowerUpEffect(string powerUpName)
    {
        if (powerUpName == "Single Sigil")
        {
            ApplyRandomMarksPerCard(powerUpName, 1);
            return true;
        }

        if (powerUpName == "Multi Sigil")
        {
            ApplyRandomMarksPerCard(powerUpName, 2);
            return true;
        }

        if (powerUpName == "Arcane Spark")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Fortune Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Wild Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Presto Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        if (powerUpName == "Pandora Sigil")
        {
            PlacePowerUpSigilOnCards(powerUpName);
            return true;
        }

        return true;
    }

    private void PlacePowerUpSigilOnCards(string powerUpName)
    {
        int placedCount = 0;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            if (!TryGetRandomPowerUpDropCell(cardIndex, out Vector2Int cell))
            {
                continue;
            }

            string key = $"{cardIndex}:{cell.x}:{cell.y}";
            placedPowerUpSigils[key] = powerUpName;
            placedCount++;
            RefreshPlacedPowerUpBadge(cardIndex, cell.x, cell.y);
        }

        string shortName = GetShortPowerUpName(powerUpName);
        SetGameplayPowerUpEventText(placedCount > 0 ? $"{shortName} dropped" : $"{shortName} found no square");
        ShowPowerUpBurst(placedCount > 0 ? $"{GetPowerUpBadge(powerUpName)} DROPPED x{placedCount}" : $"{GetPowerUpBadge(powerUpName)} NO OPEN CELL");
        if (statusText != null)
        {
            statusText.text = placedCount > 0
                ? $"{powerUpName} dropped onto {placedCount} card{(placedCount == 1 ? "" : "s")}. Daub its called square to trigger it."
                : $"{powerUpName} found no open square to drop onto.";
        }
    }

    private bool TryGetRandomPowerUpDropCell(int cardIndex, out Vector2Int cell)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        List<Vector2Int> calledCandidates = new List<Vector2Int>();
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                string key = $"{cardIndex}:{row}:{column}";
                if (IsMarkablePowerUpSquare(cardIndex, row, column) && !placedPowerUpSigils.ContainsKey(key))
                {
                    Vector2Int candidate = new Vector2Int(row, column);
                    candidates.Add(candidate);
                    int value = playerCards.GetNumber(cardIndex, row, column);
                    if (roundFlow.IsNumberCalled(value))
                    {
                        calledCandidates.Add(candidate);
                    }
                }
            }
        }

        if (calledCandidates.Count > 0)
        {
            cell = calledCandidates[Random.Range(0, calledCandidates.Count)];
            return true;
        }

        if (candidates.Count == 0)
        {
            cell = Vector2Int.zero;
            return false;
        }

        cell = candidates[Random.Range(0, candidates.Count)];
        return true;
    }

    private void RefreshPlacedPowerUpBadge(int cardIndex, int row, int column)
    {
        string key = $"{cardIndex}:{row}:{column}";
        if (!visibleCardCells.TryGetValue(key, out Button button))
        {
            return;
        }

        RemovePlacedPowerUpBadge(button.transform);
        if (!playerCards.IsMarked(cardIndex, row, column) && placedPowerUpSigils.TryGetValue(key, out string powerUpName))
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            float size = rect != null ? Mathf.Min(rect.rect.width, rect.rect.height) : CellSize;
            CreatePlacedPowerUpBadge(button.transform, GetPowerUpCellInitials(powerUpName), size);
        }
    }

    private int GetArcaneSparkMarkCount()
    {
        if (selectedCardCount <= 1) return 2;
        if (selectedCardCount == 2) return 5;
        if (selectedCardCount == 4) return 10;
        return 15;
    }

    private void ApplyRandomMarksPerCard(string powerUpName, int marksPerCard)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            AddRandomCardTargets(targets, cardIndex, marksPerCard);
        }

        ApplyPowerUpMarks(powerUpName, targets);
    }

    private void ApplyRandomMarksAcrossCards(string powerUpName, int markCount)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (IsMarkablePowerUpSquare(cardIndex, row, column))
                    {
                        candidates.Add(new Vector3Int(cardIndex, row, column));
                    }
                }
            }
        }

        List<Vector3Int> targets = new List<Vector3Int>();
        while (targets.Count < markCount && candidates.Count > 0)
        {
            int index = Random.Range(0, candidates.Count);
            targets.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        ApplyPowerUpMarks(powerUpName, targets);
    }

    private void AddRandomCardTargets(List<Vector3Int> targets, int cardIndex, int count)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (IsMarkablePowerUpSquare(cardIndex, row, column))
                {
                    candidates.Add(new Vector2Int(row, column));
                }
            }
        }

        while (count > 0 && candidates.Count > 0)
        {
            int index = Random.Range(0, candidates.Count);
            Vector2Int cell = candidates[index];
            candidates.RemoveAt(index);
            targets.Add(new Vector3Int(cardIndex, cell.x, cell.y));
            count--;
        }
    }

    private bool IsMarkablePowerUpSquare(int cardIndex, int row, int column)
    {
        return cardIndex >= 0
            && cardIndex < playerCards.Count
            && !(row == 2 && column == 2)
            && !playerCards.IsMarked(cardIndex, row, column);
    }

    private void ApplyPrestoSigil()
    {
        List<Vector3Int> targets = GetBestPrestoTargets();
        if (targets.Count == 0)
        {
            ApplyRandomMarksAcrossCards("Presto Sigil", 1);
            return;
        }

        ApplyPowerUpMarks("Presto Sigil", targets);
    }

    private List<Vector3Int> GetBestPrestoTargets()
    {
        List<Vector3Int> bestTargets = new List<Vector3Int>();
        int bestMarked = -1;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                EvaluatePrestoLine(cardIndex, GetLineTargets(cardIndex, true, row), ref bestMarked, ref bestTargets);
            }

            for (int column = 0; column < BoardSize; column++)
            {
                EvaluatePrestoLine(cardIndex, GetLineTargets(cardIndex, false, column), ref bestMarked, ref bestTargets);
            }

            EvaluatePrestoLine(cardIndex, GetDiagonalTargets(cardIndex, true), ref bestMarked, ref bestTargets);
            EvaluatePrestoLine(cardIndex, GetDiagonalTargets(cardIndex, false), ref bestMarked, ref bestTargets);
        }

        return bestTargets;
    }

    private List<Vector3Int> GetLineTargets(int cardIndex, bool rowLine, int index)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int offset = 0; offset < BoardSize; offset++)
        {
            int row = rowLine ? index : offset;
            int column = rowLine ? offset : index;
            targets.Add(new Vector3Int(cardIndex, row, column));
        }

        return targets;
    }

    private List<Vector3Int> GetDiagonalTargets(int cardIndex, bool leftToRight)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        for (int index = 0; index < BoardSize; index++)
        {
            targets.Add(new Vector3Int(cardIndex, index, leftToRight ? index : BoardSize - 1 - index));
        }

        return targets;
    }

    private void EvaluatePrestoLine(int cardIndex, List<Vector3Int> line, ref int bestMarked, ref List<Vector3Int> bestTargets)
    {
        int markedCount = 0;
        List<Vector3Int> unmarked = new List<Vector3Int>();
        for (int index = 0; index < line.Count; index++)
        {
            Vector3Int cell = line[index];
            if (playerCards.IsMarked(cardIndex, cell.y, cell.z))
            {
                markedCount++;
                continue;
            }

            if (!(cell.y == 2 && cell.z == 2))
            {
                unmarked.Add(cell);
            }
        }

        if (unmarked.Count == 0)
        {
            return;
        }

        if (markedCount > bestMarked)
        {
            bestMarked = markedCount;
            bestTargets = unmarked;
        }
    }

    private void ApplyPowerUpMarks(string powerUpName, List<Vector3Int> targets)
    {
        if (targets.Count == 0)
        {
            if (statusText != null)
            {
                statusText.text = $"{powerUpName} found no valid squares.";
            }
            RefreshPowerUpDisplays();
            return;
        }

        int totalNewBingos = 0;
        int markedSquares = 0;
        List<string> collectedRewards = new List<string>();
        for (int index = 0; index < targets.Count; index++)
        {
            Vector3Int target = targets[index];
            if (!IsMarkablePowerUpSquare(target.x, target.y, target.z))
            {
                continue;
            }

            int previousBingoCount = playerCards.GetBingoCount(target.x);
            playerCards.Mark(target.x, target.y, target.z);
            markedSquares++;
            powerUpAssistedMarks++;

            string key = $"{target.x}:{target.y}:{target.z}";
            powerUpMarkedCellLabels[key] = GetPowerUpCellInitials(powerUpName);
            placedPowerUpSigils.Remove(key);
            powerUps.ClearCellAlert(key);
            if (cellRewards.TryCollect(key, out CellReward collectedReward))
            {
                string displayQuantity = CellRewardTracker.GetDisplayQuantity(collectedReward);
                collectedRewards.Add($"{collectedReward.Name} {displayQuantity}");
                if (collectedReward.Kind == CellRewardKind.Crystals)
                {
                    ShowPowerUpBurst($"CRYSTALS {displayQuantity}");
                }
            }

            UpdateCardBingoProgress(target.x);
            int newBingoCount = playerCards.GetBingoCount(target.x);
            int bingoDelta = Mathf.Max(0, newBingoCount - previousBingoCount);
            totalNewBingos += IsBlackoutRoom() && bingoDelta > 0 ? 1 : bingoDelta;
            RefreshPowerUpMarkedCell(target.x, target.y, target.z);

            if (newBingoCount >= BingoRoomRules.JackpotBingosPerCard)
            {
                lastJackpotEarnedCardIndex = target.x;
                roundFlow.MarkJackpotState();
            }
        }

        if (totalNewBingos > 0)
        {
            if (!IsBlackoutRoom())
            {
                ConsumeRoomBingos(totalNewBingos, false);
            }
            else
            {
                RefreshRoomPoolTexts();
            }

            AddXp(totalNewBingos * GetScaledXp(roomRules.BingoClaimXp), $"{powerUpName} Bingo");
        }

        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        string rewardText = collectedRewards.Count > 0 ? $" Collected {string.Join(", ", collectedRewards)}." : "";
        if (statusText != null)
        {
            statusText.text = $"{powerUpName} marked {markedSquares} square{(markedSquares == 1 ? "" : "s")}.{rewardText}";
        }

        ShowPowerUpBurst($"{GetPowerUpBadge(powerUpName)} MARKED {markedSquares}");
        RefreshPowerUpDisplays();
        CompleteRoundIfPowerUpEndedGame(powerUpName);
    }

    private void RefreshPowerUpMarkedCell(int cardIndex, int row, int column)
    {
        string key = $"{cardIndex}:{row}:{column}";
        if (!visibleCardCells.TryGetValue(key, out Button button))
        {
            return;
        }

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = GetVisibleCellColor(true, playerCards.IsWinningCell(cardIndex, row, column));
        }

        RefreshVisibleCellLabel(button.transform, cardIndex, row, column);

        RemoveRewardBadge(button.transform);
        RemovePlacedPowerUpBadge(button.transform);
        StartCoroutine(PulseCell(button.transform));
        RefreshVisibleCardWinningCells(cardIndex);
    }

    private void RefreshVisibleCellLabel(Transform cellTransform, int cardIndex, int row, int column)
    {
        Text label = GetCellNumberLabel(cellTransform);
        if (label == null)
        {
            return;
        }

        bool isMarked = cardIndex >= 0 && cardIndex < playerCards.Count && playerCards.IsMarked(cardIndex, row, column);
        int value = cardIndex >= 0 && cardIndex < playerCards.Count ? playerCards.GetNumber(cardIndex, row, column) : 0;
        label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
        label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
    }

    private void CompleteRoundIfPowerUpEndedGame(string powerUpName)
    {
        BingoRoundEndDecision powerUpDecision = BingoRoundEndRules.ResolvePostProgressDecision(
            IsBlackoutRoom(),
            AllCardsReachedJackpot(),
            IsPrototypeBingoPoolExhausted(),
            $"Room bingo pool exhausted by {powerUpName} at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.",
            IsStandardCoverageLimitReached(),
            $"Standard room coverage limit reached after {powerUpName}.");
        TryStartResolvedRoundEndCountdown(powerUpDecision);
    }

    private void SetGameplayPowerUpEventText(string message)
    {
        if (gameplayPowerUpEventText != null)
        {
            gameplayPowerUpEventText.text = message;
        }
    }

    private void ShowPowerUpBurst(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        SetGameplayPowerUpEventText(message);
        if (gameplayPowerUpBurstText == null)
        {
            return;
        }

        if (powerUpBurstRoutine != null)
        {
            StopCoroutine(powerUpBurstRoutine);
        }

        powerUpBurstRoutine = StartCoroutine(ShowPowerUpBurstRoutine(message));
    }

    private IEnumerator ShowPowerUpBurstRoutine(string message)
    {
        gameplayPowerUpBurstText.text = message;
        gameplayPowerUpBurstText.color = new Color(0.72f, 0.95f, 1f, 1f);
        gameplayPowerUpBurstText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        powerUpBurstRoutine = null;
    }

    private string GetPowerUpBadge(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "S1";
        if (powerUpName == "Multi Sigil") return "S+";
        if (powerUpName == "Arcane Spark") return "SPRK";
        if (powerUpName == "Fortune Sigil") return "x2";
        if (powerUpName == "Wild Sigil") return "WILD";
        if (powerUpName == "Presto Sigil") return "BINGO";
        if (powerUpName == "Pandora Sigil") return "BOX";
        return "PU";
    }

    private string GetPowerUpCellInitials(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "S1";
        if (powerUpName == "Multi Sigil") return "M+";
        if (powerUpName == "Arcane Spark") return "SP";
        if (powerUpName == "Fortune Sigil") return "x2";
        if (powerUpName == "Wild Sigil") return "W";
        if (powerUpName == "Presto Sigil") return "P";
        if (powerUpName == "Pandora Sigil") return "BX";
        return "PU";
    }

    private string GetCompactPowerUpGrantSummary(List<string> grantedPowerUps)
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        for (int index = 0; index < grantedPowerUps.Count; index++)
        {
            string name = grantedPowerUps[index];
            counts[name] = counts.ContainsKey(name) ? counts[name] + 1 : 1;
        }

        List<string> summary = new List<string>();
        foreach (KeyValuePair<string, int> grant in counts)
        {
            summary.Add($"{GetShortPowerUpName(grant.Key)} x{grant.Value}");
        }

        return string.Join(", ", summary);
    }

    private string GetShortPowerUpName(string powerUpName)
    {
        if (powerUpName == "Single Sigil") return "Single";
        if (powerUpName == "Multi Sigil") return "Multi";
        if (powerUpName == "Arcane Spark") return "Spark";
        if (powerUpName == "Fortune Sigil") return "Fortune";
        if (powerUpName == "Wild Sigil") return "Wild";
        if (powerUpName == "Presto Sigil") return "Presto";
        if (powerUpName == "Pandora Sigil") return "Pandora";
        return powerUpName;
    }

    private void GrantPrototypeJackpotSpin()
    {
        GrantPrototypeJackpotSpin(false);
    }

    private void GrantPrototypeJackpotSpin(bool returnToMap)
    {
        inventory.AddPrototypeJackpotSpin();
        RefreshDevSettingsUi(returnToMap);
    }

    private void RestoreActiveRoom()
    {
        if (!IsRoomUnlocked(RealmContentCatalog.ActivePrototypeRoomIndex))
        {
            return;
        }

        if (!inventory.TryRestoreActiveRoom())
        {
            return;
        }

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.RoomRestored,
            PrototypeAnalyticsPayloadFactory.CreateRoomRestoredJson(
                selectedCardCount,
                manaBetPerCard,
                RealmContentCatalog.ActivePrototypeRoom.Progression.RestoreManaReward));

        int restoredRealmIndex = RealmContentCatalog.ActivePrototypeRealmIndex;
        if (IsRealmComplete(restoredRealmIndex))
        {
            // A newly completed realm always begins at the restored-open state.
            DurablePrefs.DeleteKey(WorldMapRestoredClosedSavePrefix + restoredRealmIndex);
            DurablePrefs.Save();
            worldMapRestoringRealmIndex = restoredRealmIndex;
            BuildWorldMapUi();
            return;
        }

        BuildRealmMapUi();
    }

    private void MaybeSimulatedPlayersClaimBingo()
    {
        if (!rewards.ShouldSimulatedPlayerClaim(roundFlow.History.Count))
        {
            return;
        }

        int simulatedClaimCount = roundFlow.History.Count >= 32 ? 2 : 1;
        ConsumeRoomBingos(simulatedClaimCount, true);
        rewards.ScheduleNextSimulatedClaim();
    }

    private void ConsumeRoomBingos(int count, bool simulated)
    {
        rewards.ConsumeRoomBingos(count, simulated);
        RefreshRoomPoolTexts();
    }

    private void RefreshRoomPoolTexts()
    {
        if (roomBingoCountText != null)
        {
            roomBingoCountText.text = IsBlackoutRoom()
                ? $"{GetTotalBingos()}/{selectedCardCount}"
                : $"{rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}";
        }

        if (roundSummaryText != null)
        {
            roundSummaryText.text = GetRoomPoolSummaryText();
        }
    }

    private IEnumerator TriggerClairvoyanceAlert(int calledNumber)
    {
        yield return new WaitForSeconds(roomRules.GetClairvoyanceAlertDelay(selectedCardCount));

        if (!roundFlow.IsActive || !powerUps.ClairvoyanceActive || !roundFlow.IsNumberCalled(calledNumber))
        {
            yield break;
        }

        bool alerted = false;
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (row == 2 && column == 2)
                    {
                        continue;
                    }

                    if (playerCards.GetNumber(cardIndex, row, column) != calledNumber || playerCards.IsMarked(cardIndex, row, column))
                    {
                        continue;
                    }

                    string visibleKey = $"{cardIndex}:{row}:{column}";
                    if (!visibleCardCells.TryGetValue(visibleKey, out Button button))
                    {
                        continue;
                    }

                    powerUps.AlertCell(visibleKey);
                    StartCoroutine(FlashClairvoyanceCell(button.transform, cardIndex, row, column));
                    alerted = true;
                }
            }
        }

        if (alerted && statusText != null)
        {
            statusText.text = "Clairvoyance reveals a called number. Daub it to keep earning XP.";
        }
    }

    private string GetBingoBannerText()
    {
        List<string> parts = new List<string>();
        if (lastJackpotEarnedCardIndex >= 0)
        {
            parts.Add(IsBlackoutRoom()
                ? $"BLACKOUT WHEELSPIN EARNED: CARD {lastJackpotEarnedCardIndex + 1}"
                : $"WHEELSPIN EARNED: CARD {lastJackpotEarnedCardIndex + 1}");
        }

        for (int index = 0; index < playerCards.Count; index++)
        {
            if (IsBlackoutRoom())
            {
                int marked = playerCards.CountMarkedPlayable(index);
                if (marked <= 0)
                {
                    continue;
                }

                parts.Add(marked >= BingoRoomRules.BlackoutPlayableSquares
                    ? $"Card {index + 1}: BLACKOUT"
                    : $"Card {index + 1}: {marked}/{BingoRoomRules.BlackoutPlayableSquares}");
                continue;
            }

            int count = playerCards.GetBingoCount(index);
            if (count <= 0)
            {
                continue;
            }

            parts.Add(count >= BingoRoomRules.JackpotBingosPerCard
                ? $"Card {index + 1}: JACKPOT"
                : $"Card {index + 1}: {count}/{BingoRoomRules.JackpotBingosPerCard}");
        }

        return parts.Count == 0 ? "" : string.Join("  |  ", parts);
    }

    private bool AllCardsReachedJackpot()
    {
        return playerCards.AllCardsReachedJackpot();
    }

    private bool IsPrototypeBingoPoolExhausted()
    {
        return rewards.IsRoomPoolExhausted;
    }

    private bool HasNumberOnAnyCard(int calledNumber)
    {
        return playerCards.HasNumberOnAnyCard(calledNumber);
    }

    private void UpdateCardBingoProgress(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= playerCards.Count)
        {
            return;
        }

        if (IsBlackoutRoom())
        {
            playerCards.RefreshBlackoutProgress(cardIndex);
        }
        else
        {
            playerCards.RefreshBingoProgress(cardIndex);
        }

        if (visibleCardBingoTexts.TryGetValue(cardIndex, out Text bingoText))
        {
            bingoText.text = GetCardBingoText(cardIndex);
        }

        if (roundSummaryText != null)
        {
            roundSummaryText.text = GetRoomPoolSummaryText();
        }
    }

    private void RefreshVisibleCardWinningCells(int cardIndex)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                string visibleKey = $"{cardIndex}:{row}:{column}";
                if (!visibleCardCells.TryGetValue(visibleKey, out Button button))
                {
                    continue;
                }

                bool isMarked = playerCards.IsMarked(cardIndex, row, column);
                bool isWinningCell = playerCards.IsWinningCell(cardIndex, row, column);
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = GetVisibleCellColor(isMarked, isWinningCell);
                }

                Text label = GetCellNumberLabel(button.transform);
                if (label != null)
                {
                    int value = playerCards.GetNumber(cardIndex, row, column);
                    label.text = GetVisibleCellLabelText(cardIndex, row, column, value, isMarked);
                    label.color = GetVisibleCellLabelColor(cardIndex, row, column, isMarked);
                }
            }
        }
    }

    private void HighlightWinningCard(int cardIndex)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (!playerCards.IsMarked(cardIndex, row, column))
                {
                    continue;
                }

                string key = $"{cardIndex}:{row}:{column}";
                if (!visibleCardCells.TryGetValue(key, out Button button))
                {
                    continue;
                }

                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(1f, 0.72f, 0.12f);
                }

                Text label = GetCellNumberLabel(button.transform);
                if (label != null)
                {
                    RefreshVisibleCellLabel(button.transform, cardIndex, row, column);
                }

                StartCoroutine(PulseCell(button.transform));
            }
        }
    }

    private IEnumerator PulseCell(Transform cellTransform)
    {
        Image image = cellTransform.GetComponent<Image>();
        Color settleColor = image != null ? image.color : new Color(0.19f, 0.78f, 0.45f);
        Color flashColor = new Color(1f, 0.78f, 0.18f);
        Vector3 baseScale = cellTransform.localScale;
        Vector3 pulseScale = baseScale * 1.22f;

        if (image != null)
        {
            image.color = flashColor;
        }

        float elapsed = 0f;
        const float pulseUpDuration = 0.14f;
        while (elapsed < pulseUpDuration)
        {
            elapsed += Time.deltaTime;
            cellTransform.localScale = Vector3.Lerp(baseScale, pulseScale, elapsed / pulseUpDuration);
            yield return null;
        }

        elapsed = 0f;
        const float settleDuration = 0.22f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            cellTransform.localScale = Vector3.Lerp(pulseScale, baseScale, elapsed / settleDuration);
            if (image != null)
            {
                image.color = Color.Lerp(flashColor, settleColor, elapsed / settleDuration);
            }
            yield return null;
        }

        cellTransform.localScale = baseScale;
        if (image != null)
        {
            image.color = settleColor;
        }
    }

    private void ToggleCell(int row, int column)
    {
        if (row == 2 && column == 2)
        {
            statusText.text = "FREE space is already yours.";
            return;
        }

        int value = numbers[row, column];
        if (marked[row, column])
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} is already daubed.";
            return;
        }

        if (!roundFlow.IsNumberCalled(value))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} has not been called yet.";
            StartCoroutine(FlashCell(cells[row, column].transform, new Color(0.95f, 0.22f, 0.26f), new Color(0.95f, 0.97f, 1f)));
            return;
        }

        marked[row, column] = true;
        SaveCurrentCard();
        UpdateCell(row, column);
        StartCoroutine(PulseCell(cells[row, column].transform));

        if (!roundFlow.HasJackpotState && HasWinningLine())
        {
            roundFlow.MarkJackpotState();
            roundFlow.StopRound();
            statusText.text = $"BINGO on Card {currentCardIndex + 1}! Reward preview: {selectedCardCount * manaBetPerCard} Mana entry cleared.";
            return;
        }

        statusText.text = $"{GetBingoLetter(value)}-{value} daubed. Keep going.";
    }

    private void UpdateCell(int row, int column)
    {
        Image image = cells[row, column].GetComponent<Image>();
        image.color = marked[row, column] ? new Color(0.19f, 0.78f, 0.45f) : new Color(0.95f, 0.97f, 1f);

        Text label = cells[row, column].GetComponentInChildren<Text>();
        label.color = marked[row, column] ? Color.white : new Color(0.08f, 0.1f, 0.16f);
    }

    private bool HasWinningLine()
    {
        for (int index = 0; index < BoardSize; index++)
        {
            if (IsRowMarked(index) || IsColumnMarked(index))
            {
                return true;
            }
        }

        return IsDiagonalMarked(true) || IsDiagonalMarked(false);
    }

    private bool HasWinningLine(int cardIndex)
    {
        bool[,] marks = playerCards.Marks[cardIndex];

        for (int index = 0; index < BoardSize; index++)
        {
            bool rowComplete = true;
            bool columnComplete = true;

            for (int offset = 0; offset < BoardSize; offset++)
            {
                rowComplete &= marks[index, offset];
                columnComplete &= marks[offset, index];
            }

            if (rowComplete || columnComplete)
            {
                return true;
            }
        }

        bool leftToRight = true;
        bool rightToLeft = true;
        for (int index = 0; index < BoardSize; index++)
        {
            leftToRight &= marks[index, index];
            rightToLeft &= marks[index, BoardSize - 1 - index];
        }

        return leftToRight || rightToLeft;
    }

    private bool IsRowMarked(int row)
    {
        for (int column = 0; column < BoardSize; column++)
        {
            if (!marked[row, column])
            {
                return false;
            }
        }

        return true;
    }

    private bool IsColumnMarked(int column)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            if (!marked[row, column])
            {
                return false;
            }
        }

        return true;
    }

    private bool IsDiagonalMarked(bool leftToRight)
    {
        for (int index = 0; index < BoardSize; index++)
        {
            int column = leftToRight ? index : BoardSize - 1 - index;
            if (!marked[index, column])
            {
                return false;
            }
        }

        return true;
    }

    private string GetBingoLetter(int number)
    {
        if (number <= 15) return "B";
        if (number <= 30) return "I";
        if (number <= 45) return "N";
        if (number <= 60) return "G";
        return "O";
    }

    private string GetCalledHistoryText()
    {
        int count = Mathf.Min(roundFlow.History.Count, 8);
        List<string> recentCalls = new List<string>();

        for (int index = 0; index < count; index++)
        {
            int number = roundFlow.History[index];
            recentCalls.Add($"{GetBingoLetter(number)}-{number}");
        }

        return string.Join("  ", recentCalls);
    }

    private string GetCalledHistoryPanelText()
    {
        if (roundFlow.History.Count == 0)
        {
            return allCalledVisible ? "No calls yet." : "Hidden";
        }

        if (!allCalledVisible)
        {
            return "Tap Show to reveal the full call history.";
        }

        string missedSummary = GetMissedDaubSummaryText();
        if (missedSummary.Length == 0)
        {
            return GetAllCalledHistoryText();
        }

        return $"<color=#ffcc33><b>MISSED DAUBS</b></color>\n{missedSummary}\n\nALL CALLS\n{GetAllCalledHistoryText()}";
    }

    private string GetAllCalledHistoryText()
    {
        List<string> calls = new List<string>();
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            int number = roundFlow.History[index];
            string call = $"{GetBingoLetter(number)}-{number}";
            calls.Add(IsMissedDaubNumber(number) ? $"<color=#ffcc33><b>{call}</b></color>" : call);
        }

        return string.Join("  ", calls);
    }

    private int GetCalledHistoryPanelFontSize()
    {
        if (!allCalledVisible)
        {
            return 18;
        }

        return HasMissedDaubs() ? 14 : 15;
    }

    private void ToggleAllCalledVisible()
    {
        allCalledVisible = !allCalledVisible;
        BuildUi();
    }

    private void RefreshCalledBallDisplays()
    {
        if (calledNumberText != null)
        {
            calledNumberText.fontSize = 46;
            calledNumberText.text = roundFlow.History.Count > 0 ? $"{GetBingoLetter(roundFlow.History[0])}-{roundFlow.History[0]}" : "Starting";
        }

        if (calledHistoryText != null)
        {
            calledHistoryText.text = GetCalledHistoryPanelText();
            calledHistoryText.fontSize = GetCalledHistoryPanelFontSize();
        }

        for (int index = 0; index < callQueueTexts.Count; index++)
        {
            callQueueTexts[index].text = GetCallQueueLabel(index);
        }
    }

    private string GetCallQueueLabel(int index)
    {
        int historyIndex = index + 1;
        if (roundFlow.History.Count <= historyIndex)
        {
            return "-";
        }

        int number = roundFlow.History[historyIndex];
        return $"{GetBingoLetter(number)}-{number}";
    }

    private bool HasMissedDaubs()
    {
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            if (IsMissedDaubNumber(roundFlow.History[index]))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMissedDaubNumber(int number)
    {
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    if (row == 2 && column == 2)
                    {
                        continue;
                    }

                    if (playerCards.GetNumber(cardIndex, row, column) == number && !playerCards.IsMarked(cardIndex, row, column))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private string GetMissedDaubSummaryText()
    {
        List<string> missed = new List<string>();
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            int number = roundFlow.History[index];
            if (!IsMissedDaubNumber(number))
            {
                continue;
            }

            missed.Add($"{GetBingoLetter(number)}-{number} {GetMissedDaubCardText(number)}");
            if (missed.Count >= 6)
            {
                break;
            }
        }

        if (missed.Count == 0)
        {
            return "";
        }

        string suffix = CountMissedDaubNumbers() > missed.Count ? "\nMore missed calls highlighted below." : "";
        return string.Join("\n", missed) + suffix;
    }

    private string GetMissedDaubCardText(int number)
    {
        List<string> cards = new List<string>();
        for (int cardIndex = 0; cardIndex < playerCards.Count; cardIndex++)
        {
            if (CardHasUnmarkedCalledNumber(cardIndex, number))
            {
                cards.Add((cardIndex + 1).ToString());
            }
        }

        return cards.Count == 0 ? "" : $"Card {string.Join(",", cards)}";
    }

    private bool CardHasUnmarkedCalledNumber(int cardIndex, int number)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (row == 2 && column == 2)
                {
                    continue;
                }

                if (playerCards.GetNumber(cardIndex, row, column) == number && !playerCards.IsMarked(cardIndex, row, column))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CountMissedDaubNumbers()
    {
        int count = 0;
        for (int index = 0; index < roundFlow.History.Count; index++)
        {
            if (IsMissedDaubNumber(roundFlow.History[index]))
            {
                count++;
            }
        }

        return count;
    }

    private void AdjustBet(int delta)
    {
        if (roundFlow.IsActive || roundFlow.History.Count > 0)
        {
            statusText.text = "Bet is locked once the round has started.";
            return;
        }

        RoomProgressionProfile progression = GetActiveProgression();
        manaBetPerCard = Mathf.Clamp(manaBetPerCard + delta, progression.MinManaBet, progression.MaxManaBet);
        inventory.SaveManaBetForActiveRoom(manaBetPerCard);
        RefreshRoomText();
        statusText.text = $"Mana bet set to {manaBetPerCard} per card.";
    }

    private void RefreshRoomText()
    {
        if (roomText == null)
        {
            return;
        }

        int totalCost = selectedCardCount * manaBetPerCard;
        roomText.text = $"{RealmContentCatalog.ActivePrototypeRoom.Name}  |  {selectedCardCount} Cards  |  {manaBetPerCard} Mana/Card  |  Entry {totalCost} Mana";
    }

    private IEnumerator FlashCell(Transform cellTransform, Color flashColor, Color settleColor)
    {
        yield return FlashCell(cellTransform, flashColor, settleColor, 1, 0.18f);
    }

    private IEnumerator FlashClairvoyanceCell(Transform cellTransform, int cardIndex, int row, int column)
    {
        Text label = GetCellNumberLabel(cellTransform);
        if (label == null)
        {
            yield break;
        }

        Color flashColor = new Color(0.42f, 0.95f, 1f);
        string visibleKey = $"{cardIndex}:{row}:{column}";

        while (roundFlow.IsActive
            && powerUps.ClairvoyanceActive
            && cardIndex >= 0
            && cardIndex < playerCards.Count
            && powerUps.IsCellAlerted(visibleKey)
            && !playerCards.IsMarked(cardIndex, row, column))
        {
            label.color = flashColor;
            yield return new WaitForSeconds(0.28f);
            label.color = GetCurrentVisibleCellTextColor(cardIndex, row, column);
            yield return new WaitForSeconds(0.28f);
        }

        label.color = GetCurrentVisibleCellTextColor(cardIndex, row, column);
    }

    private Color GetCurrentVisibleCellColor(int cardIndex, int row, int column)
    {
        bool isMarked = cardIndex >= 0 && cardIndex < playerCards.Count && playerCards.IsMarked(cardIndex, row, column);
        bool isWinningCell = playerCards.IsWinningCell(cardIndex, row, column);

        return GetVisibleCellColor(isMarked, isWinningCell);
    }

    private Color GetCurrentVisibleCellTextColor(int cardIndex, int row, int column)
    {
        bool isMarked = cardIndex >= 0 && cardIndex < playerCards.Count && playerCards.IsMarked(cardIndex, row, column);
        return isMarked ? Color.white : new Color(0.08f, 0.1f, 0.16f);
    }

    private IEnumerator FlashCell(Transform cellTransform, Color flashColor, Color settleColor, int repeats, float interval)
    {
        Image image = cellTransform.GetComponent<Image>();
        if (image == null)
        {
            yield break;
        }

        for (int index = 0; index < repeats; index++)
        {
            image.color = flashColor;
            yield return new WaitForSeconds(interval);
            image.color = settleColor;
            yield return new WaitForSeconds(interval);
        }

        image.color = settleColor;
    }
}

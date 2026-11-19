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

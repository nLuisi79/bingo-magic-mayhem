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
    private Text lobbyBetMultiplierText;
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
        playerCards.Generate(count, GetBetIngredientMultiplier());
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
        if (lobbyBetMultiplierText != null)
        {
            lobbyBetMultiplierText.text = $"<size=30>x{GetBetIngredientMultiplier()}</size>\n<size=12>POTION</size>";
        }
    }

    private int GetBetIngredientMultiplier()
    {
        return IngredientMultiplier.ForBet(manaBetPerCard, GetActiveProgression().MaxManaBet);
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

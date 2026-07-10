using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoMagicMayhem.Cosmetics;
using BingoMagicMayhem.Infrastructure;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BingoPrototype : MonoBehaviour
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
    private const float WildSigilBankOfferChance = 1f / 15f;
    private const string PrototypeSocialFreebieCode = "social-spark";
    private const string PrototypeInstagramUrl = "https://www.instagram.com/bingomagicmayhem";
    private const string PrototypeFacebookUrl = "https://www.facebook.com/bingomagicmayhem";
    private const string PrototypeXUrl = "https://x.com/bingomagicmayhem";
    private const int SocialFreebieExpirationDays = 14;
    private const int PrototypeFriendManaAmount = 10;
    private const int PrototypeFriendDailyManaLimit = 3;
    private const string PrototypeFriendsSavePrefix = "BMM.Prototype.Friends.";
    private const char PrototypeFriendSaveSeparator = '|';
    private const string PrototypeCovenDiscoverySaveKey = "BMM.Prototype.CovenDiscovery.Requests";
    private const string PrototypeTrailRewardsSaveKey = "BMM.Prototype.EnchantedTrail.ClaimedRewards";
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

    private readonly int[,] numbers = new int[BoardSize, BoardSize];
    private readonly bool[,] marked = new bool[BoardSize, BoardSize];
    private readonly Button[,] cells = new Button[BoardSize, BoardSize];
    private readonly PlayerCardSet playerCards = new PlayerCardSet();
    private readonly Dictionary<string, Button> visibleCardCells = new Dictionary<string, Button>();
    private readonly Dictionary<int, Text> visibleCardBingoTexts = new Dictionary<int, Text>();
    private readonly List<int> callPool = new List<int>();
    private readonly List<int> calledHistory = new List<int>();
    private readonly HashSet<int> calledNumbers = new HashSet<int>();
    private readonly Dictionary<int, float> calledAtTimes = new Dictionary<int, float>();
    private readonly List<Text> callQueueTexts = new List<Text>();
    private readonly List<RectTransform> jackpotWheelSegmentRects = new List<RectTransform>();
    private readonly Dictionary<string, string> powerUpMarkedCellLabels = new Dictionary<string, string>();
    private readonly Dictionary<string, string> placedPowerUpSigils = new Dictionary<string, string>();
    private readonly BingoRoomRules roomRules = new BingoRoomRules();
    private readonly BingoCaller caller = new BingoCaller();
    private readonly PowerUpRuntimeState powerUps = new PowerUpRuntimeState();
    private readonly CellRewardTracker cellRewards = new CellRewardTracker();
    private readonly PlayerInventoryState inventory = new PlayerInventoryState();
    private readonly CovenState coven = new CovenState();
    private BingoRoundState roundState;
    private BingoRewardTracker rewards;
    private RewardPreview pendingRewardPreview;

    private Font uiFont;
    private Transform contentRoot;
    private RectTransform stageRect;
    private Vector2 lastRootSize;
    private float activeStageWidth = StageWidth;
    private float activeStageHeight = StageHeight;
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
    private Text powerUpInventoryText;
    private Text lobbyClairvoyanceStatusText;
    private Text denManaCauldronText;
    private Text cauldronAmountText;
    private Text cauldronStatusText;
    private Button cauldronCollectButton;
    private PrototypeInboxCategory inboxActiveCategory = PrototypeInboxCategory.Gifts;
    private string lastInboxClaimSummary = "";
    private string lastLibraryCardGiftSummary = "";
    private string playerProfileActiveTab = "Profile";
    private string prototypeLeaderboardTab = "Friends";
    private bool prototypeSoundEnabled = true;
    private bool prototypeNotificationsEnabled = true;
    private string lastInfrastructureDiagnosticsExport = "";
    private readonly HashSet<int> prototypeClaimedTrailRewards = new HashSet<int>();
    private readonly HashSet<int> prototypeOracleSelectedCards = new HashSet<int>();
    private readonly HashSet<string> prototypeFriends = new HashSet<string> { "Luna", "Eldric", "Mira" };
    private readonly HashSet<string> prototypeIncomingFriendRequests = new HashSet<string> { "Aster", "Rowan" };
    private readonly HashSet<string> prototypeSentFriendRequests = new HashSet<string> { "Juniper" };
    private readonly HashSet<string> prototypeFriendsSentManaToday = new HashSet<string>();
    private readonly HashSet<string> prototypeFriendsReceivedManaToday = new HashSet<string>();
    private readonly HashSet<string> prototypeBlockedFriends = new HashSet<string>();
    private readonly HashSet<string> prototypeReportedFriends = new HashSet<string>();
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
    private int jackpotWheelCollectedMana;
    private int jackpotWheelLastCollectedMana;
    private bool jackpotWheelLastCollectResetPot;
    private readonly List<JackpotSpinResult> jackpotWheelCollectedResults = new List<JackpotSpinResult>();
    private JackpotSpinResult lastJackpotSpinResult;
    private JackpotSpinResult pendingJackpotSpinResult;
    private bool jackpotWheelAnimating;
    private int jackpotWheelAnimationSequence;
    private float jackpotWheelRotation;
    private int jackpotWheelTargetSegment;
    private bool rewardPreviewShown;
    private bool allCalledVisible;
    private bool jackpotWheelCollectionConfirmed;
    private int lastJackpotEarnedCardIndex = -1;
    private bool cardRevealShouldShowJackpotSpins;
    private float powerUpSaveAccumulator;
    private Coroutine powerUpBurstRoutine;
    private Coroutine finalBallCountdownRoutine;
    private bool finalBallCountdownActive;

    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeAvatars = CosmeticCatalog.Avatars;
    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeAvatarFrames = CosmeticCatalog.Frames;
    private static readonly IReadOnlyList<CosmeticDefinition> PrototypeDaubers = CosmeticCatalog.Daubers;
    private static readonly ICosmeticSpriteResolver CosmeticSpriteResolver = new ResourcesCosmeticSpriteResolver();

    private sealed class JackpotSpinResult
    {
        public JackpotSpinResult(string label, int manaAward, bool resetPot)
        {
            Label = label;
            ManaAward = manaAward;
            ResetPot = resetPot;
        }

        public string Label { get; private set; }
        public int ManaAward { get; private set; }
        public bool ResetPot { get; private set; }
    }

    private async void Start()
    {
        EnsureRuntimeState();
        await InitializeInfrastructureAsync();
        inventory.Load();
        coven.Load();
        LoadPrototypeFriendsState();
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
        if (roundState == null)
        {
            roundState = new BingoRoundState(roomRules);
        }

        if (rewards == null)
        {
            rewards = new BingoRewardTracker(roomRules);
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
        caller.Reset();
        callPool.Clear();
        calledHistory.Clear();
        calledNumbers.Clear();
        calledAtTimes.Clear();
        powerUps.ResetRound();
        rewards.ResetRound();

        for (int value = 1; value <= 75; value++)
        {
            callPool.Add(value);
        }
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

        GameObject balls = CreateAnchoredPanel(parent, "BallsLeft", new Color(0.15f, 0.08f, 0.22f), 150, 74, 565f, 386f);
        CreateAnchoredText(balls.transform, "BALLS LEFT", 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 130, 22, 0f, 18f);
        ballsLeftText = CreateAnchoredText(balls.transform, Mathf.Max(0, GetActiveMaxBallCalls() - calledHistory.Count).ToString(), 38, FontStyle.Bold, Color.white, 130, 42, 0f, -12f);
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
        CreateAnchoredText(rail.transform, "ALL CALLED\nNUMBERS", 17, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 190, 48, 0f, -110f);
        Button toggle = CreateAnchoredButton(rail.transform, allCalledVisible ? "Hide" : "Show", 15, 104, 30, new Color(0.32f, 0.12f, 0.62f), 0f, -150f);
        toggle.onClick.AddListener(ToggleAllCalledVisible);
        calledHistoryText = CreateAnchoredText(rail.transform, GetCalledHistoryPanelText(), GetCalledHistoryPanelFontSize(), FontStyle.Bold, Color.white, 198, 238, 0f, -282f);
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
        calledNumberText = CreateAnchoredText(rail.transform, calledHistory.Count > 0 ? $"{GetBingoLetter(calledHistory[0])}-{calledHistory[0]}" : "Starting", 46, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 62, 0f, 270f);
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
        CreateTextInParent(balls.transform, Mathf.Max(0, 75 - calledNumbers.Count).ToString(), 38, FontStyle.Bold, Color.white, 140, 42);
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
        calledHistoryText = CreateTextInParent(rail.transform, calledHistory.Count > 0 ? GetCalledHistoryText() : "none", 16, FontStyle.Normal, Color.white, 190, 170);
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
        calledNumberText = CreateTextInParent(rail.transform, calledHistory.Count > 0 ? $"{GetBingoLetter(calledHistory[0])}-{calledHistory[0]}" : "Starting", 46, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 62);
        timerText = CreateTextInParent(rail.transform, "--", 24, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 36);
        CreateTextInParent(rail.transform, "CALL QUEUE", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 32);

        for (int index = 0; index < 6; index++)
        {
            string label = calledHistory.Count > index ? $"{GetBingoLetter(calledHistory[index])}\n{calledHistory[index]}" : "-";
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
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundState.StopRound();
        calledHistory.Clear();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.1f, 0.22f, 0.17f, 0.98f), 0, 0);
        layout.enabled = false;

        GameObject accountBar = CreateAnchoredPanel(contentRoot, "WorldAccountBar", new Color(0.09f, 0.04f, 0.15f), 720, 74, 395f, 386f);
        lobbyManaText = CreateLobbyResourceTile(accountBar.transform, "Mana", inventory.GetManaText(), "*", 220, -245f);
        lobbyCrystalText = CreateLobbyResourceTile(accountBar.transform, "Crystals", inventory.GetCrystalText(), "<>", 220, 0f);
        CreateLobbyResourceTile(accountBar.transform, "Power-Ups", inventory.GetPowerUpText(), "+", 240, 255f);
        Button mapSettings = CreateAnchoredButton(contentRoot, "SET", 16, 72, 54, new Color(0.18f, 0.08f, 0.28f), -732f, 386f);
        mapSettings.onClick.AddListener(() => BuildDevSettingsUi(true));
        Button den = CreateAnchoredButton(contentRoot, "Den", 18, 82, 54, new Color(0.35f, 0.12f, 0.62f), -732f, 322f);
        den.onClick.AddListener(BuildPlayerDenUi);

        GameObject titlePanel = CreateAnchoredPanel(contentRoot, "WorldMapTitle", new Color(0.08f, 0.28f, 0.12f), 860, 150, -260f, 284f);
        CreateAnchoredText(titlePanel.transform, "World Map", 58, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 820, 72, 0f, 14f);
        CreateAnchoredText(titlePanel.transform, "Choose an unlocked realm", 20, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 760, 32, 0f, -52f);

        CreateWorldRealmNodes();

        CreateAnchoredText(contentRoot, "Restore every room in a realm to unlock the next realm.", 24, FontStyle.Bold, new Color(0.9f, 0.95f, 0.86f), 980, 40, 0f, -410f);

        ApplyStageScale(true);
    }

    private void CreateWorldRealmNodes()
    {
        float[] xPositions = { -560f, -280f, 0f, 280f, 560f };
        for (int index = 0; index < RealmContentCatalog.AllRealms.Count; index++)
        {
            int rowOffset = index < 5 ? 0 : 5;
            float x = xPositions[index - rowOffset];
            float y = index < 5 ? 96f : -128f;
            CreateWorldRealmNode(index, x, y);
        }
    }

    private void CreateWorldRealmNode(int realmIndex, float x, float y)
    {
        RealmDefinition realm = RealmContentCatalog.AllRealms[realmIndex];
        bool unlocked = IsRealmUnlocked(realmIndex);
        bool restored = IsRealmComplete(realmIndex);
        Color color = unlocked ? new Color(0.18f, 0.36f, 0.14f) : new Color(0.12f, 0.14f, 0.12f);
        GameObject node = CreateAnchoredPanel(contentRoot, $"WorldRealm_{realmIndex + 1}", color, 260, 142, x, y);
        CreateAnchoredText(node.transform, realm.Name, 20, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 236, 54, 0f, 34f);
        string status = restored ? "Complete" : unlocked ? "Unlocked" : $"Locked";
        CreateAnchoredText(node.transform, $"Realm {realmIndex + 1} / {RealmContentCatalog.AllRealms.Count} - {status}", 15, FontStyle.Bold, unlocked ? new Color(0.8f, 1f, 0.55f) : new Color(0.7f, 0.7f, 0.68f), 232, 24, 0f, -8f);

        Button button = CreateAnchoredButton(node.transform, unlocked ? "Enter Realm" : "Locked", 16, 142, 36, unlocked ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.25f, 0.25f, 0.25f), 0f, -48f);
        button.interactable = unlocked;
        if (unlocked)
        {
            button.onClick.AddListener(() => EnterRealm(realmIndex));
        }
    }

    private void BuildRealmMapUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundState.StopRound();
        calledHistory.Clear();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.1f, 0.22f, 0.17f, 0.98f), 0, 0);
        layout.enabled = false;

        GameObject accountBar = CreateAnchoredPanel(contentRoot, "RealmAccountBar", new Color(0.09f, 0.04f, 0.15f), 720, 74, 395f, 386f);
        lobbyManaText = CreateLobbyResourceTile(accountBar.transform, "Mana", inventory.GetManaText(), "*", 220, -245f);
        lobbyCrystalText = CreateLobbyResourceTile(accountBar.transform, "Crystals", inventory.GetCrystalText(), "<>", 220, 0f);
        CreateLobbyResourceTile(accountBar.transform, "Power-Ups", inventory.GetPowerUpText(), "+", 240, 255f);

        Button map = CreateAnchoredButton(contentRoot, "Map", 18, 82, 54, new Color(0.35f, 0.12f, 0.62f), -732f, 386f);
        map.onClick.AddListener(BuildWorldMapUi);

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
        RealmContentCatalog.SetActivePrototypeRoom(roomIndex);
        inventory.RefreshActiveRoomState();
        ApplySavedManaBetForActiveRoom();
        BuildLobbyUi();
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

        IReadOnlyList<RoomDefinition> previousRooms = RealmContentCatalog.AllRealms[realmIndex - 1].Rooms;
        return previousRooms.Count > 0 && inventory.IsRoomRestored(previousRooms[previousRooms.Count - 1]);
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
        roundState.StopRound();
        calledHistory.Clear();
        ClampManaBetToActiveRoom();

        VerticalLayoutGroup layout = PrepareStage(LobbyStageWidth, LobbyStageHeight, new Color(0.12f, 0.25f, 0.18f, 0.98f), 0, 0);
        layout.enabled = false;

        BuildLobbyHeader();

        Button back = CreateAnchoredButton(contentRoot, "Back", 18, 112, 54, new Color(0.35f, 0.12f, 0.62f), -710f, 382f);
        back.onClick.AddListener(BuildRealmMapUi);
        BuildLobbyClairvoyanceRail();

        BuildLobbyRestorePanel();
        CreateLobbyInfoTile("Jackpot\nWheelspin", $"Pot {inventory.GetCurrentJackpotPotText()}\n{inventory.GetJackpotSpinText()}", 300, 178, 610f, 118f, new Color(0.35f, 0.08f, 0.48f));
        if (inventory.PendingJackpotSpins > 0)
        {
            Button spinPending = CreateAnchoredButton(contentRoot, "Spin Pending", 16, 178, 34, new Color(0.12f, 0.55f, 0.08f), 610f, 36f);
            spinPending.onClick.AddListener(BuildJackpotSpinUi);
        }

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
        minus.onClick.AddListener(() => AdjustLobbyBet(-GetActiveProgression().BetStep));

        roomText = CreateAnchoredText(betRow.transform, "", 24, FontStyle.Bold, new Color(0.15f, 0.1f, 0.32f), 300, 56, 0f, 0f);
        RefreshLobbyBetText();

        Button plus = CreateAnchoredButton(betRow.transform, "+", 30, 72, 58, new Color(0.35f, 0.12f, 0.62f), 230f, 0f);
        plus.onClick.AddListener(() => AdjustLobbyBet(GetActiveProgression().BetStep));

        ApplyStageScale(true);
    }

    private void BuildLobbyClairvoyanceRail()
    {
        GameObject rail = CreateAnchoredPanel(contentRoot, "LobbyClairvoyanceRail", new Color(0.16f, 0.07f, 0.25f), 150, 132, -710f, -20f);
        CreateAnchoredText(rail.transform, "EYE", 26, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 120, 36, 0f, 34f);
        CreateAnchoredText(rail.transform, "Clairvoyance", 13, FontStyle.Bold, Color.white, 130, 22, 0f, 4f);
        lobbyClairvoyanceStatusText = CreateAnchoredText(rail.transform, inventory.GetClairvoyanceRoomStatusText(), 24, FontStyle.Bold, inventory.HasActiveClairvoyance() ? new Color(0.55f, 1f, 0.85f) : new Color(0.78f, 0.74f, 0.88f), 130, 34, 0f, -34f);
    }

    private void BuildLobbyHeader()
    {
        GameObject header = CreateAnchoredPanel(contentRoot, "LobbyHeader", new Color(0f, 0f, 0f, 0f), 1500, 184, 0f, 322f);

        CreateAnchoredPanel(header.transform, "LobbyTopBand", new Color(0.09f, 0.04f, 0.15f), 1500, 72, 0f, 52f);
        lobbyManaText = CreateLobbyResourceTile(header.transform, "Mana", inventory.GetManaText(), "*", 220, 190f, 52f);
        lobbyCrystalText = CreateLobbyResourceTile(header.transform, "Crystals", inventory.GetCrystalText(), "<>", 230, 430f, 52f);
        lobbyPowerUpText = CreateLobbyResourceTile(header.transform, "Power-Ups", inventory.GetPowerUpText(), "+", 230, 680f, 52f, true);

        GameObject titlePanel = CreateAnchoredPanel(header.transform, "LobbyRoomTitle", new Color(0.15f, 0.08f, 0.22f), 900, 104, -280f, -44f);
        CreateAnchoredText(titlePanel.transform, RealmContentCatalog.ActivePrototypeRoom.Name, 42, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 860, 50, 0f, 20f);
        CreateAnchoredText(titlePanel.transform, RealmContentCatalog.ActivePrototypeRealm.Name, 22, FontStyle.Bold, new Color(1f, 0.78f, 0.35f), 820, 30, 0f, -16f);
        CreateAnchoredText(titlePanel.transform, $"Potion: {RealmContentCatalog.ActivePrototypeRoom.PotionName}", 19, FontStyle.Bold, Color.white, 780, 28, 0f, -44f);
        CreateRoomModeBadge(titlePanel.transform, RealmContentCatalog.ActivePrototypeRoom, 334f, -12f, 126f, 74f);

        CreateAnchoredText(header.transform, BuildGameplayRankText(), 14, FontStyle.Bold, new Color(0.75f, 0.95f, 1f), 220, 58, 12f, 52f);
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
        roundState.StopRound();
        calledHistory.Clear();

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
        CreateAnchoredText(contentRoot, "BINGO\nMAGIC MAYHEM", 32, FontStyle.Bold, gold, 310, 68, -10f, 382f);
        CreateLobbyResourceTile(contentRoot, "Mana", profile.Mana.ToString("N0"), "*", 230, 356f, 382f);
        CreateLobbyResourceTile(contentRoot, "Crystals", profile.Crystals.ToString("N0"), "<>", 230, 606f, 382f);
        Button rankButton = CreateAnchoredButton(contentRoot, $"Profile\nL{profile.Level} | Aura TBD", 14, 300, 58, new Color(0.09f, 0.04f, 0.15f), -600f, 382f);
        rankButton.onClick.AddListener(() => BuildPlayerProfileUi("Profile"));

        Button map = CreateAnchoredButton(contentRoot, "Map", 18, 104, 50, new Color(0.35f, 0.12f, 0.62f), -474f, 382f);
        map.onClick.AddListener(BuildWorldMapUi);
        Button room = CreateAnchoredButton(contentRoot, "Room", 18, 104, 50, new Color(0.35f, 0.12f, 0.62f), -354f, 382f);
        room.onClick.AddListener(BuildLobbyUi);
        Button settings = CreateAnchoredButton(contentRoot, "SET", 16, 72, 50, new Color(0.18f, 0.08f, 0.28f), 700f, 382f);
        settings.onClick.AddListener(() => BuildDevSettingsUi(false, true));

        GameObject titlePanel = CreateAnchoredPanel(contentRoot, "DenTitle", new Color(0.15f, 0.08f, 0.22f), 720, 82, 0f, 286f);
        CreateAnchoredText(titlePanel.transform, "PLAYER'S DEN", 40, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 660, 48, 0f, 12f);
        CreateAnchoredText(titlePanel.transform, "Inventory, claims, and account hub | Rank now derives from Aura Strength", 16, FontStyle.Bold, muted, 680, 26, 0f, -24f);

        GameObject library = CreateDenDoorTile("Library\nGrimoire", "Albums and cards", -530f, 150f, 280, 150, new Color(0.18f, 0.1f, 0.16f));
        Button libraryButton = library.AddComponent<Button>();
        libraryButton.onClick.AddListener(BuildLibraryGrimoireUi);
        GameObject bazaar = CreateDenDoorTile("Bewitchment\nBazaar", "Oracle Alley", 0f, 138f, 320, 142, new Color(0.18f, 0.07f, 0.26f));
        Button bazaarButton = bazaar.AddComponent<Button>();
        bazaarButton.onClick.AddListener(BuildBewitchmentBazaarUi);
        GameObject apothecary = CreateDenDoorTile("Apothecary", "Potion workshop", 358f, 145f, 260, 132, new Color(0.12f, 0.18f, 0.11f));
        Button apothecaryButton = apothecary.AddComponent<Button>();
        apothecaryButton.onClick.AddListener(BuildApothecaryUi);
        GameObject relicWall = CreateDenDoorTile("Relic Wall", "Badges later", 626f, 128f, 220, 210, new Color(0.16f, 0.1f, 0.08f));
        Button relicWallButton = relicWall.AddComponent<Button>();
        relicWallButton.onClick.AddListener(BuildRelicWallUi);
        GameObject cabinet = CreateDenDoorTile("Cabinet of\nCuriosities", "Inventory stash", 536f, -176f, 300, 172, wood);
        Button cabinetButton = cabinet.AddComponent<Button>();
        cabinetButton.onClick.AddListener(BuildPowerUpInventoryUi);

        Button cauldron = CreateAnchoredButton(contentRoot, GetDenManaCauldronButtonText(), 22, 420, 184, new Color(0.16f, 0.08f, 0.2f), 0f, -118f);
        denManaCauldronText = cauldron.GetComponentInChildren<Text>();
        cauldron.onClick.AddListener(BuildManaCauldronUi);
        CreateAnchoredText(contentRoot, $"Restored rooms: {inventory.GetRestoredRoomCount()} | +{inventory.GetManaCauldronHourlyRefillAmount()}/hour", 18, FontStyle.Bold, muted, 520, 28, 0f, -232f);

        GameObject trailTile = CreateAnchoredPanel(contentRoot, "DenEnchantedTrailTile", deepPurple, 370, 268, -568f, -174f);
        Button trailButton = trailTile.AddComponent<Button>();
        trailButton.onClick.AddListener(BuildEnchantedTrailUi);
        CreateAnchoredText(trailTile.transform, "ENCHANTED\nTRAIL", 29, FontStyle.Bold, gold, 330, 76, 0f, 62f);
        CreateAnchoredText(trailTile.transform, "Task path and free rewards", 16, FontStyle.Bold, muted, 320, 28, 0f, 2f);
        CreateAnchoredText(trailTile.transform, "Prototype shell", 18, FontStyle.Bold, Color.white, 300, 32, 0f, -62f);
        CreateAnchoredText(trailTile.transform, "Tap to view", 13, FontStyle.Bold, muted, 300, 24, 0f, -104f);

        GameObject covenTile = CreateDenDoorTile("Coven\nCircle", "Orbs and sharing", -292f, -172f, 176, 148, new Color(0.08f, 0.05f, 0.1f));
        Button covenButton = covenTile.AddComponent<Button>();
        covenButton.onClick.AddListener(BuildCovenCircleUi);

        GameObject freebies = CreateDenBottomButton("Freebies", "social link", -620f);
        Button freebiesButton = freebies.AddComponent<Button>();
        freebiesButton.onClick.AddListener(BuildSocialFreebiesUi);
        GameObject friends = CreateDenBottomButton("Friends", $"{prototypeFriends.Count} friends", -440f);
        Button friendsButton = friends.AddComponent<Button>();
        friendsButton.onClick.AddListener(BuildFriendsUi);
        GameObject leaders = CreateDenBottomButton("Leaders", "friends | team | global", -260f);
        Button leadersButton = leaders.AddComponent<Button>();
        leadersButton.onClick.AddListener(BuildLeaderboardUi);
        GameObject market = CreateDenBottomButton("Mayhem\nMarket", "IAP shell", -80f);
        Button marketButton = market.AddComponent<Button>();
        marketButton.onClick.AddListener(BuildMayhemMarketUi);
        GameObject dailyBonus = CreateDenBottomButton("Daily Bonus", inventory.GetDailyBonusClaimStateText(), 250f);
        Button dailyBonusButton = dailyBonus.AddComponent<Button>();
        dailyBonusButton.onClick.AddListener(BuildDailyBonusUi);
        GameObject dailySpin = CreateDenBottomButton("Daily Spin", inventory.GetDailySpinClaimStateText(), 430f);
        Button dailySpinButton = dailySpin.AddComponent<Button>();
        dailySpinButton.onClick.AddListener(BuildDailySpinUi);
        GameObject inbox = CreateDenBottomButton("Inbox", GetDenInboxStatusText(), 610f);
        Button inboxButton = inbox.AddComponent<Button>();
        inboxButton.onClick.AddListener(BuildInboxUi);

        ApplyStageScale(true);
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
            CreateAnchoredText(rowPanel.transform, line.Label, 17, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 300, 22, 0f, 10f);
            CreateAnchoredText(rowPanel.transform, line.Value, 15, FontStyle.Bold, new Color(0.18f, 0.14f, 0.28f), 300, 22, 0f, -13f);
        }
    }

    private void BuildPlayerProfileUi(string tab)
    {
        playerProfileActiveTab = tab;
        RemovePlayerProfileModal();

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        GameObject panel = CreateAnchoredPanel(contentRoot, "PlayerProfileModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1080, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        CreateAnchoredText(panel.transform, "PLAYER PROFILE", 42, FontStyle.Bold, gold, 820, 56, 0f, 274f);
        CreateAnchoredText(panel.transform, $"Level {profile.Level}  |  {profile.LevelProgressText}  |  {profile.AuraStrengthText}", 18, FontStyle.Bold, muted, 820, 28, 0f, 232f);

        CreatePlayerProfileTabButton(panel.transform, "Profile", -220f, 184f);
        CreatePlayerProfileTabButton(panel.transform, "Avatars", 0f, 184f);
        CreatePlayerProfileTabButton(panel.transform, "Settings", 220f, 184f);

        if (playerProfileActiveTab == "Avatars")
        {
            BuildPlayerAvatarTab(panel.transform, profile);
        }
        else if (playerProfileActiveTab == "Settings")
        {
            BuildPlayerSettingsTab(panel.transform);
        }
        else
        {
            BuildPlayerProfileSummaryTab(panel.transform, profile);
        }

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -286f);
        close.onClick.AddListener(RemovePlayerProfileModal);
    }

    private void CreatePlayerProfileTabButton(Transform parent, string tab, float x, float y)
    {
        bool active = playerProfileActiveTab == tab;
        Button button = CreateAnchoredButton(parent, tab, 18, 190, 42, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        button.onClick.AddListener(() => BuildPlayerProfileUi(tab));
    }

    private void BuildPlayerProfileSummaryTab(Transform parent, PlayerProfileState profile)
    {
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color purple = new Color(0.18f, 0.08f, 0.32f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        GameObject identity = CreateAnchoredPanel(parent, "ProfileIdentity", cream, 320, 334, -330f, -26f);
        GameObject summaryAvatar = CreateAnchoredPanel(identity.transform, "AvatarFrame", GetPrototypeFrameColor(), 150, 150, 0f, 70f);
        if (!TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 126f, 126f))
        {
            CreateAnchoredText(summaryAvatar.transform, GetPrototypeAvatarIcon(), 46, FontStyle.Bold, Color.white, 130, 80, 0f, 14f);
        }
        TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 150f, 150f);
        CreateAnchoredText(identity.transform, "Player Name", 16, FontStyle.Bold, purple, 260, 24, 0f, -8f);
        CreateAnchoredText(identity.transform, profileSettingsState.DisplayName, 30, FontStyle.Bold, purple, 280, 40, 0f, -42f);
        CreateAnchoredText(identity.transform, $"Level {profile.Level}\nRank: Aura TBD", 18, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 260, 58, 0f, -102f);

        GameObject progress = CreateAnchoredPanel(parent, "ProfileProgress", new Color(0.12f, 0.06f, 0.18f), 640, 334, 190f, -26f);
        CreateAnchoredText(progress.transform, "ACCOUNT SUMMARY", 24, FontStyle.Bold, gold, 560, 32, 0f, 132f);
        CreateProfileSummaryRow(progress.transform, "Mana", profile.Mana.ToString("N0"), -188f, 82f);
        CreateProfileSummaryRow(progress.transform, "Crystals", profile.Crystals.ToString("N0"), 188f, 82f);
        CreateProfileSummaryRow(progress.transform, "Album", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} Grimoire", -188f, 18f);
        CreateProfileSummaryRow(progress.transform, "Book", inventory.BookOfShadowsPurchased ? $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked", 188f, 18f);
        CreateProfileSummaryRow(progress.transform, "Rooms", $"{inventory.GetRestoredRoomCount()} restored", -188f, -46f);
        CreateProfileSummaryRow(progress.transform, "Aura", "Strength formula TBD", 188f, -46f);
        CreateAnchoredText(progress.transform, profile.AuraRankNoteText, 15, FontStyle.Bold, muted, 560, 42, 0f, -116f);

        Button rank = CreateAnchoredButton(parent, "Aura Ranks", 18, 210, 42, new Color(0.35f, 0.12f, 0.62f), -330f, -218f);
        rank.onClick.AddListener(BuildRankInfoUi);
    }

    private void CreateProfileSummaryRow(Transform parent, string label, string value, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"ProfileSummary_{label}", new Color(0.96f, 0.86f, 0.62f), 286, 48, x, y);
        CreateAnchoredText(row.transform, label, 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 250, 18, 0f, 10f);
        CreateAnchoredText(row.transform, value, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 22, 0f, -10f);
    }

    private void BuildPlayerAvatarTab(Transform parent, PlayerProfileState profile)
    {
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        GameObject preview = CreateAnchoredPanel(parent, "AvatarPreview", new Color(0.12f, 0.06f, 0.18f), 360, 360, -300f, -30f);
        GameObject previewAvatar = CreateAnchoredPanel(preview.transform, "AvatarFramePreview", GetPrototypeFrameColor(), 190, 190, 0f, 58f);
        if (!TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 160f, 160f))
        {
            CreateAnchoredText(previewAvatar.transform, GetPrototypeAvatarIcon(), 58, FontStyle.Bold, Color.white, 160, 100, 0f, 16f);
        }
        TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 190f, 190f);
        CreateAnchoredText(preview.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, 25, FontStyle.Bold, gold, 300, 34, 0f, -54f);
        CreateAnchoredText(preview.transform, $"{PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName}\n{PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName} dauber", 17, FontStyle.Bold, muted, 300, 58, 0f, -106f);

        GameObject controls = CreateAnchoredPanel(parent, "AvatarControls", new Color(0.96f, 0.86f, 0.62f), 560, 360, 190f, -30f);
        CreateAnchoredText(controls.transform, "COSMETICS", 26, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 480, 34, 0f, 132f);
        CreateCosmeticSelectorRow(controls.transform, "Avatar", PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, -6f, 70f, CyclePrototypeAvatar);
        CreateCosmeticSelectorRow(controls.transform, "Frame", PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName, -6f, 6f, CyclePrototypeFrame);
        CreateCosmeticSelectorRow(controls.transform, "Dauber", PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName, -6f, -58f, CyclePrototypeDauber);
        CreateAnchoredText(controls.transform, "Avatar unlock rules, rank cosmetics, and custom avatar behavior are placeholders only.", 14, FontStyle.Bold, new Color(0.34f, 0.26f, 0.34f), 480, 42, 0f, -128f);
    }

    private void CreateCosmeticSelectorRow(Transform parent, string label, string value, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateAnchoredPanel(parent, $"Cosmetic_{label}", new Color(0.18f, 0.13f, 0.2f), 480, 48, x, y);
        CreateAnchoredText(row.transform, label, 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 118, 22, -164f, 0f);
        CreateAnchoredText(row.transform, value, 16, FontStyle.Bold, Color.white, 210, 24, -18f, 0f);
        Button next = CreateAnchoredButton(row.transform, "Next", 13, 82, 28, new Color(0.35f, 0.12f, 0.62f), 184f, 0f);
        next.onClick.AddListener(action);
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
        GameObject account = CreateAnchoredPanel(parent, "ProfileAccountSettings", new Color(0.12f, 0.06f, 0.18f), 460, 360, -250f, -30f);
        CreateAnchoredText(account.transform, "ACCOUNT", 25, FontStyle.Bold, gold, 380, 34, 0f, 132f);
        profileDisplayNameInput = CreateAnchoredInputField(account.transform, "ProfileDisplayNameInput", "Display name", 360, 44, 0f, 84f);
        profileDisplayNameInput.text = profileSettingsState.DisplayName;
        profileDisplayNameInput.characterLimit = ProfileDisplayNameValidator.BetaMaximumLength;
        profileDisplayNameInput.lineType = InputField.LineType.SingleLine;
        profileDisplayNameInput.textComponent.alignment = TextAnchor.MiddleLeft;
        Button saveName = CreateAnchoredButton(account.transform, "Save Display Name", 16, 220, 38, new Color(0.12f, 0.55f, 0.08f), 0f, 38f);
        saveName.onClick.AddListener(SavePrototypeDisplayName);
        profileDisplayNameStatusText = CreateAnchoredText(account.transform, lastProfileDisplayNameStatus, 12, FontStyle.Bold, muted, 380, 34, 0f, -2f);
        CreateAnchoredText(account.transform, "LOGIN PLACEHOLDERS", 14, FontStyle.Bold, gold, 380, 22, 0f, -42f);
        CreateCompactLoginOptionButton(account.transform, "Google", -125f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Facebook", 0f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Apple", 125f, -78f);
        CreateAnchoredText(account.transform, "Name validation is local Beta/test only. Login, uniqueness, and moderation require backend approval.", 12, FontStyle.Bold, muted, 390, 48, 0f, -132f);

        GameObject preferences = CreateAnchoredPanel(parent, "ProfilePreferences", new Color(0.96f, 0.86f, 0.62f), 460, 360, 250f, -30f);
        CreateAnchoredText(preferences.transform, "PREFERENCES", 25, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 380, 34, 0f, 132f);
        CreatePreferenceToggle(preferences.transform, "Sound", prototypeSoundEnabled, 58f, TogglePrototypeSound);
        CreatePreferenceToggle(preferences.transform, "Notifications", prototypeNotificationsEnabled, -8f, TogglePrototypeNotifications);
        CreateAnchoredText(preferences.transform, "Cosmetics and preferences save locally. Name editing and platform permissions wire up later.", 14, FontStyle.Bold, new Color(0.34f, 0.26f, 0.34f), 380, 58, 0f, -112f);
    }

    private void CreateCompactLoginOptionButton(Transform parent, string provider, float x, float y)
    {
        Button button = CreateAnchoredButton(parent, provider, 13, 110, 32, new Color(0.18f, 0.16f, 0.28f), x, y);
        button.interactable = false;
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

    private void CreatePreferenceToggle(Transform parent, string label, bool enabled, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateAnchoredPanel(parent, $"Preference_{label}", new Color(0.18f, 0.13f, 0.2f), 360, 50, 0f, y);
        CreateAnchoredText(row.transform, label, 18, FontStyle.Bold, Color.white, 170, 24, -72f, 0f);
        Button toggle = CreateAnchoredButton(row.transform, enabled ? "ON" : "OFF", 16, 92, 30, enabled ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.42f, 0.12f, 0.18f), 112f, 0f);
        toggle.onClick.AddListener(action);
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
        BuildPlayerProfileUi("Avatars");
    }

    private void CyclePrototypeFrame()
    {
        selectedPrototypeFrameIndex = (selectedPrototypeFrameIndex + 1) % PrototypeAvatarFrames.Count;
        PersistProfileSettings("frame_selected");
        BuildPlayerProfileUi("Avatars");
    }

    private void CyclePrototypeDauber()
    {
        selectedPrototypeDauberIndex = (selectedPrototypeDauberIndex + 1) % PrototypeDaubers.Count;
        PersistProfileSettings("dauber_selected");
        BuildPlayerProfileUi("Avatars");
    }

    private void TogglePrototypeSound()
    {
        prototypeSoundEnabled = !prototypeSoundEnabled;
        PersistProfileSettings("sound_preference_changed");
        BuildPlayerProfileUi("Settings");
    }

    private void TogglePrototypeNotifications()
    {
        prototypeNotificationsEnabled = !prototypeNotificationsEnabled;
        PersistProfileSettings("notification_preference_changed");
        BuildPlayerProfileUi("Settings");
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
    }

    private void BuildRankInfoUi()
    {
        RemoveRankInfoModal();

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        GameObject panel = CreateAnchoredPanel(contentRoot, "RankInfoModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 960, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.86f, 0.82f, 0.95f);
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color ink = new Color(0.18f, 0.08f, 0.32f);

        CreateAnchoredText(panel.transform, "AURA RANKS", 42, FontStyle.Bold, gold, 760, 56, 0f, 274f);
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

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -286f);
        close.onClick.AddListener(RemoveRankInfoModal);
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
        CreateAnchoredText(tile.transform, title, 28, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, height * 0.48f, 0f, height * 0.14f);
        CreateAnchoredText(tile.transform, subtitle, 15, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), width - 26, 28, 0f, -height * 0.28f);
        return tile;
    }

    private void BuildApothecaryUi()
    {
        RemoveApothecaryModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "ApothecaryModal", new Color(0.045f, 0.035f, 0.075f, 0.985f), 1120, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        CreateAnchoredText(panel.transform, "APOTHECARY", 44, FontStyle.Bold, gold, 820, 58, 0f, 274f);
        CreateAnchoredText(panel.transform, "Potion-only prototype surface. Crafting costs, recipes, upgrades, and reward tuning are not active in this pass.", 15, FontStyle.Bold, muted, 980, 34, 0f, 232f);

        CreateApothecarySection(panel.transform, "Active Potion", -344f, 82f, 360, 252, new Color(0.11f, 0.07f, 0.16f));
        CreateAnchoredText(panel.transform, room.PotionName, 26, FontStyle.Bold, gold, 310, 64, -344f, 126f);
        CreateAnchoredText(panel.transform, room.Name, 16, FontStyle.Bold, muted, 300, 24, -344f, 74f);
        CreateAnchoredText(panel.transform, inventory.GetRestoreStatusText(), 24, FontStyle.Bold, inventory.CanRestoreActiveRoom() ? new Color(0.46f, 0.9f, 0.34f) : Color.white, 300, 32, -344f, 28f);
        CreateAnchoredText(panel.transform, inventory.GetFullIngredientProgressText(), 17, FontStyle.Bold, muted, 300, 110, -344f, -48f);

        CreateApothecarySection(panel.transform, "Potion Ingredients", 84f, 82f, 568, 252, new Color(0.1f, 0.055f, 0.13f));
        CreateAnchoredText(panel.transform, inventory.GetFullIngredientProgressText(), 19, FontStyle.Bold, Color.white, 500, 156, 84f, 58f);
        CreateAnchoredText(panel.transform, "Ingredient collection still comes from room play and ingredient detail flows.", 14, FontStyle.Bold, muted, 500, 42, 84f, -62f);

        CreateApothecarySection(panel.transform, "Completion Reward", -280f, -178f, 430, 190, new Color(0.13f, 0.075f, 0.11f));
        CreateAnchoredText(panel.transform, inventory.GetRestoreRewardText(), 28, FontStyle.Bold, gold, 360, 76, -280f, -158f);
        CreateAnchoredText(panel.transform, "Reward values remain prototype placeholders until potion rewards are approved.", 14, FontStyle.Bold, muted, 380, 48, -280f, -224f);

        CreateApothecarySection(panel.transform, "Potion Workbench", 292f, -178f, 430, 190, new Color(0.11f, 0.07f, 0.14f));
        CreateApothecaryWorkbenchSlot(panel.transform, "Recipe", "Locked", 166f, -178f);
        CreateApothecaryWorkbenchSlot(panel.transform, "Ingredient", "Later", 292f, -178f);
        CreateApothecaryWorkbenchSlot(panel.transform, "Output", "Later", 418f, -178f);
        CreateAnchoredText(panel.transform, "Mixing and crafting stay configurable until potion rules are approved.", 14, FontStyle.Bold, muted, 380, 36, 292f, -248f);

        CreateAnchoredText(panel.transform, "Sigils, Clairvoyance, Pandora, and other boost inventory stay in Cabinet of Curiosities.", 15, FontStyle.Bold, muted, 900, 34, 0f, -278f);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -326f);
        close.onClick.AddListener(RemoveApothecaryModal);
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
        GameObject panel = CreateAnchoredPanel(contentRoot, "RelicWallModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1120, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);

        CreateAnchoredText(panel.transform, "RELIC WALL", 44, FontStyle.Bold, gold, 820, 58, 0f, 274f);
        CreateAnchoredText(panel.transform, "Badge display shell for rank, collection, realm, and event accomplishments. Rewards and achievement rules are not active yet.", 15, FontStyle.Bold, muted, 980, 34, 0f, 232f);

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
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -286f);
        close.onClick.AddListener(RemoveRelicWallModal);
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
        CreateAnchoredText(badge.transform, earned ? "*" : "-", 34, FontStyle.Bold, accentColor, 70, 36, 0f, 26f);
        CreateAnchoredText(badge.transform, title, 12, FontStyle.Bold, textColor, 82, 30, 0f, -10f);
        CreateAnchoredText(badge.transform, detail, 10, FontStyle.Bold, textColor, 82, 24, 0f, -40f);
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

    private void BuildEnchantedTrailUi()
    {
        RemoveEnchantedTrailModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "EnchantedTrailModal", new Color(0.045f, 0.03f, 0.075f, 0.985f), 1140, 650, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, "ENCHANTED TRAIL", 44, FontStyle.Bold, gold, 980, 58, 0f, 274f);
        CreateAnchoredText(panel.transform, "Medium-term task path prototype. Separate from Daily Bonus, Daily Spin, Freebies, Realm income, Inbox, and jackpot wheelspin.", 15, FontStyle.Bold, muted, 980, 38, 0f, 230f);

        GameObject path = CreateAnchoredPanel(panel.transform, "TrailFreePath", new Color(0.12f, 0.055f, 0.17f), 1000, 170, 0f, 112f);
        CreateAnchoredText(path.transform, "FREE PATH", 22, FontStyle.Bold, gold, 920, 28, 0f, 52f);
        for (int index = 0; index < 7; index++)
        {
            float x = -390f + index * 130f;
            CreateTrailRewardNode(path.transform, index + 1, x, -18f);
            if (index < 6)
            {
                CreateAnchoredPanel(path.transform, $"TrailConnector_{index}", new Color(0.46f, 0.28f, 0.64f), 72, 8, x + 65f, -18f);
            }
        }

        GameObject tasks = CreateAnchoredPanel(panel.transform, "TrailTasks", new Color(0.09f, 0.045f, 0.13f), 640, 250, -210f, -106f);
        CreateAnchoredText(tasks.transform, "TRAIL TASKS", 24, FontStyle.Bold, gold, 570, 32, 0f, 94f);
        CreateTrailTaskRow(tasks.transform, "Daub called numbers", "18 / 50", 0.36f, -150f, 38f);
        CreateTrailTaskRow(tasks.transform, "Play bingo rounds", "2 / 5", 0.4f, 150f, 38f);
        CreateTrailTaskRow(tasks.transform, "Restore a room", "0 / 1", 0f, -150f, -50f);
        CreateTrailTaskRow(tasks.transform, "Open Pandora", "0 / 1", 0f, 150f, -50f);

        GameObject rules = CreateAnchoredPanel(panel.transform, "TrailRules", new Color(0.13f, 0.08f, 0.15f), 300, 250, 386f, -106f);
        CreateAnchoredText(rules.transform, "SCOPE", 24, FontStyle.Bold, gold, 250, 34, 0f, 92f);
        CreateAnchoredText(rules.transform, "Free path only\nNo premium lane\nNo final reward values\nClaim state only\nNo rank scaling yet", 16, FontStyle.Bold, Color.white, 250, 140, 0f, 4f);
        CreateAnchoredText(rules.transform, "Collect persists locally; no rewards granted.", 13, FontStyle.Bold, muted, 250, 34, 0f, -94f);

        CreateAnchoredText(panel.transform, "Trail duration, point sources, task list, premium unlock, grand reward, and cosmetic rewards remain open decisions.", 15, FontStyle.Bold, muted, 940, 34, 0f, -238f);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, violet, 0f, -292f);
        close.onClick.AddListener(RemoveEnchantedTrailModal);
    }

    private void CreateTrailRewardNode(Transform parent, int step, float x, float y)
    {
        bool unlocked = step <= 2;
        bool claimed = prototypeClaimedTrailRewards.Contains(step);
        Color nodeColor = claimed ? new Color(0.18f, 0.42f, 0.16f) : unlocked ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.22f, 0.2f, 0.24f);
        Color textColor = unlocked ? Color.white : new Color(0.72f, 0.68f, 0.78f);
        GameObject node = CreateAnchoredPanel(parent, $"TrailNode_{step}", nodeColor, 92, 110, x, y);
        CreateAnchoredText(node.transform, claimed ? "+" : unlocked ? "*" : "-", 26, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 52, 28, 0f, 30f);
        CreateAnchoredText(node.transform, $"Step {step}", 12, FontStyle.Bold, textColor, 76, 18, 0f, 8f);
        CreateAnchoredText(node.transform, "Reward", 10, FontStyle.Bold, textColor, 76, 16, 0f, -12f);
        Button collect = CreateAnchoredButton(node.transform, claimed ? "Claimed" : unlocked ? "Collect" : "Locked", 10, 74, 24, claimed ? new Color(0.12f, 0.34f, 0.12f) : unlocked ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, -38f);
        collect.interactable = unlocked && !claimed;
        int rewardStep = step;
        collect.onClick.AddListener(() => ClaimPrototypeTrailReward(rewardStep));
    }

    private void ClaimPrototypeTrailReward(int step)
    {
        prototypeClaimedTrailRewards.Add(step);
        SavePrototypeTrailState();
        BuildEnchantedTrailUi();
    }

    private void LoadPrototypeTrailState()
    {
        prototypeClaimedTrailRewards.Clear();
        string saved = PlayerPrefs.GetString(PrototypeTrailRewardsSaveKey, "");
        if (string.IsNullOrWhiteSpace(saved))
        {
            return;
        }

        string[] steps = saved.Split(PrototypeFriendSaveSeparator);
        for (int index = 0; index < steps.Length; index++)
        {
            if (int.TryParse(steps[index].Trim(), out int step))
            {
                prototypeClaimedTrailRewards.Add(step);
            }
        }
    }

    private void SavePrototypeTrailState()
    {
        List<int> steps = new List<int>(prototypeClaimedTrailRewards);
        steps.Sort();
        List<string> serialized = new List<string>();
        for (int index = 0; index < steps.Count; index++)
        {
            serialized.Add(steps[index].ToString());
        }

        PlayerPrefs.SetString(PrototypeTrailRewardsSaveKey, string.Join(PrototypeFriendSaveSeparator.ToString(), serialized));
        PlayerPrefs.Save();
    }

    private void ResetPrototypeTrailState()
    {
        prototypeClaimedTrailRewards.Clear();
        SavePrototypeTrailState();
    }

    private void CreateTrailTaskRow(Transform parent, string title, string progress, float ratio, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"TrailTask_{title}", new Color(0.96f, 0.86f, 0.62f), 270, 74, x, y);
        CreateAnchoredText(row.transform, title, 14, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 242, 20, 0f, 20f);
        CreateAnchoredPanel(row.transform, "TaskBarBack", new Color(0.34f, 0.28f, 0.2f), 220, 10, 0f, -4f);
        float fillWidth = Mathf.Clamp01(ratio) * 220f;
        if (fillWidth > 0f)
        {
            CreateAnchoredPanel(row.transform, "TaskBarFill", new Color(0.35f, 0.12f, 0.62f), fillWidth, 10, -110f + fillWidth * 0.5f, -4f);
        }
        CreateAnchoredText(row.transform, progress, 12, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 220, 18, 0f, -24f);
    }

    private void RemoveEnchantedTrailModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("EnchantedTrailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildFriendsUi()
    {
        RefreshPrototypeFriendDailyManaState();
        RemoveFriendsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "FriendsModal", new Color(0.055f, 0.028f, 0.085f, 0.985f), 1080, 640, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, "FRIENDS", 44, FontStyle.Bold, gold, 900, 58, 0f, 266f);
        CreateAnchoredText(panel.transform, "Add/manage friends, daily 10-mana social taps, and message actions. Messages route to Inbox > Messages.", 15, FontStyle.Bold, muted, 900, 36, 0f, 224f);
        CreateAnchoredText(panel.transform, BuildPrototypeFriendManaSummaryText(), 14, FontStyle.Bold, muted, 900, 34, 0f, 194f);

        GameObject listPanel = CreateAnchoredPanel(panel.transform, "FriendsList", new Color(0.1f, 0.045f, 0.14f), 330, 358, -346f, 18f);
        CreateAnchoredText(listPanel.transform, "FRIEND LIST", 23, FontStyle.Bold, gold, 290, 32, 0f, 142f);
        List<string> friends = new List<string>(prototypeFriends);
        friends.Sort();
        for (int index = 0; index < friends.Count && index < 4; index++)
        {
            string friendName = friends[index];
            CreateFriendListRow(listPanel.transform, friendName, index % 2 == 0 ? "Online" : "Away", 78f - index * 76f);
        }

        GameObject incomingPanel = CreateAnchoredPanel(panel.transform, "FriendIncoming", new Color(0.12f, 0.055f, 0.16f), 330, 358, 0f, 18f);
        CreateAnchoredText(incomingPanel.transform, "REQUESTS", 23, FontStyle.Bold, gold, 290, 32, 0f, 142f);
        List<string> incoming = new List<string>(prototypeIncomingFriendRequests);
        incoming.Sort();
        if (incoming.Count == 0)
        {
            CreateAnchoredText(incomingPanel.transform, "No incoming requests.", 16, FontStyle.Bold, Color.white, 260, 36, 0f, 62f);
        }
        for (int index = 0; index < incoming.Count && index < 4; index++)
        {
            CreateIncomingFriendRequestRow(incomingPanel.transform, incoming[index], 88f - index * 66f);
        }

        GameObject addPanel = CreateAnchoredPanel(panel.transform, "FriendAddManage", new Color(0.09f, 0.045f, 0.13f), 330, 358, 346f, 18f);
        CreateAnchoredText(addPanel.transform, "ADD FRIEND", 23, FontStyle.Bold, gold, 290, 32, 0f, 142f);
        CreateAnchoredPanel(addPanel.transform, "FriendSearchBox", new Color(0.96f, 0.86f, 0.62f), 260, 46, 0f, 84f);
        CreateAnchoredText(addPanel.transform, "Player name search later", 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 238, 24, 0f, 84f);
        Button sendTest = CreateAnchoredButton(addPanel.transform, "Send Test Request", 16, 230, 42, violet, 0f, 28f);
        sendTest.onClick.AddListener(SendPrototypeFriendRequest);
        CreateAnchoredText(addPanel.transform, "Sent Requests", 18, FontStyle.Bold, gold, 260, 28, 0f, -30f);
        List<string> sent = new List<string>(prototypeSentFriendRequests);
        sent.Sort();
        if (sent.Count == 0)
        {
            CreateAnchoredText(addPanel.transform, "None pending.", 14, FontStyle.Bold, Color.white, 240, 26, 0f, -76f);
        }
        for (int index = 0; index < sent.Count && index < 2; index++)
        {
            CreateSentFriendRequestRow(addPanel.transform, sent[index], -76f - index * 50f);
        }

        CreateAnchoredText(panel.transform, string.IsNullOrEmpty(lastFriendStatusSummary) ? "Friend mana counts are prototype-only until Aura-rank limits are locked." : lastFriendStatusSummary, 15, FontStyle.Bold, muted, 900, 34, 0f, -214f);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, violet, 0f, -286f);
        close.onClick.AddListener(RemoveFriendsModal);
    }

    private void CreateFriendListRow(Transform parent, string friendName, string status, float y)
    {
        bool blocked = prototypeBlockedFriends.Contains(friendName);
        GameObject row = CreateAnchoredPanel(parent, $"Friend_{friendName}", blocked ? new Color(0.32f, 0.27f, 0.32f) : new Color(0.96f, 0.86f, 0.62f), 276, 66, 0f, y);
        Color textColor = blocked ? new Color(0.82f, 0.78f, 0.84f) : new Color(0.18f, 0.08f, 0.32f);
        Color subTextColor = blocked ? new Color(0.66f, 0.62f, 0.7f) : new Color(0.28f, 0.18f, 0.32f);
        CreateAnchoredText(row.transform, friendName, 15, FontStyle.Bold, textColor, 104, 22, -76f, 16f);
        CreateAnchoredText(row.transform, blocked ? "Blocked" : status, 10, FontStyle.Bold, subTextColor, 104, 16, -76f, -4f);
        CreateAnchoredText(row.transform, prototypeReportedFriends.Contains(friendName) ? "Reported" : "", 9, FontStyle.Bold, new Color(0.55f, 0.09f, 0.09f), 104, 14, -76f, -22f);

        Button send = CreateAnchoredButton(row.transform, prototypeFriendsSentManaToday.Contains(friendName) ? "Sent" : "Give 10", 9, 58, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.12f, 0.55f, 0.08f), -2f, 15f);
        send.interactable = !blocked && !prototypeFriendsSentManaToday.Contains(friendName) && prototypeFriendsSentManaToday.Count < PrototypeFriendDailyManaLimit;
        send.onClick.AddListener(() => SendPrototypeFriendMana(friendName));
        Button receive = CreateAnchoredButton(row.transform, prototypeFriendsReceivedManaToday.Contains(friendName) ? "Got" : "Claim", 9, 54, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.12f, 0.55f, 0.08f), 60f, 15f);
        receive.interactable = !blocked && !prototypeFriendsReceivedManaToday.Contains(friendName) && prototypeFriendsReceivedManaToday.Count < PrototypeFriendDailyManaLimit;
        receive.onClick.AddListener(() => ReceivePrototypeFriendMana(friendName));
        Button message = CreateAnchoredButton(row.transform, "Msg", 9, 48, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.35f, 0.12f, 0.62f), 116f, 15f);
        message.interactable = !blocked;
        message.onClick.AddListener(() => SendPrototypeFriendMessage(friendName));
        Button block = CreateAnchoredButton(row.transform, blocked ? "Blocked" : "Block", 9, 62, 22, new Color(0.34f, 0.32f, 0.38f), 28f, -16f);
        block.interactable = !blocked;
        block.onClick.AddListener(() => BlockPrototypeFriend(friendName));
        Button report = CreateAnchoredButton(row.transform, prototypeReportedFriends.Contains(friendName) ? "Filed" : "Report", 9, 62, 22, new Color(0.55f, 0.09f, 0.09f), 96f, -16f);
        report.interactable = !prototypeReportedFriends.Contains(friendName);
        report.onClick.AddListener(() => ReportPrototypeFriend(friendName));
    }

    private string BuildPrototypeFriendManaSummaryText()
    {
        int sendRemaining = Mathf.Max(0, PrototypeFriendDailyManaLimit - prototypeFriendsSentManaToday.Count);
        int receiveRemaining = Mathf.Max(0, PrototypeFriendDailyManaLimit - prototypeFriendsReceivedManaToday.Count);
        return $"Daily mana: give {PrototypeFriendManaAmount} to {sendRemaining} more friend(s), claim from {receiveRemaining} more friend(s). Current cap {PrototypeFriendDailyManaLimit}/day each way is a temporary Aura-rank placeholder.";
    }

    private void CreateIncomingFriendRequestRow(Transform parent, string friendName, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"IncomingFriend_{friendName}", new Color(0.96f, 0.86f, 0.62f), 276, 58, 0f, y);
        CreateAnchoredText(row.transform, friendName, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 120, 24, -68f, 10f);
        CreateAnchoredText(row.transform, "Friend request", 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 120, 16, -68f, -12f);
        Button accept = CreateAnchoredButton(row.transform, "Accept", 10, 64, 26, new Color(0.12f, 0.55f, 0.08f), 42f, 0f);
        accept.onClick.AddListener(() => AcceptPrototypeFriendRequest(friendName));
        Button decline = CreateAnchoredButton(row.transform, "Decline", 10, 64, 26, new Color(0.34f, 0.32f, 0.38f), 110f, 0f);
        decline.onClick.AddListener(() => DeclinePrototypeFriendRequest(friendName));
    }

    private void CreateSentFriendRequestRow(Transform parent, string friendName, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"SentFriend_{friendName}", new Color(0.18f, 0.13f, 0.2f), 250, 38, 0f, y);
        CreateAnchoredText(row.transform, friendName, 14, FontStyle.Bold, Color.white, 128, 20, -48f, 0f);
        Button cancel = CreateAnchoredButton(row.transform, "Cancel", 10, 70, 24, new Color(0.34f, 0.32f, 0.38f), 78f, 0f);
        cancel.onClick.AddListener(() => CancelPrototypeFriendRequest(friendName));
    }

    private void LoadPrototypeFriendsState()
    {
        LoadPrototypeFriendSet("Friends", prototypeFriends);
        LoadPrototypeFriendSet("IncomingRequests", prototypeIncomingFriendRequests);
        LoadPrototypeFriendSet("SentRequests", prototypeSentFriendRequests);
        LoadPrototypeFriendSet("Blocked", prototypeBlockedFriends);
        LoadPrototypeFriendSet("Reported", prototypeReportedFriends);

        string today = GetPrototypeFriendsTodayKey();
        string savedDailyDate = PlayerPrefs.GetString(PrototypeFriendsSavePrefix + "DailyManaDate", "");
        if (savedDailyDate == today)
        {
            LoadPrototypeFriendSet("SentManaToday", prototypeFriendsSentManaToday);
            LoadPrototypeFriendSet("ReceivedManaToday", prototypeFriendsReceivedManaToday);
        }
        else
        {
            prototypeFriendsSentManaToday.Clear();
            prototypeFriendsReceivedManaToday.Clear();
            PlayerPrefs.SetString(PrototypeFriendsSavePrefix + "DailyManaDate", today);
            SavePrototypeFriendsState();
        }
    }

    private void SavePrototypeFriendsState()
    {
        PlayerPrefs.SetString(PrototypeFriendsSavePrefix + "DailyManaDate", GetPrototypeFriendsTodayKey());
        SavePrototypeFriendSet("Friends", prototypeFriends);
        SavePrototypeFriendSet("IncomingRequests", prototypeIncomingFriendRequests);
        SavePrototypeFriendSet("SentRequests", prototypeSentFriendRequests);
        SavePrototypeFriendSet("SentManaToday", prototypeFriendsSentManaToday);
        SavePrototypeFriendSet("ReceivedManaToday", prototypeFriendsReceivedManaToday);
        SavePrototypeFriendSet("Blocked", prototypeBlockedFriends);
        SavePrototypeFriendSet("Reported", prototypeReportedFriends);
        PlayerPrefs.Save();
    }

    private void RefreshPrototypeFriendDailyManaState()
    {
        string today = GetPrototypeFriendsTodayKey();
        string savedDailyDate = PlayerPrefs.GetString(PrototypeFriendsSavePrefix + "DailyManaDate", today);
        if (savedDailyDate == today)
        {
            return;
        }

        prototypeFriendsSentManaToday.Clear();
        prototypeFriendsReceivedManaToday.Clear();
        PlayerPrefs.SetString(PrototypeFriendsSavePrefix + "DailyManaDate", today);
        SavePrototypeFriendsState();
    }

    private void LoadPrototypeFriendSet(string key, HashSet<string> target)
    {
        string fullKey = PrototypeFriendsSavePrefix + key;
        if (!PlayerPrefs.HasKey(fullKey))
        {
            return;
        }

        target.Clear();
        string saved = PlayerPrefs.GetString(fullKey, "");
        if (string.IsNullOrWhiteSpace(saved))
        {
            return;
        }

        string[] values = saved.Split(PrototypeFriendSaveSeparator);
        for (int index = 0; index < values.Length; index++)
        {
            string value = values[index].Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                target.Add(value);
            }
        }
    }

    private void SavePrototypeFriendSet(string key, HashSet<string> values)
    {
        List<string> sorted = new List<string>(values);
        sorted.Sort();
        PlayerPrefs.SetString(PrototypeFriendsSavePrefix + key, string.Join(PrototypeFriendSaveSeparator.ToString(), sorted));
    }

    private void ResetPrototypeFriendsState()
    {
        prototypeFriends.Clear();
        prototypeFriends.Add("Luna");
        prototypeFriends.Add("Eldric");
        prototypeFriends.Add("Mira");
        prototypeIncomingFriendRequests.Clear();
        prototypeIncomingFriendRequests.Add("Aster");
        prototypeIncomingFriendRequests.Add("Rowan");
        prototypeSentFriendRequests.Clear();
        prototypeSentFriendRequests.Add("Juniper");
        prototypeFriendsSentManaToday.Clear();
        prototypeFriendsReceivedManaToday.Clear();
        prototypeBlockedFriends.Clear();
        prototypeReportedFriends.Clear();
        lastFriendStatusSummary = "";
        SavePrototypeFriendsState();
    }

    private string GetPrototypeFriendsTodayKey()
    {
        return System.DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    }

    private void SendPrototypeFriendRequest()
    {
        string[] candidates = { "Selene", "Ivy", "Marisol" };
        for (int index = 0; index < candidates.Length; index++)
        {
            string candidate = candidates[index];
            if (!prototypeFriends.Contains(candidate) && !prototypeIncomingFriendRequests.Contains(candidate) && !prototypeSentFriendRequests.Contains(candidate))
            {
                prototypeSentFriendRequests.Add(candidate);
                lastFriendStatusSummary = $"Sent a prototype friend request to {candidate}.";
                SavePrototypeFriendsState();
                BuildFriendsUi();
                return;
            }
        }

        lastFriendStatusSummary = "All prototype friend request candidates are already in use.";
        BuildFriendsUi();
    }

    private void AcceptPrototypeFriendRequest(string friendName)
    {
        prototypeIncomingFriendRequests.Remove(friendName);
        prototypeFriends.Add(friendName);
        lastFriendStatusSummary = $"Accepted {friendName}'s friend request.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void DeclinePrototypeFriendRequest(string friendName)
    {
        prototypeIncomingFriendRequests.Remove(friendName);
        lastFriendStatusSummary = $"Declined {friendName}'s friend request.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void CancelPrototypeFriendRequest(string friendName)
    {
        prototypeSentFriendRequests.Remove(friendName);
        lastFriendStatusSummary = $"Canceled friend request to {friendName}.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void RemovePrototypeFriend(string friendName)
    {
        prototypeFriends.Remove(friendName);
        lastFriendStatusSummary = $"Removed {friendName} from friends.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void SendPrototypeFriendMana(string friendName)
    {
        if (prototypeBlockedFriends.Contains(friendName))
        {
            lastFriendStatusSummary = $"{friendName} is blocked.";
            BuildFriendsUi();
            return;
        }

        if (prototypeFriendsSentManaToday.Count >= PrototypeFriendDailyManaLimit || prototypeFriendsSentManaToday.Contains(friendName))
        {
            lastFriendStatusSummary = "Daily friend mana send limit reached for this prototype.";
            BuildFriendsUi();
            return;
        }

        prototypeFriendsSentManaToday.Add(friendName);
        lastFriendStatusSummary = $"Sent {PrototypeFriendManaAmount} mana to {friendName}. Prototype does not deduct player mana yet.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void ReceivePrototypeFriendMana(string friendName)
    {
        if (prototypeBlockedFriends.Contains(friendName))
        {
            lastFriendStatusSummary = $"{friendName} is blocked.";
            BuildFriendsUi();
            return;
        }

        if (prototypeFriendsReceivedManaToday.Count >= PrototypeFriendDailyManaLimit || prototypeFriendsReceivedManaToday.Contains(friendName))
        {
            lastFriendStatusSummary = "Daily friend mana receive limit reached for this prototype.";
            BuildFriendsUi();
            return;
        }

        prototypeFriendsReceivedManaToday.Add(friendName);
        lastFriendStatusSummary = $"Received {PrototypeFriendManaAmount} mana from {friendName}. Inventory grant waits for Aura-rank limits to be locked.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void SendPrototypeFriendMessage(string friendName)
    {
        if (prototypeBlockedFriends.Contains(friendName))
        {
            lastFriendStatusSummary = $"{friendName} is blocked.";
            BuildFriendsUi();
            return;
        }

        BuildFriendMessageComposerUi(friendName);
    }

    private void BuildFriendMessageComposerUi(string friendName)
    {
        RemoveFriendMessageComposer();

        GameObject panel = CreateAnchoredPanel(contentRoot, "FriendMessageComposer", new Color(0.055f, 0.028f, 0.085f, 0.995f), 700, 410, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, $"MESSAGE {friendName.ToUpperInvariant()}", 36, FontStyle.Bold, gold, 620, 46, 0f, 156f);
        CreateAnchoredText(panel.transform, "Prototype note only. Sent messages live in Inbox > Messages.", 15, FontStyle.Bold, muted, 620, 28, 0f, 118f);

        prototypeFriendMessageInput = CreateAnchoredInputField(panel.transform, "FriendMessageInput", "Type a short message...", 560, 96, 0f, 42f);
        prototypeFriendMessageInput.text = $"Hi {friendName}! ";

        Button help = CreateAnchoredButton(panel.transform, "Need help?", 13, 140, 30, violet, -170f, -38f);
        help.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Hi {friendName}! Could you help if you have extras today?"));
        Button thanks = CreateAnchoredButton(panel.transform, "Thanks!", 13, 140, 30, violet, 0f, -38f);
        thanks.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Thanks for the help, {friendName}!"));
        Button luck = CreateAnchoredButton(panel.transform, "Good luck", 13, 140, 30, violet, 170f, -38f);
        luck.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Good luck in your rooms today, {friendName}!"));

        prototypeFriendMessageStatusText = CreateAnchoredText(panel.transform, "", 14, FontStyle.Bold, new Color(1f, 0.58f, 0.58f), 560, 26, 0f, -82f);

        Button cancel = CreateAnchoredButton(panel.transform, "Cancel", 18, 170, 44, new Color(0.18f, 0.16f, 0.22f), -110f, -150f);
        cancel.onClick.AddListener(RemoveFriendMessageComposer);
        Button send = CreateAnchoredButton(panel.transform, "Send", 18, 170, 44, new Color(0.12f, 0.55f, 0.08f), 110f, -150f);
        send.onClick.AddListener(() => SendComposedPrototypeFriendMessage(friendName));
    }

    private void SetPrototypeFriendMessageDraft(string message)
    {
        if (prototypeFriendMessageInput != null)
        {
            prototypeFriendMessageInput.text = message;
            prototypeFriendMessageInput.caretPosition = prototypeFriendMessageInput.text.Length;
        }

        if (prototypeFriendMessageStatusText != null)
        {
            prototypeFriendMessageStatusText.text = "";
        }
    }

    private void SendComposedPrototypeFriendMessage(string friendName)
    {
        string draft = prototypeFriendMessageInput != null ? prototypeFriendMessageInput.text.Trim() : "";
        if (string.IsNullOrEmpty(draft))
        {
            if (prototypeFriendMessageStatusText != null)
            {
                prototypeFriendMessageStatusText.text = "Write a message before sending.";
            }

            return;
        }

        coven.EnqueueFriendMessageForPlayer(friendName, draft, true);
        lastInboxClaimSummary = $"Message to {friendName} added to Inbox > Messages.";
        lastFriendStatusSummary = $"Sent a prototype message to {friendName}.";
        inboxActiveCategory = PrototypeInboxCategory.Messages;
        RemoveFriendMessageComposer();
        RemoveFriendsModal();
        BuildInboxUi();
    }

    private void BlockPrototypeFriend(string friendName)
    {
        prototypeBlockedFriends.Add(friendName);
        lastFriendStatusSummary = $"Blocked {friendName}. Prototype blocks friend mana and messaging actions.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void ReportPrototypeFriend(string friendName)
    {
        prototypeReportedFriends.Add(friendName);
        lastFriendStatusSummary = $"Report filed for {friendName}. Moderation workflow is placeholder-only.";
        SavePrototypeFriendsState();
        BuildFriendsUi();
    }

    private void RemoveFriendsModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        RemoveFriendMessageComposer();
        Transform existing = contentRoot.Find("FriendsModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveFriendMessageComposer()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("FriendMessageComposer");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        prototypeFriendMessageInput = null;
        prototypeFriendMessageStatusText = null;
    }

    private void BuildLeaderboardUi()
    {
        RemoveLeaderboardModal();
        RemoveFriendsModal();
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "LeaderboardModal", new Color(0.055f, 0.028f, 0.085f, 0.985f), 1040, 620, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, "LEADERBOARDS", 44, FontStyle.Bold, gold, 860, 58, 0f, 258f);
        CreateAnchoredText(panel.transform, "Prototype gameplay standings only. Rewards, seasons, scoring formula, and anti-abuse rules are not active.", 15, FontStyle.Bold, muted, 880, 34, 0f, 216f);

        CreateLeaderboardTabButton(panel.transform, "Friends", -240f, 168f);
        CreateLeaderboardTabButton(panel.transform, "Team", 0f, 168f);
        CreateLeaderboardTabButton(panel.transform, "Global", 240f, 168f);

        GameObject board = CreateAnchoredPanel(panel.transform, "LeaderboardRows", new Color(0.1f, 0.045f, 0.14f), 850, 312, 0f, -22f);
        CreateAnchoredText(board.transform, GetLeaderboardTitle(), 24, FontStyle.Bold, gold, 760, 32, 0f, 124f);
        CreateAnchoredText(board.transform, "Rank", 14, FontStyle.Bold, muted, 80, 22, -352f, 86f);
        CreateAnchoredText(board.transform, "Player", 14, FontStyle.Bold, muted, 250, 22, -200f, 86f);
        CreateAnchoredText(board.transform, "Coven", 14, FontStyle.Bold, muted, 190, 22, 60f, 86f);
        CreateAnchoredText(board.transform, "Weekly Score", 14, FontStyle.Bold, muted, 160, 22, 298f, 86f);

        string[,] rows = GetPrototypeLeaderboardRows();
        for (int index = 0; index < rows.GetLength(0); index++)
        {
            CreateLeaderboardRow(board.transform, index + 1, rows[index, 0], rows[index, 1], rows[index, 2], 48f - index * 48f);
        }

        GameObject scope = CreateAnchoredPanel(panel.transform, "LeaderboardScope", new Color(0.13f, 0.08f, 0.15f), 850, 76, 0f, -220f);
        CreateAnchoredText(scope.transform, "Beta note: this is a UI shell for friends, Coven/team, and global views. Final metrics, reset cadence, rewards, ties, moderation, and backend syncing remain open.", 14, FontStyle.Bold, muted, 790, 54, 0f, 0f);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, violet, 0f, -286f);
        close.onClick.AddListener(RemoveLeaderboardModal);
    }

    private void CreateLeaderboardTabButton(Transform parent, string tab, float x, float y)
    {
        bool active = prototypeLeaderboardTab == tab;
        Button button = CreateAnchoredButton(parent, tab, 18, 200, 42, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        button.onClick.AddListener(() =>
        {
            prototypeLeaderboardTab = tab;
            BuildLeaderboardUi();
        });
    }

    private string GetLeaderboardTitle()
    {
        if (prototypeLeaderboardTab == "Team")
        {
            return "Coven / Team Standings";
        }

        if (prototypeLeaderboardTab == "Global")
        {
            return "Global Standings";
        }

        return "Friends Standings";
    }

    private string[,] GetPrototypeLeaderboardRows()
    {
        if (prototypeLeaderboardTab == "Team")
        {
            return new string[,]
            {
                { "Moonpetal Circle", "Your Coven", "18,420" },
                { "Starlit Hearth", "Coven", "17,960" },
                { "Azalea Coven", "Coven", "16,780" },
                { "Silver Dew Circle", "Coven", "15,230" },
                { "Dawnroot Guild", "Coven", "14,910" }
            };
        }

        if (prototypeLeaderboardTab == "Global")
        {
            return new string[,]
            {
                { "Celeste", "Astral Bloom", "42,300" },
                { "Marisol", "Moonpetal Circle", "39,880" },
                { "You", "Moonpetal Circle", "36,240" },
                { "Rowan", "Starlit Hearth", "35,910" },
                { "Luna", "Moonpetal Circle", "34,700" }
            };
        }

        return new string[,]
        {
            { "You", "Moonpetal Circle", "36,240" },
            { "Luna", "Moonpetal Circle", "34,700" },
            { "Mira", "Moonpetal Circle", "31,980" },
            { "Eldric", "Moonpetal Circle", "29,450" },
            { "Juniper", "Pending", "24,120" }
        };
    }

    private void CreateLeaderboardRow(Transform parent, int rank, string playerName, string covenName, string score, float y)
    {
        bool isPlayer = playerName == "You";
        Color rowColor = isPlayer ? new Color(0.96f, 0.86f, 0.62f) : new Color(0.18f, 0.13f, 0.2f);
        Color textColor = isPlayer ? new Color(0.18f, 0.08f, 0.32f) : Color.white;
        GameObject row = CreateAnchoredPanel(parent, $"LeaderboardRow_{rank}", rowColor, 780, 40, 0f, y);
        CreateAnchoredText(row.transform, $"#{rank}", 16, FontStyle.Bold, textColor, 78, 22, -350f, 0f);
        CreateAnchoredText(row.transform, playerName, 16, FontStyle.Bold, textColor, 238, 22, -200f, 0f);
        CreateAnchoredText(row.transform, covenName, 14, FontStyle.Bold, textColor, 190, 22, 60f, 0f);
        CreateAnchoredText(row.transform, score, 16, FontStyle.Bold, textColor, 150, 22, 298f, 0f);
    }

    private void RemoveLeaderboardModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("LeaderboardModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildMayhemMarketUi()
    {
        RemoveMayhemMarketModal();
        RemoveLeaderboardModal();
        RemoveFriendsModal();
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "MayhemMarketModal", new Color(0.055f, 0.028f, 0.085f, 0.985f), 1060, 630, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, "MAYHEM MARKET", 44, FontStyle.Bold, gold, 880, 58, 0f, 262f);
        CreateAnchoredText(panel.transform, "In-app purchase storefront shell. Products, prices, grants, purchase restore, platform IAP, and receipt validation are not active.", 15, FontStyle.Bold, muted, 900, 36, 0f, 220f);

        GameObject wallet = CreateAnchoredPanel(panel.transform, "MarketWallet", new Color(0.11f, 0.045f, 0.16f), 900, 54, 0f, 168f);
        CreateAnchoredText(wallet.transform, $"Current wallet preview: {inventory.GetManaText()} Mana | {inventory.GetCrystalText()} Crystals", 20, FontStyle.Bold, Color.white, 820, 28, 0f, 0f);

        CreateMarketOfferCard(panel.transform, "Mana", "Small Mana Pouch", "Price TBD\nGrant TBD", -330f, 60f);
        CreateMarketOfferCard(panel.transform, "Crystals", "Crystal Bundle", "Price TBD\nGrant TBD", 0f, 60f);
        CreateMarketOfferCard(panel.transform, "Cards", "Card Pack Offer", "Odds TBD\nPack rules TBD", 330f, 60f);
        CreateMarketOfferCard(panel.transform, "Cosmetic", "Avatar / Dauber", "Catalog TBD\nNo gameplay edge", -330f, -118f);
        CreateMarketOfferCard(panel.transform, "Special", "Starter / Event", "Limits TBD\nFairness review", 0f, -118f);
        CreateMarketOfferCard(panel.transform, "Restore", "Restore Purchases", "Platform IAP later\nNo-op prototype", 330f, -118f);

        GameObject rules = CreateAnchoredPanel(panel.transform, "MarketRules", new Color(0.13f, 0.08f, 0.15f), 900, 70, 0f, -236f);
        CreateAnchoredText(rules.transform, "Guardrails: purchases may support comfort/cosmetics, but cannot bypass core progression or let players pay their way up Aura Rank.", 14, FontStyle.Bold, muted, 830, 44, 0f, 0f);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, violet, 0f, -292f);
        close.onClick.AddListener(RemoveMayhemMarketModal);
    }

    private void CreateMarketOfferCard(Transform parent, string category, string title, string detail, float x, float y)
    {
        GameObject card = CreateAnchoredPanel(parent, $"MarketOffer_{category}", new Color(0.96f, 0.86f, 0.62f), 280, 148, x, y);
        CreateAnchoredText(card.transform, category.ToUpperInvariant(), 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 240, 22, 0f, 48f);
        CreateAnchoredText(card.transform, title, 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 240, 28, 0f, 20f);
        CreateAnchoredText(card.transform, detail, 13, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 230, 42, 0f, -18f);
        Button button = CreateAnchoredButton(card.transform, "Purchase Locked", 13, 180, 28, new Color(0.34f, 0.32f, 0.38f), 0f, -56f);
        button.interactable = false;
    }

    private void RemoveMayhemMarketModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("MayhemMarketModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildBewitchmentBazaarUi()
    {
        RemoveBewitchmentBazaarModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "BewitchmentBazaarModal", new Color(0.055f, 0.028f, 0.085f, 0.985f), 1140, 650, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, "BEWITCHMENT BAZAAR", 40, FontStyle.Bold, gold, 980, 54, 0f, 274f);
        CreateAnchoredText(panel.transform, "Prototype market hub. Center cart is the card/crystal swap area; Madame Solange and Coven are nearby destinations.", 15, FontStyle.Bold, muted, 980, 36, 0f, 232f);

        GameObject oracleAlley = CreateAnchoredPanel(panel.transform, "BazaarOracleAlley", new Color(0.11f, 0.045f, 0.16f), 250, 390, -430f, 28f);
        Button oracleArea = oracleAlley.AddComponent<Button>();
        oracleArea.onClick.AddListener(BuildOracleAlleyReadingUi);
        CreateAnchoredText(oracleAlley.transform, "ORACLE\nALLEY", 32, FontStyle.Bold, gold, 210, 82, 0f, 106f);
        CreateAnchoredText(oracleAlley.transform, "Madame Solange", 18, FontStyle.Bold, Color.white, 210, 30, 0f, 34f);
        CreateAnchoredText(oracleAlley.transform, "Reading table", 15, FontStyle.Bold, muted, 210, 28, 0f, -4f);
        CreateAnchoredText(oracleAlley.transform, "Tap the table", 13, FontStyle.Bold, muted, 190, 28, 0f, -138f);

        GameObject cart = CreateAnchoredPanel(panel.transform, "BazaarCardCart", new Color(0.14f, 0.07f, 0.16f), 570, 390, -42f, 28f);
        CreateAnchoredText(cart.transform, "CARD / CRYSTAL SWAP CART", 30, FontStyle.Bold, gold, 510, 42, 0f, 148f);
        CreateAnchoredText(cart.transform, "Daily market placeholder", 17, FontStyle.Bold, muted, 420, 24, 0f, 112f);
        CreateBazaarMarketCard(cart.transform, "Moonpetal", "30", -210f, 38f);
        CreateBazaarMarketCard(cart.transform, "Dewdrop", "35", -126f, 38f);
        CreateBazaarMarketCard(cart.transform, "Nightshade", "28", -42f, 38f);
        CreateBazaarMarketCard(cart.transform, "Stardust", "40", 42f, 38f);
        CreateBazaarMarketCard(cart.transform, "Silver Shell", "32", 126f, 38f);
        CreateBazaarMarketCard(cart.transform, "Dreamsand", "50", 210f, 38f);
        Button swap = CreateAnchoredButton(cart.transform, "Swap Later", 20, 230, 48, new Color(0.34f, 0.32f, 0.38f), 0f, -138f);
        swap.interactable = false;

        GameObject covenDoor = CreateAnchoredPanel(panel.transform, "BazaarCovenDoor", new Color(0.13f, 0.08f, 0.12f), 250, 390, 430f, 28f);
        Button covenArea = covenDoor.AddComponent<Button>();
        covenArea.onClick.AddListener(BuildCovenCircleUi);
        CreateAnchoredText(covenDoor.transform, "COVEN", 34, FontStyle.Bold, gold, 210, 48, 0f, 114f);
        CreateAnchoredText(covenDoor.transform, coven.IsJoined ? coven.CovenName : "Coven Circle", 17, FontStyle.Bold, Color.white, 210, 30, 0f, 58f);
        CreateAnchoredText(covenDoor.transform, "Social, orbs, requests,\nand member help live here.", 15, FontStyle.Bold, muted, 210, 74, 0f, -10f);
        CreateAnchoredText(covenDoor.transform, "Tap the door", 13, FontStyle.Bold, muted, 190, 28, 0f, -138f);

        CreateAnchoredText(panel.transform, "Swap costs and Bazaar catalog are placeholders only; no purchase economy is active in this pass.", 15, FontStyle.Bold, muted, 960, 36, 0f, -206f);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, violet, 0f, -292f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void CreateBazaarMarketCard(Transform parent, string title, string crystalCost, float x, float y)
    {
        GameObject card = CreateAnchoredPanel(parent, $"BazaarCard_{title}", new Color(0.96f, 0.86f, 0.62f), 74, 128, x, y);
        CreateAnchoredText(card.transform, "*", 32, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 56, 34, 0f, 32f);
        CreateAnchoredText(card.transform, title, 10, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 66, 26, 0f, -6f);
        CreateAnchoredText(card.transform, "<> " + crystalCost, 13, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 66, 22, 0f, -44f);
    }

    private void BuildOracleAlleyReadingUi()
    {
        RemoveBewitchmentBazaarModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "BewitchmentBazaarModal", new Color(0.05f, 0.02f, 0.075f, 0.99f), 1140, 650, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);
        bool readingComplete = prototypeOracleSelectedCards.Count >= 3;

        CreateAnchoredText(panel.transform, "ORACLE ALLEY", 42, FontStyle.Bold, gold, 820, 54, 0f, 274f);
        CreateAnchoredText(panel.transform, "MADAME SOLANGE", 25, FontStyle.Bold, gold, 420, 36, -372f, 206f);
        CreateAnchoredText(panel.transform, "Visual reading prototype only. Costs, odds, dust conversion, rewards, and Book of Shadows rules remain TBD.", 15, FontStyle.Bold, muted, 920, 36, 52f, 232f);

        GameObject table = CreateAnchoredPanel(panel.transform, "MadameSolangeReadingTable", new Color(0.13f, 0.045f, 0.14f), 900, 390, 0f, 22f);
        CreateAnchoredText(table.transform, "PICK 3 CARDS", 28, FontStyle.Bold, gold, 260, 42, -320f, 116f);
        CreateAnchoredText(table.transform, $"{Mathf.Max(0, 3 - prototypeOracleSelectedCards.Count)} of 3 choices left | Cost TBD", 17, FontStyle.Bold, muted, 300, 30, -300f, 70f);
        for (int index = 0; index < 5; index++)
        {
            CreateOracleFaceDownCard(table.transform, -240f + index * 120f, -22f, index + 1);
        }

        if (readingComplete)
        {
            CreateAnchoredPanel(table.transform, "OracleBonusPreview", new Color(0.08f, 0.04f, 0.12f), 330, 86, 280f, 102f);
            CreateAnchoredText(table.transform, "BOOK OF SHADOWS BONUS", 17, FontStyle.Bold, gold, 300, 24, 280f, 120f);
            CreateAnchoredText(table.transform, "Placeholder only - no bonus granted", 13, FontStyle.Bold, muted, 300, 28, 280f, 94f);
            CreateAnchoredText(table.transform, "The vision is complete. Outcome categories are previews only.", 16, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 720, 32, 0f, -160f);
        }
        else
        {
            CreateAnchoredText(table.transform, string.IsNullOrWhiteSpace(lastOracleReadingSummary) ? "Tap a card to preview a reveal. Madame Solange is not a general helper." : lastOracleReadingSummary, 16, FontStyle.Bold, Color.white, 720, 42, 0f, -160f);
        }

        Button back = CreateAnchoredButton(panel.transform, "Back to Bazaar", 20, 220, 48, violet, -130f, -292f);
        back.onClick.AddListener(BuildBewitchmentBazaarUi);
        Button reset = CreateAnchoredButton(panel.transform, readingComplete ? "Close Reading" : "Reset Reading", 20, 220, 48, readingComplete ? new Color(0.12f, 0.55f, 0.08f) : violet, 130f, -292f);
        reset.onClick.AddListener(() =>
        {
            ResetPrototypeOracleReading();
            if (readingComplete)
            {
                BuildBewitchmentBazaarUi();
                return;
            }

            BuildOracleAlleyReadingUi();
        });
    }

    private void CreateOracleFaceDownCard(Transform parent, float x, float y, int index)
    {
        bool selected = prototypeOracleSelectedCards.Contains(index);
        Color cardColor = selected ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.16f, 0.06f, 0.24f);
        GameObject card = CreateAnchoredPanel(parent, $"OracleFaceDown_{index}", cardColor, 86, 136, x, y);
        Button button = card.AddComponent<Button>();
        button.interactable = selected || prototypeOracleSelectedCards.Count < 3;
        button.onClick.AddListener(() => SelectPrototypeOracleCard(index));

        if (selected)
        {
            CreateAnchoredText(card.transform, GetPrototypeOracleOutcomeIcon(index), 28, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 70, 38, 0f, 30f);
            CreateAnchoredText(card.transform, GetPrototypeOracleOutcomeLabel(index), 12, FontStyle.Bold, Color.white, 76, 52, 0f, -14f);
            CreateAnchoredText(card.transform, "Preview", 10, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 74, 18, 0f, -52f);
            return;
        }

        CreateAnchoredText(card.transform, "*", 38, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 60, 44, 0f, 28f);
        CreateAnchoredText(card.transform, "EYE", 18, FontStyle.Bold, Color.white, 70, 28, 0f, -16f);
    }

    private void SelectPrototypeOracleCard(int index)
    {
        if (prototypeOracleSelectedCards.Contains(index) || prototypeOracleSelectedCards.Count >= 3)
        {
            return;
        }

        prototypeOracleSelectedCards.Add(index);
        lastOracleReadingSummary = $"Revealed preview: {GetPrototypeOracleOutcomeLabel(index)}. No inventory changed.";
        BuildOracleAlleyReadingUi();
    }

    private void ResetPrototypeOracleReading()
    {
        prototypeOracleSelectedCards.Clear();
        lastOracleReadingSummary = "";
    }

    private string GetPrototypeOracleOutcomeIcon(int index)
    {
        switch (index)
        {
            case 1: return "<>";
            case 2: return "*";
            case 3: return "?";
            case 4: return "+";
            default: return "...";
        }
    }

    private string GetPrototypeOracleOutcomeLabel(int index)
    {
        switch (index)
        {
            case 1: return "Crystal\nPreview";
            case 2: return "Star\nPreview";
            case 3: return "Card Pack\nPreview";
            case 4: return "Dust\nPreview";
            default: return "Vision\nFades";
        }
    }

    private void RemoveBewitchmentBazaarModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("BewitchmentBazaarModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildSocialFreebiesUi()
    {
        RemoveInboxModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();
        RemoveSocialFreebiesModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "SocialFreebiesModal", new Color(0.07f, 0.035f, 0.12f, 0.985f), 860, 430, 0f, -24f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        CreateAnchoredText(panel.transform, "SOCIAL FREEBIES", 42, FontStyle.Bold, gold, 720, 56, 0f, 156f);
        CreateAnchoredText(panel.transform, "Follow our social feeds. Freebie posts link back into the game and redeem rewards automatically.", 16, FontStyle.Bold, muted, 720, 48, 0f, 106f);

        Button instagram = CreateAnchoredButton(panel.transform, "Instagram", 18, 190, 48, new Color(0.55f, 0.18f, 0.78f), -230f, 28f);
        instagram.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeInstagramUrl, "Instagram"));
        Button facebook = CreateAnchoredButton(panel.transform, "Facebook", 18, 190, 48, new Color(0.14f, 0.25f, 0.48f), 0f, 28f);
        facebook.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeFacebookUrl, "Facebook"));
        Button x = CreateAnchoredButton(panel.transform, "X", 18, 190, 48, new Color(0.1f, 0.1f, 0.12f), 230f, 28f);
        x.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeXUrl, "X"));

        Button simulate = CreateAnchoredButton(panel.transform, "Simulate Freebie Link", 18, 260, 42, new Color(0.12f, 0.55f, 0.08f), 0f, -58f);
        simulate.onClick.AddListener(() => HandlePrototypeDeepLink(BuildPrototypeSocialFreebieAppUrl()));

        CreateAnchoredText(panel.transform, "Prototype social URLs are placeholders. Freebie reward values are placeholders, not final economy tuning.", 14, FontStyle.Bold, muted, 700, 44, 0f, -112f);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -158f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void OpenSocialFreebiesUrl(string url, string label)
    {
        Application.OpenURL(url);
        coven.AddPrototypeChatLine($"Opened {label} freebies feed.");
    }

    private void HandlePrototypeDeepLink(string url)
    {
        if (!TryExtractPrototypeFreebieCode(url, out string code, out System.DateTime expiresAtUtc))
        {
            return;
        }

        RedeemPrototypeSocialFreebieCode(code, expiresAtUtc);
    }

    private bool TryExtractPrototypeFreebieCode(string url, out string code, out System.DateTime expiresAtUtc)
    {
        code = "";
        expiresAtUtc = System.DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        string lower = url.ToLowerInvariant();
        if (!lower.Contains("freebie") || !lower.Contains("code="))
        {
            return false;
        }

        int codeStart = lower.IndexOf("code=", System.StringComparison.Ordinal) + 5;
        if (codeStart < 5 || codeStart >= url.Length)
        {
            return false;
        }

        int codeEnd = url.IndexOf('&', codeStart);
        code = codeEnd >= 0 ? url.Substring(codeStart, codeEnd - codeStart) : url.Substring(codeStart);
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        string expirationText = ExtractQueryValue(url, "exp");
        if (string.IsNullOrWhiteSpace(expirationText))
        {
            expirationText = ExtractQueryValue(url, "expires");
        }

        if (!TryParseSocialFreebieExpiration(expirationText, out expiresAtUtc))
        {
            expiresAtUtc = System.DateTime.MinValue;
        }

        return true;
    }

    private void RedeemPrototypeSocialFreebieCode(string code, System.DateTime expiresAtUtc)
    {
        bool redeemed = inventory.TryRedeemSocialFreebieCode(
            code,
            BuildPrototypeSocialFreebieReward(),
            expiresAtUtc,
            out string message);
        coven.AddPrototypeChatLine(message);
        RefreshPowerUpDisplays();

        if (contentRoot != null)
        {
            if (redeemed)
            {
                BuildPlayerDenUi();
            }
            else
            {
                BuildSocialFreebiesUi();
            }
        }
    }

    private PrototypeRewardGrant BuildPrototypeSocialFreebieReward()
    {
        return new PrototypeRewardGrant(
            "Social Freebie",
            250,
            5,
            0,
            0,
            new List<PrototypeRewardItem> { new PrototypeRewardItem("Single Sigil", 1) });
    }

    private string BuildPrototypeSocialFreebieAppUrl()
    {
        string expiration = System.DateTime.UtcNow.Date.AddDays(SocialFreebieExpirationDays).ToString("yyyyMMdd");
        return $"bingomagicmayhem://freebie?code={PrototypeSocialFreebieCode}&exp={expiration}";
    }

    private static string ExtractQueryValue(string url, string key)
    {
        string search = key + "=";
        int start = url.ToLowerInvariant().IndexOf(search, System.StringComparison.Ordinal);
        if (start < 0)
        {
            return "";
        }

        start += search.Length;
        if (start >= url.Length)
        {
            return "";
        }

        int end = url.IndexOf('&', start);
        return end >= 0 ? url.Substring(start, end - start) : url.Substring(start);
    }

    private static bool TryParseSocialFreebieExpiration(string value, out System.DateTime expiresAtUtc)
    {
        expiresAtUtc = System.DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (System.DateTime.TryParseExact(
            value.Trim(),
            "yyyyMMdd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
            out System.DateTime parsed))
        {
            expiresAtUtc = parsed.Date.AddDays(1);
            return true;
        }

        return false;
    }

    private GameObject CreateDenBottomButton(string title, string subtitle, float x)
    {
        GameObject tile = CreateAnchoredPanel(contentRoot, $"DenBottom_{title}", new Color(0.13f, 0.07f, 0.18f), 154, 86, x, -378f);
        CreateAnchoredText(tile.transform, title, 19, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 132, 28, 0f, 18f);
        CreateAnchoredText(tile.transform, subtitle, 12, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 132, 24, 0f, -18f);
        return tile;
    }

    private void BuildDailyBonusUi()
    {
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailySpinModal();
        RemoveDailyBonusModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "DailyBonusModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1220, 650, 0f, -34f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        CreateAnchoredText(panel.transform, "DAILY BONUS", 54, FontStyle.Bold, gold, 800, 66, 0f, 270f);
        Button info = CreateAnchoredButton(panel.transform, "i", 22, 42, 42, new Color(0.18f, 0.16f, 0.28f), 338f, 272f);
        info.onClick.AddListener(() => coven.AddPrototypeChatLine("Daily Bonus: claim once per day. The 7-day track is the main loop; the streak rail tracks long-running chest milestones."));
        CreateAnchoredText(panel.transform, $"Daily Streak: {inventory.DailyBonusStreak} days", 26, FontStyle.Bold, Color.white, 420, 42, -350f, 214f);
        int nextChest = inventory.GetNextDailyBonusChestDay();
        CreateAnchoredText(panel.transform, $"Next Chest: Day {nextChest}", 26, FontStyle.Bold, Color.white, 420, 42, 350f, 214f);

        if (inventory.HasMissedDailyBonusStreak())
        {
            CreateAnchoredText(panel.transform, "You missed a day. Save the streak or claim to restart at Day 1.", 17, FontStyle.Bold, new Color(1f, 0.86f, 0.62f), 760, 28, 0f, 184f);
            Button saveStreak = CreateAnchoredButton(panel.transform, inventory.GetDailyBonusStreakSaveSummary(), 16, 280, 38, inventory.CanUseDailyBonusStreakSave() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, 184f);
            saveStreak.interactable = inventory.CanUseDailyBonusStreakSave();
            saveStreak.onClick.AddListener(SaveDailyBonusStreakFromDen);
        }

        CreateDailyBonusMilestoneRail(panel.transform, nextChest);
        CreateAnchoredText(panel.transform, "7-DAY DAILY BONUS", 34, FontStyle.Bold, gold, 620, 48, 0f, 62f);

        for (int day = 1; day <= 7; day++)
        {
            DailyBonusRewardDefinition reward = inventory.GetDailyBonusRewardForDay(day);
            CreateDailyRewardTile(panel.transform, reward, day, -480f + (day - 1) * 160f, -82f);
        }

        string claimText = inventory.HasMissedDailyBonusStreak() ? "Let Streak Reset" : inventory.CanClaimDailyBonus() ? "Claim" : "Claimed Today";
        Button claim = CreateAnchoredButton(panel.transform, claimText, 24, 270, 54, inventory.CanClaimDailyBonus() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, -262f);
        claim.interactable = inventory.CanClaimDailyBonus();
        claim.onClick.AddListener(ClaimDailyBonusFromDen);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), -470f, -262f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void CreateDailyBonusMilestoneRail(Transform parent, int nextChest)
    {
        int[] milestones = { 7, 14, 30, 60, 100, 180, 365 };
        float railWidth = 860f;
        int railMax = Mathf.Max(100, nextChest);
        CreateAnchoredPanel(parent, "DailyBonusRail", new Color(0.86f, 0.53f, 0.05f), railWidth, 16, 0f, 150f);
        for (int index = 0; index < milestones.Length; index++)
        {
            int day = milestones[index];
            if (day > railMax)
            {
                continue;
            }

            float x = -railWidth * 0.5f + (day / (float)railMax) * railWidth;
            bool reached = inventory.DailyBonusStreak >= day;
            bool target = day == nextChest;
            GameObject marker = CreateAnchoredPanel(parent, $"DailyMilestone_{day}", target ? new Color(0.55f, 0.18f, 0.78f) : reached ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.2f, 0.12f, 0.28f), target ? 74 : 58, target ? 58 : 46, x, 150f);
            CreateAnchoredText(marker.transform, target ? "CHEST" : "GIFT", target ? 13 : 11, FontStyle.Bold, Color.white, target ? 66 : 50, 18, 0f, 8f);
            CreateAnchoredText(parent, day.ToString(), 22, FontStyle.Bold, Color.white, 80, 26, x, 108f);
        }

        int progressTarget = Mathf.Max(1, nextChest);
        CreateAnchoredText(parent, $"{Mathf.Min(inventory.DailyBonusStreak, progressTarget)}/{progressTarget}", 24, FontStyle.Bold, Color.white, 160, 34, 0f, 108f);
    }

    private void CreateDailyRewardTile(Transform parent, DailyBonusRewardDefinition reward, int day, float x, float y)
    {
        bool current = inventory.GetDailyBonusDisplayDay() == day;
        bool claimedCurrent = current && !inventory.CanClaimDailyBonus();
        bool ready = current && inventory.CanClaimDailyBonus();
        Color tileColor = ready ? new Color(0.72f, 0.44f, 0.08f) : new Color(0.18f, 0.08f, 0.28f);
        GameObject tile = CreateAnchoredPanel(parent, $"DailyReward_{day}", tileColor, 142, 190, x, y);
        CreateAnchoredText(tile.transform, $"DAY {day}", 23, FontStyle.Bold, Color.white, 124, 28, 0f, 68f);
        CreateAnchoredText(tile.transform, GetDailyRewardIcon(reward), 38, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 120, 54, 0f, 18f);
        string rewardText = reward.Label == "Daily Chest" ? "Daily Bonus\nChest" : reward.Summary;
        CreateAnchoredText(tile.transform, rewardText, 13, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 124, 54, 0f, -46f);

        if (claimedCurrent)
        {
            CreateAnchoredText(tile.transform, "CLAIMED", 14, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 112, 24, 0f, -78f);
        }
        else if (ready)
        {
            CreateAnchoredText(tile.transform, "READY", 14, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 112, 24, 0f, -78f);
        }
    }

    private string GetDailyRewardIcon(DailyBonusRewardDefinition reward)
    {
        if (reward.Label == "Daily Chest") return "CHEST";
        if (reward.Crystals > 0) return "<>";
        if (reward.ItemName == "Regular Card") return "CARD";
        if (reward.ItemName == "Clairvoyance") return "EYE";
        if (reward.ItemName.Contains("Sigil")) return "SIGIL";
        return "*";
    }

    private void ClaimDailyBonusFromDen()
    {
        DailyBonusRewardDefinition reward = inventory.ClaimDailyBonus();
        coven.AddPrototypeChatLine($"Daily bonus collected: {reward.Summary}");
        RefreshPowerUpDisplays();
        BuildDailyBonusUi();
    }

    private void SaveDailyBonusStreakFromDen()
    {
        bool saved = inventory.TryUseDailyBonusStreakSave();
        coven.AddPrototypeChatLine(saved ? "Daily streak saved. You can claim without resetting it." : "Daily streak save unavailable.");
        RefreshPowerUpDisplays();
        BuildDailyBonusUi();
    }

    private void BuildDailySpinUi()
    {
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "DailySpinModal", new Color(0.035f, 0.014f, 0.055f, 0.985f), 1320, 700, 0f, -42f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        bool ready = inventory.CanClaimDailySpin() && !dailySpinAnimating;

        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        CreateAnchoredPanel(panel.transform, "DailySpinHeaderRibbon", new Color(0.14f, 0.04f, 0.18f), 1040, 84, 0f, 270f);
        CreateAnchoredText(panel.transform, "Daily Wheelspin", 54, FontStyle.Bold, gold, 760, 64, -30f, 284f);
        CreateAnchoredText(panel.transform, "One free daily spin. Separate from Daily Bonus and jackpot wheelspin.", 18, FontStyle.Bold, muted, 800, 28, -30f, 238f);

        GameObject wheel = CreateAnchoredPanel(panel.transform, "DailySpinWheel", new Color(0.13f, 0.04f, 0.19f), 540, 540, -350f, 6f);
        BuildDailySpinWheel(wheel.transform, rewards, dailySpinAnimating ? -1 : lastDailySpinRewardIndex);

        GameObject prizePanel = CreateAnchoredPanel(panel.transform, "DailySpinPrizePanel", new Color(0.08f, 0.12f, 0.08f), 480, 426, 340f, 22f);
        CreateAnchoredText(prizePanel.transform, "TODAY'S SPIN", 28, FontStyle.Bold, gold, 420, 42, 0f, 162f);
        CreateAnchoredText(prizePanel.transform, dailySpinAnimating ? "Wheel spinning..." : ready ? "Free spin ready" : "Come back tomorrow", 20, FontStyle.Bold, Color.white, 400, 30, 0f, 122f);

        if (!dailySpinAnimating && lastDailySpinRewardIndex >= 0 && lastDailySpinRewardIndex < rewards.Count)
        {
            PrototypeRewardGrant result = rewards[lastDailySpinRewardIndex];
            CreateAnchoredText(prizePanel.transform, "RESULT", 18, FontStyle.Bold, muted, 380, 26, 0f, 64f);
            CreateAnchoredText(prizePanel.transform, BuildRewardGrantSummary(result), 36, FontStyle.Bold, gold, 420, 58, 0f, 16f);
            CreateAnchoredText(prizePanel.transform, "Reward added directly to inventory.", 16, FontStyle.Bold, muted, 380, 28, 0f, -34f);
        }
        else
        {
            CreateAnchoredText(prizePanel.transform, "SPIN FOR", 18, FontStyle.Bold, muted, 380, 26, 0f, 64f);
            CreateAnchoredText(prizePanel.transform, "Mana, Crystals,\nSigils, Boosts", 34, FontStyle.Bold, gold, 420, 94, 0f, 10f);
            CreateAnchoredText(prizePanel.transform, "Prototype reward values and odds are not final.", 16, FontStyle.Bold, muted, 380, 32, 0f, -62f);
        }

        if (!string.IsNullOrWhiteSpace(lastDailySpinSummary))
        {
            CreateAnchoredText(panel.transform, lastDailySpinSummary, 18, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 820, 36, 0f, -278f);
        }

        Button spin = CreateAnchoredButton(panel.transform, dailySpinAnimating ? "SPINNING" : ready ? "SPIN" : "SPUN", dailySpinAnimating ? 21 : 34, 180, 180, ready ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), -350f, 6f);
        spin.interactable = ready;
        spin.onClick.AddListener(ClaimDailySpinFromDen);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 340f, -214f);
        close.interactable = !dailySpinAnimating;
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildDailySpinWheel(Transform wheel, IReadOnlyList<PrototypeRewardGrant> rewards, int winningIndex)
    {
        CreateAnchoredPanel(wheel, "DailySpinOuterRing", new Color(0.78f, 0.48f, 0.06f), 558, 558, 0f, 0f);
        CreateAnchoredPanel(wheel, "DailySpinInnerPlate", new Color(0.09f, 0.02f, 0.14f), 500, 500, 0f, 0f);
        Color[] colors =
        {
            new Color(0.15f, 0.32f, 0.10f),
            new Color(0.17f, 0.08f, 0.34f),
            new Color(0.07f, 0.22f, 0.34f),
            new Color(0.42f, 0.20f, 0.05f),
            new Color(0.08f, 0.35f, 0.32f),
            new Color(0.34f, 0.08f, 0.34f)
        };

        int segmentCount = Mathf.Max(1, rewards.Count);
        float angleStep = 360f / segmentCount;
        for (int index = 0; index < rewards.Count; index++)
        {
            float angle = dailySpinWheelRotation + 90f - index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians) * 168f;
            float y = Mathf.Sin(radians) * 168f;
            bool winner = index == winningIndex;
            Color color = winner ? new Color(0.88f, 0.58f, 0.08f) : colors[index % colors.Length];
            float segmentWidth = segmentCount > 12 ? 118f : 162f;
            float segmentHeight = segmentCount > 12 ? 82f : 96f;
            GameObject segment = CreateAnchoredPanel(wheel, $"DailySpinSegment_{index}", color, winner ? segmentWidth + 18f : segmentWidth, segmentHeight, x, y);
            RectTransform rect = segment.GetComponent<RectTransform>();
            rect.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            float labelRadius = 186f;
            float labelX = Mathf.Cos(radians) * labelRadius;
            float labelY = Mathf.Sin(radians) * labelRadius;
            string labelText = GetDailySpinWheelLabel(rewards[index]);
            int labelSize = rewards[index].Mana > 0 ? 28 : 17;
            Text label = CreateAnchoredText(wheel, labelText, winner ? labelSize + 3 : labelSize, FontStyle.Bold, Color.white, 104, 42, labelX, labelY);
            label.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        CreateAnchoredPanel(wheel, "DailySpinPointer", new Color(0.78f, 0.48f, 0.06f), 42, 66, 0f, 262f);
        CreateAnchoredText(wheel, "v", 30, FontStyle.Bold, new Color(0.18f, 0.04f, 0.2f), 42, 42, 0f, 268f);
        CreateAnchoredPanel(wheel, "DailySpinHub", new Color(0.43f, 0.12f, 0.55f), 204, 204, 0f, 0f);
    }

    private string GetDailySpinWheelLabel(PrototypeRewardGrant reward)
    {
        if (reward.Mana > 0) return reward.Mana.ToString("N0");
        if (reward.Crystals > 0) return $"{reward.Crystals}<>";
        if (reward.ClairvoyanceMinutes > 0) return $"{reward.ClairvoyanceMinutes}m Boost";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Sigil")) return "SIGIL";
        if (reward.Items.Count > 0) return GetDailySpinFallbackLabel(reward.Items[0]);
        return "Reward";
    }

    private string GetDailySpinFallbackLabel(PrototypeRewardItem item)
    {
        return item.Quantity > 1 ? $"{item.Name}\nx{item.Quantity}" : item.Name;
    }

    private string GetDailySpinRewardIcon(PrototypeRewardGrant reward)
    {
        if (reward.Mana > 0) return "MANA";
        if (reward.Crystals > 0) return "<>";
        if (reward.ClairvoyanceMinutes > 0) return "EYE";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Pandora")) return "BOX";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Sigil")) return "SIGIL";
        return "*";
    }

    private void ClaimDailySpinFromDen()
    {
        if (!inventory.CanClaimDailySpin())
        {
            lastDailySpinSummary = "Already spun today.";
            BuildDailySpinUi();
            return;
        }

        PrototypeRewardGrant reward = inventory.ClaimDailySpin();
        string summary = BuildRewardGrantSummary(reward);
        lastDailySpinRewardIndex = FindDailySpinRewardIndex(reward);
        lastDailySpinSummary = "Wheel spinning...";
        coven.AddPrototypeChatLine($"Daily Spin collected: {summary}");
        RefreshPowerUpDisplays();
        dailySpinAnimating = true;
        dailySpinAnimationSequence++;
        int animationSequence = dailySpinAnimationSequence;
        BuildDailySpinUi();
        StartCoroutine(AnimateDailySpinWheel(animationSequence, summary, lastDailySpinRewardIndex));
    }

    private IEnumerator AnimateDailySpinWheel(int animationSequence, string summary, int targetIndex)
    {
        float startRotation = dailySpinWheelRotation;
        float targetRotation = startRotation + 1080f + GetDailySpinWheelDeltaToTarget(startRotation, targetIndex);
        float elapsed = 0f;
        const float duration = 1.35f;
        while (elapsed < duration)
        {
            if (animationSequence != dailySpinAnimationSequence)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            dailySpinWheelRotation = Mathf.Lerp(startRotation, targetRotation, eased);
            BuildDailySpinUi();
            yield return null;
        }

        if (animationSequence != dailySpinAnimationSequence)
        {
            yield break;
        }

        dailySpinWheelRotation = targetRotation;
        dailySpinAnimating = false;
        lastDailySpinSummary = $"Won: {summary}";
        BuildDailySpinUi();
    }

    private float GetDailySpinWheelDeltaToTarget(float startRotation, int targetIndex)
    {
        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        if (rewards.Count <= 0 || targetIndex < 0)
        {
            return 0f;
        }

        float angleStep = 360f / rewards.Count;
        float targetMod = Mathf.Repeat(targetIndex * angleStep, 360f);
        float currentMod = Mathf.Repeat(startRotation, 360f);
        return Mathf.Repeat(targetMod - currentMod, 360f);
    }

    private int FindDailySpinRewardIndex(PrototypeRewardGrant reward)
    {
        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        for (int index = 0; index < rewards.Count; index++)
        {
            PrototypeRewardGrant candidate = rewards[index];
            if (candidate.Mana == reward.Mana &&
                candidate.Crystals == reward.Crystals &&
                candidate.ClairvoyanceMinutes == reward.ClairvoyanceMinutes &&
                candidate.Items.Count == reward.Items.Count)
            {
                if (candidate.Items.Count == 0 ||
                    (candidate.Items[0].Name == reward.Items[0].Name && candidate.Items[0].Quantity == reward.Items[0].Quantity))
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private void BuildInboxUi()
    {
        RemoveInboxModal();
        RemoveLibraryGrimoireModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "InboxModal", new Color(0.07f, 0.035f, 0.12f, 0.985f), 840, 560, 0f, -22f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        CreateAnchoredText(panel.transform, "INBOX", 42, FontStyle.Bold, gold, 700, 56, 0f, 236f);
        CreateAnchoredText(panel.transform, GetInboxHeaderText(), 16, FontStyle.Bold, muted, 700, 30, 0f, 200f);
        CreateInboxTabButton(panel.transform, PrototypeInboxCategory.Messages, -210f, 158f);
        CreateInboxTabButton(panel.transform, PrototypeInboxCategory.Cards, 0f, 158f);
        CreateInboxTabButton(panel.transform, PrototypeInboxCategory.Gifts, 210f, 158f);

        IReadOnlyList<PrototypeInboxItem> incoming = coven.PlayerInboxItems;
        List<int> visibleItemIndexes = GetVisibleInboxItemIndexes(incoming, inboxActiveCategory);
        if (visibleItemIndexes.Count == 0)
        {
            CreateAnchoredText(panel.transform, GetEmptyInboxText(), 24, FontStyle.Bold, Color.white, 640, 46, 0f, 58f);
        }
        else
        {
            int visibleCount = Mathf.Min(visibleItemIndexes.Count, 4);
            for (int index = 0; index < visibleCount; index++)
            {
                int itemIndex = visibleItemIndexes[index];
                CreateInboxItemRow(panel.transform, incoming[itemIndex], itemIndex, 104f - index * 76f);
            }

            Button collectAll = CreateAnchoredButton(panel.transform, inboxActiveCategory == PrototypeInboxCategory.Messages ? "Clear All" : "Claim All", 18, 180, 42, new Color(0.12f, 0.55f, 0.08f), 276f, -246f);
            collectAll.onClick.AddListener(CollectAllVisibleInboxItems);
        }

        if (!string.IsNullOrWhiteSpace(lastInboxClaimSummary))
        {
            CreateAnchoredText(panel.transform, lastInboxClaimSummary, 14, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 690, 34, 0f, -194f);
        }
        else if (coven.InboxGifts.Count > 0)
        {
            CreateAnchoredText(panel.transform, BuildSentInboxPreview(), 13, FontStyle.Bold, muted, 690, 58, 0f, -194f);
        }

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), visibleItemIndexes.Count > 0 ? -42f : 0f, -246f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private string GetDenInboxStatusText()
    {
        int count = coven.PlayerInboxClaimableCount;
        int unread = coven.PlayerInboxUnreadCount;
        if (count <= 0)
        {
            return "0 waiting";
        }

        return unread > 0 ? $"{count} waiting | {unread} new" : $"{count} waiting";
    }

    private void CreateInboxTabButton(Transform parent, PrototypeInboxCategory category, float x, float y)
    {
        bool active = inboxActiveCategory == category;
        Button tab = CreateAnchoredButton(parent, $"{category} ({GetInboxCategoryCount(category)})", 15, 190, 36, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        tab.onClick.AddListener(() =>
        {
            inboxActiveCategory = category;
            BuildInboxUi();
        });
    }

    private void CreateInboxItemRow(Transform parent, PrototypeInboxItem item, int index, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"InboxItem_{index}", new Color(0.96f, 0.86f, 0.62f), 680, 62, 0f, y);
        bool isMessage = item.Category == PrototypeInboxCategory.Messages;
        if (isMessage)
        {
            Button rowOpen = row.AddComponent<Button>();
            rowOpen.targetGraphic = row.GetComponent<Image>();
            rowOpen.onClick.AddListener(() => OpenInboxMessage(index));
        }

        CreateAnchoredText(row.transform, item.Title, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), isMessage ? 350 : 430, 22, isMessage ? -126f : -88f, 14f);
        CreateAnchoredText(row.transform, BuildInboxItemDetail(item), 11, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), isMessage ? 350 : 430, 30, isMessage ? -126f : -88f, -12f);

        if (isMessage)
        {
            Color badgeColor = item.Title.StartsWith("Message to", System.StringComparison.OrdinalIgnoreCase)
                ? new Color(0.35f, 0.12f, 0.62f)
                : new Color(0.12f, 0.55f, 0.08f);
            CreateAnchoredText(row.transform, GetInboxMessageDirectionLabel(item), 10, FontStyle.Bold, badgeColor, 72, 18, 116f, 10f);
            CreateAnchoredText(row.transform, item.IsUnread ? "NEW" : "", 10, FontStyle.Bold, new Color(0.12f, 0.55f, 0.08f), 72, 18, 116f, -12f);

            Button read = CreateAnchoredButton(row.transform, item.IsUnread ? "Read" : "Open", 12, 74, 30, new Color(0.12f, 0.55f, 0.08f), 220f, 0f);
            read.onClick.AddListener(() => OpenInboxMessage(index));
            Button reply = CreateAnchoredButton(row.transform, "Reply", 12, 78, 30, new Color(0.35f, 0.12f, 0.62f), 304f, 0f);
            reply.onClick.AddListener(() => ReplyToInboxMessage(item));
            return;
        }

        CreateAnchoredText(row.transform, item.IsUnread ? "NEW" : "", 12, FontStyle.Bold, new Color(0.12f, 0.55f, 0.08f), 54, 22, 168f, 12f);
        Button collect = CreateAnchoredButton(row.transform, "Claim", 14, 112, 32, new Color(0.12f, 0.55f, 0.08f), 272f, 0f);
        collect.onClick.AddListener(() => ClaimInboxItem(index));
    }

    private void OpenInboxMessage(int index)
    {
        if (!coven.TryMarkPlayerInboxItemRead(index, out PrototypeInboxItem message))
        {
            return;
        }

        BuildInboxMessageDetailUi(message);
    }

    private void BuildInboxMessageDetailUi(PrototypeInboxItem message)
    {
        RemoveInboxMessageDetailModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "InboxMessageDetailModal", new Color(0.055f, 0.028f, 0.085f, 0.995f), 680, 400, 0f, -20f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        CreateAnchoredText(panel.transform, GetInboxMessageDirectionLabel(message), 16, FontStyle.Bold, muted, 560, 24, 0f, 144f);
        CreateAnchoredText(panel.transform, message.Title.ToUpperInvariant(), 30, FontStyle.Bold, gold, 560, 42, 0f, 108f);
        CreateAnchoredPanel(panel.transform, "InboxMessageBody", new Color(0.96f, 0.86f, 0.62f), 560, 148, 0f, 18f);
        Text body = CreateAnchoredText(panel.transform, string.IsNullOrWhiteSpace(message.Detail) ? "(No message text.)" : message.Detail, 18, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 520, 118, 0f, 18f);
        body.alignment = TextAnchor.MiddleLeft;
        CreateAnchoredText(panel.transform, "Messages live in Inbox until cleared.", 14, FontStyle.Bold, muted, 560, 24, 0f, -78f);

        Button back = CreateAnchoredButton(panel.transform, "Back", 18, 150, 42, new Color(0.18f, 0.16f, 0.22f), -178f, -142f);
        back.onClick.AddListener(() =>
        {
            RemoveInboxMessageDetailModal();
            BuildInboxUi();
        });

        if (TryGetFriendNameFromInboxMessage(message, out string friendName) && !prototypeBlockedFriends.Contains(friendName))
        {
            Button reply = CreateAnchoredButton(panel.transform, "Reply", 18, 150, 42, violet, 0f, -142f);
            reply.onClick.AddListener(() =>
            {
                RemoveInboxMessageDetailModal();
                BuildFriendMessageComposerUi(friendName);
            });
        }

        Button clear = CreateAnchoredButton(panel.transform, "Close", 18, 150, 42, violet, 178f, -142f);
        clear.onClick.AddListener(RemoveInboxMessageDetailModal);
    }

    private string GetInboxMessageDirectionLabel(PrototypeInboxItem item)
    {
        if (item.Title.StartsWith("Message to", System.StringComparison.OrdinalIgnoreCase))
        {
            return "SENT";
        }

        if (item.Title.StartsWith("Message from", System.StringComparison.OrdinalIgnoreCase))
        {
            return "RECEIVED";
        }

        return "MESSAGE";
    }

    private void ReplyToInboxMessage(PrototypeInboxItem item)
    {
        if (!TryGetFriendNameFromInboxMessage(item, out string friendName))
        {
            lastInboxClaimSummary = "Reply is only wired for friend messages in this prototype.";
            BuildInboxUi();
            return;
        }

        if (prototypeBlockedFriends.Contains(friendName))
        {
            lastInboxClaimSummary = $"{friendName} is blocked. Unblock before messaging.";
            BuildInboxUi();
            return;
        }

        BuildFriendMessageComposerUi(friendName);
    }

    private bool TryGetFriendNameFromInboxMessage(PrototypeInboxItem item, out string friendName)
    {
        friendName = "";
        const string fromPrefix = "Message from ";
        const string toPrefix = "Message to ";

        if (item.Title.StartsWith(fromPrefix, System.StringComparison.OrdinalIgnoreCase))
        {
            friendName = item.Title.Substring(fromPrefix.Length).Trim();
        }
        else if (item.Title.StartsWith(toPrefix, System.StringComparison.OrdinalIgnoreCase))
        {
            friendName = item.Title.Substring(toPrefix.Length).Trim();
        }

        return !string.IsNullOrWhiteSpace(friendName);
    }

    private void ClaimInboxItem(int index)
    {
        if (inboxActiveCategory == PrototypeInboxCategory.Messages)
        {
            if (!coven.TryMarkPlayerInboxItemRead(index, out PrototypeInboxItem message))
            {
                return;
            }

            lastInboxClaimSummary = $"Read: {message.Title}";
            coven.AddPrototypeChatLine($"You read inbox message: {message.Title}");
            BuildInboxUi();
            return;
        }

        if (!coven.TryTakePlayerInboxItem(index, out PrototypeInboxItem item))
        {
            return;
        }

        string summary = ApplyInboxItem(item);
        lastInboxClaimSummary = $"Claimed: {summary}";

        coven.AddPrototypeChatLine($"You claimed inbox reward: {item.Title}");
        BuildInboxUi();
    }

    private void CollectAllVisibleInboxItems()
    {
        int collected = 0;
        List<string> summaries = new List<string>();
        for (int index = coven.PlayerInboxItems.Count - 1; index >= 0; index--)
        {
            PrototypeInboxItem item = coven.PlayerInboxItems[index];
            if (item.Category != inboxActiveCategory)
            {
                continue;
            }

            if (!coven.TryTakePlayerInboxItem(index, out PrototypeInboxItem claimed))
            {
                continue;
            }

            summaries.Add(ApplyInboxItem(claimed));
            collected++;
        }

        lastInboxClaimSummary = collected > 0 ? $"{(inboxActiveCategory == PrototypeInboxCategory.Messages ? "Cleared" : "Claimed")} {collected}: {BuildCompactInboxClaimSummary(summaries)}" : "No rewards claimed.";
        coven.AddPrototypeChatLine(inboxActiveCategory == PrototypeInboxCategory.Messages ? $"Clear All read {collected} inbox messages." : $"Claim All collected {collected} inbox rewards.");
        BuildInboxUi();
    }

    private string ApplyInboxItem(PrototypeInboxItem item)
    {
        if (item.TryGetCovenGift(out CovenInboxGiftInfo gift))
        {
            ApplyInboxGift(gift);
            return BuildInboxGiftClaimSummary(gift);
        }

        if (item.Category == PrototypeInboxCategory.Messages && string.IsNullOrWhiteSpace(BuildRewardGrantSummary(item.Reward)))
        {
            return item.Title;
        }

        inventory.ApplyRewardGrant(item.Reward);
        RefreshPowerUpDisplays();
        string summary = BuildRewardGrantSummary(item.Reward);
        return string.IsNullOrWhiteSpace(summary) ? item.Title : summary;
    }

    private void ApplyInboxGift(CovenInboxGiftInfo gift)
    {
        if (gift.ItemType == "Ingredient")
        {
            inventory.AddIngredientForPrototype(gift.ItemName, gift.Quantity);
        }
        else if (gift.ItemType == "Regular Card")
        {
            inventory.AddInventoryReward("Regular Card", gift.Quantity);
        }
        else if (gift.ItemType == "Grimoire Card")
        {
            inventory.AddInventoryReward(CardAlbumCatalog.SpecificGrimoireCardRewardPrefix + gift.ItemName, gift.Quantity);
        }
        else
        {
            inventory.AddInventoryReward(gift.ItemName, gift.Quantity);
        }
        RefreshPowerUpDisplays();
    }

    private string BuildInboxGiftClaimSummary(CovenInboxGiftInfo gift)
    {
        if (gift.ItemType == "Regular Card")
        {
            return $"Regular Card x{gift.Quantity}";
        }

        if (gift.ItemType == "Grimoire Card")
        {
            return $"{CardAlbumCatalog.GetGrimoireCardDisplayName(gift.ItemName)} x{gift.Quantity}";
        }

        return $"{gift.ItemName} x{gift.Quantity}";
    }

    private string BuildCompactInboxClaimSummary(IReadOnlyList<string> summaries)
    {
        if (summaries.Count == 0)
        {
            return "";
        }

        int visibleCount = Mathf.Min(summaries.Count, 3);
        List<string> visible = new List<string>();
        for (int index = 0; index < visibleCount; index++)
        {
            visible.Add(summaries[index]);
        }

        if (summaries.Count > visibleCount)
        {
            visible.Add($"+{summaries.Count - visibleCount} more");
        }

        return string.Join(" + ", visible);
    }

    private int GetInboxCategoryCount(PrototypeInboxCategory category)
    {
        IReadOnlyList<PrototypeInboxItem> items = coven.PlayerInboxItems;
        int count = 0;
        for (int index = 0; index < items.Count; index++)
        {
            if (items[index].Category == category)
            {
                count++;
            }
        }

        return count;
    }

    private List<int> GetVisibleInboxItemIndexes(IReadOnlyList<PrototypeInboxItem> items, PrototypeInboxCategory category)
    {
        List<int> indexes = new List<int>();
        for (int index = 0; index < items.Count; index++)
        {
            if (items[index].Category == category)
            {
                indexes.Add(index);
            }
        }

        return indexes;
    }

    private string GetInboxHeaderText()
    {
        return $"{coven.PlayerInboxClaimableCount} waiting rewards | {coven.PlayerInboxUnreadCount} unread";
    }

    private string GetEmptyInboxText()
    {
        if (inboxActiveCategory == PrototypeInboxCategory.Messages)
        {
            return "No messages waiting.";
        }

        if (inboxActiveCategory == PrototypeInboxCategory.Cards)
        {
            return "No card rewards waiting.";
        }

        return "No gifts waiting.";
    }

    private string BuildInboxItemDetail(PrototypeInboxItem item)
    {
        List<string> parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(item.SourceLabel))
        {
            parts.Add(item.SourceLabel);
        }

        if (!string.IsNullOrWhiteSpace(item.Detail))
        {
            parts.Add(item.Detail);
        }

        string rewardText = BuildRewardGrantSummary(item.Reward);
        if (!string.IsNullOrWhiteSpace(rewardText))
        {
            parts.Add(rewardText);
        }

        if (item.HasExpiration)
        {
            System.DateTime expiresAt = new System.DateTime(item.ExpiresAtUtcTicks, System.DateTimeKind.Utc);
            System.TimeSpan remaining = expiresAt - System.DateTime.UtcNow;
            parts.Add(remaining.TotalSeconds > 0 ? $"expires in {Mathf.CeilToInt((float)remaining.TotalDays)}d" : "expired");
        }

        return string.Join(" | ", parts);
    }

    private string BuildRewardGrantSummary(PrototypeRewardGrant reward)
    {
        List<string> parts = new List<string>();
        if (reward.Mana > 0)
        {
            parts.Add($"{reward.Mana:N0} Mana");
        }

        if (reward.Crystals > 0)
        {
            parts.Add($"{reward.Crystals:N0} Crystals");
        }

        if (reward.ClairvoyanceMinutes > 0)
        {
            parts.Add($"{reward.ClairvoyanceMinutes}m Clairvoyance");
        }

        if (reward.JackpotSpins > 0)
        {
            parts.Add($"{reward.JackpotSpins} Wheelspin");
        }

        for (int index = 0; index < reward.Items.Count; index++)
        {
            PrototypeRewardItem item = reward.Items[index];
            parts.Add($"{GetRewardItemDisplayName(item.Name)} x{item.Quantity}");
        }

        return string.Join(" + ", parts);
    }

    private string GetRewardItemDisplayName(string itemName)
    {
        if (!string.IsNullOrWhiteSpace(itemName) && itemName.StartsWith(CardAlbumCatalog.SpecificGrimoireCardRewardPrefix))
        {
            string cardId = itemName.Substring(CardAlbumCatalog.SpecificGrimoireCardRewardPrefix.Length);
            return CardAlbumCatalog.GetGrimoireCardDisplayName(cardId);
        }

        return itemName;
    }

    private string BuildSentInboxPreview()
    {
        List<string> lines = new List<string> { "Sent coven gifts queued in teammate inboxes:" };
        IReadOnlyList<CovenInboxGiftInfo> sent = coven.InboxGifts;
        int visibleCount = Mathf.Min(sent.Count, 3);
        for (int index = 0; index < visibleCount; index++)
        {
            CovenInboxGiftInfo gift = sent[index];
            lines.Add($"{gift.RecipientName}: {GetInboxGiftDisplayName(gift)} x{gift.Quantity}");
        }

        if (sent.Count > visibleCount)
        {
            lines.Add($"+{sent.Count - visibleCount} more sent gifts");
        }

        return string.Join("\n", lines);
    }

    private string GetInboxGiftDisplayName(CovenInboxGiftInfo gift)
    {
        if (gift.ItemType == "Grimoire Card")
        {
            return CardAlbumCatalog.GetGrimoireCardDisplayName(gift.ItemName);
        }

        return gift.ItemName;
    }

    private void BuildManaCauldronUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundState.StopRound();
        calledHistory.Clear();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.055f, 0.025f, 0.08f, 0.98f), 0, 0);
        layout.enabled = false;

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.97f, 0.89f, 0.68f);

        CreateAnchoredPanel(contentRoot, "CauldronDimDen", new Color(0.09f, 0.05f, 0.12f), 1500, 820, 0f, -12f);
        CreateAnchoredText(contentRoot, "PLAYER'S DEN", 26, FontStyle.Bold, new Color(0.72f, 0.64f, 0.82f), 400, 40, 0f, 386f);
        CreateLobbyResourceTile(contentRoot, "Mana", profile.Mana.ToString("N0"), "*", 230, 438f, 382f);
        CreateLobbyResourceTile(contentRoot, "Crystals", profile.Crystals.ToString("N0"), "<>", 230, 690f, 382f);

        GameObject modal = CreateAnchoredPanel(contentRoot, "ManaCauldronModal", new Color(0.12f, 0.04f, 0.18f, 0.98f), 1040, 570, 0f, -12f);
        CreateAnchoredText(modal.transform, "MANA CAULDRON", 54, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 940, 70, 0f, 226f);
        CreateAnchoredPanel(modal.transform, "CauldronBowl", new Color(0.22f, 0.08f, 0.28f), 350, 230, -250f, 38f);
        CreateAnchoredText(modal.transform, "*\n*\n*", 54, FontStyle.Bold, gold, 240, 160, -250f, 62f);
        cauldronAmountText = CreateAnchoredText(modal.transform, $"{profile.ManaCauldronAmount} / {profile.ManaCauldronCapacity}", 72, FontStyle.Bold, Color.white, 370, 90, 248f, 72f);
        CreateAnchoredText(modal.transform, "Mana Coins", 38, FontStyle.Bold, gold, 360, 54, 248f, 8f);
        cauldronStatusText = CreateAnchoredText(modal.transform, GetManaCauldronDetailStatusText(), 25, FontStyle.Bold, cream, 360, 96, 248f, -76f);
        CreateAnchoredText(modal.transform, $"Room restorations increase the hourly refill. Current refill: +{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour.", 18, FontStyle.Bold, cream, 780, 42, 0f, -178f);

        cauldronCollectButton = CreateAnchoredButton(modal.transform, profile.ManaCauldronAmount > 0 ? $"Collect {profile.ManaCauldronAmount}" : "Empty", 28, 330, 58, profile.ManaCauldronAmount > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 218f, -154f);
        cauldronCollectButton.interactable = profile.ManaCauldronAmount > 0;
        cauldronCollectButton.onClick.AddListener(CollectManaCauldronFromDen);

        Button back = CreateAnchoredButton(contentRoot, "Back to Den", 20, 220, 50, new Color(0.18f, 0.16f, 0.22f), -620f, -382f);
        back.onClick.AddListener(BuildPlayerDenUi);

        ApplyStageScale(true);
    }

    private void BuildLibraryGrimoireUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        int albumOwnedCards = inventory.GetOwnedGrimoireCardCount();
        int bookOfShadowsOwnedCards = inventory.GetOwnedBookOfShadowsCardCount();

        CreateAnchoredText(panel.transform, "COLLECTION BOOKS", 48, FontStyle.Bold, gold, 1080, 62, 0f, 286f);
        CreateAnchoredText(panel.transform, "Choose a book, then browse potion sets and ingredient cards.", 18, FontStyle.Bold, muted, 1060, 30, 0f, 242f);
        CreateAnchoredPanel(panel.transform, "LibraryTotals", new Color(0.14f, 0.07f, 0.18f), 1000, 76, 0f, 186f);
        CreateLibraryStat(panel.transform, "Grimoire Cards", $"{albumOwnedCards}/{CardAlbumCatalog.TotalCards}", -310f, 186f);
        CreateLibraryStat(panel.transform, "Potions", $"{BuildCompletedGrimoireEntryCount()}/{CardAlbumCatalog.GrimoireOneEntries.Count}", -60f, 186f);
        CreateLibraryStat(panel.transform, "Book of Shadows", inventory.BookOfShadowsPurchased ? $"{bookOfShadowsOwnedCards}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked", 210f, 186f);

        CreateCollectionBookTile(panel.transform, "GRIMOIRE", "Free Album", $"{albumOwnedCards}/{CardAlbumCatalog.TotalCards} cards collected\n{inventory.GetAlbumSeasonSummaryText()}\nFull reward: {CardAlbumCatalog.BuildRewardLine(CardAlbumCatalog.GrimoireOneCompletionReward)}", true, -300f, -38f, BuildGrimoireIndexUi);
        CreateCollectionBookTile(panel.transform, "BOOK OF SHADOWS", inventory.BookOfShadowsPurchased ? "Purchased" : "Premium Album", inventory.BookOfShadowsPurchased ? $"{bookOfShadowsOwnedCards}/{CardAlbumCatalog.BookOfShadowsTotalCards} cards collected\n{inventory.GetBookOfShadowsSeasonSummaryText()}\nFull reward: {CardAlbumCatalog.BuildRewardLine(CardAlbumCatalog.BookOfShadowsCompletionReward)}" : "Purchase/access rules can be finalized later.\nPrototype unlocks the active monthly set.\nRegular cards only.", inventory.BookOfShadowsPurchased, 300f, -38f, () =>
        {
            if (!inventory.BookOfShadowsPurchased)
            {
                inventory.UnlockBookOfShadowsForPrototype();
            }

            BuildBookOfShadowsLibraryUi();
        });

        Button gifts = CreateAnchoredButton(panel.transform, "Card Gifts", 18, 220, 44, new Color(0.12f, 0.55f, 0.08f), 0f, -250f);
        gifts.onClick.AddListener(BuildLibraryCardGiftingUi);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -306f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildLibraryCardGiftingUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.065f, 0.025f, 0.095f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color cream = new Color(0.96f, 0.86f, 0.62f);

        CreateAnchoredText(panel.transform, "CARD GIFTS", 46, FontStyle.Bold, gold, 960, 58, 0f, 284f);
        CreateAnchoredText(panel.transform, "Library handles card giving. Only extra Regular Grimoire copies can be sent; received cards are claimed from Inbox.", 16, FontStyle.Bold, muted, 1040, 28, 0f, 244f);
        CreateLibraryStat(panel.transform, "Giftable Duplicates", CountGiftableRegularDuplicates().ToString(), -280f, 194f);
        CreateLibraryStat(panel.transform, "Aura Cap Draft", GetPrototypeCardGiftLimitText(), 0f, 194f);
        CreateLibraryStat(panel.transform, "Inbox Card Gifts", GetInboxCategoryCount(PrototypeInboxCategory.Cards).ToString(), 280f, 194f);

        GameObject duplicatesPanel = CreateAnchoredPanel(panel.transform, "GiftableDuplicates", new Color(0.12f, 0.06f, 0.18f), 520, 380, -282f, -26f);
        CreateAnchoredText(duplicatesPanel.transform, "Give Extra Regular Cards", 23, FontStyle.Bold, gold, 470, 34, 0f, 154f);
        CreateAnchoredText(duplicatesPanel.transform, "Your first copy stays protected.", 13, FontStyle.Bold, muted, 430, 22, 0f, 126f);
        CreateGiftableRegularDuplicateRows(duplicatesPanel.transform);

        GameObject missingPanel = CreateAnchoredPanel(panel.transform, "MissingCards", new Color(0.11f, 0.075f, 0.13f), 520, 380, 282f, -26f);
        CreateAnchoredText(missingPanel.transform, "Receive Missing Cards", 23, FontStyle.Bold, gold, 470, 34, 0f, 154f);
        CreateAnchoredText(missingPanel.transform, "Prototype receive test only.", 13, FontStyle.Bold, muted, 430, 22, 0f, 126f);
        CreateMissingRegularCardRows(missingPanel.transform);
        CreateAnchoredText(missingPanel.transform, "Real request/trade actions come after Aura rank and Friends rules are locked.", 14, FontStyle.Bold, muted, 440, 42, 0f, -156f);

        if (!string.IsNullOrWhiteSpace(lastLibraryCardGiftSummary))
        {
            CreateAnchoredText(panel.transform, lastLibraryCardGiftSummary, 16, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 900, 30, 0f, -238f);
        }
        else
        {
            CreateAnchoredText(panel.transform, "Only extra copies can be gifted. Your last copy stays protected.", 16, FontStyle.Bold, cream, 900, 30, 0f, -238f);
        }

        Button sample = CreateAnchoredButton(panel.transform, CanReceivePrototypeCardGift() ? "Receive Any Test Card" : "Receive Locked", 15, 250, 42, CanReceivePrototypeCardGift() ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.34f, 0.32f, 0.38f), -280f, -302f);
        sample.interactable = CanReceivePrototypeCardGift();
        sample.onClick.AddListener(QueueIncomingPrototypeCardGift);
        Button back = CreateAnchoredButton(panel.transform, "Back to Library", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -302f);
        back.onClick.AddListener(BuildLibraryGrimoireUi);
        int waitingCardGifts = GetInboxCategoryCount(PrototypeInboxCategory.Cards);
        Button inbox = CreateAnchoredButton(panel.transform, waitingCardGifts > 0 ? $"Open Inbox Cards ({waitingCardGifts})" : "Open Inbox Cards", 17, 230, 46, waitingCardGifts > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.18f, 0.16f, 0.22f), 280f, -302f);
        inbox.onClick.AddListener(() =>
        {
            inboxActiveCategory = PrototypeInboxCategory.Cards;
            BuildInboxUi();
        });
    }

    private void CreateGiftableRegularDuplicateRows(Transform parent)
    {
        int visible = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count && visible < 5; index++)
        {
            AlbumCardDefinition card = cards[index];
            int copies = inventory.GetGrimoireCardCopies(card.Id);
            if (card.Tier != AlbumCardTier.Regular || copies <= 1)
            {
                continue;
            }

            CreateLibraryCardGiftRow(parent, card, copies, 104f - visible * 58f);
            visible++;
        }

        if (visible == 0)
        {
            CreateAnchoredText(parent, "No extra Regular duplicates yet.\nYou need 2+ copies before one can be sent.", 17, FontStyle.Bold, Color.white, 430, 74, 0f, 46f);
            Button seed = CreateAnchoredButton(parent, "Add Prototype Duplicate", 13, 210, 34, new Color(0.35f, 0.12f, 0.62f), 0f, -36f);
            seed.onClick.AddListener(AddPrototypeGiftableRegularDuplicate);
        }
    }

    private void CreateLibraryCardGiftRow(Transform parent, AlbumCardDefinition card, int copies, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"GiftDuplicate_{card.Id}", new Color(0.96f, 0.86f, 0.62f), 460, 46, 0f, y);
        CreateAnchoredText(row.transform, card.CardName, 14, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 230, 22, -86f, 8f);
        CreateAnchoredText(row.transform, $"{card.PotionName} | owned {copies}", 9, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 230, 16, -86f, -12f);
        CreateAnchoredText(row.transform, BuildStarText(card.Stars), 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 54, 22, 74f, 0f);
        Button gift = CreateAnchoredButton(row.transform, CanSendPrototypeCardGift() ? "Send" : "Locked", 13, 86, 30, CanSendPrototypeCardGift() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 176f, 0f);
        gift.interactable = CanSendPrototypeCardGift();
        gift.onClick.AddListener(() => SendPrototypeLibraryCardGift(card, "Mira"));
    }

    private void CreateMissingRegularCardRows(Transform parent)
    {
        int visible = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count && visible < 5; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier != AlbumCardTier.Regular || inventory.GetGrimoireCardCopies(card.Id) > 0)
            {
                continue;
            }

            CreateMissingRegularCardRow(parent, card, 104f - visible * 58f);
            visible++;
        }

        if (visible == 0)
        {
            CreateAnchoredText(parent, "No missing Regular cards in the visible prototype list.", 18, FontStyle.Bold, Color.white, 430, 70, 0f, 24f);
        }
    }

    private void CreateMissingRegularCardRow(Transform parent, AlbumCardDefinition card, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"MissingRegular_{card.Id}", new Color(0.2f, 0.17f, 0.2f), 460, 46, 0f, y);
        CreateAnchoredText(row.transform, card.CardName, 14, FontStyle.Bold, Color.white, 260, 22, -78f, 8f);
        CreateAnchoredText(row.transform, $"{card.PotionName} | {BuildStarText(card.Stars)}", 9, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 260, 16, -78f, -12f);
        Button request = CreateAnchoredButton(row.transform, CanReceivePrototypeCardGift() ? "Receive" : "Locked", 12, 86, 30, CanReceivePrototypeCardGift() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 176f, 0f);
        request.interactable = CanReceivePrototypeCardGift();
        request.onClick.AddListener(() => QueueIncomingPrototypeCardGift(card));
    }

    private void AddPrototypeGiftableRegularDuplicate()
    {
        AlbumCardDefinition card = FindPrototypeGiftableDuplicateSeedTarget();
        if (card == null)
        {
            lastLibraryCardGiftSummary = "No regular Grimoire card found for a test duplicate.";
            BuildLibraryCardGiftingUi();
            return;
        }

        int copies = inventory.GetGrimoireCardCopies(card.Id);
        inventory.AddSpecificGrimoireCardForPrototype(card.Id);
        if (copies == 0)
        {
            inventory.AddSpecificGrimoireCardForPrototype(card.Id);
        }

        lastLibraryCardGiftSummary = $"Added test duplicate: {card.CardName}. You can now gift the extra copy.";
        BuildLibraryCardGiftingUi();
    }

    private void SendPrototypeLibraryCardGift(AlbumCardDefinition card, string recipientName)
    {
        if (!CanSendPrototypeCardGift())
        {
            lastLibraryCardGiftSummary = $"Temporary Aura-cap draft reached: {prototypeCardGiftsSentToday}/{GetPrototypeCardGiftDailyLimit()} sent today.";
            BuildLibraryCardGiftingUi();
            return;
        }

        if (!inventory.TryGiftGrimoireRegularDuplicate(card.Id))
        {
            lastLibraryCardGiftSummary = $"No extra {card.CardName} copy available.";
            BuildLibraryCardGiftingUi();
            return;
        }

        coven.RecordLibraryCardGift(recipientName, card.Id);
        prototypeCardGiftsSentToday++;
        lastLibraryCardGiftSummary = $"Sent {card.CardName} to {recipientName}. Sent today: {prototypeCardGiftsSentToday}/{GetPrototypeCardGiftDailyLimit()}.";
        BuildLibraryCardGiftingUi();
    }

    private void QueueIncomingPrototypeCardGift()
    {
        if (!CanReceivePrototypeCardGift())
        {
            lastLibraryCardGiftSummary = $"Temporary Aura-cap draft reached: {prototypeCardGiftsReceivedToday}/{GetPrototypeCardGiftDailyLimit()} received today.";
            BuildLibraryCardGiftingUi();
            return;
        }

        AlbumCardDefinition card = FindPrototypeIncomingCardGiftTarget();
        if (card == null)
        {
            lastLibraryCardGiftSummary = "No regular Grimoire card found for a test gift.";
            BuildLibraryCardGiftingUi();
            return;
        }

        coven.EnqueueLibraryCardGiftForPlayer("Luna", card.Id);
        prototypeCardGiftsReceivedToday++;
        lastLibraryCardGiftSummary = $"Incoming test gift queued: {card.CardName}. Received today: {prototypeCardGiftsReceivedToday}/{GetPrototypeCardGiftDailyLimit()}.";
        BuildLibraryCardGiftingUi();
    }

    private void QueueIncomingPrototypeCardGift(AlbumCardDefinition card)
    {
        if (!CanReceivePrototypeCardGift())
        {
            lastLibraryCardGiftSummary = $"Temporary Aura-cap draft reached: {prototypeCardGiftsReceivedToday}/{GetPrototypeCardGiftDailyLimit()} received today.";
            BuildLibraryCardGiftingUi();
            return;
        }

        if (card == null)
        {
            lastLibraryCardGiftSummary = "No regular Grimoire card found for a test gift.";
            BuildLibraryCardGiftingUi();
            return;
        }

        coven.EnqueueLibraryCardGiftForPlayer("Luna", card.Id);
        prototypeCardGiftsReceivedToday++;
        lastLibraryCardGiftSummary = $"Incoming test gift queued: {card.CardName}. Received today: {prototypeCardGiftsReceivedToday}/{GetPrototypeCardGiftDailyLimit()}.";
        BuildLibraryCardGiftingUi();
    }

    private bool CanSendPrototypeCardGift()
    {
        return prototypeCardGiftsSentToday < GetPrototypeCardGiftDailyLimit();
    }

    private bool CanReceivePrototypeCardGift()
    {
        return prototypeCardGiftsReceivedToday < GetPrototypeCardGiftDailyLimit();
    }

    private string GetPrototypeCardGiftLimitText()
    {
        int limit = GetPrototypeCardGiftDailyLimit();
        return $"Send {prototypeCardGiftsSentToday}/{limit} | Get {prototypeCardGiftsReceivedToday}/{limit}";
    }

    private int GetPrototypeCardGiftDailyLimit()
    {
        int level = rewards != null ? rewards.CurrentLevel : 1;
        if (level >= 1000) return 5;
        if (level >= 950) return 4;
        if (level >= 775) return 4;
        if (level >= 625) return 3;
        if (level >= 500) return 3;
        if (level >= 425) return 3;
        if (level >= 350) return 2;
        if (level >= 275) return 2;
        if (level >= 200) return 2;
        if (level >= 140) return 1;
        if (level >= 90) return 1;
        if (level >= 50) return 1;
        return 0;
    }

    private AlbumCardDefinition FindPrototypeGiftableDuplicateSeedTarget()
    {
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier == AlbumCardTier.Regular && inventory.GetGrimoireCardCopies(card.Id) > 0)
            {
                return card;
            }
        }

        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].Tier == AlbumCardTier.Regular)
            {
                return cards[index];
            }
        }

        return null;
    }

    private AlbumCardDefinition FindPrototypeIncomingCardGiftTarget()
    {
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier == AlbumCardTier.Regular && inventory.GetGrimoireCardCopies(card.Id) == 0)
            {
                return card;
            }
        }

        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].Tier == AlbumCardTier.Regular)
            {
                return cards[index];
            }
        }

        return null;
    }

    private int CountGiftableRegularDuplicates()
    {
        int count = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier == AlbumCardTier.Regular)
            {
                count += Mathf.Max(0, inventory.GetGrimoireCardCopies(card.Id) - 1);
            }
        }

        return count;
    }

    private int CountMissingRegularGrimoireCards()
    {
        int count = 0;
        IReadOnlyList<AlbumCardDefinition> cards = CardAlbumCatalog.AllCards;
        for (int index = 0; index < cards.Count; index++)
        {
            AlbumCardDefinition card = cards[index];
            if (card.Tier == AlbumCardTier.Regular && inventory.GetGrimoireCardCopies(card.Id) == 0)
            {
                count++;
            }
        }

        return count;
    }

    private void BuildGrimoireIndexUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.08f, 0.035f, 0.12f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        IReadOnlyList<GrimoireEntryDefinition> entries = CardAlbumCatalog.GrimoireOneEntries;
        selectedGrimoireIndexPage = Mathf.Clamp(selectedGrimoireIndexPage, 0, Mathf.Max(0, (entries.Count - 1) / 8));
        int start = selectedGrimoireIndexPage * 8;
        int end = Mathf.Min(start + 8, entries.Count);

        CreateAnchoredText(panel.transform, "GRIMOIRE", 48, FontStyle.Bold, gold, 940, 62, -30f, 286f);
        CreateAnchoredText(panel.transform, $"Index {selectedGrimoireIndexPage + 1}/{Mathf.Max(1, (entries.Count + 7) / 8)}  |  Sets {start + 1}-{end}  |  {inventory.GetAlbumSeasonSummaryText()}", 18, FontStyle.Bold, muted, 980, 30, 0f, 242f);
        CreateAnchoredText(panel.transform, $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} cards collected", 23, FontStyle.Bold, Color.white, 360, 34, -362f, 194f);
        CreateAnchoredText(panel.transform, $"Complete Grimoire Reward: {CardAlbumCatalog.BuildRewardLine(CardAlbumCatalog.GrimoireOneCompletionReward)}", 17, FontStyle.Bold, muted, 740, 34, 216f, 194f);
        Button fullClaim = CreateAnchoredButton(panel.transform, GetGrimoireCompletionClaimButtonText(), 15, 190, 38, GetAlbumClaimButtonColor(inventory.CanClaimGrimoireCompletionReward(), inventory.IsGrimoireCompletionRewardClaimed()), 454f, 238f);
        fullClaim.interactable = inventory.CanClaimGrimoireCompletionReward();
        fullClaim.onClick.AddListener(ClaimGrimoireCompletionReward);

        for (int index = start; index < end; index++)
        {
            CreateGrimoireIndexPotionTile(panel.transform, entries[index], index - start);
        }

        Button previous = CreateAnchoredButton(panel.transform, "<", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        previous.interactable = selectedGrimoireIndexPage > 0;
        previous.onClick.AddListener(() =>
        {
            selectedGrimoireIndexPage--;
            BuildGrimoireIndexUi();
        });

        Button next = CreateAnchoredButton(panel.transform, ">", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);
        next.interactable = end < entries.Count;
        next.onClick.AddListener(() =>
        {
            selectedGrimoireIndexPage++;
            BuildGrimoireIndexUi();
        });

        Button back = CreateAnchoredButton(panel.transform, "Back to Books", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, -306f);
        back.onClick.AddListener(BuildLibraryGrimoireUi);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, -306f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildGrimoireDetailUi()
    {
        RemoveLibraryGrimoireModal();
        IReadOnlyList<GrimoireEntryDefinition> entries = CardAlbumCatalog.GrimoireOneEntries;
        selectedGrimoireEntryIndex = Mathf.Clamp(selectedGrimoireEntryIndex, 0, entries.Count - 1);
        GrimoireEntryDefinition entry = entries[selectedGrimoireEntryIndex];
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.08f, 0.035f, 0.12f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int owned = inventory.GetOwnedGrimoireEntryCardCount(entry);

        CreateAnchoredText(panel.transform, "GRIMOIRE", 42, FontStyle.Bold, gold, 1000, 54, 0f, 286f);
        CreateAnchoredText(panel.transform, $"Potion {entry.EntryNumber}/{entries.Count}  |  {inventory.GetAlbumSeasonSummaryText()}", 17, FontStyle.Bold, muted, 980, 28, 0f, 248f);

        GameObject leftPage = CreateAnchoredPanel(panel.transform, "GrimoireLeftPage", new Color(0.94f, 0.82f, 0.62f), 500, 430, -260f, 12f);
        CreateAnchoredText(leftPage.transform, entry.PotionName, 34, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 440, 80, 0f, 142f);
        CreateAnchoredText(leftPage.transform, "Potion Set", 18, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 280, 26, 0f, 82f);
        CreateAnchoredText(leftPage.transform, "*\nPOTION\n*", 52, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 260, 150, 0f, -12f);
        CreateAnchoredText(leftPage.transform, $"{owned}/{entry.Cards.Count} Ingredients Found", 26, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 420, 42, 0f, -114f);
        CreateProgressBar(leftPage.transform, owned, entry.Cards.Count, 300, 24, 0f, -158f);

        GameObject rightPage = CreateAnchoredPanel(panel.transform, "GrimoireRightPage", new Color(0.94f, 0.82f, 0.62f), 500, 430, 260f, 12f);
        CreateAnchoredText(rightPage.transform, "Complete this potion to earn", 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 430, 30, 0f, 154f);
        CreateRewardBadgeRow(rightPage.transform, entry.Reward, 0f, 82f);
        Button claim = CreateAnchoredButton(rightPage.transform, GetGrimoireEntryClaimButtonText(entry), 17, 220, 40, GetAlbumClaimButtonColor(inventory.CanClaimGrimoireEntryReward(entry), inventory.IsGrimoireEntryRewardClaimed(entry)), 0f, 26f);
        claim.interactable = inventory.CanClaimGrimoireEntryReward(entry);
        claim.onClick.AddListener(() => ClaimGrimoireEntryReward(entry));
        CreateAnchoredText(rightPage.transform, "Ingredient Preview", 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 430, 30, 0f, -20f);
        CreateMiniCardPreviewGrid(rightPage.transform, entry, 0f, -102f);

        Button view = CreateAnchoredButton(rightPage.transform, "View Ingredients", 22, 300, 54, new Color(0.12f, 0.55f, 0.08f), 0f, -172f);
        view.onClick.AddListener(() => BuildGrimoireIngredientOverlay(entry));

        Button previous = CreateAnchoredButton(panel.transform, "<", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        previous.interactable = selectedGrimoireEntryIndex > 0;
        previous.onClick.AddListener(() => SelectGrimoireEntry(selectedGrimoireEntryIndex - 1));
        Button next = CreateAnchoredButton(panel.transform, ">", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);
        next.interactable = selectedGrimoireEntryIndex < entries.Count - 1;
        next.onClick.AddListener(() => SelectGrimoireEntry(selectedGrimoireEntryIndex + 1));

        Button back = CreateAnchoredButton(panel.transform, "Back to Index", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, -306f);
        back.onClick.AddListener(BuildGrimoireIndexUi);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, -306f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildBookOfShadowsLibraryUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.08f, 0.035f, 0.12f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        IReadOnlyList<BookOfShadowsSetDefinition> shadowSets = CardAlbumCatalog.BookOfShadowsSets;
        int activeShadowSetIndex = Mathf.Clamp(inventory.GetActiveBookOfShadowsSetNumber() - 1, 0, shadowSets.Count - 1);
        BookOfShadowsSetDefinition activeShadowSet = shadowSets[activeShadowSetIndex];

        CreateAnchoredText(panel.transform, "BOOK OF SHADOWS", 42, FontStyle.Bold, gold, 980, 58, 0f, 264f);
        CreateAnchoredText(panel.transform, inventory.GetBookOfShadowsSeasonSummaryText(), 17, FontStyle.Bold, muted, 980, 28, 0f, 224f);
        CreateLibraryStat(panel.transform, "Active Set", $"{inventory.GetOwnedBookOfShadowsSetCardCount(activeShadowSet)}/{CardAlbumCatalog.CountBookOfShadowsInSet(activeShadowSet)}", -280f, 166f);
        CreateLibraryStat(panel.transform, "All Sets", $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}", 0f, 166f);
        CreateLibraryStat(panel.transform, "Collections", $"{BuildCompletedBookOfShadowsEntryCount(activeShadowSet)}/{activeShadowSet.Entries.Count}", 280f, 166f);
        Button fullClaim = CreateAnchoredButton(panel.transform, GetBookOfShadowsCompletionClaimButtonText(), 15, 210, 38, GetAlbumClaimButtonColor(inventory.CanClaimBookOfShadowsCompletionReward(), inventory.IsBookOfShadowsCompletionRewardClaimed()), 430f, 166f);
        fullClaim.interactable = inventory.CanClaimBookOfShadowsCompletionReward();
        fullClaim.onClick.AddListener(ClaimBookOfShadowsCompletionReward);

        for (int index = 0; index < activeShadowSet.Entries.Count; index++)
        {
            CreateBookOfShadowsIndexTile(panel.transform, activeShadowSet.Entries[index], index);
        }

        CreateAnchoredText(panel.transform, "Book of Shadows cards are regular-only. Each monthly set closes when its 30-day window ends.", 15, FontStyle.Bold, muted, 860, 36, 0f, -220f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Library", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, -284f);
        back.onClick.AddListener(BuildLibraryGrimoireUi);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, -284f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildBookOfShadowsDetailUi()
    {
        RemoveLibraryGrimoireModal();
        IReadOnlyList<BookOfShadowsSetDefinition> shadowSets = CardAlbumCatalog.BookOfShadowsSets;
        int activeShadowSetIndex = Mathf.Clamp(inventory.GetActiveBookOfShadowsSetNumber() - 1, 0, shadowSets.Count - 1);
        BookOfShadowsSetDefinition activeShadowSet = shadowSets[activeShadowSetIndex];
        selectedBookOfShadowsEntryIndex = Mathf.Clamp(selectedBookOfShadowsEntryIndex, 0, activeShadowSet.Entries.Count - 1);
        BookOfShadowsEntryDefinition shadowEntry = activeShadowSet.Entries[selectedBookOfShadowsEntryIndex];
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.08f, 0.035f, 0.12f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int owned = inventory.GetOwnedBookOfShadowsEntryCardCount(shadowEntry);

        CreateAnchoredText(panel.transform, "BOOK OF SHADOWS", 42, FontStyle.Bold, gold, 980, 58, 0f, 264f);
        CreateAnchoredText(panel.transform, $"Set {activeShadowSet.SetNumber}/3  |  Collection {shadowEntry.EntryNumber}/{activeShadowSet.Entries.Count}  |  {inventory.GetBookOfShadowsSeasonSummaryText()}", 17, FontStyle.Bold, muted, 980, 28, 0f, 224f);

        GameObject page = CreateAnchoredPanel(panel.transform, "ShadowsDetailPage", new Color(0.94f, 0.82f, 0.62f), 880, 390, 0f, 14f);
        CreateAnchoredText(page.transform, shadowEntry.PotionName, 34, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 760, 54, 0f, 136f);
        CreateAnchoredText(page.transform, $"{owned}/{shadowEntry.Cards.Count} cards collected", 25, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 360, 42, -230f, 78f);
        CreateProgressBar(page.transform, owned, shadowEntry.Cards.Count, 300, 24, -230f, 40f);
        CreateAnchoredText(page.transform, $"Reward: {CardAlbumCatalog.BuildRewardLine(shadowEntry.Reward)}", 18, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 400, 70, 220f, 66f);
        Button claim = CreateAnchoredButton(page.transform, GetBookOfShadowsEntryClaimButtonText(shadowEntry), 17, 220, 40, GetAlbumClaimButtonColor(inventory.CanClaimBookOfShadowsEntryReward(shadowEntry), inventory.IsBookOfShadowsEntryRewardClaimed(shadowEntry)), 220f, 12f);
        claim.interactable = inventory.CanClaimBookOfShadowsEntryReward(shadowEntry);
        claim.onClick.AddListener(() => ClaimBookOfShadowsEntryReward(shadowEntry));
        CreateBookOfShadowsCardSlotGrid(page.transform, shadowEntry, 0f, -18f);

        Button ingredients = CreateAnchoredButton(page.transform, "View Ingredients", 22, 300, 54, new Color(0.12f, 0.55f, 0.08f), 0f, -146f);
        ingredients.onClick.AddListener(() => BuildBookOfShadowsIngredientOverlay(shadowEntry));

        Button previousShadow = CreateAnchoredButton(panel.transform, "<", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        previousShadow.interactable = selectedBookOfShadowsEntryIndex > 0;
        previousShadow.onClick.AddListener(() => SelectBookOfShadowsEntry(selectedBookOfShadowsEntryIndex - 1));
        Button nextShadow = CreateAnchoredButton(panel.transform, ">", 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);
        nextShadow.interactable = selectedBookOfShadowsEntryIndex < activeShadowSet.Entries.Count - 1;
        nextShadow.onClick.AddListener(() => SelectBookOfShadowsEntry(selectedBookOfShadowsEntryIndex + 1));

        Button back = CreateAnchoredButton(panel.transform, "Back to Index", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, -284f);
        back.onClick.AddListener(BuildBookOfShadowsLibraryUi);
        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, -284f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void BuildCovenCircleUi()
    {
        RemoveBewitchmentBazaarModal();
        RemoveCovenCircleModal();
        RemoveCovenMemberDetailModal();
        RemoveCovenEmporiumModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "CovenCircleModal", new Color(0.055f, 0.035f, 0.12f, 0.985f), 1180, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int clubOrbs = inventory.GetInventoryRewardCount("Club Orbs");

        CreateAnchoredText(panel.transform, "COVEN CIRCLE", 42, FontStyle.Bold, gold, 1040, 58, 0f, 278f);
        CreateAnchoredText(panel.transform, coven.GetHeaderText(), 17, FontStyle.Bold, muted, 1040, 28, 0f, 238f);

        CreateAnchoredPanel(panel.transform, "CovenTotals", new Color(0.12f, 0.06f, 0.2f), 960, 82, 0f, 178f);
        CreateCovenStat(panel.transform, "Held Club Orbs", clubOrbs.ToString(), -360f, 178f);
        CreateCovenStat(panel.transform, "Membership", coven.IsJoined ? "Joined" : "None", -120f, 178f);
        CreateCovenStat(panel.transform, "Weekly Challenge", coven.GetWeeklyChallengeText(clubOrbs), 120f, 178f);
        CreateCovenStat(panel.transform, "Coven Rank", coven.GetStandingsText(), 360f, 178f);

        Button contributeButton = CreateAnchoredButton(panel.transform, GetCovenContributionButtonText(clubOrbs), 15, 220, 40, new Color(0.12f, 0.55f, 0.08f), -116f, 122f);
        contributeButton.interactable = coven.IsJoined && clubOrbs > 0 && coven.GetRemainingWeeklyOrbs() > 0;
        contributeButton.onClick.AddListener(ContributeClubOrbsToCoven);

        Button membershipButton = CreateAnchoredButton(panel.transform, coven.IsJoined ? "Leave Coven" : "Join Prototype Coven", 15, 220, 40, coven.IsJoined ? new Color(0.42f, 0.12f, 0.18f) : new Color(0.12f, 0.55f, 0.08f), 116f, 122f);
        membershipButton.onClick.AddListener(coven.IsJoined ? LeavePrototypeCoven : JoinPrototypeCoven);

        CreateCovenSection(panel.transform, "Team Members", -392f, -18f, 300, 244, new Color(0.09f, 0.06f, 0.16f));
        if (coven.IsJoined)
        {
            IReadOnlyList<CovenMemberInfo> members = coven.Members;
            for (int index = 0; index < members.Count && index < 3; index++)
            {
                CreateCovenMemberRow(panel.transform, members[index], -392f, 46f - index * 60f);
            }

            CreateAnchoredText(panel.transform, "Rows later open profile, stats, and wish-list gifting.", 13, FontStyle.Bold, muted, 250, 36, -392f, -138f);
        }
        else
        {
            CreateAnchoredText(panel.transform, "Join a Coven to see member names, online status, stats, wish lists, and gifting.", 17, FontStyle.Bold, Color.white, 250, 120, -392f, -14f);
        }

        CreateCovenSection(panel.transform, "Team Chat", -72f, -18f, 300, 244, new Color(0.08f, 0.06f, 0.18f));
        CreateAnchoredText(panel.transform, coven.BuildChatPreview(), 16, FontStyle.Bold, Color.white, 256, 104, -72f, 26f);
        CreateAnchoredButton(panel.transform, coven.IsJoined ? "Chat Disabled" : "No Coven Chat", 15, 210, 34, new Color(0.28f, 0.26f, 0.32f), -72f, -106f).interactable = false;

        CreateCovenSection(panel.transform, "Coven Cards", 270f, -18f, 360, 244, new Color(0.1f, 0.07f, 0.14f));
        CreateCovenFeatureLine(panel.transform, "Weekly Challenge Card", coven.GetWeeklyChallengeCardNote(clubOrbs), 270f, 54f);
        CreateCovenFeatureLine(panel.transform, "Coven Standings Card", coven.GetStandingsCardNote(), 270f, -4f);
        CreateCovenFeatureLine(panel.transform, "Gift and Request Card", coven.IsJoined ? "wishlist items and ingredients" : "join to gift/request", 270f, -62f);
        Button emporium = CreateAnchoredButton(panel.transform, "Open Coven Emporium", 15, 286, 42, coven.IsJoined ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.28f, 0.26f, 0.32f), 270f, -120f);
        emporium.interactable = coven.IsJoined;
        emporium.onClick.AddListener(BuildCovenEmporiumUi);

        if (coven.IsJoined)
        {
            CreateCovenSection(panel.transform, "Leadership Fields", 0f, -214f, 960, 132, new Color(0.12f, 0.075f, 0.12f));
            CreateCovenLeadershipField(panel.transform, "Leader Role", "High Priestess / High Priest", -318f, -184f);
            CreateCovenLeadershipField(panel.transform, "Coven Settings", "privacy, entry level, language, notice", 0f, -184f);
            CreateAnchoredText(panel.transform, $"Members {coven.Members.Count}/{coven.MemberCap} | Pending {coven.JoinRequests.Count}", 16, FontStyle.Bold, gold, 280, 26, 318f, -174f);
            if (coven.JoinRequests.Count > 0)
            {
                for (int index = 0; index < coven.JoinRequests.Count && index < 2; index++)
                {
                    CreateCovenJoinRequestRow(panel.transform, coven.JoinRequests[index], -170f + (index * 340f), -244f);
                }
            }
            else
            {
                CreateAnchoredText(panel.transform, "No pending join requests.", 15, FontStyle.Bold, Color.white, 520, 24, 0f, -244f);
            }
        }
        else
        {
            BuildCovenDiscoveryPanel(panel.transform);
        }

        CreateAnchoredText(panel.transform, "Personal album cards stay in Library. Coven uses team cards for challenges, standings, gifts, requests, and emporium systems.", 16, FontStyle.Bold, Color.white, 900, 34, 0f, -284f);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -326f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void JoinPrototypeCoven()
    {
        coven.JoinPrototypeCoven();
        BuildCovenCircleUi();
    }

    private void LeavePrototypeCoven()
    {
        coven.LeaveCoven();
        BuildCovenCircleUi();
    }

    private void AcceptCovenJoinRequest(string applicantName)
    {
        coven.AcceptJoinRequest(applicantName);
        BuildCovenCircleUi();
    }

    private void DenyCovenJoinRequest(string applicantName)
    {
        coven.DenyJoinRequest(applicantName);
        BuildCovenCircleUi();
    }

    private void BuildCovenDiscoveryPanel(Transform parent)
    {
        CreateCovenSection(parent, "Find a Coven", 0f, -214f, 960, 132, new Color(0.12f, 0.075f, 0.12f));
        CreateAnchoredText(parent, "Discovery is a prototype shell. Public/private rules, invite links, search, and approval filters are still TBD.", 13, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 860, 22, 0f, -168f);
        CreatePrototypeCovenDiscoveryRow(parent, "Moonpetal Circle", "Casual daily helpers", "Open prototype", -310f, -220f);
        CreatePrototypeCovenDiscoveryRow(parent, "Starlit Hearth", "Ingredient-focused", "Request sent later", 0f, -220f);
        CreatePrototypeCovenDiscoveryRow(parent, "Azalea Coven", "Realm 1 restorers", "Request sent later", 310f, -220f);
    }

    private void CreatePrototypeCovenDiscoveryRow(Transform parent, string name, string summary, string note, float x, float y)
    {
        bool requested = prototypeRequestedCovenNames.Contains(name);
        GameObject row = CreateAnchoredPanel(parent, $"CovenDiscovery_{name}", new Color(0.96f, 0.86f, 0.62f), 280, 64, x, y);
        CreateAnchoredText(row.transform, name, 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 168, 20, -46f, 16f);
        CreateAnchoredText(row.transform, $"{summary}\n{note}", 9, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 168, 32, -46f, -10f);
        Button action = CreateAnchoredButton(row.transform, name == "Moonpetal Circle" ? "Join" : requested ? "Cancel" : "Request", 10, 72, 28, requested ? new Color(0.42f, 0.12f, 0.18f) : new Color(0.12f, 0.55f, 0.08f), 88f, 0f);
        if (name == "Moonpetal Circle")
        {
            action.onClick.AddListener(JoinPrototypeCovenFromDiscovery);
        }
        else
        {
            action.onClick.AddListener(() => TogglePrototypeCovenJoinRequest(name));
        }
    }

    private void JoinPrototypeCovenFromDiscovery()
    {
        prototypeRequestedCovenNames.Clear();
        SavePrototypeCovenDiscoveryState();
        JoinPrototypeCoven();
    }

    private void TogglePrototypeCovenJoinRequest(string covenName)
    {
        if (prototypeRequestedCovenNames.Contains(covenName))
        {
            prototypeRequestedCovenNames.Remove(covenName);
        }
        else
        {
            prototypeRequestedCovenNames.Add(covenName);
        }

        SavePrototypeCovenDiscoveryState();
        BuildCovenCircleUi();
    }

    private void LoadPrototypeCovenDiscoveryState()
    {
        prototypeRequestedCovenNames.Clear();
        string saved = PlayerPrefs.GetString(PrototypeCovenDiscoverySaveKey, "");
        if (string.IsNullOrWhiteSpace(saved))
        {
            return;
        }

        string[] names = saved.Split(PrototypeFriendSaveSeparator);
        for (int index = 0; index < names.Length; index++)
        {
            string name = names[index].Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                prototypeRequestedCovenNames.Add(name);
            }
        }
    }

    private void SavePrototypeCovenDiscoveryState()
    {
        List<string> names = new List<string>(prototypeRequestedCovenNames);
        names.Sort();
        PlayerPrefs.SetString(PrototypeCovenDiscoverySaveKey, string.Join(PrototypeFriendSaveSeparator.ToString(), names));
        PlayerPrefs.Save();
    }

    private string GetCovenContributionButtonText(int clubOrbs)
    {
        if (!coven.IsJoined)
        {
            return "Join to Contribute";
        }

        if (coven.GetRemainingWeeklyOrbs() <= 0)
        {
            return "Challenge Full";
        }

        if (clubOrbs <= 0)
        {
            return "No Orbs Held";
        }

        return $"Contribute {Mathf.Min(clubOrbs, coven.GetRemainingWeeklyOrbs())} Orbs";
    }

    private void ContributeClubOrbsToCoven()
    {
        int clubOrbs = inventory.GetInventoryRewardCount("Club Orbs");
        int contributed = coven.ContributeClubOrbs(clubOrbs);
        if (contributed > 0)
        {
            inventory.TryConsumeInventoryReward("Club Orbs", contributed);
        }

        BuildCovenCircleUi();
    }

    private string BuildArchivedGrimoireSummary()
    {
        List<string> restoredRooms = new List<string>();
        IReadOnlyList<RealmDefinition> realms = RealmContentCatalog.AllRealms;
        for (int realmIndex = 0; realmIndex < realms.Count; realmIndex++)
        {
            IReadOnlyList<RoomDefinition> rooms = realms[realmIndex].Rooms;
            for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
            {
                RoomDefinition room = rooms[roomIndex];
                if (inventory.IsRoomRestored(room))
                {
                    restoredRooms.Add($"{realms[realmIndex].Name}: {room.Name}");
                }
            }
        }

        if (restoredRooms.Count == 0)
        {
            return "No archived grimoires yet.\nRestore a room potion to add its grimoire record here.";
        }

        int visibleCount = Mathf.Min(restoredRooms.Count, 6);
        List<string> lines = new List<string>();
        for (int index = 0; index < visibleCount; index++)
        {
            lines.Add(restoredRooms[index]);
        }

        if (restoredRooms.Count > visibleCount)
        {
            lines.Add($"+{restoredRooms.Count - visibleCount} more archived records");
        }

        return string.Join("\n", lines);
    }

    private void CreateLibraryStat(Transform parent, string label, string value, float x, float y)
    {
        CreateAnchoredText(parent, value, 25, FontStyle.Bold, Color.white, 210, 30, x, y + 10f);
        CreateAnchoredText(parent, label, 13, FontStyle.Bold, new Color(0.84f, 0.88f, 1f), 210, 18, x, y - 18f);
    }

    private void CreateLibrarySection(Transform parent, string title, float x, float y, float width, float height, Color color)
    {
        GameObject section = CreateAnchoredPanel(parent, $"Library_{title}", color, width, height, x, y);
        CreateAnchoredText(section.transform, title.ToUpperInvariant(), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, 24, 0f, height * 0.5f - 20f);
    }

    private void CreateCollectionBookTile(Transform parent, string title, string badge, string body, bool active, float x, float y, System.Action onClick)
    {
        Color cover = active ? new Color(0.18f, 0.08f, 0.34f) : new Color(0.09f, 0.075f, 0.1f);
        GameObject book = CreateAnchoredPanel(parent, $"CollectionBook_{title}", cover, 440, 330, x, y);
        CreateAnchoredText(book.transform, badge.ToUpperInvariant(), 18, FontStyle.Bold, active ? new Color(0.8f, 1f, 0.55f) : new Color(0.82f, 0.78f, 0.88f), 360, 28, 0f, 126f);
        CreateAnchoredText(book.transform, title, 38, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 380, 82, 0f, 64f);
        CreateAnchoredPanel(book.transform, "BookMark", new Color(0.35f, 0.12f, 0.62f), 78, 190, -150f, -12f);
        CreateAnchoredText(book.transform, body, 16, FontStyle.Bold, Color.white, 340, 108, 34f, -34f);
        Button open = CreateAnchoredButton(book.transform, active ? "Open Book" : "Purchase Book", 19, 220, 48, active ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.12f, 0.55f, 0.08f), 0f, -132f);
        open.onClick.AddListener(() => onClick());
    }

    private void CreateGrimoireIndexPotionTile(Transform parent, GrimoireEntryDefinition entry, int visibleIndex)
    {
        int column = visibleIndex % 4;
        int row = visibleIndex / 4;
        float x = -390f + column * 260f;
        float y = 96f - row * 210f;
        int owned = inventory.GetOwnedGrimoireEntryCardCount(entry);
        bool complete = inventory.IsGrimoireEntryComplete(entry);
        Color color = complete ? new Color(0.12f, 0.42f, 0.16f) : new Color(0.94f, 0.82f, 0.62f);
        Color textColor = complete ? Color.white : new Color(0.18f, 0.08f, 0.32f);
        GameObject tile = CreateAnchoredPanel(parent, $"GrimoireIndexPotion_{entry.EntryNumber}", color, 230, 172, x, y);
        Button button = tile.AddComponent<Button>();
        button.targetGraphic = tile.GetComponent<Image>();
        button.onClick.AddListener(() =>
        {
            selectedGrimoireEntryIndex = entry.EntryNumber - 1;
            BuildGrimoireDetailUi();
        });

        CreateAnchoredText(tile.transform, $"Potion {entry.EntryNumber}", 17, FontStyle.Bold, complete ? new Color(0.8f, 1f, 0.55f) : new Color(0.35f, 0.12f, 0.62f), 210, 24, 0f, 58f);
        CreateAnchoredText(tile.transform, entry.PotionName, 17, FontStyle.Bold, textColor, 198, 52, 0f, 22f);
        CreateAnchoredText(tile.transform, $"{owned}/{entry.Cards.Count}", 24, FontStyle.Bold, textColor, 110, 34, 0f, -28f);
        CreateProgressBar(tile.transform, owned, entry.Cards.Count, 150, 16, 0f, -58f);
        if (inventory.HasUnseenGrimoireEntryCards(entry))
        {
            CreateNewBadge(tile.transform, 72f, 70f, 66, 28);
        }
    }

    private void CreateBookOfShadowsIndexTile(Transform parent, BookOfShadowsEntryDefinition entry, int visibleIndex)
    {
        int column = visibleIndex % 4;
        int row = visibleIndex / 4;
        float x = -390f + column * 260f;
        float y = 76f - row * 210f;
        int owned = inventory.GetOwnedBookOfShadowsEntryCardCount(entry);
        bool complete = inventory.IsBookOfShadowsEntryComplete(entry);
        Color color = complete ? new Color(0.12f, 0.42f, 0.16f) : new Color(0.94f, 0.82f, 0.62f);
        Color textColor = complete ? Color.white : new Color(0.18f, 0.08f, 0.32f);
        GameObject tile = CreateAnchoredPanel(parent, $"BookOfShadowsIndex_{entry.EntryNumber}", color, 230, 172, x, y);
        Button button = tile.AddComponent<Button>();
        button.targetGraphic = tile.GetComponent<Image>();
        button.onClick.AddListener(() =>
        {
            selectedBookOfShadowsEntryIndex = entry.EntryNumber - 1;
            BuildBookOfShadowsDetailUi();
        });

        CreateAnchoredText(tile.transform, $"Collection {entry.EntryNumber}", 17, FontStyle.Bold, complete ? new Color(0.8f, 1f, 0.55f) : new Color(0.35f, 0.12f, 0.62f), 210, 24, 0f, 58f);
        CreateAnchoredText(tile.transform, entry.PotionName, 17, FontStyle.Bold, textColor, 198, 52, 0f, 22f);
        CreateAnchoredText(tile.transform, $"{owned}/{entry.Cards.Count}", 24, FontStyle.Bold, textColor, 110, 34, 0f, -28f);
        CreateProgressBar(tile.transform, owned, entry.Cards.Count, 150, 16, 0f, -58f);
        if (inventory.HasUnseenBookOfShadowsEntryCards(entry))
        {
            CreateNewBadge(tile.transform, 72f, 70f, 66, 28);
        }
    }

    private void CreateProgressBar(Transform parent, int owned, int total, float width, float height, float x, float y)
    {
        CreateAnchoredPanel(parent, "ProgressTrack", new Color(0.45f, 0.36f, 0.25f), width, height, x, y);
        if (total <= 0)
        {
            return;
        }

        float fillWidth = Mathf.Clamp01(owned / (float)total) * width;
        if (fillWidth <= 1f)
        {
            return;
        }

        GameObject fill = CreateAnchoredPanel(parent, "ProgressFill", new Color(0.48f, 0.08f, 0.7f), fillWidth, height, x - width * 0.5f + fillWidth * 0.5f, y);
        fill.transform.SetAsLastSibling();
    }

    private void CreateRewardBadgeRow(Transform parent, AlbumRewardDefinition reward, float x, float y)
    {
        List<string> rewardsList = new List<string>();
        if (reward.Mana > 0) rewardsList.Add($"{reward.Mana:n0}\nMana");
        if (reward.Crystals > 0) rewardsList.Add($"{reward.Crystals:n0}\nCrystals");
        if (reward.PowerUps > 0) rewardsList.Add($"x{reward.PowerUps:n0}\nPower-Ups");
        if (reward.ClairvoyanceHours > 0) rewardsList.Add($"{reward.ClairvoyanceHours}h\nClairvoyance");

        float startX = x - (rewardsList.Count - 1) * 74f;
        for (int index = 0; index < rewardsList.Count; index++)
        {
            GameObject badge = CreateAnchoredPanel(parent, $"RewardBadge_{index}", new Color(0.35f, 0.12f, 0.62f), 128, 82, startX + index * 148f, y);
            CreateAnchoredText(badge.transform, rewardsList[index], 19, FontStyle.Bold, Color.white, 118, 60, 0f, 0f);
        }
    }

    private void CreateMiniCardPreviewGrid(Transform parent, GrimoireEntryDefinition entry, float x, float y)
    {
        const float slotWidth = 54f;
        const float slotHeight = 42f;
        const float gapX = 8f;
        const float gapY = 8f;
        int max = Mathf.Min(10, entry.Cards.Count);
        for (int index = 0; index < max; index++)
        {
            AlbumCardDefinition card = entry.Cards[index];
            int col = index % 5;
            int row = index / 5;
            int copies = inventory.GetGrimoireCardCopies(card.Id);
            bool owned = copies > 0;
            Color slotColor = owned ? GetOwnedAlbumCardColor(card.Tier) : GetMissingCardColor();
            GameObject slot = CreateAnchoredPanel(parent, $"MiniCard_{index}", slotColor, slotWidth, slotHeight, x - 124f + col * (slotWidth + gapX), y + 28f - row * (slotHeight + gapY));
            CreateAnchoredText(slot.transform, owned ? $"{card.Stars}*" : "?", 17, FontStyle.Bold, owned ? Color.white : GetMissingCardTextColor(), slotWidth - 6, 28, 0f, 2f);
            if (copies > 1)
            {
                CreateDuplicateBadge(slot.transform, copies - 1, 18f, 14f, 24, 9);
            }

            if (inventory.IsGrimoireCardUnseen(card.Id))
            {
                CreateNewBadge(slot.transform, 16f, 14f, 42, 18, 9);
            }
        }
    }

    private void BuildGrimoireIngredientOverlay(GrimoireEntryDefinition entry)
    {
        Transform existing = contentRoot.Find("LibraryIngredientOverlay");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        GameObject overlay = CreateAnchoredPanel(contentRoot, "LibraryIngredientOverlay", new Color(0.035f, 0.018f, 0.07f, 0.985f), 1500, 820, 0f, -12f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int owned = inventory.GetOwnedGrimoireEntryCardCount(entry);
        CreateAnchoredText(overlay.transform, entry.PotionName, 48, FontStyle.Bold, gold, 980, 60, -160f, 320f);
        CreateAnchoredText(overlay.transform, $"{owned}/{entry.Cards.Count} ingredients found", 27, FontStyle.Bold, Color.white, 520, 40, -408f, 262f);
        CreateProgressBar(overlay.transform, owned, entry.Cards.Count, 360, 24, -408f, 226f);
        CreateRewardBadgeRow(overlay.transform, entry.Reward, 350f, 262f);
        CreateAnchoredText(overlay.transform, "Tap an ingredient card for details, help requests, or Wild use.", 18, FontStyle.Bold, muted, 900, 30, 0f, -340f);
        CreateGrimoireIngredientCardGrid(overlay.transform, entry, 0f, 10f);
        inventory.MarkGrimoireEntryCardsSeen(entry);

        Button back = CreateAnchoredButton(overlay.transform, "Back", 24, 150, 54, new Color(0.35f, 0.12f, 0.62f), -640f, 338f);
        back.onClick.AddListener(() =>
        {
            Destroy(overlay);
            BuildGrimoireDetailUi();
        });
    }

    private void BuildBookOfShadowsIngredientOverlay(BookOfShadowsEntryDefinition entry)
    {
        Transform existing = contentRoot.Find("LibraryIngredientOverlay");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        GameObject overlay = CreateAnchoredPanel(contentRoot, "LibraryIngredientOverlay", new Color(0.035f, 0.018f, 0.07f, 0.985f), 1500, 820, 0f, -12f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        int owned = inventory.GetOwnedBookOfShadowsEntryCardCount(entry);
        CreateAnchoredText(overlay.transform, entry.PotionName, 48, FontStyle.Bold, gold, 980, 60, -160f, 320f);
        CreateAnchoredText(overlay.transform, $"{owned}/{entry.Cards.Count} ingredients found", 27, FontStyle.Bold, Color.white, 520, 40, -408f, 262f);
        CreateProgressBar(overlay.transform, owned, entry.Cards.Count, 360, 24, -408f, 226f);
        CreateRewardBadgeRow(overlay.transform, entry.Reward, 350f, 262f);
        CreateBookOfShadowsIngredientCardGrid(overlay.transform, entry, 0f, -10f);
        inventory.MarkBookOfShadowsEntryCardsSeen(entry);

        Button back = CreateAnchoredButton(overlay.transform, "Back", 24, 150, 54, new Color(0.35f, 0.12f, 0.62f), -640f, 338f);
        back.onClick.AddListener(() =>
        {
            Destroy(overlay);
            BuildBookOfShadowsDetailUi();
        });
    }

    private void CreateGrimoireIngredientCardGrid(Transform parent, GrimoireEntryDefinition entry, float x, float y)
    {
        const float cardWidth = 210f;
        const float cardHeight = 150f;
        const float gapX = 24f;
        const float gapY = 20f;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            AlbumCardDefinition card = entry.Cards[index];
            int col = index % 5;
            int row = index / 5;
            int copies = inventory.GetGrimoireCardCopies(card.Id);
            bool owned = copies > 0;
            Color slotColor = owned ? GetOwnedAlbumCardColor(card.Tier) : GetMissingCardColor();
            Color textColor = owned ? Color.white : GetMissingCardTextColor();
            GameObject tile = CreateAnchoredPanel(parent, $"IngredientCard_{card.SlotNumber}", slotColor, cardWidth, cardHeight, x - 468f + col * (cardWidth + gapX), y + 128f - row * (cardHeight + gapY));
            Button cardButton = tile.AddComponent<Button>();
            cardButton.targetGraphic = tile.GetComponent<Image>();
            cardButton.onClick.AddListener(() => BuildGrimoireIngredientDetailModal(entry, card));
            CreateAnchoredText(tile.transform, owned ? BuildStarText(card.Stars) : BuildMissingStarText(card.Stars), 23, FontStyle.Bold, owned ? new Color(1f, 0.9f, 0.32f) : GetMissingCardTextColor(), cardWidth - 20, 26, 0f, 52f);
            CreateAnchoredText(tile.transform, owned ? card.CardName : "Missing", 18, FontStyle.Bold, textColor, cardWidth - 26, 54, 0f, 6f);
            CreateAnchoredText(tile.transform, owned ? "Owned" : $"{GetAlbumTierLabel(card.Tier)}", 14, FontStyle.Bold, textColor, cardWidth - 20, 24, 0f, -48f);
            if (!owned)
            {
                CreateAnchoredText(tile.transform, "?", 46, FontStyle.Bold, new Color(0.22f, 0.2f, 0.23f), 80, 58, 0f, 4f);
            }

            if (copies > 1)
            {
                CreateDuplicateBadge(tile.transform, copies - 1, 74f, 56f, 42, 16);
            }

            if (inventory.IsGrimoireCardUnseen(card.Id))
            {
                CreateNewBadge(tile.transform, 68f, 58f, 72, 26);
            }
        }
    }

    private void BuildGrimoireIngredientDetailModal(GrimoireEntryDefinition entry, AlbumCardDefinition card)
    {
        RemoveIngredientDetailModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "IngredientDetailModal", new Color(0.94f, 0.82f, 0.62f, 0.985f), 980, 600, 0f, -20f);
        Color purple = new Color(0.18f, 0.08f, 0.32f);
        Color green = new Color(0.12f, 0.55f, 0.08f);
        Color muted = new Color(0.34f, 0.26f, 0.34f);
        int copies = inventory.GetGrimoireCardCopies(card.Id);

        CreateAnchoredText(panel.transform, card.CardName, 36, FontStyle.Bold, purple, 820, 54, 10f, 242f);
        CreateAnchoredText(panel.transform, $"{entry.PotionName} | {GetAlbumTierLabel(card.Tier)} | {BuildStarText(card.Stars)}", 17, FontStyle.Bold, muted, 760, 26, 18f, 204f);

        GameObject art = CreateAnchoredPanel(panel.transform, "IngredientArtwork", GetAlbumTierColor(card.Tier), 330, 320, -270f, 24f);
        CreateAnchoredText(art.transform, BuildIngredientSpecimenText(card.CardName), 30, FontStyle.Bold, Color.white, 280, 170, 0f, 22f);
        CreateAnchoredText(art.transform, "ingredient card", 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 240, 28, 0f, -122f);

        GameObject countPanel = CreateAnchoredPanel(panel.transform, "IngredientCountPanel", new Color(0.98f, 0.9f, 0.7f), 400, 126, 230f, 96f);
        CreateAnchoredText(countPanel.transform, "YOU HAVE", 18, FontStyle.Bold, new Color(0.16f, 0.38f, 0.08f), 260, 26, 0f, 36f);
        CreateAnchoredText(countPanel.transform, $"{copies} / 1", 44, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 280, 60, 0f, -8f);
        CreateAnchoredText(countPanel.transform, copies > 0 ? "Owned" : "Missing", 17, FontStyle.Bold, muted, 260, 24, 0f, -48f);

        Button help = CreateAnchoredButton(panel.transform, "Ask for Help", 20, 230, 50, new Color(0.35f, 0.12f, 0.62f), 104f, -40f);
        help.onClick.AddListener(() => BuildIngredientAskForHelpModal(entry, card, true, ""));

        Button wild = CreateAnchoredButton(panel.transform, inventory.JokerWildCards > 0 ? "Use Wild" : "No Wild Cards", 20, 230, 50, inventory.JokerWildCards > 0 ? green : new Color(0.34f, 0.32f, 0.38f), 356f, -40f);
        wild.interactable = inventory.JokerWildCards > 0;
        wild.onClick.AddListener(() => BuildUseWildConfirmation(entry, card));

        GameObject sourcePanel = CreateAnchoredPanel(panel.transform, "WhereToGet", new Color(0.98f, 0.9f, 0.7f), 500, 88, 230f, -152f);
        CreateAnchoredText(sourcePanel.transform, "WHERE TO GET", 16, FontStyle.Bold, purple, 450, 22, 0f, 22f);
        CreateAnchoredText(sourcePanel.transform, "Play realm rooms, open card packs, or ask eligible Coven/Friend helpers.", 15, FontStyle.Bold, muted, 450, 38, 0f, -12f);

        Button close = CreateAnchoredButton(panel.transform, "Back to Ingredients", 18, 240, 46, new Color(0.35f, 0.12f, 0.62f), 0f, -252f);
        close.onClick.AddListener(RemoveIngredientDetailModal);
    }

    private void BuildIngredientAskForHelpModal(GrimoireEntryDefinition entry, AlbumCardDefinition card, bool covenTab, string statusMessage)
    {
        RemoveIngredientHelpModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "IngredientHelpModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1100, 620, 0f, -20f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color purple = new Color(0.35f, 0.12f, 0.62f);
        Color blue = new Color(0.16f, 0.26f, 0.48f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int used = inventory.GetDailyHelpRequestsUsedToday();
        int limit = inventory.GetDailyHelpRequestLimit();
        bool friendsAvailable = IsFriendHelpEligibleToday(card);
        bool activeTabAvailable = covenTab || friendsAvailable;

        CreateAnchoredText(panel.transform, "ASK FOR HELP", 44, FontStyle.Bold, gold, 620, 56, 120f, 242f);
        CreateAnchoredText(panel.transform, $"Daily Help Requests  {used} / {limit} Used", 24, FontStyle.Bold, Color.white, 520, 42, 188f, 184f);

        GameObject summary = CreateAnchoredPanel(panel.transform, "HelpIngredientSummary", cream, 360, 390, -320f, 0f);
        CreateAnchoredText(summary.transform, card.CardName, 30, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 310, 76, 0f, 118f);
        CreateAnchoredText(summary.transform, BuildIngredientSpecimenText(card.CardName), 54, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 260, 100, 0f, 12f);
        CreateAnchoredText(summary.transform, $"You Have\n{inventory.GetGrimoireCardCopies(card.Id)} / 1", 28, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 260, 92, 0f, -118f);

        Button coven = CreateAnchoredButton(panel.transform, "Coven", 21, 260, 48, covenTab ? purple : new Color(0.28f, 0.26f, 0.32f), 62f, 98f);
        coven.onClick.AddListener(() => BuildIngredientAskForHelpModal(entry, card, true, statusMessage));
        Button friends = CreateAnchoredButton(panel.transform, friendsAvailable ? "Friends" : "Friends Locked", 21, 260, 48, !covenTab ? blue : new Color(0.28f, 0.26f, 0.32f), 330f, 98f);
        friends.interactable = friendsAvailable;
        friends.onClick.AddListener(() => BuildIngredientAskForHelpModal(entry, card, false, statusMessage));

        GameObject recipientPanel = CreateAnchoredPanel(panel.transform, "HelpRecipients", cream, 640, 238, 196f, -38f);
        CreateAnchoredText(recipientPanel.transform, covenTab ? "Coven helpers" : "Friend helpers", 22, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 560, 30, 0f, 86f);
        CreateHelpRecipientGrid(recipientPanel.transform, covenTab, activeTabAvailable);

        string helpNote = !activeTabAvailable
            ? "Friends rotate by ingredient/day. Try Coven or come back later."
            : string.IsNullOrWhiteSpace(statusMessage) ? "Select helpers, then send. Prototype sends to visible helpers." : statusMessage;
        CreateAnchoredText(panel.transform, helpNote, 16, FontStyle.Bold, muted, 640, 36, 196f, -184f);

        bool canSend = activeTabAvailable && inventory.CanSendSocialHelpRequest();
        Button send = CreateAnchoredButton(panel.transform, canSend ? "Send Request" : "Limit Reached", 22, 300, 54, canSend ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 196f, -242f);
        send.interactable = canSend;
        send.onClick.AddListener(() =>
        {
            string sentMessage = inventory.TrySendSocialHelpRequest()
                ? $"Sent request for {card.CardName}."
                : "Daily help request limit reached.";
            BuildIngredientAskForHelpModal(entry, card, covenTab, sentMessage);
        });

        Button back = CreateAnchoredButton(panel.transform, "Back to Ingredient", 18, 240, 46, new Color(0.18f, 0.16f, 0.22f), -390f, -242f);
        back.onClick.AddListener(RemoveIngredientHelpModal);
    }

    private void CreateHelpRecipientGrid(Transform parent, bool covenTab, bool available)
    {
        string[] names = covenTab
            ? new[] { "Luna", "Eldric", "Mira", "Rowan", "Sable", "Iris" }
            : new[] { "Nia", "Cal", "June", "Tess", "Ori", "Vale" };
        for (int index = 0; index < names.Length; index++)
        {
            int col = index % 3;
            int row = index / 3;
            float x = -200f + col * 200f;
            float y = 24f - row * 78f;
            GameObject helper = CreateAnchoredPanel(parent, $"HelpRecipient_{names[index]}", available ? new Color(0.18f, 0.13f, 0.2f) : new Color(0.32f, 0.3f, 0.32f), 150, 58, x, y);
            CreateAnchoredText(helper.transform, available ? "OK" : "--", 16, FontStyle.Bold, available ? new Color(0.8f, 1f, 0.55f) : new Color(0.66f, 0.62f, 0.7f), 36, 24, -48f, 6f);
            CreateAnchoredText(helper.transform, names[index], 18, FontStyle.Bold, Color.white, 92, 28, 20f, 8f);
            CreateAnchoredText(helper.transform, available ? "Online" : "Unavailable", 11, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 100, 18, 20f, -16f);
        }
    }

    private void BuildUseWildConfirmation(GrimoireEntryDefinition entry, AlbumCardDefinition card)
    {
        Transform existing = contentRoot.Find("UseWildConfirmation");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        GameObject panel = CreateAnchoredPanel(contentRoot, "UseWildConfirmation", new Color(0.07f, 0.035f, 0.12f, 0.985f), 620, 300, 0f, -20f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        CreateAnchoredText(panel.transform, $"Use 1 Wild for {card.CardName}?", 25, FontStyle.Bold, gold, 540, 42, 0f, 104f);

        GameObject wildCard = CreateAnchoredPanel(panel.transform, "WildCardPreview", new Color(0.35f, 0.12f, 0.62f), 150, 150, -160f, 4f);
        CreateAnchoredText(wildCard.transform, "WILD\nCARD", 28, FontStyle.Bold, Color.white, 120, 90, 0f, 0f);
        CreateAnchoredText(panel.transform, ">", 42, FontStyle.Bold, Color.white, 80, 60, 0f, 4f);
        GameObject targetCard = CreateAnchoredPanel(panel.transform, "TargetIngredientPreview", GetAlbumTierColor(card.Tier), 150, 150, 160f, 4f);
        CreateAnchoredText(targetCard.transform, BuildIngredientSpecimenText(card.CardName), 23, FontStyle.Bold, Color.white, 120, 76, 0f, 10f);
        CreateAnchoredText(targetCard.transform, card.CardName, 11, FontStyle.Bold, Color.white, 130, 34, 0f, -54f);

        Button no = CreateAnchoredButton(panel.transform, "No", 20, 150, 44, new Color(0.34f, 0.32f, 0.38f), -88f, -118f);
        no.onClick.AddListener(() => Destroy(panel));
        Button yes = CreateAnchoredButton(panel.transform, "Yes", 20, 150, 44, new Color(0.12f, 0.55f, 0.08f), 88f, -118f);
        yes.onClick.AddListener(() =>
        {
            if (inventory.TryUseJokerWildForGrimoireCard(card.Id))
            {
                Destroy(panel);
                BuildGrimoireIngredientOverlay(entry);
                BuildGrimoireIngredientDetailModal(entry, card);
            }
        });
    }

    private bool IsFriendHelpEligibleToday(AlbumCardDefinition card)
    {
        return (System.DateTime.UtcNow.DayOfYear + card.SlotNumber) % 2 == 0;
    }

    private string BuildIngredientSpecimenText(string cardName)
    {
        string[] words = cardName.Split(' ');
        if (words.Length == 1)
        {
            return words[0].Substring(0, Mathf.Min(3, words[0].Length)).ToUpperInvariant();
        }

        return $"{words[0]}\n{words[words.Length - 1]}";
    }

    private void RemoveIngredientDetailModal()
    {
        RemoveIngredientHelpModal();
        Transform wild = contentRoot.Find("UseWildConfirmation");
        if (wild != null)
        {
            Destroy(wild.gameObject);
        }

        Transform existing = contentRoot.Find("IngredientDetailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveIngredientHelpModal()
    {
        Transform existing = contentRoot.Find("IngredientHelpModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void CreateBookOfShadowsIngredientCardGrid(Transform parent, BookOfShadowsEntryDefinition entry, float x, float y)
    {
        const float cardWidth = 210f;
        const float cardHeight = 150f;
        const float gapX = 24f;
        const float gapY = 20f;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            BookOfShadowsCardDefinition card = entry.Cards[index];
            int col = index % 5;
            int row = index / 5;
            int copies = inventory.GetBookOfShadowsCardCopies(card.Id);
            bool owned = copies > 0;
            Color slotColor = owned ? GetOwnedAlbumCardColor(AlbumCardTier.Regular) : GetMissingCardColor();
            Color textColor = owned ? Color.white : GetMissingCardTextColor();
            GameObject tile = CreateAnchoredPanel(parent, $"ShadowsIngredientCard_{card.SlotNumber}", slotColor, cardWidth, cardHeight, x - 468f + col * (cardWidth + gapX), y + 92f - row * (cardHeight + gapY));
            CreateAnchoredText(tile.transform, owned ? BuildStarText(card.Stars) : BuildMissingStarText(card.Stars), 23, FontStyle.Bold, owned ? new Color(1f, 0.9f, 0.32f) : GetMissingCardTextColor(), cardWidth - 20, 26, 0f, 52f);
            CreateAnchoredText(tile.transform, owned ? card.CardName : "Missing", 18, FontStyle.Bold, textColor, cardWidth - 26, 54, 0f, 6f);
            CreateAnchoredText(tile.transform, owned ? "Owned" : "Regular", 14, FontStyle.Bold, textColor, cardWidth - 20, 24, 0f, -48f);
            if (!owned)
            {
                CreateAnchoredText(tile.transform, "?", 46, FontStyle.Bold, new Color(0.22f, 0.2f, 0.23f), 80, 58, 0f, 4f);
            }

            if (copies > 1)
            {
                CreateDuplicateBadge(tile.transform, copies - 1, 74f, 56f, 42, 16);
            }

            if (inventory.IsBookOfShadowsCardUnseen(card.Id))
            {
                CreateNewBadge(tile.transform, 68f, 58f, 72, 26);
            }
        }
    }

    private void CreateLibraryInventoryRow(Transform parent, string title, string note, int count, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"LibraryRow_{title}", new Color(0.96f, 0.86f, 0.62f), 310, 46, x, y);
        CreateAnchoredText(row.transform, title, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 176, 22, -48f, 8f);
        CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 194, 18, -36f, -13f);
        CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 62, 34, 114f, 0f);
    }

    private void CreateLibraryStarRow(Transform parent, int stars, float x, float y)
    {
        int total = CardAlbumCatalog.CountByStars(stars);
        int owned = inventory.GetOwnedGrimoireCardCountByStars(stars);
        GameObject row = CreateAnchoredPanel(parent, $"LibraryStarRow_{stars}", new Color(0.96f, 0.86f, 0.62f), 390, 34, x, y);
        CreateAnchoredText(row.transform, $"{stars} star", 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 86, 20, -142f, 3f);
        CreateAnchoredText(row.transform, "all card types", 12, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 120, 18, -42f, 2f);
        CreateAnchoredText(row.transform, $"{owned}/{total}", 17, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 84, 22, 112f, 2f);
    }

    private void CreateNewBadge(Transform parent, float x, float y, float width, float height, int fontSize = 12)
    {
        GameObject badge = CreateAnchoredPanel(parent, "NewBadge", new Color(0.12f, 0.55f, 0.08f), width, height, x, y);
        badge.transform.SetAsLastSibling();
        CreateAnchoredText(badge.transform, "NEW", fontSize, FontStyle.Bold, Color.white, width - 4f, height - 2f, 0f, 0f);
    }

    private void CreateDuplicateBadge(Transform parent, int duplicateCount, float x, float y, float size, int fontSize = 14)
    {
        if (duplicateCount <= 0)
        {
            return;
        }

        GameObject badge = CreateAnchoredPanel(parent, "DuplicateBadge", new Color(0.05f, 0.52f, 0.62f), size, size, x, y);
        badge.transform.SetAsLastSibling();
        CreateAnchoredText(badge.transform, duplicateCount.ToString(), fontSize, FontStyle.Bold, Color.white, size - 4f, size - 4f, 0f, 0f);
    }

    private void SelectGrimoireEntry(int entryIndex)
    {
        selectedGrimoireEntryIndex = Mathf.Clamp(entryIndex, 0, CardAlbumCatalog.GrimoireOneEntries.Count - 1);
        BuildGrimoireDetailUi();
    }

    private void SelectBookOfShadowsEntry(int entryIndex)
    {
        int activeShadowSetIndex = Mathf.Clamp(inventory.GetActiveBookOfShadowsSetNumber() - 1, 0, CardAlbumCatalog.BookOfShadowsSets.Count - 1);
        BookOfShadowsSetDefinition activeShadowSet = CardAlbumCatalog.BookOfShadowsSets[activeShadowSetIndex];
        selectedBookOfShadowsEntryIndex = Mathf.Clamp(entryIndex, 0, activeShadowSet.Entries.Count - 1);
        BuildBookOfShadowsDetailUi();
    }

    private void ClaimGrimoireEntryReward(GrimoireEntryDefinition entry)
    {
        if (inventory.TryClaimGrimoireEntryReward(entry))
        {
            RefreshPowerUpDisplays();
        }

        BuildGrimoireDetailUi();
    }

    private void ClaimGrimoireCompletionReward()
    {
        if (inventory.TryClaimGrimoireCompletionReward())
        {
            RefreshPowerUpDisplays();
        }

        BuildGrimoireIndexUi();
    }

    private void ClaimBookOfShadowsEntryReward(BookOfShadowsEntryDefinition entry)
    {
        if (inventory.TryClaimBookOfShadowsEntryReward(entry))
        {
            RefreshPowerUpDisplays();
        }

        BuildBookOfShadowsDetailUi();
    }

    private void ClaimBookOfShadowsCompletionReward()
    {
        if (inventory.TryClaimBookOfShadowsCompletionReward())
        {
            RefreshPowerUpDisplays();
        }

        BuildBookOfShadowsLibraryUi();
    }

    private string GetGrimoireEntryClaimButtonText(GrimoireEntryDefinition entry)
    {
        if (inventory.IsGrimoireEntryRewardClaimed(entry))
        {
            return "Reward Claimed";
        }

        return inventory.CanClaimGrimoireEntryReward(entry) ? "Claim Reward" : "Incomplete";
    }

    private string GetBookOfShadowsEntryClaimButtonText(BookOfShadowsEntryDefinition entry)
    {
        if (inventory.IsBookOfShadowsEntryRewardClaimed(entry))
        {
            return "Reward Claimed";
        }

        return inventory.CanClaimBookOfShadowsEntryReward(entry) ? "Claim Reward" : "Incomplete";
    }

    private string GetGrimoireCompletionClaimButtonText()
    {
        if (inventory.IsGrimoireCompletionRewardClaimed())
        {
            return "Full Reward Claimed";
        }

        return inventory.CanClaimGrimoireCompletionReward() ? "Claim Full Reward" : "Full Reward Locked";
    }

    private string GetBookOfShadowsCompletionClaimButtonText()
    {
        if (inventory.IsBookOfShadowsCompletionRewardClaimed())
        {
            return "Full Reward Claimed";
        }

        return inventory.CanClaimBookOfShadowsCompletionReward() ? "Claim Full Reward" : "Full Reward Locked";
    }

    private Color GetAlbumClaimButtonColor(bool canClaim, bool claimed)
    {
        if (claimed)
        {
            return new Color(0.18f, 0.34f, 0.16f);
        }

        return canClaim ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f);
    }

    private int BuildCompletedGrimoireEntryCount()
    {
        int completed = 0;
        IReadOnlyList<GrimoireEntryDefinition> entries = CardAlbumCatalog.GrimoireOneEntries;
        for (int index = 0; index < entries.Count; index++)
        {
            if (inventory.IsGrimoireEntryComplete(entries[index]))
            {
                completed++;
            }
        }

        return completed;
    }

    private int BuildCompletedBookOfShadowsEntryCount(BookOfShadowsSetDefinition set)
    {
        int completed = 0;
        for (int index = 0; index < set.Entries.Count; index++)
        {
            if (inventory.IsBookOfShadowsEntryComplete(set.Entries[index]))
            {
                completed++;
            }
        }

        return completed;
    }

    private void CreateLibraryPotionBook(Transform parent, IReadOnlyList<GrimoireEntryDefinition> entries, float x, float y)
    {
        const int columns = 4;
        const float slotWidth = 138f;
        const float slotHeight = 48f;
        const float gapX = 12f;
        const float gapY = 10f;
        for (int index = 0; index < entries.Count; index++)
        {
            GrimoireEntryDefinition entry = entries[index];
            int col = index % columns;
            int row = index / columns;
            float slotX = x - 225f + col * (slotWidth + gapX);
            float slotY = y + 104f - row * (slotHeight + gapY);
            int owned = inventory.GetOwnedGrimoireEntryCardCount(entry);
            bool complete = inventory.IsGrimoireEntryComplete(entry);
            Color color = complete ? new Color(0.12f, 0.42f, 0.16f) : new Color(0.24f, 0.14f, 0.26f);
            GameObject slot = CreateAnchoredPanel(parent, $"LibraryPotion_{entry.EntryNumber}", color, slotWidth, slotHeight, slotX, slotY);
            CreateAnchoredText(slot.transform, $"Potion {entry.EntryNumber}", 13, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), slotWidth - 10f, 18, 0f, 11f);
            CreateAnchoredText(slot.transform, $"{owned}/{entry.Cards.Count} | {CardAlbumCatalog.BuildCompactRewardLine(entry.Reward)}", 9, FontStyle.Bold, Color.white, slotWidth - 10f, 18, 0f, -8f);
        }
    }

    private void CreateGrimoireCardSlotGrid(Transform parent, GrimoireEntryDefinition entry, float x, float y)
    {
        const float slotWidth = 48f;
        const float slotHeight = 34f;
        const float gapX = 8f;
        const float gapY = 8f;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            AlbumCardDefinition card = entry.Cards[index];
            int col = index % 5;
            int row = index / 5;
            float slotX = x - 112f + col * (slotWidth + gapX);
            float slotY = y + 42f - row * (slotHeight + gapY);
            int copies = inventory.GetGrimoireCardCopies(card.Id);
            bool owned = copies > 0;
            Color slotColor = owned ? GetOwnedAlbumCardColor(card.Tier) : GetMissingCardColor();
            Color textColor = owned ? Color.white : GetMissingCardTextColor();
            GameObject slot = CreateAnchoredPanel(parent, $"GrimoireCardSlot_{card.EntryNumber}_{card.SlotNumber}", slotColor, slotWidth, slotHeight, slotX, slotY);
            CreateAnchoredText(slot.transform, $"{(owned ? card.Stars.ToString() : "?")}*{GetAlbumTierSuffix(card.Tier)}", 12, FontStyle.Bold, textColor, slotWidth - 4f, 18, 0f, 4f);
            CreateAnchoredText(slot.transform, owned ? "OWN" : "MISS", 8, FontStyle.Bold, owned ? new Color(0.92f, 0.96f, 0.84f) : new Color(0.84f, 0.82f, 0.94f), slotWidth - 4f, 12, 0f, -10f);
            if (copies > 1)
            {
                CreateDuplicateBadge(slot.transform, copies - 1, 16f, 10f, 18, 8);
            }

            if (inventory.IsGrimoireCardUnseen(card.Id))
            {
                CreateNewBadge(slot.transform, 14f, 10f, 38, 16, 8);
            }
        }
    }

    private void CreateBookOfShadowsCardSlotGrid(Transform parent, BookOfShadowsEntryDefinition entry, float x, float y)
    {
        const float slotWidth = 38f;
        const float slotHeight = 24f;
        const float gapX = 6f;
        for (int index = 0; index < entry.Cards.Count; index++)
        {
            BookOfShadowsCardDefinition card = entry.Cards[index];
            float slotX = x - 198f + index * (slotWidth + gapX);
            int copies = inventory.GetBookOfShadowsCardCopies(card.Id);
            bool owned = copies > 0;
            Color slotColor = owned ? GetOwnedAlbumCardColor(AlbumCardTier.Regular) : GetMissingCardColor();
            Color textColor = owned ? Color.white : GetMissingCardTextColor();
            GameObject slot = CreateAnchoredPanel(parent, $"BookOfShadowsCardSlot_{card.SetNumber}_{card.EntryNumber}_{card.SlotNumber}", slotColor, slotWidth, slotHeight, slotX, y);
            CreateAnchoredText(slot.transform, owned ? $"{card.Stars}*" : "?*", 11, FontStyle.Bold, textColor, slotWidth - 4f, 16, 0f, 1f);
            if (copies > 1)
            {
                CreateDuplicateBadge(slot.transform, copies - 1, 12f, 10f, 18, 8);
            }

            if (inventory.IsBookOfShadowsCardUnseen(card.Id))
            {
                CreateNewBadge(slot.transform, 12f, 10f, 38, 16, 8);
            }
        }
    }

    private Color GetAlbumTierColor(AlbumCardTier tier)
    {
        if (tier == AlbumCardTier.Gilded)
        {
            return new Color(0.26f, 0.52f, 0.96f);
        }

        if (tier == AlbumCardTier.Ancient)
        {
            return new Color(0.4f, 0.31f, 0.65f);
        }

        return new Color(0.96f, 0.86f, 0.62f);
    }

    private Color GetOwnedAlbumCardColor(AlbumCardTier tier)
    {
        if (tier == AlbumCardTier.Gilded)
        {
            return new Color(0.22f, 0.46f, 0.84f);
        }

        if (tier == AlbumCardTier.Ancient)
        {
            return new Color(0.42f, 0.29f, 0.66f);
        }

        return new Color(0.28f, 0.46f, 0.34f);
    }

    private Color GetMissingCardColor()
    {
        return new Color(0.27f, 0.24f, 0.28f);
    }

    private Color GetMissingCardTextColor()
    {
        return new Color(0.72f, 0.69f, 0.76f);
    }

    private string GetAlbumTierSuffix(AlbumCardTier tier)
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

    private string GetAlbumTierLabel(AlbumCardTier tier)
    {
        if (tier == AlbumCardTier.Gilded)
        {
            return "Gilded";
        }

        if (tier == AlbumCardTier.Ancient)
        {
            return "Ancient";
        }

        return "Regular";
    }

    private string BuildStarText(int stars)
    {
        return new string('*', Mathf.Max(1, stars));
    }

    private string BuildMissingStarText(int stars)
    {
        return new string('-', Mathf.Max(1, stars));
    }

    private void CreateCovenStat(Transform parent, string label, string value, float x, float y)
    {
        CreateAnchoredText(parent, value, 28, FontStyle.Bold, Color.white, 240, 34, x, y + 12f);
        CreateAnchoredText(parent, label, 14, FontStyle.Bold, new Color(0.84f, 0.88f, 1f), 240, 18, x, y - 20f);
    }

    private void CreateCovenSection(Transform parent, string title, float x, float y, float width, float height, Color color)
    {
        GameObject section = CreateAnchoredPanel(parent, $"Coven_{title}", color, width, height, x, y);
        CreateAnchoredText(section.transform, title.ToUpperInvariant(), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, 24, 0f, height * 0.5f - 20f);
    }

    private void CreateCovenFeatureLine(Transform parent, string title, string note, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenFeature_{title}", new Color(0.96f, 0.86f, 0.62f), 286, 42, x, y);
        CreateAnchoredText(row.transform, title, 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 20, 0f, 8f);
        CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 250, 16, 0f, -12f);
    }

    private void CreateCovenLeadershipField(Transform parent, string title, string note, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenLeadership_{title}", new Color(0.96f, 0.86f, 0.62f), 280, 50, x, y);
        CreateAnchoredText(row.transform, title, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 20, 0f, 10f);
        CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 250, 18, 0f, -13f);
    }

    private void CreateCovenJoinRequestRow(Transform parent, CovenJoinRequestInfo request, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenJoinRequest_{request.Name}", new Color(0.96f, 0.86f, 0.62f), 310, 54, x, y);
        CreateAnchoredText(row.transform, request.Name, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 120, 20, -82f, 10f);
        CreateAnchoredText(row.transform, request.Summary, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 154, 18, -64f, -12f);
        Button accept = CreateAnchoredButton(row.transform, "Accept", 11, 62, 26, new Color(0.12f, 0.55f, 0.08f), 70f, 10f);
        accept.interactable = coven.CanAcceptJoinRequests();
        accept.onClick.AddListener(() => AcceptCovenJoinRequest(request.Name));
        Button deny = CreateAnchoredButton(row.transform, "Deny", 11, 62, 26, new Color(0.42f, 0.12f, 0.18f), 70f, -18f);
        deny.onClick.AddListener(() => DenyCovenJoinRequest(request.Name));
    }

    private void CreateCovenMemberRow(Transform parent, CovenMemberInfo member, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenMember_{member.Name}", new Color(0.96f, 0.86f, 0.62f), 250, 46, x, y);
        Button button = row.AddComponent<Button>();
        button.onClick.AddListener(() => BuildCovenMemberDetailUi(member));
        CreateAnchoredText(row.transform, member.Name, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 210, 22, 0f, 9f);
        CreateAnchoredText(row.transform, member.GetSummaryText(), 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 216, 18, 0f, -13f);
    }

    private void BuildCovenMemberDetailUi(CovenMemberInfo member)
    {
        Transform existing = contentRoot.Find("CovenMemberDetailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        CovenMemberInfo currentMember = coven.GetMember(member.Name) ?? member;
        GameObject panel = CreateAnchoredPanel(contentRoot, "CovenMemberDetailModal", new Color(0.08f, 0.04f, 0.13f, 0.985f), 720, 560, 0f, -20f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color ink = new Color(0.18f, 0.08f, 0.32f);

        CreateAnchoredText(panel.transform, currentMember.Name, 38, FontStyle.Bold, gold, 620, 52, 0f, 226f);
        CreateAnchoredText(panel.transform, coven.CovenName, 18, FontStyle.Bold, muted, 620, 30, 0f, 188f);
        CreateAnchoredText(panel.transform, "PROFILE", 15, FontStyle.Bold, gold, 280, 24, -190f, 142f);
        CreateAnchoredText(panel.transform, "WISH LIST", 15, FontStyle.Bold, gold, 280, 24, 190f, 142f);

        GameObject profile = CreateAnchoredPanel(panel.transform, "CovenMemberProfileTab", new Color(0.12f, 0.075f, 0.16f), 300, 300, -190f, -20f);
        CreateAnchoredText(profile.transform, $"{currentMember.LastOnline}\n{currentMember.Stats}", 21, FontStyle.Bold, Color.white, 250, 76, 0f, 78f);
        CreateAnchoredText(profile.transform, "Wish lists are visible to covenmates only.\nFulfilled gifts are sent to this member's inbox for collection.", 15, FontStyle.Bold, muted, 250, 104, 0f, -34f);
        CreateAnchoredText(profile.transform, $"Inbox gifts queued: {coven.InboxGifts.Count}", 15, FontStyle.Bold, gold, 250, 26, 0f, -124f);

        GameObject wish = CreateAnchoredPanel(panel.transform, "CovenMemberWishTab", new Color(0.96f, 0.86f, 0.62f), 330, 300, 190f, -20f);
        CreateAnchoredText(wish.transform, "Refreshes in 48h prototype window", 13, FontStyle.Bold, ink, 290, 22, 0f, 126f);
        if (currentMember.CardWish != null)
        {
            int regularCards = inventory.GetInventoryRewardCount("Regular Card");
            string cardStatus = currentMember.CardWish.IsFulfilled ? "Fulfilled - inbox pending" : $"{currentMember.CardWish.ItemName}\nYou own regular cards: {regularCards}";
            GameObject cardRow = CreateAnchoredPanel(wish.transform, "CovenCardWish", new Color(0.18f, 0.13f, 0.2f), 290, 58, 0f, 84f);
            CreateAnchoredText(cardRow.transform, cardStatus, 12, FontStyle.Bold, Color.white, 188, 46, -40f, 0f);
            Button cardGift = CreateAnchoredButton(cardRow.transform, currentMember.CardWish.IsFulfilled ? "Sent" : "Gift Card", 11, 78, 30, regularCards > 0 && !currentMember.CardWish.IsFulfilled ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 98f, 0f);
            cardGift.interactable = regularCards > 0 && !currentMember.CardWish.IsFulfilled;
            cardGift.onClick.AddListener(() => GiftCovenMemberCardWish(currentMember.Name));
        }

        IReadOnlyList<CovenWishListRequest> ingredientWishes = currentMember.IngredientWishes;
        for (int index = 0; index < ingredientWishes.Count && index < 4; index++)
        {
            CreateCovenWishIngredientRow(wish.transform, currentMember.Name, ingredientWishes[index], 42f - index * 48f);
        }

        Button back = CreateAnchoredButton(panel.transform, "Back to Coven", 18, 220, 44, new Color(0.35f, 0.12f, 0.62f), 0f, -236f);
        back.onClick.AddListener(BuildCovenCircleUi);
    }

    private void CreateCovenWishIngredientRow(Transform parent, string memberName, CovenWishListRequest request, float y)
    {
        int owned = inventory.GetIngredientInventoryCount(request.ItemName);
        bool canGift = owned > 0 && request.RemainingQuantity > 0;
        GameObject row = CreateAnchoredPanel(parent, $"CovenIngredientWish_{request.ItemName}", new Color(0.18f, 0.13f, 0.2f), 290, 42, 0f, y);
        string label = request.IsFulfilled
            ? $"{request.ItemName}\nFulfilled - inbox pending"
            : $"{request.ItemName} {request.RemainingQuantity}/{request.RequestedQuantity}\nYou own {owned}";
        CreateAnchoredText(row.transform, label, 10, FontStyle.Bold, Color.white, 190, 34, -42f, 0f);
        Button gift = CreateAnchoredButton(row.transform, request.IsFulfilled ? "Sent" : "Gift 1", 10, 72, 26, canGift ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 98f, 0f);
        gift.interactable = canGift;
        gift.onClick.AddListener(() => GiftCovenMemberIngredientWish(memberName, request.ItemName));
    }

    private void GiftCovenMemberCardWish(string memberName)
    {
        if (inventory.TryConsumeInventoryReward("Regular Card", 1) && coven.FulfillRegularCardWish(memberName))
        {
            CovenMemberInfo member = coven.GetMember(memberName);
            if (member != null)
            {
                BuildCovenMemberDetailUi(member);
            }
        }
    }

    private void GiftCovenMemberIngredientWish(string memberName, string ingredientName)
    {
        if (inventory.TryGiftIngredient(ingredientName, 1) && coven.FulfillIngredientWish(memberName, ingredientName, 1))
        {
            CovenMemberInfo member = coven.GetMember(memberName);
            if (member != null)
            {
                BuildCovenMemberDetailUi(member);
            }
        }
    }

    private void BuildCovenEmporiumUi()
    {
        RemoveCovenMemberDetailModal();
        RemoveCovenEmporiumModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "CovenEmporiumModal", new Color(0.07f, 0.035f, 0.12f, 0.985f), 860, 540, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int clubOrbs = inventory.GetInventoryRewardCount("Club Orbs");

        CreateAnchoredText(panel.transform, "COVEN EMPORIUM", 40, FontStyle.Bold, gold, 760, 54, 0f, 220f);
        CreateAnchoredText(panel.transform, $"Spend Club Orbs earned through play and Coven challenges. Held Orbs: {clubOrbs}", 18, FontStyle.Bold, muted, 760, 30, 0f, 180f);

        CreateCovenEmporiumItem(panel.transform, "Small Power-Up Bundle", "Single Sigil x1, Multi Sigil x1", 5, -260f, 50f, GrantCovenPowerUpBundle);
        CreateCovenEmporiumItem(panel.transform, "Ingredient Gift Crate", "Adds active room gifting stock", 8, 0f, 50f, GrantCovenIngredientGiftCrate);
        CreateCovenEmporiumItem(panel.transform, "Coven Chest", "Pandora Sigil x1, Crystals +5", 12, 260f, 50f, GrantCovenChest);

        CreateAnchoredText(panel.transform, "Prototype note: final Emporium can rotate offers, require Coven rank, and use weekly challenge tiers.", 16, FontStyle.Bold, muted, 760, 44, 0f, -160f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Coven", 20, 230, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -218f);
        back.onClick.AddListener(BuildCovenCircleUi);
    }

    private void CreateCovenEmporiumItem(Transform parent, string title, string description, int cost, float x, float y, UnityEngine.Events.UnityAction purchaseAction)
    {
        int clubOrbs = inventory.GetInventoryRewardCount("Club Orbs");
        GameObject item = CreateAnchoredPanel(parent, $"CovenEmporium_{title}", new Color(0.96f, 0.86f, 0.62f), 240, 190, x, y);
        CreateAnchoredText(item.transform, title, 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 210, 48, 0f, 52f);
        CreateAnchoredText(item.transform, description, 14, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 204, 48, 0f, 2f);
        Button buy = CreateAnchoredButton(item.transform, $"Buy {cost} Orbs", 16, 170, 38, clubOrbs >= cost ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, -58f);
        buy.interactable = coven.IsJoined && clubOrbs >= cost;
        buy.onClick.AddListener(purchaseAction);
    }

    private void GrantCovenPowerUpBundle()
    {
        if (!inventory.TryConsumeInventoryReward("Club Orbs", 5))
        {
            return;
        }

        inventory.AddInventoryReward("Single Sigil", 1);
        inventory.AddInventoryReward("Multi Sigil", 1);
        coven.AddPrototypeChatLine("You bought a Small Power-Up Bundle.");
        BuildCovenEmporiumUi();
    }

    private void GrantCovenIngredientGiftCrate()
    {
        if (!inventory.TryConsumeInventoryReward("Club Orbs", 8))
        {
            return;
        }

        IReadOnlyList<IngredientRequirement> requirements = RealmContentCatalog.ActivePrototypeRoom.Ingredients;
        if (requirements.Count > 0)
        {
            inventory.AddIngredientForPrototype(requirements[0].Name, 2);
        }

        if (requirements.Count > 1)
        {
            inventory.AddIngredientForPrototype(requirements[1].Name, 1);
        }

        coven.AddPrototypeChatLine("You bought an Ingredient Gift Crate.");
        BuildCovenEmporiumUi();
    }

    private void GrantCovenChest()
    {
        if (!inventory.TryConsumeInventoryReward("Club Orbs", 12))
        {
            return;
        }

        inventory.AddInventoryReward("Pandora Sigil", 1);
        inventory.AddCrystalsForPrototype(5);
        coven.AddPrototypeChatLine("You bought a Coven Chest.");
        BuildCovenEmporiumUi();
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
        CreateAnchoredText(panel.transform, "Clairvoyance", 24, FontStyle.Bold, gold, 280, 30, -398f, 116f);
        CreateAnchoredText(panel.transform, $"{inventory.ClairvoyanceMinutes}m stock\nActive: {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}", 20, FontStyle.Bold, Color.white, 280, 58, -398f, 68f);
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
        CreateAnchoredText(panel.transform, "Pandora is stored here and opens during gameplay when loaded from the power-up bank.", 15, FontStyle.Bold, muted, 280, 48, -398f, -236f);

        CreateCabinetSection(panel.transform, "Other Destinations", 162f, -196f, 670, 164, new Color(0.13f, 0.08f, 0.15f));
        CreateCabinetDestinationRow(panel.transform, "Cards", "Open Library Grimoire", totalCards, 12f, -178f);
        CreateCabinetDestinationRow(panel.transform, "Club Orbs", "Open Coven Circle", inventory.GetInventoryRewardCount("Club Orbs"), 370f, -178f);
        CreateAnchoredText(panel.transform, "Cards now live in Library. Club Orbs will move to Covens when that view is built.", 15, FontStyle.Bold, muted, 600, 36, 162f, -236f);

        powerUpInventoryText = CreateAnchoredText(panel.transform, "", 1, FontStyle.Normal, Color.clear, 1, 1, 0f, 0f);

        Button close = CreateAnchoredButton(panel.transform, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -292f);
        close.onClick.AddListener(BuildPlayerDenUi);
    }

    private void CreateCabinetStat(Transform parent, string label, string value, float x, float y)
    {
        CreateAnchoredText(parent, value, 24, FontStyle.Bold, Color.white, 210, 30, x, y + 10f);
        CreateAnchoredText(parent, label, 13, FontStyle.Bold, new Color(0.84f, 0.88f, 1f), 210, 18, x, y - 18f);
    }

    private void CreateCabinetSection(Transform parent, string title, float x, float y, float width, float height, Color color)
    {
        GameObject section = CreateAnchoredPanel(parent, $"Cabinet_{title}", color, width, height, x, y);
        CreateAnchoredText(section.transform, title.ToUpperInvariant(), 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), width - 24, 24, 0f, height * 0.5f - 20f);
    }

    private void CreateCabinetInventoryRow(Transform parent, string title, string note, int count, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CabinetRow_{title}", new Color(0.96f, 0.86f, 0.62f), 340, 46, x, y);
        CreateAnchoredText(row.transform, title, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 180, 22, -58f, 8f);
        CreateAnchoredText(row.transform, note, 11, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 210, 18, -42f, -13f);
        CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 70, 34, 126f, 0f);
    }

    private void CreateCabinetDestinationRow(Transform parent, string title, string note, int count, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CabinetRoute_{title}", new Color(0.18f, 0.13f, 0.2f), 340, 46, x, y);
        CreateAnchoredText(row.transform, title, 17, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 180, 22, -58f, 8f);
        CreateAnchoredText(row.transform, note, 11, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 210, 18, -42f, -13f);
        CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, Color.white, 70, 34, 126f, 0f);
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

    private void RemoveLibraryGrimoireModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("LibraryGrimoireModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveCovenCircleModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("CovenCircleModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveCovenMemberDetailModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("CovenMemberDetailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveCovenEmporiumModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("CovenEmporiumModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveInboxModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        RemoveInboxMessageDetailModal();
        Transform existing = contentRoot.Find("InboxModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveInboxMessageDetailModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("InboxMessageDetailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveSocialFreebiesModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("SocialFreebiesModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveDailyBonusModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DailyBonusModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveDailySpinModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DailySpinModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void BuildDevSettingsUi(bool returnToMap)
    {
        BuildDevSettingsUi(returnToMap, false);
    }

    private void BuildDevSettingsUi(bool returnToMap, bool returnToDen)
    {
        devSettingsReturnToDen = returnToDen;
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.1f, 0.04f, 0.16f, 0.98f), 780, 600, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        CreateAnchoredText(panel.transform, "PROTOTYPE SETTINGS", 32, FontStyle.Bold, gold, 700, 44, 0f, 254f);
        CreateAnchoredText(panel.transform, "Hidden tuning tools for clean economy and progression testing.", 17, FontStyle.Bold, new Color(0.85f, 0.82f, 0.95f), 700, 28, 0f, 214f);

        CreateAnchoredText(panel.transform, $"Mana {inventory.GetManaText()}  |  Crystals {inventory.GetCrystalText()}  |  Wheelspins {inventory.PendingJackpotSpins}", 20, FontStyle.Bold, Color.white, 700, 34, 0f, 174f);
        CreateAnchoredText(panel.transform, $"Power-Ups {inventory.GetPowerUpText()}  |  Clairvoyance stock {inventory.ClairvoyanceMinutes}m  |  Active {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}", 18, FontStyle.Bold, new Color(0.85f, 0.95f, 1f), 700, 34, 0f, 142f);

        Button inspect = CreateAnchoredButton(panel.transform, "Profile State", 18, 220, 42, new Color(0.2f, 0.16f, 0.42f), -250f, 86f);
        inspect.onClick.AddListener(() => BuildProfileStateUi(returnToMap));

        Button reset = CreateAnchoredButton(panel.transform, "Fresh New Player", 18, 220, 42, new Color(0.45f, 0.08f, 0.12f), 0f, 86f);
        reset.onClick.AddListener(() => ResetPrototypeSave(returnToMap));

        Button progressReset = CreateAnchoredButton(panel.transform, "Reset Rooms Only", 18, 220, 42, new Color(0.45f, 0.22f, 0.08f), 250f, 86f);
        progressReset.onClick.AddListener(() => ResetRoomProgressKeepingInventory(returnToMap));

        Button mana = CreateAnchoredButton(panel.transform, "+500 Mana", 18, 180, 42, new Color(0.18f, 0.34f, 0.1f), -260f, 28f);
        mana.onClick.AddListener(() => GrantPrototypeMana(500, returnToMap));

        Button crystals = CreateAnchoredButton(panel.transform, "+100 Crystals", 18, 210, 42, new Color(0.14f, 0.25f, 0.48f), -30f, 28f);
        crystals.onClick.AddListener(() => GrantPrototypeCrystals(100, returnToMap));

        Button clairvoyance = CreateAnchoredButton(panel.transform, "+15 Clairvoyance", 18, 230, 42, new Color(0.25f, 0.12f, 0.43f), 230f, 28f);
        clairvoyance.onClick.AddListener(() => GrantPrototypeClairvoyance(returnToMap));

        Button wheelspin = CreateAnchoredButton(panel.transform, "+1 Wheelspin", 18, 220, 42, new Color(0.35f, 0.08f, 0.48f), -250f, -30f);
        wheelspin.onClick.AddListener(() => GrantPrototypeJackpotSpin(returnToMap));

        Button powerUps = CreateAnchoredButton(panel.transform, "+1 Each Power-Up", 18, 240, 42, new Color(0.14f, 0.4f, 0.42f), 0f, -30f);
        powerUps.onClick.AddListener(() => GrantPrototypePowerUpSet(returnToMap));

        Button unlock = CreateAnchoredButton(panel.transform, "Unlock Next Realm", 18, 230, 42, new Color(0.28f, 0.34f, 0.08f), 260f, -30f);
        unlock.onClick.AddListener(() => UnlockNextRealmForTesting(returnToMap));

        Button dailySpinReset = CreateAnchoredButton(panel.transform, "Reset Daily Spin", 18, 220, 42, new Color(0.35f, 0.12f, 0.62f), -250f, -86f);
        dailySpinReset.onClick.AddListener(() => ResetDailySpinForTesting(returnToMap));

        Button persistence = CreateAnchoredButton(panel.transform, "Persistence", 18, 220, 42, new Color(0.12f, 0.3f, 0.42f), 0f, -86f);
        persistence.onClick.AddListener(() => BuildInfrastructureDiagnosticsUi(returnToMap));

        Button albumDebug = CreateAnchoredButton(panel.transform, "Album Debug", 18, 220, 42, new Color(0.2f, 0.16f, 0.42f), 250f, -86f);
        albumDebug.onClick.AddListener(() => BuildAlbumDebugUi(returnToMap));

        CreateAnchoredText(panel.transform, "Fresh New Player resets progression and profile cosmetics while preserving Sound/Notifications and local guest identity. Reset Rooms Only keeps current currency and inventory.", 16, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 680, 78, 0f, -146f);

        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 180, 46, new Color(0.35f, 0.12f, 0.62f), 0f, -252f);
        close.onClick.AddListener(() =>
        {
            if (returnToMap)
            {
                BuildWorldMapUi();
                return;
            }

            if (devSettingsReturnToDen)
            {
                BuildPlayerDenUi();
                return;
            }

            BuildLobbyUi();
        });
    }

    private void BuildProfileStateUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.08f, 0.03f, 0.13f, 0.98f), 940, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        CreateAnchoredText(panel.transform, "SAVED PROFILE STATE", 32, FontStyle.Bold, gold, 820, 44, 0f, 302f);
        CreateAnchoredText(panel.transform, BuildProfileStateSummary(), 16, FontStyle.Bold, Color.white, 840, 500, 0f, 20f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), -140f, -300f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));

        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 170, 46, new Color(0.18f, 0.16f, 0.22f), 130f, -300f);
        close.onClick.AddListener(() => ReturnFromDevSettings(returnToMap));
    }

    private void BuildInfrastructureDiagnosticsUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.055f, 0.035f, 0.1f, 0.99f), 980, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        CreateAnchoredText(panel.transform, "LOCAL PERSISTENCE", 34, FontStyle.Bold, gold, 860, 46, 0f, 302f);
        CreateAnchoredText(panel.transform, "Redacted Beta diagnostics only - no payloads, messages, tokens, or full player id.", 15, FontStyle.Bold, muted, 860, 28, 0f, 266f);
        CreateAnchoredText(panel.transform, BuildInfrastructureDiagnosticsSummary(), 16, FontStyle.Bold, Color.white, 860, 430, 0f, 30f);

        Button export = CreateAnchoredButton(panel.transform, "Export Safe Summary", 18, 250, 46, new Color(0.12f, 0.42f, 0.46f), -270f, -278f);
        export.interactable = GameInfrastructureRuntime.IsInitialized;
        export.onClick.AddListener(() => ExportInfrastructureDiagnostics(returnToMap));

        Button refresh = CreateAnchoredButton(panel.transform, "Refresh", 18, 170, 46, new Color(0.2f, 0.16f, 0.42f), 0f, -278f);
        refresh.onClick.AddListener(() => BuildInfrastructureDiagnosticsUi(returnToMap));

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 18, 230, 46, new Color(0.35f, 0.12f, 0.62f), 250f, -278f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));
    }

    private string BuildInfrastructureDiagnosticsSummary()
    {
        if (!GameInfrastructureRuntime.IsInitialized || GameInfrastructureRuntime.Current.Diagnostics == null)
        {
            return "Infrastructure has not initialized. Profile/settings are using session defaults.";
        }

        InfrastructureDiagnosticsSnapshot diagnostics = GameInfrastructureRuntime.Current.Diagnostics.Capture();
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Environment: {diagnostics.Environment} | Identity: {diagnostics.IdentityProvider} | Player: {diagnostics.RedactedPlayerId}");
        builder.AppendLine($"Snapshots: {diagnostics.SnapshotCount} | Journal: {diagnostics.JournalRecordCount} records / {FormatDiagnosticBytes(diagnostics.JournalBytes)} | Pending action rows: {diagnostics.PendingActionRecordCount}");
        builder.AppendLine($"Last sequence: {diagnostics.LastJournalSequence} | Recovery: {diagnostics.LastRecoveredState} {diagnostics.LastRecoveryAtUtc}");
        builder.AppendLine($"Migration: {diagnostics.LastMigratedState} {diagnostics.LastMigration}");
        if (diagnostics.JournalSyncPolicy != null)
        {
            builder.AppendLine($"Journal policy: {diagnostics.JournalSyncPolicy.PolicyVersion} | live uploads {(diagnostics.JournalSyncPolicy.LiveUploadsEnabled ? "on" : "off")} | active upload eligible {diagnostics.JournalSyncPolicy.ActiveUploadEligibleRecordCount}");
            builder.AppendLine($"Future upload candidates: {diagnostics.JournalSyncPolicy.FutureUploadEligibleRecordCount} | blocked sensitive {diagnostics.JournalSyncPolicy.BlockedSensitiveRecordCount} | blocked unapproved {diagnostics.JournalSyncPolicy.BlockedUnapprovedRecordCount}");
        }

        if (diagnostics.BackendPreflight != null)
        {
            builder.AppendLine($"UGS preflight: packages {diagnostics.BackendPreflight.PackageState}, adapters {diagnostics.BackendPreflight.AdapterDefine}, live calls {(diagnostics.BackendPreflight.LiveCloudCallsEnabled ? "on" : "off")}");
            builder.AppendLine($"Cloud enablement blockers: {diagnostics.BackendPreflight.BlockedCount} | Warnings: {diagnostics.BackendPreflight.WarningCount}");
        }

        builder.AppendLine();
        builder.AppendLine("SNAPSHOT HEALTH");
        foreach (SnapshotDiagnosticsEntry entry in diagnostics.Snapshots)
        {
            builder.AppendLine($"- {entry.StateName} v{entry.SchemaVersion}: {entry.Health}, {FormatDiagnosticBytes(entry.Bytes)}, backup {(entry.HasBackup ? "yes" : "no")}");
        }

        builder.AppendLine();
        builder.AppendLine("JOURNAL SYNC STAGING");
        if (diagnostics.JournalSyncPolicy != null)
        {
            builder.AppendLine($"- retain local: {diagnostics.JournalSyncPolicy.RetainLocalRecordCount}, summary-export allowed: {diagnostics.JournalSyncPolicy.ExportSummaryAllowedRecordCount}");
            builder.AppendLine($"- status rows: pending {diagnostics.JournalSyncPolicy.PendingRecordCount}, applied {diagnostics.JournalSyncPolicy.AppliedLocalRecordCount}, synced {diagnostics.JournalSyncPolicy.SyncedRecordCount}, rejected {diagnostics.JournalSyncPolicy.RejectedRecordCount}, compensated {diagnostics.JournalSyncPolicy.CompensatedRecordCount}");
            builder.AppendLine("- live upload queue is disabled; no journal rows are uploaded in this build.");
        }

        builder.AppendLine();
        builder.AppendLine("UGS ENABLEMENT");
        if (diagnostics.BackendPreflight != null)
        {
            foreach (BackendPreflightCheck check in diagnostics.BackendPreflight.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("Journal retention/clearing is not active until policy is approved.");
        if (!string.IsNullOrWhiteSpace(lastInfrastructureDiagnosticsExport))
        {
            builder.AppendLine($"Last safe export: {lastInfrastructureDiagnosticsExport}");
        }

        return builder.ToString();
    }

    private void ExportInfrastructureDiagnostics(bool returnToMap)
    {
        if (!GameInfrastructureRuntime.IsInitialized || GameInfrastructureRuntime.Current.Diagnostics == null)
        {
            return;
        }

        string exportPath = GameInfrastructureRuntime.Current.Diagnostics.ExportSafeSummary();
        lastInfrastructureDiagnosticsExport = System.IO.Path.GetFileName(exportPath);
        BuildInfrastructureDiagnosticsUi(returnToMap);
    }

    private static string FormatDiagnosticBytes(long bytes)
    {
        if (bytes < 1024)
        {
            return bytes + " B";
        }

        return (bytes / 1024f).ToString("0.0") + " KB";
    }

    private void BuildAlbumDebugUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.08f, 0.03f, 0.13f, 0.98f), 1080, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int regularCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Regular);
        int gildedCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Gilded);
        int ancientCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Ancient);

        CreateAnchoredText(panel.transform, "ALBUM DEBUG", 34, FontStyle.Bold, gold, 980, 46, 0f, 302f);
        CreateAnchoredText(panel.transform, "Testing-only card routing, ownership, duplicates, and Book of Shadows state.", 16, FontStyle.Bold, muted, 980, 26, 0f, 266f);

        CreateAnchoredPanel(panel.transform, "AlbumDebugTotals", new Color(0.14f, 0.07f, 0.18f), 920, 74, 0f, 214f);
        CreateLibraryStat(panel.transform, "Grimoire", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards}", -330f, 214f);
        CreateLibraryStat(panel.transform, "Regular", $"{regularCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Regular)}", -110f, 214f);
        CreateLibraryStat(panel.transform, "Gilded", $"{gildedCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Gilded)}", 110f, 214f);
        CreateLibraryStat(panel.transform, "Ancient", $"{ancientCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Ancient)}", 330f, 214f);

        CreateLibrarySection(panel.transform, "Star System", -270f, 46f, 410, 254, new Color(0.12f, 0.06f, 0.2f));
        for (int stars = 1; stars <= 5; stars++)
        {
            CreateLibraryStarRow(panel.transform, stars, -270f, 124f - (stars - 1) * 42f);
        }

        IReadOnlyList<GrimoireEntryDefinition> entries = CardAlbumCatalog.GrimoireOneEntries;
        selectedGrimoireEntryIndex = Mathf.Clamp(selectedGrimoireEntryIndex, 0, entries.Count - 1);
        GrimoireEntryDefinition selectedEntry = entries[selectedGrimoireEntryIndex];
        CreateLibrarySection(panel.transform, "Grimoire Card Slots", 220f, 46f, 470, 254, new Color(0.1f, 0.05f, 0.15f));
        CreateAnchoredText(panel.transform, $"Potion {selectedEntry.EntryNumber}/{entries.Count}: {selectedEntry.PotionName}", 16, FontStyle.Bold, cream, 420, 28, 220f, 122f);
        CreateAnchoredText(panel.transform, $"{inventory.GetOwnedGrimoireEntryCardCount(selectedEntry)}/{selectedEntry.Cards.Count} owned | duplicates total {inventory.GetGrimoireDuplicateCount()}", 14, FontStyle.Bold, gold, 420, 22, 220f, 92f);
        CreateGrimoireCardSlotGrid(panel.transform, selectedEntry, 220f, 24f);
        Button previousEntry = CreateAnchoredButton(panel.transform, "<", 18, 54, 34, new Color(0.35f, 0.12f, 0.62f), 90f, -118f);
        previousEntry.interactable = selectedGrimoireEntryIndex > 0;
        previousEntry.onClick.AddListener(() => SelectAlbumDebugGrimoireEntry(selectedGrimoireEntryIndex - 1, returnToMap));
        Button nextEntry = CreateAnchoredButton(panel.transform, ">", 18, 54, 34, new Color(0.35f, 0.12f, 0.62f), 350f, -118f);
        nextEntry.interactable = selectedGrimoireEntryIndex < entries.Count - 1;
        nextEntry.onClick.AddListener(() => SelectAlbumDebugGrimoireEntry(selectedGrimoireEntryIndex + 1, returnToMap));

        CreateLibrarySection(panel.transform, "Book of Shadows Debug", 0f, -204f, 920, 150, new Color(0.11f, 0.055f, 0.18f));
        IReadOnlyList<BookOfShadowsSetDefinition> shadowSets = CardAlbumCatalog.BookOfShadowsSets;
        int activeShadowSetIndex = Mathf.Clamp(inventory.GetActiveBookOfShadowsSetNumber() - 1, 0, shadowSets.Count - 1);
        BookOfShadowsSetDefinition activeShadowSet = shadowSets[activeShadowSetIndex];
        CreateAnchoredText(panel.transform, $"{inventory.GetBookOfShadowsSeasonSummaryText()} | Purchased: {(inventory.BookOfShadowsPurchased ? "YES" : "NO")}", 16, FontStyle.Bold, cream, 820, 24, 0f, -170f);
        CreateAnchoredText(panel.transform, $"Active set owned: {inventory.GetOwnedBookOfShadowsSetCardCount(activeShadowSet)}/{CardAlbumCatalog.CountBookOfShadowsInSet(activeShadowSet)} | Total: {inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}", 15, FontStyle.Bold, gold, 820, 24, 0f, -204f);
        CreateAnchoredText(panel.transform, inventory.GetAlbumCompletionHookText(), 13, FontStyle.Bold, Color.white, 820, 48, 0f, -246f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), -140f, -310f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));
        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 170, 46, new Color(0.18f, 0.16f, 0.22f), 130f, -310f);
        close.onClick.AddListener(() => ReturnFromDevSettings(returnToMap));
    }

    private void SelectAlbumDebugGrimoireEntry(int entryIndex, bool returnToMap)
    {
        selectedGrimoireEntryIndex = Mathf.Clamp(entryIndex, 0, CardAlbumCatalog.GrimoireOneEntries.Count - 1);
        BuildAlbumDebugUi(returnToMap);
    }

    private string BuildProfileStateSummary()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Currency: {inventory.GetManaText()} mana | {inventory.GetCrystalText()} crystals | {inventory.GetPowerUpText()} power-ups");
        builder.AppendLine($"Account: Level {rewards.CurrentLevel} | {rewards.GetLevelProgressText()} | Aura Strength TBD | Rank thresholds TBD");
        builder.AppendLine($"Clairvoyance: stock {inventory.ClairvoyanceMinutes}m | active {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}");
        builder.AppendLine($"Wheelspins: {inventory.PendingJackpotSpins} pending | Cauldron {inventory.ManaCauldronAmount}/{inventory.ManaCauldronCapacity} | {inventory.GetManaCauldronCountdownText()} | +{inventory.GetManaCauldronHourlyRefillAmount()}/hr");
        builder.AppendLine($"Active: Realm {RealmContentCatalog.ActivePrototypeRealmIndex + 1} - {RealmContentCatalog.ActivePrototypeRealm.Name} | Room {RealmContentCatalog.ActivePrototypeRoomIndex + 1} - {RealmContentCatalog.ActivePrototypeRoom.Name}");
        builder.AppendLine();

        for (int realmIndex = 0; realmIndex < RealmContentCatalog.AllRealms.Count; realmIndex++)
        {
            RealmDefinition realm = RealmContentCatalog.AllRealms[realmIndex];
            int restoredRooms = GetRestoredRoomCount(realm);
            string realmState = IsRealmUnlocked(realmIndex) ? IsRealmComplete(realmIndex) ? "Complete" : "Unlocked" : "Locked";
            builder.AppendLine($"R{realmIndex + 1}: {realm.Name} - {realmState} - {restoredRooms}/{realm.Rooms.Count} rooms restored");
        }

        builder.AppendLine();
        builder.AppendLine("Current realm rooms:");
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            RoomDefinition room = rooms[roomIndex];
            string openState = IsRoomUnlocked(roomIndex) ? "Open" : "Locked";
            string restoreState = inventory.IsRoomRestored(room) ? "Restored" : inventory.CanRestoreRoom(room) ? "Ready" : "In progress";
            builder.AppendLine($"{roomIndex + 1}. {room.Name} - {openState}, {restoreState}");
            builder.AppendLine($"   {inventory.GetFullIngredientProgressText(room).Replace("\n", " | ")}");
        }

        return builder.ToString();
    }

    private int GetRestoredRoomCount(RealmDefinition realm)
    {
        int count = 0;
        IReadOnlyList<RoomDefinition> rooms = realm.Rooms;
        for (int index = 0; index < rooms.Count; index++)
        {
            if (inventory.IsRoomRestored(rooms[index]))
            {
                count++;
            }
        }

        return count;
    }

    private void RemoveDevSettingsModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DevSettingsModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RefreshDevSettingsUi(bool returnToMap)
    {
        ReturnFromDevSettings(returnToMap);
        BuildDevSettingsUi(returnToMap, devSettingsReturnToDen);
    }

    private void ReturnFromDevSettings(bool returnToMap)
    {
        if (returnToMap)
        {
            BuildWorldMapUi();
        }
        else if (devSettingsReturnToDen)
        {
            BuildPlayerDenUi();
        }
        else
        {
            BuildLobbyUi();
        }
    }

    private void BuildLobbyRestorePanel()
    {
        GameObject panel = CreateAnchoredPanel(contentRoot, "RestorePanel", new Color(0.96f, 0.86f, 0.62f), 980, 168, -245f, 118f);
        Color textColor = new Color(0.15f, 0.09f, 0.22f);
        CreateAnchoredText(panel.transform, RealmContentCatalog.ActivePrototypeRoom.PotionName, 22, FontStyle.Bold, textColor, 280, 30, -338f, 54f);
        CreateAnchoredText(panel.transform, "Collect ingredients while you play bingo.", 18, FontStyle.Bold, textColor, 300, 30, -338f, 22f);
        CreateAnchoredText(panel.transform, inventory.GetRestoreStatusText(), 21, FontStyle.Bold, inventory.CanRestoreActiveRoom() ? new Color(0.06f, 0.45f, 0.08f) : textColor, 280, 30, -338f, -12f);

        Button restore = CreateAnchoredButton(panel.transform, inventory.ActiveRoomRestored ? "Restored" : "Restore", 20, 220, 44, inventory.CanRestoreActiveRoom() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.35f, 0.12f, 0.62f), -338f, -56f);
        restore.interactable = inventory.CanRestoreActiveRoom();
        restore.onClick.AddListener(RestoreActiveRoom);

        CreateAnchoredText(panel.transform, "Ingredients", 19, FontStyle.Bold, textColor, 360, 26, 22f, 58f);
        CreateAnchoredText(panel.transform, inventory.GetFullIngredientProgressText(), 16, FontStyle.Bold, textColor, 420, 112, 22f, -14f);
        CreateAnchoredText(panel.transform, $"Restore Reward\n{inventory.GetRestoreRewardText()}", 18, FontStyle.Bold, textColor, 180, 118, 388f, 0f);
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
        CreateAnchoredText(tile.transform, $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}", 34, FontStyle.Bold, textColor, 300, 42, 0f, 86f);
        CreateAnchoredText(tile.transform, GetCardOptionFlavor(cardCount), 18, FontStyle.Bold, textColor, 290, 28, 0f, 48f);
        CreateAnchoredText(tile.transform, GetCardOptionIngredientText(cardCount), 18, FontStyle.Bold, textColor, 290, 30, 0f, 12f);
        CreateAnchoredText(tile.transform, GetCardOptionChanceText(cardCount), 16, FontStyle.Bold, textColor, 290, 48, 0f, -28f);

        string buttonLabel = canAfford ? $"Play {entryCost} Mana" : $"Need {entryCost} Mana";
        Button play = CreateAnchoredButton(tile.transform, buttonLabel, 20, 260, 48, canAfford ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.36f, 0.36f, 0.36f), 0f, -88f);
        play.interactable = canAfford;
        play.onClick.AddListener(() => BeginRoundFromLobby(cardCount));
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

        return $"Up to {GetMaxIngredientTypesForCardCount(cardCount)} Ingredients  |  {FormatPercent(GetActiveProgression().IngredientDropChance)} drop";
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
        calledNumberText = CreateTextInParent(rail.transform, calledHistory.Count > 0 ? $"{GetBingoLetter(calledHistory[0])}-{calledHistory[0]}" : "Starting", 29, FontStyle.Bold, new Color(1f, 0.84f, 0.18f), 116, 48);
        timerText = CreateTextInParent(rail.transform, "Next call: --", 15, FontStyle.Bold, new Color(0.74f, 0.92f, 1f), 116, 26);
        CreateTextInParent(rail.transform, "RECENT", 15, FontStyle.Bold, Color.white, 116, 24);
        calledHistoryText = CreateTextInParent(rail.transform, calledHistory.Count > 0 ? GetCalledHistoryText() : "none", 13, FontStyle.Normal, new Color(0.84f, 0.88f, 1f), 116, 86);
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
        lastRootSize = Vector2.zero;

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
        if (!force && rootSize == lastRootSize)
        {
            return;
        }

        lastRootSize = rootSize;

        float availableWidth = Mathf.Max(1f, rootSize.x - SafePadding * 2f);
        float availableHeight = Mathf.Max(1f, rootSize.y - SafePadding * 2f);
        float scale = Mathf.Min(availableWidth / activeStageWidth, availableHeight / activeStageHeight);
        scale = Mathf.Clamp(scale, 0.55f, 1.15f);

        stageRect.localScale = new Vector3(scale, scale, 1f);
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
            lobbyClairvoyanceStatusText.text = inventory.GetClairvoyanceRoomStatusText();
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

    private string GetDenManaCauldronButtonText()
    {
        return $"MANA CAULDRON\n{inventory.ManaCauldronAmount} / {inventory.ManaCauldronCapacity}\n+{inventory.GetManaCauldronHourlyRefillAmount()}/hr | {GetManaCauldronShortTimerText()}";
    }

    private string GetManaCauldronDetailStatusText()
    {
        if (inventory.ManaCauldronAmount >= inventory.ManaCauldronCapacity)
        {
            return $"Full - ready to collect\n+{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour";
        }

        string collectLine = inventory.ManaCauldronAmount > 0 ? "Collect available now" : "Still brewing...";
        return $"{collectLine}\n+{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour\n{inventory.GetManaCauldronCountdownText()}";
    }

    private string GetManaCauldronShortTimerText()
    {
        return inventory.ManaCauldronAmount >= inventory.ManaCauldronCapacity
            ? "Full"
            : $"Next {inventory.GetManaCauldronRemainingTimeText()}";
    }

    private string GetRoomPoolSummaryText()
    {
        if (IsBlackoutRoom())
        {
            return $"Blackouts {GetTotalBingos()}/{selectedCardCount}\nCalls {calledHistory.Count}/{GetActiveMaxBallCalls()}";
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
        pendingRewardPreview = null;
        lastJackpotEarnedCardIndex = -1;
        StopFinalBallCountdown();
        roundState.StartRound();
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
        calledAtTimes.Clear();
        powerUps.ClairvoyanceActive = inventory.HasActiveClairvoyance();
        powerUps.ResetRound();
        rewards.ResetRound();
        BuildCallPool();
        GenerateCards(selectedCardCount);
        cellRewards.GenerateForCards(playerCards, selectedCardCount, manaBetPerCard, GetActiveProgression());
        SelectNextBankPowerUp("Bank ready to charge.");
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
        if (roomText == null)
        {
            return;
        }

        RoomProgressionProfile progression = GetActiveProgression();
        roomText.text = $"{manaBetPerCard} Mana / Card\nRange {progression.MinManaBet}-{progression.MaxManaBet}";
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
            if (roundState.IsActive && !rewardPreviewShown)
            {
                SelectNextBankPowerUp("Next power-up charging");
            }

            return;
        }

        string key = $"{cardIndex}:{row}:{column}";
        if (!calledNumbers.Contains(value))
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
            int bingoDelta = newBingoCount - previousBingoCount;
            int claimedCount = IsBlackoutRoom() ? 1 : bingoDelta;
            int bingoBonus = claimedCount * GetScaledXp(roomRules.BingoClaimXp);
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
                roundState.MarkJackpotState();
                int jackpotStateXp = GetScaledXp(roomRules.JackpotStateXp);
                AddXp(jackpotStateXp, "Jackpot State");
                if (IsBlackoutRoom() && AllCardsReachedJackpot())
                {
                    StartBlackoutCompletionCountdown();
                    return;
                }

                if (!IsBlackoutRoom() && IsPrototypeBingoPoolExhausted())
                {
                    StartRoomPoolCountdown($"Room bingo pool exhausted. Jackpot reached on Card {cardIndex + 1}.");
                    return;
                }

                if (bingoBannerText != null)
                {
                    bingoBannerText.text = GetBingoBannerText();
                }

                statusText.text = $"{earnedLabel}! Card {cardIndex + 1} {reachedLabel}. +{daubXp + bingoBonus + jackpotStateXp} XP.{collectedRewardText}{placedPowerUpText}";
                return;
            }

            if (!IsBlackoutRoom() && IsPrototypeBingoPoolExhausted())
            {
                StartRoomPoolCountdown($"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.");
                return;
            }

            statusText.text = IsBlackoutRoom()
                ? $"Card {cardIndex + 1} blackout complete. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}"
                : $"Card {cardIndex + 1} auto-claimed {newBingoCount}/{BingoRoomRules.JackpotBingosPerCard}. +{daubXp + bingoBonus} XP.{collectedRewardText}{placedPowerUpText}";
            TryStartStandardCoverageCountdown($"Standard room coverage limit reached after Card {cardIndex + 1} bingo.");
            return;
        }

        statusText.text = IsBlackoutRoom()
            ? $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. Blackout {playerCards.CountMarkedPlayable(cardIndex)}/{BingoRoomRules.BlackoutPlayableSquares}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}"
            : $"{GetBingoLetter(value)}-{value} daubed on Card {cardIndex + 1}. +{daubXp} XP.{collectedRewardText}{placedPowerUpText}";

        if (!IsBlackoutRoom() && IsPrototypeBingoPoolExhausted())
        {
            StartRoomPoolCountdown($"Room bingo pool exhausted at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.");
            return;
        }

        TryStartStandardCoverageCountdown($"Standard room coverage limit reached after {GetBingoLetter(value)}-{value}.");
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
        if (!calledAtTimes.TryGetValue(value, out float calledAt))
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
        if (finalBallCountdownActive)
        {
            return;
        }

        if (timerText != null)
        {
            timerText.text = roundState.IsActive ? $"{Mathf.Max(0f, roundState.NextCallTimer):0.0}s" : "Stopped";
        }

        if (!roundState.TickAutoCallTimer(Time.deltaTime))
        {
            return;
        }

        CallNumber();
    }

    private void CallNumber()
    {
        if (!caller.TryCallNext(out int calledNumber))
        {
            StartFinalBallCountdown("No more numbers to call.");
            return;
        }

        calledHistory.Clear();
        calledHistory.AddRange(caller.History);
        calledNumbers.Clear();
        foreach (int number in caller.CalledNumbers)
        {
            calledNumbers.Add(number);
        }

        calledAtTimes[calledNumber] = Time.time;
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
            ballsLeftText.text = Mathf.Max(0, GetActiveMaxBallCalls() - calledHistory.Count).ToString();
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

        if (!IsBlackoutRoom() && IsPrototypeBingoPoolExhausted())
        {
            StartRoomPoolCountdown($"Room bingo pool exhausted. You claimed {GetTotalBingos()}, simulated players claimed {rewards.SimulatedBingosClaimed}.");
            return;
        }

        if (TryStartStandardCoverageCountdown("Standard room coverage limit reached."))
        {
            return;
        }

        if (calledHistory.Count >= GetActiveMaxBallCalls())
        {
            StartFinalBallCountdown("Max balls reached.");
        }
    }

    private void CompleteRound(string reason)
    {
        finalBallCountdownActive = false;
        finalBallCountdownRoutine = null;
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        roundState.StopRound();

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
        if (statusText != null)
        {
            statusText.text = preview.GetStatusLine();
        }

        ShowRewardPreview(preview);
    }

    private void StartFinalBallCountdown(string reason)
    {
        StartRoundEndCountdown(reason, "Final ball called. Daub anything you missed.");
    }

    private void StartBlackoutCompletionCountdown()
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        ShowPowerUpBurst("BLACKOUT COMPLETE");
        StartRoundEndCountdown("Every card reached blackout.", "All active cards reached blackout.");
    }

    private void StartRoomPoolCountdown(string reason)
    {
        RefreshRoomPoolTexts();
        if (bingoBannerText != null)
        {
            bingoBannerText.text = GetBingoBannerText();
        }

        StartRoundEndCountdown(reason, "Room bingo pool exhausted.");
    }

    private void StartRoundEndCountdown(string reason, string countdownMessage)
    {
        if (finalBallCountdownActive || rewardPreviewShown)
        {
            return;
        }

        finalBallCountdownActive = true;
        finalBallCountdownRoutine = StartCoroutine(RoundEndCountdownRoutine(reason, countdownMessage));
    }

    private IEnumerator RoundEndCountdownRoutine(string reason, string countdownMessage)
    {
        for (int seconds = 5; seconds >= 1; seconds--)
        {
            if (rewardPreviewShown)
            {
                finalBallCountdownActive = false;
                finalBallCountdownRoutine = null;
                yield break;
            }

            if (timerText != null)
            {
                timerText.text = $"END {seconds}";
            }

            if (calledNumberText != null)
            {
                calledNumberText.fontSize = 30;
                calledNumberText.text = calledHistory.Count > 0
                    ? $"{GetBingoLetter(calledHistory[0])}-{calledHistory[0]}\nEND {seconds}"
                    : $"END {seconds}";
            }

            if (ballsLeftText != null)
            {
                ballsLeftText.text = seconds.ToString();
            }

            if (gameplayPowerUpBurstText != null)
            {
                gameplayPowerUpBurstText.text = $"ROUND ENDS IN {seconds}";
                gameplayPowerUpBurstText.color = new Color(0.72f, 0.95f, 1f, 1f);
                gameplayPowerUpBurstText.gameObject.SetActive(true);
            }

            if (statusText != null)
            {
                statusText.text = $"{countdownMessage} Round ends in {seconds}.";
            }

            yield return new WaitForSeconds(1f);
        }

        finalBallCountdownActive = false;
        finalBallCountdownRoutine = null;
        if (gameplayPowerUpBurstText != null)
        {
            gameplayPowerUpBurstText.gameObject.SetActive(false);
        }

        if (!rewardPreviewShown)
        {
            CompleteRound(reason);
        }
    }

    private void StopFinalBallCountdown()
    {
        if (finalBallCountdownRoutine != null)
        {
            StopCoroutine(finalBallCountdownRoutine);
        }

        finalBallCountdownRoutine = null;
        finalBallCountdownActive = false;
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
        if (IsBlackoutRoom() || playerCards.Count == 0)
        {
            return false;
        }

        int totalMarked = 0;
        for (int index = 0; index < playerCards.Count; index++)
        {
            totalMarked += playerCards.CountMarkedPlayable(index);
        }

        float averageMarked = (float)totalMarked / playerCards.Count;
        return averageMarked >= GetStandardCoverageLimit();
    }

    private int GetStandardCoverageLimit()
    {
        if (selectedCardCount <= 1) return 17;
        if (selectedCardCount <= 2) return 16;
        if (selectedCardCount <= 4) return 15;
        return 14;
    }

    private bool TryStartStandardCoverageCountdown(string reason)
    {
        if (!IsBlackoutRoom() && IsStandardCoverageLimitReached())
        {
            StartFinalBallCountdown(reason);
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
        CreateAnchoredText(panel.transform, "ROUND COMPLETE", 42, FontStyle.Bold, ink, 900, 58, 0f, 210f);
        string roundResultText = IsBlackoutRoom()
            ? $"{preview.JackpotCards} {(preview.JackpotCards == 1 ? "BLACKOUT" : "BLACKOUTS")}"
            : $"{preview.PlayerBingos} {(preview.PlayerBingos == 1 ? "BINGO" : "BINGOS")}";
        CreateAnchoredText(panel.transform, roundResultText, 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 360, 46, 0f, 160f);

        CreateRoundSummaryStat(panel.transform, "MANA EARNED", preview.ManaReward.ToString("N0"), "*", -312f, 100f);
        CreateRoundSummaryStat(panel.transform, "XP EARNED", preview.PlayerXp.ToString("N0"), "XP", 0f, 100f);
        CreateRoundSummaryStat(panel.transform, preview.EndLevel > preview.StartLevel ? "LEVEL UP" : $"LEVEL {preview.EndLevel}", BuildRoundRankSummary(preview), "", 312f, 100f);

        CreateAnchoredText(panel.transform, "COLLECTED REWARDS", 22, FontStyle.Bold, ink, 820, 30, 0f, 36f);
        BuildRoundCollectedRewards(panel.transform, preview);

        CreateAnchoredText(panel.transform, "POTION PROGRESS", 22, FontStyle.Bold, ink, 820, 30, 0f, -106f);
        BuildRoundIngredientProgress(panel.transform, preview);

        Button redeem = CreateAnchoredButton(panel.transform, "Collect", 22, 260, 50, new Color(0.12f, 0.55f, 0.08f), 0f, -220f);
        redeem.onClick.AddListener(RedeemRewardsAndReturnHome);
    }

    private void CreateRoundSummaryStat(Transform parent, string label, string value, string icon, float x, float y)
    {
        GameObject tile = CreateAnchoredPanel(parent, label, new Color(0.99f, 0.91f, 0.72f), 280, 94, x, y);
        Color ink = new Color(0.15f, 0.08f, 0.24f);
        CreateAnchoredText(tile.transform, label, 17, FontStyle.Bold, ink, 240, 24, 0f, 28f);
        string body = string.IsNullOrEmpty(icon) ? value : $"{icon}  {value}";
        CreateAnchoredText(tile.transform, body, value.Length > 12 ? 21 : 34, FontStyle.Bold, ink, 250, 46, 0f, -12f);
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
        CreateAnchoredText(panel.transform, "ADDED TO GRIMOIRE", 44, FontStyle.Bold, gold, 980, 58, 0f, 286f);
        CreateAnchoredText(panel.transform, "Open the Library to find NEW cards marked in their potion set.", 19, FontStyle.Bold, new Color(0.86f, 0.84f, 0.94f), 920, 30, 0f, 238f);

        int visibleCount = Mathf.Min(cards.Count, 5);
        float startX = -(visibleCount - 1) * 126f;
        for (int index = 0; index < visibleCount; index++)
        {
            AwardedAlbumCardRecord card = cards[index];
            float x = startX + index * 252f;
            CreateCardRevealTile(panel.transform, card, x, 38f);
        }

        if (cards.Count > visibleCount)
        {
            CreateAnchoredText(panel.transform, $"+{cards.Count - visibleCount} more card wins", 20, FontStyle.Bold, Color.white, 360, 34, 0f, -182f);
        }

        Button continueButton = CreateAnchoredButton(panel.transform, "Continue", 24, 280, 56, new Color(0.12f, 0.55f, 0.08f), 0f, -266f);
        continueButton.onClick.AddListener(ContinueAfterCardReveal);
    }

    private void CreateCardRevealTile(Transform parent, AwardedAlbumCardRecord card, float x, float y)
    {
        Color cardColor = GetRevealCardColor(card.RarityLabel);
        GameObject tile = CreateAnchoredPanel(parent, $"CardReveal_{card.CardName}", cardColor, 210, 300, x, y);
        CreateAnchoredText(tile.transform, BuildStarText(card.Stars), 30, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 180, 40, 0f, 104f);
        CreateAnchoredText(tile.transform, card.CardName, 24, FontStyle.Bold, Color.white, 176, 86, 0f, 32f);
        CreateAnchoredText(tile.transform, card.RarityLabel, 18, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 160, 28, 0f, -58f);
        CreateAnchoredText(tile.transform, card.PotionName, 13, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 174, 46, 0f, -102f);

        if (card.IsNew)
        {
            CreateNewBadge(tile.transform, 58f, 132f, 82, 30, 14);
        }
        else if (card.Copies > 1)
        {
            CreateAnchoredText(tile.transform, $"Duplicate x{card.Copies}", 16, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 160, 26, 0f, 132f);
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
            CreateAnchoredText(tile.transform, GetRewardTileIcon(rewardsList[index]), 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 120, 30, 0f, 18f);
            CreateAnchoredText(tile.transform, rewardsList[index], 13, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), maxTiles > 5 ? 118 : 134, 44, 0f, -20f);
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
        CreateAnchoredText(parent, RealmContentCatalog.ActivePrototypeRoom.PotionName, 18, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 360, 26, -260f, -140f);
        CreateAnchoredText(parent, preview.IngredientDrops.Count == 0 ? "No ingredients added this round" : $"{preview.IngredientDrops.Count} ingredient types added this round", 16, FontStyle.Bold, new Color(0.16f, 0.38f, 0.08f), 420, 24, -230f, -166f);

        for (int index = 0; index < requirements.Count; index++)
        {
            IngredientRequirement requirement = requirements[index];
            int earned = GetIngredientDropQuantity(preview, requirement.Name);
            float x = -6f + index * 132f;
            GameObject tile = CreateAnchoredPanel(parent, $"IngredientTile_{index}", earned > 0 ? new Color(1f, 0.9f, 0.58f) : new Color(0.91f, 0.82f, 0.62f), 112, 78, x, -154f);
            CreateAnchoredText(tile.transform, earned > 0 ? $"+{earned}" : "-", 21, FontStyle.Bold, earned > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.45f, 0.4f, 0.36f), 80, 28, 0f, 16f);
            CreateAnchoredText(tile.transform, GetShortRoundIngredientName(requirement.Name), 12, FontStyle.Bold, new Color(0.15f, 0.08f, 0.24f), 96, 36, 0f, -18f);
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

    private void BuildJackpotSpinUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundState.StopRound();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.08f, 0.02f, 0.14f, 0.98f), 0, 0);
        layout.enabled = false;

        CreateAnchoredPanel(contentRoot, "JackpotHeader", new Color(0.09f, 0.04f, 0.15f), 1440, 84, 0f, 370f);
        CreateAnchoredText(contentRoot, "Jackpot Wheelspin Earned!", 46, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 900, 62, 110f, 382f);
        CreateAnchoredText(contentRoot, "Your bingo win unlocked a chance at the Grand Jackpot.", 20, FontStyle.Bold, Color.white, 900, 30, 110f, 338f);
        CreateLobbyResourceTile(contentRoot, "Mana", inventory.GetManaText(), "*", 220, -570f, 370f);

        CreateAnchoredText(contentRoot, "v", 46, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 80, 56, -390f, 298f);
        GameObject wheel = CreateAnchoredPanel(contentRoot, "JackpotWheel", new Color(0.2f, 0.05f, 0.32f), 560, 560, -390f, 38f);
        BuildJackpotWheelSegments(wheel.transform);
        bool hasStackToCollect = jackpotWheelCollectedMana > 0 && jackpotWheelCollectedResults.Count > 0;
        bool canSpinWheel = inventory.PendingJackpotSpins > 0 && !jackpotWheelAnimating;
        bool canBigCollect = hasStackToCollect && inventory.PendingJackpotSpins <= 0 && !jackpotWheelAnimating;
        bool canUseCenterButton = canSpinWheel || canBigCollect;
        string centerButtonLabel = jackpotWheelAnimating ? "SPINNING" : canSpinWheel ? "SPIN" : "COLLECT";
        Color centerButtonColor = canBigCollect
            ? new Color(0.86f, 0.55f, 0.06f)
            : canSpinWheel ? new Color(0.08f, 0.48f, 0.12f) : new Color(0.35f, 0.35f, 0.35f);
        Button centerSpin = CreateAnchoredButton(wheel.transform, centerButtonLabel, jackpotWheelAnimating ? 22 : 34, 178, 178, centerButtonColor, 0f, 0f);
        centerSpin.interactable = canUseCenterButton;
        centerSpin.onClick.AddListener(() =>
        {
            if (inventory.PendingJackpotSpins <= 0 && jackpotWheelCollectedMana > 0)
            {
                BigCollectJackpotWheelStack();
                return;
            }

            SpinJackpotWheel();
        });

        BuildJackpotPotPanel();
        CreateAnchoredText(contentRoot, RealmContentCatalog.ActivePrototypeRoom.Name, 28, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 620, 42, 350f, 274f);
        string spinStatus = jackpotWheelAnimating ? "Wheel spinning..." : canBigCollect ? "Ready to collect" : $"Spins Left: {inventory.PendingJackpotSpins}";
        CreateAnchoredText(contentRoot, spinStatus, 24, FontStyle.Bold, canBigCollect ? new Color(1f, 0.9f, 0.32f) : new Color(0.8f, 1f, 0.55f), 420, 38, -390f, -238f);
        BuildJackpotCollectedStackPanel();
        BuildJackpotCollectionConfirmationPanel();

        string result = string.IsNullOrEmpty(jackpotSpinResultText)
            ? "Spin each wheelspin, stack the rewards, then collect once."
            : jackpotSpinResultText;
        CreateAnchoredText(contentRoot, result, 26, FontStyle.Bold, Color.white, 720, 58, 350f, -184f);
        BuildJackpotResultPanel();

        string collectLabel = canBigCollect ? "Collect" : canSpinWheel && hasStackToCollect ? "Keep Spinning" : "Return Home";
        Button collect = CreateAnchoredButton(contentRoot, collectLabel, 22, 220, 56, canBigCollect ? new Color(0.86f, 0.55f, 0.06f) : new Color(0.35f, 0.12f, 0.62f), 350f, -332f);
        collect.interactable = !jackpotWheelAnimating && (!hasStackToCollect || canBigCollect);
        collect.onClick.AddListener(() =>
        {
            if (inventory.PendingJackpotSpins <= 0 && jackpotWheelCollectedMana > 0)
            {
                BigCollectJackpotWheelStack();
                return;
            }

            ReturnFromJackpotWheel();
        });

        ApplyStageScale(true);
    }

    private void SpinJackpotWheel()
    {
        if (jackpotWheelAnimating)
        {
            return;
        }

        if (inventory.PendingJackpotSpins <= 0)
        {
            jackpotSpinResultText = "No jackpot spins are available.";
            BuildJackpotSpinUi();
            return;
        }

        lastJackpotSpinResult = null;

        if (!inventory.TryConsumeJackpotSpin())
        {
            jackpotSpinResultText = "No jackpot spins are available.";
            BuildJackpotSpinUi();
            return;
        }

        jackpotWheelCollectionConfirmed = false;
        jackpotWheelTargetSegment = GetJackpotWheelSegmentForRoll(Random.Range(0, 100));
        pendingJackpotSpinResult = ResolveJackpotSpinReward(jackpotWheelTargetSegment);
        jackpotWheelAnimating = true;
        jackpotWheelAnimationSequence++;
        int animationSequence = jackpotWheelAnimationSequence;
        jackpotSpinResultText = "Wheel spinning...";
        BuildJackpotSpinUi();
        StartCoroutine(AnimateJackpotWheelSpin(animationSequence));
    }

    private IEnumerator AnimateJackpotWheelSpin(int animationSequence)
    {
        float duration = 1.7f;
        float elapsed = 0f;
        float startRotation = jackpotWheelRotation;
        float targetRotation = startRotation + 1080f + GetJackpotWheelDeltaToTarget(startRotation, jackpotWheelTargetSegment);

        while (elapsed < duration)
        {
            elapsed += 0.035f;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            jackpotWheelRotation = Mathf.Lerp(startRotation, targetRotation, eased);
            UpdateJackpotWheelSegmentTransforms();
            yield return new WaitForSeconds(0.035f);
        }

        if (animationSequence != jackpotWheelAnimationSequence)
        {
            yield break;
        }

        jackpotWheelRotation = targetRotation;
        jackpotWheelAnimating = false;
        lastJackpotSpinResult = pendingJackpotSpinResult;
        pendingJackpotSpinResult = null;

        if (lastJackpotSpinResult != null)
        {
            jackpotWheelCollectedResults.Add(lastJackpotSpinResult);
            jackpotWheelCollectedMana += lastJackpotSpinResult.ManaAward;
            jackpotSpinResultText = inventory.PendingJackpotSpins > 0
                ? $"{lastJackpotSpinResult.Label} stacked +{lastJackpotSpinResult.ManaAward:N0}. Spin {inventory.PendingJackpotSpins} more, then collect."
                : $"All wheelspins stacked. Collect +{jackpotWheelCollectedMana:N0}.";
        }

        BuildJackpotSpinUi();
    }

    private float GetJackpotWheelRestingRotation(int targetSegment)
    {
        float angleStep = 360f / GetJackpotWheelSegmentCount();
        return Mathf.Repeat(-(targetSegment * angleStep), 360f);
    }

    private float GetJackpotWheelDeltaToTarget(float startRotation, int targetSegment)
    {
        float targetMod = GetJackpotWheelRestingRotation(targetSegment);
        float currentMod = Mathf.Repeat(startRotation, 360f);
        return Mathf.Repeat(targetMod - currentMod, 360f);
    }

    private int GetJackpotWheelSegmentForRoll(int roll)
    {
        if (roll < 18)
        {
            return 0;
        }

        if (roll < 32)
        {
            return 1;
        }

        if (roll < 46)
        {
            return 2;
        }

        if (roll < 58)
        {
            return 3;
        }

        if (roll < 69)
        {
            return 4;
        }

        if (roll < 79)
        {
            return 5;
        }

        if (roll < 87)
        {
            return 6;
        }

        if (roll < 94)
        {
            return 7;
        }

        if (roll < 97)
        {
            return 8;
        }

        if (roll < 99)
        {
            return 9;
        }

        return 10;
    }

    private JackpotSpinResult ResolveJackpotSpinReward(int segmentIndex)
    {
        if (segmentIndex < 8)
        {
            int standardValue = GetJackpotWheelStandardValue(segmentIndex);
            return new JackpotSpinResult("STANDARD", standardValue, false);
        }

        if (segmentIndex == 8)
        {
            int jackpotValue = inventory.GetJackpotValue();
            return new JackpotSpinResult("JACKPOT", jackpotValue, true);
        }

        if (segmentIndex == 9)
        {
            int epicValue = inventory.GetEpicValue();
            return new JackpotSpinResult("EPIC", epicValue, true);
        }

        int legendaryValue = inventory.GetLegendaryValue();
        return new JackpotSpinResult("LEGENDARY", legendaryValue, true);
    }

    private void BigCollectJackpotWheelStack()
    {
        if (jackpotWheelCollectedMana <= 0 || jackpotWheelCollectedResults.Count == 0)
        {
            ReturnFromJackpotWheel();
            return;
        }

        int collectedMana = jackpotWheelCollectedMana;
        bool shouldResetPot = false;
        for (int index = 0; index < jackpotWheelCollectedResults.Count; index++)
        {
            shouldResetPot = shouldResetPot || jackpotWheelCollectedResults[index].ResetPot;
        }

        if (shouldResetPot)
        {
            inventory.GrantJackpotSpecialMana(collectedMana);
        }
        else
        {
            inventory.GrantJackpotMana(collectedMana);
        }

        jackpotWheelLastCollectedMana = collectedMana;
        jackpotWheelLastCollectResetPot = shouldResetPot;
        jackpotWheelCollectionConfirmed = true;
        jackpotSpinResultText = $"Collected +{collectedMana:N0} to profile mana.";
        jackpotWheelCollectedMana = 0;
        jackpotWheelCollectedResults.Clear();
        lastJackpotSpinResult = null;
        pendingJackpotSpinResult = null;
        jackpotWheelAnimating = false;
        jackpotWheelAnimationSequence++;
        BuildJackpotSpinUi();
    }

    private void ReturnFromJackpotWheel()
    {
        jackpotWheelAnimationSequence++;
        jackpotSpinResultText = "";
        jackpotWheelCollectedMana = 0;
        jackpotWheelLastCollectedMana = 0;
        jackpotWheelLastCollectResetPot = false;
        jackpotWheelCollectionConfirmed = false;
        jackpotWheelCollectedResults.Clear();
        lastJackpotSpinResult = null;
        pendingJackpotSpinResult = null;
        jackpotWheelAnimating = false;
        BuildLobbyUi();
    }

    private void BuildJackpotWheelSegments(Transform wheel)
    {
        jackpotWheelSegmentRects.Clear();
        Color[] colors =
        {
            new Color(0.07f, 0.2f, 0.72f),
            new Color(0.78f, 0.15f, 0.58f),
            new Color(0.94f, 0.52f, 0.05f),
            new Color(0.06f, 0.58f, 0.18f),
            new Color(0.08f, 0.55f, 0.8f),
            new Color(0.96f, 0.8f, 0.08f)
        };

        int segmentCount = GetJackpotWheelSegmentCount();
        float angleStep = 360f / segmentCount;
        for (int index = 0; index < segmentCount; index++)
        {
            float angle = jackpotWheelRotation + index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians) * 190f;
            float y = Mathf.Cos(radians) * 190f;
            bool special = index >= 8;
            GameObject segment = CreateAnchoredPanel(wheel, $"WheelSegment_{index}", special ? GetJackpotSpecialColor(index) : colors[index % colors.Length], special ? 150 : 136, 82, x, y);
            jackpotWheelSegmentRects.Add(segment.GetComponent<RectTransform>());
            segment.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
            string label = GetJackpotWheelSegmentLabel(index);
            CreateAnchoredText(segment.transform, label, special ? 21 : 26, FontStyle.Bold, Color.white, special ? 138 : 118, 46, 0f, special ? 0f : 8f);
            if (!special)
            {
                CreateAnchoredText(segment.transform, "*", 18, FontStyle.Bold, new Color(1f, 0.86f, 0.16f), 44, 26, 0f, -22f);
            }
        }
    }

    private void UpdateJackpotWheelSegmentTransforms()
    {
        int segmentCount = Mathf.Min(GetJackpotWheelSegmentCount(), jackpotWheelSegmentRects.Count);
        if (segmentCount == 0)
        {
            return;
        }

        float angleStep = 360f / GetJackpotWheelSegmentCount();
        for (int index = 0; index < segmentCount; index++)
        {
            RectTransform segment = jackpotWheelSegmentRects[index];
            if (segment == null)
            {
                continue;
            }

            float angle = jackpotWheelRotation + index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            segment.anchoredPosition = new Vector2(Mathf.Sin(radians) * 190f, Mathf.Cos(radians) * 190f);
            segment.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }

    private void BuildJackpotPotPanel()
    {
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotPotPanel", new Color(0.15f, 0.06f, 0.22f), 520, 390, 350f, 38f);
        CreateAnchoredText(panel.transform, "CURRENT POT", 25, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 460, 40, 0f, 142f);
        CreateAnchoredText(panel.transform, inventory.GetCurrentJackpotPotText(), 70, FontStyle.Bold, Color.white, 340, 82, -26f, 82f);
        CreateAnchoredText(panel.transform, "*", 52, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 70, 70, 168f, 82f);
        CreateJackpotTierRow(panel.transform, "JACKPOT", inventory.GetJackpotValue(), -2f, new Color(0.58f, 0.18f, 0.85f));
        CreateJackpotTierRow(panel.transform, "EPIC", inventory.GetEpicValue(), -84f, new Color(0.72f, 0.12f, 0.78f));
        CreateJackpotTierRow(panel.transform, "LEGENDARY", inventory.GetLegendaryValue(), -166f, new Color(0.92f, 0.62f, 0.06f));
    }

    private void BuildJackpotResultPanel()
    {
        if (lastJackpotSpinResult == null)
        {
            return;
        }

        Color accent = GetJackpotResultAccent(lastJackpotSpinResult);
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotResultPanel", new Color(0.1f, 0.04f, 0.16f), 520, 132, 350f, -252f);
        CreateAnchoredText(panel.transform, GetJackpotResultEyebrow(lastJackpotSpinResult), 15, FontStyle.Bold, new Color(0.82f, 0.9f, 1f), 460, 24, 0f, 48f);
        CreateAnchoredText(panel.transform, lastJackpotSpinResult.Label, 30, FontStyle.Bold, accent, 240, 40, -116f, 14f);
        CreateAnchoredText(panel.transform, $"+{lastJackpotSpinResult.ManaAward:N0}", 38, FontStyle.Bold, Color.white, 170, 46, 96f, 14f);
        CreateAnchoredText(panel.transform, "*", 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 190f, 14f);
        CreateAnchoredText(panel.transform, GetJackpotResultDescription(lastJackpotSpinResult), 17, FontStyle.Bold, new Color(0.82f, 0.9f, 1f), 460, 30, 0f, -40f);
    }

    private void BuildJackpotCollectionConfirmationPanel()
    {
        if (!jackpotWheelCollectionConfirmed || jackpotWheelLastCollectedMana <= 0)
        {
            return;
        }

        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotCollectConfirmation", new Color(0.08f, 0.24f, 0.1f), 520, 122, 350f, -252f);
        CreateAnchoredText(panel.transform, "COLLECTED", 18, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 460, 28, 0f, 38f);
        CreateAnchoredText(panel.transform, $"+{jackpotWheelLastCollectedMana:N0}", 42, FontStyle.Bold, Color.white, 180, 52, -68f, 2f);
        CreateAnchoredText(panel.transform, "*", 32, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 44f, 2f);
        CreateAnchoredText(panel.transform, jackpotWheelLastCollectResetPot ? "Special tier collected. Pot reset to room minimum." : "Standard rewards collected. Pot carries forward.", 17, FontStyle.Bold, new Color(0.84f, 0.92f, 1f), 470, 30, 0f, -42f);
    }

    private void BuildJackpotCollectedStackPanel()
    {
        bool readyToCollect = jackpotWheelCollectedMana > 0 && inventory.PendingJackpotSpins <= 0;
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotCollectedStack", readyToCollect ? new Color(0.2f, 0.1f, 0.03f) : new Color(0.12f, 0.04f, 0.18f), 500, 136, -390f, -360f);
        CreateAnchoredText(panel.transform, readyToCollect ? "REWARD STACK READY" : "WHEEL REWARD STACK", 18, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 460, 26, 0f, 50f);
        CreateAnchoredText(panel.transform, $"+{jackpotWheelCollectedMana:N0}", 38, FontStyle.Bold, Color.white, 180, 48, -142f, 12f);
        CreateAnchoredText(panel.transform, "*", 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 40, 40, -36f, 12f);

        string stackText = GetJackpotCollectedStackText();
        CreateAnchoredText(panel.transform, stackText, 15, FontStyle.Bold, new Color(0.86f, 0.9f, 1f), 240, 86, 116f, -8f);
    }

    private string GetJackpotCollectedStackText()
    {
        if (jackpotWheelCollectedResults.Count == 0)
        {
            return "Spin to build the reward stack.";
        }

        int firstVisible = Mathf.Max(0, jackpotWheelCollectedResults.Count - 5);
        string text = "";
        if (firstVisible > 0)
        {
            text = $"+{firstVisible} earlier";
        }

        for (int index = firstVisible; index < jackpotWheelCollectedResults.Count; index++)
        {
            JackpotSpinResult result = jackpotWheelCollectedResults[index];
            if (text.Length > 0)
            {
                text += "\n";
            }

            text += $"{index + 1}. {result.Label} +{result.ManaAward:N0}";
        }

        return text;
    }

    private string GetJackpotResultEyebrow(JackpotSpinResult result)
    {
        if (result == null)
        {
            return "LATEST SPIN";
        }

        if (result.Label == "LEGENDARY")
        {
            return "LEGENDARY HIT";
        }

        if (result.Label == "EPIC")
        {
            return "EPIC HIT";
        }

        if (result.Label == "JACKPOT")
        {
            return "JACKPOT HIT";
        }

        return "LATEST SPIN";
    }

    private string GetJackpotResultDescription(JackpotSpinResult result)
    {
        if (result == null)
        {
            return "";
        }

        if (result.Label == "LEGENDARY")
        {
            return "Legendary tier: 3x pot and pot resets on collect";
        }

        if (result.Label == "EPIC")
        {
            return "Epic tier: 2x pot and pot resets on collect";
        }

        if (result.Label == "JACKPOT")
        {
            return "Jackpot tier: 1x pot and pot resets on collect";
        }

        return "Standard tier: pot carries forward";
    }

    private Color GetJackpotResultAccent(JackpotSpinResult result)
    {
        if (result == null)
        {
            return Color.white;
        }

        if (result.Label == "LEGENDARY")
        {
            return new Color(1f, 0.74f, 0.12f);
        }

        if (result.Label == "EPIC")
        {
            return new Color(0.94f, 0.28f, 1f);
        }

        if (result.Label == "JACKPOT")
        {
            return new Color(1f, 0.9f, 0.32f);
        }

        return Color.white;
    }

    private void CreateJackpotTierRow(Transform parent, string label, int value, float y, Color color)
    {
        GameObject row = CreateAnchoredPanel(parent, label, new Color(color.r * 0.35f, color.g * 0.25f, color.b * 0.45f, 1f), 440, 58, 0f, y);
        CreateAnchoredText(row.transform, label, 24, FontStyle.Bold, color, 190, 42, -104f, 0f);
        CreateAnchoredText(row.transform, value.ToString("N0"), 33, FontStyle.Bold, Color.white, 150, 44, 82f, 0f);
        CreateAnchoredText(row.transform, "*", 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 180f, 0f);
    }

    private int GetJackpotWheelSegmentCount()
    {
        return 11;
    }

    private string GetJackpotWheelSegmentLabel(int segmentIndex)
    {
        if (segmentIndex < 8)
        {
            return GetJackpotWheelStandardValue(segmentIndex).ToString("N0");
        }

        if (segmentIndex == 8)
        {
            return "JACKPOT";
        }

        if (segmentIndex == 9)
        {
            return "EPIC";
        }

        return "LEGENDARY";
    }

    private int GetJackpotWheelStandardValue(int segmentIndex)
    {
        float[] multipliers = { 0.2f, 0.3f, 0.4f, 0.6f, 0.8f, 1f, 1.25f, 1.5f };
        return inventory.GetWheelStandardValue(multipliers[Mathf.Clamp(segmentIndex, 0, multipliers.Length - 1)]);
    }

    private Color GetJackpotSpecialColor(int segmentIndex)
    {
        if (segmentIndex == 8)
        {
            return new Color(0.48f, 0.06f, 0.56f);
        }

        if (segmentIndex == 9)
        {
            return new Color(0.66f, 0.08f, 0.82f);
        }

        return new Color(0.96f, 0.62f, 0.04f);
    }

    private void ResetPrototypeSave()
    {
        ResetPrototypeSave(true);
    }

    private void ResetPrototypeSave(bool returnToMap)
    {
        pendingRewardPreview = null;
        rewardPreviewShown = false;
        roundState.StopRound();
        rewards.ResetRound();
        rewards.ResetSavedXp();
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        inventory.ResetToDefaults();
        ResetPrototypeFriendsState();
        prototypeRequestedCovenNames.Clear();
        SavePrototypeCovenDiscoveryState();
        ResetPrototypeTrailState();
        if (profileSettingsPersistence != null)
        {
            ApplyProfileSettings(profileSettingsPersistence.ResetProfileIdentityPreservingPreferences(profileSettingsState));
        }
        else
        {
            bool soundEnabled = prototypeSoundEnabled;
            bool notificationsEnabled = prototypeNotificationsEnabled;
            ApplyProfileSettings(ProfileSettingsState.CreateDefault());
            prototypeSoundEnabled = soundEnabled;
            prototypeNotificationsEnabled = notificationsEnabled;
            profileSettingsState.SoundEnabled = soundEnabled;
            profileSettingsState.NotificationsEnabled = notificationsEnabled;
        }
        ReturnFromDevSettings(returnToMap);
    }

    private void ResetRoomProgressKeepingInventory(bool returnToMap)
    {
        pendingRewardPreview = null;
        rewardPreviewShown = false;
        roundState.StopRound();
        rewards.ResetRound();
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        inventory.ResetRoomProgressKeepingInventory();
        ReturnFromDevSettings(returnToMap);
    }

    private void UnlockNextRealmForTesting(bool returnToMap)
    {
        int nextRealmIndex = Mathf.Min(RealmContentCatalog.ActivePrototypeRealmIndex + 1, RealmContentCatalog.AllRealms.Count - 1);
        inventory.UnlockRealmForTesting(nextRealmIndex);
        ReturnFromDevSettings(returnToMap);
    }

    private void GrantPrototypeClairvoyance()
    {
        GrantPrototypeClairvoyance(false);
    }

    private void GrantPrototypeClairvoyance(bool returnToMap)
    {
        inventory.AddClairvoyanceMinutes(15);
        RefreshPowerUpDisplays();
        RefreshDevSettingsUi(returnToMap);
    }

    private void GrantPrototypeMana(int amount, bool returnToMap)
    {
        inventory.DevGrantMana(amount);
        RefreshDevSettingsUi(returnToMap);
    }

    private void GrantPrototypeCrystals(int amount, bool returnToMap)
    {
        inventory.DevGrantCrystals(amount);
        RefreshDevSettingsUi(returnToMap);
    }

    private void ResetDailySpinForTesting(bool returnToMap)
    {
        inventory.DevResetDailySpinClaim();
        lastDailySpinSummary = "";
        lastDailySpinRewardIndex = -1;
        dailySpinAnimating = false;
        dailySpinAnimationSequence++;
        dailySpinWheelRotation = 0f;
        RefreshDevSettingsUi(returnToMap);
    }

    private void GrantPrototypePowerUpSet(bool returnToMap)
    {
        for (int index = 0; index < PlayerInventoryState.GameplayPowerUps.Length; index++)
        {
            inventory.AddInventoryReward(PlayerInventoryState.GameplayPowerUps[index].Name, 1);
        }

        RefreshDevSettingsUi(returnToMap);
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

    private void CollectManaCauldronFromDen()
    {
        inventory.CollectManaCauldron();
        BuildPlayerDenUi();
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
        return roundState.IsActive
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
        if (!roundState.IsActive || string.IsNullOrEmpty(bankPowerUpName) || IsPowerUpBankReady())
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

        if (selectNext && roundState.IsActive && !rewardPreviewShown)
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
                    if (calledNumbers.Contains(value))
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
                roundState.MarkJackpotState();
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
        if (IsBlackoutRoom() && AllCardsReachedJackpot())
        {
            StartBlackoutCompletionCountdown();
            return;
        }

        if (!IsBlackoutRoom() && IsPrototypeBingoPoolExhausted())
        {
            StartRoomPoolCountdown($"Room bingo pool exhausted by {powerUpName} at {rewards.RoomBingosClaimed}/{roomRules.RoomBingoPool}.");
            return;
        }

        if (!IsBlackoutRoom() && IsStandardCoverageLimitReached())
        {
            StartFinalBallCountdown($"Standard room coverage limit reached after {powerUpName}.");
        }
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
        if (!inventory.TryRestoreActiveRoom())
        {
            return;
        }

        BuildRealmMapUi();
    }

    private void MaybeSimulatedPlayersClaimBingo()
    {
        if (!rewards.ShouldSimulatedPlayerClaim(calledHistory.Count))
        {
            return;
        }

        int simulatedClaimCount = calledHistory.Count >= 32 ? 2 : 1;
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

        if (!roundState.IsActive || !powerUps.ClairvoyanceActive || !calledNumbers.Contains(calledNumber))
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

        if (!calledNumbers.Contains(value))
        {
            statusText.text = $"{GetBingoLetter(value)}-{value} has not been called yet.";
            StartCoroutine(FlashCell(cells[row, column].transform, new Color(0.95f, 0.22f, 0.26f), new Color(0.95f, 0.97f, 1f)));
            return;
        }

        marked[row, column] = true;
        SaveCurrentCard();
        UpdateCell(row, column);
        StartCoroutine(PulseCell(cells[row, column].transform));

        if (!roundState.HasJackpotState && HasWinningLine())
        {
            roundState.MarkJackpotState();
            roundState.StopRound();
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
        int count = Mathf.Min(calledHistory.Count, 8);
        List<string> recentCalls = new List<string>();

        for (int index = 0; index < count; index++)
        {
            int number = calledHistory[index];
            recentCalls.Add($"{GetBingoLetter(number)}-{number}");
        }

        return string.Join("  ", recentCalls);
    }

    private string GetCalledHistoryPanelText()
    {
        if (calledHistory.Count == 0)
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
        for (int index = 0; index < calledHistory.Count; index++)
        {
            int number = calledHistory[index];
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
            calledNumberText.text = calledHistory.Count > 0 ? $"{GetBingoLetter(calledHistory[0])}-{calledHistory[0]}" : "Starting";
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
        if (calledHistory.Count <= historyIndex)
        {
            return "-";
        }

        int number = calledHistory[historyIndex];
        return $"{GetBingoLetter(number)}-{number}";
    }

    private bool HasMissedDaubs()
    {
        for (int index = 0; index < calledHistory.Count; index++)
        {
            if (IsMissedDaubNumber(calledHistory[index]))
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
        for (int index = 0; index < calledHistory.Count; index++)
        {
            int number = calledHistory[index];
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
        for (int index = 0; index < calledHistory.Count; index++)
        {
            if (IsMissedDaubNumber(calledHistory[index]))
            {
                count++;
            }
        }

        return count;
    }

    private void AdjustBet(int delta)
    {
        if (roundState.IsActive || calledHistory.Count > 0)
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

        while (roundState.IsActive
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

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

// Gameplay screen layout: fixed header, left/right rails and the center card area.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
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
}

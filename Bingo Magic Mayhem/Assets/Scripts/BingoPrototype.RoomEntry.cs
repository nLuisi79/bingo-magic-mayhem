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

// Room entry (lobby) screen: header, jackpot and restore panels, card-count options.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
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
        // Potion multiplier earned at this bet (locked rule: higher bet -> more ingredients per bingo).
        lobbyBetMultiplierText = CreateAnchoredText(betRow.transform, "", 30, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 80, 62, 308f, 0f);
        lobbyBetMultiplierText.supportRichText = true;
        lobbyBetMultiplierText.lineSpacing = 0.8f;

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
}

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

// Player Den hub plus its rooms: rank info, Apothecary, Relic Wall and the power-up cabinet.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
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
}

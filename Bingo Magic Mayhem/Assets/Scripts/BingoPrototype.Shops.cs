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

// Mayhem Market, Bewitchment Bazaar and the Oracle Alley reading inside the Bazaar.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildMayhemMarketUi()
    {
        RemoveMayhemMarketModal();
        RemoveLeaderboardModal();
        RemoveFriendsModal();
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateModalShell("MayhemMarketModal", 1060, 630, new Color(0.055f, 0.028f, 0.085f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "MAYHEM MARKET",
            "In-app purchase storefront shell. Products, prices, grants, purchase restore, platform IAP, and receipt validation are not active.",
            "Back to Den",
            gold,
            muted,
            violet,
            880f,
            262f,
            900f,
            36f,
            220f);
        chromeView.CloseRequested += RemoveMayhemMarketModal;

        GameObject wallet = CreateModalSectionPanel(panel.transform, "MarketWallet", 900, 54, 0f, 168f);
        string walletPreviewText = $"Current wallet preview: {inventory.GetManaText()} Mana | {inventory.GetCrystalText()} Crystals";
        Text walletText = CreateAnchoredText(wallet.transform, walletPreviewText, 20, FontStyle.Bold, Color.white, 820, 28, 0f, 0f);
        MayhemMarketWalletView walletView = wallet.GetComponent<MayhemMarketWalletView>();
        if (walletView == null)
        {
            walletView = wallet.AddComponent<MayhemMarketWalletView>();
        }

        walletView.ResetRuntimeBindings();
        walletView.Initialize(walletText);
        walletView.Apply(new MayhemMarketWalletDisplayModel(walletPreviewText));

        CreateMarketOfferCard(panel.transform, "Mana", "Small Mana Pouch", "Price TBD\nGrant TBD", -330f, 60f);
        CreateMarketOfferCard(panel.transform, "Crystals", "Crystal Bundle", "Price TBD\nGrant TBD", 0f, 60f);
        CreateMarketOfferCard(panel.transform, "Cards", "Card Pack Offer", "Odds TBD\nPack rules TBD", 330f, 60f);
        CreateMarketOfferCard(panel.transform, "Cosmetic", "Avatar / Dauber", "Catalog TBD\nNo gameplay edge", -330f, -118f);
        CreateMarketOfferCard(panel.transform, "Special", "Starter / Event", "Limits TBD\nFairness review", 0f, -118f);
        CreateMarketOfferCard(panel.transform, "Restore", "Restore Purchases", "Platform IAP later\nNo-op prototype", 330f, -118f);

        GameObject rules = CreateAnchoredPanel(panel.transform, "MarketRules", new Color(0.13f, 0.08f, 0.15f), 900, 70, 0f, -236f);
        const string marketRulesText = "Guardrails: purchases may support comfort/cosmetics, but cannot bypass core progression or let players pay their way up Aura Rank.";
        Text rulesText = CreateAnchoredText(rules.transform, marketRulesText, 14, FontStyle.Bold, muted, 830, 44, 0f, 0f);
        MayhemMarketRulesNoteView rulesView = rules.GetComponent<MayhemMarketRulesNoteView>();
        if (rulesView == null)
        {
            rulesView = rules.AddComponent<MayhemMarketRulesNoteView>();
        }

        rulesView.ResetRuntimeBindings();
        rulesView.Initialize(rulesText);
        rulesView.Apply(new MayhemMarketRulesNoteDisplayModel(marketRulesText));
    }

    private void CreateMarketOfferCard(Transform parent, string category, string title, string detail, float x, float y)
    {
        GameObject card = CreateModalInfoCard(parent, $"MarketOffer_{category}", 280, 148, x, y);
        Text categoryText = CreateAnchoredText(card.transform, category.ToUpperInvariant(), 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 240, 22, 0f, 48f);
        Text titleText = CreateAnchoredText(card.transform, title, 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 240, 28, 0f, 20f);
        Text detailText = CreateAnchoredText(card.transform, detail, 13, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 230, 42, 0f, -18f);
        Button button = CreateAnchoredButton(card.transform, "Purchase Locked", 13, 180, 28, new Color(0.34f, 0.32f, 0.38f), 0f, -56f);
        MayhemMarketOfferCardView cardView = card.GetComponent<MayhemMarketOfferCardView>();
        if (cardView == null)
        {
            cardView = card.AddComponent<MayhemMarketOfferCardView>();
        }

        cardView.ResetRuntimeBindings();
        cardView.Initialize(categoryText, titleText, detailText, button);
        cardView.Apply(new MayhemMarketOfferDisplayModel(category.ToUpperInvariant(), title, detail, "Purchase Locked", false));
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
        GameObject panel = CreateModalShell("BewitchmentBazaarModal", 1140, 650, new Color(0.055f, 0.028f, 0.085f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "BEWITCHMENT BAZAAR",
            "Prototype market hub. Center cart is the card/crystal swap area; Madame Solange and Coven are nearby destinations.",
            "Back to Den",
            gold,
            muted,
            violet,
            980f,
            274f,
            980f,
            36f,
            232f);
        chromeView.CloseRequested += BuildPlayerDenUi;

        BazaarDestinationPanelView oracleAlley = CreateBazaarDestinationPanel(
            panel.transform,
            "BazaarOracleAlley",
            new BazaarDestinationPanelDisplayModel(
                "ORACLE\nALLEY",
                "Madame Solange",
                "Reading table",
                "Tap the table"),
            gold,
            muted,
            -430f,
            28f);
        oracleAlley.Selected += BuildOracleAlleyReadingUi;

        GameObject cart = CreateAnchoredPanel(panel.transform, "BazaarCardCart", new Color(0.14f, 0.07f, 0.16f), 570, 390, -42f, 28f);
        Text cartTitleText = CreateModalSectionHeader(cart.transform, "CARD / CRYSTAL SWAP CART", gold, 510, 0f, 148f, 30, 42f);
        Text cartSubtitleText = CreateAnchoredText(cart.transform, "Daily market placeholder", 17, FontStyle.Bold, muted, 420, 24, 0f, 112f);
        CreateBazaarMarketCard(cart.transform, "Moonpetal", "30", -210f, 38f);
        CreateBazaarMarketCard(cart.transform, "Dewdrop", "35", -126f, 38f);
        CreateBazaarMarketCard(cart.transform, "Nightshade", "28", -42f, 38f);
        CreateBazaarMarketCard(cart.transform, "Stardust", "40", 42f, 38f);
        CreateBazaarMarketCard(cart.transform, "Silver Shell", "32", 126f, 38f);
        CreateBazaarMarketCard(cart.transform, "Dreamsand", "50", 210f, 38f);
        Button swap = CreateAnchoredButton(cart.transform, "Swap Later", 20, 230, 48, new Color(0.34f, 0.32f, 0.38f), 0f, -138f);
        swap.interactable = false;
        BazaarSwapCartView cartView = cart.GetComponent<BazaarSwapCartView>();
        if (cartView == null)
        {
            cartView = cart.AddComponent<BazaarSwapCartView>();
        }

        cartView.ResetRuntimeBindings();
        cartView.Initialize(cartTitleText, cartSubtitleText, swap);
        cartView.Apply(new BazaarSwapCartDisplayModel("CARD / CRYSTAL SWAP CART", "Daily market placeholder", "Swap Later"));

        BazaarDestinationPanelView covenDoor = CreateBazaarDestinationPanel(
            panel.transform,
            "BazaarCovenDoor",
            new BazaarDestinationPanelDisplayModel(
                "COVEN",
                coven.IsJoined ? coven.CovenName : "Coven Circle",
                "Social, orbs, requests,\nand member help live here.",
                "Tap the door"),
            gold,
            muted,
            430f,
            28f);
        covenDoor.Selected += BuildCovenCircleUi;

        const string bazaarFooterText = "Swap costs and Bazaar catalog are placeholders only; no purchase economy is active in this pass.";
        Text footerNoteText = CreateAnchoredText(panel.transform, bazaarFooterText, 15, FontStyle.Bold, muted, 960, 36, 0f, -206f);
        BazaarFooterNoteView footerNoteView = panel.GetComponent<BazaarFooterNoteView>();
        if (footerNoteView == null)
        {
            footerNoteView = panel.AddComponent<BazaarFooterNoteView>();
        }

        footerNoteView.ResetRuntimeBindings();
        footerNoteView.Initialize(footerNoteText);
        footerNoteView.Apply(new BazaarFooterNoteDisplayModel(bazaarFooterText));
    }

    private void CreateBazaarMarketCard(Transform parent, string title, string crystalCost, float x, float y)
    {
        GameObject card = CreateModalInfoCard(parent, $"BazaarCard_{title}", 74, 128, x, y);
        Text accentText = CreateAnchoredText(card.transform, "*", 32, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 56, 34, 0f, 32f);
        Text titleText = CreateAnchoredText(card.transform, title, 10, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 66, 26, 0f, -6f);
        Text detailText = CreateAnchoredText(card.transform, "<> " + crystalCost, 13, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 66, 22, 0f, -44f);
        UIBadgeCardView badgeView = card.GetComponent<UIBadgeCardView>();
        if (badgeView == null)
        {
            badgeView = card.AddComponent<UIBadgeCardView>();
        }

        badgeView.ResetRuntimeBindings();
        badgeView.Initialize(accentText, titleText, detailText);
        badgeView.Apply("*", title, "<> " + crystalCost);
    }

    private BazaarDestinationPanelView CreateBazaarDestinationPanel(Transform parent, string panelName, BazaarDestinationPanelDisplayModel displayModel, Color titleColor, Color mutedColor, float x, float y)
    {
        GameObject panel = CreateModalSectionPanel(parent, panelName, 250, 390, x, y);
        Button button = panel.GetComponent<Button>();
        if (button == null)
        {
            button = panel.AddComponent<Button>();
        }

        Text titleText = CreateAnchoredText(panel.transform, displayModel.Title, 32, FontStyle.Bold, titleColor, 210, 82, 0f, 106f);
        Text subtitleText = CreateAnchoredText(panel.transform, displayModel.Subtitle, 18, FontStyle.Bold, Color.white, 210, 30, 0f, 34f);
        Text detailText = CreateAnchoredText(panel.transform, displayModel.Detail, 15, FontStyle.Bold, mutedColor, 210, 74, 0f, -10f);
        Text hintText = CreateAnchoredText(panel.transform, displayModel.Hint, 13, FontStyle.Bold, mutedColor, 190, 28, 0f, -138f);

        BazaarDestinationPanelView panelView = panel.GetComponent<BazaarDestinationPanelView>();
        if (panelView == null)
        {
            panelView = panel.AddComponent<BazaarDestinationPanelView>();
        }

        panelView.ResetRuntimeBindings();
        panelView.Initialize(titleText, subtitleText, detailText, hintText, button);
        panelView.Apply(displayModel);
        return panelView;
    }

    private void BuildOracleAlleyReadingUi()
    {
        RemoveBewitchmentBazaarModal();
        GameObject panel = CreateModalShell("BewitchmentBazaarModal", 1140, 650, new Color(0.05f, 0.02f, 0.075f, 0.99f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);
        bool readingComplete = prototypeOracleSelectedCards.Count >= 3;

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "ORACLE ALLEY",
            "Visual reading prototype only. Costs, odds, dust conversion, rewards, and Book of Shadows rules remain TBD.",
            "Back to Bazaar",
            gold,
            muted,
            violet,
            820f,
            274f,
            920f,
            36f,
            232f,
            15,
            220f,
            48f,
            -130f,
            -292f,
            20);
        chromeView.CloseRequested += BuildBewitchmentBazaarUi;
        CreateAnchoredText(panel.transform, "MADAME SOLANGE", 25, FontStyle.Bold, gold, 420, 36, -372f, 206f);

        GameObject table = CreateModalSectionPanel(panel.transform, "MadameSolangeReadingTable", 900, 390, 0f, 22f);
        string oracleTableTitle = "PICK 3 CARDS";
        string oracleStatusText = $"{Mathf.Max(0, 3 - prototypeOracleSelectedCards.Count)} of 3 choices left | Cost TBD";
        Text oracleTableTitleText = CreateModalSectionHeader(table.transform, oracleTableTitle, gold, 260, -320f, 116f, 28, 42f);
        Text oracleStatusLabelText = CreateAnchoredText(table.transform, oracleStatusText, 17, FontStyle.Bold, muted, 300, 30, -300f, 70f);
        OracleReadingTableView tableView = table.GetComponent<OracleReadingTableView>();
        if (tableView == null)
        {
            tableView = table.AddComponent<OracleReadingTableView>();
        }

        tableView.ResetRuntimeBindings();
        tableView.Initialize(oracleTableTitleText, oracleStatusLabelText);
        tableView.Apply(new OracleReadingTableDisplayModel(oracleTableTitle, oracleStatusText));
        for (int index = 0; index < 5; index++)
        {
            CreateOracleFaceDownCard(table.transform, -240f + index * 120f, -22f, index + 1);
        }

        string summaryTextValue;
        if (readingComplete)
        {
            CreateAnchoredPanel(table.transform, "OracleBonusPreview", new Color(0.08f, 0.04f, 0.12f), 330, 86, 280f, 102f);
            CreateAnchoredText(table.transform, "BOOK OF SHADOWS BONUS", 17, FontStyle.Bold, gold, 300, 24, 280f, 120f);
            CreateAnchoredText(table.transform, "Placeholder only - no bonus granted", 13, FontStyle.Bold, muted, 300, 28, 280f, 94f);
            summaryTextValue = "The vision is complete. Outcome categories are previews only.";
        }
        else
        {
            summaryTextValue = string.IsNullOrWhiteSpace(lastOracleReadingSummary) ? "Tap a card to preview a reveal. Madame Solange is not a general helper." : lastOracleReadingSummary;
        }

        Text summaryText = CreateAnchoredText(table.transform, summaryTextValue, 16, FontStyle.Bold, readingComplete ? new Color(0.8f, 1f, 0.55f) : Color.white, 720, readingComplete ? 32 : 42, 0f, -160f);
        string resetButtonText = readingComplete ? "Close Reading" : "Reset Reading";
        Button reset = CreateAnchoredButton(panel.transform, resetButtonText, 20, 220, 48, readingComplete ? new Color(0.12f, 0.55f, 0.08f) : violet, 130f, -292f);
        OracleReadingSummaryView summaryView = panel.GetComponent<OracleReadingSummaryView>();
        if (summaryView == null)
        {
            summaryView = panel.AddComponent<OracleReadingSummaryView>();
        }

        summaryView.ResetRuntimeBindings();
        summaryView.Initialize(summaryText, reset);
        summaryView.Apply(new OracleReadingSummaryDisplayModel(summaryTextValue, resetButtonText));
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
}

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

// Library: Grimoire and Book of Shadows screens, ingredient detail/help overlays, card gifting and album claims.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildLibraryGrimoireUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        int albumOwnedCards = inventory.GetOwnedGrimoireCardCount();
        int bookOfShadowsOwnedCards = inventory.GetOwnedBookOfShadowsCardCount();

        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 48, FontStyle.Bold, gold, 1080, 62, 0f, 286f);
        Text subtitleText = CreateAnchoredText(panel.transform, string.Empty, 18, FontStyle.Bold, muted, 1060, 30, 0f, 242f);
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
        Button back = CreateDenReturnFooter(panel.transform, 0f, -306f, BuildPlayerDenUi);
        LibraryBooksLandingView landingView = panel.GetComponent<LibraryBooksLandingView>();
        if (landingView == null)
        {
            landingView = panel.AddComponent<LibraryBooksLandingView>();
        }

        landingView.ResetRuntimeBindings();
        landingView.Initialize(titleText, subtitleText, gifts, back);
        landingView.GiftsRequested += BuildLibraryCardGiftingUi;
        landingView.BackRequested += BuildPlayerDenUi;
        landingView.Apply(new LibraryBooksLandingDisplayModel
        {
            TitleText = "COLLECTION BOOKS",
            SubtitleText = "Choose a book, then browse potion sets and ingredient cards.",
            GiftsButtonText = "Card Gifts",
            BackButtonText = "Back to Den"
        });
    }

    private void BuildLibraryCardGiftingUi()
    {
        RemoveLibraryGrimoireModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "LibraryGrimoireModal", new Color(0.065f, 0.025f, 0.095f, 0.985f), 1220, 690, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color cream = new Color(0.96f, 0.86f, 0.62f);

        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 46, FontStyle.Bold, gold, 960, 58, 0f, 284f);
        Text subtitleText = CreateAnchoredText(panel.transform, string.Empty, 16, FontStyle.Bold, muted, 1040, 28, 0f, 244f);
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

        string summary = !string.IsNullOrWhiteSpace(lastLibraryCardGiftSummary)
            ? lastLibraryCardGiftSummary
            : "Only extra copies can be gifted. Your last copy stays protected.";
        Text summaryText = CreateAnchoredText(
            panel.transform,
            string.Empty,
            16,
            FontStyle.Bold,
            !string.IsNullOrWhiteSpace(lastLibraryCardGiftSummary) ? new Color(0.8f, 1f, 0.55f) : cream,
            900,
            30,
            0f,
            -238f);

        Button sample = CreateAnchoredButton(panel.transform, CanReceivePrototypeCardGift() ? "Receive Any Test Card" : "Receive Locked", 15, 250, 42, CanReceivePrototypeCardGift() ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.34f, 0.32f, 0.38f), -280f, -302f);
        Button back = CreateAnchoredButton(panel.transform, "Back to Library", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), 0f, -302f);
        int waitingCardGifts = GetInboxCategoryCount(PrototypeInboxCategory.Cards);
        Button inbox = CreateAnchoredButton(panel.transform, waitingCardGifts > 0 ? $"Open Inbox Cards ({waitingCardGifts})" : "Open Inbox Cards", 17, 230, 46, waitingCardGifts > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.18f, 0.16f, 0.22f), 280f, -302f);
        LibraryCardGiftingView view = panel.GetComponent<LibraryCardGiftingView>();
        if (view == null)
        {
            view = panel.AddComponent<LibraryCardGiftingView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, subtitleText, summaryText, sample, back, inbox);
        view.SampleRequested += QueueIncomingPrototypeCardGift;
        view.BackRequested += BuildLibraryGrimoireUi;
        view.InboxRequested += () =>
        {
            inboxActiveCategory = PrototypeInboxCategory.Cards;
            BuildInboxUi();
        };
        view.Apply(new LibraryCardGiftingDisplayModel
        {
            TitleText = "CARD GIFTS",
            SubtitleText = "Library handles card giving. Only extra Regular Grimoire copies can be sent; received cards are claimed from Inbox.",
            SummaryText = summary,
            SampleButtonText = CanReceivePrototypeCardGift() ? "Receive Any Test Card" : "Receive Locked",
            BackButtonText = "Back to Library",
            InboxButtonText = waitingCardGifts > 0 ? $"Open Inbox Cards ({waitingCardGifts})" : "Open Inbox Cards",
            CanSample = CanReceivePrototypeCardGift()
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
        Text cardNameText = CreateAnchoredText(row.transform, card.CardName, 14, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 230, 22, -86f, 8f);
        Text detailText = CreateAnchoredText(row.transform, $"{card.PotionName} | owned {copies}", 9, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 230, 16, -86f, -12f);
        Text starText = CreateAnchoredText(row.transform, BuildStarText(card.Stars), 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 54, 22, 74f, 0f);
        Button gift = CreateAnchoredButton(row.transform, CanSendPrototypeCardGift() ? "Send" : "Locked", 13, 86, 30, CanSendPrototypeCardGift() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 176f, 0f);
        LibraryCardGiftRowView view = row.GetComponent<LibraryCardGiftRowView>();
        if (view == null)
        {
            view = row.AddComponent<LibraryCardGiftRowView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(cardNameText, detailText, starText, gift);
        view.Apply(new LibraryCardGiftRowDisplayModel
        {
            CardName = card.CardName,
            DetailText = $"{card.PotionName} | owned {copies}",
            StarText = BuildStarText(card.Stars),
            ActionButtonText = CanSendPrototypeCardGift() ? "Send" : "Locked",
            ActionInteractable = CanSendPrototypeCardGift()
        });
        view.ActionRequested += () => SendPrototypeLibraryCardGift(card, "Mira");
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
        Text cardNameText = CreateAnchoredText(row.transform, card.CardName, 14, FontStyle.Bold, Color.white, 260, 22, -78f, 8f);
        Text detailText = CreateAnchoredText(row.transform, $"{card.PotionName} | {BuildStarText(card.Stars)}", 9, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 260, 16, -78f, -12f);
        Button request = CreateAnchoredButton(row.transform, CanReceivePrototypeCardGift() ? "Receive" : "Locked", 12, 86, 30, CanReceivePrototypeCardGift() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 176f, 0f);
        LibraryMissingCardRowView view = row.GetComponent<LibraryMissingCardRowView>();
        if (view == null)
        {
            view = row.AddComponent<LibraryMissingCardRowView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(cardNameText, detailText, request);
        view.Apply(new LibraryMissingCardRowDisplayModel
        {
            CardName = card.CardName,
            DetailText = $"{card.PotionName} | {BuildStarText(card.Stars)}",
            ActionButtonText = CanReceivePrototypeCardGift() ? "Receive" : "Locked",
            ActionInteractable = CanReceivePrototypeCardGift()
        });
        view.ActionRequested += () => QueueIncomingPrototypeCardGift(card);
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

        Text titleText = CreateModalTitle(panel.transform, string.Empty, gold, 940f, 286f);
        Text subtitleText = CreateModalSubtitle(panel.transform, string.Empty, muted, 980f, 30f, 242f, 18);
        Text collectedCountText = CreateAnchoredText(panel.transform, string.Empty, 23, FontStyle.Bold, Color.white, 360, 34, -362f, 194f);
        Text completionRewardText = CreateAnchoredText(panel.transform, string.Empty, 17, FontStyle.Bold, muted, 740, 34, 216f, 194f);
        Button fullClaim = CreateAnchoredButton(panel.transform, string.Empty, 15, 190, 38, GetAlbumClaimButtonColor(inventory.CanClaimGrimoireCompletionReward(), inventory.IsGrimoireCompletionRewardClaimed()), 454f, 238f);

        for (int index = start; index < end; index++)
        {
            CreateGrimoireIndexPotionTile(panel.transform, entries[index], index - start);
        }

        Button previous = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        Button next = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);
        CreateDualFooterNavigation(panel.transform, "Back to Books", -306f, BuildLibraryGrimoireUi, BuildPlayerDenUi);
        GrimoireIndexPanelView view = panel.GetComponent<GrimoireIndexPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<GrimoireIndexPanelView>();
        }

        CollectionModalHeaderView headerView = panel.GetComponent<CollectionModalHeaderView>();
        if (headerView == null)
        {
            headerView = panel.AddComponent<CollectionModalHeaderView>();
        }

        CollectionPagerView pagerView = panel.GetComponent<CollectionPagerView>();
        if (pagerView == null)
        {
            pagerView = panel.AddComponent<CollectionPagerView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(collectedCountText, completionRewardText, fullClaim);
        view.ClaimRequested += ClaimGrimoireCompletionReward;
        headerView.ResetRuntimeBindings();
        headerView.Initialize(titleText, subtitleText);
        pagerView.ResetRuntimeBindings();
        pagerView.Initialize(previous, next);
        pagerView.PreviousRequested += HandleGrimoireIndexPreviousRequested;
        pagerView.NextRequested += HandleGrimoireIndexNextRequested;
        headerView.Apply(new CollectionModalHeaderDisplayModel
        {
            TitleText = "GRIMOIRE",
            SubtitleText = $"Index {selectedGrimoireIndexPage + 1}/{Mathf.Max(1, (entries.Count + 7) / 8)}  |  Sets {start + 1}-{end}  |  {inventory.GetAlbumSeasonSummaryText()}"
        });
        view.Apply(new GrimoireIndexPanelDisplayModel
        {
            CollectedCountText = $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} cards collected",
            CompletionRewardText = $"Complete Grimoire Reward: {CardAlbumCatalog.BuildRewardLine(CardAlbumCatalog.GrimoireOneCompletionReward)}",
            ClaimButtonText = GetGrimoireCompletionClaimButtonText(),
            ClaimButtonInteractable = inventory.CanClaimGrimoireCompletionReward()
        });
        pagerView.Apply(new CollectionPagerDisplayModel
        {
            PreviousButtonText = "<",
            PreviousButtonInteractable = selectedGrimoireIndexPage > 0,
            NextButtonText = ">",
            NextButtonInteractable = end < entries.Count
        });
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

        Text titleText = CreateModalTitle(panel.transform, string.Empty, gold, 1000f, 286f);
        Text subtitleText = CreateModalSubtitle(panel.transform, string.Empty, muted, 980f, 28f, 248f, 17);

        GameObject leftPage = CreateAnchoredPanel(panel.transform, "GrimoireLeftPage", new Color(0.94f, 0.82f, 0.62f), 500, 430, -260f, 12f);
        Text potionNameText = CreateAnchoredText(leftPage.transform, string.Empty, 34, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 440, 80, 0f, 142f);
        Text setLabelText = CreateAnchoredText(leftPage.transform, string.Empty, 18, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 280, 26, 0f, 82f);
        Text artPlaceholderText = CreateAnchoredText(leftPage.transform, string.Empty, 52, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 260, 150, 0f, -12f);
        Text progressText = CreateAnchoredText(leftPage.transform, string.Empty, 26, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 420, 42, 0f, -114f);
        CreateProgressBar(leftPage.transform, owned, entry.Cards.Count, 300, 24, 0f, -158f);

        GameObject rightPage = CreateAnchoredPanel(panel.transform, "GrimoireRightPage", new Color(0.94f, 0.82f, 0.62f), 500, 430, 260f, 12f);
        Text rewardIntroText = CreateAnchoredText(rightPage.transform, string.Empty, 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 430, 30, 0f, 154f);
        CreateRewardBadgeRow(rightPage.transform, entry.Reward, 0f, 82f);
        Button claim = CreateAnchoredButton(rightPage.transform, string.Empty, 17, 220, 40, GetAlbumClaimButtonColor(inventory.CanClaimGrimoireEntryReward(entry), inventory.IsGrimoireEntryRewardClaimed(entry)), 0f, 26f);
        Text ingredientPreviewText = CreateAnchoredText(rightPage.transform, string.Empty, 20, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 430, 30, 0f, -20f);
        CreateMiniCardPreviewGrid(rightPage.transform, entry, 0f, -102f);

        Button view = CreateAnchoredButton(rightPage.transform, string.Empty, 22, 300, 54, new Color(0.12f, 0.55f, 0.08f), 0f, -172f);

        Button previous = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        Button next = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);

        GrimoireDetailPanelView detailView = panel.GetComponent<GrimoireDetailPanelView>();
        if (detailView == null)
        {
            detailView = panel.AddComponent<GrimoireDetailPanelView>();
        }

        CollectionModalHeaderView headerView = panel.GetComponent<CollectionModalHeaderView>();
        if (headerView == null)
        {
            headerView = panel.AddComponent<CollectionModalHeaderView>();
        }

        CollectionPagerView pagerView = panel.GetComponent<CollectionPagerView>();
        if (pagerView == null)
        {
            pagerView = panel.AddComponent<CollectionPagerView>();
        }

        detailView.ResetRuntimeBindings();
        detailView.Initialize(potionNameText, setLabelText, artPlaceholderText, progressText, rewardIntroText, claim, ingredientPreviewText, view);
        detailView.ClaimRequested += () => ClaimGrimoireEntryReward(entry);
        detailView.ViewIngredientsRequested += () => BuildGrimoireIngredientOverlay(entry);
        headerView.ResetRuntimeBindings();
        headerView.Initialize(titleText, subtitleText);
        pagerView.ResetRuntimeBindings();
        pagerView.Initialize(previous, next);
        pagerView.PreviousRequested += HandleGrimoireDetailPreviousRequested;
        pagerView.NextRequested += HandleGrimoireDetailNextRequested;
        headerView.Apply(new CollectionModalHeaderDisplayModel
        {
            TitleText = "GRIMOIRE",
            SubtitleText = $"Potion {entry.EntryNumber}/{entries.Count}  |  {inventory.GetAlbumSeasonSummaryText()}"
        });
        detailView.Apply(new GrimoireDetailPanelDisplayModel
        {
            PotionNameText = entry.PotionName,
            SetLabelText = "Potion Set",
            ArtPlaceholderText = "*\nPOTION\n*",
            ProgressText = $"{owned}/{entry.Cards.Count} Ingredients Found",
            RewardIntroText = "Complete this potion to earn",
            ClaimButtonText = GetGrimoireEntryClaimButtonText(entry),
            ClaimButtonInteractable = inventory.CanClaimGrimoireEntryReward(entry),
            IngredientPreviewText = "Ingredient Preview",
            ViewIngredientsButtonText = "View Ingredients"
        });
        pagerView.Apply(new CollectionPagerDisplayModel
        {
            PreviousButtonText = "<",
            PreviousButtonInteractable = selectedGrimoireEntryIndex > 0,
            NextButtonText = ">",
            NextButtonInteractable = selectedGrimoireEntryIndex < entries.Count - 1
        });

        CreateDualFooterNavigation(panel.transform, "Back to Index", -306f, BuildGrimoireIndexUi, BuildPlayerDenUi);
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

        Text titleText = CreateModalTitle(panel.transform, string.Empty, gold, 980f, 264f);
        Text subtitleText = CreateModalSubtitle(panel.transform, string.Empty, muted, 980f, 28f, 224f, 17);
        CreateLibraryStat(panel.transform, "Active Set", $"{inventory.GetOwnedBookOfShadowsSetCardCount(activeShadowSet)}/{CardAlbumCatalog.CountBookOfShadowsInSet(activeShadowSet)}", -280f, 166f);
        CreateLibraryStat(panel.transform, "All Sets", $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}", 0f, 166f);
        CreateLibraryStat(panel.transform, "Collections", $"{BuildCompletedBookOfShadowsEntryCount(activeShadowSet)}/{activeShadowSet.Entries.Count}", 280f, 166f);
        Button fullClaim = CreateAnchoredButton(panel.transform, string.Empty, 15, 210, 38, GetAlbumClaimButtonColor(inventory.CanClaimBookOfShadowsCompletionReward(), inventory.IsBookOfShadowsCompletionRewardClaimed()), 430f, 166f);

        for (int index = 0; index < activeShadowSet.Entries.Count; index++)
        {
            CreateBookOfShadowsIndexTile(panel.transform, activeShadowSet.Entries[index], index);
        }

        Text footerNoteText = CreateAnchoredText(panel.transform, string.Empty, 15, FontStyle.Bold, muted, 860, 36, 0f, -220f);

        BookOfShadowsLibraryPanelView libraryView = panel.GetComponent<BookOfShadowsLibraryPanelView>();
        if (libraryView == null)
        {
            libraryView = panel.AddComponent<BookOfShadowsLibraryPanelView>();
        }

        CollectionModalHeaderView headerView = panel.GetComponent<CollectionModalHeaderView>();
        if (headerView == null)
        {
            headerView = panel.AddComponent<CollectionModalHeaderView>();
        }

        libraryView.ResetRuntimeBindings();
        libraryView.Initialize(fullClaim, footerNoteText);
        libraryView.FullClaimRequested += ClaimBookOfShadowsCompletionReward;
        headerView.ResetRuntimeBindings();
        headerView.Initialize(titleText, subtitleText);
        headerView.Apply(new CollectionModalHeaderDisplayModel
        {
            TitleText = "BOOK OF SHADOWS",
            SubtitleText = inventory.GetBookOfShadowsSeasonSummaryText()
        });
        libraryView.Apply(new BookOfShadowsLibraryPanelDisplayModel
        {
            FullClaimButtonText = GetBookOfShadowsCompletionClaimButtonText(),
            FullClaimButtonInteractable = inventory.CanClaimBookOfShadowsCompletionReward(),
            FooterNoteText = "Book of Shadows cards are regular-only. Each monthly set closes when its 30-day window ends."
        });

        CreateDualFooterNavigation(panel.transform, "Back to Library", -284f, BuildLibraryGrimoireUi, BuildPlayerDenUi);
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

        Text titleText = CreateModalTitle(panel.transform, string.Empty, gold, 980f, 264f);
        Text subtitleText = CreateModalSubtitle(panel.transform, string.Empty, muted, 980f, 28f, 224f, 17);

        GameObject page = CreateAnchoredPanel(panel.transform, "ShadowsDetailPage", new Color(0.94f, 0.82f, 0.62f), 880, 390, 0f, 14f);
        Text potionNameText = CreateAnchoredText(page.transform, string.Empty, 34, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 760, 54, 0f, 136f);
        Text progressText = CreateAnchoredText(page.transform, string.Empty, 25, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 360, 42, -230f, 78f);
        CreateProgressBar(page.transform, owned, shadowEntry.Cards.Count, 300, 24, -230f, 40f);
        Text rewardText = CreateAnchoredText(page.transform, string.Empty, 18, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 400, 70, 220f, 66f);
        Button claim = CreateAnchoredButton(page.transform, string.Empty, 17, 220, 40, GetAlbumClaimButtonColor(inventory.CanClaimBookOfShadowsEntryReward(shadowEntry), inventory.IsBookOfShadowsEntryRewardClaimed(shadowEntry)), 220f, 12f);
        CreateBookOfShadowsCardSlotGrid(page.transform, shadowEntry, 0f, -18f);

        Button ingredients = CreateAnchoredButton(page.transform, string.Empty, 22, 300, 54, new Color(0.12f, 0.55f, 0.08f), 0f, -146f);

        Button previousShadow = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), -548f, -18f);
        Button nextShadow = CreateAnchoredButton(panel.transform, string.Empty, 28, 62, 54, new Color(0.35f, 0.12f, 0.62f), 548f, -18f);

        BookOfShadowsDetailPanelView detailView = panel.GetComponent<BookOfShadowsDetailPanelView>();
        if (detailView == null)
        {
            detailView = panel.AddComponent<BookOfShadowsDetailPanelView>();
        }

        CollectionModalHeaderView headerView = panel.GetComponent<CollectionModalHeaderView>();
        if (headerView == null)
        {
            headerView = panel.AddComponent<CollectionModalHeaderView>();
        }

        CollectionPagerView pagerView = panel.GetComponent<CollectionPagerView>();
        if (pagerView == null)
        {
            pagerView = panel.AddComponent<CollectionPagerView>();
        }

        detailView.ResetRuntimeBindings();
        detailView.Initialize(potionNameText, progressText, rewardText, claim, ingredients);
        detailView.ClaimRequested += () => ClaimBookOfShadowsEntryReward(shadowEntry);
        detailView.ViewIngredientsRequested += () => BuildBookOfShadowsIngredientOverlay(shadowEntry);
        headerView.ResetRuntimeBindings();
        headerView.Initialize(titleText, subtitleText);
        pagerView.ResetRuntimeBindings();
        pagerView.Initialize(previousShadow, nextShadow);
        pagerView.PreviousRequested += HandleBookOfShadowsDetailPreviousRequested;
        pagerView.NextRequested += HandleBookOfShadowsDetailNextRequested;
        headerView.Apply(new CollectionModalHeaderDisplayModel
        {
            TitleText = "BOOK OF SHADOWS",
            SubtitleText = $"Set {activeShadowSet.SetNumber}/3  |  Collection {shadowEntry.EntryNumber}/{activeShadowSet.Entries.Count}  |  {inventory.GetBookOfShadowsSeasonSummaryText()}"
        });
        detailView.Apply(new BookOfShadowsDetailPanelDisplayModel
        {
            PotionNameText = shadowEntry.PotionName,
            ProgressText = $"{owned}/{shadowEntry.Cards.Count} cards collected",
            RewardText = $"Reward: {CardAlbumCatalog.BuildRewardLine(shadowEntry.Reward)}",
            ClaimButtonText = GetBookOfShadowsEntryClaimButtonText(shadowEntry),
            ClaimButtonInteractable = inventory.CanClaimBookOfShadowsEntryReward(shadowEntry),
            ViewIngredientsButtonText = "View Ingredients"
        });
        pagerView.Apply(new CollectionPagerDisplayModel
        {
            PreviousButtonText = "<",
            PreviousButtonInteractable = selectedBookOfShadowsEntryIndex > 0,
            NextButtonText = ">",
            NextButtonInteractable = selectedBookOfShadowsEntryIndex < activeShadowSet.Entries.Count - 1
        });

        CreateDualFooterNavigation(panel.transform, "Back to Index", -284f, BuildBookOfShadowsLibraryUi, BuildPlayerDenUi);
    }

    private void HandleGrimoireDetailPreviousRequested()
    {
        SelectGrimoireEntry(selectedGrimoireEntryIndex - 1);
    }

    private void HandleGrimoireDetailNextRequested()
    {
        SelectGrimoireEntry(selectedGrimoireEntryIndex + 1);
    }

    private void HandleBookOfShadowsDetailPreviousRequested()
    {
        SelectBookOfShadowsEntry(selectedBookOfShadowsEntryIndex - 1);
    }

    private void HandleBookOfShadowsDetailNextRequested()
    {
        SelectBookOfShadowsEntry(selectedBookOfShadowsEntryIndex + 1);
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
        Text titleText = CreateAnchoredText(overlay.transform, string.Empty, 48, FontStyle.Bold, gold, 980, 60, -160f, 320f);
        Text progressText = CreateAnchoredText(overlay.transform, string.Empty, 27, FontStyle.Bold, Color.white, 520, 40, -408f, 262f);
        CreateProgressBar(overlay.transform, owned, entry.Cards.Count, 360, 24, -408f, 226f);
        CreateRewardBadgeRow(overlay.transform, entry.Reward, 350f, 262f);
        Text helperText = CreateAnchoredText(overlay.transform, string.Empty, 18, FontStyle.Bold, muted, 900, 30, 0f, -340f);
        CreateGrimoireIngredientCardGrid(overlay.transform, entry, 0f, 10f);
        inventory.MarkGrimoireEntryCardsSeen(entry);

        Button back = CreateAnchoredButton(overlay.transform, string.Empty, 24, 150, 54, new Color(0.35f, 0.12f, 0.62f), -640f, 338f);
        LibraryIngredientOverlayView overlayView = overlay.GetComponent<LibraryIngredientOverlayView>();
        if (overlayView == null)
        {
            overlayView = overlay.AddComponent<LibraryIngredientOverlayView>();
        }

        overlayView.ResetRuntimeBindings();
        overlayView.Initialize(titleText, progressText, helperText, back);
        overlayView.BackRequested += () =>
        {
            Destroy(overlay);
            BuildGrimoireDetailUi();
        };
        overlayView.Apply(new LibraryIngredientOverlayDisplayModel
        {
            TitleText = entry.PotionName,
            ProgressText = $"{owned}/{entry.Cards.Count} ingredients found",
            HelperText = "Tap an ingredient card for details, help requests, or Wild use.",
            BackButtonText = "Back"
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
        Text titleText = CreateAnchoredText(overlay.transform, string.Empty, 48, FontStyle.Bold, gold, 980, 60, -160f, 320f);
        Text progressText = CreateAnchoredText(overlay.transform, string.Empty, 27, FontStyle.Bold, Color.white, 520, 40, -408f, 262f);
        CreateProgressBar(overlay.transform, owned, entry.Cards.Count, 360, 24, -408f, 226f);
        CreateRewardBadgeRow(overlay.transform, entry.Reward, 350f, 262f);
        Text helperText = CreateAnchoredText(overlay.transform, string.Empty, 18, FontStyle.Bold, new Color(0.84f, 0.82f, 0.94f), 900, 30, 0f, -340f);
        CreateBookOfShadowsIngredientCardGrid(overlay.transform, entry, 0f, -10f);
        inventory.MarkBookOfShadowsEntryCardsSeen(entry);

        Button back = CreateAnchoredButton(overlay.transform, string.Empty, 24, 150, 54, new Color(0.35f, 0.12f, 0.62f), -640f, 338f);
        LibraryIngredientOverlayView overlayView = overlay.GetComponent<LibraryIngredientOverlayView>();
        if (overlayView == null)
        {
            overlayView = overlay.AddComponent<LibraryIngredientOverlayView>();
        }

        overlayView.ResetRuntimeBindings();
        overlayView.Initialize(titleText, progressText, helperText, back);
        overlayView.BackRequested += () =>
        {
            Destroy(overlay);
            BuildBookOfShadowsDetailUi();
        };
        overlayView.Apply(new LibraryIngredientOverlayDisplayModel
        {
            TitleText = entry.PotionName,
            ProgressText = $"{owned}/{entry.Cards.Count} ingredients found",
            HelperText = "Tap an ingredient card for details, duplicates, and collection progress.",
            BackButtonText = "Back"
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

        Text cardNameText = CreateAnchoredText(panel.transform, string.Empty, 36, FontStyle.Bold, purple, 820, 54, 10f, 242f);
        Text metadataText = CreateAnchoredText(panel.transform, string.Empty, 17, FontStyle.Bold, muted, 760, 26, 18f, 204f);

        GameObject art = CreateAnchoredPanel(panel.transform, "IngredientArtwork", GetAlbumTierColor(card.Tier), 330, 320, -270f, 24f);
        Text artPlaceholderText = CreateAnchoredText(art.transform, string.Empty, 30, FontStyle.Bold, Color.white, 280, 170, 0f, 22f);
        Text artFooterText = CreateAnchoredText(art.transform, string.Empty, 15, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 240, 28, 0f, -122f);

        GameObject countPanel = CreateAnchoredPanel(panel.transform, "IngredientCountPanel", new Color(0.98f, 0.9f, 0.7f), 400, 126, 230f, 96f);
        Text countTitleText = CreateAnchoredText(countPanel.transform, string.Empty, 18, FontStyle.Bold, new Color(0.16f, 0.38f, 0.08f), 260, 26, 0f, 36f);
        Text countValueText = CreateAnchoredText(countPanel.transform, string.Empty, 44, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 280, 60, 0f, -8f);
        Text countStatusText = CreateAnchoredText(countPanel.transform, string.Empty, 17, FontStyle.Bold, muted, 260, 24, 0f, -48f);

        Button help = CreateAnchoredButton(panel.transform, string.Empty, 20, 230, 50, new Color(0.35f, 0.12f, 0.62f), 104f, -40f);

        Button wild = CreateAnchoredButton(panel.transform, string.Empty, 20, 230, 50, inventory.JokerWildCards > 0 ? green : new Color(0.34f, 0.32f, 0.38f), 356f, -40f);

        GameObject sourcePanel = CreateAnchoredPanel(panel.transform, "WhereToGet", new Color(0.98f, 0.9f, 0.7f), 500, 88, 230f, -152f);
        Text sourceTitleText = CreateAnchoredText(sourcePanel.transform, string.Empty, 16, FontStyle.Bold, purple, 450, 22, 0f, 22f);
        Text sourceBodyText = CreateAnchoredText(sourcePanel.transform, string.Empty, 15, FontStyle.Bold, muted, 450, 38, 0f, -12f);

        Button close = CreateAnchoredButton(panel.transform, string.Empty, 18, 240, 46, new Color(0.35f, 0.12f, 0.62f), 0f, -252f);
        IngredientDetailModalView view = panel.GetComponent<IngredientDetailModalView>();
        if (view == null)
        {
            view = panel.AddComponent<IngredientDetailModalView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(cardNameText, metadataText, artPlaceholderText, artFooterText, countTitleText, countValueText, countStatusText, help, wild, sourceTitleText, sourceBodyText, close);
        view.HelpRequested += () => BuildIngredientAskForHelpModal(entry, card, true, "");
        view.WildRequested += () => BuildUseWildConfirmation(entry, card);
        view.CloseRequested += RemoveIngredientDetailModal;
        view.Apply(new IngredientDetailModalDisplayModel
        {
            CardNameText = card.CardName,
            MetadataText = $"{entry.PotionName} | {GetAlbumTierLabel(card.Tier)} | {BuildStarText(card.Stars)}",
            ArtPlaceholderText = BuildIngredientSpecimenText(card.CardName),
            ArtFooterText = "ingredient card",
            CountTitleText = "YOU HAVE",
            CountValueText = $"{copies} / 1",
            CountStatusText = copies > 0 ? "Owned" : "Missing",
            HelpButtonText = "Ask for Help",
            WildButtonText = inventory.JokerWildCards > 0 ? "Use Wild" : "No Wild Cards",
            WildButtonInteractable = inventory.JokerWildCards > 0,
            SourceTitleText = "WHERE TO GET",
            SourceBodyText = "Play realm rooms, open card packs, or ask eligible Coven/Friend helpers.",
            CloseButtonText = "Back to Ingredients"
        });
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

        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 44, FontStyle.Bold, gold, 620, 56, 120f, 242f);
        Text usageText = CreateAnchoredText(panel.transform, string.Empty, 24, FontStyle.Bold, Color.white, 520, 42, 188f, 184f);

        GameObject summary = CreateAnchoredPanel(panel.transform, "HelpIngredientSummary", cream, 360, 390, -320f, 0f);
        Text summaryCardNameText = CreateAnchoredText(summary.transform, string.Empty, 30, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 310, 76, 0f, 118f);
        Text summarySpecimenText = CreateAnchoredText(summary.transform, string.Empty, 54, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 260, 100, 0f, 12f);
        Text summaryCountText = CreateAnchoredText(summary.transform, string.Empty, 28, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 260, 92, 0f, -118f);

        Button coven = CreateAnchoredButton(panel.transform, string.Empty, 21, 260, 48, covenTab ? purple : new Color(0.28f, 0.26f, 0.32f), 62f, 98f);
        Button friends = CreateAnchoredButton(panel.transform, string.Empty, 21, 260, 48, !covenTab ? blue : new Color(0.28f, 0.26f, 0.32f), 330f, 98f);

        GameObject recipientPanel = CreateAnchoredPanel(panel.transform, "HelpRecipients", cream, 640, 238, 196f, -38f);
        Text recipientsTitleText = CreateAnchoredText(recipientPanel.transform, string.Empty, 22, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 560, 30, 0f, 86f);
        CreateHelpRecipientGrid(recipientPanel.transform, covenTab, activeTabAvailable);

        string helpNote = !activeTabAvailable
            ? "Friends rotate by ingredient/day. Try Coven or come back later."
            : string.IsNullOrWhiteSpace(statusMessage) ? "Select helpers, then send. Prototype sends to visible helpers." : statusMessage;
        Text helpNoteText = CreateAnchoredText(panel.transform, string.Empty, 16, FontStyle.Bold, muted, 640, 36, 196f, -184f);

        bool canSend = activeTabAvailable && inventory.CanSendSocialHelpRequest();
        Button send = CreateAnchoredButton(panel.transform, string.Empty, 22, 300, 54, canSend ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 196f, -242f);
        Button back = CreateAnchoredButton(panel.transform, string.Empty, 18, 240, 46, new Color(0.18f, 0.16f, 0.22f), -390f, -242f);

        IngredientHelpModalView view = panel.GetComponent<IngredientHelpModalView>();
        if (view == null)
        {
            view = panel.AddComponent<IngredientHelpModalView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, usageText, summaryCardNameText, summarySpecimenText, summaryCountText, coven, friends, recipientsTitleText, helpNoteText, send, back);
        view.CovenRequested += () => BuildIngredientAskForHelpModal(entry, card, true, statusMessage);
        view.FriendsRequested += () => BuildIngredientAskForHelpModal(entry, card, false, statusMessage);
        view.SendRequested += () =>
        {
            bool sent = inventory.TrySendSocialHelpRequest();
            if (sent)
            {
                TrackPrototypeAnalytics(
                    PrototypeAnalyticsEvents.SocialHelpRequestSent,
                    PrototypeAnalyticsPayloadFactory.CreateSocialHelpRequestSentJson(
                        covenTab,
                        card,
                        inventory.GetDailyHelpRequestsUsedToday(),
                        inventory.GetDailyHelpRequestLimit()));
            }

            string sentMessage = sent
                ? $"Sent request for {card.CardName}."
                : "Daily help request limit reached.";
            BuildIngredientAskForHelpModal(entry, card, covenTab, sentMessage);
        };
        view.BackRequested += RemoveIngredientHelpModal;
        view.Apply(new IngredientHelpModalDisplayModel
        {
            TitleText = "ASK FOR HELP",
            UsageText = $"Daily Help Requests  {used} / {limit} Used",
            SummaryCardNameText = card.CardName,
            SummarySpecimenText = BuildIngredientSpecimenText(card.CardName),
            SummaryCountText = $"You Have\n{inventory.GetGrimoireCardCopies(card.Id)} / 1",
            CovenButtonText = "Coven",
            CovenButtonInteractable = true,
            FriendsButtonText = friendsAvailable ? "Friends" : "Friends Locked",
            FriendsButtonInteractable = friendsAvailable,
            RecipientsTitleText = covenTab ? "Coven helpers" : "Friend helpers",
            HelpNoteText = helpNote,
            SendButtonText = canSend ? "Send Request" : "Limit Reached",
            SendButtonInteractable = canSend,
            BackButtonText = "Back to Ingredient"
        });
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
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 25, FontStyle.Bold, gold, 540, 42, 0f, 104f);

        GameObject wildCard = CreateAnchoredPanel(panel.transform, "WildCardPreview", new Color(0.35f, 0.12f, 0.62f), 150, 150, -160f, 4f);
        Text wildCardText = CreateAnchoredText(wildCard.transform, string.Empty, 28, FontStyle.Bold, Color.white, 120, 90, 0f, 0f);
        Text arrowText = CreateAnchoredText(panel.transform, string.Empty, 42, FontStyle.Bold, Color.white, 80, 60, 0f, 4f);
        GameObject targetCard = CreateAnchoredPanel(panel.transform, "TargetIngredientPreview", GetAlbumTierColor(card.Tier), 150, 150, 160f, 4f);
        Text targetSpecimenText = CreateAnchoredText(targetCard.transform, string.Empty, 23, FontStyle.Bold, Color.white, 120, 76, 0f, 10f);
        Text targetCardNameText = CreateAnchoredText(targetCard.transform, string.Empty, 11, FontStyle.Bold, Color.white, 130, 34, 0f, -54f);

        Button no = CreateAnchoredButton(panel.transform, string.Empty, 20, 150, 44, new Color(0.34f, 0.32f, 0.38f), -88f, -118f);
        Button yes = CreateAnchoredButton(panel.transform, "Yes", 20, 150, 44, new Color(0.12f, 0.55f, 0.08f), 88f, -118f);
        WildConfirmationModalView view = panel.GetComponent<WildConfirmationModalView>();
        if (view == null)
        {
            view = panel.AddComponent<WildConfirmationModalView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, wildCardText, arrowText, targetSpecimenText, targetCardNameText, no, yes);
        view.NoRequested += () => Destroy(panel);
        view.YesRequested += () =>
        {
            if (inventory.TryUseJokerWildForGrimoireCard(card.Id))
            {
                TrackPrototypeAnalytics(
                    PrototypeAnalyticsEvents.WildCardUsed,
                    PrototypeAnalyticsPayloadFactory.CreateWildCardUsedJson(
                        entry,
                        card,
                        inventory.JokerWildCards));
                Destroy(panel);
                BuildGrimoireIngredientOverlay(entry);
                BuildGrimoireIngredientDetailModal(entry, card);
            }
        };
        view.Apply(new WildConfirmationModalDisplayModel
        {
            TitleText = $"Use 1 Wild for {card.CardName}?",
            WildCardText = "WILD\nCARD",
            ArrowText = ">",
            TargetSpecimenText = BuildIngredientSpecimenText(card.CardName),
            TargetCardNameText = card.CardName,
            NoButtonText = "No",
            YesButtonText = "Yes"
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
        GameObject row = CreateModalInfoCard(parent, $"LibraryRow_{title}", 310, 46, x, y);
        Text titleText = CreateAnchoredText(row.transform, title, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 176, 22, -48f, 8f);
        Text noteText = CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 194, 18, -36f, -13f);
        Text countText = CreateAnchoredText(row.transform, "x" + count, 22, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 62, 34, 114f, 0f);
        UITitleNoteCountRowView rowView = row.AddComponent<UITitleNoteCountRowView>();
        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, noteText, countText);
        rowView.Apply(title, note, "x" + count);
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
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.AlbumRewardClaimed,
                PrototypeAnalyticsPayloadFactory.CreateAlbumRewardClaimedJson(
                    "grimoire_entry_reward",
                    "grimoire",
                    "entry",
                    entry.Reward,
                    entry.EntryNumber));
            RefreshPowerUpDisplays();
        }

        BuildGrimoireDetailUi();
    }

    private void ClaimGrimoireCompletionReward()
    {
        if (inventory.TryClaimGrimoireCompletionReward())
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.AlbumRewardClaimed,
                PrototypeAnalyticsPayloadFactory.CreateAlbumRewardClaimedJson(
                    "grimoire_completion_reward",
                    "grimoire",
                    "completion",
                    CardAlbumCatalog.GrimoireOneCompletionReward));
            RefreshPowerUpDisplays();
        }

        BuildGrimoireIndexUi();
    }

    private void ClaimBookOfShadowsEntryReward(BookOfShadowsEntryDefinition entry)
    {
        if (inventory.TryClaimBookOfShadowsEntryReward(entry))
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.AlbumRewardClaimed,
                PrototypeAnalyticsPayloadFactory.CreateAlbumRewardClaimedJson(
                    "book_of_shadows_entry_reward",
                    "book_of_shadows",
                    "entry",
                    entry.Reward,
                    entry.EntryNumber,
                    entry.SetNumber));
            RefreshPowerUpDisplays();
        }

        BuildBookOfShadowsDetailUi();
    }

    private void ClaimBookOfShadowsCompletionReward()
    {
        if (inventory.TryClaimBookOfShadowsCompletionReward())
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.AlbumRewardClaimed,
                PrototypeAnalyticsPayloadFactory.CreateAlbumRewardClaimedJson(
                    "book_of_shadows_completion_reward",
                    "book_of_shadows",
                    "completion",
                    CardAlbumCatalog.BookOfShadowsCompletionReward));
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

    private void HandleGrimoireIndexPreviousRequested()
    {
        selectedGrimoireIndexPage--;
        BuildGrimoireIndexUi();
    }

    private void HandleGrimoireIndexNextRequested()
    {
        selectedGrimoireIndexPage++;
        BuildGrimoireIndexUi();
    }
}

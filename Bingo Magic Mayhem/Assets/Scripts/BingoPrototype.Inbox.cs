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

// Inbox screens (gifts, requests, messages and replies). Inbox state lives in CovenState / the inbox models; this part of
// BingoPrototype only builds the UI and applies claimed items. Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildInboxUi()
    {
        RemoveInboxModal();
        RemoveLibraryGrimoireModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();
        GameObject panel = CreateModalShell("InboxModal", 840, 560, new Color(0.07f, 0.035f, 0.12f, 0.985f));
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, -22f, panel.transform.localPosition.z);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "INBOX",
            GetInboxHeaderText(),
            "Back to Den",
            gold,
            muted,
            violet,
            700f,
            236f,
            700f,
            30f,
            200f,
            16,
            220f,
            48f,
            0f,
            -246f,
            20);
        chromeView.CloseRequested += BuildPlayerDenUi;
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
        UITabButtonBinder binder = tab.GetComponent<UITabButtonBinder>();
        if (binder == null)
        {
            binder = tab.gameObject.AddComponent<UITabButtonBinder>();
        }

        binder.SetSelected(active);
        InboxTabButtonView tabView = tab.gameObject.GetComponent<InboxTabButtonView>();
        if (tabView == null)
        {
            tabView = tab.gameObject.AddComponent<InboxTabButtonView>();
        }

        tabView.ResetRuntimeBindings();
        tabView.Initialize(binder, category.ToString());
        tabView.TabSelected += HandleInboxTabSelected;
    }

    private void HandleInboxTabSelected(string tabId)
    {
        if (System.Enum.TryParse(tabId, out PrototypeInboxCategory category))
        {
            inboxActiveCategory = category;
            BuildInboxUi();
        }
    }

    private void CreateInboxItemRow(Transform parent, PrototypeInboxItem item, int index, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"InboxItem_{index}", 680, 62, 0f, y);
        bool isMessage = item.Category == PrototypeInboxCategory.Messages;
        if (isMessage)
        {
            Button rowOpen = row.AddComponent<Button>();
            rowOpen.targetGraphic = row.GetComponent<Image>();
            rowOpen.onClick.AddListener(() => OpenInboxMessage(index));
        }

        string detail = BuildInboxItemDetail(item);
        Text titleText = CreateAnchoredText(row.transform, item.Title, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), isMessage ? 350 : 430, 22, isMessage ? -126f : -88f, 14f);
        Text detailText = CreateAnchoredText(row.transform, detail, 11, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), isMessage ? 350 : 430, 30, isMessage ? -126f : -88f, -12f);

        if (isMessage)
        {
            Color badgeColor = item.Title.StartsWith("Message to", System.StringComparison.OrdinalIgnoreCase)
                ? new Color(0.35f, 0.12f, 0.62f)
                : new Color(0.12f, 0.55f, 0.08f);
            string directionLabel = GetInboxMessageDirectionLabel(item);
            Text primaryBadgeText = CreateAnchoredText(row.transform, directionLabel, 10, FontStyle.Bold, badgeColor, 72, 18, 116f, 10f);
            Text secondaryBadgeText = CreateAnchoredText(row.transform, item.IsUnread ? "NEW" : "", 10, FontStyle.Bold, new Color(0.12f, 0.55f, 0.08f), 72, 18, 116f, -12f);

            Button read = CreateAnchoredButton(row.transform, item.IsUnread ? "Read" : "Open", 12, 74, 30, new Color(0.12f, 0.55f, 0.08f), 220f, 0f);
            Button reply = CreateAnchoredButton(row.transform, "Reply", 12, 78, 30, new Color(0.35f, 0.12f, 0.62f), 304f, 0f);
            InboxItemRowView rowView = row.GetComponent<InboxItemRowView>();
            if (rowView == null)
            {
                rowView = row.AddComponent<InboxItemRowView>();
            }

            rowView.ResetRuntimeBindings();
            rowView.Initialize(titleText, detailText, primaryBadgeText, secondaryBadgeText, read, reply);
            rowView.Apply(new InboxItemRowDisplayModel(
                item.Title,
                detail,
                directionLabel,
                item.IsUnread ? "NEW" : "",
                item.IsUnread ? "Read" : "Open",
                "Reply",
                true,
                true));
            rowView.PrimaryActionRequested += () => OpenInboxMessage(index);
            rowView.SecondaryActionRequested += () => ReplyToInboxMessage(item);
            return;
        }

        Text claimBadgeText = CreateAnchoredText(row.transform, item.IsUnread ? "NEW" : "", 12, FontStyle.Bold, new Color(0.12f, 0.55f, 0.08f), 54, 22, 168f, 12f);
        Button collect = CreateAnchoredButton(row.transform, "Claim", 14, 112, 32, new Color(0.12f, 0.55f, 0.08f), 272f, 0f);
        InboxItemRowView claimRowView = row.GetComponent<InboxItemRowView>();
        if (claimRowView == null)
        {
            claimRowView = row.AddComponent<InboxItemRowView>();
        }

        claimRowView.ResetRuntimeBindings();
        claimRowView.Initialize(titleText, detailText, claimBadgeText, null, collect, null);
        claimRowView.Apply(new InboxItemRowDisplayModel(
            item.Title,
            detail,
            item.IsUnread ? "NEW" : "",
            string.Empty,
            "Claim",
            string.Empty,
            true,
            false));
        claimRowView.PrimaryActionRequested += () => ClaimInboxItem(index);
    }

    private void OpenInboxMessage(int index)
    {
        if (!coven.TryMarkPlayerInboxItemRead(index, out PrototypeInboxItem message))
        {
            return;
        }

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.InboxMessageRead,
            PrototypeAnalyticsPayloadFactory.CreateInboxMessageReadJson(
                "open",
                coven.PlayerInboxUnreadCount,
                coven.PlayerInboxClaimableCount));
        BuildInboxMessageDetailUi(message);
    }

    private void BuildInboxMessageDetailUi(PrototypeInboxItem message)
    {
        RemoveInboxMessageDetailModal();

        GameObject panel = CreateModalShell("InboxMessageDetailModal", 680, 400, new Color(0.055f, 0.028f, 0.085f, 0.995f));
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, -20f, panel.transform.localPosition.z);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            message.Title.ToUpperInvariant(),
            GetInboxMessageDirectionLabel(message),
            "Close",
            gold,
            muted,
            violet,
            560f,
            108f,
            560f,
            24f,
            144f,
            16,
            150f,
            42f,
            178f,
            -142f,
            18);
        CreateModalInfoCard(panel.transform, "InboxMessageBody", 560, 148, 0f, 18f);
        Text body = CreateAnchoredText(panel.transform, string.IsNullOrWhiteSpace(message.Detail) ? "(No message text.)" : message.Detail, 18, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 520, 118, 0f, 18f);
        body.alignment = TextAnchor.MiddleLeft;
        Text footnoteText = CreateAnchoredText(panel.transform, "Messages live in Inbox until cleared.", 14, FontStyle.Bold, muted, 560, 24, 0f, -78f);

        Button back = CreateAnchoredButton(panel.transform, "Back", 18, 150, 42, new Color(0.18f, 0.16f, 0.22f), -178f, -142f);
        bool canReply = TryGetFriendNameFromInboxMessage(message, out string friendName) && !friendsState.IsBlocked(friendName);
        Button reply = canReply
            ? CreateAnchoredButton(panel.transform, "Reply", 18, 150, 42, violet, 0f, -142f)
            : null;

        InboxMessageDetailView detailView = panel.GetComponent<InboxMessageDetailView>();
        if (detailView == null)
        {
            detailView = panel.AddComponent<InboxMessageDetailView>();
        }

        detailView.ResetRuntimeBindings();
        detailView.Initialize(body, footnoteText, back, reply);
        detailView.Apply(new InboxMessageDetailDisplayModel
        {
            BodyText = string.IsNullOrWhiteSpace(message.Detail) ? "(No message text.)" : message.Detail,
            FootnoteText = "Messages live in Inbox until cleared.",
            BackButtonText = "Back",
            ReplyButtonText = "Reply",
            ShowReplyButton = canReply
        });
        detailView.BackRequested += () =>
        {
            RemoveInboxMessageDetailModal();
            BuildInboxUi();
        };
        if (canReply)
        {
            detailView.ReplyRequested += () =>
            {
                RemoveInboxMessageDetailModal();
                BuildFriendMessageComposerUi(friendName);
            };
        }

        chromeView.CloseRequested += RemoveInboxMessageDetailModal;
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

        if (friendsState.IsBlocked(friendName))
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

            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.InboxMessageRead,
                PrototypeAnalyticsPayloadFactory.CreateInboxMessageReadJson(
                    "claim_button",
                    coven.PlayerInboxUnreadCount,
                    coven.PlayerInboxClaimableCount));
            lastInboxClaimSummary = $"Read: {message.Title}";
            coven.AddPrototypeChatLine($"You read inbox message: {message.Title}");
            BuildInboxUi();
            return;
        }

        if (!coven.TryTakePlayerInboxItem(index, out PrototypeInboxItem item))
        {
            return;
        }

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.InboxItemClaimed,
            PrototypeAnalyticsPayloadFactory.CreateInboxItemClaimedJson(item, false));
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

        if (collected > 0)
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.InboxCategoryCleared,
                PrototypeAnalyticsPayloadFactory.CreateInboxCategoryClearedJson(inboxActiveCategory, collected));
        }

        lastInboxClaimSummary = collected > 0 ? $"{(inboxActiveCategory == PrototypeInboxCategory.Messages ? "Cleared" : "Claimed")} {collected}: {BuildCompactInboxClaimSummary(summaries)}" : "No rewards claimed.";
        coven.AddPrototypeChatLine(inboxActiveCategory == PrototypeInboxCategory.Messages ? $"Clear All read {collected} inbox messages." : $"Claim All collected {collected} inbox rewards.");
        BuildInboxUi();
    }

    private void TrackPrototypeAnalytics(string eventName, string safePayloadJson = "{}")
    {
        if (!GameInfrastructureRuntime.IsInitialized || GameInfrastructureRuntime.Current?.Analytics == null)
        {
            return;
        }

        GameInfrastructureRuntime.Current.Analytics.Track(eventName, string.IsNullOrWhiteSpace(safePayloadJson) ? "{}" : safePayloadJson);
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
}

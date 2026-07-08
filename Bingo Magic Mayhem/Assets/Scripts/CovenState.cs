using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class CovenState
{
    private const string SavePrefix = "BMM.Coven.";
    private const string InboxItemsJsonKey = SavePrefix + "PlayerInboxItemsJson";
    private const string InboxInitializedKey = SavePrefix + "PlayerInboxInitialized";
    private const string DefaultCovenName = "Moonpetal Circle";
    private const string DefaultLeaderTitle = "High Priestess";
    private const string DefaultLeaderName = "Mira";
    private const int DefaultWeeklyOrbGoal = 75;
    private const int DefaultTeamOrbProgress = 18;
    private const int DefaultWeeklyRank = 24;
    private const int DefaultWeeklyPoints = 420;
    private const int CovenMemberCap = 50;

    public bool IsJoined { get; private set; } = true;
    public string CovenName { get; private set; } = DefaultCovenName;
    public string LeaderTitle { get; private set; } = DefaultLeaderTitle;
    public string LeaderName { get; private set; } = DefaultLeaderName;
    public int WeeklyOrbGoal { get; private set; } = DefaultWeeklyOrbGoal;
    public int TeamOrbProgress { get; private set; } = DefaultTeamOrbProgress;
    public int WeeklyRank { get; private set; } = DefaultWeeklyRank;
    public int WeeklyPoints { get; private set; } = DefaultWeeklyPoints;
    public int MemberCap => CovenMemberCap;
    public IReadOnlyList<CovenMemberInfo> Members => members;
    public IReadOnlyList<CovenJoinRequestInfo> JoinRequests => joinRequests;
    public IReadOnlyList<string> ChatLines => chatLines;
    public IReadOnlyList<CovenInboxGiftInfo> InboxGifts => inboxGifts;
    public IReadOnlyList<PrototypeInboxItem> PlayerInboxItems => playerInboxItems;
    public IReadOnlyList<CovenInboxGiftInfo> PlayerInboxGifts => playerInboxGifts;
    public int PlayerInboxClaimableCount => playerInboxItems.Count;
    public int PlayerInboxUnreadCount => CountUnreadPlayerInboxItems();

    private readonly List<CovenMemberInfo> members = new List<CovenMemberInfo>();
    private readonly List<CovenJoinRequestInfo> joinRequests = new List<CovenJoinRequestInfo>();
    private readonly List<string> chatLines = new List<string>();
    private readonly List<CovenInboxGiftInfo> inboxGifts = new List<CovenInboxGiftInfo>();
    private readonly List<PrototypeInboxItem> playerInboxItems = new List<PrototypeInboxItem>();
    private readonly List<CovenInboxGiftInfo> playerInboxGifts = new List<CovenInboxGiftInfo>();

    public void Load()
    {
        IsJoined = PlayerPrefs.GetInt(SavePrefix + "IsJoined", IsJoined ? 1 : 0) == 1;
        CovenName = PlayerPrefs.GetString(SavePrefix + "CovenName", DefaultCovenName);
        LeaderTitle = PlayerPrefs.GetString(SavePrefix + "LeaderTitle", DefaultLeaderTitle);
        LeaderName = PlayerPrefs.GetString(SavePrefix + "LeaderName", DefaultLeaderName);
        WeeklyOrbGoal = Mathf.Max(1, PlayerPrefs.GetInt(SavePrefix + "WeeklyOrbGoal", DefaultWeeklyOrbGoal));
        TeamOrbProgress = Mathf.Clamp(PlayerPrefs.GetInt(SavePrefix + "TeamOrbProgress", DefaultTeamOrbProgress), 0, WeeklyOrbGoal);
        WeeklyRank = Mathf.Max(0, PlayerPrefs.GetInt(SavePrefix + "WeeklyRank", DefaultWeeklyRank));
        WeeklyPoints = Mathf.Max(0, PlayerPrefs.GetInt(SavePrefix + "WeeklyPoints", DefaultWeeklyPoints));
        SeedPrototypeMembers();
        SeedPrototypeJoinRequests();
        SeedPrototypeChat();
        LoadPlayerInboxItems();
        SeedPrototypePlayerInbox();
    }

    public void JoinPrototypeCoven()
    {
        IsJoined = true;
        CovenName = DefaultCovenName;
        LeaderTitle = DefaultLeaderTitle;
        LeaderName = DefaultLeaderName;
        SeedPrototypeMembers();
        SeedPrototypeJoinRequests();
        SeedPrototypeChat();
        SeedPrototypePlayerInbox();
        Save();
    }

    public void LeaveCoven()
    {
        IsJoined = false;
        Save();
    }

    public string GetHeaderText()
    {
        return IsJoined
            ? $"{CovenName} | {LeaderTitle}: {LeaderName}"
            : "Find, join, or create a Coven to unlock team systems.";
    }

    public string GetWeeklyChallengeText(int playerClubOrbs)
    {
        if (!IsJoined)
        {
            return "-";
        }

        return $"{TeamOrbProgress}/{WeeklyOrbGoal}";
    }

    public int GetRemainingWeeklyOrbs()
    {
        return Mathf.Max(0, WeeklyOrbGoal - TeamOrbProgress);
    }

    public int ContributeClubOrbs(int availableClubOrbs)
    {
        if (!IsJoined || availableClubOrbs <= 0)
        {
            return 0;
        }

        int contribution = Mathf.Min(availableClubOrbs, GetRemainingWeeklyOrbs());
        if (contribution <= 0)
        {
            return 0;
        }

        TeamOrbProgress = Mathf.Clamp(TeamOrbProgress + contribution, 0, WeeklyOrbGoal);
        WeeklyPoints += contribution * 5;
        Save();
        return contribution;
    }

    public string GetStandingsText()
    {
        if (!IsJoined)
        {
            return "-";
        }

        return $"Rank {WeeklyRank}";
    }

    public string GetWeeklyChallengeCardNote(int playerClubOrbs)
    {
        if (!IsJoined)
        {
            return "join to call Club Orbs";
        }

        int heldOrbs = Mathf.Max(0, playerClubOrbs);
        return $"orb call {GetWeeklyChallengeText(playerClubOrbs)}, held {heldOrbs}";
    }

    public string GetStandingsCardNote()
    {
        if (!IsJoined)
        {
            return "join to rank";
        }

        return $"rank {WeeklyRank}, {WeeklyPoints} pts";
    }

    public string BuildChatPreview()
    {
        if (!IsJoined)
        {
            return "Chat opens after joining a Coven.";
        }

        return string.Join("\n", chatLines);
    }

    public void AddPrototypeChatLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        chatLines.Insert(0, line);
        while (chatLines.Count > 4)
        {
            chatLines.RemoveAt(chatLines.Count - 1);
        }
    }

    public CovenMemberInfo GetMember(string memberName)
    {
        for (int index = 0; index < members.Count; index++)
        {
            if (members[index].Name == memberName)
            {
                return members[index];
            }
        }

        return null;
    }

    public bool FulfillIngredientWish(string memberName, string ingredientName, int quantity)
    {
        if (quantity <= 0)
        {
            return false;
        }

        CovenMemberInfo member = GetMember(memberName);
        if (member == null)
        {
            return false;
        }

        CovenWishListRequest request = member.FindIngredientWish(ingredientName);
        if (request == null || request.RemainingQuantity <= 0)
        {
            return false;
        }

        int giftedQuantity = Mathf.Min(quantity, request.RemainingQuantity);
        request.FulfilledQuantity += giftedQuantity;
        inboxGifts.Insert(0, new CovenInboxGiftInfo(memberName, "Ingredient", ingredientName, giftedQuantity));
        AddPrototypeChatLine($"You sent {memberName}: {ingredientName} x{giftedQuantity}");
        return true;
    }

    public bool FulfillRegularCardWish(string memberName)
    {
        CovenMemberInfo member = GetMember(memberName);
        if (member == null || member.CardWish == null || member.CardWish.RemainingQuantity <= 0)
        {
            return false;
        }

        member.CardWish.FulfilledQuantity = 1;
        inboxGifts.Insert(0, new CovenInboxGiftInfo(memberName, "Regular Card", member.CardWish.ItemName, 1));
        AddPrototypeChatLine($"You sent {memberName}: {member.CardWish.ItemName}");
        return true;
    }

    public void RecordLibraryCardGift(string recipientName, string cardId)
    {
        string cardName = CardAlbumCatalog.GetGrimoireCardDisplayName(cardId);
        inboxGifts.Insert(0, new CovenInboxGiftInfo(recipientName, "Grimoire Card", cardId, 1));
        AddPrototypeChatLine($"You sent {recipientName}: {cardName}");
    }

    public void EnqueueCovenGiftForPlayer(CovenInboxGiftInfo gift, string senderName)
    {
        playerInboxItems.Insert(0, PrototypeInboxItem.FromCovenGift(gift, senderName));
        SyncLegacyPlayerInboxGifts();
        SavePlayerInboxItems();
    }

    public void EnqueueLibraryCardGiftForPlayer(string senderName, string cardId)
    {
        EnqueueCovenGiftForPlayer(new CovenInboxGiftInfo("You", "Grimoire Card", cardId, 1), senderName);
    }

    public void EnqueueSystemCompensationReward(string title, string detail, PrototypeRewardGrant reward, DateTime? expiresAtUtc)
    {
        playerInboxItems.Insert(0, PrototypeInboxItem.FromRewardGrant(PrototypeInboxCategory.Messages, title, detail, reward, "System", expiresAtUtc));
        SyncLegacyPlayerInboxGifts();
        SavePlayerInboxItems();
    }

    public bool TryTakePlayerInboxGift(int index, out CovenInboxGiftInfo gift)
    {
        gift = default;
        if (index < 0)
        {
            return false;
        }

        int giftIndex = 0;
        for (int itemIndex = 0; itemIndex < playerInboxItems.Count; itemIndex++)
        {
            if (!playerInboxItems[itemIndex].TryGetCovenGift(out CovenInboxGiftInfo candidate))
            {
                continue;
            }

            if (giftIndex == index)
            {
                gift = candidate;
                playerInboxItems.RemoveAt(itemIndex);
                SyncLegacyPlayerInboxGifts();
                SavePlayerInboxItems();
                return true;
            }

            giftIndex++;
        }

        return false;
    }

    public bool TryTakePlayerInboxItem(int index, out PrototypeInboxItem item)
    {
        item = default;
        if (index < 0 || index >= playerInboxItems.Count)
        {
            return false;
        }

        item = playerInboxItems[index];
        playerInboxItems.RemoveAt(index);
        SyncLegacyPlayerInboxGifts();
        SavePlayerInboxItems();
        return true;
    }

    public bool CanAcceptJoinRequests()
    {
        return IsJoined && members.Count < CovenMemberCap;
    }

    public bool AcceptJoinRequest(string applicantName)
    {
        int requestIndex = FindJoinRequestIndex(applicantName);
        if (!CanAcceptJoinRequests() || requestIndex < 0)
        {
            return false;
        }

        CovenJoinRequestInfo request = joinRequests[requestIndex];
        joinRequests.RemoveAt(requestIndex);
        members.Add(new CovenMemberInfo(request.Name, "Online now", request.Summary, request.WishListItem));
        AddPrototypeChatLine($"{request.Name} joined the Coven.");
        Save();
        return true;
    }

    public bool DenyJoinRequest(string applicantName)
    {
        int requestIndex = FindJoinRequestIndex(applicantName);
        if (!IsJoined || requestIndex < 0)
        {
            return false;
        }

        CovenJoinRequestInfo request = joinRequests[requestIndex];
        joinRequests.RemoveAt(requestIndex);
        AddPrototypeChatLine($"{request.Name}'s join request was denied.");
        Save();
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SavePrefix + "IsJoined", IsJoined ? 1 : 0);
        PlayerPrefs.SetString(SavePrefix + "CovenName", CovenName);
        PlayerPrefs.SetString(SavePrefix + "LeaderTitle", LeaderTitle);
        PlayerPrefs.SetString(SavePrefix + "LeaderName", LeaderName);
        PlayerPrefs.SetInt(SavePrefix + "WeeklyOrbGoal", WeeklyOrbGoal);
        PlayerPrefs.SetInt(SavePrefix + "TeamOrbProgress", TeamOrbProgress);
        PlayerPrefs.SetInt(SavePrefix + "WeeklyRank", WeeklyRank);
        PlayerPrefs.SetInt(SavePrefix + "WeeklyPoints", WeeklyPoints);
        PlayerPrefs.Save();
    }

    private void SeedPrototypeMembers()
    {
        members.Clear();
        members.Add(new CovenMemberInfo(
            LeaderName,
            "Online now",
            "42 gifts",
            new CovenWishListRequest("Regular Card", "Pack 01 Card 01", 1),
            new List<CovenWishListRequest>
            {
                new CovenWishListRequest("Ingredient", "Fountain Dewdrops", 4),
                new CovenWishListRequest("Ingredient", "Spire Pollen Grains", 3),
                new CovenWishListRequest("Ingredient", "Nectarflow Ribbons", 3),
            }));
        members.Add(new CovenMemberInfo(
            "Sol",
            "Last online 18m",
            "31 bingos",
            new CovenWishListRequest("Regular Card", "Pack 02 Card 03", 1),
            new List<CovenWishListRequest>
            {
                new CovenWishListRequest("Ingredient", "Velvet Thorn Tips", 5),
                new CovenWishListRequest("Ingredient", "Terrace Moss Tufts", 3),
                new CovenWishListRequest("Ingredient", "Rosegold Sap Drops", 2),
            }));
        members.Add(new CovenMemberInfo(
            "Juniper",
            "Last online 2h",
            "18 restores",
            new CovenWishListRequest("Regular Card", "Pack 03 Card 05", 1),
            new List<CovenWishListRequest>
            {
                new CovenWishListRequest("Ingredient", "Gilded Azalea Petals", 6),
                new CovenWishListRequest("Ingredient", "Sunwarm Dewdrops", 4),
            }));
    }

    private void SeedPrototypeJoinRequests()
    {
        joinRequests.Clear();
        joinRequests.Add(new CovenJoinRequestInfo("Aster", "Level 18 | daily player", "Sunwarm Dewdrops"));
        joinRequests.Add(new CovenJoinRequestInfo("Rowan", "Level 22 | gifts often", "Spire Pollen Grains"));
    }

    private void SeedPrototypeChat()
    {
        chatLines.Clear();
        chatLines.Add("Mira: Need Fountain Dewdrops");
        chatLines.Add("Sol: Sent gifts for today");
        chatLines.Add("Juniper: Saving orbs for weekly call");
    }

    private void SeedPrototypePlayerInbox()
    {
        if (PlayerPrefs.GetInt(InboxInitializedKey, 0) == 1)
        {
            SyncLegacyPlayerInboxGifts();
            return;
        }

        EnqueueCovenGiftForPlayer(new CovenInboxGiftInfo("You", "Ingredient", "Sunwarm Dewdrops", 1), "Juniper");
        EnqueueCovenGiftForPlayer(new CovenInboxGiftInfo("You", "Regular Card", "Pack 01 Card 02", 1), "Mira");
        PlayerPrefs.SetInt(InboxInitializedKey, 1);
        SavePlayerInboxItems();
        PlayerPrefs.Save();
    }

    private void LoadPlayerInboxItems()
    {
        playerInboxItems.Clear();
        string json = PlayerPrefs.GetString(InboxItemsJsonKey, "");
        if (string.IsNullOrWhiteSpace(json))
        {
            SyncLegacyPlayerInboxGifts();
            return;
        }

        PrototypeInboxSaveData data = JsonUtility.FromJson<PrototypeInboxSaveData>(json);
        if (data == null || data.Items == null)
        {
            SyncLegacyPlayerInboxGifts();
            return;
        }

        for (int index = 0; index < data.Items.Count; index++)
        {
            PrototypeInboxItem item = data.Items[index].ToInboxItem();
            if (item.SourceLabel == "Freebies" || item.Reward.Source == "Freebies" || item.Reward.Source == "Social Freebie")
            {
                continue;
            }

            playerInboxItems.Add(item);
        }

        SyncLegacyPlayerInboxGifts();
    }

    private void SavePlayerInboxItems()
    {
        PrototypeInboxSaveData data = new PrototypeInboxSaveData();
        for (int index = 0; index < playerInboxItems.Count; index++)
        {
            data.Items.Add(PrototypeInboxSavedItem.FromInboxItem(playerInboxItems[index]));
        }

        PlayerPrefs.SetString(InboxItemsJsonKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private int CountUnreadPlayerInboxItems()
    {
        int count = 0;
        for (int index = 0; index < playerInboxItems.Count; index++)
        {
            if (playerInboxItems[index].IsUnread)
            {
                count++;
            }
        }

        return count;
    }

    private void SyncLegacyPlayerInboxGifts()
    {
        playerInboxGifts.Clear();
        for (int index = 0; index < playerInboxItems.Count; index++)
        {
            if (playerInboxItems[index].TryGetCovenGift(out CovenInboxGiftInfo gift))
            {
                playerInboxGifts.Add(gift);
            }
        }
    }

    private int FindJoinRequestIndex(string applicantName)
    {
        for (int index = 0; index < joinRequests.Count; index++)
        {
            if (joinRequests[index].Name == applicantName)
            {
                return index;
            }
        }

        return -1;
    }
}

public readonly struct CovenJoinRequestInfo
{
    public CovenJoinRequestInfo(string name, string summary, string wishListItem)
    {
        Name = name;
        Summary = summary;
        WishListItem = wishListItem;
    }

    public string Name { get; }
    public string Summary { get; }
    public string WishListItem { get; }

    public string GetSummaryText()
    {
        return $"{Summary} | wish: {WishListItem}";
    }
}

public sealed class CovenMemberInfo
{
    private readonly List<CovenWishListRequest> ingredientWishes;

    public CovenMemberInfo(string name, string lastOnline, string stats, CovenWishListRequest cardWish, List<CovenWishListRequest> ingredientWishes)
    {
        Name = name;
        LastOnline = lastOnline;
        Stats = stats;
        CardWish = cardWish;
        this.ingredientWishes = ingredientWishes ?? new List<CovenWishListRequest>();
    }

    public string Name { get; }
    public string LastOnline { get; }
    public string Stats { get; }
    public CovenWishListRequest CardWish { get; }
    public IReadOnlyList<CovenWishListRequest> IngredientWishes => ingredientWishes;

    public CovenMemberInfo(string name, string lastOnline, string stats, string wishListItem)
        : this(
            name,
            lastOnline,
            stats,
            new CovenWishListRequest("Regular Card", $"{wishListItem} Card", 1),
            new List<CovenWishListRequest> { new CovenWishListRequest("Ingredient", wishListItem, 3) })
    {
    }

    public string GetSummaryText()
    {
        return $"{LastOnline} | {Stats} | wish list";
    }

    public CovenWishListRequest FindIngredientWish(string ingredientName)
    {
        for (int index = 0; index < IngredientWishes.Count; index++)
        {
            if (IngredientWishes[index].ItemName == ingredientName)
            {
                return IngredientWishes[index];
            }
        }

        return null;
    }
}

public sealed class CovenWishListRequest
{
    public CovenWishListRequest(string itemType, string itemName, int requestedQuantity)
    {
        ItemType = itemType;
        ItemName = itemName;
        RequestedQuantity = Mathf.Max(1, requestedQuantity);
    }

    public string ItemType { get; }
    public string ItemName { get; }
    public int RequestedQuantity { get; }
    public int FulfilledQuantity { get; set; }
    public int RemainingQuantity => Mathf.Max(0, RequestedQuantity - FulfilledQuantity);
    public bool IsFulfilled => RemainingQuantity <= 0;
}

public readonly struct CovenInboxGiftInfo
{
    public CovenInboxGiftInfo(string recipientName, string itemType, string itemName, int quantity)
    {
        RecipientName = recipientName;
        ItemType = itemType;
        ItemName = itemName;
        Quantity = quantity;
    }

    public string RecipientName { get; }
    public string ItemType { get; }
    public string ItemName { get; }
    public int Quantity { get; }
}

public enum PrototypeInboxCategory
{
    Messages,
    Cards,
    Gifts
}

public readonly struct PrototypeInboxItem
{
    public PrototypeInboxItem(
        string id,
        PrototypeInboxCategory category,
        string title,
        string detail,
        PrototypeRewardGrant reward,
        string sourceLabel,
        bool isUnread,
        long createdAtUtcTicks,
        long expiresAtUtcTicks,
        CovenInboxGiftInfo? covenGift)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id;
        Category = category;
        Title = title;
        Detail = detail;
        Reward = reward;
        SourceLabel = sourceLabel;
        IsUnread = isUnread;
        CreatedAtUtcTicks = createdAtUtcTicks;
        ExpiresAtUtcTicks = expiresAtUtcTicks;
        CovenGift = covenGift;
    }

    public string Id { get; }
    public PrototypeInboxCategory Category { get; }
    public string Title { get; }
    public string Detail { get; }
    public PrototypeRewardGrant Reward { get; }
    public string SourceLabel { get; }
    public bool IsUnread { get; }
    public long CreatedAtUtcTicks { get; }
    public long ExpiresAtUtcTicks { get; }
    public CovenInboxGiftInfo? CovenGift { get; }
    public bool HasExpiration => ExpiresAtUtcTicks > 0;

    public static PrototypeInboxItem FromCovenGift(CovenInboxGiftInfo gift, string senderName)
    {
        bool isCardGift = gift.ItemType == "Regular Card" || gift.ItemType == "Grimoire Card";
        PrototypeInboxCategory category = isCardGift ? PrototypeInboxCategory.Cards : PrototypeInboxCategory.Gifts;
        string displayName = gift.ItemType == "Grimoire Card" ? CardAlbumCatalog.GetGrimoireCardDisplayName(gift.ItemName) : gift.ItemName;
        string grantItemName = gift.ItemType == "Ingredient"
            ? gift.ItemName
            : gift.ItemType == "Regular Card"
                ? "Regular Card"
                : gift.ItemType == "Grimoire Card"
                    ? CardAlbumCatalog.SpecificGrimoireCardRewardPrefix + gift.ItemName
                    : gift.ItemName;
        return new PrototypeInboxItem(
            Guid.NewGuid().ToString("N"),
            category,
            $"{gift.ItemType}: {displayName} x{gift.Quantity}",
            $"Coven gift from {senderName}",
            new PrototypeRewardGrant("Coven fulfillment", 0, 0, 0, 0, new List<PrototypeRewardItem> { new PrototypeRewardItem(grantItemName, gift.Quantity) }),
            "Coven",
            true,
            DateTime.UtcNow.Ticks,
            0,
            gift);
    }

    public static PrototypeInboxItem FromRewardGrant(PrototypeInboxCategory category, string title, string detail, PrototypeRewardGrant reward, string sourceLabel, DateTime? expiresAtUtc)
    {
        return new PrototypeInboxItem(
            Guid.NewGuid().ToString("N"),
            category,
            title,
            detail,
            reward,
            sourceLabel,
            true,
            DateTime.UtcNow.Ticks,
            expiresAtUtc.HasValue ? expiresAtUtc.Value.Ticks : 0,
            null);
    }

    public bool TryGetCovenGift(out CovenInboxGiftInfo gift)
    {
        if (CovenGift.HasValue)
        {
            gift = CovenGift.Value;
            return true;
        }

        gift = default;
        return false;
    }
}

[Serializable]
public sealed class PrototypeInboxSaveData
{
    public List<PrototypeInboxSavedItem> Items = new List<PrototypeInboxSavedItem>();
}

[Serializable]
public sealed class PrototypeInboxSavedItem
{
    public string Id;
    public int Category;
    public string Title;
    public string Detail;
    public string SourceLabel;
    public bool IsUnread;
    public long CreatedAtUtcTicks;
    public long ExpiresAtUtcTicks;
    public string RewardSource;
    public int Mana;
    public int Crystals;
    public int ClairvoyanceMinutes;
    public int JackpotSpins;
    public List<PrototypeInboxSavedRewardItem> Items = new List<PrototypeInboxSavedRewardItem>();
    public bool HasCovenGift;
    public string CovenGiftRecipientName;
    public string CovenGiftItemType;
    public string CovenGiftItemName;
    public int CovenGiftQuantity;

    public static PrototypeInboxSavedItem FromInboxItem(PrototypeInboxItem item)
    {
        PrototypeInboxSavedItem saved = new PrototypeInboxSavedItem
        {
            Id = item.Id,
            Category = (int)item.Category,
            Title = item.Title,
            Detail = item.Detail,
            SourceLabel = item.SourceLabel,
            IsUnread = item.IsUnread,
            CreatedAtUtcTicks = item.CreatedAtUtcTicks,
            ExpiresAtUtcTicks = item.ExpiresAtUtcTicks,
            RewardSource = item.Reward.Source,
            Mana = item.Reward.Mana,
            Crystals = item.Reward.Crystals,
            ClairvoyanceMinutes = item.Reward.ClairvoyanceMinutes,
            JackpotSpins = item.Reward.JackpotSpins
        };

        for (int index = 0; index < item.Reward.Items.Count; index++)
        {
            PrototypeRewardItem rewardItem = item.Reward.Items[index];
            saved.Items.Add(new PrototypeInboxSavedRewardItem
            {
                Name = rewardItem.Name,
                Quantity = rewardItem.Quantity
            });
        }

        if (item.TryGetCovenGift(out CovenInboxGiftInfo gift))
        {
            saved.HasCovenGift = true;
            saved.CovenGiftRecipientName = gift.RecipientName;
            saved.CovenGiftItemType = gift.ItemType;
            saved.CovenGiftItemName = gift.ItemName;
            saved.CovenGiftQuantity = gift.Quantity;
        }

        return saved;
    }

    public PrototypeInboxItem ToInboxItem()
    {
        List<PrototypeRewardItem> rewardItems = new List<PrototypeRewardItem>();
        if (Items != null)
        {
            for (int index = 0; index < Items.Count; index++)
            {
                rewardItems.Add(new PrototypeRewardItem(Items[index].Name, Items[index].Quantity));
            }
        }

        CovenInboxGiftInfo? covenGift = null;
        if (HasCovenGift)
        {
            covenGift = new CovenInboxGiftInfo(CovenGiftRecipientName, CovenGiftItemType, CovenGiftItemName, CovenGiftQuantity);
        }

        PrototypeInboxCategory category = Enum.IsDefined(typeof(PrototypeInboxCategory), Category)
            ? (PrototypeInboxCategory)Category
            : PrototypeInboxCategory.Gifts;
        return new PrototypeInboxItem(
            Id,
            category,
            Title,
            Detail,
            new PrototypeRewardGrant(RewardSource, Mana, Crystals, ClairvoyanceMinutes, JackpotSpins, rewardItems),
            SourceLabel,
            IsUnread,
            CreatedAtUtcTicks,
            ExpiresAtUtcTicks,
            covenGift);
    }
}

[Serializable]
public sealed class PrototypeInboxSavedRewardItem
{
    public string Name;
    public int Quantity;
}

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

// Coven Circle, discovery, member detail and Coven Emporium screens. Coven rules and saves live in CovenState.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
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

        CreateDenReturnFooter(panel.transform, 0f, -326f, BuildPlayerDenUi);
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
        Text nameText = CreateAnchoredText(row.transform, name, 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 168, 20, -46f, 16f);
        Text summaryText = CreateAnchoredText(row.transform, $"{summary}\n{note}", 9, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 168, 32, -46f, -10f);
        Button action = CreateAnchoredButton(row.transform, name == "Moonpetal Circle" ? "Join" : requested ? "Cancel" : "Request", 10, 72, 28, requested ? new Color(0.42f, 0.12f, 0.18f) : new Color(0.12f, 0.55f, 0.08f), 88f, 0f);
        CovenDiscoveryRowView view = row.GetComponent<CovenDiscoveryRowView>();
        if (view == null)
        {
            view = row.AddComponent<CovenDiscoveryRowView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(nameText, summaryText, action);
        view.Apply(new CovenDiscoveryRowDisplayModel
        {
            NameText = name,
            SummaryText = $"{summary}\n{note}",
            ActionButtonText = name == "Moonpetal Circle" ? "Join" : requested ? "Cancel" : "Request"
        });
        if (name == "Moonpetal Circle")
        {
            view.ActionRequested += JoinPrototypeCovenFromDiscovery;
        }
        else
        {
            view.ActionRequested += () => TogglePrototypeCovenJoinRequest(name);
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
        string saved = DurablePrefs.GetString(PrototypeCovenDiscoverySaveKey, "");
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
        DurablePrefs.SetString(PrototypeCovenDiscoverySaveKey, string.Join(PrototypeFriendSaveSeparator.ToString(), names));
        DurablePrefs.Save();
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
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.CovenOrbsContributed,
                PrototypeAnalyticsPayloadFactory.CreateCovenOrbsContributedJson(
                    contributed,
                    inventory.GetInventoryRewardCount("Club Orbs")));
        }

        BuildCovenCircleUi();
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
        Text titleText = CreateAnchoredText(row.transform, title, 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 20, 0f, 8f);
        Text noteText = CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 250, 16, 0f, -12f);
        UITitleNoteRowView rowView = row.GetComponent<UITitleNoteRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<UITitleNoteRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, noteText);
        rowView.Apply(title, note);
    }

    private void CreateCovenLeadershipField(Transform parent, string title, string note, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenLeadership_{title}", new Color(0.96f, 0.86f, 0.62f), 280, 50, x, y);
        Text titleText = CreateAnchoredText(row.transform, title, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 20, 0f, 10f);
        Text noteText = CreateAnchoredText(row.transform, note, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 250, 18, 0f, -13f);
        UITitleNoteRowView rowView = row.GetComponent<UITitleNoteRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<UITitleNoteRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, noteText);
        rowView.Apply(title, note);
    }

    private void CreateCovenJoinRequestRow(Transform parent, CovenJoinRequestInfo request, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenJoinRequest_{request.Name}", new Color(0.96f, 0.86f, 0.62f), 310, 54, x, y);
        Text nameText = CreateAnchoredText(row.transform, request.Name, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 120, 20, -82f, 10f);
        Text summaryText = CreateAnchoredText(row.transform, request.Summary, 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 154, 18, -64f, -12f);
        Button accept = CreateAnchoredButton(row.transform, "Accept", 11, 62, 26, new Color(0.12f, 0.55f, 0.08f), 70f, 10f);
        Button deny = CreateAnchoredButton(row.transform, "Deny", 11, 62, 26, new Color(0.42f, 0.12f, 0.18f), 70f, -18f);
        CovenJoinRequestRowView view = row.GetComponent<CovenJoinRequestRowView>();
        if (view == null)
        {
            view = row.AddComponent<CovenJoinRequestRowView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(nameText, summaryText, accept, deny);
        view.Apply(new CovenJoinRequestRowDisplayModel
        {
            NameText = request.Name,
            SummaryText = request.Summary,
            AcceptButtonText = "Accept",
            CanAccept = coven.CanAcceptJoinRequests(),
            DenyButtonText = "Deny",
            CanDeny = true
        });
        view.AcceptRequested += () => AcceptCovenJoinRequest(request.Name);
        view.DenyRequested += () => DenyCovenJoinRequest(request.Name);
    }

    private void CreateCovenMemberRow(Transform parent, CovenMemberInfo member, float x, float y)
    {
        GameObject row = CreateAnchoredPanel(parent, $"CovenMember_{member.Name}", new Color(0.96f, 0.86f, 0.62f), 250, 46, x, y);
        Button button = row.AddComponent<Button>();
        Text nameText = CreateAnchoredText(row.transform, member.Name, 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 210, 22, 0f, 9f);
        Text summaryText = CreateAnchoredText(row.transform, member.GetSummaryText(), 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 216, 18, 0f, -13f);
        CovenMemberRowView view = row.GetComponent<CovenMemberRowView>();
        if (view == null)
        {
            view = row.AddComponent<CovenMemberRowView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(nameText, summaryText, button);
        view.Apply(new CovenMemberRowDisplayModel
        {
            NameText = member.Name,
            SummaryText = member.GetSummaryText()
        });
        view.OpenRequested += () => BuildCovenMemberDetailUi(member);
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
        Text profileSummaryText = CreateAnchoredText(profile.transform, $"{currentMember.LastOnline}\n{currentMember.Stats}", 21, FontStyle.Bold, Color.white, 250, 76, 0f, 78f);
        Text profileNotesText = CreateAnchoredText(profile.transform, "Wish lists are visible to covenmates only.\nFulfilled gifts are sent to this member's inbox for collection.", 15, FontStyle.Bold, muted, 250, 104, 0f, -34f);
        Text profileQueueText = CreateAnchoredText(profile.transform, $"Inbox gifts queued: {coven.InboxGifts.Count}", 15, FontStyle.Bold, gold, 250, 26, 0f, -124f);
        CovenMemberProfileView profileView = profile.GetComponent<CovenMemberProfileView>();
        if (profileView == null)
        {
            profileView = profile.AddComponent<CovenMemberProfileView>();
        }

        profileView.ResetRuntimeBindings();
        profileView.Initialize(profileSummaryText, profileNotesText, profileQueueText);
        profileView.Apply(new CovenMemberProfileDisplayModel
        {
            SummaryText = $"{currentMember.LastOnline}\n{currentMember.Stats}",
            NotesText = "Wish lists are visible to covenmates only.\nFulfilled gifts are sent to this member's inbox for collection.",
            QueueText = $"Inbox gifts queued: {coven.InboxGifts.Count}"
        });

        GameObject wish = CreateAnchoredPanel(panel.transform, "CovenMemberWishTab", new Color(0.96f, 0.86f, 0.62f), 330, 300, 190f, -20f);
        CreateAnchoredText(wish.transform, "Refreshes in 48h prototype window", 13, FontStyle.Bold, ink, 290, 22, 0f, 126f);
        if (currentMember.CardWish != null)
        {
            int regularCards = inventory.GetInventoryRewardCount("Regular Card");
            string cardStatus = currentMember.CardWish.IsFulfilled ? "Fulfilled - inbox pending" : $"{currentMember.CardWish.ItemName}\nYou own regular cards: {regularCards}";
            GameObject cardRow = CreateAnchoredPanel(wish.transform, "CovenCardWish", new Color(0.18f, 0.13f, 0.2f), 290, 58, 0f, 84f);
            Text cardStatusText = CreateAnchoredText(cardRow.transform, cardStatus, 12, FontStyle.Bold, Color.white, 188, 46, -40f, 0f);
            Button cardGift = CreateAnchoredButton(cardRow.transform, currentMember.CardWish.IsFulfilled ? "Sent" : "Gift Card", 11, 78, 30, regularCards > 0 && !currentMember.CardWish.IsFulfilled ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 98f, 0f);
            CovenWishCardRowView cardRowView = cardRow.GetComponent<CovenWishCardRowView>();
            if (cardRowView == null)
            {
                cardRowView = cardRow.AddComponent<CovenWishCardRowView>();
            }

            cardRowView.ResetRuntimeBindings();
            cardRowView.Initialize(cardStatusText, cardGift);
            cardRowView.Apply(new CovenWishCardRowDisplayModel
            {
                StatusText = cardStatus,
                ActionButtonText = currentMember.CardWish.IsFulfilled ? "Sent" : "Gift Card",
                ActionInteractable = regularCards > 0 && !currentMember.CardWish.IsFulfilled
            });
            cardRowView.ActionRequested += () => GiftCovenMemberCardWish(currentMember.Name);
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
        Text labelText = CreateAnchoredText(row.transform, label, 10, FontStyle.Bold, Color.white, 190, 34, -42f, 0f);
        Button gift = CreateAnchoredButton(row.transform, request.IsFulfilled ? "Sent" : "Gift 1", 10, 72, 26, canGift ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 98f, 0f);
        CovenWishIngredientRowView rowView = row.GetComponent<CovenWishIngredientRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<CovenWishIngredientRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, gift);
        rowView.Apply(new CovenWishIngredientRowDisplayModel
        {
            StatusText = label,
            ActionButtonText = request.IsFulfilled ? "Sent" : "Gift 1",
            ActionInteractable = canGift
        });
        rowView.ActionRequested += () => GiftCovenMemberIngredientWish(memberName, request.ItemName);
    }

    private void GiftCovenMemberCardWish(string memberName)
    {
        if (inventory.TryConsumeInventoryReward("Regular Card", 1) && coven.FulfillRegularCardWish(memberName))
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.CovenWishGiftSent,
                PrototypeAnalyticsPayloadFactory.CreateCovenWishGiftSentJson(
                    "regular_card",
                    "Regular Card",
                    1,
                    inventory.GetInventoryRewardCount("Regular Card")));
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
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.CovenWishGiftSent,
                PrototypeAnalyticsPayloadFactory.CreateCovenWishGiftSentJson(
                    "ingredient",
                    ingredientName,
                    1,
                    inventory.GetIngredientInventoryCount(ingredientName)));
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
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.CovenEmporiumPurchase,
            PrototypeAnalyticsPayloadFactory.CreateCovenEmporiumPurchaseJson(
                "small_power_up_bundle",
                "Small Power-Up Bundle",
                5,
                inventory.GetInventoryRewardCount("Club Orbs"),
                itemName: "Single Sigil + Multi Sigil",
                itemQuantity: 2));
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

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.CovenEmporiumPurchase,
            PrototypeAnalyticsPayloadFactory.CreateCovenEmporiumPurchaseJson(
                "ingredient_gift_crate",
                "Ingredient Gift Crate",
                8,
                inventory.GetInventoryRewardCount("Club Orbs"),
                itemName: requirements.Count > 0 ? "active_room_ingredient_bundle" : "",
                itemQuantity: requirements.Count > 0 ? 2 + (requirements.Count > 1 ? 1 : 0) : 0));
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
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.CovenEmporiumPurchase,
            PrototypeAnalyticsPayloadFactory.CreateCovenEmporiumPurchaseJson(
                "coven_chest",
                "Coven Chest",
                12,
                inventory.GetInventoryRewardCount("Club Orbs"),
                crystals: 5,
                itemName: "Pandora Sigil",
                itemQuantity: 1));
        coven.AddPrototypeChatLine("You bought a Coven Chest.");
        BuildCovenEmporiumUi();
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
}

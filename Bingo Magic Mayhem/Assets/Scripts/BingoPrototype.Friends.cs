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

// Friends screen, friend requests, mana gifts and the message composer. Friend rules and saves live in FriendsState;
// this part of BingoPrototype only builds the UI. Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildFriendsUi()
    {
        friendsState.RefreshDailyMana();
        RemoveFriendsModal();
        GameObject panel = CreateModalShell("FriendsModal", 1080, 640, new Color(0.055f, 0.028f, 0.085f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "FRIENDS",
            "Add/manage friends, daily 10-mana social taps, and message actions. Messages route to Inbox > Messages.",
            "Back to Den",
            gold,
            muted,
            violet,
            900f,
            266f,
            900f,
            36f,
            224f);
        chromeView.CloseRequested += RemoveFriendsModal;
        Text summaryText = CreateAnchoredText(panel.transform, string.Empty, 14, FontStyle.Bold, muted, 900, 34, 0f, 194f);

        GameObject listPanel = CreateModalSectionPanel(panel.transform, "FriendsList", 330, 358, -346f, 18f);
        Text listPanelTitleText = CreateModalSectionHeader(listPanel.transform, "FRIEND LIST", gold, 290, 0f, 142f);
        List<string> friends = new List<string>(friendsState.Friends);
        friends.Sort();
        FriendsSectionPanelView listPanelView = listPanel.GetComponent<FriendsSectionPanelView>();
        if (listPanelView == null)
        {
            listPanelView = listPanel.AddComponent<FriendsSectionPanelView>();
        }

        listPanelView.ResetRuntimeBindings();
        listPanelView.Initialize(listPanelTitleText, null);
        listPanelView.Apply(new FriendsSectionPanelDisplayModel("FRIEND LIST", string.Empty));
        for (int index = 0; index < friends.Count && index < 4; index++)
        {
            string friendName = friends[index];
            CreateFriendListRow(listPanel.transform, friendName, index % 2 == 0 ? "Online" : "Away", 78f - index * 76f);
        }

        GameObject incomingPanel = CreateModalSectionPanel(panel.transform, "FriendIncoming", 330, 358, 0f, 18f, new Color(0.12f, 0.055f, 0.16f));
        Text incomingTitleText = CreateModalSectionHeader(incomingPanel.transform, "REQUESTS", gold, 290, 0f, 142f);
        List<string> incoming = new List<string>(friendsState.IncomingRequests);
        incoming.Sort();
        Text incomingEmptyStateText = null;
        if (incoming.Count == 0)
        {
            incomingEmptyStateText = CreateAnchoredText(incomingPanel.transform, "No incoming requests.", 16, FontStyle.Bold, Color.white, 260, 36, 0f, 62f);
        }

        FriendsSectionPanelView incomingPanelView = incomingPanel.GetComponent<FriendsSectionPanelView>();
        if (incomingPanelView == null)
        {
            incomingPanelView = incomingPanel.AddComponent<FriendsSectionPanelView>();
        }

        incomingPanelView.ResetRuntimeBindings();
        incomingPanelView.Initialize(incomingTitleText, incomingEmptyStateText);
        incomingPanelView.Apply(new FriendsSectionPanelDisplayModel("REQUESTS", incoming.Count == 0 ? "No incoming requests." : string.Empty));
        for (int index = 0; index < incoming.Count && index < 4; index++)
        {
            CreateIncomingFriendRequestRow(incomingPanel.transform, incoming[index], 88f - index * 66f);
        }

        GameObject addPanel = CreateModalSectionPanel(panel.transform, "FriendAddManage", 330, 358, 346f, 18f, new Color(0.09f, 0.045f, 0.13f));
        Text addPanelTitleText = CreateModalSectionHeader(addPanel.transform, "ADD FRIEND", gold, 290, 0f, 142f);
        CreateModalInfoCard(addPanel.transform, "FriendSearchBox", 260, 46, 0f, 84f);
        CreateAnchoredText(addPanel.transform, "Player name search later", 15, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 238, 24, 0f, 84f);
        Button sendTest = CreateAnchoredButton(addPanel.transform, "Send Test Request", 16, 230, 42, violet, 0f, 28f);
        sendTest.onClick.AddListener(SendPrototypeFriendRequest);
        CreateModalSectionHeader(addPanel.transform, "Sent Requests", gold, 260, 0f, -30f, 18, 28f);
        List<string> sent = new List<string>(friendsState.SentRequests);
        sent.Sort();
        Text sentEmptyStateText = null;
        if (sent.Count == 0)
        {
            sentEmptyStateText = CreateAnchoredText(addPanel.transform, "None pending.", 14, FontStyle.Bold, Color.white, 240, 26, 0f, -76f);
        }

        FriendsSectionPanelView addPanelView = addPanel.GetComponent<FriendsSectionPanelView>();
        if (addPanelView == null)
        {
            addPanelView = addPanel.AddComponent<FriendsSectionPanelView>();
        }

        addPanelView.ResetRuntimeBindings();
        addPanelView.Initialize(addPanelTitleText, sentEmptyStateText);
        addPanelView.Apply(new FriendsSectionPanelDisplayModel("ADD FRIEND", sent.Count == 0 ? "None pending." : string.Empty));
        for (int index = 0; index < sent.Count && index < 2; index++)
        {
            CreateSentFriendRequestRow(addPanel.transform, sent[index], -76f - index * 50f);
        }

        Text footerStatusText = CreateAnchoredText(panel.transform, string.Empty, 15, FontStyle.Bold, muted, 900, 34, 0f, -214f);
        FriendsModalView modalView = panel.GetComponent<FriendsModalView>();
        if (modalView == null)
        {
            modalView = panel.AddComponent<FriendsModalView>();
        }

        modalView.ResetRuntimeBindings();
        modalView.Initialize(summaryText, footerStatusText);
        modalView.Apply(new FriendsModalDisplayModel
        {
            SummaryText = BuildPrototypeFriendManaSummaryText(),
            FooterStatusText = string.IsNullOrEmpty(lastFriendStatusSummary) ? "Friend mana counts are prototype-only until Aura-rank limits are locked." : lastFriendStatusSummary
        });
    }

    private void CreateFriendListRow(Transform parent, string friendName, string status, float y)
    {
        bool blocked = friendsState.IsBlocked(friendName);
        GameObject row = CreateAnchoredPanel(parent, $"Friend_{friendName}", blocked ? new Color(0.32f, 0.27f, 0.32f) : new Color(0.96f, 0.86f, 0.62f), 276, 66, 0f, y);
        Color textColor = blocked ? new Color(0.82f, 0.78f, 0.84f) : new Color(0.18f, 0.08f, 0.32f);
        Color subTextColor = blocked ? new Color(0.66f, 0.62f, 0.7f) : new Color(0.28f, 0.18f, 0.32f);
        Text friendNameText = CreateAnchoredText(row.transform, friendName, 15, FontStyle.Bold, textColor, 104, 22, -76f, 16f);
        Text statusText = CreateAnchoredText(row.transform, blocked ? "Blocked" : status, 10, FontStyle.Bold, subTextColor, 104, 16, -76f, -4f);
        Text reportStatusText = CreateAnchoredText(row.transform, friendsState.IsReported(friendName) ? "Reported" : "", 9, FontStyle.Bold, new Color(0.55f, 0.09f, 0.09f), 104, 14, -76f, -22f);

        Button send = CreateAnchoredButton(row.transform, friendsState.HasSentManaTo(friendName) ? "Sent" : "Give 10", 9, 58, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.12f, 0.55f, 0.08f), -2f, 15f);
        bool canSend = friendsState.CanSendManaTo(friendName);
        send.interactable = canSend;
        Button receive = CreateAnchoredButton(row.transform, friendsState.HasReceivedManaFrom(friendName) ? "Got" : "Claim", 9, 54, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.12f, 0.55f, 0.08f), 60f, 15f);
        bool canReceive = friendsState.CanReceiveManaFrom(friendName);
        receive.interactable = canReceive;
        Button message = CreateAnchoredButton(row.transform, "Msg", 9, 48, 22, blocked ? new Color(0.34f, 0.32f, 0.38f) : new Color(0.35f, 0.12f, 0.62f), 116f, 15f);
        bool canMessage = !blocked;
        message.interactable = canMessage;
        Button block = CreateAnchoredButton(row.transform, blocked ? "Blocked" : "Block", 9, 62, 22, new Color(0.34f, 0.32f, 0.38f), 28f, -16f);
        bool canBlock = !blocked;
        block.interactable = canBlock;
        Button report = CreateAnchoredButton(row.transform, friendsState.IsReported(friendName) ? "Filed" : "Report", 9, 62, 22, new Color(0.55f, 0.09f, 0.09f), 96f, -16f);
        bool canReport = !friendsState.IsReported(friendName);
        report.interactable = canReport;

        FriendListRowView rowView = row.GetComponent<FriendListRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<FriendListRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(friendNameText, statusText, reportStatusText, send, receive, message, block, report);
        rowView.SendRequested += () => SendPrototypeFriendMana(friendName);
        rowView.ReceiveRequested += () => ReceivePrototypeFriendMana(friendName);
        rowView.MessageRequested += () => SendPrototypeFriendMessage(friendName);
        rowView.BlockRequested += () => BlockPrototypeFriend(friendName);
        rowView.ReportRequested += () => ReportPrototypeFriend(friendName);
        rowView.Apply(new FriendListRowDisplayModel(
            friendName,
            blocked ? "Blocked" : status,
            friendsState.IsReported(friendName) ? "Reported" : string.Empty,
            friendsState.HasSentManaTo(friendName) ? "Sent" : "Give 10",
            friendsState.HasReceivedManaFrom(friendName) ? "Got" : "Claim",
            "Msg",
            blocked ? "Blocked" : "Block",
            friendsState.IsReported(friendName) ? "Filed" : "Report",
            canSend,
            canReceive,
            canMessage,
            canBlock,
            canReport));
    }

    private string BuildPrototypeFriendManaSummaryText()
    {
        int sendRemaining = Mathf.Max(0, PrototypeFriendDailyManaLimit - friendsState.SentManaTodayCount);
        int receiveRemaining = Mathf.Max(0, PrototypeFriendDailyManaLimit - friendsState.ReceivedManaTodayCount);
        return $"Daily mana: give {PrototypeFriendManaAmount} to {sendRemaining} more friend(s), claim from {receiveRemaining} more friend(s). Current cap {PrototypeFriendDailyManaLimit}/day each way is a temporary Aura-rank placeholder.";
    }

    private void CreateIncomingFriendRequestRow(Transform parent, string friendName, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"IncomingFriend_{friendName}", 276, 58, 0f, y);
        Text friendNameText = CreateAnchoredText(row.transform, friendName, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 120, 24, -68f, 10f);
        Text detailText = CreateAnchoredText(row.transform, "Friend request", 10, FontStyle.Bold, new Color(0.28f, 0.18f, 0.32f), 120, 16, -68f, -12f);
        Button accept = CreateAnchoredButton(row.transform, "Accept", 10, 64, 26, new Color(0.12f, 0.55f, 0.08f), 42f, 0f);
        Button decline = CreateAnchoredButton(row.transform, "Decline", 10, 64, 26, new Color(0.34f, 0.32f, 0.38f), 110f, 0f);

        IncomingFriendRequestRowView rowView = row.GetComponent<IncomingFriendRequestRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<IncomingFriendRequestRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(friendNameText, detailText, accept, decline);
        rowView.AcceptRequested += () => AcceptPrototypeFriendRequest(friendName);
        rowView.DeclineRequested += () => DeclinePrototypeFriendRequest(friendName);
        rowView.Apply(new IncomingFriendRequestRowDisplayModel(friendName, "Friend request", "Accept", "Decline"));
    }

    private void CreateSentFriendRequestRow(Transform parent, string friendName, float y)
    {
        GameObject row = CreateModalSectionPanel(parent, $"SentFriend_{friendName}", 250, 38, 0f, y, new Color(0.18f, 0.13f, 0.2f));
        Text friendNameText = CreateAnchoredText(row.transform, friendName, 14, FontStyle.Bold, Color.white, 128, 20, -48f, 0f);
        Button cancel = CreateAnchoredButton(row.transform, "Cancel", 10, 70, 24, new Color(0.34f, 0.32f, 0.38f), 78f, 0f);

        SentFriendRequestRowView rowView = row.GetComponent<SentFriendRequestRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<SentFriendRequestRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(friendNameText, cancel);
        rowView.CancelRequested += () => CancelPrototypeFriendRequest(friendName);
        rowView.Apply(new SentFriendRequestRowDisplayModel(friendName, "Cancel"));
    }

    // Friends data and rules live in FriendsState; these handlers add the status text, analytics and UI refresh.
    private void ResetPrototypeFriendsState()
    {
        friendsState.ResetToPrototypeDefaults();
        lastFriendStatusSummary = "";
    }

    private void SendPrototypeFriendRequest()
    {
        string candidate = friendsState.SendRequestToNextCandidate();
        lastFriendStatusSummary = candidate != null
            ? $"Sent a prototype friend request to {candidate}."
            : "All prototype friend request candidates are already in use.";
        BuildFriendsUi();
    }

    private void AcceptPrototypeFriendRequest(string friendName)
    {
        friendsState.AcceptRequest(friendName);
        lastFriendStatusSummary = $"Accepted {friendName}'s friend request.";
        BuildFriendsUi();
    }

    private void DeclinePrototypeFriendRequest(string friendName)
    {
        friendsState.DeclineRequest(friendName);
        lastFriendStatusSummary = $"Declined {friendName}'s friend request.";
        BuildFriendsUi();
    }

    private void CancelPrototypeFriendRequest(string friendName)
    {
        friendsState.CancelSentRequest(friendName);
        lastFriendStatusSummary = $"Canceled friend request to {friendName}.";
        BuildFriendsUi();
    }

    private void RemovePrototypeFriend(string friendName)
    {
        friendsState.RemoveFriend(friendName);
        lastFriendStatusSummary = $"Removed {friendName} from friends.";
        BuildFriendsUi();
    }

    private void SendPrototypeFriendMana(string friendName)
    {
        switch (friendsState.TrySendMana(friendName))
        {
            case FriendManaResult.Blocked:
                lastFriendStatusSummary = $"{friendName} is blocked.";
                break;
            case FriendManaResult.LimitReached:
                lastFriendStatusSummary = "Daily friend mana send limit reached for this prototype.";
                break;
            default:
                lastFriendStatusSummary = $"Sent {PrototypeFriendManaAmount} mana to {friendName}. Prototype does not deduct player mana yet.";
                TrackPrototypeAnalytics(
                    PrototypeAnalyticsEvents.FriendManaSent,
                    PrototypeAnalyticsPayloadFactory.CreateFriendManaJson(
                        "friends_ui",
                        PrototypeFriendManaAmount,
                        friendsState.SentManaTodayCount,
                        PrototypeFriendDailyManaLimit));
                break;
        }

        BuildFriendsUi();
    }

    private void ReceivePrototypeFriendMana(string friendName)
    {
        switch (friendsState.TryReceiveMana(friendName))
        {
            case FriendManaResult.Blocked:
                lastFriendStatusSummary = $"{friendName} is blocked.";
                break;
            case FriendManaResult.LimitReached:
                lastFriendStatusSummary = "Daily friend mana receive limit reached for this prototype.";
                break;
            default:
                lastFriendStatusSummary = $"Received {PrototypeFriendManaAmount} mana from {friendName}. Inventory grant waits for Aura-rank limits to be locked.";
                TrackPrototypeAnalytics(
                    PrototypeAnalyticsEvents.FriendManaReceived,
                    PrototypeAnalyticsPayloadFactory.CreateFriendManaJson(
                        "friends_ui",
                        PrototypeFriendManaAmount,
                        friendsState.ReceivedManaTodayCount,
                        PrototypeFriendDailyManaLimit));
                break;
        }

        BuildFriendsUi();
    }

    private void SendPrototypeFriendMessage(string friendName)
    {
        if (friendsState.IsBlocked(friendName))
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

        GameObject panel = CreateModalShell("FriendMessageComposer", 700, 410, new Color(0.055f, 0.028f, 0.085f, 0.995f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            $"MESSAGE {friendName.ToUpperInvariant()}",
            "Prototype note only. Sent messages live in Inbox > Messages.",
            "Cancel",
            gold,
            muted,
            new Color(0.18f, 0.16f, 0.22f),
            620f,
            156f,
            620f,
            28f,
            118f,
            15,
            170f,
            44f,
            -110f,
            -150f,
            18);
        chromeView.CloseRequested += RemoveFriendMessageComposer;

        prototypeFriendMessageInput = CreateAnchoredInputField(panel.transform, "FriendMessageInput", "Type a short message...", 560, 96, 0f, 42f);
        prototypeFriendMessageInput.text = $"Hi {friendName}! ";

        Button help = CreateAnchoredButton(panel.transform, "Need help?", 13, 140, 30, violet, -170f, -38f);
        help.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Hi {friendName}! Could you help if you have extras today?"));
        Button thanks = CreateAnchoredButton(panel.transform, "Thanks!", 13, 140, 30, violet, 0f, -38f);
        thanks.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Thanks for the help, {friendName}!"));
        Button luck = CreateAnchoredButton(panel.transform, "Good luck", 13, 140, 30, violet, 170f, -38f);
        luck.onClick.AddListener(() => SetPrototypeFriendMessageDraft($"Good luck in your rooms today, {friendName}!"));

        prototypeFriendMessageStatusText = CreateAnchoredText(panel.transform, "", 14, FontStyle.Bold, new Color(1f, 0.58f, 0.58f), 560, 26, 0f, -82f);

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
        friendsState.Block(friendName);
        lastFriendStatusSummary = $"Blocked {friendName}. Prototype blocks friend mana and messaging actions.";
        BuildFriendsUi();
    }

    private void ReportPrototypeFriend(string friendName)
    {
        friendsState.Report(friendName);
        lastFriendStatusSummary = $"Report filed for {friendName}. Moderation workflow is placeholder-only.";
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
}

using System;
using System.Collections.Generic;
using BingoMagicMayhem.Infrastructure;

public enum FriendManaResult
{
    Done,
    Blocked,
    LimitReached
}

// Prototype friends data and rules: friend list, requests, blocks/reports and the daily friend-mana
// send/receive caps. Extracted from BingoPrototype; the Friends screen and its status text stay there.
// Save keys are unchanged from the original implementation so existing test saves keep loading.
// TODO(product): friend mana caps are a temporary stand-in until Aura-derived rank limits are approved.
public sealed class FriendsState
{
    public const string SavePrefix = "BMM.Prototype.Friends.";
    private const char Separator = '|';
    private static readonly string[] RequestCandidates = { "Selene", "Ivy", "Marisol" };

    private readonly HashSet<string> friends = new HashSet<string>();
    private readonly HashSet<string> incomingRequests = new HashSet<string>();
    private readonly HashSet<string> sentRequests = new HashSet<string>();
    private readonly HashSet<string> sentManaToday = new HashSet<string>();
    private readonly HashSet<string> receivedManaToday = new HashSet<string>();
    private readonly HashSet<string> blocked = new HashSet<string>();
    private readonly HashSet<string> reported = new HashSet<string>();
    private readonly Func<DateTime> utcNow;

    public FriendsState(Func<DateTime> utcNow = null)
    {
        this.utcNow = utcNow ?? (() => DateTime.UtcNow);
        SeedPrototypeDefaults();
    }

    public IReadOnlyCollection<string> Friends => friends;
    public IReadOnlyCollection<string> IncomingRequests => incomingRequests;
    public IReadOnlyCollection<string> SentRequests => sentRequests;
    public int SentManaTodayCount => sentManaToday.Count;
    public int ReceivedManaTodayCount => receivedManaToday.Count;
    public int DailyManaLimit => EconomyTuning.Current.Social.FriendDailyManaLimit;
    public int ManaAmount => EconomyTuning.Current.Social.FriendManaAmount;

    public bool IsBlocked(string name) => blocked.Contains(name);
    public bool IsReported(string name) => reported.Contains(name);
    public bool HasSentManaTo(string name) => sentManaToday.Contains(name);
    public bool HasReceivedManaFrom(string name) => receivedManaToday.Contains(name);
    public bool CanSendManaTo(string name) => !IsBlocked(name) && !HasSentManaTo(name) && sentManaToday.Count < DailyManaLimit;
    public bool CanReceiveManaFrom(string name) => !IsBlocked(name) && !HasReceivedManaFrom(name) && receivedManaToday.Count < DailyManaLimit;

    public void Load()
    {
        LoadSet("Friends", friends);
        LoadSet("IncomingRequests", incomingRequests);
        LoadSet("SentRequests", sentRequests);
        LoadSet("Blocked", blocked);
        LoadSet("Reported", reported);

        string today = TodayKey();
        if (DurablePrefs.GetString(SavePrefix + "DailyManaDate", "") == today)
        {
            LoadSet("SentManaToday", sentManaToday);
            LoadSet("ReceivedManaToday", receivedManaToday);
        }
        else
        {
            sentManaToday.Clear();
            receivedManaToday.Clear();
            Save();
        }
    }

    public void Save()
    {
        DurablePrefs.SetString(SavePrefix + "DailyManaDate", TodayKey());
        SaveSet("Friends", friends);
        SaveSet("IncomingRequests", incomingRequests);
        SaveSet("SentRequests", sentRequests);
        SaveSet("SentManaToday", sentManaToday);
        SaveSet("ReceivedManaToday", receivedManaToday);
        SaveSet("Blocked", blocked);
        SaveSet("Reported", reported);
        DurablePrefs.Save();
    }

    // Clears the daily mana caps when the UTC day has rolled over since they were last saved.
    public void RefreshDailyMana()
    {
        string today = TodayKey();
        if (DurablePrefs.GetString(SavePrefix + "DailyManaDate", today) == today)
        {
            return;
        }

        sentManaToday.Clear();
        receivedManaToday.Clear();
        Save();
    }

    public void ResetDailyMana()
    {
        sentManaToday.Clear();
        receivedManaToday.Clear();
        Save();
    }

    public void ResetToPrototypeDefaults()
    {
        SeedPrototypeDefaults();
        Save();
    }

    // Returns the name the request went to, or null when every prototype candidate is already in use.
    public string SendRequestToNextCandidate()
    {
        foreach (string candidate in RequestCandidates)
        {
            if (!friends.Contains(candidate) && !incomingRequests.Contains(candidate) && !sentRequests.Contains(candidate))
            {
                sentRequests.Add(candidate);
                Save();
                return candidate;
            }
        }

        return null;
    }

    public void AcceptRequest(string name)
    {
        incomingRequests.Remove(name);
        friends.Add(name);
        Save();
    }

    public void DeclineRequest(string name)
    {
        incomingRequests.Remove(name);
        Save();
    }

    public void CancelSentRequest(string name)
    {
        sentRequests.Remove(name);
        Save();
    }

    public void RemoveFriend(string name)
    {
        friends.Remove(name);
        Save();
    }

    public void Block(string name)
    {
        blocked.Add(name);
        Save();
    }

    public void Report(string name)
    {
        reported.Add(name);
        Save();
    }

    public FriendManaResult TrySendMana(string name)
    {
        if (IsBlocked(name))
        {
            return FriendManaResult.Blocked;
        }

        if (sentManaToday.Count >= DailyManaLimit || sentManaToday.Contains(name))
        {
            return FriendManaResult.LimitReached;
        }

        sentManaToday.Add(name);
        Save();
        return FriendManaResult.Done;
    }

    public FriendManaResult TryReceiveMana(string name)
    {
        if (IsBlocked(name))
        {
            return FriendManaResult.Blocked;
        }

        if (receivedManaToday.Count >= DailyManaLimit || receivedManaToday.Contains(name))
        {
            return FriendManaResult.LimitReached;
        }

        receivedManaToday.Add(name);
        Save();
        return FriendManaResult.Done;
    }

    private void SeedPrototypeDefaults()
    {
        friends.Clear();
        friends.Add("Luna");
        friends.Add("Eldric");
        friends.Add("Mira");
        incomingRequests.Clear();
        incomingRequests.Add("Aster");
        incomingRequests.Add("Rowan");
        sentRequests.Clear();
        sentRequests.Add("Juniper");
        sentManaToday.Clear();
        receivedManaToday.Clear();
        blocked.Clear();
        reported.Clear();
    }

    private string TodayKey()
    {
        return utcNow().Date.ToString("yyyy-MM-dd");
    }

    // A missing key keeps the current (seeded) values, matching the original prototype behavior.
    private static void LoadSet(string key, HashSet<string> target)
    {
        string fullKey = SavePrefix + key;
        if (!DurablePrefs.HasKey(fullKey))
        {
            return;
        }

        target.Clear();
        string saved = DurablePrefs.GetString(fullKey, "");
        if (string.IsNullOrWhiteSpace(saved))
        {
            return;
        }

        foreach (string value in saved.Split(Separator))
        {
            string trimmed = value.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                target.Add(trimmed);
            }
        }
    }

    private static void SaveSet(string key, HashSet<string> values)
    {
        List<string> sorted = new List<string>(values);
        sorted.Sort();
        DurablePrefs.SetString(SavePrefix + key, string.Join(Separator.ToString(), sorted));
    }
}

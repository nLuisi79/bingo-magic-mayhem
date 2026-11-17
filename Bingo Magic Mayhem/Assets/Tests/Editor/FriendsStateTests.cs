using System;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;

public sealed class FriendsStateTests
{
    private static readonly DateTime Day1 = new DateTime(2026, 11, 17, 10, 0, 0, DateTimeKind.Utc);
    private string root;

    [SetUp]
    public void SetUp()
    {
        root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        DurablePrefs.ResetForTests(root);
        EconomyTuning.OverrideForTests(null);
    }

    [TearDown]
    public void TearDown()
    {
        DurablePrefs.ResetForTests(Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N")));
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public void NewPlayersStartWithThePrototypeFriendsAndRequests()
    {
        FriendsState friends = Loaded(Day1);

        CollectionAssert.AreEquivalent(new[] { "Luna", "Eldric", "Mira" }, friends.Friends);
        CollectionAssert.AreEquivalent(new[] { "Aster", "Rowan" }, friends.IncomingRequests);
        CollectionAssert.AreEquivalent(new[] { "Juniper" }, friends.SentRequests);
    }

    [Test]
    public void AcceptingARequestMakesAFriendThatPersists()
    {
        FriendsState friends = Loaded(Day1);
        friends.AcceptRequest("Aster");

        FriendsState reloaded = Loaded(Day1);
        CollectionAssert.Contains(reloaded.Friends, "Aster");
        CollectionAssert.DoesNotContain(reloaded.IncomingRequests, "Aster");
    }

    [Test]
    public void SendingRequestsUsesEachCandidateOnceThenStops()
    {
        FriendsState friends = Loaded(Day1);
        Assert.AreEqual("Selene", friends.SendRequestToNextCandidate());
        Assert.AreEqual("Ivy", friends.SendRequestToNextCandidate());
        Assert.AreEqual("Marisol", friends.SendRequestToNextCandidate());
        Assert.IsNull(friends.SendRequestToNextCandidate());
    }

    [Test]
    public void FriendManaIsCappedPerDayAndOncePerFriend()
    {
        FriendsState friends = Loaded(Day1);
        Assert.AreEqual(3, friends.DailyManaLimit);

        Assert.AreEqual(FriendManaResult.Done, friends.TrySendMana("Luna"));
        Assert.AreEqual(FriendManaResult.LimitReached, friends.TrySendMana("Luna"), "Only once per friend per day.");
        Assert.AreEqual(FriendManaResult.Done, friends.TrySendMana("Eldric"));
        Assert.AreEqual(FriendManaResult.Done, friends.TrySendMana("Mira"));
        Assert.AreEqual(FriendManaResult.LimitReached, friends.TrySendMana("Aster"), "Daily cap of 3.");
        Assert.AreEqual(3, friends.SentManaTodayCount);
        Assert.IsFalse(friends.CanSendManaTo("Aster"));
        Assert.IsTrue(friends.CanReceiveManaFrom("Luna"), "Send and receive caps are separate.");
    }

    [Test]
    public void BlockedFriendsCannotTradeMana()
    {
        FriendsState friends = Loaded(Day1);
        friends.Block("Luna");

        Assert.AreEqual(FriendManaResult.Blocked, friends.TrySendMana("Luna"));
        Assert.AreEqual(FriendManaResult.Blocked, friends.TryReceiveMana("Luna"));
        Assert.IsTrue(Loaded(Day1).IsBlocked("Luna"));
    }

    [Test]
    public void DailyManaResetsWhenTheDayChanges()
    {
        FriendsState friends = Loaded(Day1);
        friends.TrySendMana("Luna");
        friends.TryReceiveMana("Mira");

        Assert.AreEqual(1, Loaded(Day1).SentManaTodayCount, "Same day keeps the counts.");

        FriendsState nextDay = Loaded(Day1.AddDays(1));
        Assert.AreEqual(0, nextDay.SentManaTodayCount);
        Assert.AreEqual(0, nextDay.ReceivedManaTodayCount);
        CollectionAssert.AreEquivalent(new[] { "Luna", "Eldric", "Mira" }, nextDay.Friends, "Friends survive the daily reset.");
    }

    private FriendsState Loaded(DateTime now)
    {
        DurablePrefs.Save();
        DurablePrefs.ResetForTests(root);
        FriendsState friends = new FriendsState(() => now);
        friends.Load();
        return friends;
    }
}

using NUnit.Framework;

public sealed class RoomAccessRulesTests
{
    [Test]
    public void NormalProgressionOpensFirstRoomAndRoomsAfterARestore()
    {
        Assert.AreEqual(RoomProgressState.Open, RoomAccessRules.ResolveRoom(false, RoomStateOverride.None, true, true, false));
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.ResolveRoom(false, RoomStateOverride.None, true, false, false));
        Assert.AreEqual(RoomProgressState.Open, RoomAccessRules.ResolveRoom(false, RoomStateOverride.None, true, false, true));
        Assert.AreEqual(RoomProgressState.Complete, RoomAccessRules.ResolveRoom(true, RoomStateOverride.None, true, false, false));
    }

    [Test]
    public void LockedRealmLocksItsRoomsWithoutOverrides()
    {
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.ResolveRoom(false, RoomStateOverride.None, false, true, false));
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.ResolveRoom(false, RoomStateOverride.None, false, false, true));
    }

    [Test]
    public void OverridesBeatNormalProgression()
    {
        Assert.AreEqual(RoomProgressState.Open, RoomAccessRules.ResolveRoom(false, RoomStateOverride.Open, false, false, false));
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.ResolveRoom(false, RoomStateOverride.Locked, true, true, false));
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.ResolveRoom(false, RoomStateOverride.Locked, true, false, true));
    }

    [Test]
    public void RealmUnlocksFromPreviousRealmOrForcedRoom()
    {
        Assert.IsTrue(RoomAccessRules.IsRealmUnlocked(true, false, false));
        Assert.IsTrue(RoomAccessRules.IsRealmUnlocked(false, true, false));
        Assert.IsTrue(RoomAccessRules.IsRealmUnlocked(false, false, true));
        Assert.IsFalse(RoomAccessRules.IsRealmUnlocked(false, false, false));
    }

    [Test]
    public void StateCyclesLockedOpenComplete()
    {
        Assert.AreEqual(RoomProgressState.Open, RoomAccessRules.Next(RoomProgressState.Locked));
        Assert.AreEqual(RoomProgressState.Complete, RoomAccessRules.Next(RoomProgressState.Open));
        Assert.AreEqual(RoomProgressState.Locked, RoomAccessRules.Next(RoomProgressState.Complete));
    }
}

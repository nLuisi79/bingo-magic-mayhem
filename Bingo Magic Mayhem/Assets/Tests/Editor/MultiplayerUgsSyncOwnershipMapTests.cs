using BingoMagicMayhem.Multiplayer;
using NUnit.Framework;

public sealed class MultiplayerUgsSyncOwnershipMapTests
{
    [Test]
    public void Get_CallBroadcast_MapsToRelayWithValidation()
    {
        MultiplayerUgsSyncOwnershipEntry entry = MultiplayerUgsSyncOwnershipMap.Get(MultiplayerSessionSyncEventKind.CallBroadcast);

        Assert.That(entry.PrimaryOwner, Is.EqualTo(MultiplayerUgsServiceOwner.Relay));
        Assert.That(entry.SecondaryOwner, Is.EqualTo(MultiplayerUgsServiceOwner.Analytics));
        Assert.That(entry.RequiresAuthoritativeValidation, Is.True);
    }

    [Test]
    public void Get_MatchEnd_MapsToCloudCodeAndCloudSave()
    {
        MultiplayerUgsSyncOwnershipEntry entry = MultiplayerUgsSyncOwnershipMap.Get(MultiplayerSessionSyncEventKind.MatchEnd);

        Assert.That(entry.PrimaryOwner, Is.EqualTo(MultiplayerUgsServiceOwner.CloudCode));
        Assert.That(entry.SecondaryOwner, Is.EqualTo(MultiplayerUgsServiceOwner.CloudSave));
        Assert.That(entry.ResponsibilitySummary, Does.Contain("round closure"));
    }
}

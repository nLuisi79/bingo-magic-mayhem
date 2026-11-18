using System;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;

public sealed class DailyRewardsStateTests
{
    private static readonly DateTime Day1 = new DateTime(2026, 11, 17, 15, 0, 0, DateTimeKind.Utc);
    private DateTime now;
    private string root;

    [SetUp]
    public void SetUp()
    {
        now = Day1;
        root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        DurablePrefs.ResetForTests(root);
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
    public void ConsecutiveDaysBuildTheStreakAndCycleThroughSevenDays()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        for (int day = 1; day <= 8; day++)
        {
            Assert.IsTrue(daily.CanClaimBonus(), $"day {day} claimable");
            Assert.AreEqual(((day - 1) % 7) + 1, daily.RegisterBonusClaim());
            Assert.IsFalse(daily.CanClaimBonus(), "only once per day");
            now = now.AddDays(1);
        }

        Assert.AreEqual(8, daily.BonusStreak);
    }

    [Test]
    public void MissingADayIsDetectedAndClaimingRestartsTheStreak()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        daily.RegisterBonusClaim();
        now = now.AddDays(1);
        daily.RegisterBonusClaim();
        Assert.IsFalse(daily.HasMissedStreak());

        now = now.AddDays(2);
        Assert.IsTrue(daily.HasMissedStreak());
        Assert.AreEqual(1, daily.RegisterBonusClaim());
        Assert.AreEqual(1, daily.BonusStreak);
    }

    [Test]
    public void AStreakSaveLetsTheNextClaimContinue()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        daily.RegisterBonusClaim();
        now = now.AddDays(3);

        daily.RegisterStreakSave();

        Assert.IsFalse(daily.HasMissedStreak());
        daily.RegisterBonusClaim();
        Assert.AreEqual(2, daily.BonusStreak);
        Assert.AreEqual(1, daily.StreakSaveUses);
    }

    [Test]
    public void StreakSaveCostsClimbTheLadderAndStopAtTheTop()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        int[] ladder = { 0, 50, 100 };
        Assert.AreEqual(0, daily.GetStreakSaveCost(ladder));
        daily.RegisterStreakSave();
        Assert.AreEqual(50, daily.GetStreakSaveCost(ladder));
        daily.RegisterStreakSave();
        daily.RegisterStreakSave();
        Assert.AreEqual(100, daily.GetStreakSaveCost(ladder));
    }

    [Test]
    public void ChestMilestonesFollowTheStreak()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        Assert.AreEqual(7, daily.GetNextChestDay());
        for (int day = 0; day < 7; day++)
        {
            daily.RegisterBonusClaim();
            now = now.AddDays(1);
        }

        Assert.AreEqual(14, daily.GetNextChestDay());
    }

    [Test]
    public void TheDailySpinIsOncePerUtcDay()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        Assert.IsTrue(daily.CanClaimSpin());
        daily.RegisterSpinClaim();
        Assert.IsFalse(daily.CanClaimSpin());

        now = new DateTime(Day1.Year, Day1.Month, Day1.Day, 23, 59, 0, DateTimeKind.Utc);
        Assert.IsFalse(daily.CanClaimSpin(), "same UTC day");
        now = now.AddMinutes(2);
        Assert.IsTrue(daily.CanClaimSpin(), "next UTC day");
    }

    [Test]
    public void ClaimsSurviveASaveAndReload()
    {
        DailyRewardsState daily = new DailyRewardsState(() => now);
        daily.RegisterBonusClaim();
        daily.RegisterSpinClaim();
        daily.Write();
        DurablePrefs.Save();

        DurablePrefs.ResetForTests(root);
        DailyRewardsState reloaded = new DailyRewardsState(() => now);
        reloaded.Load();

        Assert.AreEqual(1, reloaded.BonusStreak);
        Assert.IsFalse(reloaded.CanClaimBonus());
        Assert.IsFalse(reloaded.CanClaimSpin());
    }
}

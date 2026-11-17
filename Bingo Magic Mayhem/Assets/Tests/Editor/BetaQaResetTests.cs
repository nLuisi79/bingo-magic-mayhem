using System;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;

public sealed class BetaQaResetTests
{
    private string root;
    private PlayerInventoryState inventory;

    [SetUp]
    public void SetUp()
    {
        root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        DurablePrefs.ResetForTests(root);
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        inventory = new PlayerInventoryState();
        inventory.Load();
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
    public void AdvancingTheDailyBonusDayContinuesTheStreak()
    {
        inventory.ClaimDailyBonus();
        Assert.IsFalse(inventory.CanClaimDailyBonus());
        int streak = inventory.DailyBonusStreak;

        inventory.DevAdvanceDailyBonusDay();
        Assert.IsTrue(inventory.CanClaimDailyBonus());
        inventory.ClaimDailyBonus();

        Assert.AreEqual(streak + 1, inventory.DailyBonusStreak);
    }

    [Test]
    public void ResettingTheStreakStartsAtDayOne()
    {
        inventory.ClaimDailyBonus();
        inventory.DevAdvanceDailyBonusDay();
        inventory.ClaimDailyBonus();

        inventory.DevResetDailyBonusStreak();

        Assert.AreEqual(0, inventory.DailyBonusStreak);
        Assert.IsTrue(inventory.CanClaimDailyBonus());
        Assert.AreEqual(1, inventory.GetDailyBonusDisplayDay());
    }

    [Test]
    public void FillingRoomIngredientsMakesThePotionRestorable()
    {
        Assert.IsFalse(inventory.CanRestoreActiveRoom());

        inventory.DevFillActiveRoomIngredients();

        Assert.IsTrue(inventory.CanRestoreActiveRoom());
    }
}

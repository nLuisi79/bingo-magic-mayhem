using BingoMagicMayhem.Jackpot;
using NUnit.Framework;
using UnityEngine;

// Moving tuning into config must not change behavior: the shipped JSON equals the built-in defaults, and the
// derived values match what the hard-coded formulas produced.
public sealed class EconomyTuningTests
{
    [TearDown]
    public void TearDown()
    {
        EconomyTuning.OverrideForTests(null);
    }

    [Test]
    public void ShippedConfigMatchesBuiltInDefaults()
    {
        Assert.IsNotNull(Resources.Load<TextAsset>(EconomyTuning.ResourcePath), "economy_tuning.json should ship in Resources/config.");
        Assert.AreEqual(JsonUtility.ToJson(new EconomyTuning(), true), JsonUtility.ToJson(EconomyTuning.Load(), true));
    }

    [Test]
    public void JackpotWheelRollsLandOnTheOriginalSegments()
    {
        // Original thresholds: <18, <32, <46, <58, <69, <79, <87, <94, <97, <99, else.
        int[] firstRollOfSegment = { 0, 18, 32, 46, 58, 69, 79, 87, 94, 97, 99 };
        for (int segment = 0; segment < firstRollOfSegment.Length; segment++)
        {
            Assert.AreEqual(segment, JackpotWheelRules.GetSegmentForRoll(firstRollOfSegment[segment]), $"first roll of segment {segment}");
            if (segment > 0)
            {
                Assert.AreEqual(segment - 1, JackpotWheelRules.GetSegmentForRoll(firstRollOfSegment[segment] - 1), $"last roll before segment {segment}");
            }
        }

        Assert.AreEqual(10, JackpotWheelRules.GetSegmentForRoll(99));
    }

    [Test]
    public void RoomProgressionMatchesTheOriginalFormula()
    {
        RoomProgressionProfile special = RoomProgressionProfile.Create(4, 4, 3);
        Assert.AreEqual(16, special.Level);
        Assert.AreEqual(100, special.MinManaBet);
        Assert.AreEqual(800, special.MaxManaBet);
        Assert.AreEqual(50, special.BetStep);
        Assert.AreEqual(1375, special.MinimumJackpotPot);
        Assert.AreEqual(0.17f, special.JackpotContributionRate, 0.0001f);
        Assert.AreEqual(2.2f, special.XpMultiplier, 0.0001f);
        Assert.AreEqual(0.016f, special.NoBingoIngredientDropChance, 0.0001f);
        Assert.AreEqual(0.60f, special.CellRewardChance, 0.0001f);
        Assert.AreEqual(3250, special.RestoreManaReward);

        RoomProgressionProfile first = RoomProgressionProfile.Create(1, 1, 0);
        Assert.AreEqual(25, first.MinManaBet);
        Assert.AreEqual(150, first.MaxManaBet);
        Assert.AreEqual(25, first.BetStep);
        Assert.AreEqual(250, first.MinimumJackpotPot);
        Assert.AreEqual(1000, first.RestoreManaReward);
    }

    [Test]
    public void DailyBonusChestCombinesScaledManaWithItsBonus()
    {
        EconomyTuning.DailyBonusDayEntry chest = new EconomyTuning().DailyBonus.Days[6];
        Assert.IsTrue(chest.AddScaledMana);
        Assert.AreEqual(250, chest.Mana);
        Assert.AreEqual(25, chest.Crystals);
        Assert.AreEqual("Pandora Sigil", chest.ItemName);
    }

    [Test]
    public void OverridesChangeTheValuesTheGameReads()
    {
        EconomyTuning tuning = new EconomyTuning();
        tuning.Social.FriendManaAmount = 25;
        tuning.PowerUps.CrystalCost = 12;
        EconomyTuning.OverrideForTests(tuning);

        Assert.AreEqual(12, PlayerInventoryState.GameplayPowerUpCrystalCost);
        Assert.AreEqual(25, EconomyTuning.Current.Social.FriendManaAmount);
    }
}

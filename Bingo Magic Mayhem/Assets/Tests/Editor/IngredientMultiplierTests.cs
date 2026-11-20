using NUnit.Framework;

// Locked rule: the card potion multiplier follows the mana bet (1x, 2x past 50%, 3x past 75%, 4x at the max bet).
public sealed class IngredientMultiplierTests
{
    private static readonly EconomyTuning.IngredientMultiplierSettings Settings = new EconomyTuning.IngredientMultiplierSettings();

    [TestCase(25, 1)]
    [TestCase(100, 1)]
    [TestCase(101, 2)]
    [TestCase(150, 2)]
    [TestCase(151, 3)]
    [TestCase(199, 3)]
    [TestCase(200, 4)]
    [TestCase(250, 4)]
    public void MultiplierFollowsTheBetThresholds(int bet, int expected)
    {
        Assert.AreEqual(expected, IngredientMultiplier.ForBet(bet, 200, Settings));
    }

    [Test]
    public void BadInputsFallBackToOneX()
    {
        Assert.AreEqual(1, IngredientMultiplier.ForBet(0, 200, Settings));
        Assert.AreEqual(1, IngredientMultiplier.ForBet(50, 0, Settings));
    }

    [Test]
    public void EveryRoomsMinimumBetIsOneXAndMaximumBetIsFourX()
    {
        foreach (RoomDefinition room in RealmContentCatalog.AllRooms)
        {
            RoomProgressionProfile progression = room.Progression;
            Assert.AreEqual(1, IngredientMultiplier.ForBet(progression.MinManaBet, progression.MaxManaBet, Settings), $"{room.Name} min bet");
            Assert.AreEqual(4, IngredientMultiplier.ForBet(progression.MaxManaBet, progression.MaxManaBet, Settings), $"{room.Name} max bet");
        }
    }

    [Test]
    public void GeneratedCardsCarryTheBetMultiplier()
    {
        PlayerCardSet cards = new PlayerCardSet();
        cards.Generate(4, 3);
        for (int index = 0; index < cards.Count; index++)
        {
            Assert.AreEqual(3, cards.GetPotionMultiplier(index));
        }
    }

    [Test]
    public void TuningConfigLoadsTheMultiplierThresholds()
    {
        EconomyTuning.IngredientMultiplierSettings loaded = EconomyTuning.Current.IngredientMultiplier;
        Assert.AreEqual(0.5f, loaded.DoubleAboveBetFraction, 0.0001f);
        Assert.AreEqual(0.75f, loaded.TripleAboveBetFraction, 0.0001f);
        Assert.AreEqual(4, loaded.MaxMultiplier);
    }
}

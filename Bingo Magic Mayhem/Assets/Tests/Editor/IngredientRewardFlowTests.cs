using System;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;

// End-to-end check of the round-reward path: bingos on a card -> reward preview -> Collect (Redeem) -> saved
// room ingredient progress that survives a reload.
public sealed class IngredientRewardFlowTests
{
    private string root;

    [SetUp]
    public void SetUp()
    {
        root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        DurablePrefs.ResetForTests(root);
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
    }

    [TearDown]
    public void TearDown()
    {
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        DurablePrefs.ResetForTests(Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N")));
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    [Test]
    public void CollectedBingosAddRoomIngredientsThatPersist()
    {
        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        PlayerInventoryState inventory = new PlayerInventoryState();
        inventory.Load();
        int before = CollectedTotal(inventory, room);

        PlayerCardSet cards = new PlayerCardSet();
        cards.Generate(1);
        cards.BingoCounts[0] = 3;
        RewardPreview preview = RewardPreview.Build(cards, new BingoRewardTracker(new BingoRoomRules()), new CellRewardTracker(), 1, room.Progression.MinManaBet, "test");

        Assert.Greater(preview.IngredientDrops.Count, 0, "Three bingos must produce ingredient drops.");
        int dropped = 0;
        foreach (IngredientDrop drop in preview.IngredientDrops)
        {
            dropped += drop.Quantity;
        }

        inventory.Redeem(preview);
        Assert.AreEqual(before + dropped, CollectedTotal(inventory, room), "Collect should add every dropped ingredient.");

        DurablePrefs.ResetForTests(root);
        PlayerInventoryState reloaded = new PlayerInventoryState();
        reloaded.Load();
        Assert.AreEqual(before + dropped, CollectedTotal(reloaded, room), "Collected ingredients must survive a reload.");
    }

    [Test]
    public void SaveBeforeLoadDoesNotOverwriteSavedProgress()
    {
        RoomDefinition room = RealmContentCatalog.ActivePrototypeRoom;
        string firstIngredient = room.Ingredients[0].Name;
        PlayerInventoryState session = new PlayerInventoryState();
        session.Load();
        session.AddIngredientForPrototype(firstIngredient, 4);
        Assert.IsTrue(session.TrySpendMana(120), "Spend so the saved mana differs from the default.");
        session.Save();
        int savedMana = session.Mana;

        // Startup used to save from the per-frame power-up tick before Load() finished, writing defaults.
        DurablePrefs.ResetForTests(root);
        PlayerInventoryState nextLaunch = new PlayerInventoryState();
        nextLaunch.Save();
        DurablePrefs.Save();

        DurablePrefs.ResetForTests(root);
        PlayerInventoryState reloaded = new PlayerInventoryState();
        reloaded.Load();
        Assert.AreEqual(4, reloaded.GetIngredientInventoryCount(firstIngredient));
        Assert.AreEqual(savedMana, reloaded.Mana);
    }

    private static int CollectedTotal(PlayerInventoryState inventory, RoomDefinition room)
    {
        int total = 0;
        foreach (IngredientRequirement requirement in room.Ingredients)
        {
            total += inventory.GetIngredientInventoryCount(requirement.Name);
        }

        return total;
    }
}

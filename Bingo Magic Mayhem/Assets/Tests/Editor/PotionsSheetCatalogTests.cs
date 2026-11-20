using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

// The BMM Potions Google Sheet is the source of truth for realm order, room names, potions, ingredients and amounts.
// These tests compare RealmContentCatalog against the checked-in snapshot (Tests/Editor/Data/bmm_potions_sheet.txt).
// When the sheet changes: refresh the snapshot (Tools > Bingo Magic Mayhem > Refresh Potions Sheet Snapshot), run these
// tests, and update RealmContentCatalog until they pass. Names compare case-insensitively because the sheet has a few
// lowercase room names that the game shows capitalised. Rarity is compared too (filled into the sheet 2026-11-20).
public sealed class PotionsSheetCatalogTests
{
    private const int LaunchRealmCount = 10;

    private sealed class SheetRoom
    {
        public string Name;
        public string Potion;
        public List<SheetIngredient> Ingredients = new List<SheetIngredient>();
    }

    private sealed class SheetIngredient
    {
        public string Name;
        public int Amount;
        public string Rarity;
    }

    private sealed class SheetRealm
    {
        public string Name;
        public List<SheetRoom> Rooms = new List<SheetRoom>();
    }

    [Test]
    public void TheTenLaunchRealmsMatchTheSheetInOrder()
    {
        List<SheetRealm> sheet = LoadSnapshot();
        Assert.GreaterOrEqual(sheet.Count, LaunchRealmCount, "snapshot should list at least the ten launch realms");
        Assert.AreEqual(LaunchRealmCount, RealmContentCatalog.AllRealms.Count);

        for (int index = 0; index < LaunchRealmCount; index++)
        {
            AssertRealmMatches(sheet[index], RealmContentCatalog.AllRealms[index], index + 1);
        }
    }

    [Test]
    public void EverfrostValeMatchesTheSheetsEleventhRealm()
    {
        List<SheetRealm> sheet = LoadSnapshot();
        Assert.Greater(sheet.Count, LaunchRealmCount, "snapshot has no 11th realm");
        AssertRealmMatches(sheet[LaunchRealmCount], RealmContentCatalog.EverfrostVale, LaunchRealmCount + 1);
    }

    private static void AssertRealmMatches(SheetRealm expected, RealmDefinition actual, int realmNumber)
    {
        string where = $"Realm {realmNumber}";
        AssertSameName(expected.Name, actual.Name, $"{where} name");
        Assert.AreEqual(expected.Rooms.Count, actual.Rooms.Count, $"{where} ({actual.Name}) room count");

        for (int roomIndex = 0; roomIndex < expected.Rooms.Count; roomIndex++)
        {
            SheetRoom sheetRoom = expected.Rooms[roomIndex];
            RoomDefinition room = actual.Rooms[roomIndex];
            string roomWhere = $"{actual.Name} room {roomIndex + 1}";
            AssertSameName(sheetRoom.Name, room.Name, $"{roomWhere} name");
            AssertSameName(sheetRoom.Potion, room.PotionName, $"{roomWhere} potion");
            Assert.AreEqual(sheetRoom.Ingredients.Count, room.Ingredients.Count, $"{roomWhere} ({room.Name}) ingredient count");

            for (int ingredientIndex = 0; ingredientIndex < sheetRoom.Ingredients.Count; ingredientIndex++)
            {
                SheetIngredient sheetIngredient = sheetRoom.Ingredients[ingredientIndex];
                IngredientRequirement ingredient = room.Ingredients[ingredientIndex];
                AssertSameName(sheetIngredient.Name, ingredient.Name, $"{room.Name} ingredient {ingredientIndex + 1}");
                Assert.AreEqual(sheetIngredient.Amount, ingredient.Required, $"{room.Name} / {ingredient.Name} amount");
                Assert.IsNotEmpty(sheetIngredient.Rarity, $"{room.Name} / {ingredient.Name}: the sheet has no rarity");
                AssertSameName(sheetIngredient.Rarity, ingredient.Rarity, $"{room.Name} / {ingredient.Name} rarity");
            }
        }
    }

    private static void AssertSameName(string expected, string actual, string message)
    {
        Assert.IsTrue(
            string.Equals(Normalize(expected), Normalize(actual), StringComparison.OrdinalIgnoreCase),
            $"{message}: sheet has '{expected}', game has '{actual}'");
    }

    // Treats straight and curly apostrophes as the same; the sheet and code mix both.
    private static string Normalize(string value) => (value ?? string.Empty).Trim().Replace('’', '\'');

    private static List<SheetRealm> LoadSnapshot()
    {
        string path = Path.Combine(Application.dataPath, "Tests", "Editor", "Data", "bmm_potions_sheet.txt");
        Assert.IsTrue(File.Exists(path), $"missing sheet snapshot at {path}");

        List<SheetRealm> realms = new List<SheetRealm>();
        foreach (string rawLine in File.ReadAllLines(path))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("# "))
            {
                continue;
            }

            if (line.StartsWith("## "))
            {
                realms.Add(new SheetRealm { Name = line.Substring(3).Trim() });
                continue;
            }

            string[] parts = line.Split(new[] { " | " }, StringSplitOptions.None);
            Assert.AreEqual(3, parts.Length, $"bad snapshot line: {line}");
            Assert.IsNotEmpty(realms, $"room before any realm: {line}");

            SheetRoom room = new SheetRoom { Name = parts[0].Trim(), Potion = parts[1].Trim() };
            foreach (string entry in parts[2].Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries))
            {
                // Name=Amount=Rarity (rarity may be blank in older snapshots).
                string[] fields = entry.Split('=');
                Assert.GreaterOrEqual(fields.Length, 2, $"bad ingredient entry: {entry}");
                room.Ingredients.Add(new SheetIngredient
                {
                    Name = fields[0].Trim(),
                    Amount = int.Parse(fields[1].Trim()),
                    Rarity = fields.Length > 2 ? fields[2].Trim() : string.Empty
                });
            }

            realms[realms.Count - 1].Rooms.Add(room);
        }

        return realms;
    }
}

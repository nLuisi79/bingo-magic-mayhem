using System;
using System.Collections.Generic;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;
using UnityEngine;

public sealed class DurablePrefsTests
{
    private readonly List<string> temporaryRoots = new List<string>();
    private readonly List<string> legacyKeys = new List<string>();

    [TearDown]
    public void TearDown()
    {
        foreach (string key in legacyKeys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.Save();
        foreach (string root in temporaryRoots)
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }

        // Leave the static store pointed at a fresh scratch folder so no test touches real player data.
        DurablePrefs.ResetForTests(CreateRoot());
    }

    [Test]
    public void ValuesPersistAcrossReload()
    {
        string root = CreateRoot();
        DurablePrefs.ResetForTests(root);
        DurablePrefs.SetInt("BMM.Test.Mana", 1250);
        DurablePrefs.SetFloat("BMM.Test.Ratio", 0.75f);
        DurablePrefs.SetString("BMM.Test.Name", "Gilded Azalea Arboretum");
        DurablePrefs.Save();

        DurablePrefs.ResetForTests(root);

        Assert.AreEqual(1250, DurablePrefs.GetInt("BMM.Test.Mana"));
        Assert.AreEqual(0.75f, DurablePrefs.GetFloat("BMM.Test.Ratio"));
        Assert.AreEqual("Gilded Azalea Arboretum", DurablePrefs.GetString("BMM.Test.Name"));
        Assert.IsTrue(File.Exists(Path.Combine(root, DurablePrefs.StateName + ".snapshot.json")));
    }

    [Test]
    public void UnsavedChangesAreNotWritten()
    {
        string root = CreateRoot();
        DurablePrefs.ResetForTests(root);
        DurablePrefs.SetInt("BMM.Test.Unsaved", 7);

        DurablePrefs.ResetForTests(root);

        Assert.AreEqual(0, DurablePrefs.GetInt("BMM.Test.Unsaved"));
    }

    [Test]
    public void LegacyPlayerPrefsValuesAreImportedOnFirstRead()
    {
        string root = CreateRoot();
        string key = LegacyKey("Imported");
        PlayerPrefs.SetInt(key, 42);
        PlayerPrefs.Save();
        DurablePrefs.ResetForTests(root, readLegacyPlayerPrefs: true);

        Assert.IsTrue(DurablePrefs.HasKey(key));
        Assert.AreEqual(42, DurablePrefs.GetInt(key));
        Assert.AreEqual(1, DurablePrefs.ImportedLegacyKeyCount);
        DurablePrefs.Save();

        PlayerPrefs.DeleteKey(key);
        DurablePrefs.ResetForTests(root, readLegacyPlayerPrefs: true);
        Assert.AreEqual(42, DurablePrefs.GetInt(key), "Imported values must survive without the legacy copy.");
    }

    [Test]
    public void DurableValueWinsOverStaleLegacyValue()
    {
        string root = CreateRoot();
        string key = LegacyKey("Stale");
        PlayerPrefs.SetString(key, "old");
        PlayerPrefs.Save();
        DurablePrefs.ResetForTests(root, readLegacyPlayerPrefs: true);

        DurablePrefs.SetString(key, "new");

        Assert.AreEqual("new", DurablePrefs.GetString(key));
    }

    [Test]
    public void DeleteKeyClearsLegacyCopySoItCannotReappear()
    {
        string root = CreateRoot();
        string key = LegacyKey("Deleted");
        PlayerPrefs.SetInt(key, 5);
        PlayerPrefs.Save();
        DurablePrefs.ResetForTests(root, readLegacyPlayerPrefs: true);
        Assert.AreEqual(5, DurablePrefs.GetInt(key));

        DurablePrefs.DeleteKey(key);
        DurablePrefs.Save();
        DurablePrefs.ResetForTests(root, readLegacyPlayerPrefs: true);

        Assert.IsFalse(DurablePrefs.HasKey(key));
        Assert.IsFalse(PlayerPrefs.HasKey(key));
        Assert.AreEqual(-1, DurablePrefs.GetInt(key, -1));
    }

    [Test]
    public void CorruptPrimarySnapshotRecoversFromBackup()
    {
        string root = CreateRoot();
        DurablePrefs.ResetForTests(root);
        DurablePrefs.SetInt("BMM.Test.Restored", 1);
        DurablePrefs.Save();
        DurablePrefs.SetInt("BMM.Test.Restored", 2);
        DurablePrefs.Save();

        File.WriteAllText(Path.Combine(root, DurablePrefs.StateName + ".snapshot.json"), "{ not json");
        DurablePrefs.ResetForTests(root);

        Assert.AreEqual(1, DurablePrefs.GetInt("BMM.Test.Restored"), "Expected the last-known-good backup to load.");
    }

    private string LegacyKey(string suffix)
    {
        string key = "BMM.DurablePrefsTests." + suffix + "." + Guid.NewGuid().ToString("N");
        legacyKeys.Add(key);
        return key;
    }

    private string CreateRoot()
    {
        string root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        temporaryRoots.Add(root);
        return root;
    }
}

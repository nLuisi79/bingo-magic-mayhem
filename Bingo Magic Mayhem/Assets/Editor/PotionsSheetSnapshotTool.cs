using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

// Downloads the BMM Potions Google Sheet (the current source of truth for realm content) and rewrites the snapshot that
// PotionsSheetCatalogTests compares RealmContentCatalog against. The sheet must be viewable by link for this to work.
public static class PotionsSheetSnapshotTool
{
    private const string SheetId = "1DqMJ5OEEApMTrUETyeDbowfqiDpLdRBxOoGN6uavi-4";
    private const string CsvUrl = "https://docs.google.com/spreadsheets/d/" + SheetId + "/gviz/tq?tqx=out:csv&sheet=Sheet1";
    private const string SnapshotAssetPath = "Assets/Tests/Editor/Data/bmm_potions_sheet.txt";
    private const int FirstIngredientColumn = 2;
    private const int MaxIngredients = 7;

    [MenuItem("Tools/Bingo Magic Mayhem/Refresh Potions Sheet Snapshot")]
    public static void Refresh()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(CsvUrl))
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Potions sheet", "Downloading the BMM Potions sheet...", request.downloadProgress))
                {
                    request.Abort();
                    break;
                }
            }

            EditorUtility.ClearProgressBar();
            if (request.result != UnityWebRequest.Result.Success)
            {
                EditorUtility.DisplayDialog("Potions sheet", $"Download failed: {request.error}\nCheck that the sheet is shared as viewable by link.", "OK");
                return;
            }

            string snapshot = BuildSnapshot(request.downloadHandler.text, out int realmCount, out int roomCount);
            File.WriteAllText(Path.GetFullPath(SnapshotAssetPath), snapshot, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(SnapshotAssetPath);
            Debug.Log($"Potions sheet snapshot refreshed: {realmCount} realms, {roomCount} rooms. Run PotionsSheetCatalogTests to compare with RealmContentCatalog.");
        }
    }

    // Row 0 holds the first realm name in its first cell ("Realm/Room <name>"). After that, a row with a name and no
    // potion starts a new realm; a row with both is a room, followed by ingredient/amount/rarity column triples.
    public static string BuildSnapshot(string csv, out int realmCount, out int roomCount)
    {
        List<List<string>> rows = ParseCsv(csv);
        StringBuilder builder = new StringBuilder();
        builder.Append("# Snapshot of the BMM Potions Google Sheet (source of truth for realm/room/potion content).\n");
        builder.Append("# Refresh with Tools > Bingo Magic Mayhem > Refresh Potions Sheet Snapshot. Format: '## Realm' then 'Room | Potion | Ingredient=Amount; ...'.\n");
        builder.Append($"# Captured {DateTime.UtcNow:yyyy-MM-dd}.\n");

        realmCount = 0;
        roomCount = 0;
        if (rows.Count == 0)
        {
            return builder.ToString();
        }

        builder.Append("## ").Append(Cell(rows[0], 0).Replace("Realm/Room", string.Empty).Trim()).Append('\n');
        realmCount++;
        for (int rowIndex = 1; rowIndex < rows.Count; rowIndex++)
        {
            List<string> row = rows[rowIndex];
            string name = Cell(row, 0);
            if (name.Length == 0)
            {
                continue;
            }

            string potion = Cell(row, 1);
            if (potion.Length == 0)
            {
                builder.Append("## ").Append(name).Append('\n');
                realmCount++;
                continue;
            }

            List<string> ingredients = new List<string>();
            for (int slot = 0; slot < MaxIngredients; slot++)
            {
                int column = FirstIngredientColumn + slot * 3;
                string ingredient = Cell(row, column);
                if (ingredient.Length > 0)
                {
                    ingredients.Add($"{ingredient}={Cell(row, column + 1)}");
                }
            }

            builder.Append(name).Append(" | ").Append(potion).Append(" | ").Append(string.Join("; ", ingredients)).Append('\n');
            roomCount++;
        }

        return builder.ToString();
    }

    private static string Cell(List<string> row, int index) => index < row.Count ? row[index].Trim() : string.Empty;

    private static List<List<string>> ParseCsv(string text)
    {
        List<List<string>> rows = new List<List<string>>();
        List<string> row = new List<string>();
        StringBuilder field = new StringBuilder();
        bool quoted = false;
        for (int index = 0; index < text.Length; index++)
        {
            char c = text[index];
            if (quoted)
            {
                if (c == '"')
                {
                    if (index + 1 < text.Length && text[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else if (c == '"')
            {
                quoted = true;
            }
            else if (c == ',')
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if (c == '\n')
            {
                row.Add(field.ToString());
                field.Clear();
                rows.Add(row);
                row = new List<string>();
            }
            else if (c != '\r')
            {
                field.Append(c);
            }
        }

        row.Add(field.ToString());
        rows.Add(row);
        return rows;
    }
}

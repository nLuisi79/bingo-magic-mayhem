using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    [Serializable]
    public sealed class GameProgressState : IVersionedLocalState
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion = CurrentSchemaVersion;
        public List<IntEntry> Ints = new List<IntEntry>();
        public List<FloatEntry> Floats = new List<FloatEntry>();
        public List<StringEntry> Strings = new List<StringEntry>();

        int IVersionedLocalState.SchemaVersion => SchemaVersion;

        [Serializable]
        public sealed class IntEntry
        {
            public string Key = "";
            public int Value;
        }

        [Serializable]
        public sealed class FloatEntry
        {
            public string Key = "";
            public float Value;
        }

        [Serializable]
        public sealed class StringEntry
        {
            public string Key = "";
            public string Value = "";
        }
    }

    /// <summary>
    /// Drop-in replacement for PlayerPrefs for game progress (inventory, rooms, album, Inbox, social, daily systems).
    /// Values live in a versioned durable snapshot with atomic writes and a last-known-good backup instead of
    /// PlayerPrefs. Keys missing from the snapshot are read through from PlayerPrefs once and imported, so existing
    /// players keep their progress; deletes clear both stores so legacy values cannot reappear.
    /// Synchronous on purpose: inventory loads before the async infrastructure root finishes initializing.
    /// </summary>
    public static class DurablePrefs
    {
        public const string StateName = "game_progress";

        private static readonly Dictionary<string, int> ints = new Dictionary<string, int>(StringComparer.Ordinal);
        private static readonly Dictionary<string, float> floats = new Dictionary<string, float>(StringComparer.Ordinal);
        private static readonly Dictionary<string, string> strings = new Dictionary<string, string>(StringComparer.Ordinal);

        private static ILocalDurableStateStore store;
        private static string storeRootOverride;
        private static bool loaded;
        private static bool dirty;
        private static bool lifecycleHooked;

        public static int ImportedLegacyKeyCount { get; private set; }

        public static int GetInt(string key, int defaultValue = 0)
        {
            EnsureLoaded();
            if (ints.TryGetValue(key, out int value))
            {
                return value;
            }

            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key, defaultValue);
                ints[key] = value;
                MarkImported();
                return value;
            }

            return defaultValue;
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            EnsureLoaded();
            if (floats.TryGetValue(key, out float value))
            {
                return value;
            }

            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key, defaultValue);
                floats[key] = value;
                MarkImported();
                return value;
            }

            return defaultValue;
        }

        public static string GetString(string key, string defaultValue = "")
        {
            EnsureLoaded();
            if (strings.TryGetValue(key, out string value))
            {
                return value;
            }

            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetString(key, defaultValue);
                strings[key] = value;
                MarkImported();
                return value;
            }

            return defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            EnsureLoaded();
            if (ints.TryGetValue(key, out int existing) && existing == value)
            {
                return;
            }

            ints[key] = value;
            dirty = true;
        }

        public static void SetFloat(string key, float value)
        {
            EnsureLoaded();
            if (floats.TryGetValue(key, out float existing) && existing.Equals(value))
            {
                return;
            }

            floats[key] = value;
            dirty = true;
        }

        public static void SetString(string key, string value)
        {
            EnsureLoaded();
            value ??= "";
            if (strings.TryGetValue(key, out string existing) && existing == value)
            {
                return;
            }

            strings[key] = value;
            dirty = true;
        }

        public static bool HasKey(string key)
        {
            EnsureLoaded();
            return ints.ContainsKey(key) || floats.ContainsKey(key) || strings.ContainsKey(key) || PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            EnsureLoaded();
            bool removed = ints.Remove(key);
            removed |= floats.Remove(key);
            removed |= strings.Remove(key);
            dirty |= removed;

            // Clear the legacy copy too, otherwise the next read would import it again.
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        public static void Save()
        {
            if (!loaded || !dirty)
            {
                return;
            }

            GetStore().Save(StateName, CaptureState());
            dirty = false;
        }

        // Test hook: points the store at a scratch directory and clears cached values.
        public static void ResetForTests(string rootDirectory)
        {
            storeRootOverride = rootDirectory;
            store = null;
            loaded = false;
            dirty = false;
            ImportedLegacyKeyCount = 0;
            ints.Clear();
            floats.Clear();
            strings.Clear();
        }

        private static void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            loaded = true;
            if (GetStore().TryLoad(StateName, out GameProgressState state))
            {
                foreach (GameProgressState.IntEntry entry in state.Ints) ints[entry.Key] = entry.Value;
                foreach (GameProgressState.FloatEntry entry in state.Floats) floats[entry.Key] = entry.Value;
                foreach (GameProgressState.StringEntry entry in state.Strings) strings[entry.Key] = entry.Value ?? "";
            }

            HookLifecycle();
        }

        private static ILocalDurableStateStore GetStore()
        {
            if (store == null)
            {
                string root = storeRootOverride ?? Path.Combine(Application.persistentDataPath, "bmm_infrastructure", "state");
                store = new JsonFileDurableStateStore(root);
            }

            return store;
        }

        private static GameProgressState CaptureState()
        {
            GameProgressState state = new GameProgressState();
            foreach (KeyValuePair<string, int> pair in ints) state.Ints.Add(new GameProgressState.IntEntry { Key = pair.Key, Value = pair.Value });
            foreach (KeyValuePair<string, float> pair in floats) state.Floats.Add(new GameProgressState.FloatEntry { Key = pair.Key, Value = pair.Value });
            foreach (KeyValuePair<string, string> pair in strings) state.Strings.Add(new GameProgressState.StringEntry { Key = pair.Key, Value = pair.Value });
            return state;
        }

        private static void MarkImported()
        {
            ImportedLegacyKeyCount++;
            dirty = true;
        }

        // PlayerPrefs flushed itself on quit; keep that guarantee, and also save when a phone backgrounds the app.
        private static void HookLifecycle()
        {
            if (lifecycleHooked || storeRootOverride != null)
            {
                return;
            }

            lifecycleHooked = true;
            Application.quitting += Save;
            Application.focusChanged += hasFocus =>
            {
                if (!hasFocus)
                {
                    Save();
                }
            };
        }
    }
}

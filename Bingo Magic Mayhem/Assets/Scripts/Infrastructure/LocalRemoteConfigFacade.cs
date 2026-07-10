using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Typed local defaults only. It deliberately contains no gameplay or economy
    /// values; approved defaults can be supplied by a separate composition root.
    /// </summary>
    public sealed class LocalRemoteConfigFacade : IRemoteConfigFacade
    {
        private readonly Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);

        public string Source => "local_defaults";
        public string Revision { get; }

        public LocalRemoteConfigFacade(IEnumerable<RemoteConfigEntry> defaults = null, string revision = "local-v1")
        {
            Revision = string.IsNullOrWhiteSpace(revision) ? "local-v1" : revision;
            if (defaults == null)
            {
                return;
            }

            foreach (RemoteConfigEntry entry in defaults)
            {
                if (entry != null && !string.IsNullOrWhiteSpace(entry.Key))
                {
                    values[entry.Key.Trim()] = entry.Value ?? "";
                }
            }
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public bool HasKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && values.ContainsKey(key);
        }

        public string GetString(string key, string fallback = "")
        {
            return TryGet(key, out string value) ? value : fallback;
        }

        public int GetInt(string key, int fallback = 0)
        {
            return TryGet(key, out string value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : fallback;
        }

        public float GetFloat(string key, float fallback = 0f)
        {
            return TryGet(key, out string value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed)
                ? parsed
                : fallback;
        }

        public bool GetBool(string key, bool fallback = false)
        {
            return TryGet(key, out string value) && bool.TryParse(value, out bool parsed) ? parsed : fallback;
        }

        private bool TryGet(string key, out string value)
        {
            value = "";
            return !string.IsNullOrWhiteSpace(key) && values.TryGetValue(key, out value);
        }
    }
}

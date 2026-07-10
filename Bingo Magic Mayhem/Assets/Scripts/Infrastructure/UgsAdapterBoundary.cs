using System;
using System.Threading;
using System.Threading.Tasks;
#if BMM_UGS_ADAPTERS
using System.Collections.Generic;
using Unity.RemoteConfig;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
#endif

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// UGS package entries may exist in the manifest while runtime adapters remain
    /// disabled. Define BMM_UGS_ADAPTERS only after package resolution, consent,
    /// environment, and project-link checks are complete.
    /// </summary>
    public static class UgsAdapterBoundary
    {
        public const string RequiredScriptingDefine = "BMM_UGS_ADAPTERS";
        public const bool EnabledByDefault = false;
        public const string RequiredEnvironment = "development";
    }

#if BMM_UGS_ADAPTERS
    /// <summary>
    /// Live UGS adapters are intentionally compile-gated. They preserve the local
    /// facades but are not constructed by GameInfrastructureServices.CreateLocal.
    /// </summary>
    public sealed class UgsIdentityFacade : IIdentityFacade
    {
        public IdentitySession Current { get; private set; }

        public async Task<IdentitySession> InitializeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Current = new IdentitySession
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                Provider = "unity_authentication_anonymous",
                IsCloudAuthenticated = true,
                IsAnonymous = true
            };
            return Current;
        }
    }

    public sealed class UgsAnalyticsFacade : IAnalyticsFacade
    {
        public void Track(string eventName, string safePayloadJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("An analytics event name is required.", nameof(eventName));
            }

            // Event schemas must be created in the Dashboard before live recording.
            AnalyticsService.Instance.RecordEvent(eventName.Trim());
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AnalyticsService.Instance.Flush();
            return Task.CompletedTask;
        }
    }

    public sealed class UgsRemoteConfigFacade : IRemoteConfigFacade
    {
        public string Source => "ugs_remote_config";
        public string Revision => RemoteConfigService.Instance.appConfig.configAssignmentHash;

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
        }

        public bool HasKey(string key) => RemoteConfigService.Instance.appConfig.HasKey(key);
        public string GetString(string key, string fallback = "") => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetString(key) : fallback;
        public int GetInt(string key, int fallback = 0) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetInt(key) : fallback;
        public float GetFloat(string key, float fallback = 0f) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetFloat(key) : fallback;
        public bool GetBool(string key, bool fallback = false) => HasKey(key) ? RemoteConfigService.Instance.appConfig.GetBool(key) : fallback;
    }

    public sealed class UgsCloudSaveProfileSettingsSync
    {
        private const string ProfileSettingsKey = "bmm.profile_settings.v2";

        public async Task SaveAsync(ProfileSettingsState state, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { ProfileSettingsKey, JsonUtility.ToJson(state) }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        public async Task<ProfileSettingsState> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, Unity.Services.CloudSave.Models.Data.Player.PlayerData> data =
                await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { ProfileSettingsKey });
            if (!data.TryGetValue(ProfileSettingsKey, out Unity.Services.CloudSave.Models.Data.Player.PlayerData value))
            {
                return null;
            }

            return JsonUtility.FromJson<ProfileSettingsState>(value.Value.GetAs<string>());
        }
    }
#endif
}

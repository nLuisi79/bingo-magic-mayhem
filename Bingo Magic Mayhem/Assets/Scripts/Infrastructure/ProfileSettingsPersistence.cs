using System;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    [Serializable]
    public sealed class ProfileSettingsState : IVersionedLocalState
    {
        public const int CurrentSchemaVersion = 2;
        public const string DefaultDisplayName = "Bingo Witch";
        public const string DefaultAvatarId = "avatar_moon_witch";
        public const string DefaultFrameId = "frame_plain_gold";
        public const string DefaultDauberId = "dauber_classic_star";

        public int SchemaVersion = CurrentSchemaVersion;
        public string DisplayName = DefaultDisplayName;
        public string AvatarId = DefaultAvatarId;
        public string FrameId = DefaultFrameId;
        public string DauberId = DefaultDauberId;
        public bool SoundEnabled = true;
        public bool NotificationsEnabled = true;

        int IVersionedLocalState.SchemaVersion => SchemaVersion;

        public static ProfileSettingsState CreateDefault()
        {
            return new ProfileSettingsState();
        }
    }

    /// <summary>
    /// First narrow durable-state consumer. The JSON snapshot is primary while
    /// PlayerPrefs receives compatibility writes during the migration window.
    /// </summary>
    public sealed class ProfileSettingsPersistence
    {
        public const string StateName = "profile_settings";
        private const string DefaultLegacyPrefix = "BMM.Prototype.Profile.";

        private readonly ILocalDurableStateStore stateStore;
        private readonly IActionJournal journal;
        private readonly IIdentityFacade identity;
        private readonly string legacyPrefix;

        public ProfileSettingsPersistence(
            ILocalDurableStateStore stateStore,
            IActionJournal journal,
            IIdentityFacade identity,
            string legacyPrefix = DefaultLegacyPrefix)
        {
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            this.legacyPrefix = string.IsNullOrWhiteSpace(legacyPrefix) ? DefaultLegacyPrefix : legacyPrefix;
        }

        public ProfileSettingsState Load()
        {
            if (stateStore.TryLoad(StateName, out ProfileSettingsState snapshot))
            {
                Normalize(snapshot);
                WriteCompatibilityState(snapshot);
                return snapshot;
            }

            bool hadCompatibilityState = HasCompatibilityState();
            ProfileSettingsState migrated = ReadCompatibilityState();
            SaveSnapshotAndCompatibility(migrated);
            Record(hadCompatibilityState ? "profile_settings_migrated" : "profile_settings_initialized", migrated);
            return migrated;
        }

        public ProfileSettingsState Save(ProfileSettingsState state, string actionType)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (string.IsNullOrWhiteSpace(actionType))
            {
                throw new ArgumentException("A profile/settings action type is required.", nameof(actionType));
            }

            Normalize(state);
            SaveSnapshotAndCompatibility(state);
            Record(actionType.Trim(), state);
            return state;
        }

        public ProfileSettingsState ResetProfileIdentityPreservingPreferences(ProfileSettingsState current)
        {
            ProfileSettingsState reset = new ProfileSettingsState
            {
                DisplayName = ProfileSettingsState.DefaultDisplayName,
                AvatarId = ProfileSettingsState.DefaultAvatarId,
                FrameId = ProfileSettingsState.DefaultFrameId,
                DauberId = ProfileSettingsState.DefaultDauberId,
                SoundEnabled = current?.SoundEnabled ?? true,
                NotificationsEnabled = current?.NotificationsEnabled ?? true
            };
            return Save(reset, "profile_identity_reset");
        }

        public static string MigrateV1ToV2(string payloadJson)
        {
            ProfileSettingsState migrated = JsonUtility.FromJson<ProfileSettingsState>(payloadJson) ??
                                            ProfileSettingsState.CreateDefault();
            migrated.SchemaVersion = 2;
            if (string.IsNullOrWhiteSpace(migrated.DisplayName))
            {
                migrated.DisplayName = ProfileSettingsState.DefaultDisplayName;
            }

            return JsonUtility.ToJson(migrated);
        }

        private void SaveSnapshotAndCompatibility(ProfileSettingsState state)
        {
            stateStore.Save(StateName, state);
            WriteCompatibilityState(state);
        }

        private bool HasCompatibilityState()
        {
            return PlayerPrefs.HasKey(GetLegacyKey("AvatarId")) ||
                   PlayerPrefs.HasKey(GetLegacyKey("DisplayName")) ||
                   PlayerPrefs.HasKey(GetLegacyKey("FrameId")) ||
                   PlayerPrefs.HasKey(GetLegacyKey("DauberId")) ||
                   PlayerPrefs.HasKey(GetLegacyKey("SoundEnabled")) ||
                   PlayerPrefs.HasKey(GetLegacyKey("NotificationsEnabled"));
        }

        private ProfileSettingsState ReadCompatibilityState()
        {
            ProfileSettingsState state = ProfileSettingsState.CreateDefault();
            state.DisplayName = PlayerPrefs.GetString(GetLegacyKey("DisplayName"), state.DisplayName);
            state.AvatarId = PlayerPrefs.GetString(GetLegacyKey("AvatarId"), state.AvatarId);
            state.FrameId = PlayerPrefs.GetString(GetLegacyKey("FrameId"), state.FrameId);
            state.DauberId = PlayerPrefs.GetString(GetLegacyKey("DauberId"), state.DauberId);
            state.SoundEnabled = PlayerPrefs.GetInt(GetLegacyKey("SoundEnabled"), state.SoundEnabled ? 1 : 0) == 1;
            state.NotificationsEnabled = PlayerPrefs.GetInt(GetLegacyKey("NotificationsEnabled"), state.NotificationsEnabled ? 1 : 0) == 1;
            Normalize(state);
            return state;
        }

        private void WriteCompatibilityState(ProfileSettingsState state)
        {
            PlayerPrefs.SetString(GetLegacyKey("DisplayName"), state.DisplayName);
            PlayerPrefs.SetString(GetLegacyKey("AvatarId"), state.AvatarId);
            PlayerPrefs.SetString(GetLegacyKey("FrameId"), state.FrameId);
            PlayerPrefs.SetString(GetLegacyKey("DauberId"), state.DauberId);
            PlayerPrefs.SetInt(GetLegacyKey("SoundEnabled"), state.SoundEnabled ? 1 : 0);
            PlayerPrefs.SetInt(GetLegacyKey("NotificationsEnabled"), state.NotificationsEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void Record(string actionType, ProfileSettingsState state)
        {
            journal.RecordAction(
                identity.Current?.PlayerId ?? "uninitialized_local_guest",
                "profile_settings",
                actionType,
                JsonUtility.ToJson(new ProfileSettingsJournalPayload
                {
                    AvatarId = state.AvatarId,
                    FrameId = state.FrameId,
                    DauberId = state.DauberId,
                    SoundEnabled = state.SoundEnabled,
                    NotificationsEnabled = state.NotificationsEnabled,
                    HasCustomDisplayName = state.DisplayName != ProfileSettingsState.DefaultDisplayName
                }),
                status: JournalActionStatus.AppliedLocal);
        }

        private string GetLegacyKey(string suffix)
        {
            return legacyPrefix + suffix;
        }

        private static void Normalize(ProfileSettingsState state)
        {
            state.SchemaVersion = ProfileSettingsState.CurrentSchemaVersion;
            DisplayNameValidationResult displayName = ProfileDisplayNameValidator.ValidateBeta(state.DisplayName);
            state.DisplayName = displayName.IsValid ? displayName.NormalizedName : ProfileSettingsState.DefaultDisplayName;
            state.AvatarId = NormalizeId(state.AvatarId, ProfileSettingsState.DefaultAvatarId);
            state.FrameId = NormalizeId(state.FrameId, ProfileSettingsState.DefaultFrameId);
            state.DauberId = NormalizeId(state.DauberId, ProfileSettingsState.DefaultDauberId);
        }

        private static string NormalizeId(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        [Serializable]
        private sealed class ProfileSettingsJournalPayload
        {
            public string AvatarId = "";
            public string FrameId = "";
            public string DauberId = "";
            public bool SoundEnabled;
            public bool NotificationsEnabled;
            public bool HasCustomDisplayName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using BingoMagicMayhem.Infrastructure;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class InfrastructureServiceTests
{
    private readonly List<string> temporaryRoots = new List<string>();
    private readonly List<string> legacyPrefixes = new List<string>();

    [TearDown]
    public void TearDown()
    {
        foreach (string root in temporaryRoots)
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }

        temporaryRoots.Clear();

        foreach (string prefix in legacyPrefixes)
        {
            PlayerPrefs.DeleteKey(prefix + "AvatarId");
            PlayerPrefs.DeleteKey(prefix + "DisplayName");
            PlayerPrefs.DeleteKey(prefix + "FrameId");
            PlayerPrefs.DeleteKey(prefix + "DauberId");
            PlayerPrefs.DeleteKey(prefix + "SoundEnabled");
            PlayerPrefs.DeleteKey(prefix + "NotificationsEnabled");
        }

        PlayerPrefs.Save();
        legacyPrefixes.Clear();
    }

    [Test]
    public void DurableState_RoundTripsAndRecoversLastKnownGoodBackup()
    {
        string root = CreateTemporaryRoot();
        JsonFileDurableStateStore store = new JsonFileDurableStateStore(root);
        store.Save("profile", new TestState { Count = 1, Label = "first" });
        store.Save("profile", new TestState { Count = 2, Label = "second" });

        File.WriteAllText(Path.Combine(root, "profile.snapshot.json"), "malformed");

        Assert.That(store.TryLoad("profile", out TestState recovered), Is.True);
        Assert.That(recovered.Count, Is.EqualTo(1));
        Assert.That(recovered.Label, Is.EqualTo("first"));
    }

    [Test]
    public void ActionJournal_AppendsTransitionsAndContinuesMonotonicSequence()
    {
        string path = Path.Combine(CreateTemporaryRoot(), "actions.jsonl");
        JsonLinesActionJournal journal = new JsonLinesActionJournal(path);
        ActionJournalRecord action = journal.RecordAction("guest_1", "daily_bonus", "claim", idempotencyKey: "claim_day_1");
        ActionJournalRecord transition = journal.RecordStatus(
            action.ActionId,
            "guest_1",
            "daily_bonus",
            "claim",
            JournalActionStatus.AppliedLocal);

        JsonLinesActionJournal reopened = new JsonLinesActionJournal(path);
        ActionJournalRecord next = reopened.RecordAction("guest_1", "settings", "changed");
        IReadOnlyList<ActionJournalRecord> records = reopened.ReadAll();

        Assert.That(action.Sequence, Is.EqualTo(1));
        Assert.That(transition.Sequence, Is.EqualTo(2));
        Assert.That(transition.RecordKind, Is.EqualTo(JournalRecordKind.StatusTransition));
        Assert.That(next.Sequence, Is.EqualTo(3));
        Assert.That(records.Count, Is.EqualTo(3));
    }

    [Test]
    public void IdentityFacade_ReusesStableLocalGuestId()
    {
        string root = CreateTemporaryRoot();
        JsonFileDurableStateStore store = new JsonFileDurableStateStore(root);
        LocalIdentityFacade firstFacade = new LocalIdentityFacade(store);
        IdentitySession first = firstFacade.InitializeAsync().GetAwaiter().GetResult();

        LocalIdentityFacade secondFacade = new LocalIdentityFacade(store);
        IdentitySession second = secondFacade.InitializeAsync().GetAwaiter().GetResult();

        Assert.That(first.PlayerId, Does.StartWith("guest_"));
        Assert.That(second.PlayerId, Is.EqualTo(first.PlayerId));
        Assert.That(second.IsCloudAuthenticated, Is.False);
    }

    [Test]
    public void RemoteConfig_ParsesTypedDefaultsAndUsesFallbacks()
    {
        LocalRemoteConfigFacade config = new LocalRemoteConfigFacade(new[]
        {
            new RemoteConfigEntry("test.integer", "12"),
            new RemoteConfigEntry("test.float", "2.5"),
            new RemoteConfigEntry("test.enabled", "true")
        });

        Assert.That(config.GetInt("test.integer"), Is.EqualTo(12));
        Assert.That(config.GetFloat("test.float"), Is.EqualTo(2.5f));
        Assert.That(config.GetBool("test.enabled"), Is.True);
        Assert.That(config.GetInt("missing", 7), Is.EqualTo(7));
    }

    [Test]
    public void ProfileSettings_MigratesCompatibilityStateAndResetsOnlyCosmetics()
    {
        string root = CreateTemporaryRoot();
        string prefix = "BMM.Tests." + Guid.NewGuid().ToString("N") + ".";
        legacyPrefixes.Add(prefix);
        PlayerPrefs.SetString(prefix + "AvatarId", "avatar_garden_seer");
        PlayerPrefs.SetInt(prefix + "SoundEnabled", 0);

        JsonFileDurableStateStore store = new JsonFileDurableStateStore(Path.Combine(root, "state"));
        JsonLinesActionJournal journal = new JsonLinesActionJournal(Path.Combine(root, "actions.jsonl"));
        LocalIdentityFacade identity = new LocalIdentityFacade(store);
        identity.InitializeAsync().GetAwaiter().GetResult();
        ProfileSettingsPersistence persistence = new ProfileSettingsPersistence(store, journal, identity, prefix);

        ProfileSettingsState migrated = persistence.Load();
        Assert.That(migrated.AvatarId, Is.EqualTo("avatar_garden_seer"));
        Assert.That(migrated.SoundEnabled, Is.False);

        migrated.DauberId = "dauber_garden_bloom";
        migrated.DisplayName = "Garden Mage";
        persistence.Save(migrated, "dauber_selected");
        ProfileSettingsState reloaded = new ProfileSettingsPersistence(store, journal, identity, prefix).Load();
        Assert.That(reloaded.DauberId, Is.EqualTo("dauber_garden_bloom"));
        Assert.That(reloaded.DisplayName, Is.EqualTo("Garden Mage"));
        Assert.That(journal.ReadAll(), Has.None.Matches<ActionJournalRecord>(record =>
            record.PayloadJson.Contains("Garden Mage")));

        ProfileSettingsState reset = persistence.ResetProfileIdentityPreservingPreferences(reloaded);
        Assert.That(reset.DisplayName, Is.EqualTo(ProfileSettingsState.DefaultDisplayName));
        Assert.That(reset.AvatarId, Is.EqualTo(ProfileSettingsState.DefaultAvatarId));
        Assert.That(reset.DauberId, Is.EqualTo(ProfileSettingsState.DefaultDauberId));
        Assert.That(reset.SoundEnabled, Is.False);
    }

    [Test]
    public void DisplayNameValidator_UsesExplicitLocalBetaRules()
    {
        DisplayNameValidationResult valid = ProfileDisplayNameValidator.ValidateBeta("  Luna   Star-Caller  ");
        DisplayNameValidationResult invalid = ProfileDisplayNameValidator.ValidateBeta("Luna✨");

        Assert.That(valid.IsValid, Is.True);
        Assert.That(valid.NormalizedName, Is.EqualTo("Luna Star-Caller"));
        Assert.That(invalid.IsValid, Is.False);
    }

    [Test]
    public void ProfileSettings_VersionOneMigratesToDisplayNameSchema()
    {
        string root = CreateTemporaryRoot();
        LocalStateMigrationRegistry migrations = new LocalStateMigrationRegistry();
        migrations.RegisterState(ProfileSettingsPersistence.StateName, ProfileSettingsState.CurrentSchemaVersion);
        migrations.RegisterMigration(ProfileSettingsPersistence.StateName, 1, ProfileSettingsPersistence.MigrateV1ToV2);
        SnapshotEnvelopeForTest envelope = new SnapshotEnvelopeForTest
        {
            SchemaVersion = 1,
            StateSchemaVersion = 1,
            SavedAtUtc = DateTime.UtcNow.ToString("O"),
            PayloadJson = JsonUtility.ToJson(new LegacyProfileSettingsV1
            {
                SchemaVersion = 1,
                AvatarId = "avatar_sun_mage",
                FrameId = "frame_violet_gem",
                DauberId = "dauber_moon_drop",
                SoundEnabled = false,
                NotificationsEnabled = true
            })
        };
        File.WriteAllText(
            Path.Combine(root, ProfileSettingsPersistence.StateName + ".snapshot.json"),
            JsonUtility.ToJson(envelope));

        JsonFileDurableStateStore store = new JsonFileDurableStateStore(root, migrations);

        Assert.That(store.TryLoad(ProfileSettingsPersistence.StateName, out ProfileSettingsState migrated), Is.True);
        Assert.That(migrated.SchemaVersion, Is.EqualTo(2));
        Assert.That(migrated.DisplayName, Is.EqualTo(ProfileSettingsState.DefaultDisplayName));
        Assert.That(migrated.AvatarId, Is.EqualTo("avatar_sun_mage"));
        Assert.That(migrated.SoundEnabled, Is.False);
    }

    [Test]
    public void CompositionRoot_JournalsSnapshotRecovery()
    {
        string root = CreateTemporaryRoot();
        GameInfrastructureServices services = GameInfrastructureServices.CreateLocal(storageRoot: root);
        services.InitializeAsync().GetAwaiter().GetResult();
        services.DurableState.Save("recovery_test", new TestState { Count = 1 });
        services.DurableState.Save("recovery_test", new TestState { Count = 2 });
        File.WriteAllText(Path.Combine(root, "state", "recovery_test.snapshot.json"), "malformed");

        Assert.That(services.DurableState.TryLoad("recovery_test", out TestState recovered), Is.True);
        Assert.That(recovered.Count, Is.EqualTo(1));
        Assert.That(services.ActionJournal.ReadAll(), Has.Some.Matches<ActionJournalRecord>(record =>
            record.Source == "persistence" && record.Type == "snapshot_recovered"));
    }

    [Test]
    public void DurableState_AppliesOrderedSchemaMigrationAndPersistsCurrentVersion()
    {
        string root = CreateTemporaryRoot();
        LocalStateMigrationRegistry migrations = new LocalStateMigrationRegistry();
        migrations.RegisterState("migrating", 2);
        migrations.RegisterMigration("migrating", 1, payload =>
        {
            LegacyStateV1 legacy = JsonUtility.FromJson<LegacyStateV1>(payload);
            return JsonUtility.ToJson(new MigratedStateV2
            {
                SchemaVersion = 2,
                Count = legacy.Count,
                Label = "migrated"
            });
        });

        SnapshotEnvelopeForTest envelope = new SnapshotEnvelopeForTest
        {
            SchemaVersion = 1,
            StateSchemaVersion = 1,
            SavedAtUtc = DateTime.UtcNow.ToString("O"),
            PayloadJson = JsonUtility.ToJson(new LegacyStateV1 { SchemaVersion = 1, Count = 9 })
        };
        File.WriteAllText(Path.Combine(root, "migrating.snapshot.json"), JsonUtility.ToJson(envelope));

        JsonFileDurableStateStore store = new JsonFileDurableStateStore(root, migrations);
        SnapshotMigrationInfo observedMigration = null;
        store.StateMigrated += migration => observedMigration = migration;

        Assert.That(store.TryLoad("migrating", out MigratedStateV2 migrated), Is.True);
        Assert.That(migrated.Count, Is.EqualTo(9));
        Assert.That(migrated.Label, Is.EqualTo("migrated"));
        Assert.That(observedMigration, Is.Not.Null);
        Assert.That(observedMigration.FromVersion, Is.EqualTo(1));
        Assert.That(observedMigration.ToVersion, Is.EqualTo(2));
        Assert.That(store.CaptureSnapshotDiagnostics(), Has.Some.Matches<SnapshotDiagnosticsEntry>(entry =>
            entry.StateName == "migrating" && entry.SchemaVersion == 2 && entry.Health == "healthy"));
    }

    [Test]
    public void DiagnosticsExport_RedactsIdentityAndOmitsJournalPayloads()
    {
        string root = CreateTemporaryRoot();
        GameInfrastructureServices services = GameInfrastructureServices.CreateLocal(storageRoot: root);
        services.InitializeAsync().GetAwaiter().GetResult();
        string fullPlayerId = services.Identity.Current.PlayerId;
        services.ActionJournal.RecordAction(
            fullPlayerId,
            "test",
            "safe_event",
            "{\"secret\":\"never-export-this\"}",
            status: JournalActionStatus.AppliedLocal);

        string exportPath = services.Diagnostics.ExportSafeSummary();
        string export = File.ReadAllText(exportPath);

        Assert.That(File.Exists(exportPath), Is.True);
        Assert.That(export, Does.Not.Contain(fullPlayerId));
        Assert.That(export, Does.Not.Contain("never-export-this"));
        Assert.That(export, Does.Contain("test/safe_event"));
    }

    [Test]
    public void BackendPreflight_ReportsResolvedPackagesAndDisabledLiveAdapters()
    {
        BackendPreflightSnapshot preflight = UgsPreflightDiagnostics.Capture(ServiceEnvironment.Prototype);

        Assert.That(preflight.PackageState, Is.EqualTo("resolved_lockfile_cache"));
        Assert.That(preflight.AdapterDefine, Is.EqualTo("absent"));
        Assert.That(preflight.LiveCloudCallsEnabled, Is.False);
        Assert.That(preflight.Checks, Has.Some.Matches<BackendPreflightCheck>(check =>
            check.Name == "UGS packages" && check.Status == BackendPreflightStatus.Pass));
        Assert.That(preflight.BlockedCount, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void DisabledProfileCloudSync_BlocksUploadAndDownload()
    {
        DisabledProfileSettingsCloudSync sync = new DisabledProfileSettingsCloudSync();

        CloudProfileSyncStatus status = sync.RefreshStatusAsync().GetAwaiter().GetResult();
        bool uploaded = sync.TryUploadAsync(new ProfileSettingsState()).GetAwaiter().GetResult();
        ProfileSettingsState downloaded = sync.TryDownloadAsync().GetAwaiter().GetResult();

        Assert.That(status.Service, Is.EqualTo("ugs_cloud_save_profile_settings"));
        Assert.That(status.CloudKey, Is.EqualTo("bmm.profile_settings.v2"));
        Assert.That(status.LiveSyncEnabled, Is.False);
        Assert.That(status.AdapterCompiled, Is.False);
        Assert.That(status.CanUpload, Is.False);
        Assert.That(status.CanDownload, Is.False);
        Assert.That(uploaded, Is.False);
        Assert.That(downloaded, Is.Null);
        Assert.That(status.Checks, Has.Some.Matches<BackendPreflightCheck>(check =>
            check.Name == "Conflict policy" && check.Status == BackendPreflightStatus.Blocked));
        Assert.That(status.Checks, Has.Some.Matches<BackendPreflightCheck>(check =>
            check.Name == "Gameplay/economy scope" && check.Status == BackendPreflightStatus.Pass));
    }

    [Test]
    public void Diagnostics_ReportsDisabledProfileCloudSync()
    {
        string root = CreateTemporaryRoot();
        GameInfrastructureServices services = GameInfrastructureServices.CreateLocal(storageRoot: root);
        services.InitializeAsync().GetAwaiter().GetResult();

        InfrastructureDiagnosticsSnapshot diagnostics = services.Diagnostics.Capture();

        Assert.That(diagnostics.ProfileCloudSync, Is.Not.Null);
        Assert.That(diagnostics.ProfileCloudSync.LiveSyncEnabled, Is.False);
        Assert.That(diagnostics.ProfileCloudSync.CanUpload, Is.False);
        Assert.That(diagnostics.ProfileCloudSync.CanDownload, Is.False);
        Assert.That(diagnostics.ProfileCloudSync.CloudKey, Is.EqualTo("bmm.profile_settings.v2"));
    }

    [Test]
    public void JournalPolicy_StagesSafeEventsWithoutUploadingSensitivePayloads()
    {
        string root = CreateTemporaryRoot();
        GameInfrastructureServices services = GameInfrastructureServices.CreateLocal(storageRoot: root);
        services.InitializeAsync().GetAwaiter().GetResult();
        string playerId = services.Identity.Current.PlayerId;
        services.ActionJournal.RecordAction(
            playerId,
            "profile_settings",
            "avatar_selected",
            "{\"AvatarId\":\"avatar_moon_witch\"}",
            status: JournalActionStatus.AppliedLocal);
        services.ActionJournal.RecordAction(
            playerId,
            "friend_message",
            "sent",
            "{\"message\":\"keep-this-local\"}",
            status: JournalActionStatus.AppliedLocal);

        InfrastructureDiagnosticsSnapshot diagnostics = services.Diagnostics.Capture();

        Assert.That(diagnostics.JournalSyncPolicy.LiveUploadsEnabled, Is.False);
        Assert.That(diagnostics.JournalSyncPolicy.ActiveUploadEligibleRecordCount, Is.EqualTo(0));
        Assert.That(diagnostics.JournalSyncPolicy.FutureUploadEligibleRecordCount, Is.GreaterThanOrEqualTo(2));
        Assert.That(diagnostics.JournalSyncPolicy.BlockedSensitiveRecordCount, Is.EqualTo(1));
        Assert.That(diagnostics.JournalSyncPolicy.SourceSummaries, Has.Some.Matches<JournalPolicySourceSummary>(summary =>
            summary.Event == "friend_message/sent" && summary.BlockedSensitiveCount == 1));

        string export = File.ReadAllText(services.Diagnostics.ExportSafeSummary());
        Assert.That(export, Does.Not.Contain("keep-this-local"));
        Assert.That(export, Does.Not.Contain(playerId));
    }

    [Test]
    public void DurableState_RejectsNewerSnapshotWithoutDowngradingToBackup()
    {
        string root = CreateTemporaryRoot();
        LocalStateMigrationRegistry migrations = new LocalStateMigrationRegistry();
        migrations.RegisterState("future", 1);
        SnapshotEnvelopeForTest future = new SnapshotEnvelopeForTest
        {
            SchemaVersion = 1,
            StateSchemaVersion = 2,
            SavedAtUtc = DateTime.UtcNow.ToString("O"),
            PayloadJson = JsonUtility.ToJson(new MigratedStateV2 { SchemaVersion = 2, Count = 12 })
        };
        SnapshotEnvelopeForTest olderBackup = new SnapshotEnvelopeForTest
        {
            SchemaVersion = 1,
            StateSchemaVersion = 1,
            SavedAtUtc = DateTime.UtcNow.AddMinutes(-1).ToString("O"),
            PayloadJson = JsonUtility.ToJson(new LegacyStateV1 { SchemaVersion = 1, Count = 3 })
        };
        File.WriteAllText(Path.Combine(root, "future.snapshot.json"), JsonUtility.ToJson(future));
        File.WriteAllText(Path.Combine(root, "future.snapshot.json.bak"), JsonUtility.ToJson(olderBackup));

        JsonFileDurableStateStore store = new JsonFileDurableStateStore(root, migrations);

        LogAssert.Expect(
            LogType.Error,
            "Cannot load local state 'future': Snapshot 'future' uses schema 2, newer than supported schema 1.");

        Assert.That(store.TryLoad("future", out MigratedStateV2 loaded), Is.False);
        Assert.That(loaded, Is.Null);
        Assert.That(store.LastRecoveredState, Is.EqualTo("none"));
    }

    private string CreateTemporaryRoot()
    {
        string root = Path.Combine(Path.GetTempPath(), "BingoMagicMayhemTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        temporaryRoots.Add(root);
        return root;
    }

    [Serializable]
    private sealed class TestState
    {
        public int Count;
        public string Label = "";
    }

    [Serializable]
    private sealed class LegacyStateV1
    {
        public int SchemaVersion;
        public int Count;
    }

    [Serializable]
    private sealed class MigratedStateV2 : IVersionedLocalState
    {
        public int SchemaVersion;
        public int Count;
        public string Label = "";

        int IVersionedLocalState.SchemaVersion => SchemaVersion;
    }

    [Serializable]
    private sealed class SnapshotEnvelopeForTest
    {
        public int SchemaVersion;
        public int StateSchemaVersion;
        public string SavedAtUtc = "";
        public string PayloadJson = "";
    }

    [Serializable]
    private sealed class LegacyProfileSettingsV1
    {
        public int SchemaVersion;
        public string AvatarId = "";
        public string FrameId = "";
        public string DauberId = "";
        public bool SoundEnabled;
        public bool NotificationsEnabled;
    }
}

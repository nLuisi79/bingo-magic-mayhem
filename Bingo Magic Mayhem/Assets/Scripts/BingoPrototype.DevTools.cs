using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoMagicMayhem.Cosmetics;
using BingoMagicMayhem.Infrastructure;
using BingoMagicMayhem.Jackpot;
using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using BingoMagicMayhem.UI.Common;
using BingoMagicMayhem.UI.Bazaar;
using BingoMagicMayhem.UI.Collections;
using BingoMagicMayhem.UI.Coven;
using BingoMagicMayhem.UI.Den;
using BingoMagicMayhem.UI.Friends;
using BingoMagicMayhem.UI.Inbox;
using BingoMagicMayhem.UI.Leaderboard;
using BingoMagicMayhem.UI.Market;
using BingoMagicMayhem.UI.Rewards;
using BingoMagicMayhem.UI.RoomEntry;
using BingoMagicMayhem.UI.Trail;
using BingoMagicMayhem.UI.Navigation;
using BingoMagicMayhem.UI.Profile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Dev settings, infrastructure diagnostics, album debug and Beta QA reset tools. Not player-facing.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildDevSettingsUi(bool returnToMap)
    {
        BuildDevSettingsUi(returnToMap, false);
    }

    private void BuildDevSettingsUi(bool returnToMap, bool returnToDen)
    {
        devSettingsReturnToDen = returnToDen;
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.1f, 0.04f, 0.16f, 0.98f), 780, 600, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        CreateAnchoredText(panel.transform, "PROTOTYPE SETTINGS", 32, FontStyle.Bold, gold, 700, 44, 0f, 254f);
        CreateAnchoredText(panel.transform, "Hidden tuning tools for clean economy and progression testing.", 17, FontStyle.Bold, new Color(0.85f, 0.82f, 0.95f), 700, 28, 0f, 214f);

        CreateAnchoredText(panel.transform, $"Mana {inventory.GetManaText()}  |  Crystals {inventory.GetCrystalText()}  |  Wheelspins {inventory.PendingJackpotSpins}", 20, FontStyle.Bold, Color.white, 700, 34, 0f, 174f);
        CreateAnchoredText(panel.transform, $"Power-Ups {inventory.GetPowerUpText()}  |  Clairvoyance stock {inventory.ClairvoyanceMinutes}m  |  Active {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}", 18, FontStyle.Bold, new Color(0.85f, 0.95f, 1f), 700, 34, 0f, 142f);

        Button inspect = CreateAnchoredButton(panel.transform, "Profile State", 18, 220, 42, new Color(0.2f, 0.16f, 0.42f), -250f, 86f);
        inspect.onClick.AddListener(() => BuildProfileStateUi(returnToMap));

        Button reset = CreateAnchoredButton(panel.transform, "Fresh New Player", 18, 220, 42, new Color(0.45f, 0.08f, 0.12f), 0f, 86f);
        reset.onClick.AddListener(() => ResetPrototypeSave(returnToMap));

        Button progressReset = CreateAnchoredButton(panel.transform, "Reset Rooms Only", 18, 220, 42, new Color(0.45f, 0.22f, 0.08f), 250f, 86f);
        progressReset.onClick.AddListener(() => ResetRoomProgressKeepingInventory(returnToMap));

        Button mana = CreateAnchoredButton(panel.transform, "+500 Mana", 18, 180, 42, new Color(0.18f, 0.34f, 0.1f), -260f, 28f);
        mana.onClick.AddListener(() => GrantPrototypeMana(500, returnToMap));

        Button crystals = CreateAnchoredButton(panel.transform, "+100 Crystals", 18, 210, 42, new Color(0.14f, 0.25f, 0.48f), -30f, 28f);
        crystals.onClick.AddListener(() => GrantPrototypeCrystals(100, returnToMap));

        Button clairvoyance = CreateAnchoredButton(panel.transform, "+15 Clairvoyance", 18, 230, 42, new Color(0.25f, 0.12f, 0.43f), 230f, 28f);
        clairvoyance.onClick.AddListener(() => GrantPrototypeClairvoyance(returnToMap));

        Button wheelspin = CreateAnchoredButton(panel.transform, "+1 Wheelspin", 18, 220, 42, new Color(0.35f, 0.08f, 0.48f), -250f, -30f);
        wheelspin.onClick.AddListener(() => GrantPrototypeJackpotSpin(returnToMap));

        Button powerUps = CreateAnchoredButton(panel.transform, "+1 Each Power-Up", 18, 240, 42, new Color(0.14f, 0.4f, 0.42f), 0f, -30f);
        powerUps.onClick.AddListener(() => GrantPrototypePowerUpSet(returnToMap));

        Button unlock = CreateAnchoredButton(panel.transform, "Unlock Next Realm", 18, 230, 42, new Color(0.28f, 0.34f, 0.08f), 260f, -30f);
        unlock.onClick.AddListener(() => UnlockNextRealmForTesting(returnToMap));

        Button dailySpinReset = CreateAnchoredButton(panel.transform, "Reset Daily Spin", 18, 220, 42, new Color(0.35f, 0.12f, 0.62f), -250f, -86f);
        dailySpinReset.onClick.AddListener(() => ResetDailySpinForTesting(returnToMap));

        Button persistence = CreateAnchoredButton(panel.transform, "Persistence", 18, 220, 42, new Color(0.12f, 0.3f, 0.42f), 0f, -86f);
        persistence.onClick.AddListener(() => BuildInfrastructureDiagnosticsUi(returnToMap));

        Button albumDebug = CreateAnchoredButton(panel.transform, "Album Debug", 18, 220, 42, new Color(0.2f, 0.16f, 0.42f), 250f, -86f);
        albumDebug.onClick.AddListener(() => BuildAlbumDebugUi(returnToMap));

        Button sunpetalStates = CreateAnchoredButton(panel.transform, "Sunpetal Mixed QA", 18, 240, 42, new Color(0.12f, 0.42f, 0.24f), 0f, -142f);
        sunpetalStates.onClick.AddListener(SetSunpetalMixedStateForTesting);

        Button betaQa = CreateAnchoredButton(panel.transform, "Beta QA Resets", 18, 220, 42, new Color(0.42f, 0.24f, 0.08f), 250f, -142f);
        betaQa.onClick.AddListener(() => BuildBetaQaResetsUi(returnToMap, ""));

        CreateAnchoredText(panel.transform, "Fresh New Player resets progression and profile cosmetics while preserving preferences. Sunpetal Mixed QA shows restored, unlocked-incomplete, and locked room states together.", 15, FontStyle.Bold, new Color(0.86f, 0.82f, 0.95f), 680, 58, 0f, -194f);

        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 180, 46, new Color(0.35f, 0.12f, 0.62f), 0f, -252f);
        close.onClick.AddListener(() =>
        {
            if (returnToMap)
            {
                BuildWorldMapUi();
                return;
            }

            if (devSettingsReturnToDen)
            {
                BuildPlayerDenUi();
                return;
            }

            BuildLobbyUi();
        });
    }

    private void BuildProfileStateUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.08f, 0.03f, 0.13f, 0.98f), 940, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        CreateAnchoredText(panel.transform, "SAVED PROFILE STATE", 32, FontStyle.Bold, gold, 820, 44, 0f, 302f);
        CreateAnchoredText(panel.transform, BuildProfileStateSummary(), 16, FontStyle.Bold, Color.white, 840, 500, 0f, 20f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), -140f, -300f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));

        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 170, 46, new Color(0.18f, 0.16f, 0.22f), 130f, -300f);
        close.onClick.AddListener(() => ReturnFromDevSettings(returnToMap));
    }

    private void BuildInfrastructureDiagnosticsUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.055f, 0.035f, 0.1f, 0.99f), 980, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        CreateAnchoredText(panel.transform, "LOCAL PERSISTENCE", 34, FontStyle.Bold, gold, 860, 46, 0f, 302f);
        CreateAnchoredText(panel.transform, "Redacted Beta diagnostics only - no payloads, messages, tokens, or full player id.", 15, FontStyle.Bold, muted, 860, 28, 0f, 266f);
        CreateAnchoredText(panel.transform, BuildInfrastructureDiagnosticsSummary(), 16, FontStyle.Bold, Color.white, 860, 430, 0f, 30f);

        Button export = CreateAnchoredButton(panel.transform, "Export Safe Summary", 18, 250, 46, new Color(0.12f, 0.42f, 0.46f), -270f, -278f);
        export.interactable = GameInfrastructureRuntime.IsInitialized;
        export.onClick.AddListener(() => ExportInfrastructureDiagnostics(returnToMap));

        Button refresh = CreateAnchoredButton(panel.transform, "Refresh", 18, 170, 46, new Color(0.2f, 0.16f, 0.42f), 0f, -278f);
        refresh.onClick.AddListener(() => BuildInfrastructureDiagnosticsUi(returnToMap));

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 18, 230, 46, new Color(0.35f, 0.12f, 0.62f), 250f, -278f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));
    }

    private string BuildInfrastructureDiagnosticsSummary()
    {
        if (!GameInfrastructureRuntime.IsInitialized || GameInfrastructureRuntime.Current.Diagnostics == null)
        {
            return "Infrastructure has not initialized. Profile/settings are using session defaults.";
        }

        InfrastructureDiagnosticsSnapshot diagnostics = GameInfrastructureRuntime.Current.Diagnostics.Capture();
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Environment: {diagnostics.Environment} | Identity: {diagnostics.IdentityProvider} | Player: {diagnostics.RedactedPlayerId}");
        builder.AppendLine($"Snapshots: {diagnostics.SnapshotCount} | Journal: {diagnostics.JournalRecordCount} records / {FormatDiagnosticBytes(diagnostics.JournalBytes)} | Pending action rows: {diagnostics.PendingActionRecordCount}");
        builder.AppendLine($"Last sequence: {diagnostics.LastJournalSequence} | Recovery: {diagnostics.LastRecoveredState} {diagnostics.LastRecoveryAtUtc}");
        builder.AppendLine($"Migration: {diagnostics.LastMigratedState} {diagnostics.LastMigration}");
        if (diagnostics.JournalSyncPolicy != null)
        {
            builder.AppendLine($"Journal policy: {diagnostics.JournalSyncPolicy.PolicyVersion} | live uploads {(diagnostics.JournalSyncPolicy.LiveUploadsEnabled ? "on" : "off")} | active upload eligible {diagnostics.JournalSyncPolicy.ActiveUploadEligibleRecordCount}");
            builder.AppendLine($"Future upload candidates: {diagnostics.JournalSyncPolicy.FutureUploadEligibleRecordCount} | blocked sensitive {diagnostics.JournalSyncPolicy.BlockedSensitiveRecordCount} | blocked unapproved {diagnostics.JournalSyncPolicy.BlockedUnapprovedRecordCount}");
        }

        if (diagnostics.JournalRetentionPolicy != null)
        {
            builder.AppendLine($"Journal retention: {diagnostics.JournalRetentionPolicy.PolicyVersion} | retained {diagnostics.JournalRetentionPolicy.RetainedRecordCount} | archive candidates {diagnostics.JournalRetentionPolicy.ArchiveCandidateCount} | delete blocked {(diagnostics.JournalRetentionPolicy.DeleteEnabled ? "off" : "on")}");
        }

        if (diagnostics.BackendPreflight != null)
        {
            builder.AppendLine($"UGS preflight: packages {diagnostics.BackendPreflight.PackageState}, adapters {diagnostics.BackendPreflight.AdapterDefine}, live calls {(diagnostics.BackendPreflight.LiveCloudCallsEnabled ? "on" : "off")}");
            builder.AppendLine($"Cloud enablement blockers: {diagnostics.BackendPreflight.BlockedCount} | Warnings: {diagnostics.BackendPreflight.WarningCount}");
        }

        if (diagnostics.Composition != null)
        {
            builder.AppendLine($"Composition: identity {diagnostics.Composition.ActiveIdentityProvider} / wants {diagnostics.Composition.DesiredIdentityProvider} | analytics {diagnostics.Composition.ActiveAnalyticsProvider} / wants {diagnostics.Composition.DesiredAnalyticsProvider}");
            builder.AppendLine($"Profile sync {diagnostics.Composition.ActiveProfileCloudSyncProvider} / wants {diagnostics.Composition.DesiredProfileCloudSyncProvider} | local fallback {(diagnostics.Composition.UsesLocalFallback ? "on" : "off")}");
        }

        if (diagnostics.IdentitySafety != null)
        {
            builder.AppendLine($"Identity safety: {diagnostics.IdentitySafety.PolicyVersion} | provider {diagnostics.IdentitySafety.Provider} | cloud auth {(diagnostics.IdentitySafety.IsCloudAuthenticated ? "on" : "off")} | account link {(diagnostics.IdentitySafety.AllowsAccountLink ? "ready" : "blocked")}");
        }

        if (diagnostics.AnalyticsSafety != null)
        {
            builder.AppendLine($"Analytics safety: {diagnostics.AnalyticsSafety.PolicyVersion} | upload {(diagnostics.AnalyticsSafety.LiveUploadEnabled ? "on" : "off")} | allowlisted {diagnostics.AnalyticsSafety.AllowlistedAnalyticsRecordCount} | blocked {diagnostics.AnalyticsSafety.BlockedAnalyticsRecordCount}");
        }

        if (diagnostics.ExportSafety != null)
        {
            builder.AppendLine($"Export safety: {diagnostics.ExportSafety.PolicyVersion} | local file {(diagnostics.ExportSafety.LocalFileExportEnabled ? "ready" : "blocked")} | external share {(diagnostics.ExportSafety.ExternalShareEnabled ? "ready" : "blocked")} | redaction rows {diagnostics.ExportSafety.ExportBlockedRecordCount}");
        }

        if (diagnostics.ProfileCloudSync != null)
        {
            builder.AppendLine($"Profile Cloud Save: live sync {(diagnostics.ProfileCloudSync.LiveSyncEnabled ? "on" : "off")} | upload {(diagnostics.ProfileCloudSync.CanUpload ? "ready" : "blocked")} | download {(diagnostics.ProfileCloudSync.CanDownload ? "ready" : "blocked")}");
            builder.AppendLine($"Profile Cloud key: {diagnostics.ProfileCloudSync.CloudKey}");
            if (diagnostics.ProfileCloudSync.ConflictPolicy != null)
            {
                builder.AppendLine($"Conflict policy: {diagnostics.ProfileCloudSync.ConflictPolicy.PolicyVersion} | mode {diagnostics.ProfileCloudSync.ConflictPolicy.ConflictMode} | local authoritative {(diagnostics.ProfileCloudSync.ConflictPolicy.LocalSnapshotAuthoritative ? "yes" : "no")}");
            }
        }

        if (diagnostics.RemoteConfigSafety != null)
        {
            builder.AppendLine($"Remote Config safety: {diagnostics.RemoteConfigSafety.PolicyVersion} | risky enabled {diagnostics.RemoteConfigSafety.RiskyEnabledKeyCount} | missing {diagnostics.RemoteConfigSafety.MissingRequiredKeyCount} | unknown {diagnostics.RemoteConfigSafety.UnknownKeyCount}");
        }

        builder.AppendLine();
        builder.AppendLine("SNAPSHOT HEALTH");
        foreach (SnapshotDiagnosticsEntry entry in diagnostics.Snapshots)
        {
            builder.AppendLine($"- {entry.StateName} v{entry.SchemaVersion}: {entry.Health}, {FormatDiagnosticBytes(entry.Bytes)}, backup {(entry.HasBackup ? "yes" : "no")}");
        }

        builder.AppendLine();
        builder.AppendLine("JOURNAL SYNC STAGING");
        if (diagnostics.JournalSyncPolicy != null)
        {
            builder.AppendLine($"- retain local: {diagnostics.JournalSyncPolicy.RetainLocalRecordCount}, summary-export allowed: {diagnostics.JournalSyncPolicy.ExportSummaryAllowedRecordCount}");
            builder.AppendLine($"- status rows: pending {diagnostics.JournalSyncPolicy.PendingRecordCount}, applied {diagnostics.JournalSyncPolicy.AppliedLocalRecordCount}, synced {diagnostics.JournalSyncPolicy.SyncedRecordCount}, rejected {diagnostics.JournalSyncPolicy.RejectedRecordCount}, compensated {diagnostics.JournalSyncPolicy.CompensatedRecordCount}");
            builder.AppendLine("- live upload queue is disabled; no journal rows are uploaded in this build.");
        }

        builder.AppendLine();
        builder.AppendLine("JOURNAL RETENTION");
        if (diagnostics.JournalRetentionPolicy != null)
        {
            builder.AppendLine($"- retention enabled {diagnostics.JournalRetentionPolicy.RetentionEnabled}, archive enabled {diagnostics.JournalRetentionPolicy.ArchiveEnabled}, compaction enabled {diagnostics.JournalRetentionPolicy.CompactionEnabled}, delete enabled {diagnostics.JournalRetentionPolicy.DeleteEnabled}");
            builder.AppendLine($"- total rows {diagnostics.JournalRetentionPolicy.TotalRecordCount}, retained {diagnostics.JournalRetentionPolicy.RetainedRecordCount}, archive candidates {diagnostics.JournalRetentionPolicy.ArchiveCandidateCount}, compaction candidates {diagnostics.JournalRetentionPolicy.CompactionCandidateCount}, delete candidates {diagnostics.JournalRetentionPolicy.DeleteCandidateCount}");
            builder.AppendLine($"- export allowed {diagnostics.JournalRetentionPolicy.ExportAllowed}, export-blocked rows {diagnostics.JournalRetentionPolicy.ExportBlockedRecordCount}, sensitive redaction required {diagnostics.JournalRetentionPolicy.SensitivePayloadRedactionRequired}");
            builder.AppendLine($"- reason: {diagnostics.JournalRetentionPolicy.Reason}");
            foreach (BackendPreflightCheck check in diagnostics.JournalRetentionPolicy.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("ANALYTICS SAFETY");
        if (diagnostics.AnalyticsSafety != null)
        {
            builder.AppendLine($"- adapter compiled {diagnostics.AnalyticsSafety.AdapterCompiled}, consent approved {diagnostics.AnalyticsSafety.ConsentApproved}, live upload enabled {diagnostics.AnalyticsSafety.LiveUploadEnabled}");
            builder.AppendLine($"- total analytics rows {diagnostics.AnalyticsSafety.TotalAnalyticsRecordCount}, allowlisted {diagnostics.AnalyticsSafety.AllowlistedAnalyticsRecordCount}, blocked {diagnostics.AnalyticsSafety.BlockedAnalyticsRecordCount}");
            builder.AppendLine($"- remote flags request upload {diagnostics.AnalyticsSafety.RemoteFlagsRequestUpload}, runtime upload allowed {diagnostics.AnalyticsSafety.AllowsRuntimeUpload}");
            builder.AppendLine($"- reason: {diagnostics.AnalyticsSafety.Reason}");
            foreach (BackendPreflightCheck check in diagnostics.AnalyticsSafety.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("EXPORT SAFETY");
        if (diagnostics.ExportSafety != null)
        {
            builder.AppendLine($"- local file export enabled {diagnostics.ExportSafety.LocalFileExportEnabled}, payload-free only {diagnostics.ExportSafety.PayloadFreeOnly}, sensitive redaction required {diagnostics.ExportSafety.SensitivePayloadRedactionRequired}");
            builder.AppendLine($"- external share enabled {diagnostics.ExportSafety.ExternalShareEnabled}, in-app share enabled {diagnostics.ExportSafety.InAppShareEnabled}, clipboard copy enabled {diagnostics.ExportSafety.ClipboardCopyEnabled}");
            builder.AppendLine($"- Remote Config requests export enabled {diagnostics.ExportSafety.RemoteConfigRequestsExportEnabled}, Remote Config can disable local export {diagnostics.ExportSafety.RemoteConfigCanDisableLocalExport}, export-blocked rows {diagnostics.ExportSafety.ExportBlockedRecordCount}");
            builder.AppendLine($"- reason: {diagnostics.ExportSafety.Reason}");
            foreach (BackendPreflightCheck check in diagnostics.ExportSafety.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("COMPOSITION");
        if (diagnostics.Composition != null)
        {
            builder.AppendLine($"- desired identity provider {diagnostics.Composition.DesiredIdentityProvider}, active {diagnostics.Composition.ActiveIdentityProvider}");
            builder.AppendLine($"- desired analytics provider {diagnostics.Composition.DesiredAnalyticsProvider}, active {diagnostics.Composition.ActiveAnalyticsProvider}");
            builder.AppendLine($"- desired profile sync provider {diagnostics.Composition.DesiredProfileCloudSyncProvider}, active {diagnostics.Composition.ActiveProfileCloudSyncProvider}");
            builder.AppendLine($"- UGS adapters compiled {diagnostics.Composition.UgsAdaptersCompiled}, uses local fallback {diagnostics.Composition.UsesLocalFallback}");
            if (!string.IsNullOrWhiteSpace(diagnostics.Composition.FallbackReason))
            {
                builder.AppendLine($"- fallback reason: {diagnostics.Composition.FallbackReason}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("IDENTITY SAFETY");
        if (diagnostics.IdentitySafety != null)
        {
            builder.AppendLine($"- provider: {diagnostics.IdentitySafety.Provider}, cloud authenticated {diagnostics.IdentitySafety.IsCloudAuthenticated}, anonymous {diagnostics.IdentitySafety.IsAnonymous}");
            builder.AppendLine($"- adapter compiled {diagnostics.IdentitySafety.AdapterCompiled}, cloud sign-in allowed {diagnostics.IdentitySafety.AllowsCloudSignIn}, account link allowed {diagnostics.IdentitySafety.AllowsAccountLink}, recovery allowed {diagnostics.IdentitySafety.AllowsRecovery}");
            builder.AppendLine($"- remote flags request live auth {diagnostics.IdentitySafety.RemoteFlagsRequestLiveAuth}, live runtime changes allowed {diagnostics.IdentitySafety.LiveRuntimeChangeAllowed}");
            builder.AppendLine($"- reason: {diagnostics.IdentitySafety.Reason}");
            foreach (BackendPreflightCheck check in diagnostics.IdentitySafety.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("PROFILE CLOUD SAVE SYNC");
        if (diagnostics.ProfileCloudSync != null)
        {
            builder.AppendLine($"- service: {diagnostics.ProfileCloudSync.Service}, adapter compiled {(diagnostics.ProfileCloudSync.AdapterCompiled ? "yes" : "no")}");
            builder.AppendLine($"- local snapshots remain authoritative; upload {diagnostics.ProfileCloudSync.CanUpload}, download {diagnostics.ProfileCloudSync.CanDownload}");
            builder.AppendLine($"- reason: {diagnostics.ProfileCloudSync.Reason}");
            if (diagnostics.ProfileCloudSync.ConflictPolicy != null)
            {
                builder.AppendLine($"- policy: {diagnostics.ProfileCloudSync.ConflictPolicy.PolicyVersion}, required approvals {diagnostics.ProfileCloudSync.ConflictPolicy.RequiredApprovalCount}, blocked gates {diagnostics.ProfileCloudSync.ConflictPolicy.BlockedGateCount}");
                builder.AppendLine($"- cloud overwrite allowed: {diagnostics.ProfileCloudSync.ConflictPolicy.AllowsRemoteOverwrite}, auto-merge allowed: {diagnostics.ProfileCloudSync.ConflictPolicy.AllowsAutomaticMerge}, gameplay sync allowed: {diagnostics.ProfileCloudSync.ConflictPolicy.AllowsGameplayStateSync}");
                builder.AppendLine($"- next approval: {diagnostics.ProfileCloudSync.ConflictPolicy.NextRequiredApproval}");
                foreach (BackendPreflightCheck check in diagnostics.ProfileCloudSync.ConflictPolicy.Checks)
                {
                    builder.AppendLine($"- policy/{check.Name}: {check.Status} - {check.Detail}");
                }
            }
            foreach (BackendPreflightCheck check in diagnostics.ProfileCloudSync.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("REMOTE CONFIG SAFETY");
        if (diagnostics.RemoteConfigSafety != null)
        {
            builder.AppendLine($"- source: {diagnostics.RemoteConfigSafety.Source}, revision {diagnostics.RemoteConfigSafety.Revision}, live runtime changes allowed {diagnostics.RemoteConfigSafety.LiveRuntimeChangeAllowed}");
            builder.AppendLine($"- UGS adapters {diagnostics.RemoteConfigSafety.UgsAdaptersEnabled}, Cloud profile sync {diagnostics.RemoteConfigSafety.CloudProfileSyncEnabled}, journal upload {diagnostics.RemoteConfigSafety.JournalUploadEnabled}, diagnostics export {diagnostics.RemoteConfigSafety.DiagnosticsExportEnabled}");
            builder.AppendLine($"- required keys: {diagnostics.RemoteConfigSafety.PresentRequiredKeyCount}/{diagnostics.RemoteConfigSafety.RequiredKeyCount}, risky enabled {diagnostics.RemoteConfigSafety.RiskyEnabledKeyCount}, unknown {diagnostics.RemoteConfigSafety.UnknownKeyCount}");
            builder.AppendLine($"- reason: {diagnostics.RemoteConfigSafety.Reason}");
            foreach (BackendPreflightCheck check in diagnostics.RemoteConfigSafety.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("UGS ENABLEMENT");
        if (diagnostics.BackendPreflight != null)
        {
            foreach (BackendPreflightCheck check in diagnostics.BackendPreflight.Checks)
            {
                builder.AppendLine($"- {check.Name}: {check.Status} - {check.Detail}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("Journal retention/clearing is not active until policy is approved.");
        if (!string.IsNullOrWhiteSpace(lastInfrastructureDiagnosticsExport))
        {
            builder.AppendLine($"Last safe export: {lastInfrastructureDiagnosticsExport}");
        }

        return builder.ToString();
    }

    private void ExportInfrastructureDiagnostics(bool returnToMap)
    {
        if (!GameInfrastructureRuntime.IsInitialized || GameInfrastructureRuntime.Current.Diagnostics == null)
        {
            return;
        }

        string exportPath = GameInfrastructureRuntime.Current.Diagnostics.ExportSafeSummary();
        lastInfrastructureDiagnosticsExport = System.IO.Path.GetFileName(exportPath);
        BuildInfrastructureDiagnosticsUi(returnToMap);
    }

    private static string FormatDiagnosticBytes(long bytes)
    {
        if (bytes < 1024)
        {
            return bytes + " B";
        }

        return (bytes / 1024f).ToString("0.0") + " KB";
    }

    private void BuildAlbumDebugUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.08f, 0.03f, 0.13f, 0.98f), 1080, 700, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.96f, 0.9f, 0.72f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        int regularCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Regular);
        int gildedCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Gilded);
        int ancientCards = inventory.GetOwnedGrimoireCardCountByTier(AlbumCardTier.Ancient);

        CreateAnchoredText(panel.transform, "ALBUM DEBUG", 34, FontStyle.Bold, gold, 980, 46, 0f, 302f);
        CreateAnchoredText(panel.transform, "Testing-only card routing, ownership, duplicates, and Book of Shadows state.", 16, FontStyle.Bold, muted, 980, 26, 0f, 266f);

        CreateAnchoredPanel(panel.transform, "AlbumDebugTotals", new Color(0.14f, 0.07f, 0.18f), 920, 74, 0f, 214f);
        CreateLibraryStat(panel.transform, "Grimoire", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards}", -330f, 214f);
        CreateLibraryStat(panel.transform, "Regular", $"{regularCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Regular)}", -110f, 214f);
        CreateLibraryStat(panel.transform, "Gilded", $"{gildedCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Gilded)}", 110f, 214f);
        CreateLibraryStat(panel.transform, "Ancient", $"{ancientCards}/{CardAlbumCatalog.CountByTier(AlbumCardTier.Ancient)}", 330f, 214f);

        CreateLibrarySection(panel.transform, "Star System", -270f, 46f, 410, 254, new Color(0.12f, 0.06f, 0.2f));
        for (int stars = 1; stars <= 5; stars++)
        {
            CreateLibraryStarRow(panel.transform, stars, -270f, 124f - (stars - 1) * 42f);
        }

        IReadOnlyList<GrimoireEntryDefinition> entries = CardAlbumCatalog.GrimoireOneEntries;
        selectedGrimoireEntryIndex = Mathf.Clamp(selectedGrimoireEntryIndex, 0, entries.Count - 1);
        GrimoireEntryDefinition selectedEntry = entries[selectedGrimoireEntryIndex];
        CreateLibrarySection(panel.transform, "Grimoire Card Slots", 220f, 46f, 470, 254, new Color(0.1f, 0.05f, 0.15f));
        CreateAnchoredText(panel.transform, $"Potion {selectedEntry.EntryNumber}/{entries.Count}: {selectedEntry.PotionName}", 16, FontStyle.Bold, cream, 420, 28, 220f, 122f);
        CreateAnchoredText(panel.transform, $"{inventory.GetOwnedGrimoireEntryCardCount(selectedEntry)}/{selectedEntry.Cards.Count} owned | duplicates total {inventory.GetGrimoireDuplicateCount()}", 14, FontStyle.Bold, gold, 420, 22, 220f, 92f);
        CreateGrimoireCardSlotGrid(panel.transform, selectedEntry, 220f, 24f);
        Button previousEntry = CreateAnchoredButton(panel.transform, "<", 18, 54, 34, new Color(0.35f, 0.12f, 0.62f), 90f, -118f);
        previousEntry.interactable = selectedGrimoireEntryIndex > 0;
        previousEntry.onClick.AddListener(() => SelectAlbumDebugGrimoireEntry(selectedGrimoireEntryIndex - 1, returnToMap));
        Button nextEntry = CreateAnchoredButton(panel.transform, ">", 18, 54, 34, new Color(0.35f, 0.12f, 0.62f), 350f, -118f);
        nextEntry.interactable = selectedGrimoireEntryIndex < entries.Count - 1;
        nextEntry.onClick.AddListener(() => SelectAlbumDebugGrimoireEntry(selectedGrimoireEntryIndex + 1, returnToMap));

        CreateLibrarySection(panel.transform, "Book of Shadows Debug", 0f, -204f, 920, 150, new Color(0.11f, 0.055f, 0.18f));
        IReadOnlyList<BookOfShadowsSetDefinition> shadowSets = CardAlbumCatalog.BookOfShadowsSets;
        int activeShadowSetIndex = Mathf.Clamp(inventory.GetActiveBookOfShadowsSetNumber() - 1, 0, shadowSets.Count - 1);
        BookOfShadowsSetDefinition activeShadowSet = shadowSets[activeShadowSetIndex];
        CreateAnchoredText(panel.transform, $"{inventory.GetBookOfShadowsSeasonSummaryText()} | Purchased: {(inventory.BookOfShadowsPurchased ? "YES" : "NO")}", 16, FontStyle.Bold, cream, 820, 24, 0f, -170f);
        CreateAnchoredText(panel.transform, $"Active set owned: {inventory.GetOwnedBookOfShadowsSetCardCount(activeShadowSet)}/{CardAlbumCatalog.CountBookOfShadowsInSet(activeShadowSet)} | Total: {inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}", 15, FontStyle.Bold, gold, 820, 24, 0f, -204f);
        CreateAnchoredText(panel.transform, inventory.GetAlbumCompletionHookText(), 13, FontStyle.Bold, Color.white, 820, 48, 0f, -246f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), -140f, -310f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));
        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 170, 46, new Color(0.18f, 0.16f, 0.22f), 130f, -310f);
        close.onClick.AddListener(() => ReturnFromDevSettings(returnToMap));
    }

    private void SelectAlbumDebugGrimoireEntry(int entryIndex, bool returnToMap)
    {
        selectedGrimoireEntryIndex = Mathf.Clamp(entryIndex, 0, CardAlbumCatalog.GrimoireOneEntries.Count - 1);
        BuildAlbumDebugUi(returnToMap);
    }

    private string BuildProfileStateSummary()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Currency: {inventory.GetManaText()} mana | {inventory.GetCrystalText()} crystals | {inventory.GetPowerUpText()} power-ups");
        builder.AppendLine($"Account: Level {rewards.CurrentLevel} | {rewards.GetLevelProgressText()} | Aura Strength TBD | Rank thresholds TBD");
        builder.AppendLine($"Clairvoyance: stock {inventory.ClairvoyanceMinutes}m | active {(inventory.HasActiveClairvoyance() ? inventory.GetActiveClairvoyanceTimeText() : "OFF")}");
        builder.AppendLine($"Wheelspins: {inventory.PendingJackpotSpins} pending | Cauldron {inventory.ManaCauldronAmount}/{inventory.ManaCauldronCapacity} | {inventory.GetManaCauldronCountdownText()} | +{inventory.GetManaCauldronHourlyRefillAmount()}/hr");
        builder.AppendLine($"Active: Realm {RealmContentCatalog.ActivePrototypeRealmIndex + 1} - {RealmContentCatalog.ActivePrototypeRealm.Name} | Room {RealmContentCatalog.ActivePrototypeRoomIndex + 1} - {RealmContentCatalog.ActivePrototypeRoom.Name}");
        builder.AppendLine();

        for (int realmIndex = 0; realmIndex < RealmContentCatalog.AllRealms.Count; realmIndex++)
        {
            RealmDefinition realm = RealmContentCatalog.AllRealms[realmIndex];
            int restoredRooms = GetRestoredRoomCount(realm);
            string realmState = IsRealmUnlocked(realmIndex) ? IsRealmComplete(realmIndex) ? "Complete" : "Unlocked" : "Locked";
            builder.AppendLine($"R{realmIndex + 1}: {realm.Name} - {realmState} - {restoredRooms}/{realm.Rooms.Count} rooms restored");
        }

        builder.AppendLine();
        builder.AppendLine("Current realm rooms:");
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.ActivePrototypeRealm.Rooms;
        for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            RoomDefinition room = rooms[roomIndex];
            string openState = IsRoomUnlocked(roomIndex) ? "Open" : "Locked";
            string restoreState = inventory.IsRoomRestored(room) ? "Restored" : inventory.CanRestoreRoom(room) ? "Ready" : "In progress";
            builder.AppendLine($"{roomIndex + 1}. {room.Name} - {openState}, {restoreState}");
            builder.AppendLine($"   {inventory.GetFullIngredientProgressText(room).Replace("\n", " | ")}");
        }

        return builder.ToString();
    }

    private int GetRestoredRoomCount(RealmDefinition realm)
    {
        int count = 0;
        IReadOnlyList<RoomDefinition> rooms = realm.Rooms;
        for (int index = 0; index < rooms.Count; index++)
        {
            if (inventory.IsRoomRestored(rooms[index]))
            {
                count++;
            }
        }

        return count;
    }

    private void RemoveDevSettingsModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DevSettingsModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RefreshDevSettingsUi(bool returnToMap)
    {
        ReturnFromDevSettings(returnToMap);
        BuildDevSettingsUi(returnToMap, devSettingsReturnToDen);
    }

    private void ReturnFromDevSettings(bool returnToMap)
    {
        if (returnToMap)
        {
            BuildWorldMapUi();
        }
        else if (devSettingsReturnToDen)
        {
            BuildPlayerDenUi();
        }
        else
        {
            BuildLobbyUi();
        }
    }

    private void ResetPrototypeSave()
    {
        ResetPrototypeSave(true);
    }

    private void ResetPrototypeSave(bool returnToMap)
    {
        pendingRewardPreview = null;
        rewardPreviewShown = false;
        roundFlow.StopRound();
        rewards.ResetRound();
        rewards.ResetSavedXp();
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        inventory.ResetToDefaults();
        ResetPrototypeFriendsState();
        prototypeRequestedCovenNames.Clear();
        SavePrototypeCovenDiscoveryState();
        ResetPrototypeTrailState();
        if (profileSettingsPersistence != null)
        {
            ApplyProfileSettings(profileSettingsPersistence.ResetProfileIdentityPreservingPreferences(profileSettingsState));
        }
        else
        {
            bool soundEnabled = prototypeSoundEnabled;
            bool notificationsEnabled = prototypeNotificationsEnabled;
            ApplyProfileSettings(ProfileSettingsState.CreateDefault());
            prototypeSoundEnabled = soundEnabled;
            prototypeNotificationsEnabled = notificationsEnabled;
            profileSettingsState.SoundEnabled = soundEnabled;
            profileSettingsState.NotificationsEnabled = notificationsEnabled;
        }
        ReturnFromDevSettings(returnToMap);
    }

    private void ResetRoomProgressKeepingInventory(bool returnToMap)
    {
        pendingRewardPreview = null;
        rewardPreviewShown = false;
        roundFlow.StopRound();
        rewards.ResetRound();
        RealmContentCatalog.SetActivePrototypeRoom(0, 0);
        inventory.ResetRoomProgressKeepingInventory();
        ReturnFromDevSettings(returnToMap);
    }

    private void UnlockNextRealmForTesting(bool returnToMap)
    {
        int nextRealmIndex = Mathf.Min(RealmContentCatalog.ActivePrototypeRealmIndex + 1, RealmContentCatalog.AllRealms.Count - 1);
        inventory.UnlockRealmForTesting(nextRealmIndex);
        ReturnFromDevSettings(returnToMap);
    }

    private void SetSunpetalMixedStateForTesting()
    {
        pendingRewardPreview = null;
        rewardPreviewShown = false;
        roundFlow.StopRound();
        rewards.ResetRound();
        RealmContentCatalog.SetActivePrototypeRoom(0, 1);
        inventory.SetSunpetalMixedStateForTesting();
        BuildRealmMapUi();
    }

    private void GrantPrototypeClairvoyance()
    {
        GrantPrototypeClairvoyance(false);
    }

    private void GrantPrototypeClairvoyance(bool returnToMap)
    {
        inventory.AddClairvoyanceMinutes(15);
        RefreshPowerUpDisplays();
        RefreshDevSettingsUi(returnToMap);
    }

    private void GrantPrototypeMana(int amount, bool returnToMap)
    {
        inventory.DevGrantMana(amount);
        RefreshDevSettingsUi(returnToMap);
    }

    private void GrantPrototypeCrystals(int amount, bool returnToMap)
    {
        inventory.DevGrantCrystals(amount);
        RefreshDevSettingsUi(returnToMap);
    }

    private void ResetDailySpinForTesting(bool returnToMap)
    {
        ResetDailySpinClaimForTesting();
        RefreshDevSettingsUi(returnToMap);
    }

    private void ResetDailySpinClaimForTesting()
    {
        inventory.DevResetDailySpinClaim();
        lastDailySpinSummary = "";
        lastDailySpinRewardIndex = -1;
        dailySpinAnimating = false;
        dailySpinAnimationSequence++;
        dailySpinWheelRotation = 0f;
    }

    // Beta QA page: targeted resets so testers can recover or replay daily and social systems without wiping progress.
    private void BuildBetaQaResetsUi(bool returnToMap, string status)
    {
        RemoveDevSettingsModal();
        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.1f, 0.04f, 0.16f, 0.98f), 780, 600, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.85f, 0.82f, 0.95f);
        CreateAnchoredText(panel.transform, "BETA QA RESETS", 32, FontStyle.Bold, gold, 700, 44, 0f, 254f);
        CreateAnchoredText(panel.transform, "Targeted resets for testers. These change this device's test save only; progress elsewhere is kept.", 16, FontStyle.Bold, muted, 700, 44, 0f, 208f);
        CreateAnchoredText(panel.transform, string.IsNullOrEmpty(status) ? "Choose a reset." : status, 18, FontStyle.Bold, Color.white, 700, 40, 0f, 160f);

        AddBetaQaButton(panel.transform, "Daily Bonus: Next Day", -180f, 96f, returnToMap, () =>
        {
            inventory.DevAdvanceDailyBonusDay();
            return "Daily Bonus is claimable again; claiming continues the streak.";
        });
        AddBetaQaButton(panel.transform, "Daily Bonus: Reset Streak", 180f, 96f, returnToMap, () =>
        {
            inventory.DevResetDailyBonusStreak();
            return "Daily Bonus streak reset to day 1.";
        });
        AddBetaQaButton(panel.transform, "Daily Spin: Spin Again", -180f, 36f, returnToMap, () =>
        {
            ResetDailySpinClaimForTesting();
            return "Daily Spin is available again.";
        });
        AddBetaQaButton(panel.transform, "Social Daily Limits", 180f, 36f, returnToMap, () =>
        {
            ResetSocialDailyLimitsForTesting();
            return "Friend mana, card gift and help-request counters reset for today.";
        });
        AddBetaQaButton(panel.transform, "Clear Inbox", -180f, -24f, returnToMap, () =>
        {
            coven.ClearPlayerInboxForTesting();
            return "Inbox emptied.";
        });
        AddBetaQaButton(panel.transform, "Reset Enchanted Trail", 180f, -24f, returnToMap, () =>
        {
            ResetPrototypeTrailState();
            return "Enchanted Trail claims cleared.";
        });
        AddBetaQaButton(panel.transform, "Fill Room Ingredients", -180f, -84f, returnToMap, () =>
        {
            inventory.DevFillActiveRoomIngredients();
            return $"{RealmContentCatalog.ActivePrototypeRoom.Name} ingredients filled to its potion requirement.";
        });

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), 0f, -252f);
        back.onClick.AddListener(() => BuildDevSettingsUi(returnToMap, devSettingsReturnToDen));
    }

    private void AddBetaQaButton(Transform parent, string label, float x, float y, bool returnToMap, System.Func<string> action)
    {
        Button button = CreateAnchoredButton(parent, label, 17, 320, 46, new Color(0.2f, 0.16f, 0.42f), x, y);
        button.onClick.AddListener(() => BuildBetaQaResetsUi(returnToMap, action()));
    }

    private void ResetSocialDailyLimitsForTesting()
    {
        friendsState.ResetDailyMana();
        prototypeCardGiftsSentToday = 0;
        prototypeCardGiftsReceivedToday = 0;
        inventory.DevResetSocialHelpRequests();
    }

    private void GrantPrototypePowerUpSet(bool returnToMap)
    {
        for (int index = 0; index < PlayerInventoryState.GameplayPowerUps.Length; index++)
        {
            inventory.AddInventoryReward(PlayerInventoryState.GameplayPowerUps[index].Name, 1);
        }

        RefreshDevSettingsUi(returnToMap);
    }
}

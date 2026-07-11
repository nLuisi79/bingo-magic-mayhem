# UGS-Ready Service Layer

Last updated: 2026-07-10

Status: first infrastructure implementation pass. This document does not lock gameplay, economy, rewards, progression, monetization, album, Coven, jackpot, or Aura/rank rules.

## Implemented Scope

The Unity project now has an SDK-free service layer under `Assets/Scripts/Infrastructure`:

- `GameInfrastructureServices`: local composition root and future UGS adapter seam.
- `GameInfrastructureRuntime`: process-wide initialize-once lifecycle owner.
- `JsonFileDurableStateStore`: versioned JSON snapshots, atomic replacement where supported, and last-known-good backup recovery.
- `JsonLinesActionJournal`: append-only JSON-lines action and status-transition records with monotonic local sequence numbers.
- `LocalIdentityFacade`: stable durable local guest identity.
- `IdentitySafetyDiagnostics`: identity/auth readiness policy that keeps local guest authority and reports sign-in/link/recovery blockers.
- `LocalAnalyticsFacade`: local-only safe event recording through the action journal.
- `AnalyticsSafetyDiagnostics`: analytics consent/upload policy that keeps live upload blocked and reports allowlisted versus local-only analytics rows.
- `PrototypeAnalyticsPayloadFactory`: reusable local analytics payload/schema helper for prototype event shaping.
- local-only feature analytics instrumentation for room enter, round start, bingo claim, round completion, round reward collect, room restore, album reward claim, social freebie redeem, social help request send, friend mana send/receive, daily bonus claim, daily spin claim, inbox reward/message actions, coven orb contribution, wild-card use, coven wish gifting, coven emporium purchases, and jackpot collect.
- `LocalRemoteConfigFacade`: typed reads from explicitly supplied local defaults.
- `RemoteConfigSafetyDiagnostics`: infrastructure-only Remote Config safety policy for live/cloud enablement flags.
- `ExportSafetyDiagnostics`: local-file-only diagnostics export/share policy that keeps support exports payload-free and external share flows blocked.
- `InfrastructureContracts`: service interfaces and transport-neutral models.
- `ProfileSettingsPersistence`: first narrow durable-state consumer with compatibility `PlayerPrefs` writes.
- `LocalStateMigrationRegistry`: explicit ordered state-schema upgrades with no skipped versions.
- `InfrastructureDiagnosticsFacade`: redacted snapshot/journal health capture and payload-free export.
- `UgsPreflightDiagnostics`: local package/adapter/cloud-readiness checks with live calls disabled by default.
- `UgsAdapterRuntimePolicy`: second-stage runtime gating for future Authentication/Analytics/Cloud Save adapters, separate from the compile define.
- `JournalPolicyDiagnostics`: read-only local journal policy counts for retain/export/future-upload staging, sensitive payload blocking, and unapproved source/type blocking.
- `DisabledProfileSettingsCloudSync` / `CloudProfileSyncDiagnostics`: profile/settings Cloud Save seam that declares future key `bmm.profile_settings.v2` and conflict policy `profile_cloud_conflict_policy_v0.1` while keeping upload/download/merge/remote overwrite blocked.

The approved UGS SDK packages are resolved into the project lockfile/cache, but no project environment, cloud endpoint, authentication call, analytics upload, or Remote Config fetch is connected in this pass.

Runtime use remains disabled until project-link, consent/privacy, Cloud Save conflict policy, and offline fallback checks complete. `BMM_UGS_ADAPTERS` remains absent by default.

## Persistence Layout

The default composition root writes beneath:

```text
Application.persistentDataPath/
  bmm_infrastructure/
    state/
      identity.snapshot.json
      identity.snapshot.json.bak
      profile_settings.snapshot.json
      profile_settings.snapshot.json.bak
    journal/
      actions.jsonl
    diagnostics/
      bmm_diagnostics_YYYYMMDD_HHMMSS.json
```

Additional feature snapshots should use separate file-safe state names. Snapshot envelopes carry a schema version and UTC save timestamp. Gameplay systems have not been migrated from `PlayerPrefs`; that must happen incrementally with explicit compatibility and reset behavior.

## First Consumer Migration

Prototype startup now initializes `GameInfrastructureRuntime` once before loading the existing game state. The profile/settings slice stores a local display name, stable avatar/frame/dauber ids, and Sound/Notifications preferences in `profile_settings.snapshot.json`.

During the compatibility window, the same values are written to namespaced `PlayerPrefs` keys. If the snapshot is missing, those compatibility keys are read and promoted into the durable snapshot. This is intentionally limited to non-economic identity and preference state.

Fresh New Player resets display name, avatar, frame, and dauber selections to their prototype defaults while preserving Sound, Notifications, and the stable local guest identity. It does not alter cosmetic unlock rules because those remain unresolved.

## Snapshot Migration Rules

- Every registered state declares its current schema version.
- Migrations advance exactly one version per registered step.
- Missing intermediate migrations fail safely rather than skipping data transforms.
- Snapshots newer than the running client are rejected and are not silently replaced by an older backup.
- Successfully migrated primary snapshots are immediately rewritten in the current envelope while retaining the previous version as backup.
- Older envelopes without an explicit state-schema field infer the version from the payload's `SchemaVersion`, defaulting to version 1 for legacy unversioned state.

## Beta Persistence Diagnostics

Prototype Settings includes a Persistence panel with:

- redacted local player id and identity provider;
- snapshot names, schema versions, health, byte size, and backup presence;
- journal record count, pending action-row count, byte size, and latest sequence;
- last observed recovery and migration;
- UGS preflight package/adapter/cloud-readiness state;
- identity safety state: policy `identity_safety_v0.1`, local guest provider, cloud-auth blocked state, account-link/recovery blockers, and Remote Config bypass blocking;
- analytics safety state: policy `analytics_safety_v0.1`, consent blocked, live upload blocked, allowlisted analytics rows across infrastructure and first-pass feature events, local-only blocked rows, and Remote Config bypass blocking;
- diagnostics export/share safety state: policy `diagnostics_export_safety_v0.1`, local-file export enabled, payload-free-only contract, external share blocked, clipboard blocked, and advisory-only Remote Config export flag status;
- Remote Config safety state: policy `infra_remote_config_safety_v0.1`, required infra key coverage, risky enable flags, unknown keys, and local-only runtime-change blocking;
- profile/settings Cloud Save sync state: live sync off, adapter compile gate, key `bmm.profile_settings.v2`, upload blocked, download blocked, local snapshots authoritative, automatic merge blocked, remote overwrite blocked, and gameplay sync blocked;
- journal sync-staging state: live uploads off, active upload eligible 0, future-upload candidates, sensitive/unapproved blocked rows, and status counts;
- journal retention/privacy state: policy `journal_retention_policy_v0.1`, retention/archive/compaction/delete gates blocked, planning candidate counts, and export-redaction counts;
- safe summary export.

The export contains operational counts only. It excludes full player ids, action ids, idempotency keys, journal payloads, message content, tokens, receipts, and credentials. Journal clearing, compaction, automatic retention, deletion, live upload, clipboard forwarding, and in-app external sharing are not available until their policies are approved.

## Journal Contract

Each appended record carries:

- action id and optional idempotency key;
- local guest id or future UGS player id;
- monotonic local sequence;
- UTC timestamp;
- source and action type;
- structured JSON payload;
- record kind: action or status transition;
- status: pending, applied local, synced, rejected, or compensated.

Status changes append a new transition record. Existing journal rows are never rewritten. Payloads must contain safe structured context and must not include message text, credentials, receipts, access tokens, or other sensitive data.

## Intended UGS Adapters

Future approved adapters should preserve the current contracts:

| Current facade | Preferred future adapter |
|---|---|
| `IIdentityFacade` | Unity Authentication with anonymous sign-in and approved account linking |
| `ILocalDurableStateStore` | Remains local; Cloud Save adds reconciliation above it |
| `IActionJournal` | Remains local; approved records become sync/upload inputs |
| `IAnalyticsFacade` | Unity Analytics after privacy/consent policy is implemented |
| `IRemoteConfigFacade` | UGS Remote Config with local defaults and last-known-good cache |
| `IProfileSettingsCloudSync` | UGS Cloud Save for profile/settings only, after conflict/offline policy approval |

UGS Economy, Cloud Code, broader Cloud Save reconciliation, IAP, Leaderboards, and social backends are intentionally outside this pass.

## Adoption Rules

- Do not replace all `PlayerPrefs` use in one migration.
- Write and verify a local snapshot before or alongside meaningful migrated state changes.
- Journal sensitive/recoverable actions before applying them locally.
- Use stable idempotency keys for claims, grants, purchases, gifts, and redemptions.
- Preserve explicit claim semantics; Inbox items must not auto-grant.
- Keep unresolved tuning in labeled local config defaults rather than hard-coded as final.
- Do not upload analytics or journal records before consent, retention, and safe-payload rules are approved.
- Do not treat the local journal as server authority.
- Treat the current journal policy as diagnostics-only. It may identify future upload candidates, but active upload eligibility remains 0 until a live adapter, consent, retention, and upload allowlist are approved together.

## Verification

Edit-mode tests cover:

- durable snapshot round-trip and backup recovery;
- append-only status transitions and sequence continuity after restart;
- stable local guest identity;
- identity safety blockers for cloud sign-in, account linking, recovery, and Remote Config bypass;
- typed local config parsing and fallbacks.
- profile/settings compatibility migration and cosmetic-only reset behavior;
- recovery-event journaling.
- ordered schema migration and newer-client rejection;
- redacted diagnostics export with payload/identity exclusion.
- profile-settings schema 1 to 2 migration for local display name;
- stable cosmetic ids and logical asset keys with placeholder-safe sprite resolution.
- journal policy staging for safe future-upload candidates, sensitive payload blocking, and live uploads disabled.
- disabled profile/settings Cloud Save sync status and blocked upload/download behavior.
- conflict/offline policy gates for timestamp authority, merge/overwrite behavior, stale remote handling, offline retry/idempotency, and gameplay/economy isolation.
- Remote Config safety defaults, risky enable-flag blocking, unknown key visibility, and diagnostics capture.
- analytics safety defaults, first-pass feature-event allowlisting, reusable payload shaping, local event catalog coverage, consent/upload blocking, and Remote Config bypass blocking.
- journal retention/privacy defaults, blocked retention/archive/compaction/delete controls, and export-redaction planning counts.
- diagnostics export/share safety defaults, external-share blocking, and advisory-only Remote Config export disable handling.

Unity EditMode `InfrastructureServiceTests` last passed 38/38 on 2026-07-10 after the analytics helper/schema consolidation pass. This final local-only analytics coverage pass adds two EditMode tests for the next Unity Test Runner pass.

The current Unity solution build succeeds with 0 errors. Gameplay rules and package dependencies are unchanged; the prototype startup/profile shell now consumes the infrastructure layer.

## Next Narrow Infrastructure Passes

1. Run the full edit-mode suite through Unity Test Runner when the project is not held by another Unity process.
2. Decide whether `infra_diagnostics_export_enabled` should stay advisory-only or eventually become authoritative for Beta support builds after support/privacy workflow approval.
3. Product-approve analytics consent/privacy and upload policy before enabling live analytics.
4. Product-approve Unity Authentication identity/link/recovery policy before enabling cloud sign-in.
5. Product-approve the Cloud Save conflict/offline policy for profile/settings before enabling upload/download.
6. Product-approve journal retention, archive/compaction, privacy, and upload allowlists before enabling any destructive journal controls.
7. Decide whether diagnostics exports need an approved in-app share flow for external Beta; current export remains local-file-only.
8. Decide whether profile display-name editing belongs in the next Beta slice; it remains a placeholder now.
9. Add UGS Authentication/Cloud Save/Remote Config/Analytics adapters only after package installation and project connection are explicitly approved.

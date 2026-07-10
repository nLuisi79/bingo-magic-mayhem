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
- `LocalAnalyticsFacade`: local-only safe event recording through the action journal.
- `LocalRemoteConfigFacade`: typed reads from explicitly supplied local defaults.
- `InfrastructureContracts`: service interfaces and transport-neutral models.
- `ProfileSettingsPersistence`: first narrow durable-state consumer with compatibility `PlayerPrefs` writes.
- `LocalStateMigrationRegistry`: explicit ordered state-schema upgrades with no skipped versions.
- `InfrastructureDiagnosticsFacade`: redacted snapshot/journal health capture and payload-free export.
- `UgsPreflightDiagnostics`: local package/adapter/cloud-readiness checks with live calls disabled by default.

The approved UGS SDK packages are resolved into the project lockfile/cache, but no project environment, cloud endpoint, authentication call, analytics upload, or Remote Config fetch is connected in this pass.

Runtime use remains disabled until project-link, consent, Cloud Save conflict policy, and offline fallback checks complete. `BMM_UGS_ADAPTERS` remains absent by default.

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
- safe summary export.

The export contains operational counts only. It excludes full player ids, action ids, idempotency keys, journal payloads, message content, tokens, receipts, and credentials. Journal clearing, compaction, automatic retention, and upload are not available until their policies are approved.

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

UGS Economy, Cloud Code, Cloud Save reconciliation, IAP, Leaderboards, and social backends are intentionally outside this pass.

## Adoption Rules

- Do not replace all `PlayerPrefs` use in one migration.
- Write and verify a local snapshot before or alongside meaningful migrated state changes.
- Journal sensitive/recoverable actions before applying them locally.
- Use stable idempotency keys for claims, grants, purchases, gifts, and redemptions.
- Preserve explicit claim semantics; Inbox items must not auto-grant.
- Keep unresolved tuning in labeled local config defaults rather than hard-coded as final.
- Do not upload analytics or journal records before consent, retention, and safe-payload rules are approved.
- Do not treat the local journal as server authority.

## Verification

Edit-mode tests cover:

- durable snapshot round-trip and backup recovery;
- append-only status transitions and sequence continuity after restart;
- stable local guest identity;
- typed local config parsing and fallbacks.
- profile/settings compatibility migration and cosmetic-only reset behavior;
- recovery-event journaling.
- ordered schema migration and newer-client rejection;
- redacted diagnostics export with payload/identity exclusion.
- profile-settings schema 1 to 2 migration for local display name;
- stable cosmetic ids and logical asset keys with placeholder-safe sprite resolution.

The current Unity solution build succeeds with 0 errors. Gameplay rules and package dependencies are unchanged; the prototype startup/profile shell now consumes the infrastructure layer.

## Next Narrow Infrastructure Passes

1. Run the full edit-mode suite through Unity Test Runner when the project is not held by another Unity process.
2. Define journal retention, archive/compaction, privacy, and upload allowlists.
3. Decide whether diagnostics exports need an in-app share flow for external Beta; current export is local only.
4. Decide whether profile display-name editing belongs in the next Beta slice; it remains a placeholder now.
5. Add UGS Authentication/Cloud Save/Remote Config/Analytics adapters only after package installation and project connection are explicitly approved.

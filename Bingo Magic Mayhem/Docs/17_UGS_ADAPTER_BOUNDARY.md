# UGS Adapter Boundary

Last updated: 2026-07-10

## Package State

The approved package entries are present in `Packages/manifest.json`:

- `com.unity.services.core` 1.18.0
- `com.unity.services.authentication` 3.7.3
- `com.unity.services.cloudsave` 3.4.1
- `com.unity.remote-config` 4.2.5
- `com.unity.services.analytics` 6.3.0

Unity has resolved these entries into `packages-lock.json` and `Library/PackageCache`. No production environment is linked and no runtime UGS calls are active.

## Adapter Gate

`Assets/Scripts/Infrastructure/UgsAdapterBoundary.cs` contains the future live adapters behind the `BMM_UGS_ADAPTERS` scripting define. The define is intentionally absent by default.

It now also contains a second runtime-gate scaffold for future Authentication, Analytics, and Cloud Save calls. Even if the compile define is enabled later, live service calls should remain blocked unless explicit runtime approvals are supplied for project link, environment, services initialization, consent, and per-service enablement.

When enabled after package resolution and environment review, the adapters provide:

- Unity Services Core initialization plus anonymous Authentication;
- Analytics event/flush calls;
- Remote Config fetch and typed reads;
- Cloud Save profile/settings read/write only.

`GameInfrastructureServices.CreateLocal()` does not construct these adapters. Local snapshot, journal, identity fallback, and local config remain the default path until an explicit composition change is approved.

`GameInfrastructureServices.CreateConfigured(...)` now accepts desired provider preferences for identity, analytics, and profile/settings cloud sync, but the current build still resolves back to local-first providers unless UGS compile/runtime gates are satisfied. This gives the project a safer place to stage future composition changes without silently changing the default prototype path.

The local diagnostics panel now includes a UGS preflight summary, identity safety summary, analytics safety summary, Remote Config safety summary, profile/settings Cloud Save sync status section, composition desired-versus-active provider summary, journal retention/privacy summary, and diagnostics export/share safety summary. They are informational only: package resolution is marked pass, identity safety policy is labeled `identity_safety_v0.1`, analytics safety policy is labeled `analytics_safety_v0.1`, diagnostics export/share policy is labeled `diagnostics_export_safety_v0.1`, Remote Config safety policy is labeled `infra_remote_config_safety_v0.1`, the future profile/settings key is `bmm.profile_settings.v2`, conflict policy is labeled `profile_cloud_conflict_policy_v0.1`, and project environment link, consent/privacy, Cloud Save conflict policy, timestamp authority, merge/overwrite behavior, offline retry/idempotency, and any external diagnostics share flow remain blocked before live adapter enablement.

The `IProfileSettingsCloudSync` seam is deliberately disabled in local composition. Its upload and download methods return blocked/no-op results until an approved composition root replaces it and the enablement checks below pass. Local snapshots remain authoritative; automatic merge, remote overwrite, and gameplay/economy sync are not permitted by the scaffold.

Remote Config safety keys are currently diagnostics-only. Values such as `infra_ugs_adapters_enabled`, `infra_cloud_profile_sync_enabled`, and `infra_journal_upload_enabled` may be reported as risky if true, but they cannot enable runtime behavior without an approved code composition change. `infra_diagnostics_export_enabled` is currently advisory-only as well; it does not silently remove the local support export path.

Identity is also diagnostics-only in the current composition. The local guest path remains authoritative; cloud sign-in, account linking, recovery, and any Remote Config-driven auth upgrade remain blocked until explicitly approved.

The new runtime policy scaffold is not wired into the local composition root yet. Its purpose is to make future adapter composition safer by requiring a second explicit approval layer beyond the compile define alone.

## Required Enablement Checks

Before enabling `BMM_UGS_ADAPTERS`:

1. Confirm the five packages resolve and the lockfile is updated by Unity.
2. Link the correct Unity Dashboard project and use the `development` environment.
3. Confirm anonymous Authentication behavior and account recovery expectations.
4. Add Analytics event schemas and consent handling; do not call live collection before consent.
5. Confirm Cloud Save conflict/write-lock strategy for profile/settings.
6. Confirm stale remote data, multi-device merge/replace rules, and last-known-good local recovery behavior.
7. Run local-first fallback tests with network unavailable.
8. Confirm Remote Config live fetch cannot enable UGS adapters, Cloud Save sync, or journal upload without code approval.
9. Keep Economy, Cloud Code, IAP, Leaderboards, and all gameplay/economy sync disabled.

Unity’s current documentation requires Services Core initialization before Authentication/Cloud Save use and requires consent before Analytics collection. Remote Config values are not secret and must not hold credentials or sensitive data.

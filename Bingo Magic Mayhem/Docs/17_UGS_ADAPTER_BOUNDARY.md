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

When enabled after package resolution and environment review, the adapters provide:

- Unity Services Core initialization plus anonymous Authentication;
- Analytics event/flush calls;
- Remote Config fetch and typed reads;
- Cloud Save profile/settings read/write only.

`GameInfrastructureServices.CreateLocal()` does not construct these adapters. Local snapshot, journal, identity fallback, and local config remain the default path until an explicit composition change is approved.

The local diagnostics panel now includes a UGS preflight summary, identity safety summary, Remote Config safety summary, and profile/settings Cloud Save sync status section. They are informational only: package resolution is marked pass, identity safety policy is labeled `identity_safety_v0.1`, Remote Config safety policy is labeled `infra_remote_config_safety_v0.1`, the future profile/settings key is `bmm.profile_settings.v2`, conflict policy is labeled `profile_cloud_conflict_policy_v0.1`, and project environment link, consent/privacy, Cloud Save conflict policy, timestamp authority, merge/overwrite behavior, and offline retry/idempotency remain blocked before live adapter enablement.

The `IProfileSettingsCloudSync` seam is deliberately disabled in local composition. Its upload and download methods return blocked/no-op results until an approved composition root replaces it and the enablement checks below pass. Local snapshots remain authoritative; automatic merge, remote overwrite, and gameplay/economy sync are not permitted by the scaffold.

Remote Config safety keys are currently diagnostics-only. Values such as `infra_ugs_adapters_enabled`, `infra_cloud_profile_sync_enabled`, and `infra_journal_upload_enabled` may be reported as risky if true, but they cannot enable runtime behavior without an approved code composition change.

Identity is also diagnostics-only in the current composition. The local guest path remains authoritative; cloud sign-in, account linking, recovery, and any Remote Config-driven auth upgrade remain blocked until explicitly approved.

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

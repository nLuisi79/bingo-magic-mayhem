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

The local diagnostics panel now includes a UGS preflight summary and a profile/settings Cloud Save sync status section. They are informational only: package resolution is marked pass, the future profile/settings key is `bmm.profile_settings.v2`, and project environment link, consent/privacy, Cloud Save conflict policy, and offline fallback remain blocked before live adapter enablement.

The `IProfileSettingsCloudSync` seam is deliberately disabled in local composition. Its upload and download methods return blocked/no-op results until an approved composition root replaces it and the enablement checks below pass.

## Required Enablement Checks

Before enabling `BMM_UGS_ADAPTERS`:

1. Confirm the five packages resolve and the lockfile is updated by Unity.
2. Link the correct Unity Dashboard project and use the `development` environment.
3. Confirm anonymous Authentication behavior and account recovery expectations.
4. Add Analytics event schemas and consent handling; do not call live collection before consent.
5. Confirm Cloud Save conflict/write-lock strategy for profile/settings.
6. Confirm stale remote data, multi-device merge/replace rules, and last-known-good local recovery behavior.
7. Run local-first fallback tests with network unavailable.
8. Keep Economy, Cloud Code, IAP, Leaderboards, and all gameplay/economy sync disabled.

Unity’s current documentation requires Services Core initialization before Authentication/Cloud Save use and requires consent before Analytics collection. Remote Config values are not secret and must not hold credentials or sensitive data.

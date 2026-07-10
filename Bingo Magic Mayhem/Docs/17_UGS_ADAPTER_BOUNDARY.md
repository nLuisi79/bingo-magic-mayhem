# UGS Adapter Boundary

Last updated: 2026-07-09

## Package State

The approved package entries are present in `Packages/manifest.json`:

- `com.unity.services.core` 1.18.0
- `com.unity.services.authentication` 3.7.3
- `com.unity.services.cloudsave` 3.4.1
- `com.unity.remote-config` 4.2.5
- `com.unity.services.analytics` 6.3.0

Unity’s current open editor has not yet resolved these entries into `packages-lock.json`/`Library/PackageCache`. No production environment is linked and no runtime UGS calls are active.

## Adapter Gate

`Assets/Scripts/Infrastructure/UgsAdapterBoundary.cs` contains the future live adapters behind the `BMM_UGS_ADAPTERS` scripting define. The define is intentionally absent by default.

When enabled after package resolution and environment review, the adapters provide:

- Unity Services Core initialization plus anonymous Authentication;
- Analytics event/flush calls;
- Remote Config fetch and typed reads;
- Cloud Save profile/settings read/write only.

`GameInfrastructureServices.CreateLocal()` does not construct these adapters. Local snapshot, journal, identity fallback, and local config remain the default path until an explicit composition change is approved.

## Required Enablement Checks

Before enabling `BMM_UGS_ADAPTERS`:

1. Confirm the five packages resolve and the lockfile is updated by Unity.
2. Link the correct Unity Dashboard project and use the `development` environment.
3. Confirm anonymous Authentication behavior and account recovery expectations.
4. Add Analytics event schemas and consent handling; do not call live collection before consent.
5. Confirm Cloud Save conflict/write-lock strategy for profile/settings.
6. Run local-first fallback tests with network unavailable.
7. Keep Economy, Cloud Code, IAP, Leaderboards, and all gameplay/economy sync disabled.

Unity’s current documentation requires Services Core initialization before Authentication/Cloud Save use and requires consent before Analytics collection. Remote Config values are not secret and must not hold credentials or sensitive data.

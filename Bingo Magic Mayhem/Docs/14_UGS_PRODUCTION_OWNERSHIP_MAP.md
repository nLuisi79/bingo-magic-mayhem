# UGS Production Ownership Map

Last updated: 2026-07-09

Status: production-planning map. This is not a gameplay, economy, reward, rarity, monetization, progression, or Aura/rank lock document.

Read with:

- `LOCKED_DECISIONS.md`
- `09_OPEN_DECISIONS.md`
- `10_VERIFIED_PROJECT_STATE.md`
- `11_RANK_REWARDS_V0.1.md`
- `12_BETA_1_PRODUCTION_READINESS.md`
- `13_BETA_QA_CHECKLIST.md`

## Purpose

This map assigns each major Bingo Magic Mayhem system to the preferred Unity Gaming Services owner for the Beta backend path. UGS is the implementation direction unless a concrete blocker appears. Service facades should still keep the game from being tightly coupled to SDK calls in gameplay/UI code.

## Guiding Principle

Player progress should be local-first, cloud-backed, event-journaled, and server-validated for sensitive actions.

The game should not lose track of where a player was because Wi-Fi dropped, the app closed, or the device crashed.

Production state should use three layers:

1. Local durable snapshot: the latest recoverable player state.
2. Local action journal: append-only records of important player actions.
3. Cloud sync/server validation: authoritative reconciliation for economy-sensitive or social actions.

## Current Implementation Status

The first SDK-free infrastructure pass is implemented under `Assets/Scripts/Infrastructure`:

- local durable snapshots with schema envelopes and last-known-good backup recovery;
- append-only JSON-lines action journal with monotonic sequence and status-transition records;
- stable local guest identity facade;
- local-only analytics facade backed by the journal;
- typed local Remote Config facade with no embedded gameplay/economy defaults;
- one composition root designed for future UGS adapters.

Existing gameplay `PlayerPrefs` state is unchanged. No UGS packages or live services are connected. See `15_UGS_READY_SERVICE_LAYER.md` for the implementation contract and adoption rules.

The first consumer migration is now active for profile cosmetics and Sound/Notifications. It uses stable cosmetic ids, a versioned local snapshot, compatibility `PlayerPrefs` writes, and journaled change/recovery events. No inventory, currency, reward, progress, social, or economy state moved in this pass.

Snapshot storage now supports explicitly registered, ordered one-version-at-a-time migrations and refuses snapshots newer than the current client. Prototype Settings exposes redacted persistence diagnostics and payload-free export. Journal retention, compaction, clearing, and upload allowlists remain open and inactive.

Profile settings schema 2 adds a local display name and stable cosmetic catalog ids. Display-name validation is Beta/test-only; production uniqueness/moderation requires approved backend authority. Cosmetic logical keys currently resolve through a local Resources adapter and are designed to move to Addressables without changing saved ids.

The five approved UGS packages are staged in the manifest, but no live UGS adapter is enabled. The gated boundary is documented in `17_UGS_ADAPTER_BOUNDARY.md`; Economy, Cloud Code, IAP, Leaderboards, and gameplay/economy sync remain out of scope.

## UGS Service Roles

These roles describe the preferred UGS mapping for Beta. Firebase, PlayFab, custom backend, or a hybrid remain fallback options if UGS hits a concrete blocker, but they are no longer equal first-pass targets.

| Service | Project role | Notes |
|---|---|---|
| Unity Authentication | Player identity | Start with anonymous sign-in for Beta, then add Apple/Google/Facebook linking when login scope is approved. |
| Cloud Save | Player state and synced snapshots | Owns cross-device player data, progress, preferences, claim states, and non-critical snapshots. |
| Remote Config | Tunable values and feature flags | Owns reward tables, limits, draft rank tables, event flags, and Beta/test config values. |
| Economy | Currencies, item definitions, virtual purchases | Candidate owner for Mana, Crystals, power-up inventory, entitlements, and virtual store definitions. Needs careful migration because current values are not final. |
| Cloud Code | Server-side sensitive actions | Owns validation for reward claims, freebies, gifts, purchases, daily claims, and anti-repeat logic. |
| Analytics | Beta telemetry and behavior tracking | Owns event collection for QA, retention, progression, reward claims, crashes/recovery, and feature use. |
| Leaderboards | Score/rank storage for leaderboard views | Owns Friends/Team/Global standings after scoring rules are approved. |
| Unity IAP | Real-money purchase flow | Owns platform purchase transactions, receipt handling, restore purchases, and IAP product catalog integration after monetization approval. |

## Adjacent Unity Production Systems

| System | Project role | Notes |
|---|---|---|
| Unity Addressables | Content and asset delivery candidate | Strong candidate for seasonal rooms, card/ingredient art, event assets, room themes, UI skins, and large UI asset groups. Not yet a locked Beta 1 requirement. |
| Sprite Atlases | Runtime art/performance organization | Strong candidate for repeated UI, card, ingredient, and reward icons. Atlas grouping and memory targets remain open. |
| UGUI / UI Toolkit | UI implementation strategy | Current prototype is UGUI-style runtime UI. Keep UGUI where practical; evaluate UI Toolkit only where it clearly helps. |
| Lobby/Relay/Netcode or Photon | Real-time networking candidates | Do not choose until real-time shared rooms or synchronized calls become explicit scope. Asynchronous social systems may not need real-time multiplayer. |

## Local-First State Model

### Local Durable Snapshot

The client should write a recoverable local snapshot before or alongside meaningful state changes. This replaces direct `PlayerPrefs` reliance over time.

Snapshot candidates:

- Player profile id, display name, avatar/frame/dauber selections.
- Current map context: last realm, room, and screen.
- Current room progress and restoration state.
- Current inventory snapshot.
- Current album/card snapshot.
- Current ingredient and potion progress.
- Current daily claim states.
- Current Inbox state.
- Current Friends/Coven placeholder state where locally testable.
- Pending rewards not yet claimed.
- Last successful cloud sync version.

### Local Action Journal

The client should append an action record before applying sensitive or recoverable changes. The journal makes crash recovery and offline sync explainable.

Every action should have:

- `actionId`: unique id.
- `playerId`: UGS player id when available, local guest id before auth.
- `sequence`: local monotonic sequence.
- `createdAtUtc`: client timestamp.
- `source`: feature/system name.
- `type`: action type.
- `payload`: safe structured details.
- `status`: pending, applied_local, synced, rejected, compensated.
- `idempotencyKey`: stable key for claims/grants.

### Cloud Reconciliation

Cloud sync should upload unsynced journal entries and fetch the latest accepted snapshot. Sensitive entries should be validated by Cloud Code before they become authoritative.

Conflict rule direction:

- Never blindly overwrite newer local progress with older cloud data.
- Use local sequence and cloud revision/version metadata.
- Use idempotency keys for all reward and purchase claims.
- Keep a last-known-good snapshot for recovery.
- If cloud rejects an action, record the rejection and show a recoverable user-safe state.

## System Ownership Matrix

| System | Local snapshot | Action journal | Cloud Save | Remote Config | Economy | Cloud Code | Analytics | IAP | Notes |
|---|---|---|---|---|---|---|---|---|---|
| Account/Profile | Yes | Profile edits | Yes | Feature flags | Optional cosmetics later | Optional validation | Yes | No | Auth owns player id; Cloud Save owns profile/preferences. |
| Settings | Yes | Setting changed | Yes | Defaults/experiments | No | No | Optional | No | Safe first Cloud Save candidate. |
| Avatar/Frame/Dauber | Yes | Selection changed/unlock earned | Yes | Unlock tables | Optional inventory items | Unlock validation later | Yes | Optional cosmetics later | Keep gameplay-neutral. |
| Level/XP | Yes | XP earned, level up | Yes | XP curves draft | Optional | Server validation later | Yes | No | Rank remains Aura-derived, not Level-derived. |
| Aura/Rank | Yes | Aura source event, rank changed | Yes | Aura formula/table after approval | No | Yes, once live | Yes | No direct rank purchase | Purchase contribution must remain small/capped. |
| Mana/Crystals | Yes | Earn/spend/claim | Yes initially, Economy candidate | Reward tables | Yes candidate | Yes for claims/spends | Yes | IAP may grant via validated flow | Economy values are not final. |
| Power-Ups/Sigils | Yes | Earn/spend/use | Yes | Drop/reward tables | Yes candidate | Yes for grants/spends | Yes | Optional bundles later | Gameplay power-up behavior remains locked. |
| Clairvoyance | Yes | Time granted/activated/expired | Yes | Duration tables | Yes candidate | Yes for grants | Yes | Optional later | Must retain remaining time safely. |
| Realms/Rooms | Yes | Enter room, restore, unlock | Yes | Room config/reward tables | No | Restore/unlock validation later | Yes | No | Fresh reset begins Realm 1 Room 1 only. |
| Active Round | Yes local only | Round start/daub/bingo/complete | Not every daub by default | Ball/reward config | No | Later for anti-cheat if needed | Yes summary events | No | Primary goal is crash recovery, not cloud replay of every daub. |
| Room Rewards | Yes pending claims | Reward earned/claimed | Yes | Reward tables | Yes candidate | Yes for final grants | Yes | No | Claims must remain explicit. |
| Jackpot Wheelspin | Yes pending spins/results | Spin earned/spun/collected | Yes | Wheel segments/odds | Yes candidate for grants | Yes before live economy | Yes | No | Post-round only; rewards stack before collect. |
| Ingredients/Potions | Yes | Ingredient earned/gifted/used, potion completed | Yes | Ingredient/reward tables | Optional items later | Yes for gifts/claims later | Yes | No | Ingredient awards from gameplay remain core. |
| Grimoire/Book of Shadows | Yes | Card acquired/viewed/claimed | Yes | Album config/rewards | Optional item definitions | Yes for claims/gifts later | Yes | Book of Shadows entitlement later | Avoid final duplicate economy until approved. |
| Inbox | Yes | Item queued/read/claimed/cleared | Yes | Expiry/category rules | Optional reward payloads | Yes for claim validation | Yes | Purchase receipts later | Inbox gifts do not auto-grant before claim. |
| Daily Bonus | Yes | Claimed/streak save | Yes | Reward ladder/streak settings | Yes candidate | Yes for live claim validation | Yes | Optional streak save crystals later | Direct claim, not Inbox. |
| Daily Spin | Yes | Spin claimed/result granted | Yes | Wheel table/odds | Yes candidate | Yes for live claim validation | Yes | Optional ticket/ad later | Separate from jackpot wheelspin. |
| Freebies | Yes redeemed codes | Link opened/redeemed/rejected | Yes | Reward pools/expiry | Yes candidate | Yes required for anti-repeat/expiry | Yes | No | Direct redemption, not Inbox. Expiry hidden from URL. |
| Friends | Yes local cache | Add/remove/block/report/message/mana tap | Yes for social state | Limits/feature flags | Optional mana grant later | Yes for live sends/reports | Yes | No | Messaging likely Inbox-backed; moderation needs backend. |
| Coven | Yes local cache | Join/request/gift/contribute/chat action | Yes for player-facing cache | Limits/settings | Optional orbs/items | Yes for gifts/contribution/join | Yes | No | Coven state may need backend data beyond Cloud Save player data. |
| Card Gifting/Trading | Yes pending actions | Gift/trade queued/sent/claimed | Yes | Limits/eligibility | Yes candidate | Yes required before live | Yes | No | Library/Grimoire owns card social actions. |
| Leaderboards | Local cache | Score submitted/viewed | Optional cache | Season/scoring flags | No | Yes if score validation needed | Yes | No | UGS Leaderboards may own standings after score formula approval if UGS is selected. |
| Enchanted Trail | Yes | Points earned/reward claimed | Yes | Path/tasks/rewards | Yes candidate | Yes for claims later | Yes | Premium lane later only if approved | Current free path is placeholder. |
| Oracle Alley | Yes event state | Reading started/card selected/reward claimed | Yes | Cadence/cost/reward/odds | Yes candidate | Yes required before rewards live | Yes | Optional later | Protect scarcity; no reward economy until approved. |
| Mayhem Market | Yes display/receipt state | Purchase started/completed/restored | Yes entitlements | Product/offer flags | Yes virtual products | Yes for fulfillment | Yes | Yes | Current shell disabled; no live store yet. |
| Reports/Moderation | Yes local action record | Block/report/message events | Yes local state | Feature flags | No | Yes required | Yes | No | Avoid storing unsafe message content in analytics. |

## First Safe Implementation Order

1. UGS bootstrap wrapper
   - Initialize Unity Services.
   - Sign in anonymously when available.
   - Create local guest id fallback.
   - Expose environment label: local, prototype, beta.
   - Keep the wrapper replaceable even while implementing toward UGS.

2. Local durable save facade
   - Replace direct future `PlayerPrefs` calls with a local save abstraction.
   - Do not migrate all systems at once.
   - Preserve existing state behavior while adding better structure.

3. Local action journal
   - Append action records for screen open, claim, grant, reset, and recovery events.
   - Store locally first.
   - Add export/debug view later for testing.

4. Analytics event facade
   - Create no-op/local logger first.
   - Later connect to Unity Analytics after consent/privacy setup.

5. Remote Config facade
   - Create typed config objects with local defaults.
   - Later fetch UGS Remote Config.
   - Keep all draft values labeled Beta/test.

6. Cloud Save sync prototype
   - Start with profile/settings/cosmetic selections only.
   - Add progress/inventory only after conflict policy is implemented.

7. Cloud Code claim prototypes
   - Start with one non-final validation path such as freebie redemption or Daily Bonus claim.
   - Do not move jackpot, card gifting, or purchase fulfillment first.

8. Content delivery planning
   - Define stable catalog ids and asset keys for cards, ingredients, rooms, and UI skins.
   - Evaluate Addressables for seasonal/downloadable asset groups.
   - Keep temporary Beta art replaceable without changing gameplay data.

9. UI performance planning
   - Isolate frequently changing bingo-card canvases from static UI.
   - Define Sprite Atlas and 9-slice standards for repeated UI.
   - Keep animations, particles, sound, and visual effects presentation-only.

10. Real-time authority greybox only if needed
   - If shared rooms or synchronized calls enter scope, test server-owned calls and bingo validation in plain UI before visual polish.
   - Do not choose Netcode, Relay, Lobby, or Photon until that product scope is explicit.

## Beta 1 Recommended Scope

For first external Beta, the safest minimum UGS scope is:

- Authentication: anonymous sign-in plus local fallback.
- Local durable snapshot: required.
- Local action journal: required.
- Analytics facade: local/no-op first, UGS Analytics after consent.
- Remote Config facade: local defaults first, UGS fetch when project settings are ready.
- Cloud Save: profile/settings/cosmetic selections first.
- Addressables: evaluate for asset groups, but do not require for Beta 1 unless first-install size or seasonal content demands it.

Defer until rules and validation are clearer:

- Economy as authoritative owner for all currencies/items.
- Cloud Code validation for every reward claim.
- Live IAP products.
- Live leaderboards.
- Live Friends/Coven networking.
- Live card trading marketplace.
- Real-time multiplayer/shared rooms unless explicitly approved.

## Open Decisions Before Deep UGS Wiring

- Local-only Beta vs cloud-backed external Beta.
- Concrete blocker criteria that would force a move away from UGS toward Firebase, PlayFab, custom backend, or hybrid.
- Whether UGS Economy should own all currencies/items or only store-backed/critical balances.
- Which player progress snapshot fields must sync in Beta 1.
- Which action journal events are uploaded, retained locally, or both.
- How long local action logs are retained.
- How crash recovery is surfaced to the player.
- Whether active round state should be restored exactly or resumed from a safe checkpoint.
- Which actions are server-authoritative on day one.
- Privacy/consent timing for analytics and social messaging.
- Exact product catalog for Mayhem Market.
- Whether Leaderboards use UGS Leaderboards directly or Cloud Code-mediated score submission.
- Whether Addressables are required for Beta 1 or remain production hardening.
- Final asset key, catalog id, bundle grouping, and content-versioning standards.
- Final UGUI/UI Toolkit split, canvas isolation rules, Sprite Atlas grouping, 9-slice standards, and mobile performance targets.
- Whether real-time shared rooms/synchronized number calls are product scope.

## Official UGS Reference Links

- Unity Authentication: https://docs.unity.com/en-us/authentication
- Cloud Save: https://docs.unity.com/en-us/cloud-save
- Remote Config: https://docs.unity.com/en-us/remote-config
- Economy: https://docs.unity.com/en-us/economy
- Cloud Code: https://docs.unity.com/en-us/cloud-code
- Analytics: https://docs.unity.com/en-us/analytics
- Leaderboards: https://docs.unity.com/en-us/leaderboards
- Unity IAP: https://docs.unity.com/en-us/iap

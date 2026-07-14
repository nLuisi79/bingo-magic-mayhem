# Beta 1 Production Readiness

Last updated: 2026-07-10

Status: working planning checklist. This is not a gameplay/economy lock document. Use it to decide what must become real enough for Beta 1 and what can remain a labeled placeholder.

Read with:

- `LOCKED_DECISIONS.md`
- `09_OPEN_DECISIONS.md`
- `10_VERIFIED_PROJECT_STATE.md`
- `11_RANK_REWARDS_V0.1.md`
- `13_BETA_QA_CHECKLIST.md`
- `14_UGS_PRODUCTION_OWNERSHIP_MAP.md`
- `IMPLEMENTATION_GAPS.md`

## Goal

Move from feature-shape prototype toward a production-minded Beta 1 without stopping useful feature coverage.

The target rhythm is two-track:

1. Finish visible Beta 1 feature coverage at a narrow, testable level.
2. Start replacing fragile prototype shortcuts with production-ready architecture, config, save, account, analytics, QA, and backend boundaries.

## Beta 1 Intent

Beta 1 should let testers understand the core game promise:

- Play magical bingo rooms.
- Progress through the first realm.
- Restore rooms/realm areas.
- Collect ingredients and album cards.
- Use the Player's Den as the account/inventory hub.
- Interact with daily retention systems.
- See social systems and collection assistance boundaries, even if some are still limited.

Beta 1 does not need every long-term system fully monetized, tuned, socially networked, or backend-complete, but it must not confuse testers about which systems are real, placeholder, or deferred.

## Must-Have Beta 1 Feature Coverage

These should be present enough for testers to use and understand:

| Area | Beta 1 target | Current state | Readiness |
|---|---|---|---|
| Core bingo rooms | Playable, stable round loop with card count and mana entry | Prototype playable | Needs hardening |
| Realm 1 progression | Room map, room restoration, first realm sense of progress | Prototype shell/playable | Needs QA and save review |
| Room rewards | Rewards visible and understandable | Prototype | Needs tuning/config separation |
| Jackpot wheelspin | Earned from 5/5 card state, separate from Daily Spin | Prototype | Needs odds/config validation |
| Ingredient collection | Ingredients awarded from room play and visible in potion flow | Prototype | Needs UX/QA pass |
| Potion/restoration flow | Apothecary/potion progress understandable | Prototype shell | Needs final Beta scope |
| Grimoire | Card collection browsing, card states, NEW flags | Prototype | Needs cleanup and asset plan |
| Book of Shadows | Premium/special collection presence | Prototype | Needs product scope decision |
| Player's Den | Hub for profile, inventory routes, daily systems, social routes | Prototype shell | Needs navigation polish |
| Cabinet of Curiosities | Power-ups, sigils, Clairvoyance, Pandora inventory home | Prototype shell | Needs inventory/source clarity |
| Daily Bonus | Direct claim daily loop, separate from Inbox | Prototype | Needs tuning/config |
| Daily Spin | Separate daily wheel, direct claim | Prototype | Needs visual polish/config |
| Inbox | Claim layer for gifts/system items and message home | Prototype | Needs category/clear/expiry rules |
| Freebies | Social links/deep-link redemption concept | Prototype stub | Needs link/deep-link implementation |
| Friends | Add/manage shell, mana taps, block/report/message direction | Prototype shell | Needs composer and Aura-rank limits |
| Coven | Coven shell, member popup, wish list/gifts | Prototype | Needs join/manage scope |
| Library card gifting | Regular duplicate card gifting in Library only | Prototype with rank cap | Needs anti-abuse and reset/QA tools |
| Enchanted Trail | Free path shell and collect states | Prototype shell | Needs point source/reward scope |
| Oracle Alley / Madame Solange | Limited Oracle reading-table placeholder | Prototype shell | Rewards/costs remain open |
| Reset/dev tools | Testers can recover/reset daily systems and prototype state | Partial | Needs Beta QA panel |

## Nice-To-Have Beta 1 Feature Coverage

These add value if time permits, but should not block first beta unless they are central to a test objective:

- Friend message composer.
- Friend mana send/receive limits by Aura-derived rank.
- Coven join/manage placeholder.
- Cleaner Library card gift/trade staging.
- Basic onboarding/tutorial prompts.
- Relic Wall badge display polish.
- Player avatar/frame/dauber placeholder polish.
- Enchanted Trail free-path task logic.
- Oracle Alley visual polish with rewards still disabled or clearly placeholder.

## Defer Past Beta 1 Unless Explicitly Approved

- Real-money purchases and live store.
- Final monetization tuning.
- Final jackpot odds/economy balance.
- Full live messaging/chat moderation.
- Full card trading marketplace.
- Automated social sharing.
- Competitive Coven scoring and live events.
- Full Oracle Alley reward economy.
- Final duplicate conversion/Stardust/Joker Wild economy.
- Premium Enchanted Trail lane.
- Final rank-up chest contents.
- Full production analytics dashboard.

## Production Architecture Checklist

### Save And Account

- Inventory, currencies, album state, ingredient state, daily systems, room progress, Inbox, and social state currently rely heavily on `PlayerPrefs`.
- First infrastructure foundation is implemented: versioned local JSON snapshots, last-known-good backup recovery, append-only action journal, durable local guest identity, and replaceable service contracts. Existing gameplay state has not been migrated yet.
- The service layer now initializes once at startup. Profile cosmetic selections and Sound/Notifications have moved to a versioned durable snapshot with compatibility `PlayerPrefs` writes; gameplay progress, currencies, inventory, rewards, and social state remain on their existing prototype persistence paths.
- The multiplayer prototype now also has a provider/factory seam plus two backend-facing service contracts: runtime selection through `IPrototypeMultiplayerRuntimeProvider`, room/session lifecycle through `IMultiplayerRoomSessionService`, and call/claim/round-end authority through `IMultiplayerMatchAuthorityService`. The current `Ugs` runtime mode is an intentional local fallback only.
- Ordered per-state schema migrations and a redacted Persistence diagnostics/export panel are implemented. The panel also reports local UGS preflight state, Remote Config infrastructure safety state, disabled profile/settings Cloud Save sync state, conflict/offline policy gates, journal sync-staging policy, journal retention/privacy policy, and diagnostics export/share safety policy: packages resolved, adapter define status, live-call status, Remote Config risky/missing/unknown key counts, Cloud Save upload/download blocked status, automatic merge/remote overwrite blocked status, future-upload candidate counts, active upload eligibility, retained-record counts, archive/compaction/delete planning counts, export-redaction counts, local-file export readiness, external-share blocked status, and remaining cloud enablement blockers. No automatic journal retention, compaction, clearing, deletion, live upload, or external diagnostics share policy is active.
- Profile display name now persists in snapshot schema 2 with local Beta/test validation. A stable cosmetic catalog and runtime asset intake structure exist; ownership/unlock rules and final moderation remain unresolved.
- UGS Core, Authentication, Cloud Save, Remote Config, and Analytics package entries are resolved in the manifest, lockfile, and Unity package cache. Runtime adapters remain disabled pending project-link, consent, environment, profile/settings Cloud Save conflict policy, and offline-fallback checks. The profile/settings Cloud Save scaffold declares key `bmm.profile_settings.v2` and policy `profile_cloud_conflict_policy_v0.1` for future use but does not upload, download, merge, or overwrite local snapshots.
- Remote Config safety policy `infra_remote_config_safety_v0.1` is diagnostics-only. It defines infrastructure keys `infra_ugs_adapters_enabled`, `infra_cloud_profile_sync_enabled`, `infra_journal_upload_enabled`, `infra_analytics_upload_enabled`, and `infra_diagnostics_export_enabled`; live/cloud flags default off and do not enable runtime behavior.
- Diagnostics export/share safety policy `diagnostics_export_safety_v0.1` is also diagnostics-only. Local payload-free support export remains code-authoritative, while in-app share, clipboard forwarding, email/social handoff, and external Beta sharing remain blocked until privacy/support policy is approved.
- Beta 1 needs a decision: keep local-only testing, or introduce account/cloud persistence before external testers.
- Production must not rely on client-only local state for economy-critical rewards.

Candidate production paths:

- Unity Authentication plus Cloud Save/Economy/Remote Config.
- Firebase/Auth/Firestore/Cloud Functions.
- PlayFab.
- Custom backend.

UGS is the preferred Beta backend path unless a concrete blocker appears. Implement toward these services through replaceable facades rather than scattering SDK calls through gameplay/UI code:

- Authentication for anonymous first-run accounts and later Apple/Google/Facebook linking.
- Cloud Save for durable profile/progress state.
- Economy for server-owned balances, inventory items, album/card entries, and grant validation.
- Remote Config for tunable room, reward, odds, event, and offer values.
- Cloud Code for server-authoritative reward rolls, duplicate/card conversion checks, social grants, and anti-abuse validation.

Current multiplayer-specific backend boundaries now align with that direction:

- use the runtime provider as the only backend-mode selector;
- keep room/session lifecycle behind `IMultiplayerRoomSessionService`;
- keep call/claim/round-end authority behind `IMultiplayerMatchAuthorityService`;
- keep `BingoPrototype` and gameplay presentation unaware of concrete backend assembly.

Decision needed before production hardening:

- Which backend owns account identity?
- Which backend owns inventory and reward grants?
- Which values are remote-configurable?
- Which actions must be server-authoritative?
- Whether any concrete blocker forces the project away from the preferred UGS Beta backend path.

### Config And Tuning

Move open/tunable values out of prototype code and into data/config:

- Room reward values.
- Jackpot odds and wheel segments.
- Daily Bonus rewards.
- Daily Spin rewards.
- Aura/rank benefit table.
- Friend/Coven/card gift limits.
- Freebie expiration and reward payloads.
- Album rewards.
- Potion/restoration rewards.
- Enchanted Trail reward path.

Rules:

- Do not hard-code unresolved economy values as final.
- Keep draft values labeled as Beta/test config.
- Make config readable enough for product review.

### Content And Asset Delivery

Production content should be planned so seasonal rooms, card art, event assets, UI skins, and future room themes can be added without app-store updates where practical.

Direction to evaluate:

- Use Unity Addressables for loadable room themes, card/ingredient art, seasonal event content, and large UI asset groups.
- Store stable asset keys in content/catalog data instead of hardcoded scene references.
- Keep catalog metadata separate from final art so temporary Beta assets can be replaced cleanly.
- Define album/card/ingredient data so UI can instantiate reusable templates from IDs, set/page info, rarity/frame data, and Addressable asset keys.

Open decisions:

- Which asset groups must be bundled for first install vs downloadable.
- Whether Addressables become required for Beta 1 or remain a production-hardening task.
- Final naming conventions for Addressable keys, catalog IDs, and content versioning.

### UI Performance And Implementation

Bingo UI can become expensive once multiple active cards, daub states, animations, particles, power-up feedback, and reward overlays are visible at once.

Direction to evaluate:

- Keep 1/2/4/6 card play performance-safe on mobile before increasing polish.
- Continue using UGUI where practical, but isolate frequently changing bingo-card surfaces from static layout canvases to reduce unnecessary rebuilds.
- Evaluate UI Toolkit only for screens where its workflow and performance clearly help; do not rewrite working prototype UI solely for novelty.
- Use Sprite Atlases for repeated UI/card/ingredient icons.
- Use 9-sliced sprites for scalable panels, buttons, popups, and book/page surfaces.
- Keep visual feedback, particles, audio, and animation as client-side presentation that reacts to game state rather than owning reward or gameplay truth.

Open decisions:

- Final UI framework split, if any, between UGUI and UI Toolkit.
- Final asset atlas grouping and memory budget.
- Final mobile performance targets for active multi-card play.

### Network And Authority Prototype

If shared rooms, synchronized number calls, competitive bingo calls, or multiplayer-adjacent social state become Beta scope, create a greybox technical prototype before visual polish.

Direction to evaluate:

- Test server-owned number calls, round state, and bingo verification with plain UI first.
- Keep bingo/reward validation server-authoritative for economy-sensitive outcomes.
- Decouple network/data packets from local presentation effects.
- Consider UGS Lobby/Relay/Netcode or Photon only after deciding whether the product needs real-time shared rooms or mainly asynchronous social systems.

Open decisions:

- Whether Beta 1 needs real-time multiplayer at all.
- Whether bingo number calls are local-simulated, backend-seeded, or live server-broadcast.
- Which social and reward actions require server authority before external testing.

### Code Structure

Current prototype concentrates a lot of UI and state in `BingoPrototype.cs`.

Production direction:

- Split screen UI controllers by system.
- Split game services by responsibility.
- Keep catalogs/data separate from runtime state.
- Keep reward grants explicit about source/context.
- Keep Inbox claim semantics explicit.
- Avoid hidden rank/economy modifiers inside generic reward helpers.

Initial service boundaries:

- Account/Profile
- Inventory
- Rewards
- Rooms/Realms
- Albums/Collections
- Potions/Ingredients
- Daily Systems
- Inbox
- Friends/Social
- Coven
- Marketplace/Bazaar
- Analytics
- Remote Config

### Backend-Sensitive Systems

These should be treated carefully before external testing:

- Inventory grants.
- Inbox claim/clear behavior.
- Card gifting/trading.
- Friend mana sends/receives.
- Coven ingredient gifts.
- Freebie redemption and expiration.
- Daily Bonus and Daily Spin claim state.
- Aura-derived rank benefits.
- Purchases/receipts.
- Report/block/moderation actions.

### Analytics For Beta

Infrastructure status: a local analytics facade now records safe event envelopes into the local action journal. Analytics safety policy `analytics_safety_v0.1` keeps consent and live upload blocked, reports allowlisted versus local-only analytics events, and prevents Remote Config from enabling upload at runtime. The current local-only instrumentation surface now covers room enter, round start, bingo claim, round completion, round reward collect, room restore, album reward claim, social freebie redeem, social help request send, friend mana send/receive, inbox reward/message actions, daily bonus claim, daily spin claim, coven orb contribution, wild-card use, coven wish gifting, coven emporium purchases, and jackpot collect.

Payload/schema shaping for these events now routes through a reusable infrastructure helper instead of continuing to grow directly inside `BingoPrototype.cs`. Feature code still decides when to emit events; infrastructure owns the safe local payload shapes. See `Docs/18_LOCAL_ANALYTICS_EVENT_CATALOG.md` for the current event list and excluded payload classes.

The local journal now has a read-only diagnostics policy surface. It classifies records as retained locally, safe for payload-free summary export, candidate for future upload, blocked by sensitive payload markers, or blocked because the source/type is not allowlisted. Live upload remains disabled and active upload-eligible rows remain 0.

These feature events currently remain support-only local telemetry. They are useful for Beta verification and event-schema iteration, but they do not imply consent approval, server ingestion, dashboards, or production analytics ownership yet.

Diagnostics export remains local-file-only. Even if `infra_diagnostics_export_enabled` is toggled false in test config, the current build treats that flag as advisory rather than authority to silently remove the local support export path.

Minimum Beta event plan:

- App/session start.
- Room entered.
- Round completed.
- Bingo claimed.
- Jackpot spin earned.
- Reward claimed.
- Daily Bonus claimed.
- Daily Spin claimed.
- Inbox item claimed/read/cleared.
- Card acquired.
- Ingredient acquired.
- Potion/restoration progress updated.
- Freebie redeemed.
- Friend action used.
- Coven action used.
- Error/reset/dev action used.

Every event should include safe context:

- Player Level, Aura Strength, and Rank.
- Realm/room id when relevant.
- Reward source.
- Claim source.
- Prototype/Beta build marker.

### QA And Test Readiness

Before Beta 1, create QA passes for:

- New install first session.
- Returning player session.
- Daily reset behavior.
- Offline/clock-change behavior.
- Room completion and replay.
- Inbox gift claim.
- Album new/duplicate/missing card states.
- Daily Bonus and Daily Spin separation.
- Freebie redemption once/expiry behavior.
- Friend block/report/message controls.
- Aura/rank cap visibility.
- Reset/dev tools.

## Known Beta Risks

- Many systems still use local `PlayerPrefs`; testers can produce inconsistent state.
- Some feature shells look interactive before full production rules exist.
- Rank benefits are a working draft, not locked economy data.
- Existing prototype code still derives Rank from Level. This must be corrected to the Aura model before production hardening.
- Purchase contribution to Aura must be small, capped, and unable to independently advance Rank.
- Card gift/trade limits are not final, especially send-vs-receive treatment.
- Friend mana limits are requested but not tuned.
- Oracle Alley/Madame Solange remains visually desirable but reward/cost/odds are unresolved.
- Inbox has evolving category and read/claim/clear semantics.
- Freebie redemption needs real deep-link and anti-repeat implementation.
- The giant prototype script can slow safe production changes if not split soon.

## Recommended Work Order

1. Finish Friends messaging composer as a visible feature gap.
2. Add/clarify Friend mana capacity display without locking final values.
3. Add Beta reset/dev controls for Inbox card gifts, daily systems, and social counters.
4. Create a Beta 1 feature matrix and mark each system Must, Nice, Deferred.
5. Decide local-only Beta vs account/cloud-backed Beta.
6. Extract Aura/rank/social/reward tunables into a config surface.
7. Split the highest-risk prototype services out of `BingoPrototype.cs`.
8. Add analytics event stubs.
9. Build QA checklist and run it every narrow pass.
10. Only then broaden visual polish and beta packaging.

## Definition Of Beta-Ready

Beta 1 is ready when:

- The feature map is broad enough for testers to understand the game.
- Must-have systems do not dead-end or contradict locked rules.
- Prototype-only features are clearly labeled or safely disabled.
- Reward claims do not silently auto-grant before intended claim points.
- Core progress survives app restart in the chosen Beta persistence model.
- Reset/dev tools can recover bad test state.
- A QA checklist exists and can be repeated.
- Economy/Aura/rank/social values used in Beta are documented as draft or locked.
- Known deferred systems are not presented as complete.

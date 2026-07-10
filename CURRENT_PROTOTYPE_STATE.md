# Bingo Magic Mayhem - Current Prototype State

Last updated: 2026-07-10

Use this as the short entry point for fresh Codex threads. The detailed rules are split into the docs below.

## Start Here

- Locked product rules: `Bingo Magic Mayhem/Docs/LOCKED_DECISIONS.md`
- Unresolved product decisions: `Bingo Magic Mayhem/Docs/OPEN_DECISIONS.md`
- Known code/design mismatches: `Bingo Magic Mayhem/Docs/IMPLEMENTATION_GAPS.md`
- Verified project-state audit: `Bingo Magic Mayhem/Docs/10_VERIFIED_PROJECT_STATE.md`
- Numbered open-decision audit: `Bingo Magic Mayhem/Docs/09_OPEN_DECISIONS.md`
- Rank rewards working draft: `Bingo Magic Mayhem/Docs/11_RANK_REWARDS_V0.1.md`
- Beta 1 production readiness checklist: `Bingo Magic Mayhem/Docs/12_BETA_1_PRODUCTION_READINESS.md`
- Repeatable Beta QA checklist: `Bingo Magic Mayhem/Docs/13_BETA_QA_CHECKLIST.md`
- UGS production ownership map: `Bingo Magic Mayhem/Docs/14_UGS_PRODUCTION_OWNERSHIP_MAP.md`
- UGS-ready local service layer: `Bingo Magic Mayhem/Docs/15_UGS_READY_SERVICE_LAYER.md`
- Cosmetic asset intake contract: `Bingo Magic Mayhem/Docs/16_COSMETIC_ASSET_INTAKE.md`
- UGS adapter boundary: `Bingo Magic Mayhem/Docs/17_UGS_ADAPTER_BOUNDARY.md`
- Older baseline docs:
  - `Bingo Magic Mayhem/Docs/Gameplay Rules v0.1.md`
  - `Bingo Magic Mayhem/Docs/Game Systems Roadmap v0.1.md`
  - `Bingo Magic Mayhem/Docs/Prototype Architecture v0.1.md`
- Important external handoff reference:
  - `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md`
- ChatGPT project export archive:
  - `Bingo Magic Mayhem/Docs/Archive/ChatGPT_Project_Exports/`
- Cocos/Cocos2d chat export archive:
  - `Bingo Magic Mayhem/Docs/Archive/Cocos2d_Chat_Exports/`

## Working Agreement

- Unity is the chosen prototype direction.
- Existing docs are a baseline, but development has expanded beyond them.
- Check with the user before changing core gameplay, economy tuning, progression, jackpot odds, album structure, coven behavior, navigation architecture, or reward cadence.
- Prefer narrow implementation passes with clean verification.
- From July 2026 onward, treat new prototype work as Beta-directed: either visibly scoped placeholder work or production-direction architecture, not untracked throwaway behavior.
- Keep visuals prototype-level unless visual polish is explicitly requested.

## Project Layout

- Unity project: `Bingo Magic Mayhem`
- Main prototype script: `Bingo Magic Mayhem/Assets/Scripts/BingoPrototype.cs`
- Inventory/profile state: `Bingo Magic Mayhem/Assets/Scripts/PlayerInventoryState.cs`
- Realm/room data: `Bingo Magic Mayhem/Assets/Scripts/RealmContentCatalog.cs`
- Album/card data: `Bingo Magic Mayhem/Assets/Scripts/CardAlbumCatalog.cs`
- Coven state: `Bingo Magic Mayhem/Assets/Scripts/CovenState.cs`

Build check:

```powershell
dotnet build "Bingo Magic Mayhem.sln" -v:quiet -clp:ErrorsOnly
```

Most recent result: build succeeded with 0 errors and 2 warnings.

## Beta Direction

- The project is intended for production and external testing.
- Continue finishing visible Beta feature coverage, but start production hardening in parallel.
- New planning surface: `Bingo Magic Mayhem/Docs/12_BETA_1_PRODUCTION_READINESS.md`.
- UGS is the preferred Beta backend path unless a concrete blocker appears; implementation should proceed through replaceable service facades with local-first durable state and an action journal.
- First infrastructure pass is implemented under `Assets/Scripts/Infrastructure`: versioned durable JSON snapshots with backup recovery, append-only local action journal, stable local guest identity, identity safety diagnostics, local analytics facade, typed local Remote Config defaults, Remote Config infrastructure safety flags, a disabled profile/settings Cloud Save sync facade, and an SDK-free composition root. Gameplay/economy state has not been migrated.
- The infrastructure composition root now initializes once at prototype startup. Profile identity selections and Sound/Notifications are the first durable snapshot consumer, with compatibility `PlayerPrefs` writes during migration and local journal records for changes/recovery. Fresh New Player resets display name/cosmetics but preserves device preferences and the stable local guest identity.
- Prototype Settings now includes a Persistence diagnostics panel showing redacted identity, identity safety status, snapshot schema/health, journal counts, last recovery/migration, UGS preflight status, Remote Config safety status, disabled profile/settings Cloud Save sync status, blocked conflict/offline policy gates, local journal sync-staging policy, and journal retention/privacy policy. It can export a payload-free safe summary. Ordered snapshot migrations are supported; journal retention/deletion remains disabled until policy is approved.
- Profile settings snapshot schema 2 adds a locally editable display name with explicitly provisional Beta validation. Cosmetic choices now come from a stable id/asset-key catalog; profile avatar/frame previews automatically use matching Resources sprites when supplied and keep placeholder rendering when absent.
- The approved UGS package entries are resolved in `Packages/manifest.json`, `Packages/packages-lock.json`, and the Unity package cache. Live adapters are compile-gated behind `BMM_UGS_ADAPTERS` and remain disabled; the local preflight now reports package, define, project-link, consent, Cloud Save policy, and gameplay/economy sync readiness. The profile/settings Cloud Save scaffold uses key `bmm.profile_settings.v2`; conflict policy `profile_cloud_conflict_policy_v0.1` keeps upload, download, automatic merge, remote overwrite, and gameplay/economy sync blocked until project link, consent/privacy, timestamp authority, merge/overwrite behavior, and offline retry/idempotency are approved. Local journal policy classifies safe future-upload candidates versus sensitive/unapproved rows, but active upload eligibility remains 0 and no upload path is active.
- Remote Config safety policy `infra_remote_config_safety_v0.1` defines infrastructure-only keys for UGS adapters, profile Cloud Save sync, journal upload, and diagnostics export. The first three default false and cannot enable runtime behavior; diagnostics flags risky true values, missing keys, and unknown keys.
- Identity safety policy `identity_safety_v0.1` keeps cloud sign-in, account linking, recovery, and Remote Config-driven auth bypass blocked while the local guest path remains authoritative.
- Analytics safety policy `analytics_safety_v0.1` keeps consent and live upload blocked, tracks allowlisted vs local-only analytics events, and blocks Remote Config-driven upload bypass.
- Journal retention/privacy policy `journal_retention_policy_v0.1` keeps retention, archive, compaction, delete, and clear operations blocked while counting planning candidates and export-redaction requirements.
- Unity EditMode `InfrastructureServiceTests` passed 22/22 on 2026-07-10 after adding the disabled Cloud Save profile/settings sync, conflict/offline policy, Remote Config safety, and identity safety scaffolds. Journal retention/privacy scaffolding adds three more EditMode tests for the next Unity Test Runner pass.
- Key transition risks: `PlayerPrefs` local persistence, giant prototype script concentration, unresolved backend/account strategy, reward/config tuning still embedded in prototype code, and social/economy anti-abuse rules still draft.

## Current Implemented Prototype Areas

- Standard bingo gameplay.
- Special blackout room prototype.
- Room home/card count/mana bet flow.
- World map and realm map shells.
- Room restoration and realm progression prototype.
- Jackpot wheelspin prototype.
- Power-up bank and gameplay sigil prototype.
- Clairvoyance timed boost prototype.
- Ingredient collection and potion restoration prototype.
- Player's Den shell.
- Player Profile shell from the Den with Profile, Avatars, and Settings tabs. Includes placeholder player name/avatar/frame/dauber/login/toggle controls only; no gifting or trading hooks.
- Relic Wall display shell from the Den with identity, collection, realm, social/event, and prestige badge placeholders. Display-only; no achievement rewards or badge criteria are active.
- Enchanted Trail shell from the Den replacing the old Quick Inventory panel. Free-path task/reward placeholders with prototype Collect/Claimed button states only; claimed states persist locally through PlayerPrefs and reset with Fresh New Player. No final duration, point source, task list, reward value, premium lane, inventory reward grant, or rank scaling is active.
- Bewitchment Bazaar shell from the Den with a center card/crystal swap cart placeholder, Oracle Alley route, and Coven route. Oracle Alley opens Madame Solange Lumiere's reading table prototype with selectable face-down cards, preview-only reveals, and a Book of Shadows bonus placeholder after three selections. Reading cadence, costs, rewards, odds, dust conversion, Book of Shadows interactions, and scarcity protections remain unresolved; no Oracle economy or reward logic is active.
- Friends add/manage shell from the Den with prototype friend list, incoming requests, sent requests, add/search placeholder, and accept/decline/remove/cancel controls. Prototype daily 10-mana friend mana actions are labeled as Give 10 and Claim, with current same-day send/receive counts shown clearly. Friend lists, requests, blocks/reports, and same-day mana send/receive states persist locally through PlayerPrefs for testing. Friend messages open a simple compose popup, then route to Inbox > Messages and live there as open/read/reply/clear items with sent/received labels; friend mana Aura-rank limits and inventory grants remain unresolved placeholders. No gifting, trading, ingredient help, or join-a-club flow is active from Friends.
- Leaderboards shell from the Den with Friends, Coven/team, and Global tabs. Standings are static prototype rows for layout/testing only; final score formula, season cadence, rewards, ties, moderation, and backend syncing remain unresolved.
- Mayhem Market shell from the Den for future in-app purchases. Displays placeholder categories for Mana, Crystals, Card Packs, Cosmetics, Special Offers, and Restore Purchases; all purchase buttons are disabled and no platform IAP, prices, grants, receipts, restore flow, or monetization tuning is active.
- Apothecary shell from the Den is potion-only: active potion status, ingredient progress, completion reward placeholder, and locked workbench placeholders. Sigils, Clairvoyance, Pandora, and boost inventory stay in Cabinet of Curiosities.
- Account rank display and rank ladder popup in Player's Den/profile debug now label Aura Strength and Aura-derived Rank as TBD. The prototype no longer presents rank thresholds as Level bands, but card/friend social caps still use temporary stand-in logic until the Aura formula is implemented.
- Cabinet of Curiosities prototype is the Den home for playable sigils, Clairvoyance/timed boosts, Pandora Sigil, and power-up inventory. Cards route to Library/Grimoire; Club Orbs route to Coven systems.
- Library/Grimoire/Book of Shadows prototype.
- Library card gifting prototype for extra regular Grimoire duplicates, with clearer give/receive test states and Inbox card-claim delivery. Prototype send/receive card gift buttons currently use the older Level-derived rank working-draft daily cap; this must move to Aura-derived Rank. Apprentice and Novice are locked at 0/day, Spellbinder starts at 1/day in the draft benefit table. Final shared-vs-separate send/receive limits remain unresolved.

## Aura Correction

- Level and Rank are separate systems.
- XP fills Level progression.
- Aura Strength determines Rank.
- Aura Strength should combine approved account-strength sources, including Level/XP history, gameplay/account progress, collection/restoration progress, social contribution, and a small capped purchase contribution.
- Purchases can contribute to Aura only as a minor capped support signal. Purchases cannot independently cause Rank advancement or bypass gameplay/collection/restoration progress.
- Existing prototype social cap logic still uses temporary Level-derived stand-ins and should be treated as provisional until the Aura model is implemented.
- Library card state readability pass: missing/owned/duplicate states and number-only duplicate badges in Grimoire/Book of Shadows card grids.
- Grimoire ingredient detail prototype with Ask for Help and Use Wild shells.
- Card reveal and NEW card flags.
- Coven shell, member popup, wish list, join request, coven gift prototype, and Find/Join Coven discovery placeholder. Discovery requests persist locally for testing, but public/private rules, invite links, search filters, approvals, and backend membership are not active.
- Daily Bonus prototype with streak-save state.
- Daily Spin prototype as a separate direct-claim daily loop.
- Inbox prototype with shared reward item plumbing for gifts/system rewards.
- Freebies/social-link prototype stub.

## Recommended Next Thread Prompt

```text
Continue from CURRENT_PROTOTYPE_STATE.md plus Docs/LOCKED_DECISIONS.md, Docs/09_OPEN_DECISIONS.md, Docs/10_VERIFIED_PROJECT_STATE.md, Docs/11_RANK_REWARDS_V0.1.md, Docs/12_BETA_1_PRODUCTION_READINESS.md, Docs/OPEN_DECISIONS.md, Docs/IMPLEMENTATION_GAPS.md, the ChatGPT export archive at Docs/Archive/ChatGPT_Project_Exports, the Cocos/Cocos2d archive at Docs/Archive/Cocos2d_Chat_Exports, and the comprehensive handoff at C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md. Work narrowly. The project is now Beta-directed: finish visible feature coverage while beginning production hardening. Do not change core gameplay, economy tuning, jackpot, album, coven, room progression, or navigation rules without asking first.
```

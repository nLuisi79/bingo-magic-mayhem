# Open Decisions Audit

Last updated: 2026-07-09

This file is the verified open-decision register for the current Unity prototype. It consolidates `OPEN_DECISIONS.md`, `IMPLEMENTATION_GAPS.md`, the current prototype state file, the locked-system Word handoffs, `covens.txt`, and the comprehensive June 26 handoff.

Authority rule: locked docs and current handoffs win over older v0.1 docs and archive/chat-derived materials. Do not finalize gameplay, economy, progression, rarity, monetization, or reward decisions from this file without explicit user approval.

## Documentation Structure Open Issues

- `AGENTS.md` was requested by the audit instruction but is not present in this checkout.
- `docs/00_READ_ME_FIRST.md` was requested by the audit instruction but is not present in this checkout.
- The active docs directory is `Bingo Magic Mayhem/Docs`, not root-level `docs`.
- `CURRENT_PROTOTYPE_STATE.md` is the short entry point, but it did not yet reference this numbered audit set before this pass.
- `OPEN_DECISIONS.md` and this numbered file overlap. Future doc cleanup should decide whether the numbered file replaces the older unnumbered register or whether the unnumbered file remains the quick working list.
- Archive folders now define their own authority rules. Raw ChatGPT/Cocos exports are searchable reference only until reviewed and reconciled into locked/current docs.

## Economy and Reward Values

- Final mana economy scale remains open.
- Final crystal economy scale remains open.
- Final XP curve and per-level reward values remain open.
- Final room reward math, restore reward amounts, jackpot contribution rates, and jackpot odds remain open.
- Reward categories are more locked than reward amounts. Amounts and odds should remain remotely tunable.
- Potion/Grimoire reward categories are locked around mana, crystals, general power-ups, and Clairvoyance duration, but exact values remain tunable.
- World restore reward categories are mana, card pack, and crystals, but exact amounts remain tunable.
- Realm restore reward categories are mana, card pack, chance at rare cards, and hourly restore multiplier increase, but exact amounts and odds remain tunable.

## Daily, Freebie, Inbox, and Retention Loops

- Daily Bonus is direct-claiming and separate from Inbox, Daily Spin, Freebies, Realm income, and Enchanted Trail.
- Daily Spin is separate from Daily Bonus and jackpot wheelspin, but its final cadence, reward values, ticket/ad relationship, and odds remain open.
- Freebies are official social/deep-link rewards and should not be routed through Inbox unless a later decision explicitly changes that. Final deep-link format, backend validation, reward pools, cooldowns, and anti-abuse rules remain open.
- Freebie links should expire after about two weeks, but expiration should not be exposed in the URL.
- Inbox is for claimable gifts/system items where explicit collection is required. Final tabs, categories, expiration rules, claim-all behavior, purchase receipts, and notification badges remain open.
- Enchanted Reward Trail is not implemented and remains largely undefined.

## Grimoire, Cards, Rarity, and Trading

- Grimoire is locked as the main/free album.
- Grimoire scale is now locked by the Word handoff as 32 sets/pages x 10 cards each = 320 cards total, with 8 sets per index page.
- Grimoire duration is locked as 90 days.
- Book of Shadows is locked/strong as the premium/special collection, but exact set/card scale remains open.
- Current rarity frame structure is Regular, Gilded, Ancient. Older rare/extra-rare language should be mapped to Gilded/Ancient carefully.
- Gilded/Ancient distribution by set range is strong/current, but exact economy impacts and acquisition odds remain open.
- Duplicate card behavior, duplicate conversion value, Joker Wild use, Stardust/market value, and pre-completion duplicate uses remain open.
- Card trading send/receive limits, rarity eligibility, cooldowns, and anti-abuse rules remain open.
- Card gifting/trading surface is locked to Library/Grimoire.
- Current prototype supports gifting extra regular Grimoire duplicates from Library and claiming incoming card gifts through Inbox > Cards.
- The exact production Library trade/gift UI remains open.
- Whether special cards/purchased systems share the same Library UI remains open.

## Ask for Help, Use Wild, and Collection Assist

- Ask for Help is anchored from Individual Ingredient Detail, not the Coven hub itself.
- Locked screen structure: left ingredient summary, right recipient selector, Coven/Friends tabs, shared daily help request counter, recipient grid, Send Request button.
- Coven and Friends share one rank-based daily help request pool.
- Coven is available for eligible ingredients; Friends availability rotates by ingredient/day.
- Exact friend rotation schedule remains open.
- Exact request expiration timing remains open beyond the locked 48-hour wish-list refresh.
- Exact production UI for choosing ingredient request quantities remains open, but the wish-list quantity rule is locked: up to 10 total requested quantity across selected realm ingredients.
- Use Wild is locked as a card-to-card confirmation from Individual Ingredient Detail. No separate Use Wild inventory screen.
- Wild count decreases and selected ingredient count increases only after confirmation.
- Collection Assist belongs to the Coven/social support ecosystem and should show needs/extras, but sharing remains manual.
- Collection Assist MVP scope remains open enough to require a narrow product pass before implementation.

## Coven System

- Teams are called Covens.
- Coven is a major retention/social system, not a side feature.
- Manual sharing, daily share limits, needs/extras visibility, and contribution tracking are established.
- Member profile can be a popup/modal over Coven Circle; it does not need to be a full screen.
- Coven wish lists are ingredient-help focused; card gifting/trading belongs in Library/Grimoire.
- Covens cap at 50 members in the current locked docs, while `covens.txt` still lists max size as needing final lock. Treat 50 as current implementation lock unless user reopens it.
- Full role/permission list remains open beyond High Priestess/High Priest leadership language.
- Join requirements, public/private/invite settings, officer/admin tools, chat moderation, request expiration, contribution rewards, and anti-abuse rules remain open.
- Coven Ritual naming is strong/current: Ritual Board, Ritual Calls, Ritual Marks, Sigil, Sigil Set, Coven Points, Circle Rankings, Coven Orbs, Coven Emporium, Ritual Summary.
- Coven Ritual cadence details, reward tiers, Emporium prices, and standings rewards remain open.

## Aura, Rank, And Level Systems

- Rank is now Aura-derived, not Level-derived. XP fills Level; Level is one input into Aura Strength; Aura Strength lifts Rank.
- Long account rank titles are locked in `LOCKED_DECISIONS.md` from Novice through Sorcerer Supreme.
- `11_RANK_REWARDS_V0.1.md` is the current single review surface for the rank benefit working draft.
- Final Aura formula and source weights remain open.
- Final Aura thresholds per rank remain open.
- Purchase contribution to Aura must remain small, capped, and unable to independently cause rank advancement.
- The Word handoff also contains a five-bracket benefit scale: Apprentice, Spellcaster, Wizard, Archmage, Oracle.
- Older Level-band language conflicts with the Aura model and should be treated as superseded threshold scaffolding, not final rank math.
- Rank benefits are locked to four lanes: Identity, Daily Comfort, Social Help Capacity, and Rank-Up Chest.
- Current rank benefit direction favors a smaller capped comfort scale instead of a universal percentage across all rewards.
- Current working draft for rank benefit scale remains attached to rank titles, not final Aura thresholds:
  - Novice: 0% comfort bonus, 3 ingredient sends/day, 0 card gifts/trades/day.
  - Apprentice: 2% comfort bonus, 4 ingredient sends/day, 0 card gifts/trades/day.
  - Spellbinder: 4% comfort bonus, 5 ingredient sends/day, 1 card gift/trade/day.
  - Mage: 5% comfort bonus, 6 ingredient sends/day, 1 card gift/trade/day.
  - Thaumaturge: 8% comfort bonus, 7 ingredient sends/day, 1 card gift/trade/day.
  - Mystic: 10% comfort bonus, 8 ingredient sends/day, 2 card gifts/trades/day.
  - Enchanter: 12% comfort bonus, 9 ingredient sends/day, 2 card gifts/trades/day.
  - Wizard: 15% comfort bonus, 10 ingredient sends/day, 2 card gifts/trades/day.
  - Spellmaster: 18% comfort bonus, 11 ingredient sends/day, 3 card gifts/trades/day.
  - Archmage: 20% comfort bonus, 12 ingredient sends/day, 3 card gifts/trades/day.
  - Grand Archmage: 22% comfort bonus, 13 ingredient sends/day, 3 card gifts/trades/day.
  - Paragon: 25% comfort bonus, 14 ingredient sends/day, 4 card gifts/trades/day.
  - Ascendant: 30% comfort bonus, 15 ingredient sends/day, 4 card gifts/trades/day.
  - Sorcerer Supreme: 35% comfort bonus, 16 ingredient sends/day, 5 card gifts/trades/day.
- The comfort bonus should apply only to safe daily/account reward sources if approved: Daily Bonus mana, Daily Spin common mana, Mana Cauldron refill/capacity, level-up currency rewards, and Enchanted Trail free-path currency.
- The comfort bonus should not apply to bingo odds, jackpot odds or values, rare/Gilded/Ancient card odds, ingredient drop odds, trade value, duplicate market value, Coven/event scoring, or room/realm progression requirements.
- Coven ingredient wish-list request quantity remains 10 total per 48-hour refresh; rank should affect help sent per day, not how much a player can request.
- Rank-up chest content categories are narrowed to power-ups, Clairvoyance, and infrequent stars; exact quantities, durations, star frequency, cosmetic unlocks, card gift/trade eligibility, rounding rules, and implementation timing remain open.
- Level-up screen structure is locked, but exact per-level values remain open.

## Realm, Room, and Restoration

- Unity prototype currently locks only Realm 1 Room 1 open on reset, with restoration unlocking the next room and final room unlocking the next realm.
- The Word handoff locks four room/world locations per realm: three regular rooms and one special/finale room.
- Older handoff language about 4-5 Mystical Lands, Threshold Boards, and closed post-completion boards is superseded or reference-only.
- Whether player-facing terminology should say room, world, or both remains partially unresolved. Current implementation and locked docs use realm/room; Word handoff uses world/room.
- Exact Realm 2+ structure, special room frequency, replay rewards, and hundreds-of-realms scalability remain open.
- Broken/restored paired background state rules are locked in visual direction, but final art per room remains open.

## Bazaar, Oracle Alley, and Madame Solange

- Bewitchment Bazaar is the marketplace hub name and should not be replaced without approval.
- Oracle Alley is a limited-time Oracle/tarot subset in or near the Bazaar.
- Madame Solange Lumiere is the Oracle/tarot NPC or event host, not a general quest guide or Daily/Coven/album coach.
- Oracle readings must be chance-based and must protect scarcity.
- Oracle Alley timing, Oracle Dust source/sink/pricing, reward table, odds, Book of Shadows bonuses, and anti-brute-force limits remain open.

## Visual and UX Decisions

- Landscape is locked for gameplay, Grimoire, potion, reward, social, and background systems unless explicitly changed later.
- Current style target is bright magical storybook/casual-game UI: deep purple, warm gold, magenta/pink magic, lavender glow, teal accents, cream parchment, green buttons.
- Older dark/painterly map imagery should not override the brighter readable storybook target.
- Most final art assets remain open.
- Mockups and screenshots are visual direction, not exact implementation requirements unless explicitly locked.

## Technical and Data Model Decisions

- Unity is the active prototype direction; Cocos materials are archive/reference.
- PlayerPrefs persistence is prototype-only.
- Final profile/backend save format remains open.
- Remote config/data-driven reward tables are strongly required for economy and odds.
- `BingoPrototype.cs` remains monolithic and needs future decomposition, but refactors should remain behavior-preserving.
- Ingredient/social help state needs durable fields: eligibility, daily help used/limit, selected recipients, sources, and Wild inventory.
- UGS is the preferred Beta backend path unless a concrete blocker appears. Begin planning and implementation around Unity Authentication, Cloud Save, Economy, Remote Config, Analytics, and Cloud Code through replaceable service facades. Firebase, PlayFab, custom backend, or hybrid remain fallback options, not equal first-pass targets.
- Economy-sensitive actions should become server-authoritative before production use: inventory grants, reward rolls, duplicate/card conversions, social grants, Freebie redemptions, daily claims, purchases/receipts, and abuse-prone trading/gifting.
- Unity Addressables are a strong content-delivery candidate for seasonal rooms, card/ingredient art, event assets, and large UI asset groups, but exact Beta 1 requirement and asset-bundle strategy remain open.
- Album/card/ingredient UI should trend toward reusable data-populated templates with stable catalog IDs, set/page data, rarity/frame data, and asset keys; final metadata schema remains open.
- Multi-card bingo UI performance needs a production pass before broad mobile testing: canvas rebuild strategy, Sprite Atlas grouping, 9-sliced scalable UI, animation/particle budget, and final UGUI vs UI Toolkit split remain open.
- Real-time multiplayer is not locked. If live shared rooms or synchronized bingo calls become scope, build a greybox network-authority prototype before visual polish.

## Sunpetal Background Intake Update — 2026-10-06

See [Sunpetal background state intake](21_SUNPETAL_BACKGROUND_STATE_INTAKE.md) for the five user-supplied source images, confirmed dimensions, artwork path, and current handoff status.

- Source receipt is verified; repository placement, Unity import, runtime keys, and rendering are not yet verified.
- Navigation and title graphics will be supplied by the user after the backgrounds are in place.
- Confirm state-to-room/hotspot mapping without treating `realm1restore` / `realm2restore` / `realm3restore` as global realm identities.
- Verify alignment, transition behavior, crop/safe areas, and overlay readability before closing visual integration work.
- Retained thorny vines in the final upper-right terrace are an assistant review observation, not an approved art change.
- This asset set does not finalize restoration triggers, progression, rewards, economy, rarity, or monetization.

Documentation-location correction: root `AGENTS.md` and root `00_READ_ME_FIRST.md` are present and were read for this update. The earlier statement that AGENTS.md was absent is outdated. `docs/00_READ_ME_FIRST.md` remains absent. The current entry point already links the numbered audit documents. Root packet confidence labels and newer Unity locks differ in places; reconcile explicitly rather than applying the older packet indiscriminately. Existing duplicated open-decision registers still need consolidation.

### Sunpetal Import Follow-Up — 2026-10-06

The five supplied backgrounds have replaced the existing `sunpetal-realm-broken0`, `broken1`, `broken2`, `broken3`, and `restored` PNGs in `Assets/Resources/prototype/ui/realm_home/sunpetal`. Image hashes and preservation of all existing metadata were verified. Existing restoration-count selection and Resources loading already address these files; no code changed. Repository placement and runtime-path discovery are complete, superseding the earlier pending status. Unity editor reimport/on-screen verification and title/navigation overlay review remain pending. See `21_SUNPETAL_BACKGROUND_STATE_INTAKE.md` for the exact mapping.

### Sunpetal Production Asset Supersession — 2026-10-06

The newer production-tree files `sunpetal-realm-broken.png`, `sunpetal-realm-1.png`, `sunpetal-realm-2.png`, `sunpetal-realm-3.png`, and `sunpetal-realm-restored.png` now occupy the existing `broken0` through `restored` runtime slots. This supersedes the prior artwork import without changing runtime keys, metadata, gameplay code, or restoration-count selection. Remaining open checks are Unity editor reimport, on-screen crop/safe-area review, overlay readability with the pending navigation/title graphics, and transition registration between all five states. The previous thorn observation described the superseded artwork and is not an approved requirement for the current set.

### Sunpetal Realm-Home UI Follow-Up — 2026-10-14

The complete parchment header, dark-green room labels, arrows, medallion frame/lock, and four potion/shadow pairs are now installed and wired to the existing Sunpetal realm-home flow. The original `fullBroken` through `st-fullrestore` sequence was re-supplied and now supersedes the 2026-10-06 production-tree background set; all five installed hashes match the newly supplied files.

Unity import and editor compilation are confirmed. Still open: device-safe placement, text fit, overlay contrast, button hit-target review, disabled-state treatment for the next-realm arrow, and visual review of every restoration state. This visual integration does not resolve or change room unlock logic, restoration triggers, progression, gameplay, economy, rarity, rewards, or monetization.

The first-pass follow-up removes redundant textual room-state labels, uses full/shadow/lock medallions as the visual state language, adds the supplied combined ingredient-count badge to unfinished unlocked rooms, and replaces the temporary top-right bar with the existing production world-map counter/home assets. Runtime visual review remains open; the presentation change does not alter underlying state rules.

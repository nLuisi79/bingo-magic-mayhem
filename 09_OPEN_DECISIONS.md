# 09 — Open Decisions

## Source-of-truth rule for Codex

Use these Markdown files as the Codex-facing packet, but treat them as derived from the extraction documents below:

1. `Bingo_Magic_Blast_Locked_Decision_Register.docx`
2. `Bingo_Magic_Blast_Economy_Design_Document.docx`
3. `Bingo_Magic_Blast_Realms_World_Progression_Document.docx`
4. `Bingo_Magic_Blast_Visual_UI_Direction_Document.docx`

When a feature is labeled **LOCKED**, Codex may model data structures, UI placeholders, and configuration around it. When a feature is **PROPOSED**, **OPEN**, or **ARCHIVE / OUTDATED**, Codex must not build irreversible implementation logic around it without explicit product approval.

## Confidence labels

- **LOCKED** — appears explicitly chosen in later discussion, reflected in a packaged locked handoff, or called out as the current source of truth in the extraction documents.
- **HIGH / LIKELY FINAL** — repeated later preference or consistent direction, but still needs exact tuning, naming, or implementation confirmation.
- **PROPOSED** — usable as design context only; do not implement as a final rule without approval.
- **OPEN** — known unresolved decision or contradiction.
- **ARCHIVE / OUTDATED** — older architecture or replaced idea; do not implement unless re-approved.

## Open-decision handling rule

Everything in this file requires product approval before Codex treats it as implementation truth. Codex may create configurable placeholders, TODOs, interfaces, or test fixtures, but must not hardcode final values or lock gameplay outcomes.

## Extracted open decisions and unresolved tuning

## 19. Open decisions

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Final public title: Bingo Magic Blast vs Bingo Magic Mayhem. | Open identity decision | Older handoffs use Bingo Magic Blast; later locked visual file uses Bingo Magic Mayhem. | Choose one before logo, app-store metadata, and master handoff. | Current user requested Bingo Magic Blast for this document, so file title uses Bingo Magic Blast. |
| Final economy relationship between mana, coins, energy, crystals, and tickets. | Open economy decision | Later UI locks mana bet per card, while older architecture includes coins/energy/crystals. | Top currency bar and reward tables depend on this. | Do not balance economy until resource hierarchy is locked. |
| Final jackpot model: wheelspin only, Portal Surge, Jackpot Spins, or layered system. | Open reward decision | Later UI locks jackpot wheel placement; older architecture includes Portal Surge and Jackpot Spins. | Need decide if Portal Surge survives as separate rare event. | Avoid casino-like wheel language/visuals beyond approved jackpot wheel. |
| Final collection counts for Grimoire and Book of Shadows. | Open collection decision | Later 80-card preference conflicts with older 25-set/90-day and 32-set rarity discussions. | Must lock before final card list, rarity tables, UI slots, rewards, and drop rates. | 10 cards per set appears locked. |
| Final Ancient/Gilded quantity and rules per set. | Open rarity decision | The topic exists but the actual counts are not surfaced in available source snippets. | Impacts card art, rarity frames, trading, wildcard rules, and pack odds. | Retrieve specific project discussion before deciding. |
| Final duplicate conversion formula and market value behavior. | Open economy decision | Duplicate tracking/conversion is required, and market value was strongly discussed, but final formula is not locked. | Must align with trading, set completion, card packs, and monetization fairness. | Variable market value may add complexity. |
| Final trade/gift eligibility rules. | Open social/economy decision | Coven + Bazaar handoff scope includes what can/cannot be traded, but details are not visible in available snippets. | Needs daily limits, cooldowns, rarity restrictions, ingredient/card differences, and Coven-only/global rules. | Fairness and abuse prevention are key. |
| Final Oracle Alley / Madame Solange role. | Open feature/naming decision | Oracle Alley remained a possible sub-feature; Madame Solange was not found in available source search. | Needs name, host character, availability, output, odds, and economy constraints. | Do not include as locked until explicitly chosen. |
| Final daily rewards calendar/streak design. | Open retention decision | Rewards/Retention handoff exists, but exact daily ladder is not visible in snippets. | Needs calendar length, streak rules, missed-day handling, reward types, and Council/Coven interactions. | Daily reward claim animation is clearly needed. |
| Final MVP vs Phase 2 scope for Coven Ritual, Coven Emporium, Player Den, Bazaar, Oracle Alley, and Book of Shadows. | Open scope decision | Some systems have locked handoffs but older architecture placed full Coven Ritual and advanced systems in Phase 2. | Master MVP handoff should not be created until scope is reconciled. | Locked concept does not automatically mean MVP. |

## Unresolved tuning values

Mana cost per card and per room.

Whether mana cost scales by 1 / 2 / 4 / 6 card selection linearly or with a discount/premium.

Whether older coins and energy remain as separate resources.

Crystals earn rate, purchase rate, and spend sinks.

Charm Token earn rate, redemption value, and acquisition sources, if Charm Tokens are retained.

Stardust earn rate, duplicate conversion ratio, and redemption thresholds, if Stardust is retained.

Oracle Dust earn rate, reading cost, and wild-card odds, if Oracle Dust is retained.

Regular duplicate conversion ratio.

Rare and Extra Rare duplicate conversion ratio.

Book of Shadows duplicate behavior and whether it uses a separate conversion path.

Wild card shard thresholds and rarity eligibility.

Daily reward calendar length, streak rules, and missed-day behavior.

Daily spin availability and prize table.

Weekly reward track milestones, free-track reward amounts, and paid upgrade reward amounts.

Portal Surge odds, jackpot wheel odds, and whether those are one system or two.

Marketplace/set value formula and whether value can be cashed out, traded, or only displayed.

## Global risks / balance concerns

Currency clutter risk: Mana, coins, crystals, Charm Tokens, Stardust, Oracle Dust, Coven Orbs, wildcard shards, Jackpot Spins, Ritual Calls, and other event resources can overwhelm players if too many appear at once.

Duplicate frustration risk: Collections are a major motivator, but duplicate-heavy progression can feel punishing unless conversion, sharing, requesting, and New-to-You rewards are generous and transparent.

Premium collection fairness risk: Book of Shadows should feel premium/special, but not predatory. Duplicate behavior for a premium track must be more protective than the base collection or clearly more rewarding.

Jackpot/casino tone risk: The jackpot wheel must stay magical and casual-game-like, not casino-like. Portal Surge should be visually distinct from a standard prize wheel if retained.

Market value complexity risk: Variable set value may make the economy feel richer, but can confuse players and complicate balancing, trading, fraud prevention, and messaging.

Social abuse risk: Card/ingredient trading, gifting, and requests require limits, cooldowns, rarity restrictions, and anti-exploit controls.

Paid track risk: A paid upgrade track must not make free players feel blocked from core collection completion.

## Things Codex or a developer must not assume

Do not assume mana, coins, and energy are all final separate currencies.

Do not assume Charm Tokens, Stardust, or Oracle Dust have confirmed earning or spending rules.

Do not assume duplicate conversion rates; no final formula is locked.

Do not assume rare, extra rare, Ancient, Gilded, or Book of Shadows duplicates convert into the same resource as regular duplicates.

Do not assume any paid upgrade track exists as final monetization without a lock.

Do not assume Portal Surge, jackpot wheel, and Jackpot Spins are interchangeable names for the same feature.

Do not assume Book of Shadows is always paid, always premium-only, or always separate from the main Grimoire reward logic.

Do not assume completed set market value is spendable currency; its function is unresolved.

Do not assume trading is global; Coven-only vs broader marketplace scope is unresolved.

Do not assume rare/extra rare/Ancient/Gilded cards are tradeable or wildcard-eligible.

## Final lock checklist before Codex/development implementation

Confirm final resource hierarchy: mana vs coins vs energy.

Confirm whether Charm Tokens, Stardust, and Oracle Dust exist.

Confirm duplicate conversion output and thresholds by card class.

Confirm Book of Shadows duplicate behavior separately from Grimoire.

Confirm wild card types and eligibility rules.

Confirm Grimoire and Book of Shadows collection sizes and durations.

Confirm realm completion payout table.

Confirm jackpot wheel vs Portal Surge architecture.

Confirm daily rewards, daily spin, weekly track, and paid track scope.

Confirm Marketplace / Bewitchment Bazaar value formula and trade limits.

Confirm monetization constraints and premium currency sinks.

## 15. Unresolved contradictions between older and newer versions

| Topic | Older version | Newer/later-current version | Required decision |
| --- | --- | --- | --- |
| Realm structure | Realm contains 4-5 Mystical Lands. | Realm contains four rooms: three regular + one special/finale. | Use four-room model unless older terminology is explicitly revived. |
| Progression object | Restore Enchanted Artifact through fragments. | Complete potions/spells through ingredient cards and cast them in special room. | Decide whether artifacts are display relics only, narrative items, or gameplay objects. |
| Finale board | Threshold Board is a special finale experience that closes after completion. | Special room remains part of the four-room realm and may remain replayable/challenge-ready. | Rename Threshold Board, retire it, or map it to special room. |
| Room access after completion | Standard boards remain playable; Threshold Boards close except during Portal Reawakening. | Rooms should remain accessible so players can replay for ingredients. | Lock exact access rules for special room after restoration. |
| Realm names/order | Whispering Fen -> Shattered Observatory -> Clockwork Spire -> Dragonwake Ruins -> Frostveil Peaks -> Mirage Dunes. | Current locked work centers on Everbloom Sanctuary and Sunpetal Conservatory. | Lock final Realm 1, Realm 2, and world order. |
| Portal system | Threshold Board opens next realm; Portal Surge and Enchanted Keys exist as rare jackpot-style feature. | Open portal state and visible jackpot wheel are later-current UI/world pieces. | Define whether portal progression, jackpot wheel, and Portal Surge are separate systems. |
| Special mode | Threshold Board has custom board mechanic and higher buy-in. | Special room may be blackout or multi-bingo, but not chosen. | Select special-room format and whether it varies by realm. |
| Rewards | Older architecture includes broad rewards: coins, energy, crystals, power-ups, collection cards, Jackpot Spins, Ritual Calls, Realm progress. | Restore reward specifically constrained to mana and card packs. | Separate match rewards from realm-restore rewards. |

## 17. Developer / Codex must not assume

Do not assume the older 4-5 Mystical Lands model is final.

Do not assume Whispering Fen is Realm 1 or Shattered Observatory is Realm 2.

Do not assume Threshold Board is the final user-facing name for the special room.

Do not assume Threshold Boards close after completion; later direction says rooms should remain accessible for ingredients.

Do not assume artifact fragments are still the primary realm progression item.

Do not assume every potion has exactly 10 ingredients until the Ingredient / Potion Drop Rules locked handoff is fully reviewed, even though 10-item structures recur strongly.

Do not invent missing room names for Everbloom Sanctuary or Sunpetal Conservatory from visual guesses.

Do not implement blackout as the special room mode until it is explicitly selected over multi-bingo or another format.

Do not merge jackpot wheel, Portal Surge, Jackpot Spins, Enchanted Keys, and portal progression into one system without a decision.

Do not show chests as restoration rewards; later direction says restore rewards are mana and card packs.

Do not close completed rooms in a way that prevents ingredient replay.

Do not make weekly witch revisit punitive without explicit approval; the idea should create purpose/rewards, not frustration.

Do not move locked room-entry UI regions to fit background art.

Do not change realm names, room names, or room count inside generated assets without explicit instruction.

## 18. Open decisions checklist

| Decision needed | Why it matters | Dependency |
| --- | --- | --- |
| Final public title and realm naming convention. | Affects handoffs, app metadata, logo, and asset naming. | Brand lock. |
| Final Realm 1 / Realm 2 order. | Affects onboarding, progression tuning, locked asset order, and story. | Everbloom/Sunpetal ordering decision. |
| Exact room lists for Everbloom Sanctuary and Sunpetal Conservatory. | Needed for content tables, asset IDs, ingredient ownership, and quest routing. | Locked realm handoffs. |
| Special room format: blackout, multi-bingo, rotating special modes, or realm-specific modes. | Affects match engine, UI, rewards, and tutorial. | Gameplay screen lock. |
| Whether special room is always open after restoration. | Affects replay, weekly challenge, and ingredient access. | Realm replay rules. |
| Exact potion/spell count per realm. | Current reading is three regular-room spells feeding one ritual; needs formal lock. | Ingredient and drop rules. |
| Potion vs artifact vs relic terminology. | Affects story copy, UI labels, Player Den Relic Wall, and developer schema. | Narrative/system naming lock. |
| Portal progression model. | Need decide open-portal visual only vs interactive portal vs Threshold Board vs Portal Surge links. | World map and rewards. |
| Realm completion reward values. | Need mana amounts, card pack type/count, first-time-only rules, and repeat rewards. | Economy tuning. |
| Post-restoration replay drop rules. | Needed to avoid ingredient dead ends and economy exploits. | Ingredient drop tables. |
| Weekly witch challenge rules. | Need host character, cadence, target rooms, reward size, and whether anything breaks. | Event/quest system. |
| Ingredient gifting/requesting eligibility. | Affects Coven/Bazaar inventory schema and room replay value. | Trading/social lock. |

## 27. Open visual decisions

| Open item | Decision needed |
| --- | --- |
| Final logo | Need final chosen logo direction and app icon treatment. |
| Final project title lock | Bingo Magic Blast vs Bingo Magic Mayhem must be aligned in art files. |
| Player Den layout | Need lock on single hub room vs separate Apothecary/Library/Relic Wall screens. |
| Oracle Alley status | Need feature/name lock before producing final UI. |
| Madame Solange status | Need character/name lock before character art. |
| Crystal ball reveal | Need system placement: Oracle, daily bonus, mystery reward, or general reveal. |
| Marketplace scope | Need Coven-only vs global trading before final Bazaar UI. |
| Collection spread dimensions | Need final card count/page layout constraints after final collection size is locked. |
| Book of Shadows access model | Need free/premium/paid-track relationship before final UI hierarchy. |
| Portal Surge vs jackpot wheel | Need final distinction before animating portal/jackpot systems. |
| Daily/weekly paid track visuals | Need monetization scope lock before VIP/track-like visual treatment. |
| Power-up icon set | Need final named power-up list and generic category icon approval. |
| Rarity frame system | Need final rarity taxonomy including Rare, Extra Rare, Ancient, Gilded, Astral, Wild. |

## Cross-system open-decision index

| Area | Open decision |
|---|---|
| Product identity | Final public title: Bingo Magic Blast vs Bingo Magic Mayhem. |
| Economy | Whether mana replaces coins/energy or coexists with them. |
| Economy | Whether crystals are active, and what they buy. |
| Economy | Whether Charm Tokens, Stardust, and Oracle Dust exist. |
| Economy | Duplicate conversion values and thresholds. |
| Economy | Rare / Extra Rare / Ancient / Gilded duplicate behavior. |
| Economy | Whether set/market value is informational, redeemable, or marketplace-facing. |
| Collections | Final Grimoire card count, page count, duration, and rarity distribution. |
| Collections | Final Book of Shadows card count, duplicate behavior, rewards, and wild-card eligibility. |
| Realms | Final realm order and exact room list per realm. |
| Realms | Exact special-room mode: blackout, multi-bingo, ritual-only, or other. |
| Realms | Exact room unlock and realm restoration rules. |
| Realms | Exact portal system and whether Portal Surge survives. |
| Social | Trade/gift/request limits, cooldowns, eligibility, and scope. |
| Social | Coven Ritual MVP/Phase 2 status. |
| Marketplace | Bewitchment Bazaar exact scope and whether Oracle Alley is a sub-feature. |
| Oracle | Whether Madame Solange is official. |
| Rewards | Daily reward calendar, streak, daily spin, weekly track, paid track. |
| Player Den | MVP scope and mechanics for Apothecary, Library, Relic Wall. |
| Visual | Final logo, typography, icon set, power-up icon set, card frames, active bingo screen. |

# 04 — Economy, Rewards, and Scarcity

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

## Major system confidence

| System | Confidence | Codex handling |
|---|---|---|
| Mana bet per card | LOCKED UI RULE | Use in room-entry UI and match setup model. |
| Mana as restoration reward | HIGH | Use as reward category; broader relation to coins/energy open. |
| Coins | OPEN / LEGACY | Do not assume active unless approved. |
| Crystals | PROPOSED / LEGACY | Treat as premium-currency placeholder until confirmed. |
| Charm Tokens | OPEN | No confirmed rules found. |
| Stardust | OPEN | No confirmed rules found. |
| Oracle Dust | OPEN | No confirmed rules found. |
| Duplicate conversion | HIGH system need, OPEN values | Build conversion hooks, not fixed rates. |
| Grimoire rewards | HIGH | Support reward hooks; exact table open. |
| Book of Shadows rewards | HIGH / OPEN values | Support separate reward hooks. |
| Realm completion rewards | HIGH | Mana + card packs are strong current constraints. |
| Portal Surge rewards | ARCHIVE / CONFLICT | Keep separate from current jackpot wheel until reconciled. |
| Daily rewards | LOCKED SYSTEM AREA / OPEN values | Model retention system with configurable rewards. |
| Weekly reward track | PROPOSED / SOCIAL-EVENT | Keep configurable. |
| Paid upgrade track | OPEN | Do not implement without monetization approval. |
| Marketplace/set value ideas | PROPOSED | Do not treat as active currency until approved. |

## Complete economy extraction

Scope: inventory-level economy design based on currently available Project materials, later locked decisions, and explicitly discussed economy concepts.

## Document purpose and source guardrails

Purpose: Capture every known economy-facing rule, reward, conversion, scarcity gate, progression gate, payout type, event reward, collection reward, duplicate behavior, and monetization constraint discussed for Bingo Magic Blast.

Not a balance sheet: This document does not set final drop rates, pack odds, prices, redemption thresholds, SKU prices, or reward amounts unless those values were explicitly surfaced in available Project materials.

Later material overrides early architecture: The later locked room-entry direction uses mana bet per card, a visible jackpot wheel, ingredient progress, restore rewards, and four card-count options. Older coins/energy/buy-in language is preserved only where not contradicted or where it remains an open decision.

No invented values: Charm Tokens, Stardust, and Oracle Dust are included because the user requested those areas, but no confirmed rules for those resources were found in the available searchable Project files. They are therefore marked unconfirmed/unresolved.

Developer warning: Anything listed as proposed or unresolved must not be implemented as final economy logic without a follow-up lock.

## Economy state overview

| Economy area | Current status | Known use | Lock caveat |
| --- | --- | --- | --- |
| Mana / Coins | Partially confirmed / unresolved relationship | Mana is locked in later room-entry as bet per card; coins are older soft-currency language. | Do not assume coins and mana are separate or merged until explicitly locked. |
| Crystals | Known resource, final role open | Premium-lite/premium currency and possible collection reward. | Exact purchase/spend sinks not locked. |
| Card packs | Confirmed reward/acquisition source | Collection acquisition and realm/restore reward. | Pack types and odds not locked. |
| Power-ups | Confirmed inventory/reward class | Match boosts; active power-ups sit bottom-right in room-entry. | Specific power-up list and prices not locked. |
| Duplicates | Confirmed tracking/conversion need | Duplicate indicators, duplicate conversion feedback, sharing/trading logic. | Conversion formula and currency output not locked. |
| Wild cards / shards | Known collection support | Wildcard and wildcard shards support completion; Wild Ritual Calls support Coven Ritual. | Types, eligibility, and thresholds not locked. |
| Daily / weekly rewards | Confirmed system area | Daily rewards, weekly Coven Ritual, reward tracks discussed. | Exact ladders, tracks, and reset rules are open. |
| Marketplace / Bazaar | Confirmed system area; specific mechanics partly open | Bewitchment Bazaar, ingredient gifting/requesting, card/duplicate trading. | Market value formula and trade limits open. |
| Portal Surge / jackpot | Conflicted / needs lock | Current UI has jackpot wheel; older architecture has Portal Surge and Jackpot Spins. | Do not collapse these into one system without approval. |

## Confirmed economy rules

Fairness principle: Pricing should be fair and consistent for everyone. Progression scaling is allowed, but unpredictable player-by-player pricing should be avoided.

Remote tuning requirement: Rewards, drop rules, collection pages, shop offers, event timing, realm/room configurations, and balancing should be data-driven and remotely tunable.

Core payout categories: Known reward classes include mana/coins, crystals, power-ups, card packs, collection cards, Jackpot Spins/jackpot wheel rewards, Ritual Calls, Coven Orbs, realm progress, New-to-You cards, wildcards, and wildcard shards.

Room-entry economy UI: Later locked room-entry layout includes mana bet per card, card-count selection, jackpot wheel, restore reward, ingredient progress, and active power-ups.

Restore reward constraint: Realm/room restore rewards should be mana and card packs, not chests.

Manual sharing rule: Collection Assist / Coven sharing should be manual, not automatic.

Trading/gifting scope: Coven + Bazaar system scope includes ingredient gifting/requesting, card trading/duplicate trading, limits, cooldowns, fairness rules, and what can/cannot be traded.

Collection page structure: Known/current collection structure uses 10 cards per set/page.

Collection size caveat: 80 cards was later stated as final unless a layout restriction is uncovered, but it still needs to be tied explicitly to Grimoire, Book of Shadows, or both before final economy tuning.

Power-up inventory: Cabinet of Curiosities is the named storage location for power-ups.

## Proposed economy rules

Mana as primary play currency: Because the later UI says mana bet per card, the safest proposed model is that mana becomes the primary play/bet resource and older coins/energy language is either renamed, retired, or separated into secondary functions.

Marketplace value after set completion: Once a collection set is complete, show market value instead of the set reward; the player should not need to see both at once.

Duplicate conversion ladder: Regular duplicates, rare/extra rare duplicates, and Book of Shadows duplicates should not all convert at the same value; rarity and collection track should affect conversion output or redemption progress.

Redemption-first economy: Duplicate value should preferably build toward specific helpful redemptions instead of becoming an unlimited faucet of spendable currency.

Event-earned shop: Coven Emporium, if used, should spend earned Coven Orbs and feel like a weekly event shop, not a cash shop.

Oracle economy: Oracle Alley / Madame Solange should use a bounded chance/redeem system rather than direct guaranteed high-value wild-card sale, if the feature is later locked.

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

## Area-by-area economy design

### 1. Mana coins

Confirmed economy rules

Later locked room-entry layout includes mana bet per card.

Older architecture uses coins as standard soft currency and energy as play/session fuel.

Rewards Summary visual direction included coins, crystals, energy, cards, and Jackpot Spins.

Restore rewards were later constrained to mana and card packs, not chests.

Proposed economy rules

Treat mana as the likely primary play/bet resource unless the economy is explicitly split into mana, coins, and energy.

If coins remain, assign coins to store purchases, upgrades, or general reward value rather than card-entry betting.

If energy remains, make it session pacing rather than duplicate the function of mana.

Unresolved tuning values

Final name: mana, mana coins, coins, energy, or a two-resource model.

Starting balance, per-card costs, room scaling, bet tiers, loss/win payout curve.

Whether mana can be purchased directly with crystals or earned only through play/rewards.

Whether jackpot contributions scale with mana bet.

Risks / balance concerns

Having both mana and coins may confuse players if both behave like soft currency.

If mana is both entry cost and reward, inflation must be tightly controlled.

If energy remains as a separate gate, players may feel double-gated by both mana and energy.

Things Codex or a developer must not assume

Do not implement coins as final just because older handoff used coins.

Do not remove coins/energy automatically without a formal rename/retirement decision.

Do not assume mana cost scales linearly by card count.

### 2. Crystals

Confirmed economy rules

Crystals are named in older architecture as premium-lite or premium currency.

Crystals appear in Grimoire reward examples and Rewards Summary visual needs.

Basic shop/IAP framework is expected in early architecture.

Proposed economy rules

Use crystals as the safest premium currency candidate.

Crystals may purchase card packs, power-up bundles, cosmetic items, event conveniences, or limited shop items, but exact sinks are not locked.

Crystals should not be required to complete core free collection progression.

Unresolved tuning values

Free earn rate, paid exchange rate, maximum daily free crystals, shop prices.

Whether crystals can buy mana, wildcards, Oracle readings, or paid track upgrade.

Whether crystals are awarded in Book of Shadows and Grimoire page completion.

Risks / balance concerns

Premium currency can create pay-to-complete pressure if tied directly to rare cards or wildcards.

If crystals buy too many progression skips, Collection Assist and duplicate conversion lose value.

Things Codex or a developer must not assume

Do not assume crystals are hard premium only; source calls them premium-lite or premium.

Do not assume direct wildcard purchase with crystals.

Do not assume crystal packs or SKU sizes.

### 3. Charm tokens

Confirmed economy rules

No confirmed item found in the available Project source set. Treat as unresolved.

Proposed economy rules

Possible role, if retained: social/helper token, marketplace trading token, collection redemption token, or paid-track token.

Could function as a bounded redemption currency for duplicates or set-value conversion, but no source-confirmed rule was found.

Unresolved tuning values

Whether Charm Tokens exist at all.

Earn sources, sinks, caps, expiration, conversion ratios, and eligibility.

Whether Charm Tokens are tied to marketplace value or social help.

Risks / balance concerns

Adding Charm Tokens on top of mana, crystals, Stardust, Oracle Dust, and Coven Orbs risks severe currency clutter.

If Charm Tokens become a trade currency, abuse/farming controls will be needed.

Things Codex or a developer must not assume

Do not create Charm Token balances, stores, conversion tables, or reward grants until explicitly locked.

Do not treat Charm Tokens as confirmed replacement for coins, crystals, Stardust, or Coven Orbs.

### 4. Stardust

Confirmed economy rules

No confirmed item found in the available Project source set. Treat as unresolved.

Proposed economy rules

Possible role, if retained: duplicate-conversion dust for regular Grimoire cards.

Could be used toward redemption thresholds, wildcard shards, New-to-You chances, or market-value conversion, but this is proposed only.

Unresolved tuning values

Whether Stardust exists.

Regular duplicate-to-Stardust conversion rate.

Rare/extra rare duplicate handling.

Redemption thresholds and weekly caps.

Whether Stardust is Grimoire-only or universal.

Risks / balance concerns

A dust economy can become an infinite faucet if duplicates are common and spend sinks are weak.

If thresholds are too high, duplicate conversion feels meaningless; if too low, collections finish too quickly.

Things Codex or a developer must not assume

Do not assume all duplicates convert to Stardust.

Do not assume Book of Shadows duplicates use Stardust.

Do not implement Stardust redemption thresholds without final values.

### 5. Oracle Dust

Confirmed economy rules

No confirmed item found in the available Project source set. Treat as unresolved.

Proposed economy rules

Possible role, if Oracle Alley / Madame Solange is retained: resource earned from duplicates/events and spent on Oracle readings or wild-card chances.

Could provide a chance-based path to wild cards or specific missing-card help; must be bounded.

Unresolved tuning values

Whether Oracle Dust exists.

Reading cost.

Dust earn sources.

Wild-card odds.

Pity meter or guaranteed threshold.

Whether Oracle Dust expires or has weekly cap.

Risks / balance concerns

Chance-based Oracle outcomes can feel gambling-adjacent if not messaged carefully.

Oracle Dust should not undermine normal collection progression or make premium wilds feel mandatory.

Things Codex or a developer must not assume

Do not assume Oracle Dust is confirmed.

Do not assume Oracle readings produce guaranteed wildcards.

Do not build random paid Oracle mechanics without explicit monetization/legal review.

### 6. Regular card duplicates

Confirmed economy rules

Duplicate tracking is required.

Duplicate indicators are part of Grimoire UI needs.

Rewards Summary should show duplicate conversion feedback.

Collection Assist requires duplicate cards and share eligibility data.

Manual sharing and daily sharing limits are required in social/collection support.

Proposed economy rules

Regular duplicates should either convert into a bounded resource, feed a redemption meter, support sharing, or contribute to marketplace/set value.

Duplicate conversion should be visible immediately in rewards summary.

Duplicates should be surfaced in You Can Share and Team Needs flows where eligible.

Unresolved tuning values

Exact duplicate count needed before conversion.

Conversion resource: mana, Stardust, Charm Tokens, wildcard shards, market value, or other.

Share/trade eligibility and cooldowns.

Whether first duplicate is shareable before conversion.

Risks / balance concerns

Automatic conversion can conflict with manual sharing if duplicates disappear too quickly.

Holding duplicates for sharing can clutter inventory if not well managed.

Low conversion values can intensify duplicate frustration.

Things Codex or a developer must not assume

Do not auto-convert all duplicates if the social system needs duplicate inventory.

Do not assume duplicates are worthless.

Do not assume all duplicates are tradeable.

### 7. Rare / extra rare duplicate conversion

Confirmed economy rules

Rarity is an active collection concern; rare/extra rare distribution was discussed later.

Final rarity quantities are not fully surfaced, but rare and extra rare card classes are known concepts.

Duplicate conversion formula remains an open economy decision.

Proposed economy rules

Rare and extra rare duplicates should convert at higher value or provide stronger redemption progress than regular duplicates.

Rare duplicates may be trade-restricted, request-restricted, or eligible only for special conversion.

Extra rare duplicates may require special handling to avoid making rare completion too easy or too frustrating.

Unresolved tuning values

Rare-to-regular conversion multiplier.

Extra-rare-to-regular conversion multiplier.

Whether rare/extra rare duplicates can be gifted, traded, or only converted.

Whether conversion grants Stardust, Charm Tokens, wildcard shards, or market value.

Risks / balance concerns

If rare duplicates convert too generously, rare scarcity collapses.

If they convert too poorly, rare duplicate pulls feel punishing.

Trading rare duplicates can enable account farming unless gated.

Things Codex or a developer must not assume

Do not use the regular duplicate conversion rate for rare/extra rare cards by default.

Do not assume rare or extra rare cards are shareable.

Do not assume rare/extra rare conversion grants premium currency.

### 8. Book of Shadows duplicate behavior

Confirmed economy rules

Book of Shadows exists as a separate special/premium collection track.

Book of Shadows should feel rarer, more mysterious, more ornate, and premium without being sinister.

Duplicate tracking/protection is part of collection quality-of-life requirements, but Book of Shadows-specific duplicate rules are not locked.

Proposed economy rules

Book of Shadows duplicates should probably have stronger protection or higher conversion value than regular Grimoire duplicates because the track is premium/special.

Book of Shadows may need its own duplicate currency, Oracle Dust path, or elevated wildcard-shard progress.

If Book of Shadows is paid or time-limited, duplicate protection should be clearer and more generous.

Unresolved tuning values

Whether Book of Shadows duplicates convert to the same resource as Grimoire duplicates.

Whether Book of Shadows duplicates can be traded or gifted.

Whether Book of Shadows has duplicate pity / New-to-You protections.

Conversion rates by rarity.

Risks / balance concerns

Premium-track duplicates can create strong frustration if conversion feels weak.

If Book of Shadows duplicates feed regular economy too strongly, players may farm premium duplicates for base progression.

If tradeable, Book of Shadows cards may create unfair pressure or black-market behavior.

Things Codex or a developer must not assume

Do not assume Book of Shadows duplicates behave like Grimoire duplicates.

Do not assume Book of Shadows cards are tradeable.

Do not assume Book of Shadows completion is purchasable.

### 9. Wild card types

Confirmed economy rules

Wildcards and wildcard shards are known collection completion support items.

Wild Ritual Calls are separate Coven Ritual items that clear any open Ritual Mark.

New-to-You cards are known collection support rewards.

Prioritized Page system is known collection support.

Proposed economy rules

Separate wild systems by use case: collection wildcards, wildcard shards, Wild Ritual Calls, New-to-You rewards, and Prioritized Page boosts should not be collapsed into one item.

Wildcards may need rarity-specific types to preserve Ancient/Gilded/Book of Shadows scarcity.

Wildcard shards should build toward a selectable or constrained wildcard redemption.

Unresolved tuning values

Wild card categories and names.

Shard threshold per wildcard.

Rarity eligibility.

Set/page eligibility.

Expiration rules.

Whether wildcards can complete final card in a set.

Whether paid track grants wildcards.

Risks / balance concerns

Too many wildcard types may confuse players.

Universal wildcards can trivialize rare collection completion.

Overly restricted wildcards may feel deceptive.

Things Codex or a developer must not assume

Do not use Wild Ritual Call as a collection wildcard.

Do not assume one wildcard can fill all rarities and collections.

Do not assume shards have a threshold value.

### 10. Redemption thresholds

Confirmed economy rules

A recommended Coven Ritual duplicate meter exists: 5 duplicate/unusable Ritual Calls = 1 Wild Ritual Call.

Sigil structure exists in older architecture: 1 completed Ritual Board = 1 Sigil; 8 Sigils = 1 Sigil Set.

No final thresholds are found for card duplicate redemption, Stardust, Charm Tokens, Oracle Dust, or wildcard shards.

Proposed economy rules

Use visible meters for redemption thresholds to reduce frustration.

Keep duplicate redemptions bounded and paced by collection scarcity.

Different tracks may need different thresholds: regular Grimoire, rare/extra rare, Book of Shadows, Oracle, Coven Ritual.

Unresolved tuning values

Duplicate-to-wildcard shard ratio.

Wildcard shard threshold.

Stardust redemption costs.

Charm Token redemption costs.

Oracle Dust reading threshold.

Pity thresholds for New-to-You rewards.

Weekly caps and reset rules.

Risks / balance concerns

Hidden thresholds can feel manipulative.

Too many meters can overwhelm players.

Uncapped redemption can break scarcity and shorten collection lifespan.

Things Codex or a developer must not assume

Do not reuse the 5 Ritual Call meter for card duplicates without approval.

Do not assume 8 Sigils applies to any non-Coven system.

Do not implement thresholds as hidden-only.

### 11. Grimoire rewards

Confirmed economy rules

Grimoire is the main collection system.

Each set/page uses 10 cards.

Older architecture lists page rewards in the 600-2500 range and full completion target of 250,000 + power-ups + crystals + wildcards.

Later discussion says 80 cards is final unless layout restriction is uncovered; final applicability must be confirmed.

Grimoire UI requires page rewards/progress, missing cards, duplicate indicators, Prioritized Page marker, and completion rewards.

Proposed economy rules

Grimoire should reward regular play with mana/coins, power-ups, crystals, card packs, wildcards/shards, and/or collection support.

Set completion rewards may be hidden/replaced by market value after set completion.

Grimoire rewards should encourage continued play without making duplicates feel like dead pulls.

Unresolved tuning values

Final total card count and number of sets.

Page reward amounts.

Full completion reward amount.

Rarity reward scaling by set.

Whether rewards are mana or coins.

Whether market value replaces visible completion reward.

Risks / balance concerns

Older 25-set/90-day numbers conflict with later 80-card direction.

Large full-completion payouts can create inflation if collections are completed frequently.

If market value replaces rewards visually, players may miss what they already earned.

Things Codex or a developer must not assume

Do not implement 90-day / 25-set structure unless re-confirmed.

Do not assume older 250,000 full completion target is final.

Do not assume completion reward chest, because later restore rewards reject chests.

### 12. Book of Shadows rewards

Confirmed economy rules

Book of Shadows exists as separate premium/special collection.

Older architecture lists 30 days, 8 sets/pages, 10 cards/page, page rewards 1200-2400, and full completion around 46,000 + premium extras.

Later discussions continued Book of Shadows volume/potion-name work, but final count/reward table is not fully locked.

Proposed economy rules

Book of Shadows rewards should feel more premium than Grimoire rewards, with stronger protection, rarer items, or exclusive cosmetics/progression bonuses.

Rewards could include mana/coins, crystals, special card packs, wildcard shards, Oracle Dust, or premium extras if approved.

Unresolved tuning values

Final duration.

Number of books/volumes.

Page rewards.

Full-completion reward.

Duplicate conversion behavior.

Whether rewards include premium currency or exclusive wildcards.

Risks / balance concerns

Premium-track rewards can become pay-to-win if they grant too much completion power.

If Book of Shadows rewards are too small, the premium/special framing feels hollow.

Older reward numbers may not fit the later 80-card or 10-card set direction.

Things Codex or a developer must not assume

Do not implement older 30-day / 8-set numbers as final without confirmation.

Do not assume Book of Shadows is paid-only.

Do not assume Book of Shadows rewards can bypass Grimoire scarcity.

### 13. Realm completion rewards

Confirmed economy rules

Realm/room restoration rewards should be mana and card packs, not chests.

Room-entry UI has restore reward placement at the right side of the ingredient strip.

Realm restoration is tied to completing potions/spells and casting them in the special room.

A restored realm can have a final open-portal state.

Proposed economy rules

Realm completion rewards should scale by realm difficulty and reinforce collection progress through card packs.

Special-room restoration should provide a larger payout than regular room/potion completion.

Revisit/remake rewards, if allowed, should be smaller than first-time restoration rewards.

Unresolved tuning values

Mana amount per potion/room/realm.

Card pack type and count.

Whether completion grants crystals, power-ups, cosmetics, relics, or portal unlock rewards.

Repeat-completion reward rules.

Whether higher bet affects restoration reward.

Risks / balance concerns

Too-large realm payouts can inflate mana and pack supply.

If replay rewards are too generous, players may farm old realms instead of progressing.

If rewards are too small, realm restoration loses satisfaction.

Things Codex or a developer must not assume

Do not use chests as restore rewards.

Do not assume room closure after completion; later direction keeps rooms accessible.

Do not assume exact reward values.

### 14. Portal Surge rewards

Confirmed economy rules

Older architecture defines Portal Surge as rare jackpot-style feature triggered by Enchanted Key, with portal layers Fae Passage, Elemental Tunnel, Astral Gate.

Older reward pots: Rune = small pot, Astral = bonus pot, Eternal = unlimited play at current bet for 1 hour, Ancient = top pot.

Later locked room-entry UI uses jackpot wheel placement and visible jackpot value.

Developer note says Portal Surge should be visually/mechanically distinct from a standard prize wheel.

Proposed economy rules

Portal Surge may survive as a separate rare escalation from the standard jackpot wheel.

Jackpot wheel could be the ordinary visible prize mechanic while Portal Surge is a rarer portal-layer event.

Eternal pot should be carefully capped if retained.

Unresolved tuning values

Trigger odds.

Enchanted Key drop chance.

Layer progression odds.

Pot payout values.

Whether jackpot wheel and Portal Surge share jackpot pool.

Whether Eternal reward remains 1 hour or changes.

Bet scaling.

Risks / balance concerns

Jackpot features can push the game toward casino tone if too realistic or cash-like.

Unlimited play rewards can break economy if entry costs are normally meaningful.

Layered jackpot systems can be confusing if Portal Surge and wheelspin both coexist.

Things Codex or a developer must not assume

Do not assume Portal Surge is final just because it exists in older handoff.

Do not assume jackpot wheel equals Portal Surge.

Do not implement Enchanted Keys without confirming they still exist.

### 15. Daily rewards

Confirmed economy rules

Daily rewards are part of MVP/retention systems.

Council Standing benefit ideas include extra daily energy and larger login bonuses.

Rewards/Retention Utility Systems locked handoff exists as a packaged system area, but detailed values are not visible in available snippets.

Proposed economy rules

Daily rewards should include light economy support such as mana/coins, power-ups, card packs, collection support, crystals, or wildcard shards.

Daily reward pacing should help free players keep playing without replacing collection/challenge rewards.

Council Standing may increase daily reward value later.

Unresolved tuning values

Calendar length.

Streak rules.

Missed-day grace.

Reward table by day.

Whether rewards include crystals/wildcards.

Council Standing multipliers.

New-player vs mature-player tuning.

Risks / balance concerns

Too-generous daily rewards can reduce need to play rooms.

Too-weak daily rewards can hurt retention.

Streak loss can feel punishing.

Things Codex or a developer must not assume

Do not assume daily reward values.

Do not assume daily rewards include premium currency.

Do not assume Council Standing benefits are MVP.

### 16. Daily spin

Confirmed economy rules

Jackpot Spins exist in older architecture as bonus reward mechanic.

Daily spin is requested as an area, but no detailed final daily-spin rule is surfaced in available files.

Current room-entry UI includes a jackpot wheel, but daily spin and jackpot wheel may not be the same feature.

Proposed economy rules

Daily spin could serve as a retention utility reward separate from jackpot wheel.

Daily spin prize table could include mana/coins, power-ups, card pack fragments, crystals, wildcard shards, or event resources.

Daily spin should be visually magical and not casino-like.

Unresolved tuning values

Availability cadence.

Free daily spin count.

Ad-supported or paid extra spins, if any.

Prize table.

Jackpot eligibility.

Whether spin is connected to Jackpot Spins resource.

Risks / balance concerns

Spin mechanics can feel casino-like if presented as gambling or if real-money purchase is attached.

If prize variance is too high, daily reward balance becomes unpredictable.

Things Codex or a developer must not assume

Do not assume daily spin is the same as jackpot wheel.

Do not assume extra spins can be sold.

Do not assume prize odds.

### 17. Weekly reward track

Confirmed economy rules

Coven Ritual is a weekly team event in older architecture.

Coven Orbs are weekly Coven Ritual currency.

Coven Emporium is weekly event shop; active free players should earn enough Coven Orbs weekly to buy 1-2 meaningful items.

Sigil Sets award Coven Points and event ends with Ritual Summary.

Proposed economy rules

Weekly reward track may sit alongside or replace Coven Ritual rewards.

Free weekly track should reward engagement with mana/coins, card packs, power-ups, Coven Orbs, wildcard shards, and/or cosmetics.

Coven contribution rewards should reinforce helping, not just spending.

Unresolved tuning values

Track length.

Point source.

Milestones.

Free reward values.

Team vs personal reward split.

Reset day/time.

Catch-up mechanics.

Risks / balance concerns

Weekly FOMO can become stressful if completion requires heavy play.

Team events can punish casual Coven members if rewards are too dependent on others.

Too much weekly value can reduce daily play balance.

Things Codex or a developer must not assume

Do not assume Coven Ritual is MVP; older handoff places full Coven Ritual in Phase 2.

Do not assume weekly track and Coven Ritual are the same system.

Do not assume paid track exists.

### 18. Paid upgrade track

Confirmed economy rules

Basic shop/IAP framework is expected.

Monetization should not be pushy or casino-like.

Specific paid upgrade track rules are not locked in available files.

Proposed economy rules

A paid upgrade track, if used, should add convenience, cosmetics, and bonus rewards without blocking free collection completion.

Paid track could add extra card packs, cosmetics, power-ups, crystals, wildcard shards, or Oracle attempts if approved.

Paid upgrade should be optional and transparent.

Unresolved tuning values

Price.

Duration.

Reward ladder.

Whether it is tied to weekly track, Book of Shadows, or events.

Whether retroactive claiming is allowed.

Whether paid track grants exclusive collection completion power.

Risks / balance concerns

Paid completion shortcuts risk undermining collection fairness.

Paid wildcards can feel predatory if base drop rates are harsh.

Subscription/pass complexity can overwhelm early MVP.

Things Codex or a developer must not assume

Do not implement paid upgrade track as final.

Do not assume paid track includes wildcards or rare cards.

Do not assume Book of Shadows equals paid upgrade track.

### 19. Social/freebie rewards

Confirmed economy rules

Covens exist as team/social system.

Collection Assist requires manual sharing, share limits, needs/extras visibility, and daily sharing limits.

Coven contribution rewards and helper/contribution rewards are known planned concepts.

Coven Gift Bundle appears as Coven Emporium item category example.

Relic Wall may display Coven contribution honors.

Proposed economy rules

Social rewards should encourage helping teammates: small mana/coins, Coven Orbs, contribution points, badges, or weekly helper rewards.

Freebie rewards should be capped daily to prevent farming.

Receiving help should feel generous without replacing core acquisition.

Unresolved tuning values

Daily gift limit.

Request cooldown.

Helper reward amount.

Coven-only vs friends/global.

Eligibility by rarity.

Whether ingredient gifts and card gifts share limits.

Abuse prevention and anti-alt rules.

Risks / balance concerns

Uncapped free gifts can break collection scarcity.

Too many limits can make social help feel useless.

Helper rewards can be farmed by alt accounts if not constrained.

Things Codex or a developer must not assume

Do not assume sharing is automatic.

Do not assume all cards/ingredients are giftable.

Do not assume helper rewards grant premium currency.

### 20. Marketplace value / set value ideas

Confirmed economy rules

User proposed replacing set rewards with market value once the set is complete.

User preferred seeing one or the other, not both.

Coven + Bazaar system scope includes card trading/duplicate trading and marketplace/event timing.

Bewitchment Bazaar is the preferred/locked marketplace name.

Proposed economy rules

Completed-set market value could become a display value, trade valuation, duplicate conversion reference, or marketplace pricing anchor.

Market value could vary by rarity, completeness, event timing, demand, or remaining collection time, but this is not locked.

If used, set value should be explained simply and not become noisy UI.

Unresolved tuning values

Whether set value is spendable, informational, or trade-only.

Formula factors.

How often value updates.

Whether player can cash out value.

Whether value applies to partial sets, completed sets, duplicate cards, or ingredients.

Whether market value interacts with Charm Tokens/Stardust/Oracle Dust.

Risks / balance concerns

Dynamic market value can be hard to balance and explain.

If values fluctuate too much, players may feel manipulated.

Cash-out behavior can create inflation or farming incentives.

Marketplace trading requires fraud and collusion controls.

Things Codex or a developer must not assume

Do not implement dynamic market pricing without explicit formula approval.

Do not assume completed sets can be sold.

Do not assume market value replaces actual earned set rewards retroactively.

## Implementation notes for economy data model

Resource ledger: Every currency/resource should have a ledger event type, source, sink, amount, timestamp, player state, and experiment/config version.

Reward source taxonomy: Classify every grant as match payout, daily reward, spin reward, collection reward, realm completion reward, social reward, event reward, shop purchase, duplicate conversion, compensation, or admin grant.

Config-driven tables: Mana costs, drop odds, pack contents, duplicate conversion, wildcard eligibility, daily rewards, shop prices, and event rewards should be remote-configurable.

Anti-abuse: Trading, gifting, requests, market value, and social freebies need daily caps, cooldowns, account-age checks, and suspicious-pattern telemetry.

Analytics: Track duplicate frustration, time-to-set-complete, free-player completion rate, currency inflation, sink/source ratio, paid conversion, and social-gift utilization.

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

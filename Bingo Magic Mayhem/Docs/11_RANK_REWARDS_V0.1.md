# Rank Rewards v0.1

Last updated: 2026-07-08

Status: working draft for review. Do not hard-code these values until the user explicitly promotes them to locked economy data.

Important correction: Rank is based on Aura Strength, not Level alone. The older level-band table is now a visual scaffold for rank order only, not final rank thresholds.

This document gives account rank progression one clean review surface. It should be read with `LOCKED_DECISIONS.md`, `09_OPEN_DECISIONS.md`, and `10_VERIFIED_PROJECT_STATE.md`.

## Purpose

Rank rewards should make long-term players feel recognized and gently supported without creating an unfair gameplay advantage.

## Progression Model

Use three separate concepts:

- XP: gameplay-earned progress toward Level.
- Level: gameplay progression and one input into Aura Strength.
- Aura Strength: broader account-strength score that determines Rank.

Aura Strength may include:

- Level and XP history.
- Realm, room, potion, restoration, and collection progress.
- Healthy social/help participation.
- Small capped purchase contribution.

Purchases may contribute to Aura, but only as a capped support signal. A player must not be able to pay their way up the Rank ladder. Purchases cannot independently trigger rank advancement, bypass gameplay milestones, or become the dominant Aura source.

The intended shape:

- Early ranks: identity unlocks and small comfort.
- Mid ranks: modest convenience and social capacity.
- Late ranks: larger but capped daily comfort and stronger prestige.
- Very late ranks: status, cosmetics, and celebration more than power.

## Non-Negotiable Guardrails

Rank benefits must not improve:

- Bingo odds.
- Jackpot odds.
- Jackpot values.
- Rare, Gilded, or Ancient card odds.
- Ingredient drop odds.
- Trade value.
- Duplicate market value.
- Coven or event competitive scoring.
- Room or realm progression requirements.

Rank benefits should stay readable. A player should understand why ranking up matters without needing to memorize dozens of hidden modifiers.

## Benefit Lanes

Rank progression uses four lanes:

1. Identity: avatars, frames, daubs, badges, and titles.
2. Daily Comfort: small capped boosts to safe daily/account rewards.
3. Social Help Capacity: more ingredient help capacity and later card gift/trade capacity.
4. Rank-Up Chest: a one-time celebration reward when entering a new rank band.

## Comfort Bonus Scope

If approved, the comfort bonus may apply to:

- Daily Bonus mana.
- Daily Spin common mana.
- Mana Cauldron refill/capacity.
- Level-up currency rewards.
- Enchanted Trail free-path currency, if/when built.

The comfort bonus should not apply to cards, rarity odds, jackpot outcomes, ingredient drops, competitive scoring, or progression requirements.

## Rank Reward Scale Draft

The threshold column is intentionally labeled as Aura Band TBD. Earlier level ranges are not final rank thresholds.

| Aura Band | Rank | Comfort Bonus | Ingredient Sends / Day | Card Gifts / Trades / Day | Rank-Up Chest Tier | Identity Unlock Direction |
|---|---|---:|---:|---:|---|---|
| TBD | Novice | 0% | 3 | 0 | None | Starter profile identity |
| TBD | Apprentice | 2% | 4 | 0 | Small | Basic frame |
| TBD | Spellbinder | 4% | 5 | 1 | Small+ | First daub unlock |
| TBD | Mage | 5% | 6 | 1 | Medium | Rank badge or title |
| TBD | Thaumaturge | 8% | 7 | 1 | Medium+ | Avatar option |
| TBD | Mystic | 10% | 8 | 2 | Strong | Premium frame |
| TBD | Enchanter | 12% | 9 | 2 | Strong | Animated daub |
| TBD | Wizard | 15% | 10 | 2 | Very Strong | Prestige frame |
| TBD | Spellmaster | 18% | 11 | 3 | Very Strong | Advanced daub |
| TBD | Archmage | 20% | 12 | 3 | Prestige | Elite title/frame |
| TBD | Grand Archmage | 22% | 13 | 3 | Prestige+ | Rare avatar/robe |
| TBD | Paragon | 25% | 14 | 4 | Premium | Paragon visual set |
| TBD | Ascendant | 30% | 15 | 4 | Major | Ascendant visual set |
| TBD | Sorcerer Supreme | 35% | 16 | 5 | Supreme | Exclusive treatment |

## Aura Purchase Guardrail

Purchases can be acknowledged by Aura but must remain small and capped.

Working guardrail:

- Purchases may contribute no more than a small minority of Aura progress within a rank band.
- A purchase contribution should have diminishing returns after a low cap.
- No direct rank-up purchase.
- No purchase-only rank.
- No purchase contribution that bypasses gameplay, collection, realm, potion, or restoration milestones.
- Purchase contribution should never improve gameplay odds, jackpot odds/values, card rarity odds, ingredient drops, trade value, or competitive scoring.

## Social Capacity Rules

Ingredient sends per day are the amount of help a player can give to others.

The Coven ingredient wish-list request amount remains separate:

- A player can request up to 10 total ingredient quantity across selected realm ingredients.
- The request list refreshes every 48 hours.
- Rank should not increase how much a player can request unless a later decision explicitly changes that.

Card gifting/trading belongs in Library/Grimoire, not Coven. The card capacity column is a future Library/Grimoire social limit, not a Coven member-profile rule.

## Rank-Up Chest Direction

Chest tiers are labels for relative strength, not final contents.

Allowed chest contents should stay narrow:

- General power-ups.
- Clairvoyance time.
- Stars, but not frequently.

Rank-up chests should not become a broad reward bundle. They should avoid routine mana, crystal, card pack, album card, ingredient, jackpot, or progression-rule rewards unless a later decision explicitly reopens that scope.

Chest contents still need exact quantities, star frequency, tier scaling, and economy validation before implementation.

## Still Open

- Exact rank-up chest power-up quantities, Clairvoyance durations, and star frequency.
- Exact cosmetic unlock names and art assets.
- Final Aura formula and source weights.
- Final Aura thresholds per rank.
- Exact purchase Aura cap and diminishing-return rule.
- Whether card gifts and card trades share one daily limit or have separate limits.
- Whether card gift/trade limits apply to sent gifts only, received gifts only, or both.
- Whether the comfort bonus rounds up, rounds down, or uses reward-table-specific rounding.
- Whether the working draft should be promoted to locked economy data.

## Implementation Notes

- Keep the Aura/rank table data-driven or remote-configurable.
- Do not bury rank bonuses inside individual reward grant helpers.
- Reward grant helpers should receive explicit context so bonuses only apply to approved reward sources.
- Inbox gifts should not auto-apply rank comfort bonuses unless the underlying reward source is explicitly eligible.
- UI should describe benefits plainly by rank instead of exposing hidden math everywhere.

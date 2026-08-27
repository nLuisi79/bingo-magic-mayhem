# Bingo Magic Mayhem Game Systems Roadmap v0.1

## Purpose

This document tracks the major game systems beyond the active bingo-room prototype. The goal is to keep the product shape visible while we continue building one stable milestone at a time.

## Current Implemented Core

- Realm map shell for Sunpetal Conservatory.
- Room home with card count, mana selection, restore progress, jackpot pot, and prototype settings.
- Standard bingo gameplay with auto-calls, manual daubing, auto-claim, room bingo pool, and end countdown.
- Special room mode with blackout prototype behavior.
- Jackpot wheelspin earning, animated spin, stacked wheel rewards, and collect behavior.
- Mana, crystals, XP, player levels, pending wheelspins, room jackpot pots, power-up inventory, and Realm ingredients.
- Gameplay power-up bank with random offerings, charge cadence, crystal fallback, and Auto toggle.
- Cell rewards for cards, crystals, orbs, Pandora Sigil, Clairvoyance, and gameplay sigils.
- First Realm rooms, potions, ingredient requirements, and room restoration unlock flow.

## Near-Term Milestone: Meta Shell + Player Profile + Den Inventory

This should be the next major milestone because it creates a home for systems that should not live inside the bingo room.

- Add a top-level shell/navigation layer.
- Add placeholder destinations for Realm Map, Player's Den, Bewitchment Bazaar, Albums, Enchanted Trail, Covens, and Settings.
- Create a single profile snapshot that summarizes currencies, XP, power-ups, cards, ingredients, room progress, and pending rewards.
- Build Player's Den v0.1 as the inventory home for power-ups, cards, Pandora chests, Clairvoyance stock, album cards, and future gifts.
- Keep gameplay unchanged while the meta shell is introduced.

## System Map

### Player's Den

Status: Not implemented.

Primary role:
- Player home and inventory hub.
- Shows currencies, XP/level, active timed boosts, power-up stock, card inventory, chests/Pandora, album progress, and claimed reward history.

First v0.1 scope:
- Read-only inventory screen.
- Activate Clairvoyance from inventory.
- See gameplay sigil stock by type.
- See cards, club orbs, crystals, and pending wheelspins.

Later:
- Cosmetic room personalization.
- Witch avatar/identity.
- Collectibles display.
- Gift inbox.

### Bewitchment Bazaar

Status: Not implemented.

Primary role:
- Shop for mana, crystals, power-ups, timed boosts, card packs, event passes, and limited bundles.

First v0.1 scope:
- Placeholder shop with non-purchase prototype buttons.
- Define product categories and currencies.

Later:
- Real purchase integration.
- Offer rotation.
- Discount bundles.
- Purchase receipts and inbox delivery.

### Daily and Hourly Rewards

Status: Not implemented.

Primary role:
- Return incentives and light resource drip.

First v0.1 scope:
- Daily claim timer.
- Hourly claim timer.
- Rewards can include mana, crystals, Clairvoyance minutes, Pandora Sigil, power-ups, and card packs.

Design note:
- Realm ingredients should primarily come from Realm gameplay, not generic login rewards, unless a special event explicitly does that.

### Enchanted Trail

Status: Not implemented.

Primary role:
- Task/quest path that nudges play variety and gives medium-term goals.

First v0.1 scope:
- Simple task list with progress bars.
- Example tasks: daub X numbers, play X rounds, restore a room, earn X bingos, open Pandora, complete a blackout.

Later:
- Branching trail boards.
- Seasonal/event trail variants.
- Premium reward lane if desired.

### Covens

Status: Not implemented.

Primary role:
- Social group/guild system.

First v0.1 scope:
- Placeholder Coven screen.
- Define membership, basic chat/inbox assumptions, group goals, gifting, and group rewards.

Later:
- Coven events.
- Coven leaderboards.
- Shared restoration goals.
- Coven orb economy.

### Friends and Gifting

Status: Not implemented.

Primary role:
- Social retention and ingredient help.

First v0.1 scope:
- Define giftable categories: ingredients, limited power-up gifts, cards, and daily help.
- Define anti-abuse limits before implementation.

Open decisions:
- Whether key ingredients can be gifted.
- Whether album cards can be traded directly or only through a trade market.
- Whether gifts require mutual friendship or Coven membership.

### Album Collection and Trading

Status: Not implemented.

Primary role:
- Long-term collection system using regular, gilded, ancient, and special cards.

First v0.1 scope:
- Album collection data model.
- Card pack opening placeholder.
- Card counts by rarity.
- Duplicate tracking.

Later:
- Trading.
- Album set completion rewards.
- Limited-time collections.
- Special-card purchase system.

### Rank Progression and Leaderboards

Status: Not implemented.

Primary role:
- Competitive/progression layer separate from XP level.

First v0.1 scope:
- Define rank points and sources.
- Decide whether ranking is weekly, seasonal, room-based, or Coven-based.

Later:
- Leaderboards.
- Rank rewards.
- Seasonal reset rules.

### Mail, Inbox, and Claims

Status: Not implemented.

Primary role:
- Safe delivery and collection for gifts, purchases, compensation, event rewards, and system messages.

First v0.1 scope:
- Placeholder inbox model.
- Claim button behavior.

Later:
- Message types.
- Expiration timers.
- Claim-all behavior.

### Tutorial and Onboarding

Status: Not implemented.

Primary role:
- Teach card selection, daubing, auto-calls, Clairvoyance, power-up bank, bingos, jackpot wheelspins, room restoration, and the Den.

First v0.1 scope:
- Scripted first round prompts.
- First reward summary explanation.
- First Den visit explanation.

### Events and Special Rooms

Status: Partially implemented via blackout room mode.

Primary role:
- Rotating variants and short-term engagement.

First v0.1 scope:
- Keep only two room categories: Standard and Special.
- Room 4 is currently the special blackout prototype.
- Define room metadata so future special rooms can rotate without rewriting core rules.

Later:
- Event modifiers.
- Special ingredient/reward tables.
- Limited-time rooms.

### Backend/Profile Persistence

Status: Prototype uses PlayerPrefs.

Primary role:
- Eventually move from local prototype storage to structured profile data and server-controlled tuning.

First v0.1 scope:
- Add a local profile snapshot class.
- Separate profile concepts from gameplay controllers.

Later:
- JSON save format.
- Remote config tables.
- Server profile sync.

## Recommended Build Order

1. Player profile snapshot and meta shell navigation.
2. Player's Den inventory v0.1.
3. Economy tuning tables for rewards, ingredients, power-ups, and jackpot odds.
4. Daily/hourly rewards.
5. Album collection and card-pack opening.
6. Bewitchment Bazaar placeholder with product categories.
7. Enchanted Trail task system.
8. Friends/gifting and Coven shell.
9. Mail/inbox.
10. Onboarding/tutorial.

## Open Questions

- What is the final top-level navigation structure?
- Should Player's Den be a true home screen or one tab in a broader home hub?
- Which reward systems can grant Realm ingredients, and which should never do so?
- Are album card trades direct player-to-player, Coven-only, or market/listing based?
- Do Covens unlock before or after the first Realm is restored?
- What is the earliest point at which the player sees the Bazaar?
- Which systems need notification badges on the header?

# Locked Decisions

Last updated: 2026-07-07

These decisions should not be contradicted without explicit user approval.

## Development Direction

- Unity is the chosen implementation/prototype engine.
- Cocos work is useful context, but active forward development is in Unity.
- Prototype visuals can stay rough; rules, systems, and flow integrity matter more right now.

## Core Game Identity

- Bingo Magic Mayhem is a magical bingo adventure, not a casino product.
- The player is restoring realms/rooms by collecting ingredients and completing potions.
- Bingo gameplay, album collection, covens, daily systems, and reward loops are connected through the player's profile.

## Navigation Model

- Player's Den is the account/home hub.
- World Map is the zoomed-out all-realm view.
- Realm Map is the room selection view for one realm.
- Room Home is where the player chooses card count and mana bet.
- Gameplay is the active bingo round.
- Desired navigation:
  - World Map: enter an unlocked realm or return to Den.
  - Realm Map: enter an unlocked room or return to World Map.
  - Room Home/Game: back only, not Den and Map buttons together.

## Realm and Room Progression

- A fully reset new player should start with only Realm 1 Room 1 unlocked.
- Completing/restoring a room unlocks the next room in that realm.
- Completing the final room in a realm unlocks the first room of the next realm.
- The first room of every realm is not globally unlocked from the start.
- A room that has already been restored can be entered again.
- Room 4 of Realm 1 is currently the special blackout room.
- Standard rooms and special rooms are the only broad room categories for now.

## Standard Bingo Rules

- Standard rooms use standard bingo patterns: horizontal, vertical, diagonal.
- Standard rounds do not end after the first bingo.
- Standard rounds do not end just because a jackpot state is reached.
- Rounds can end from room bingo pool exhaustion, ball limit, room objective, or all active human cards reaching max reward state.
- A bingo/blackout should not be credited until the needed square has actually been daubed.
- Final called ball should include a visible countdown so the user has a chance to daub it.

## Blackout Rules

- Blackout is special-room gameplay, not a standard room rule.
- A blackout should require the user to daub the required final square; the system should not assume it from called numbers alone.
- Blackout card count should not be forced to 4 cards. Four cards was an example, not a locked requirement.

## Jackpot Wheelspin

- Jackpot wheelspin is a post-round reward moment, not a gameplay setup screen.
- A card reaching 5/5 bingos earns exactly one jackpot wheelspin.
- Multiple earned spins are spun one at a time, then collected once.
- Wheelspin rewards stack before collection.
- Wheel is mana-only for now.
- The wheel's center button says SPIN while spins remain and COLLECT after all spins are complete.
- Standard wheel results do not reset the room jackpot pot.
- Jackpot, Epic, and Legendary wheel outcomes reset that room's pot to its minimum.
- Room pot values scale the whole wheel.
- For Gilded Azalea Arboretum, the minimum pot is 250 Mana.

## Power-Up System

- During gameplay, players do not choose specific power-ups from inventory.
- The system selects a random eligible power-up into the bank.
- Bank charge cadence:
  - 1 card: ready after 1 valid daub.
  - 2 cards: ready after 2 valid daubs.
  - 4 cards: ready after 4 valid daubs.
  - 6 cards: ready after 6 valid daubs.
- Auto starts off for new players and is remembered after toggled.
- Auto on: bank fires when charged.
- Auto off: bank waits for player tap when charged.
- If selected power-up stock is 0, prototype purchase cost is 10 crystals.
- Wild Sigil must be extremely rare, roughly once every 10-20 rounds, and only appears for 2+ card play.

## Power-Up Definitions

- Single Sigil: one random square on each active card.
- Multi Sigil: two random squares on each active card.
- Arcane Spark: drops and must be daubed; later should feel like lightning across active cards.
- Fortune Sigil: drops and must be daubed; applies x2 winnings to active cards.
- Wild Sigil: premium/rare choose-any-square behavior.
- Presto Sigil: drops and must be daubed; counts that cell as an extra bingo without changing other squares.
- Pandora Sigil is the only chest-style gameplay sigil.
- Board-earned Pandora Sigil goes to inventory for later.
- Inventory-loaded Pandora Sigil opens during gameplay and grants random gameplay power-ups.

## Clairvoyance

- Clairvoyance is earned, purchased, or granted. It is not standard play.
- Clairvoyance is represented by a crystal ball.
- Clairvoyance can be awarded in timed increments such as 15, 30, or 60 minutes.
- When active, it should show in the room/game side rail with remaining time.
- After a called ball, matching unmarked numbers flash after a delay.
- Flashing should affect number text, not the whole cell background.
- Flashing continues until the player daubs the number or the round ends.
- Clairvoyance-assisted daubs should award less XP than unaided daubs.

## Ingredients and Potion Restoration

- Ingredients are room-specific.
- Ingredients are primarily earned through realm gameplay, not generic login rewards.
- A card must earn at least one bingo to award ingredients.
- Base ingredient model: 1 bingo = 1 ingredient.
- Bet multipliers:
  - Past 50% of possible bet: 2x.
  - Past 75% of possible bet: 3x.
  - Highest bet: 4x.
- Ingredient progression should take weeks in real gameplay, not minutes.
- No ingredient count should fall below 3 in tuned data.
- Rooms should have 5-7 ingredient types.
- Standard rooms should mostly have common ingredients, with some uncommon and 1-2 rare/key ingredients.
- Special rooms should have fewer common ingredients and more uncommon/key ingredients.

## Player Starting State

- A new player should not begin with ingredients.
- A new player should begin with enough mana, crystals, and starter power-ups to start playing.
- Crystals should start around 50.

## Player's Den and Inventory Routing

- Player's Den is the account hub.
- Cabinet of Curiosities stores gameplay sigils, timed boosts, and Pandora Sigil.
- Cards belong in Library/Grimoire, not Cabinet.
- Club orbs belong in Covens, not Cabinet.
- Top-right power-up count should remain a general count of all power-ups.
- Clairvoyance remaining time belongs in room/game context, not as the global top-right power-up count.

## Library, Grimoire, and Book of Shadows

- Grimoire is a free collection album.
- Grimoire is a 90-day game-wide event, not an individual user timer.
- Grimoire includes regular, gilded, and ancient cards according to spreadsheet/star-system data.
- Book of Shadows is premium/purchased.
- Book of Shadows contains only regular cards.
- Book of Shadows has three 30-day sets inside the same 90-day Grimoire event.
- Timers for Grimoire and Book of Shadows begin simultaneously.
- Potion completions reward mana, crystals, and power-ups.
- Full album completion rewards much larger mana, crystals, and power-ups.
- Grimoire duplicate cards convert into Joker Wild cards at completion/reset.
- Card wins should reveal what card was added.
- Newly won cards should be marked NEW in the album until viewed.
- Card gifting and card trading belong in the Library/Grimoire collection area, not the Coven member-profile wish list.
- Library/Grimoire is where players should manage missing regular cards, duplicate regular cards, eligible card gifts, and eventual card trades.
- Sent card gifts can still use Inbox as the recipient's delivery/claim layer.

## Coven Rules

- Cards do not display as inventory inside Coven.
- Club orbs belong to Coven systems.
- Covens cap at 50 members.
- Coven leadership/admin roles include High Priestess or High Priest.
- Coven admin area needs accept/deny for join requests.
- Member profile can be a popup.
- Wish lists are only visible to covenmates.
- Wish list items are selected by the requesting player.
- Coven wish lists are for ingredient help.
- Ingredient Ask for Help can launch from ingredient detail, but the help/request model is Coven/social.
- Request limits:
  - Ingredient requests: up to 10 total requested quantity across selected realm ingredients.
  - Example: Sunwarm Dewdrops x5 uses 5 of 10, leaving up to 5 more total quantity across other ingredient requests.
  - Choosing ingredient quantity matters more than the number of visible ingredient slots.
- Wish list refreshes every 48 hours.
- Fulfilled items leave empty slots until reset.
- Fulfilled ingredient quantities reduce or empty the visible wish list until the 48-hour refresh.
- Fulfilled gifts go to the requester's Inbox for collection.
- Club card item sharing is a separate instant-sharing system to define later.

## Daily Bonus, Streaks, Freebies, and Inbox

- Daily Bonus is a 7-day loop.
- Day 7 is a Daily Bonus Chest.
- The true streak is a separate long-running track.
- True streak milestones include 7, 14, 30, 60, 100, 180, and 365 days.
- Streak Save:
  - First save is free.
  - Future saves cost crystals.
  - Prototype cost ladder: 50, 100, 200, 300+.
  - Streak Save should be explicit, not hidden/automatic.
- Daily Bonus, Daily Spin, Freebies, Realm income, and Enchanted Trail are separate reward loops.
- Daily Spin is not the jackpot wheelspin.
- Freebies are social/deep-link rewards, not Daily Bonus claims.
- Freebie links should expire after about two weeks.

## Bazaar, Oracle Alley, and Madame Solange

- Bewitchment Bazaar is the preferred marketplace hub name and should not be replaced without user approval.
- Oracle Alley is the preferred limited-time Oracle/tarot subset inside or near Bewitchment Bazaar.
- Madame Solange Lumiere is the preferred Oracle/tarot NPC or event host.
- Madame Solange's role is Oracle Alley readings/events, not general quest guidance, Daily Bonus guidance, Coven prompts, or album coaching.
- Oracle readings should be chance-based and should not guarantee constant high-value rewards.
- Oracle Alley should preserve scarcity around rare cards, gilded cards, ancient cards, wild assets, and special assets.
- Oracle Alley should not become permanently open with unlimited rare access unless explicitly approved.

## Aura And Rank Bands

Aura Strength is the account-strength measure that lifts Rank. Level and XP are inputs into Aura, but Rank is not directly derived from Level alone.

Confirmed progression model:

- XP supports gameplay Level progression.
- Aura Strength combines approved account progression sources, including Level/XP history, gameplay/account progress, collection/restoration progress, social contribution, and a small capped purchase contribution.
- Rank is derived from Aura Strength.
- Rank bands are broad account progression eras and are separate from realm difficulty.
- Purchases may contribute to Aura, but only as a small capped support signal. Purchases cannot independently cause rank advancement or bypass gameplay, collection, or restoration progress.

Current rank titles are locked as:

- Novice
- Apprentice
- Spellbinder
- Mage
- Thaumaturge
- Mystic
- Enchanter
- Wizard
- Spellmaster
- Archmage
- Grand Archmage
- Paragon
- Ascendant
- Sorcerer Supreme

Previous numeric level ranges for these rank titles are superseded by the Aura model. Final Aura thresholds remain open and should be configured after the Aura formula is approved.

## Rank Benefit Principles

- Rank progression rewards long-term Aura growth through four benefit lanes:
  - Identity: avatars, frames, daubs, badges, and titles.
  - Daily Comfort: small capped boosts to safe daily/account rewards.
  - Social Help Capacity: more ingredient help capacity and later card gift/trade capacity.
  - Rank-Up Chest: a one-time celebration reward when entering a new rank band.
- Rank benefits should stay readable and limited; not every rank needs to touch every system.
- Rank must not improve core gameplay odds, jackpot odds, jackpot values, rare-card odds, ingredient drop odds, trade value, Coven/event competitive scoring, or room/realm progression requirements.
- Exact percentages, counts, chest contents, and unlocks remain tunable and should not be hard-coded without approval.

## Reference Resources

- Tuned potion/ingredient spreadsheet: `BMM Potions Tuned v1.xlsx`
- Google Sheets version from prior upload: `https://docs.google.com/spreadsheets/d/1oyHHRWvOl4P3XL8JFm-cmj2_-TCeTvGRRHQVwUpBfAY/edit`
- Daily rewards notes/resource: `daily.docx`
- Comprehensive handoff reference: `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md`
- Project docs GitHub repo: `https://github.com/nLuisi79/bingo-magic-mayhem`
- Mockups and visual references live in the user's `OneDrive/Desktop/nLuisi Laptop/Bingo Magic Mayhem` folder and were used as visual direction, not exact implementation requirements unless explicitly stated.

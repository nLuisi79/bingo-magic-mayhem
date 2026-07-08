# Bingo Magic Mayhem Gameplay Rules v0.1

## Core Position

Bingo Magic Mayhem is a magical bingo adventure game, not a casino product. Players earn in-game progression: coins, energy, mana, collection cards, power-ups, Realm ingredients, restore rewards, jackpot states, and experience points.

The game should feel fast enough for mobile sessions, fair to casual players, multiplayer in feel, and tunable for economy balance.

## Room Lifecycle

- Minimum human players for standard rooms: 1.
- v0.1 has two high-level room modes: Standard Room and Special Room.
- Standard Room is the core everyday loop and uses line, column, and diagonal bingos.
- Special Room is reserved for realm-randomized variants such as blackout or other special gameplay.
- The prototype starts on a Realm Map shell for Sunpetal Conservatory.
- The active playable room is Gilded Azalea Arboretum.
- Restoring Gilded Azalea Arboretum visually unlocks Thorn-Tangled Terrace on the map, though Room 2 gameplay is deferred.
- Target visible room size: 12-24 players.
- Simulated players may fill empty room seats and appear as normal users.
- Standard room wait target: 10-15 seconds maximum, with immediate start allowed for MVP testing.
- Same room means same call sequence for every player.
- Player-specific cards are generated per room and round.

## Standard Round End Conditions

Standard rooms do not end after the first bingo and do not automatically end after a jackpot state.

The primary round clock is the number of balls called. Bingo counts are reward progression, not the main timer.

A standard round ends when one of these occurs:

- The room Bingo Pool is exhausted.
- The maximum ball limit is reached.
- A safety timer expires.
- All active human players reach their maximum reward state.
- A room-specific objective is completed.

MVP prototype behavior:

- The prototype uses a shared room Bingo Pool.
- Human auto-claims and lightweight simulated-player claims both consume the room pool.
- Human player card rewards are still tracked independently.
- The prototype standard room uses a dynamic ball-call maximum and a 16-bingo room pool so jackpot states are reachable without appearing every round.
- Jackpot target tuning: an active player should generally see a jackpot about once every 7-10 standard rounds, assuming attentive daubing and a normal mix of card counts and mana tiers.
- Jackpot frequency should scale by setup: fewer cards and lower mana should have lower jackpot odds; more cards and higher mana should have more opportunity.
- Full simulated player cards, daubing, and reward simulation are deferred until room simulation exists.
- Auto-calling continues after a card reaches jackpot state unless all player cards are maxed or the ball pool ends.

## Round Pacing Guidelines

- A standard casual room should usually call about 42-62 balls depending on card count and mana tier.
- Prototype call cap baseline: 1 card starts near 42 calls, 2 cards near 46, 4 cards near 50, and 6 cards near 54.
- Higher mana tiers can add more calls to the standard-room cap, but should not exceed the room maximum.
- Event and jackpot-forward rooms may call 60-75 balls.
- The room Bingo Pool should scale with expected room population and average card count.
- Suggested starting formula: players * average cards * 0.35, rounded to the nearest whole bingo.
- Example: 12 players averaging 2 cards creates about 8 room bingos.
- Example: 20 players averaging 3 cards creates about 21 room bingos.
- Example: 24 players averaging 4 cards creates about 34 room bingos.
- More players/cards means bingos appear sooner, so either the Bingo Pool or max calls should rise if jackpot play is expected.
- A 5/5 jackpot spin is awarded when any one player card reaches five completed bingo patterns before the round ends.
- Jackpot wheelspins are awarded after the round is redeemed, not at the instant the fifth bingo is hit.
- Each 5/5 jackpot card awards exactly one jackpot wheelspin.
- A 5/5 card does not directly grant the grand jackpot payout. It grants a wheelspin, and the grand jackpot is one possible wheel result.
- Multiple jackpot cards in one standard round should be possible but rare.
- If normal play regularly produces jackpot states, reduce max calls, reduce room Bingo Pool, or move that room into an event/jackpot pacing profile.
- Simulated-player claims should create room pressure, but should not hide or override the player's own card-by-card bingo progress.

## Jackpot Wheelspin

The jackpot wheelspin screen is a reward moment, not a gameplay setup screen.

- The lobby/card-selection screen can show the jackpot wheelspin opportunity and current room pot.
- The full wheel view appears only after the player wins, qualifies for a wheelspin reward, and redeems the round.
- The wheel screen resolves pending wheelspins one at a time so each spin has its own suspense beat.
- Each spin consumes one pending wheelspin and adds its result to one visible reward stack.
- The wheel should visibly spin before each result lands.
- Rewards are not added to the profile until the player presses Collect.
- Collect appears after all pending wheelspins have been spun and applies the full stack to profile mana in one payout moment.
- Each room owns its own jackpot pot.
- Level 1 / Gilded Azalea Arboretum has a minimum jackpot pot of 250 Mana.
- The Gilded Azalea pot should never fall below 250 Mana.
- The pot grows as players spend mana in that room.
- The exact contribution rate is still tunable; the prototype currently contributes a small portion of room-entry mana into the room pot.
- Later rooms can have higher minimum pots, larger contribution impact, and stronger room scaling.

All wheel values scale from the current room jackpot pot.

- Small standard slots use fractions of the current pot.
- Base standard slot = 1x current pot.
- Jackpot = 1x current pot.
- Epic = 2x current pot.
- Legendary = 3x current pot.

Level 1 prototype wheel values:

- Standard slots: 20%, 30%, 40%, 60%, 80%, 100%, 125%, and 150% of current pot.
- Jackpot slot: 1x current pot.
- Epic slot: 2x current pot.
- Legendary slot: 3x current pot.
- Displayed values can be rounded into clean readable numbers.

Pot reset behavior:

- Standard wheel slots do not reset the pot.
- Jackpot, Epic, and Legendary outcomes reset the pot to the room minimum.
- For Gilded Azalea Arboretum, reset means returning the pot to 250 Mana.

Wheel UI rules:

- Landscape format.
- Large wheel on the left or center-left.
- Value panel on the right.
- The center wheel button says SPIN while pending wheelspins remain, then COLLECT after all spin results have stacked.
- No separate bottom spin button.
- Mana is represented by coin visuals, not crystals.
- Standard slices show large numbers only.
- Jackpot, Epic, and Legendary slices show only their labels.
- Jackpot, Epic, and Legendary values are defined in the side panel.
- Legendary should feel most visually dramatic; Epic should feel more valuable than Jackpot; Jackpot should still feel exciting.

## Bingo Model

One bingo is one completed valid pattern on one card.

Standard valid patterns:

- Horizontal line.
- Vertical line.
- Diagonal line.

Room-specific future patterns may include:

- Four corners.
- Special shapes.
- Blackout or full-card objectives.

Each card can independently earn reward states:

- 1/5: first bingo.
- 2/5: two completed patterns.
- 3/5: stronger reward state.
- 4/5: rare reward chance.
- 5/5: jackpot state.

Each card caps at 5/5 in standard rooms.

## Auto-Claim

v0.1 uses auto-claim.

When a card completes a valid bingo pattern:

- The game detects the completed pattern automatically.
- The card reward state increments.
- The card can continue earning until its reward cap.
- Invalid manual claim UX is deferred because no manual claim button is required for v0.1.

## Called-Ball Visibility

- The right rail shows the call queue with the newest called ball at the top.
- The next entries descend in call order so players can quickly scan recent calls.
- The left rail keeps a full called-ball history behind a Show/Hide control.
- The full called-ball history is a player aid and should not cover cards or daub targets.
- If a round ends and Clairvoyance was not active, missed daubs should open the full called-ball panel automatically.
- If Clairvoyance was active, missed-daub review does not need to interrupt the reward summary.
- Missed daub calls should be visually highlighted and summarized by card so the player understands what they left behind.

## Ingredients and Restore Rewards

Ingredients are Realm-specific restoration items. They should come from Realm gameplay, not generic login rewards or unrelated systems.

### Realm 1: Sunpetal Conservatory

Room 1: Gilded Azalea Arboretum

- Potion: Azalea Sunmend Essence
- Gilded Azalea Petals: 18 Common. Main floral base of the potion.
- Sunwarm Dewdrops: 14 Common. Rehydrates wilted blooms.
- Honeyglow Pollen: 10 Uncommon. Reawakens blooming magic.
- Amberroot Shavings: 7 Uncommon. Repairs the arboretum's root network.
- Arboretum Heartseed: 3 Key Ingredient. Final catalyst that completes the restoration.

Room 2: Thorn-Tangled Terrace

- Potion: Thornweave Taming Elixir
- Velvet Thorn Tips: 20 Common.
- Terrace Moss Tufts: 15 Common.
- Rosegold Sap Drops: 11 Uncommon.
- Bramblelace Fibers: 8 Uncommon.
- Thornheart Bud: 2 Key Ingredient.

Room 3: Pollenspire Fountain

- Potion: Pollenspire Renewal Tonic
- Spire Pollen Grains: 21 Common.
- Fountain Dewdrops: 16 Common.
- Nectarflow Ribbons: 12 Uncommon.
- Lilygold Filaments: 7 Uncommon.
- Pollenspire Pearl: 1 Key Ingredient.

Room 4: Solstice Bloom Glasshouse

- Potion: Solstice Revival Tincture
- Sunlit Glass Slivers: 20 Common.
- Golden Hour Motes: 16 Common.
- Prismvine Twine: 13 Uncommon.
- Dawnmirror Fragments: 10 Uncommon.
- Clockwork Stamen: 7 Key Ingredient.

Ingredient drop direction:

- Each card loads with a visible potion multiplier from 1-4.
- The multiplier represents the ingredient strength for that card.
- A card must earn at least one bingo to award ingredients.
- No bingo on a card means no ingredient reward from that card.
- 1/5: first room ingredient multiplied by the card potion value.
- 2/5: first and second room ingredients multiplied by the card potion value.
- 3/5: adds the first uncommon ingredient multiplied by the card potion value.
- 4/5: adds the second uncommon ingredient multiplied by the card potion value.
- 5/5: adds the key ingredient for that card's jackpot state.

Some individual bingo squares can also hold visible reward markers. These markers should be visible before daubing, and the reward is collected only if the player daubs that square during the round.

Prototype cell reward marker examples:

- Card.
- Chest.
- Club Orbs.
- Double Prize.

Restore Rewards are granted when the player contributes enough ingredients to complete a Realm restoration step.

Prototype restore behavior:

- The active room shows its potion name, all required ingredients, and restore reward on the room home screen.
- The Restore button is disabled until every required ingredient is met.
- Restoring Gilded Azalea Arboretum consumes the required ingredients.
- The prototype restore reward is 1,000 Mana and 1 Card Pack.
- Once restored, the room is marked restored in local save state.

Examples:

- Minor restoration: coins, energy, small power-up bundle.
- Mid restoration: collection cards, crystals, stronger power-ups.
- Major artifact stage: rare card, rare power-up, large reward chest.
- Full restoration or Threshold Board: next Realm unlock or portal opening.

## Power-Ups

Power-ups should make the player feel magically empowered without harming other players. Standard power-ups primarily affect the player's own cards or reward odds.

### Clairvoyance

Clairvoyance is the first v0.1 power-up.

Player-facing fantasy:

- Clairvoyance is represented by a crystal ball.
- It helps the player notice called numbers on their cards.

Award durations:

- 15 minutes.
- 30 minutes.
- 60 minutes.

Gameplay behavior:

- If Clairvoyance is active, after a ball is called, the game waits a few seconds.
- If the called number exists on one of the player's active cards and has not been daubed, that cell begins flashing.
- The flash is an alert, not an automatic daub.
- The alert should be delayed long enough that attentive players can daub unaided, especially when playing four cards.
- The number text should flash, not the whole cell background, so Clairvoyance does not visually fight daubs or bingo-line highlighting.
- The number should keep flashing until the player daubs it, the round ends, or the cell is otherwise no longer eligible.
- The player still earns lower XP than a clean unaided daub.

Prototype behavior:

- Clairvoyance is not standard room behavior.
- Clairvoyance must be earned, purchased, or granted by a prototype debug control before it becomes active.
- The alert delay is a few seconds after each call, tuned slower than the first prototype pass for four-card readability.
- Matching unmarked called numbers flash visually until daubed.

## Experience Points

Players earn XP during every game.

XP supports player level progression and should reward attention without making casual play feel punishing.

XP rules:

- Every valid daub can grant XP.
- Faster daubs after the ball is called grant more XP.
- Daubs made before Clairvoyance assistance begins are worth more.
- Daubs made after Clairvoyance highlights the cell are valid but worth less.
- Invalid daubs do not grant XP.
- Bingo states can grant bonus XP.
- Jackpot states can grant larger bonus XP.

Prototype XP direction:

- Fast unaided daub: highest daub XP.
- Normal unaided daub: medium daub XP.
- Clairvoyance-assisted daub: lower daub XP.
- Bingo bonus XP is added when a new bingo state is auto-claimed.
- Jackpot bonus XP is added when a card reaches 5/5.

Prototype level direction:

- Level starts at 1.
- Level 2 requires 100 total XP.
- Each next level requires 50 more XP than the previous level.
- Reward preview should call out a level-up when a round crosses a level threshold.

## Economy Baseline

- MVP card count: 1-4 cards.
- Mana is magical intensity per card, not real-money betting language.
- Rewards scale by room, card count, mana tier, card state, event modifier, and progression state.
- Higher mana increases opportunity and scaling, not guaranteed outcomes.
- Jackpot odds and reward tables should eventually be server-controlled and remotely tunable.
- End-of-round reward presentation should separate mana won, XP earned, total bingos, collected cell rewards, and ingredients won.
- Mana and XP should appear as top summary stats, not again in the collected-reward tile list.
- Repeated collected rewards should be grouped into one tile with a quantity indicator such as x2 or x3.
- Ingredients won should appear in the bottom potion-progress section.
- The Collect action is the moment round rewards are collected into inventory.
- If Collect awards jackpot wheelspins, those spins are added to inventory and the player is routed to a wheelspin claim screen automatically.
- Jackpot wheelspin count can be emphasized on the wheel screen rather than the round-complete modal.
- Prototype jackpot wheelspin rewards are placeholder-only and currently grant mana based on the current room pot so the reward loop can be tested before final wheel tables exist.

## Locked v0.1 Decisions

- Standard rooms do not end after first bingo.
- Standard rooms do not automatically end after jackpot state.
- Standard rooms use normal line, column, and diagonal bingo patterns.
- Special rooms are not implemented in v0.1 gameplay yet, but the rules layer should leave room for blackout and realm-specific variants.
- Same room, same calls, different cards.
- Minimum human players for standard room is 1.
- Simulated players can consume a small number of prototype room Bingo Pool claims for pacing.
- Auto-claim is the v0.1 bingo claim model.
- Each card can independently earn rewards.
- 5/5 on a card triggers jackpot state.
- Clairvoyance is the first implemented power-up.
- XP accumulates in every game and rewards faster, unaided daubing.
- XP contributes to player levels.
- Reward preview requires a Redeem action before returning to the room home.

# Prototype Architecture v0.1

## Current Milestone

The Unity prototype now supports the core room loop:

- Lobby card-count and mana selection.
- Four-card gameplay layout.
- Shared auto-calling.
- Manual daubing.
- Auto-claim bingo detection.
- Jackpot wheelspin earning from 5/5 cards.
- Animated placeholder jackpot wheelspin screen.
- Room jackpot pot growth and reset behavior for Gilded Azalea Arboretum.
- Room bingo pool with lightweight simulated-player claims.
- Clairvoyance power-up alerting.
- XP rewards based on daub speed and bingo state.

## Extracted Scripts

### BingoCaller

Owns the 1-75 call pool, called-number history, and called-number set.

### BingoCardModel

Owns standalone card generation and bingo-line detection logic. The main prototype still has legacy card arrays, but this class is the target model for the next deeper card refactor.

### BingoRoomRules

Owns tunable standard-room rules:

- Auto-call interval.
- Max ball calls.
- Room bingo pool.
- Clairvoyance delay.
- Daub XP windows.
- XP values.
- Jackpot state target.

### BingoRewardTracker

Owns progression state that is not UI:

- Player XP.
- Round XP.
- Player level and XP-to-next-level math.
- Room bingo pool consumption.
- Simulated-player bingo claim count.
- Simulated-player claim pacing.

### RewardPreview

Builds the current end-of-round reward preview:

- End reason.
- XP earned.
- Entry mana.
- Player and simulated bingo totals.
- Best card state and jackpot card count.
- Jackpot wheelspins earned one-for-one from 5/5 jackpot cards.
- Prototype Realm ingredient drops.
- Collected cell reward summary.

### CellRewardTracker

Owns prototype reward-bearing bingo square state:

- Which cells have visible reward markers.
- Which reward markers have been collected by daubing.
- Round summary text for collected cell rewards.

### PlayerInventoryState

Owns prototype player inventory state:

- Mana and crystal balances.
- Active power-up count.
- Realm ingredient counts.
- Redeemed cell reward inventory.
- Pending jackpot wheelspins.
- Current room jackpot pot.
- Mana spending for room entry.
- PlayerPrefs save/load for prototype persistence.

### Persistence

Current persistence uses Unity `PlayerPrefs`:

- Inventory keys are prefixed with `BMM.Inventory.`
- XP key is `BMM.Rewards.PlayerXp`
- This is prototype-only and should eventually move to JSON or backend profile data.
- The room home includes a prototype-only `Reset Save` button for clearing saved test progress.
- The room home includes prototype-only grant buttons for Clairvoyance and jackpot wheelspins so reward flows can be tested without grinding setup conditions.

### BingoRoundState

Owns active round lifecycle state:

- Whether the round is active.
- Whether a jackpot state has been reached.
- Auto-call countdown timing.
- Starting and stopping the round.

### PowerUpRuntimeState

Owns active runtime power-up state:

- Whether Clairvoyance is active.
- Which cells are currently being revealed by Clairvoyance.
- Round reset cleanup for temporary power-up alerts.

### PlayerCardSet

Owns active player card state:

- Generated card numbers.
- Per-card marks.
- Per-card bingo counts.
- Winning-cell sets.
- Card generation.
- Total bingo counts.
- Jackpot completion checks.
- Called-number lookup across all player cards.

## Remaining Monolith Areas

`BingoPrototype` still owns too much:

- UI construction.
- Daub handling and controller glue.
- Clairvoyance visual coroutines.
- Reward preview presentation polish.
- Reward redemption persistence beyond the prototype session.
- Legacy single-card helper code.

This is acceptable for the current milestone because it keeps the working scene stable, but it should be split before adding larger systems.

## Recommended Next Refactor

Split in this order:

1. `GameplayHud`: fixed layout/UI text and image creation after reward flow stabilizes.
2. `PowerUpVisuals`: Clairvoyance alert coroutines and visual treatment.
3. `RewardEconomyTable`: server-style tunable ingredient and reward odds.

## Implementation Rule

Keep each refactor behavior-preserving before adding new features. The current prototype is finally playable enough that stability matters.

# Beta 1 UI Scope

Last updated: 2026-07-17

Status: working production-minded UI scope for Beta 1. This is not a gameplay, economy, progression, rarity, monetization, reward, or Aura/rank lock document.

Read with:

- `LOCKED_DECISIONS.md`
- `09_OPEN_DECISIONS.md`
- `10_VERIFIED_PROJECT_STATE.md`
- `12_BETA_1_PRODUCTION_READINESS.md`
- `13_BETA_QA_CHECKLIST.md`

## Purpose

Define the UI bar for Beta 1 so implementation effort goes toward the surfaces testers will actually touch, understand, and judge.

This document intentionally separates:

- UI that must feel close to real/final for Beta 1
- UI that may remain simplified but coherent
- UI that may remain shell-only or placeholder
- asset needs that should be prepared now versus later

## Beta 1 UI Goal

Beta 1 UI should make the game feel intentional, readable, and on-brand even though the full ten-realm production game is not in scope yet.

A Beta 1 tester should quickly understand:

- where to go
- how to start a room
- what is happening during bingo play
- what they earned at round end
- why jackpot wheelspin is exciting
- where profile, collection, and daily systems live
- which systems are real versus still placeholder

## Core UI Principle

For Beta 1, depth is less important than clarity and consistency.

Priority order:

1. readable navigation
2. strong gameplay readability
3. satisfying reward/jackpot presentation
4. consistent visual language across hubs and support screens
5. broader content/surface coverage

## Beta 1 UI Tiers

### Tier 1 — Must Feel Real

These surfaces should receive the strongest layout, polish, hierarchy, and motion attention for Beta 1:

| Surface | Beta 1 UI expectation |
|---|---|
| World Map | Clear entry point, realm selection clarity, readable locked/unlocked/completed state |
| Realm Map | Clear room progression, readable room status, strong path-to-play cues |
| Room Entry | Strong “pre-match” presentation with card choice, mana bet, restore state, jackpot visibility |
| Gameplay Screen | Most polished screen in Beta 1; high readability for called numbers, card state, rewards, power-up state, multiplayer status |
| Round-End Reward Preview | Clear summary, reward hierarchy, satisfying payout moment |
| Card Reveal | Clear “new card” excitement and readable duplicate/new state |
| Jackpot Wheel | Strong excitement, obvious action state, readable collect/spin flow |
| Player’s Den | Clear hub identity and understandable routing to profile, collection, daily, social, and inventory systems |

### Tier 2 — Must Be Clean And Coherent

These should look consistent and usable, but can remain lighter-weight and more system-driven:

| Surface | Beta 1 UI expectation |
|---|---|
| Profile | Clear account identity, cosmetics selection, settings readability |
| Library / Grimoire | Clean collection browsing with readable missing/owned/new/duplicate states |
| Book of Shadows shell | Distinct from Grimoire and clearly labeled if still limited |
| Daily Bonus | Clean direct-claim panel |
| Daily Spin | Separate from jackpot, readable result flow |
| Inbox | Clear category switching, row readability, message/card/gift distinctions |
| Friends | Usable list/action shell without deep polish dependency |
| Coven Circle | Usable shell with readable member/wishlist states |
| Cabinet of Curiosities | Clear inventory/power-up identity |
| Apothecary | Clear potion/ingredient identity |
| Enchanted Trail | Clean placeholder progression shell |

### Tier 3 — Can Remain Lighter Placeholder Surfaces

These should still be on-brand and understandable, but do not need premium-level polish for Beta 1:

- Relic Wall
- Leaderboards
- Mayhem Market shell
- Oracle Alley placeholder
- message composer
- deep social/moderation surfaces
- long-tail modal variants

## Beta 1 Visual System Deliverables

The following UI system work should happen before broad per-screen polish expands too far:

### Must establish

- panel style hierarchy
- button hierarchy
- typography hierarchy
- status/notification styling
- card/state highlight treatments
- reward emphasis styling
- modal/surface spacing rules
- icon treatment consistency
- screen transition behavior for major loops

### Beta 1 minimum motion language

- room-entry intro/emphasis
- gameplay entry clarity
- reward preview reveal
- card reveal presentation
- jackpot wheel intro/result emphasis
- modal open/close consistency

Motion should support readability and delight, not obscure action timing or gameplay truth.

## Asset Scope For Beta 1

### Locked working counts for Beta 1 production planning

- Avatars: 4-6
- Daubers: 3
  - 1 default
  - 1 early tutorial/progression unlock
  - 1 later progression unlock
- Frames: 4

These counts are planning targets for Beta 1 scope control, not final long-term catalog limits.

### Must-have assets now

These are the assets most likely to improve perceived quality quickly:

- avatars
- avatar frames
- daubers
- mana/crystal/power-up/jackpot/inbox/social icon set
- button and panel sprite kit
- reward/chest/tile accent kit
- card rarity/frame treatments
- “new” / “claimed” / “locked” / “ready” state badges
- jackpot wheel segment visuals
- room-entry and gameplay decorative accents

### Strongly recommended for Beta 1

- Den destination art or strong feature-tile treatments
- at least one cohesive background treatment for:
  - world/realm navigation
  - room entry
  - gameplay
  - player den
  - reward/jackpot moments
- collection placeholder art style for card/library presentation
- simple VFX-compatible assets for reward and jackpot emphasis

### Can be temporary if styled consistently

- per-feature decorative illustrations
- deep market/support-screen artwork
- extended social art
- many secondary icons
- per-screen bespoke prop art

### Can wait until later

- bespoke full-art support for all ten realms
- large-scale seasonal visual variants
- extensive cosmetic catalog breadth
- premium embellishment for low-priority shells

## Realm And Content Scope For Beta 1 UI

Beta 1 UI should not assume all ten realms need fully bespoke visual treatment.

Recommended Beta 1 UI content scope:

- polish one realm presentation style deeply
- support the active first realm path clearly
- allow additional realms to inherit the same system safely
- use controlled palette/layout variation rather than bespoke full-screen redesigns for each future realm

This keeps the visual foundation scalable without forcing full production art coverage too early.

Working Beta 1 assumption:

- Realm 1 should look as polished as practical within Beta scope, even if it is not yet at final 100% production-art completeness.
- Later realms may remain lighter/system-driven until Beta priorities are satisfied.

## Beta 1 UI Implementation Order

### Phase 1 — Core loop polish

Must focus on:

- room entry
- gameplay
- round-end rewards
- card reveal
- jackpot wheel

Reason:

These surfaces determine whether the game feels good to play.

### Phase 2 — Hub and navigation polish

Must focus on:

- world map
- realm map
- player’s den
- profile/customization

Reason:

These surfaces determine whether the product feels coherent outside the match.

### Phase 3 — Collection and retention support

Must focus on:

- library / grimoire
- daily bonus
- daily spin
- inbox
- cabinet / apothecary

Reason:

These reinforce progression and daily return value.

### Phase 4 — Social/support shell consistency

Must focus on:

- friends
- coven
- leaderboard
- market shell
- Oracle Alley shell
- Enchanted Trail shell

Reason:

These should read clearly, but they do not need to out-polish the main gameplay loop for Beta 1.

## Beta 1 “Must Look Final Enough” List

These are the surfaces where obvious placeholder-ness will hurt the beta most:

- room entry
- gameplay board and gameplay rails
- round-end reward flow
- jackpot wheel flow
- world/realm navigation
- player’s den
- basic profile identity/customization

These surfaces should receive bespoke art direction first rather than relying only on generic system styling:

- gameplay card/board treatment
- gameplay rails and called-number treatment
- room-entry header and room-entry presentation accents
- jackpot wheel presentation
- reward preview and card reveal presentation
- Player’s Den destination tiles
- avatar, frame, and dauber presentation

## Beta 1 “Placeholder Acceptable” List

These can remain cleaner prototype panels if clearly labeled and consistent:

- leaderboard rows/data
- market purchase surfaces
- Oracle Alley rewards
- deep Coven management
- advanced friend messaging
- extended Enchanted Trail logic

For Beta 1, these surfaces may rely primarily on system-generated styling plus limited accent art:

- inbox
- friends
- coven shell
- leaderboard
- market shell
- settings/profile rows
- Enchanted Trail shell
- Oracle Alley shell
- most utility/support modals

## Temporary External Asset Strategy

Beta 1 should not combine many unrelated UI skins. Too many mixed packs will make the product feel stitched together.

Recommended approach:

- 1 main UI skin/source for panel, button, frame, and chrome construction
- 1 icon source for economy, rewards, inventory, and social symbols
- 1 portrait/avatar source only if placeholder avatar coverage is needed quickly
- optional low-priority filler source for support screens only

### Recommended source categories

- Unity Asset Store as the preferred temporary foundation for the main UI skin
- Unity Asset Store or a single consistent icon pack for economy/reward/support icon coverage
- itch.io or free/open sources as secondary filler only, not as the primary visual identity

### Recommended temporary sourcing direction

Primary UI foundation candidates:

- Unity Asset Store 2D GUI directory
- `GUI - Casual Fantasy`
- `RPG UI`

Portrait/avatar fallback candidates:

- `Character avatar icons - RPG bundle`
- `100+ Avatar Icons Vol.2`

Broad icon fallback candidate:

- `5592 Fantasy RPG Icons`

Secondary low-cost filler exploration:

- fantasy or magic UI packs on itch.io
- free fantasy GUI packs on itch.io for support-screen placeholders only

### Temporary asset use rule

External packs should support the Beta UI foundation, not define the game’s final identity by themselves.

If a pack clashes with the bright magical casual tone, gameplay clarity, or panel system consistency, it should be treated as reference/filler only.

## UI Risks To Avoid

- Polishing support screens before gameplay readability feels solid
- Making every surface unique instead of building a reusable UI system
- Over-scoping bespoke realm art before the first realm loop is convincing
- Letting placeholder art styles clash heavily from screen to screen
- Building reward/jackpot motion that makes action states unclear
- Treating every existing prototype screen as equally important for Beta 1

## Beta 1 UI Open Planning Questions

These should be answered before large-volume asset production starts:

- How many avatars are enough for Beta 1?
- How many daubers are enough for Beta 1?
- How many frames are enough for Beta 1?
- How distinct should Realm 1 look compared with later placeholder realms?
- Which outside asset packs/resources are acceptable for temporary Beta use?
- Which surfaces must use bespoke art immediately versus system-generated styling?
- Whether gameplay UI should prioritize decorative richness or clean clarity when tradeoffs appear

## Recommended Working Assumption

Until a more specific UI plan is approved, work toward this Beta 1 assumption:

- one strong, polished visual language
- one strongly polished realm/gameplay loop
- one clear Den hub
- simplified but coherent support screens
- real excitement in rewards and jackpot
- customization breadth kept intentionally modest
- clean gameplay clarity prioritized first when decorative richness would compromise readability

## Immediate Next UI Tasks

Recommended next production-minded UI tasks:

1. Finalize Beta 1 surface priority list.
2. Finalize Beta 1 asset list by priority: must-have, recommended, placeholder-acceptable.
3. Pick a visual-system pass for gameplay/room-entry/rewards/jackpot before broad support-screen polish.
4. Decide which temporary outside assets are acceptable for Beta use.
5. Define a minimum customization target for avatars, frames, and daubers.

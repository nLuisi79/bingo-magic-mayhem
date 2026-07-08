# Implementation Gaps

Last updated: 2026-07-07

These are known places where the code is incomplete, prototype-only, or may not yet match the intended product.

## Architecture Gaps

- `BingoPrototype.cs` is still a large monolithic script owning gameplay UI, map UI, Den UI, Daily Bonus, Inbox, Library, Coven, wheelspin, and many glue behaviors.
- Recommended future split:
  - `GameplayHud`
  - `RewardEconomyTable`
  - `InboxState`
  - `DailyRewardsState`
  - `DenNavigation`
  - `PowerUpVisuals`
- Profile state is still mostly PlayerPrefs prototype storage.
- There is not yet a durable backend/profile persistence model.
- Reward grants have a shared prototype type, but not every system uses it yet.

## Gameplay Gaps

- Final cadence tuning for 1, 2, 4, and 6 card play is still in flux.
- Standard play can still feel too close to blackout depending on power-up and ball cap tuning.
- Jackpot odds need final tuning by card count and mana bet.
- Ingredient progression is likely too fast for production.
- Final special room pacing and blackout reward tuning are not locked.
- Some countdown/round-end visibility may still need playtest verification across all rooms.

## Power-Up Gaps

- Visual readability of dropped power-up badges is still prototype-level.
- Arcane Spark final lightning-style presentation is not built.
- Fortune Sigil final feedback and reward multiplier presentation need polish.
- Wild Sigil is logic/prototype-level and needs premium-feeling UX.
- Presto Sigil behavior needs continued validation.
- Pandora opening animation is prototype-simple.
- Power-up inventory and activation still need a final Den/Cabinet UX.

## Reward and Economy Gaps

- Final reward values are not tuned.
- Daily Bonus reward values are placeholders.
- Day 7 Daily Bonus Chest contents are not final.
- Daily Spin exists as a direct-claim prototype shell, but values, odds, animation, and cadence are not final.
- Enchanted Reward Trail is not built.
- Freebie reward values are placeholders.
- Purchase confirmation flow is not built.
- System compensation flow is only prototype-ready through shared inbox concepts.
- Mana cauldron scale likely needs review.
- Bewitchment Bazaar is placeholder.
- Oracle Alley is not implemented.
- Madame Solange is not implemented.
- Oracle Dust is not implemented.
- Oracle reading odds, rewards, limits, and scarcity protections are not implemented.

## Inbox Gaps

- Inbox UI is prototype-level.
- Inbox has shared reward plumbing, but not every reward source routes through it.
- Need final tabs for Messages, Cards, Gifts.
- Need expiration/read/unread behavior.
- Need claim-all behavior across mixed rewards.
- Need message/read-only rows.
- Need purchase, freebie, compensation, and coven gift examples.

## Daily / Retention Gaps

- Streak Save state exists, but the full recovery UX needs playtest.
- Streak milestone chest rewards are not final.
- Daily Bonus info modal/help copy is not final.
- Daily Spin is separate from Daily Bonus and jackpot wheelspin, but still prototype-simple.
- Freebies are only a social/deep-link prototype stub.
- Enchanted Trail is not implemented.
- Weekly witch visit is not implemented.

## Map and Progression Gaps

- World Map and Realm Map are functional prototypes, not final visual maps.
- Need continued verification that reset/unlock rules match:
  - only Realm 1 Room 1 open on reset
  - restored room unlocks next room
  - final room restores next realm
- Realm 2+ data may exist from spreadsheet/import work but needs validation.
- Hundreds-of-realms scalability is not solved in UI or data pipeline yet.

## Ingredients and Potions Gaps

- Spreadsheet tuning has been introduced, but code/data should be re-verified against the latest Google Sheet.
- Ingredient rarity labels/weights are not fully formalized.
- Production pacing should be much slower than testing.
- Need final per-room potion rewards.
- Ingredient detail now has a prototype Ask for Help / Use Wild shell, but final request delivery, helper eligibility, expiration, trading limits, and production UX remain open.
- Need ingredient wish-list integration with final Coven profile UI.

## Library / Albums Gaps

- Library UI is prototype-level.
- Grimoire/Book of Shadows navigation roughly follows the desired book/index/detail model but not final art/layout.
- Card state readability has a prototype pass for missing/owned/duplicate states and number-only duplicate badges, based on current mockup direction.
- Book of Shadows purchase/access flow is placeholder.
- Card pack opening/reveal exists as a prototype, but full pack source routing is incomplete.
- Card gifting now has a narrow Library prototype for extra regular Grimoire duplicates, with card delivery through Inbox > Cards.
- Card trading is not built.
- Final card gifting/trading limits, eligibility, cooldowns, Friends/Profile integration, and anti-abuse rules remain open.
- Book of Shadows set transitions need production-ready timer handling.
- Joker Wild card use/reset flow is not fully built.

## Coven Gaps

- Coven shell exists, but many systems are placeholders.
- Needs final join/leave flow.
- Needs final member roles and admin permissions.
- Needs full member stats/profile popup.
- Needs team chat polish/moderation.
- Needs weekly challenge/coven card system.
- Needs Coven Emporium final catalog.
- Needs standings/scoring.
- Needs Club Orb systems.
- Needs ingredient gift/request limits, quantity picker, reset timers, and final request delivery behavior.

## Den / Meta Gaps

- Player's Den is still a placeholder layout.
- Bewitchment Bazaar is placeholder.
- Apothecary is placeholder.
- Relic Wall is placeholder.
- Quests are placeholder.
- Profile settings and SSO are not built.
- Avatar/companion/cosmetics are not built.

## Visual / UX Gaps

- Most visuals are placeholder blocks/text, not final art.
- Game cards should better use available horizontal space by card count.
- Called history/all-called numbers need readability improvements.
- Large center alert space should be used for important messages, countdowns, and collected reward feedback instead of tiny rail messages.
- Yellow text should be minimized; it has been hard to read in several contexts.
- Final landscape/mobile layout needs further testing.

## Documentation Gaps

- Older docs may say some systems are "not implemented" even though prototype shells now exist.
- This split handoff should be treated as newer than the older v0.1 docs where they conflict.
- GitHub docs should be updated after major prototype milestones.
- Mockups and screenshots need an indexed resource list if future threads need precise visual references.

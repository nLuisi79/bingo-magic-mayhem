# Verified Project State

Last updated: 2026-07-09

This audit summarizes the current verified state of the Bingo Magic Mayhem repository and project documentation. It is not a gameplay/economy finalization document. It separates locked decisions from strong preferences, unresolved items, superseded ideas, and future implementation risks.

## Files Reviewed

Active local entry point:

- `CURRENT_PROTOTYPE_STATE.md`

Active Unity docs:

- `Bingo Magic Mayhem/Docs/LOCKED_DECISIONS.md`
- `Bingo Magic Mayhem/Docs/OPEN_DECISIONS.md`
- `Bingo Magic Mayhem/Docs/IMPLEMENTATION_GAPS.md`
- `Bingo Magic Mayhem/Docs/12_BETA_1_PRODUCTION_READINESS.md`
- `Bingo Magic Mayhem/Docs/11_RANK_REWARDS_V0.1.md`
- `Bingo Magic Mayhem/Docs/Gameplay Rules v0.1.md`
- `Bingo Magic Mayhem/Docs/Game Systems Roadmap v0.1.md`
- `Bingo Magic Mayhem/Docs/Prototype Architecture v0.1.md`

Archive/reference:

- `BingoMagicMayhem/assets/docs/prototype-plan.md`
- `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md`
- `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\covens.txt`
- `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Locked_Systems_Handoff_trades.docx`
- `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\detailedhandoff.docx`

Expected but missing:

- `docs/00_READ_ME_FIRST.md`

Present at repository root rather than expected nested path:

- `AGENTS.md`
- `00_READ_ME_FIRST.md`

## Documentation Structure

The repository currently has three documentation layers:

- Root handoff entry: `CURRENT_PROTOTYPE_STATE.md`.
- Active Unity documentation: `Bingo Magic Mayhem/Docs`.
- Older Cocos prototype documentation: `BingoMagicMayhem/assets/docs`.

Source-of-truth order for future work:

1. Explicit current user instruction in the active thread.
2. `CURRENT_PROTOTYPE_STATE.md` as the entry point.
3. Current locked/verified docs in `Bingo Magic Mayhem/Docs`, especially `LOCKED_DECISIONS.md`, this file, and `09_OPEN_DECISIONS.md`.
4. Focused current-system drafts such as `11_RANK_REWARDS_V0.1.md`, where marked as working draft rather than locked.
5. Current Word handoffs that explicitly identify themselves as locked/current.
6. `OPEN_DECISIONS.md` and `IMPLEMENTATION_GAPS.md`.
7. Older v0.1 docs as baseline/reference where they do not conflict.
8. Cocos prototype files and older archive materials as historical reference only.

## Confirmed Decisions

- Unity is the active implementation direction.
- Bingo Magic Mayhem is a magical bingo adventure, not a casino product.
- Player progression centers on Realms, rooms, ingredients, potion/restoration progress, collections, rewards, and social systems.
- Player's Den is the account/home hub.
- Den areas are strongly/currently established as Library/Grimoire, Apothecary, Cabinet of Curiosities, and Relic Wall.
- Cards belong in Library/Grimoire, not Cabinet.
- Ingredients belong in room/potion/Apothecary contexts.
- Club orbs belong to Coven/event systems, not Cabinet.
- Daily Bonus, Daily Spin, Freebies, Realm income, Enchanted Trail, Inbox, and jackpot wheelspin are separate loops.
- Daily Bonus is direct-claiming and should not be placed in Inbox.
- Freebies are social/deep-link direct redemption and should not be placed in Inbox unless a later decision explicitly changes that.
- Freebie links should expire after about two weeks, without exposing the expiration date in the URL.
- Inbox is appropriate for claimable gifts/system items, including fulfilled Coven gifts.
- Jackpot wheelspin is a post-round reward moment, not gameplay setup.
- Jackpot wheelspin rewards are not granted until the player collects the stacked result.
- Standard rooms use horizontal, vertical, and diagonal bingo patterns.
- Standard rounds do not end after first bingo or automatically after jackpot state.
- Ingredients are primarily earned through realm gameplay.
- A card must earn at least one bingo to award ingredients.
- Realm/room progression starts with only Realm 1 Room 1 unlocked after full reset.
- Restoring a room unlocks the next room; completing the final room unlocks the first room of the next realm.
- Blackout is special-room gameplay, not a standard room rule.
- Grimoire is the main/free collection album.
- Book of Shadows is premium/special.
- Grimoire is a 90-day event.
- Grimoire is locked by the latest Word handoff at 32 sets/pages x 10 cards = 320 cards.
- Card frame rarity is currently Regular, Gilded, Ancient with 1/3/5-star treatment.
- Grimoire index uses 8 potion tiles per open-book index page.
- Card gifting/trading belongs in the Library/Grimoire area.
- Library/Grimoire should own missing regular cards, duplicate regular cards, eligible card gifts, and eventual card trades.
- Current prototype includes a narrow Library card gifting shell for extra regular Grimoire duplicates, with incoming card gifts claimed through Inbox > Cards.
- Current prototype has a card state readability pass for missing, owned, duplicate, and unseen card states; final card art/frame assets remain open.
- Potion/ingredient detail flow is landscape open-book or modal UI, not active gameplay UI.
- Ask for Help is launched from Individual Ingredient Detail and uses Coven/Friends tabs with one shared rank-based daily request pool.
- Use Wild is a card-to-card conversion confirmation from Individual Ingredient Detail.
- Covens are the team/social group system.
- Coven sharing should be manual, not automatic.
- Coven wish lists are ingredient-help focused.
- Coven wish list ingredient requests allow up to 10 total requested quantity across selected realm ingredients.
- Coven member profile can be a popup/modal over Coven Circle rather than a separate screen.
- Needs/extras visibility and Collection Assist are major Coven differentiators.
- Bewitchment Bazaar is the marketplace hub name.
- Oracle Alley is the limited-time Oracle/tarot subset.
- Madame Solange Lumiere is the Oracle/tarot NPC/event host and should not be repurposed into a general helper.
- Account rank titles from Novice through Sorcerer Supreme are locked in `LOCKED_DECISIONS.md`.
- Rank is now Aura-derived, not Level-derived. XP fills Level; Level contributes to Aura Strength; Aura Strength determines Rank.
- Aura Strength is intended to combine approved account-strength signals such as Level/XP history, gameplay/account progress, collection/restoration progress, social contribution, and a small capped purchase contribution.
- Purchases may contribute to Aura only as a capped support signal and cannot independently cause rank advancement.
- Rank benefits are locked to four lanes: Identity, Daily Comfort, Social Help Capacity, and Rank-Up Chest.
- Rank benefits must not improve core gameplay odds, jackpot odds or values, rare-card odds, ingredient drop odds, trade value, competitive scoring, or room/realm progression requirements.
- Rank benefit philosophy is confirmed: early ranks should mostly unlock identity/cosmetics, mid ranks should add modest convenience and social capacity, late ranks should increase capped daily comfort, and very late ranks should emphasize prestige/status rather than gameplay power.
- Landscape orientation is locked for gameplay, Grimoire, potion, reward, social, and background systems unless explicitly changed.

## Strong But Not Fully Locked Preferences

- The player home is conceptually a Witch's Den, while the current prototype labels it Player's Den.
- Realm structure is strongly current as four room/world locations: three regular and one special/finale.
- Rooms should remain revisitable after restoration for ingredients and progression support.
- Broken/restored backgrounds should use the same camera angle and landmarks.
- Visual style should be bright magical storybook/casual-game UI rather than dark or painterly.
- Gilded/Ancient distribution by set range is strong/current but still needs economy validation.
- Coven Ritual, Ritual Board, Ritual Calls, Ritual Marks, Sigil Sets, Coven Points, Circle Rankings, Coven Orbs, Coven Emporium, and Ritual Summary are strong/current names.
- Oracle Alley should be scarce, chance-based, and limited-time.
- Reward tables and odds should be remote-configurable.
- Current rank benefit working draft caps comfort bonus at 35% by Sorcerer Supreme and uses small social-capacity increases instead of a broad across-the-board reward multiplier.
- `11_RANK_REWARDS_V0.1.md` is the current single review surface for rank benefit values and remaining rank reward questions.
- Current proposed rank comfort bonus targets only safe daily/account sources: Daily Bonus mana, Daily Spin common mana, Mana Cauldron refill/capacity, level-up currency rewards, and Enchanted Trail free-path currency.
- Current proposed social-capacity draft reaches 16 ingredient sends/day and 5 card gifts/trades/day at Sorcerer Supreme, while keeping Coven ingredient wish-list requests at 10 total requested quantity per 48-hour refresh.
- Current proposed rank-up chest categories are power-ups, Clairvoyance, and infrequent stars.
- Unity Addressables are a strong technical preference for downloadable/replaceable seasonal rooms, card/ingredient art, event content, and large UI asset groups, but they are not yet a locked Beta 1 requirement.
- UGS is the preferred Beta backend path unless a concrete blocker appears, especially Authentication, Cloud Save, Economy, Remote Config, Analytics, and Cloud Code. Firebase, PlayFab, custom backend, or hybrid remain fallback options rather than equal first-pass targets.
- Multi-card bingo UI should receive a mobile performance pass around canvas rebuilds, Sprite Atlases, 9-slicing, animation/particle budget, and reusable data-driven UI templates.
- Economy-sensitive reward, inventory, social, daily, Freebie, and purchase actions should trend server-authoritative before production use.

## Unresolved Decisions

- Final economy values for mana, crystals, XP, level rewards, rank rewards, potion rewards, room rewards, jackpot odds, and restore rewards.
- Final Daily Bonus reward values and Day 7 chest contents.
- Final Daily Spin cadence, odds, reward table, and any ticket/ad relationship.
- Final Freebie deep-link format, validation, reward pools, cooldowns, and anti-abuse design.
- Final Inbox tabs, message categories, expiration rules, read/unread behavior, and claim-all behavior.
- Final Book of Shadows scale and paid value proposition.
- Final duplicate card conversion, Joker Wild, Stardust, and trading rules.
- Final card trading send/receive limits, rarity restrictions, cooldowns, and anti-abuse rules.
- Final card gifting limits, recipient model, Friends/Profile integration, and production gift UI.
- Final Ask for Help friend rotation schedule, request expiration, and exact ingredient eligibility limits.
- Final Collection Assist MVP scope.
- Final Coven roles, permissions, join settings, moderation, contribution rewards, and anti-abuse rules.
- Final Aura formula, source weights, purchase cap, and Aura thresholds per rank.
- Final promotion of rank benefit values from working draft to locked economy data.
- Final rank-up chest power-up quantities, Clairvoyance durations, star frequency, cosmetic unlocks, card gift/trade eligibility, rounding rules, and per-rank implementation timing.
- Final Realm 2+ progression, special room cadence, and replay reward rules.
- Final Bazaar catalog, currencies, prices, purchase confirmation behavior, and social/trade boundaries.
- Final Oracle Alley timing, Oracle Dust economy, reward table, odds, and scarcity limits.
- Final backend/profile persistence and remote config implementation.
- Final visual asset pipeline and which mockups are exact layout references versus mood references.
- Whether any concrete blocker forces a move away from the preferred UGS Beta backend path toward Firebase, PlayFab, custom backend, or hybrid.
- Final content delivery plan: bundled assets vs Addressables/downloadable content, asset-key naming, catalog versioning, and first-install size targets.
- Final UI technology/performance plan: UGUI-only, selective UI Toolkit use, canvas isolation strategy, atlas grouping, 9-slice standards, and mobile performance targets.
- Whether real-time shared rooms/synchronized number calls are product scope, or whether Beta can remain local-simulated plus asynchronous social systems.

## Outdated or Replaced Ideas

- Cocos Creator is no longer the active implementation path; it is archive/reference.
- Bingo Magic Blast is an older working title; Bingo Magic Mayhem is current.
- Mystical Lands is older terminology; rooms/worlds inside Realms are the current direction.
- Threshold Board closing after completion conflicts with newer/current revisitable room direction.
- Older 25-set Grimoire references are superseded by the 32-set / 320-card lock.
- Older rare/extra-rare language should be translated carefully to Gilded/Ancient frame language.
- Older dark, muddy, or painterly environment direction should not override the brighter storybook/casual-game target.
- Balthazar's Bazaar, Mayhem Market, and Bewitched Bazaar are deprioritized in favor of Bewitchment Bazaar.
- Automatic sharing is rejected; sharing is manual.
- Madame Solange should not be used as a quest coach, Daily Bonus guide, Coven prompt, or album helper.

## Archive Conflicts To Watch

- `Game Systems Roadmap v0.1.md` says Freebies rewards return through Inbox > Gifts, but later user instruction and current locked docs say Freebies are direct social/deep-link redemption and not Inbox.
- `covens.txt` lists max Coven size as still needing final design lock, while current `LOCKED_DECISIONS.md` says Covens cap at 50 members.
- The Word handoff rank benefit table uses Apprentice, Spellcaster, Wizard, Archmage, Oracle, while current rank bands use Novice, Apprentice, Spellbinder, Mage, Thaumaturge, Mystic, Enchanter, Wizard, Spellmaster, Archmage, Grand Archmage, Paragon, Ascendant.
- The user's earlier exploratory 2%-50% rank multiplier scale should not be implemented as a universal reward multiplier; the newer direction favors a capped 0%-35% comfort bonus on limited safe systems only.
- Older Realm design uses 4-5 Mystical Lands and Threshold Boards; current direction uses four room/world locations and special/finale rooms.
- Older album handoffs use 25-set Grimoire and Astral/Ancient rarity language; current handoff uses 32-set Grimoire and Regular/Gilded/Ancient frames.
- Older Cocos prototype says first playable Realm is Whispering Fen; current Unity prototype uses Sunpetal Conservatory / Gilded Azalea Arboretum.

## Risks for Future Implementation

- Reward routing can regress if Inbox is treated as a universal reward destination. Daily Bonus, Daily Spin, Freebies, and jackpot collection each have separate claim rules.
- `applyRewardGrant`-style helpers need explicit claim semantics so Inbox gifts are not automatically granted before claim.
- The prototype currently has many systems in `BingoPrototype.cs`; adding more social/collection features there will increase regression risk.
- Collection Assist touches Covens, album state, ingredient state, requests, gifts, trading limits, and Inbox; it should be built in narrow passes.
- Existing prototype code/UI still derives Rank directly from Level and must be revised to the Aura model before production hardening.
- Rank and level terminology can confuse UI and economy unless Level, Aura Strength, and Rank are kept separate.
- Rank reward implementation can create unfairness if the comfort bonus is accidentally applied to gameplay odds, jackpot values, rarity odds, drop odds, trade value, or competitive scoring.
- Grimoire card rarity, ingredient card identity, and potion inventory can blur together; UI must distinguish collection browsing from ingredient management.
- Oracle Alley can damage scarcity if reward odds, limits, and currency sinks are not designed before implementation.
- Freebie deep links need expiration and duplicate prevention before public/social use.
- Remote-tunable economy tables should be introduced before locking final values into code.
- Backend guidance should treat UGS as the preferred Beta backend path, but should not deeply couple gameplay/economy/social code to UGS SDK calls without service facades and should not lock Photon, Netcode, or true multiplayer before the product scope decision is made.
- Addressables and remote content need catalog/versioning discipline, otherwise seasonal assets and card art can become difficult to migrate.
- Multi-card UI effects can become a mobile performance bottleneck if daub animations, particles, and reward overlays rebuild too much static UI.
- Client-side visuals, animation, sound, and daub feedback should stay decoupled from server-owned gameplay/reward truth.
- Documentation may drift again unless `CURRENT_PROTOTYPE_STATE.md` and the numbered verified docs are updated after major decisions.

## Verified Supplemental State — 2026-10-06: Sunpetal Asset Handoff

This is a focused asset/documentation update. It does not revalidate the earlier full gameplay/backend audit or promote working preferences into locks.

### Confirmed

- The current project chat supplied five Sunpetal Conservatory background PNGs: `fullBroken.png`, `realm1restore.png`, `realm2restore.png`, `realm3restore.png`, and `st-fullrestore.png`.
- All five source files are available and measure 1920 x 1080.
- The user will provide navigation and title graphics after background placement.
- The saved repository directory is `C:\Users\nLuis\Documents\Bingo Magic Mayhem`; the nested `Bingo Magic Mayhem` directory is the active Unity project.
- Root `AGENTS.md` and root `00_READ_ME_FIRST.md` exist and were read. The nested `docs/00_READ_ME_FIRST.md` remains absent.

### Strong preferences and observations

The supplied sequence visibly restores the left pavilion, upper-right terrace, lower-right pavilion, and finally the foreground crystal/environment with an active portal. This describes artwork only; exact named-room ownership and triggers are not confirmed. The broad camera composition is consistent; transition registration remains untested.

### Unresolved

Background repository placement, Unity import, runtime keys/state selection, named-room and hotspot mapping, transition behavior, and crop/safe-area testing remain pending. Navigation/title assets are not yet supplied in this handoff. See [the intake record](21_SUNPETAL_BACKGROUND_STATE_INTAKE.md).

### Outdated or replaced documentation statements

The earlier missing-AGENTS claim in `09_OPEN_DECISIONS.md` is obsolete: the file is at repository root. The current prototype entry point already references the numbered audit set. Older source packets and archive exports do not override explicit current asset context; neither do these images override locked gameplay rules.

### Future implementation risks

Intermediate filenames could be mistaken for separate realms; background receipt could be mistaken for completed integration; detail shifts could make transitions jump; retained thorns could read as remaining damage; bright scene detail could reduce overlay readability. These are review risks, not new product decisions.

No game code or gameplay/economy decisions changed in this update.

### Sunpetal Import Follow-Up — 2026-10-06

The five supplied backgrounds have replaced the existing `sunpetal-realm-broken0`, `broken1`, `broken2`, `broken3`, and `restored` PNGs in `Assets/Resources/prototype/ui/realm_home/sunpetal`. Image hashes and preservation of all existing metadata were verified. Existing restoration-count selection and Resources loading already address these files; no code changed. Repository placement and runtime-path discovery are complete, superseding the earlier pending status. Unity editor reimport/on-screen verification and title/navigation overlay review remain pending. See `21_SUNPETAL_BACKGROUND_STATE_INTAKE.md` for the exact mapping.

### Sunpetal Production Asset Supersession — 2026-10-06

The active Sunpetal background slots now contain the newer production-tree set: `sunpetal-realm-broken.png` maps to `broken0`, numbered files `1` through `3` map to `broken1` through `broken3`, and `sunpetal-realm-restored.png` maps to `restored`. Each installed PNG is 1920 x 1080 and hash-identical to its supplied source. Existing `.meta` files are hash-identical to the preserved pre-swap metadata, so GUIDs and importer settings remain stable. The prior installed set is recoverable from `C:\Users\nLuis\Documents\Codex\2026-10-06\i\work\sunpetal-before-prod-assets-import`. Existing Resources loading and restored-room-count selection remain unchanged; no gameplay code or design decisions changed. Unity editor/on-screen verification and navigation/title overlay review are still pending.

### Sunpetal Realm-Home UI Integration — 2026-10-14

Confirmed: the user supplied 31 Sunpetal realm-home UI PNGs comprising two nine-tile frames, four potion/shadow pairs, two arrows, a round medallion frame, a lock image, and a potion-count badge. They are installed under `Assets/Resources/prototype/ui/realm_home/sunpetal/ui`. The re-supplied five-background sequence is also installed in the existing state slots, and every background source/destination hash matches.

Confirmed implementation state: Sunpetal alone now receives the supplied realm header, restoration-state potion row, room labels, medallions, and arrows. Existing room availability, restored-room state, realm gating, and entry callbacks remain authoritative. Other realms retain the generic renderer.

Strong but not fully locked presentation mapping: the supplied `sunpetal-conservatory`, `thorn-tangled`, `pollenspire`, and `solstice-bloom` potion art is associated in that order with the four current Sunpetal room definitions. This is reversible presentation wiring and does not create a gameplay lock.

Verified tooling state: Unity imported the supplied files, generated metadata, and completed its script recompile/domain reload with no compiler errors in `Editor.log`. Unresolved: runtime composition review, safe-area/device scaling, text fit, hit targets, disabled arrow styling, and five-state transition registration. The machine has no standalone .NET SDK, so `dotnet build` could not validate the project independently.

Outdated/replaced: the 2026-10-06 production-tree Sunpetal backgrounds are no longer the active files; the user-re-supplied `fullBroken`, `realm1restore`, `realm2restore`, `realm3restore`, and `st-fullrestore` sequence supersedes them.

Risk: high-resolution tiled UI can increase import time and texture memory, and seams may appear if filtering/compression or runtime scaling differs between tiles. Validate Unity importer settings and assembled edges before treating the presentation as final.

First-pass polish now reuses the existing production world-map resource counters and home button in the Sunpetal top-right navigation. Room-state words are intentionally omitted: full potion, shadow potion, and lock art communicate restored, unfinished-unlocked, and locked states respectively. Unfinished unlocked rooms display combined ingredient progress on the supplied count badge. This presentation mapping does not revise room-state authority.

Measured-layout follow-up: the Sunpetal header, arrows, four room nodes, medallions, labels, progress badge, resource counters, and home button are positioned independently against the supplied 1920 x 1080 full-stack reference rather than distributed by a generic layout. Adjacent nine-tile pieces overlap by two pixels to suppress filtering seams while preserving the original one-third tile steps.

A `Sunpetal Mixed QA` prototype-settings preset is available to stage one restored room, one unlocked incomplete room with partial ingredient progress, and two locked rooms on the broken-one background. Applying it intentionally replaces local Sunpetal room/ingredient progress; it is a QA convenience only and does not change progression rules.

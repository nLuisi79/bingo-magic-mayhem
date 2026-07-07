# 02 — Core Loop and Realms

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
| Enter room → choose card count/mana bet → play bingo → earn rewards/progress | LOCKED / HIGH | Use as main loop. |
| 1 / 2 / 4 / 6 card count options on room-entry | LOCKED UI RULE | Use in room-entry screen. |
| Four rooms per realm | LOCKED / HIGH | Use as current realm model. |
| Three regular rooms + one special room | HIGH / LIKELY FINAL | Use as current structure, keep mode configurable. |
| Special room as restoration/finale room | HIGH / LIKELY FINAL | Use as progression concept. |
| Blackout / multi-bingo special format | PROPOSED | Keep configurable; do not hardcode. |
| Rooms remain replayable for ingredients | HIGH / LIKELY FINAL | Do not close completed rooms permanently. |
| Realm restoration via spells/potions/ritual | HIGH / LIKELY FINAL | Use as current design model. |
| Portal progression/open portal state | HIGH | Use as visual/progression state, but older Portal Surge mechanics are separate. |
| Witch revisit / weekly challenge | PROPOSED | Do not implement as required without approval. |

## Locked core-loop decisions

## 2. Core gameplay loop

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Player enters a realm/room, chooses card count and mana bet, plays bingo, earns rewards, and advances collections/realm progress. | Primary loop | Later approved room-entry layout locks card selection, mana bet, active power-ups, ingredient progress, restore rewards, and jackpot wheel in the same setup flow. | Room-entry screen must make play setup clear; progress should be visible but not dominate the core action. | Earlier terms like buy-in, coins, and energy exist, but later UI specifically uses mana bet per card. |
| Card-count options on the locked room-entry screen are 1, 2, 4, and 6. | Match setup | The later locked visual target explicitly lists 1 / 2 / 4 / 6 card options as four columns across the lower screen. | Keep the four-column lower-screen layout unless intentionally redesigning UX. | This locks the visible room-entry options; it does not fully define advanced unlocks or maximum cards outside this screen. |
| Active power-ups belong in the bottom-right area of the room-entry structure. | Power-up UX | The later locked layout rules explicitly preserve Active power-ups at bottom right. | Power-up UI should be readable, touch-friendly, and visually distinct from card-count and mana controls. | Specific individual power-up list is not locked in the available later material. |
| Called balls do not belong on the room-entry/setup screen. | UX exclusion | Later discussion corrected that called balls are irrelevant to the screen where players choose bet/card count. | Called-number information belongs to active bingo gameplay, not room entry. | The active bingo play screen still needs called-number display rules. |

## 3. Realms/world progression

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| The current realm model is four rooms per realm. | Realm structure | Later discussion explicitly moved to each realm having four rooms and the locked realm handoffs for Everbloom and Sunpetal each capture four room-entry screens. | Design realm maps and progression around four places/rooms per realm. | Older 4-5 Mystical Lands / Threshold Board model should not be treated as current lock unless re-approved. |
| Three rooms are regular bingo rooms and one room is a special/finale room. | Realm structure | Later discussion favored three regular play rooms plus one special room, with the special room hosting the ritual/restoration moment. | Special room should feel distinct and may support special bingo format or final ritual function. | Exact special-room bingo mode is still open. |
| Realm restoration is tied to completing powerful spells/potions and then casting them in the special room. | Progression logic | Later discussion replaced simple room closure with a model where three powerful spells restore the realm and open the portal to the next world. | Regular-room potion/spell progress must feed into the special-room restoration ritual. | Exact number of potions per realm appears to be three regular-room potions plus special-room ritual, but needs final systemic wording. |
| Rooms should remain accessible after completion so players can revisit if ingredients are needed. | Replay/access | Later user direction corrected the idea of closing regular rooms and favored keeping rooms open for ingredient needs. | Completion states should change reward/progression behavior without making ingredients inaccessible. | There may still be special reward states after restoration; exact replay rewards remain open. |
| A restored realm can have a final open-portal state. | Realm state | Everbloom Sanctuary background work included the final view with the open portal in the back of the realm and was locked before moving forward. | Realm background states should support broken/restored/open-portal progression where applicable. | Exact number and names of states per realm may vary by realm. |

## 4. Rooms/places

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Everbloom Sanctuary is a locked realm with approved map, four room-entry screens, potion/ingredient sheet, and visual constraints. | Realm/place | The Everbloom Sanctuary Locked Realm Handoff was created and verified, explicitly capturing those items. | Future Everbloom work should follow its locked map, rooms, potion/ingredient sheet, and style constraints. | Exact room list is not fully reproduced in the available snippets; rely on the locked handoff for names/assets. |
| Sunpetal Conservatory is a locked realm with locked map, four room-entry screens, potion/ingredient sheet, and not-fully-restored state guidance. | Realm/place | The Sunpetal Conservatory Locked Realm Handoff was created and rendered with those contents. | Do not redo Sunpetal as if undefined; it is already covered. | Exact room list is not fully reproduced in the available snippets; rely on the locked handoff for names/assets. |
| Blossomveil Promenade is the approved Everbloom Sanctuary room-entry screen and current visual calibration screen. | Room/screen | The later visual target names Blossomveil Promenade as the approved style calibration screen. | Use it as the anchor for room-entry layout, UI density, background rendering, jackpot wheel, ingredient strip, restore reward placement, card selection, mana, and active power-up structure. | This is a style/layout lock, not necessarily the final pattern for every gameplay screen. |
| Gilded Azalea Arboretum exists as a room/place concept within Sunpetal Conservatory. | Room/place | Later room landing page work specifically refers to Gilded Azalea Arboretum in Sunpetal Conservatory. | Should respect the locked room-entry purpose: card amount, mana bet, play, visible potion progress, and jackpot. | Need verify final locked room-entry asset from Sunpetal handoff before treating every detail as final. |

## Complete realms and world progression extraction

Inventory-level design: confirmed decisions, proposed structures, contradictions, and developer guardrails

| Scope and source guardrails<br>This document captures every known decision and unresolved idea available in the current Project materials related to realms, rooms, realm spells/potions/artifacts, ingredients, restoration, portals, replay, and world progression. It does not invent exact room lists, drop rates, or event tuning where the available source material does not surface them. Later locked realm/room-entry direction takes priority over early developer placeholder architecture, but older terms are retained in the contradiction and migration sections so developers know what not to assume. |
| --- |

## 1. Executive model

| Area | Current best design reading |
| --- | --- |
| Realm unit | A Realm is the world/chapter container. Later direction centers the current model on four rooms per realm rather than the older 4-5 Mystical Lands model. |
| Room count | Each current realm should be structured around four places/rooms. Locked realm handoff references for Everbloom Sanctuary and Sunpetal Conservatory each capture four room-entry screens. |
| Room roles | Three rooms are regular bingo rooms. One room is a special/finale room. The special room hosts the restoration ritual and may use a special bingo mode such as blackout or multi-bingo. |
| Room access | Rooms should remain accessible after completion so players can replay for ingredients. The older rule that finale/Threshold boards close is not safe to implement as final. |
| Progression object | Current room-facing progression is potion/spell/ingredient based. Older artifact-fragment language may survive as relic/flavor, but should not replace the later potion/ingredient structure without a lock. |
| Realm restoration | Regular-room potion/spell completion feeds into a special-room ritual. Restoring the realm can unlock or display a final open-portal state and lead to the next world. |
| Portal progression | The final portal model is unresolved. The current map/background direction supports an open portal state, while older architecture includes Threshold Boards, Portal Reawakening, Portal Surge, and Enchanted Keys. |
| Replay purpose | Replaying rooms is needed for ingredients, collection drops, rewards, and possibly weekly challenge variants after restoration. |

## 2. Confirmed / later-locked realm decisions

| Decision | Status | Design consequence | Caveat |
| --- | --- | --- | --- |
| Four rooms per realm | Confirmed by later discussion and locked handoff references | Realm maps, room-entry assets, progression gates, and world-state art should be planned around four places per realm. | Exact room names for every locked realm are not fully surfaced in the searchable snippets; do not invent them. |
| Three regular rooms + one special/finale room | Strong later decision | Regular rooms produce potion/spell progress; special room delivers the ritual/finale and may use a special bingo format. | Exact special-room format is still open. |
| Rooms remain accessible after completion | Explicit later correction | Do not make regular rooms permanently inaccessible after restoration; players need ingredient replay access. | Replay reward tables still need tuning. |
| Potion/ingredient sheets belong to realms | Confirmed by locked Everbloom and Sunpetal handoff references | Every realm handoff should include potion/ingredient sheet data and assets. | Exact drop logic belongs to the Ingredient / Potion Drop Rules handoff and needs the full locked doc for implementation. |
| Room-entry screen includes ingredient progress and restoration item | Confirmed by locked visual target | Room-entry UI must show potion/restoration item, ingredient progress strip, restore reward, card count, mana bet, power-ups, and jackpot wheel. | This is a room-entry lock, not yet the active bingo screen lock. |
| Everbloom Sanctuary is a locked realm | Confirmed by locked realm handoff reference | Preserve approved Everbloom map, four room-entry screens, potion/ingredient sheet, and visual constraints. | Exact order among realms is unresolved. |
| Sunpetal Conservatory is a locked realm | Confirmed by locked realm handoff reference | Preserve locked Sunpetal map, four room-entry screens, potion/ingredient sheet, layout constraints, and not-fully-restored guidance. | Exact order among realms is unresolved. |
| Blossomveil Promenade is approved style calibration | Confirmed by later visual lock | Use as the visual anchor for room-entry density, layout, background rendering, jackpot placement, ingredient strip, restore reward, card selection, mana, and power-up placement. | It is not proof that every room uses identical background art or title treatment. |
| Open portal state exists for restored realm presentation | Confirmed by Everbloom background-state work | World backgrounds should support progression from broken/restored toward open-portal completion where appropriate. | Exact state count and naming per realm still need lock. |
| Restore rewards should be mana and card packs, not chests | Confirmed by later correction | Realm/room restoration reward tables and UI should not show generic chests as the restore payout. | Exact quantities and pack types are not tuned. |

## 3. Core player progression through worlds

| Step | Player-facing action | System result | Implementation note |
| --- | --- | --- | --- |
| 1 | Player enters the current Realm map. | The map shows available rooms/places and their visual restoration state. | Realm should have availability states and a current/next-room pointer. |
| 2 | Player enters one of the three regular bingo rooms. | Room-entry screen shows room title, potion/restoration item, ingredient progress strip, restore reward, jackpot wheel, card-count selection, mana bet, and active power-ups. | Use Blossomveil Promenade layout rules as the current reference. |
| 3 | Player chooses 1, 2, 4, or 6 cards and a mana bet, then plays bingo. | Player earns match rewards and room-linked drops. | Called balls belong in the active bingo screen, not room entry. |
| 4 | Player collects room ingredients / ingredient cards. | Ingredient progress advances toward completing a room potion/spell. | Ingredient cards should remain readable and collectible, with title/stars where card identity is shown. |
| 5 | Player completes required regular-room potions/spells. | The realm approaches restoration readiness. | Current reading: three powerful spells/potions feed the special room. Exact wording must be locked. |
| 6 | Player enters the special/finale room. | Special room hosts the ritual/finale and may use blackout or multi-bingo style play. | Special mode is unresolved; do not hard-code one format. |
| 7 | Player casts/completes the realm ritual. | Realm restores and/or opens portal state. | Can trigger realm completion reward: mana + card packs. |
| 8 | Player advances to the next Realm / world. | Next realm unlocks or portal path becomes available. | Final portal mechanics are unresolved; map state should support portal-open visual. |
| 9 | Player may replay rooms. | Replay supports ingredient needs, additional rewards, collection drops, and possible weekly witch challenge variants. | Do not lock players out of needed ingredient sources. |

## 4. Realm data model

Realm ID and internal key.

Realm display name.

Realm theme and story description.

Realm order / world sequence position.

Unlock requirement: previous realm restored, portal opened, player level, collection state, event state, or manual unlock flag.

Four room/place entries: three regular rooms and one special/finale room.

Realm visual states: locked, available, unrestored/broken, partially restored, restored, and open-portal where used.

Realm potion/spell set or restoration recipe list.

Ingredient item list and drop ownership by room.

Completion criteria: all required room potions/spells + special-room ritual result.

Realm completion reward table: currently constrained to mana and card packs unless specifically expanded.

Portal/next-world link and portal presentation state.

Replay state: what changes after restoration, what remains farmable, and what rewards are reduced, boosted, or replaced.

Weekly challenge eligibility: whether a witch revisit or timed challenge can target the realm.

Live-ops availability: standard, event-highlighted, replay-only, or hidden/retired.

## 5. Room / place model

| Field | Regular bingo room | Special/finale bingo room |
| --- | --- | --- |
| Count per realm | Three. | One. |
| Core purpose | Primary repeatable bingo play and ingredient/potion progress. | Finale, ritual, restoration, and special reward moment. |
| Bingo format | Standard bingo unless a room-specific variation is later locked. | Candidate formats include blackout or multi-bingo style play; final format unresolved. |
| Room-entry UI | Uses locked room-entry layout: title card, potion image, ingredient strip, restore reward, card count, mana bet, power-ups, jackpot wheel. | Should share the same core UI but may emphasize ritual/finale state more strongly. |
| Ingredients | Primary source for room-tied ingredient drops. | May consume/validate completed potions/spells or drop special ritual ingredients if later locked. |
| Completion role | Completes one room potion/spell or contributes one major spell toward realm restoration. | Completes the realm ritual/restoration and opens the portal/next realm state. |
| Replay after completion | Remain open for ingredient needs and reward play. | Replay rules unresolved; may stay open for weekly challenges or special reward farming. |
| Closure rule | Do not close permanently. | Do not assume older Threshold Board closure rule; special-room closure/replay needs explicit lock. |

## 6. Known realms and places

### 6.1 Locked / later-current realms

| Realm / place | Known status | Known content | Open detail |
| --- | --- | --- | --- |
| Everbloom Sanctuary | Locked realm handoff created and verified. | Approved realm map; four room-entry screens; potion/ingredient sheet; visual constraints; background-state work including restored/open-portal direction. | Exact room list and final progression order are not fully surfaced in the searchable snippets. |
| Blossomveil Promenade | Approved room-entry screen and visual calibration anchor. | Defines current style/layout target for room-entry UI, jackpot placement, ingredient progress strip, restore reward, card selection, mana bet, and active power-up structure. | Its room function within the four-room structure should be preserved from the locked Everbloom handoff. |
| Sunpetal Conservatory | Locked realm handoff created and rendered. | Locked map; four room-entry screens; potion/ingredient sheet; layout constraints; not-fully-restored state guidance. | Exact final realm order vs Everbloom is not locked in available snippets. |
| Gilded Azalea Arboretum | Named Sunpetal room/place concept. | Discussed as a room landing page where player chooses card amount and plays bingo while seeing potion progress and jackpot. | Need verify final locked Sunpetal room-entry asset before implementation. |

### 6.2 Older sample realms / not safe as final

| Developer warning<br>The following older realm chain is useful as source context and possible future inspiration, but it must not be treated as the current locked world order because later work locked Everbloom Sanctuary and Sunpetal Conservatory around a four-room/potion-ingredient model. |
| --- |

| Order | Older realm name | Older artifact | Older finale board | Current status |
| --- | --- | --- | --- | --- |
| 1 | Whispering Fen | Mirelight Compass | Sunken Waygate | Early architecture / not later confirmed as current Realm 1. |
| 2 | Shattered Observatory | Starfall Astrolabe | Constellation Gate | Early architecture / not later confirmed as current Realm 2. |
| 3 | Clockwork Spire | Brass Meridian Key | Chronolock Chamber | Early architecture / possible future inspiration only. |
| 4 | Dragonwake Ruins | Ashen Scale Talisman | Hoardwake Chamber | Early architecture / possible future inspiration only. |
| 5 | Frostveil Peaks | Frostvein Chalice | Glacier Gate | Early architecture / possible future inspiration only. |
| 6 | Mirage Dunes | Sandglass Lens | Sunspun Mirage | Early architecture / possible future inspiration only. |

## 7. Ingredients and room collection progression

Ingredients are room/realm progression items, visually treated as collectible magical cards/icons rather than generic materials.

Each potion/spell appears to use a 10-ingredient-card structure in later discussions. This aligns with the broader 10-card collection-page pattern, but the exact universal rule should still be locked for all realm potions.

Ingredient progress is visible directly on the room-entry screen as a horizontal strip beneath the title area.

Ingredient icons in progress rows should be glossy, magical, readable at small size, and centered in round frames.

Ingredient cards shown in completion/redeem states should look like actual redeemed cards and show at minimum the title and stars.

Ingredient checkmarks were rejected for the potion completion display.

A compact room-entry view may show ingredient icons/fill levels; an expanded ingredient view should provide detailed inventory status.

Ingredient gifting/requesting is planned as a future social/Bazaar/Coven function and should be considered in the inventory data model.

Rooms must remain replayable so players can return to ingredient sources after progression or restoration.

### 7.1 Known ingredient examples surfaced in Project context

| Potion / context | Known ingredient names |
| --- | --- |
| Warlock Binding Brew | Warlock’s Binding Cord; Blackthorn Knot Tips; Witchsalt Circle Crystals; Oathbreaker Ash; Iron Sigil Filings; Moonless Ink Drops; Hawthorn Thread Fibers; Shadowwax Sealings; Crowcall Feather Barbs; Binding Circle Essence. |
| Saltcircle Ward Tonic | Saltcircle Crystals; Threshold Chalk Dust; Moon-Blessed Brine Drops; Protective Sage Ash; Hearthline Rosemary Sprigs; Iron Boundary Filings; Circlewax Sealings; Four-Corner Pearl Dust; Warding Bell Chime Shards; Sanctuary Circle Essence. |
| Iron Pin Uncrossing Potion | Iron Uncrossing Pins; Crossroad Salt Crystals; Jinxroot Shavings; Red Thread Snippings; Sharpthorn Needle Tips; Unhexing Sage Ash; Rustless Iron Filings; Knotbreak Brine Drops; Mirrorback Pearl Dust; Uncrossing Spark Essence. |
| Hexmark Revealing Ink | Hexmark Ink Drops; Revealer’s Quill Barbs; Moonlit Soot Dust; Truthglass Shavings; Witchlight Pearl Dust; Sigilroot Fibers; Crow-Eye Dewdrops; Unmasking Salt Crystals; Obsidian Inkbinder. |
| Sunpetal / Everbloom examples | Gilded Azalea Petals; Sunwarm Dewdrops; Honeyglow Pollen; Amberroot Shavings; Golden Heartseed; Velvet Thorn Tips; Terrace Moss Tufts; Rosegold Sap Drops; Thornheart Bud; Bramblelace Fibers; Spire Pollen Grains; Fountain Dewdrops; Nectarflow Ribbons; Lilygold Filaments; Pollen Dust; Sunlit Glass Slivers; Golden Hour Motes; Prismvine Twine; Dawnmirror Fragments; Golden Stamen; Veilpetal Blossoms; Roselight Essence; Marblemoss Threads; Petalsheen Dust; Heartbloom Petals; Graceyard Soil Cakes; Polished Trowel Charms; Terracotta Mender’s Clay; Rootwax Binding Cord; Keepsake Clover; Luminara Waterglass; Silver Basin Coins. |

## 8. Realm spells, potions, artifacts, and relics

### 8.1 Current potion/spell-forward model

Later world-progression discussion shifted the core restoration model toward completing powerful realm spells/potions.

Current best model: three regular rooms each feed one powerful spell/potion, and the special room is where those spells/potions are cast to restore the realm.

The room-entry title area should show a potion/restoration item image to the left of the room title, replacing meaningless decorative emblems.

Potion completion should trigger a ceremonial completion overlay: potion beside banner, title + Complete, redeemed ingredient cards arched like a tarot spread, then rewards below.

Realm completion rewards should be mana and card packs rather than chests.

### 8.2 Artifact/relic language

Older architecture restored Enchanted Artifacts through fragments and used the restored artifact to foreshadow the next realm.

Later Player Den / Relic Wall ideas include restored Realm relics, achievement badges, event trophies, and Coven contribution honors.

Current interpretation: artifacts/relics may remain as display/story rewards, but potion/ingredient progression is the room-facing restoration mechanic unless explicitly changed.

Developers should not implement artifact fragments as the primary current realm progression system without a fresh lock.

## 9. Special room design

| Topic | Known decision / idea | Implementation guardrail |
| --- | --- | --- |
| Role | One special room exists per realm and functions as a special/finale room. | Treat it as the fourth room, not an unrelated modal, unless UX later changes. |
| Restoration | Special room is where the completed regular-room spells/potions are cast to restore the realm and open the portal to the next world. | Store a ritual-ready state when required room potions are complete. |
| Mode candidate | Blackout or multi-bingo style play was discussed as a possible special-room format. | Do not lock blackout or multi-bingo until explicitly chosen. |
| Rewards | Special room can support larger/rarer rewards than regular rooms and may be the replayable reward location after restoration. | Exact payout table is not tuned. |
| Weekly challenge | Special room may be where a visiting witch returns, breaks/challenges something, or offers weekly bigger rewards. | Do not implement witch revisit as core without a lock, but leave event hooks. |
| Replay | Special room replay state is unresolved but should not inherit older Threshold closure behavior automatically. | Add config flags for always open, event-only, replay-reduced rewards, or challenge state. |

## 10. Portal progression

| Portal concept | Known status | Design notes | Open question |
| --- | --- | --- | --- |
| Open portal realm state | Later Everbloom background-state work supports a final view showing an open portal in the back of the realm. | This is the clearest later-current portal expression: visual/world progression after restoration. | Exact portal interaction and next-world unlock flow. |
| Next realm unlock | Core progression requires moving from one world/realm to the next. | Restoration and/or ritual completion should unlock or reveal the next realm. | Whether unlock also needs player level, collection progress, or resource spend. |
| Threshold Board | Older architecture term for special finale board. | May map loosely to the current special/finale room but is not safe as final terminology. | Rename, retire, or use only internally? |
| Portal Reawakening | Older architecture event for reopening Threshold Boards. | Could inspire event replay of restored special rooms. | Is this feature retained, renamed, or cut? |
| Portal Surge | Older rare jackpot-style feature triggered by Enchanted Key. | Reward pots: Rune, Astral, Eternal, Ancient. Designed to be distinct from standard prize wheel. | How it relates to the later locked jackpot wheel. |
| Jackpot wheel | Later room-entry layout locks top-right jackpot wheel placement. | Player should see a potentially large jackpot number on room-entry screen. | Whether jackpot wheel and Portal Surge coexist. |

## 11. Replaying rooms for ingredients

Replaying rooms for ingredients is a later-current requirement: rooms should not close in a way that prevents players from collecting needed ingredients.

A completed room can change state visually or in reward weighting without becoming inaccessible.

Replay can support: missing ingredient drops, duplicate ingredient conversion, room reward farming, collection drops, jackpot wheel participation, power-up use, and daily/weekly tasks.

Ingredient inventory should track room source, owned count, needed count, completion status, duplicate state, giftability/requestability, and conversion eligibility if duplicate ingredients are later allowed to convert.

Replay should be data-driven: per-room drop tables, post-completion drop modifiers, and event modifiers must be remote-configurable.

## 12. Witch revisit / weekly challenge idea

| Idea | Known discussion | Status / guardrail |
| --- | --- | --- |
| Weekly witch visit | A witch may revisit completed rooms/realms weekly, creating a bigger reward opportunity. | Proposed; do not implement as final without lock. |
| Room breaks / realm disturbance | The visiting witch could break or disturb a completed realm/room, giving purpose to replaying restored rooms. | Proposed; needs tone and UX lock so it does not feel punitive. |
| Special spell remake | Players could remake a special spell as often as they want to earn room rewards and boost daily earnings. | Proposed; needs limits and reward rules. |
| Large weekly reward | Weekly witch reward could be bigger than normal room rewards. | Proposed; exact payout unresolved. |
| Limited-time rewards | Possible reward examples included limited-time effects such as unlimited power-up for 48 hours. | Proposed; high balance risk. |
| Special room as challenge hub | The special room may be the natural place for weekly revisit/challenge content. | Likely direction, but still unresolved. |

## 13. Realm completion rewards

Confirmed constraint: restore rewards should be mana and card packs, not chests.

Room-entry UI reserves a restore reward area on the right side of the ingredient progress strip.

Realm completion should be a celebratory state change, not only a currency payout.

Likely completion outputs: mana reward, card pack reward, realm restored visual state, portal opened, Player Den / Relic Wall relic unlock, possible collection progress, and next realm access.

Exact amounts, pack types, rarity boosts, first-time-only vs repeat rewards, and post-restoration replay rewards are unresolved.

## 14. Visual progression and background states

| State | Meaning | Known use / note |
| --- | --- | --- |
| Locked | Realm unavailable. | Should show future content without implying playable access. |
| Unrestored / broken | Realm has magical damage, mayhem, or incomplete restoration. | Everbloom background-state work included broken/restored direction. |
| Partially restored | Some rooms/potions are complete but final ritual not done. | Sunpetal handoff references not-fully-restored state guidance. |
| Restored | Realm core restoration complete. | All four restored views were discussed/approved during background work. |
| Open portal | Restored realm now shows portal to next world. | Final Everbloom view included open portal in back of realm. |
| Event disturbed | Optional future state for witch revisit / weekly challenge. | Proposed only; do not assume production requirement. |

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

## 16. Proposed implementation configuration

### 16.1 Realm config keys

realm_id

display_name

theme_key

world_order

unlock_rule_id

previous_realm_id

next_realm_id

realm_state

visual_state_key

rooms_regular_ids

room_special_id

potion_set_ids

ingredient_sheet_id

restoration_ritual_id

portal_state_key

completion_reward_table_id

replay_reward_table_id

weekly_challenge_eligible

event_overrides

### 16.2 Room config keys

room_id

realm_id

display_name

room_role: regular or special

bingo_mode: standard / blackout / multi-bingo / other

entry_cost_table_id

mana_bet_options

card_count_options

ingredient_drop_table_id

potion_or_spell_id

restore_reward_preview_id

jackpot_config_id

powerup_rules_id

completion_state

replay_availability

post_completion_modifiers

weekly_challenge_state

### 16.3 Ingredient config keys

ingredient_id

display_name

realm_id

room_id

potion_or_spell_id

rarity_or_star_value

icon_asset_id

card_frame_asset_id

required_count

owned_count

drop_source_rules

giftable_flag

requestable_flag

duplicate_behavior

conversion_behavior

marketplace_visibility

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

## 19. Summary of current safest design implementation

| Implementation-safe baseline<br>Build the realm system around four-room realms, with three standard bingo rooms and one configurable special/finale room. Keep all rooms accessible after completion. Use ingredient/potion/spell progress as the current room-facing restoration mechanic. Treat artifacts/relics as narrative/display rewards unless re-locked. Show ingredient progress and restore rewards on room-entry. Let restoration open a portal visual/state and unlock the next realm. Keep Portal Surge, Threshold Board, Portal Reawakening, blackout, multi-bingo, and witch revisit as configurable/proposed features until final locks are made. |
| --- |

## 20. Source basis notes

Later visual lock: Blossomveil Promenade room-entry screen as approved style/layout target.

Locked realm references: Everbloom Sanctuary Locked Realm Handoff and Sunpetal Conservatory Locked Realm Handoff.

Locked system references: Realm Progression / Unlock Rules, Ingredient / Potion Drop Rules, Economy + Reward Logic, Rewards / Retention Utility Systems.

Older developer handoff retained only for contradiction/migration analysis: Mystical Lands, Enchanted Artifacts, Threshold Boards, Portal Reawakening, Portal Surge, Enchanted Keys, and sample realm chain.

Project chat context retained for later corrections: four rooms per realm, three regular + one special, rooms remain open for ingredients, special room restoration ritual, jackpot wheel visibility, no called balls on room entry, restore rewards are mana and card packs.

# 06 — Player Den and Inventory

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
| Player Den / Hub Systems | LOCKED SYSTEM AREA | Safe to model as player hub. |
| Apothecary | HIGH / VISUAL CONCEPT | Include as hub area; exact mechanics open. |
| Library | HIGH / VISUAL CONCEPT | Include as collection/lore area; exact mechanics open. |
| Relic Wall | HIGH | Displays realm relics, achievement badges, event trophies, Coven honors. |
| Cabinet of Curiosities stores power-ups | HIGH / LIKELY FINAL | Safe naming for power-up inventory. |
| Ingredient compact + detail inventory views | HIGH | Model both overview and detail access. |
| Potion completion overlay | HIGH / LIKELY FINAL | Use design requirements in UI. |

## Extracted Player Den and inventory details

## 14. Player Den

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Player Den / Hub Systems have a locked handoff. | Hub system | Later Project source says the Player Den / Hub Systems Locked Handoff was packaged and rendered. | Use the locked handoff as the authority for Player Den functions and placement. | Detailed mechanics need the handoff itself. |
| Player Den includes an Apothecary, Library, and Relic Wall concept. | Hub areas | Later Project chat specifically requested/generated concepts for Player Den Apothecary, Library, and Relic Wall. | Each area should have a clear function and visual identity. | MVP inclusion and upgrade mechanics remain open. |
| Relic Wall displays restored realm relics, achievement badges, event trophies, and Coven contribution honors. | Player Den display | Later Player Den discussion explicitly listed these Relic Wall contents. | Relic Wall should reflect accomplishments across realms, events, and Covens. | Exact reward/stat hooks remain open. |

## 15. Apothecary / inventory

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Cabinet of Curiosities is where power-ups are stored. | Inventory naming | Later user corrected that Cabinet of Curiosities discussion existed and clarified it is where power-ups are stored. | Use Cabinet of Curiosities for power-up inventory/storage language unless renamed. | Need retrieve full Cabinet discussion for exact UI/placement. |
| Ingredient inventory must support compact and detailed views. | Inventory UX | Later room-entry discussion notes a compact icon/fill progress view and a click-through detailed inventory status for ingredients. | Compact view must be readable; detailed view should support future gifting/requesting. | Exact screen layout and inventory counts are not locked here. |
| Potion completion display must show the potion, completion banner, redeemed ingredient cards, and rewards. | Completion/inventory UX | Later completion-overlay discussion explicitly preferred potion next to banner, “Complete,” arched/tarot spread ingredient cards, and rewards beneath. | Ingredient cards should look like redeemed cards and show at minimum title and stars; remove checkmarks. | Final visual asset still needs locked production file if not already in a handoff. |

## 7. Player Den

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Function | Player Den is the personal hub / owned magical space. It can contain inventory, collections, relics, achievements, event trophies, and social contribution honors. | Do not treat it as merely decorative if inventory and relic systems are expected to live there. |
| Visual tone | Cozy magical personal room, warm light, collectible shelves, potion work areas, books, magical objects, glowing details, and personal progression display. | Avoid dark, dusty, horror-like witch hut treatment. |
| Navigation | Should serve as a hub for Apothecary, Library, Relic Wall, and Cabinet of Curiosities / power-up inventory. | Keep paths clear; do not bury gameplay-critical inventory behind unclear decoration. |
| Generated concepts | Player Den concepts requested/discussed include apothecary, library, and relic wall views. These should influence the hub as distinct zones, not unrelated menu screens. | Final room composition and whether it is navigable/panoramic still needs lock. |

## 8. Apothecary

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Apothecary is the natural visual home for potions, ingredients, crafting/restoration progress, and possibly ingredient requests/gifting. | Exact mechanics are not fully locked; visual design should support inventory and progression without implying unsupported crafting. |
| Visuals | Potion shelves, glass bottles, glowing liquid, labeled ingredient jars, magical measuring tools, herb bundles, cauldron/workbench, parchment recipe cards. | Items must remain readable as collectible UI assets, not just background decoration. |
| UI overlay | Use parchment/gold panels over the apothecary scene for inventory lists, progress bars, ingredient cards, and potion status. | Avoid full-screen clutter; ingredient detail may be an expandable panel from the room-entry strip. |
| Inventory connection | Should connect to ingredient inventory and the Cabinet of Curiosities if power-up/inventory systems overlap. | Do not decide duplicate conversion or crafting costs visually until economy is locked. |

## 9. Library

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Library is the natural visual location for Grimoire, Book of Shadows, spellbook collections, card albums, lore, and possibly completed set review. | Must not make collection navigation feel slow or dusty; still a casual mobile collection UI. |
| Visuals | Floating books, open spellbook desks, glowing bookmarks, page tabs, candlelight, purple/gold shelves, magical dust, readable collection category portals. | Book visuals should frame the UI but not reduce card readability. |
| Collection access | Library should give clear entry points to Grimoire and Book of Shadows. | Do not mix the two so strongly that premium/special collection status becomes unclear. |

## 10. Relic Wall

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Relic Wall displays restored Realm relics, achievement badges, event trophies, and Coven contribution honors. | Relics can preserve the older artifact fantasy as display/story rewards even if potion ingredients are the current room-facing progression. |
| Visuals | Wall of plaques, magical shelves, glowing frames, realm medallions, trophies, sigils, badges, restored objects, and hover/tap details. | Avoid an austere museum wall; it should feel earned and magical. |
| Progression readability | Locked/earned states should be obvious; restored realms should feel visually more alive/glowing than unfinished realms. | Do not imply a realm is complete unless the completion rules are met. |

## 23. Cabinet of Curiosities / power-up inventory

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Cabinet of Curiosities is where power-ups are stored. | Do not rename it or assume it stores all currencies unless approved. |
| Visuals | Magical cabinet with compartments, glowing bottles/orbs, labeled shelves, power-up icons, inventory counts, rarity frames. | Avoid generic backpack UI. |
| Generic power-up icon | Orb + sparkle direction was preferred over a wand/bolt-like icon to avoid confusion with individual wand/bolt power-ups. | The icon must remain readable at small size. |
| Inventory states | Show count, locked/unlocked, source hints, use restrictions, and active power-up status where relevant. | Do not imply power-ups are available in modes where they are restricted. |

## 11. Replaying rooms for ingredients

Replaying rooms for ingredients is a later-current requirement: rooms should not close in a way that prevents players from collecting needed ingredients.

A completed room can change state visually or in reward weighting without becoming inaccessible.

Replay can support: missing ingredient drops, duplicate ingredient conversion, room reward farming, collection drops, jackpot wheel participation, power-up use, and daily/weekly tasks.

Ingredient inventory should track room source, owned count, needed count, completion status, duplicate state, giftability/requestability, and conversion eligibility if duplicate ingredients are later allowed to convert.

Replay should be data-driven: per-room drop tables, post-completion drop modifiers, and event modifiers must be remote-configurable.

## 13. Realm completion rewards

Confirmed constraint: restore rewards should be mana and card packs, not chests.

Room-entry UI reserves a restore reward area on the right side of the ingredient progress strip.

Realm completion should be a celebratory state change, not only a currency payout.

Likely completion outputs: mana reward, card pack reward, realm restored visual state, portal opened, Player Den / Relic Wall relic unlock, possible collection progress, and next realm access.

Exact amounts, pack types, rarity boosts, first-time-only vs repeat rewards, and post-restoration replay rewards are unresolved.

## Codex implementation guardrails

- Keep Player Den systems modular. Do not assume all are MVP.
- Do not assign upgrade mechanics to Apothecary/Library/Relic Wall unless approved.
- Power-up storage should use Cabinet of Curiosities naming unless explicitly renamed.
- Ingredient inventory needs room/potion ties and future gifting/requesting hooks.
- Realm relics may be achievement/display objects even if older artifact-fragment progression is archived.

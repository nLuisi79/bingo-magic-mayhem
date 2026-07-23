# 10 — Verified Project State

## Purpose

This file summarizes the safest current reading of the packet as of **July 23, 2026**. It is meant to help future chats quickly distinguish:

- what appears confirmed enough to build around,
- what feels strongly preferred but not fully locked,
- what remains unresolved,
- what should be treated as replaced or archival,
- and where implementation risk is highest.

This document is a packet-level verification summary. It is not a gameplay rebalance, not a monetization lock, and not a substitute for explicit product approval on open items.

## Source-of-truth packet structure

The current source-of-truth packet appears to be:

- `AGENTS.md`
- `00_READ_ME_FIRST.md`
- `01_LOCKED_GAME_VISION.md`
- `02_CORE_LOOP_AND_REALMS.md`
- `03_COLLECTIONS_GRIMOIRE_BOOK_OF_SHADOWS.md`
- `04_ECONOMY_REWARDS_AND_SCARCITY.md`
- `05_COVENS_SOCIAL_AND_MARKETPLACE.md`
- `06_PLAYER_DEN_AND_INVENTORY.md`
- `07_UI_VISUAL_DIRECTION.md`
- `08_NAMING_GLOSSARY.md`
- `09_OPEN_DECISIONS.md`
- `99_ARCHIVE_AND_OUTDATED_IDEAS.md`

Safe usage rule:

- Treat `00` through `09` plus `AGENTS.md` as the active packet.
- Treat `99_ARCHIVE_AND_OUTDATED_IDEAS.md` as reference-only unless a concept is explicitly revived.

## Confirmed decisions

These appear safe enough to treat as the current product shape unless a later explicit override appears.

### Product identity and tone

- The game is a magical mobile bingo adventure with a bright whimsical premium tone.
- The UI/art direction should avoid casino-first, muddy, dark gothic, overly realistic, or cluttered presentation.
- Readability and player clarity are important production constraints.

### Core structure

- The current safer realm model is **one realm containing four rooms**.
- The current safer room model is **three regular rooms plus one special room**.
- Rooms should not be assumed permanently closed after completion; replayability for ingredient acquisition remains important.
- Realm progression is currently safer to read as **ingredient / potion / spell-forward**, not artifact-fragment-forward.

### Current named systems

- **Grimoire** is the main collection system.
- **Book of Shadows** is a real secondary/special collection system area.
- **Player Den** is a real system area / hub concept.
- **Covens** are a real social system area.
- **Bewitchment Bazaar** is the current preferred marketplace name.
- **Cabinet of Curiosities** is the current preferred name for power-up storage/inventory.

### Current visual anchors

- `Blossomveil Promenade` is an approved room-entry visual/layout anchor.
- Room-entry should keep gameplay-relevant elements prioritized.
- Called balls do not belong on room-entry setup screens.
- Decorative UI should not displace functional gameplay layout.

### Rewards and economy constraints

- Mana is currently real enough to support room-entry bet-per-card UX.
- Restore rewards should not be assumed to be chests; the safer current direction is mana and card packs.
- Monetization must avoid pay-to-win collection shortcuts.

## Strong but not fully locked preferences

These look directionally strong, but still need exact product confirmation or exact values before becoming implementation truth.

### Realm / room direction

- Everbloom Sanctuary and Sunpetal Conservatory are the current safe named realms.
- The special room is likely a distinct finale/fourth room, but the exact mode remains open.
- Special-room replay likely remains part of the design, but exact post-restoration behavior is not fully locked.

### Collections

- 10-card-per-set thinking appears recurrent and strong.
- Rare and Extra Rare look like active rarity tiers.
- Ancient and Gilded look like important special card treatments or rarity categories, but exact live taxonomy is still unresolved.
- Book of Shadows is strongly treated as distinct from Grimoire, especially in duplicate/reward handling.

### Marketplace / social

- Bewitchment Bazaar seems to be the preferred marketplace naming direction.
- Collection-assist / social-help concepts are real and important.
- Trading/gifting/requesting are plausible major systems, but exact launch scope and rules remain open.

### Player Den

- Player Den likely serves as a core personal hub.
- Apothecary, Library, Relic Wall, and Cabinet of Curiosities are all important named Den destinations.
- Not all of those destinations should automatically be assumed Beta 1 gameplay-critical.

### UI

- Clean clarity should win over decorative richness when tradeoffs appear.
- Realm and room visuals should feel premium and magical, but not at the cost of usability.

## Unresolved decisions

These remain the most important open areas before implementation-heavy work should be treated as safe.

### Brand / identity

- Final public title: **Bingo Magic Blast** vs **Bingo Magic Mayhem**

### Scope

- Exact Beta 1 / MVP boundary
- Which major systems are launch-scope vs later-phase

### Realms / gameplay

- Final realm order
- Exact room list per realm
- Exact special-room mode
- Exact portal/jackpot relationship
- Exact post-restoration special-room replay behavior

### Economy

- Final resource hierarchy
- Whether crystals are fully live
- Whether Charm Tokens, Stardust, and Oracle Dust are live
- Duplicate conversion outputs and thresholds
- Wildcard structure and thresholds
- Final realm completion reward values
- Daily reward / daily spin / weekly track / paid track details

### Collections

- Final Grimoire size and layout totals
- Final Book of Shadows size and access model
- Final live rarity taxonomy
- Per-set counts for special rarity/treatment cards

### Social / marketplace / oracle

- Final trade/gift/request rules
- Exact Coven launch scope
- Exact marketplace scope
- Oracle Alley status
- Madame Solange status

### Player Den / UI

- Exact Beta 1 Den feature set
- Final world map packaging
- Final active gameplay HUD composition
- Final logo treatment
- Final power-up icon set
- Final rarity frame set

## Outdated or replaced ideas

These should be treated as archive/reference-only unless product explicitly revives them.

- The older **4–5 Mystical Lands** realm chain model.
- **Whispering Fen**, **Shattered Observatory**, **Clockwork Spire**, **Dragonwake Ruins**, **Frostveil Peaks**, and **Mirage Dunes** as assumed current realm order.
- **Enchanted Artifact fragment** progression as the primary current progression model.
- **Threshold Board** as the assumed final user-facing special-room model.
- Restore rewards as chests.
- Called balls on room-entry setup screens.
- Background-first layout shifts that break locked UX structure.
- Marketplace naming alternatives superseded by Bewitchment Bazaar preference.
- Treating Oracle Alley or Madame Solange as already locked MVP truth.

## Risks for future implementation

### 1. Cross-repo confusion

The packet repo is the current design/source-of-truth repo, but implementation may occur elsewhere. Future chats can easily continue in the wrong repository unless the user explicitly identifies the active implementation workspace.

### 2. Scope creep

Many systems are conceptually important, but not all are necessarily Beta 1. Without an explicit MVP boundary, future implementation chats may overbuild.

### 3. Naming drift

The unresolved public title conflict can leak into code constants, asset exports, folder names, and handoff docs.

### 4. Economy overcommitment

The packet still contains multiple unresolved currencies and conversion ideas. Implementing these too early would harden unapproved economy assumptions.

### 5. Archive contamination

Older realm chains, portal systems, progression objects, and reward structures are detailed enough to accidentally feel “real.” Future chats must actively resist using archive completeness as a proxy for current truth.

### 6. UI polish before scope lock

The visual direction is strong, but many surfaces still depend on unresolved product scope. Premature high-fidelity UI work can lock the team into unapproved product structures.

### 7. Premium / fairness risk

Book of Shadows, duplicate handling, marketplace value, and paid-track concepts all carry monetization and fairness risk if implemented before exact rules are approved.

## Safest next-step reading for future chats

If a future chat needs the fastest safe orientation:

1. Read `AGENTS.md`
2. Read `00_READ_ME_FIRST.md`
3. Read this file (`10_VERIFIED_PROJECT_STATE.md`)
4. Read `09_OPEN_DECISIONS.md`
5. Then branch into the relevant domain file:
   - `02` for realms/progression
   - `03` for collections
   - `04` for economy
   - `05` for social/marketplace
   - `06` for Player Den
   - `07` for UI/visual

# 00 — READ ME FIRST

This is the Codex-ready documentation packet for **Bingo Magic Blast**. It is designed to keep confirmed decisions separate from proposals, unresolved ideas, and archived/outdated mechanics.

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

## Packet contents

1. `01_LOCKED_GAME_VISION.md` — locked product identity, current vision, source-of-truth rules, and major system confidence.
2. `02_CORE_LOOP_AND_REALMS.md` — core play loop, rooms, realm progression, restoration, replay, portals, and world contradictions.
3. `03_COLLECTIONS_GRIMOIRE_BOOK_OF_SHADOWS.md` — Grimoire, Book of Shadows, collection UI, rarity, duplicates, rewards, and unresolved collection rules.
4. `04_ECONOMY_REWARDS_AND_SCARCITY.md` — all economy-facing rules, resources, payouts, conversion ideas, scarcity, daily/weekly rewards, and monetization constraints.
5. `05_COVENS_SOCIAL_AND_MARKETPLACE.md` — Covens, Collection Assist, gifting/requesting, trading, Bazaar, Oracle Alley, and social rewards.
6. `06_PLAYER_DEN_AND_INVENTORY.md` — Player Den, Apothecary, Library, Relic Wall, Cabinet of Curiosities, ingredients, inventory, and completion presentation.
7. `07_UI_VISUAL_DIRECTION.md` — locked visual style, room-entry layout, collection UI, marketplace visuals, rejected visual paths, and production constraints.
8. `08_NAMING_GLOSSARY.md` — project vocabulary and confidence state for key names.
9. `09_OPEN_DECISIONS.md` — all open decisions and contradictions that need product approval.
10. `99_ARCHIVE_AND_OUTDATED_IDEAS.md` — older architecture, replaced mechanics, and ideas Codex must not treat as current source of truth.
11. `AGENTS.md` — operating instructions for Codex/AI agents working in this project.

## Global implementation guardrails

- Do not merge unresolved ideas into confirmed systems.
- Do not “fill in” tuning numbers unless a document explicitly locks them.
- Do not revive older architecture just because it is well-defined; older does not mean current.
- Do not assume that every named currency is live. Some named resources are intentionally listed as unresolved.
- Do not treat early realm chains as final realm order.
- Do not implement Oracle Alley or Madame Solange as final unless the current packet labels the item as locked.
- Do not treat monetization as allowed wherever possible; monetization must respect the fairness constraints and avoid pay-to-win collection shortcuts.

## Major system confidence snapshot

| System | Confidence | Codex handling |
|---|---|---|
| Mobile magical bingo adventure identity | LOCKED | Safe to use as product premise. |
| Bright whimsical premium magical tone | LOCKED | Safe to use as design premise. |
| Blossomveil Promenade room-entry visual/layout anchor | LOCKED | Safe to use as UI source of truth for room-entry structure. |
| Four-room realm structure | LOCKED / HIGH | Safe as current realm model; exact room lists still source-specific. |
| Three regular rooms + one special room | HIGH / LIKELY FINAL | Use as current design, but keep special-room mode configurable. |
| Rooms remain replayable for ingredients | HIGH / LIKELY FINAL | Do not hard-close completed rooms. |
| Grimoire main collection system | LOCKED SYSTEM AREA | Safe to model as major collection; exact card counts need confirmation where contradictory. |
| Book of Shadows | HIGH / LOCKED SYSTEM AREA | Treat as special/premium collection; exact behavior still has open rules. |
| Mana bet per card | LOCKED UI RULE | Safe for room-entry UX; broader economy relationship still open. |
| Coins / energy / crystals | MIXED | Do not assume final resource hierarchy without approval. |
| Charm Tokens / Stardust / Oracle Dust | OPEN | Include as placeholders only if needed; no conversion logic. |
| Coven/social systems | LOCKED SYSTEM AREA | Safe as major system area; detailed trade limits remain open. |
| Bewitchment Bazaar | HIGH / LIKELY FINAL | Use as marketplace naming unless renamed explicitly. |
| Oracle Alley | PROPOSED | Do not build as required core loop. |
| Madame Solange | OPEN | Not confirmed in extracted docs. |
| Player Den | LOCKED SYSTEM AREA | Safe as hub/personal space concept. |
| Cabinet of Curiosities stores power-ups | HIGH / LIKELY FINAL | Safe for inventory naming unless renamed. |
| Daily rewards | LOCKED SYSTEM AREA | Safe to model as retention system; exact ladder open. |
| Weekly witch revisit | PROPOSED | Do not implement final cadence/rewards without approval. |
| Portal Surge older mechanics | ARCHIVE / CONFLICT | Keep separate from current jackpot wheel until reconciled. |

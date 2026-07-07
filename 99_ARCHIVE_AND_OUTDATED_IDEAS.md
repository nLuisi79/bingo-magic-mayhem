# 99 — Archive and Outdated Ideas

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

## Archive handling rule

Archived ideas may be useful for inspiration, lore, or later expansions, but Codex must not implement them as current systems unless product explicitly revives them.

## Known archived / replaced / deprioritized areas

- Older 4–5 Mystical Lands realm structure.
- Enchanted Artifact fragment progression as primary realm model.
- Threshold Board as final room model, unless reinterpreted as the current special room.
- Older sample realm chain: Whispering Fen, Shattered Observatory, Clockwork Spire, Dragonwake Ruins, Frostveil Peaks, Mirage Dunes.
- Older Portal Surge mechanics if treated as replacement for the currently visible jackpot wheel.
- Restore rewards as chests.
- Called balls on room-entry screen.
- Hamburger menu on room-entry screen without purpose.
- Decorative emblem/flower space that does not help gameplay clarity.
- Rogue image/layout generation, wrong rooms, wrong realm names, numbers added to rooms, or missing potions on rooms.
- Overly dark, realistic, muddy, gray, busy, or painterly visual direction.
- Marketplace names superseded by Bewitchment Bazaar preference: Mayhem Market, Balthazar’s Bazaar, Bewitched Bazaar.

## Extracted archive material

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

## 25. Visual ideas rejected or deprioritized

| Rejected / deprioritized idea | Reason / replacement direction |
| --- | --- |
| Overly dark gothic fantasy | Does not match bright magical adventure target. |
| Solemn wizard fantasy / generic medieval kingdom | Too generic and not aligned with casual collectible bingo. |
| Traditional casino clone / direct casino wheel imitation | Game should feel magical, not casino-first. Jackpot wheel must remain whimsical. |
| Overly realistic environments / photoreal materials | Current style is stylized magical storybook with polished casual rendering. |
| Older painterly realm maps as final UI target | Later style lock moved toward less painterly, clearer, character-friendly rendering. |
| Muddy realism and gray stone texture | Makes scenes feel dark and lowers readability. |
| Tiny overworked background detail | Competes with mobile UI. |
| Background-first layout changes | Do not move locked UI regions to fit scenic art. |
| Wrong room/realm names or added room numbers | Breaks continuity and wastes production/generation time. |
| Potions missing from room layout | Potion/restoration item communicates progress and should remain in room title area. |
| Decorative flower/emblem beside room title | Replace with actual potion/restoration item. |
| Top collection emblem consuming space | Reclaim for title image and card count. |
| Hamburger menu on room-entry screen | No clear reason for it in that context. |
| Called balls on room-entry setup | Called balls belong to active gameplay, not card/bet selection. |
| Garden Breeze power-up | Not a real locked power-up. |
| Ingredient card checkmarks in completion overlay | Use actual card identity with title/stars instead. |
| Restore reward chest | Restore reward should be mana/card packs. |
| Hyper-detailed ingredient icons | Need small-size readability; use brighter simpler collectible icon style. |
| Jewelry-like icons when item is not jewelry | Icons must communicate the actual ingredient/item. |
| Oracle Alley / Madame Solange as final MVP assumption | Visual support is fine, but feature/name is not fully locked. |

## 17. Naming glossary

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Coven | Glossary/social | Teams are consistently named Covens across handoffs and later locked social systems. | Use for team/guild feature. | None. |
| Bewitchment Bazaar | Glossary/marketplace | Later discussion selected it as the current marketplace name, and later handoff planning says it was already named. | Use for market/trading hub. | Oracle Alley may remain a sub-feature. |
| Grimoire | Glossary/collection | Main collection system with locked handoff. | Use for main card collection. | Final count/duration still needs lock. |
| Book of Shadows | Glossary/collection | Special/premium collection discussed later by volume/book and potion names. | Use for special/premium collection. | Final structure/count needs lock. |
| Mana | Glossary/resource | Appears in locked room-entry layout as mana bet per card. | Use for play/bet UI unless economy changes. | Relationship to coins/energy is open. |
| Card packs | Glossary/reward | Explicitly named as restore reward alongside mana. | Use for restoration reward and collection acquisition. | Pack types/drop tables open. |
| Cabinet of Curiosities | Glossary/inventory | User clarified it stores power-ups. | Use for power-up storage/inventory unless renamed. | Exact UI placement open. |
| Blossomveil Promenade | Glossary/room | Named as approved Everbloom room-entry screen and style calibration. | Use as visual reference anchor. | Not necessarily all-screen template. |
| Everbloom Sanctuary | Glossary/realm | Locked realm handoff exists. | Preserve locked map/rooms/assets. | Exact order among realms open. |
| Sunpetal Conservatory | Glossary/realm | Locked realm handoff exists. | Preserve locked map/rooms/assets. | Exact order among realms open. |

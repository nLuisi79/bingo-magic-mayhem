# 09 — Open Decisions

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

## Open-decision handling rule

Everything in this file requires product approval before Codex treats it as implementation truth. Codex may create configurable placeholders, TODOs, interfaces, schemas, and test fixtures, but must not hardcode final values or lock gameplay outcomes from these items.

## Documentation-level open decisions

These are the biggest unresolved areas surfaced by the current packet structure itself.

| Open area | Why it is still open | Why it matters |
|---|---|---|
| Final public title: **Bingo Magic Blast** vs **Bingo Magic Mayhem** | The packet title and glossary still show a naming conflict between the document set and later visual references. | Affects app/store metadata, final logo lock, repo naming, asset naming, and handoff consistency. |
| Exact MVP / Beta 1 feature boundary | Multiple systems are conceptually established, but the packet does not yet provide one explicit “ship now vs later” scope sheet. | Prevents safe implementation sequencing and makes it easy to overbuild. |
| Which repo / workspace is the implementation source of truth | The packet is authoritative for design intent, but implementation may occur elsewhere. | Future chats can accidentally continue in the doc repo instead of the game repo, or vice versa. |
| Final “verified current state” summary | The packet previously lacked a dedicated verified-state document. | Makes it harder for new chats to distinguish current truth from strong preferences and archive material. |

## Product identity and scope

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final public game title | `08_NAMING_GLOSSARY.md` still presents both titles as current candidates. | Needed before final branding, logo exports, repo normalization, and production handoff. |
| Beta 1 / MVP system scope | Core systems exist conceptually, but feature-phase boundaries are still distributed across docs instead of formally consolidated. | Needed before implementation sequencing, UI prioritization, and live-content planning. |
| What is explicitly Phase 2+ | Some concepts are real but may not be launch-scope. | Prevents mixing “important long-term” systems into immediate Beta build work. |

## Realms, rooms, and progression

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final realm order | Everbloom Sanctuary and Sunpetal Conservatory are both treated as real/current realms, but exact ordering remains unresolved. | Affects onboarding, map flow, art production order, and reward tuning. |
| Exact room list per locked realm | The packet confirms realm structure and some named rooms, but not a single final master room index for every realm. | Needed for content IDs, scene naming, ingredient ownership, and quest routing. |
| Special room format | Blackout, multi-bingo, or another special mode remains open. | Affects rules engine, UI, tutorialization, rewards, and difficulty curve. |
| Post-restoration access rules for the special room | Replayability is strongly preferred, but exact special-room behavior after restoration is not fully locked. | Needed to avoid content dead ends or reward exploits. |
| Portal model | Later packet guidance supports visible portal/jackpot presentation, but the relationship to older Portal Surge structures is unresolved. | Needed to keep progression, jackpot, and event systems distinct. |
| Potion/spell vs artifact/relic language | Potion/spell-forward progression is current, but older artifact terminology still exists in archival materials. | Needed for consistent UI copy, data naming, and future lore hooks. |

## Economy, rewards, and scarcity

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final resource hierarchy | Mana is strong/current in UI direction, but coins, energy, crystals, and other named resources remain unresolved or legacy. | Needed for every HUD, reward table, store, and balance sheet. |
| Whether crystals are active in Beta 1 | Crystals remain present in concepts but not fully normalized as final. | Affects premium/pseudo-premium sink design and interface commitment. |
| Whether Charm Tokens, Stardust, and Oracle Dust are real live resources | Mentioned in economy planning, but not locked as active currencies. | Prevents unnecessary currency clutter and false backend assumptions. |
| Duplicate conversion outputs and thresholds | Duplicate handling is important, but exact formulas remain unresolved. | Needed before implementing collection progression, reward loops, or marketplace behavior. |
| Book of Shadows duplicate behavior | Treated as distinct from Grimoire, but exact reward/protection logic is still open. | Needed to avoid predatory or confusing premium-collection behavior. |
| Wildcard types and redemption thresholds | Wildcard support is a strong concept, but exact forms and costs remain open. | Impacts collection completion pacing and scarcity tuning. |
| Realm completion reward values | Reward categories are constrained more than exact amounts. | Needed for retention pacing and room replay balance. |
| Daily reward, daily spin, weekly track, and paid track specifics | These systems exist conceptually, but exact ladders and payout curves remain unresolved. | Needed for retention balance, economy fairness, and monetization scope control. |
| Marketplace / set value behavior | Value-display and reward ideas exist, but final use of “set value” remains unresolved. | Needed before adding marketplace logic, redemption loops, or social economy messaging. |

## Collections

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final Grimoire size and page structure | Main collection is real/locked as a system area, but exact totals remain contradictory. | Needed for UI slots, pack odds, drop pacing, and asset production. |
| Final Book of Shadows size and access structure | System direction exists, but exact scope and progression remain open. | Needed for launch-scope planning and premium collection rules. |
| Final rarity taxonomy in production | Rare / Extra Rare are strong; Ancient / Gilded are named; Astral appears archival; exact live taxonomy is not fully normalized. | Needed for frame sets, card data, duplicate rules, and trading rules. |
| Per-set counts for special card types | Ancient and Gilded presence is discussed, but exact per-set counts remain open. | Needed for art production, rarity balance, and collection progression. |

## Social, marketplace, and oracle features

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Trade / gift / request eligibility rules | Social direction exists, but exact limits, cooldowns, and scope remain open. | Needed for exploit prevention and economy protection. |
| Coven launch scope | Covens are real as a system area, but specific MVP operations are still not fully normalized. | Needed before backend and UI sequencing. |
| Marketplace scope | Bewitchment Bazaar is the current preferred name, but exact scope is still open. | Needed to separate collection help, trade, gifting, and shop behaviors. |
| Oracle Alley status | Still explicitly unresolved / proposed in the packet. | Needed before UI, economy, or character art production. |
| Madame Solange status | Still not confirmed as an official locked character. | Needed before writing lore, character art, or feature-specific UI. |

## Player Den and inventory

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final Beta 1 Player Den scope | Player Den is real, but exact launch features within it remain distributed across docs. | Needed to keep hub work focused and shippable. |
| Single Den surface vs multiple destination screens | The packet supports Den subareas, but exact UX packaging remains open. | Needed before navigation architecture is finalized. |
| Which inventory surfaces must ship first | Cabinet of Curiosities, Library, Apothecary, and Relic Wall are not equally urgent. | Needed for UI staging and content production sequencing. |

## Visual and UI decisions still open

| Decision needed | Current packet evidence | Why it must be locked |
|---|---|---|
| Final logo treatment | Direction exists, but the packet still flags logo as an open visual decision. | Needed for consistent shell/header/app icon exports. |
| Final active gameplay UI composition | Room-entry is far more locked than gameplay HUD. | Needed before polishing the in-round player experience. |
| Final world map structure and density | Realm presentation direction exists, but world-map packaging is not fully formalized in the packet. | Needed for hub flow, future realm placeholders, and onboarding. |
| Final power-up icon set | Power-up presentation is needed, but the final icon set is still open. | Needed before inventory, gameplay, and monetization polish. |
| Final rarity frame set | Rarity names exist, but final frame taxonomy is not fully locked. | Needed before card art production can stabilize. |

## Archive conflicts that still require conscious handling

These are the archive-driven contradictions that remain risky unless a future implementation chat is explicitly warned away from them.

| Conflict area | Older / archive direction | Current safer direction |
|---|---|---|
| Realm structure | 4–5 Mystical Lands per realm chain | Four-room realm structure with three regular rooms plus one special room |
| Realm progression object | Enchanted Artifact fragments | Potion / spell / ingredient-forward progression |
| Finale room model | Threshold Board closes after completion | Rooms, including special content, should not be assumed permanently closed |
| Realm chain | Whispering Fen / Shattered Observatory / etc. | Everbloom Sanctuary and Sunpetal Conservatory are the current safe named realms |
| Jackpot / portal | Portal Surge and related legacy structures | Do not merge legacy portal concepts into the current jackpot-visible direction without approval |
| Restoration rewards | Chests and broader reward bundles | Mana and card packs are the current safer restore-reward direction |
| Room-entry UI | Called balls and extra decorative clutter | Keep room-entry focused on selection, bet, potion/progress, and clarity |

## Thin or missing packet areas

These are not necessarily “product decisions,” but they are documentation gaps that future chats should treat carefully.

- No dedicated master Beta 1 / MVP scope doc.
- No dedicated cross-repo implementation handoff note explaining where live implementation is occurring.
- No dedicated current-state verification file before this audit pass.
- No single launch-priority list that orders systems by immediate implementation importance.
- No formal “do this next” execution sequence for future chats.

## What Codex or a developer must not assume

- Do not assume the public title is finally locked.
- Do not assume every named currency in the packet is live for Beta 1.
- Do not assume older archive realm chains are valid current realm order.
- Do not assume Portal Surge and the later jackpot-visible direction are the same system.
- Do not assume Book of Shadows duplicate rules match Grimoire duplicate rules.
- Do not assume Oracle Alley or Madame Solange are launch-approved.
- Do not assume all Den subareas are equal-priority MVP work.
- Do not assume all conceptually “real” systems are Beta 1 scope.

## Final lock checklist before implementation-heavy work resumes

- Confirm final public game title.
- Confirm Beta 1 / MVP system boundary.
- Confirm exact realm order and exact room list per active realm.
- Confirm special-room format and replay rules.
- Confirm final active resource hierarchy.
- Confirm duplicate conversion behavior by collection/card class.
- Confirm final Grimoire and Book of Shadows size/taxonomy.
- Confirm trade / gift / request rules and marketplace scope.
- Confirm Player Den MVP surfaces.
- Confirm final logo / HUD / world-map production direction.

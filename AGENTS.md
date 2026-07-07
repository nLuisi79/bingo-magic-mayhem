# AGENTS.md — Codex / AI Agent Instructions for Bingo Magic Blast

## Primary rule

Use this packet as the immediate Codex-facing documentation source. These files are derived from the four extraction documents:

- `Bingo_Magic_Blast_Locked_Decision_Register.docx`
- `Bingo_Magic_Blast_Economy_Design_Document.docx`
- `Bingo_Magic_Blast_Realms_World_Progression_Document.docx`
- `Bingo_Magic_Blast_Visual_UI_Direction_Document.docx`

Do not rely on memory, older chat fragments, or early brainstorms when a packet file gives a current confidence label.

## Confidence labels

- **LOCKED** — appears explicitly chosen in later discussion, reflected in a packaged locked handoff, or called out as the current source of truth in the extraction documents.
- **HIGH / LIKELY FINAL** — repeated later preference or consistent direction, but still needs exact tuning, naming, or implementation confirmation.
- **PROPOSED** — usable as design context only; do not implement as a final rule without approval.
- **OPEN** — known unresolved decision or contradiction.
- **ARCHIVE / OUTDATED** — older architecture or replaced idea; do not implement unless re-approved.

## Implementation behavior

### Allowed

- Create data models, interfaces, schemas, config files, placeholder UI components, and TODO markers for **LOCKED** or **HIGH / LIKELY FINAL** systems.
- Make open values configurable instead of hardcoded.
- Preserve separate namespaces for Grimoire, Book of Shadows, Realm progression, Coven/social, marketplace, Player Den, and visual/UI constants.
- Add tests that verify locked layout/data assumptions, such as 1/2/4/6 card selection options in room-entry config.
- Add comments that reference the relevant packet file and confidence label.

### Not allowed

- Do not treat **PROPOSED**, **OPEN**, or **ARCHIVE / OUTDATED** items as final mechanics.
- Do not implement final conversion rates for duplicates, wildcards, tokens, dust, or marketplace value unless explicitly provided.
- Do not assume Charm Tokens, Stardust, Oracle Dust, Coven Orbs, Ritual Calls, or Portal Surge resources are active core currencies.
- Do not assume Madame Solange is an official character.
- Do not revive older realm chains as current realm order.
- Do not hard-close completed rooms; current direction is replayability for ingredients.
- Do not make restore rewards chests; current constraint is mana and card packs.
- Do not place called balls on room-entry screens.
- Do not build casino-like, dark gothic, muddy, realistic, or cluttered UI.
- Do not replace Bewitchment Bazaar with Oracle Alley.
- Do not merge Grimoire and Book of Shadows duplicate behavior.
- Do not make social sharing automatic.

## Recommended project structure for implementation

```text
src/
  config/
    realms/
    rooms/
    collections/
    economy/
    rewards/
    covens/
    marketplace/
    ui/
  systems/
    bingo/
    realms/
    collections/
    economy/
    inventory/
    covens/
    marketplace/
    player_den/
  ui/
    room_entry/
    bingo_play/
    rewards/
    grimoire/
    book_of_shadows/
    player_den/
    marketplace/
  docs/
    codex_packet/
```

## Configuration-first requirement

Anything with unresolved tuning must be represented as config or TODO, not hardcoded:

- drop rates
- duplicate conversion amounts
- wildcard redemption thresholds
- card pack odds
- rarity distribution
- daily reward ladder
- weekly reward track
- paid upgrade track
- marketplace/set value formulas
- social trade/gift/request limits
- special-room mode
- portal/jackpot values

## Major system source files

| Need | Read first |
|---|---|
| Overall source-of-truth rules | `00_READ_ME_FIRST.md` |
| Game premise and locked vision | `01_LOCKED_GAME_VISION.md` |
| Core loop / realms / rooms / portals | `02_CORE_LOOP_AND_REALMS.md` |
| Collections / rarity / duplicates / wildcards | `03_COLLECTIONS_GRIMOIRE_BOOK_OF_SHADOWS.md` |
| Economy / rewards / scarcity | `04_ECONOMY_REWARDS_AND_SCARCITY.md` |
| Covens / social / marketplace / Oracle | `05_COVENS_SOCIAL_AND_MARKETPLACE.md` |
| Player Den / inventory / Apothecary | `06_PLAYER_DEN_AND_INVENTORY.md` |
| UI / art / layout / rejected visuals | `07_UI_VISUAL_DIRECTION.md` |
| Names and confidence states | `08_NAMING_GLOSSARY.md` |
| Product decisions still needed | `09_OPEN_DECISIONS.md` |
| Older/replaced ideas | `99_ARCHIVE_AND_OUTDATED_IDEAS.md` |

## When uncertain

Prefer a conservative implementation:

1. Create a typed placeholder or config stub.
2. Add `TODO(product):` with the exact open decision.
3. Link the relevant packet file.
4. Avoid final gameplay values.
5. Avoid irreversible economy or monetization assumptions.

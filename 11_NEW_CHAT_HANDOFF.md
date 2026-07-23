# 11 — New Chat Handoff

## Why this file exists

This file is a clean handoff for starting a fresh Codex chat after the current thread became slow/noisy. It is intended to preserve working context without requiring a new chat to reconstruct packet intent from a long conversation.

Date prepared: **July 23, 2026**

## What this repository is

This repository is the **design / source-of-truth packet repo** for Bingo Magic Blast / Bingo Magic Mayhem.

It contains:

- the active Codex-facing packet,
- the current open-decisions inventory,
- the verified-state summary,
- and the archive/reference file for older replaced ideas.

It should be treated as the **documentation authority** for product/design intent unless the user explicitly says otherwise.

## What this repository is not

This repo is **not automatically the live implementation repo**.

Future chats should confirm:

- whether the user wants to continue in this packet repo, or
- whether the user wants to continue in a separate Unity / implementation workspace.

That distinction matters because recent work may have happened outside this doc packet repo.

## Authoritative reading order for the next chat

The next chat should begin by reading these files in order:

1. `AGENTS.md`
2. `00_READ_ME_FIRST.md`
3. `10_VERIFIED_PROJECT_STATE.md`
4. `09_OPEN_DECISIONS.md`
5. The domain file most relevant to the requested task:
   - `02_CORE_LOOP_AND_REALMS.md`
   - `03_COLLECTIONS_GRIMOIRE_BOOK_OF_SHADOWS.md`
   - `04_ECONOMY_REWARDS_AND_SCARCITY.md`
   - `05_COVENS_SOCIAL_AND_MARKETPLACE.md`
   - `06_PLAYER_DEN_AND_INVENTORY.md`
   - `07_UI_VISUAL_DIRECTION.md`
6. `99_ARCHIVE_AND_OUTDATED_IDEAS.md` only when checking conflicts or replaced ideas

## Current packet conclusions

### Safe current truths

- Magical mobile bingo adventure identity is current.
- Bright whimsical premium tone is current.
- Current safe realm model is four rooms per realm.
- Current safe room model is three regular rooms plus one special room.
- Rooms should remain replayable for ingredients.
- Progression is safer to treat as potion/spell/ingredient-forward than artifact-fragment-forward.
- Grimoire, Book of Shadows, Player Den, Covens, and Bewitchment Bazaar are real/current system areas.
- Mana is real enough to support room-entry bet-per-card UX.
- Restore rewards are safer as mana and card packs than as chests.

### Strong but still not fully locked

- Everbloom Sanctuary and Sunpetal Conservatory are the current safe named realms.
- Bewitchment Bazaar is the preferred marketplace name.
- Cabinet of Curiosities is the preferred power-up inventory name.
- Clean clarity should take priority over decorative richness when tradeoffs appear.

### Biggest open decisions still blocking full implementation confidence

- Final public title: Bingo Magic Blast vs Bingo Magic Mayhem
- Exact Beta 1 / MVP scope
- Final realm order
- Exact room list per realm
- Exact special-room format
- Final resource hierarchy
- Duplicate conversion rules
- Final collection totals / taxonomy
- Marketplace / gifting / trading rules
- Oracle Alley / Madame Solange status
- Exact Player Den Beta 1 scope

## Most important archive traps

Future chats must not drift back into these archive ideas accidentally:

- 4–5 Mystical Lands as the assumed current realm model
- Whispering Fen / Shattered Observatory / other older realm chains as current realm order
- Enchanted Artifact fragments as the default progression model
- Threshold Board as the assumed current special-room structure
- Portal Surge legacy mechanics as automatically equivalent to the later jackpot-visible direction
- Restoration chests as the assumed current restore reward
- Called balls on room-entry screens

## Recommended future-chat behavior

When a future chat starts:

- first determine whether the task is **documentation**, **planning**, **implementation**, or **UI/presentation**,
- confirm the active repo/workspace before editing,
- use `10_VERIFIED_PROJECT_STATE.md` to avoid overcommitting on open items,
- use `09_OPEN_DECISIONS.md` whenever a question touches economy, progression, collection totals, marketplace scope, or unresolved product naming.

## Copy/paste starter prompt for the next chat

Use this prompt to start the next Codex chat cleanly:

```text
We are continuing Bingo Magic Blast / Bingo Magic Mayhem from a fresh chat.

First, read:
- AGENTS.md
- 00_READ_ME_FIRST.md
- 10_VERIFIED_PROJECT_STATE.md
- 09_OPEN_DECISIONS.md

Treat this repo as the source-of-truth documentation packet unless I explicitly redirect you to a separate implementation repo/workspace.

Important constraints:
- Do not treat archive/reference materials as equal to locked docs.
- Do not finalize gameplay, economy, progression, rarity, monetization, or reward decisions unless explicitly locked.
- Do not assume the final public title is resolved.
- Do not assume older realm chains, Threshold Board, Portal Surge legacy structures, or artifact-fragment progression are current.

Before making changes:
1. Briefly summarize the current verified project state.
2. Identify any repo/workspace ambiguity if present.
3. State the safest next actions for the task I give you.

Current handoff note:
- 10_VERIFIED_PROJECT_STATE.md contains the best quick summary of what is confirmed, strongly preferred, unresolved, outdated, and risky.
- 11_NEW_CHAT_HANDOFF.md contains the fresh-chat continuation note.
```

## Suggested first tasks for a future chat

Depending on what the user wants next, the cleanest starting tracks are:

- create a dedicated Beta 1 / MVP scope doc,
- create a launch-priority implementation sequence doc,
- normalize naming around the final public title,
- or switch to the actual implementation repo and continue only against the packet’s locked guidance.

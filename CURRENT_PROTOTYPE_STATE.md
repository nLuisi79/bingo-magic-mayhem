# Bingo Magic Mayhem - Current Prototype State

Last updated: 2026-07-08

Use this as the short entry point for fresh Codex threads. The detailed rules are split into the docs below.

## Start Here

- Locked product rules: `Bingo Magic Mayhem/Docs/LOCKED_DECISIONS.md`
- Unresolved product decisions: `Bingo Magic Mayhem/Docs/OPEN_DECISIONS.md`
- Known code/design mismatches: `Bingo Magic Mayhem/Docs/IMPLEMENTATION_GAPS.md`
- Verified project-state audit: `Bingo Magic Mayhem/Docs/10_VERIFIED_PROJECT_STATE.md`
- Numbered open-decision audit: `Bingo Magic Mayhem/Docs/09_OPEN_DECISIONS.md`
- Rank rewards working draft: `Bingo Magic Mayhem/Docs/11_RANK_REWARDS_V0.1.md`
- Older baseline docs:
  - `Bingo Magic Mayhem/Docs/Gameplay Rules v0.1.md`
  - `Bingo Magic Mayhem/Docs/Game Systems Roadmap v0.1.md`
  - `Bingo Magic Mayhem/Docs/Prototype Architecture v0.1.md`
- Important external handoff reference:
  - `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md`
- ChatGPT project export archive:
  - `Bingo Magic Mayhem/Docs/Archive/ChatGPT_Project_Exports/`
- Cocos/Cocos2d chat export archive:
  - `Bingo Magic Mayhem/Docs/Archive/Cocos2d_Chat_Exports/`

## Working Agreement

- Unity is the chosen prototype direction.
- Existing docs are a baseline, but development has expanded beyond them.
- Check with the user before changing core gameplay, economy tuning, progression, jackpot odds, album structure, coven behavior, navigation architecture, or reward cadence.
- Prefer narrow implementation passes with clean verification.
- Keep visuals prototype-level unless visual polish is explicitly requested.

## Project Layout

- Unity project: `Bingo Magic Mayhem`
- Main prototype script: `Bingo Magic Mayhem/Assets/Scripts/BingoPrototype.cs`
- Inventory/profile state: `Bingo Magic Mayhem/Assets/Scripts/PlayerInventoryState.cs`
- Realm/room data: `Bingo Magic Mayhem/Assets/Scripts/RealmContentCatalog.cs`
- Album/card data: `Bingo Magic Mayhem/Assets/Scripts/CardAlbumCatalog.cs`
- Coven state: `Bingo Magic Mayhem/Assets/Scripts/CovenState.cs`

Build check:

```powershell
dotnet build "Bingo Magic Mayhem.sln" -v:quiet -clp:ErrorsOnly
```

Most recent result: build succeeded with 0 errors and 2 warnings.

## Current Implemented Prototype Areas

- Standard bingo gameplay.
- Special blackout room prototype.
- Room home/card count/mana bet flow.
- World map and realm map shells.
- Room restoration and realm progression prototype.
- Jackpot wheelspin prototype.
- Power-up bank and gameplay sigil prototype.
- Clairvoyance timed boost prototype.
- Ingredient collection and potion restoration prototype.
- Player's Den shell.
- Locked account rank display and rank ladder popup in Player's Den/profile debug.
- Cabinet of Curiosities prototype.
- Library/Grimoire/Book of Shadows prototype.
- Library card gifting prototype for extra regular Grimoire duplicates, with Inbox card-claim delivery test.
- Library card state readability pass: missing/owned/duplicate states and number-only duplicate badges in Grimoire/Book of Shadows card grids.
- Grimoire ingredient detail prototype with Ask for Help and Use Wild shells.
- Card reveal and NEW card flags.
- Coven shell, member popup, wish list, join request, and coven gift prototype.
- Daily Bonus prototype with streak-save state.
- Daily Spin prototype as a separate direct-claim daily loop.
- Inbox prototype with shared reward item plumbing for gifts/system rewards.
- Freebies/social-link prototype stub.

## Recommended Next Thread Prompt

```text
Continue from CURRENT_PROTOTYPE_STATE.md plus Docs/LOCKED_DECISIONS.md, Docs/09_OPEN_DECISIONS.md, Docs/10_VERIFIED_PROJECT_STATE.md, Docs/11_RANK_REWARDS_V0.1.md, Docs/OPEN_DECISIONS.md, Docs/IMPLEMENTATION_GAPS.md, the ChatGPT export archive at Docs/Archive/ChatGPT_Project_Exports, the Cocos/Cocos2d archive at Docs/Archive/Cocos2d_Chat_Exports, and the comprehensive handoff at C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\Bingo_Magic_Mayhem_Comprehensive_Project_Handoff-jun26.md. Work narrowly. Do not change core gameplay, economy tuning, jackpot, album, coven, room progression, or navigation rules without asking first.
```

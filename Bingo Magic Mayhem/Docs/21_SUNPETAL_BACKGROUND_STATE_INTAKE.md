# Sunpetal Conservatory Background State Intake

Recorded: 2026-10-06

## Authority and scope

The user supplied these five background state images in the project chat `Review Sunpetal Conservatory assets` (chat ID `01a10fe4-4e79-7622-823d-02e00b8081d8`). The user explicitly identified them as Sunpetal Conservatory backgrounds and said navigation and title graphics will follow after the backgrounds are in place. This is current user-provided asset context, not an old chat export.

This record confirms receipt and inspection of source files. It does not claim Unity import, runtime wiring, transition testing, or final visual approval. It does not lock restoration triggers, gameplay, progression, economy, rarity, monetization, or rewards.

## Project locations

- Repository directory: `C:\Users\nLuis\Documents\Bingo Magic Mayhem`
- Active Unity project: `C:\Users\nLuis\Documents\Bingo Magic Mayhem\Bingo Magic Mayhem`
- Active Unity documentation: `Bingo Magic Mayhem/docs`
- Source artwork folder: `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\realms- bingo\sunpetal conservatory\worlds\final`

Read root `AGENTS.md`, root `00_READ_ME_FIRST.md`, and `CURRENT_PROTOTYPE_STATE.md` before future edits. The requested `docs/00_READ_ME_FIRST.md` is absent; the entry document is at the repository root. The root packet uses older Bingo Magic Blast terminology and differs from newer Unity documentation in some areas. Consult confidence labels and record conflicts rather than silently combining them. Archive/chat exports remain reference only.

## Supplied images

All five source PNGs were opened successfully for dimension inspection and measure **1920 x 1080** (16:9 landscape). Visual descriptions below are observations from the supplied images, not a confirmed mapping to named rooms or progression events.

| Original filename | Visible state | Intake status |
|---|---|---|
| `fullBroken.png` | Overgrown/damaged conservatory; damaged pavilion glass; inactive rear portal arch. | Source received and inspected. |
| `realm1restore.png` | Left pavilion and nearby garden/water brighten; other major areas remain damaged. | Source received and inspected. |
| `realm2restore.png` | Left pavilion remains restored; upper-right terrace and surrounding area brighten. | Source received and inspected. |
| `realm3restore.png` | Lower-right pavilion also restores; foreground crystal area and some rear glass remain damaged. | Source received and inspected. |
| `st-fullrestore.png` | Foreground crystal/fountain, remaining environment, and rear portal restore; portal swirl is visible. | Source received and inspected. |

Preserve original filenames in intake records. `realm1restore`, `realm2restore`, and `realm3restore` describe files within Sunpetal; they do not establish global Realm 1/2/3 identity or order.

## Confirmed next handoff

- These backgrounds belong to Sunpetal Conservatory.
- Navigation and title graphics are pending from the user after the background placement step.
- Background receipt, repository placement, Unity import, and runtime integration must be tracked separately. Only source receipt and inspection are verified by this update.

## Review notes and remaining checks

- The restoration sequence is visually legible, with the crystal and active portal providing a distinct final reveal.
- The upper-right terrace retains prominent thorny vines in the fully restored image. They may be intentional living architecture; this is feedback for review, not an instruction to remove them.
- Major landmarks share a broadly consistent composition, but individual details shift between images. Pixel registration and transition quality have not been verified. Equal dimensions alone do not guarantee clean fades.
- Check title/navigation contrast and mobile readability once the overlays arrive. The backgrounds are bright and detailed.
- The exact runtime destination, asset keys, named-room/hotspot mapping, crop/safe-area behavior, transition method, and state-selection rules remain unverified. Do not infer mechanics from filenames or art.
- `20_WORLD_MAP_COMPOSITION_SPEC.md` covers the global five-door world map. Do not automatically apply its layout to this interior conservatory background.

## Completion status

Documentation recorded; source files verified as available. Background placement/import and navigation/title intake remain pending. No game code was changed by this documentation update.

## Background Replacement Completed — 2026-10-06

This status supersedes the earlier pending repository-placement/runtime-key notes above.

The five supplied PNGs now replace the older backgrounds under `Assets/Resources/prototype/ui/realm_home/sunpetal`:

| Source | Existing runtime filename |
|---|---|
| `fullBroken.png` | `sunpetal-realm-broken0.png` |
| `realm1restore.png` | `sunpetal-realm-broken1.png` |
| `realm2restore.png` | `sunpetal-realm-broken2.png` |
| `realm3restore.png` | `sunpetal-realm-broken3.png` |
| `st-fullrestore.png` | `sunpetal-realm-restored.png` |

All destination image hashes match their sources. Existing `.meta` files, GUIDs, and texture importer settings were preserved and verified against backups. Older background copies were retained in this chat's `work/sunpetal-before-import` folder.

Read-only inspection confirmed that `CreateActiveRealmMapBackground()` already selects broken0/broken1/broken2/broken3/restored using restored-room count, and `CreateAnchoredImage()` loads these textures through Resources. Thus the replacements use the existing runtime paths without code or progression-rule changes. This confirms asset placement and existing wiring, not Unity editor reimport or on-screen rendering. Editor refresh, crop/registration review, and navigation/title overlays remain unverified/pending.

## Production Asset Set Supersession — 2026-10-06

The user subsequently supplied a newer set from:

`C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\prod assets\UnityFiles\rooms unity ready\01_sunpetal\world\final`

This set supersedes the earlier imported artwork while retaining the same runtime filenames:

| New source | Existing runtime filename |
|---|---|
| `sunpetal-realm-broken.png` | `sunpetal-realm-broken0.png` |
| `sunpetal-realm-1.png` | `sunpetal-realm-broken1.png` |
| `sunpetal-realm-2.png` | `sunpetal-realm-broken2.png` |
| `sunpetal-realm-3.png` | `sunpetal-realm-broken3.png` |
| `sunpetal-realm-restored.png` | `sunpetal-realm-restored.png` |

All five source and destination PNGs are 1920 x 1080 and hash-identical after installation. Existing `.meta` files were left in place and remain hash-identical to the preserved metadata, keeping GUIDs and importer settings stable. The displaced set was reconstructed from its still-available source files and backed up at `C:\Users\nLuis\Documents\Codex\2026-10-06\i\work\sunpetal-before-prod-assets-import`.

The existing runtime mapping remains valid and no code changed. This update does not lock named-room ownership, restoration triggers, progression, rewards, economy, rarity, or monetization. The earlier note about dark retained thorns in the fully restored upper-right terrace applies to the superseded artwork, not the active production set. Unity editor refresh, on-screen crop/safe-area review, transition registration, mobile overlay contrast, and the pending navigation/title graphics remain the next visual integration checks.

## Complete Realm-Home UI Handoff — 2026-10-14

The user supplied the complete Sunpetal realm-home presentation set and re-supplied the original five-background sequence. The re-supplied backgrounds now occupy the existing `broken0` through `restored` runtime slots, superseding the 2026-10-06 production-tree artwork. Source/destination SHA-256 hashes match for all five files, and the existing background `.meta` files remain in place.

Thirty-one UI PNGs are installed under `Assets/Resources/prototype/ui/realm_home/sunpetal/ui`:

- parchment/gold realm header: nine tiles under `realm_header`
- dark-green/gold room label: nine tiles under `room_label`
- four full potion images and four shadow-state images under `potions`
- `room-frame.png`, `room-lock.png`, `nav-left.png`, `nav-right.png`, and `potion-count.png`

`BingoPrototype.cs` now uses these assets only when the active prototype realm is Sunpetal. It composes the two nine-tile frames at runtime, displays full or shadow potions in the realm header from existing restored-room state, shows potion or lock medallions on room labels from existing room availability, and routes clicks through the existing world-map, realm-entry, and room-entry callbacks. Other realms retain the previous generic realm-home renderer.

The visual mapping used for the four existing room definitions is:

1. Gilded Azalea Arboretum → `sunpetal-conservatory`
2. Thorn-Tangled Terrace → `thorn-tangled`
3. Pollenspire Fountain → `pollenspire`
4. Solstice Bloom Glasshouse → `solstice-bloom`

This mapping is presentation metadata for the supplied named assets. It does not alter room order, unlock conditions, restoration requirements, rewards, economy, rarity, monetization, or any other gameplay rule.

Unity imported the new PNGs and generated their metadata. The editor then completed a forced synchronous script recompile and domain reload with no compiler errors in `Editor.log`. Remaining verification is an on-screen review of the composed header, room-label placement, safe areas, text fit, button hit targets, disabled next-realm arrow, and all five background states at runtime. A standalone `dotnet build` could not run because no .NET SDK is installed on the machine.

### First-pass polish follow-up — 2026-10-14

The temporary code-drawn account bar has been replaced on the Sunpetal screen with the existing production mana, crystal, power-up, add-button, and home assets already used by the world map. Room labels no longer display textual state such as `Restored`; the full potion, shadow potion, or lock medallion is the state indicator. Unfinished unlocked rooms use the supplied `potion-count.png` badge to show combined collected/required ingredient progress. These are presentation changes only.

### Measured placement and mixed-state QA — 2026-10-14

The full-stack 1920 x 1080 reference is now the coordinate source for the Sunpetal-specific header, navigation arrows, four room nodes, and top-right navigation. Each room node owns independently positioned label, medallion, title, and count-badge elements; the screen does not rely on a generic grid or automatic distribution. Runtime nine-tile composition uses a two-pixel overlap to hide filtering seams without changing the source tile mapping.

Prototype Settings now includes `Sunpetal Mixed QA`. The preset clears only Sunpetal room restoration and ingredient keys, then stages Gilded Azalea Arboretum as restored, Thorn-Tangled Terrace as unlocked and partially collected, and the remaining two rooms as locked. This supports simultaneous visual review of potion, shadow, lock, count-badge, and broken-background states. It is destructive to local Sunpetal QA progress and must be applied deliberately.

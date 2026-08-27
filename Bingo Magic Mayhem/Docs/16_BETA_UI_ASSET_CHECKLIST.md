# Beta 1 UI Asset Checklist

Last updated: 2026-07-17

Status: working Beta 1 asset-production checklist. This is not a gameplay, economy, progression, rarity, monetization, reward, or Aura/rank lock document.

Read with:

- `15_BETA_UI_SCOPE.md`
- `12_BETA_1_PRODUCTION_READINESS.md`
- `13_BETA_QA_CHECKLIST.md`
- `10_VERIFIED_PROJECT_STATE.md`

## Purpose

Turn Beta 1 UI scope into a practical asset list that can be prepared, sourced, replaced, or deferred intentionally.

This checklist is meant to answer:

- what art is needed now
- what can be placeholder
- what should be custom first
- what can come from temporary external sources

## Priority Legend

- P0 — needed for the main Beta 1 play loop to feel real
- P1 — strongly recommended for Beta readability and cohesion
- P2 — useful but safe to simplify or defer

## Beta 1 Count Targets

- Avatars: 4-6
- Frames: 4
- Daubers: 3
  - 1 default
  - 1 early unlock
  - 1 later unlock

## File Format, Size, And Naming Specs

This section defines the safest current delivery format for Beta 1 UI assets based on the current Unity prototype intake.

### General delivery rule

For Beta 1 UI assets, prefer:

- exported file type: `.png`
- background: transparent unless the asset is a full opaque background
- Unity import type: `Sprite (2D and UI)`
- sprite mode: `Single`

Keep layered source files separately where possible:

- `.psd`, `.afdesign`, or equivalent editable source
- exported `.png` for Unity intake

When requesting or preparing production-ready UI art, specify the runtime assembly needs up front:

- whether Unity needs a single flattened sprite or multiple separate sprites
- whether the asset needs empty interior space for live text, numbers, icons, or badges
- whether frames, glows, overlays, plus buttons, icons, counters, and decorative flourishes should export separately
- whether the asset is intended for `Image`, `Button`, or 9-slice use
- target display size band, safe text area, and expected aspect ratio

If the final UI will be data-driven, do not request only a flattened mockup. Request the separated sprite pieces needed for production assembly.

### Cosmetic assets — exact current intake paths

The current code loads cosmetics from `Resources` as `Sprite`s.

#### Avatars

Folder:

- `Assets/Resources/cosmetics/avatars/`

Current filename pattern:

- `avatar_<name>_v001.png`

Current known keys:

- `avatar_moon_witch_v001`
- `avatar_sun_mage_v001`
- `avatar_garden_seer_v001`
- `avatar_star_caller_v001`

Recommended source/export size:

- export at `512x512`

Current UI display sizes:

- summary avatar render: about `126x126`
- profile preview avatar render: about `160x160`

Recommended art rules:

- square portrait
- transparent background
- readable face/silhouette at small size
- avoid ultra-fine details that disappear below 160 px

#### Frames

Folder:

- `Assets/Resources/cosmetics/frames/`

Current filename pattern:

- `frame_<name>_v001.png`

Current known keys:

- `frame_plain_gold_v001`
- `frame_violet_gem_v001`
- `frame_moonlit_vine_v001`

Recommended source/export size:

- `512x512` minimum
- `1024x1024` preferred if the frame has fine ornament detail

Current UI display sizes:

- summary frame render: about `150x150`
- profile preview frame render: about `190x190`

Recommended art rules:

- square overlay
- transparent center
- transparency around the outside edge
- avoid frame details that become muddy when reduced to 150-190 px

#### Dauber icons

Folder:

- `Assets/Resources/cosmetics/daubers/icons/`

Current filename pattern:

- `dauber_<name>_icon_v001.png`

Recommended source/export size:

- `256x256` minimum
- `512x512` preferred

Recommended art rules:

- transparent background
- simple centered symbol
- readable in selector UI at small scale

#### Dauber marks

Folder:

- `Assets/Resources/cosmetics/daubers/marks/`

Current filename pattern:

- `dauber_<name>_mark_v001.png`

Recommended source/export size:

- `256x256` minimum
- `512x512` preferred

Recommended art rules:

- transparent background
- centered mark/splat/sigil
- must remain clear when reduced to roughly bingo-cell scale
- avoid very thin lines that disappear when scaled down

### Shared UI assets — recommended Beta 1 production specs

These are not strict current-code requirements, but they are the recommended prep sizes for consistent production work.

#### Buttons

File type:

- `.png`

Preferred source sizes:

- small button: `256x96`
- medium button: `384x128`
- large CTA button: `512x160`

Recommended art rules:

- build with 9-slice in mind
- keep edge thickness consistent
- maintain enough interior padding for dynamic text

#### Panels / cards / modal shells

File type:

- `.png`

Preferred source sizes:

- small tile/card: `512x256`
- medium panel: `1024x512`
- large modal shell: `1536x1024`

Recommended art rules:

- support 9-slicing where practical
- corners and borders should scale cleanly
- keep center area flexible for text/content swapping

#### Icons

File type:

- `.png`

Preferred source sizes:

- standard UI icon: `128x128`
- more detailed feature/reward icon: `256x256`

Recommended art rules:

- transparent background
- strong silhouette
- readable at small scale

#### Badges / labels / states

File type:

- `.png`

Preferred source sizes:

- `256x96`
- `256x128`

Recommended art rules:

- should remain readable when scaled down
- avoid tiny text baked into the image unless necessary

#### Header flourishes / decorative accents

File type:

- `.png`

Preferred source sizes:

- header/banner accent: `1024x256`
- wide header/banner accent: `1536x384`

Recommended art rules:

- best used as overlays or decorative framing
- should not carry critical text unless explicitly designed as a title plate

#### Full-screen backgrounds / scenic backplates

File type:

- `.png`

Preferred source sizes:

- `1920x1080`

Recommended art rules:

- landscape orientation
- safe composition around UI overlay areas
- avoid placing critical detail where panels/buttons usually sit

If a screen depends on a scenic page backplate, request layered delivery when needed:

- base scenic background
- separate floor or stage plane if gameplay or navigation elements must feel grounded on it
- optional atmosphere overlay for glow, haze, vignette, or particles

Do not rely on a single flattened scene if the final screen needs movable bands, live labels, or independent foreground anchoring.

### Unity import guidance

Recommended default import settings for UI-critical sprites:

- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Single`
- Mesh Type: `Full Rect`
- Mip Maps: Off
- Alpha Is Transparency: On

Filter guidance:

- `Bilinear` for painted / soft / modern casual art
- `Point` only for intentional pixel-art assets

Compression guidance:

- low compression or uncompressed for UI-critical assets
- avoid heavy compression on avatars, frames, text plates, and button edges

### Practical safe-prep recommendation

If preparing assets now with the least risk of rework, use these defaults:

- avatars: `512x512` transparent PNG
- frames: `512x512` transparent PNG overlay
- dauber icons: `256x256` or `512x512` transparent PNG
- dauber marks: `256x256` or `512x512` transparent PNG
- icons: `128x128` or `256x256` transparent PNG
- buttons/panels: transparent PNGs designed for 9-slicing

## P0 — Main Loop Presentation Assets

### Gameplay screen

- [ ] Gameplay card visual treatment
- [ ] Gameplay card selected/active state
- [ ] Gameplay daub state treatment
- [ ] Called-number display styling
- [ ] Called-history / queue styling
- [ ] Gameplay left/right rail background treatment
- [ ] Gameplay resource display icons
- [ ] Gameplay power-up state icon/treatment
- [ ] Gameplay button hierarchy styling
- [ ] Gameplay status banner treatment

Notes:

- Prioritize clean readability over decorative detail.
- This is the most important Beta 1 screen.

### Room entry

- [ ] Room-entry header treatment
- [ ] Room-entry panel background treatment
- [ ] Card-count option tile treatment
- [ ] Card-count action button styling
- [ ] Mana bet row styling
- [ ] Restore-panel treatment
- [ ] Ingredient-progress readability treatment
- [ ] Room-entry jackpot panel styling
- [ ] Room-entry mode badge treatment

Notes:

- Room entry should feel like a strong “pre-match” screen, not a debug menu.

### Round-end rewards

- [ ] Reward preview panel treatment
- [ ] Round-summary stat tile treatment
- [ ] Collected-reward tile treatment
- [ ] Ingredient-progress-at-end-of-round tile treatment
- [ ] Reward action button styling
- [ ] Reward emphasis/accent art

### Card reveal

- [ ] Card reveal panel treatment
- [ ] Card reveal tile treatment
- [ ] NEW badge treatment
- [ ] Duplicate-state treatment
- [ ] Card rarity/frame presentation in reveal flow

### Jackpot wheel

- [ ] Jackpot wheel segment visuals
- [ ] Jackpot center button treatment
- [ ] Jackpot pot panel treatment
- [ ] Jackpot result panel treatment
- [ ] Jackpot collected-stack treatment
- [ ] Jackpot collect/return button styling
- [ ] Jackpot “big win” accent treatment

## P0 — Main Navigation And Hub Assets

### World map / realm map

- [ ] World Map header treatment
- [ ] Realm node treatment
- [ ] Realm locked/unlocked/completed states
- [ ] Room node treatment
- [ ] Room locked/unlocked/restored/selected states
- [ ] Navigation button styling for map flow

World map production-request reminder:

- request realm doors as separate sprites per visible realm
- request blank title plates when realm names are runtime text
- request potion bottles as separate sprites when spacing may change
- request rail backgrounds, arrows, markers, and line as separate sprites
- request profile/header shells as separate backing sprites, not flattened with avatar or live counters
- request resource capsules with separate shell, icon, plus button, and empty number-safe interior

Current approved Realm 1 hero-row implementation sizes:

- door frame base assets: `466 x 596`
- door interior and locked insert assets: `293 x 394`
- potion inner/outer assets: `51 x 78`
- potion spacing: `15px`
- title plate left cap: `94 x 82`
- title plate center fill: `96 x 82`
- title plate right cap: `98 x 82`
- title plate short assembled width: `288px`
- title plate long assembled width: `384px`
- adjacent door-base overlap target: about `50px`

### Player’s Den

- [ ] Den top-band treatment
- [ ] Den title treatment
- [ ] Den destination tile system
- [ ] Den route iconography/accent treatment
- [ ] Den bottom action-tile treatment
- [ ] Den background/floor/wall visual treatment

Notes:

- Den should feel like a real hub, even if some routes remain lighter-weight.

## P0 — Identity / Customization Assets

### Avatars

- [ ] Avatar 01
- [ ] Avatar 02
- [ ] Avatar 03
- [ ] Avatar 04
- [ ] Avatar 05 optional
- [ ] Avatar 06 optional

### Frames

- [ ] Frame 01
- [ ] Frame 02
- [ ] Frame 03
- [ ] Frame 04

### Daubers

- [ ] Default dauber
- [ ] Early unlock dauber
- [ ] Later unlock dauber

Notes:

- For Beta 1, breadth is less important than visual quality and clear differentiation.

## P1 — Shared UI System Assets

### Panel kit

- [ ] Primary panel background
- [ ] Secondary panel background
- [ ] Modal panel background
- [ ] Highlight/accent panel variant
- [ ] 9-slice-ready corners/edges if needed

### Button kit

- [ ] Primary button
- [ ] Secondary button
- [ ] Disabled button
- [ ] Reward/claim button variant
- [ ] Warning/blocking button variant
- [ ] Small utility button variant

### State badges / labels

- [ ] Locked
- [ ] Ready
- [ ] Claimed
- [ ] New
- [ ] Restored
- [ ] Selected
- [ ] Waiting / reconnecting if needed visually

### Core icon set

- [ ] Mana icon
- [ ] Crystal icon
- [ ] Power-up icon
- [ ] Jackpot icon
- [ ] Inbox icon
- [ ] Friends/social icon
- [ ] Coven/team icon
- [ ] Collection/library icon
- [ ] Daily Bonus icon
- [ ] Daily Spin icon
- [ ] Settings icon

## P1 — Collection / Retention / Support Screen Assets

### Library / Grimoire

- [ ] Library landing treatment
- [ ] Grimoire index tile treatment
- [ ] Grimoire detail card-slot treatment
- [ ] Missing/owned/duplicate/new card state visuals
- [ ] Book of Shadows visual distinction treatment
- [ ] Card gifting row/tile treatment

### Daily systems

- [ ] Daily Bonus panel treatment
- [ ] Daily reward tile treatment
- [ ] Daily Spin wheel/panel treatment
- [ ] Daily Spin result emphasis treatment

### Inbox / friends / coven

- [ ] Inbox row treatment
- [ ] Inbox tab treatment
- [ ] Message/detail modal treatment
- [ ] Friend list row treatment
- [ ] Friend request row treatment
- [ ] Coven member row treatment
- [ ] Coven wish/request row treatment

### Cabinet / Apothecary

- [ ] Cabinet inventory row/tile treatment
- [ ] Apothecary panel treatment
- [ ] Ingredient row/tile treatment
- [ ] Potion progress treatment

## P2 — Lower-Priority Beta Shell Assets

- [ ] Leaderboard row polish
- [ ] Market category tile polish
- [ ] Oracle Alley shell art
- [ ] Enchanted Trail shell accents
- [ ] Relic Wall shell polish
- [ ] Message composer embellishment
- [ ] Secondary decorative flourishes

## External Asset Sourcing Checklist

Use this section when evaluating outside packs/resources.

### Foundation pack

- [ ] One main UI pack selected
- [ ] Fits bright magical casual tone
- [ ] Works with clean readability-first gameplay
- [ ] Can be sliced/reused without fighting the layout
- [ ] Does not push the game into dark RPG, grim fantasy, or medieval-heavy tone

### Icon support pack

- [ ] One main icon source selected
- [ ] Includes enough economy/reward/support symbols
- [ ] Style is compatible with chosen panel/button kit
- [ ] Does not require broad replacement immediately

### Avatar fallback source

- [ ] Temporary avatar source selected if custom avatars are not ready
- [ ] Portrait style is compatible with the game tone
- [ ] Can be framed cleanly inside the profile/customization UI

### Filler asset use rules

- [ ] No more than one primary UI skin source
- [ ] Low-priority filler art does not clash visibly with core screens
- [ ] Temporary pack use is documented so bespoke replacements are easier later

## Placeholder-Acceptable Asset Areas

These can use simpler or temporary art for Beta 1 if the main loop is stronger:

- Leaderboards
- Mayhem Market shell
- Oracle Alley shell
- Enchanted Trail shell
- Deep Coven management
- Advanced friend messaging
- Secondary utility modals

## Bespoke-First Asset Areas

These should receive custom attention first:

- Gameplay board and gameplay rails
- Room entry
- Reward preview
- Card reveal
- Jackpot wheel
- Den destination surfaces
- Avatar/frame/dauber presentation

## Recommended Production Order

1. Shared panel/button/icon system
2. Gameplay board and gameplay rails
3. Room entry
4. Reward preview and card reveal
5. Jackpot wheel
6. World/realm navigation
7. Player’s Den
8. Profile/customization assets
9. Library / daily systems
10. Social/support shells

## Working Notes Section

Use this section to track sourcing and ownership decisions.

### Temporary external source candidates

- Main UI skin:
  - [ ] selected
  - [ ] not yet selected

- Icon source:
  - [ ] selected
  - [ ] not yet selected

- Avatar fallback source:
  - [ ] selected
  - [ ] not yet selected

### Replacement plan

- [ ] temporary assets acceptable for Beta only
- [ ] temporary assets acceptable through wider test phase
- [ ] bespoke replacement required before external Beta

### Asset readiness snapshot

- [ ] P0 mostly ready
- [ ] P1 partially ready
- [ ] P2 intentionally deferred

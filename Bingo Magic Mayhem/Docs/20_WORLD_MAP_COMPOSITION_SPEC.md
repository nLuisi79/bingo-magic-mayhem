# 20 - World Map Composition Spec

## Purpose

This file is the working composition contract for the Beta 1 world map screen in the Unity prototype.

It exists because iterative offset-only tweaks have not been enough to make the current runtime-built screen match the target mockups closely. The next implementation passes should use this spec as the visual/layout guide so we can separate:

- layout problems,
- asset problems,
- and placeholder-art limitations.

This is not a gameplay lock. It is a screen-composition and asset-slot spec.

## Scope

This spec applies to the runtime-built World Map screen in:

- `Assets/Scripts/BingoPrototype.cs`

This spec is specifically for the current five-visible-door composition direction:

- Realm 1 visible and available first
- four additional visible doors in the main hero row
- potion placeholders under each visible realm cluster
- center banner strip
- bottom realm rail

## Asset Request Standard

Any new or replacement art request for this screen must be written as a production-ready intake request, not a loose visual ask.

Every request should explicitly state:

- exact asset type: sprite, layered sprite set, overlay, icon, plate, background, or text-safe shell
- whether the asset is for final production, near-final prototype, or temporary placeholder
- whether Unity needs one flattened sprite or multiple separated sprites
- whether the center or interior must remain empty for runtime text, numbers, icons, badges, or meters
- whether glows, frames, gems, plus buttons, counters, highlights, or decorative flourishes need separate exports
- target orientation and intended display size band
- whether the asset should support `Image`, `Button`, or 9-slice usage

If an element contains dynamic content, request the non-baked version up front.

Examples:

- profile/avatar shell: request separate avatar art, frame overlay, badge plate, level badge, and any number-safe empty capsule areas
- resource capsule: request empty/background shell, icon, plus button, and any glow or ornament overlays as separate sprites when values are runtime text
- title plate: request a clean plate sprite without baked realm text when labels are runtime-rendered
- navigation rail: request separate background, arrows, line, current marker, plain marker, and labels kept as live text

Do not request only a flattened mockup if the implementation needs interchangeable or data-driven UI parts.

## Primary target reference

Current strongest page-shape reference:

- `C:\Users\nLuis\OneDrive\Desktop\nLuisi Laptop\Bingo Magic Mayhem\realms- bingo\ChatGPT Image Jul 18, 2026, 05_21_03 AM.png`

Use that mockup as the primary composition target for:

- top shell proportions
- hero door row dominance
- plate attachment
- potion-row spacing
- banner placement
- bottom rail grouping

## Composition rule

The world map must be treated as four grouped horizontal bands, not as isolated absolute-position elements:

1. Top shell
2. Five-door hero row
3. Center banner strip
4. Bottom realm rail

Each band should be internally composed relative to a local parent container. Future layout changes should prefer adjusting band containers first, then child elements inside the band.

## Screen frame

Current runtime stage:

- `1600 x 900`

World map composition should preserve a clear vertical rhythm:

1. top shell sits high and thin
2. hero row dominates the middle of the screen
3. banner sits clearly below the potion row
4. rail sits clearly below the banner

The main visual mistake to avoid is band collision:

- plates colliding with neighboring plates
- potion rows colliding with banner
- banner colliding with rail
- rail labels falling into the black gutter below the bar

## Band 0 - Background Container

### Intent

The background is not just decoration. It is the first visual container that establishes where every other band can sit.

If the background is too flattened or too compositionally busy, every overlay band has to fight it.

### Role

The background should provide:

- the cosmic atmosphere of the page
- the stage or floor plane that grounds the five-door row
- safe quiet zones for header, banner, and rail readability

### Recommended layer model

For production readiness, treat the background as up to three separate layers even if the current prototype only uses one:

1. base scenic background
2. floor or stage plane
3. optional atmosphere overlay

### Production-ready request requirements

If background art is being created or recreated, request:

- one full-screen base background sprite sized for `1920x1080`
- one separate floor, platform, or stage-plane sprite if the lower page needs to anchor doors, plates, potions, banner, and rail independently
- one optional overlay sprite for haze, glow, particles, vignette, or atmospheric depth if that effect should remain adjustable
- clear safe zones for the top shell
- clear safe zones for the center banner
- clear safe zones for the bottom rail and labels

### What should not be baked in

Do not bake the following into the background if they may need to move independently:

- title plates
- potion rows
- center banner
- rail markers or labels
- profile/resource text areas

### Current implementation state

Current Unity resource:

- `prototype/ui/world_map/backgrounds/world_map_bg_v001`

Status:

- workable as a prototype full-page backplate
- likely too flattened to support a polished final composition by itself

### Likely missing production piece

The most likely missing asset for this screen is a separated floor or stage-plane sprite.

That would let us anchor:

- realm doors
- title plates
- potion rows
- banner
- rail

without forcing all of them to compete with a single baked background composition.

## Band 1 - Top Shell

### Intent

The top shell should read as one light-weight header band across the screen, not a heavy opaque slab.

### Contents

- logo at top-left
- player profile cluster next to logo
- resource capsules across the top-right
- home/settings shell controls at the far right

### Target behavior

- The top shell must remain visually thinner than the hero row.
- The logo + profile cluster should feel like one grouped block.
- Resource capsules should align to a common top baseline.
- The shell should not visually compete with the realm doors.

### Current implementation issue

The prototype top shell still reads more like a simplified debug/prototype band than the richer mockup shell.

### Asset/spec notes

- Existing shell is acceptable as prototype.
- If new shell art is recreated, keep it narrow vertically and let the hero row dominate the page.

### Exact production asset breakdown

The top shell should be treated as four sub-clusters:

1. logo cluster
2. profile cluster
3. resource capsule row
4. right utility controls

#### Logo cluster

Current working asset:

- `prototype/ui/world_map/logo_v001`

Status:

- workable for prototype
- likely acceptable through polish if the rest of the shell improves

Request requirements if recreated:

- one flattened logo sprite for immediate use
- transparent background
- no extra backing panel baked into the logo file

#### Profile cluster

Current implementation is assembled from:

- avatar sprite: `cosmetics/avatars/avatar_moon_witch_v001`
- frame sprite: `cosmetics/frames/frame_plain_gold_v001`
- runtime text for player name
- runtime text for level
- runtime text for XP progress
- a generic dark backing panel drawn in code

Status:

- avatar and frame split is correct structurally
- current backing panel is prototype-only
- current profile group does not yet match the mockup language

Production-ready request requirements:

- separate avatar art sprite
- separate frame overlay sprite
- separate profile backing shell sprite
- separate level badge sprite if level should not sit as plain text
- empty text-safe area for player name
- empty text-safe area for XP/progress copy

Do not request this as one flattened profile image. The profile cluster contains live player-specific data.

#### Resource capsule row

Current implementation uses:

- `prototype/ui/nav/nav_mana_empty_v001`
- `prototype/ui/nav/nav_mana_v001`
- `prototype/ui/nav/nav_crystal_empty_v001`
- `prototype/ui/nav/nav_crystal_v001`
- `prototype/ui/nav/nav_powerup_empty_v001`
- `prototype/ui/nav/nav_powerup_v001`
- runtime value text
- runtime plus glyph/button treatment

Status:

- structurally reusable for prototype
- visually still too placeholder-like compared to target shell

Production-ready request requirements per capsule:

- separate empty or background capsule sprite
- separate filled icon sprite
- separate plus button sprite
- optional separate glow or accent overlay if used
- number-safe empty interior sized for live values

Do not bake numbers into the capsule art. These are runtime counters.

#### Right utility controls

Current implementation uses:

- `prototype/ui/nav/home_icon_v001`
- `prototype/ui/nav/settings_icon_v001`

Status:

- workable for prototype
- likely need cleaner circular button shells for final match

Production-ready request requirements:

- separate button shell/background sprite
- separate icon sprite for each function
- optional hover/active overlay only if interactive state polish is needed later

### Current top-shell mismatch diagnosis

The top shell still feels unlike the mockup for three specific reasons:

1. the header band is still a generic code-drawn rectangle rather than a purpose-built shell
2. the profile cluster lacks a real backing treatment and badge hierarchy
3. the resource capsules are assembled from usable parts but not yet from a fully coherent final set

## Band 2 - Five-Door Hero Row

### Intent

This is the visual centerpiece of the page.

The current page will not feel like the mockup until the five-door row reads as one intentional hero composition rather than five repeated nodes.

### Contents per realm cluster

Each visible realm cluster is:

1. door art
2. title plate attached directly beneath the door base
3. potion row directly beneath the plate

### Locked composition rules

- exactly five visible realm doors in the main row
- Realm 1 appears first on the left
- the row must feel centered as one grouped feature
- the outer doors can be slightly larger/more dominant than inner doors, but the difference must be modest enough to avoid collision
- title plates must visually attach to the door base, not float well below it
- potion rows must feel subordinate to the plate, not like a second primary label row

### Target row behavior

- door silhouettes should overlap visually in rhythm, not literally collide
- clusters should have tighter spacing than the early prototype
- cluster spacing must leave readable gaps between title plates
- the row should feel taller and more premium than the current flat prototype

### Approximate target hierarchy

- Door art is the dominant element.
- Plate is the secondary anchor.
- Potion row is the tertiary progress indicator.

### Asset/spec notes

Current prototype limitation:

- 4/5 realms still use repeated locked-door art

This is the biggest remaining reason the page still feels unlike the mockup even when layout improves.

Recreation or intake priority for this band:

1. distinct realm door/portal art for at least visible realms 2-4
2. final or closer-match title plate treatment
3. improved potion bottle placeholder set if current bottles remain too prototype-like

When replacement assets are requested for this band, ask for:

- each realm door or portal as its own sprite
- separate optional glow, lock, or active-state overlays if those states may change at runtime
- a blank title plate sprite with no baked realm name
- potion bottle sprites as separate per-bottle assets, not a baked row, if spacing may keep changing

### Locked Realm 1 cluster measurements

These values are the current approved implementation targets for the first final-quality realm cluster.

#### Realm 1 door kit

Approved source asset sizes:

- broken frame base: `466 x 596`
- restored frame base: `466 x 596`
- broken interior: `293 x 394`
- restored interior: `293 x 394`
- portal interior: `293 x 394`
- locked insert: `293 x 394`

Approved layer stack:

- `Locked` = locked insert behind broken frame
- `UnlockedBroken` = broken interior behind broken frame
- `UnlockedRestored` = restored interior behind restored frame
- `UnlockedPortal` = portal interior behind restored frame

Frame art is the top layer for every state.

#### Realm 1 potion kit

Approved source asset sizes:

- potion inner: `51 x 78`
- potion outer: `51 x 78`

Approved layer stack:

- potion inner on bottom
- potion outer on top

Approved spacing:

- `15px` between adjacent bottles

Approved runtime behavior:

- inner layer can be tinted in Unity
- outer layer remains unchanged
- under-bottle glow can be generated in Unity instead of baked into the source art

#### Title plate kit

Approved segmented plate pieces:

- left cap: `94 x 82`
- center fill: `96 x 82`
- right cap: `98 x 82`

Approved assembly rule:

- fixed height
- width grows horizontally by adding center-fill span
- live realm name text renders on top

Approved assembled widths:

- short plate = `288px` total width
- long plate = `384px` total width

#### Five-door spacing note

The visible five-door composition should allow the door bases to overlap by about `50px` from one adjacent door cluster to the next.

## Band 3 - Center Banner Strip

### Intent

The banner should function as a clean separator between the hero row and the bottom rail.

It should not crowd the potion row and should not feel glued to the rail.

### Contents

- center banner asset
- no extra duplicate text on top of it

### Target behavior

- centered horizontally
- wide enough to feel intentional, but narrower than the full rail width
- enough vertical separation above and below to read as its own band

### Current asset

- `realm banner text.png`

Unity resource:

- `Assets/Resources/prototype/ui/nav/realm_banner_text_v001.png`

### Asset/spec notes

Banner art is workable and close enough for prototype use.

If this banner is recreated, request:

- one flattened production sprite for immediate use
- one empty or text-safe base variant if the copy may change later

## Band 4 - Bottom Realm Rail

### Intent

The rail should read as one cohesive navigation component.

### Allowed asset set

Only these images should define the rail:

- background bar
- left arrow
- right arrow
- long line
- landed/current marker
- plain marker

### Rail structure

The rail must be built as a grouped component with:

1. background
2. arrows inside left and right ends of the background
3. long centered line
4. landed/current marker aligned to the visible start of the line
5. repeated markers distributed along the visible line span
6. labels directly below markers, inside the grouped rail composition

### Current marker rule

- plain markers should never render below `36px`

### Current implementation notes

The rail finally behaves more like one component after being rebuilt under a local `railRoot`.

The remaining task is proportion polish, not conceptual structure.

## Exact current asset-slot map

### World map background

- `prototype/ui/world_map/backgrounds/world_map_bg_v001`

### Realm door states

- `prototype/ui/world_map/realms/realm_01/door_unrepaired_v001`
- `prototype/ui/world_map/realms/realm_01/door_unlocked_v001`
- `prototype/ui/world_map/realms/realm_01/door_complete_v001`
- `prototype/ui/world_map/realms/realm_01/door_locked_v001`

### Title plate

- `prototype/ui/world_map/realms/realm_01/realm_TitlePlateSmall`

### Potion placeholder

- `prototype/ui/world_map/potions/potion_empty_v001`

### Bottom rail assets

- `prototype/ui/nav/nav_progress_bg_v001`
- `prototype/ui/nav/nav_progress_line_v001`
- `prototype/ui/nav/nav_progress_landed_v001`
- `prototype/ui/nav/nav_progress_marker_v001`
- `prototype/ui/nav/nav_arrow_left_v001`
- `prototype/ui/nav/nav_arrow_right_v001`

### Center banner

- `prototype/ui/nav/realm_banner_text_v001`

## Current mismatch summary

The current prototype still feels different from the mockup for three main reasons:

### 1. Repeated realm art

The mockup gets much of its identity from distinct portal/realm visuals per visible realm.

The current prototype still repeats locked-door art across multiple realms.

### 2. Shell language mismatch

The top shell/profile/resource area is still more prototype-like than the mockup.

### 3. Hero-row composition sensitivity

The five-door row has been highly sensitive to small numerical changes because all clusters are still runtime-built from absolute coordinates inside one script.

## What to do next

### Implementation order

1. Keep the current grouped-band approach.
2. Tune the five-door row only in grouped hero-row terms, not random element-by-element offsets.
3. When a realm cluster still feels wrong, first ask:
   - is this a spacing problem?
   - a scale problem?
   - or an art-identity problem?
4. Prefer creating/replacing missing realm-specific art over endlessly compensating in layout once the grouped composition is close.

### Asset recreation priorities

Highest value:

1. distinct visible realm door/portal art for realms 2-4
2. closer-match title plate if the current plate keeps reading as a different visual language
3. top-shell cleanup assets if current profile/resource shell remains too prototype-like

### Working rule

If a pass changes more than one of these at once:

- door scale
- plate scale
- cluster spacing
- potion spacing
- banner spacing

then the change should be considered a composition pass and evaluated against the full page, not judged in isolation.

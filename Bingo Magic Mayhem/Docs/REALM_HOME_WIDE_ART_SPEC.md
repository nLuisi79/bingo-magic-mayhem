# Realm Home Wide ("Bleed") Background Art Spec

Status: **HIGH / LIKELY FINAL** (approved 2026-11-20). Code support is in place, and the game falls back to the current 16:9 art until wide files are delivered.

## Why

The realm home art is 16:9 (1920×1080 px = 1600×900 stage units). The room medallions are placed on that art, so the game always shows the whole 16:9 image. On screens with a different shape, the leftover area is currently filled with a dimmed, enlarged copy of the art:

| Screen | Leftover area today |
|---|---|
| Modern phones (19.5:9–21:9) | Strips on the left and right, up to ~22% of the width |
| iPads (4:3, 3:2) | Bands above and below, ~19% of the height |
| Foldables (near-square) | Larger bands above and below |

Wide art replaces those dimmed areas with real scenery.

## Canvas

| | Stage units | Pixels to deliver (1.2 px/unit) |
|---|---|---|
| **Full wide canvas** | **2200 × 1350** | **2640 × 1620** |
| Original composition (centred, untouched) | 1600 × 900 | 1920 × 1080 |
| Extra on the left and right | 300 each | 360 px each |
| Extra on the top and bottom | 225 each | 270 px each |

```
+--------------------------------------------------------------+  2640 x 1620 px
|                        270 px bleed                          |
|        +--------------------------------------------+        |
| 360 px |   ORIGINAL 1920 x 1080 ART - PIXEL-IDENTICAL   | 360 px |
| bleed  |   (room medallions, paths and titles line up)  | bleed  |
|        +--------------------------------------------+        |
|                        270 px bleed                          |
+--------------------------------------------------------------+
```

Rules:
1. **Keep the centre pixel-identical:** the centre 1920×1080 must match the existing art exactly, starting at pixel offset (360, 270). Medallion positions come from that area, so any shift misaligns the rooms.
2. **Keep the edges low-detail:** the bleed areas are only seen on some devices. Put no gameplay-relevant elements there (paths, rooms, text). Use scenery continuation such as foliage, sky, water and terrain.
3. **Avoid hard seams:** paint or generate the bleed as a continuation of the scene so it reads as one image.
4. **Keep the visual direction:** follow 07_UI_VISUAL_DIRECTION.md (bright, whimsical, not dark gothic or muddy).

## Which images

Each realm has five state backgrounds. Every state needs its own wide version:

| Existing file | Wide file to add |
|---|---|
| `<realm>-realm-broken0.png` | `<realm>-realm-broken0-wide.png` |
| `<realm>-realm-broken1.png` | `<realm>-realm-broken1-wide.png` |
| `<realm>-realm-broken2.png` | `<realm>-realm-broken2-wide.png` |
| `<realm>-realm-broken3.png` | `<realm>-realm-broken3-wide.png` |
| `<realm>-realm-restored.png` | `<realm>-realm-restored-wide.png` |

Location: `Assets/Resources/prototype/ui/realm_home/<realm>/`, for example `sunpetal/sunpetal-realm-broken0-wide.png`.

Format: PNG, sRGB, no transparency needed.

Suggested order: Sunpetal Conservatory first (the most-seen realm), then Floating Lotus Lagoon, then the rest in realm order. Wide art can arrive one realm (or one state) at a time. Any missing file simply falls back to the 16:9 version.

## What the game does (implemented)

- **Loading:** `BingoPrototype.RealmHome.cs` (`CreateRealmHomeBackground`) loads `<state>-wide` when it exists and places it at 2200×1350 stage units, centred on the stage. Otherwise it uses the 16:9 art.
- **Fallback for unusual screens:** the dimmed ambient backdrop still sits behind the art, in case a screen ever exceeds the wide envelope.
- **Import settings:** `Assets/Editor/RealmHomeWideArtImporter.cs` sets the right options for any `*-wide.png` in the realm-home folder: no power-of-two rescaling, a 4096 max size and no mipmaps.
- **Envelope test:** `StageFitTests.WideRealmArtCoversTheWholeScreen` checks that 2200×1350 covers the full screen on 12 phone, tablet, foldable and desktop sizes, including notch, camera-cutout and home-indicator areas.

## Open items

- `TODO(product)`: texture memory. Each wide background is about 4.3 megapixels. Confirm the per-platform compression (ASTC on mobile) during the performance pass.

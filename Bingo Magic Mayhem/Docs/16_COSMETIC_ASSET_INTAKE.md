# Cosmetic Asset Intake

Last updated: 2026-07-09

Status: technical Beta asset contract. Working cosmetic names and exact unlock/ownership rules are not product locks.

## Runtime Locations

Export final PNG sprites beneath `Assets/Resources/cosmetics` using the exact catalog filenames:

```text
avatars/avatar_moon_witch_v001.png
avatars/avatar_sun_mage_v001.png
avatars/avatar_garden_seer_v001.png
avatars/avatar_star_caller_v001.png
frames/frame_plain_gold_v001.png
frames/frame_violet_gem_v001.png
frames/frame_moonlit_vine_v001.png
daubers/icons/dauber_classic_star_icon_v001.png
daubers/marks/dauber_classic_star_mark_v001.png
```

Continue the same icon/mark naming pattern for Moon Drop, Crystal Spark, and Garden Bloom. Do not create final Rank Frame art yet; `frame_rank_placeholder` intentionally has no asset key while rank cosmetic rules remain open.

## Export And Import Settings

| Asset | Export | Unity max size | Notes |
|---|---:|---:|---|
| Avatar | 1024x1024 RGBA PNG | 1024 | Face/details inside central circular safe area. |
| Frame | 1024x1024 RGBA PNG | 1024 | Transparent center; avatar is not baked in. |
| Dauber selector icon | 512x512 RGBA PNG | 512 | Readable as a small inventory/profile icon. |
| Daub mark | 256x256 RGBA PNG | 256 | Must not completely obscure the bingo number. |

Unity importer:

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- sRGB: enabled
- Alpha Is Transparency: enabled
- Mesh Type: Full Rect for predictable UI alignment
- Pivot: Center
- Mip Maps: disabled for these UI sprites
- Compression: Normal for prototype; final platform overrides remain a production task

Keep layered 2048x2048 master files outside the runtime folder or in an approved art-source location. Runtime code expects only exported PNGs.

## Catalog And Resolver

`Assets/Scripts/Cosmetics/CosmeticCatalog.cs` owns stable ids, display labels, and logical asset keys. The profile UI automatically loads available sprites and retains its text/color placeholder when an asset is absent.

`ResourcesCosmeticSpriteResolver` is the local Beta adapter. A later Addressables adapter can replace it without changing cosmetic ids or profile save data.

## Display Name Scope

Profile settings snapshot schema 2 adds a durable local display name. Current validation is explicitly Beta/test-only:

- 3-24 characters;
- letters, numbers, spaces, apostrophes, and hyphens;
- surrounding whitespace trimmed and repeated whitespace collapsed.

Final moderation, uniqueness, localization, reserved-word, parental-control, and backend enforcement rules remain open. Journal records note that a custom name exists but do not store the display-name text.

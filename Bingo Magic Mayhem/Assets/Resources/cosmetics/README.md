# Cosmetic Runtime Asset Intake

Place exported PNG sprites at the exact catalog paths below. Filenames are stable runtime keys; replace image contents without renaming the key.

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

Continue the same naming pattern for Moon Drop, Crystal Spark, and Garden Bloom. Import every PNG as Sprite (2D and UI), Single, sRGB, Alpha Is Transparency, with no baked text. Avatar/frame masters export at 1024x1024; dauber icons at 512x512; daub marks at 256x256.

`ResourcesCosmeticSpriteResolver` is the local Beta adapter only. Stable ids and logical keys are preserved so an approved Addressables adapter can replace it later.

# 07 — UI and Visual Direction

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

## Major system confidence

| Visual/UI system | Confidence | Codex/design handling |
|---|---|---|
| Blossomveil Promenade as room-entry style anchor | LOCKED | Use as source of truth for room-entry layout and visual density. |
| Cream parchment, gold borders, deep purple banners, green buttons | LOCKED | Use in UI theme. |
| Stylized magical storybook casual-game rendering | LOCKED | Use as art direction. |
| Logo direction | OPEN | Do not finalize logo from old explorations. |
| Purple side arrows preference | HIGH PREFERENCE | Preserve where collection navigation uses arrows, unless UX changes. |
| Removed/reclaimed emblem space | HIGH PREFERENCE | Do not waste top page space on low-value emblem. |
| Oracle/tarot/crystal-ball visuals | PROPOSED | Use as concept direction only. |
| Rejected dark/noisy/rogue-layout directions | LOCKED EXCLUSIONS | Avoid. |

## Complete visual/UI extraction

Inventory-level visual direction based on later locked UI style, Project discussions, generated/reference concepts, and rejected design paths

## Document guardrails

This is a visual/UI direction document, not a final asset list and not a new art style exploration.

Later locked direction takes precedence over early broad creative direction. The approved Blossomveil Promenade room-entry screen is treated as the current visual calibration target.

Early fantasy/world ideas are included only when they still influence the visual language or are called out as deprecated/rejected.

Do not move locked UI regions, rename rooms/realms, add decorative systems, or generate new layouts unless intentionally redesigning UX with approval.

All gameplay-facing UI must remain mobile-readable in forced landscape.

## 1. Overall visual tone

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Core tone | Bright magical adventure with collectible fantasy objects, rewarding feedback, whimsical magic, and polished casual-game readability. | Avoid dark gothic fantasy, solemn wizard fantasy, casino clones, generic medieval kingdoms, and overly realistic scenic fantasy. |
| Current target | Stylized magical storybook UI with polished casual-game rendering. The game should feel whimsical, bright, collectible, premium, and friendly. | Should be less painterly than older realm maps and closer to the Mischievous Witch, Whisk, Ancient Guardian, and loading-screen references. |
| Energy level | High-energy and rewarding without becoming cluttered. Every screen should imply progress, collecting, magic, and a near-term next action. | Effects must not obscure gameplay; reward animations should celebrate without delaying flow too much. |
| Audience feel | Whimsical but not childish; polished but not overly luxury; magical but not visually heavy. | Use approachable, readable shapes instead of thin elegant fantasy styling. |

## 2. Logo direction

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Readability | The logo must read quickly at mobile splash, app icon context, store preview, and in-game header sizes. | Avoid thin fantasy fonts, ornate gothic letters, and strokes that close up at small sizes. |
| Tone | Punchy, rounded, high-energy, friendly magical game logo. | Should communicate bingo + magic + motion/blast + collectibility. |
| Motifs | Possible use of a bingo ball integrated into the O/dot, star sparks, magical burst behind Blast, portal ring, purple/blue/gold gradient, crystal highlight, slight 3D mobile-game treatment. | Do not make the wheel the main logo symbol; jackpot/wheel visuals are a feature, not the brand identity. |
| Color | Deep violet base, electric blue/teal portal accents, warm gold reward highlights, magenta/pink energy, crystal-white highlights. | Keep enough contrast against both dark and light backgrounds. |

Logo rejection notes: previous logo directions that looked unlike the built world should not be repeated. The logo should borrow from the approved UI palette and collectible/storybook treatment rather than from generic casino bingo or medieval fantasy branding.

## 3. Magical fantasy style

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Rendering | 2D illustrated mobile game UI with soft gradients, rounded dimensional panels, polished magical objects, readable icons, particles, sparkles, smoke, glows, and light trails. | No photoreal materials; no muddy realism; no tiny overworked background details. |
| Environment treatment | Colorful, readable, stylized rather than realistic; clean silhouettes; slightly exaggerated shapes and scale; rich in magic but not noisy. | Backgrounds must support UI readability; do not compose cinematic/scenic backgrounds that compete with the interface. |
| UI material language | Cream parchment panels, gold beveled borders, deep purple banners, green action buttons, bright readable icons, large fantasy serif titles, clean hierarchy. | Ornate enough to feel magical; never so ornate that gameplay and counts become hard to scan. |
| Icon treatment | Glossy, collectible, magical, centered, and readable at small sizes. Potion/ingredient icons should sit in round frames in progress rows. | Avoid icons that read as room scenes, buildings, jewelry when not jewelry, or miniature paintings. |
| Primary palette | Deep purple, warm gold, magenta/pink magic, lavender glow, teal accents, cream parchment, and green buttons. | Realm palettes may vary, but should remain inside the brighter character-friendly system. |

## 4. Locked room-entry UI direction

The approved Blossomveil Promenade room-entry screen is the current calibration screen for UI density, layout, rendering, and screen hierarchy. Major regions should not move unless the UX is intentionally redesigned.

| Element | Locked placement / treatment | Notes |
| --- | --- | --- |
| Back button | Top left | Keep small but obvious; do not create competing menu clutter. |
| Currency bar | Top right | Must remain readable and aligned with broader currency decisions. |
| Room title card | Top left/center | Title should be readable; potion/restoration object belongs on the left side of the title card. |
| Potion/restoration item | Left side of title card | Replaces meaningless decorative room flower/emblem. |
| Jackpot wheel | Top right | Must show the jackpot amount clearly; players care about a large pot. |
| Ingredient progress strip | Full-width band beneath title area | Use small readable collectible icons/cards; can later expand to detailed ingredient inventory. |
| Restore reward | Right side of ingredient strip | Restore reward should show mana/card packs, not a chest. |
| Pick cards section | Center/lower main panel | Primary purpose of this screen is choosing card amount and playing bingo. |
| Card options | Four columns: 1 / 2 / 4 / 6 | Options should show cost/bet relationship clearly. |
| Card view toggle | Bottom left | Keep out of primary decision zone but reachable. |
| Mana bet per card | Bottom center | The economic control should be visible and understandable. |
| Active power-ups | Bottom right | Power-up icons should be compact, readable, and not confused with generic category icons. |

## 5. Bingo gameplay screen UI

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Gameplay priority | Readability is the highest priority during bingo play. Called number, board cells, daubs, card count, and win state must be instantly legible. | Special effects must never hide cards, numbers, or player taps. |
| Multiple cards | The screen must support multiple bingo cards cleanly. | Card scaling, swipe/toggle, or layout mode must preserve tap accuracy. |
| Power-ups | Power-up buttons should live in thumb-friendly zones and be visually distinct by function. | Do not use a generic power-up symbol that looks like a wand/bolt if those are individual power-ups. |
| Event drops | The gameplay screen may surface reward/progress drops such as ingredients, cards, or event calls, but these should not distract from daubing. | Use short animated confirmations rather than large blocking popups. |
| Base bingo vs setup | Do not confuse the room-entry setup screen with the active bingo play screen. The setup screen chooses card count/bet; the gameplay screen handles numbers/daubing/wins. | Called balls belong in active play, not on room-entry setup. |

## 6. Rewards and completion UI

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Potion completion | Use a ceremonial composition: potion beside/near banner, title + Complete, ingredient cards below in an arched tarot-spread feel, rewards below. | Avoid a generic rectangular popup; avoid checkmarks on ingredient cards. |
| Ingredient cards | Cards shown after completion should look like the redeemed cards, with at least title and stars visible. | Cards can be small, but not so small that they become anonymous icons. |
| Reward reveal | Use cascading reveal, glow, sparkles, and clear category grouping for mana, card packs, cards, power-ups, crystals, wild cards, and progress. | Do not overfill the summary screen; group duplicate/conversion feedback. |
| New-to-You | New cards should receive a highlighted reveal treatment distinct from duplicate cards. | Avoid making duplicates feel like total failure; conversion feedback should be visible and reassuring. |
| Near completion | Reward summary may prompt near-complete collections, missing ingredient/card needs, or Coven help. | Prompts should be helpful and not aggressive. |

## 7. Player Den

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Function | Player Den is the personal hub / owned magical space. It can contain inventory, collections, relics, achievements, event trophies, and social contribution honors. | Do not treat it as merely decorative if inventory and relic systems are expected to live there. |
| Visual tone | Cozy magical personal room, warm light, collectible shelves, potion work areas, books, magical objects, glowing details, and personal progression display. | Avoid dark, dusty, horror-like witch hut treatment. |
| Navigation | Should serve as a hub for Apothecary, Library, Relic Wall, and Cabinet of Curiosities / power-up inventory. | Keep paths clear; do not bury gameplay-critical inventory behind unclear decoration. |
| Generated concepts | Player Den concepts requested/discussed include apothecary, library, and relic wall views. These should influence the hub as distinct zones, not unrelated menu screens. | Final room composition and whether it is navigable/panoramic still needs lock. |

## 8. Apothecary

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Apothecary is the natural visual home for potions, ingredients, crafting/restoration progress, and possibly ingredient requests/gifting. | Exact mechanics are not fully locked; visual design should support inventory and progression without implying unsupported crafting. |
| Visuals | Potion shelves, glass bottles, glowing liquid, labeled ingredient jars, magical measuring tools, herb bundles, cauldron/workbench, parchment recipe cards. | Items must remain readable as collectible UI assets, not just background decoration. |
| UI overlay | Use parchment/gold panels over the apothecary scene for inventory lists, progress bars, ingredient cards, and potion status. | Avoid full-screen clutter; ingredient detail may be an expandable panel from the room-entry strip. |
| Inventory connection | Should connect to ingredient inventory and the Cabinet of Curiosities if power-up/inventory systems overlap. | Do not decide duplicate conversion or crafting costs visually until economy is locked. |

## 9. Library

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Library is the natural visual location for Grimoire, Book of Shadows, spellbook collections, card albums, lore, and possibly completed set review. | Must not make collection navigation feel slow or dusty; still a casual mobile collection UI. |
| Visuals | Floating books, open spellbook desks, glowing bookmarks, page tabs, candlelight, purple/gold shelves, magical dust, readable collection category portals. | Book visuals should frame the UI but not reduce card readability. |
| Collection access | Library should give clear entry points to Grimoire and Book of Shadows. | Do not mix the two so strongly that premium/special collection status becomes unclear. |

## 10. Relic Wall

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Relic Wall displays restored Realm relics, achievement badges, event trophies, and Coven contribution honors. | Relics can preserve the older artifact fantasy as display/story rewards even if potion ingredients are the current room-facing progression. |
| Visuals | Wall of plaques, magical shelves, glowing frames, realm medallions, trophies, sigils, badges, restored objects, and hover/tap details. | Avoid an austere museum wall; it should feel earned and magical. |
| Progression readability | Locked/earned states should be obvious; restored realms should feel visually more alive/glowing than unfinished realms. | Do not imply a realm is complete unless the completion rules are met. |

## 11. Grimoire UI

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Purpose | Main collection interface. It should feel organized, magical, collectible, rewarding, and readable. | Do not make it dusty, old-fashioned, or overly text-heavy. |
| Structure | Pages/sets with 10 cards per set/page; card slots, page progress, missing items, duplicate indicators, Prioritized Page marker, and completion reward/market value area. | Final collection size and duration must follow the locked collection doc/economy docs; do not invent extra pages. |
| Visual treatment | Open magical book, parchment pages, gold/purple tabs, readable card frames, glowing completion effects, gentle page transitions. | The book frame should not consume too much card area. |
| Completed set treatment | After completion, the set reward area may be replaced by market/set value if that economy idea is later locked. | Do not show both set reward and market value if the replacement model is adopted. |

## 12. Book of Shadows UI

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Purpose | Premium/special collection interface. Rarer, more mysterious, more ornate than Grimoire while still readable. | Darker does not mean sinister or unreadable. |
| Visual treatment | Deep purple/black cover, silver/gold magical lines, glowing seals, animated page-edge shimmer, elegant shadows, premium sparkles. | Avoid horror occult tone; keep within bright magical mobile game brand. |
| Layout | Should share enough collection logic with Grimoire for comprehension but have distinctive frame, page, and reveal treatment. | Do not assume duplicate rules or paid access rules visually until economy is locked. |
| Reward feel | Book of Shadows completion/reveal should feel more premium and rare than normal Grimoire rewards. | Premium presentation must not look like a predatory paywall. |

## 13. Collection spread layout

The collection spread/page should emphasize the cards, the set name, progress, rarity, and completion status. The user specifically liked reclaiming the top emblem area to give more room to the title image and count.

| Component | Direction | Rejected/deprioritized notes |
| --- | --- | --- |
| Top title/image zone | Use reclaimed vertical space for title image and card count, stacked cleanly if it improves readability. | The decorative emblem at the top of the page had no clear value and should not consume space. |
| Card grid/spread | Use clear card slots with rarity frames, missing silhouettes, duplicate badges, and star/rank information where applicable. | Do not overdecorate frames until card names/counts are hard to scan. |
| Progress count | Card count display should be prominent and vertically stackable with the title image. | Avoid tiny progress counters hidden in corners. |
| Side navigation | Purple side arrows were preferred over alternate arrow treatments. | Not a high-priority detail but should be retained when feasible. |
| Completion state | Show completion reward or market value area, not both if the market replacement model is used. | Avoid reward clutter after a set is complete. |
| Prioritized Page | Use a visible marker/toggle for prioritized collection targeting. | Do not make the marker look like a purchase-only boost unless it is monetized. |

## 14. Set tile layout

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Set tiles | Tiles should show set title/art, completion count, rarity/collection identity, reward preview or post-completion value, and clear locked/available/completed states. | Do not hide the card count; players need a fast read of progress. |
| Completed set | Completed sets may show market value instead of reward if that economy is locked. | Do not implement market value as spendable currency without final economy rules. |
| Layout density | Tiles should be scrollable/scannable and not too small for mobile. | Do not reuse desktop album layouts that rely on tiny text. |
| Visual hierarchy | Set title > card count/progress > reward/value > action/enter. | Avoid putting decorative emblems above useful information. |

## 15. Card count display

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Room-entry card count | 1 / 2 / 4 / 6 card options must appear as four columns across the lower screen. | Each option should communicate total cost or mana bet relationship. |
| Collection card count | Collection page/card count should be prominent, potentially stacked under/with the title image. | Do not bury count under decorative emblem space. |
| Pack/reward count | Card pack and reward reveals should show quantity, rarity/chance where allowed, and New-to-You distinction. | Avoid unreadable small badge stacks. |

## 16. Removed/reclaimed emblem space

The top emblem on collection pages was deprioritized because it did not add enough value.

That space should be reclaimed for more useful information: title image, set name, card count/progress, reward/value, or collection state.

On room-entry screens, decorative flowers/emblems should be replaced by the relevant potion/restoration item, because the potion communicates gameplay progress.

Do not add numbers on rooms, rename realms, or introduce new decorative systems unless they are part of the approved design.

## 17. Purple side arrows preference

Purple side arrows were specifically preferred in the collection/navigation context. This is a minor visual preference, not a core layout lock, but it should be respected unless readability, accessibility, or screen constraints require a different treatment.

Use purple arrows for page/set navigation where they fit the UI palette.

Keep arrows large enough for touch and readable against book/page backgrounds.

Do not let arrows cover card slots or reward information.

Maintain consistent left/right placement between Grimoire, Book of Shadows, and album-like screens.

## 18. Portal visuals

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| General portal role | Portals connect Realms/worlds and should communicate progression, magical travel, and unlock states. | Do not make portals look like casino wheels. |
| Realm map portals | Use glowing nodes, locked/unlocked portal paths, portal rings, floating realm islands, parchment/map hybrid elements, and distinct realm silhouettes. | Older realm-map backgrounds were considered too painterly/dark at times; keep readability and style consistency. |
| Open portal state | Everbloom/Sunpetal-style background systems include broken/restored/open-portal realm states. The open portal in the back of the realm is a final state visual. | Do not show open portals before completion. |
| Portal Surge | If retained, should be visually distinct from a basic prize wheel: layered portal passage, escalating magical chambers, light tunnels, runes, and reward pots. | Portal Surge, jackpot wheel, and daily spin should not be conflated visually without a final economy/system lock. |
| Jackpot wheel | Room-entry jackpot wheel goes top right and should show large pot value clearly. | Avoid direct casino imitation; magic wheelspin is acceptable but should feel whimsical and collectible. |

## 19. Marketplace / Bewitchment Bazaar

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Name/tone | Bewitchment Bazaar is the current preferred/locked marketplace naming direction. | Do not revert to Mayhem Market, Balthazar’s Bazaar, or Bewitched Bazaar without approval. |
| Role | Marketplace/social hub for ingredient gifting/requesting, card trading, duplicate trading, Coven-related exchange, and future social features. | Limits, cooldowns, and trade eligibility must be reflected visually if trading is implemented. |
| Visual tone | Magical market street or shop court: glowing stalls, potion crates, card cabinets, trade counters, purple/gold signage, floating request notes, coven emblems. | Should feel helpful and social, not like a pressure-sales shop. |
| UI needs | Tabs or zones for Requests, Offers, Team Needs, You Can Share, Missing Ingredients, Duplicate Cards, and possible event shop. | Do not overbuild global trading if Coven-only trading is the actual MVP. |
| Fairness signaling | Show daily limits, cooldowns, eligibility, rarity restrictions, and confirmation states clearly. | No hidden cost surprises. |

## 20. Oracle Alley

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Status | Oracle Alley has been discussed as a possible permanent-but-sometimes-open sub-area of the broader Bazaar. | Not fully locked as MVP; do not build as core economy without final approval. |
| Visual role | Mystical alley stall/side room for readings, chance reveals, wild-card chances, or magical insight rewards. | Should be visually connected to Bewitchment Bazaar but feel more mysterious and special. |
| Tone | Candlelit, purple/blue magical glow, hanging cards, crystal ball, moons/stars, velvet cloth, small charms, smoke, curtains, readable reward tray. | Mysterious but not sinister; no horror seance styling. |
| Mechanic visual | If readings are used, one reading should produce a single clear result/asset; multiple readings may increase chances only if system design confirms it. | Do not imply every reading guarantees a wild card unless locked. |

## 21. Madame Solange / tarot reading visuals

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Status | Madame Solange was not confirmed as locked in the available decision register, but the requested visual direction should support a fortune-teller/oracle host if later approved. | Do not treat Madame Solange as a final named character until locked. |
| Character/host direction | Elegant magical reader, warm but mysterious, not scary; ornate scarf/jewelry, glowing eyes or glasses, crystal-ball light, tarot cards, moon/star motifs. | Avoid stereotypes or overly dark occult design; keep casual-game friendliness. |
| Tarot reading layout | Use a spread of cards or one-card reveal over velvet/parchment with soft glow, crystal ball, and clear reward result. | Card spread should not conflict with ingredient-card tarot arc used in potion completion; keep each ritual visually distinct. |
| Reward reveal | Reading result should reveal a single asset, chance outcome, dust/token, wild-card shard, or no-prize result if the economy locks that model. | Never visually promise guaranteed high-value wilds without confirmed odds/rules. |

## 22. Crystal ball bonus reveal

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Visual idea | Crystal ball can be used as a bonus reveal device: fog clears, symbol appears, card/wild/dust/reward rises out, glow bursts, result lands in reward tray. | Must be quick and readable; avoid long opaque animations. |
| Use cases | Oracle Alley reading, bonus reveal, daily spin alternative, mystery reward, or special card reveal. | Do not use the same reveal for every reward type or it will lose impact. |
| Color/effects | Lavender glow, teal highlights, magenta sparkles, star flecks, prismatic shine for wild-card results. | Avoid realistic smoky crystal balls that feel dark or muted. |
| UI clarity | Result label and quantity must be shown after animation. | No mystery after the reveal; player should know exactly what was earned. |

## 23. Cabinet of Curiosities / power-up inventory

| Area | Direction / requirement | Constraints / caveats |
| --- | --- | --- |
| Role | Cabinet of Curiosities is where power-ups are stored. | Do not rename it or assume it stores all currencies unless approved. |
| Visuals | Magical cabinet with compartments, glowing bottles/orbs, labeled shelves, power-up icons, inventory counts, rarity frames. | Avoid generic backpack UI. |
| Generic power-up icon | Orb + sparkle direction was preferred over a wand/bolt-like icon to avoid confusion with individual wand/bolt power-ups. | The icon must remain readable at small size. |
| Inventory states | Show count, locked/unlocked, source hints, use restrictions, and active power-up status where relevant. | Do not imply power-ups are available in modes where they are restricted. |

## 24. Generated / uploaded reference concepts that should influence design

The Project contains visual/mockup/reference images that should inform design patterns. These are not all final project screens, but they reveal useful UI approaches and cautionary examples.

Reference/contact sheet of uploaded visual examples and mockups for UI patterns, reward reveal, card exchange, VIP track, store, wild cards, album, card inventory, and badge inventory.

| Reference concept | Useful influence | Do not copy / caution |
| --- | --- | --- |
| Room-entry mockup | Confirms the need for card count selection, jackpot visibility, currency bar, power-up strip, and readable play setup. | Do not add irrelevant called-ball data or unrelated menus. |
| Congratulations/reward reveal | Shows value of a centered celebratory reward with clear item row and collection/reward confirmation. | Do not let reward rows become too small or visually anonymous. |
| King card exchange / card exchange reference | Useful for trading/exchange communication and large card visuals. | Avoid copying casino/social bingo UI literally; adapt to magical storybook system. |
| VIP / paid track reference | Useful for seeing free/paid ladder, milestones, and track comparison. | Paid upgrade track is not final unless locked; do not overemphasize monetization. |
| Diamond store / shop reference | Useful for shop tabs, timers, bundles, and currency packs. | Bingo Magic shop should feel magical and fair, not generic store grid. |
| Wild card reference | Useful for wildcard card visual hierarchy and use buttons. | Final wild-card types/eligibility are economy decisions, not visual assumptions. |
| Album/card collection reference | Useful for page navigation dots, side arrows, and card slots. | Collection should use the Grimoire/Book of Shadows language, not a generic album skin. |
| Card pack manager reference | Useful for showing collected cards, duplicates, side categories, and card inventory browsing. | Do not overcrowd; keep title/count/progress hierarchy strong. |
| Badges/inventory reference | Useful for Relic Wall, badge inventory, trophy tabs, and player achievement display. | Avoid dark, plain utility UI; translate into magical cabinet/wall treatment. |

## 25. Visual ideas rejected or deprioritized

| Rejected / deprioritized idea | Reason / replacement direction |
| --- | --- |
| Overly dark gothic fantasy | Does not match bright magical adventure target. |
| Solemn wizard fantasy / generic medieval kingdom | Too generic and not aligned with casual collectible bingo. |
| Traditional casino clone / direct casino wheel imitation | Game should feel magical, not casino-first. Jackpot wheel must remain whimsical. |
| Overly realistic environments / photoreal materials | Current style is stylized magical storybook with polished casual rendering. |
| Older painterly realm maps as final UI target | Later style lock moved toward less painterly, clearer, character-friendly rendering. |
| Muddy realism and gray stone texture | Makes scenes feel dark and lowers readability. |
| Tiny overworked background detail | Competes with mobile UI. |
| Background-first layout changes | Do not move locked UI regions to fit scenic art. |
| Wrong room/realm names or added room numbers | Breaks continuity and wastes production/generation time. |
| Potions missing from room layout | Potion/restoration item communicates progress and should remain in room title area. |
| Decorative flower/emblem beside room title | Replace with actual potion/restoration item. |
| Top collection emblem consuming space | Reclaim for title image and card count. |
| Hamburger menu on room-entry screen | No clear reason for it in that context. |
| Called balls on room-entry setup | Called balls belong to active gameplay, not card/bet selection. |
| Garden Breeze power-up | Not a real locked power-up. |
| Ingredient card checkmarks in completion overlay | Use actual card identity with title/stars instead. |
| Restore reward chest | Restore reward should be mana/card packs. |
| Hyper-detailed ingredient icons | Need small-size readability; use brighter simpler collectible icon style. |
| Jewelry-like icons when item is not jewelry | Icons must communicate the actual ingredient/item. |
| Oracle Alley / Madame Solange as final MVP assumption | Visual support is fine, but feature/name is not fully locked. |

## 26. Production rules for designers and image generation

Start from the approved Blossomveil Promenade UI hierarchy before generating any room-entry derivative.

Keep forced landscape composition in mind for room-entry, bingo gameplay, realm background, and reward states.

Never change room names, realm names, potion placement, card-count options, or major UI regions in generated concepts unless the prompt specifically requests a redesign.

Create backgrounds as support layers for UI, not standalone scenic paintings.

Use bright magical storybook palette and clean silhouettes; avoid gloomy high-detail renders.

When designing small icons, test at small size; if the icon cannot be identified quickly, simplify it.

When designing collection screens, prioritize title image, card count/progress, card slots, reward/value area, and navigation arrows before decorative emblems.

Separate visual treatments for Grimoire, Book of Shadows, Bazaar, Oracle Alley, Player Den, Apothecary, Library, and Relic Wall while maintaining shared UI language.

Do not introduce monetization visuals, guaranteed wild-card claims, or paid-track hierarchy until economy decisions are locked.

## 27. Open visual decisions

| Open item | Decision needed |
| --- | --- |
| Final logo | Need final chosen logo direction and app icon treatment. |
| Final project title lock | Bingo Magic Blast vs Bingo Magic Mayhem must be aligned in art files. |
| Player Den layout | Need lock on single hub room vs separate Apothecary/Library/Relic Wall screens. |
| Oracle Alley status | Need feature/name lock before producing final UI. |
| Madame Solange status | Need character/name lock before character art. |
| Crystal ball reveal | Need system placement: Oracle, daily bonus, mystery reward, or general reveal. |
| Marketplace scope | Need Coven-only vs global trading before final Bazaar UI. |
| Collection spread dimensions | Need final card count/page layout constraints after final collection size is locked. |
| Book of Shadows access model | Need free/premium/paid-track relationship before final UI hierarchy. |
| Portal Surge vs jackpot wheel | Need final distinction before animating portal/jackpot systems. |
| Daily/weekly paid track visuals | Need monetization scope lock before VIP/track-like visual treatment. |
| Power-up icon set | Need final named power-up list and generic category icon approval. |
| Rarity frame system | Need final rarity taxonomy including Rare, Extra Rare, Ancient, Gilded, Astral, Wild. |

## Appendix A. Naming and visual glossary

| Term | Visual meaning / direction |
| --- | --- |
| Blossomveil Promenade | Approved room-entry style calibration screen. |
| Everbloom Sanctuary | Locked/current realm reference with map, room-entry screens, potion/ingredient sheet, and background-state handoff. |
| Sunpetal Conservatory | Locked/current realm reference with map, room-entry screens, potion/ingredient sheet, and not-fully-restored guidance. |
| Grimoire | Main collection book UI. Bright magical, organized, readable. |
| Book of Shadows | Special/premium collection book UI. Darker, rarer, ornate, still readable. |
| Cabinet of Curiosities | Power-up inventory/storage visual home. |
| Player Den | Personal hub space. |
| Apothecary | Potion/ingredient/progression inventory zone. |
| Library | Spellbook/collection zone. |
| Relic Wall | Restored realm relics, achievements, trophies, Coven contribution honors. |
| Bewitchment Bazaar | Marketplace/social trading hub direction. |
| Oracle Alley | Potential timed/special reading sub-area of Bazaar. Not fully locked. |
| Madame Solange | Potential oracle/tarot host name. Not confirmed as locked. |
| Portal Surge | Potential rare jackpot portal sequence distinct from normal jackpot wheel if retained. |
| Jackpot wheel | Visible room-entry jackpot/wheelspin feature. |
| Purple side arrows | Preferred page/set navigation arrow treatment. |

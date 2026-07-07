# 01 — Locked Game Vision

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

## Current product identity

**Confidence: LOCKED**

- **Bingo Magic Blast** is currently documented as a mobile magical bingo adventure.
- It is designed around bingo play, magical realm/world progression, collection systems, rewards, and social/Coven systems.
- The experience should feel bright, whimsical, collectible, premium, magical, and rewarding.
- It should avoid dark gothic fantasy, muddy realism, casino imitation, overly realistic environments, or noisy clutter.
- Major game systems should be data-driven/configurable where possible: realms, rewards, collections, events, shop offers, board configurations, drop rules, progression, and balancing.

## Locked-decision register extract

## 1. Game identity

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Mobile magical bingo adventure for iOS and Android. | Product identity | The original developer handoff defines the product as a mobile social bingo adventure, and later work continues to build room-entry, realm, collection, and social systems around that premise. | Must support bingo gameplay, magical realm progression, collection systems, rewards, and social/Coven systems. | The final public title still has a caveat: older resources say Bingo Magic Blast, while later locked visual material uses Bingo Magic Mayhem. |
| The experience should feel bright, whimsical, collectible, premium, magical, and rewarding rather than dark fantasy or casino-like. | Tone | The later locked visual style target explicitly reinforces whimsical, bright, collectible, premium, stylized magical storybook direction. | Avoid dark gothic fantasy, muddy realism, casino imitation, overly realistic environments, or noisy clutter. | Earlier graphic references can still inform UI, but later Blossomveil Promenade style lock overrides earlier broader art direction. |
| Use data-driven configuration for major game systems. | Technical/product rule | Developer handoff states realms, rewards, collections, events, shop offers, and board configurations should be adjustable without full app updates; later locked handoffs imply the same live-tunable structure. | Remote config/live-ops support should be assumed for rewards, drop rules, progression, shop/event tuning, and balancing. | Specific backend vendor is not locked. |

## 2. Core gameplay loop

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Player enters a realm/room, chooses card count and mana bet, plays bingo, earns rewards, and advances collections/realm progress. | Primary loop | Later approved room-entry layout locks card selection, mana bet, active power-ups, ingredient progress, restore rewards, and jackpot wheel in the same setup flow. | Room-entry screen must make play setup clear; progress should be visible but not dominate the core action. | Earlier terms like buy-in, coins, and energy exist, but later UI specifically uses mana bet per card. |
| Card-count options on the locked room-entry screen are 1, 2, 4, and 6. | Match setup | The later locked visual target explicitly lists 1 / 2 / 4 / 6 card options as four columns across the lower screen. | Keep the four-column lower-screen layout unless intentionally redesigning UX. | This locks the visible room-entry options; it does not fully define advanced unlocks or maximum cards outside this screen. |
| Active power-ups belong in the bottom-right area of the room-entry structure. | Power-up UX | The later locked layout rules explicitly preserve Active power-ups at bottom right. | Power-up UI should be readable, touch-friendly, and visually distinct from card-count and mana controls. | Specific individual power-up list is not locked in the available later material. |
| Called balls do not belong on the room-entry/setup screen. | UX exclusion | Later discussion corrected that called balls are irrelevant to the screen where players choose bet/card count. | Called-number information belongs to active bingo gameplay, not room entry. | The active bingo play screen still needs called-number display rules. |

## 16. Visual direction

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Approved Blossomveil Promenade screen is the current visual/style anchor. | Visual lock | Later style document explicitly says Blossomveil Promenade becomes the current visual target and current visual lock. | Do not move major UI regions unless intentionally redesigning UX. | Other screens can adapt, but should not contradict the locked room-entry language without reason. |
| Core art direction is stylized magical storybook UI with polished casual-game rendering. | Art direction | Later locked visual target states this directly. | Whimsical, bright, collectible, premium; not overly realistic or painterly. | Older painterly realm-map experiments should not drive future production. |
| Locked UI style uses cream parchment panels, gold beveled borders, deep purple banners, green action buttons, bright icons, fantasy serif titles, and clean hierarchy. | UI style | Later locked visual target defines these UI style rules. | Ornate but not cluttered; decorative but gameplay-readable. | Screen-specific variations are allowed if they preserve readability. |
| Primary brand colors are deep purple, warm gold, magenta/pink magic, lavender glow, teal accents, cream parchment, and green buttons. | Color system | Later locked visual target lists these as primary brand colors. | Realm-specific palettes can vary within the brighter character-friendly palette. | Logo color system still needs final lock. |
| Potion/ingredient icons should be glossy, magical, collectible, readable small, and centered in round frames when used in progress rows. | Icon direction | Later locked visual target defines potion and ingredient icon rules. | Avoid room-scene icons, environmental structures, overly realistic paintings, jewelry-like icons unless actually jewelry, and excessive detail. | Some earlier generated icons may need restyle to meet this. |
| Do not generate rogue layout changes or change room/realm names/structure without instruction. | Production constraint | Later discussions repeatedly corrected unwanted layout/name/room changes and locked the approved style/layout anchor. | Preserve layout, names, potion placement, and established room states. | Intentional redesigns are allowed only when explicitly requested. |

## 18. Monetization rules

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Monetization should not feel pushy or casino-like. | Monetization tone | Graphic direction explicitly avoids casino imitation and says Coven Emporium should be helpful, not aggressive; later visual direction also avoids casino/realism clutter. | Reward/shop UI should feel earned, magical, fair, and transparent. | Specific shop/IAP offers are not locked. |
| Pricing should be fair and consistent, with progression scaling allowed but unpredictable player-by-player pricing avoided. | Economy fairness | Developer handoff states this as an economy design principle; later locked Economy + Reward Logic handoff exists. | Avoid opaque personalized pricing or manipulative dynamic pricing. | Exact price points are open. |
| Basic shop/IAP framework is expected, but specific monetization catalog is not locked. | IAP scope | Original architecture includes IAP framework, but later locked discussions focus more on reward systems, mana/card packs, Covens, and Bazaar than SKU design. | Must fit the magical/non-pushy tone and fairness principle. | Need explicit monetization pass before production handoff. |
| Coven Emporium, where used, should be an earned-event shop rather than a pushy cash shop. | Event shop | Graphic direction says Coven Emporium should feel like a place to spend earned Coven Orbs and avoid pushy monetization visuals. | Limited weekly stock, clear item limits, orb currency display, helpful shop tone. | Coven Ritual/Coven Emporium may be Phase 2. |

## Codex source-of-truth permissions

Codex may safely use the following as core premise:

- Mobile magical bingo adventure.
- Magical realm progression.
- Rooms as playable places.
- Collection-driven progression.
- Social/Coven support.
- Bright storybook fantasy UI.
- Configurable/live-tunable systems.

Codex must not assume:

- Final public title is resolved between **Bingo Magic Blast** and **Bingo Magic Mayhem**.
- All older currencies remain active.
- All old realm names remain canonical.
- Monetization can bypass collection scarcity or social fairness.

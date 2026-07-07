# 03 — Collections: Grimoire and Book of Shadows

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

| System | Confidence | Codex handling |
|---|---|---|
| Grimoire as main collection system | LOCKED SYSTEM AREA | Safe to model as primary collection. |
| Book of Shadows as separate special/premium collection | HIGH / LOCKED SYSTEM AREA | Safe to model as separate collection type, but exact behavior open. |
| 10 cards per set/page | HIGH | Appears repeatedly; still reconcile with total count contradictions. |
| 80-card final direction | HIGH / CONFLICTED | Do not assume where it applies without product confirmation. |
| 32-set rarity ladder | PROPOSED / CONFLICTED | Archive/configurable only; conflicts with 80-card model. |
| Ancient/Gilded per-set counts | OPEN | Do not hardcode. |
| Duplicate conversion | HIGH as required system, OPEN values | Model conversion hooks, not final rates. |
| Wildcards / wildcard shards | HIGH | Model as collection helper; exact thresholds open. |

## Extracted collection design details

## 5. Collections

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| The Grimoire is the main collection system. | Collection | Later resources include a locked Grimoire Main Collection System Handoff and locked Card Reward / Grimoire Flow Handoff. | Must support sets/pages, card slots, page progress, missing cards, duplicate indicators, prioritized page cues, and rewards. | Final total count/duration has caveats; do not use older 25-set/90-day numbers without re-confirmation. |
| Each collection set/page uses 10 cards. | Collection structure | Later discussion repeatedly corrected that each set has 10 cards and used that as the basis for Grimoire/Book of Shadows review. | Card layouts, rewards, and rarity distribution should assume 10-card sets unless a later layout restriction changes this. | Number of total sets remains conflicted across discussions. |
| The current final collection-size preference is 80 cards unless a layout restriction is uncovered. | Collection size | Later discussion explicitly states “80 is final unless there is some layout restriction we uncover.” | Assume 80 total cards for the relevant collection planning pass. | Need confirm whether the 80-card final applies to Grimoire only, Book of Shadows, or both. |
| Book of Shadows exists as a separate special/premium collection track. | Collection | Later discussions moved into Book of Shadows 1, 2, and 3 and locked/renamed specific potion concepts; older handoff also identifies it as premium/special. | Should feel rarer, darker/more mysterious, ornate, and still readable. | Final count/volume structure needs explicit lock. |
| Potion/ingredient card collections are tied to rooms and realm restoration. | Collection/progression | Locked realm handoffs include potion/ingredient sheets; later UI locks an ingredient progress strip and potion/restoration item image in room-entry. | Potion and ingredient assets must remain readable at small sizes and connected to room progress. | Exact drop rules live in the locked Ingredient / Potion Drop Rules handoff; full operational details need source file review. |

## 6. Card rarity

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Rarity remains a collection/card-system concern and must have visual frame treatment. | Card rarity | Later locked Grimoire/Card Reward handoffs exist, and visual direction continues to require card frames, rarity frames, missing-card placeholders, duplicate badges, and New-to-You badges. | Rarity treatments must remain readable and collectible-looking. | The final rarity taxonomy is not fully locked in available snippets. |
| Ancient and Gilded cards are recognized rarity/special-card concepts requiring final per-set rules. | Card rarity | Later user specifically asked to retrieve decisions about Ancient and Gilded cards per set, indicating they are active concepts, not discarded early ideas. | Do not invent counts until the prior discussion is fully retrieved. | Ancient/Gilded quantity per set remains open in this register. |
| Early sets should be lighter on rare cards and later sets should escalate rarity/rewards. | Rarity progression | Later discussion accepted the general shape of a rarity ramp where early sets have no rares and later sets gain rare/extra rare cards. | Rarity should ramp gradually to support progression and reward value. | The specific 32-set ladder conflicts with the 80-card/10-card-set model and is not locked here. |

## 7. Duplicates and conversion economy

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Duplicate tracking is required. | Collection economy | Collection Assist, Grimoire UI, and rewards summary direction all require duplicate indicators, duplicate tracking, and duplicate conversion feedback. | Inventory must know owned/missing/duplicate status and sharing eligibility. | Exact conversion value/formula is still open. |
| Duplicate conversion feedback belongs in reward/reveal flows. | UX/economy | Graphic handoff lists duplicate conversion as a rewards summary need and animation priority; later locked reward/economy handoff exists. | Players should clearly understand when duplicates convert and what they become. | The target converted resource is not fully locked across later materials. |
| Duplicate/card economy connects to trading and market value, but final conversion rules are not locked. | Economy bridge | Later discussion added market value after set completion and locked Coven + Bazaar / Trading handoff scope includes card trading/duplicate trading. | Do not design irreversible duplicate sinks until trading/market-value rules are confirmed. | Static vs variable market value is still open. |

## 9. Wild cards

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Wild cards/wildcard support is part of collection completion support. | Collection helper | Earlier handoff includes Wildcards/Wildcard Shards as collection completion support, and later discussion around Oracle Alley/readings continues to reference wild outcomes. | Must not trivialize rare/special-card scarcity. | Exact distinction between wildcard, wild card, wildcard shard, and Wild Ritual Call must be clarified. |
| Wild Ritual Call is separate from collection wildcards and belongs to Coven Ritual mechanics. | Event helper | Coven Ritual rules define Wild Ritual Calls as clearing any open Ritual Mark. | Terminology should prevent players confusing Wild Ritual Calls with card wildcards. | Coven Ritual final MVP/Phase 2 status remains open. |
| Oracle-style readings may involve wild outcomes, but are not locked as a final wild-card source. | Potential source | Later discussion liked Oracle Alley but did not fully lock its mechanics; Madame Solange is not found in available locked sources. | Do not include Oracle wild generation in economy balance until confirmed. | Covered again under Oracle Alley / Madame Solange. |



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

## Codex implementation guardrails

- Model collections as data-driven: collection id, collection type, page/set id, card id, rarity, duplicate status, trade eligibility, wildcard eligibility, reward state, completion state.
- Keep Grimoire and Book of Shadows behavior separate in configuration.
- Do not hardcode final card counts until the 80-card/32-set contradiction is resolved.
- Do not hardcode Ancient/Gilded counts.
- Do not assume Book of Shadows duplicates convert the same way as regular Grimoire duplicates.
- Do not assume all rarity tiers are tradeable.
- Do not assume market value is redeemable currency; it is currently an idea/open system.

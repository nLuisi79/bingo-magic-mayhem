# UI Track State and Boundary

Last updated: 2026-07-13

This note summarizes the current frontend-only extraction state for the Bingo Magic Mayhem UI track.

The intent of this track remains:

- presentation-side only
- no gameplay authority rewrite
- no backend or service-layer ownership
- no reward, economy, progression, jackpot, save, auth, analytics, or multiplayer authority changes

## Working rule

The UI layer may:

- render text, labels, counts, and summary blocks
- own panel shell structure
- own button labels and interactable state presentation
- expose `...Requested` events upward
- centralize modal chrome and repeated panel patterns
- prepare screens for prefab, animator, and polish passes

The UI layer should not:

- decide outcomes
- resolve rewards
- mutate progression rules
- own social/help eligibility rules
- own wild-card consumption rules
- replace prototype/state authority during this phase

## Current extraction pattern

The working extraction pattern in this branch is:

1. `...DisplayModel`
2. `...View`
3. Prototype builder creates primitives
4. View applies text/state and raises intent events
5. Prototype remains the authority for meaning and follow-through

This has been working well because it reduces overlap risk while still pulling presentation responsibilities out of `BingoPrototype.cs`.

## Areas already meaningfully extracted

### Den / profile shell

- Player Den top band shell
- Player Den cauldron/title shell
- Mana cauldron modal shell
- Profile settings/login/header helpers

### Collections shell

- Library landing shell
- Library gifting shell
- Grimoire index shell
- Grimoire detail shell
- Book of Shadows library shell
- Book of Shadows detail shell
- Ingredient overlay shell
- Ingredient detail modal shell
- Ingredient help modal shell
- Wild confirmation modal shell

### Rewards / jackpot shell

- Reward preview shell
- Card reveal shell
- Daily Bonus modal shell
- Reward preview sections
- Jackpot spin screen shell
- Jackpot pot/result/confirmation/stack shells
- Round reward summary tiles and sections

### Friends / social shell

- Friends modal summary/footer shell
- Friends list/request row shells

## What is still intentionally prototype-owned

The following kinds of behavior are still intentionally owned by `Assets/Scripts/BingoPrototype.cs`:

- page and selection state
- collection reward eligibility and claims
- help request eligibility, limits, and send resolution
- wild-card use resolution
- inventory mutation and analytics calls
- jackpot outcome and collect authority
- gameplay flow and round authority
- den/system navigation consequences

This is expected for the current branch phase.

## Safe remaining structural targets

These are still good frontend-only candidates before styling becomes the main work:

1. Den shell cleanup beyond the top band
   - reusable door/tile shell helpers
   - title/header shell normalization
   - bottom action strip consistency

2. Collection modal/chrome consolidation
   - shared collection modal shell helper
   - shared confirm/detail/info block structure
   - shared CTA button patterns

3. Reward/jackpot cleanup
   - review remaining reward/jackpot builders for shell-only extraction
   - standardize action area and footer interaction patterns

4. Friends / inbox summary-shell cleanup
   - keep list/message authority in prototype
   - continue extracting top summary, footer note, and modal chrome helpers

5. Navigation and modal orchestration consistency
   - normalize close/back/request event naming
   - reduce one-off modal callback wiring where a reusable pattern exists

## Lower-ROI structural work

Possible, but lower value until styling begins:

- broad renames with no ownership gain
- large-scale folder reshuffles with no behavioral payoff
- generic abstraction that does not remove real prototype presentation load

## Work that should wait for styling / graphics

These are real UI tasks, but they are no longer primarily structural:

- layout refinement
- spacing and typography tuning
- sprite/art hookup
- motion/transition polish
- button hover/press/disabled visual state polish
- modal hierarchy polish for readability
- reward reveal emphasis and micro-interaction feel

## Recommended next order

1. finish remaining safe structural extractions
2. consolidate repeated collection and modal shell patterns
3. freeze the presentation boundary for this phase
4. hand off to a styling/polish pass

## Guardrails for future UI-track work

- Prefer extracting shell responsibilities over rewriting flow.
- Keep prototype callbacks authoritative for now.
- If a UI improvement requires changing resolution order, eligibility, or outcome logic, mark it as cross-branch risk and defer it.
- If a screen is mostly waiting on art, styling, or spacing decisions, avoid over-engineering it structurally.

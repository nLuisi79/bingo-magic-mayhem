# Beta 1 Production Readiness

Last updated: 2026-07-08

Status: working planning checklist. This is not a gameplay/economy lock document. Use it to decide what must become real enough for Beta 1 and what can remain a labeled placeholder.

Read with:

- `LOCKED_DECISIONS.md`
- `09_OPEN_DECISIONS.md`
- `10_VERIFIED_PROJECT_STATE.md`
- `11_RANK_REWARDS_V0.1.md`
- `IMPLEMENTATION_GAPS.md`

## Goal

Move from feature-shape prototype toward a production-minded Beta 1 without stopping useful feature coverage.

The target rhythm is two-track:

1. Finish visible Beta 1 feature coverage at a narrow, testable level.
2. Start replacing fragile prototype shortcuts with production-ready architecture, config, save, account, analytics, QA, and backend boundaries.

## Beta 1 Intent

Beta 1 should let testers understand the core game promise:

- Play magical bingo rooms.
- Progress through the first realm.
- Restore rooms/realm areas.
- Collect ingredients and album cards.
- Use the Player's Den as the account/inventory hub.
- Interact with daily retention systems.
- See social systems and collection assistance boundaries, even if some are still limited.

Beta 1 does not need every long-term system fully monetized, tuned, socially networked, or backend-complete, but it must not confuse testers about which systems are real, placeholder, or deferred.

## Must-Have Beta 1 Feature Coverage

These should be present enough for testers to use and understand:

| Area | Beta 1 target | Current state | Readiness |
|---|---|---|---|
| Core bingo rooms | Playable, stable round loop with card count and mana entry | Prototype playable | Needs hardening |
| Realm 1 progression | Room map, room restoration, first realm sense of progress | Prototype shell/playable | Needs QA and save review |
| Room rewards | Rewards visible and understandable | Prototype | Needs tuning/config separation |
| Jackpot wheelspin | Earned from 5/5 card state, separate from Daily Spin | Prototype | Needs odds/config validation |
| Ingredient collection | Ingredients awarded from room play and visible in potion flow | Prototype | Needs UX/QA pass |
| Potion/restoration flow | Apothecary/potion progress understandable | Prototype shell | Needs final Beta scope |
| Grimoire | Card collection browsing, card states, NEW flags | Prototype | Needs cleanup and asset plan |
| Book of Shadows | Premium/special collection presence | Prototype | Needs product scope decision |
| Player's Den | Hub for profile, inventory routes, daily systems, social routes | Prototype shell | Needs navigation polish |
| Cabinet of Curiosities | Power-ups, sigils, Clairvoyance, Pandora inventory home | Prototype shell | Needs inventory/source clarity |
| Daily Bonus | Direct claim daily loop, separate from Inbox | Prototype | Needs tuning/config |
| Daily Spin | Separate daily wheel, direct claim | Prototype | Needs visual polish/config |
| Inbox | Claim layer for gifts/system items and message home | Prototype | Needs category/clear/expiry rules |
| Freebies | Social links/deep-link redemption concept | Prototype stub | Needs link/deep-link implementation |
| Friends | Add/manage shell, mana taps, block/report/message direction | Prototype shell | Needs composer and Aura-rank limits |
| Coven | Coven shell, member popup, wish list/gifts | Prototype | Needs join/manage scope |
| Library card gifting | Regular duplicate card gifting in Library only | Prototype with rank cap | Needs anti-abuse and reset/QA tools |
| Enchanted Trail | Free path shell and collect states | Prototype shell | Needs point source/reward scope |
| Oracle Alley / Madame Solange | Limited Oracle reading-table placeholder | Prototype shell | Rewards/costs remain open |
| Reset/dev tools | Testers can recover/reset daily systems and prototype state | Partial | Needs Beta QA panel |

## Nice-To-Have Beta 1 Feature Coverage

These add value if time permits, but should not block first beta unless they are central to a test objective:

- Friend message composer.
- Friend mana send/receive limits by Aura-derived rank.
- Coven join/manage placeholder.
- Cleaner Library card gift/trade staging.
- Basic onboarding/tutorial prompts.
- Relic Wall badge display polish.
- Player avatar/frame/dauber placeholder polish.
- Enchanted Trail free-path task logic.
- Oracle Alley visual polish with rewards still disabled or clearly placeholder.

## Defer Past Beta 1 Unless Explicitly Approved

- Real-money purchases and live store.
- Final monetization tuning.
- Final jackpot odds/economy balance.
- Full live messaging/chat moderation.
- Full card trading marketplace.
- Automated social sharing.
- Competitive Coven scoring and live events.
- Full Oracle Alley reward economy.
- Final duplicate conversion/Stardust/Joker Wild economy.
- Premium Enchanted Trail lane.
- Final rank-up chest contents.
- Full production analytics dashboard.

## Production Architecture Checklist

### Save And Account

- Inventory, currencies, album state, ingredient state, daily systems, room progress, Inbox, and social state currently rely heavily on `PlayerPrefs`.
- Beta 1 needs a decision: keep local-only testing, or introduce account/cloud persistence before external testers.
- Production must not rely on client-only local state for economy-critical rewards.

Candidate production paths:

- Unity Authentication plus Cloud Save/Economy/Remote Config.
- Firebase/Auth/Firestore/Cloud Functions.
- PlayFab.
- Custom backend.

Decision needed before production hardening:

- Which backend owns account identity?
- Which backend owns inventory and reward grants?
- Which values are remote-configurable?
- Which actions must be server-authoritative?

### Config And Tuning

Move open/tunable values out of prototype code and into data/config:

- Room reward values.
- Jackpot odds and wheel segments.
- Daily Bonus rewards.
- Daily Spin rewards.
- Aura/rank benefit table.
- Friend/Coven/card gift limits.
- Freebie expiration and reward payloads.
- Album rewards.
- Potion/restoration rewards.
- Enchanted Trail reward path.

Rules:

- Do not hard-code unresolved economy values as final.
- Keep draft values labeled as Beta/test config.
- Make config readable enough for product review.

### Code Structure

Current prototype concentrates a lot of UI and state in `BingoPrototype.cs`.

Production direction:

- Split screen UI controllers by system.
- Split game services by responsibility.
- Keep catalogs/data separate from runtime state.
- Keep reward grants explicit about source/context.
- Keep Inbox claim semantics explicit.
- Avoid hidden rank/economy modifiers inside generic reward helpers.

Initial service boundaries:

- Account/Profile
- Inventory
- Rewards
- Rooms/Realms
- Albums/Collections
- Potions/Ingredients
- Daily Systems
- Inbox
- Friends/Social
- Coven
- Marketplace/Bazaar
- Analytics
- Remote Config

### Backend-Sensitive Systems

These should be treated carefully before external testing:

- Inventory grants.
- Inbox claim/clear behavior.
- Card gifting/trading.
- Friend mana sends/receives.
- Coven ingredient gifts.
- Freebie redemption and expiration.
- Daily Bonus and Daily Spin claim state.
- Aura-derived rank benefits.
- Purchases/receipts.
- Report/block/moderation actions.

### Analytics For Beta

Minimum Beta event plan:

- App/session start.
- Room entered.
- Round completed.
- Bingo claimed.
- Jackpot spin earned.
- Reward claimed.
- Daily Bonus claimed.
- Daily Spin claimed.
- Inbox item claimed/read/cleared.
- Card acquired.
- Ingredient acquired.
- Potion/restoration progress updated.
- Freebie redeemed.
- Friend action used.
- Coven action used.
- Error/reset/dev action used.

Every event should include safe context:

- Player Level, Aura Strength, and Rank.
- Realm/room id when relevant.
- Reward source.
- Claim source.
- Prototype/Beta build marker.

### QA And Test Readiness

Before Beta 1, create QA passes for:

- New install first session.
- Returning player session.
- Daily reset behavior.
- Offline/clock-change behavior.
- Room completion and replay.
- Inbox gift claim.
- Album new/duplicate/missing card states.
- Daily Bonus and Daily Spin separation.
- Freebie redemption once/expiry behavior.
- Friend block/report/message controls.
- Aura/rank cap visibility.
- Reset/dev tools.

## Known Beta Risks

- Many systems still use local `PlayerPrefs`; testers can produce inconsistent state.
- Some feature shells look interactive before full production rules exist.
- Rank benefits are a working draft, not locked economy data.
- Existing prototype code still derives Rank from Level. This must be corrected to the Aura model before production hardening.
- Purchase contribution to Aura must be small, capped, and unable to independently advance Rank.
- Card gift/trade limits are not final, especially send-vs-receive treatment.
- Friend mana limits are requested but not tuned.
- Oracle Alley/Madame Solange remains visually desirable but reward/cost/odds are unresolved.
- Inbox has evolving category and read/claim/clear semantics.
- Freebie redemption needs real deep-link and anti-repeat implementation.
- The giant prototype script can slow safe production changes if not split soon.

## Recommended Work Order

1. Finish Friends messaging composer as a visible feature gap.
2. Add/clarify Friend mana capacity display without locking final values.
3. Add Beta reset/dev controls for Inbox card gifts, daily systems, and social counters.
4. Create a Beta 1 feature matrix and mark each system Must, Nice, Deferred.
5. Decide local-only Beta vs account/cloud-backed Beta.
6. Extract Aura/rank/social/reward tunables into a config surface.
7. Split the highest-risk prototype services out of `BingoPrototype.cs`.
8. Add analytics event stubs.
9. Build QA checklist and run it every narrow pass.
10. Only then broaden visual polish and beta packaging.

## Definition Of Beta-Ready

Beta 1 is ready when:

- The feature map is broad enough for testers to understand the game.
- Must-have systems do not dead-end or contradict locked rules.
- Prototype-only features are clearly labeled or safely disabled.
- Reward claims do not silently auto-grant before intended claim points.
- Core progress survives app restart in the chosen Beta persistence model.
- Reset/dev tools can recover bad test state.
- A QA checklist exists and can be repeated.
- Economy/Aura/rank/social values used in Beta are documented as draft or locked.
- Known deferred systems are not presented as complete.

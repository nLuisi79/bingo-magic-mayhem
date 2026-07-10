# Beta QA Checklist

Last updated: 2026-07-10

Status: working QA script for Beta readiness. This is not a gameplay, economy, reward, progression, monetization, or Aura/rank lock document.

Read with:

- `LOCKED_DECISIONS.md`
- `09_OPEN_DECISIONS.md`
- `10_VERIFIED_PROJECT_STATE.md`
- `11_RANK_REWARDS_V0.1.md`
- `12_BETA_1_PRODUCTION_READINESS.md`
- `14_UGS_PRODUCTION_OWNERSHIP_MAP.md`

## Purpose

Use this checklist after each narrow implementation pass to catch routing, claim, persistence, and placeholder-state regressions before they pile up.

## Global Rules To Verify

- Daily Bonus, Daily Spin, Freebies, Realm income, Enchanted Trail, Inbox, and jackpot wheelspin remain separate loops.
- Inbox gifts/cards/messages do not auto-apply before claim/read/clear actions.
- Placeholder systems are visibly labeled as prototype, TBD, disabled, or not active.
- No rank/Aura benefit is applied to gameplay odds, jackpot odds/values, rare-card odds, ingredient drops, trade value, competitive scoring, or room/realm requirements.
- Navigation from Den features returns to Den unless a feature intentionally routes to World Map, Realm Map, Room Home, or gameplay.
- Fresh New Player reset returns the player to only Realm 1 Room 1 unlocked.

## Smoke Test

- Open the project and enter Play Mode.
- Confirm startup reaches the World Map without errors.
- Open Player's Den.
- Open and close each Den route:
  - Profile
  - Library/Grimoire
  - Bewitchment Bazaar
  - Apothecary
  - Relic Wall
  - Cabinet of Curiosities
  - Enchanted Trail
  - Coven Circle
  - Freebies
  - Friends
  - Leaders
  - Mayhem Market
  - Daily Bonus
  - Daily Spin
  - Inbox
- Confirm no modal remains layered behind the current modal after using Back to Den.

## New Player And Reset

- Use Fresh New Player.
- Confirm currencies, inventory, room progress, album state, social state, Trail claim state, and daily/social counters reset to intended prototype defaults.
- Confirm only Realm 1 Room 1 is unlocked.
- Confirm Room 1 can be entered and replayed.
- Confirm reset from Den returns to Den when opened from Den settings.

## Room And Gameplay

- Enter Realm 1 Room 1 from World Map and Realm Map.
- Confirm Room Home shows card count and mana bet controls.
- Confirm Room Home back returns to Realm Map.
- Start a standard round.
- Confirm daubs, bingos, reward preview, and round completion still function.
- Confirm final called ball countdown remains visible.
- Confirm gameplay does not expose Den and Map buttons together in contradiction of the locked navigation model.

## Room Restoration And Realm Progress

- Complete enough progress to restore a room using test controls if needed.
- Confirm restoring Room 1 unlocks Room 2.
- Confirm restored rooms remain replayable.
- Confirm final room completion unlocks the next realm's first room only.

## Jackpot Wheelspin

- Earn or grant a pending jackpot wheelspin.
- Confirm jackpot wheelspin opens as a post-round/claim moment, not setup.
- Confirm multiple spins stack before collection.
- Confirm rewards are not granted until Collect.
- Confirm Daily Spin remains separate.

## Daily Bonus

- Open Daily Bonus from Den.
- Confirm it is direct-claiming and not Inbox-routed.
- Claim once.
- Confirm claimed state persists until reset.
- Confirm Daily Bonus close returns to Den.
- Confirm Daily Bonus does not alter Daily Spin state.

## Daily Spin

- Open Daily Spin from Den.
- Confirm it is separate from Daily Bonus and jackpot wheelspin.
- Spin once.
- Confirm result is applied directly according to prototype behavior.
- Reset Daily Spin from dev settings.
- Confirm reset returns to Den when launched from Den settings.

## Inbox

- Open Inbox.
- Switch between Messages, Cards, and Gifts.
- Confirm messages can open from row click and Read/Open.
- Confirm Read marks messages read.
- Confirm Reply opens the friend message composer when applicable.
- Confirm card gifts require claim before applying.
- Confirm Claim All does not affect messages as reward grants.
- Confirm Clear All reads/clears message items only.

## Freebies

- Open Freebies.
- Confirm official social links are visible.
- Simulate social freebie redemption.
- Confirm redemption applies directly and does not route through Inbox.
- Confirm duplicate/expiry behavior remains labeled prototype until backend validation exists.

## Friends

- Open Friends.
- Confirm friend list, incoming requests, sent requests, add/search placeholder, block/report, and message actions are visible.
- Confirm Give 10 and Claim daily mana actions show current same-day capacity.
- Confirm same-day send/receive state persists after leaving and reopening Friends.
- Confirm blocked friends cannot receive mana/message actions.
- Confirm sent messages route to Inbox > Messages.
- Confirm friend mana does not grant or deduct inventory until Aura-rank limits and economy behavior are approved.

## Coven

- Open Coven Circle.
- If not joined, confirm Find/Join Coven discovery placeholder works.
- If joined, confirm member list and member popup open.
- Confirm member wish list remains ingredient-focused.
- Confirm Coven gifts route to requester Inbox for collection.
- Confirm card gifting/trading is not managed from Coven wish lists.
- Confirm Bazaar -> Coven clears Bazaar modal before opening Coven.

## Library, Grimoire, And Book Of Shadows

- Open Library/Grimoire.
- Confirm Grimoire and Book of Shadows are separate.
- Confirm card states are readable: missing, owned, duplicate, selected, and NEW where applicable.
- Open card gifts.
- Confirm only extra regular Grimoire duplicates are giftable in the prototype.
- Confirm incoming card gifts route to Inbox > Cards and require claim.
- Confirm Library -> Inbox clears the Library modal before opening Inbox.

## Ingredients, Apothecary, Ask For Help, And Use Wild

- Open Apothecary.
- Confirm Apothecary presents potion-only content.
- Confirm Cabinet-only items do not appear as Apothecary inventory.
- Open ingredient detail from Grimoire/potion flow.
- Confirm Ask for Help opens from ingredient detail, not as a standalone Coven hub responsibility.
- Confirm Use Wild is a card-to-card confirmation and does not have a separate inventory screen.

## Cabinet Of Curiosities

- Open Cabinet.
- Confirm playable sigils, Clairvoyance/timed boosts, Pandora Sigil, and power-up inventory live here.
- Confirm cards and Club Orbs do not live here.
- Confirm activating Clairvoyance returns to the expected flow and shows in room/game context.

## Enchanted Trail

- Open Enchanted Trail.
- Confirm free-path placeholder nodes show Collect/Claimed/Locked state.
- Claim an unlocked placeholder node.
- Confirm claimed state persists after leaving and reopening Den.
- Confirm Fresh New Player resets claimed Trail state.
- Confirm no inventory reward grant occurs.

## Oracle Alley And Madame Solange

- Open Bewitchment Bazaar.
- Confirm center area is the card/crystal swap cart placeholder.
- Confirm Oracle Alley opens Madame Solange's reading-table prototype.
- Select face-down cards.
- Confirm reveals are preview-only and do not grant rewards.
- Confirm Book of Shadows bonus is placeholder-only.
- Confirm reading cadence, costs, dust conversion, odds, and rewards remain unresolved.

## Leaderboards

- Open Leaders.
- Switch between Friends, Team/Coven, and Global tabs.
- Confirm standings are static prototype rows only.
- Confirm no rewards, scoring formula, seasons, backend sync, or competitive economy behavior is active.

## Mayhem Market

- Open Mayhem Market.
- Confirm Mana, Crystals, Card Packs, Cosmetics, Special Offers, and Restore Purchases categories are visible.
- Confirm all purchase buttons are disabled.
- Confirm no prices, grants, receipts, restore purchases, platform IAP, or monetization tuning is active.

## Profile, Aura, Rank, And Settings

- Open Player Profile.
- Confirm player name/avatar/rank/level/mana/crystals/album/room/basic stat placeholders appear.
- Confirm Rank display labels Aura-derived Rank as TBD.
- Confirm Level and Aura/Rank are not presented as the same thing.
- Confirm sound/notification/login/avatar/frame/dauber controls are placeholder-safe.
- Change avatar, frame, dauber, Sound, and Notifications selections; restart Play Mode and confirm they persist.
- Use Fresh New Player and confirm avatar/frame/dauber return to defaults while Sound/Notifications remain unchanged.
- Confirm Fresh New Player does not replace the stable local guest identity.
- Edit the display name, restart Play Mode, and confirm the normalized name persists.
- Confirm invalid display-name characters/lengths are rejected as local Beta validation and are not presented as final moderation rules.
- Confirm profile-settings journal payloads do not contain display-name text.
- Drop a correctly named test avatar/frame Sprite into the documented cosmetic Resources paths and confirm Profile/Avatars previews use it without changing the saved cosmetic id.
- Remove the test Sprite and confirm the placeholder rendering returns without corrupting profile state.

## Local Infrastructure

- Run the edit-mode `InfrastructureServiceTests` suite.
- Confirm durable snapshots round-trip and recover from a malformed primary file through the last-known-good backup.
- Confirm journal sequence numbers continue monotonically after reopening the journal.
- Confirm status changes append transition records rather than rewriting existing actions.
- Confirm a local guest id remains stable across facade recreation.
- Confirm local Remote Config reads typed supplied defaults and returns explicit fallbacks for missing values.
- Confirm Remote Config safety diagnostics show policy `infra_remote_config_safety_v0.1`, UGS adapters off, profile Cloud Save sync off, journal upload off, diagnostics export on, risky enabled count 0, missing required key count 0, and unknown key count 0 for the default local composition.
- Confirm test-only risky Remote Config values are reported as blocked diagnostics and do not enable live UGS, Cloud Save upload/download, or journal upload.
- Confirm UGS packages are resolved but no live cloud calls, analytics uploads, adapter define, or gameplay-state migrations are active in this pass.
- Confirm profile/settings changes append local journal records and malformed profile snapshots recover through the last-known-good backup.
- Open Prototype Settings > Persistence and confirm identity is redacted, snapshot schema/health is visible, and journal counts appear.
- Export a safe diagnostics summary and confirm it contains no full player id, journal payload, action id, idempotency key, message content, or token.
- Confirm Prototype Settings > Persistence shows journal sync staging with live uploads off, active upload eligible 0, and sensitive/unapproved row counts separated.
- Confirm Prototype Settings > Persistence shows Profile Cloud Save Sync with live sync off, upload blocked, download blocked, adapter compiled no, and key `bmm.profile_settings.v2`.
- Confirm Profile Cloud Save Sync also shows conflict policy `profile_cloud_conflict_policy_v0.1`, local snapshots authoritative, remote overwrite blocked, automatic merge blocked, gameplay sync blocked, and five blocked approval gates.
- Confirm the disabled profile/settings Cloud Save facade does not create remote writes or reads when changing display name, avatar, frame, dauber, sound, or notification settings.
- Confirm safe diagnostics export includes journal policy counts only and still excludes payloads, action ids, idempotency keys, messages, tokens, receipts, and full player ids.
- Confirm an older supported snapshot migrates one schema step at a time and records `snapshot_migrated`.
- Confirm a snapshot newer than the client is rejected rather than silently overwritten or downgraded through its backup.
- Confirm the panel provides no journal clear/retention action while retention policy remains unresolved.
- Confirm the five approved UGS package entries resolve in Package Manager and update `packages-lock.json` before enabling the adapter define.
- Confirm Prototype Settings > Persistence shows UGS preflight with packages resolved, live calls off, and project-link/consent/Cloud Save policy still blocked.
- With `BMM_UGS_ADAPTERS` absent, confirm the local-first path still starts and profile/settings work with network unavailable.
- Do not enable the adapter define until the development environment is linked and Analytics consent behavior is verified.
- Unity EditMode `InfrastructureServiceTests` passed 19/19 on 2026-07-10 after adding the disabled Cloud Save profile/settings sync, conflict/offline policy, and Remote Config safety scaffolds.

## Documentation Follow-Up After Each Pass

- Update `CURRENT_PROTOTYPE_STATE.md` when behavior, scope, or persistence changes.
- Update `09_OPEN_DECISIONS.md` only when an open issue is added, changed, or resolved.
- Update `10_VERIFIED_PROJECT_STATE.md` only after a larger audit or confirmed decision shift.
- Update `12_BETA_1_PRODUCTION_READINESS.md` when readiness status changes.
- Keep archive/chat exports as reference only unless reconciled into locked/current docs.

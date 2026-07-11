# 18 — Local Analytics Event Catalog

This catalog defines the current local-only Beta analytics surface for Bingo Magic Mayhem.

- All events remain local-first and journal-backed.
- No live upload, dashboard wiring, or UGS Analytics connection is active in this pass.
- `analytics_safety_v0.1` still blocks consentless collection/upload behavior.
- Payloads should stay normalized, support-safe, and free of message body text or direct social-message content.

## Event Families

### Room and round flow

- `prototype_room_entered`
  - room context only: realm index, room index, room name, room mode, selected card count, mana bet per card, restored state
- `prototype_round_started`
  - room context plus entry cost
- `prototype_bingo_claimed`
  - room context plus card index, bingo delta, claimed count, room bingo state, blackout flag, trigger
- `prototype_round_completed`
  - room context plus completion reason and reward-preview summary
- `prototype_round_rewards_collected`
  - reward-collect summary from the round result surface
- `prototype_room_restored`
  - room context plus restore reward mana

### Daily and inbox flow

- `prototype_daily_bonus_claimed`
  - streak day and safe reward summary
- `prototype_daily_spin_claimed`
  - reward index and safe reward summary
- `prototype_inbox_message_read`
  - category, read source, unread remaining, claimable remaining
- `prototype_inbox_item_claimed`
  - category, claim-all flag, reward source, safe reward summary, coven-gift flag when present
- `prototype_inbox_category_cleared`
  - category and cleared count

### Album and social flow

- `prototype_album_reward_claimed`
  - album, scope, entry/set references, safe reward summary
- `prototype_social_freebie_redeemed`
  - safe reward summary and local freebie code
- `prototype_social_help_request_sent`
  - ingredient-help context, coven-tab source, card id/name, daily usage counters
- `prototype_friend_mana_sent`
  - safe amount and daily send counters
- `prototype_friend_mana_received`
  - safe amount and daily receive counters
- `prototype_wild_card_used`
  - grimoire-only use-wild success, entry number, card id/name, tier, remaining wild cards
- `prototype_coven_wish_gift_sent`
  - wish type, item name, quantity, remaining local inventory count

### Coven and jackpot flow

- `prototype_coven_orbs_contributed`
  - contributed amount and remaining held orbs
- `prototype_coven_emporium_purchase`
  - offer id, offer name, orb cost, remaining held orbs, safe reward summary
- `prototype_jackpot_collected`
  - collected mana, spin result count, reset-pot flag, pending spins remaining

## Explicit exclusions for this pass

- no message body text
- no freeform social chat text
- no receipts, payment identifiers, or account tokens
- no live upload toggles
- no Remote Config authority to bypass consent/upload blocking
- no gameplay/economy balancing decisions inferred from analytics wiring

## Implementation anchors

- Event names: `Assets/Scripts/Infrastructure/LocalAnalyticsFacade.cs`
- Payload shaping: `Assets/Scripts/Infrastructure/PrototypeAnalyticsPayloadFactory.cs`
- Feature emission timing: `Assets/Scripts/BingoPrototype.cs`
- Safety verification: `Assets/Tests/Editor/InfrastructureServiceTests.cs`

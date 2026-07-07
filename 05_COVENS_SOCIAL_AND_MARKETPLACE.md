# 05 — Covens, Social Systems, and Marketplace

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
| Teams are called Covens | LOCKED | Use consistently. |
| Collection Assist as cooperative feature | LOCKED / HIGH | Safe to model. |
| Manual sharing/helping | LOCKED / HIGH | Do not auto-transfer inventory. |
| Ingredient gifting/requesting | HIGH / SYSTEM AREA LOCKED | Build hooks, wait for limits. |
| Card/duplicate trading | HIGH / SYSTEM AREA LOCKED | Build hooks, wait for eligibility. |
| Bewitchment Bazaar | HIGH / LIKELY FINAL | Use as marketplace container unless renamed. |
| Oracle Alley | PROPOSED | Treat as sub-feature, not core marketplace. |
| Madame Solange | OPEN | Not confirmed in extracted docs. |
| Crystal ball bonus reveal | PROPOSED VISUAL | Do not bind to economy without approval. |
| Coven Ritual | PROPOSED / PHASE UNCLEAR | Keep separate from MVP unless approved. |
| Coven Orbs / Ritual Calls / Sigils | PROPOSED / EVENT-SPECIFIC | Do not treat as active currencies unless Coven Ritual is in scope. |

## Extracted social and marketplace details

## 11. Covens/social systems

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Teams are called Covens. | Social naming | Developer and graphic handoffs define teams as Covens, and later Coven + Bazaar / Trading handoff keeps this naming. | Use Coven consistently for team/social features. | No caveat other than final UI copy polish. |
| Collection Assist is a flagship cooperative feature. | Social UX | Both developer and graphic handoffs identify Collection Assist as a major differentiator/flagship UX, and later trading scope builds on surfacing missing cards/ingredients. | Must show what players need, what they can share, team needs, suggested helps, and ending-soon items. | Advanced suggestions/request-reserve flow may be phased. |
| Sharing/helping should be manual, not automatic. | Social rule | Developer handoff states sharing should be manual, not automatic; later trading scope includes limits and fairness rules. | Player agency and daily limits are required. | Auto-suggestions are allowed, but auto-transfer is not locked. |
| Coven + Bazaar / Trading System has a locked handoff. | Social/trading | Later Project source says the locked Coven + Bazaar / Trading System handoff was packaged and rendered. | Must cover Coven basics, ingredient gifting/requesting, card/duplicate trading, limits, cooldowns, fairness, and surfacing missing items. | Full detailed rules require consulting the locked handoff itself. |

## 12. Marketplace

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Bewitchment Bazaar is the preferred/locked marketplace name currently in use. | Marketplace naming | Later discussion chose Bewitchment Bazaar as unique and fitting, and later handoff planning says it had already been named. | Use Bewitchment Bazaar for the hub marketplace/trading zone unless renamed explicitly. | Exact scope of marketplace vs Coven hub vs Oracle Alley remains open. |
| Marketplace scope includes ingredient gifting/requesting and card/duplicate trading. | Marketplace scope | Coven + Bazaar handoff planning explicitly lists these systems as defining content, and a locked handoff was subsequently packaged. | Requires limits, cooldowns, fairness rules, eligibility, and missing-item surfacing. | What can/cannot be traded remains an open detailed rule. |
| Marketplace/event timing must be considered. | Live ops | Coven + Bazaar planning explicitly calls out marketplace/event timing. | Do not assume every marketplace feature is always-on until event timing is locked. | Oracle Alley may be limited-time or intermittently open. |

## 13. Oracle Alley / Madame Solange

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Oracle Alley is not locked as a core marketplace name; Bewitchment Bazaar supersedes it as the current marketplace container. | Oracle/marketplace | Later discussion moved toward Bewitchment Bazaar for the broader market, while Oracle Alley remained a possible subset/feature. | Do not replace Bewitchment Bazaar with Oracle Alley in locked docs. | Oracle Alley may still exist as a limited-time or sub-area feature. |
| Oracle Alley can be treated only as a possible feature until rules are locked. | Oracle feature | Later discussion liked it being permanent but sometimes open, and possibly producing a single asset from readings, but this was not packaged as a locked system in available sources. | Do not balance wild-card or trade economy around Oracle Alley yet. | Needs final decision on availability, output, odds, and whether every reading can produce a wild. |
| Madame Solange has no locked decision found in available Project files/search results. | Character/naming | Searches for Madame Solange did not surface a locked Project source in the available files. | Do not treat Madame Solange as official until confirmed by user or source. | Could exist in unsurfaced chat context; needs retrieval or explicit lock. |

## 10. Daily/weekly rewards

| Decision | Category | Why it appears locked | Related constraints | Possible conflict / caveat |
| --- | --- | --- | --- | --- |
| Daily rewards are part of the retained/locked rewards system work. | Retention | A locked Rewards / Retention Utility Systems handoff exists, and daily rewards were repeatedly identified as an active system. | Daily reward claim should be visually rewarding and animation-ready. | Exact daily calendar/streak reward ladder is not visible in available snippets. |
| Weekly social/event rewards are tied to Coven systems where Coven Ritual is active. | Weekly rewards | Coven Ritual and Coven Emporium rules define weekly event reward loops; later Coven/Bazaar handoff locks social/trading system direction. | Weekly systems should reward contribution and avoid feeling purely competitive. | Full Coven Ritual may be Phase 2 rather than MVP. |
| Realm/special-room replay should support ongoing rewards after restoration. | Retention/progression | Later discussion favored keeping rooms open and using special rooms/witch visits for continued earnings and larger weekly rewards. | Post-restoration rewards should not block access to needed ingredients. | Exact cadence and reward values remain open. |



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

## Codex implementation guardrails

- Keep social transfer systems server-authoritative.
- Do not create automatic sharing; players may receive suggestions, but inventory movement must be manual unless later approved.
- Gifting, requesting, and trading require eligibility checks, daily limits, cooldowns, and rarity restrictions.
- Do not assume global trading; marketplace may be Coven-only, event-timed, or limited.
- Do not assume Oracle Alley is always open.
- Do not assume Madame Solange is an official NPC.
- Do not assume tarot/crystal-ball outcomes produce wildcards or currency until approved.

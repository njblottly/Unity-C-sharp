# Marshlight campaign backlog
Updated 6 November 2026. Only Milestone 0 is authorized in the current run. Milestone 0 is complete and verified. The full chapter/enemy/mechanic/boss designs are proposals for later bounded milestones, not implemented content.

## Recommended next milestone
Milestone 1: extend existing story/level coordination with chapter/section data, stable trigger policies and an isolated five-gate campaign blockout. Preserve the original scene and all completion/coin/sword regressions. Do not implement the later enemy roster or mechanics during that run. Ask before changing build settings or packages.

## Suggested next three
1. Milestone 1: chapter/section/story data and isolated five-gate campaign blockout, with ActI/II delivery and the original scene preserved.
2. A fresh human timed baseline without F6: check sword/coin clarity and confirm the preserved top-down perspective before campaign production.
3. Plan the visual adapter for Aiken's irregular frames and replacement art specifications; implement art only in its scoped later milestone.

## Decisions/risks to resolve at the relevant milestone
- Current top-down XY movement differs from the side-view brief. Keep existing input/controller for now; a conversion needs a separate decision before implementation.
- Source art is mixed: Aiken uses large irregular frames, duck/mite use 32x32. Specify a visual-only adapter before normalizing player art; never change root collider scale to make a sprite fit.
- EnemyHitPoints and Weapon already own working damage/feedback. Extend their contracts to support data-driven foes and a boss health bar without double damage or twenty tiny boss pips. Preserve legacy contact tests.
- Boss entry must not strand a player without a sword or carried coin. Make missed equipment and every outstanding loan recoverable before arena lock-in.
- Build/scene order is Project Settings: prepare the campaign scene and Editor tests before requesting that separate settings change.
- Initial Editor HandleUtility font notices have no identified missing scene font; investigate if reproducible, without masking errors or changing settings speculatively.

## Planned, not built
- Three mechanics: Duck Perches, Inspection Lantern and Return Mail Routing.
- Reed Marks journal collectible (8), four new enemy types, one mini-boss and one final boss.
- Per-section configuration, boss phases, full Act I–IV delivery, sound set/master volume and chapter visuals.
- Exhaustive reachable-state checks for the combined lamp/perch/routing coin economy.

## Outside the agreed expansion
- Shields, parry system, extra weapon variants, crafting/rarities, persistent metaprogression, procedural levels, additional chapters, platforming/jumps and package/input migration are deferred.
- Do not add these while implementing a named milestone merely because a related system is being edited.

## Human validation
Time the current prototype and, later, each campaign chapter without F6; assess sword/coin clarity, duck behavior, telegraphs, journal discoverability, other aspect ratios and audio. The proposed 25–32 minute campaign time is not yet a measured result.

## Existing backlog retained for reference
# Marshlight backlog
Updated 6 November 2026. Iteration 1, outdoor-art/name pass, and the authorized mailbox/signpost/sword extension are complete and verified.

## Suggested next three
1. Run a fresh human playtest without F6. Check first-time sword discovery, facing/attack feel, coin-loan clarity, exchange comprehension and section telemetry; time runs with and without the optional ledger page. The 5–10 minute target still needs a timed human result.
2. Finish the broader art/animation pass: readable player/duck/patrol idle and walk frames, dedicated relay-cup art and optional bell animation, and a complete replacement specification for any newly commissioned animation sheets. ASSETS_NEEDED.md now inventories current placeholders and the new sword/props; existing Outdoors assets remain directly usable.
3. Add the broader synthesized sound set and pause-menu master volume. Only the sword swish is wired today; footsteps, coin lend/reclaim, bell, dialogue, hit, defeat and victory audio remain to be made.

## Incoming art
When Josef places replacement art/audio in Assets/_Incoming, validate names, dimensions, pivots, PPU and frame layouts against the manifest before importing. Preserve GUIDs and serialized scene/prefab references. Report missing or mismatched files instead of guessing. No incoming-art import was part of the sword extension.

## Deferred mechanics
- Shields, a second equipment slot, blocking/parrying and extra sword variants are ideas only. The new panel currently displays the single Reedkeeper Sword.
- Alternative coin-borrowing tools: a short-range reed clapper, a thrown brass echo, or a temporary lantern ward. Design each separately before implementation; retain three-coin conservation and all crossing/exchange prerequisites.
- Patrol timing/route-planning variants should complement the established sword rules. Keep coin progression independent of enemy health and never require a particular patrol to remain alive.
- Consider an explicit telemetry retention/export workflow if development logs accumulate; do not delete existing logs automatically.

## Completed
- Required archive counterweight exchange, optional story pocket, two story entries, development pacing overlay/logs, permanent coin-state regressions, atomic returns, player/Duckduckgoose teleport fixes and full route verification.
- Reused original outdoor ground/water/grass, trees, rocks, bushes, tufts, stumps and logs. Current names: Aiken, Duckduckgoose and Ada. Original outdoor textures/importers and LevelOne preserved.
- Pixel mailbox, four signposts and six coin-return mailboxes; Reedkeeper Sword pickup, data-driven swipe damage through existing EnemyHitPoints, temporary/refundable coin cost, equipment HUD, HP pips/flash, swish, one new story entry and combat/coin regression coverage. See SWORD_AND_PROPS_REPORT.md.

- Bell/field-letter/mailbox grounding follow-up complete: three framed bells, visible paper/seal, seven full posted mailboxes and ten lanterns with live glass markers. Forty cases and ninety-seven route assertions pass; all collision and runtime sources unchanged. See PROP_FINISH_REPORT.md.

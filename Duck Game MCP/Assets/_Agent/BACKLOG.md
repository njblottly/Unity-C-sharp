# Marshlight campaign backlog
Updated6November2026. Milestones1–4 COMPLETE; stop before5. No git commits. Human M0 checks passed. Current playable scene: Assets/_Agent/Scenes/Marshlight_Campaign.unity. Historical M0 proposals are preserved in campaign-run/design evidence; the latest user authorization supersedes old approval/commit rules.

## Next three priorities
1. Milestone5: implement mini-boss/final boss from DESIGN, explicit BossData/phases/telegraphs, immediate safe checkpoints, entry recovery for sword/coin loans, and climax/twist beats. Preserve all98 regressions plus gate/puzzle dependencies. This run has not started M5.
2. Human timed campaign playtest without F6: time each chapter, read normally, try both mail paths, optional marks and combat. Record first-time learning/deaths and compare with the final25–32minute target. Mechanical exploration3m29s is not a human duration.
3. Milestone6 art adapter/production art and environmental set pieces. ART_ADAPTER.md specification is complete; apply visual-child adapters to mixed Aiken/duck/mite frame sizes only in that scoped work. Replace labelled placeholder enemy/mechanic/mark/badge assemblies using ASSETS_NEEDED. Preserve root collider/animation bindings and asset GUIDs where shared.

## Remaining planned work
- M5 bosses plus full climax/emotional duck reveal; loan-safe arenas, boss health presentation beyond8pips, readable phases and recovery.
- M6 chapter visuals/set pieces, final resolution expansion, actor art adapter and production replacement sheets.
- M7 feedback/audio/volume, readability across aspect ratios and measured balance/standalone validation.
- Player/duck/enemy animations and dedicated attack/seed/mark/perch art/sound; current stand-ins are manifest-listed. The badge is currently a visible HUD/journal text award.
- More optional mail-route variants or weapon/shield variants only in a future scoped run. No procedural levels, crafting, metaprogression, platforming conversion or additional chapters added here.

## Human and technical follow-ups
- Time an initial read-through, a zero-mark route and the all-eight route; assess whether both addressed destinations and lamp refunds are clear.
- Controller/hardware input feel, other aspect ratios, sound balance and a standalone build remain untested. Existing legacy Input Manager retained.
- Finite coin audit covers203states of the current topology/cost preset, not arbitrary future topology or physical trajectories. Extend its explicit model when adding arena locks/extra resource types; retain actual runtime tests.
- If MCP disconnects during a capture, resume from the saved checkpoint/view list; do not redo asset writes in Play. Wait for the Test Framework to restore the saved scene after its result callback before starting Play.

## Completed this run
Data-driven chapter/story framework and ActI/II delivery; four new normal types/section difficulty; perches/inspection lamps/selectable mail;8cosmetic Reed Marks/journal; clean checkpoint/replay/coin regressions; art-adapter specification. No unimplemented item above is represented as built.

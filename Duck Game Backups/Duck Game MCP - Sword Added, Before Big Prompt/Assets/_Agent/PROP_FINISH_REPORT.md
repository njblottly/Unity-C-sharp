# Marshlight — bells, field letter and posted mailboxes
6 November 2026

## What changed
The Landing Bell, Archive Bell and Haven Bell now use a hanging brass bell in a weathered timber frame. Ada's first mailbox carries a small parchment letter with a wax seal. Four checkpoint lamps and the six haven lamps now use framed pixel lanterns; the live checkpoint light stays inside the glass and still turns warm when reached.

The buried-looking return mailboxes were using a 32x24 crop containing only the mailbox head. All six now use the first mailbox's complete 32x48 sprite, scale, pivot and base placement. The first mailbox body is unchanged. Coin-state indicators sit on the doors, and the three relay return labels were raised above the taller boxes. The obsolete head-crop asset is retained for rollback but is not referenced by any current scene renderer.

New art: Assets/_Agent/Placeholder/Props/BellFrame.png (32x48), FieldLetter.png (32x32), MarshLantern.png (32x48). All use the established fifteen opaque colours plus transparency, binary alpha, Point filtering, no compression and 32 PPU. The two tall props have base pivots (16,2); the letter uses (16,16). ASSETS_NEEDED.md records the full specifications. The built-in image generator created the source art; nearest-neighbour size/palette preparation produced the Unity sprites. Exact prompts and preparation metadata are archived in Data/PropFix20261106/ArtPrompts.json and ArtPreparation.json.

## What stayed intact
All 59 collider fingerprints and every serialized gameplay component/marker reference match before and after this pass. Node/root positions, interactions, twelve story entries, coins, gates, checkpoints, patrols, sword, inventory and movement are preserved. No manager, Canvas, gameplay component, sound or story entry was added. The established names remain Aiken, Duckduckgoose and Ada.

Thirty-three of thirty-five existing scripts are byte-for-byte unchanged; every runtime source is unchanged. The two targeted extensions are:

| Script | Why it changed |
| --- | --- |
| Assets/_Agent/Scripts/Editor/MarshSwordBuilder.cs | Uses the full posted return mailbox, door-height coin indicator and raised return labels, so rebuilding cannot restore the cropped-head bug. No public API change. |
| Assets/_Agent/Tests/PlayMode/MarshCoinRegressionTests.cs | Adds two permanent cases for complete mailbox posts/base alignment/coin visibility and bell-letter-lantern interactions/live checkpoint status. Existing cases retained. |

One new editor-only helper, Assets/_Agent/Scripts/Editor/MarshPropFinish.cs, applies the visual changes and checks collider, serialized-logic and marker-identity invariants. It can safely reapply the same dressing. Source/scene/document backups are in Data/PropFix20261106/.

## How to try it
Open Assets/_Agent/Scenes/Marshlight.unity and press Play. At the landing, read the letter attached to Ada's mailbox with E. Collect the three coins, visit the Landing Bell, lend them and cross to the posted return mailbox. Use E to reclaim them. Inspect the matching art at the later bells, return boxes and haven. WASD/arrows move; E interacts; Space/Enter advance text; J/left click swipe; Q equips/packs; Esc pauses; R restarts after defeat. F6/F7 remain development-only.

## Verification
| Check | Before | After |
| --- | ---: | ---: |
| Complete PlayMode suite | 38 passed | 40 passed, 0 failed/skipped |
| Full no-F6 physical route | 97 assertions | 97 passed, 0 failures |
| Native console errors / warnings | 0 / 0 | 0 / 0 |
| Colliders | 59 | 59, every fingerprint unchanged |

Final suite: cb071d0588434262bd0e4bb358b529ee (SDK 14.883 seconds). Two new cases supplement every existing regression. Unity's SDK leaves its NUnit assertion-count field at zero, so case counts and the reliable walker assertion total are reported separately.

The full 70-step title-to-victory route walked 159.57 world units through actual movement, triggers, enemy contact, sword use/refund, death/restart, every crossing and all twelve stories. It finished with exactly three coins and zero escrow, teleported=false. Development F6 destinations 0–3 were separately captured and their consoles read; they granted no bell/checkpoint/coin progress. All six returns were visually inspected, including separate close-ups of all three archive boxes. Fifteen images cover those views, the Landing Bell, every ordinary checkpoint, swipe, defeat and victory.

Telemetry records 106.01 active simulated seconds (29.75 / 27.76 / 44.90 / 3.60 by section); reading is skipped and screenshot holds are excluded. This art-only pass adds no route distance or gameplay wait. It is not a timed human playtest. Fresh replay restored the title, zero carried coins/sword, three recoverable coins, full patrol HP, no sword loan and all six full posted mailbox sprites.

Evidence is saved in Assets/_Agent/Data/PropFix20261106: baseline/source/scene backups, BuildAudit, unchanged collision/logic signatures, scene/source and replay audits, final tests/recorder, FullRoute, pacing JSON/JSONL, console summary and fifteen screenshots. Scene saved and left in Edit Mode, unpaused.

The new regressions check all six return boxes against the good first mailbox, ensure tall complete silhouettes and visible door markers, and verify relay quantities stay above their boxes. The interaction case reads the original field letter, lends/reclaims the same three coins at the new Landing Bell/mailbox and enters a real checkpoint trigger to check its live light. All earlier coin soft-lock, replay, combat and text-fit tests remain.

During development, the first builder guard checked read-story count instead of authored-story count and stopped before editing the scene. That guard now checks the existing story asset's entries. Project script compilation completed with no errors or warnings.

## Known limits
No gameplay blocker was found in the tested Editor states. Visual review is at 1280x720; extra aspect ratios, hardware input feel, sound balance, human pacing and a standalone player still need human playtesting. The bell/lantern art is static, and the broader character animation, relay-cup art and sound/volume work remain backlog. This pass introduces no new pacing content or mechanic.

## Numbered manual checklist
1. Start a fresh run and read Ada's field letter. Confirm the paper/seal and first mailbox remain clear and correctly placed.
2. Inspect the Landing Bell before and after lending. Check the hanging bell/frame, cost prompt, bridge opening and ordinary story text.
3. At each of the three crossing returns, check the complete wooden post, door coin indicator and three-coin refund. None should look cut off or buried.
4. Inspect all three archive returns. Confirm full posts, clear 1/2 RETURN labels, correct coin totals and no overlap hiding the exchange route.
5. Reach each checkpoint normally. Check the light changes inside its glass, restart placement still works, and the haven's six lanterns sit correctly.
6. Complete the level without F6, verify twelve stories/three recovered coins, then replay. Check sword controls and the existing inventory as part of the run.

## Suggested next three backlog items
1. Fresh timed human run checking prop readability, coin/sword clarity and the first-time pacing target.
2. Character idle/walk frames and dedicated relay-cup/mechanism art using the manifest.
3. Broader sound set and pause-menu master volume.

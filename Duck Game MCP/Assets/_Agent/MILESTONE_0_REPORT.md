# Marshlight — Milestone 0 report
6 November 2026. COMPLETE. Milestone 1 has not started.

## What changed and how to try it
Completed analysis and planning only. Read DESIGN.md for the four-act story, all 22 planned beats, five chapter themes/time budgets, four new enemy types and two bosses, three new mechanics, Reed Marks collectible and future data architecture. PROGRESS tracks milestones1–7; BACKLOG and ASSETS_NEEDED retain existing work and planned asset specifications. Root CHANGELOG.md was created. Inventory/art snapshots and seventeen verification images are under Data/Milestone0.

The playable baseline remains Assets/_Agent/Scenes/Marshlight.unity. Open it and press Play; WASD/arrows move, E interacts, Space/Enter advance text, J/left click swipes, Q equips/packs, Esc pauses, R restarts after defeat. F6 cycles existing checkpoints in Editor/development only; F7 toggles pacing. F6 grants no coins, gates or story progress.

Important finding: this project currently uses top-down XY movement, three main bells plus three relay gates, four checkpoints and twelve stories. The brief described side-view and roughly five gates. The plan preserves current movement and proposes five main bells in a future separate campaign scene; M0 does not convert the camera or controller.

## Existing scripts modified
Only Assets/_Agent/Tests/PlayMode/MarshCoinRegressionTests.cs: one appended UnityTest, CurrentGameCompletesFromTitleWithAllStoriesAndThreeCoins. Existing helpers/tests are retained. It starts a fresh real title, collects real pickups, uses public gate/story interactions, traverses checkpoint triggers and ends with all twelve stories, exactly three coins and zero escrow. It positions actors as test fixtures; physical reachability is verified separately by the walker.

No production API, runtime source, scene/prefab, gameplay data, original art/importer or package/input change. Existing F6 was already guarded and regression-covered, so it was reused rather than duplicated. MCP's temporary persisted test-runner option was restored to starting settings bytes; no Project Settings policy change.

## Tests and mechanical verification
| Check | Before | After |
| --- | ---: | ---: |
| PlayMode test cases | 40/40 | 41/41 |
| Failed / skipped | 0/0 | 0/0 |
| Physical walker assertions | Existing 97-check walker | Fresh97/97 pass |
| Native console errors / warnings | Fresh baseline0/0 | Final0/0 |

Baseline test job 702f4e2b464d45cf83c1ce3ed4bad93e, SDK 15.201s. Final job 2d0418d12a054ea4b1cf777858b00116, SDK 15.558s. All existing coin-state/death/checkpoint/replay/sword/art/telemetry cases still pass. No suite assertion total is inferred from the test recorder's unavailable 0 count.

Fresh full title-to-ending route without F6:70 steps,159.57 world units. Real MovementController/colliders/triggers, sword pickup/hit/refund, enemy contact, deliberate defeat/restart, all gate dependencies, twelve stories, four checkpoints and conserved three coins. Four debug destinations plus six gate close-ups captured with console reads at each; seventeen images including normal checkpoints/sword/defeat/win. Debug inspection invokes the same guarded method as F6, not hardware input injection. Zero missing scripts;59 colliders unchanged; one each manager/UI/player/duck/weapon/relay.

## Play time
Fresh Milestone 0 route, 6 November 2026: the existing editor walker started at the real title and reached victory without F6. It passed 97/97 assertions across 70 steps and 159.57 world units. Active telemetry was 106.024 seconds (1:46.02); text advanced immediately, giving about 0.04 seconds recorded reading time. One death/restart was deliberate test input.

| Current checkpoint interval | Active time | Deaths |
| --- | ---: | ---: |
| Landing to Reedbank | 29.76 s | 0 |
| Reedbank to Archive | 27.76 s | 1, deliberate |
| Archive to Haven | 44.90 s | 0 |
| Haven to reunion | 3.60 s | 0 |
| Total | 106.02 s | 1 |

These are the current prototype's four intervals, not five campaign chapter timings. Simulation/checkpoint holds, including the MCP outage, are excluded; this is not wall time or a first-time human result. The five proposed chapters have no measured time yet. Time them as they are built, then validate the 25–32 minute campaign target with a human run.

Proposed chapter targets, not measured:
| Chapter | Target |
| --- | --- |
| Keeper's Landing | 4–5min |
| Listening Reeds | 5–6min |
| Old Archive | 5–7min |
| Storm Locks | 5–6min |
| Lantern Haven | 6–8min |
| Total | 25–32min |

## Known limits
No known gameplay blocker in tested Editor states. Human reading/learning/input feel, other aspect ratios, standalone builds and audio balance remain manual checks. Aiken has high-resolution irregular frames; duck/mite 32x32. No new art is produced. Two pre-existing HandleUtility font notices seen initially were recorded and did not recur; active scene font references were present. Do not suppress them if they recur.

The campaign, roster, mechanics, collectible and bosses remain plans. A temporary connection interruption/checkpoint hold is excluded from active timing. Unrelated pre-existing repository changes/deletions/logs/layout/connection config were left untouched and excluded.

## Manual playtest checklist
1. Open Marshlight.unity, press Play and start from the title. Confirm Aiken/Duckduckgoose, movement, inventory and controls feel unchanged.
2. Without F6, collect the three coins and sword; time each checkpoint interval, allowing natural reading and puzzle learning.
3. At each main bell, try insufficient payment, then pay and reclaim; confirm the same three coins return and repeat interaction cannot duplicate them.
4. In the archive, split/reclaim/re-lend the pair; read the optional pocket. Confirm the final bell requires completing the exchange.
5. Die after lending but before reclaiming; restart at each reached checkpoint and finish with the coins recoverable. Check both player and duck placement.
6. Swipe, check damage/one-coin refund and hidden idle blade, then pause/resume and continue to reunion. Replay; verify fresh pickups/closed gates/reset enemies.
7. In a separate debug run, press actual F6 through four destinations and F7 on/off. Confirm no progression grants; inspect each gate and Console.
8. Run all PlayMode tests in Test Runner: expect 41 passes,0 failures/skips. Review the plan's five chapters and confirm the retained top-down perspective before the next run.

## Recommended next milestone and backlog
Next: Milestone 1, reusable chapter/section/story progression and an isolated five-main-gate campaign blockout with ActI/II beats. Preserve baseline scene/tests and keep later enemy/mechanic/boss work deferred. Ask before build settings/packages/input changes.

Top three backlog items: (1) Milestone 1 framework/blockout; (2) human timed baseline and coin/sword clarity; (3) visual adapter/specification for the existing actor frame mismatch.

## Git
Start:229d2d757710c14bd9a008a10550b037db0d582a, chore: checkpoint playable Marshlight before milestone 0.
End: the commit containing this report, docs/evidence and one test; see git log -1. Only scoped project work is staged. No push requested.

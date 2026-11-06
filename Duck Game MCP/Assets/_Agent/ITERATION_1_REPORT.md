# Marshlight: The Borrowed Bells — Iteration 1 report
Completed 3 November 2026. Run target: Iteration 1 only.

## What changed
The Old Archive now has a counterweight exchange using the same three coins. Lend two to the reed cup and one to the spill cup, in either order; each opens a different arm. Reclaim the reed pair, lend that pair again at the exit cup, and recover all three coins before the Haven Bell. Gates stay latched after death. An exit shutter rejects premature payment, and every return pays exactly once. The final bell and ending require the exchange to be settled.

An optional dry ledger page sits above the archive trail. Exactly two story entries were added (the exchange instruction plate and ledger page), bringing the total from nine to eleven. All original entries remain. The page rewards exploration with more of Ada and Pip's story and introduces no extra currency.

F7 toggles development pacing telemetry: active seconds, reading seconds and deaths for each checkpoint section. JSONL logs are written under Application.persistentDataPath/Marshlight/Telemetry. Title, pause and defeat waiting are excluded; F6 marks a run unsuitable for pacing estimates. The logger and overlay are absent from release compilation.

Permanent PlayMode regressions exposed and fixed two pose bugs: a restart far from a checkpoint could move the player Transform while leaving its Rigidbody2D at the death position; Pip's teleport could leave his visible Transform behind. Both paths now synchronize the relevant physics and visible positions immediately. Coin returns also preflight inventory capacity so they cannot partially credit coins or consume a tray when full.

Movement remains 1.6. The original three crossings retain their costs and routes. No new patrol behavior was added: the exchange supplied the preferred coin challenge and stays meaningful with a future patrol tool. The final patrol track was shifted to avoid crossing the new latch. Art, audio, weapons and incoming imports remain for Iterations 2–4.

## How to try it
1. Open Assets/_Agent/Scenes/Marshlight.unity and press Play, then Enter or Begin.
2. WASD/arrows move; E uses cups, trays and notes; Space/Enter/E advances text; Esc pauses; R restarts after defeat or replays after victory.
3. After the Old Archive checkpoint, follow the two-coin/one-coin markings and the objective text. Reclaim the reed pair before using the exit cup. The optional page is just above the upper archive trail.
4. In Editor/development builds, F7 shows telemetry and F6 cycles checkpoint destinations. Use no F6 for a pacing playtest.

## Pre-existing scripts modified and why
| Script | Targeted change / public API |
| --- | --- |
| Assets/Scripts/MonoBehaviours/Inventory/Inventory.cs | Added TryAddItems(Item,int) for an atomic multi-coin return; existing add, count and spend APIs retained. |
| Assets/Scripts/MonoBehaviours/Player.cs | RespawnAt now synchronizes Rigidbody2D position and rotation before clearing velocity. No API change. |
| Assets/Scripts/MonoBehaviours/DuckFollower.cs | TeleportTo now updates the visible Transform immediately and clears velocity. No API change. |
| Assets/_Agent/Scripts/MarshNode.cs | Added RelayLoan/RelayReturn enum values without changing existing values; added Cost, DisplayName, OutstandingCoins; delegates relay interactions, uses atomic returns and checks exchange completion at the last bell. |
| Assets/_Agent/Scripts/MarshLevel.cs | Added serialized relay/pickup references, CoinRelay, CoinsInEscrow, RecoverableCoins, AssertCoinConservation and RecordCoinAction; objectives/win checks and development-only Pacing hooks. |
| Assets/_Agent/Scripts/Editor/MarshVerification.cs | Extended the existing route/assertions for the exchange and page; one player-frame step per Editor update and simulation-time timeout for remote execution. Public API retained. |
| Assets/_Agent/Scripts/Editor/MarshBuilder.cs | Presentation repair now validates the nine original story IDs while permitting new entries. No API change. |

The source audit compares all 24 pre-iteration scripts against saved byte-for-byte backups: seven targeted changes, seventeen unchanged. No script was removed or replaced wholesale, and no existing public method was removed or renamed. Original MovementController, health, pickups, managers and patrol scripts are unchanged this iteration.

## What was tested
Baseline: the existing mechanical walker passed **66/66 assertions**, with 0 errors and 0 warnings. There were no pre-existing Unity Test Framework test assemblies.

Final: the extended walker passed **89/89 assertions** over 159.57 world units, from title screen to victory without F6, preserving all original checks. It exercised real movement, gate colliders, coin pickup triggers, a real enemy hit, deliberate health depletion/restart, all four checkpoint triggers, all eleven story entries, the exchange, duplicate rejection and exactly three coins at victory. Checkpoint, defeat and win captures are saved.

All **21 permanent PlayMode cases passed**, 0 failed and 0 skipped, in the final complete Unity Test Framework rerun (3.87 seconds of test execution). Coverage includes death with each original loan, loans at every checkpoint, all three relay phases, both initial funding orders, both spill-return orders, premature exit payment, repeated deposits/returns, full-inventory atomic return, replay after victory, patrol removal/no-bypass and telemetry/pause/F6 behavior. Every tested outstanding-loan state can settle its escrow and reach victory with exactly three coins. The test fixtures collect the actual scene pickups and use runtime positioning to arrange edge cases; the full walker separately proves physical route reachability.

All four F6 destinations were visited and captured, with the console checked at each. F6 grants no progression and marks pacing logs teleported. Final native Unity console counts after the full walk and the final suite: **0 errors, 0 warnings**. The scene audit found zero missing scripts, one instance of each existing manager/UI service, one relay, 22 nodes and eleven story entries. Release-symbol compilation of the telemetry file produced no diagnostics and no telemetry type; the saved scene has no serialized telemetry component.

Evidence: Data/Iteration1/BaselineTests.json, FullRoute.json, PlayModeResults.json, PlayModeRecorder.json, MechanicalPacing.jsonl, PacingSnapshot.json, PacingAnalysis.json, SourceAudit.json, SceneAudit.json, ReleaseExclusion.json, ConsoleSummary.json, I1_Final_F6_0–3.png and I1_Walk_*.png. Unity's SDK recorder reports assertion totals as zero for NUnit cases; the reliable before/after assertion comparison is the walker (66 to 89), separate from the 21 test cases.

## Pacing results
The table compares the same mechanical capture points. Dialogue is advanced immediately; this is movement/interaction simulation time, not a human completion time.

| Section | Baseline | Iteration 1 | Change |
| --- | ---: | ---: | ---: |
| Landing → Reedbank | 29.6 s | 29.6 s | 0.0 s |
| Reedbank → Old Archive | 27.8 s | 27.8 s | 0.0 s |
| Old Archive → Haven landing | 27.6 s | 44.9 s | +17.3 s |
| Haven landing → reunion | 2.9 s | 2.9 s | 0.0 s |
| Full run, including startup checks | 88.0 s | 105.3 s | +17.3 s |

The development telemetry independently recorded section active times of 29.1, 27.8, 44.9 and 3.6 seconds (105.3 seconds total), reading time 0.02 seconds and deaths 0/1/0/0. The death in section two was intentional. Telemetry switches sections on the actual checkpoint trigger; screenshots are held slightly later, explaining the small allocation difference at the first and last landings. Run label: mechanical; teleported: false.

Anchored to the reported four-minute human baseline, the estimate adds 17.3 seconds measured route time, 30–40 seconds for 121 new words at 180–240 words/minute, 10–15 seconds for seven new E interactions plus three page advances, and 15–35 seconds to reason through the new dependency. Total: approximately **73–108 extra seconds**, or **5.2–5.8 minutes** including the optional page. The optional detour/page accounts for roughly 17–21 seconds of that estimate; a practiced player skipping it could still finish near or below five minutes.

A first-time run now likely falls within the **5–10 minute target**, especially with the optional page. This remains an estimate: no new human pacing measurement has been made. Only the archive section changes; movement speed and the earlier sections are preserved.

## Known issues and limits
No gameplay blocker was found in the tested paths. The pacing estimate needs a fresh human run; the walker advances dialogue immediately and cannot measure comprehension or keyboard feel. Its intentional defeat is a regression check, not a difficulty sample. Placeholder art and lack of audio remain intentional. No standalone player build or additional aspect ratios were tested. Release telemetry exclusion was checked by compilation without development symbols, not by running a release build.

## Numbered human playtest checklist
1. Complete a fresh run without F6, including reading; time it and compare with the previous four-minute run. Try both with and without the optional ledger page.
2. Fund reed then spill on one run and spill then reed on another. Check that markings and the objective explain the remote gates without this report.
3. Try the exit cup before reclaiming the reed pair. Confirm your inventory keeps its coins and the prompt explains the shutter, then complete the exchange. Retrieve the spill coin before the exit payment on one run and after it on another.
4. Die away from a checkpoint after an original bell payment and during the exchange. Restart: Rowan and Pip should appear at the lantern, paid gates should remain open, and every lent coin should still be recoverable exactly once.
5. Toggle F7. Check section times and death counts, wait in pause/defeat and confirm those waits are excluded. Check the local JSONL file. In a separate debug run use F6 and confirm the overlay/log marks it teleported.
6. Read the optional ledger page, reclaim every tray and finish with three coins and Pip. Verify all eleven entries can be read. Play Again should restore all pickups, close all gates and clear loans without duplicating the inventory or HUD.
7. Check new labels, linking lines, prompts and F7 text at your normal resolution. Watch the final patrol near the latch and Pip around turns.

## Top three backlog items
1. Iteration 2: asset manifest, limited palette, procedural placeholder animation and synthesized sound with pause-menu volume.
2. Iteration 3: a data-driven keeper tool that borrows coins and affects patrols while preserving every crossing dependency.
3. Iteration 4: validate Josef's incoming filenames, dimensions and import settings against the manifest, then replace references safely.

## Persistent records
DESIGN.md records the exchange before implementation. PROGRESS.md records milestones, API additions, fixes and timing assumptions. BACKLOG.md holds future work. Evidence is saved under Assets/_Agent/Data/Iteration1/. Marshlight is saved and left in Edit Mode, unpaused. No external assets or packages were downloaded.

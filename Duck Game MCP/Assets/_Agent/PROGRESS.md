# Progress — Marshlight
Updated 6 November 2026

## Current run — Milestones 1–4
Status: COMPLETE. M1–M4 verified; stopped before M5. Final98/98 PlayMode cases,124/124 no-F6 route assertions,0errors/0warnings. Campaign saved in Edit Mode. No git commits. Original scene preserved. ART_ADAPTER.md is the finished specification; human campaign timing remains outstanding. Final report: MILESTONES_1_4_REPORT.md.

## Historical Campaign expansion — Milestone 0
Status: COMPLETE and verified. Run target Milestone 0 only; Milestone 1 has not started. Starting commit 229d2d757710c14bd9a008a10550b037db0d582a checkpoints the existing playable work. The closing commit records this planning, evidence and one additional test. Current report: MILESTONE_0_REPORT.md. Baseline scene remains Assets/_Agent/Scenes/Marshlight.unity, clean/saved in Edit Mode and unpaused.

### Expansion milestones and acceptance gates
- [x] 0 — Inventory/art audit, timed route, four-act/five-chapter plan, retained F6, completion regression, gate/checkpoint captures, final tests and closing commit. No gameplay changes.
- [x] 1 — Extend MarshStory/MarshLevel with CampaignData/ChapterData/SectionData, stable trigger policy, isolated five-main-gate campaign blockout and Act I/II beats. Keep baseline scene/tests. M1 delivered separately before later authorized content. Historical M0 setting rule superseded by latest authorization; build-scene/settings edits.
- [x] 2 — Configurable enemy agent composed with EnemyHitPoints/feedback; shared player damage/invincibility path, telegraphs/knockback, Skitter and Crab with safe lessons.
- [x] 3 — Wick Moth and Counterweight Sprout; section encounter assets and data-only HP/damage/speed/timing curve.
- [x] 4 — Duck Perches, Inspection Lantern, Return Mail Routing, eight unique Reed Marks/journal; tutorials and exhaustive coin-state recovery checks.
- [ ] 5 — Siltlock Tender and Storm Bell Engine using BossData, safe pre-boss checkpoints, equipment/coin recovery, phases, climax/twist. Boss death cannot pay a coin gate.
- [ ] 6 — Chapter set pieces, environmental notes/visuals and full resolution/reunion. Handle source frame mismatch with visual-only adapters, preserve physics/references.
- [ ] 7 — UI/readability, feedback/audio/master volume and data-only balance using fresh chapter timings; human validate the 25–32 minute target.

### Findings and deliberate decisions
- Read all five requested persistent paths (root CHANGELOG.md was missing), all 36 sources, 14 prefab assets, both scenes/dependencies and serialized connections. Detailed inventory and art audit: Data/Milestone0/ProjectInventory.json and ArtAudit.json. Before-document snapshots preserve prior design/history.
- Actual movement is top-down XY, not side-view. Three main bells plus three relay gates, four checkpoints/patrols, twelve stories. Preserve controller/camera/input; no perspective conversion assumed.
- Existing F6 cycles checkpoints only in Editor/development builds; F7 pacing already exists. Reused and verified; no duplicate manager/UI/debug handler added.
- DESIGN v6 deliberately plans a four-act/five-chapter 25–32 minute campaign: four new normal foes, mini/final boss, three new mechanics and optional Reed Marks. Future definitions/gameplay remain unbuilt. Retain original twelve story IDs and Aiken/Duckduckgoose/Ada.
- Actor art is mixed: duck/enemy 32x32 at PPU 32; Aiken has irregular large frames (sample 227x358) and many colors. Point/uncompressed/no mipmaps throughout. Duck has no continuous black exterior stroke. No art/importer edits.
- Reuse Item/HitPoints/MarshStory/WeaponData and existing health/inventory/level composition. New definition schemas belong to their listed later milestones; no second run-state singleton.

### Existing scripts changed / public API
Only Assets/_Agent/Tests/PlayMode/MarshCoinRegressionTests.cs: appended UnityTest CurrentGameCompletesFromTitleWithAllStoriesAndThreeCoins. Verifies a real fresh title/replay, actual pickups, four checkpoint triggers, all bells/relay, twelve story IDs, victory with three coins/zero escrow and no F6. Existing methods/helpers intact. The test fixture gains this public test method; no production public API change. Fixtures position actors at runtime; the separate walker proves physical reachability without F6.

No production source, scene, prefab, gameplay ScriptableObject, existing image/importer, package or input change. All 59 colliders remain, zero missing scripts, one each existing level/relay/managers/player/duck/inventory/health UI/weapon/equipment HUD. No scene rebuild needed; saved scene bytes unchanged.

### Verification and cleanup
- Baseline ALL 40 PlayMode cases pass, zero failures/skips; job 702f4e2b464d45cf83c1ce3ed4bad93e, SDK 15.201s.
- Added completion case compiled cleanly before Play. Final ALL 41 cases pass, zero failures/skips; job 2d0418d12a054ea4b1cf777858b00116, SDK 15.558s. Existing coin/death/restart/replay/sword/art/telemetry cases preserved. Report test case counts; recorder's NUnit assertion count is unavailable (0), not a meaningful total.
- Fresh physical title-to-victory run:97/97 walker assertions,70 steps,159.57 units; twelve stories/four checkpoints, three coins/zero escrow, no F6. Real movement, pickups, collision, sword hit/refund, contact damage and intentional defeat/restart.
- Four F6 destinations and six physical gate views captured and console read at each; seventeen PNGs total. Called the same DebugTeleport entry point as the key; gate close-ups use Inspector references. No hardware-key injection or debug progression grants. Hardware F6 remains a human check.
- Native console0 errors/0 warnings after compilation, during route, at each view and after final tests. Two initial pre-existing HandleUtility font notices recorded; no missing active-scene text font or loaded TMP text object. They did not recur. No suppression/disabled components.
- MCP TestRunnerService temporarily saves DisableDomainReload; its source was inspected. The one-line persisted option residue was restored to exact starting EditorSettings.asset bytes after tests. No settings policy change; final settings/packages/input diff empty.
- A temporary ngrok outage held the walker safely at checkpoint 0; resumed and excluded the hold from telemetry. Read-only git audit initially used an unscoped whitespace check and encountered unrelated log whitespace; scoped project checks clean. Unrelated pre-existing repository deletions, logs, layout and connection config are untouched/excluded from commits.
- Assets/evidence persisted only after exiting Play. Scene clean, Edit Mode unpaused. Final scope audit and closing commit record only docs/evidence/test changes.

### Current timing
Fresh Milestone 0 route, 6 November 2026: the existing editor walker started at the real title and reached victory without F6. It passed 97/97 assertions across 70 steps and 159.57 world units. Active telemetry was 106.024 seconds (1:46.02); text advanced immediately, giving about 0.04 seconds recorded reading time. One death/restart was deliberate test input.

| Current checkpoint interval | Active time | Deaths |
| --- | ---: | ---: |
| Landing to Reedbank | 29.76 s | 0 |
| Reedbank to Archive | 27.76 s | 1, deliberate |
| Archive to Haven | 44.90 s | 0 |
| Haven to reunion | 3.60 s | 0 |
| Total | 106.02 s | 1 |

These are the current prototype's four intervals, not five campaign chapter timings. Simulation/checkpoint holds, including the MCP outage, are excluded; this is not wall time or a first-time human result. The five proposed chapters have no measured time yet. Time them as they are built, then validate the 25–32 minute campaign target with a human run.

### Known limits / next run
No known gameplay blocker in tested Editor states. Current short prototype is unchanged; the five campaign chapters and all new gameplay are plans only. Human reading/learning, hardware input feel, other aspect ratios, standalone builds and audio balance remain manual checks. Initial Editor font notices remain a diagnostic if they recur. Actor sources are not stylistically uniform.

Next recommended bounded run: Milestone 1, story/progression data and an isolated campaign blockout. Do not start M2–M7 early. Ask before package/settings/build-scene edits.

## Retained earlier history
The following entries are historical checkpoints. Their pending states are superseded by the complete current-run status above and their corresponding delivery reports.

## Bell, letter and mailbox repair — 6 November 2026
Status: COMPLETE and verified. New matching bell/letter/lantern art; six return mailboxes fixed to use full posts. 40/40 PlayMode cases (38 before), 97/97 no-F6 route assertions, 0 errors/warnings. All 59 colliders and serialized gameplay unchanged; all runtime sources unchanged. Fifteen captures, full route, replay and final scene audit complete. Saved in Edit Mode, unpaused. Current report: PROP_FINISH_REPORT.md; evidence: Data/PropFix20261106.

## Mailbox/signposts and sword — 6 November 2026
Status: COMPLETE and verified. 38/38 PlayMode cases (23 before), 97/97 no-F6 assertions (89 before), 0 errors/warnings. Twelve stories; all checkpoint/F6 captures, sword hit/refund, death/restart and replay verified. One Weapon/equipment HUD, 58 unchanged old colliders plus one pickup trigger, zero missing scripts. Scene saved in Edit Mode, unpaused. Report: SWORD_AND_PROPS_REPORT.md; evidence: Data/SwordPass20261106.

## Outdoor art and character names — 6 November 2026
Status: COMPLETE and verified. The scene is saved in Edit Mode, unpaused. Current report: OUTDOOR_ART_REPORT.md. Aiken and Duckduckgoose replace the former story names in all live content; outdoor art reuses the existing project sheets. Baseline/final PlayMode cases: 21/21 to 23/23. Existing walker assertions: 89/89 to 89/89, complete no-F6 route. Final console: 0 errors, 0 warnings. Evidence: Data/OutdoorArt20261106.

## Previous delivery
Iteration 1 is complete and verified. Assets/_Agent/Scenes/Marshlight.unity is saved in Edit Mode, unpaused. The broader art/animation/audio pass and incoming-art import remain unimplemented. The later user-authorized mailbox/sword extension is recorded above. ITERATION_1_REPORT.md contains the current report and numbered human checklist; FINAL_REPORT.md is retained as the original level's delivery record.

## Iteration 1 milestones
- [x] Resume: reread DESIGN.md, PROGRESS.md and BACKLOG.md; verified the live MCP session and saved fixes after the connection returned.
- [x] Baseline: reran all existing checks before changes — 66/66 walker assertions, 0 errors, 0 warnings. No Unity Test Framework test assemblies existed.
- [x] Design: wrote the Old Archive exchange, optional page, telemetry and regression plan into DESIGN.md before building.
- [x] Core/content: three split-weight loans and returns, prerequisite exit shutter, persistent latched gates/escrow, final bell/win prerequisite and optional ledger pocket.
- [x] Story: two entries added; nine originals preserved; eleven total. No new named character.
- [x] Telemetry: editor/development-only F7 overlay; local JSONL active/reading time and deaths per checkpoint section; title/pause/defeat waiting excluded; F6 runs marked teleported.
- [x] Permanent regressions: 21 PlayMode cases, all passing, 0 failed, 0 skipped. Full final rerun job da142b9f319b43cabb7b619b675f0a99; SDK duration 3.867 seconds.
- [x] Full route: title-to-victory mechanical walk with no F6 — 89/89 assertions, 159.57 world units, all eleven entries, all four checkpoints, exactly three coins at victory.
- [x] F6 verification: all four destinations visited, screens captured and console checked at each. No progression granted by teleport.
- [x] Cleanup/save: 22 nodes, one relay, no duplicate managers/UI, zero missing scripts, movement 1.6 and legacy Input Manager retained. Scene saved; final console 0 errors, 0 warnings.

## Iteration 1 changes to pre-existing scripts / public API
All 24 pre-iteration sources were backed up in Data/Iteration1/BaselineSource before edits. Final SHA-256 audit: seven targeted modifications, seventeen byte-for-byte unchanged. No script deleted or wholesale replaced; no existing public method removed or renamed.
1. Assets/Scripts/MonoBehaviours/Inventory/Inventory.cs: added public TryAddItems(Item,int), which preflights capacity and credits the full return or changes nothing. Existing AddItem, Count and TrySpend retained.
2. Assets/Scripts/MonoBehaviours/Player.cs: RespawnAt synchronizes Rigidbody2D position/rotation as well as Transform and clears velocities. No public API change.
3. Assets/Scripts/MonoBehaviours/DuckFollower.cs: TeleportTo synchronizes visible Transform immediately and clears body velocity. No public API change.
4. Assets/_Agent/Scripts/MarshNode.cs: appended RelayLoan and RelayReturn to MarshNodeKind without changing existing ordinal values; added public Cost, DisplayName and OutstandingCoins getters. Delegates new node roles to the relay, commits atomic original returns, records coin actions, and requires a completed exchange before final-bell payment.
5. Assets/_Agent/Scripts/MarshLevel.cs: added serialized coinRelay and coinPickups references; public CoinRelay, CoinsInEscrow and RecoverableCoins getters, AssertCoinConservation() and RecordCoinAction(string,int,string). Added development-only public Pacing getter and telemetry lifecycle hooks. Updated objectives and final-bell/ending requirements. Existing level/menu/death methods retained.
6. Assets/_Agent/Scripts/Editor/MarshVerification.cs: retained all baseline assertions and public entry points, extended the route through every new interaction and story; one real player-frame step per Editor update, with simulation-time timeouts.
7. Assets/_Agent/Scripts/Editor/MarshBuilder.cs: presentation repair validates all nine original story IDs while permitting added entries. No public API change.

Original movement, Character/health, pickups, health bar, slots, managers and patrol scripts are unchanged during Iteration 1. The original scene and source sprite/prefab assets remain available.

## New implementation
- Scripts/MarshCoinRelay.cs: serialized three-loan dependency with refundable 2/1/2 weights, idempotent deposits/returns, objective/prompt feedback and explicit reference validation.
- Scripts/MarshPacingTelemetry.cs: entire file conditional on UNITY_EDITOR || DEVELOPMENT_BUILD. Created only by guarded code, never serialized into the scene. F7 uses legacy input. Logs to Application.persistentDataPath/Marshlight/Telemetry/pacing-<UTC>-<run-id>.jsonl.
- Scripts/Editor/MarshIteration1Builder.cs: additive edit-mode builder, existing instance checks and serialized references. Exchange is grouped under 09_ArchiveExchange.
- Tests/PlayMode/MarshCoinRegressionTests.cs + asmdef: permanent Unity Test Framework coverage using a reflection bridge to Assembly-CSharp, without moving or replacing original scripts.
- Tests/Editor/MarshTestResultRecorder.cs + asmdef: persists SDK results outside Assets during Play Mode; evidence copied into Assets after stopping.
- Story asset expanded to eleven entries while retaining its GUID. Exactly two new entries: exchange (80 words, two pages) and archive-pocket (41 words, one page). Reused existing placeholder art/materials/fonts; no downloads or new audio/art pass.

## Coin-state coverage and fixes
Original loans through death (three cases); loans at every checkpoint (four); three relay loan phases through death; both initial cup orders; both spill-return orders; premature exit rejection; duplicate payment/return rejection; full-inventory atomic return; replay after victory; patrol-removal/no-bypass; telemetry timing/deaths/pause/overlay; teleport exclusion. Every tested loan state can recover all escrow and finish with exactly three coins.
The full walker additionally drives actual MovementController, real colliders, pickup/checkpoint triggers, native enemy contact, deliberate death, restart, story interactions and victory. The fixtures arrange edge-case states by runtime positioning; route reachability is verified separately by the no-F6 walker.

The new regressions found a real respawn defect: Transform-only player placement could leave the Rigidbody at a distant death position. Pip's body could teleport while his visible Transform remained stale. Both paths now synchronize immediately. Early exit payment was blocked by design to avoid stranding the pair. Atomic inventory credit prevents partial refunds or a consumed-but-uncredited tray.
QA corrections: removed duplicate explicit test-runner assembly references before the successful compile. Batched Editor frame stepping caused engine UI temporary-allocation warnings; the walker now steps one frame per Editor update. Final clean F6 captures, full walk and complete test rerun have 0 native errors and 0 warnings.

## Pacing analysis and playtest decisions
Human baseline: approximately 240 seconds, reported by a player already familiar with the basic coin mechanic. Matched walker capture-point times:
| Section | Baseline | Iteration 1 | Delta |
| --- | ---: | ---: | ---: |
| Landing to Reedbank | 29.6 s | 29.6 s | 0.0 s |
| Reedbank to Archive | 27.8 s | 27.8 s | 0.0 s |
| Archive to Haven landing | 27.6 s | 44.9 s | +17.3 s |
| Haven landing to reunion | 2.9 s | 2.9 s | 0.0 s |
| Full run | 88.0 s | 105.3 s | +17.3 s |

Telemetry's actual checkpoint-trigger intervals are 29.1 / 27.8 / 44.9 / 3.6 active seconds, reading 0.02 seconds, deaths 0 / 1 / 0 / 0. The section-two death is intentional test input. Screenshot holds occur shortly after checkpoint triggers, causing the small first/last section allocation difference. Mechanical run 20261103-212924-3a33f40e is labeled mechanical, teleported false. Its original JSONL remains in the persistent data directory and is copied to Data/Iteration1/MechanicalPacing.jsonl.

Model: +17.3 measured route seconds; +30–40 seconds to read 121 new words at 180–240 wpm; +10–15 seconds for seven new E uses and three page advances; +15–35 seconds reasoning about remote gates and re-lending. Estimated +73–108 seconds, bringing the reported four-minute baseline to 5.2–5.8 minutes with the optional page. That page/detour contributes roughly 17–21 seconds. A practiced player skipping it could remain near or below five minutes. A first-time run now likely falls within the 5–10 minute target, but this is a model and requires a fresh human timed run.

No movement slowdown, longer empty corridor, wait timer, filler story, or extra cost at the three original bells. The new coin dependency and optional page address the pacing notes. No new patrol encounter was added: the preferred coin challenge supplies the added decision time and will remain relevant with a patrol-affecting tool. The final patrol track was moved clear of the latch; existing patrol behavior is preserved. Further patrol design is deferred.

## Evidence and final checks
Data/Iteration1/: BaselineTests.json, BaselineSource/, BaselineScene.unity.txt, FullRoute.json, PlayModeResults.json, PlayModeRecorder.json, MechanicalPacing.jsonl, PacingSnapshot.json, PacingAnalysis.json, SourceAudit.json, SceneAudit.json, ReleaseExclusion.json, ConsoleSummary.json, I1_Final_F6_0–3.png, all I1_Walk checkpoint/defeat/win images and I1_Exchange_Walk.png.
The SDK assertion-count field is zero for these NUnit cases; report test-case counts separately. The reliable before/after assertion count is the walker: 66 to 89.
Release-symbol compilation has no telemetry type and no diagnostics; the scene contains no serialized telemetry script reference. No standalone release build was produced.
No known gameplay blocker in tested states. Human pace, keyboard feel, extra aspect ratios and standalone player remain untested. Placeholder art and no sound remain intentional until Iteration 2.

## Original delivery archive
The following pre-Iteration-1 record is retained for continuity. Current results and limits are recorded above.

## Milestones
- [x] 1. Explore — all 16 Assets C# scripts read; sprites, prefabs, scene, packages and input inventoried.
- [x] 2. Design — DESIGN.md version 1 written before implementation.
- [x] 3. Core loop
- [x] 4. Level
- [x] 5. Story
- [x] 6. UI and feel
- [x] 7. Verification
- [x] 8. Polish and cleanup — saved, audited and verified

## Findings
Unity 6000.3.9f1; project Duck Game MCP. Original scene Assets/Scenes/LevelOne.unity.
Legacy Input Manager only (activeInputHandler 0); Input System package absent.
16 scripts, 10 prefab GameObjects, 1 scene, 16 textures, 478 tile assets, 14 animations, 5 animator controllers, 6 fonts; no AudioClip assets.
Packages include Cinemachine 3.1.5, 2D feature 2.0.2, Test Framework 1.6.0, UGUI 2.0.0 and Unity MCP.
Existing hierarchy has one Player, DuckFollower, RPGGameManager, RPGCameraManager, HealthBar; zero Inventory scene instances and zero EventSystems.
Baseline console: Unity emits an Input Manager deprecation notice at Editor startup. Keep the requested legacy input; record separately from runtime verification.

## Existing defects identified
- Player.TakeDamage calls inherited KillCharacter, which destroys the GameObject; private Die is never reached.
- Scene reference fields were audited and explicitly bound: manager player/spawn/camera, duck follow target, health UI meter/text/character, and Player UI references. All required bindings were validated after serialization.
- Inventory.CreateSlots uses GetChild indices; replace those lookups with serialized or explicit named references while preserving behavior.
- Duck is also tagged Player; use explicit Player component reference to prevent ambiguous lookup.

## Decisions
Build Assets/_Agent/Scenes/Marshlight.unity from a saved copy of LevelOne. Keep the original scene and original asset files. Reuse existing character scripts/assets and singleton managers. No downloads and no duplicate UI/managers. New visuals are editable placeholders.
All work and verification evidence remain in the git-backed project.

## Pre-existing script changes / public API
Six existing scripts extended with targeted changes:
- Player.cs: PlayerInventory getter, Died/Respawned events, KillCharacter override; optional serialized sceneInventory/sceneHealthBar. Startup checks existing UI before instantiating, preventing stale duplicates. Existing pickup/heal/damage methods retained.
- MovementController.cs: InputEnabled getter and SetInputEnabled(bool); development-only SetDebugInput(Vector2,bool). Existing movement/animation preserved; modal text and menus suppress input explicitly.
- Inventory.cs: Count(ItemType), TrySpend(ItemType,int); serialized slotsRoot, idempotent slot creation. Existing AddItem/stacking retained. Removed GetChild index lookups in favor of named/serialized references.
- Slot.cs: itemImage reference added; existing qtyText retained.
- DuckFollower.cs: SnapToPlayer() resets follow history on respawn/teleport; original follow loop retained.
- HealthBar.cs: text now shows actual current / maximum HP instead of a percentage.
No existing public method was removed or renamed. All 16 baseline sources preserved in Data/BaselineScripts.json.

## Known bugs / remaining work
Final mechanical run passed 66 assertions over 133.65 world units of actual movement, including all three bells, all four landings and all nine story entries. Native Unity console counters after the run and extra tests: 0 errors, 0 warnings. Win-menu replay reset bells/coins to zero with one Player, Inventory and HealthBar.
Polish complete: visible inventory with readable coin icon and quantity; responsive HUD; clean full-screen menus; final-tray objective and win requirement. Native enemy-contact damage and real gate collider checks passed.
QA issues corrected: story import used the wrong JSON API; prefab CanvasScaler overrides and movement speed needed explicit recording; unfocused remote Editor needs explicit player-frame stepping. One preliminary verifier failure occurred before physics advanced and was corrected by driving actual Unity frames. The initial MCP/native screenshot path produced two Metal memoryless-depth errors; replaced with an editor-only stored-depth capture path. The clean gameplay run has no such errors.
Placeholder art is intentional. Human pacing/keyboard feel and audio remain unmeasured; no audio assets exist in the source project.

## First verification evidence
Data/Verification/FirstPass.json, FirstPassFreshRun.json and checkpoint/title/defeat/win PNGs. Main route used no teleport or progression grants. A temporary one-coin debit/refund tested insufficient funds at bell 1 without adding net currency. All four development teleport destinations and duck placement passed; teleport granted no bell, coin, story or late-checkpoint progress. Serialized primary-button binding, pause/resume and defeat restart actions passed.

## Final delivery
- Open Assets/_Agent/Scenes/Marshlight.unity and press Play. Scene is saved and left in Edit Mode, unpaused.
- Marshlight is first in the build scene order; the original LevelOne scene remains available as a second entry.
- DESIGN.md, FINAL_REPORT.md and Data/EXPLORATION.md describe design, controls, asset inventory, validation and human playtest steps.
- Data/Verification/FinalPass.json contains all 66 passing assertions. ExtraVerification.json covers replay, menu binding and development teleport. ConsoleSummary.json records zero errors and zero warnings. SourceAudit.json proves ten baseline scripts unchanged and exactly six targeted extensions.
- Final checkpoint screenshots plus title, defeat and victory screenshots are in Data/Verification/Final_*.png. First-pass evidence is retained separately.
- No gameplay blocker found in the mechanical Editor tests. Placeholder art and lack of source audio are intentional. First human playthrough pacing (target 5–10 minutes), keyboard feel and additional aspect ratios still need the manual checklist. No standalone build was produced.
- Runtime health and coin display data use new Assets/_Agent/Data assets; original source assets and original LevelOne terrain are preserved.
- The copied scene removed the old test Grid and the unconfigured WheatObject, then organized Actors, Services, Cameras, Marshlight_World and existing UI. No baseline script was deleted.
- A prefab-variant UI reparent was rejected by Unity during polish; retained the inherited hierarchy and corrected its anchors instead. The clean final run includes this fix.

## Backlog
See BACKLOG.md. Suggested next: Iteration 2 manifest/palette/procedural art and audio; Iteration 3 coin-borrowing keeper tool; Iteration 4 validated incoming art/audio swap.

### Outdoor pass implementation checkpoint
Applied 525 renderer replacements and 531 decorative renderers using original outdoor source textures. Added eleven assembled Sprite assets under Sprites/OutdoorReuse. ArtAudit.json verifies all 58 colliders unchanged and source PNG/importer files untouched. Character names updated in eleven story entries, runtime menus/objectives and both story builders; no public gameplay API changes. Added two PlayMode cases covering visual-only dressing and renamed text/panel fit (23 total pending verification). Existing scripts modified this pass: MarshLevel.cs, MarshBuilder.cs and MarshIteration1Builder.cs for text only; MarshCoinRegressionTests.cs extended.
QA: compilation completed with zero errors. Unity MCP emitted one WebSocket-not-initialised warning during domain reload; the connection recovered and subsequent commands succeeded. This is recorded before clearing the console for the clean verification run.

### Outdoor pass test checkpoint
Final complete PlayMode rerun after ground refinement: 23/23 cases passed, 0 failed, 0 skipped (job e2e08c18ee6446d4a469836504f765a8). All eleven dialogue entries, title, defeat and victory use Aiken/Duckduckgoose and fit their existing panels. The second visual pass uses complete two-by-two grass patches and a darker dusk tint; its separate before/after collision signatures also match. Remaining: final F6 captures, full no-F6 mechanical run, console audit and delivery report.

### Outdoor pass final milestones
- [x] Reused six original outdoor sheets; thirteen assembled Sprite assets at 32 PPU, original PNGs/importers/slicing preserved. 525 renderer replacements and 531 grouped decorative renderers.
- [x] Aiken/Duckduckgoose in all eleven entries, speakers, menus, objectives and rebuild strings; Ada and every story ID retained. Three production/builder files compare exactly to the old source with only the requested name replacements.
- [x] All 58 original colliders unchanged; main pass and refinement before/after signatures match. Art has 0 colliders and 0 runtime behaviours. Movement remains 1.6, legacy input unchanged.
- [x] Final PlayMode suite: 23 passed, 0 failed/skipped. Two added public test cases: OutdoorArtReusesProjectSheetsWithoutAddingCollisionOrLogic and RenamedStoryAndMenusUseAikenAndDuckduckgooseAndFitPanels. All pages/speakers and title/defeat/victory text fit their panels. No existing public gameplay API changed.
- [x] All four F6 destinations captured/read: 0 errors/warnings at every view, no progression grant.
- [x] Full no-F6 title-to-victory walker: 89 assertions, 159.57 world units, 105.31 simulated active seconds, eleven stories, all four checkpoints, three coins at victory, 0 errors/warnings. Reading is skipped by the walker; no new human pacing claim.
- [x] Final title, intro, defeat, win and north/south bank detail captures reviewed. Ended in Edit Mode, unpaused; no missing scripts or duplicate managers/UI. Temporary test interaction preferences restored; queued repaint helpers self-remove at capture/end.

Source audit: 25/29 pre-pass scripts byte-for-byte unchanged. Three existing production/builder scripts contain name replacements only; the fourth changed file extends the regression fixture. One new edit-time builder, MarshOutdoorArt.cs. No assets downloaded, original LevelOne retained, no audio/weapon/animation scope added. The full report lists limits and a numbered human checklist.

### Sword source milestone / existing public API changes
Staged and installed ten source files (five existing extensions, five new components/builders). Item.ItemType appends SWORD=2 (COIN=0/HEALTH=1 retained). Player adds public ItemCollected(Item) event after successful pickup and routes SWORD through existing AddItem. MarshLevel adds a serialized Weapon reference/public Weapon getter, counts weapon escrow in CoinsInEscrow, cancels/refunds on modal transitions/replay, and includes weapon controls/status in HUD. No public method removed or renamed. EnemyHitPoints, patrol, movement, inventory and original health logic remain intact. Existing test fixture extended by fifteen sword cases and twelve-entry story expectation; existing walker extended by real pickup/hit/refund checks and a swipe capture. New files: WeaponData, Weapon, MarshEquipmentHUD, MarshEnemyFeedback, Editor/MarshSwordBuilder. Compilation and scene binding pending; do not enter Play until complete.

### Sword build milestone
Compilation completed with 0 errors/warnings. Scene built and saved: twelve story entries, eleven upgraded mailbox/sign/return-box props, one Weapon and one equipment HUD. All 58 previous colliders unchanged; the only new collider is the sword pickup trigger. Existing EnemyHitPoints and patrol logic retained; hit flash/HP pips are separate presentation components. Three prepared PNGs use a sixteen-entry palette including transparency, Point/no-compression/32 PPU; return boxes use a Sprite region of the mailbox texture. Sword and pickup prefabs, WeaponData, inventory item and synthesized swish saved under Assets/_Agent. All-test and physical-route verification pending.

Sword pass verification milestone: all 38 PlayMode cases pass (23 preserved + 15 new), zero failures/skips; SDK duration 16.479 seconds. Initial source compile was clean. Test runner reported its asset-cleanup warning for three builder audit files written before the run but not yet imported; import and rerun are scheduled. Play-through and capture verification in progress.

### Sword pass final milestones
- [x] Resume/baseline: all thirty scripts and persistent records read; hierarchy searched before additions; 23/23 baseline cases passed; legacy input retained.
- [x] Design before build: mailbox/signposts, one inventory-owned coin-borrowing sword, equipment panel and Ada's new note; twelve stories total.
- [x] Art/build: three palette-limited pixel PNGs, a mailbox-head Sprite, eleven upgraded props, two prefabs, WeaponData/item and a synthesized swish; all 58 original colliders unchanged plus one pickup trigger.
- [x] Core: actual existing EnemyHitPoints damage/death, one hit per enemy per swipe, facing/range/wall protection, cooldown, temporary visibility, HP pips/flash/particles and equip/pack UI.
- [x] Coin safety: insufficient funds, pause/dialogue/death/unequip refunds, blocked-refund recovery, replay reset and no crossing bypass verified.
- [x] Final complete PlayMode suite: 38 passed, 0 failed/skipped; job 5117a567170e48c398358895cbc840d0; SDK 16.270 s. Fifteen added cases, twenty-three retained. SDK assertion field remains zero; report case totals separately.
- [x] Full physical no-F6 route: 97 assertions (89 before), 70 steps, 159.57 world units, all twelve stories/four checkpoints, real sword pickup/hit/refund, death/restart and victory with exactly three coins, zero escrow.
- [x] Eighteen screenshots reviewed, including normal checkpoints, active swipe, defeat/win, all four F6 destinations, mailbox/signpost, both new pages, pause and a separate arranged combat showcase. Native console 0 errors/0 warnings. Fresh replay resets sword/escrow/bells and four full-health enemies; debug teleports grant no progress.
- [x] Final scene/source audit: 25/30 pre-pass scripts unchanged; five targeted extensions and five new scripts. Existing enemy health, inventory, movement, patrol and base health sources unchanged. One Weapon/equipment HUD, one of each existing manager/player/duck/inventory/health UI, one AudioListener; 59 colliders, zero missing scripts.
- [x] DESIGN/PROGRESS/BACKLOG/ASSETS_NEEDED/report and evidence saved. Scene left clean in Edit Mode, unpaused.

Public API additions are recorded in the sword source milestone; none removed/renamed. The one-off builder now refreshes its diagnostic files before testing, resolving the initial asset-cleanup warning; final reruns are clean.

Like-for-like pacing telemetry: Landing 29.48 -> 29.74 s (+0.25), Reedbank 27.76 unchanged, Archive 44.90 unchanged, Haven 3.60 unchanged; total 105.74 -> 106.00 active seconds. Route length unchanged. The extra Ada note has 78 words, estimated 20–26 s reading at 180–240 wpm; the walker skips text. The earlier 5–10 minute first-time pacing hypothesis still needs a timed human run. Section-two death is intentional verification input.

No known gameplay blocker in tested Editor states. Hardware input feel, sound balance, extra aspect ratios and standalone build remain untested. Broader character/mechanism art/animation, sound/master volume, incoming-art import and shields/variants remain backlog. Current report SWORD_AND_PROPS_REPORT.md includes the numbered manual checklist and suggested next three items. Evidence: Data/SwordPass20261106, including final tests, recorder, full route, telemetry, replay/F6/source/scene/collider audits, eighteen images and zero-error/warning console summary.

## Prop follow-up milestone — 6 November 2026
Baseline all 38 PlayMode cases pass, zero failures/skips (job d96437e011f1496ab4f1454ce482711e, SDK 14.662 s). Read current design/progress/backlog/manifest and all 35 sources. Saved source/scene/document backups under Data/PropFix20261106. Root cause: six return props use a postless head crop; first mailbox uses full art. Wrote the bell/letter/full-mailbox/checkpoint-lantern design before building. Existing systems and 59 colliders to be preserved. Art/build/verification pending.

### Prop follow-up build milestone
Compiled with 0 errors/warnings. Applied and saved seven complete posted mailboxes, three bell frames, one field letter and ten lanterns. All 59 collider fingerprints and every serialized gameplay component/marker identity match before/after. Return markers sit on mailbox doors; relay return labels moved above the taller sprites. Original first mailbox is unchanged except its decorative letter. One new editor helper; MarshSwordBuilder private placement fixed to prevent the old cropped-head bug on rebuild; test fixture adds two visual/interaction regressions. No existing public API changed and no runtime gameplay source changed. QA: initial builder guard checked read-story count instead of authored-entry count and stopped before scene mutation; corrected to the existing story asset Entries.Length. Compilation stayed clean. Forty-case suite and route/captures pending.

### Prop follow-up test milestone
All 40 PlayMode cases passed (38 preserved + 2 new), zero failed/skipped, SDK 14.865 s; native console 0 errors/warnings. Scene saved; Play Mode art/capture and full-route verification next.

### Prop follow-up route milestone
Full no-F6 title-to-victory walker passed 97/97 assertions over 159.57 world units, all twelve stories and all four checkpoints, three coins at victory and no escrow. F6 views and all six mailboxes visually checked. Fifteen screenshots, telemetry, route and replay audits saved after leaving Play Mode. Native console 0 errors/warnings. Fresh replay resets inventory/coins/loans/enemy health and retains all six complete posted mailbox sprites. Final all-test rerun and report pending.

### Prop follow-up final milestones
- [x] Read persistent records and all 35 baseline sources; baseline 38/38 PlayMode cases passed. Design written before building; source/scene/document backups saved.
- [x] Generated and prepared BellFrame 32x48, FieldLetter 32x32 and MarshLantern 32x48 using the existing sixteen-entry palette, binary alpha, Point/no compression/32 PPU. Exact prompts/preparation data archived.
- [x] Seven full posted mailboxes, three pixel bell frames, one paper/seal letter, four checkpoint lanterns and six haven lanterns. First mailbox body unchanged; six return boxes now share its complete sprite/base, door coin markers and clear raised relay labels. Legacy head-crop asset has zero live renderer references.
- [x] All 59 collider fingerprints and every serialized gameplay component/marker identity unchanged. 33/35 prior scripts unchanged; all runtime sources unchanged. Only MarshSwordBuilder private visual placement and the regression fixture extended; one new editor-only MarshPropFinish helper. No existing public API changed.
- [x] Final ALL-tests run: 40 passed, 0 failed/skipped, job cb071d0588434262bd0e4bb358b529ee, SDK 14.883 s. Two new art/interaction regressions cover posts, base alignment, coin visibility, return labels, letter/bell interactions and live checkpoint light.
- [x] Full physical no-F6 title-to-victory run: 97/97 assertions, 70 steps, 159.57 world units, twelve stories/four checkpoints, three recovered coins and zero escrow. Real movement, triggers, sword hit/refund, enemy contact, death/restart and all puzzle dependencies retained.
- [x] All four F6 views and each normal checkpoint captured/console read; all six returns inspected, including three archive close-ups. Fifteen images saved. Fresh replay resets ownership/coins/loans/enemy health and retains all six complete mailbox sprites.
- [x] Native console 0 errors/0 warnings; zero missing scripts; one of each existing manager/UI/player/duck/weapon/equipment HUD. Scene clean, saved in Edit Mode, unpaused.
- [x] DESIGN, PROGRESS, BACKLOG, ASSETS_NEEDED and PROP_FINISH_REPORT updated; evidence under Data/PropFix20261106.

Pacing unchanged by content: mechanical active telemetry 106.01 seconds, sections 29.75 / 27.76 / 44.90 / 3.60; same 159.57-unit route. Dialogue is advanced immediately and the long held checkpoint during chat interruption is excluded. This is not a human timing result and adds no new pacing claim.

No known gameplay blocker in tested Editor states. Other aspect ratios, hardware input feel, audio balance, human pacing and a standalone build remain manual checks. Broader character animation, relay-cup art and sound/master volume remain backlog. The initial editor guard used read-story count rather than authored-entry count and stopped before scene mutation; corrected before the successful build. Project compilation and final test/route runs are clean. Current report: PROP_FINISH_REPORT.md, including numbered human checks and top three backlog suggestions.

## Campaign Milestone 0 — analysis checkpoint
RUN TARGET Milestone 0 only. Started from git 229d2d757710c14bd9a008a10550b037db0d582a (scoped Assets/Packages/ProjectSettings checkpoint of the existing playable work; local logs/layout/connection config excluded). Read all five requested persistent paths (root CHANGELOG missing), all 36 sources, 14 prefab assets and both scene dependencies/component wiring. Baseline 40/40 tests pass. Actual game is top-down XY, not side-view: three main bells plus three relay gates, four checkpoints, four patrols, twelve stories. Preserve current controller/input; no viewpoint conversion in this run. Existing development-only F6 already cycles checkpoints; retain and verify it. Art audit recorded before any art work; no art is created this milestone. Two initial Editor HandleUtility font notices recorded; no missing scene/world text fonts or loaded TMP objects; fresh baseline tests clean. Full timed route, five-chapter/four-act design, milestone plan, basic completion regression, final tests and end commit remain.


## Milestone 1 — COMPLETE, 6 November 2026
Latest scope is M1–M4 without further approval checkpoints, no git commits. M1 complete; M2 complete; M3 active. Human M0 manual checks passed; campaign human timing is not supplied.

- Separate Scenes/Marshlight_Campaign.unity, now the first enabled build scene; original Marshlight.unity verified byte-for-byte unchanged. Existing UI/managers/player/duck/inventory/sword/health reused. Five main bells, six checkpoints, retained physical archive relay, five ChapterData/SectionData assets and one CampaignData.
- Campaign story copy has16 entries: all12 IDs retained plus perch-intro, reedbank-bell, lantern-intro and sluice-warning. All ActI/II bindings placed. Perch/lamp lessons are capability-gated until M4; fourteen currently available stories fire in the route.
- Existing script changes: MarshStory.Entry adds oncePerRun/requiredCapability/prerequisites and ValidateData(); MarshLevel adds Campaign/Interactions/RequiredBells/PacingSectionNames, RequiresArchiveExchange(int), CanReadStory(string), TryShowStory(string); retains ShowStory and all prior public methods. Optional campaign mode generalizes counts/HUD/win/checkpoint validation; legacy three-bell/four-checkpoint defaults remain. MarshNode queries the configured archive requirement and story availability; no public API removed. MarshPacingTelemetry reads level-provided section names and sizes its overlay; API unchanged.
- New ChapterData, SectionData, CampaignData, MarshCampaign configuration component, MarshInteraction, MarshStoryTrigger; editor builder and campaign walker; separate regression fixture. No duplicate managers/UI. No packages/input change. Build settings intentionally add campaign first under the new authorization.
- Baseline41/41; M1 final56/56, zero failures/skips, jobcd1710101e25488eb6fc8dbbde99fc4d, SDK17.990s. All six checkpoint loan/death recoveries, replay, ordered gates and story policies pass; original41 preserved.
- Full physical title-to-ending run without F6:75/75 checks,70steps; five paid gates, twelve original plus two active new stories, three coins/zero escrow. One intentional death while coins lent. Six normal checkpoint/win captures plus six F6 destination/eight gate views:21PNGs. Native console0errors/0warnings at all views/final tests.
- Active chapter seconds: Keeper's Landing 27.98; Listening Reeds 18.40; The Old Archive 52.92; Storm Locks 22.28; Lantern Haven 26.48. Total148.06s, immediate text, holds excluded. First walk used editor frame stepping; subsequent walker uses the focused live PlayerLoop at normal game clock, avoiding idle-editor throttling. Captures pause the Editor so holds cannot inflate telemetry. Respawn relocation is excluded from subsequent distance totals.
- Corrected opening objective (collect original coins before talking about refunds), next-bell objective after refunds and bridge visual footprint to match the dry corridor. Final56-case rerun after corrections clean. The full collision/story route preceded these presentation-only corrections.
- Known limits: intermediate campaign scaffold; new enemies/mechanics not yet built in M1, bosses/climax/resolution/set pieces remain M5/M6. Final25–32minute target unmeasured. Actor art mismatch documented in ART_ADAPTER.md; no character reslicing/art changes. Initial asset styles reused, no downloads. Temporary ngrok interruption during debug inspection recovered; completed all remaining captures.
- Saved scene clean in Edit Mode. Evidence:Data/CampaignRuns/M1. No git command or commit made in this run.

M2 implementation checkpoint: compiled cleanly, scene saved with5Skitter/4Crab and1section-configured original patrol; ALL67/67PlayMode tests pass (job48b7306e11f84084a798ebf876ffe4f9,24.935s). Source extensions: Player.TryTakeEnemyDamage(int)/InvulnerabilityRemaining and shared contact cooldown; EnemyHitPoints.ConfigureForEncounter(int), Damaged/Died events and full collision/physics reset; MarshPatrol optional section/encounter plus Definition; MarshEnemyFeedback event-driven shader flash, scalable pips, ShowGuard()/ResetFeedback(); Weapon routes hits through EnemyAgent guard and relies on HP feedback event; SectionData.Encounter/GetEncounter(); editor walker uses normal focused PlayerLoop with route-planning waypoints. No prior public method removed. Permanent tests cover telegraph lock, real hit/protection, sword/flash/knockback, crab guard/flank/recovery, reset, death/loan recovery, no-bypass and pause. Physical route/debug captures pending; M3 not started. No git actions.

M2 QA correction: first live-loop route passed75/75, but unscaled telemetry included Editor capture-pause gaps on resumed frames (raw277.2s versus158s game elapsed). Preserved that diagnostic under M2/BeforeClockFix. MarshPacingTelemetry now records clock mode, exposes SimulationClock/SetSimulationClock(bool), excludes zero-timescale capture frames and subtracts Editor pause spans from the human clock. Campaign walker explicitly uses simulation seconds. Added permanent hold-time regression: ALL68/68pass, job34af5c7d577542359dc160ccd538785e,25.229s. Enlarged enemy tell/name labels and moved the chapter4crab clear of the promise sign through section data. Corrected full timing rerun pending; M3 not started.


## Milestone 2 — COMPLETE, 6 November 2026
- Shared EnemyData/EnemyAgent with explicit section encounters; Reed Skitter and guarded Silt Crab placed with safe approach lanes. One original MarshPatrol remains configured from SectionData. Existing EnemyHitPoints, Weapon, Player and feedback extended; no replacement of health/inventory/movement.
- Final ALL PlayMode:68 passed,0 failed/skipped (M1:56), job34af5c7d577542359dc160ccd538785e,25.229s. Includes actual attack/sword/guard/knockback/reset, shared damage immunity, unpaid-gate protection and frozen telemetry-clock regression.
- Corrected live no-F6 route:75/75 checks,74 steps,240.04 world units, exactly3 coins/zero escrow at victory. Active simulation157.89s:31.19 /20.62 /57.12 /22.43 /26.53 seconds (last combines Haven/reunion). One deliberate loan/death. Text advanced immediately; no human-duration claim.
- Six debug destinations, all5 main+3 relay gates and each physical checkpoint captured/console-read. Native0 errors/0 warnings throughout. Evidence/screenshots and both the pre-clock-fix diagnostic and corrected timing retained under Data/CampaignRuns/M2. Scene left saved in Edit Mode before M3.
- Timing QA found unscaled screenshot-hold debt in mechanical telemetry. MarshPacingTelemetry adds SimulationClock/SetSimulationClock(bool), clock field and Editor-pause subtraction; CampaignVerification uses scaled simulation time for its physical route. Human mode remains unscaled with Editor pause excluded. A separate2.2s pause probe and automated frozen-frame test were run. No player speed change.
- Additional pre-existing API edits: EnemyHitPoints adds ConfigureForEncounter(int), Damaged/Died events; Player adds TryTakeEnemyDamage(int), InvulnerabilityRemaining; MarshPatrol adds optional Definition; MarshEnemyFeedback adds ShowGuard/ResetFeedback; SectionData adds Encounter[]/GetEncounter. Existing methods retained. Weapon respects EnemyAgent guard and delegates hit feedback through health events.
- Placeholder enemy prefab assemblies reuse the unchanged32px enemy frame and outlined geometric limbs; shader white flash verified. Final animation sheets remain in ASSETS_NEEDED. Counterpart chapter visuals/bosses/art migration remain M5–M7. No git commands or commits.

## Milestone 3 — ACTIVE
Read persistent records and current enemy/section/health/weapon/scene sources. Baseline68/68 passed at M2 closure. Build Wick Moth/Counterweight Sprout only, tune section data, retain all gates and currency ownership. M4 follows without approval checkpoint; stop before M5.


## Milestone 3 — COMPLETE
68 to77 passing PlayMode cases;0 failures/skips. Final clean jobcaec4731403d4c11850218682ae51bea,38.178s. Physical no-F6 route75/75,240.04units;Timings:30.531 /20.761 /56.496 /22.564 /26.417 seconds,156.768 total. Text advanced immediately,1 deliberate death. All6debug destinations and8gates captured/console-read, plus moth/seed views;0native errors/warnings. Interim capture attempted before SDK scene restoration finished, causing Test Framework RestoreSceneSetupTask error; stopped, restored saved campaign, reran ALL77 clean and captured again. Cause fixed by waiting for scene restoration before Play. Intermittent MCP drop recovered without asset loss.

WickMoth/CounterweightSprout/MarshSeedProjectile new; existing EnemyData appends kinds/seed parameters, EnemyAgent adds OwnerLevel/Position and virtual WarningReach; builder extended with additional placeholder prefabs. Existing enemy fixture now derives expected count from section definitions rather than nine hardcoded actors. Data-only curve recorded in DESIGN. Three Moths/two Sprouts replace three scaffold M2 actors,12 total enemies including retained original mite. New types use HP/damage protection/terrain checks; owner death/reset clears seeds; one live seed per Sprout. No currency drops or gate bypass. Presentation-only prefab layer correction gives outlined grey wings and visible brass pod/seed. Baseline scene unchanged; no commits.

## Milestone 4 — ACTIVE
All persistent files reread. Baseline77/77. Implement perch/lamp/mail routing/eight Reed Marks, section gate definitions, paged journal and coin recovery audit. Stop before M5; no bosses or unrelated scope.


## Milestone 4 — COMPLETE, 6 November 2026
- Seven data-driven station instances:3 Duck Perches,2 refundable Inspection Lamps,2 selectable Return Mail Routes. GateData requires inspections/returns while all5 main crossings still charge3; archive2/1/2 retained. Eight unique Reed Marks use separate journal storage;4 award a HUD/journal BADGE,8 add an optional closing memory. Paged journal reuses the existing menu. Current campaign has17 story entries, all fire in the full route; original baseline scene is byte-for-byte unchanged.
- All98 PlayMode cases pass,0failed/skipped; M0/M1/M2/M3 baselines41/56/68/77. Final job45ffb622b3044711a15d64ab82f79e3f,44.4297s. Added21 cases for new content plus updated fixtures. Existing41 baseline cases retained. Case totals are separate from walker assertions.
- Full live no-F6 title-to-ending route:124/124 assertions,130steps,312.44world units, all8marks/all7mechanics/all17stories,5paid main bridges+3relay gates,6checkpoints,3coins/0escrow at victory. One deliberate checkpoint death with coins lent. Text advances immediately; patrols remain active and movement speed is unchanged. The run takes the optional Storm HIGH route and Haven LOW route.
- Finite resource audit:203reachable states,783transitions,2ending resource states,203recoverable; all11main payment/refund stages, every3-phase relay loan, both4-phase addressed loans and all4lamp-held combinations covered. Actual configured costs: sword1,lamps1,mail2,main3,relay2/1/2. This proves recovery in the abstract resource model; physical collision/duck motion are covered by runtime regressions and the full walker. Cosmetic mark bits are independent. It does not claim a proof of all physics trajectories or arbitrary future definitions.
- Six F6 destinations and all8gates captured/read; physical checkpoint/win images and extra perch/lamp/mail/pause/journal/BADGE views reviewed. Final labels separated, current mail address explicit, journal button fits and pause checkpoint line visible. These last presentation changes preserve the verified physical layout/resource rules. Native0errors/0warnings throughout final route and final visual review. Earlier transient MCP WebSocket warning recovered; no new source warnings remain.
- QA: first resource test expected an arbitrary>1000states although the complete model has203. Replaced that guessed count with explicit stage/phase/combination coverage assertions. A CS0252 fixture reference warning was corrected before Play. Review tightened the safe-exit rule so proximity cannot open a closed shutter; a permanent regression protects it. A named lookup containing slash was corrected to exact TextMesh-name selection before the label edit. No error suppression or disabled components.

| Chapter | M3 active | M4 exploration route | Change |
| --- | ---: | ---: | ---: |
| Keeper's Landing | 30.53s | 32.28s | +1.75s |
| Listening Reeds | 20.76s | 34.36s | +13.60s |
| The Old Archive | 56.50s | 58.71s | +2.22s |
| Storm Locks | 22.56s | 40.64s | +18.07s |
| Lantern Haven | 26.42s | 43.03s | +16.62s |
| Total | 156.77s | 209.02s | +52.25s |

Times are scaled active simulation seconds, excluding screenshot/Editor holds and menus; reading is separately logged. Reeds +13.60s, Storm +18.07s and Haven +16.62s reflect actual new inspections/routes/detours. Archive +2.22s adds lamp/mark stops; Landing +1.75s adds a mark detour. Full human first-time time is not measured. The final25–32minute campaign target has NOT been demonstrated; M5/M6 content and M7 balance remain. Do not pad walking or slow movement to meet it.

### M4 changes to existing contracts
- DuckFollower: additive AssistanceTarget/AtPerch properties, HoldAt(Transform), Recall(); SnapToPlayer now recalls assistance before its preserved physical teleport. Existing following remains the default.
- MarshLevel: adds JournalOpen/JournalPage, ToggleJournal(), ChangeJournalPage(int); existing menu/PrimaryAction/pause handle journal, CoinsInEscrow includes station loans, freezes settle temporary lamps, victory checks station escrow. Existing signatures retained. HUD/title/pause controls updated.
- MarshNode: Cost reads optional GateData in campaign; spend/return/outstanding accounting uses that same value. Paid main interaction additionally checks configured mechanic prerequisites; stable IDs and legacy defaults retained.
- MarshPacingTelemetry: journal pages count as reading. Existing developer-only compilation boundary retained.
- Classes introduced earlier in this run: SectionData adds optional mainGate; MarshCampaign adds station/mark references, unique token/collection queries, gate readiness/hints, ExtraEscrow, journal pages/rewards and FreezeLoans. CampaignVerification physically walks each new task and all8marks. MarshCampaignTests fixture prepares actual settled-duck/inside-plaque and station interactions for existing completion/checkpoint cases. No production inventory/controller/manager rewrite.

### Final run audit
36pre-run scripts compared to backups:26unchanged,10extended (DuckFollower,EnemyHitPoints,Player,MarshEnemyFeedback,MarshLevel,MarshNode,MarshPacingTelemetry,MarshPatrol,MarshStory,Weapon).35new C#files under Assets/_Agent. Original scene SHA256 024CBE0337605493CF34DDC44398E29EE5CD3F57DA634A6BD0FF2BF25870DA89. Exactly1each original level/campaign/game/camera/player/duck/inventory/health/weapon/equipment coordinator;12enemies,0missing scripts,0temporary test scenes. Legacy Input Manager activeInputHandler0 retained. Scene clean/saved in Edit Mode, unpaused. No git commands or commits in the authorized M1–M4 run.

### Completed scope and remaining limits
M1/M2/M3/M4 complete. STOP before M5. Bosses, full climax/twist/resolution expansion, final chapter visuals, production animations/audio, standalone build and final human balance remain M5–M7. Current new visuals are labelled prefab/scene assemblies in the manifest; badge currently uses HUD/journal text. ART_ADAPTER.md is the completed specification, not an actor-art migration. No known gameplay blocker in the tested Editor configuration; human input feel, other aspect ratios and campaign duration need manual testing. Evidence: Data/CampaignRuns/M1..M4; final report MILESTONES_1_4_REPORT.md.


## Resumption verification — 7 November 2026 (Melbourne)
The supplied screenshot showed an earlier M2 stage; disk records/source/scene/evidence confirm M1–M4 already complete. Read DESIGN,PROGRESS,BACKLOG,ASSETS_NEEDED,CHANGELOG and final report. Enemy collision/physics reset and white hit flash are already present. Fresh ALL98/98passed,0failed/skipped,43.5136s, job259ea5f9b8a74c72a497377cc5ebd41e. After SDK scene restoration: saved campaign in Edit Mode/unpaused,0errors/0warnings/0missing scripts; original scene byte-for-byte unchanged. Fresh resource audit203/203states recover/finish. Prior124/124 physical route and28captures preserved; no duplicate full walk needed for this unchanged gameplay state. No gameplay scripts changed; no git commits. Stop beforeM5 remains in force. Evidence Data/CampaignRuns/Resume20261107 and RESUME_VERIFICATION.md.

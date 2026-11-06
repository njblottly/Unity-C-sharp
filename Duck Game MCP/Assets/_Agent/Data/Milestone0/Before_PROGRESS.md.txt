# Progress — Marshlight
Updated 6 November 2026

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

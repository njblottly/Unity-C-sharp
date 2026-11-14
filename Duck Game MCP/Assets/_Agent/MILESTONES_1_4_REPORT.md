# Marshlight — Milestones 1–4 complete
6 November2026. Stopped before Milestone5. No git commits.

## What changed
- M1: isolated five-chapter campaign,5paid main crossings,6checkpoints and reusable CampaignData/ChapterData/SectionData. Existing story data/UI extended with stable IDs, prerequisites, capability gates and once-per-run beats. All ActI/II bindings placed; current17entries include the retained reunion endpoint.
- M2: shared EnemyData/EnemyAgent composed with existing health and sword systems; Reed Skitter and guarded Silt Crab. Locked attack tells, HP pips, white hit flash/knockback and shared1second player invulnerability. Original configured patrol retained.
- M3: Wick Moth and Counterweight Sprout. Terrain-blocked, short-lived single seeds clear on death/reset. Per-section assets tune the12enemy roster; no enemy defeat opens a gate.
- M4:3duck-held perch inspections,2refundable inspection lamps,2selectable mail routes,8optional Reed Marks and paged journal. Main gates still require3coins; archive2/1/2 exchange retained.4marks earn a visible text BADGE,8add a closing memory. Coin recovery is checked in runtime tests and a finite resource model.
- ART_ADAPTER.md specifies the mixed actor-frame integration. Current new enemies/props are labelled existing-sprite/geometric placeholders; production replacements are in ASSETS_NEEDED.md.

## How to try it
Open Assets/_Agent/Scenes/Marshlight_Campaign.unity (first enabled build scene), press Play and Begin. Original Marshlight.unity remains available for comparison and is byte-for-byte unchanged.

WASD/arrows move; E interacts; Space/Enter advance story. J/left click swipes; Q equips/packs the inventory-owned sword. Tab opens/closes the journal; Space/arrows turn pages, Esc closes/pauses. R restarts defeat/replay. Editor/development F6 cycles six checkpoints without granting progress; F7 toggles pacing telemetry. Developer logs remain under Application.persistentDataPath/Marshlight/Telemetry and are absent from release builds.

Reeds: E at the duck perch, enter after Duckduckgoose settles, E at INSPECT to latch/recall. Inside call/recall is free and an occupied open doorway stays safe. Archive: E borrows one lamp coin to inspect; E again returns it. Pause/death/dialogue/journal also settle that temporary loan. Storm/Haven: E at sorter sends2coins to LOW; E again reroutes the same2to HIGH. Current address appears in the prompt. Claim once at the addressed box; Storm HIGH requires the optional perch inspection, and LOW stays freely reachable. Each main bell still needs all3coins afterward.

## Existing scripts changed
All36pre-run sources compared to backups:26unchanged,10extended;35new C#files under Assets/_Agent. Existing public signatures retained; additions and semantic details are logged in PROGRESS.md.

| Existing source | Reason |
| --- | --- |
| MarshStory.cs |Entry policies/prerequisites/capabilities and validation.|
| MarshLevel.cs |Optional campaign counts/progression, station escrow, story queries, paged journal/menu/HUD and win checks.|
| MarshNode.cs |Configured gate cost/prerequisites with common spend/refund accounting; legacy defaults retained.|
| MarshPacingTelemetry.cs |Dynamic chapters, simulation clock/Editor-hold correction and journal reading.|
| EnemyHitPoints.cs |Configured encounter HP, damage/death events and complete collision/physics reset.|
| MarshPatrol.cs |Optional section asset configuration for the retained original patrol.|
| MarshEnemyFeedback.cs |Event-driven hit/guard feedback, scalable pips and reset.|
| Player.cs |One shared enemy-damage immunity timer, additive TryTakeEnemyDamage/InvulnerabilityRemaining.|
| Weapon.cs |Route sword hits through enemy guard/health events, retaining inventory/coin ownership.|
| DuckFollower.cs |Additive walk-to-perch/recall API; checkpoint/debug snaps recall assistance.|

Classes created earlier in this run were extended in later milestones: SectionData gains encounter/gate data, MarshCampaign gains mechanic/mark/journal bindings, and editor builders/walker/tests gain the corresponding coverage. Original input/controller/inventory/game/camera managers remain in use. No package/input migration.

## Verification
| Checkpoint in run | Passing PlayMode cases | Full-route assertions |
| --- | ---: | ---: |
| Start after M0 |41|97baseline|
| M1 |56|75campaign|
| M2 |68|75|
| M3 |77|75|
| M4 final |98|124|

Final SDK job45ffb622b3044711a15d64ab82f79e3f:98passed,0failed/skipped,44.4297s. Covers enemy damage/tells/guard/seed collision/cleanup, shared immunity, main/relay/temporary/mail loan deaths/restarts, blocked inventory refunds, both addresses/reroute/duplicate rejection, every checkpoint, gate prerequisites/payment, one-time story triggers, cosmetic marks/journal and replay.

The real no-F6 title-to-ending walk passed124/124 across130steps/312.44world units. It collected all8marks, completed all7station instances and17stories, crossed5paid main bridges+3relay gates and finished with3coins/0escrow. One death with an outstanding bell loan was deliberate. Enemies remain active; combat is additionally verified in tests. Text advances immediately. Last label/button/pause/address presentation fixes preserve that physical layout and resource logic; ALL98reran afterward.

Resource audit:203reachable states/783transitions, all203can finish,2ending resource states. Every payment/refund stage, relay phase, mail address/claim phase and lamp-held combination is covered using the current3coin/1sword/1lamp/2mail/2-1-2relay preset. This is an abstract resource proof; collision and companion motion are covered separately by actual runtime tests/walker. Optional cosmetic mark bits do not enter the coin graph.

All6debug destinations,5main gates and3relay gates captured and console-read, plus every physical checkpoint/win and final perch/lamp/mail/pause/journal/BADGE views.0native errors/0warnings in final tests/route/visual audit;0missing scripts; exactly1of each existing manager/player/duck/inventory/UI/weapon coordinator. Scene saved clean in Edit Mode, unpaused. Evidence under Assets/_Agent/Data/CampaignRuns/M1..M4. The resolved SDK scene-restoration race, guessed graph-size test threshold, shutter exit issue and transient MCP disconnects are recorded in PROGRESS.

## Play time
| Chapter | M3 active | M4 exploration route | Change |
| --- | ---: | ---: | ---: |
| Keeper's Landing | 30.53s | 32.28s | +1.75s |
| Listening Reeds | 20.76s | 34.36s | +13.60s |
| The Old Archive | 56.50s | 58.71s | +2.22s |
| Storm Locks | 22.56s | 40.64s | +18.07s |
| Lantern Haven | 26.42s | 43.03s | +16.62s |
| Total | 156.77s | 209.02s | +52.25s |

Haven includes its short reunion interval. M4 deliberately takes optional Storm HIGH, Haven LOW and all8marks.209.02s(3m29s) is active simulation with instant text and1deliberate death; capture/pause time is excluded. It is not a human first-time duration. No movement slowdown or empty-distance padding. The final25–32minute campaign target remains unproven while M5/M6content and M7balance are pending.

## Known limits
No known gameplay blocker in the tested Editor configuration. Full boss/climax/resolution expansion is future scope, and the current reunion remains the intermediate playable endpoint. New enemy/mechanic art is labelled placeholder assembly; badge is a text award awaiting its icon. Hardware input feel, human pacing, other aspect ratios, audio balance and a standalone build remain untested. Art adapter is specified, not applied. The finite resource proof covers this topology/cost preset; extend it when adding arena locks/resource types. No commits were made.

## Human playtest checklist
1. Start the campaign title without F6, collect sword/3coins and time each named chapter while reading normally.
2. Bait each enemy tell: sidestep Skitter/Moth, flank Crab or strike in recovery; let a Sprout seed hit a bank. Confirm sword HP loss/flash and brief player immunity.
3. At the Reeds perch, inspect with Duckduckgoose holding. Also recall inside before inspection and confirm you can exit/retry freely.
4. Inspect/refund Archive lamp; pause or die while it holds a coin. Complete the original2/1/2relay and recover all3before the main bell.
5. At Storm sorter choose HIGH, try the wrong box, reroute to LOW, then reclaim. Die with an addressed loan and restart; recover exactly2, never a duplicate. Try the optional HIGH perch path in a second run.
6. Complete Haven perch/lamp/mail sequence. Confirm a main bridge stays closed until its ordinary3coin payment, regardless of enemies/marks.
7. Keep4then8marks. Check HUD BADGE, journal pages/Tab/Esc,8mark closing memory, and a separate0mark victory. Marks must not occupy inventory slots.
8. Replay after victory; check zero bells/coins/marks/loans and fresh enemies. Separately test F6/F7 in Editor, then exclude that run from pacing.

## Recommended next milestone / backlog priorities
1. Milestone5: data-driven mini-boss/final boss, safe pre-arena checkpoints, loan-safe entry and climax/duck twist beats.
2. Human timed campaign playtest and learning/route/combat feedback.
3. Milestone6 art adapter/production placeholders and chapter set pieces, then M7balance/audio/standalone checks.


## Resumption check — 7 November 2026
Fresh ALL98/98PlayMode cases pass (43.5136s, job259ea5f9b8a74c72a497377cc5ebd41e). Saved campaign restored in Edit Mode,0errors/0warnings/0missing scripts, original scene unchanged. No gameplay source changes; prior route/timings above are preserved evidence. See RESUME_VERIFICATION.md. M5 has not started; no git commits.

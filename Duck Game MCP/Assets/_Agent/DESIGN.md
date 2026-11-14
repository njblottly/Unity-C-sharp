# Marshlight: The Borrowed Bells — campaign design
Version 8, 6 November 2026. Milestones1–4 complete; stop before5. Current campaign: Assets/_Agent/Scenes/Marshlight_Campaign.unity.

## Status and deliberate scope change
The current build implements the five-chapter progression/story framework, four new normal enemy types, three mechanics and eight Reed Marks from Milestones1–4. Bosses and the expanded climax/resolution remain M5/M6 plans; final balance remains M7. The original playable Assets/_Agent/Scenes/Marshlight.unity is preserved byte-for-byte. M0 inventory and original design proposals below are historical; the later execution/as-built sections record deliberate changes. Prior v5 is preserved in Data/Milestone0/Before_DESIGN.md.txt.

The preserved pre-campaign project differs from the supplied brief: MovementController reads Horizontal and Vertical and moves a zero-gravity Rigidbody2D on both axes. It is top-down, progressing mainly west to east with upper/lower routes. Its baseline has three main bridges/three relays, four checkpoints/four patrols/twelve stories. The current campaign has five main bridges/three relays, six checkpoints, twelve enemies and seventeen stories. Preserve the working perspective, camera, controller and input. A side-view conversion needs a separate decision; none is assumed by this plan.

## Baseline inventory and system map
Inventory before this run's evidence: Unity 6000.3.9f1; two scenes (Marshlight first enabled build scene, original Assets/Scenes/LevelOne.unity second); 36 C# sources, 14 prefab assets including the tile palette, 14 animation clips. There are 105 Texture2D source files: 72 historical evidence images and 33 outside those evidence directories, not 105 gameplay sheets. Sprite enumeration gives 41 distinct source paths after evidence exclusion. Data/Milestone0/ProjectInventory.json lists every path, prefab component, scene connection, package and node.

Installed packages include Cinemachine 3.1.5, 2D feature 2.0.2, Test Framework 1.6.0, UGUI 2.0.0, Timeline 1.8.10 and Unity MCP. Legacy Input Manager is active (activeInputHandler=0); no Input System migration or package change is authorized.

| Existing system | Connection and extension point |
| --- | --- |
| RPGGameManager / RPGCameraManager | Existing single instances own player/spawn/camera lifecycle; MarshLevel coordinates the level. Reuse them. |
| MovementController | Normalized XY movement, serialized speed 1.6 units/s, directional Animator and modal input suppression. |
| Player / Character / HitPoints | Five starting/max HP; 1 contact damage and 1-second contact cooldown. Direct TakeDamage is not a universal invincibility gateway yet. Death preserves the actor; respawn synchronizes Rigidbody and Transform. |
| DuckFollower | Serialized player reference, smooth following, directional animation and SnapToPlayer on restart/debug movement. Extend this for perches. |
| Consumable / Item / Inventory / Slot | Actual trigger pickups, stackable brass coins, healing hearts, one sword, five-slot UI. Item is already a ScriptableObject; use atomic TrySpend/TryAddItems and ItemCollected. |
| MarshNode / MarshCoinRelay | Three 3-coin bells and far-side refunds; relay loans 2/1/2 with re-lending prerequisite. Paid gates latch; loans and progression survive checkpoint death. |
| MarshLevel / MarshStory | Title/intro, play, dialogue, pause, defeat and win; twelve data-driven entries with stable IDs/read tracking. Notes reread without repeated rewards. No separate asynchronous loading framework was found. |
| Weapon / WeaponData | Reedkeeper Sword: damage 1, range 1.15, arc 100 degrees, visible 0.20 s, cooldown 0.50 s; borrows/refunds one carried coin. One hit per target/swipe, solid banks block hits. |
| EnemyHitPoints / MarshPatrol / MarshEnemyFeedback | Four 3-HP mites, speed 0.85, contact via Player; reusable health/death with pips/flash/particles. Compose future enemy behavior with these. |
| MarshPacingTelemetry / MarshVerification | Development-only F7 overlay/JSONL, guarded F6 cycling, editor walker with 97 assertions. Retain safeguards. |

Controls: WASD/arrows move; E interacts; Space/Enter advance text; J/left click swipes toward last movement direction; Q equips/packs; Esc pauses; R restarts after defeat. F6 cycles four checkpoints in Editor/development builds without granting progress; F7 toggles pacing. No duplicate debug handler or UI is needed.

## Core fantasy and invariants
Aiken and Duckduckgoose repair a storm-broken marsh route. Useful things are borrowed and returned; rescue is patient maintenance. Coins are counterweights, not grind currency. The same three original brass coins circulate through the proposed campaign.

- Carried coins + uncollected original pickups + outstanding gate/tool/sword loans always total three. No enemy or collectible permanently spends or creates these.
- Every valid puzzle state permits recovery of enough coins to finish. Full inventory, missed equipment, death and changed routes must not strand refunds.
- A main bridge opens only after its own required coin transaction. Enemy/boss defeat, a perch or a lantern never substitutes for payment.
- Completed main crossings latch open; never close a collider under an actor.
- Definitions live in ScriptableObjects; HP, loans, marks and read flags belong to the current run.
- Extend play through decisions, encounters and discovery, not slower movement, empty distances, repeated text or tedious old gates.

## Story bible and four acts
Rill Marsh after a storm: bells and sluices that guide duck families to Lantern Haven are jammed. The final engine is damaged maintenance machinery, not an evil duck. Combat clears pests and frees machinery; defeated creatures retreat.

Characters: Aiken, junior keeper; Duckduckgoose, companion and partner; Ada, senior keeper initially present through letters. The flock and injured young duck remain unnamed.

Emotional turn: Aiken assumes Duckduckgoose is lost. The archive/refuge message reveals he knows where the flock sheltered: he came back for Ada, who stayed with a young duck unable to fly. The fifth place in the old ledger was for her. At the ending the flock makes room for its keeper, paying off the existing ledger text.

| Act | Chapters | Dramatic progression |
| --- | --- | --- |
| I — Setup | 1 | Repair the first bell, learn every coin returns, establish the duck's determination. |
| II — Rising tension | 2–3 | Become partners, follow linked archive gates and discover someone is still missing; a jammed guardian blocks the dry route. |
| III — Climax | 4 and most of 5 | Reveal why the duck returned; reconnect locks and stop the storm engine so Ada can leave. |
| IV — Resolution | 5 after engine | Return every loan, ring Haven Bell, reunite flock and Ada, leave the route usable for the next traveler. |

### Complete planned story beats
Outline dialogue below is not final authored pages. Automatic beats fire once per run; notes remain rereadable without repeat rewards.

| Beat | Act / chapter / location | Rough content | Entry ID |
| --- | --- | --- | --- |
| S01 | I / 1 / title and landing | Aiken promises to help Duckduckgoose home after the storm silenced the route. | intro, retained |
| S02 | I / 1 / Ada's letterbox | “The bells borrow their weights. Take the same three coins with you.” | letter, retained |
| S03 | I / 1 / sword pickup | Ada's maintenance blade borrows one coin per swing; pack it when the guard is empty. | sword, retained |
| S04 | I / 1 / Landing Bell | A distant call answers; the far-side box returns all three coins. | bell1, retained |
| S05 | II / 2 / safe perch | Duck waits on a marked perch to open a service shutter. “You know this place.” | perch-intro, planned |
| S06 | II / 2 / reed ledger | Ada records repairs for all route users, not tolls. | ledger, retained |
| S07 | II / 2 / Reedbank Bell | A reply comes from farther east; the duck listens for a second voice. | reedbank-bell, planned |
| S08 | II / 3 / flood note | Ada sheltered the flock and a young bird unable to fly. | flood, retained |
| S09 | II / 3 / inspection lamp | Repair glyphs appear; the clue remains in the journal after refund. | lantern-intro, planned |
| S10 | II / 3 / first relay use | Split 2 and 1; recover the pair and lend it again. Links show the distant gate. | exchange, retained |
| S11 | II / 3 / optional dry pocket | Four places and a fifth footprint: the duck made room for Ada too. | archive-pocket, retained |
| S12 | II / 3 / Archive Bell | Dry route restored, jammed guardian ahead. | bell2, retained |
| S13 | II / 3 / mini-boss antechamber | Ada's note identifies warning rhythm and safe recovery windows. | sluice-warning, planned |
| S14 | III / 4 / refuge message | Duckduckgoose was not lost; he returned for Ada. His friends stayed with the injured bird. | returned-for-ada, planned |
| S15 | III / 4 / safe mail selector | Choose a reachable refund box; the bell never owns the coins. | mail-route, planned |
| S16 | III / 4 / keeper's promise | “A bridge is finished when the next traveler can use it.” Aiken will return for everyone. | promise, retained |
| S17 | III / 4 / Storm Bell | Last channel aligns; the engine pulls against jammed weights. | storm-bell, planned |
| S18 | III / 5 / final safe checkpoint | Settle loans/recover missed sword; Ada answers and describes three warning patterns. | engine-warning, planned |
| S19 | III / 5 / engine midpoint | Brief nonmodal HUD line points out a safe lane. Never cover telegraphs; queue longer text until recovery. | engine-midpoint, planned |
| S20 | IV / 5 / Haven Bell/refund | Engine rests; ring the last bell and take back the original three coins. | bell3, retained |
| S21 | IV / 5 / reunion circle | Flock surrounds Duckduckgoose; Ada arrives with the young bird. A place remains beside them for the keeper. | ending, expand campaign copy |
| S22 | IV / 5 / optional journal coda | Eight marks recall small acts that kept the route open. Cosmetic/text reward only. | marks-coda, planned |

Preserve all twelve original IDs: intro, letter, sword, bell1, ledger, flood, bell2, exchange, archive-pocket, promise, bell3, ending. Keep the baseline MarshStory asset; extend a campaign copy later. No story asset is edited in M0.

## Five chapters and target duration
Target first campaign playthrough: 25–32 minutes including learning, text and ordinary retries; optional exploration adds about 2–4 minutes. These are proposed targets, not current measured content.

| Chapter / theme | Act | Target | Enemies / main crossing | New mechanic or coin twist | Beats |
| --- | --- | --- | --- | --- | --- |
| 1. Keeper's Landing — warm brass/storm-dark bank | I | 4–5 min | Existing mite, isolated Skitters; Landing Bell | Safe 3-coin collect/lend/reclaim, sword before required combat. | S01–S04; 1 mark |
| 2. Listening Reeds — shelter/shared work | II | 5–6 min | Skitters, Crabs; new Reedbank Bell | Harmless perch lesson, then a side shutter combined with a paid bridge. | S05–S07; 2 marks |
| 3. The Old Archive — reflected light/records | II | 5–7 min | Crabs, Moths; Archive Bell, Siltlock Tender | Teach lamp/refund, retain split/re-lend relay, safe pre-boss checkpoint. | S08–S13; 2 marks |
| 4. Storm Locks — exposed links/routing | III | 5–6 min | Moths, Sprouts; new Storm Bell | Safe return routing, then combine familiar perch/lamp rules; optional delivery route. | S14–S17; 2 marks |
| 5. Lantern Haven — amber refuge/rain easing | III→IV | 6–8 min | Familiar foes, Storm Bell Engine; Haven Bell | Short synthesis, pre-boss checkpoint, three fair phases, final refund/reunion. | S18–S22; 1 mark |

Future five main bells: existing bell-0, new campaign-bell-reed, existing bell-1, new campaign-bell-storm, existing bell-2. Retain original IDs. Current baseline still has three main bells and four checkpoints.

M1 should create Assets/_Agent/Scenes/Marshlight_Campaign.unity as an isolated copy, retaining the tested baseline. Reuse one level coordinator and existing manager/UI instances. Baseline configuration defaults to three bells; campaign data permits five. Build-scene changes are now authorized; M1 adds the campaign first while retaining existing scenes. Unbuilt chapters may have functional ordinary coin-gate blockouts and existing patrols, clearly marked incomplete. Author later tutorial definitions/locations but keep their triggers/objectives inactive until their capability ships; never ask players to use an unbuilt control.

## Enemy roster and initial data values
Proposed future EnemyData/BossData values only. HP/damage integer; speed/range world units. M2 extends the existing 1-second contact protection to a common damage gateway so different attack sources cannot double-hit within it.

| Type / introduction | HP | Damage | Speed / reach | Telegraph / recovery | Behavior and counterplay |
| --- | ---: | --- | --- | --- | --- |
| Existing Marsh Mite, current/ch.1 | 3 | Contact 1 | Patrol 0.85 | Existing contact behavior | Preserve baseline health/death and coverage. |
| Reed Skitter, new/ch.1/M2 | 2 | Contact/lunge 1 | Move 0.90; lunge 1.80 for 0.35 s | 0.50 s crouch / 0.85 s recovery | Locks direction before a straight lunge; sidestep then strike. First has wide safe lane. |
| Silt Crab, new/ch.2/M2 | 4 | Contact/claw 1 | Move 0.55; claw reach 0.70 | 0.75 s claw raise / 1.10 s recovery | Visible frontal guard; flanks/recovery vulnerable, no hidden invulnerability. |
| Wick Moth, new/ch.3/M3 | 2 | Dive 1 | Orbit 0.80; dive 1.60 for 0.45 s | 0.80 s wing/light warning / 1.20 s recovery | Authored waypoints; marked dive lane, terrain blocks attacks. |
| Counterweight Sprout, new/ch.4/M3 | 5 | Contact/seed 1 | Move 0.45; seed 2.00, range 2.5 | 1.00 s swelling pod / 1.25 s recovery | One visible terrain-blocked projectile; route planning, no progression coin drops. |
| Siltlock Tender, mini-boss/end ch.3/M5 | 12 | Contact 1, heavy sweep 2 | Move 0.55; charge 1.50 | Charge 1.10 s, sweep 1.25 s / 1.50 s recovery | Above 6 HP one charge lane; at/below 6 alternates charge/sweep. Safe side lane, guaranteed punish window, no adds. |
| Storm Bell Engine, final/ch.5/M5 | 20 | Marked hazard 1, slam 2 | Anchored; hazards 1.20 | At least 1.20 s warning / 1.50 s recovery | HP20–14 one marked lane;13–7 alternates lanes;6–0 familiar sequence/longer vulnerable window. Always reachable safe lane. |

Normal foes recover from hit reaction in 0.20 s; repeated hits refresh feedback, never permanent stun. Common flash/knockback/health feedback; bosses resist displacement and use data-driven phase/recovery rules. Existing feedback expects one pip per max HP: extend with a boss-bar mode deliberately, not twenty pips or suppressed reference errors. Enemy/boss death never pays a main gate; arena completion opens only its separate exit.

Safe dry checkpoint immediately before each boss: normal full health, instructions, accessible loan refunds and an exit before fight begins. Validate sword ownership and a carried coin; a missed starter sword remains recoverable from an accessible unique-item rack with no duplication. Never lock a player in with every coin inaccessible. Player defeat during a boss resets that encounter's HP/phase; latched gates, marks and loans retain run state. Fresh replay resets everything.

## Three new mechanics, M4
Existing sword and archive relay do not count as new mechanics.

### Duck Perches
E at a marked dry perch asks Duckduckgoose to wait on its pressure plate; E at a recall marker resumes following. Extend DuckFollower with assist state/Inspector bindings. Opens only a service shutter, never a main bridge. First lesson is harmless with visible plate/shutter link; later service lanes reach refunds/selectors. Dry recall/exit routes prevent trapping; death snaps the duck to the checkpoint. Paid main gates stay latched. No ferry, duck jump or free payment.

### Inspection Lantern
An anchored lamp borrows one carried coin to reveal Ada's glyphs for a nearby selector; E extinguishes/refunds. Discovered clues remain in the journal so all three coins can be used elsewhere. Teach safely with three coins before testing a choice. It does not create bridges, stun bosses or remove patrols. Modal transitions/death/section exit settle escrow or preserve an explicitly recoverable pending refund if inventory is full, following Weapon's pattern. The lamp never controls the sole route to its own refund.

### Return Mail Routing
A dry selector chooses which of two reachable refund boxes receives a two-coin loan. Extend existing 2/1 split exchange ownership, adding delivery choice without extra currency. First both boxes visible/safe; later one shortens a service route while the other offers an optional mark/note. Exactly one owner and one redeemable destination; switching never duplicates a loan. Reject switches that place the only useful coins behind their own unpaid gate. Selectors remain reachable, paid main gates latch, clear link/prompt names the holder.

Progression: ch.1 pay/refund3; ch.2 companion service shutter plus paid bridge; ch.3 inspect/refund and split/re-lend; ch.4 route refunds/optional detours; ch.5 combine known rules then settle every loan. Keep supply 3/main costs 3: challenge comes from order, ownership and routes.

M4 must enumerate the reachable small state graph (carried coins, loan owners/destinations, gate latches, lamp/perch state). Every reachable state must conserve three coins and reach a valid finish. Cover both destinations, cancellation/recall, full inventory, each checkpoint death phase and replay. Invalid definitions produce clear validation errors, not free coins or disabled colliders.

## New collectible: Reed Marks
Eight optional carved feather tokens distributed 1/2/2/2/1. Unique IDs unlock short journal memories; four grant a cosmetic brass duck badge, eight an optional ending paragraph. No health/damage bonus, gate discount or victory requirement. Separate journal from five-slot inventory so marks cannot block a refund. Once per ID/run; persists through checkpoint death, resets on replay. First in sight of trail; later detours use learned rules and return to the main route. No grinding or mandatory backtracking.

## Future data architecture and implementation boundaries
Create only in the listed milestone. Validate stable IDs, Inspector references, positive values, coin dependencies and duplicate binders.

| Definition | Fields / owner | First build |
| --- | --- | --- |
| CampaignData / ChapterData | Ordered chapters/main gates, entry/end story IDs, target time, journal rewards; three-gate baseline defaults. | M1 |
| SectionData | Chapter, explicit checkpoint/gate/story IDs, encounter records, visual theme, time budget, mechanic permissions. | M1 |
| MarshStory trigger data | Extend entries/pages/IDs with policy/prerequisites/once rewards; notes reread without repeat progression. | M1 |
| EnemyData | HP, damage, speed/range, detection, telegraph/recovery, hit response, projectile/effects and art/audio; section encounter assets select/tune. | M2/M3 |
| BossData | Enemy values plus HP thresholds, phase attacks, safe lanes, arena/exit/checkpoint/loan-entry policy. | M5 |
| MechanicData / gate graph | Costs, legal destinations, prerequisites, latch/refund and typed perch/lamp/routing/tutorial definitions. | M4 |
| CollectibleData | Unique ID, sprite, journal entry, cosmetic thresholds and once policy. | M4 |
| Existing Item / PowerUpData if needed | Retain data-driven health quantity/effect; any new power-up has explicit effect/duration data. | M4 if added |
| Existing WeaponData | Reuse sword and working inventory/HP integration. | Already built |

Run state stays in the existing level coordinator, not shared definition assets or another singleton. Compose enemy behavior with EnemyHitPoints/MarshEnemyFeedback and extend contact damage without double application. Never mutate source ScriptableObjects in Play Mode. Log every future existing public API change in PROGRESS.

## Art audit before future production
No art or importer edits in M0. Sources are mixed; do not claim every character already uses 32x32.

| Actor/source | Measured frame/PPU/pivot | Palette/outline | Import/animation |
| --- | --- | --- | --- |
| Aiken: Assets/Sprites/Player/player.png | Sheet955x1453; serialized frame227x358, centre113.5/179; animation slices roughly186–227x288–358. PPU 32, root scale0.065. | 15,606 opaque RGB colors sampled (high-resolution/antialiased); brown #6A4133, navy #161F33, blue #17639A, skin #F4C298; edge near-black/navy including #000006. | Point, uncompressed, no mipmaps, Tight.12fps east/west/north4 keys each,south/idle2. |
| Duckduckgoose: Assets/Sprites/Companions/duck 1.png | Sheet128x160; frames32x32, centre16/16, PPU 32, scale0.48. | 11 colors sampled; greys #DCDCDC/#E1E1E1/#C5C5C5/#BCBCBC, orange #FF8514/#FF922C/#FF7B00, eye #0E0E0E. No continuous black exterior stroke. | Point, uncompressed, no mipmaps, Tight.12fps four-direction walks4 keys each,idle2. |
| Marsh mite: Assets/Sprites/Enemies/EnemyIdle_1.png, EnemyWalk_1.png | Idle sheet160x32; frames32x32, centre16/16, PPU 32, scale0.72. | 4 colors #08FF00/#06BE00/#000000/#FFFFFF; black outline. | Point, uncompressed, no mipmaps, Tight.12fps idle/walk5 keys each. |

Data/Milestone0/ArtAudit.json contains sampled rectangles, clip frame variants, dominant colors and edge samples. Future new enemies/collectibles use the existing 32x32 family at PPU 32, Point/no compression/no mipmaps/Tight, with the requested one-logical-pixel #000000 outline and two/three flat shades per material. Bosses use the same frame grid with visual-only scaling or modular pieces; colliders are independent.

Future actor palette: 15 opaque colors plus transparent — #000000,#161F33,#6A4133,#17639A,#F4C298,#E3E3E3,#DCDCDC,#C5C5C5,#BCBCBC,#FF8514,#FF7B00,#08FF00,#06BE00,#FFFFFF,#E2C57C. This is a guide, not the current player's literal palette. Existing prop palette and tall32x48 props stay unchanged.

Do not normalize/reslice Aiken or retrofit duck outlines in this run. Future normalized actor art requires a visual-child adapter preserving root collider/body scale and animation bindings. Where matching quality is unavailable, label placeholders under Assets/_Agent/Placeholder and record actual file/frame/animation specifications in ASSETS_NEEDED.md. No internet downloads.

## Milestone boundaries
0. Inventory, fresh baseline timing, this design, retained F6 and completion regression only.
1. Extend story/chapter/section data, isolated five-gate campaign blockout and ActI/II beats; preserve original scene/tests. No new enemies/tools/bosses.
2. Shared enemy data/agent/damage protection/telegraphs/feedback; Skitter and Crab with safe lessons.
3. Moth and Sprout, encounter assets, data-only difficulty tuning.
4. Perches, lamp, mail routing and eight marks; tutorials and exhaustive coin-state recovery.
5. Both bosses, safe checkpoints/phases/telegraphs, climax/twist; no coin gate bypass.
6. Chapter set pieces/environmental story/visuals and complete resolution; deliberate art adapter.
7. Feedback/readability/audio/volume and measured balance; human validate duration.

Each milestone rereads persistent files. The latest user instruction prohibits git commits and authorizes settings/package/folder changes when needed; preserve current input/packages unless a concrete need arises. Extend targeted code, preserve Inspector references, check existing instances, edit only outside Play, compile/read console then tests/debug views/full physical route.

## Measured baseline and remaining observations
Fresh Milestone 0 route, 6 November 2026: the existing editor walker started at the real title and reached victory without F6. It passed 97/97 assertions across 70 steps and 159.57 world units. Active telemetry was 106.024 seconds (1:46.02); text advanced immediately, giving about 0.04 seconds recorded reading time. One death/restart was deliberate test input.

| Current checkpoint interval | Active time | Deaths |
| --- | ---: | ---: |
| Landing to Reedbank | 29.76 s | 0 |
| Reedbank to Archive | 27.76 s | 1, deliberate |
| Archive to Haven | 44.90 s | 0 |
| Haven to reunion | 3.60 s | 0 |
| Total | 106.02 s | 1 |

These are the current prototype's four intervals, not five campaign chapter timings. Simulation/checkpoint holds, including the MCP outage, are excluded; this is not wall time or a first-time human result. The five proposed chapters have no measured time yet. Time them as they are built, then validate the 25–32 minute campaign target with a human run.

All four existing F6 destinations and six gates were captured/console-read this run; seventeen screenshots total across physical/debug passes. Debug inspection invokes the same guarded DebugTeleport entry point as F6, with serialized gate references for framing; hardware delivery remains a human check. Debug did not fund gates or advance progress.

Baseline 40/40 PlayMode cases; final 41/41, no failures/skips, SDK 15.558s. Compilation, fresh route, gate views and final native console:0 errors/0 warnings. Two initial pre-existing HandleUtility font warnings were recorded before baseline testing; active-scene UI/world font references were present and no TMP text objects loaded. They did not recur. Investigate a recurrence; do not suppress components or change fonts/settings speculatively.

Current game remains the short baseline, not the planned campaign. Human input feel/reading time, other aspect ratios, audio and standalone build remain manual checks. Evidence is under Data/Milestone0.


## Milestones 1–4 execution plan — 6 November 2026
The latest user instruction authorizes sequential M1, M2, M3 and M4 without approval checkpoints and explicitly prohibits git commits. It supersedes the earlier commit/approval wording above. Stop before M5. Preserve the tested baseline scene and legacy input; use an isolated campaign scene. The human reports all M0 manual checks passed; no fresh human timing was supplied.

Campaign coordinates are deliberate content blocks, not a movement slowdown: chapter starts x=0/22/50/86/120, five main bridge channels x=18/46/82/116/150, final checkpoint154 and reunion162. Traversable banks y=-7..7 with continuous outer blocking and solid water banks above/below every bridge. Checkpoints:0,22,50,86,120,154. Main IDs/order: bell-0, campaign-bell-reed, bell-1, campaign-bell-storm, bell-2. Original stable story IDs are retained in a copied story asset. Five ChapterData and five SectionData assets own names/objectives/timing/encounters/mechanic permissions; MarshLevel receives optional campaign configuration and retains three-bell legacy defaults.

M1 extends the existing story entry data with prerequisites, capability requirements and a once-per-run policy. Scene triggers use explicit references and stable IDs. Place all ActI/II entries; perch/lamp tutorials are capability-gated until M4. Existing dialogue/menu UI is reused. M2 composes configurable enemy agents with EnemyHitPoints and MarshEnemyFeedback, uses a common enemy-damage cooldown on Player, and adds Skitter/Crab. M3 adds Moth/Sprout and per-section encounter tuning through assets. M4 adds perches, inspection/refund lamps, reachable selectable mail refunds and eight unique journal marks. Tutorial spaces are safe, followed by combinations; neither tool nor enemy death may open a main bridge.

Boss arenas/engine and final campaign-resolution expansion remain M5/M6. The current reunion remains a playable endpoint during these intermediate milestones. The25–32minute target is a final campaign target; report intermediate measured timings honestly and do not pad them.


## M2 implementation detail before build
EnemyData/SectionData encounters are the tuning authority. Shared EnemyAgent owns patrol/chase-with-leash, locked telegraph, attack, recovery, hit reaction and checkpoint reset; EnemyHitPoints remains HP/death owner and Weapon remains damage source. Skitter:HP2,contact/attack1,move0.9,lunge1.8 for0.35s,warn0.50,recover0.85. Crab:HP4,damage1,move0.55,reach0.70,warn0.75,attack0.15,recover1.10; visible frontal150-degree guard, flanks/recovery vulnerable. All combat uses Player.TryTakeEnemyDamage with the existing1s cooldown; direct TakeDamage keeps scripted/forced-damage semantics. Hit reaction0.20s with1.25s reaction cooldown prevents repeated sword hits from renewing stun indefinitely. Terrain casts and2-unit home leashes prevent attack/knockback through banks.

M2 replaces four scaffold patrols and retains one original MarshPatrol through optional section configuration; baseline patrol mode remains unchanged. Campaign has5Skitter/4Crab plus1configured original mite. New attack warning lines/arcs, health pips, true white silhouette flash and brief player blink are presentation only. New prefab visuals are explicitly labelled placeholder assemblies under Placeholder/Enemies: unchanged first32x32 EnemyIdle frame, Point/uncompressed/32PPU/black outline, plus outlined Square-sprite limbs. No original bitmap edited or new raster art created; proper animated replacement sheets remain in ASSETS_NEEDED.


## M3 content specification before build
Add Wick Moth (HP2, locked amber dive,0.8–0.9s tell) to Archive/Storm/Haven and Counterweight Sprout (HP5, one short-range seed at a time,1.0–1.1s tell) to Storm/Haven. Projectiles collide with Blocking terrain, expire after2.5 units, deal1 through the shared invulnerability and vanish on owner death/checkpoint reset. Enemy defeat never funds/opens a gate. Keep introductions isolated from mechanism tutorial spaces.

Encounter roster changes: Landing1Skitter; Reeds1Skitter+1Crab; Archive1Crab+1Moth; Storm1Moth+1Sprout; Haven1Skitter+1Crab+1Moth+1Sprout+retainedMite. Moth positions(71.5,3.2)/(107,3.2)/(142,2), Sprouts(112,-2)/(146,-2). Moths orbit authored anchors and commit a locked dive. Sprouts pace slowly, fire once along the marked direction and cannot have two live seeds. Seed art is a labelled outlined prefab assembly; no source bitmap change.

| Chapter/type | HP | Move | Attack speed | Tell | Recovery | Range |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Archive Moth |2|0.80|1.60|0.80|1.20|1.8 activation|
| Storm Moth |2|0.85|1.60|0.85|1.20|1.8 activation|
| Haven Moth |2|0.85|1.65|0.90|1.25|1.8 activation|
| Storm Sprout |5|0.45|2.00 seed|1.00|1.25|2.5 seed|
| Haven Sprout |5|0.45|2.10 seed|1.10|1.35|2.5 seed|
| Archive Crab |4|0.55|0|0.80|1.20|0.7 claw|
| Haven Skitter |2|0.95|1.80|0.60|0.90|0.7 + lunge|
| Haven Crab |4|0.60|0|0.90|1.20|0.7 claw|
All damage1. Other M2 values retained. Per-section SO tuning only; no mandatory kill counts or HP padding. Bosses remain Milestone5.


## M4 construction specification before build
Keep three reusable coins, sword loans and archive2/1/2 exchange. GateData retains cost3 on all five main crossings; only prerequisite inspections change. New MechanicData assets own type/cost/instructions/journal memory; CollectibleData and JournalRewardData own optional memories/4-and8 thresholds. No runtime mutations of definition assets.

Three duck-held inspection rooms: Reeds pad(28.5,3.5), roomx30.25..33.75/y2.25..4.75; optional Storm pad(98.5,3.5), roomx100.25..103.75; Haven pad(124.5,3.5), roomx126.25..129.75. Each has continuous top/bottom/east walls, west shutter, inside free call/recall control and inspection plaque. Duck walks to the perch; E at the inside plaque while he is settled latches the shutter and recalls him. Death/debug teleport recalls him. Occupied doorway/room keeps an exit open if recalled before completion. Main Reeds gate requires its lesson; Haven requires its own inspection. Optional Storm room offers alternate mailbox/mark. No perch opens a main water crossing.

Inspection lamps at(55,2) and inside Haven room(129,3.5),cost1. Inspect reveals a persistent journal clue, E returns the coin; pause/dialogue/death/journal settle the temporary loan. A blocked refund stays in explicit escrow and retries when there is space. Archive gate requires its lamp clue plus original relay completion. Haven lamp requires its perch inspection.

Storm sorting box(96,-2),cost2, return A(100,-2) open low road, B(102,3.5) optional perch room. Haven sorter(138,0),requires Haven lamp clue; A(142,0),B(143,3.5). E first sends2 to A, E again reroutes the same2 between A/B with no extra spend. Only addressed box can claim once; source cannot borrow again after completion. Both choices remain west of their paid main gate; Storm B can always be rerouted to free A. Mail loans persist through checkpoint death. Storm/Haven main gates require completed mail return, then still charge3 normally.

Eight optional Reed Marks at(11,3.5),(33.5,4),(41,4.5),(58,4.5),(72.5,6.1),(102,3.5),(111,-4.5),(144,4.5). Existing FieldLetter sprite and outlined Square stand-ins, explicitly labelled; no new bitmap art. E keeps each unique ID once. Separate journal avoids inventory/refund obstruction; checkpoint persists, replay clears. Four unlock a cosmetic brass duck badge in the journal; eight add a short closing memory. No coin, health, combat or gate advantage. Tab opens paged journal in existing menu, Space/arrows change pages, Tab/Esc closes. No duplicate UI manager.

Enable existing perch/lamp tutorial stories; add one short mail-route note before sorter, update copied flood note with the injured young duck's wing. Full climax/twist/bosses and expanded ending remain M5/M6. Validate combined resource reachability with a finite state audit (separate from physical collision tests), both destination runtime cases, blocked refunds, each checkpoint death/replay and a real no-debug route. Target25–32minutes remains unproven until final campaign/human pacing passes.


## M4 as built and verified
All planned mechanics/marks above now exist in the campaign. Actual Haven lamp is(128,3.5), Storm HIGH return(102.8,3); these positions separate nearby controls/icons. Reed ledger moved from(32,2) to(32,0) to clear the new room's south rail. Recall labels sit below controls; Haven LOW label sits below its mailbox to avoid the moth's name. Safe-exit protection keeps an already-open occupied doorway open; it never opens a closed shutter merely on approach. Only a settled Duckduckgoose and an inside inspection complete the perch.

Return sources show the current address and next reroute action. Loans never repeat after a route is completed. Four-mark badge is currently a text award on the HUD/journal; production badge icon is still requested. Journal pages use the existing menu with Space/arrows and Tab/Esc, with no duplicated UI coordinator. All new art is an explicitly labelled existing-sprite/geometric stand-in; source actor frames/importers are unchanged. Updated replacements are in ASSETS_NEEDED.md.

| Chapter | M3 active | M4 exploration route | Change |
| --- | ---: | ---: | ---: |
| Keeper's Landing | 30.53s | 32.28s | +1.75s |
| Listening Reeds | 20.76s | 34.36s | +13.60s |
| The Old Archive | 56.50s | 58.71s | +2.22s |
| Storm Locks | 22.56s | 40.64s | +18.07s |
| Lantern Haven | 26.42s | 43.03s | +16.62s |
| Total | 156.77s | 209.02s | +52.25s |

M4 route includes all8marks, optional Storm HIGH and Haven LOW, active enemies, instant story advancement and1 deliberate loan/death.209.02active seconds is NOT a human first-time duration. Reading/learning/combat experimentation require a human timed run. The final25–32minute target remains unverified pending M5/M6/M7; movement and empty trail distances were not padded. M4 tests98/98;124physical assertions;203abstract resource states all recover/finish. Scene remains an intermediate playable reunion endpoint, not a claim that the planned boss campaign/climax is already built.

# Marshlight — mailbox, signposts and sword
6 November 2026

## What changed
Ada's letterbox now has a 32x48 pixel mailbox; four story signs use matching timber signposts. All six original/relay return boxes reuse the mailbox head with their existing coin markers. Three new PNGs use a shared sixteen-entry palette, crisp alpha, Point filtering, no compression and 32 PPU. Existing outdoor sheets were reused without downloading or replacing them.

Aiken can collect Ada's Reedkeeper Sword beside the first trail. It enters the existing inventory and equips automatically. The dedicated equipment box sits under the brass counter; the world sword and gold swipe arc appear only during an active swing. Q packs/equips it. J or left click swipes toward the last movement direction.

Each swipe deals 1 HP damage, has a 1.15-unit range, 100-degree arc, 0.20-second visible swing and 0.50-second cooldown. Mites retain their existing 3-HP health/death system. HP pips, a brief hit flash, particles and a soft synthesized swish provide feedback. There is no stun or HP regeneration; defeated mites stay removed until a fresh replay. A single swipe hits each enemy only once, even across frames or multiple colliders, and cannot hit through solid banks.

The sword borrows one carried coin and returns it after the swipe. Pause, dialogue, death or packing it cancels the swipe and refunds its coin. If an artificially full inventory prevents a refund, the coin remains in explicit weapon escrow, the HUD reports it and refund retries when space returns. Another attack and final victory cannot consume or discard that unsettled coin. No carried coin means no attack: reclaim a coin before fighting when every coin is lent. Defeating every patrol leaves all three bells and the archive exchange required.

One concise two-page sword note from Ada raises the story total from eleven to twelve. Aiken, Duckduckgoose, Ada and the existing story beats remain. Shields and additional weapons were deferred.

## How to try it
Open Assets/_Agent/Scenes/Marshlight.unity and press Play. Begin the game, walk north past Ada's mailbox and collect the sword near the first trail. Collect at least one brass coin before swiping. Move toward a mite, press J or left click, and watch its HP pips. Press Q to compare equipped/packed states. Continue lending and reclaiming coins through the original crossings and archive exchange.

WASD/arrows move; E interacts; Space/Enter advance text; Esc pauses; R restarts after defeat. Development-only F6 cycles checkpoints and F7 shows pacing. The equipment icon remains visible while equipped; the blade itself is hidden between swipes.

## Existing scripts modified

| File | Targeted extension |
| --- | --- |
| Assets/Scripts/ScriptableObjects/Item.cs | Appends SWORD=2; existing COIN=0 and HEALTH=1 values preserved. |
| Assets/Scripts/MonoBehaviours/Player.cs | Routes sword pickups through existing AddItem; adds ItemCollected(Item), fired after successful collection. |
| Assets/_Agent/Scripts/MarshLevel.cs | Serialized Weapon/public getter, weapon escrow in the coin ledger, cancel/refund at modal/replay boundaries, victory settlement check and J/Q controls. |
| Assets/_Agent/Scripts/Editor/MarshVerification.cs | Extends the existing physical walker with sword pickup/story, actual hit, visibility/refund assertions and an active-swipe capture. |
| Assets/_Agent/Tests/PlayMode/MarshCoinRegressionTests.cs | Keeps existing cases, updates twelve-entry story expectation and adds fifteen permanent sword regressions. |

Twenty-five of thirty pre-pass scripts remain byte-for-byte unchanged, including EnemyHitPoints, Inventory, MovementController, MarshPatrol and the existing player health base. No public method was removed or renamed. Five new scripts are WeaponData, Weapon, MarshEquipmentHUD, MarshEnemyFeedback and the edit-time MarshSwordBuilder. Weapon values, item/sprite/sound and optional projectile reference live in Assets/_Agent/Data/ReedkeeperSword.asset; projectile is empty for this melee weapon.

All fifty-eight prior colliders remain unchanged. Only the pickup's trigger was added. One Weapon and one equipment panel reuse the existing Player, Canvas, inventory and managers. The original LevelOne scene is retained. Source/scene/story backups and audit evidence are under Assets/_Agent/Data/SwordPass20261106/.

## Verification
| Check | Before | After |
| --- | ---: | ---: |
| Complete PlayMode suite | 23 passed | 38 passed; 0 failed/skipped |
| Full physical no-F6 walker | 89 assertions | 97 assertions; 0 failures |
| Runtime errors / warnings | 0 / 0 | 0 / 0 |
| Original collider changes | 0 | 0; one new pickup trigger |

Final complete PlayMode rerun: job 5117a567170e48c398358895cbc840d0, SDK duration 16.270 seconds, 38 passed with no failures/skips. Detailed results are in Data/SwordPass20261106/PlayModeTests.json. Unity's SDK reports zero in its NUnit assertion-count field, so test-case totals and the walker's assertion count are reported separately.

The 70-step full route covers 159.57 world units using real MovementController movement, physics, pickups, damage/death/restart, every crossing and all twelve story entries. It finishes with three coins, zero escrow and teleported=false. Screenshots and native console checks cover all four ordinary checkpoints, a real active swipe, defeat and victory. A separate fresh replay confirms empty sword ownership/escrow, closed bells and four full-health mites; all four F6 destinations were captured through the same debug entry point without granting coin/bell/checkpoint progress.

Section telemetry compares like-for-like active seconds: Landing 29.48 -> 29.74 (+0.25); Reedbank 27.76 -> 27.76; Archive 44.90 -> 44.90; Haven 3.60 -> 3.60. Total 105.74 -> 106.00 seconds. The route length is unchanged. The only measured section change is the short sword demonstration/refund wait in Landing. Ada's new note is 78 words (roughly 20–26 seconds at 180–240 words/minute); the walker skips reading. These are mechanical measurements and a reading estimate, not a new human pacing result. The existing 5–10 minute first-time target still requires a fresh timed playtest.

All eighteen captures, FullRoute.json, FullRoutePacing.json, MechanicalPacing.jsonl, BuildAudit.json, collider fingerprints, source/scene audits, replay/F6 checks and console summary are in Data/SwordPass20261106. New note and pause copy were visually reviewed at 1280x720; permanent text-fit tests cover all twelve entries and menus.

Permanent cases cover real pickup and automatic equip, duplicate pickup, idle/swing visibility, damage, cooldown, range/facing, wall protection, multiple colliders, enemy defeat, insufficient carried coins, pause/dialogue/death/unequip refunds, blocked-refund recovery, replay reset, equipment-panel text fit and defeating every patrol without crossing bypass. All original death/loan/checkpoint/exchange regressions remain.

An initial runner warning identified three builder audit files awaiting import. The builder now refreshes those files before tests; the complete clean rerun passes without that warning. No compile error occurred in project scripts.

## Known limits
No gameplay blocker was found in the tested Editor states. Hardware keyboard/mouse feel, audio balance, extra aspect ratios and a standalone player build still need human checks. The automated route uses real movement/physics/triggers and public interaction/attack APIs; it does not synthesize hardware key events. Its dialogue advances immediately, so simulated route time is not human play time.

This is one sword and one equipment slot. Shield/block/parry systems and other weapon variants were not built. Existing character animations and geometric bell/cup placeholders remain; only the sword swish was added, and master-volume/broader sound work remains backlog. ASSETS_NEEDED.md lists current placeholders, sizes, pivots, PPU, palettes and frame/audio specifications.

## Manual playtest checklist
1. Start a fresh run. Confirm Aiken/Duckduckgoose text, mailbox and signpost readability, and the empty-hand equipment state.
2. Collect the sword on the first trail. Read Ada's two pages. Confirm one sword in inventory, automatic equip and no blade beside idle Aiken.
3. Collect a coin. Move in each direction and swipe with both J and left click; verify the blade follows the last movement direction, appears briefly and disappears completely afterward.
4. Hit a mite three times. Check one pip lost per accepted swipe, visible hit feedback, audible swish, cooldown and eventual removal. Try an enemy behind Aiken, outside reach and beyond a solid bank.
5. Pack/equip with Q. Confirm panel status/icon changes and no attack while packed. Watch the coin leave/return around a swing without a duplicated coin.
6. Lend all carried coins to a bell or relay. Verify attack rejection, then reclaim a coin and attack successfully. Complete every required crossing even after defeating patrols.
7. Pause, read a note, pack the sword and take lethal damage during separate swings. Confirm the blade cancels and its coin returns. Restart at each reached checkpoint and check sword ownership, gate progress and the three-coin total.
8. Finish the level with three recovered coins and Duckduckgoose, then replay. Check the fresh sword pickup, empty inventory, full enemy HP and reset bell/exchange state.
9. Check the equipment box, dialogue and pause controls at another aspect ratio; assess sound volume. Use F6/F7 separately, then time a fresh full run without F6 for pacing.

## Suggested next three backlog items
1. Fresh timed human playtest with sword/coin comprehension and combat feel checks.
2. Complete the character/mechanism animation and art replacement specification.
3. Broader sound set plus pause-menu volume control.

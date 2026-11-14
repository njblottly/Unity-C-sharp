# Marshlight asset manifest
Updated 6 November 2026. Current scene: Assets/_Agent/Scenes/Marshlight.unity.

This lists the current assets under Placeholder, including the mailbox/signpost/sword and bell/letter/grounding passes. It does not mark the full character-animation or sound overhaul complete. Keep asset GUIDs, sprite names and serialized references when replacing these files. Existing outdoor sheets and original character art remain in use and do not need uploading again.

## New shared palette
Transparent plus these fifteen opaque colours: #142E39, #244653, #385E4A, #567546, #7D914D, #342F26, #665039, #927044, #B79758, #E2C57C, #F4E7C0, #71858B, #A8B6B0, #D45E46, #FFF7DC. New PNGs use binary alpha (0/255), Point filtering, no compression, no mipmaps, Full Rect meshes and 32 pixels per unit. Pivot coordinates below are in pixels from the bottom left of the sprite rectangle.

## Current placeholder sprites

| File path | Pixel size | Pivot | PPU | Palette | Animation / frames | Depiction and use |
| --- | --- | --- | ---: | --- | --- | --- |
| Assets/_Agent/Placeholder/Props/KeeperMailbox.png | 32x48 | (16, 6) | 32 | Shared palette, 11 opaque colours used | Static / 1 | Complete weathered teal/brass letterbox and post; the landing letterbox and all six coin returns use this same sprite/base. The letterbox now also carries the small FieldLetter sprite. |
| Assets/_Agent/Placeholder/Props/BellFrame.png | 32x48 | (16, 2) | 32 | Shared palette, 11 opaque colours | Static / 1 | Rounded brass bell, clapper and weathered timber frame; all three original bell stations, including Landing Bell. Coin-state marker remains on the lower frame. |
| Assets/_Agent/Placeholder/Props/FieldLetter.png | 32x32 | (16, 16) | 32 | Shared palette, 11 opaque colours | Static / 1 | Folded-corner parchment and coral wax seal on Ada's first mailbox; existing letter story opens through E. |
| Assets/_Agent/Placeholder/Props/MarshLantern.png | 32x48 | (16, 2) | 32 | Shared palette, 12 opaque colours | Static / 1 | Complete framed brass/teal lamp and timber post at four checkpoints and six haven lights; live mint/gold core stays inside the glass. |
| Assets/_Agent/Placeholder/Props/TrailSignpost.png | 32x48 | (16, 6) | 32 | Shared palette, 10 opaque colours used | Static / 1 | Mossy timber direction sign; reed, archive and storm notes plus the optional dry-ledger pocket. Text remains in dialogue/UI. |
| Assets/_Agent/Placeholder/Props/ReturnMailboxHead.asset | 32x24 | (16, 12) | 32 | Shared palette; region of KeeperMailbox | Static / 1, legacy unused | Historical head crop retained for rollback/evidence. No current scene renderer uses it; it omitted the post and caused the buried appearance. |
| Assets/_Agent/Placeholder/Weapons/ReedkeeperSword.png | 32x32 | (16, 6) | 32 | Shared palette, 8 opaque colours used | Static / 1; runtime swipe rotation | Straight steel blade, brass guard and reed-green grip; ground pickup, inventory/equipment icon and active swipe. Hidden on the player between swings. |
| Assets/_Agent/Placeholder/BrassCoinIcon.asset | 16x16 | (8, 8) | 16 | #985838, #CFD526, #E89840, #F2F840, #FEFFA4, transparent; unchanged | Static / 1 | Clear brass coin in inventory, mechanism markers and return boxes. Region (8,72,16,16) of Assets/Sprites/Objects/hearts-and-coins32x32.png. |
| Assets/_Agent/Placeholder/Square.png | 32x32 | (16, 16) | 32 | White #FFFFFF, tinted per renderer | Static / 1 | Reusable geometric relay cups, causeway planks, small lantern cores, mechanical links, HP pips and particles. Bell frames and full lanterns now have dedicated pixel sprites. Replace each semantic prop through its own sprite reference rather than replacing this shared square with a single illustrated prop. |
| Assets/_Agent/Placeholder/MarshGrass.png | 32x32 | (16, 16) | 32 | #36574A, #416255 | Static / 1 | Legacy grass tile retained for earlier scenes/evidence; current outdoor ground uses the project's existing sheet. |
| Assets/_Agent/Placeholder/NightWater.png | 32x32 | (16, 16) | 32 | #133340, #1F3E4B | Static / 1 | Legacy water tile retained for earlier scenes/evidence; current water uses the project's existing sheet. |

Assets/_Agent/Placeholder/MarshSprite.mat is the shared sprite/particle material, not an additional image. The swipe's arc is a procedural LineRenderer, not a texture or authored animation clip. No new character animation clips were created in this pass.

## Current placeholder sound

| File path | Format / length | Playback | Description and use |
| --- | --- | --- | --- |
| Assets/_Agent/Placeholder/Weapons/ReedkeeperSwipe.wav | Mono PCM16, 44,100 Hz, 6,174 samples / 0.14 s | One-shot, 2D, WeaponData SoundVolume 0.25 | Code-synthesized soft blade swish for an accepted sword attack. No external audio download. |

The sword sound reference is in Assets/_Agent/Data/ReedkeeperSword.asset. Replace the WAV while retaining its .meta/GUID. Footsteps, coin lend/reclaim, bell, dialogue blip, hit, defeat and victory sounds are still future assets; there are no fabricated paths for nonexistent files. A pause-menu volume control remains backlog work.

## Existing art reused directly

- Outdoors: the six existing sheets under Assets/Sprites/Outdoors, assembled through Assets/_Agent/Sprites/OutdoorReuse. See Data/OutdoorArt20261106 for exact source regions/importer audit. Do not reslice or overwrite the original sheets as part of this manifest.
- Aiken: Assets/Sprites/Player/player.png and existing animations/controllers.
- Duckduckgoose/flock: Assets/Sprites/Companions/duck 1.png and existing animation setup.
- Patrols: Assets/Sprites/Enemies/EnemyIdle_1.png and existing animation setup; new HP pips/flash are separate presentation components.
- Coin/health pickups: existing Objects sheets. These are retained original assets, not newly authored placeholders.

Future character/bell animation commissions need an expanded frame-by-frame specification before art replacement. Shields, weapon variants, extra sound sets and incoming-art imports were not built in this pass.


## Campaign expansion: planned assets, not created in Milestone 0
The following are specifications for later milestones. None of these paths exists because of M0. Existing assets listed above retain their names, GUIDs, importers and references. Current actor measurements are in DESIGN.md/Data/Milestone0/ArtAudit.json; Aiken is not actually a uniform 32x32 source sheet.

New enemy/collectible frames: 32x32 at 32 PPU, centre pivot (16,16), Point, uncompressed, no mipmaps, Tight mesh matching current actors; black #000000 one-logical-pixel outline, limited flat colour clusters. Use the future actor palette recorded in DESIGN.md. Bosses retain this grid using deliberate visual scaling or modular pieces; do not scale a Rigidbody root merely to enlarge the image. Animations use 12 fps as a visual starting point; attack telegraph/recovery durations come from data and may hold frames longer.

| Planned file under Assets/_Agent/Placeholder/Expansion/ | Frame size | Planned frame count | Animation names and counts | Description / intended chapter |
| --- | --- | ---: | --- | --- |
| ReedSkitter.png | 32x32 | 20 | idle 4, walk 4, windup 3, lunge 2, recovery 2, hit 1, retreat 4 | Low reed-backed mite with a clear crouch/lunge silhouette; ch.1 onward, M2. |
| SiltCrab.png | 32x32 | 23 | idle 4, walk 4, guard 2, windup 3, claw 3, recovery 2, hit 1, retreat 4 | Broad clay/brown crab with an obvious raised claw and visible flanks; ch.2 onward, M2. |
| WickMoth.png | 32x32 | 21 | idle 4, fly 4, windup 3, dive 3, recovery 2, hit 1, retreat 4 | Pale marsh moth with brass wing marks and a readable dive warning; ch.3 onward, M3. |
| CounterweightSprout.png | 32x32 | 20 | idle 4, walk 4, windup 3, fire 2, recovery 2, hit 1, retreat 4 | Reed/brass sprout with a swelling seed pod; ch.4 onward, M3. |
| SproutSeed.png | 32x32 | 4 | travel 2, impact 2 | Simple outlined seed projectile, terrain-blocked; M3. |
| SiltlockTender.png | 32x32 visual grid | 29 | idle 4, walk 4, charge-windup 3, charge 3, sweep-windup 3, sweep 3, recovery 3, hit 1, rest 5 | Large jammed sluice guardian, readable claws/weights; end ch.3, M5. |
| StormBellEngine.png | 32x32 visual grid | 34 | idle 4, phase1-warning 3, phase1-attack 3, phase2-warning 3, phase2-attack 3, phase3-warning 4, phase3-attack 4, recovery 3, hit 1, rest 6 | Anchored damaged bell mechanism with clear lane/phase signals; ch.5, M5. |
| ReedMark.png | 32x32 | 4 | idle 4 | Optional carved feather token; eight unique world instances, journal/cosmetic reward, M4. |
| DuckPerch.png | 32x32 | 2 | empty 1, occupied 1 | Dry reed pressure perch with distinct occupied state; ch.2 tutorial, M4. |
| InspectionLantern.png | 32x32 | 3 | off 1, lit 2 | Anchored inspection lamp; its coin can always be reclaimed; ch.3, M4. |
| ReturnRouteSelector.png | 32x32 | 2 | route-a 1, route-b 1 | Clear two-destination mail selector with visible pointer/coin link; ch.4, M4. |
| ReedMarkBadge.png | 32x32 | 1 | static 1 | Cosmetic brass duck badge after four marks; existing HUD/journal presentation, M4. |

Do not create replacement Aiken/duck animation sheets until the visual adapter and source-frame mismatch are resolved at the relevant art milestone. If matching art is unavailable, use clearly labelled placeholder files with these specifications and record actual delivered counts, rather than claiming final-quality replacements.

Planned sound requests, not files: skitter warning/lunge, crab guard/claw, moth warning/dive, seed fire/impact, boss phase warning/recovery, perch/recall, lamp on/refund, route selector, Reed Mark pickup, journal reward and the previously listed bell/coin/dialogue/hit/defeat/victory set. Keep the existing sword swish. Synthesized or supplied sounds only; no internet downloads. Master volume remains M7 (or an explicitly scoped earlier audio task).

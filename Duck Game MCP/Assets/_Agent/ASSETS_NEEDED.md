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

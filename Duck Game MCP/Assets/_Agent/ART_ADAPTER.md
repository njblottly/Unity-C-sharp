# Actor art adapter specification
6 November 2026. Specification only; no actor renderer migration is implemented in Milestones1–4.

## Measurements retained
Aiken's source sheet955x1453 uses irregular approximately186–227x288–358 frames at32PPU, root scale0.065; sampled frame227x358, centre pivot113.5/179, approximately0.461x0.727world units. Its15,606 sampled opaque colors include antialiased edges. Duck sheet128x160 uses32x32 frames at32PPU, root0.48, grey/orange palette and no continuous black outer stroke. Mite frames32x32 at32PPU, root0.72, four-color green/black/white palette. All use Point/uncompressed/no mipmaps. Exact audit is Data/Milestone0/ArtAudit.json and DESIGN.md.

## Future visual-only migration
Keep the actor root transform, Rigidbody2D, collider geometry, movement speed, health, inventory, weapon origin, camera target and spawn coordinates unchanged. Introduce a named Visual child in the campaign instance. Put the replacement SpriteRenderer there, set its visual scale/offset to the current world-space silhouette envelope, and assign explicit references in feedback components. Do not resize a physics root to fit a32x32 sprite.

Keep Animator/controller state names expected by MovementController and DuckFollower. Copy controllers/clips under Assets/_Agent, remapping SpriteRenderer animation bindings to Visual; do not overwrite original clips/controllers. Reuse explicit frame mapping data for source sprite IDs to new frames rather than relying on child indices, arbitrary sheet order or sprite names guessed from incoming files. Prefer rebinding the renderer over a per-frame mirror component. Update every serialized renderer consumer before removing an old campaign renderer; no component is disabled to conceal a broken reference.

For Aiken, a32PPU replacement has a1-unit logical frame; visual scale/offset must reproduce the roughly0.73-unit silhouette without changing the0.065 physics root. Final fitting depends on the supplied transparent margins and feet position, not just the canvas. Record pivot, occupied pixel bounds and world footprint per animation. Duck/mite frames already use the32x32 family and need no root-scale conversion.

## Acceptance
Capture all directional idle/walk frames before/after; compare silhouette centre/feet, sword alignment, HP/HUD feedback and camera framing. Assert collider/body/weapon/spawn fingerprints unchanged. Re-run movement, contact, sword range/occlusion, pickup, gate and checkpoint regressions plus full route. Validate artist sheet dimensions/names/frame counts/imports against ASSETS_NEEDED before swap; report mismatches instead of guessing.

New enemy/collectible art in M2–M4 may use clearly labelled assemblies of current sprites as placeholders. Record underlying32x32 frame source, static/procedural state behavior, palette and replacement specification. Do not claim final commissioned animations or normalize the existing player as part of enemy/gameplay work.

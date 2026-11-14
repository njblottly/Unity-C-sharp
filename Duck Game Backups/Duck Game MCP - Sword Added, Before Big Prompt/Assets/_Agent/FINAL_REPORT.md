# Marshlight: The Borrowed Bells — final report
Completed 3 November 2026

## What was built
A complete top-down 2D rescue level in Assets/_Agent/Scenes/Marshlight.unity. Rowan and Pip reopen the storm-damaged route to Lantern Haven and reunite four ducklings with their flock. The original LevelOne scene remains available.

The borrowed-bell mechanic uses the existing inventory: collect three coins, lend them to a bell to lower a causeway, cross, and reclaim the same coins from its return tray. Repeat for three crossings. Win requires every bell and return tray, plus reaching the haven with Pip nearby.

The level includes four safe checkpoints, four patrol hazards using the existing enemy sprite and contact damage, three healing pickups, nine data-driven story entries, objectives, interaction prompts, a five-slot inventory, health display, start/pause/defeat/victory screens, particle bursts and a damage flash. Checkpoints restore health. Defeat restarts the same player and duck while retaining coins and latched crossings.

## How to play
1. Open Assets/_Agent/Scenes/Marshlight.unity and press Play. Click the Game view so it receives keyboard input.
2. Press Enter or click Begin. Use WASD or arrow keys to move.
3. Follow the raised trail to collect the three brass coins. Press E near notes, bells and return trays.
4. Use Space, Enter or E to advance dialogue. Esc pauses/resumes.
5. After defeat, press R or click Try Again. After victory, press R or click Play Again.
6. F6 cycles checkpoint teleport destinations in Editor and development builds. It does not grant progression; avoid it during a normal playthrough.

Marshlight is the startup build scene. The original LevelOne scene remains in the build list.

## What was tested
The final Editor-only autopilot passed 66 assertions while walking 133.65 world units through the real MovementController, player loop, colliders, pickup triggers and story interactions. The full route used no teleport or progression grants. A temporary one-coin debit/refund exercised the insufficient-funds case without adding net currency.

Coverage includes spawn, native enemy contact damage, coin collection and stacking, failed spend, all bell payments and physical gate collisions, all coin returns and duplicate prevention, every story entry, every checkpoint, pause/resume, health depletion, checkpoint restart, retained progression, duck placement, single UI/manager instances and reachable victory. Screenshots and console checks were taken at all four checkpoints, with extra title, defeat and victory images.

Additional tests passed for the serialized menu button binding, win-menu fresh-run reset, pause and defeat primary actions, and all four development teleport destinations. Teleport could not bypass progression.

Final native Unity console counts: 0 errors and 0 warnings. Ten original scripts are byte-for-byte unchanged; six contain targeted extensions listed in PROGRESS.md. No existing public method was removed or renamed.

Evidence: Data/Verification/FinalPass.json, ExtraVerification.json, SourceAudit.json, ConsoleSummary.json and Final_*.png.

## Known issues and limits
No gameplay blocker was found in the mechanical Editor tests. New world art is intentionally replaceable placeholder art. There were no source AudioClip assets, so this version has no sound. The target first playthrough is 5–10 minutes; a human has not timed it. The automated run skips reading pauses. Keyboard feel, additional aspect ratios and a standalone player build have not been manually tested.

## Human playtest checklist
- [ ] Complete a first run without F6; time it including reading and exploration.
- [ ] Check movement, corners, camera follow and Pip's behavior around reed banks.
- [ ] Confirm the three coins are easy to spot, the E prompt is clear and the same coins are recovered at every tray.
- [ ] Deliberately die after lending coins but before reclaiming them; restart and recover them through the still-open crossing.
- [ ] Read each note and dialogue page; check HUD/menu text at your usual resolution and one other aspect ratio.
- [ ] Try keyboard and mouse menu actions, pause/resume, defeat restart, the ending and Play Again.

## Persistent records
DESIGN.md contains the concept, mechanic, story bible, layout and conditions. PROGRESS.md contains milestones, public API changes, QA corrections and remaining human checks. Data/EXPLORATION.md inventories the baseline project and input setup.

All new scripts/assets are under Assets/_Agent/. Existing scripts were extended in place. The original scene, original prefabs and original sprite sources were retained. No external assets or packages were downloaded.

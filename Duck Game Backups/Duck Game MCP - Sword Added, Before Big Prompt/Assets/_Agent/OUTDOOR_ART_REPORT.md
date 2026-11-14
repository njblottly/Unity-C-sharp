# Marshlight — outdoor art and character names
6 November 2026

## What changed
The keeper is now **Aiken**, and the companion is **Duckduckgoose**. All eleven story entries, speakers, title/defeat/victory copy, objectives and the two story builders use the new names. Ada and the story beats remain intact. The current design bible is updated; historical reports, screenshots and backups retain the names used when they were recorded.

The environment now reuses the outdoor sheets already in the project. Grass, dirt and water fills replace generated textures; complete irregular grass patches and tufts break up the terrain; stone trail art replaces square trail markers; shrubs and grass dress the existing reed barriers; and trees, boulders, stumps and fallen logs frame the solid banks. Cool tints keep the dusk setting and warm brass markers readable.

| Existing sheet | Use |
| --- | --- |
| Ground Tileset 1.1 | Grass/dirt/water fills, shore fringes and complete grass patches |
| Rocks 1.1 | Two stone trail variants and bank boulders |
| Bushes 1.1 | Dense vegetation over the existing solid reed banks |
| Nature Details | Marsh grass tufts |
| Trees 1.2 | Three assembled tree variants along the outer banks |
| Tree Stumps and Logs | Stumps and fallen logs at the bank edges |

There are 525 existing renderer replacements and 531 new decorative renderers. Thirteen assembled Sprite assets live under Assets/_Agent/Sprites/OutdoorReuse/. These reference whole regions of the original textures at 32 PPU; the original PNGs, sprite slices, GUIDs and importer settings are preserved. The original art textures are reused directly; there were no downloads. New scene dressing is organized under Marshlight_World/10_OutdoorArt.

All 58 existing colliders retain their serialized configuration, transforms and world bounds. The new dressing has no colliders or runtime behaviours. Movement, patrols, pickups, checkpoints, camera bounds, gate dependencies, refundable coin economy and the three crossings are preserved. Brass bells/cups/trays, lanterns and mechanical linking lines remain distinct puzzle markers.

## Scripts changed
- MarshLevel.cs: character-name text only in title, defeat, victory, objective and companion-wait feedback.
- Editor/MarshBuilder.cs and Editor/MarshIteration1Builder.cs: story-generation strings renamed so a rebuild cannot restore the former names.
- Tests/PlayMode/MarshCoinRegressionTests.cs: added two regression cases for visual-only dressing and renamed text/panel fit.
- New Editor/MarshOutdoorArt.cs: edit-time assembly and assignment of existing sprites, hierarchy/manager checks, source-file and collision audits.

No existing public gameplay API changed. Twenty-five of the twenty-nine pre-pass scripts are byte-for-byte unchanged. The three production/builder edits are text-only; the fourth change extends the test fixture.

## Verification
Baseline: 21/21 PlayMode cases passed. After the final visual refinements: **23/23 passed**, zero failed or skipped. The added cases inspect the real story and title/defeat/victory text, check every page and speaker fits its panel, and verify the outdoor group uses original project textures at 32 PPU with no collision or runtime logic.

The existing mechanical suite remains **89/89 assertions passing** (89 before this pass, 89 after). The complete title-to-victory route walked 159.57 world units without F6, fired all eleven story entries, traversed all four checkpoints, exercised death/restart and settled every loan with exactly three coins. Its active simulation time was 105.31 seconds with dialogue advanced immediately; this is not a human pacing measurement. Final native Unity console: **0 errors, 0 warnings**.

Before/after collision signatures match both for the main art pass and the ground refinement. The source-file audit confirms original sheet/importer files unchanged. All four F6 checkpoint views were captured and their native console counters read: 0 errors and 0 warnings at each. The longer character names were also reviewed visually on the title, intro, defeat and victory screens.

A Unity MCP WebSocket reconnect warning was observed during a script/domain reload; the connection recovered. It was recorded before the clean runtime verification. No project compile error or new gameplay-code warning remains. A preview caught partial grass-patch shapes and over-bright ground; complete patch regions and a darker tint corrected those before final verification.

Evidence is under Assets/_Agent/Data/OutdoorArt20261106/: baseline scene/scripts/story/tests, ArtAudit.json with exact source rectangles, collision signatures, source/name audits, final test and walker reports, console checks and screenshots.

## How to try it
Open Assets/_Agent/Scenes/Marshlight.unity and press Play, then Begin/Enter. Move with WASD/arrows, interact with E, advance text with Space/Enter/E, and pause with Esc. R retries after defeat or starts a fresh run after victory. F6 cycles development checkpoint destinations and F7 toggles pacing telemetry.

## Human checklist and limits
1. Read the intro, optional ledger page and ending; confirm Aiken and Duckduckgoose appear consistently and text remains comfortable to read at your usual resolution.
2. Walk the north and south routes to inspect the trees, rocks, shrubs and trail contrast. Check that vegetation never conceals a coin, prompt or patrol.
3. Complete the exchange and all three crossings without F6; deliberately die with coins lent, restart, recover all coins and finish with Duckduckgoose.
4. Check another aspect ratio and compare the dusk palette with your preferred look. Try Play Again and confirm the dressed environment and fresh progression reset correctly.

The brass mechanisms, lantern geometry and UI still use their readable placeholders. Player/duck art, animation, sound and weapons were outside this requested environment pass. No standalone player build or new human pacing measurement was produced. Future work remains in BACKLOG.md.

## Suggested next backlog items
1. Finish the remaining art manifest, character animation and sound work; the environment reuse is now complete.
2. Design the coin-borrowing keeper tool with patrol feedback and tests preserving every crossing dependency.
3. Validate incoming art/audio against the manifest before replacing asset references.

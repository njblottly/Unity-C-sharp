# Milestone 1 report
Complete. M2–M4 remain authorized; M2 starts next. No git commits.

Built a separate five-chapter campaign scene with five ordered three-coin crossings, six safe checkpoints, the retained archive exchange and the existing player/duck/health/inventory/sword/menu systems. Added chapter/section/campaign ScriptableObjects and extended the existing story entries with prerequisite, capability and once policies. All ActI/II bindings are placed; perch/lamp lessons remain inactive until M4. The original Marshlight scene is byte-for-byte unchanged. Campaign is now first in build settings; input/packages unchanged.

Open Assets/_Agent/Scenes/Marshlight_Campaign.unity, Play and Begin. Existing WASD/E/Space/J/Q/Esc controls remain, F6 cycles six checkpoints and F7 toggles pacing. Future art migration is specified in ART_ADAPTER.md, with no actor reslicing.

Existing source extensions: MarshStory (entry policies/validation), MarshLevel (optional campaign counts/story interactions/HUD), MarshNode (configured archive/story checks), MarshPacingTelemetry (dynamic section names). No old public method removed; API additions recorded in PROGRESS.

Tests41 to56, all pass; final jobcd1710101e25488eb6fc8dbbde99fc4d, 17.990s. All existing cases retained. Added story/configuration/gate ordering, six checkpoint outstanding-loan recovery and campaign replay cases. Physical no-F6 title-to-victory:75/75 checks,70steps, three coins/zero escrow, all available stories.21screenshots across six normal checkpoints/win, six debug destinations and eight gates; console0errors/0warnings. Full route preceded final objective/bridge presentation fixes; final tests after fixes pass.

| Chapter | Active simulated seconds |
| --- | ---: |
| Keeper's Landing | 27.98 |
| Listening Reeds | 18.40 |
| The Old Archive | 52.92 |
| Storm Locks | 22.28 |
| Lantern Haven | 26.48 |

Total148.06s; text instant, one intentional death, capture holds excluded, not a human estimate. This first walk used editor stepping; future normal-clock PlayerLoop driving removes idle-editor delay without changing gameplay speed.

Limits: campaign remains an intermediate scaffold; bosses, final story expansion and visual themes belong to M5/M6.25–32minutes is still a future target. Human pacing/aspect ratios/standalone build remain unverified. Debug inspections invoke the guarded F6 entry point; hardware delivery remains manual.

Manual checks:1.Start fresh and follow all five bells.2.Read notes and observe chapter/objective changes.3.Die with coins lent at each checkpoint and reclaim them.4.Complete relay, reach reunion and replay.5.Check F6/F7 and run56PlayMode tests.

Next:M2 shared configurable enemy framework plus Skitter/Crab; then M3 and M4 within current authorization. No M5 work.

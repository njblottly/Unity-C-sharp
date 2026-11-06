using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DuckAdventure.Agent
{
    public enum MarshState { Intro, Playing, Dialogue, Paused, Defeat, Won }

    [DefaultExecutionOrder(100)]
    public sealed class MarshLevel : MonoBehaviour
    {
        [Header("Existing systems")]
        [SerializeField] private Player player;
        [SerializeField] private MovementController movement;
        [SerializeField] private DuckFollower duck;
        [SerializeField] private RPGGameManager gameManager;
        [SerializeField] private RPGCameraManager cameraManager;
        [SerializeField] private Item coinItem;
        [Header("Level data")]
        [SerializeField] private MarshStory story;
        [SerializeField] private MarshCoinRelay coinRelay;
        [SerializeField] private Weapon weapon;
        [SerializeField] private Consumable[] coinPickups;
        [SerializeField] private MarshNode[] nodes;
        [SerializeField] private SpawnPoint[] checkpoints;
        [SerializeField] private string[] checkpointNames;
        [SerializeField] private Vector2 havenPosition;
        [Header("HUD")]
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private Canvas inventoryCanvas;
        [SerializeField] private Text chapterText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text coinText;
        [SerializeField] private Text promptText;
        [SerializeField] private Text toastText;
        [SerializeField] private Image damageFlash;
        [SerializeField] private Text ledgerText;
        [Header("Text and menus")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Text menuEyebrow;
        [SerializeField] private Text menuTitle;
        [SerializeField] private Text menuBody;
        [SerializeField] private Text primaryLabel;
        [SerializeField] private ParticleSystem feedbackParticles;

        private MarshStory.Entry activeEntry;
        private int page;
        private float toastUntil;
        private float lastHealth;
        private float elapsed;
        private bool endingDialogue;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private int debugCheckpoint;
        private MarshPacingTelemetry pacing;
        public MarshPacingTelemetry Pacing => pacing;
#endif
        private readonly HashSet<string> readStories = new HashSet<string>();
        private readonly HashSet<string> reached = new HashSet<string>();
        public MarshCoinRelay CoinRelay => coinRelay;
        public Weapon Weapon => weapon;
        public int CoinsInEscrow
        {
            get
            {
                int total = coinRelay.CoinsInEscrow + weapon.BorrowedCoins;
                foreach (MarshNode node in nodes) total += node.OutstandingCoins;
                return total;
            }
        }
        public int RecoverableCoins
        {
            get
            {
                int total = player.PlayerInventory.Count(Item.ItemType.COIN) + CoinsInEscrow;
                foreach (Consumable pickup in coinPickups) if (pickup.gameObject.activeSelf) total++;
                return total;
            }
        }
        public void AssertCoinConservation()
        {
            if (RecoverableCoins != 3) throw new InvalidOperationException("Marshlight must conserve three recoverable coins; found " + RecoverableCoins);
        }
        public void RecordCoinAction(string action, int amount, string nodeId)
        {
            AssertCoinConservation();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            pacing.WriteEvent(action, amount, nodeId);
#endif
        }
        public Player Player => player;
        public DuckFollower Duck => duck;
        public Item CoinItem => coinItem;
        public MarshNode[] Nodes => nodes;
        public SpawnPoint[] Checkpoints => checkpoints;
        public MarshState State { get; private set; }
        public int BellsOpened { get; private set; }
        public int CheckpointIndex { get; private set; }
        public bool CanExplore => State == MarshState.Playing;
        public int StoryCount => readStories.Count;
        public bool HasRead(string id) => readStories.Contains(id);
        public bool HasCheckpoint(string id) => reached.Contains(id);
        public float Elapsed => elapsed;

        private void Start()
        {
            ValidateReferences();
            player.Died += OnPlayerDied;
            lastHealth = player.hitPoints.value;
            State = MarshState.Intro;
            dialoguePanel.SetActive(false);
            SetFrozen(true);
            ShowMenu("A DUCK ADVENTURE", "MARSHLIGHT", "The Borrowed Bells\n\nThe storm has silenced Rill Marsh. Aiken and Duckduckgoose must reopen the old causeways and bring the flock home.\n\nWASD / arrows: move    E: interact    Space / Enter: read\nJ / left click: swipe    Q: equip    Esc: pause\n\nFind Ada's sword and three reusable coins. Every loan comes back.", "BEGIN  /  ENTER");
            UpdateHud();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            MarshPacingTelemetry[] existing = FindObjectsByType<MarshPacingTelemetry>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (existing.Length > 1 || (existing.Length == 1 && existing[0].gameObject != gameObject))
                throw new InvalidOperationException("Stale or duplicate pacing telemetry found.");
            pacing = existing.Length == 1 ? existing[0] : gameObject.AddComponent<MarshPacingTelemetry>();
            pacing.Configure(this, hudRoot.transform, objectiveText.font);
#endif
        }

        private void ValidateReferences()
        {
            if (weapon == null || coinRelay == null || coinPickups == null || coinPickups.Length != 3 || player == null || movement == null || duck == null || gameManager == null || cameraManager == null || coinItem == null || story == null || nodes == null || checkpoints == null || checkpoints.Length != 4)
                throw new InvalidOperationException("MarshLevel has missing gameplay references. Run Tools/Duck Adventure/Validate Marshlight in Edit Mode.");
            if (hudRoot == null || inventoryCanvas == null || objectiveText == null || chapterText == null || coinText == null || promptText == null || toastText == null || dialoguePanel == null || speakerText == null || dialogueText == null || menuPanel == null || menuTitle == null || menuBody == null || primaryLabel == null || damageFlash == null || ledgerText == null || feedbackParticles == null)
                throw new InvalidOperationException("MarshLevel has missing UI references.");
        }

        private void Update()
        {
            if (State == MarshState.Intro && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) Begin();
            else if (State == MarshState.Dialogue && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))) AdvanceStory();
            else if (State == MarshState.Defeat && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return))) RestartCheckpoint();
            else if (State == MarshState.Won && Input.GetKeyDown(KeyCode.R)) RestartRun();
            else if ((State == MarshState.Playing || State == MarshState.Paused) && Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (CanExplore)
            {
                elapsed += Time.deltaTime;
                MarshNode nearest = null;
                float distance = 1.5f;
                foreach (MarshNode node in nodes)
                {
                    if (!node.IsInteractive) continue;
                    float d = Vector2.Distance(player.transform.position, node.transform.position);
                    if (d < distance) { nearest = node; distance = d; }
                }
                promptText.text = nearest == null ? "WASD MOVE   E INTERACT   J / CLICK SWIPE   Q EQUIP   ESC PAUSE" : nearest.Prompt;
                if (nearest != null && Input.GetKeyDown(KeyCode.E)) nearest.TryInteract();
            }
            else promptText.text = "";
            if (player.hitPoints.value < lastHealth) { Pulse(player.transform.position, new Color(1f, 0.35f, 0.27f)); damageFlash.color = new Color(1f, 0.2f, 0.15f, 0.22f); }
            lastHealth = player.hitPoints.value;
            Color flash = damageFlash.color;
            flash.a = Mathf.MoveTowards(flash.a, 0f, Time.unscaledDeltaTime * 0.7f);
            damageFlash.color = flash;
            if (Time.unscaledTime > toastUntil) toastText.text = "";
            UpdateHud();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Input.GetKeyDown(KeyCode.F7)) pacing.ToggleOverlay();
            if (CanExplore && Input.GetKeyDown(KeyCode.F6))
            {
                debugCheckpoint = (debugCheckpoint + 1) % checkpoints.Length;
                DebugTeleport(debugCheckpoint);
            }
#endif
        }

        public void Begin()
        {
            if (State != MarshState.Intro) return;
            menuPanel.SetActive(false);
            hudRoot.SetActive(true);
            inventoryCanvas.enabled = true;
            State = MarshState.Playing;
            ShowStory("intro");
        }

        public void PrimaryAction()
        {
            if (State == MarshState.Intro) Begin();
            else if (State == MarshState.Paused) TogglePause();
            else if (State == MarshState.Defeat) RestartCheckpoint();
            else if (State == MarshState.Won) RestartRun();
        }

        public void ShowStory(string id)
        {
            activeEntry = story.Get(id);
            readStories.Add(id);
            page = 0;
            State = MarshState.Dialogue;
            SetFrozen(true);
            hudRoot.SetActive(true);
            inventoryCanvas.enabled = true;
            dialoguePanel.SetActive(true);
            RenderPage();
        }

        private void RenderPage()
        {
            speakerText.text = activeEntry.speaker.ToUpperInvariant();
            dialogueText.text = activeEntry.pages[page];
        }

        public void AdvanceStory()
        {
            if (State != MarshState.Dialogue) return;
            page++;
            if (page < activeEntry.pages.Length) { RenderPage(); return; }
            dialoguePanel.SetActive(false);
            if (endingDialogue) { ShowWinMenu(); return; }
            State = MarshState.Playing;
            SetFrozen(false);
        }

        public void BellOpened(MarshNode node)
        {
            BellsOpened++;
            Notify("Bell " + BellsOpened + "/3 rings. Cross east and reclaim your coins.");
        }

        public void ReachCheckpoint(MarshNode node, SpawnPoint spawn)
        {
            CheckpointIndex = node.Sequence;
            gameManager.SetPlayerSpawnPoint(spawn);
            reached.Add(node.Id);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            pacing.ReachedCheckpoint(CheckpointIndex);
#endif
            player.Heal(Mathf.CeilToInt(player.maxHitPoints));
            Notify("Safe landing: " + checkpointNames[CheckpointIndex] + ". Health restored.");
            Pulse(spawn.transform.position, new Color(0.55f, 0.95f, 0.77f));
        }

        public void Notify(string message)
        {
            toastText.text = message;
            toastUntil = Time.unscaledTime + 5f;
        }

        public void Pulse(Vector3 position, Color color)
        {
            feedbackParticles.transform.position = position;
            ParticleSystem.EmitParams burst = new ParticleSystem.EmitParams { startColor = color };
            feedbackParticles.Emit(burst, 16);
        }

        private void OnPlayerDied()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            pacing.Died();
#endif
            State = MarshState.Defeat;
            SetFrozen(true);
            dialoguePanel.SetActive(false);
            ShowMenu("THE MARSH IS PATIENT", "A STUMBLE, NOT THE END", "Duckduckgoose waits beside the last safe lantern.\n\nRestart at " + checkpointNames[CheckpointIndex] + ".\nYour coins, recovered trays, and open causeways are kept.", "TRY AGAIN  /  R");
        }

        public void RestartCheckpoint()
        {
            if (State != MarshState.Defeat) return;
            Vector3 old = player.transform.position;
            gameManager.RespawnPlayerNow();
            AssertCoinConservation();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            pacing.WriteEvent("restart", 0, "Checkpoint " + CheckpointIndex);
#endif
            cameraManager.VirtualCamera.OnTargetObjectWarped(player.transform, player.transform.position - old);
            duck.SnapToPlayer();
            menuPanel.SetActive(false);
            hudRoot.SetActive(true);
            inventoryCanvas.enabled = true;
            State = MarshState.Playing;
            SetFrozen(false);
            lastHealth = player.hitPoints.value;
            Notify("Back at the lantern. The bells remember your work.");
        }

        public void TogglePause()
        {
            if (State == MarshState.Playing)
            {
                State = MarshState.Paused;
                SetFrozen(true);
                ShowMenu("TAKE A BREATH", "AT THE WATER'S EDGE", "The tide can wait.\n\nWASD / arrows: move    E: interact    Esc: resume\nJ / left click: swipe    Q: equip or pack\nA swipe borrows one carried coin, then returns it.\nRecover the same three coins at every crossing.\n\nCheckpoint: " + checkpointNames[CheckpointIndex], "RESUME  /  ESC");
            }
            else if (State == MarshState.Paused)
            {
                menuPanel.SetActive(false);
            hudRoot.SetActive(true);
            inventoryCanvas.enabled = true;
                State = MarshState.Playing;
                SetFrozen(false);
            }
        }

        public bool TryWin()
        {
            if (!CanExplore || BellsOpened != 3 || Vector2.Distance(player.transform.position, havenPosition) > 1.7f) return false;
            if (Vector2.Distance(duck.transform.position, player.transform.position) > 2.2f) { Notify("Wait for Duckduckgoose to catch up, then step into the lantern circle."); return false; }
            foreach (MarshNode node in nodes)
                if (node.Kind == MarshNodeKind.ReturnTray && !node.Reclaimed)
                {
                    Notify("Your borrowed coins are still in a return tray. Recover them before joining the flock.");
                    return false;
                }
            if (!coinRelay.Completed) { Notify(coinRelay.Objective); return false; }
            weapon.InterruptSwing();
            if (weapon.BorrowedCoins != 0) { Notify("Make inventory space to recover the sword coin before joining the flock."); return false; }
            AssertCoinConservation();
            endingDialogue = true;
            ShowStory("ending");
            return true;
        }

        private void ShowWinMenu()
        {
            State = MarshState.Won;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            pacing.Finished();
#endif
            SetFrozen(true);
            int seconds = Mathf.RoundToInt(elapsed);
            ShowMenu("THE FLOCK IS HOME", "THREE BELLS. ONE WAY HOME.", "Aiken opened the causeways. Duckduckgoose found the flock.\nAda's bells are ready for whoever comes next.\n\nBells restored: 3 / 3     Notes and voices: " + readStories.Count + " / " + story.Entries.Length + "\nExploration time: " + (seconds / 60) + "m " + (seconds % 60) + "s", "PLAY AGAIN  /  R");
        }

        public void RestartRun()
        {
            weapon.InterruptSwing();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        private void SetFrozen(bool value)
        {
            if (value) weapon.InterruptSwing();
            movement.SetInputEnabled(!value);
            Time.timeScale = value ? 0f : 1f;
        }

        private void ShowMenu(string eyebrow, string title, string body, string action)
        {
            hudRoot.SetActive(false);
            inventoryCanvas.enabled = false;
            menuPanel.SetActive(true);
            menuEyebrow.text = eyebrow;
            menuTitle.text = title;
            menuBody.text = body;
            primaryLabel.text = action;
        }

        private void UpdateHud()
        {
            int carriedCoins = player.PlayerInventory.Count(Item.ItemType.COIN);
            int effectiveCoins = carriedCoins + weapon.BorrowedCoins;
            coinText.text = "BRASS  " + carriedCoins.ToString("00") + (weapon.BorrowedCoins > 0 ? " (+" + weapon.BorrowedCoins + ")" : "");
            chapterText.text = "RILL MARSH  /  " + checkpointNames[CheckpointIndex].ToUpperInvariant();
            ledgerText.text = "BELLS  " + BellsOpened + " / 3";
            if (BellsOpened == 0) objectiveText.text = "Gather three brass coins. Find the first bell to the east.";
            else if (BellsOpened == 3 && effectiveCoins < 3) objectiveText.text = "Cross the last causeway. Recover your coins from its brass return tray.";
            else if (BellsOpened == 3) objectiveText.text = "Reach Lantern Haven in the east. Bring Duckduckgoose into the lantern circle.";
            else if (BellsOpened == 2 && !coinRelay.Completed) objectiveText.text = coinRelay.Objective;
            else if (effectiveCoins < 3) objectiveText.text = "Cross the causeway. Press E at the far-side tray to recover your coins.";
            else objectiveText.text = "Follow the raised trail east. Lend the same coins to bell " + (BellsOpened + 1) + ".";
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void DebugTeleport(int index)
        {
            if (index < 0 || index >= checkpoints.Length) throw new ArgumentOutOfRangeException(nameof(index));
            debugCheckpoint = index;
            pacing.MarkTeleport(index);
            Vector3 old = player.transform.position;
            player.GetComponent<Rigidbody2D>().position = checkpoints[index].transform.position;
            player.transform.position = checkpoints[index].transform.position;
            cameraManager.VirtualCamera.OnTargetObjectWarped(player.transform, player.transform.position - old);
            duck.SnapToPlayer();
            Physics2D.SyncTransforms();
            Notify("Development teleport: " + checkpointNames[index] + ". Progression is unchanged.");
        }
#endif

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            if (player != null) player.Died -= OnPlayerDied;
        }
    }
}


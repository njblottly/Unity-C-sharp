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
        [SerializeField] private MarshCampaign campaign;
        [SerializeField] private MarshInteraction[] interactions = new MarshInteraction[0];
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
        public MarshCampaign Campaign => campaign;
        public MarshInteraction[] Interactions => interactions;
        public int RequiredBells => campaign == null ? 3 : campaign.Data.sections.Length;
        public string[] PacingSectionNames => campaign == null
            ? new[] { "Landing to Reedbank", "Reedbank to Archive", "Archive to Haven", "Haven to reunion" }
            : new[] { campaign.Data.chapters[0].displayName, campaign.Data.chapters[1].displayName, campaign.Data.chapters[2].displayName, campaign.Data.chapters[3].displayName, campaign.Data.chapters[4].displayName, "Haven reunion" };
        public bool RequiresArchiveExchange(int sequence) => sequence == (campaign == null ? 2 : campaign.Data.archiveGateIndex);
        public MarshCoinRelay CoinRelay => coinRelay;
        public Weapon Weapon => weapon;
        public int CoinsInEscrow
        {
            get
            {
                int total = coinRelay.CoinsInEscrow + weapon.BorrowedCoins + (campaign==null?0:campaign.ExtraEscrow);
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
        public bool JournalOpen {get;private set;}
        public int JournalPage {get;private set;}
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
            ShowMenu("A DUCK ADVENTURE", "MARSHLIGHT", "The Borrowed Bells\n\nThe storm has silenced Rill Marsh. Aiken and Duckduckgoose must reopen the old causeways and bring the flock home.\n\nWASD / arrows: move    E: interact    Space / Enter: read\nJ / left click: swipe    Q: equip    Tab: journal    Esc: pause\n\nFind Ada's sword and three reusable coins. Every loan comes back.", "BEGIN  /  ENTER");
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
            if (weapon == null || coinRelay == null || coinPickups == null || coinPickups.Length != 3 || player == null || movement == null || duck == null || gameManager == null || cameraManager == null || coinItem == null || story == null || nodes == null || checkpoints == null || checkpoints.Length != RequiredBells + 1 || checkpointNames == null || checkpointNames.Length != checkpoints.Length || interactions == null)
                throw new InvalidOperationException("MarshLevel has missing gameplay references. Run Tools/Duck Adventure/Validate Marshlight in Edit Mode.");
            if (hudRoot == null || inventoryCanvas == null || objectiveText == null || chapterText == null || coinText == null || promptText == null || toastText == null || dialoguePanel == null || speakerText == null || dialogueText == null || menuPanel == null || menuTitle == null || menuBody == null || primaryLabel == null || damageFlash == null || ledgerText == null || feedbackParticles == null)
                throw new InvalidOperationException("MarshLevel has missing UI references.");
            story.ValidateData();
            if (campaign != null) { campaign.ValidateReferences(); if (campaign.Level != this) throw new InvalidOperationException("Campaign is bound to another level."); }
            foreach (MarshInteraction interaction in interactions) if (interaction == null || interaction.Owner != this) throw new InvalidOperationException("Missing/misbound interaction reference.");
        }

        public bool CanReadStory(string id)
        {
            MarshStory.Entry entry = story.Get(id);
            if (entry.oncePerRun && readStories.Contains(id)) return false;
            if (!string.IsNullOrEmpty(entry.requiredCapability) && (campaign == null || !campaign.HasCapability(entry.requiredCapability))) return false;
            foreach (string prerequisite in entry.prerequisites) if (!readStories.Contains(prerequisite)) return false;
            return true;
        }

        private void Update()
        {
            if(JournalOpen&&(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.RightArrow)))ChangeJournalPage(1);
            else if(JournalOpen&&Input.GetKeyDown(KeyCode.LeftArrow))ChangeJournalPage(-1);
            else if(campaign!=null&&(CanExplore||JournalOpen)&&Input.GetKeyDown(KeyCode.Tab))ToggleJournal();
            else if (State == MarshState.Intro && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) Begin();
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
                promptText.text = nearest == null ? "WASD MOVE   E INTERACT   J / CLICK SWIPE   Q EQUIP   TAB JOURNAL   ESC PAUSE" : nearest.Prompt;
                MarshInteraction extension = null;
                if (nearest != null) distance = Vector2.Distance(player.transform.position, nearest.transform.position);
                foreach (MarshInteraction interaction in interactions)
                {
                    if (!interaction.gameObject.activeInHierarchy || !interaction.IsAvailable) continue;
                    float d = Vector2.Distance(player.transform.position, interaction.transform.position);
                    if (d < distance) { extension = interaction; distance = d; }
                }
                if (extension != null) promptText.text = extension.Prompt;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (extension != null) extension.TryInteract();
                    else if (nearest != null) nearest.TryInteract();
                }
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
            if(JournalOpen)ChangeJournalPage(1);
            else if (State == MarshState.Intro) Begin();
            else if (State == MarshState.Paused) TogglePause();
            else if (State == MarshState.Defeat) RestartCheckpoint();
            else if (State == MarshState.Won) RestartRun();
        }

        public void ShowStory(string id) { TryShowStory(id); }

        public bool TryShowStory(string id)
        {
            if (!CanReadStory(id)) return false;
            activeEntry = story.Get(id);
            readStories.Add(id);
            page = 0;
            State = MarshState.Dialogue;
            SetFrozen(true);
            hudRoot.SetActive(true);
            inventoryCanvas.enabled = true;
            dialoguePanel.SetActive(true);
            RenderPage();
            return true;
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
            Notify("Bell " + BellsOpened + "/" + RequiredBells + " rings. Cross east and reclaim your coins.");
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
            if(JournalOpen){ToggleJournal();return;}
            if (State == MarshState.Playing)
            {
                State = MarshState.Paused;
                SetFrozen(true);
                ShowMenu("TAKE A BREATH", "AT THE WATER'S EDGE", "The tide can wait.\nWASD / arrows: move    E: interact    Esc: resume\nJ / click: swipe    Q: equip / pack    Tab: journal\nPerches: E call / recall; inspect inside to latch.\nLamps: E inspect / refund. Mail: E send / reroute.\nA swipe borrows one coin. Every loan comes back.\nReed Marks: 4 earn a badge; 8 add a closing memory.\nCheckpoint: " + checkpointNames[CheckpointIndex], "RESUME  /  ESC");
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

        public void ToggleJournal()
        {
            if(campaign==null)return;
            if(JournalOpen)
            {
                JournalOpen=false;menuPanel.SetActive(false);hudRoot.SetActive(true);inventoryCanvas.enabled=true;State=MarshState.Playing;SetFrozen(false);return;
            }
            if(!CanExplore)return;
            JournalOpen=true;State=MarshState.Paused;SetFrozen(true);
            JournalPage=0;RenderJournal();
        }

        public void ChangeJournalPage(int direction)
        {
            if(!JournalOpen)return;
            int count=campaign.JournalPages.Length;JournalPage=(JournalPage+direction%count+count)%count;RenderJournal();
        }
        private void RenderJournal()
        {
            var pages=campaign.JournalPages;
            ShowMenu("A POCKET OF MEMORIES","REED JOURNAL "+(JournalPage+1)+" / "+pages.Length,pages[JournalPage],"NEXT / SPACE    CLOSE / TAB");
        }

        public bool TryWin()
        {
            if (!CanExplore || BellsOpened != RequiredBells || Vector2.Distance(player.transform.position, havenPosition) > 1.7f) return false;
            if (Vector2.Distance(duck.transform.position, player.transform.position) > 2.2f) { Notify("Wait for Duckduckgoose to catch up, then step into the lantern circle."); return false; }
            foreach (MarshNode node in nodes)
                if (node.Kind == MarshNodeKind.ReturnTray && !node.Reclaimed)
                {
                    Notify("Your borrowed coins are still in a return tray. Recover them before joining the flock.");
                    return false;
                }
            if (!coinRelay.Completed) { Notify(coinRelay.Objective); return false; }
            if(campaign!=null){campaign.FreezeLoans();if(campaign.ExtraEscrow!=0){Notify("Recover addressed mail and lamp coins before the reunion.");return false;}}
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
            ShowMenu("THE FLOCK IS HOME", campaign == null ? "THREE BELLS. ONE WAY HOME." : "FIVE BELLS. ONE WAY HOME.", "Aiken opened the causeways. Duckduckgoose found the flock.\nAda's bells are ready for whoever comes next.\n\nBells restored: " + BellsOpened + " / " + RequiredBells + "     Notes and voices: " + readStories.Count + " / " + story.Entries.Length + "\nExploration time: " + (seconds / 60) + "m " + (seconds % 60) + "s" + (campaign==null?"":("\n\n"+campaign.EndingCoda)), "PLAY AGAIN  /  R");
        }

        public void RestartRun()
        {
            weapon.InterruptSwing();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        private void SetFrozen(bool value)
        {
            if (value){weapon.InterruptSwing();if(campaign!=null)campaign.FreezeLoans();}
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
            ledgerText.text = "BELLS "+BellsOpened+"/"+RequiredBells+(campaign==null?"":"  MARKS "+campaign.MarkCount+"/8"+(campaign.HasBadge?" BADGE":""));
            if (campaign != null)
            {
                chapterText.text = "RILL MARSH / " + campaign.ChapterName.ToUpperInvariant();
                objectiveText.text = campaign.Objective;
                return;
            }
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

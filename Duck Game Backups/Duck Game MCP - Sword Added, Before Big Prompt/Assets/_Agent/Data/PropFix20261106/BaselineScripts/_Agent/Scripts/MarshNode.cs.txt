using UnityEngine;

namespace DuckAdventure.Agent
{
    public enum MarshNodeKind { Bell, ReturnTray, Note, Checkpoint, Haven, RelayLoan, RelayReturn }

    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class MarshNode : MonoBehaviour
    {
        [SerializeField] private MarshLevel level;
        [SerializeField] private MarshNodeKind kind;
        [SerializeField] private string nodeId;
        [SerializeField] private string displayName;
        [SerializeField] private string storyId;
        [SerializeField] private int sequence;
        [SerializeField] private int cost = 3;
        [SerializeField] private MarshNode pairedBell;
        [SerializeField] private GameObject closedGate;
        [SerializeField] private SpriteRenderer bridge;
        [SerializeField] private SpriteRenderer marker;
        [SerializeField] private SpawnPoint spawnPoint;

        public MarshNodeKind Kind => kind;
        public int Cost => cost;
        public string DisplayName => displayName;
        public int OutstandingCoins => kind == MarshNodeKind.ReturnTray && pairedBell.Opened && !Reclaimed ? cost : 0;
        public string Id => nodeId;
        public int Sequence => sequence;
        public bool Opened { get; private set; }
        public bool Reclaimed { get; private set; }
        public bool Visited { get; private set; }
        public bool GateClosed => closedGate != null && closedGate.activeSelf;
        public bool IsInteractive => kind == MarshNodeKind.Bell || kind == MarshNodeKind.ReturnTray || kind == MarshNodeKind.Note || kind == MarshNodeKind.RelayLoan || kind == MarshNodeKind.RelayReturn;
        public string Prompt
        {
            get
            {
                if (kind == MarshNodeKind.RelayLoan || kind == MarshNodeKind.RelayReturn) return level.CoinRelay.Prompt(this);
                if (kind == MarshNodeKind.Bell) return Opened ? displayName + " — causeway latched" : "E  Lend " + cost + " coins to " + displayName;
                if (kind == MarshNodeKind.ReturnTray) return Reclaimed ? "Coins recovered — keep heading east" : "E  Recover " + cost + " coins from " + displayName;
                return "E  Read " + displayName;
            }
        }

        public bool TryInteract()
        {
            if (!level.CanExplore || !IsInteractive || Vector2.Distance(level.Player.transform.position, transform.position) > 1.5f) return false;
            if (kind == MarshNodeKind.RelayLoan || kind == MarshNodeKind.RelayReturn) return level.CoinRelay.TryInteract(this);
            Inventory inventory = level.Player.PlayerInventory;
            if (kind == MarshNodeKind.Bell)
            {
                if (Opened) { level.Notify("The causeway is latched. Your coins are in the far-side tray."); return false; }
                if (sequence == 2 && !level.CoinRelay.Completed) { level.Notify(level.CoinRelay.Objective); return false; }
                if (level.BellsOpened != sequence) { level.Notify("Follow the bells from west to east."); return false; }
                if (!inventory.TrySpend(Item.ItemType.COIN, cost))
                {
                    level.Notify("This bell needs three coins. Follow the brass glints along the landing trail.");
                    return false;
                }
                Opened = true;
                Visited = true;
                closedGate.SetActive(false);
                bridge.color = new Color(0.75f, 0.65f, 0.40f);
                marker.color = new Color(1f, 0.88f, 0.48f);
                level.BellOpened(this);
                level.RecordCoinAction("bell_lend", cost, nodeId);
                level.Pulse(transform.position, new Color(1f, 0.8f, 0.3f));
                level.ShowStory(storyId);
                return true;
            }
            if (kind == MarshNodeKind.ReturnTray)
            {
                if (!pairedBell.Opened) { level.Notify("The matching bell has not been rung yet."); return false; }
                if (Reclaimed) { level.Notify("You already recovered these coins."); return false; }
                if (!inventory.TryAddItems(level.CoinItem, cost))
                {
                    level.Notify("Make space in the inventory. The tray will keep your coins safe.");
                    return false;
                }
                Reclaimed = true;
                level.RecordCoinAction("bell_reclaim", cost, nodeId);
                Visited = true;
                marker.color = new Color(0.45f, 0.8f, 0.67f);
                level.Notify("Three coins returned. Lend them to the next bell.");
                level.Pulse(transform.position, new Color(1f, 0.8f, 0.3f));
                return true;
            }
            Visited = true;
            level.ShowStory(storyId);
            return true;
        }

        private void OnTriggerEnter2D(Collider2D other) { Enter(other); }
        private void OnTriggerStay2D(Collider2D other) { Enter(other); }
        private void Enter(Collider2D other)
        {
            if (other.GetComponent<Player>() != level.Player || !level.CanExplore) return;
            if (kind == MarshNodeKind.Checkpoint && !Visited && level.BellsOpened >= sequence)
            {
                Visited = true;
                level.ReachCheckpoint(this, spawnPoint);
                marker.color = new Color(1f, 0.85f, 0.45f);
            }
            if (kind == MarshNodeKind.Haven && level.TryWin()) Visited = true;
        }
    }
}

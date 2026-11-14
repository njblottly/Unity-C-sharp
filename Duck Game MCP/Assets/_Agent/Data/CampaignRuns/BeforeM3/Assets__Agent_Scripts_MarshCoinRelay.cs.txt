using System;
using UnityEngine;

namespace DuckAdventure.Agent
{
    /// <summary>The archive's refundable, split-weight exchange. Doors latch; death never resets loans.</summary>
    public sealed class MarshCoinRelay : MonoBehaviour
    {
        [Serializable]
        public sealed class Loan
        {
            public MarshNode cup;
            public MarshNode tray;
            public GameObject gate;
            public SpriteRenderer indicator;
            public int requiresReturnedLoan = -1;
            [NonSerialized] public bool lent;
            [NonSerialized] public bool returned;
        }

        [SerializeField] private MarshLevel level;
        [SerializeField] private Loan[] loans;
        private bool introductionRead;
        public int LoanCount => loans.Length;
        public bool Completed => IsReturned(0) && IsReturned(1) && IsReturned(2);
        public int CoinsInEscrow
        {
            get
            {
                int total = 0;
                foreach (Loan loan in loans) if (loan.lent && !loan.returned) total += loan.cup.Cost;
                return total;
            }
        }
        public bool IsLent(int index) => loans[index].lent;
        public bool IsReturned(int index) => loans[index].returned;
        public int Cost(int index) => loans[index].cup.Cost;
        public bool GateClosed(int index) => loans[index].gate.activeSelf;

        private void Awake()
        {
            if (level == null || loans == null || loans.Length != 3)
                throw new InvalidOperationException("Archive exchange requires its level and three loans.");
            for (int i = 0; i < loans.Length; i++)
            {
                Loan loan = loans[i];
                if (loan.cup == null || loan.tray == null || loan.gate == null || loan.indicator == null || loan.cup.Cost <= 0)
                    throw new InvalidOperationException("Archive exchange loan " + i + " has missing references.");
                if (loan.requiresReturnedLoan >= i)
                    throw new InvalidOperationException("Archive exchange dependencies must point to an earlier loan.");
            }
        }

        private int IndexOf(MarshNode node)
        {
            for (int i = 0; i < loans.Length; i++) if (loans[i].cup == node || loans[i].tray == node) return i;
            throw new InvalidOperationException("Unregistered archive exchange node: " + node.Id);
        }

        public string Prompt(MarshNode node)
        {
            Loan loan = loans[IndexOf(node)];
            if (node == loan.cup)
            {
                if (loan.lent) return "Counterweight latched — its coins wait in the matching tray";
                if (loan.requiresReturnedLoan >= 0 && !loans[loan.requiresReturnedLoan].returned)
                    return "Exit cup locked — reclaim the reed pair first";
                return "E  Lend " + loan.cup.Cost + " to " + node.DisplayName;
            }
            return loan.returned ? "Coins recovered — the arm stays latched" : "E  Reclaim " + loan.cup.Cost + " from " + node.DisplayName;
        }

        public bool TryInteract(MarshNode node)
        {
            if (!level.CanExplore || Vector2.Distance(level.Player.transform.position, node.transform.position) > 1.5f) return false;
            Loan loan = loans[IndexOf(node)];
            Inventory inventory = level.Player.PlayerInventory;
            if (node == loan.cup)
            {
                if (loan.lent) { level.Notify("That counterweight is already latched. It cannot take another payment."); return false; }
                if (loan.requiresReturnedLoan >= 0 && !loans[loan.requiresReturnedLoan].returned)
                {
                    level.Notify("The exit cup's shutter opens when the reed pair is reclaimed. Keep those two coins for the reed cup.");
                    return false;
                }
                if (!inventory.TrySpend(Item.ItemType.COIN, loan.cup.Cost))
                {
                    level.Notify("This cup borrows " + loan.cup.Cost + ". The other coins are safe in the exchange's return trays.");
                    return false;
                }
                loan.lent = true;
                loan.gate.SetActive(false);
                loan.indicator.color = new Color(1f, 0.86f, 0.44f);
                level.RecordCoinAction("relay_lend", loan.cup.Cost, node.Id);
                level.Pulse(node.transform.position, new Color(1f, 0.8f, 0.3f));
                level.Notify(Objective);
                if (!introductionRead)
                {
                    introductionRead = true;
                    level.ShowStory("exchange");
                }
                return true;
            }
            if (!loan.lent) { level.Notify("This tray is empty until its matching cup has been funded."); return false; }
            if (loan.returned) { level.Notify("These coins have already been reclaimed."); return false; }
            if (!inventory.TryAddItems(level.CoinItem, loan.cup.Cost))
            {
                level.Notify("Make space in the inventory. The tray will hold its coins until you can take them.");
                return false;
            }
            loan.returned = true;
            loan.indicator.color = new Color(0.48f, 0.83f, 0.68f);
            level.RecordCoinAction("relay_reclaim", loan.cup.Cost, node.Id);
            level.Pulse(node.transform.position, new Color(1f, 0.8f, 0.3f));
            level.Notify(Objective);
            return true;
        }

        public string Objective
        {
            get
            {
                if (!loans[0].lent && !loans[1].lent) return "Archive exchange: lend 2 to reed, 1 to spill. Reclaim the pair, then use the exit cup.";
                if (!loans[0].lent) return "Lend the remaining pair to the reed cup. It lowers the distant south arm.";
                if (!loans[1].lent) return "Lend the remaining coin to the spill cup. It lowers the north arm.";
                if (!loans[0].returned) return "Pass the north arm and reclaim the reed pair from the middle tray.";
                if (!loans[2].lent) return "Lend the recovered pair again, this time to the exit cup.";
                if (!loans[1].returned) return "The spill coin waits beyond the south arm. Reclaim it before leaving.";
                if (!loans[2].returned) return "Pass the archive latch and reclaim the exit pair on the eastern trail.";
                return "The exchange is settled. Take all three coins to the Haven Bell.";
            }
        }
    }
}

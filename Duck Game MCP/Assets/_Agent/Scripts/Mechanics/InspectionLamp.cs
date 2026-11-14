using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class InspectionLamp:MarshMechanic
    {
        [SerializeField] private SpriteRenderer glow;
        private int borrowed;
        private bool lit,inspected;
        protected override MarshMechanicKind Kind=>MarshMechanicKind.InspectionLamp;
        public override bool Completed=>inspected;
        public override int CoinsInEscrow=>borrowed;
        public bool IsLit=>lit;
        public override string Prompt=>borrowed>0?"E  Extinguish / reclaim the lamp coin":"E  Inspect under "+Data.title+" (borrows 1 coin)";
        protected override void Awake(){base.Awake();if(glow==null)throw new InvalidOperationException("Inspection lamp needs a glow reference.");Render();}
        public override bool TryInteract()
        {
            if(!CanUse)return false;
            if(borrowed>0){bool result=Refund();if(!result)Level.Notify("Make an inventory slot: the lamp keeps its coin safe until you can reclaim it.");return result;}
            if(!string.IsNullOrEmpty(Data.prerequisite)&&!Level.Campaign.HasToken(Data.prerequisite)){Level.Notify(Level.Campaign.Mechanic(Data.prerequisite).Data.hint);return false;}
            if(!Level.Player.PlayerInventory.TrySpend(Item.ItemType.COIN,Data.cost)){Level.Notify("Carry one coin for the lamp; it comes back when you turn it off.");return false;}
            borrowed=Data.cost;lit=true;inspected=true;Render();Level.RecordCoinAction("lamp_lend",borrowed,Id);Level.Notify(Data.journalText+"  E again returns the coin. Tab opens your journal.");return true;
        }
        private bool Refund()
        {
            lit=false;Render();
            if(borrowed==0)return true;
            if(!Level.Player.PlayerInventory.TryAddItems(Level.CoinItem,borrowed))return false;
            int returned=borrowed;borrowed=0;Level.RecordCoinAction("lamp_reclaim",returned,Id);return true;
        }
        public override void FreezeTemporaryLoan(){if(borrowed>0)Refund();}
        private void Update(){if(!lit&&borrowed>0&&Level.CanExplore)Refund();}
        private void Render(){glow.color=lit?new Color(1,.88f,.48f):new Color(.45f,.52f,.54f);}
    }
}

using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class MailRouting:MarshMechanic
    {
        [SerializeField] private MailReturnBox[] destinations;
        private int borrowed,selected;
        private bool completed;
        protected override MarshMechanicKind Kind=>MarshMechanicKind.MailRouting;
        public override bool Completed=>completed;
        public override int CoinsInEscrow=>borrowed;
        public int SelectedDestination=>selected;
        public MailReturnBox[] Destinations=>destinations;
        public override string Prompt=>completed?Data.title+" — returned":borrowed==0?"E  Send 2 coins to "+destinations[selected].DisplayName:"Address: "+destinations[selected].DisplayName+" | E reroute the same 2 coins to "+destinations[1-selected].DisplayName;
        protected override void Awake()
        {
            base.Awake();if(destinations==null||destinations.Length!=2||destinations[0]==null||destinations[1]==null||destinations[0]==destinations[1])throw new InvalidOperationException("Mail routing requires two unique explicit return boxes.");
        }
        public override bool TryInteract()
        {
            if(!CanUse||completed)return false;
            if(borrowed>0){selected=1-selected;Level.Notify("The same two coins now wait at "+destinations[selected].DisplayName+". Reroute here again if you change your mind.");Level.RecordCoinAction("mail_reroute",0,Id);return true;}
            if(!string.IsNullOrEmpty(Data.prerequisite)&&!Level.Campaign.HasToken(Data.prerequisite)){Level.Notify(Level.Campaign.Mechanic(Data.prerequisite).Data.hint);return false;}
            if(!Level.Player.PlayerInventory.TrySpend(Item.ItemType.COIN,Data.cost)){Level.Notify("This sorting box needs two carried coins. Reclaim other loans first.");return false;}
            borrowed=Data.cost;Level.RecordCoinAction("mail_lend",borrowed,Id);Level.Notify("Two coins sent to "+destinations[selected].DisplayName+". Follow its label and reclaim them. E here changes the destination without another payment.");return true;
        }
        public bool TryClaim(MailReturnBox box)
        {
            if(!Level.CanExplore||borrowed==0||box!=destinations[selected])return false;
            if(!Level.Player.PlayerInventory.TryAddItems(Level.CoinItem,borrowed)){Level.Notify("Make inventory space. The return box keeps both coins safe.");return false;}
            int amount=borrowed;borrowed=0;completed=true;Level.RecordCoinAction("mail_reclaim",amount,Id);Level.Notify(Data.journalText);return true;
        }
    }
}

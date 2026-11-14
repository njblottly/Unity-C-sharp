using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class MailReturnBox:MarshInteraction
    {
        [SerializeField] private MailRouting routing;
        [SerializeField] private string displayName;
        [SerializeField] private DuckPerch requiredInspection;
        public string DisplayName=>displayName;
        public override bool IsAvailable=>!routing.Completed;
        public override string Prompt=>"E  Collect routed coins at "+displayName;
        protected override void Awake(){base.Awake();if(routing==null||string.IsNullOrWhiteSpace(displayName))throw new InvalidOperationException("Return box requires its routing and label.");}
        public override bool TryInteract()
        {
            if(!CanUse)return false;
            if(requiredInspection!=null&&!requiredInspection.Completed){Level.Notify("Inspect the perch room first, or reroute to the open low-road box.");return false;}
            bool result=routing.TryClaim(this);if(!result)Level.Notify("No coins addressed here. Check the sorting box's destination or make inventory space.");return result;
        }
    }
}

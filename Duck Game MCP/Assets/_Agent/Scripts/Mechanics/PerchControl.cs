using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class PerchControl:MarshInteraction
    {
        [SerializeField] private DuckPerch perch;
        [SerializeField] private bool inspectionPlaque;
        public override bool IsAvailable=>!perch.Completed;
        public override string Prompt=>inspectionPlaque?(perch.Completed?"Inspection recorded":"E  Latch inspection while Duckduckgoose holds the perch"):"E  Call / recall Duckduckgoose (free)";
        protected override void Awake(){base.Awake();if(perch==null||perch.Owner!=Owner)throw new InvalidOperationException("Perch control requires its explicit perch.");}
        public override bool TryInteract()
        {
            if(!CanUse)return false;
            bool result=inspectionPlaque?perch.CompleteInspection():perch.Command();
            if(!result&&!perch.Completed)Level.Notify("Ask Duckduckgoose to settle on the perch, enter through the open shutter, then inspect this plaque.");
            return result;
        }
    }
}

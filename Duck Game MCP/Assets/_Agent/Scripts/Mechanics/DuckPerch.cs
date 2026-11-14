using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class DuckPerch:MarshMechanic
    {
        [SerializeField] private Transform perch;
        [SerializeField] private GameObject shutter;
        [SerializeField] private BoxCollider2D shutterCollider;
        [SerializeField] private Rect safeRoom;
        private Bounds doorway;
        private bool completed;
        protected override MarshMechanicKind Kind=>MarshMechanicKind.Perch;
        public override bool Completed=>completed;
        public override bool IsAvailable=>!completed;
        public bool DuckReady=>Level.Duck.AssistanceTarget==perch&&Level.Duck.AtPerch;
        public bool ShutterOpen=>!shutter.activeSelf;
        public Rect SafeRoom=>safeRoom;
        public override string Prompt=>completed?Data.title+" — inspection latched":Level.Duck.AssistanceTarget==perch?"E  Recall Duckduckgoose (free)":"E  Ask Duckduckgoose to hold this perch (free)";
        protected override void Awake()
        {
            base.Awake();if(perch==null||shutter==null||shutterCollider==null||shutterCollider.gameObject!=shutter||safeRoom.width<=0||safeRoom.height<=0)throw new InvalidOperationException("Perch requires explicit pad, shutter and safe room.");
            doorway=new Bounds(shutterCollider.bounds.center,shutterCollider.bounds.size+new Vector3(.9f,.9f,0));
        }
        public override bool TryInteract(){if(!CanUse)return false;return Command();}
        public bool Command()
        {
            if(!Level.CanExplore||completed)return false;
            if(Level.Duck.AssistanceTarget==perch)Level.Duck.Recall();else Level.Duck.HoldAt(perch);
            Level.Notify(Level.Duck.AssistanceTarget==perch?"Duckduckgoose walks to the perch. Enter when the shutter opens; E at the inspection plaque latches it.":"Duckduckgoose returns. The room keeps a safe exit open.");
            return true;
        }
        public bool CompleteInspection()
        {
            if(!Level.CanExplore||completed||!DuckReady||!safeRoom.Contains(Level.Player.transform.position))return false;
            completed=true;shutter.SetActive(false);Level.Duck.Recall();Level.Notify(Data.journalText);return true;
        }
        private void Update()
        {
            if(!Level.CanExplore)return;
            bool occupied=safeRoom.Contains(Level.Player.transform.position)||(!shutter.activeSelf&&doorway.Contains(Level.Player.transform.position));
            bool open=completed||DuckReady||occupied;
            if(shutter.activeSelf==open)shutter.SetActive(!open);
        }
    }
}

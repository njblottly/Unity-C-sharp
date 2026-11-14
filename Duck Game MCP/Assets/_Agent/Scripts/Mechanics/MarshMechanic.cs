using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public abstract class MarshMechanic:MarshInteraction
    {
        [SerializeField] private MechanicData definition;
        protected abstract MarshMechanicKind Kind {get;}
        public MechanicData Data=>definition;
        public abstract bool Completed {get;}
        public virtual int CoinsInEscrow=>0;
        public virtual void FreezeTemporaryLoan(){}
        protected override void Awake()
        {
            base.Awake();if(definition==null)throw new InvalidOperationException("Missing mechanic definition: "+name);
            definition.ValidateData();if(definition.id!=Id||definition.kind!=Kind||Level.Campaign==null)throw new InvalidOperationException("Mechanic ID/kind/campaign mismatch: "+name);
        }
    }
}

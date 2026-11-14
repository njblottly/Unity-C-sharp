using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class ReedMark:MarshInteraction
    {
        [SerializeField] private CollectibleData definition;
        [SerializeField] private SpriteRenderer visual;
        public CollectibleData Data=>definition;
        public bool Collected=>Level.Campaign.HasMark(definition.id);
        public override bool IsAvailable=>!Collected;
        public override string Prompt=>"E  Keep Reed Mark: "+definition.title;
        protected override void Awake(){base.Awake();if(definition==null||visual==null||Level.Campaign==null)throw new InvalidOperationException("Reed Mark needs its data, sprite and campaign.");definition.ValidateData();if(Id!=definition.id)throw new InvalidOperationException("Reed Mark ID mismatch.");visual.sprite=definition.sprite;}
        public override bool TryInteract()
        {
            if(!CanUse)return false;
            if(!Level.Campaign.CollectMark(definition))return false;
            gameObject.SetActive(false);Level.Notify("Reed Mark "+Level.Campaign.MarkCount+"/8: "+definition.title+". Tab reads the memory.");Level.Pulse(transform.position,new Color(.89f,.77f,.49f));return true;
        }
    }
}

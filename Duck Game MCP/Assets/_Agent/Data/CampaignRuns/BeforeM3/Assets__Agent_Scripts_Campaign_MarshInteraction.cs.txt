using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    // Explicit Inspector-bound extension point alongside the original MarshNode.
    public abstract class MarshInteraction : MonoBehaviour
    {
        [SerializeField] private MarshLevel level;
        [SerializeField] private string interactionId;
        protected MarshLevel Level => level;
        public MarshLevel Owner => level;
        public string Id => interactionId;
        public virtual bool IsAvailable => true;
        public abstract string Prompt { get; }
        protected bool CanUse => level.CanExplore && IsAvailable && Vector2.Distance(level.Player.transform.position, transform.position) <= 1.5f;
        protected virtual void Awake()
        {
            if (level == null || string.IsNullOrWhiteSpace(interactionId)) throw new InvalidOperationException("Interaction requires a level and stable ID: " + name);
        }
        public abstract bool TryInteract();
    }
}

using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class MarshStoryTrigger : MarshInteraction
    {
        [SerializeField] private string storyId;
        [SerializeField] private bool automatic;
        public string StoryId => storyId;
        public override bool IsAvailable => Level.CanReadStory(storyId);
        public override string Prompt => "E  Read Ada's trail message";
        protected override void Awake()
        {
            base.Awake();
            if (string.IsNullOrWhiteSpace(storyId) || !GetComponent<BoxCollider2D>().isTrigger)
                throw new InvalidOperationException("Story trigger requires its story ID and a trigger collider.");
        }
        public override bool TryInteract() => CanUse && Level.TryShowStory(storyId);
        private void OnTriggerStay2D(Collider2D other)
        {
            if (automatic && other.GetComponent<Player>() == Level.Player && !Level.HasRead(storyId)) TryInteract();
        }
    }
}

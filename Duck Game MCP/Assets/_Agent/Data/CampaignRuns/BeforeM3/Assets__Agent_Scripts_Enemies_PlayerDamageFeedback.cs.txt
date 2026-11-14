using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [DisallowMultipleComponent]
    public sealed class PlayerDamageFeedback:MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private SpriteRenderer bodySprite;
        private Color resting;
        private void Awake(){if(player==null||bodySprite==null)throw new InvalidOperationException("Player feedback requires its player and renderer.");resting=bodySprite.color;}
        private void LateUpdate(){Color color=resting;if(player.InvulnerabilityRemaining>0&&Mathf.FloorToInt(Time.time*14)%2==0)color.a=.35f;bodySprite.color=color;}
        private void OnDisable(){if(bodySprite!=null)bodySprite.color=resting;}
    }
}

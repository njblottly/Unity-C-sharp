using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class MarshSeedProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private CircleCollider2D hitbox;
        [SerializeField] private SpriteRenderer visual;
        private EnemyAgent owner;
        private Vector2 direction;
        private float remaining;
        private int blocking;
        public bool IsFlying {get;private set;}
        public EnemyAgent Owner=>owner;
        private void Awake()
        {
            if(body==null||hitbox==null||visual==null||!hitbox.isTrigger)throw new InvalidOperationException("Seed needs explicit body, trigger and sprite.");
            blocking=LayerMask.GetMask("Blocking");if(blocking==0)throw new InvalidOperationException("Seed requires Blocking terrain.");
        }
        public void Launch(EnemyAgent source,Vector2 heading)
        {
            if(IsFlying||source==null||heading.sqrMagnitude<.01f)throw new InvalidOperationException("Invalid seed launch.");
            owner=source;direction=heading.normalized;remaining=source.Data.projectileRange;IsFlying=true;
        }
        private void FixedUpdate()
        {
            if(!IsFlying||!owner.OwnerLevel.CanExplore)return;
            float distance=Mathf.Min(remaining,owner.Data.projectileSpeed*Time.fixedDeltaTime);
            var hit=Physics2D.CircleCast(body.position,hitbox.radius,direction,distance+.015f,blocking);
            if(hit.collider!=null){Cancel();return;}
            body.MovePosition(body.position+direction*distance);remaining-=distance;
            if(remaining<=.001f)Cancel();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!IsFlying)return;
            var player=other.GetComponentInParent<Player>();
            if(player==null)return; // Other world triggers are not damage targets.
            if(player!=owner.OwnerLevel.Player)throw new InvalidOperationException("Unexpected player in seed encounter.");
            player.TryTakeEnemyDamage(owner.Data.attackDamage);Cancel();
        }
        public void Cancel(){IsFlying=false;gameObject.SetActive(false);Destroy(gameObject);}
    }
}

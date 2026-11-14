using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public enum EnemyPhase { Patrol, Telegraph, Attack, Recovery, Stagger, Dead }
    [DefaultExecutionOrder(-40)]
    [RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D),typeof(EnemyHitPoints))]
    public abstract class EnemyAgent : MonoBehaviour
    {
        [SerializeField] private MarshLevel level;
        [SerializeField] private Player player;
        [SerializeField] private SectionData section;
        [SerializeField] private string encounterId;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private CircleCollider2D bodyCollider;
        [SerializeField] private EnemyHitPoints health;
        [SerializeField] private MarshEnemyFeedback feedback;
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private LineRenderer warning;
        [SerializeField] private TextMesh stateLabel;
        private SectionData.Encounter encounter;
        private Vector2 patrolTarget;
        private Vector2 attackDirection=Vector2.right;
        private Vector2 facing=Vector2.right;
        private Vector2 knockback;
        private float phaseEnds,nextReaction;
        private int blocking;
        public EnemyData Data => encounter.data;
        public MarshLevel OwnerLevel => level;
        public Vector2 Position => body.position;
        protected virtual float WarningReach => Data.attackReach+Data.attackSpeed*Data.attackSeconds;
        public EnemyPhase Phase {get;private set;}
        public Vector2 AttackDirection => attackDirection;
        public Vector2 Facing => facing;
        public Vector2 SpawnPosition => encounter.position;
        public bool TelegraphVisible => warning.enabled;
        public EnemyHitPoints Health => health;
        protected Rigidbody2D Body => body;
        protected MarshLevel Level => level;
        protected abstract MarshEnemyKind Kind {get;}
        protected virtual bool CanStartAttack => true;
        protected virtual void Awake()
        {
            if(level==null||player==null||player!=level.Player||section==null||body==null||bodyCollider==null||health==null||feedback==null||bodySprite==null||warning==null||stateLabel==null)
                throw new InvalidOperationException("Enemy requires explicit actor, section, health and feedback references: "+name);
            encounter=section.GetEncounter(encounterId);Data.ValidateData();if(Data.kind!=Kind)throw new InvalidOperationException("Enemy prefab/definition kind mismatch.");
            if(!bodyCollider.isTrigger)throw new InvalidOperationException("Enemy contact collider must be a trigger.");
            blocking=LayerMask.GetMask("Blocking");if(blocking==0)throw new InvalidOperationException("Missing Blocking layer.");
            health.ConfigureForEncounter(Data.health);bodySprite.sprite=Data.sprite;body.position=encounter.position;transform.position=encounter.position;patrolTarget=encounter.pointB;
            health.Damaged+=OnDamaged;health.Died+=OnDied;player.Respawned+=OnPlayerRespawned;
            Phase=EnemyPhase.Patrol;RenderState();
        }
        private void FixedUpdate()
        {
            if(!level.CanExplore||health.IsDead)return;
            float dt=Time.fixedDeltaTime;
            switch(Phase)
            {
                case EnemyPhase.Patrol:
                    if(CanStartAttack&&Vector2.Distance(body.position,player.transform.position)<=Data.attackRange&&CanSeePlayer())BeginTelegraph();
                    else Patrol(dt);
                    break;
                case EnemyPhase.Telegraph:
                    if(Time.time>=phaseEnds){Phase=EnemyPhase.Attack;phaseEnds=Time.time+Data.attackSeconds;OnAttackStarted();}
                    break;
                case EnemyPhase.Attack:
                    PerformAttack(dt);if(Time.time>=phaseEnds)BeginRecovery();break;
                case EnemyPhase.Recovery:
                    if(Time.time>=phaseEnds)Phase=EnemyPhase.Patrol;break;
                case EnemyPhase.Stagger:
                    MoveSafely(knockback*dt);if(Time.time>=phaseEnds)BeginRecovery();break;
            }
            RenderState();
        }
        protected virtual void Patrol(float dt)
        {
            if(Vector2.Distance(body.position,patrolTarget)<.08f)patrolTarget=patrolTarget==encounter.pointA?encounter.pointB:encounter.pointA;
            Vector2 delta=patrolTarget-body.position;
            if(Data.chases&&Vector2.Distance(body.position,player.transform.position)<Data.detectionRange&&CanSeePlayer())delta=(Vector2)player.transform.position-body.position;
            if(delta.sqrMagnitude>.001f)facing=delta.normalized;
            MoveSafely(Vector2.ClampMagnitude(delta,Data.moveSpeed*dt));
        }
        protected Vector2 PointA => encounter.pointA;
        protected Vector2 PointB => encounter.pointB;
        private bool CanSeePlayer()=>Physics2D.Linecast(body.position,player.transform.position,blocking).collider==null;
        private void BeginTelegraph()
        {
            attackDirection=((Vector2)player.transform.position-body.position).normalized;if(attackDirection.sqrMagnitude<.001f)attackDirection=facing;
            facing=attackDirection;Phase=EnemyPhase.Telegraph;phaseEnds=Time.time+Data.telegraphSeconds;
        }
        protected virtual void OnAttackStarted() { }
        protected abstract void PerformAttack(float dt);
        protected void LungeAndStrike(float dt){MoveSafely(attackDirection*Data.attackSpeed*dt);Strike();}
        protected void Strike()
        {
            Vector2 offset=(Vector2)player.transform.position-body.position;
            if(offset.magnitude<=Data.attackReach&&Vector2.Dot(attackDirection,offset.normalized)>Mathf.Cos(Data.attackArcDegrees*.5f*Mathf.Deg2Rad)&&CanSeePlayer())player.TryTakeEnemyDamage(Data.attackDamage);
        }
        protected void MoveSafely(Vector2 delta)
        {
            delta=encounter.position+Vector2.ClampMagnitude(body.position+delta-encounter.position,Data.leashDistance)-body.position;
            if(delta.sqrMagnitude<.000001f)return;
            float radius=bodyCollider.radius*Mathf.Max(Mathf.Abs(transform.lossyScale.x),Mathf.Abs(transform.lossyScale.y));
            RaycastHit2D hit=Physics2D.CircleCast(body.position,radius,delta.normalized,delta.magnitude+.015f,blocking);
            float distance=hit.collider==null?delta.magnitude:Mathf.Max(0,hit.distance-.015f);
            body.MovePosition(body.position+delta.normalized*Mathf.Min(delta.magnitude,distance));
        }
        private void BeginRecovery(){Phase=EnemyPhase.Recovery;phaseEnds=Time.time+Data.recoverySeconds;}
        protected virtual bool AcceptHit(Vector2 source)=>true;
        public bool TryReceiveWeaponHit(int damage,Vector2 source)
        {
            if(damage<=0||health.IsDead)return false;
            if(!AcceptHit(source)){feedback.ShowGuard();return false;}
            health.TakeDamage(damage);return true;
        }
        private void OnDamaged(int amount)
        {
            if(health.CurrentHitPoints<=0||Time.time<nextReaction)return;
            nextReaction=Time.time+Data.hitReactionCooldown;
            knockback=(body.position-(Vector2)player.transform.position).normalized*Data.knockbackSpeed;
            Phase=EnemyPhase.Stagger;phaseEnds=Time.time+Data.hitRecoverySeconds;RenderState();
        }
        protected virtual void OnDied(){Phase=EnemyPhase.Dead;RenderState();}
        private void OnPlayerRespawned(){if(encounter.resetOnCheckpoint)ResetEncounter();}
        public virtual void ResetEncounter()
        {
            Phase=EnemyPhase.Patrol;nextReaction=0;health.ResetHitPoints();body.position=encounter.position;transform.position=encounter.position;body.linearVelocity=Vector2.zero;patrolTarget=encounter.pointB;feedback.ResetFeedback();RenderState();Physics2D.SyncTransforms();
        }
        protected virtual string IdleLabel=>"";
        private void RenderState()
        {
            warning.enabled=Phase==EnemyPhase.Telegraph;
            if(warning.enabled)
            {
                if(Data.kind==MarshEnemyKind.Crab)
                {
                    warning.positionCount=17;float axis=Mathf.Atan2(attackDirection.y,attackDirection.x);
                    for(int i=0;i<17;i++){float angle=axis+Mathf.Lerp(-Data.attackArcDegrees*.5f,Data.attackArcDegrees*.5f,i/16f)*Mathf.Deg2Rad;warning.SetPosition(i,body.position+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*Data.attackReach);}
                }
                else{warning.positionCount=2;warning.SetPosition(0,body.position);warning.SetPosition(1,body.position+attackDirection*WarningReach);}
            }
            stateLabel.text=Phase==EnemyPhase.Telegraph?"!":Phase==EnemyPhase.Recovery?"...":Phase==EnemyPhase.Patrol?IdleLabel:"";
        }
        protected virtual void OnDestroy()
        {
            if(health!=null){health.Damaged-=OnDamaged;health.Died-=OnDied;}if(player!=null)player.Respawned-=OnPlayerRespawned;
        }
    }
}

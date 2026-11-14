using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class WickMoth : EnemyAgent
    {
        [SerializeField] private Transform leftWing,rightWing;
        protected override MarshEnemyKind Kind=>MarshEnemyKind.Moth;
        protected override void Awake()
        {
            if(leftWing==null||rightWing==null)throw new InvalidOperationException("Wick Moth needs both wing references.");
            base.Awake();
        }
        protected override void Patrol(float dt)
        {
            float angle=Time.time*Data.moveSpeed*1.6f;
            Vector2 target=SpawnPosition+new Vector2(Mathf.Cos(angle)*.65f,Mathf.Sin(angle)*.45f);
            MoveSafely(Vector2.ClampMagnitude(target-Body.position,Data.moveSpeed*dt));
        }
        protected override void PerformAttack(float dt)=>LungeAndStrike(dt);
        private void Update()
        {
            float spread=Phase==EnemyPhase.Telegraph?.65f:.78f+Mathf.Sin(Time.time*12)*.18f;
            leftWing.localScale=new Vector3(-spread,.48f,1);rightWing.localScale=new Vector3(spread,.48f,1);
        }
    }
}

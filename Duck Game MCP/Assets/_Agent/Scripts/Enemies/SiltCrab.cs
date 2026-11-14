using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class SiltCrab : EnemyAgent
    {
        protected override MarshEnemyKind Kind=>MarshEnemyKind.Crab;
        protected override void PerformAttack(float dt)=>Strike();
        protected override string IdleLabel=>"GUARD";
        protected override bool AcceptHit(Vector2 source)
        {
            if(Phase==EnemyPhase.Recovery||Phase==EnemyPhase.Stagger)return true;
            return Vector2.Dot((source-Body.position).normalized,Facing)<Mathf.Cos(Data.guardArcDegrees*.5f*Mathf.Deg2Rad);
        }
    }
}

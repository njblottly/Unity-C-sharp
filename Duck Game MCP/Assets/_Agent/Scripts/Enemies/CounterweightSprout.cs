using UnityEngine;
namespace DuckAdventure.Agent
{
    public sealed class CounterweightSprout : EnemyAgent
    {
        private MarshSeedProjectile seed;
        private bool fired;
        public bool HasProjectile=>seed!=null&&seed.IsFlying;
        public MarshSeedProjectile CurrentProjectile=>seed;
        protected override MarshEnemyKind Kind=>MarshEnemyKind.Sprout;
        protected override bool CanStartAttack=>!HasProjectile;
        protected override float WarningReach=>Data.projectileRange;
        protected override void OnAttackStarted(){fired=false;}
        protected override void PerformAttack(float dt)
        {
            if(fired)return;
            fired=true;
            seed=Instantiate(Data.projectilePrefab,Body.position+AttackDirection*.45f,Quaternion.identity).GetComponent<MarshSeedProjectile>();
            seed.Launch(this,AttackDirection);
        }
        private void ClearSeed(){if(seed!=null)seed.Cancel();seed=null;}
        protected override void OnDied(){ClearSeed();base.OnDied();}
        public override void ResetEncounter(){ClearSeed();fired=false;base.ResetEncounter();}
    }
}

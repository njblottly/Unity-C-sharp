namespace DuckAdventure.Agent
{
    public sealed class ReedSkitter : EnemyAgent
    {
        protected override MarshEnemyKind Kind=>MarshEnemyKind.Skitter;
        protected override void PerformAttack(float dt)=>LungeAndStrike(dt);
    }
}

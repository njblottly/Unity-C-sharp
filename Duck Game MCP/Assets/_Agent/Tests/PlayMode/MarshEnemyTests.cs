using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace Marshlight.PlayModeTests
{
    public sealed class MarshEnemyTests:CampaignTestBase
    {
        private Component Skitter=>All("ReedSkitter").Single(c=>c.name=="skitter-0");
        private Component Crab=>All("SiltCrab").Single(c=>c.name=="crab-1");
        private float HP=>Convert.ToSingle(Field(Field(player,"hitPoints"),"value"));
        private static Vector2 Position(Component actor)=>actor.GetComponent<Rigidbody2D>().position;
        [Test] public void NewEnemiesReadTheirSectionAssetsAndKeepPixelImports()
        {
            var actors=All("ReedSkitter").Concat(All("SiltCrab")).ToArray();int expected=((Array)Field(Read(campaign,"Data"),"sections")).Cast<object>().Sum(s=>((Array)Field(s,"encounters")).Cast<object>().Count(e=>new[]{"Skitter","Crab"}.Contains(Field(Field(e,"data"),"kind").ToString())));Assert.That(actors.Length,Is.EqualTo(expected));
            foreach(var actor in actors)
            {
                var data=Read(actor,"Data");Call(data,"ValidateData");Assert.That((int)Read(Read(actor,"Health"),"MaxHitPoints"),Is.EqualTo((int)Field(data,"health")));
                var sprite=(Sprite)Field(data,"sprite");Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(32,32)));Assert.That(sprite.pixelsPerUnit,Is.EqualTo(32));Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));
            }
            Assert.That(All("PlayerDamageFeedback").Length,Is.EqualTo(1));
            Assert.That(Read(All("MarshPatrol").Single(),"Definition"),Is.Not.Null,"Retained patrol is configured from a section asset");
        }
        [UnityTest] public IEnumerator AllEnemyDamageSourcesSharePlayerInvincibility()
        {
            Warp(new Vector2(0,-2));float before=HP;Assert.That((bool)Call(player,"TryTakeEnemyDamage",1),Is.True);Assert.That(HP,Is.EqualTo(before-1));Assert.That((bool)Call(player,"TryTakeEnemyDamage",1),Is.False);Assert.That(HP,Is.EqualTo(before-1));
            yield return new WaitForSeconds(Convert.ToSingle(Field(player,"enemyDamageCooldown"))+.06f);
            Assert.That((bool)Call(player,"TryTakeEnemyDamage",1),Is.True);Assert.That(HP,Is.EqualTo(before-2));
        }
        [UnityTest] public IEnumerator SkitterTelegraphsAndCommitsToItsMarkedDirection()
        {
            var actor=Skitter;Warp(Position(actor)+Vector2.left*1.2f);yield return new WaitForFixedUpdate();yield return null;
            Assert.That(Read(actor,"Phase").ToString(),Is.EqualTo("Telegraph"));Assert.That((bool)Read(actor,"TelegraphVisible"),Is.True);
            Vector2 direction=(Vector2)Read(actor,"AttackDirection");float before=HP;Warp(Position(actor)+Vector2.down*3.5f);
            yield return new WaitForSeconds(.15f);Assert.That((Vector2)Read(actor,"AttackDirection"),Is.EqualTo(direction));Assert.That(HP,Is.EqualTo(before));
            yield return new WaitForSeconds(.9f);Assert.That(HP,Is.EqualTo(before),"Sidestepping the locked lane avoids the attack");
        }
        [UnityTest] public IEnumerator RealSkitterAttackDealsOnlyOneProtectedHit()
        {
            var actor=Skitter;Warp(Position(actor)+Vector2.left*1.15f);float before=HP;
            yield return new WaitForSeconds(.98f);
            Assert.That(HP,Is.EqualTo(before-1));Assert.That(Convert.ToSingle(Read(player,"InvulnerabilityRemaining")),Is.GreaterThan(0));
        }
        [UnityTest] public IEnumerator SwordUsesExistingHealthAndProducesFlashAndKnockback()
        {
            yield return PickupSword();var actor=Skitter;Vector2 origin=Position(actor);Warp(origin+Vector2.left*.8f);int before=(int)Read(Read(actor,"Health"),"CurrentHitPoints");
            Assert.That((bool)Call(Read(level,"Weapon"),"TryAttack",Vector2.right),Is.True);
            Assert.That((int)Read(Read(actor,"Health"),"CurrentHitPoints"),Is.EqualTo(before-1));Assert.That(Read(actor,"Phase").ToString(),Is.EqualTo("Stagger"));
            var feedback=actor.GetComponents<MonoBehaviour>().Single(c=>c.GetType().Name=="MarshEnemyFeedback");Assert.That((bool)Read(feedback,"IsFlashing"),Is.True);
            var renderer=actor.GetComponentsInChildren<SpriteRenderer>().Single(s=>s.name=="Body");var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block);Assert.That(block.GetFloat("_HitFlash"),Is.EqualTo(1));Assert.That(renderer.sharedMaterial.shader.isSupported,Is.True);
            yield return new WaitForSeconds(.08f);Assert.That(Position(actor).x,Is.GreaterThan(origin.x));Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));
        }
        [UnityTest] public IEnumerator CrabRejectsFrontalSwordButAllowsAFlank()
        {
            yield return PickupSword();var actor=Crab;Vector2 facing=(Vector2)Read(actor,"Facing");Warp(Position(actor)+facing*.9f);int before=(int)Read(Read(actor,"Health"),"CurrentHitPoints");
            Assert.That((bool)Call(Read(level,"Weapon"),"TryAttack",-facing),Is.True);Assert.That((int)Read(Read(actor,"Health"),"CurrentHitPoints"),Is.EqualTo(before));
            yield return new WaitForSeconds(.57f);facing=(Vector2)Read(actor,"Facing");Warp(Position(actor)-facing*.9f);
            Assert.That((bool)Call(Read(level,"Weapon"),"TryAttack",facing),Is.True);Assert.That((int)Read(Read(actor,"Health"),"CurrentHitPoints"),Is.EqualTo(before-1));
        }
        [UnityTest] public IEnumerator CrabRecoveryIsVulnerableFromTheFront()
        {
            yield return PickupSword();var actor=Crab;Vector2 facing=(Vector2)Read(actor,"Facing");Warp(Position(actor)+facing*.95f);float deadline=Time.time+3;
            while(Read(actor,"Phase").ToString()!="Recovery"&&Time.time<deadline)yield return null;
            Assert.That(Read(actor,"Phase").ToString(),Is.EqualTo("Recovery"));facing=(Vector2)Read(actor,"Facing");Warp(Position(actor)+facing*.9f);int before=(int)Read(Read(actor,"Health"),"CurrentHitPoints");
            Assert.That((bool)Call(Read(level,"Weapon"),"TryAttack",-facing),Is.True);Assert.That((int)Read(Read(actor,"Health"),"CurrentHitPoints"),Is.EqualTo(before-1));
        }
        [Test] public void DefeatedEnemyResetRestoresHealthCollisionAndPhysics()
        {
            var actor=Skitter;var health=Read(actor,"Health");Call(health,"TakeDamage",999);Assert.That((bool)Read(health,"IsDead"),Is.True);Assert.That(actor.gameObject.activeSelf,Is.False);Assert.That(actor.GetComponent<Collider2D>().enabled,Is.False);Assert.That(actor.GetComponent<Rigidbody2D>().simulated,Is.False);
            Call(actor,"ResetEncounter");Assert.That((bool)Read(health,"IsDead"),Is.False);Assert.That(actor.gameObject.activeSelf,Is.True);Assert.That(actor.GetComponent<Collider2D>().enabled,Is.True);Assert.That(actor.GetComponent<Rigidbody2D>().simulated,Is.True);Assert.That((int)Read(health,"CurrentHitPoints"),Is.EqualTo((int)Read(health,"MaxHitPoints")));
        }
        [Test] public void CheckpointDeathResetsNewEnemiesWithoutLosingMainLoan()
        {
            var actor=Skitter;Call(Read(actor,"Health"),"TakeDamage",999);Use("bell-0");Call(player,"TakeDamage",999);Call(level,"RestartCheckpoint");
            Assert.That(actor.gameObject.activeSelf,Is.True);Assert.That((bool)Read(Read(actor,"Health"),"IsDead"),Is.False);Assert.That((int)Read(level,"CoinsInEscrow"),Is.EqualTo(3));Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));Use("tray-0");Finish();
        }
        [Test] public void DefeatingEveryEnemyCannotPayOrBypassAnyMainGate()
        {
            foreach(var health in All("EnemyHitPoints"))Call(health,"TakeDamage",999);
            Assert.That((int)Read(level,"BellsOpened"),Is.Zero);Assert.That(Coins,Is.EqualTo(3));Assert.That(Gates.All(g=>(bool)Read(g,"GateClosed")),Is.True);Finish();
        }
        [UnityTest] public IEnumerator PauseFreezesTheEnemyTelegraphAndMovement()
        {
            var actor=Skitter;Warp(Position(actor)+Vector2.left*1.2f);yield return new WaitForFixedUpdate();yield return null;Assert.That(Read(actor,"Phase").ToString(),Is.EqualTo("Telegraph"));
            Vector2 position=Position(actor);Call(level,"TogglePause");yield return new WaitForSecondsRealtime(.12f);Assert.That(Position(actor),Is.EqualTo(position));Assert.That(Read(actor,"Phase").ToString(),Is.EqualTo("Telegraph"));Call(level,"TogglePause");Assert.That(State,Is.EqualTo("Playing"));
        }
        [UnityTest] public IEnumerator MechanicalTelemetryExcludesFrozenCaptureFrames()
        {
            Warp(new Vector2(0,-2));var pacing=Read(level,"Pacing");Call(pacing,"SetSimulationClock",true);
            Func<float> seconds=()=>((Array)Read(pacing,"Sections")).Cast<object>().Sum(row=>Convert.ToSingle(Field(row,"activeSeconds")));
            float before=seconds();Time.timeScale=0;yield return new WaitForSecondsRealtime(.15f);
            Assert.That(seconds(),Is.EqualTo(before).Within(.001f),"A held capture adds no active simulation time");
            Time.timeScale=1;yield return new WaitForSeconds(.12f);
            Assert.That(seconds()-before,Is.InRange(.08f,.6f));Assert.That((bool)Read(pacing,"SimulationClock"),Is.True);
        }

    }
}

using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
namespace Marshlight.PlayModeTests
{
    public sealed class MarshRemainingEnemyTests:CampaignTestBase
    {
        private Component Moth=>All("WickMoth").Single(c=>c.name=="moth-2");
        private Component Sprout=>All("CounterweightSprout").Single(c=>c.name=="sprout-3");
        private float HP=>Convert.ToSingle(Field(Field(player,"hitPoints"),"value"));
        private Vector2 Pos(Component c)=>c.GetComponent<Rigidbody2D>().position;
        private Component Launch(Vector2 at,Vector2 heading)
        {
            var prefab=(GameObject)Field(Read(Sprout,"Data"),"projectilePrefab");
            var go=Object.Instantiate(prefab,at,Quaternion.identity);var seed=go.GetComponents<Component>().Single(c=>c.GetType().Name=="MarshSeedProjectile");Call(seed,"Launch",Sprout,heading);return seed;
        }
        [Test] public void AllFourNewKindsHaveConfiguredEncounters()
        {
            foreach(string name in new[]{"ReedSkitter","SiltCrab","WickMoth","CounterweightSprout"})Assert.That(All(name).Length,Is.GreaterThan(0));
            Assert.That(All("WickMoth").Length,Is.EqualTo(3));Assert.That(All("CounterweightSprout").Length,Is.EqualTo(2));
            foreach(var actor in All("WickMoth").Concat(All("CounterweightSprout"))){Call(Read(actor,"Data"),"ValidateData");Assert.That((int)Read(Read(actor,"Health"),"MaxHitPoints"),Is.EqualTo((int)Field(Read(actor,"Data"),"health")));}
        }
        [UnityTest] public IEnumerator MothWarnsLocksItsDiveAndCanBeDodged()
        {
            Warp(Pos(Moth)+Vector2.left*1.5f);yield return new WaitForFixedUpdate();yield return null;Assert.That(Read(Moth,"Phase").ToString(),Is.EqualTo("Telegraph"));Assert.That((bool)Read(Moth,"TelegraphVisible"),Is.True);
            var direction=(Vector2)Read(Moth,"AttackDirection");float before=HP;Warp(Pos(Moth)+Vector2.down*3);
            yield return new WaitForSeconds(.2f);Assert.That((Vector2)Read(Moth,"AttackDirection"),Is.EqualTo(direction));yield return new WaitForSeconds(1.3f);
            Assert.That(HP,Is.EqualTo(before));Assert.That(Vector2.Distance(Pos(Moth),(Vector2)Read(Moth,"SpawnPosition")),Is.LessThanOrEqualTo((float)Field(Read(Moth,"Data"),"leashDistance")+.03f));
        }
        [UnityTest] public IEnumerator MothUsesExistingHealthAndSharedDamageCooldown()
        {
            Warp(Pos(Moth)+Vector2.left*1.15f);float before=HP;yield return new WaitForSeconds(1.4f);
            Assert.That(HP,Is.LessThan(before));Assert.That(before-HP,Is.EqualTo(1));Call(Moth,"TryReceiveWeaponHit",99,(Vector2)player.transform.position);Assert.That((bool)Read(Read(Moth,"Health"),"IsDead"),Is.True);Assert.That(Coins,Is.EqualTo(3));Assert.That((int)Read(level,"BellsOpened"),Is.Zero);
        }
        [UnityTest] public IEnumerator SproutWarnsThenHasAtMostOneLiveSeed()
        {
            Warp(Pos(Sprout)+Vector2.left*2);yield return new WaitForFixedUpdate();yield return null;Assert.That(Read(Sprout,"Phase").ToString(),Is.EqualTo("Telegraph"));yield return new WaitForSeconds(1.2f);
            Assert.That((bool)Read(Sprout,"HasProjectile"),Is.True);Assert.That(All("MarshSeedProjectile").Length,Is.EqualTo(1));var seed=(Component)Read(Sprout,"CurrentProjectile");Assert.That(Read(seed,"Owner"),Is.SameAs(Sprout));
            Warp(new Vector2(90,4));yield return new WaitForSeconds(1.6f);Assert.That(All("MarshSeedProjectile").Length,Is.Zero);
        }
        [UnityTest] public IEnumerator RealSeedHitsPlayerOnlyOnceAndHonorsInvincibility()
        {
            Warp(new Vector2(96,0));float before=HP;Launch(new Vector2(95.5f,0),Vector2.right);yield return new WaitForSeconds(.4f);Assert.That(HP,Is.EqualTo(before-1));
            Launch(new Vector2(95.5f,0),Vector2.right);yield return new WaitForSeconds(.3f);Assert.That(HP,Is.EqualTo(before-1));Assert.That(All("MarshSeedProjectile").Length,Is.Zero);
        }
        [UnityTest] public IEnumerator SeedStopsAtWaterBankInsteadOfDamagingAcrossIt()
        {
            Warp(new Vector2(118.5f,-3));float before=HP;var seed=Launch(new Vector2(114,-3),Vector2.right);yield return new WaitForSeconds(.8f);Assert.That(seed==null,Is.True);Assert.That(HP,Is.EqualTo(before));Assert.That((bool)Read(Gates[3],"GateClosed"),Is.True);
        }
        [UnityTest] public IEnumerator SeedExpiresAtItsConfiguredRange()
        {
            Warp(new Vector2(90,4));var seed=Launch(new Vector2(94,0),Vector2.right);yield return new WaitForSeconds(1.4f);Assert.That(seed==null,Is.True);
        }
        [UnityTest] public IEnumerator CheckpointResetAndOwnerDeathClearOutstandingSeeds()
        {
            Warp(Pos(Sprout)+Vector2.left*2);yield return new WaitForSeconds(1.2f);Assert.That((bool)Read(Sprout,"HasProjectile"),Is.True);Call(Sprout,"ResetEncounter");yield return null;Assert.That((bool)Read(Sprout,"HasProjectile"),Is.False);Assert.That(All("MarshSeedProjectile").Length,Is.Zero);
            Call(Sprout,"TryReceiveWeaponHit",99,Pos(Sprout)+Vector2.left);Assert.That((bool)Read(Read(Sprout,"Health"),"IsDead"),Is.True);Call(Sprout,"ResetEncounter");Warp(Pos(Sprout)+Vector2.left*2);yield return new WaitForSeconds(1.2f);
            Assert.That((bool)Read(Sprout,"HasProjectile"),Is.True);Call(Sprout,"TryReceiveWeaponHit",99,Pos(Sprout)+Vector2.left);yield return null;Assert.That(All("MarshSeedProjectile").Length,Is.Zero);
        }
        [Test] public void LaterCurveComesFromAssetsWithoutIncreasingDamage()
        {
            foreach(var a in All("WickMoth").Concat(All("CounterweightSprout"))){Assert.That((int)Field(Read(a,"Data"),"attackDamage"),Is.EqualTo(1));Assert.That((float)Field(Read(a,"Data"),"telegraphSeconds"),Is.GreaterThanOrEqualTo(.8f));}
            Assert.That((float)Field(Read(All("WickMoth").Single(a=>a.name=="moth-4"),"Data"),"telegraphSeconds"),Is.GreaterThan((float)Field(Read(Moth,"Data"),"telegraphSeconds")));
        }
    }
}

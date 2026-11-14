using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace Marshlight.PlayModeTests
{
    public sealed class MarshMechanicTests:CampaignTestBase
    {
        private void PlaceOnly(Vector2 pos){var rb=player.GetComponent<Rigidbody2D>();rb.position=pos;rb.linearVelocity=Vector2.zero;player.transform.position=pos;Physics2D.SyncTransforms();}
        private void ReachChapter(int cp)
        {
            for(int i=0;i<cp;i++){PrepareGate(i);if(!(bool)Read(Gates[i],"Opened"))Use((string)Read(Gates[i],"Id"));if(!(bool)Read(Node(Tray(i)),"Reclaimed"))Use(Tray(i));}
            Call(level,"ReachCheckpoint",Node("checkpoint-"+cp),((Array)Read(level,"Checkpoints")).GetValue(cp));
        }
        [Test] public void MechanicAndCollectibleDefinitionsAreUniqueAndGateCostsStayThree()
        {
            Assert.That(((Array)Read(campaign,"Mechanics")).Length,Is.EqualTo(7));Assert.That(((Array)Read(campaign,"Marks")).Length,Is.EqualTo(8));
            Assert.That(All("DuckPerch").Length,Is.EqualTo(3));Assert.That(All("InspectionLamp").Length,Is.EqualTo(2));Assert.That(All("MailRouting").Length,Is.EqualTo(2));
            foreach(var m in ((Array)Read(campaign,"Mechanics")).Cast<object>())Call(Read(m,"Data"),"ValidateData");
            foreach(var gate in Gates)Assert.That((int)Read(gate,"Cost"),Is.EqualTo(3));Call(campaign,"ValidateReferences");
        }
        [UnityTest] public IEnumerator DuckWalksToPerchAndInsideInspectionLatchesAFreeExit()
        {
            var perch=Interaction("perch-reeds");UseExtension("perch-reeds");Assert.That((bool)Read(perch,"Completed"),Is.False);
            yield return new WaitForSeconds(.65f);Assert.That((bool)Read(perch,"DuckReady"),Is.True);Assert.That((bool)Read(perch,"ShutterOpen"),Is.True);
            var goal=Interaction("perch-reeds-inspect");Assert.That((bool)Call(goal,"TryInteract"),Is.False,"Cannot inspect from the outside pad");
            PlaceOnly(goal.transform.position);Assert.That((bool)Call(goal,"TryInteract"),Is.True);
            Assert.That((bool)Read(perch,"Completed"),Is.True);Assert.That(Read(Read(level,"Duck"),"AssistanceTarget"),Is.Null);Assert.That((bool)Read(perch,"ShutterOpen"),Is.True);Assert.That(Coins,Is.EqualTo(3));Assert.That((bool)Read(Gates[1],"GateClosed"),Is.True);
        }
        [UnityTest] public IEnumerator RecallingInsideUnfinishedRoomCannotTrapThePlayer()
        {
            var perch=Interaction("perch-reeds");UseExtension("perch-reeds");yield return new WaitForSeconds(.65f);PlaceOnly(Interaction("perch-reeds-recall").transform.position);
            Assert.That((bool)Call(Interaction("perch-reeds-recall"),"TryInteract"),Is.True);yield return null;
            Assert.That((bool)Read(perch,"Completed"),Is.False);Assert.That((bool)Read(perch,"ShutterOpen"),Is.True);
            Call(player,"TakeDamage",999);Call(level,"RestartCheckpoint");yield return null;
            Assert.That(Read(Read(level,"Duck"),"AssistanceTarget"),Is.Null);Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));
        }
        [Test] public void AnUnassistedRoomCannotCompleteItsInspectionOrPayAMainGate()
        {
            Warp(new Vector2(29.3f,3.5f));Assert.That(Physics2D.Raycast(new Vector2(29.3f,3.5f),Vector2.right,1.4f,LayerMask.GetMask("Blocking")).collider,Is.Not.Null);
            var goal=Interaction("perch-reeds-inspect");Warp(goal.transform.position);Assert.That((bool)Call(goal,"TryInteract"),Is.False);Assert.That((bool)Read(Interaction("perch-reeds"),"Completed"),Is.False);Assert.That((bool)Read(Gates[1],"GateClosed"),Is.True);Assert.That(Coins,Is.EqualTo(3));
        }
        [UnityTest] public IEnumerator ClosedShutterDoesNotOpenMerelyBecausePlayerApproaches()
        {
            PlaceOnly(new Vector2(29.65f,3.5f));yield return null;yield return new WaitForFixedUpdate();
            Assert.That((bool)Read(Interaction("perch-reeds"),"ShutterOpen"),Is.False);Assert.That(Read(Read(level,"Duck"),"AssistanceTarget"),Is.Null);
        }
        [Test] public void LampBorrowsOnceAndPauseRefundsWhileKeepingTheClue()
        {
            var lamp=Interaction("lamp-archive");UseExtension("lamp-archive");Assert.That(Coins,Is.EqualTo(2));Assert.That((int)Read(lamp,"CoinsInEscrow"),Is.EqualTo(1));Assert.That((bool)Read(lamp,"Completed"),Is.True);
            Call(level,"TogglePause");Assert.That(Coins,Is.EqualTo(3));Assert.That((int)Read(lamp,"CoinsInEscrow"),Is.Zero);Assert.That((bool)Read(lamp,"IsLit"),Is.False);Call(level,"TogglePause");UseExtension("lamp-archive");UseExtension("lamp-archive");Assert.That(Coins,Is.EqualTo(3));
        }
        [UnityTest] public IEnumerator BlockedLampRefundStaysInEscrowUntilInventoryHasSpace()
        {
            UseExtension("lamp-archive");UseExtension("route-storm");Assert.That(Coins,Is.Zero);
            var sword=Field(All("Consumable").Single(c=>Field(Field(c,"item"),"itemType").ToString()=="SWORD"),"item");
            Assert.That((bool)Call(inventory,"TryAddItems",sword,5),Is.True);Call(level,"TogglePause");Assert.That((int)Read(Interaction("lamp-archive"),"CoinsInEscrow"),Is.EqualTo(1));Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));Call(level,"TogglePause");
            Type item=coinType.GetType();Assert.That((bool)Call(inventory,"TrySpend",Enum.Parse(item,"SWORD"),1),Is.True);yield return null;yield return null;
            Assert.That((int)Read(Interaction("lamp-archive"),"CoinsInEscrow"),Is.Zero);Assert.That(Coins,Is.EqualTo(1));UseExtension("route-storm-low");Assert.That(Coins,Is.EqualTo(3));
        }
        [TestCase(0)][TestCase(1)] public void EitherMailAddressReturnsExactlyTheSameTwoCoins(int address)
        {
            if(address==1)PrepareMechanic("perch-storm");var route=Interaction("route-storm");UseExtension("route-storm");if(address==1)UseExtension("route-storm");
            Assert.That(((string)Read(route,"Prompt")).StartsWith("Address: Storm "+(address==0?"LOW":"HIGH")),Is.True);Assert.That(Coins,Is.EqualTo(1));Assert.That((int)Read(route,"CoinsInEscrow"),Is.EqualTo(2));var wrong=Interaction(address==0?"route-storm-high":"route-storm-low");Warp(wrong.transform.position);Assert.That((bool)Call(wrong,"TryInteract"),Is.False);
            UseExtension(address==0?"route-storm-low":"route-storm-high");Assert.That(Coins,Is.EqualTo(3));Assert.That((bool)Read(route,"Completed"),Is.True);
            Assert.That((bool)Call(Interaction(address==0?"route-storm-low":"route-storm-high"),"TryInteract"),Is.False);Warp(route.transform.position);Assert.That((bool)Call(route,"TryInteract"),Is.False);Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));Assert.That(Gates.All(g=>(bool)Read(g,"GateClosed")),Is.True);
        }
        [Test] public void LockedHighRoadCanAlwaysBeReroutedToTheOpenLowRoad()
        {
            UseExtension("route-storm");UseExtension("route-storm");var high=Interaction("route-storm-high");Warp(high.transform.position);Assert.That((bool)Call(high,"TryInteract"),Is.False);
            UseExtension("route-storm");UseExtension("route-storm-low");Assert.That(Coins,Is.EqualTo(3));Assert.That((bool)Read(Interaction("perch-storm"),"Completed"),Is.False);
        }
        [TestCase("route-storm",3,0)][TestCase("route-storm",3,1)][TestCase("route-haven",4,0)][TestCase("route-haven",4,1)]
        public void MailLoanAndAddressSurviveDeathAtTheirCheckpoint(string id,int checkpoint,int address)
        {
            ReachChapter(checkpoint);
            if(id=="route-haven"){PrepareMechanic("perch-haven");PrepareMechanic("lamp-haven");}
            if(id=="route-storm"&&address==1)PrepareMechanic("perch-storm");
            UseExtension(id);if(address==1)UseExtension(id);Call(player,"TakeDamage",999);Assert.That(State,Is.EqualTo("Defeat"));Call(level,"RestartCheckpoint");
            Assert.That((int)Read(Interaction(id),"SelectedDestination"),Is.EqualTo(address));Assert.That((int)Read(Interaction(id),"CoinsInEscrow"),Is.EqualTo(2));Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));
            UseExtension(id+(address==0?"-low":"-high"));Finish();
        }
        [Test] public void LampDeathRefundAndCheckpointRetainInspectionWithoutOpeningGate()
        {
            ReachChapter(2);UseExtension("lamp-archive");Call(player,"TakeDamage",999);Assert.That(Coins,Is.EqualTo(3));Call(level,"RestartCheckpoint");Assert.That((bool)Read(Interaction("lamp-archive"),"Completed"),Is.True);Assert.That((bool)Read(Gates[2],"GateClosed"),Is.True);Finish();
        }
        [Test] public void MechanicPrerequisitesNeverReplaceMainCoinPayment()
        {
            for(int i=0;i<5;i++)PrepareRequirements(i);Assert.That(Gates.All(g=>(bool)Read(g,"GateClosed")),Is.True);Assert.That(Coins,Is.EqualTo(3));
            UseExtension("lamp-archive");Warp(Gates[0].transform.position);Assert.That((bool)Call(Gates[0],"TryInteract"),Is.False);Assert.That((bool)Read(Gates[0],"GateClosed"),Is.True);UseExtension("lamp-archive");Finish();
        }
        [UnityTest] public IEnumerator ReedMarksAreUniqueCosmeticAndResetOnlyOnReplay()
        {
            var authored=((Array)Read(campaign,"Marks")).Cast<Component>().ToArray();
            for(int i=0;i<authored.Length;i++)
            {
                UseExtension((string)Read(authored[i],"Id"));Assert.That((int)Read(campaign,"MarkCount"),Is.EqualTo(i+1));Assert.That((bool)Read(campaign,"HasBadge"),Is.EqualTo(i>=3));Assert.That((bool)Read(campaign,"HasCoda"),Is.EqualTo(i==7));Assert.That((bool)Call(authored[i],"TryInteract"),Is.False);Assert.That(Coins,Is.EqualTo(3));
            }
            yield return null;Assert.That((string)Read(Field(level,"ledgerText"),"text"),Does.Contain("BADGE"));
            Assert.That((string)Read(campaign,"EndingCoda"),Does.Contain("eight"));Assert.That(Gates.All(g=>(bool)Read(g,"GateClosed")),Is.True);Call(player,"TakeDamage",999);Call(level,"RestartCheckpoint");Assert.That((int)Read(campaign,"MarkCount"),Is.EqualTo(8));
            Finish();Call(level,"RestartRun");yield return null;yield return null;campaign=Find("MarshCampaign");Assert.That((int)Read(campaign,"MarkCount"),Is.Zero);Assert.That((bool)Read(campaign,"HasBadge"),Is.False);Assert.That(((Array)Read(campaign,"Mechanics")).Cast<object>().All(m=>!(bool)Read(m,"Completed")),Is.True);
        }
        [Test] public void PagedJournalUsesExistingMenuAndPausesWithoutSpendingCoins()
        {
            UseExtension("reed-mark-1");UseExtension("lamp-archive");Call(level,"ToggleJournal");Assert.That((bool)Read(level,"JournalOpen"),Is.True);Assert.That(State,Is.EqualTo("Paused"));Assert.That(Time.timeScale,Is.Zero);Assert.That(Coins,Is.EqualTo(3));
            var pages=(string[])Read(campaign,"JournalPages");Assert.That(pages.Length,Is.GreaterThan(1));Assert.That(pages.All(p=>p.Length<800),Is.True);Call(level,"ChangeJournalPage",1);Assert.That((int)Read(level,"JournalPage"),Is.EqualTo(1));Call(level,"TogglePause");Assert.That((bool)Read(level,"JournalOpen"),Is.False);Assert.That(State,Is.EqualTo("Playing"));Assert.That(Time.timeScale,Is.EqualTo(1));
        }
        [Test] public void AllNewTutorialBeatsFireOnce()
        {
            foreach(string id in new[]{"perch-story","lamp-story","mail-route-story"})
            {
                UseExtension(id);Warp(Interaction(id).transform.position);Assert.That((bool)Call(Interaction(id),"TryInteract"),Is.False);
            }
            foreach(string id in new[]{"perch-intro","lantern-intro","mail-route"})Assert.That((bool)Call(level,"HasRead",id),Is.True);
        }
        [Test] public void EveryReturnDestinationStaysBeforeItsMainWaterGate()
        {
            foreach(var route in All("MailRouting"))
            {
                int gate=(string)Read(route,"Id")=="route-storm"?3:4;float water=gate==3?116:150;
                foreach(var box in ((Array)Read(route,"Destinations")).Cast<Component>())Assert.That(box.transform.position.x,Is.LessThan(water-2));
                Assert.That((int)Read(Gates[gate],"Cost"),Is.EqualTo(3));
            }
        }
        [Test] public void EveryReachableCombinedResourceStateCanRecoverAndFinish()
        {
            var type=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("DuckAdventure.Agent.Editor.CampaignCoinAudit")).Single(t=>t!=null);
            var result=type.GetMethod("Run").Invoke(null,new object[]{campaign});Assert.That((bool)Field(result,"conserved"),Is.True);Assert.That((bool)Field(result,"allCanFinish"),Is.True);Assert.That((int)Field(result,"states"),Is.GreaterThan(100));Assert.That((int)Field(result,"recoverableStates"),Is.EqualTo((int)Field(result,"states")));Assert.That(((int[])Field(result,"stages")).All(c=>c>0),Is.True);Assert.That((int[])Field(result,"relayPhaseCoverage"),Is.All.EqualTo(7));Assert.That((int[])Field(result,"routePhaseCoverage"),Is.All.EqualTo(15));Assert.That((int)Field(result,"lampCombinationCoverage"),Is.EqualTo(15));
        }
    }
}

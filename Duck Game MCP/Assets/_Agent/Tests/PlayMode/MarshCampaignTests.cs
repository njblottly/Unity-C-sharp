using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
namespace Marshlight.PlayModeTests
{
    public abstract class CampaignTestBase
    {
        protected Component level, campaign, player, inventory;
        protected object coinType;
        protected static object Read(object obj,string property) => obj.GetType().GetProperty(property,BindingFlags.Instance|BindingFlags.Public).GetValue(obj);
        protected static object Field(object obj,string field) => obj.GetType().GetField(field,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).GetValue(obj);
        protected static object Call(object obj,string method,params object[] args)
        {
            var methods=obj.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Public).Where(m=>m.Name==method&&m.GetParameters().Length==args.Length).ToArray();
            Assert.That(methods.Length,Is.EqualTo(1),"Unambiguous public method: "+method);return methods[0].Invoke(obj,args);
        }
        protected static Component Find(string name) => Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(c=>c.GetType().Name==name);
        protected static Component[] All(string name) => Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.GetType().Name==name).Cast<Component>().ToArray();
        protected Component Node(string id) => ((Array)Read(level,"Nodes")).Cast<Component>().Single(n=>(string)Read(n,"Id")==id);
        protected Component Interaction(string id) => ((Array)Read(level,"Interactions")).Cast<Component>().Single(n=>(string)Read(n,"Id")==id);
        protected int Coins => (int)Call(inventory,"Count",coinType);
        protected string State => Read(level,"State").ToString();
        protected Component[] Gates => ((Array)Read(campaign,"MainGates")).Cast<Component>().ToArray();
        protected void Dismiss(){for(int i=0;i<12&&State=="Dialogue";i++)Call(level,"AdvanceStory");Assert.That(State,Is.Not.EqualTo("Dialogue"));}
        protected void Warp(Vector2 pos)
        {
            var body=player.GetComponent<Rigidbody2D>();body.position=pos;body.linearVelocity=Vector2.zero;player.transform.position=pos;
            Call(Read(level,"Duck"),"SnapToPlayer");Physics2D.SyncTransforms();
        }
        protected bool InteractNode(string id){var node=Node(id);Warp(node.transform.position);bool result=(bool)Call(node,"TryInteract");Dismiss();Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));return result;}
        protected void Use(string id)=>Assert.That(InteractNode(id),Is.True,"Interaction: "+id);
        protected void CompleteRelay()
        {
            var relay=Read(level,"CoinRelay");string[] ids={"reed","spill","exit"};
            for(int i=0;i<3;i++)
            {
                if(i==2 && !(bool)Call(relay,"IsReturned",0))Use("relay-reed-tray");
                if(!(bool)Call(relay,"IsLent",i))Use("relay-"+ids[i]+"-cup");
            }
            for(int i=0;i<3;i++)if(!(bool)Call(relay,"IsReturned",i))Use("relay-"+ids[i]+"-tray");
        }
        protected string Tray(int sequence)=>new[]{"tray-0","campaign-tray-reed","tray-1","campaign-tray-storm","tray-2"}[sequence];
        protected void UseExtension(string id)
        {
            var item=Interaction(id);Warp(item.transform.position);Assert.That((bool)Call(item,"TryInteract"),Is.True,id);Dismiss();Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));
        }
        protected void PrepareMechanic(string id)
        {
            var m=Interaction(id);if((bool)Read(m,"Completed"))return;
            if(m.GetType().Name=="DuckPerch")
            {
                var plaque=All("PerchControl").Single(c=>(bool)Field(c,"inspectionPlaque")&&ReferenceEquals(Field(c,"perch"),m));
                Warp(plaque.transform.position);
                var duck=(Component)Read(level,"Duck");var target=(Transform)Field(m,"perch");Call(duck,"HoldAt",target);
                duck.GetComponent<Rigidbody2D>().position=target.position;duck.transform.position=target.position;Physics2D.SyncTransforms();
                Assert.That((bool)Call(plaque,"TryInteract"),Is.True,"Fixture uses actual settled duck and inside plaque: "+id);
            }
            else if(m.GetType().Name=="InspectionLamp"){UseExtension(id);UseExtension(id);}
            else
            {
                if((int)Read(m,"CoinsInEscrow")==0)UseExtension(id);
                int selected=(int)Read(m,"SelectedDestination");var box=((Array)Read(m,"Destinations")).GetValue(selected);UseExtension((string)Read(box,"Id"));
            }
            Assert.That((bool)Read(m,"Completed"),Is.True,id);
        }
        protected void PrepareRequirements(int index)
        {
            var sections=(Array)Field(Read(campaign,"Data"),"sections");var gate=Field(sections.GetValue(index),"mainGate");
            if(gate==null)return;
            foreach(string id in (string[])Field(gate,"requiredMechanics"))PrepareMechanic(id);
        }
        protected void PrepareGate(int index)
        {
            for(int i=0;i<index;i++)
            {
                if(i==2 && !(bool)Read(Read(level,"CoinRelay"),"Completed"))CompleteRelay();
                PrepareRequirements(i);
                if(!(bool)Read(Gates[i],"Opened"))Use((string)Read(Gates[i],"Id"));
                if(!(bool)Read(Node(Tray(i)),"Reclaimed"))Use(Tray(i));
            }
            if(index>=2 && !(bool)Read(Read(level,"CoinRelay"),"Completed"))CompleteRelay();
            PrepareRequirements(index);
        }
        protected void Finish()
        {
            for(int i=0;i<5;i++)
            {
                PrepareGate(i);
                if(!(bool)Read(Gates[i],"Opened"))Use((string)Read(Gates[i],"Id"));
                if(!(bool)Read(Node(Tray(i)),"Reclaimed"))Use(Tray(i));
            }
            Warp(new Vector2(162,0));Assert.That((bool)Call(level,"TryWin"),Is.True);Dismiss();
            Assert.That(State,Is.EqualTo("Won"));Assert.That(Coins,Is.EqualTo(3));Assert.That((int)Read(level,"CoinsInEscrow"),Is.Zero);
        }
        protected IEnumerator PickupSword()
        {
            var item=All("Consumable").Single(p=>Field(Field(p,"item"),"itemType").ToString()=="SWORD");Warp(item.transform.position);yield return new WaitForFixedUpdate();yield return null;Dismiss();
            Assert.That((bool)Read(Read(level,"Weapon"),"HasSword"),Is.True);
        }
        [UnitySetUp] public IEnumerator SetUpCampaign()
        {
            Time.timeScale=1;SceneManager.LoadScene("Assets/_Agent/Scenes/Marshlight_Campaign.unity");yield return null;yield return null;
            level=Find("MarshLevel");campaign=(Component)Read(level,"Campaign");player=(Component)Read(level,"Player");inventory=(Component)Read(player,"PlayerInventory");
            Type item=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("Item")).Single(t=>t!=null);coinType=Enum.Parse(item.GetNestedType("ItemType"),"COIN");
            Assert.That(State,Is.EqualTo("Intro"));Call(level,"Begin");Dismiss();
            foreach(var coin in All("Consumable").Where(c=>Field(Field(c,"item"),"itemType").ToString()=="COIN")){Warp(coin.transform.position);yield return new WaitForFixedUpdate();yield return null;Dismiss();}
            Assert.That(Coins,Is.EqualTo(3));Call(Read(level,"Pacing"),"SetRunLabel","campaign-regression");
        }
        [UnityTearDown] public IEnumerator TearDownCampaign(){Time.timeScale=1;yield return null;}
    }
    public sealed class MarshCampaignTests : CampaignTestBase
    {
        [Test] public void FiveChaptersUseAssetsAndExistingManagers()
        {
            Assert.That(((Array)Field(Read(campaign,"Data"),"chapters")).Length,Is.EqualTo(5));Assert.That(Gates.Length,Is.EqualTo(5));Assert.That(((Array)Read(level,"Checkpoints")).Length,Is.EqualTo(6));
            foreach(string type in new[]{"MarshLevel","MarshCampaign","RPGGameManager","RPGCameraManager","Inventory","HealthBar","Player","DuckFollower","Weapon","MarshEquipmentHUD"})Assert.That(All(type).Length,Is.EqualTo(1),type);
            Call(campaign,"ValidateReferences");
        }
        [Test] public void ActOneAndTwoEntriesHaveExplicitBindings()
        {
            var data=Read(campaign,"Data");var story=Field(data,"story");
            foreach(string id in new[]{"intro","letter","sword","bell1","perch-intro","ledger","reedbank-bell","flood","lantern-intro","exchange","archive-pocket","bell2","sluice-warning"})Assert.That(Call(story,"Get",id),Is.Not.Null,id);
            Assert.That((string)Read(Interaction("perch-story"),"StoryId"),Is.EqualTo("perch-intro"));Assert.That((string)Read(Interaction("lamp-story"),"StoryId"),Is.EqualTo("lantern-intro"));Assert.That((string)Read(Interaction("sluice-story"),"StoryId"),Is.EqualTo("sluice-warning"));
        }
        [Test] public void StoryOncePolicyAndPrerequisitesDoNotRepeatProgress()
        {
            int before=(int)Read(level,"StoryCount");Assert.That((bool)Call(level,"TryShowStory","intro"),Is.False);Assert.That((bool)Call(level,"TryShowStory","sluice-warning"),Is.False);Assert.That((int)Read(level,"StoryCount"),Is.EqualTo(before));
            Use("reed-note");int count=(int)Read(level,"StoryCount");Use("reed-note");Assert.That((int)Read(level,"StoryCount"),Is.EqualTo(count));
        }
        [Test] public void TutorialAvailabilityMatchesImplementedCapabilities()
        {
            foreach(var pair in new[]{new[]{"perch","perch-intro"},new[]{"lantern","lantern-intro"}})
                Assert.That((bool)Call(level,"CanReadStory",pair[1]),Is.EqualTo((bool)Call(campaign,"HasCapability",pair[0])));
        }
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)] public void LaterMainGateCannotBypassEarlierPayments(int index)
        {
            var gate=Gates[index];Warp(gate.transform.position);Assert.That((bool)Call(gate,"TryInteract"),Is.False);Assert.That((bool)Read(gate,"GateClosed"),Is.True);Assert.That(Coins,Is.EqualTo(3));Assert.That((int)Read(level,"BellsOpened"),Is.Zero);
        }
        [TestCase(0)][TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)] public void EveryCampaignCheckpointRecoversAnOutstandingMainLoan(int index)
        {
            int gate=Mathf.Min(index,4);PrepareGate(gate);var cp=Node("checkpoint-"+index);var spawn=((Array)Read(level,"Checkpoints")).GetValue(index);
            if(index<5)Call(level,"ReachCheckpoint",cp,spawn);
            Use((string)Read(Gates[gate],"Id"));if(index==5)Call(level,"ReachCheckpoint",cp,spawn);
            Call(player,"TakeDamage",999);Assert.That(State,Is.EqualTo("Defeat"));Call(level,"RestartCheckpoint");
            Assert.That(Vector2.Distance(player.GetComponent<Rigidbody2D>().position,((Component)spawn).transform.position),Is.LessThan(.15f));Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));Assert.That((int)Read(level,"CoinsInEscrow"),Is.EqualTo(3));
            Use(Tray(gate));Finish();
        }
        [UnityTest] public IEnumerator CampaignCompletesAndReplayResetsAllFiveGates()
        {
            yield return PickupSword();Use("keeper-note");Use("reed-note");Use("archive-note");Use("archive-pocket");Use("storm-note");Finish();
            Call(level,"RestartRun");yield return null;yield return null;level=Find("MarshLevel");campaign=(Component)Read(level,"Campaign");player=(Component)Read(level,"Player");inventory=(Component)Read(player,"PlayerInventory");
            Assert.That(State,Is.EqualTo("Intro"));Assert.That((int)Read(level,"BellsOpened"),Is.Zero);Assert.That(Coins,Is.Zero);Assert.That((int)Read(level,"RecoverableCoins"),Is.EqualTo(3));Assert.That(Gates.All(g=>(bool)Read(g,"GateClosed")),Is.True);
        }
    }
}

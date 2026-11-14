#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;
namespace DuckAdventure.Agent.Editor
{
    public static class MarshMechanicBuilder
    {
        private const string Folder="Assets/_Agent/Data/Campaign/Mechanics/";
        private static MarshLevel level;
        private static Transform group;
        private static Sprite square,letter,mailbox,lamp,duckSprite;
        private static Material material;
        private static Font font;
        private static readonly List<MarshInteraction> interactions=new List<MarshInteraction>();
        private static readonly List<MarshMechanic> mechanics=new List<MarshMechanic>();
        private static readonly List<ReedMark> marks=new List<ReedMark>();
        private static void Ref(Object obj,string key,Object value)=>MarshCampaignBuilder.Ref(obj,key,value);
        private static void Text(Object obj,string key,string value)=>MarshCampaignBuilder.Text(obj,key,value);
        private static SpriteRenderer Art(string name,Transform parent,Vector2 position,Vector2 size,Sprite sprite,Color color,int order=50,bool solid=false)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=position;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=material;sr.color=color;sr.sortingOrder=order;
            // Keep root collider sizes independent from decorative scale.
            go.transform.localScale=new Vector3(size.x/(sprite.rect.width/sprite.pixelsPerUnit),size.y/(sprite.rect.height/sprite.pixelsPerUnit),1);
            if(solid){go.layer=LayerMask.NameToLayer("Blocking");var c=go.AddComponent<BoxCollider2D>();c.size=sprite.rect.size/sprite.pixelsPerUnit;}return sr;
        }
        private static void Label(Transform parent,Vector2 position,string text)
        {
            var go=new GameObject("Label "+text);go.transform.SetParent(parent,false);go.transform.position=position;
            var t=go.AddComponent<TextMesh>();t.text=text;t.font=font;t.fontSize=32;t.characterSize=.08f;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;t.color=new Color(.96f,.89f,.69f);go.GetComponent<MeshRenderer>().sharedMaterial=font.material;go.GetComponent<MeshRenderer>().sortingOrder=80;
        }
        private static T Interaction<T>(string id,Vector2 position) where T:MarshInteraction
        {
            var root=new GameObject(id);root.transform.SetParent(group,false);root.transform.position=position;
            var trigger=root.AddComponent<BoxCollider2D>();trigger.isTrigger=true;trigger.size=new Vector2(.65f,.65f);
            var c=root.AddComponent<T>();Ref(c,"level",level);Text(c,"interactionId",id);interactions.Add(c);return c;
        }
        private static MechanicData Definition(string id,string title,MarshMechanicKind kind,string hint,string memory,string prerequisite="")
        {
            var data=MarshCampaignBuilder.Asset<MechanicData>(Folder+id+".asset");data.id=id;data.title=title;data.kind=kind;data.cost=kind==MarshMechanicKind.Perch?0:kind==MarshMechanicKind.InspectionLamp?1:2;data.hint=hint;data.journalText=memory;data.prerequisite=prerequisite;data.ValidateData();EditorUtility.SetDirty(data);return data;
        }
        private static DuckPerch Perch(string id,float x)
        {
            var pad=new Vector2(x,3.5f);var controller=Interaction<DuckPerch>(id,pad);
            var data=Definition(id,id=="perch-reeds"?"Listening perch":id=="perch-storm"?"High-road perch":"Haven inspection perch",MarshMechanicKind.Perch,
                "Inspect the "+(id=="perch-reeds"?"Listening Reeds":id=="perch-haven"?"Haven":"optional high-road")+" room: E at its duck perch, enter the open shutter, then E at the INSPECT plaque.",
                "Duckduckgoose holds the old reed lever while Aiken checks the latch. A small job done together; the way stays open for the next traveller.");
            Ref(controller,"definition",data);mechanics.Add(controller);
            var anchor=new GameObject("Duck settling point");anchor.transform.SetParent(controller.transform,false);Ref(controller,"perch",anchor.transform);
            Art("Perch PLACEHOLDER outline",controller.transform,pad,new Vector2(.9f,.6f),square,Color.black);
            Art("Perch reed top",controller.transform,pad,new Vector2(.78f,.48f),square,new Color(.34f,.46f,.27f),51);
            Art("Duck pictogram",controller.transform,pad+Vector2.up*.2f,new Vector2(.4f,.4f),duckSprite,Color.white,52);
            Label(controller.transform,pad+Vector2.up*.85f,"PERCH / E CALL");
            float west=x+1.5f,east=x+5.5f;
            var gate=Art("Inspection shutter",group,new Vector2(west,3.5f),new Vector2(.24f,3),square,new Color(.4f,.31f,.22f),55,true);
            Art("Room north rail",group,new Vector2((west+east)*.5f,5),new Vector2(4.3f,.24f),square,new Color(.2f,.28f,.22f),55,true);
            Art("Room south rail",group,new Vector2((west+east)*.5f,2),new Vector2(4.3f,.24f),square,new Color(.2f,.28f,.22f),55,true);
            Art("Room east rail",group,new Vector2(east,3.5f),new Vector2(.24f,3.24f),square,new Color(.2f,.28f,.22f),55,true);
            Ref(controller,"shutter",gate.gameObject);Ref(controller,"shutterCollider",gate.GetComponent<BoxCollider2D>());
            var so=new SerializedObject(controller);so.FindProperty("safeRoom").rectValue=new Rect(west+.25f,2.25f,3.5f,2.5f);so.ApplyModifiedPropertiesWithoutUndo();
            var inside=Interaction<PerchControl>(id+"-recall",new Vector2(west+1,3.5f));Ref(inside,"perch",controller);Label(inside.transform,inside.transform.position+Vector3.down*.85f,"E CALL / RECALL");
            var goal=Interaction<PerchControl>(id+"-inspect",new Vector2(east-1,3.5f));Ref(goal,"perch",controller);so=new SerializedObject(goal);so.FindProperty("inspectionPlaque").boolValue=true;so.ApplyModifiedPropertiesWithoutUndo();
            Art("Inspection plaque PLACEHOLDER",goal.transform,goal.transform.position,new Vector2(.45f,.45f),letter,Color.white);
            Label(goal.transform,goal.transform.position+Vector3.up*.65f,"INSPECT");
            return controller;
        }
        private static InspectionLamp Lamp(string id,Vector2 position,string prerequisite)
        {
            var controller=Interaction<InspectionLamp>(id,position);
            var data=Definition(id,id=="lamp-archive"?"Archive inspection lamp":"Haven routing lamp",MarshMechanicKind.InspectionLamp,
                "Inspect the "+(id=="lamp-archive"?"Archive lamp":"lamp inside the Haven perch room")+" with one coin, then E again to reclaim it.",
                id=="lamp-archive"?"Under the warm light: REED 2, SPILL 1. Reclaim the reed pair before lending it to EXIT. The figures were written for wet hands and tired eyes.":"Beneath the Haven lamp: both return boxes are safe. The low road is shortest; the high road remembers the waiting ducks. Check the address, then carry all three coins to the bell.",prerequisite);
            Ref(controller,"definition",data);mechanics.Add(controller);
            Art("Lamp PLACEHOLDER",controller.transform,position+Vector2.up*.35f,new Vector2(.65f,.98f),lamp,Color.white);
            var glow=Art("Inspection glow",controller.transform,position+Vector2.up*.75f,new Vector2(.18f,.2f),square,new Color(.45f,.52f,.54f),60);Ref(controller,"glow",glow);Label(controller.transform,position+Vector2.up*1.15f,"INSPECT / 1 COIN");
            return controller;
        }
        private static MailRouting Route(string id,Vector2 source,Vector2 a,Vector2 b,DuckPerch optionalPerch,string prerequisite)
        {
            var controller=Interaction<MailRouting>(id,source);
            var data=Definition(id,id=="route-storm"?"Storm return mail":"Haven return mail",MarshMechanicKind.MailRouting,
                "Send two coins at the "+(id=="route-storm"?"Storm":"Haven")+" sorting box. E there reroutes the same loan. Claim it at the addressed LOW or HIGH box before paying the main bell.",
                "Ada never trusted a single return route. The sorter held the same two coins for either road; no one had to pay twice to change their mind.",prerequisite);
            Ref(controller,"definition",data);mechanics.Add(controller);
            Art("Sorting box PLACEHOLDER",controller.transform,source+Vector2.up*.35f,new Vector2(.7f,1.05f),mailbox,Color.white);Label(controller.transform,source+Vector2.up*1.15f,"SORT / 2 COINS");
            var boxes=new MailReturnBox[2];for(int i=0;i<2;i++)
            {
                var box=Interaction<MailReturnBox>(id+(i==0?"-low":"-high"),i==0?a:b);boxes[i]=box;Ref(box,"routing",controller);Text(box,"displayName",(id=="route-storm"?"Storm ":"Haven ")+(i==0?"LOW":"HIGH"));
                if(i==1&&optionalPerch!=null)Ref(box,"requiredInspection",optionalPerch);
                Art("Posted return PLACEHOLDER",box.transform,box.transform.position+Vector3.up*.35f,new Vector2(.7f,1.05f),mailbox,Color.white);Label(box.transform,box.transform.position+(id=="route-haven"&&i==0?Vector3.down*.85f:Vector3.up*1.15f),i==0?"LOW RETURN":"HIGH RETURN");
            }
            MarshCampaignBuilder.Refs(controller,"destinations",boxes);return controller;
        }
        public static void Build()
        {
            if(EditorApplication.isPlaying||EditorApplication.isCompiling||SceneManager.GetActiveScene().path!=MarshCampaignBuilder.ScenePath)throw new InvalidOperationException("Open saved campaign in Edit Mode after compilation.");
            var levels=Object.FindObjectsByType<MarshLevel>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            if(levels.Length!=1||levels[0].Campaign==null)throw new InvalidOperationException("Expected existing campaign coordinator.");level=levels[0];
            if(Object.FindObjectsByType<MarshMechanic>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=0||Object.FindObjectsByType<ReedMark>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=0)throw new InvalidOperationException("M4 already built; refuse duplicate mechanics.");
            // Existing managers, UI and singletons are reused; only world objects and data are added.
            Directory.CreateDirectory(Folder);Directory.CreateDirectory("Assets/_Agent/Data/Campaign/Marks");AssetDatabase.Refresh();
            square=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png");letter=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Props/FieldLetter.png");mailbox=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Props/KeeperMailbox.png");lamp=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Props/MarshLantern.png");duckSprite=AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Companions/duck 1.png").OfType<Sprite>().OrderBy(s=>s.rect.y).ThenBy(s=>s.rect.x).First();material=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");
            if(square==null||letter==null||mailbox==null||lamp==null||duckSprite==null||material==null||font==null)throw new InvalidOperationException("Missing inspected sprite/font assets.");
            var world=SceneManager.GetActiveScene().GetRootGameObjects().Single(g=>g.name=="Marshlight_World");
            if(world.transform.Find("09_CampaignMechanics")!=null)throw new InvalidOperationException("Duplicate mechanics hierarchy.");
            var root=new GameObject("09_CampaignMechanics");root.transform.SetParent(world.transform,false);group=root.transform;
            interactions.Clear();mechanics.Clear();marks.Clear();
            var reeds=Perch("perch-reeds",28.5f);var storm=Perch("perch-storm",98.5f);var haven=Perch("perch-haven",124.5f);
            level.Nodes.Single(n=>n.Id=="reed-note").transform.position=new Vector2(32,0);
            Lamp("lamp-archive",new Vector2(55,2),"");Lamp("lamp-haven",new Vector2(128,3.5f),"perch-haven");
            Route("route-storm",new Vector2(96,-2),new Vector2(100,-2),new Vector2(102.8f,3),storm,"");
            Route("route-haven",new Vector2(138,0),new Vector2(142,0),new Vector2(143,3.5f),null,"lamp-haven");
            var markPositions=new[]{new Vector2(11,3.5f),new Vector2(33.5f,4),new Vector2(41,4.5f),new Vector2(58,4.5f),new Vector2(72.5f,6.1f),new Vector2(102,3.5f),new Vector2(111,-4.5f),new Vector2(144,4.5f)};
            var titles=new[]{"First feather","A patient perch","The listening reeds","Ink under glass","The dry ledger","Two roads home","A sheltering wing","Lanterns for everyone"};
            var memories=new[]{"Aiken finds a carved feather beside the trail. Someone marked a path for feet smaller than his.","A notch in the rail is polished by generations of duck feet. Waiting here has always been part of the work.","Duckduckgoose pauses to listen. The reeds carry several answers, never just his own.","A tiny feather was pressed into the archive seal. The keeper filed it with the important things.","The ledger counts supplies and safe crossings. Beside each total is a small, careful drawing of a duck.","Ada's two arrows reach the same lantern. A longer road is still a way home.","Beneath the storm rail, a broad feather sheltered a smaller one. Aiken leaves both where they are.","Eight feather marks lead to one circle of light. The marsh has room for the last traveller too."};
            for(int i=0;i<8;i++)
            {
                var data=MarshCampaignBuilder.Asset<CollectibleData>("Assets/_Agent/Data/Campaign/Marks/ReedMark_"+(i+1)+".asset");data.id="reed-mark-"+(i+1);data.title=titles[i];data.memory=memories[i];data.sprite=letter;data.ValidateData();EditorUtility.SetDirty(data);
                var mark=Interaction<ReedMark>(data.id,markPositions[i]);Ref(mark,"definition",data);marks.Add(mark);
                Art("Mark PLACEHOLDER outline",mark.transform,markPositions[i],new Vector2(.48f,.48f),square,Color.black,50);
                var visual=Art("Carved mark PLACEHOLDER",mark.transform,markPositions[i],new Vector2(.38f,.38f),letter,new Color(.89f,.77f,.49f),51);Ref(mark,"visual",visual);Label(mark.transform,markPositions[i]+Vector2.up*.65f,"REED MARK");
            }
            string[][] requirements={new string[0],new[]{"perch-reeds"},new[]{"lamp-archive"},new[]{"route-storm"},new[]{"perch-haven","lamp-haven","route-haven"}};
            for(int i=0;i<5;i++){var section=level.Campaign.Data.sections[i];var data=MarshCampaignBuilder.Asset<GateData>(Folder+"Gate_"+i+".asset");data.id=level.Campaign.MainGates[i].Id;data.cost=3;data.requiredMechanics=requirements[i];data.ValidateData();EditorUtility.SetDirty(data);section.mainGate=data;EditorUtility.SetDirty(section);}
            var rewards=MarshCampaignBuilder.Asset<JournalRewardData>(Folder+"JournalRewards.asset");rewards.badgeThreshold=4;rewards.codaThreshold=8;rewards.coda="Aiken sets the eight feather marks beside the lantern. Duckduckgoose nudges the last one into the circle. There was room for it all along.";rewards.ValidateData(8);EditorUtility.SetDirty(rewards);Ref(level.Campaign,"journalRewards",rewards);
            MarshCampaignBuilder.Refs(level.Campaign,"mechanics",mechanics);MarshCampaignBuilder.Refs(level.Campaign,"marks",marks);
            var cap=new SerializedObject(level.Campaign);var caps=cap.FindProperty("capabilities");caps.arraySize=3;for(int i=0;i<3;i++)caps.GetArrayElementAtIndex(i).stringValue=new[]{"perch","lantern","routing"}[i];cap.ApplyModifiedPropertiesWithoutUndo();
            var story=level.Campaign.Data.story;story.Get("perch-intro").pages[0]="Duckduckgoose studies the reed perch as if he has seen it before. Aiken: 'Will you hold the lever while I inspect the latch?' E at the perch calls him; E at INSPECT inside latches the shutter. Call or recall is always free.";
            story.Get("lantern-intro").pages[0]="Ada's note: 'One coin warms the inspection lamp. Read the scratched figures, then take the coin back with E. Light helps us notice; it is not another toll.'";
            story.Get("flood").pages[0]="Ada's flood note: 'Four ducklings are safe in the Haven. The youngest has a hurt wing. Duckduckgoose went west when the paths failed. Keep every borrowed coin moving; the flock is waiting for a way to bring everyone home.'";
            var added=new MarshStory.Entry{id="mail-route",speaker="Ada",oncePerRun=true,requiredCapability="routing",pages=new[]{"The sorting box borrows two coins. Its LOW return is open; the HIGH return lies in the perch room. E at the sorter changes the address without borrowing again. Claim the same two coins at your chosen box before the next bell."}};
            var storySo=new SerializedObject(story);var entries=storySo.FindProperty("entries");entries.arraySize=story.Entries.Length+1;var entry=entries.GetArrayElementAtIndex(entries.arraySize-1);entry.FindPropertyRelative("id").stringValue=added.id;entry.FindPropertyRelative("speaker").stringValue=added.speaker;entry.FindPropertyRelative("oncePerRun").boolValue=true;entry.FindPropertyRelative("requiredCapability").stringValue=added.requiredCapability;entry.FindPropertyRelative("prerequisites").arraySize=0;entry.FindPropertyRelative("pages").arraySize=1;entry.FindPropertyRelative("pages").GetArrayElementAtIndex(0).stringValue=added.pages[0];storySo.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(story);
            var tutorial=Interaction<MarshStoryTrigger>("mail-route-story",new Vector2(94,0));Text(tutorial,"storyId","mail-route");var tutorialSo=new SerializedObject(tutorial);tutorialSo.FindProperty("automatic").boolValue=true;tutorialSo.ApplyModifiedPropertiesWithoutUndo();tutorial.GetComponent<BoxCollider2D>().size=new Vector2(1.6f,1.6f);Art("Mail instructions",tutorial.transform,tutorial.transform.position,new Vector2(.5f,.5f),letter,Color.white);
            level.Campaign.Data.sections[3].storyIds=level.Campaign.Data.sections[3].storyIds.Concat(new[]{"mail-route"}).ToArray();EditorUtility.SetDirty(level.Campaign.Data.sections[3]);
            MarshCampaignBuilder.Refs(level,"interactions",level.Interactions.Concat(interactions));
            // Widen the existing ledger within its HUD area; reuse the menu for the journal.
            var ledger=(UnityEngine.UI.Text)new SerializedObject(level).FindProperty("ledgerText").objectReferenceValue;ledger.fontSize=14;ledger.rectTransform.sizeDelta=new Vector2(460,40);ledger.rectTransform.anchoredPosition=new Vector2(-250,-26);ledger.alignment=TextAnchor.MiddleRight;
            level.Campaign.ValidateReferences();story.ValidateData();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("M4 saved: perches, refundable lamps, addressed mail,8 Reed Marks and paid gate requirements.");
        }
    }
}
#endif

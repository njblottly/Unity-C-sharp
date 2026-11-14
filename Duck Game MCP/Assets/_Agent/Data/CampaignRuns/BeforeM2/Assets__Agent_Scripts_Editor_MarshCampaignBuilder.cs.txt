#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace DuckAdventure.Agent.Editor
{
    public static class MarshCampaignBuilder
    {
        public const string ScenePath = "Assets/_Agent/Scenes/Marshlight_Campaign.unity";
        private const string Data = "Assets/_Agent/Data/Campaign/";
        private static MarshLevel level;
        private static Sprite square;
        private static Material material;
        private static Font font;
        private static readonly List<MarshNode> nodes = new List<MarshNode>();
        private static readonly List<MarshInteraction> interactions = new List<MarshInteraction>();
        private static T One<T>() where T:Component
        {
            T[] all = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(c => c.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (all.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + ", found " + all.Length);
            return all[0];
        }
        public static void Ref(Object obj, string key, Object value) { var so = new SerializedObject(obj); so.FindProperty(key).objectReferenceValue = value; so.ApplyModifiedPropertiesWithoutUndo(); }
        public static void Text(Object obj, string key, string value) { var so = new SerializedObject(obj); so.FindProperty(key).stringValue = value; so.ApplyModifiedPropertiesWithoutUndo(); }
        public static void Num(Object obj, string key, int value) { var so = new SerializedObject(obj); so.FindProperty(key).intValue = value; so.ApplyModifiedPropertiesWithoutUndo(); }
        public static void Vec(Object obj, string key, Vector2 value) { var so = new SerializedObject(obj); so.FindProperty(key).vector2Value = value; so.ApplyModifiedPropertiesWithoutUndo(); }
        public static void Refs(Object obj, string key, IEnumerable<Object> refs) { var list = refs.ToArray(); var so = new SerializedObject(obj); var array = so.FindProperty(key); array.arraySize = list.Length; for (int i=0;i<list.Length;i++) array.GetArrayElementAtIndex(i).objectReferenceValue=list[i]; so.ApplyModifiedPropertiesWithoutUndo(); }
        public static T Asset<T>(string path) where T:ScriptableObject { T asset=AssetDatabase.LoadAssetAtPath<T>(path); if(asset==null){asset=ScriptableObject.CreateInstance<T>();AssetDatabase.CreateAsset(asset,path);} return asset; }
        private static Transform Group(string name, Transform parent) { var g=new GameObject(name);g.transform.SetParent(parent,false);return g.transform; }
        public static SpriteRenderer Shape(string name, Vector2 position, Vector2 size, Sprite sprite, Color color, int order, Transform parent, bool solid=false)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=position;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=material;sr.color=color;sr.sortingOrder=order;
            go.transform.localScale=new Vector3(size.x/(sprite.rect.width/sprite.pixelsPerUnit),size.y/(sprite.rect.height/sprite.pixelsPerUnit),1);
            if(solid){go.layer=LayerMask.NameToLayer("Blocking");var box=go.AddComponent<BoxCollider2D>();box.size=new Vector2(sprite.rect.width/sprite.pixelsPerUnit,sprite.rect.height/sprite.pixelsPerUnit);}
            return sr;
        }
        private static void Label(string value,Vector2 position,Transform parent)
        {
            var go=new GameObject("Plate "+value);go.transform.SetParent(parent,false);go.transform.position=position;
            var text=go.AddComponent<TextMesh>();text.font=font;text.text=value;text.fontSize=32;text.characterSize=.065f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.color=new Color(.96f,.89f,.69f);
            var mesh=go.GetComponent<MeshRenderer>();mesh.sharedMaterial=font.material;mesh.sortingOrder=80;
        }
        private static MarshNode CloneNode(MarshNode template,string id,MarshNodeKind kind,Vector2 position,Transform parent,string story="",int sequence=0)
        {
            var go=Object.Instantiate(template.gameObject,parent);go.name=id;go.transform.position=position;
            var node=go.GetComponent<MarshNode>();Ref(node,"level",level);Text(node,"nodeId",id);Text(node,"storyId",story);Num(node,"kind",(int)kind);Num(node,"sequence",sequence);nodes.Add(node);return node;
        }
        private static MarshStoryTrigger Trigger(string id,string story,Vector2 position,Transform parent)
        {
            var go=new GameObject(id);go.transform.SetParent(parent,false);go.transform.position=position;
            var box=go.AddComponent<BoxCollider2D>();box.isTrigger=true;box.size=new Vector2(1.6f,1.6f);
            var trigger=go.AddComponent<MarshStoryTrigger>();Ref(trigger,"level",level);Text(trigger,"interactionId",id);Text(trigger,"storyId",story);
            var so=new SerializedObject(trigger);so.FindProperty("automatic").boolValue=true;so.ApplyModifiedPropertiesWithoutUndo();interactions.Add(trigger);
            Shape("Trail message",position,new Vector2(.6f,.8f),AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Props/TrailSignpost.png"),Color.white,40,go.transform);return trigger;
        }
        [Serializable] private sealed class StoryDocument { public MarshStory.Entry[] entries; }
        private static MarshStory BuildStory()
        {
            var original=AssetDatabase.LoadAssetAtPath<MarshStory>("Assets/_Agent/Data/MarshStory.asset");
            var story=Asset<MarshStory>(Data+"CampaignStory.asset");
            var entries=original.Entries.Select(e=>new MarshStory.Entry {id=e.id,speaker=e.speaker,pages=e.pages.Select(p=>p.Replace("three sluices","five sluices").Replace("Three coins, three bells","Three coins, five bells").Replace("The second bell finds the first bell's note","The archive bell joins the earlier notes").Replace("ready for one more crossing","ready for the locks ahead")).ToArray(),oncePerRun=new[]{"intro","sword","bell1","bell2","bell3","ending","exchange"}.Contains(e.id)}).ToList();
            entries.Add(new MarshStory.Entry{id="perch-intro",speaker="Aiken",oncePerRun=true,requiredCapability="perch",pages=new[]{"Duckduckgoose settles on the reed perch as if he has done it before. E asks him to wait or follow. The little shutter follows his weight; the main bell still needs its three coins. You know this place, don't you?"}});
            entries.Add(new MarshStory.Entry{id="reedbank-bell",speaker="The listening reeds",oncePerRun=true,pages=new[]{"A second bell answers the first. Duckduckgoose listens beyond the chorus, waiting for one more voice. Aiken checks the return box. The same three coins are waiting."}});
            entries.Add(new MarshStory.Entry{id="lantern-intro",speaker="Ada's inspection note",oncePerRun=true,requiredCapability="lantern",pages=new[]{"Lend one coin to this inspection lamp with E. Its light reveals the repair glyph; the journal remembers it after you extinguish the lamp and reclaim the coin. Read first, refund, then carry all three onward."}});
            entries.Add(new MarshStory.Entry{id="sluice-warning",speaker="Ada's repair note",oncePerRun=true,prerequisites=new[]{"bell2"},pages=new[]{"The old sluice tender is jammed farther along the route. A lowered head means a charge; a raised arm means a sweep. Step clear of the warning and work during its recovery. The dry lantern is a safe place to prepare."}});
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new StoryDocument{entries=entries.ToArray()}),story);EditorUtility.SetDirty(story);return story;
        }
        [MenuItem("Tools/Duck Adventure/Campaign/Build Milestone 1")]
        public static void Build()
        {
            if(EditorApplication.isPlaying||EditorApplication.isCompiling)throw new InvalidOperationException("Build only in Edit Mode after compilation.");
            if(File.Exists(ScenePath))throw new InvalidOperationException("Campaign scene already exists; extend the existing scene instead of rebuilding it.");
            Directory.CreateDirectory(Data);AssetDatabase.Refresh();
            if(!AssetDatabase.CopyAsset("Assets/_Agent/Scenes/Marshlight.unity",ScenePath))throw new InvalidOperationException("Could not copy the baseline scene.");
            EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            level=One<MarshLevel>();One<Player>();One<DuckFollower>();One<RPGGameManager>();One<RPGCameraManager>();One<Inventory>();One<HealthBar>();One<Weapon>();One<MarshEquipmentHUD>();One<MarshCoinRelay>();
            if(Object.FindObjectsByType<MarshCampaign>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=0)throw new InvalidOperationException("Duplicate campaign configuration.");
            square=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png");material=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");
            if(square==null||material==null||font==null)throw new InvalidOperationException("Existing art/font references missing.");
            var world=SceneManager.GetActiveScene().GetRootGameObjects().Single(g=>g.name=="Marshlight_World").transform;
            var templates=Group("Campaign build templates",null);
            string[] ids={"bell-0","tray-0","keeper-note","reed-note","checkpoint-0","haven"};var templateNodes=new Dictionary<string,MarshNode>();
            foreach(string id in ids)templateNodes[id]=Object.Instantiate(level.Nodes.Single(n=>n.Id==id).gameObject,templates).GetComponent<MarshNode>();
            var relayTemplate=Object.Instantiate(world.Find("09_ArchiveExchange").gameObject,templates);
            var enemyTemplate=Object.Instantiate(Object.FindObjectsByType<EnemyHitPoints>(FindObjectsSortMode.None).First().gameObject,templates);
            var pickups=Object.FindObjectsByType<Consumable>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.transform.IsChildOf(world)).ToArray();
            var coinTemplate=Object.Instantiate(pickups.First(c=>c.item.itemType==Item.ItemType.COIN).gameObject,templates);
            var heartTemplate=Object.Instantiate(pickups.First(c=>c.item.itemType==Item.ItemType.HEALTH).gameObject,templates);
            var swordTemplate=Object.Instantiate(pickups.Single(c=>c.item.itemType==Item.ItemType.SWORD).gameObject,templates);
            foreach(Transform child in world.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);
            nodes.Clear();interactions.Clear();
            var terrain=Group("01_Terrain",world);var banks=Group("02_SolidBanks",world);var decor=Group("03_OutdoorDetails",world);var landmarks=Group("04_ChapterCrossings",world);var items=Group("05_Pickups",world);var storyGroup=Group("06_Story",world);var enemyGroup=Group("07_Encounters",world);
            var grass=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Sprites/OutdoorReuse/GrassFill.asset");var water=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Sprites/OutdoorReuse/WaterFill.asset");var dirt=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Sprites/OutdoorReuse/DirtFill.asset");
            var sea=Shape("Marsh water",new Vector2(80,0),new Vector2(190,24),water,Color.white,-30,terrain);sea.drawMode=SpriteDrawMode.Tiled;sea.transform.localScale=Vector3.one;sea.size=new Vector2(190,24);
            var land=Shape("Raised marsh",new Vector2(80,0),new Vector2(174,14),grass,Color.white,-20,terrain);land.drawMode=SpriteDrawMode.Tiled;land.transform.localScale=Vector3.one;land.size=new Vector2(174,14);
            var trail=Shape("Keeper trail",new Vector2(80,0),new Vector2(174,1.1f),dirt,new Color(.83f,.85f,.75f),-10,terrain);trail.drawMode=SpriteDrawMode.Tiled;trail.transform.localScale=Vector3.one;trail.size=new Vector2(174,1.1f);
            Shape("North bank",new Vector2(80,7.4f),new Vector2(175,.8f),square,new Color(.16f,.28f,.20f),35,banks,true);
            Shape("South bank",new Vector2(80,-7.4f),new Vector2(175,.8f),square,new Color(.16f,.28f,.20f),35,banks,true);
            Shape("West bank",new Vector2(-4.4f,0),new Vector2(.8f,15.6f),square,new Color(.16f,.28f,.20f),35,banks,true);
            Shape("East bank",new Vector2(166.4f,0),new Vector2(.8f,15.6f),square,new Color(.16f,.28f,.20f),35,banks,true);
            for(int i=0;i<38;i++)
            {
                float x=i*4.3f-1;var tree=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Sprites/OutdoorReuse/MarshTree"+(i%2==0?"A":"B")+".asset");
                Shape("Bank tree",new Vector2(x,i%2==0?5.1f:-5.3f),new Vector2(2.1f,2.7f),tree,Color.white,25,decor);
            }
            float[] channels={18,46,82,116,150};float[] starts={0,22,50,86,120,154};
            string[] gateIds={"bell-0","campaign-bell-reed","bell-1","campaign-bell-storm","bell-2"};string[] trayIds={"tray-0","campaign-tray-reed","tray-1","campaign-tray-storm","tray-2"};
            string[] titles={"Keeper's Landing","Listening Reeds","The Old Archive","Storm Locks","Lantern Haven"};string[] storyIds={"bell1","reedbank-bell","bell2","promise","bell3"};string[] gateNames={"Landing Bell","Reedbank Bell","Archive Bell","Storm Bell","Haven Bell"};
            var campaign=level.gameObject.AddComponent<MarshCampaign>();var data=Asset<CampaignData>(Data+"MarshlightCampaign.asset");data.chapters=new ChapterData[5];data.sections=new SectionData[5];data.story=BuildStory();data.archiveGateIndex=2;
            var gates=new MarshNode[5];var spawns=new SpawnPoint[6];var originalSpawn=One<RPGGameManager>().PlayerSpawnPoint;
            var previousSpawns=Object.FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(s=>s!=originalSpawn).ToArray();foreach(var spawn in previousSpawns)Object.DestroyImmediate(spawn.gameObject);
            for(int i=0;i<6;i++)
            {
                spawns[i]=i==0?originalSpawn:Group("Campaign Spawn "+i,level.transform).gameObject.AddComponent<SpawnPoint>();spawns[i].transform.position=new Vector3(starts[i],0,0);
                var cp=CloneNode(templateNodes["checkpoint-0"],"checkpoint-"+i,MarshNodeKind.Checkpoint,new Vector2(starts[i],0),landmarks,"",i);Ref(cp,"spawnPoint",spawns[i]);Text(cp,"displayName",i<5?titles[i]:"Haven reunion");
            }
            string[][] beats={new[]{"intro","letter","sword","bell1"},new[]{"perch-intro","ledger","reedbank-bell"},new[]{"flood","lantern-intro","exchange","archive-pocket","bell2","sluice-warning"},new[]{"promise"},new[]{"bell3","ending"}};
            for(int i=0;i<5;i++)
            {
                var chapter=Asset<ChapterData>(Data+"Chapter"+(i+1)+".asset");chapter.id="chapter-"+(i+1);chapter.displayName=titles[i];chapter.act=i==0?1:i<3?2:3;chapter.theme=new[]{"Warm brass and shelter","Listening reeds and partnership","Water-stained records","Exposed links and choices","Lantern refuge"}[i];chapter.targetMinMinutes=new[]{4,5,5,5,6}[i];chapter.targetMaxMinutes=new[]{5,6,7,6,8}[i];
                var section=Asset<SectionData>(Data+"Section"+(i+1)+".asset");section.id="section-"+(i+1);section.chapter=chapter;section.start=new Vector2(starts[i],0);section.end=new Vector2(starts[i+1],0);section.storyIds=beats[i];section.mechanicPermissions=i==0?new string[0]:i==1?new[]{"perch"}:i==2?new[]{"lantern","relay"}:new[]{"perch","lantern","routing"};section.objective="Follow the keeper trail. Lend the same three coins to the "+gateNames[i]+".";data.chapters[i]=chapter;data.sections[i]=section;EditorUtility.SetDirty(chapter);EditorUtility.SetDirty(section);
                float x=channels[i];Shape("Water channel "+i,new Vector2(x,0),new Vector2(2.6f,14),water,Color.white,-9,terrain);
                Shape("Water bank north "+i,new Vector2(x,4.25f),new Vector2(2.6f,5.5f),square,new Color(0,0,0,0),0,banks,true);Shape("Water bank south "+i,new Vector2(x,-4.25f),new Vector2(2.6f,5.5f),square,new Color(0,0,0,0),0,banks,true);
                var bridge=Shape("Raised causeway "+i,new Vector2(x,0),new Vector2(3.3f,2.8f),square,new Color(.22f,.34f,.32f),5,landmarks);
                var barrier=Shape("Main gate "+i,new Vector2(x,0),new Vector2(.45f,3),square,new Color(.68f,.35f,.25f),32,landmarks,true);
                gates[i]=CloneNode(templateNodes["bell-0"],gateIds[i],MarshNodeKind.Bell,new Vector2(x-1.4f,0),landmarks,storyIds[i],i);Ref(gates[i],"closedGate",barrier.gameObject);Ref(gates[i],"bridge",bridge);Text(gates[i],"displayName",gateNames[i]);
                var tray=CloneNode(templateNodes["tray-0"],trayIds[i],MarshNodeKind.ReturnTray,new Vector2(x+2.4f,-.35f),landmarks);Ref(tray,"pairedBell",gates[i]);Text(tray,"displayName",gateNames[i]+" return box");
                Label(gateNames[i].ToUpperInvariant(),new Vector2(x-2,2),landmarks);
            }
            EditorUtility.SetDirty(data);Ref(campaign,"level",level);Ref(campaign,"data",data);Refs(campaign,"mainGates",gates);Ref(level,"campaign",campaign);Ref(level,"story",data.story);Refs(level,"checkpoints",spawns);
            var levelSo=new SerializedObject(level);var names=levelSo.FindProperty("checkpointNames");names.arraySize=6;for(int i=0;i<6;i++)names.GetArrayElementAtIndex(i).stringValue=i<5?titles[i]:"Haven reunion";levelSo.FindProperty("havenPosition").vector2Value=new Vector2(162,0);levelSo.ApplyModifiedPropertiesWithoutUndo();
            var coins=new Consumable[3];Vector2[] coinPositions={new Vector2(2,0),new Vector2(5,2),new Vector2(8,-2)};
            for(int i=0;i<3;i++){var go=Object.Instantiate(coinTemplate,items);go.name="Campaign brass coin "+i;go.transform.position=coinPositions[i];coins[i]=go.GetComponent<Consumable>();}Refs(level,"coinPickups",coins);
            var sword=Object.Instantiate(swordTemplate,items);sword.name="ReedkeeperSwordPickup";sword.transform.position=new Vector2(1.5f,1.5f);
            for(int i=0;i<5;i++){var heart=Object.Instantiate(heartTemplate,items);heart.name="Chapter heart "+(i+1);heart.transform.position=new Vector2(starts[i]+4,-1.5f);}
            CloneNode(templateNodes["keeper-note"],"keeper-note",MarshNodeKind.Note,new Vector2(0,1.4f),storyGroup,"letter");
            CloneNode(templateNodes["reed-note"],"reed-note",MarshNodeKind.Note,new Vector2(32,2),storyGroup,"ledger");
            var flood=CloneNode(templateNodes["reed-note"],"archive-note",MarshNodeKind.Note,new Vector2(54,-2),storyGroup,"flood");Text(flood,"displayName","Ada's flood note");
            var promise=CloneNode(templateNodes["reed-note"],"storm-note",MarshNodeKind.Note,new Vector2(110,2),storyGroup,"promise");Text(promise,"displayName","the keeper's promise");
            Trigger("perch-story","perch-intro",new Vector2(27,2),storyGroup);Trigger("lamp-story","lantern-intro",new Vector2(55,2),storyGroup);Trigger("sluice-story","sluice-warning",new Vector2(84,2),storyGroup);
            var relay=Object.Instantiate(relayTemplate,world);relay.name="08_ArchiveExchange";relay.transform.position+=Vector3.right*14;
            foreach(var node in relay.GetComponentsInChildren<MarshNode>(true)){Ref(node,"level",level);nodes.Add(node);}
            Shape("Archive north approach bank",new Vector2(67,-2.5f),new Vector2(.8f,11),square,new Color(.15f,.28f,.20f),30,banks,true);
            Shape("Archive south return bank",new Vector2(74,2.5f),new Vector2(.8f,11),square,new Color(.15f,.28f,.20f),30,banks,true);
            var relaySo=new SerializedObject(level.CoinRelay);var loans=relaySo.FindProperty("loans");string[] loanIds={"reed","spill","exit"};
            for(int i=0;i<3;i++){var loan=loans.GetArrayElementAtIndex(i);var cup=nodes.Single(n=>n.Id=="relay-"+loanIds[i]+"-cup");loan.FindPropertyRelative("cup").objectReferenceValue=cup;loan.FindPropertyRelative("tray").objectReferenceValue=nodes.Single(n=>n.Id=="relay-"+loanIds[i]+"-tray");loan.FindPropertyRelative("gate").objectReferenceValue=relay.GetComponentsInChildren<Transform>(true).Single(t=>t.name=="Latched "+loanIds[i]+" arm").gameObject;loan.FindPropertyRelative("indicator").objectReferenceValue=cup.GetComponentsInChildren<SpriteRenderer>(true).Single(s=>s.name=="Counterweight cup");}relaySo.ApplyModifiedPropertiesWithoutUndo();
            Vector2[] centers={new Vector2(38,-2.5f),new Vector2(58,-3),new Vector2(100,3),new Vector2(138,-3),new Vector2(157,3)};
            for(int i=0;i<centers.Length;i++){var go=Object.Instantiate(enemyTemplate,enemyGroup);go.name="CampaignMite_"+i;go.transform.position=centers[i];var patrol=go.GetComponent<MarshPatrol>();Vec(patrol,"pointA",centers[i]+Vector2.left);Vec(patrol,"pointB",centers[i]+Vector2.right);Ref(go.GetComponent<MarshEnemyFeedback>(),"level",level);}
            var haven=CloneNode(templateNodes["haven"],"haven",MarshNodeKind.Haven,new Vector2(162,0),landmarks);
            var duckSprite=One<DuckFollower>().GetComponent<SpriteRenderer>().sprite;for(int i=0;i<4;i++)Shape("Haven duckling "+i,new Vector2(162+(i%2)*.9f,1.7f+(i/2)*.8f),new Vector2(.48f,.48f),duckSprite,Color.white,41,haven.transform);
            Refs(level,"nodes",nodes);Refs(level,"interactions",interactions);
            var cameraBounds=Object.FindObjectsByType<PolygonCollider2D>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(c=>c.name=="CameraBounds");cameraBounds.transform.position=Vector3.zero;cameraBounds.SetPath(0,new[]{new Vector2(-8,-13),new Vector2(-8,13),new Vector2(172,13),new Vector2(172,-13)});
            level.Player.transform.position=Vector3.zero;level.Player.GetComponent<Rigidbody2D>().position=Vector2.zero;level.Duck.transform.position=Vector3.down*.7f;
            One<RPGGameManager>().SetPlayerSpawnPoint(spawns[0]);Object.DestroyImmediate(templates.gameObject);
            var build=EditorBuildSettings.scenes.Where(s=>s.path!=ScenePath).ToList();build.Insert(0,new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=build.ToArray();
            foreach(var component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include,FindObjectsSortMode.None))if(PrefabUtility.IsPartOfPrefabInstance(component))PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            campaign.ValidateReferences();EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("Campaign M1 saved: five chapters/bells, six checkpoints, Act I/II story bindings; baseline scene preserved.");
        }
    }
}
#endif

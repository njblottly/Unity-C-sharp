#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;
namespace DuckAdventure.Agent.Editor
{
    public static class MarshEnemyBuilder
    {
        private const string Folder="Assets/_Agent/Placeholder/Enemies/";
        private const string DataFolder="Assets/_Agent/Data/Campaign/Enemies/";
        private static Sprite square,bodySprite;
        private static Material plain,flash;
        private static Font font;
        private static void Ref(Object o,string k,Object v)=>MarshCampaignBuilder.Ref(o,k,v);
        private static void Num(Object o,string k,int v)=>MarshCampaignBuilder.Num(o,k,v);
        private static SpriteRenderer Part(string name,Transform parent,Vector2 p,Vector2 size,Color color,Material material)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=new Vector3(size.x,size.y,1);
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=square;sr.sharedMaterial=material;sr.color=color;sr.sortingOrder=name=="Body"?102:(name=="Pale wing"||name=="Amber seed pod"||name=="Brass seed"?101:100);return sr;
        }
        private static TextMesh Label(string name,Transform parent,Vector2 p,string text,float size)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=p;var label=go.AddComponent<TextMesh>();label.font=font;label.fontSize=32;label.characterSize=size;label.text=text;label.anchor=TextAnchor.MiddleCenter;label.alignment=TextAlignment.Center;label.color=new Color(.89f,.78f,.49f);var mesh=go.GetComponent<MeshRenderer>();mesh.sharedMaterial=font.material;mesh.sortingOrder=110;return label;
        }
        private static GameObject Prefab(MarshEnemyKind kind)
        {
            var go=new GameObject(kind+" PLACEHOLDER");go.tag="Enemy";go.layer=0;
            var body=go.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;body.gravityScale=0;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.interpolation=RigidbodyInterpolation2D.Interpolate;
            var collider=go.AddComponent<CircleCollider2D>();collider.radius=.32f;collider.isTrigger=true;
            var hp=go.AddComponent<EnemyHitPoints>();var healthSo=new SerializedObject(hp);healthSo.FindProperty("destroyOnDeath").boolValue=false;healthSo.FindProperty("disableInsteadOfDestroy").boolValue=true;healthSo.ApplyModifiedPropertiesWithoutUndo();
            var dimensions=kind==MarshEnemyKind.Skitter?new Vector2(.85f,.55f):kind==MarshEnemyKind.Crab?new Vector2(1.05f,.65f):kind==MarshEnemyKind.Moth?new Vector2(.5f,.55f):new Vector2(.55f,.85f);
            var visual=Part("Body",go.transform,Vector2.zero,dimensions,Color.white,flash);visual.sprite=bodySprite;
            Transform leftWing=null,rightWing=null;
            if(kind==MarshEnemyKind.Moth)
            {
                for(int side=-1;side<=1;side+=2)
                {
                    var wing=Part(side<0?"Left wing":"Right wing",go.transform,new Vector2(side*.4f,.1f),new Vector2(.78f,.48f),Color.black,plain);
                    Part("Pale wing",wing.transform,Vector2.zero,new Vector2(.83f,.75f),new Color(.77f,.77f,.77f),plain);
                    if(side<0)leftWing=wing.transform;else rightWing=wing.transform;
                }
            }
            if(kind==MarshEnemyKind.Sprout)
            {
                Part("Pod outline",go.transform,new Vector2(0,.52f),new Vector2(.5f,.34f),Color.black,plain);
                Part("Amber seed pod",go.transform,new Vector2(0,.52f),new Vector2(.38f,.22f),new Color(.89f,.77f,.49f),plain);
            }
            for(int i=0;i<(kind==MarshEnemyKind.Moth?0:4);i++)
            {
                float x=(i<2?-1:1)*(kind==MarshEnemyKind.Crab?.48f:.32f);float y=(i%2==0?-1:1)*.17f;
                Part("Outlined limb",go.transform,new Vector2(x,y),kind==MarshEnemyKind.Crab?new Vector2(.26f,.20f):new Vector2(.22f,.08f),Color.black,plain);
                Part("Limb fill",go.transform,new Vector2(x,y),kind==MarshEnemyKind.Crab?new Vector2(.20f,.14f):new Vector2(.16f,.035f),new Color(.42f,.25f,.20f),plain);
            }
            var feedback=go.AddComponent<MarshEnemyFeedback>();Ref(feedback,"health",hp);Ref(feedback,"bodySprite",visual);var so=new SerializedObject(feedback);so.FindProperty("scalablePips").boolValue=true;so.ApplyModifiedPropertiesWithoutUndo();
            var pips=new SpriteRenderer[8];for(int i=0;i<pips.Length;i++)pips[i]=Part("HP pip "+i,go.transform,new Vector2((i-3.5f)*.13f,kind==MarshEnemyKind.Sprout?.9f:.58f),new Vector2(.09f,.055f),new Color(.97f,.76f,.35f),plain);MarshCampaignBuilder.Refs(feedback,"healthPips",pips);
            var warningObject=new GameObject("Attack warning");warningObject.transform.SetParent(go.transform,false);var warning=warningObject.AddComponent<LineRenderer>();warning.sharedMaterial=plain;warning.useWorldSpace=true;warning.startWidth=.065f;warning.endWidth=.065f;warning.startColor=new Color(1,.8f,.25f);warning.endColor=new Color(1,.8f,.25f);warning.sortingOrder=109;warning.enabled=false;
            var state=Label("State tell",go.transform,new Vector2(0,kind==MarshEnemyKind.Sprout?1.22f:.93f),"",.16f);Label("Enemy name",go.transform,new Vector2(0,-.75f),kind==MarshEnemyKind.Skitter?"REED SKITTER":kind==MarshEnemyKind.Crab?"SILT CRAB":kind==MarshEnemyKind.Moth?"WICK MOTH":"COUNTERWEIGHT\nSPROUT",.10f);
            EnemyAgent agent=kind==MarshEnemyKind.Skitter?(EnemyAgent)go.AddComponent<ReedSkitter>():kind==MarshEnemyKind.Crab?(EnemyAgent)go.AddComponent<SiltCrab>():kind==MarshEnemyKind.Moth?(EnemyAgent)go.AddComponent<WickMoth>():go.AddComponent<CounterweightSprout>();if(kind==MarshEnemyKind.Moth){Ref(agent,"leftWing",leftWing);Ref(agent,"rightWing",rightWing);}Ref(agent,"body",body);Ref(agent,"bodyCollider",collider);Ref(agent,"health",hp);Ref(agent,"feedback",feedback);Ref(agent,"bodySprite",visual);Ref(agent,"warning",warning);Ref(agent,"stateLabel",state);
            string path=Folder+kind+"_PLACEHOLDER.prefab";var prefab=PrefabUtility.SaveAsPrefabAsset(go,path);Object.DestroyImmediate(go);return prefab;
        }
        public static GameObject AdditionalPrefab(MarshEnemyKind kind)
        {
            square=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png");bodySprite=AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Enemies/EnemyIdle_1.png").OfType<Sprite>().OrderBy(s=>s.rect.x).First();plain=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/_Agent/Shaders/MarshHitFlash.shader");
            if(square==null||bodySprite==null||plain==null||font==null||shader==null)throw new InvalidOperationException("Missing existing sprite/font or hit shader.");
            var errors=ShaderUtil.GetShaderMessages(shader).Where(m=>m.severity.ToString()=="Error").ToArray();if(errors.Length>0)throw new InvalidOperationException(string.Join(";",errors.Select(e=>e.message)));
            flash=AssetDatabase.LoadAssetAtPath<Material>(Folder+"MarshEnemyFlash.mat");if(flash==null){flash=new Material(shader);AssetDatabase.CreateAsset(flash,Folder+"MarshEnemyFlash.mat");}
            if(kind!=MarshEnemyKind.Moth&&kind!=MarshEnemyKind.Sprout)throw new InvalidOperationException("Additional type required.");return Prefab(kind);
        }
        public static GameObject SeedPrefab()
        {
            square=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png");bodySprite=AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Enemies/EnemyIdle_1.png").OfType<Sprite>().OrderBy(s=>s.rect.x).First();plain=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/_Agent/Shaders/MarshHitFlash.shader");
            if(square==null||bodySprite==null||plain==null||font==null||shader==null)throw new InvalidOperationException("Missing existing sprite/font or hit shader.");
            var errors=ShaderUtil.GetShaderMessages(shader).Where(m=>m.severity.ToString()=="Error").ToArray();if(errors.Length>0)throw new InvalidOperationException(string.Join(";",errors.Select(e=>e.message)));
            flash=AssetDatabase.LoadAssetAtPath<Material>(Folder+"MarshEnemyFlash.mat");if(flash==null){flash=new Material(shader);AssetDatabase.CreateAsset(flash,Folder+"MarshEnemyFlash.mat");}
            var go=new GameObject("Seed PLACEHOLDER");var rb=go.AddComponent<Rigidbody2D>();rb.bodyType=RigidbodyType2D.Kinematic;rb.gravityScale=0;var col=go.AddComponent<CircleCollider2D>();col.radius=.09f;col.isTrigger=true;var shape=Part("Black seed outline",go.transform,Vector2.zero,new Vector2(.22f,.16f),Color.black,plain);Part("Brass seed",go.transform,Vector2.zero,new Vector2(.15f,.09f),new Color(.89f,.77f,.49f),plain);var seed=go.AddComponent<MarshSeedProjectile>();Ref(seed,"body",rb);Ref(seed,"hitbox",col);Ref(seed,"visual",shape);var result=PrefabUtility.SaveAsPrefabAsset(go,Folder+"Seed_PLACEHOLDER.prefab");Object.DestroyImmediate(go);return result;
        }
        private static EnemyData Definition(string id,MarshEnemyKind kind)
        {
            var data=MarshCampaignBuilder.Asset<EnemyData>(DataFolder+id+".asset");data.id=id;data.kind=kind;data.displayName=kind==MarshEnemyKind.Skitter?"Reed Skitter":"Silt Crab";data.sprite=bodySprite;data.health=kind==MarshEnemyKind.Skitter?2:4;data.moveSpeed=kind==MarshEnemyKind.Skitter?.9f:.55f;data.attackSpeed=kind==MarshEnemyKind.Skitter?1.8f:0;data.telegraphSeconds=kind==MarshEnemyKind.Skitter?.5f:.75f;data.attackSeconds=kind==MarshEnemyKind.Skitter?.35f:.15f;data.recoverySeconds=kind==MarshEnemyKind.Skitter?.85f:1.1f;data.chases=kind==MarshEnemyKind.Skitter;data.ValidateData();EditorUtility.SetDirty(data);return data;
        }
        public static void Spawn(EnemyData definition,SectionData section,SectionData.Encounter encounter,GameObject prefab,Transform group,MarshLevel level)
        {
            var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,group);go.name=encounter.id;go.transform.position=encounter.position;var actor=go.GetComponent<EnemyAgent>();Ref(actor,"level",level);Ref(actor,"player",level.Player);Ref(actor,"section",section);MarshCampaignBuilder.Text(actor,"encounterId",encounter.id);Ref(go.GetComponent<MarshEnemyFeedback>(),"level",level);
            Num(go.GetComponent<EnemyHitPoints>(),"maxHitPoints",definition.health);Num(go.GetComponent<EnemyHitPoints>(),"startingHitPoints",definition.health);
            foreach(var c in go.GetComponentsInChildren<Component>(true))if(PrefabUtility.IsPartOfPrefabInstance(c))PrefabUtility.RecordPrefabInstancePropertyModifications(c);
        }
        [MenuItem("Tools/Duck Adventure/Campaign/Build Milestone 2 Enemies")]
        public static void Build()
        {
            if(EditorApplication.isPlaying||EditorApplication.isCompiling||SceneManager.GetActiveScene().path!=MarshCampaignBuilder.ScenePath)throw new InvalidOperationException("Open the campaign in Edit Mode after compilation.");
            var levels=Object.FindObjectsByType<MarshLevel>(FindObjectsInactive.Include,FindObjectsSortMode.None);if(levels.Length!=1||levels[0].Campaign==null)throw new InvalidOperationException("Expected the existing campaign coordinator.");var level=levels[0];
            if(Object.FindObjectsByType<EnemyAgent>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length>0)throw new InvalidOperationException("M2 enemies already exist; extend them rather than duplicating.");
            Directory.CreateDirectory(Folder);Directory.CreateDirectory(DataFolder);AssetDatabase.Refresh();
            square=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png");bodySprite=AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Enemies/EnemyIdle_1.png").OfType<Sprite>().OrderBy(s=>s.rect.x).First();plain=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/_Agent/Shaders/MarshHitFlash.shader");
            if(square==null||bodySprite==null||plain==null||font==null||shader==null)throw new InvalidOperationException("Missing existing sprite/font or hit shader.");
            var errors=ShaderUtil.GetShaderMessages(shader).Where(m=>m.severity.ToString()=="Error").ToArray();if(errors.Length>0)throw new InvalidOperationException(string.Join(";",errors.Select(e=>e.message)));
            flash=AssetDatabase.LoadAssetAtPath<Material>(Folder+"MarshEnemyFlash.mat");if(flash==null){flash=new Material(shader);AssetDatabase.CreateAsset(flash,Folder+"MarshEnemyFlash.mat");}
            var skitter=Prefab(MarshEnemyKind.Skitter);var crab=Prefab(MarshEnemyKind.Crab);
            var group=SceneManager.GetActiveScene().GetRootGameObjects().Single(g=>g.name=="Marshlight_World").transform.Find("07_Encounters");
            // Retain one original patrol as a data-configured legacy example; replace the scaffold's other four.
            var legacy=group.GetComponentsInChildren<MarshPatrol>(true).Single(p=>p.name=="CampaignMite_4");
            foreach(var old in group.GetComponentsInChildren<MarshPatrol>(true).Where(p=>p!=legacy).ToArray())Object.DestroyImmediate(old.gameObject);
            for(int i=0;i<5;i++)
            {
                var section=level.Campaign.Data.sections[i];var entries=new System.Collections.Generic.List<SectionData.Encounter>();
                var definition=Definition("Skitter_Ch"+(i+1),MarshEnemyKind.Skitter);var pos=new[]{new Vector2(12,-1),new Vector2(35,1.5f),new Vector2(60,-3),new Vector2(103,-3),new Vector2(128,-2)}[i];
                entries.Add(new SectionData.Encounter{id="skitter-"+i,data=definition,position=pos,pointA=pos+Vector2.left*.8f,pointB=pos+Vector2.right*.8f});
                if(i>=1){var cd=Definition("Crab_Ch"+(i+1),MarshEnemyKind.Crab);var cp=new[]{Vector2.zero,new Vector2(40,.8f),new Vector2(58,-3),new Vector2(106,3.5f),new Vector2(133,3)}[i];entries.Add(new SectionData.Encounter{id="crab-"+i,data=cd,position=cp,pointA=cp+Vector2.left*.5f,pointB=cp+Vector2.right*.5f});}
                section.encounters=entries.ToArray();section.ValidateData();EditorUtility.SetDirty(section);foreach(var encounter in entries)Spawn(encounter.data,section,encounter,encounter.data.kind==MarshEnemyKind.Skitter?skitter:crab,group,level);
            }
            var lastSection=level.Campaign.Data.sections[4];var mite=Definition("Mite_Ch5",MarshEnemyKind.LegacyMite);mite.displayName="Marsh Mite";mite.health=3;mite.moveSpeed=.85f;mite.attackSpeed=0;mite.chases=false;EditorUtility.SetDirty(mite);
            var legacyEntry=new SectionData.Encounter{id="legacy-mite-5",data=mite,position=new Vector2(157,3),pointA=new Vector2(156,3),pointB=new Vector2(158,3),resetOnCheckpoint=false};lastSection.encounters=lastSection.encounters.Concat(new[]{legacyEntry}).ToArray();lastSection.ValidateData();EditorUtility.SetDirty(lastSection);
            Ref(legacy,"section",lastSection);MarshCampaignBuilder.Text(legacy,"encounterId",legacyEntry.id);Ref(legacy,"health",legacy.GetComponent<EnemyHitPoints>());
            var legacyFeedback=legacy.GetComponent<MarshEnemyFeedback>();var legacySo=new SerializedObject(legacyFeedback);var oldPips=legacySo.FindProperty("healthPips");var legacyPips=new SpriteRenderer[8];for(int i=0;i<oldPips.arraySize;i++)legacyPips[i]=(SpriteRenderer)oldPips.GetArrayElementAtIndex(i).objectReferenceValue;for(int i=oldPips.arraySize;i<8;i++)legacyPips[i]=Part("HP pip "+i,legacy.transform,new Vector2(0,.8f),new Vector2(.09f,.055f),Color.white,plain);((SpriteRenderer)legacySo.FindProperty("bodySprite").objectReferenceValue).sharedMaterial=flash;legacySo.FindProperty("scalablePips").boolValue=true;legacySo.ApplyModifiedPropertiesWithoutUndo();MarshCampaignBuilder.Refs(legacyFeedback,"healthPips",legacyPips);
            if(Object.FindObjectsByType<PlayerDamageFeedback>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=0)throw new InvalidOperationException("Duplicate player damage feedback.");
            var playerFeedback=level.Player.gameObject.AddComponent<PlayerDamageFeedback>();Ref(playerFeedback,"player",level.Player);Ref(playerFeedback,"bodySprite",level.Player.GetComponent<SpriteRenderer>());
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("M2 saved: shared enemy data/agent, five Skitters and four Crabs; one original data-configured patrol retained.");
        }
    }
}
#endif

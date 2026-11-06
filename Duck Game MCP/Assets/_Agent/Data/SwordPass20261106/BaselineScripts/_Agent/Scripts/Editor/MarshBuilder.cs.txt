#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DuckAdventure.Agent.Editor
{
    /// <summary>Deterministic, Edit Mode-only construction of this one level.</summary>
    public static class MarshBuilder
    {
        private const string Root = "Assets/_Agent/";
        private static Sprite square;
        private static Sprite grass;
        private static Sprite water;
        private static Font bodyFont;
        private static Font titleFont;
        private static Material spriteMaterial;
        private static Transform world;
        private static MarshLevel level;
        private static readonly List<MarshNode> nodes = new List<MarshNode>();
        private static readonly Color Ink = new Color(0.045f, 0.10f, 0.13f);
        private static readonly Color Cream = new Color(0.94f, 0.91f, 0.78f);
        private static readonly Color Gold = new Color(0.97f, 0.77f, 0.35f);

        private static T One<T>() where T : Component
        {
            T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (found.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + ", found " + found.Length);
            return found[0];
        }

        private static void Set(Object target, string field, Object value)
        {
            SerializedObject so = new SerializedObject(target);
            SerializedProperty property = so.FindProperty(field);
            if (property == null) throw new InvalidOperationException(target.name + " has no field " + field);
            property.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void Value(Object target, string field, string value)
        {
            SerializedObject so = new SerializedObject(target);
            so.FindProperty(field).stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void Number(Object target, string field, int value)
        {
            SerializedObject so = new SerializedObject(target);
            so.FindProperty(field).intValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform Group(string name, Transform parent = null)
        {
            GameObject go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static SpriteRenderer Rect(string name, Vector2 position, Vector2 size, Color color, int order, Transform parent, bool solid = false)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.sharedMaterial = spriteMaterial;
            sr.color = color;
            sr.sortingOrder = order;
            if (solid)
            {
                go.layer = LayerMask.NameToLayer("Blocking");
                go.AddComponent<BoxCollider2D>().size = Vector2.one;
            }
            return sr;
        }

        private static void TextureAsset(string name, Color baseColor, bool flecks)
        {
            string path = Root + "Placeholder/" + name + ".png";
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[1024];
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                {
                    float variation = flecks && ((x * 23 + y * 13) % 29 < 3) ? 0.045f : 0f;
                    pixels[y * 32 + x] = new Color(baseColor.r + variation, baseColor.g + variation, baseColor.b + variation, 1f);
                }
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        [MenuItem("Tools/Duck Adventure/Build Marshlight")]
        public static void Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling) throw new InvalidOperationException("Build requires Edit Mode after compilation.");
            if (Object.FindObjectsByType<MarshLevel>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length != 0)
                throw new InvalidOperationException("Marshlight already exists. Refusing to duplicate its manager or world.");
            if (!SceneManager.GetActiveScene().path.EndsWith("/_Agent/Scenes/Marshlight.unity"))
                throw new InvalidOperationException("Open the copied Marshlight scene before building.");

            Player player = One<Player>();
            MovementController movement = One<MovementController>();
            DuckFollower duck = One<DuckFollower>();
            RPGGameManager gm = One<RPGGameManager>();
            RPGCameraManager cm = One<RPGCameraManager>();
            HealthBar health = One<HealthBar>();
            CinemachineCamera vcam = One<CinemachineCamera>();
            SpawnPoint oldSpawn = One<SpawnPoint>();
            if (Object.FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0)
                throw new InvalidOperationException("Existing Inventory must be reused deliberately before this builder runs.");
            foreach (string directory in new[] { "Placeholder", "Prefabs", "Data", "Scripts/Editor", "Data/Verification" }) Directory.CreateDirectory(Root + directory);
            TextureAsset("Square", Color.white, false);
            TextureAsset("MarshGrass", new Color(0.21f, 0.34f, 0.29f), true);
            TextureAsset("NightWater", new Color(0.075f, 0.20f, 0.25f), true);
            square = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "Placeholder/Square.png");
            grass = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "Placeholder/MarshGrass.png");
            water = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "Placeholder/NightWater.png");
            bodyFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            titleFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");
            spriteMaterial = new Material(Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(spriteMaterial, Root + "Placeholder/MarshSprite.mat");

            Transform actors = Group("Actors");
            Transform services = Group("Services");
            Transform cameras = Group("Cameras");
            player.transform.SetParent(actors, true);
            duck.transform.SetParent(actors, true);
            gm.transform.SetParent(services, true);
            cm.transform.SetParent(services, true);
            Camera.main.transform.SetParent(cameras, true);
            vcam.transform.SetParent(cameras, true);

            // The old test terrain survives in LevelOne; only this copied scene is replaced.
            GameObject oldGrid = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "Grid");
            Object.DestroyImmediate(oldGrid);
            GameObject wheat = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "WheatObject");
            Object.DestroyImmediate(wheat);
            world = Group("Marshlight_World");
            level = services.gameObject.AddComponent<MarshLevel>();
            nodes.Clear();
            Set(level, "player", player); Set(level, "movement", movement); Set(level, "duck", duck);
            Set(level, "gameManager", gm); Set(level, "cameraManager", cm);
            Item coin = AssetDatabase.LoadAssetAtPath<Item>("Assets/Scripts/ScriptableObjects/Coin.asset");
            Set(level, "coinItem", coin);
            Set(gm, "player", player); Set(gm, "cameraManager", cm); Set(cm, "virtualCamera", vcam);
            Set(duck, "player", player.transform);
            SerializedObject gmSo = new SerializedObject(gm);
            gmSo.FindProperty("respawnPlayerOnDeath").boolValue = false;
            gmSo.ApplyModifiedPropertiesWithoutUndo();
            player.transform.position = Vector3.zero;
            player.transform.localScale = Vector3.one * 0.065f;
            HitPoints runHealth=Object.Instantiate(player.hitPoints);
            AssetDatabase.CreateAsset(runHealth,Root+"Data/MarshHitPoints.asset");
            player.hitPoints=runHealth;
            player.maxHitPoints = 5; player.startingHitPoints = 5;
            movement.movementSpeed = 1.6f;
            player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
            player.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            player.GetComponent<SpriteRenderer>().sortingLayerID = 0;
            player.GetComponent<SpriteRenderer>().sortingOrder = 100;
            player.GetComponent<SpriteRenderer>().sharedMaterial = spriteMaterial;
            duck.transform.position = Vector3.down * 0.7f;
            duck.transform.localScale = Vector3.one * 0.48f;
            duck.tag = "Untagged";
            duck.GetComponent<SpriteRenderer>().sortingLayerID = 0;
            duck.GetComponent<SpriteRenderer>().sortingOrder = 102;
            duck.GetComponent<SpriteRenderer>().sharedMaterial = spriteMaterial;
            SerializedObject duckSo = new SerializedObject(duck);
            duckSo.FindProperty("followBehindDistance").floatValue = 0.65f;
            duckSo.FindProperty("stopRadius").floatValue = 0.09f;
            duckSo.ApplyModifiedPropertiesWithoutUndo();

            Transform terrain = Group("01_Terrain", world);
            SpriteRenderer waterPlane = Rect("Rill Marsh water", new Vector2(41, 0), new Vector2(98, 28), Color.white, -30, terrain);
            waterPlane.sprite = water;
            waterPlane.drawMode = SpriteDrawMode.Tiled;
            waterPlane.transform.localScale = Vector3.one;
            waterPlane.size = new Vector2(98, 28);
            for (int k = 0; k < 4; k++)
            {
                // Islands are separated by 4-unit channels at x=20,44,68.
                float left = k * 24 - 4;
                float right = k == 3 ? 84 : k * 24 + 18;
                Rect("Bank shadow " + k, new Vector2((left + right) / 2, -0.2f), new Vector2(right - left + 0.5f, 17), new Color(0.10f,0.23f,0.21f), -18, terrain);
                SpriteRenderer island = Rect("Raised island " + k, new Vector2((left + right)/2,0), new Vector2(right-left,16), Color.white,-15,terrain);
                island.sprite = grass; island.drawMode = SpriteDrawMode.Tiled; island.transform.localScale = Vector3.one; island.size = new Vector2(right-left,16);
            }
            Transform banks = Group("02_SolidBanks", world);
            Rect("North bank", new Vector2(40,8.4f),new Vector2(90,0.8f),new Color(0.11f,0.24f,0.21f),35,banks,true);
            Rect("South bank", new Vector2(40,-8.4f),new Vector2(90,0.8f),new Color(0.11f,0.24f,0.21f),35,banks,true);
            Rect("West bank",new Vector2(-4.4f,0),new Vector2(0.8f,17),new Color(0.11f,0.24f,0.21f),35,banks,true);
            Rect("East bank",new Vector2(84.4f,0),new Vector2(0.8f,17),new Color(0.11f,0.24f,0.21f),35,banks,true);
            Transform trails = Group("03_RaisedTrails",world);
            for (int zone = 0; zone < 3; zone++)
            {
                float z = zone * 24;
                ReedWall(banks,z+5,-2.5f,11);
                ReedWall(banks,z+12,2.5f,11);
                Trail(trails,new Vector2(z,0),new Vector2(z+2,5.8f));
                Trail(trails,new Vector2(z+2,5.8f),new Vector2(z+8,5.8f));
                Trail(trails,new Vector2(z+8,5.8f),new Vector2(z+8,-5.8f));
                Trail(trails,new Vector2(z+8,-5.8f),new Vector2(z+15,-5.8f));
                Trail(trails,new Vector2(z+15,-5.8f),new Vector2(z+16,0));
                Trail(trails,new Vector2(z+16,0),new Vector2(z+18,0));
            }
            Transform landmarks = Group("04_Bells_And_Lanterns",world);
            SpawnPoint[] spawns = new SpawnPoint[4];
            string[] cpNames={"Keeper's Landing","Reedbank","Old Archive","Lantern Haven"};
            for(int i=0;i<4;i++)
            {
                float x=i==0?0:i*24-1;
                SpawnPoint spawn=i==0?oldSpawn:Group("Spawn_"+i,services).gameObject.AddComponent<SpawnPoint>();
                spawn.transform.SetParent(services,true);spawn.transform.position=new Vector3(x,0,0);spawns[i]=spawn;
                MarshNode checkpoint=Node("checkpoint-"+i,MarshNodeKind.Checkpoint,new Vector2(x,0),landmarks);
                Number(checkpoint,"sequence",i);Set(checkpoint,"spawnPoint",spawn);Value(checkpoint,"displayName",cpNames[i]);
                checkpoint.GetComponent<BoxCollider2D>().size=new Vector2(1.7f,2.2f);
                Lamp(checkpoint.transform,new Color(0.45f,0.77f,0.64f));
            }
            Set(gm,"playerSpawnPoint",spawns[0]);
            for(int i=0;i<3;i++)
            {
                float x=18+i*24;
                Rect("Uncrossable north channel "+i,new Vector2(x+1,4.65f),new Vector2(2,6.7f),new Color(0.06f,0.17f,0.22f),0,banks,true);
                Rect("Uncrossable south channel "+i,new Vector2(x+1,-4.65f),new Vector2(2,6.7f),new Color(0.06f,0.17f,0.22f),0,banks,true);
                SpriteRenderer bridge=Rect("Causeway "+(i+1),new Vector2(x+1,0),new Vector2(3.4f,2.6f),new Color(0.15f,0.26f,0.27f),5,landmarks);
                for(int plank=0;plank<7;plank++) Rect("Slat "+plank,new Vector2(x-0.4f+plank*0.45f,0),new Vector2(0.06f,2.3f),new Color(0.25f,0.32f,0.29f),6,landmarks);
                SpriteRenderer gate=Rect("Latched gate "+(i+1),new Vector2(x+0.5f,0),new Vector2(0.45f,2.8f),new Color(0.69f,0.36f,0.27f),32,landmarks,true);
                MarshNode bell=Node("bell-"+i,MarshNodeKind.Bell,new Vector2(x-1.4f,0),landmarks);
                Number(bell,"sequence",i);Value(bell,"displayName",new[]{"Landing Bell","Archive Bell","Haven Bell"}[i]);Value(bell,"storyId","bell"+(i+1));
                Set(bell,"closedGate",gate.gameObject);Set(bell,"bridge",bridge);
                BellArt(bell.transform);
                MarshNode tray=Node("tray-"+i,MarshNodeKind.ReturnTray,new Vector2(x+3.6f,-0.35f),landmarks);
                Set(tray,"pairedBell",bell);Value(tray,"displayName","the return tray");
                Rect("Tray body",tray.transform.position,new Vector2(0.6f,0.45f),new Color(0.74f,0.59f,0.30f),40,tray.transform);
                Rect("Tray rim",(Vector2)tray.transform.position+Vector2.up*0.14f,new Vector2(0.72f,0.10f),Gold,41,tray.transform);
            }

            Transform pickups=Group("05_ExistingPickups",world);
            Consumable[] coins=Object.FindObjectsByType<Consumable>(FindObjectsSortMode.None).Where(c=>c.item.itemType==Item.ItemType.COIN).OrderBy(c=>c.name).ToArray();
            Vector2[] coinPositions={new Vector2(2,5.8f),new Vector2(8,-5.8f),new Vector2(15,-4.6f)};
            for(int i=0;i<3;i++)
            {
                coins[i].name="BrassCoin_"+(i+1);coins[i].transform.SetParent(pickups,true);coins[i].transform.position=coinPositions[i];coins[i].transform.localScale=Vector3.one*0.44f;
                coins[i].GetComponent<CircleCollider2D>().radius=0.65f;
                coins[i].GetComponent<SpriteRenderer>().sortingLayerID=0;coins[i].GetComponent<SpriteRenderer>().sortingOrder=75;
            }
            Consumable heart=Object.FindObjectsByType<Consumable>(FindObjectsSortMode.None).Single(c=>c.item.itemType==Item.ItemType.HEALTH);
            heart.name="LandingHeart";heart.transform.SetParent(pickups,true);heart.transform.position=new Vector3(14,-2,0);heart.transform.localScale=Vector3.one*0.45f;
            heart.GetComponent<SpriteRenderer>().sortingLayerID=0;heart.GetComponent<SpriteRenderer>().sortingOrder=75;
            foreach(Vector2 pos in new[]{new Vector2(32,4),new Vector2(55,-4)})
            {
                GameObject h=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HeartObject.prefab"));
                h.name="RestorativeHeart";h.transform.SetParent(pickups,true);h.transform.position=pos;h.transform.localScale=Vector3.one*0.45f;
                h.GetComponent<SpriteRenderer>().sortingLayerID=0;h.GetComponent<SpriteRenderer>().sortingOrder=75;
            }
            Transform storyGroup=Group("06_Story",world);
            Note("keeper-note","Ada's field letter","letter",new Vector2(0,1.4f),storyGroup);
            Note("reed-note","the reed ledger","ledger",new Vector2(27,5.8f),storyGroup);
            Note("archive-note","Ada's flood note","flood",new Vector2(39,-5.8f),storyGroup);
            Note("storm-note","the keeper's promise","promise",new Vector2(56,5.8f),storyGroup);
            MarshNode haven=Node("haven",MarshNodeKind.Haven,new Vector2(77,0),landmarks);
            haven.GetComponent<BoxCollider2D>().size=new Vector2(2.6f,3);
            Rect("Lantern circle",new Vector2(77,0),new Vector2(3,3),new Color(0.53f,0.54f,0.34f),3,landmarks);
            for(int i=0;i<6;i++)
            {
                Vector2 p=new Vector2(75.4f+(i%3)*1.6f, i<3?1.8f:-1.8f);
                Transform lamp=Group("Haven lantern "+i,landmarks);lamp.position=p;Lamp(lamp,Gold);
            }
            Sprite duckSprite=duck.GetComponent<SpriteRenderer>().sprite;
            for(int i=0;i<4;i++)
            {
                SpriteRenderer d=Rect("Sheltered duckling "+i,new Vector2(78.5f+(i%2)*0.7f,-0.5f+(i/2)*0.8f),new Vector2(0.35f,0.35f),Cream,65,landmarks);
                d.sprite=duckSprite;
            }
            Trail(trails,new Vector2(71,0),new Vector2(77,0));
            BuildPatrols(world);
            Scatter(world);
            MakeStory();
            BuildUi(health,player);

            Transform fx=Group("Feedback",services);
            ParticleSystem ps=fx.gameObject.AddComponent<ParticleSystem>();
            var main=ps.main;main.playOnAwake=false;main.loop=false;main.duration=1;main.startLifetime=0.65f;main.startSpeed=1.4f;main.startSize=0.07f;main.maxParticles=128;main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=ps.emission;emission.enabled=false;
            var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=0.16f;
            var particleRenderer=ps.GetComponent<ParticleSystemRenderer>();particleRenderer.sharedMaterial=spriteMaterial;particleRenderer.sortingOrder=120;
            Set(level,"feedbackParticles",ps);

            SerializedObject levelSo=new SerializedObject(level);
            var array=levelSo.FindProperty("nodes");array.arraySize=nodes.Count;for(int i=0;i<nodes.Count;i++)array.GetArrayElementAtIndex(i).objectReferenceValue=nodes[i];
            array=levelSo.FindProperty("checkpoints");array.arraySize=spawns.Length;for(int i=0;i<spawns.Length;i++)array.GetArrayElementAtIndex(i).objectReferenceValue=spawns[i];
            array=levelSo.FindProperty("checkpointNames");array.arraySize=cpNames.Length;for(int i=0;i<cpNames.Length;i++)array.GetArrayElementAtIndex(i).stringValue=cpNames[i];
            levelSo.FindProperty("havenPosition").vector2Value=new Vector2(77,0);levelSo.ApplyModifiedPropertiesWithoutUndo();
            vcam.Follow=player.transform;
            var lens=vcam.Lens;lens.OrthographicSize=4.6f;vcam.Lens=lens;
            var composer=vcam.GetComponent<CinemachinePositionComposer>();composer.Damping=new Vector3(0.25f,0.25f,0);
            Camera.main.backgroundColor=Ink;
            Transform bounds=Group("CameraBounds",cameras);
            PolygonCollider2D poly=bounds.gameObject.AddComponent<PolygonCollider2D>();poly.isTrigger=true;poly.excludeLayers=~0;
            poly.points=new[]{new Vector2(-8,-13),new Vector2(-8,13),new Vector2(89,13),new Vector2(89,-13)};
            CinemachineConfiner2D confiner=vcam.GetComponent<CinemachineConfiner2D>();confiner.BoundingShape2D=poly;confiner.enabled=true;
            EditorUtility.SetDirty(player);EditorUtility.SetDirty(duck);EditorUtility.SetDirty(gm);EditorUtility.SetDirty(cm);EditorUtility.SetDirty(health);EditorUtility.SetDirty(vcam);
            foreach (Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            MarshPolish.Apply();
            Debug.Log("Marshlight built: three borrowed-coin bells, four safe landings, four notes, four patrols, one preserved player and duck.");
        }

        private static void ReedWall(Transform parent,float x,float y,float height)
        {
            Rect("Dense reed bank",new Vector2(x,y),new Vector2(0.95f,height),new Color(0.12f,0.24f,0.20f),30,parent,true);
            for(float yy=y-height/2+0.3f;yy<y+height/2;yy+=0.65f)
            {
                Rect("Reed stem",new Vector2(x-0.19f,yy),new Vector2(0.12f,0.58f),new Color(0.42f,0.48f,0.29f),31,parent);
                Rect("Reed tip",new Vector2(x+0.12f,yy+0.13f),new Vector2(0.14f,0.40f),new Color(0.53f,0.51f,0.30f),32,parent);
            }
        }
        private static void Trail(Transform parent,Vector2 a,Vector2 b)
        {
            float length=Vector2.Distance(a,b);int count=Mathf.CeilToInt(length/0.65f);
            for(int i=0;i<=count;i++)Rect("Raised trail stone",Vector2.Lerp(a,b,i/(float)count),new Vector2(0.62f,0.52f),new Color(0.43f,0.47f,0.35f),-5,parent);
        }
        private static MarshNode Node(string id,MarshNodeKind kind,Vector2 pos,Transform parent)
        {
            GameObject go=new GameObject(id);go.transform.SetParent(parent,false);go.transform.position=pos;
            BoxCollider2D box=go.AddComponent<BoxCollider2D>();box.isTrigger=true;box.size=Vector2.one;
            MarshNode node=go.AddComponent<MarshNode>();Set(node,"level",level);Value(node,"nodeId",id);Number(node,"kind",(int)kind);
            SpriteRenderer marker=Rect("Marker",pos,new Vector2(0.18f,0.18f),Gold,45,go.transform);Set(node,"marker",marker);
            nodes.Add(node);return node;
        }
        private static void BellArt(Transform parent)
        {
            Vector2 p=parent.position;
            Rect("Bell post",p+Vector2.up*0.30f,new Vector2(0.12f,1.3f),new Color(0.36f,0.29f,0.20f),36,parent);
            Rect("Brass bell",p+new Vector2(0.15f,0.65f),new Vector2(0.64f,0.50f),Gold,40,parent);
            Rect("Bell lip",p+new Vector2(0.15f,0.42f),new Vector2(0.80f,0.12f),new Color(0.78f,0.53f,0.23f),41,parent);
            Rect("Bell slot",p+new Vector2(0.15f,0.63f),new Vector2(0.27f,0.08f),Ink,42,parent);
        }
        private static void Lamp(Transform parent,Color color)
        {
            Vector2 p=parent.position;
            Rect("Lantern foot",p+Vector2.left*0.6f,new Vector2(0.14f,0.8f),new Color(0.38f,0.32f,0.22f),35,parent);
            Rect("Lantern glow",p+new Vector2(-0.6f,0.5f),new Vector2(0.50f,0.5f),new Color(color.r,color.g,color.b,0.16f),34,parent);
            Rect("Lantern flame",p+new Vector2(-0.6f,0.5f),new Vector2(0.22f,0.28f),color,40,parent);
        }
        private static void Note(string id,string label,string entry,Vector2 p,Transform parent)
        {
            MarshNode n=Node(id,MarshNodeKind.Note,p,parent);Value(n,"displayName",label);Value(n,"storyId",entry);
            Rect("Note post",p,new Vector2(0.12f,0.9f),new Color(0.36f,0.29f,0.20f),34,n.transform);
            Rect("Paper",p+Vector2.up*0.32f,new Vector2(0.65f,0.52f),Cream,38,n.transform);
            for(int i=0;i<3;i++)Rect("Ink",p+new Vector2(0,0.2f+i*0.1f),new Vector2(0.42f,0.035f),Ink,39,n.transform);
        }

        private static void BuildPatrols(Transform parent)
        {
            Transform group=Group("07_MarshMites",parent);
            EnemyHitPoints first=One<EnemyHitPoints>();
            Vector2[] centers={new Vector2(9,2),new Vector2(33,-2),new Vector2(56,-1.8f),new Vector2(63,-4.5f)};
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyObject.prefab");
            for(int i=0;i<centers.Length;i++)
            {
                GameObject go=i==0?first.gameObject:(GameObject)PrefabUtility.InstantiatePrefab(prefab);
                go.name="MarshMite_"+(i+1);go.transform.SetParent(group,true);go.transform.position=centers[i];go.transform.localScale=Vector3.one*0.72f;
                go.GetComponent<SpriteRenderer>().sortingLayerID=0;go.GetComponent<SpriteRenderer>().sortingOrder=90;go.GetComponent<SpriteRenderer>().color=new Color(0.90f,0.55f,0.45f);
                Rigidbody2D body=go.GetComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;body.gravityScale=0;
                body.interpolation=RigidbodyInterpolation2D.Interpolate;
                MarshPatrol patrol=go.AddComponent<MarshPatrol>();SerializedObject so=new SerializedObject(patrol);
                so.FindProperty("pointA").vector2Value=centers[i]+Vector2.left*1.4f;so.FindProperty("pointB").vector2Value=centers[i]+Vector2.right*1.4f;
                so.FindProperty("phase").floatValue=i*0.7f;so.ApplyModifiedPropertiesWithoutUndo();
                Rect("Hazard track",centers[i],new Vector2(3.6f,0.08f),new Color(0.64f,0.39f,0.31f),-3,group);
            }
        }

        private static void Scatter(Transform parent)
        {
            Transform group=Group("08_Reeds_And_Ripples",parent);
            System.Random rng=new System.Random(20261103);
            for(int i=0;i<120;i++)
            {
                float x=(float)rng.NextDouble()*88-5;float y=(i%2==0?1:-1)*(9+(float)rng.NextDouble()*3);
                Rect("Water glint",new Vector2(x,y),new Vector2(0.20f+(float)rng.NextDouble()*0.45f,0.045f),new Color(0.25f,0.40f,0.43f,0.8f),-25,group);
            }
            for(int zone=0;zone<3;zone++)for(int i=0;i<20;i++)
            {
                float x=zone*24-2+(float)rng.NextDouble()*19;float y=i%2==0?7.5f:-7.5f;
                Rect("Bank reed",new Vector2(x,y),new Vector2(0.08f,0.45f),new Color(0.50f,0.51f,0.31f),25,group);
            }
        }

        private static void MakeStory()
        {
            MarshStory data=ScriptableObject.CreateInstance<MarshStory>();
            string json="{\"entries\":["+
                Entry("intro","Ada's field letter", "Aiken — the storm has jammed the three sluices. I took the ducklings to Lantern Haven. Duckduckgoose knows their voices. Bring him east, and bring three brass coins.","A bell does not take a toll. Lend it three coins to lower the causeway. Its return tray waits on the far bank. Follow the raised stones; stay clear of the coral-backed marsh mites.")+","+
                Entry("letter","Aiken", "Ada's writing leans uphill, even when the water rises. Three coins, three bells, one very determined duck. All right, Duckduckgoose. Let's find your family.")+","+
                Entry("bell1","The landing bell", "Three brass notes roll over the reeds. Beneath the water, wooden steps rise and catch on their latches.","Duckduckgoose lifts his head. One answering peep comes from far to the east. Cross the causeway, then press E at the brass tray to reclaim your coins.")+","+
                Entry("ledger","The reed ledger", "KEEPER'S RULE: Never keep what a traveler needs for the next crossing. The bell borrows the weight; the latch does the work. — Ada","Below it, someone has drawn a duck with a crown. Aiken suspects Duckduckgoose has been supervising.")+","+
                Entry("flood","Ada's flood note", "The archive roof held. All four ducklings are dry. I can hear the first bell again. If that's you, Aiken, you're doing well.","The storm channel is narrow. Wait for a mite to pass, then follow the stones. The lantern landings restore your health. We will keep a light for you.")+","+
                Entry("bell2","The archive bell", "The second bell finds the first bell's note. Duckduckgoose answers with a sound much larger than he is.","A small chorus answers. They are still together. Aiken checks the far-side tray: the same three coins, warm from the mechanism, ready for one more crossing.")+","+
                Entry("promise","The keeper's promise", "A safe path is a promise made to someone you may never meet. Leave the steps raised. Leave the bells ready. Bring each other home. — Ada इत".Replace("\u0907\u0924", ""))+","+
                Entry("bell3","The haven bell", "The last bell rings. Across the water, four small shapes gather at the light.","Duckduckgoose stops listening and starts hurrying. The final causeway is raised. Lantern Haven is just east of the return tray.")+","+
                Entry("ending","Aiken and Ada", "Duckduckgoose disappears into a tumble of wings and delighted peeps. Ada sets down her lantern. 'I knew you would hear us.'", "Aiken leaves the last causeway latched. Three borrowed coins have opened a way for everyone. Behind them, the bells keep speaking across the dark water.")+"]}";
            JsonUtility.FromJsonOverwrite(json,data);AssetDatabase.CreateAsset(data,Root+"Data/MarshStory.asset");Set(level,"story",data);
            File.WriteAllText(Root+"Data/Story.json",json);
        }
        private static string Entry(string id,string speaker,params string[] pages)
        {
            return "{\"id\":\""+id+"\",\"speaker\":\""+speaker+"\",\"pages\":["+string.Join(",",pages.Select(p=>"\""+p.Replace("\\","\\\\").Replace("\"","\\\"").Replace("\n","\\n")+"\""))+"]}";
        }

        private static RectTransform Ui(string name,Transform parent,Vector2 anchor,Vector2 pivot,Vector2 position,Vector2 size)
        {
            GameObject go=new GameObject(name,typeof(RectTransform));go.layer=5;RectTransform rt=(RectTransform)go.transform;
            rt.SetParent(parent,false);rt.anchorMin=anchor;rt.anchorMax=anchor;rt.pivot=pivot;rt.anchoredPosition=position;rt.sizeDelta=size;return rt;
        }
        private static Image Panel(string name,Transform parent,Vector2 anchor,Vector2 pivot,Vector2 position,Vector2 size,Color color)
        {Image img=Ui(name,parent,anchor,pivot,position,size).gameObject.AddComponent<Image>();img.color=color;return img;}
        private static Text Text(string name,Transform parent,Vector2 anchor,Vector2 pivot,Vector2 position,Vector2 size,int fontSize,Color color,string value="",bool title=false,TextAnchor alignment=TextAnchor.UpperLeft)
        {
            Text text=Ui(name,parent,anchor,pivot,position,size).gameObject.AddComponent<Text>();text.font=title?titleFont:bodyFont;text.fontSize=fontSize;text.color=color;text.text=value;text.alignment=alignment;text.raycastTarget=false;
            text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;return text;
        }
        private static void Stretch(RectTransform rt)
        {rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=Vector2.zero;rt.offsetMax=Vector2.zero;}

        private static void BuildUi(HealthBar health,Player player)
        {
            Canvas canvas=health.GetComponent<Canvas>();canvas.sortingOrder=20;
            CanvasScaler scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=0.5f;
            Transform old=health.transform.Find("Background");if(old!=null)Object.DestroyImmediate(old.gameObject);
            Transform hud=Ui("MarshHUD",canvas.transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);Stretch((RectTransform)hud);
            Image top=Panel("Top veil",hud,new Vector2(0,1),new Vector2(0,1),Vector2.zero,new Vector2(1280,126),new Color(Ink.r,Ink.g,Ink.b,0.94f));top.raycastTarget=false;
            Text chapter=Text("Region",hud,new Vector2(0,1),new Vector2(0,1),new Vector2(32,-20),new Vector2(850,24),16,Gold,"RILL MARSH",true);
            Text objective=Text("Objective",hud,new Vector2(0,1),new Vector2(0,1),new Vector2(32,-52),new Vector2(950,38),23,Cream);
            Text ledger=Text("Bells",hud,new Vector2(1,1),new Vector2(1,1),new Vector2(-32,-22),new Vector2(260,24),16,Gold,"BELLS 0 / 3",true,TextAnchor.UpperRight);
            Text brass=Text("Brass",hud,new Vector2(1,1),new Vector2(1,1),new Vector2(-32,-56),new Vector2(260,32),22,Cream,"BRASS 00",true,TextAnchor.UpperRight);
            Panel("HP track",hud,new Vector2(0,1),new Vector2(0,1),new Vector2(32,-98),new Vector2(150,9),new Color(0.13f,0.22f,0.23f));
            Image meter=Panel("HP meter",hud,new Vector2(0,1),new Vector2(0,1),new Vector2(32,-98),new Vector2(150,9),new Color(0.48f,0.80f,0.64f));meter.sprite=square;meter.type=Image.Type.Filled;meter.fillMethod=Image.FillMethod.Horizontal;meter.fillOrigin=0;
            Text hp=Text("HP count",hud,new Vector2(0,1),new Vector2(0,1),new Vector2(196,-94),new Vector2(160,24),16,Cream);
            health.character=player;health.hitPoints=player.hitPoints;health.meterImage=meter;health.hpText=hp;
            Panel("Footer veil",hud,Vector2.zero,Vector2.zero,Vector2.zero,new Vector2(1280,90),new Color(Ink.r,Ink.g,Ink.b,0.88f)).raycastTarget=false;
            Text prompt=Text("Interaction",hud,new Vector2(0.5f,0),new Vector2(0.5f,0),new Vector2(130,22),new Vector2(880,34),20,Cream,"",false,TextAnchor.MiddleCenter);
            Text toast=Text("Toast",hud,new Vector2(0.5f,0),new Vector2(0.5f,0),new Vector2(0,111),new Vector2(1120,40),21,Gold,"",false,TextAnchor.MiddleCenter);
            Image flash=Panel("Damage flash",hud,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,Color.clear);Stretch(flash.rectTransform);flash.raycastTarget=false;
            Set(level,"chapterText",chapter);Set(level,"objectiveText",objective);Set(level,"coinText",brass);Set(level,"ledgerText",ledger);Set(level,"promptText",prompt);Set(level,"toastText",toast);Set(level,"damageFlash",flash);

            Image dialogue=Panel("Dialogue",canvas.transform,new Vector2(0.5f,0),new Vector2(0.5f,0),new Vector2(0,96),new Vector2(1120,200),new Color(0.055f,0.12f,0.15f,0.99f));
            Panel("Dialogue gold rule",dialogue.transform,new Vector2(0,1),new Vector2(0,1),Vector2.zero,new Vector2(1120,3),Gold);
            Text speaker=Text("Speaker",dialogue.transform,new Vector2(0,1),new Vector2(0,1),new Vector2(28,-20),new Vector2(1050,28),18,Gold,"",true);
            Text lines=Text("Pages",dialogue.transform,new Vector2(0,1),new Vector2(0,1),new Vector2(28,-60),new Vector2(1060,102),25,Cream);
            Text("Continue hint",dialogue.transform,new Vector2(1,0),new Vector2(1,0),new Vector2(-28,12),new Vector2(600,24),15,Gold,"SPACE / ENTER / E   CONTINUE",true,TextAnchor.LowerRight);
            Set(level,"dialoguePanel",dialogue.gameObject);Set(level,"speakerText",speaker);Set(level,"dialogueText",lines);

            Image menu=Panel("Story menu",canvas.transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,new Color(Ink.r,Ink.g,Ink.b,0.96f));Stretch(menu.rectTransform);
            Text eyebrow=Text("Eyebrow",menu.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,215),new Vector2(1100,28),17,Gold,"",true,TextAnchor.MiddleCenter);
            Text title=Text("Title",menu.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,148),new Vector2(1140,86),38,Cream,"",true,TextAnchor.MiddleCenter);
            Panel("Menu rule",menu.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,92),new Vector2(90,3),Gold);
            Text body=Text("Body",menu.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,-33),new Vector2(850,250),22,Cream,"",false,TextAnchor.MiddleCenter);
            Image button=Panel("Primary action",menu.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,-214),new Vector2(340,54),Gold);
            Button action=button.gameObject.AddComponent<Button>();action.targetGraphic=button;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(action.onClick,level.PrimaryAction);
            Text primary=Text("Action label",button.transform,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),Vector2.zero,new Vector2(330,48),17,Ink,"",true,TextAnchor.MiddleCenter);
            Set(level,"menuPanel",menu.gameObject);Set(level,"menuEyebrow",eyebrow);Set(level,"menuTitle",title);Set(level,"menuBody",body);Set(level,"primaryLabel",primary);
            dialogue.gameObject.SetActive(false);
            EventSystem[] eventSystems=Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            if(eventSystems.Length>1)throw new InvalidOperationException("Duplicate EventSystems found.");
            if(eventSystems.Length==0)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));

            GameObject slot=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Slot.prefab"));
            slot.name="MarshSlot";
            Slot slotScript=slot.GetComponent<Slot>();slotScript.itemImage=slot.transform.Find("ItemImage").GetComponent<Image>();
            ((RectTransform)slot.transform).sizeDelta=new Vector2(44,44);
            ((RectTransform)slotScript.itemImage.transform).sizeDelta=new Vector2(34,34);
            slotScript.itemImage.enabled=false;slotScript.qtyText.font=bodyFont;slotScript.qtyText.fontSize=16;slotScript.qtyText.enabled=false;slotScript.qtyText.color=Cream;
            foreach(Image image in slot.GetComponentsInChildren<Image>())if(image!=slotScript.itemImage){image.sprite=null;image.color=new Color(0.17f,0.27f,0.28f);}
            GameObject slotPrefab=PrefabUtility.SaveAsPrefabAsset(slot,Root+"Prefabs/MarshSlot.prefab");Object.DestroyImmediate(slot);
            GameObject inventoryObject=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryObject.prefab"));inventoryObject.name="InventoryObject";
            inventoryObject.transform.localScale=Vector3.one;
            Inventory inventory=inventoryObject.GetComponent<Inventory>();inventory.slotPrefab=slotPrefab;
            Canvas invCanvas=inventory.GetComponent<Canvas>();invCanvas.sortingOrder=10;
            CanvasScaler invScaler=inventory.GetComponent<CanvasScaler>();invScaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;invScaler.referenceResolution=new Vector2(1280,720);invScaler.matchWidthOrHeight=0.5f;
            RectTransform container=(RectTransform)inventory.transform.Find("InventoryBackground");container.anchorMin=Vector2.zero;container.anchorMax=Vector2.zero;container.pivot=Vector2.zero;container.anchoredPosition=new Vector2(30,20);container.sizeDelta=new Vector2(236,44);
            HorizontalLayoutGroup layout=container.GetComponent<HorizontalLayoutGroup>();layout.spacing=4;layout.childAlignment=TextAnchor.MiddleLeft;layout.childForceExpandWidth=false;layout.childForceExpandHeight=false;
            Set(inventory,"slotsRoot",container);
            Transform unused=inventory.transform.Find("Inventory");if(unused!=null)Object.DestroyImmediate(unused.gameObject);
            GameObject inventoryPrefab=PrefabUtility.SaveAsPrefabAsset(inventoryObject,Root+"Prefabs/MarshInventory.prefab");
            player.inventoryPrefab=inventoryPrefab.GetComponent<Inventory>();Set(player,"sceneInventory",inventory);Set(player,"sceneHealthBar",health);
            player.healthBarPrefab=AssetDatabase.LoadAssetAtPath<HealthBar>("Assets/Prefabs/HealthBarObject.prefab");
        }

        [MenuItem("Tools/Duck Adventure/Repair Serialized Presentation")]
        public static void RepairPresentation()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode before editing assets.");
            MarshStory data=AssetDatabase.LoadAssetAtPath<MarshStory>(Root+"Data/MarshStory.asset");
            JsonUtility.FromJsonOverwrite(File.ReadAllText(Root+"Data/Story.json"),data);
            if(new[]{"intro","letter","bell1","ledger","flood","bell2","promise","bell3","ending"}.Any(id=>!data.Entries.Any(e=>e.id==id)))throw new InvalidOperationException("Expected all nine baseline story entries.");
            EditorUtility.SetDirty(data);
            foreach(CanvasScaler scaler in Object.FindObjectsByType<CanvasScaler>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=0.5f;
                PrefabUtility.RecordPrefabInstancePropertyModifications(scaler);
            }
            One<MovementController>().movementSpeed=1.6f;
            HitPoints runHealth=AssetDatabase.LoadAssetAtPath<HitPoints>(Root+"Data/MarshHitPoints.asset");
            if(runHealth==null){runHealth=Object.Instantiate(One<Player>().hitPoints);AssetDatabase.CreateAsset(runHealth,Root+"Data/MarshHitPoints.asset");}
            One<Player>().hitPoints=runHealth;One<HealthBar>().hitPoints=runHealth;
            Font readable=AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            foreach(Text text in Object.FindObjectsByType<Text>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if(text.font!=null && text.font.name=="Adventurer")text.font=readable;
                if(text.name=="Body") {text.fontSize=22;text.rectTransform.sizeDelta=new Vector2(850,250);}
            }
            foreach(Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(PrefabUtility.IsPartOfPrefabInstance(component))PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            Validate();
        }

        [MenuItem("Tools/Duck Adventure/Validate Marshlight")]
        public static void Validate()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Use the Play Mode verifier during play.");
            One<MarshLevel>();One<Player>();One<DuckFollower>();One<RPGGameManager>();One<RPGCameraManager>();One<HealthBar>();One<Inventory>();One<EventSystem>();
            List<string> missing=new List<string>();
            foreach(MonoBehaviour behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if(behaviour==null)throw new InvalidOperationException("Missing script in scene.");
                if(!(behaviour is MarshLevel))continue;
                SerializedObject so=new SerializedObject(behaviour);SerializedProperty property=so.GetIterator();
                while(property.NextVisible(true))if(property.propertyType==SerializedPropertyType.ObjectReference && property.name!="m_Script" && property.objectReferenceValue==null)missing.Add(property.propertyPath);
            }
            if(missing.Count>0)throw new InvalidOperationException("Missing refs: "+string.Join(", ",missing));
            Debug.Log("Marshlight validation passed: singletons, UI, serialized level references, and player/duck found.");
        }
    }
}
#endif

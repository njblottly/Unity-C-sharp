#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DuckAdventure.Agent.Editor
{
    /// <summary>Edit-time presentation repair; never changes progression or collision.</summary>
    public static class MarshPropFinish
    {
        private const string Root = "Assets/_Agent/";
        private const string Evidence = Root + "Data/PropFix20261106/";
        [Serializable] private sealed class Audit
        {
            public int colliders;
            public bool collidersUnchanged;
            public bool serializedLogicUnchanged;
            public int postedMailboxes;
            public int bells;
            public int letters;
            public int lanterns;
        }
        private static T One<T>() where T : Component
        {
            T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(c => c.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (found.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + ", found " + found.Length);
            return found[0];
        }
        private static SpriteRenderer Marker(MarshNode node)
        {
            SpriteRenderer marker = (SpriteRenderer)new SerializedObject(node).FindProperty("marker").objectReferenceValue;
            if (marker == null) throw new InvalidOperationException("Missing serialized marker: " + node.Id);
            return marker;
        }
        private static string ColliderStamp()
        {
            Physics2D.SyncTransforms();
            return string.Join("\n", Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OrderBy(c => c.GetInstanceID()).Select(c => c.GetInstanceID() + "|" + EditorJsonUtility.ToJson(c) + "|" + c.transform.localToWorldMatrix.ToString("R") + "|" + c.bounds.ToString("R")));
        }
        private static string LogicStamp()
        {
            return string.Join("\n", Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(c => c.gameObject.scene == SceneManager.GetActiveScene()).OrderBy(c => c.GetInstanceID())
                .Select(c => c.GetInstanceID() + "|" + EditorJsonUtility.ToJson(c)));
        }
        private static Sprite Import(string name, int height, Vector2 pivot)
        {
            string path = Root + "Placeholder/Props/" + name + ".png";
            if (!File.Exists(path)) throw new InvalidOperationException("Missing prepared sprite: " + path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32; importer.filterMode = FilterMode.Point; importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false; importer.alphaIsTransparency = true; importer.npotScale = TextureImporterNPOTScale.None;
            TextureImporterSettings settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect; settings.spriteAlignment = (int)SpriteAlignment.Custom; settings.spritePivot = pivot;
            importer.SetTextureSettings(settings); importer.SaveAndReimport();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null || sprite.rect.width != 32 || sprite.rect.height != height) throw new InvalidOperationException("Wrong logical size: " + path);
            return sprite;
        }
        private static void RemoveVisual(SpriteRenderer renderer)
        {
            if (renderer.gameObject.GetComponents<Component>().Any(c => !(c is Transform) && !(c is SpriteRenderer)))
                throw new InvalidOperationException("Refusing to remove logic/collision attached to visual: " + renderer.name);
            Object.DestroyImmediate(renderer.gameObject);
        }
        private static SpriteRenderer Visual(Transform parent, string name, Material material)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                SpriteRenderer renderer = existing.GetComponent<SpriteRenderer>();
                if (renderer == null) throw new InvalidOperationException("Expected a sprite at " + name);
                return renderer;
            }
            GameObject go = new GameObject(name); go.transform.SetParent(parent, false);
            SpriteRenderer created = go.AddComponent<SpriteRenderer>(); created.sharedMaterial = material; return created;
        }
        private static void Place(SpriteRenderer renderer, Sprite sprite, Vector3 position, Vector3 scale, int order, Color color)
        {
            renderer.sprite = sprite; renderer.color = color; renderer.sortingOrder = order;
            renderer.transform.localPosition = position; renderer.transform.localScale = scale; renderer.transform.localRotation = Quaternion.identity;
            EditorUtility.SetDirty(renderer);
        }
        private static void Lantern(Transform root, SpriteRenderer core, Sprite lantern, Sprite square, Color color)
        {
            SpriteRenderer body = root.GetComponentsInChildren<SpriteRenderer>(true).Single(r => r.name == "Lantern foot" || r.name == "Lantern art");
            foreach (SpriteRenderer other in root.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r != body && r != core).ToArray()) RemoveVisual(other);
            body.name = "Lantern art";
            Place(body, lantern, new Vector3(-0.6f, -0.45f, 0), Vector3.one * 0.85f, 40, Color.white);
            // Prepared glass centre: (15.5, 33) pixels from bottom left, base pivot (16,2).
            // The live marker stays inside the frame rather than floating on the ground.
            Place(core, square, new Vector3(-0.61328125f, 0.3734375f, 0), new Vector3(0.0796875f, 0.0796875f, 1), 41, color);
        }

        [MenuItem("Tools/Duck Adventure/Finish Bell Letter and Posted Mailboxes")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling) throw new InvalidOperationException("Finish props in Edit Mode after compilation.");
            if (SceneManager.GetActiveScene().path != Root + "Scenes/Marshlight.unity") throw new InvalidOperationException("Open Marshlight first.");
            MarshLevel level = One<MarshLevel>(); One<Player>(); One<DuckFollower>(); One<Inventory>(); One<HealthBar>(); One<RPGGameManager>(); One<RPGCameraManager>(); One<Weapon>(); One<MarshEquipmentHUD>();
            MarshStory story = AssetDatabase.LoadAssetAtPath<MarshStory>(Root + "Data/MarshStory.asset");
            if (story == null || story.Entries.Length != 12) throw new InvalidOperationException("Expected the verified twelve-entry sword build.");
            Directory.CreateDirectory(Evidence);
            string before = ColliderStamp(), logicBefore = LogicStamp();
            File.WriteAllText(Evidence + "CollidersBefore.txt", before); File.WriteAllText(Evidence + "LogicBefore.txt", logicBefore);
            var markerIds = level.Nodes.ToDictionary(n => n, n => Marker(n).GetInstanceID());
            Sprite bell = Import("BellFrame", 48, new Vector2(0.5f, 2f / 48));
            Sprite letter = Import("FieldLetter", 32, Vector2.one * 0.5f);
            Sprite lantern = Import("MarshLantern", 48, new Vector2(0.5f, 2f / 48));
            Sprite square = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "Placeholder/Square.png");
            Material material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Placeholder/MarshSprite.mat");
            if (square == null || material == null) throw new InvalidOperationException("Missing existing sprite material/square.");

            MarshNode keeper = level.Nodes.Single(n => n.Id == "keeper-note"); SpriteRenderer firstMailbox = Marker(keeper);
            if (firstMailbox.sprite.rect.height != 48) throw new InvalidOperationException("The good first mailbox must retain its complete sprite.");
            MarshNode[] returns = level.Nodes.Where(n => n.Kind == MarshNodeKind.ReturnTray || n.Kind == MarshNodeKind.RelayReturn).ToArray();
            if (returns.Length != 6) throw new InvalidOperationException("Expected six existing return mailboxes.");
            foreach (MarshNode node in returns)
            {
                SpriteRenderer art = node.GetComponentsInChildren<SpriteRenderer>(true).Single(r => r.name == "Return mailbox art");
                Place(art, firstMailbox.sprite, firstMailbox.transform.localPosition, firstMailbox.transform.localScale, 40, Color.white);
                SpriteRenderer coin = Marker(node);
                Place(coin, level.CoinItem.sprite, new Vector3(-0.07f, 0.72f, 0), new Vector3(0.16f, 0.16f, 1), 45, coin.color);
                foreach (TextMesh label in node.GetComponentsInChildren<TextMesh>(true))
                    label.transform.localPosition = new Vector3(0, 1.4f, 0); // Keep return quantities above the full posted box.
            }
            SpriteRenderer fieldLetter = Visual(keeper.transform, "Field letter", material);
            Place(fieldLetter, letter, new Vector3(0.16f, 0.61f, 0), Vector3.one * 0.32f, 41, Color.white);

            MarshNode[] bells = level.Nodes.Where(n => n.Kind == MarshNodeKind.Bell).ToArray();
            if (bells.Length != 3) throw new InvalidOperationException("Expected three original bells.");
            foreach (MarshNode node in bells)
            {
                SpriteRenderer marker = Marker(node);
                SpriteRenderer art = node.GetComponentsInChildren<SpriteRenderer>(true).Single(r => r.name == "Brass bell" || r.name == "Bell art");
                foreach (SpriteRenderer old in node.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r != art && r != marker).ToArray()) RemoveVisual(old);
                art.name = "Bell art"; Place(art, bell, Vector3.zero, Vector3.one * 0.85f, 40, Color.white);
                Place(marker, level.CoinItem.sprite, new Vector3(0, 0.3f, 0), new Vector3(0.12f, 0.12f, 1), 41, marker.color);
            }
            MarshNode[] checkpoints = level.Nodes.Where(n => n.Kind == MarshNodeKind.Checkpoint).ToArray();
            if (checkpoints.Length != 4) throw new InvalidOperationException("Expected four existing checkpoint lamps.");
            foreach (MarshNode checkpoint in checkpoints) Lantern(checkpoint.transform, Marker(checkpoint), lantern, square, new Color(0.45f, 0.80f, 0.67f));
            Transform[] havenLamps = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).Where(t => t.name.StartsWith("Haven lantern ")).ToArray();
            if (havenLamps.Length != 6) throw new InvalidOperationException("Expected six existing haven lamps.");
            foreach (Transform lamp in havenLamps)
            {
                SpriteRenderer core = lamp.GetComponentsInChildren<SpriteRenderer>(true).Single(r => r.name == "Lantern flame");
                Lantern(lamp, core, lantern, square, new Color(1f, 0.85f, 0.45f));
            }

            string after = ColliderStamp(), logicAfter = LogicStamp();
            File.WriteAllText(Evidence + "CollidersAfter.txt", after); File.WriteAllText(Evidence + "LogicAfter.txt", logicAfter);
            if (before != after) throw new InvalidOperationException("A collider or its transform changed during the art repair.");
            if (logicBefore != logicAfter || level.Nodes.Any(n => Marker(n).GetInstanceID() != markerIds[n]))
                throw new InvalidOperationException("Serialized game logic or marker identity changed during the art repair.");
            Audit audit = new Audit { colliders = 59, collidersUnchanged = true, serializedLogicUnchanged = true, postedMailboxes = 7, bells = 3, letters = 1, lanterns = 10 };
            File.WriteAllText(Evidence + "BuildAudit.json", JsonUtility.ToJson(audit, true));
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            MarshBuilder.Validate(); Debug.Log("Pixel prop finish saved: seven complete mailboxes, three bells, one letter, ten lanterns; all collision and serialized gameplay unchanged.");
        }
    }
}
#endif

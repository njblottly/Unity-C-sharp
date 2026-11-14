#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DuckAdventure.Agent.Editor
{
    /// <summary>Edit-time art reuse only. No colliders, runtime controllers, or source texture edits.</summary>
    public static class MarshOutdoorArt
    {
        private const string Output = "Assets/_Agent/Sprites/OutdoorReuse/";
        private const string Evidence = "Assets/_Agent/Data/OutdoorArt20261106/";
        private static readonly Dictionary<string, Sprite[]> sheets = new Dictionary<string, Sprite[]>();
        private static readonly Color GrassTint = new Color(0.34f, 0.54f, 0.96f);
        private static readonly Color LeafTint = new Color(0.68f, 0.84f, 0.88f);
        private static Material material;

        [Serializable] private sealed class SpriteMapping
        {
            public string asset;
            public string source;
            public Rect sourcePixels;
            public float pixelsPerUnit;
        }
        [Serializable] private sealed class Report
        {
            public string utc;
            public int originalColliders;
            public int artRenderers;
            public int replacements;
            public bool collisionsUnchanged;
            public bool sourceFilesUnchanged;
            public string collisionSha256;
            public SpriteMapping[] assembledSprites;
        }
        private static readonly List<SpriteMapping> mappings = new List<SpriteMapping>();

        private static T One<T>() where T : Component
        {
            T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => x.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (found.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + ", found " + found.Length);
            return found[0];
        }
        private static string SheetPath(string name)
        {
            string[] paths = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites/Outdoors" })
                .Select(AssetDatabase.GUIDToAssetPath).Where(p => Path.GetFileNameWithoutExtension(p) == "Topdown RPG 32x32 - " + name).ToArray();
            if (paths.Length != 1) throw new InvalidOperationException("Expected one source sheet: " + name);
            return paths[0];
        }
        private static Sprite Slice(string name, int index)
        {
            string path = SheetPath(name);
            if (!sheets.ContainsKey(path)) sheets.Add(path, AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray());
            string expected = Path.GetFileNameWithoutExtension(path) + "_" + index;
            return sheets[path].Single(s => s.name == expected);
        }
        private static Sprite Assemble(string name, string sheet, Rect rect, Vector2 pivot)
        {
            string source = SheetPath(sheet), path = Output + name + ".asset";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(source);
            if (rect.xMin < 0 || rect.yMin < 0 || rect.xMax > texture.width || rect.yMax > texture.height)
                throw new InvalidOperationException("Assembled sprite exceeds source sheet: " + name);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                sprite = Sprite.Create(texture, rect, pivot, 32, 0, SpriteMeshType.FullRect);
                sprite.name = name; AssetDatabase.CreateAsset(sprite, path);
            }
            if (sprite.texture != texture || sprite.rect != rect || sprite.pixelsPerUnit != 32)
                throw new InvalidOperationException("An existing assembled sprite has different source metadata: " + path);
            mappings.Add(new SpriteMapping { asset = path, source = source, sourcePixels = rect, pixelsPerUnit = 32 });
            return sprite;
        }
        private static Transform Group(string name, Transform parent)
        {
            if (parent.Find(name) != null) throw new InvalidOperationException("Duplicate art group: " + name);
            Transform group = new GameObject(name).transform; group.SetParent(parent, false); return group;
        }
        private static SpriteRenderer Place(string name, Sprite sprite, Vector2 position, Vector2 scale, Color tint, int order, Transform parent)
        {
            GameObject go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.position = position;
            go.transform.localScale = new Vector3(scale.x, scale.y, 1);
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.color = tint;
            renderer.sharedMaterial = material; renderer.sortingLayerID = 0; renderer.sortingOrder = order; return renderer;
        }
        private static void FitDecoration(SpriteRenderer renderer, Sprite sprite, Vector2 size, Color tint)
        {
            if (renderer.GetComponent<Collider2D>() != null) throw new InvalidOperationException("Do not resize collider-bearing art: " + renderer.name);
            renderer.sprite = sprite; renderer.drawMode = SpriteDrawMode.Simple; renderer.color = tint;
            renderer.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1);
        }
        private static void Tiled(SpriteRenderer renderer, Sprite sprite, Color tint)
        {
            if (renderer.GetComponent<Collider2D>() != null) throw new InvalidOperationException("Do not rescale collider-bearing terrain: " + renderer.name);
            Vector2 size = renderer.bounds.size;
            renderer.sprite = sprite; renderer.color = tint; renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.transform.localScale = Vector3.one; renderer.size = size;
        }
        private static string HierarchyPath(Transform transform)
        {
            return transform.parent == null ? transform.name : HierarchyPath(transform.parent) + "/" + transform.name;
        }
        private static string Number(float value) => value.ToString("R", CultureInfo.InvariantCulture);
        public static string CollisionSignature()
        {
            Physics2D.SyncTransforms();
            return string.Join("\n", Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(c => c.gameObject.scene == SceneManager.GetActiveScene()).Select(c =>
                {
                    Transform t = c.transform;
                    return HierarchyPath(t) + "|" + c.GetType().Name + "|" + EditorJsonUtility.ToJson(c) + "|" +
                        Number(t.position.x) + "," + Number(t.position.y) + "," + Number(t.position.z) + "|" +
                        Number(t.lossyScale.x) + "," + Number(t.lossyScale.y) + "|" + Number(t.eulerAngles.z) + "|" +
                        Number(c.bounds.center.x) + "," + Number(c.bounds.center.y) + "|" + Number(c.bounds.size.x) + "," + Number(c.bounds.size.y) + "|" +
                        c.gameObject.layer + "|" + c.gameObject.activeSelf;
                }).OrderBy(s => s));
        }
        private static string Hash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        [MenuItem("Tools/Duck Adventure/Reuse Outdoor Environment Art")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling) throw new InvalidOperationException("Use Edit Mode after compilation.");
            if (SceneManager.GetActiveScene().path != "Assets/_Agent/Scenes/Marshlight.unity") throw new InvalidOperationException("Open Marshlight first.");
            MarshBuilder.Validate(); MarshLevel level = One<MarshLevel>(); One<Player>(); One<DuckFollower>();
            One<RPGGameManager>(); One<RPGCameraManager>(); One<Inventory>(); One<HealthBar>();
            Transform world = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "Marshlight_World").transform;
            if (world.Find("10_OutdoorArt") != null) throw new InvalidOperationException("Outdoor art already exists. Refusing to duplicate it.");
            material = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat");
            if (material == null) throw new InvalidOperationException("Missing existing sprite material.");
            Directory.CreateDirectory(Output); Directory.CreateDirectory(Evidence); mappings.Clear();
            string before = CollisionSignature(); File.WriteAllText(Evidence + "CollisionBefore.txt", before);
            string[] sourceFiles = Directory.GetFiles("Assets/Sprites/Outdoors", "*", SearchOption.AllDirectories);
            var sourceHashes = sourceFiles.ToDictionary(p => p, p => Hash(File.ReadAllBytes(p)));
            Sprite grass = Assemble("GrassFill", "Ground Tileset 1.1", new Rect(352, 352, 32, 32), Vector2.one * 0.5f);
            Sprite dirt = Assemble("DirtFill", "Ground Tileset 1.1", new Rect(320, 320, 32, 32), Vector2.one * 0.5f);
            Sprite water = Assemble("WaterFill", "Ground Tileset 1.1", new Rect(352, 0, 32, 32), Vector2.one * 0.5f);
            Sprite[] patches = {
                Assemble("GrassPatchA", "Ground Tileset 1.1", new Rect(256, 64, 64, 64), Vector2.one * 0.5f),
                Assemble("GrassPatchB", "Ground Tileset 1.1", new Rect(320, 64, 64, 64), Vector2.one * 0.5f)
            };
            Sprite[] slabs = {
                Assemble("TrailSlabA", "Rocks 1.1", new Rect(192, 32, 64, 32), Vector2.one * 0.5f),
                Assemble("TrailSlabB", "Rocks 1.1", new Rect(192, 0, 64, 32), Vector2.one * 0.5f)
            };
            Sprite[] trees = {
                Assemble("MarshTreeA", "Trees 1.2", new Rect(0, 192, 96, 128), new Vector2(0.5f, 0.12f)),
                Assemble("MarshTreeB", "Trees 1.2", new Rect(96, 192, 96, 128), new Vector2(0.5f, 0.12f)),
                Assemble("MarshTreeC", "Trees 1.2", new Rect(192, 192, 96, 128), new Vector2(0.5f, 0.12f))
            };
            Sprite boulder = Assemble("BankBoulder", "Rocks 1.1", new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
            Sprite stump = Assemble("BankStump", "Tree Stumps and Logs", new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
            Sprite log = Assemble("FallenLog", "Tree Stumps and Logs", new Rect(128, 32, 64, 32), Vector2.one * 0.5f);
            Sprite[] bushes = { Slice("Bushes 1.1", 8), Slice("Bushes 1.1", 9), Slice("Bushes 1.1", 10) };
            Sprite[] tufts = { Slice("Nature Details", 0), Slice("Nature Details", 1), Slice("Nature Details", 2), Slice("Nature Details", 4) };
            Transform art = Group("10_OutdoorArt", world);
            Transform shore = Group("01_GrassShore", art), plants = Group("02_GrassAndFlowers", art), border = Group("03_BankTreesRocksLogs", art);
            int changed = 0;
            foreach (SpriteRenderer renderer in world.Find("01_Terrain").GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name == "Rill Marsh water") Tiled(renderer, water, new Color(0.34f, 0.48f, 0.42f));
                else if (renderer.name.StartsWith("Raised island ")) Tiled(renderer, grass, GrassTint);
                else if (renderer.name.StartsWith("Bank shadow ")) Tiled(renderer, dirt, new Color(0.35f, 0.48f, 0.49f));
                changed++;
            }
            foreach (SpriteRenderer renderer in world.Find("02_SolidBanks").GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name.StartsWith("Uncrossable ")) { renderer.sprite = water; renderer.color = new Color(0.31f, 0.43f, 0.40f); }
                else if (renderer.GetComponent<Collider2D>() != null) { renderer.sprite = dirt; renderer.color = new Color(0.40f, 0.54f, 0.48f); }
                else if (renderer.name == "Reed stem") FitDecoration(renderer, bushes[changed % bushes.Length], new Vector2(1.05f, 0.83f), LeafTint);
                else if (renderer.name == "Reed tip") FitDecoration(renderer, tufts[changed % tufts.Length], new Vector2(0.9f, 0.8f), new Color(0.78f, 0.86f, 0.74f));
                changed++;
            }
            int trail = 0;
            SpriteRenderer[] trailStones = world.Find("03_RaisedTrails").GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer renderer in trailStones)
            {
                FitDecoration(renderer, slabs[trail++ % slabs.Length], new Vector2(0.74f, 0.40f), new Color(0.76f, 0.85f, 0.78f)); changed++;
            }
            foreach (SpriteRenderer renderer in world.Find("08_Reeds_And_Ripples").GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.name == "Bank reed"))
            {
                FitDecoration(renderer, tufts[changed % tufts.Length], new Vector2(1, 0.85f), new Color(0.80f, 0.89f, 0.78f)); changed++;
            }
            foreach (SpriteRenderer renderer in world.Find("09_ArchiveExchange").GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name == "Archive latch reed bank") { renderer.sprite = dirt; renderer.color = new Color(0.40f, 0.54f, 0.48f); changed++; }
                else if (renderer.name == "Latch bank reed") { FitDecoration(renderer, bushes[changed % bushes.Length], new Vector2(1.0f, 0.83f), LeafTint); changed++; }
            }
            var random = new System.Random(20261106);
            for (int island = 0; island < 4; island++)
            {
                float left = island * 24 - 4, right = island == 3 ? 84 : island * 24 + 18;
                for (float x = left + 0.5f; x < right; x += 1)
                {
                    Place("South grass fringe", Slice("Ground Tileset 1.1", 25), new Vector2(x, -8.05f), Vector2.one, GrassTint, -14, shore);
                    Place("North grass fringe", Slice("Ground Tileset 1.1", 37), new Vector2(x, 8.05f), Vector2.one, GrassTint, -14, shore);
                }
                for (float y = -7.5f; y < 8; y += 1)
                {
                    Place("West grass fringe", Slice("Ground Tileset 1.1", 27), new Vector2(left - 0.05f, y), Vector2.one, GrassTint, -14, shore);
                    Place("East grass fringe", Slice("Ground Tileset 1.1", 24), new Vector2(right + 0.05f, y), Vector2.one, GrassTint, -14, shore);
                }
                for (int i = 0; i < 6; i++)
                {
                    float x = Mathf.Lerp(left + 2.5f, right - 2.5f, (i % 3) / 2f);
                    float y = i < 3 ? 8.45f : -9.65f;
                    Place("Bank tree " + island + "-" + i, trees[(island + i) % trees.Length], new Vector2(x, y), Vector2.one * 0.78f, LeafTint, 38, border);
                }
                for (int i = 0; i < 10; i++)
                {
                    float x = Mathf.Lerp(left + 0.8f, right - 0.8f, i / 9f), y = i % 2 == 0 ? 8.3f : -8.45f;
                    Place("Shore rock " + island + "-" + i, boulder, new Vector2(x, y), Vector2.one * (0.38f + (i % 3) * 0.06f), new Color(0.70f, 0.80f, 0.71f), 37, border);
                }
                Place("Bank stump " + island, stump, new Vector2(left + 4, 8.25f), Vector2.one * 0.60f, new Color(0.78f, 0.82f, 0.76f), 39, border);
                Place("Fallen log " + island, log, new Vector2(right - 4, -8.5f), Vector2.one * 0.75f, new Color(0.78f, 0.82f, 0.76f), 39, border);
                for (int i = 0; i < 40; i++)
                {
                    Vector2 p = new Vector2(Mathf.Lerp(left + 0.8f, right - 0.8f, (float)random.NextDouble()), Mathf.Lerp(-6.8f, 6.8f, (float)random.NextDouble()));
                    if (level.Nodes.Any(n => Vector2.Distance(n.transform.position, p) < 1.45f) || trailStones.Any(s => Vector2.Distance(s.transform.position, p) < 0.85f)) continue;
                    Place("Marsh tuft " + island + "-" + i, tufts[i % tufts.Length], p, Vector2.one * (0.8f + (i % 3) * 0.15f), new Color(0.77f, 0.85f, 0.75f), -7, plants);
                    if (i % 3 == 0) Place("Grass patch " + island + "-" + i, patches[i % patches.Length], p, Vector2.one * 0.8f, new Color(0.40f, 0.59f, 0.98f), -13, plants);
                }
            }
            if (art.GetComponentsInChildren<Collider2D>(true).Length != 0 || art.GetComponentsInChildren<MonoBehaviour>(true).Length != 0)
                throw new InvalidOperationException("Outdoor decoration must have no colliders or runtime behaviours.");
            string after = CollisionSignature(); File.WriteAllText(Evidence + "CollisionAfter.txt", after);
            if (before != after) throw new InvalidOperationException("A gameplay collider changed during the visual pass.");
            if (sourceFiles.Any(p => Hash(File.ReadAllBytes(p)) != sourceHashes[p])) throw new InvalidOperationException("A source sheet or importer changed.");
            Report report = new Report { utc = DateTime.UtcNow.ToString("o"), originalColliders = Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length,
                artRenderers = art.GetComponentsInChildren<SpriteRenderer>(true).Length, replacements = changed, collisionsUnchanged = true, sourceFilesUnchanged = true,
                collisionSha256 = Hash(System.Text.Encoding.UTF8.GetBytes(before)), assembledSprites = mappings.ToArray() };
            File.WriteAllText(Evidence + "ArtAudit.json", JsonUtility.ToJson(report, true));
            foreach (Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
            MarshBuilder.Validate(); Debug.Log("Outdoor art applied: source sheets and every collider preserved; " + changed + " replacements and " + report.artRenderers + " decorative renderers.");
        }
    }
}
#endif

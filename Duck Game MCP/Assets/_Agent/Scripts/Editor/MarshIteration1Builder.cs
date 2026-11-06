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
    public static class MarshIteration1Builder
    {
        [Serializable] private sealed class StoryDocument { public MarshStory.Entry[] entries; }
        private static Sprite square;
        private static Material material;
        private static Font font;
        private static MarshLevel level;

        private static T One<T>() where T : Component
        {
            T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (found.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + "; found " + found.Length);
            return found[0];
        }
        private static void Ref(Object target, string field, Object reference)
        {
            SerializedObject so = new SerializedObject(target); so.FindProperty(field).objectReferenceValue = reference; so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void Number(Object target, string field, int value)
        {
            SerializedObject so = new SerializedObject(target); so.FindProperty(field).intValue = value; so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void TextValue(Object target, string field, string value)
        {
            SerializedObject so = new SerializedObject(target); so.FindProperty(field).stringValue = value; so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static SpriteRenderer Shape(string name, Vector2 position, Vector2 size, Color color, Transform parent, int order = 40, bool solid = false)
        {
            GameObject go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.position = position; go.transform.localScale = new Vector3(size.x, size.y, 1);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>(); sr.sprite = square; sr.sharedMaterial = material; sr.color = color; sr.sortingOrder = order;
            if (solid) { go.layer = LayerMask.NameToLayer("Blocking"); go.AddComponent<BoxCollider2D>().size = Vector2.one; }
            return sr;
        }
        private static void Label(string text, Vector2 position, Transform parent)
        {
            // No existing label is overwritten; this group is created once after a hierarchy check.
            GameObject go = new GameObject("Plate: " + text.Replace("\n", " / ")); go.transform.SetParent(parent, false); go.transform.position = position;
            TextMesh label = go.AddComponent<TextMesh>(); label.text = text; label.font = font; label.fontSize = 32; label.characterSize = 0.08f;
            label.anchor = TextAnchor.MiddleCenter; label.alignment = TextAlignment.Center; label.color = new Color(0.95f, 0.91f, 0.78f);
            MeshRenderer renderer = go.GetComponent<MeshRenderer>(); renderer.sharedMaterial = font.material; renderer.sortingOrder = 80;
        }
        private static MarshNode Node(string id, string name, MarshNodeKind kind, Vector2 position, int cost, Transform parent)
        {
            GameObject go = new GameObject(id); go.transform.SetParent(parent, false); go.transform.position = position;
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>(); collider.isTrigger = true; collider.size = Vector2.one;
            MarshNode node = go.AddComponent<MarshNode>(); Ref(node, "level", level); Number(node, "kind", (int)kind); Number(node, "cost", cost);
            TextValue(node, "nodeId", id); TextValue(node, "displayName", name);
            SpriteRenderer marker = Shape("Brass marker", position, new Vector2(0.2f, 0.2f), new Color(0.96f, 0.76f, 0.35f), go.transform, 45);
            Ref(node, "marker", marker); return node;
        }
        private static void Wire(Vector2 from, Vector2 to, Color color, Transform parent)
        {
            Shape("Mechanical link", (from + to) * 0.5f, new Vector2(Vector2.Distance(from, to), 0.055f), color, parent, 9).transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
        }

        [MenuItem("Tools/Duck Adventure/Build Iteration 1 Exchange")]
        public static void Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling) throw new InvalidOperationException("Build Iteration 1 in Edit Mode after compilation.");
            if (SceneManager.GetActiveScene().path != "Assets/_Agent/Scenes/Marshlight.unity") throw new InvalidOperationException("Open Marshlight first.");
            level = One<MarshLevel>(); One<Player>(); One<RPGGameManager>(); One<RPGCameraManager>(); One<Inventory>(); One<HealthBar>();
            if (Object.FindObjectsByType<MarshCoinRelay>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length != 0) throw new InvalidOperationException("An exchange already exists; refusing to duplicate it.");
            Transform world = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "Marshlight_World").transform;
            if (world.Find("09_ArchiveExchange") != null) throw new InvalidOperationException("Stale archive exchange hierarchy found.");
            square = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Agent/Placeholder/Square.png"); material = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Agent/Placeholder/MarshSprite.mat"); font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/slkscr.ttf");
            if (square == null || material == null || font == null) throw new InvalidOperationException("Existing placeholder art or font is missing.");
            Transform group = new GameObject("09_ArchiveExchange").transform; group.SetParent(world, false);
            MarshCoinRelay relay = level.gameObject.AddComponent<MarshCoinRelay>(); Ref(relay, "level", level); Ref(level, "coinRelay", relay);
            Color[] colors = { new Color(0.78f, 0.64f, 0.34f), new Color(0.45f, 0.77f, 0.64f), new Color(0.77f, 0.57f, 0.38f) };
            Vector2[] cups = { new Vector2(50, 4.5f), new Vector2(50, -4.5f), new Vector2(57, -5.8f) };
            Vector2[] trays = { new Vector2(54.8f, -5.8f), new Vector2(61.2f, -5.8f), new Vector2(64.2f, -3) };
            Vector2[] gates = { new Vector2(60, -5.5f), new Vector2(53, 5.5f), new Vector2(62.5f, -5.5f) };
            int[] costs = { 2, 1, 2 }; string[] ids = { "reed", "spill", "exit" };
            string[] names = { "the reed cup", "the spill cup", "the exit cup" };
            string[] labels = { "SOUTH ARM\n2 COINS", "NORTH ARM\n1 COIN", "EXIT LATCH\n2 AGAIN" };
            List<MarshNode> allNodes = level.Nodes.ToList();
            SerializedObject relaySo = new SerializedObject(relay); SerializedProperty loans = relaySo.FindProperty("loans"); loans.arraySize = 3;
            for (int i = 0; i < 3; i++)
            {
                MarshNode cup = Node("relay-" + ids[i] + "-cup", names[i], MarshNodeKind.RelayLoan, cups[i], costs[i], group);
                MarshNode tray = Node("relay-" + ids[i] + "-tray", "the " + ids[i] + " tray", MarshNodeKind.RelayReturn, trays[i], costs[i], group);
                SpriteRenderer gate = Shape("Latched " + ids[i] + " arm", gates[i], new Vector2(0.45f, 5), colors[i], group, 35, true);
                SpriteRenderer bowl = Shape("Counterweight cup", cups[i] + Vector2.up * 0.25f, new Vector2(0.65f, 0.46f), colors[i], cup.transform, 42);
                Shape("Cup base", cups[i], new Vector2(0.16f, 0.65f), new Color(0.35f, 0.29f, 0.21f), cup.transform, 36);
                for (int coin = 0; coin < costs[i]; coin++) Shape("Coin mark", cups[i] + new Vector2(-0.12f + coin * 0.24f, 0.28f), new Vector2(0.10f, 0.14f), new Color(0.08f, 0.15f, 0.16f), cup.transform, 43);
                Shape("Return tray", trays[i], new Vector2(0.65f, 0.42f), colors[i], tray.transform, 42);
                Label(labels[i], cups[i] + Vector2.up * 1.15f, cup.transform);
                Label(costs[i] + " RETURN", trays[i] + Vector2.up * 0.65f, tray.transform);
                SerializedProperty entry = loans.GetArrayElementAtIndex(i); entry.FindPropertyRelative("cup").objectReferenceValue = cup; entry.FindPropertyRelative("tray").objectReferenceValue = tray;
                entry.FindPropertyRelative("gate").objectReferenceValue = gate.gameObject; entry.FindPropertyRelative("indicator").objectReferenceValue = bowl;
                entry.FindPropertyRelative("requiresReturnedLoan").intValue = i == 2 ? 0 : -1;
                allNodes.Add(cup); allNodes.Add(tray);
                Vector2 elbow = new Vector2(gates[i].x - 0.7f, cups[i].y); Wire(cups[i], elbow, colors[i], group); Wire(elbow, gates[i], colors[i], group);
            }
            relaySo.ApplyModifiedPropertiesWithoutUndo();
            Shape("Archive latch reed bank", new Vector2(62.5f, 2.5f), new Vector2(0.8f, 11), new Color(0.12f, 0.24f, 0.20f), group, 30, true);
            for (float y = -2.6f; y < 8; y += 0.65f) Shape("Latch bank reed", new Vector2(62.5f, y), new Vector2(0.12f, 0.45f), new Color(0.48f, 0.50f, 0.31f), group, 31);
            MarshNode pocket = Node("archive-pocket", "the dry ledger page", MarshNodeKind.Note, new Vector2(57.8f, 6.8f), 0, group); TextValue(pocket, "storyId", "archive-pocket");
            Shape("Dry ledger paper", new Vector2(57.8f, 7.05f), new Vector2(0.6f, 0.45f), new Color(0.94f, 0.91f, 0.78f), pocket.transform, 42);
            Label("DRY LEDGER", new Vector2(57.8f, 7.55f), pocket.transform); allNodes.Add(pocket);
            SerializedObject levelSo = new SerializedObject(level); SerializedProperty nodes = levelSo.FindProperty("nodes"); nodes.arraySize = allNodes.Count;
            for (int i = 0; i < allNodes.Count; i++) nodes.GetArrayElementAtIndex(i).objectReferenceValue = allNodes[i];
            Consumable[] coins = Object.FindObjectsByType<Consumable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(c => c.item.itemType == Item.ItemType.COIN).ToArray();
            if (coins.Length != 3) throw new InvalidOperationException("Expected the original three coin pickups.");
            SerializedProperty pickups = levelSo.FindProperty("coinPickups"); pickups.arraySize = coins.Length; for (int i = 0; i < coins.Length; i++) pickups.GetArrayElementAtIndex(i).objectReferenceValue = coins[i];
            levelSo.ApplyModifiedPropertiesWithoutUndo();
            MarshPatrol lastPatrol = Object.FindObjectsByType<MarshPatrol>(FindObjectsSortMode.None).Single(p => p.name == "MarshMite_4");
            lastPatrol.transform.position = new Vector3(65, -4.5f, 0); SerializedObject patrolSo = new SerializedObject(lastPatrol);
            patrolSo.FindProperty("pointA").vector2Value = new Vector2(64.4f, -4.5f); patrolSo.FindProperty("pointB").vector2Value = new Vector2(65.6f, -4.5f); patrolSo.ApplyModifiedPropertiesWithoutUndo();
            MarshStory story = AssetDatabase.LoadAssetAtPath<MarshStory>("Assets/_Agent/Data/MarshStory.asset");
            if (story.Entries.Length != 9) throw new InvalidOperationException("Expected exactly nine baseline story entries before Iteration 1.");
            List<MarshStory.Entry> entries = story.Entries.ToList();
            entries.Add(new MarshStory.Entry { id = "exchange", speaker = "Ada's counterweight plate", pages = new[] {
                "Two coins in the reed cup lower the distant SOUTH arm. One in the spill cup lowers the nearer NORTH arm. Either cup can be filled first. The brass links show which weight moves which arm.",
                "Reclaim the reed pair from the middle tray, then lend that same pair to the EXIT cup. Its shutter opens only after the pair has been recovered. Take the spill coin and the exit pair with you. Every latch holds; every coin comes back." } });
            entries.Add(new MarshStory.Entry { id = "archive-pocket", speaker = "A dry page from the ledger", pages = new[] {
                "Four names, four dry blankets, four bowls of grain. Beneath Ada's careful count, a damp webbed footprint has become a fifth mark. Aiken smiles. Duckduckgoose had not been counting the flock. He had been making sure Ada had a place, too." } });
            string json = JsonUtility.ToJson(new StoryDocument { entries = entries.ToArray() }, true); JsonUtility.FromJsonOverwrite(json, story); EditorUtility.SetDirty(story); File.WriteAllText("Assets/_Agent/Data/Story.json", json);
            foreach (Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
            MarshBuilder.Validate();
            Debug.Log("Iteration 1 exchange built: three refundable split loans, seven new nodes, eleven story entries; movement and original crossings preserved.");
        }
    }
}
#endif

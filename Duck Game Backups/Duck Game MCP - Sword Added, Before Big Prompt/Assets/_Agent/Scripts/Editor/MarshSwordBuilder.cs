#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DuckAdventure.Agent.Editor
{
    public static class MarshSwordBuilder
    {
        private const string Root = "Assets/_Agent/";
        private const string Evidence = Root + "Data/SwordPass20261106/";
        [Serializable] private sealed class StoryDocument { public MarshStory.Entry[] entries; }
        [Serializable] private sealed class Audit { public int oldColliders; public int newPickupColliders; public bool oldCollidersUnchanged; public int props; public int stories; public int weapons; public int equipmentPanels; }

        private static T One<T>() where T : Component
        {
            T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(c => c.gameObject.scene == SceneManager.GetActiveScene()).ToArray();
            if (found.Length != 1) throw new InvalidOperationException("Expected one " + typeof(T).Name + ", found " + found.Length);
            return found[0];
        }
        private static void Ref(Object owner, string name, Object value)
        {
            SerializedObject so = new SerializedObject(owner); SerializedProperty p = so.FindProperty(name);
            if (p == null) throw new InvalidOperationException("Missing serialized field " + owner.name + "." + name);
            p.objectReferenceValue = value; so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(owner);
        }
        private static string Stamp(IEnumerable<Collider2D> colliders)
        {
            Physics2D.SyncTransforms();
            return string.Join("\n", colliders.OrderBy(c => c.GetInstanceID()).Select(c => c.GetInstanceID() + "|" + EditorJsonUtility.ToJson(c) + "|" + c.transform.localToWorldMatrix.ToString("R") + "|" + c.bounds.ToString("R")));
        }
        private static Sprite Import(string path, int width, int height)
        {
            if (!File.Exists(path)) throw new InvalidOperationException("Missing prepared pixel sprite: " + path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32; importer.filterMode = FilterMode.Point; importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false; importer.alphaIsTransparency = true; importer.npotScale = TextureImporterNPOTScale.None;
            TextureImporterSettings settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect; settings.spriteAlignment = (int)SpriteAlignment.Custom; settings.spritePivot = new Vector2(0.5f, 6f / height);
            importer.SetTextureSettings(settings); importer.SaveAndReimport();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null || sprite.rect.width != width || sprite.rect.height != height) throw new InvalidOperationException("Wrong logical sprite size: " + path);
            return sprite;
        }
        private static AudioClip MakeSwish()
        {
            string path = Root + "Placeholder/Weapons/ReedkeeperSwipe.wav";
            if (!File.Exists(path))
            {
                const int rate = 44100, samples = 6174;
                System.Random noise = new System.Random(20261106);
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + samples * 2); writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                    writer.Write(16); writer.Write((short)1); writer.Write((short)1); writer.Write(rate); writer.Write(rate * 2); writer.Write((short)2); writer.Write((short)16);
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(samples * 2);
                    float previous = 0;
                    for (int i = 0; i < samples; i++)
                    {
                        float t = i / (float)rate, p = i / (float)samples;
                        float raw = (float)noise.NextDouble() * 2 - 1;
                        float high = raw - previous * 0.72f; previous = raw;
                        float envelope = Mathf.Sin(Mathf.PI * p) * (1 - p);
                        float value = (high * 0.5f + Mathf.Sin(t * (1300 - p * 900) * Mathf.PI * 2) * 0.10f) * envelope * 0.35f;
                        writer.Write((short)Mathf.Clamp(value * short.MaxValue, short.MinValue, short.MaxValue));
                    }
                }
            }
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null) throw new InvalidOperationException("The synthesized sword swish failed to import.");
            return clip;
        }
        private static SpriteRenderer SpriteObject(string name, Sprite sprite, Transform parent, Vector3 localPosition, Vector3 localScale, Material material, int order)
        {
            GameObject go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.localPosition = localPosition; go.transform.localScale = localScale;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sprite; sr.sharedMaterial = material; sr.color = Color.white; sr.sortingOrder = order; return sr;
        }
        private static void RemoveVisual(SpriteRenderer sr)
        {
            if (sr.gameObject.GetComponents<Component>().Any(c => !(c is Transform) && !(c is SpriteRenderer)))
                throw new InvalidOperationException("Refusing to remove nonvisual prop component: " + sr.name);
            Object.DestroyImmediate(sr.gameObject);
        }
        private static RectTransform Ui(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            RectTransform rt = (RectTransform)new GameObject(name, typeof(RectTransform)).transform; rt.SetParent(parent, false);
            rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = pivot; rt.anchoredPosition = position; rt.sizeDelta = size; return rt;
        }
        private static Text Text(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, Font font)
        {
            Text text = Ui(name, parent, anchor, pivot, position, size).gameObject.AddComponent<Text>();
            text.font = font; text.fontSize = fontSize; text.color = new Color(0.94f, 0.91f, 0.78f); text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate; return text;
        }

        [MenuItem("Tools/Duck Adventure/Build Sword and Story Props")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling) throw new InvalidOperationException("Build requires Edit Mode after compilation.");
            if (SceneManager.GetActiveScene().path != Root + "Scenes/Marshlight.unity") throw new InvalidOperationException("Open Marshlight first.");
            MarshLevel level = One<MarshLevel>(); Player player = One<Player>(); MovementController movement = One<MovementController>();
            One<Inventory>(); One<HealthBar>(); One<RPGGameManager>(); One<RPGCameraManager>();
            if (Object.FindObjectsByType<Weapon>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length != 0 || Object.FindObjectsByType<MarshEquipmentHUD>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length != 0)
                throw new InvalidOperationException("Weapon/equipment already exists; refusing stale duplicate instances.");
            Transform world = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "Marshlight_World").transform;
            Transform actors = SceneManager.GetActiveScene().GetRootGameObjects().Single(g => g.name == "Actors").transform;
            GameObject hud = (GameObject)new SerializedObject(level).FindProperty("hudRoot").objectReferenceValue;
            if (world.Find("11_SwordAndProps") != null || hud.transform.Find("EquippedWeaponPanel") != null || actors.Find("ReedkeeperSwipe") != null)
                throw new InvalidOperationException("Stale sword hierarchy or equipment panel found.");
            Font font = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            Material material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Placeholder/MarshSprite.mat");
            Sprite square = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "Placeholder/Square.png");
            MarshStory story = AssetDatabase.LoadAssetAtPath<MarshStory>(Root + "Data/MarshStory.asset");
            if (font == null || material == null || square == null || story == null || story.Entries.Length != 11) throw new InvalidOperationException("Expected the complete eleven-entry outdoor pass before adding the sword.");
            if (level.Nodes.Count(n => n.Kind == MarshNodeKind.Note) != 5) throw new InvalidOperationException("Expected five existing note props.");
            Collider2D[] oldColliders = Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            string before = Stamp(oldColliders); Directory.CreateDirectory(Evidence); File.WriteAllText(Evidence + "CollidersBefore.txt", before);
            Directory.CreateDirectory(Root + "Placeholder/Props"); Directory.CreateDirectory(Root + "Placeholder/Weapons");
            Sprite mailbox = Import(Root + "Placeholder/Props/KeeperMailbox.png", 32, 48);
            Sprite sign = Import(Root + "Placeholder/Props/TrailSignpost.png", 32, 48);
            Sprite sword = Import(Root + "Placeholder/Weapons/ReedkeeperSword.png", 32, 32);
            // Returns use the complete posted mailbox; a head crop reads as buried.
            AudioClip swish = MakeSwish();
            Item item = AssetDatabase.LoadAssetAtPath<Item>(Root + "Data/ReedkeeperSwordItem.asset");
            if (item == null) { item = ScriptableObject.CreateInstance<Item>(); AssetDatabase.CreateAsset(item, Root + "Data/ReedkeeperSwordItem.asset"); }
            item.objectName = "Reedkeeper Sword"; item.itemType = Item.ItemType.SWORD; item.quantity = 1; item.stackable = false; item.sprite = sword; EditorUtility.SetDirty(item);
            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(Root + "Data/ReedkeeperSword.asset");
            if (data == null) { data = ScriptableObject.CreateInstance<WeaponData>(); AssetDatabase.CreateAsset(data, Root + "Data/ReedkeeperSword.asset"); }
            Ref(data, "inventoryItem", item); Ref(data, "sprite", sword); Ref(data, "sound", swish); Ref(data, "projectilePrefab", null);
            data.ValidateData();

            // Keep every node/trigger and its serialized marker identity; replace decorative children only.
            foreach (MarshNode node in level.Nodes.Where(n => n.Kind == MarshNodeKind.Note || n.Kind == MarshNodeKind.ReturnTray || n.Kind == MarshNodeKind.RelayReturn))
            {
                SpriteRenderer marker = (SpriteRenderer)new SerializedObject(node).FindProperty("marker").objectReferenceValue;
                if (marker == null) throw new InvalidOperationException("Missing existing prop marker: " + node.Id);
                foreach (SpriteRenderer sr in node.GetComponentsInChildren<SpriteRenderer>(true).Where(sr => sr != marker).ToArray()) RemoveVisual(sr);
                if (node.Kind == MarshNodeKind.Note)
                {
                    marker.sprite = node.Id == "keeper-note" ? mailbox : sign; marker.color = Color.white; marker.sortingOrder = 40;
                    marker.transform.localPosition = Vector3.zero; marker.transform.localScale = Vector3.one * 0.85f;
                    foreach (TextMesh label in node.GetComponentsInChildren<TextMesh>(true))
                    {
                        if (label.gameObject.GetComponents<Component>().Any(c => !(c is Transform) && !(c is TextMesh) && !(c is MeshRenderer)))
                            throw new InvalidOperationException("Unexpected logic on a story-prop label.");
                        Object.DestroyImmediate(label.gameObject);
                    }
                }
                else
                {
                    SpriteObject("Return mailbox art", mailbox, node.transform, Vector3.zero, Vector3.one * 0.85f, material, 40);
                    marker.sprite = level.CoinItem.sprite; marker.color = Color.white; marker.transform.localPosition = new Vector3(-0.07f, 0.72f, 0);
                    marker.transform.localScale = new Vector3(0.16f / marker.sprite.bounds.size.x, 0.16f / marker.sprite.bounds.size.y, 1);
                    foreach (TextMesh label in node.GetComponentsInChildren<TextMesh>(true)) label.transform.localPosition = new Vector3(0, 1.4f, 0);
                }
            }
            Transform group = new GameObject("11_SwordAndProps").transform; group.SetParent(world, false);

            GameObject visual = new GameObject("ReedkeeperSwipe");
            SpriteRenderer blade = SpriteObject("Blade", sword, visual.transform, new Vector3(0, 0.18f, 0), Vector3.one * 0.9f, material, 111); blade.enabled = false;
            LineRenderer arc = visual.AddComponent<LineRenderer>(); arc.sharedMaterial = material; arc.useWorldSpace = false; arc.widthMultiplier = 0.035f; arc.sortingOrder = 110; arc.positionCount = 0; arc.enabled = false;
            AudioSource audio = visual.AddComponent<AudioSource>(); audio.playOnAwake = false; audio.spatialBlend = 0; audio.volume = 1;
            GameObject visualPrefab = PrefabUtility.SaveAsPrefabAsset(visual, Root + "Prefabs/ReedkeeperSwipe.prefab"); Object.DestroyImmediate(visual);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(visualPrefab, actors); instance.name = "ReedkeeperSwipe";
            Weapon weapon = player.gameObject.AddComponent<Weapon>();
            Ref(weapon, "level", level); Ref(weapon, "player", player); Ref(weapon, "movement", movement); Ref(weapon, "body", player.GetComponent<Rigidbody2D>()); Ref(weapon, "data", data);
            Ref(weapon, "swingRoot", instance.transform); Ref(weapon, "blade", instance.transform.Find("Blade").GetComponent<SpriteRenderer>());
            Ref(weapon, "swipeArc", instance.GetComponent<LineRenderer>()); Ref(weapon, "swipeAudio", instance.GetComponent<AudioSource>()); Ref(level, "weapon", weapon);
            AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (listeners.Length > 1) throw new InvalidOperationException("Duplicate audio listeners found.");
            if (listeners.Length == 0) One<Camera>().gameObject.AddComponent<AudioListener>();

            GameObject pickup = new GameObject("ReedkeeperSwordPickup"); pickup.tag = "CanBePickedUp";
            SpriteRenderer pickupArt = pickup.AddComponent<SpriteRenderer>(); pickupArt.sprite = sword; pickupArt.sharedMaterial = material; pickupArt.sortingOrder = 75;
            CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>(); trigger.isTrigger = true; trigger.radius = 0.65f;
            pickup.AddComponent<Consumable>().item = item; pickup.transform.localScale = Vector3.one * 0.6f; pickup.transform.rotation = Quaternion.Euler(0, 0, -35);
            GameObject pickupPrefab = PrefabUtility.SaveAsPrefabAsset(pickup, Root + "Prefabs/ReedkeeperSwordPickup.prefab"); Object.DestroyImmediate(pickup);
            GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(pickupPrefab, group); placed.name = "ReedkeeperSwordPickup"; placed.transform.position = new Vector3(0.55f, 2.6f, 0);

            foreach (EnemyHitPoints enemy in Object.FindObjectsByType<EnemyHitPoints>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (enemy.GetComponent<MarshEnemyFeedback>() != null) throw new InvalidOperationException("Stale enemy combat feedback found.");
                MarshEnemyFeedback feedback = enemy.gameObject.AddComponent<MarshEnemyFeedback>();
                Ref(feedback, "health", enemy); Ref(feedback, "bodySprite", enemy.GetComponent<SpriteRenderer>()); Ref(feedback, "level", level);
                SpriteRenderer[] pips = new SpriteRenderer[enemy.MaxHitPoints];
                for (int i = 0; i < pips.Length; i++) pips[i] = SpriteObject("HP pip " + i, square, enemy.transform, new Vector3((i - (pips.Length - 1) * 0.5f) * 0.22f, 0.70f, 0), new Vector3(0.16f, 0.075f, 1), material, 113);
                SerializedObject feedbackSo = new SerializedObject(feedback); SerializedProperty array = feedbackSo.FindProperty("healthPips"); array.arraySize = pips.Length;
                for (int i = 0; i < pips.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = pips[i]; feedbackSo.ApplyModifiedPropertiesWithoutUndo();
            }
            RectTransform panel = Ui("EquippedWeaponPanel", hud.transform, Vector2.one, Vector2.one, new Vector2(-32, -98), new Vector2(220, 40));
            Image background = panel.gameObject.AddComponent<Image>(); background.color = new Color(0.04f, 0.10f, 0.13f, 0.95f); background.raycastTarget = false;
            Image icon = Ui("Equipped icon", panel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(6, 0), new Vector2(32, 32)).gameObject.AddComponent<Image>();
            icon.sprite = sword; icon.preserveAspect = true; icon.enabled = false; icon.raycastTarget = false;
            Text name = Text("Equipment name", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(46, -3), new Vector2(168, 17), 13, font);
            Text hint = Text("Equipment hint", panel, Vector2.zero, Vector2.zero, new Vector2(46, 3), new Vector2(168, 14), 12, font);
            MarshEquipmentHUD equipment = panel.gameObject.AddComponent<MarshEquipmentHUD>(); Ref(equipment, "weapon", weapon); Ref(equipment, "icon", icon); Ref(equipment, "nameText", name); Ref(equipment, "hintText", hint);

            List<MarshStory.Entry> entries = story.Entries.ToList();
            entries.Add(new MarshStory.Entry { id = "sword", speaker = "Ada's sword note", pages = new[] {
                "Aiken — my reedkeeper's sword is yours. A coin in its guard balances each swing, then comes straight back. J or left mouse swipes toward the way you last moved. Q equips the blade or packs it away.",
                "Marsh mites bruise the bank roots. Three clean strikes send one back into the mud. The bells still need their own weights: reclaim a coin before fighting if every coin is lent. Leave every crossing open for the next traveler." } });
            string json = JsonUtility.ToJson(new StoryDocument { entries = entries.ToArray() }, true); JsonUtility.FromJsonOverwrite(json, story); EditorUtility.SetDirty(story); File.WriteAllText(Root + "Data/Story.json", json);
            string after = Stamp(oldColliders); File.WriteAllText(Evidence + "CollidersAfter.txt", after);
            if (before != after) throw new InvalidOperationException("An existing collider changed while adding props or sword presentation.");
            Audit audit = new Audit { oldColliders = oldColliders.Length, newPickupColliders = 1, oldCollidersUnchanged = true, props = 11, stories = entries.Count, weapons = 1, equipmentPanels = 1 };
            File.WriteAllText(Evidence + "BuildAudit.json", JsonUtility.ToJson(audit, true));
            foreach (Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            MarshBuilder.Validate(); Debug.Log("Sword and pixel props built: eleven dressed props, twelve stories, one inventory-owned weapon/equipment panel, existing colliders unchanged.");
        }
    }
}
#endif


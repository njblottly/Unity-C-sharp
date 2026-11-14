#if UNITY_EDITOR
using System;
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
    public static class MarshPolish
    {
        private const string Root="Assets/_Agent/";
        private static T One<T>() where T:Component
        {
            T[] items=Object.FindObjectsByType<T>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(x=>x.gameObject.scene==SceneManager.GetActiveScene()).ToArray();
            if(items.Length!=1)throw new InvalidOperationException("Expected one "+typeof(T).Name+", found "+items.Length);
            return items[0];
        }
        private static void Ref(Object target,string name,Object reference)
        {
            SerializedObject so=new SerializedObject(target);so.FindProperty(name).objectReferenceValue=reference;so.ApplyModifiedPropertiesWithoutUndo();
        }
        private static RectTransform Named(Transform root,string path)
        {
            Transform item=root.Find(path);
            if(item==null)throw new InvalidOperationException("Missing UI path: "+path);
            return (RectTransform)item;
        }
        [MenuItem("Tools/Duck Adventure/Polish Marshlight")]
        public static void Apply()
        {
            if(EditorApplication.isPlaying || EditorApplication.isCompiling)throw new InvalidOperationException("Polish requires Edit Mode after compilation.");
            MarshLevel level=One<MarshLevel>();
            Inventory inventory=One<Inventory>();
            HealthBar health=One<HealthBar>();
            Transform hud=Named(health.transform,"MarshHUD");
            Canvas inventoryCanvas=inventory.GetComponent<Canvas>();inventoryCanvas.sortingOrder=30;inventoryCanvas.enabled=false;
            Ref(level,"hudRoot",hud.gameObject);Ref(level,"inventoryCanvas",inventoryCanvas);
            RectTransform top=Named(hud,"Top veil");top.anchorMin=new Vector2(0,1);top.anchorMax=Vector2.one;top.sizeDelta=new Vector2(0,140);
            RectTransform footer=Named(hud,"Footer veil");footer.anchorMin=Vector2.zero;footer.anchorMax=new Vector2(1,0);footer.sizeDelta=new Vector2(0,90);
            RectTransform objective=Named(hud,"Objective");objective.anchorMin=new Vector2(0,1);objective.anchorMax=Vector2.one;objective.pivot=new Vector2(0,1);objective.sizeDelta=new Vector2(-332,48);objective.anchoredPosition=new Vector2(32,-52);objective.GetComponent<Text>().fontSize=22;
            foreach(string path in new[]{"HP track","HP meter"})Named(hud,path).anchoredPosition=new Vector2(32,-116);
            Named(hud,"HP count").anchoredPosition=new Vector2(196,-110);
            RectTransform prompt=Named(hud,"Interaction");prompt.anchorMin=Vector2.zero;prompt.anchorMax=new Vector2(1,0);prompt.pivot=new Vector2(0.5f,0);prompt.sizeDelta=new Vector2(-320,52);prompt.anchoredPosition=new Vector2(140,18);prompt.GetComponent<Text>().fontSize=18;
            RectTransform toast=Named(hud,"Toast");toast.anchorMin=Vector2.zero;toast.anchorMax=new Vector2(1,0);toast.pivot=new Vector2(0.5f,0);toast.sizeDelta=new Vector2(-64,48);toast.anchoredPosition=new Vector2(0,105);toast.GetComponent<Text>().fontSize=20;
            Image menu=Named(health.transform,"Story menu").GetComponent<Image>();Color menuColor=menu.color;menuColor.a=1;menu.color=menuColor;
            hud.gameObject.SetActive(false);

            GameObject slot=PrefabUtility.LoadPrefabContents(Root+"Prefabs/MarshSlot.prefab");
            try
            {
                Slot script=slot.GetComponent<Slot>();
                RectTransform frame=Named(slot.transform,"Background");frame.anchorMin=Vector2.one*0.5f;frame.anchorMax=Vector2.one*0.5f;frame.pivot=Vector2.one*0.5f;frame.anchoredPosition=Vector2.zero;frame.sizeDelta=new Vector2(44,44);
                frame.GetComponent<Image>().color=new Color(0.17f,0.27f,0.28f);
                RectTransform badge=Named(slot.transform,"Background/Tray");badge.anchorMin=new Vector2(1,0);badge.anchorMax=new Vector2(1,0);badge.pivot=new Vector2(1,0);badge.anchoredPosition=Vector2.zero;badge.sizeDelta=new Vector2(18,16);badge.GetComponent<Image>().color=new Color(0.06f,0.12f,0.15f);
                RectTransform quantity=script.qtyText.rectTransform;quantity.anchorMin=Vector2.one*0.5f;quantity.anchorMax=Vector2.one*0.5f;quantity.pivot=Vector2.one*0.5f;quantity.anchoredPosition=Vector2.zero;quantity.sizeDelta=new Vector2(18,16);
                script.qtyText.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");script.qtyText.fontSize=13;script.qtyText.alignment=TextAnchor.MiddleCenter;script.qtyText.enabled=false;
                script.itemImage.preserveAspect=true;
                PrefabUtility.SaveAsPrefabAsset(slot,Root+"Prefabs/MarshSlot.prefab");
            }
            finally{PrefabUtility.UnloadPrefabContents(slot);}

            Item coin=AssetDatabase.LoadAssetAtPath<Item>(Root+"Data/BrassCoin.asset");
            if(coin==null)
            {
                Item original=AssetDatabase.LoadAssetAtPath<Item>("Assets/Scripts/ScriptableObjects/Coin.asset");
                Sprite source=Object.FindObjectsByType<Consumable>(FindObjectsSortMode.None).First(c=>c.item.itemType==Item.ItemType.COIN).GetComponent<SpriteRenderer>().sprite;
                Rect rect=source.rect;float side=Mathf.Min(16,Mathf.Min(rect.width,rect.height));
                Sprite icon=Sprite.Create(source.texture,new Rect(rect.center.x-side/2,rect.center.y-side/2,side,side),Vector2.one*0.5f,side);
                icon.name="Brass Coin Icon";AssetDatabase.CreateAsset(icon,Root+"Placeholder/BrassCoinIcon.asset");
                coin=Object.Instantiate(original);coin.sprite=icon;coin.quantity=1;coin.objectName="Borrowed Brass";AssetDatabase.CreateAsset(coin,Root+"Data/BrassCoin.asset");
            }
            Ref(level,"coinItem",coin);
            foreach(Consumable item in Object.FindObjectsByType<Consumable>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(item.item.itemType==Item.ItemType.COIN)item.item=coin;

            string storyPath=Root+"Data/Story.json";
            string json=File.ReadAllText(storyPath).Replace("stay clear of the coral-backed marsh mites.","avoid the marsh mites on the rust-colored patrol tracks.");
            File.WriteAllText(storyPath,json);
            MarshStory story=AssetDatabase.LoadAssetAtPath<MarshStory>(Root+"Data/MarshStory.asset");JsonUtility.FromJsonOverwrite(json,story);EditorUtility.SetDirty(story);
            foreach(Component component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(PrefabUtility.IsPartOfPrefabInstance(component))PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            MarshBuilder.Validate();
            Debug.Log("Marshlight polished: visible inventory, responsive HUD, clean modal screens, readable coin icon, final coin recovery required.");
        }
    }
}
#endif

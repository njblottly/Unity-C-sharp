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
    public static class MarshRemainingEnemyBuilder
    {
        private const string Folder="Assets/_Agent/Data/Campaign/Enemies/";
        private static EnemyData Definition(string id,MarshEnemyKind kind,int chapter,GameObject seed)
        {
            var d=MarshCampaignBuilder.Asset<EnemyData>(Folder+id+".asset");d.id=id;d.kind=kind;
            d.displayName=kind==MarshEnemyKind.Moth?"Wick Moth":"Counterweight Sprout";
            d.sprite=AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Enemies/EnemyIdle_1.png").OfType<Sprite>().OrderBy(s=>s.rect.x).First();
            d.health=kind==MarshEnemyKind.Moth?2:5;d.chases=false;d.attackRange=kind==MarshEnemyKind.Moth?1.8f:3.2f;d.detectionRange=4;
            d.moveSpeed=kind==MarshEnemyKind.Moth?(chapter==3?.8f:.85f):.45f;
            d.attackSpeed=kind==MarshEnemyKind.Moth?(chapter==5?1.65f:1.6f):0;
            d.telegraphSeconds=kind==MarshEnemyKind.Moth?(chapter==3?.8f:chapter==4?.85f:.9f):(chapter==4?1:1.1f);
            d.attackSeconds=kind==MarshEnemyKind.Moth?.45f:.12f;
            d.recoverySeconds=kind==MarshEnemyKind.Moth?(chapter==5?1.25f:1.2f):(chapter==4?1.25f:1.35f);
            if(kind==MarshEnemyKind.Sprout){d.projectilePrefab=seed;d.projectileSpeed=chapter==5?2.1f:2;d.projectileRange=2.5f;}
            d.ValidateData();EditorUtility.SetDirty(d);return d;
        }
        public static void Build()
        {
            if(EditorApplication.isPlaying||EditorApplication.isCompiling||SceneManager.GetActiveScene().path!=MarshCampaignBuilder.ScenePath)throw new InvalidOperationException("Campaign Edit Mode required.");
            var levels=Object.FindObjectsByType<MarshLevel>(FindObjectsInactive.Include,FindObjectsSortMode.None);if(levels.Length!=1||levels[0].Campaign==null)throw new InvalidOperationException("Expected existing campaign coordinator.");var level=levels[0];
            if(Object.FindObjectsByType<WickMoth>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length>0)throw new InvalidOperationException("M3 already built.");
            var moth=MarshEnemyBuilder.AdditionalPrefab(MarshEnemyKind.Moth);var sprout=MarshEnemyBuilder.AdditionalPrefab(MarshEnemyKind.Sprout);var seed=MarshEnemyBuilder.SeedPrefab();
            var group=SceneManager.GetActiveScene().GetRootGameObjects().Single(g=>g.name=="Marshlight_World").transform.Find("07_Encounters");
            foreach(string remove in new[]{"skitter-2","skitter-3","crab-3"})Object.DestroyImmediate(group.GetComponentsInChildren<EnemyAgent>(true).Single(a=>a.name==remove).gameObject);
            for(int i=2;i<5;i++)
            {
                var section=level.Campaign.Data.sections[i];var entries=section.encounters.Where(e=>!new[]{"skitter-2","skitter-3","crab-3"}.Contains(e.id)).ToList();
                var mpos=new[]{new Vector2(71.5f,3.2f),new Vector2(107,3.2f),new Vector2(142,2)}[i-2];
                var md=Definition("Moth_Ch"+(i+1),MarshEnemyKind.Moth,i+1,seed);entries.Add(new SectionData.Encounter{id="moth-"+i,data=md,position=mpos,pointA=mpos+Vector2.left*.65f,pointB=mpos+Vector2.right*.65f});
                if(i>=3){var pos=i==3?new Vector2(112,-2):new Vector2(146,-2);var sd=Definition("Sprout_Ch"+(i+1),MarshEnemyKind.Sprout,i+1,seed);entries.Add(new SectionData.Encounter{id="sprout-"+i,data=sd,position=pos,pointA=pos+Vector2.left*.35f,pointB=pos+Vector2.right*.35f});}
                section.encounters=entries.ToArray();section.ValidateData();EditorUtility.SetDirty(section);
                foreach(var e in entries.Where(e=>e.data.kind==MarshEnemyKind.Moth||e.data.kind==MarshEnemyKind.Sprout))MarshEnemyBuilder.Spawn(e.data,section,e,e.data.kind==MarshEnemyKind.Moth?moth:sprout,group,level);
            }
            foreach(var tuple in new[]{new[]{"Crab_Ch3",".55",".8","1.2"},new[]{"Crab_Ch5",".6",".9","1.2"},new[]{"Skitter_Ch5",".95",".6",".9"}})
            {
                var d=AssetDatabase.LoadAssetAtPath<EnemyData>(Folder+tuple[0]+".asset");d.moveSpeed=float.Parse(tuple[1],System.Globalization.CultureInfo.InvariantCulture);d.telegraphSeconds=float.Parse(tuple[2],System.Globalization.CultureInfo.InvariantCulture);d.recoverySeconds=float.Parse(tuple[3],System.Globalization.CultureInfo.InvariantCulture);d.ValidateData();EditorUtility.SetDirty(d);
            }
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("M3 saved: three Wick Moths/two Sprouts, section-data curve, bounded terrain-blocked seeds.");
        }
    }
}
#endif

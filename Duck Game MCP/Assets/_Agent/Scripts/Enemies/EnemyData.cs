using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public enum MarshEnemyKind { Skitter, Crab, LegacyMite, Moth, Sprout }
    [CreateAssetMenu(menuName="Duck Adventure/Enemy Data")]
    public sealed class EnemyData : ScriptableObject
    {
        public string id;
        public string displayName;
        public MarshEnemyKind kind;
        public int health=2;
        public int contactDamage=1;
        public int attackDamage=1;
        public float moveSpeed=.9f;
        public float attackSpeed=1.8f;
        public float detectionRange=3.5f;
        public bool chases=true;
        public float leashDistance=2;
        public float hitReactionCooldown=1.25f;
        public float attackRange=1.35f;
        public float attackReach=.7f;
        public float attackArcDegrees=150;
        public float guardArcDegrees=150;
        public float telegraphSeconds=.5f;
        public float attackSeconds=.35f;
        public float recoverySeconds=.85f;
        public float hitRecoverySeconds=.2f;
        public float knockbackSpeed=1.75f;
        public Sprite sprite;
        public GameObject projectilePrefab;
        public float projectileSpeed=2, projectileRange=2.5f;
        public void ValidateData()
        {
            if(string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(displayName)||health<1||health>8||contactDamage<0||attackDamage<1||moveSpeed<=0||attackSpeed<0||leashDistance<=0||hitReactionCooldown<hitRecoverySeconds||detectionRange<attackRange||attackRange<attackReach||attackArcDegrees<=0||attackArcDegrees>180||guardArcDegrees<=0||guardArcDegrees>180||attackReach<=0||telegraphSeconds<.35f||attackSeconds<=0||recoverySeconds<.3f||hitRecoverySeconds<=0||knockbackSpeed<0||sprite==null)
                throw new InvalidOperationException("Invalid enemy definition: "+name);
            if(kind==MarshEnemyKind.Sprout&&(projectilePrefab==null||projectilePrefab.GetComponent<MarshSeedProjectile>()==null||projectileSpeed<=0||projectileRange<=0))throw new InvalidOperationException("Sprout requires a valid seed definition: "+name);
        }
    }
}

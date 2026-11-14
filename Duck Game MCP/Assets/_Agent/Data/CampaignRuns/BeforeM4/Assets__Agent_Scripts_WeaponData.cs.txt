using System;
using UnityEngine;

namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName = "Duck Adventure/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
        [SerializeField] private string displayName = "Reedkeeper Sword";
        [SerializeField] private Item inventoryItem;
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 0.50f;
        [SerializeField] private float range = 1.15f;
        [SerializeField] private float arcDegrees = 100f;
        [SerializeField] private float swipeDuration = 0.20f;
        [SerializeField] private int coinCost = 1;
        [SerializeField] private Sprite sprite;
        [SerializeField] private AudioClip sound;
        [SerializeField, Range(0, 1)] private float soundVolume = 0.25f;
        [Tooltip("Null for this melee weapon. Reserved for a future projectile implementation.")]
        [SerializeField] private GameObject projectilePrefab;

        public string DisplayName => displayName;
        public Item InventoryItem => inventoryItem;
        public int Damage => damage;
        public float Cooldown => cooldown;
        public float Range => range;
        public float ArcDegrees => arcDegrees;
        public float SwipeDuration => swipeDuration;
        public int CoinCost => coinCost;
        public Sprite Sprite => sprite;
        public AudioClip Sound => sound;
        public float SoundVolume => soundVolume;
        public GameObject ProjectilePrefab => projectilePrefab;

        public void ValidateData()
        {
            if (string.IsNullOrWhiteSpace(displayName) || inventoryItem == null || inventoryItem.itemType != Item.ItemType.SWORD ||
                damage <= 0 || range <= 0 || swipeDuration <= 0 || cooldown < swipeDuration || coinCost <= 0 ||
                arcDegrees <= 0 || arcDegrees > 180 || sprite == null || sound == null)
                throw new InvalidOperationException("Reedkeeper weapon data has missing references or invalid combat values.");
            if (projectilePrefab != null) throw new InvalidOperationException("The Reedkeeper Sword is melee; its projectile prefab must be empty.");
        }
    }
}

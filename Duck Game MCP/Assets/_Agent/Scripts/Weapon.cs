using System;
using System.Collections.Generic;
using UnityEngine;

namespace DuckAdventure.Agent
{
    /// <summary>Inventory-owned melee weapon. Its temporary coin loan is part of the level's conserved economy.</summary>
    [DefaultExecutionOrder(50)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Player), typeof(MovementController))]
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField] private MarshLevel level;
        [SerializeField] private Player player;
        [SerializeField] private MovementController movement;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private WeaponData data;
        [SerializeField] private Transform swingRoot;
        [SerializeField] private SpriteRenderer blade;
        [SerializeField] private LineRenderer swipeArc;
        [SerializeField] private AudioSource swipeAudio;

        private Inventory inventory;
        private int blockingMask;
        private bool initialized;
        private bool refundBlockedNotified;
        private float swipeStarted;
        private float nextAttackTime;
        private Vector2 direction;
        private readonly Collider2D[] contacts = new Collider2D[64];
        private readonly HashSet<EnemyHitPoints> hitThisSwipe = new HashSet<EnemyHitPoints>();

        public WeaponData Data => data;
        public bool IsReady => initialized;
        public bool HasSword => initialized && inventory.Count(Item.ItemType.SWORD) > 0;
        public bool IsEquipped { get; private set; }
        public bool IsSwiping { get; private set; }
        public int BorrowedCoins { get; private set; }
        public bool ModelVisible => initialized && blade.enabled;
        public float CooldownRemaining => Mathf.Max(0, nextAttackTime - Time.time);
        public float SwipeProgress => IsSwiping ? Mathf.Clamp01((Time.time - swipeStarted) / data.SwipeDuration) : 0;

        private void Start()
        {
            if (level == null || player == null || movement == null || body == null || data == null || swingRoot == null || blade == null || swipeArc == null || swipeAudio == null)
                throw new InvalidOperationException("Weapon requires explicit level, actor, data, visual and audio references.");
            if (level.Weapon != this) throw new InvalidOperationException("MarshLevel must reference this player Weapon; remove stale duplicates.");
            data.ValidateData();
            blockingMask = LayerMask.GetMask("Blocking");
            if (blockingMask == 0) throw new InvalidOperationException("Sword wall protection requires the existing Blocking layer.");
            inventory = player.PlayerInventory;
            if (inventory == null) throw new InvalidOperationException("Player inventory must initialize before Weapon (execution order 50).");
            blade.sprite = data.Sprite;
            HideVisual();
            player.ItemCollected += OnItemCollected;
            player.Died += OnPlayerDied;
            initialized = true;
            if (HasSword) Equip();
        }

        private void Update()
        {
            if (!level.CanExplore)
            {
                if (IsSwiping) InterruptSwing();
                return;
            }
            if (IsEquipped && !HasSword) Unequip();
            if (!IsSwiping && BorrowedCoins > 0) TryRefund();
            if (Input.GetKeyDown(KeyCode.Q)) { if (IsEquipped) Unequip(); else Equip(); }
            if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) TryAttack(movement.LastMoveDir);
            if (!IsSwiping) return;
            float progress = SwipeProgress;
            if (progress >= 1) { IsSwiping = false; HideVisual(); TryRefund(); return; }
            UpdateVisual(progress);
            SampleHits();
        }

        private void OnItemCollected(Item item)
        {
            if (item.itemType != Item.ItemType.SWORD) return;
            if (item != data.InventoryItem) throw new InvalidOperationException("This sword pickup does not match the equipped WeaponData inventory item.");
            Equip();
            if (!level.HasRead("sword")) level.ShowStory("sword");
        }
        public bool Equip()
        {
            if (!initialized) return false;
            if (!HasSword) { level.Notify("Ada's sword waits beside the first trail, near the letterbox."); return false; }
            IsEquipped = true;
            level.Notify("Reedkeeper Sword equipped. J / left click swipes; Q packs it away. Each swipe borrows one coin.");
            return true;
        }
        public bool Unequip()
        {
            if (!initialized) return false;
            IsEquipped = false;
            InterruptSwing();
            if (HasSword) level.Notify("Sword packed away. Press Q to equip it again.");
            return true;
        }
        public bool TryAttack(Vector2 facing)
        {
            if (!initialized || !level.CanExplore || !IsEquipped || !HasSword || IsSwiping || Time.time < nextAttackTime) return false;
            if (BorrowedCoins > 0 && !TryRefund()) return false;
            if (facing.sqrMagnitude < 0.0001f) return false;
            if (!inventory.TrySpend(Item.ItemType.COIN, data.CoinCost))
            {
                level.Notify("Reclaim a coin before swiping. The sword borrows one carried coin and returns it immediately afterward.");
                return false;
            }
            BorrowedCoins = data.CoinCost;
            IsSwiping = true;
            swipeStarted = Time.time;
            nextAttackTime = Time.time + data.Cooldown;
            direction = facing.normalized;
            hitThisSwipe.Clear();
            level.RecordCoinAction("sword_lend", BorrowedCoins, "reedkeeper-sword");
            UpdateVisual(0);
            swipeAudio.PlayOneShot(data.Sound, data.SoundVolume);
            SampleHits();
            return true;
        }

        private void SampleHits()
        {
            Vector2 origin = body.position;
            ContactFilter2D filter = new ContactFilter2D { useTriggers = true, useDepth = false };
            filter.SetLayerMask(Physics2D.AllLayers);
            int count = Physics2D.OverlapCircle(origin, data.Range, filter, contacts);
            if (count == contacts.Length) throw new InvalidOperationException("Sword overlap buffer exhausted; increase its configured capacity rather than silently missing enemies.");
            float threshold = Mathf.Cos(data.ArcDegrees * 0.5f * Mathf.Deg2Rad);
            for (int i = 0; i < count; i++)
            {
                Collider2D contact = contacts[i];
                EnemyHitPoints enemy = contact.GetComponentInParent<EnemyHitPoints>();
                if (enemy == null)
                {
                    if (contact.CompareTag("Enemy")) throw new InvalidOperationException("An Enemy-tagged collider has no EnemyHitPoints: " + contact.name);
                    continue; // Non-enemy terrain, notes and pickups are expected query results.
                }
                if (enemy.IsDead || hitThisSwipe.Contains(enemy)) continue;
                Vector2 offset = (Vector2)contact.bounds.center - origin;
                if (offset.sqrMagnitude > 0.0001f && Vector2.Dot(direction, offset.normalized) < threshold) continue;
                if (Physics2D.Linecast(origin, contact.ClosestPoint(origin), blockingMask).collider != null) continue;
                MarshEnemyFeedback feedback = enemy.GetComponent<MarshEnemyFeedback>();
                if (feedback == null) throw new InvalidOperationException("A sword target needs its serialized MarshEnemyFeedback: " + enemy.name);
                hitThisSwipe.Add(enemy);
                EnemyAgent agent=enemy.GetComponent<EnemyAgent>();
                if(agent==null)enemy.TakeDamage(data.Damage);
                else agent.TryReceiveWeaponHit(data.Damage,origin);
            }
        }

        private void UpdateVisual(float progress)
        {
            swingRoot.position = player.transform.position;
            swingRoot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            blade.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-data.ArcDegrees * 0.5f, data.ArcDegrees * 0.5f, progress));
            blade.enabled = true;
            swipeArc.enabled = true;
            swipeArc.positionCount = 15;
            for (int i = 0; i < swipeArc.positionCount; i++)
            {
                float angle = Mathf.Lerp(-data.ArcDegrees * 0.5f, data.ArcDegrees * 0.5f, i / 14f) * Mathf.Deg2Rad;
                swipeArc.SetPosition(i, new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * data.Range * 0.78f);
            }
            Color color = new Color(0.96f, 0.83f, 0.47f, Mathf.Lerp(0.55f, 0.08f, progress));
            swipeArc.startColor = color; swipeArc.endColor = color;
        }
        private void HideVisual()
        {
            blade.enabled = false;
            swipeArc.enabled = false;
            swipeAudio.Stop();
        }
        public bool InterruptSwing()
        {
            if (!initialized) return false; // Explicit lifecycle state, before the validated Start callback.
            IsSwiping = false;
            HideVisual();
            return TryRefund();
        }
        private bool TryRefund()
        {
            if (BorrowedCoins == 0) return true;
            if (!inventory.TryAddItems(level.CoinItem, BorrowedCoins))
            {
                if (!refundBlockedNotified)
                {
                    level.Notify("The sword still holds its coin. Make inventory space; it will return the coin before another swipe.");
                    refundBlockedNotified = true;
                }
                return false;
            }
            int returned = BorrowedCoins;
            BorrowedCoins = 0;
            refundBlockedNotified = false;
            level.RecordCoinAction("sword_reclaim", returned, "reedkeeper-sword");
            return true;
        }
        private void OnPlayerDied() { InterruptSwing(); }
        private void OnDisable()
        {
            // Player death disables this existing actor after Died has fired. Scene unload resets the whole run.
            if (initialized && player.IsDead) InterruptSwing();
        }
        private void OnDestroy()
        {
            if (!initialized) return;
            player.ItemCollected -= OnItemCollected;
            player.Died -= OnPlayerDied;
        }
    }
}

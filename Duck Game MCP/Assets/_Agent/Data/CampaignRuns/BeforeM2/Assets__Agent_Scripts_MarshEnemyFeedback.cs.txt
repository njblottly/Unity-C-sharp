using System;
using UnityEngine;

namespace DuckAdventure.Agent
{
    /// <summary>Presentation only; EnemyHitPoints retains ownership of damage, death and reset.</summary>
    [DisallowMultipleComponent]
    public sealed class MarshEnemyFeedback : MonoBehaviour
    {
        [SerializeField] private EnemyHitPoints health;
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private SpriteRenderer[] healthPips;
        [SerializeField] private MarshLevel level;
        private Color restingColor;
        private float flashUntil;
        public bool IsFlashing => Time.time < flashUntil;
        public int ShownHitPoints => health.CurrentHitPoints;
        private void Awake()
        {
            if (health == null || bodySprite == null || level == null || healthPips == null || healthPips.Length != health.MaxHitPoints)
                throw new InvalidOperationException("Enemy feedback needs its health, body, level and one serialized pip per HP.");
            foreach (SpriteRenderer pip in healthPips) if (pip == null) throw new InvalidOperationException("Missing enemy HP pip reference.");
            restingColor = bodySprite.color;
        }
        public void ShowHit()
        {
            flashUntil = Time.time + 0.12f;
            Refresh();
            level.Pulse(transform.position, health.IsDead ? new Color(0.76f, 0.87f, 0.59f) : new Color(1f, 0.82f, 0.43f));
        }
        private void LateUpdate() { Refresh(); }
        private void Refresh()
        {
            bodySprite.color = IsFlashing ? Color.white : restingColor;
            for (int i = 0; i < healthPips.Length; i++)
                healthPips[i].color = i < health.CurrentHitPoints ? new Color(0.97f, 0.76f, 0.35f) : new Color(0.13f, 0.21f, 0.23f);
        }
    }
}

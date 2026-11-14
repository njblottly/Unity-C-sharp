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
        [SerializeField] private bool scalablePips;
        private MaterialPropertyBlock properties;
        private bool hitShader;
        private Color restingColor;
        private float flashUntil;
        public bool IsFlashing => Time.time < flashUntil;
        public int ShownHitPoints => health.CurrentHitPoints;
        private void Awake()
        {
            if (health == null || bodySprite == null || level == null || healthPips == null || (scalablePips ? healthPips.Length < health.MaxHitPoints : healthPips.Length != health.MaxHitPoints))
                throw new InvalidOperationException("Enemy feedback needs its health, body, level and one serialized pip per HP.");
            foreach (SpriteRenderer pip in healthPips) if (pip == null) throw new InvalidOperationException("Missing enemy HP pip reference.");
            restingColor = bodySprite.color;
            hitShader=bodySprite.sharedMaterial.HasProperty("_HitFlash");properties=new MaterialPropertyBlock();
            if(scalablePips&&!hitShader)throw new InvalidOperationException("Campaign feedback requires its hit-flash material.");
            health.Damaged+=OnDamage;
        }
        public void ShowHit()
        {
            flashUntil = Time.time + 0.12f;
            Refresh();
            level.Pulse(transform.position, health.CurrentHitPoints == 0 ? new Color(0.76f, 0.87f, 0.59f) : new Color(1f, 0.82f, 0.43f));
        }
        private void OnDamage(int amount){ShowHit();}
        public void ShowGuard(){level.Pulse(transform.position,new Color(1,.82f,.25f));}
        public void ResetFeedback(){flashUntil=0;Refresh();}
        private void OnDestroy(){if(health!=null)health.Damaged-=OnDamage;}
        private void LateUpdate() { Refresh(); }
        private void Refresh()
        {
            if(hitShader){bodySprite.GetPropertyBlock(properties);properties.SetFloat("_HitFlash",IsFlashing?1:0);bodySprite.SetPropertyBlock(properties);}
            bodySprite.color = IsFlashing && !hitShader ? new Color(restingColor.r*.35f,restingColor.g*.35f,restingColor.b*.35f,restingColor.a) : restingColor;
            for (int i = 0; i < healthPips.Length; i++)
            {
                if(scalablePips){healthPips[i].enabled=i<health.MaxHitPoints;var pos=healthPips[i].transform.localPosition;pos.x=(i-(health.MaxHitPoints-1)*.5f)*.13f;healthPips[i].transform.localPosition=pos;}
                healthPips[i].color = i < health.CurrentHitPoints ? new Color(0.97f, 0.76f, 0.35f) : new Color(0.13f, 0.21f, 0.23f);
            }
        }
    }
}

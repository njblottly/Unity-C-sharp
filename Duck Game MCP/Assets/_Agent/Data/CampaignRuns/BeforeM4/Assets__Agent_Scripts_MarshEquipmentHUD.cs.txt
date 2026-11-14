using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckAdventure.Agent
{
    [DisallowMultipleComponent]
    public sealed class MarshEquipmentHUD : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text hintText;
        public Weapon Weapon => weapon;
        private void Awake()
        {
            if (weapon == null || icon == null || nameText == null || hintText == null)
                throw new InvalidOperationException("Equipment HUD requires explicit weapon, icon and text references.");
        }
        private void Update() { Refresh(); }
        public void Refresh()
        {
            if (!weapon.IsReady) { icon.enabled = false; nameText.text = "EQUIPPED HAND"; hintText.text = "Preparing equipment"; return; }
            icon.sprite = weapon.Data.Sprite;
            icon.enabled = weapon.HasSword;
            icon.color = weapon.IsEquipped ? Color.white : new Color(1, 1, 1, 0.35f);
            nameText.text = weapon.HasSword ? weapon.Data.DisplayName : "EMPTY HAND";
            nameText.color = weapon.IsEquipped ? new Color(0.97f, 0.80f, 0.40f) : new Color(0.70f, 0.75f, 0.73f);
            if (!weapon.HasSword) hintText.text = "Find Ada's sword";
            else if (weapon.BorrowedCoins > 0) hintText.text = weapon.IsSwiping ? "1 coin in the guard" : "Coin held: make space";
            else hintText.text = weapon.IsEquipped ? "J / CLICK SWIPE   Q PACK" : "Q EQUIP SWORD";
        }
    }
}

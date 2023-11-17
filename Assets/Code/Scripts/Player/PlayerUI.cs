using Airhead.Runtime.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Airhead.Runtime.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerUI : MonoBehaviour
    {
        public float weaponFillFadeoutTime = 0.4f;
        
        private PlayerWeaponManager weaponManager;
        private CanvasGroup weaponGroup;
        private Image weaponFill;
        private float weaponFillMaxAlpha;
        private TMP_Text weaponName;
        private TMP_Text weaponAmmo;

        private PlayerHealth health;
        private TMP_Text damageTaken;

        private void Awake()
        {
            health = GetComponentInParent<PlayerHealth>();
            damageTaken = transform.Find<TMP_Text>("PlayerInfo/DamageTaken");
            
            weaponManager = GetComponentInParent<PlayerWeaponManager>();
            weaponGroup = transform.Find<CanvasGroup>("WeaponInfo");
            weaponFill = weaponGroup.transform.Find<Image>("Fill");
            weaponName = weaponGroup.transform.Find<TMP_Text>("Name");
            weaponAmmo = weaponGroup.transform.Find<TMP_Text>("Ammo");

            weaponFillMaxAlpha = weaponFill.color.a;
        }

        private void Update()
        {
            var weapon = weaponManager ? weaponManager.CurrentWeapon : null;
            if (weapon)
            {
                weaponGroup.Show();
                weaponName.text = weapon.DisplayName.ToUpper();
                weaponAmmo.text = weapon.AmmoString.ToUpper();

                if (weapon.IsReloading)
                {
                    weaponFill.fillAmount = weapon.IsReloading ? weapon.ReloadPercent : 0.0f;
                    weaponFill.color = Color.white.Alpha(weaponFillMaxAlpha);
                }
                else
                {
                    var alpha = weaponFill.color.a;
                    alpha -= alpha * Time.deltaTime * 2.0f / weaponFillFadeoutTime; 
                    weaponFill.color = Color.white.Alpha(alpha);
                }
            }
            else weaponGroup.Hide();

            damageTaken.text = (health ? health.damageTaken : 0).ToString().PadLeft(3, '0');
        }
    }
}
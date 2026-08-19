using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Gameplay.Weapon;

namespace Game.UI.HUD
{
    public class WeaponHUD : MonoBehaviour
    {
        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private Image cooldownOverlay; // Image Type = Filled, Radial 360 — phủ mờ dần khi sẵn sàng
        [SerializeField] private TMP_Text ammoText;

        private void Update()
        {
            if (weaponHandler == null || weaponHandler.CurrentWeapon == null) return;

            if (weaponIcon != null)
                weaponIcon.sprite = weaponHandler.CurrentWeapon.icon;

            if (cooldownOverlay != null)
            {
                // CooldownRatio01: 0 = vừa bắn (che kín), 1 = sẵn sàng (không che) -> overlay fillAmount ngược lại
                cooldownOverlay.fillAmount = 1f - weaponHandler.CooldownRatio01;
            }

            if (ammoText != null)
            {
                if (weaponHandler.HasUnlimitedAmmo)
                    ammoText.text = "∞";
                else if (weaponHandler.IsReloading)
                    ammoText.text = "Reloading...";
                else
                    ammoText.text = $"{weaponHandler.CurrentAmmo}/{weaponHandler.MagazineSize}";
            }
        }
    }
}

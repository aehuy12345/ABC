using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay.Player;

namespace Game.UI.HUD
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Image fillImage; // Image Type = Filled, Fill Method = Horizontal
        [SerializeField] private Text hpText;      // hiển thị "80/100", để trống nếu không cần

        private void Update()
        {
            if (playerStats == null || fillImage == null) return;

            float ratio = playerStats.MaxHP > 0f ? playerStats.CurrentHP / playerStats.MaxHP : 0f;
            fillImage.fillAmount = ratio;

            if (hpText != null)
                hpText.text = $"{Mathf.CeilToInt(playerStats.CurrentHP)}/{Mathf.CeilToInt(playerStats.MaxHP)}";
        }
    }
}

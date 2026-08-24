using UnityEngine;
using TMPro;
using Game.Data;

namespace Game.UI.CharacterSelect
{
    /// <summary>
    /// Đặt panel này ở bên trái màn hình (Canvas Screen Space). CharacterSelectController
    /// gọi ShowStats() mỗi khi có nhân vật được chọn, Hide() lúc chưa chọn ai.
    /// </summary>
    public class CharacterStatsPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot; // toàn bộ khung panel, bật/tắt cả cụm
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text atkText;
        [SerializeField] private TMP_Text defText;
        [SerializeField] private TMP_Text speedText;

        private void Awake()
        {
            Hide();
        }

        public void ShowStats(CharacterClassSO data)
        {
            if (data == null) return;

            if (panelRoot != null) panelRoot.SetActive(true);

            if (nameText != null) nameText.text = data.displayName;
            if (roleText != null) roleText.text = data.role.ToString();
            if (descriptionText != null) descriptionText.text = data.description;
            if (hpText != null) hpText.text = $"HP: {data.baseHP:0}";
            if (atkText != null) atkText.text = $"ATK: {data.baseATK:0}";
            if (defText != null) defText.text = $"DEF: {data.baseDEF:0}";
            if (speedText != null) speedText.text = $"Tốc độ: {data.moveSpeed:0.#}";
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }
    }
}

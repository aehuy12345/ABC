using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using Game.Core;
using Game.Patterns.Observer;

namespace Game.UI.CharacterSelect
{
    public class CharacterSelectController : MonoBehaviour
    {
        [Header("Cinemachine")]
        [SerializeField] private CinemachineCamera overviewCamera; // vcam mặc định nhìn toàn cảnh
        [SerializeField] private int selectedPriority = 20;
        [SerializeField] private int defaultPriority = 10;

        [Header("UI")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text selectedNameLabel;
        [SerializeField] private CharacterStatsPanelUI statsPanel;

        [Header("Events (Observer pattern - tuỳ chọn)")]
        [SerializeField] private GameEventSO onCharacterConfirmedEvent;

        [Header("Portal")]
        [SerializeField] private Game.Gameplay.World.Portal portal;

        [Header("Character slots (kéo-thả tất cả nhân vật trong scene vào đây)")]
        [SerializeField] private CharacterSelectSlot[] allSlots;

        private CharacterSelectSlot _currentSlot;

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

            // Không cần tự SetActive(false) cho confirmButton nữa — nó giờ là CON của StatsPanel
            // (kéo Btn_Confirm vào làm con của StatsPanel trong Hierarchy), nên statsPanel.Hide()
            // tự động ẩn luôn cả nút theo, không cần code quản lý riêng 2 chỗ.
            if (selectedNameLabel != null) selectedNameLabel.gameObject.SetActive(false);
            statsPanel?.Hide();

            if (portal != null) portal.SetActive(false); // cổng chỉ mở sau khi confirm
        }

        public void SelectCharacter(CharacterSelectSlot slot)
        {
            _currentSlot = slot;

            // Hạ priority tất cả vcam nhân vật khác, chỉ cái được chọn có priority cao nhất
            foreach (var s in allSlots)
            {
                if (s.VirtualCamera == null) continue;
                SetPriority(s.VirtualCamera, (s == slot) ? selectedPriority : defaultPriority);
            }

            if (overviewCamera != null) SetPriority(overviewCamera, defaultPriority);

            if (selectedNameLabel != null)
            {
                selectedNameLabel.gameObject.SetActive(true);
                selectedNameLabel.text = slot.CharacterData.displayName;
            }

            // ShowStats() bật StatsPanel lên -> Btn_Confirm (con của panel) tự hiện theo cùng lúc
            statsPanel?.ShowStats(slot.CharacterData);
        }

        /// <summary>
        /// Cinemachine 3.x đổi Priority từ int sang struct PrioritySettings (có field Value + Enabled).
        /// Hàm này thay cho việc gán "vcam.Priority = x" như CM2.x cũ.
        /// </summary>
        private void SetPriority(CinemachineCamera vcam, int value)
        {
            var settings = vcam.Priority;
            settings.Value = value;
            vcam.Priority = settings;
        }

        private void OnConfirmClicked()
        {
            if (_currentSlot == null) return;

            GameSession.Instance.SelectCharacter(_currentSlot.CharacterData);

            onCharacterConfirmedEvent?.Raise();

            if (portal != null) portal.SetActive(true);
        }

        /// <summary>
        /// Gọi từ nút Back/Thoát trong StatsPanel — huỷ chọn nhân vật hiện tại,
        /// hạ hết vcam nhân vật về mặc định, đưa CM_Overview lên lại làm camera chính.
        /// </summary>
        private void OnBackClicked()
        {
            _currentSlot = null;

            foreach (var s in allSlots)
            {
                if (s.VirtualCamera == null) continue;
                SetPriority(s.VirtualCamera, defaultPriority);
            }

            // Đưa overview lên cao hơn mọi vcam nhân vật (đang ở defaultPriority) để nó được hiển thị lại
            if (overviewCamera != null) SetPriority(overviewCamera, selectedPriority);

            if (selectedNameLabel != null) selectedNameLabel.gameObject.SetActive(false);
            statsPanel?.Hide(); // ẩn panel -> Btn_Confirm (con của panel) tự ẩn theo
        }
    }
}

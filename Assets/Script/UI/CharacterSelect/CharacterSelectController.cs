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
        [Tooltip("Camera nhìn toàn cảnh 4 nhân vật, bật mặc định lúc vào scene, tắt khi có nhân vật được chọn.")]
        [SerializeField] private CinemachineCamera overviewCamera;

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

            if (selectedNameLabel != null) selectedNameLabel.gameObject.SetActive(false);
            statsPanel?.Hide();

            if (portal != null) portal.SetActive(false); // cổng chỉ mở sau khi confirm

            // Đảm bảo trạng thái ban đầu đúng: Overview bật, mọi camera nhân vật tắt
            // (CharacterSelectSlot.Awake() của từng nhân vật cũng tự tắt camera của nó,
            // đây chỉ là bước đảm bảo thêm ở cấp controller).
            if (overviewCamera != null) overviewCamera.gameObject.SetActive(true);
            foreach (var s in allSlots) s.DisableAllCameras();
        }

        /// <summary>Gọi từ CharacterSelectSlot.OnMouseDown() khi Player click vào 1 nhân vật.</summary>
        public void SelectCharacter(CharacterSelectSlot slot)
        {
            // Tắt camera của nhân vật đang chọn trước đó (nếu có), tránh 2 camera cùng bật 1 lúc
            if (_currentSlot != null && _currentSlot != slot)
                _currentSlot.DisableAllCameras();

            _currentSlot = slot;

            if (overviewCamera != null) overviewCamera.gameObject.SetActive(false);

            // Bật đúng Zoom Camera của nhân vật vừa chọn (Confirm Camera của nó vẫn tắt cho tới khi bấm Confirm)
            if (slot.ZoomCamera != null) slot.ZoomCamera.gameObject.SetActive(true);
            if (slot.ConfirmCamera != null) slot.ConfirmCamera.gameObject.SetActive(false);

            if (selectedNameLabel != null)
            {
                selectedNameLabel.gameObject.SetActive(true);
                selectedNameLabel.text = slot.CharacterData.displayName;
            }

            // ShowStats() bật StatsPanel lên -> Btn_Confirm/Btn_Back (con của panel) tự hiện theo cùng lúc
            statsPanel?.ShowStats(slot.CharacterData);
        }

        private void OnConfirmClicked()
        {
            if (_currentSlot == null) return;

            // Đổi từ Zoom Camera sang Confirm Camera của đúng nhân vật đang chọn
            if (_currentSlot.ZoomCamera != null) _currentSlot.ZoomCamera.gameObject.SetActive(false);
            if (_currentSlot.ConfirmCamera != null) _currentSlot.ConfirmCamera.gameObject.SetActive(true);

            GameSession.Instance.SelectCharacter(_currentSlot.CharacterData);

            onCharacterConfirmedEvent?.Raise();

            if (portal != null) portal.SetActive(true);
        }

        /// <summary>
        /// Gọi từ nút Back/Thoát trong StatsPanel — huỷ chọn nhân vật hiện tại,
        /// tắt cả 2 camera của nhân vật đó, đưa CM_Overview bật lại làm camera chính.
        /// </summary>
        private void OnBackClicked()
        {
            if (_currentSlot != null) _currentSlot.DisableAllCameras();
            _currentSlot = null;

            if (overviewCamera != null) overviewCamera.gameObject.SetActive(true);

            if (selectedNameLabel != null) selectedNameLabel.gameObject.SetActive(false);
            statsPanel?.Hide(); // ẩn panel -> Btn_Confirm/Btn_Back (con của panel) tự ẩn theo
        }
    }
}

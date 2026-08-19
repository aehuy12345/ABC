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
        [SerializeField] private TMP_Text selectedNameLabel;

        [Header("Events (Observer pattern - tuỳ chọn)")]
        [SerializeField] private GameEventSO onCharacterConfirmedEvent;

        [Header("Portal")]
        [SerializeField] private Game.Gameplay.World.Portal portal;

        [Header("Character slots (kéo-thả tất cả nhân vật trong scene vào đây)")]
        [SerializeField] private CharacterSelectSlot[] allSlots;

        private CharacterSelectSlot _currentSlot;

        private void Awake()
        {
            confirmButton.interactable = false;
            confirmButton.onClick.AddListener(OnConfirmClicked);

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

            if (selectedNameLabel != null) selectedNameLabel.text = slot.CharacterData.displayName;
            confirmButton.interactable = true;
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
    }
}

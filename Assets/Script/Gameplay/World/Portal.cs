using UnityEngine;
using Game.Audio;

namespace Game.Gameplay.World
{
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string targetSceneName = "03_GameplayMap";
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private GameObject visualEffect; // hiệu ứng cổng dịch chuyển, ẩn/hiện theo trạng thái active
        [SerializeField] private AudioClip teleportSfx;

        [Header("Prompt UI (nút hiện lên khi Player đứng gần cổng)")]
        [Tooltip("Kéo GameObject chứa Canvas/Button 'Vào Map' đặt cạnh cổng trong scene. " +
                 "Nên dùng Canvas Render Mode = World Space để nút nằm đúng vị trí cạnh cổng.")]
        [SerializeField] private GameObject promptUI;

        private bool _isActive;   // cổng đã được mở (VD sau khi Confirm nhân vật) hay chưa
        private bool _playerInRange; // Player có đang đứng trong vùng trigger hay không

        private void Awake()
        {
            SetActive(false);
            HidePrompt();
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            if (visualEffect != null) visualEffect.SetActive(active);

            // Nếu cổng bị đóng lại trong lúc Player đang đứng gần, ẩn luôn prompt cho nhất quán
            if (!active) HidePrompt();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive) return;
            if (!other.CompareTag(playerTag)) return;

            _playerInRange = true;
            ShowPrompt();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            _playerInRange = false;
            HidePrompt();
        }

        private void ShowPrompt()
        {
            if (promptUI != null) promptUI.SetActive(true);
        }

        private void HidePrompt()
        {
            if (promptUI != null) promptUI.SetActive(false);
        }

        /// <summary>
        /// Gọi từ OnClick() của nút trong promptUI (kéo-thả trong Inspector).
        /// Chỉ thực sự dịch chuyển nếu cổng đang mở và Player vẫn đang đứng trong vùng cổng.
        /// </summary>
        public void OnEnterButtonClicked()
        {
            if (!_isActive || !_playerInRange) return;

            AudioManager.Instance?.PlaySFX(teleportSfx);
            Game.Core.SceneLoader.LoadScene(targetSceneName);
        }
    }
}

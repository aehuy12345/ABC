using UnityEngine;
using Unity.Cinemachine;
using Game.Data;
using Game.Audio;

namespace Game.UI.CharacterSelect
{
    /// <summary>
    /// Gắn lên từng nhân vật đứng sẵn trong scene CharacterSelect (world-space, không phải UI).
    /// Cần có Collider2D (isTrigger = false, hoặc dùng OnMouseDown nên không cần trigger)
    /// để nhận click chuột trực tiếp lên nhân vật.
    ///
    /// Mỗi nhân vật có 2 CM Camera làm CON của chính nó (đặt trong Hierarchy):
    /// - Zoom Camera: bật khi Player click chọn nhân vật này (zoom cận).
    /// - Confirm Camera: bật khi Player bấm Confirm và dùng chính nhân vật này (Lens rộng hơn, VD Orthographic Size 4.29).
    /// Cả 2 mặc định TẮT (GameObject Inactive) — CharacterSelectController là nơi bật/tắt đúng lúc.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CharacterSelectSlot : MonoBehaviour
    {
        [SerializeField] private CharacterClassSO characterData;
        [SerializeField] private CinemachineCamera zoomCamera;
        [SerializeField] private CinemachineCamera confirmCamera;
        [SerializeField] private CharacterSelectController controller; // kéo-thả trong Inspector, thay vì Find lúc runtime
        [SerializeField] private AudioClip selectSfx;

        public CharacterClassSO CharacterData => characterData;
        public CinemachineCamera ZoomCamera => zoomCamera;
        public CinemachineCamera ConfirmCamera => confirmCamera;

        private void Awake()
        {
            // Đảm bảo cả 2 camera của nhân vật này luôn tắt lúc bắt đầu, đúng như thiết kế —
            // dù trong Editor bạn có lỡ để quên bật thì lúc chạy game vẫn tự tắt đúng.
            if (zoomCamera != null) zoomCamera.gameObject.SetActive(false);
            if (confirmCamera != null) confirmCamera.gameObject.SetActive(false);
        }

        /// <summary>Tắt cả 2 camera của nhân vật này — dùng khi bỏ chọn hoặc chuyển sang nhân vật khác.</summary>
        public void DisableAllCameras()
        {
            if (zoomCamera != null) zoomCamera.gameObject.SetActive(false);
            if (confirmCamera != null) confirmCamera.gameObject.SetActive(false);
        }

        // OnMouseDown yêu cầu Collider2D + Camera có Physics2DRaycaster (UI) hoặc chỉ cần Collider2D thường
        private void OnMouseDown()
        {
            AudioManager.Instance?.PlaySFX(selectSfx);
            controller?.SelectCharacter(this);
        }
    }
}

using UnityEngine;
using Game.Core;
using Game.Gameplay.MapGeneration;
using Game.Audio;

namespace Game.Gameplay.World
{
    [RequireComponent(typeof(Collider2D))]
    public class NextFloorPortal : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private MapGenerator mapGenerator; // kéo-thả object chứa MapGenerator trong scene
        [SerializeField] private GameObject visualEffect;   // hiệu ứng cổng, ẩn/hiện theo trạng thái đóng/mở
        [SerializeField] private AudioClip teleportSfx;

        private bool _isOpen;

        private void Awake()
        {
            SetActive(false); // mặc định đóng, chỉ mở khi Boss chết (EnemyController.onDeath gọi hàm này)
        }

        /// <summary>Gọi từ UnityEvent onDeath của Boss (kéo-thả trong Inspector, không cần code cứng).</summary>
        public void SetActive(bool open)
        {
            _isOpen = open;
            if (visualEffect != null) visualEffect.SetActive(open);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isOpen) return;
            if (!other.CompareTag(playerTag)) return;

            AudioManager.Instance?.PlaySFX(teleportSfx);
            GameSession.Instance.GoToNextFloor();

            mapGenerator.Generate(); // sinh map mới cho tầng tiếp theo

            other.transform.position = mapGenerator.GetStartRoomWorldPosition();
        }
    }
}

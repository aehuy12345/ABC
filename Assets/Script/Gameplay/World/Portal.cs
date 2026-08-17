using UnityEngine;

namespace Game.Gameplay.World
{
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string targetSceneName = "03_GameplayMap";
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private GameObject visualEffect; // hiệu ứng cổng dịch chuyển, ẩn/hiện theo trạng thái active

        private bool _isActive;

        private void Awake()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            if (visualEffect != null) visualEffect.SetActive(active);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive) return;
            if (!other.CompareTag(playerTag)) return;

            Game.Core.SceneLoader.LoadScene(targetSceneName);
        }
    }
}

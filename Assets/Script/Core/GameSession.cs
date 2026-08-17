using UnityEngine;
using Game.Data;
using Game.Patterns.Observer;

namespace Game.Core
{
    /// <summary>
    /// Không phải ScriptableObject vì đây là state runtime, thay đổi liên tục trong 1 lần chơi
    /// (khác với SO dùng làm dữ liệu tĩnh/cấu hình). Sống xuyên suốt các scene qua DontDestroyOnLoad.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [Header("Events (tuỳ chọn - HUD lắng nghe qua GameEventListener)")]
        [SerializeField] private GameEventSO onFloorChangedEvent;
        [SerializeField] private GameEventSO onGoldChangedEvent;

        public CharacterClassSO SelectedCharacter { get; private set; }
        public int CurrentFloor { get; private set; } = 1;
        public int Gold { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SelectCharacter(CharacterClassSO character)
        {
            SelectedCharacter = character;
        }

        /// <summary>Gọi khi Player đi qua cổng lên tầng tiếp theo (VD sau khi hạ Boss).</summary>
        public void GoToNextFloor()
        {
            CurrentFloor++;
            onFloorChangedEvent?.Raise();
        }

        public void AddGold(int amount)
        {
            Gold += amount;
            onGoldChangedEvent?.Raise();
        }

        /// <summary>Trừ vàng (VD mua đồ ở Shop room), trả về false nếu không đủ.</summary>
        public bool TrySpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            onGoldChangedEvent?.Raise();
            return true;
        }
    }
}

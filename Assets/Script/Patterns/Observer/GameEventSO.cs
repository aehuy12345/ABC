using System.Collections.Generic;
using UnityEngine;

namespace Game.Patterns.Observer
{
    /// <summary>
    /// Observer pattern kiểu Unity: mỗi sự kiện là 1 asset SO (VD: OnPlayerDeath.asset,
    /// OnRoomCleared.asset, OnWeaponPickup.asset). Các script Raise() để bắn event,
    /// GameEventListener đăng ký lắng nghe qua Inspector — không cần reference code cứng
    /// giữa các hệ thống (Player không cần biết UI, UI không cần biết Enemy...).
    /// </summary>
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game/Events/Game Event")]
    public class GameEventSO : ScriptableObject
    {
        private readonly List<IGameEventListener> _listeners = new List<IGameEventListener>();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(IGameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }

    public interface IGameEventListener
    {
        void OnEventRaised();
    }
}

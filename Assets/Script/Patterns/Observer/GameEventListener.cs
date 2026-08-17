using UnityEngine;
using UnityEngine.Events;

namespace Game.Patterns.Observer
{
    public class GameEventListener : MonoBehaviour, IGameEventListener
    {
        [SerializeField] private GameEventSO gameEvent;
        [SerializeField] private UnityEvent response;

        private void OnEnable()
        {
            gameEvent?.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent?.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            response?.Invoke();
        }
    }
}

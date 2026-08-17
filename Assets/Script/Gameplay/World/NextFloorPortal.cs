using UnityEngine;
using Game.Core;
using Game.Gameplay.MapGeneration;

namespace Game.Gameplay.World
{
    [RequireComponent(typeof(Collider2D))]
    public class NextFloorPortal : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private MapGenerator mapGenerator; // kéo-thả object chứa MapGenerator trong scene

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            GameSession.Instance.GoToNextFloor();

            mapGenerator.Generate(); // sinh map mới cho tầng tiếp theo

            other.transform.position = mapGenerator.GetStartRoomWorldPosition();
        }
    }
}

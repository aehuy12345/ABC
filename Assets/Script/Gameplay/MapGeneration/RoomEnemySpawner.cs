using UnityEngine;
using Game.Patterns.Pooling;

namespace Game.Gameplay.MapGeneration
{
    /// <summary>
    /// Gắn cùng GameObject với RoomInstance (trên mỗi room prefab).
    /// Không spawn quái ở phòng Start/Shop (phòng an toàn).
    /// </summary>
    [RequireComponent(typeof(RoomInstance))]
    public class RoomEnemySpawner : MonoBehaviour
    {
        [Tooltip("Danh sách prefab quái có thể xuất hiện trong phòng này (mỗi prefab đã tự gắn EnemyDataSO riêng)")]
        [SerializeField] private GameObject[] possibleEnemyPrefabs;
        [SerializeField] private int minEnemies = 2;
        [SerializeField] private int maxEnemies = 4;

        private RoomInstance _room;

        private void Awake()
        {
            _room = GetComponent<RoomInstance>();
        }

        private void Start()
        {
            if (_room.roomType == RoomType.Start || _room.roomType == RoomType.Shop) return;
            if (possibleEnemyPrefabs == null || possibleEnemyPrefabs.Length == 0) return;
            if (_room.enemySpawnPoints == null || _room.enemySpawnPoints.Length == 0) return;

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            int count = Mathf.Min(Random.Range(minEnemies, maxEnemies + 1), _room.enemySpawnPoints.Length);

            for (int i = 0; i < count; i++)
            {
                var prefab = possibleEnemyPrefabs[Random.Range(0, possibleEnemyPrefabs.Length)];
                var point = _room.enemySpawnPoints[i];

                PoolManager.Instance.Spawn(prefab, point.position, Quaternion.identity);
            }
        }
    }
}

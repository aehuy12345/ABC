using System.Collections.Generic;
using UnityEngine;
using Game.Patterns.Observer;

namespace Game.Gameplay.MapGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Generation settings")]
        [SerializeField] private int roomCount = 14;
        [SerializeField, Range(0f, 1f)] private float branchChance = 0.25f;
        [SerializeField] private int shopCount = 1;
        [SerializeField] private int treasureCount = 1;
        [SerializeField] private int seed = 0; // 0 = random mỗi lần chơi

        [Header("Room size trong world (đơn vị Unity)")]
        [SerializeField] private Vector2 roomWorldSize = new Vector2(20f, 12f);

        [Header("Room prefabs theo loại")]
        [SerializeField] private GameObject startRoomPrefab;
        [SerializeField] private GameObject normalRoomPrefab;
        [SerializeField] private GameObject shopRoomPrefab;
        [SerializeField] private GameObject treasureRoomPrefab;
        [SerializeField] private GameObject bossRoomPrefab;

        [Header("Events")]
        [SerializeField] private GameEventSO onMapGeneratedEvent; // HUD (Minimap) lắng nghe để vẽ lại

        private DungeonGraph _graph;
        private readonly Dictionary<Vector2Int, RoomInstance> _spawnedRooms = new Dictionary<Vector2Int, RoomInstance>();

        public DungeonGraph Graph => _graph;
        public Vector2 RoomWorldSize => roomWorldSize;

        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            // Xoá map cũ nếu Generate() được gọi lại (VD: nút "regenerate" khi test)
            foreach (var room in _spawnedRooms.Values) Destroy(room.gameObject);
            _spawnedRooms.Clear();

            var rng = seed == 0 ? new System.Random() : new System.Random(seed);

            _graph = new DungeonGraph();
            _graph.GenerateLayout(roomCount, branchChance, rng);      // Bước 1
            _graph.AssignRoomTypes(rng, shopCount, treasureCount);    // Bước 2
            _graph.ComputeDoors();                                    // Bước 3

            foreach (var node in _graph.Rooms.Values)
            {
                SpawnRoom(node);                                      // Bước 4
            }

            foreach (var room in _spawnedRooms.Values)
            {
                room.ApplyDoors(_graph.Rooms[GridPosOf(room)].Doors); // Bước 5
            }

            onMapGeneratedEvent?.Raise();
        }

        private Vector3 WorldPositionOf(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * roomWorldSize.x, gridPos.y * roomWorldSize.y, 0f);
        }

        private void SpawnRoom(RoomNode node)
        {
            GameObject prefab = node.Type switch
            {
                RoomType.Start => startRoomPrefab,
                RoomType.Shop => shopRoomPrefab,
                RoomType.Treasure => treasureRoomPrefab,
                RoomType.Boss => bossRoomPrefab,
                _ => normalRoomPrefab,
            };

            Vector3 worldPos = WorldPositionOf(node.GridPosition);

            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            var roomInstance = instance.GetComponent<RoomInstance>();
            roomInstance.roomType = node.Type;

            _spawnedRooms[node.GridPosition] = roomInstance;
        }

        private Vector2Int GridPosOf(RoomInstance room)
        {
            foreach (var kvp in _spawnedRooms)
                if (kvp.Value == room) return kvp.Key;
            return Vector2Int.zero;
        }

        /// <summary>Vị trí world của phòng Start, dùng để đặt cổng dịch chuyển / spawn player.</summary>
        public Vector3 GetStartRoomWorldPosition()
        {
            return new Vector3(0f, 0f, 0f); // Start luôn ở grid (0,0)
        }

        /// <summary>Vị trí world của phòng Boss, dùng để đặt cổng lên tầng tiếp theo (NextFloorPortal).</summary>
        public Vector3 GetBossRoomWorldPosition()
        {
            foreach (var node in _graph.Rooms.Values)
                if (node.Type == RoomType.Boss) return WorldPositionOf(node.GridPosition);
            return Vector3.zero;
        }
    }
}

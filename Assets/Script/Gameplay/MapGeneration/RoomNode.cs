using UnityEngine;

namespace Game.Gameplay.MapGeneration
{
    public enum RoomType
    {
        Start,
        Normal,
        Shop,
        Treasure,
        Boss
    }

    /// <summary>
    /// Bitmask 4 hướng cửa của 1 phòng. Dùng [Flags] để có thể kết hợp bằng OR (|).
    /// </summary>
    [System.Flags]
    public enum DoorDirection
    {
        None = 0,
        North = 1 << 0,
        South = 1 << 1,
        East = 1 << 2,
        West = 1 << 3
    }

    /// <summary>
    /// 1 node trong dungeon graph, tương ứng 1 ô lưới sẽ có phòng thật.
    /// Đây là data thuần (không phải MonoBehaviour) — tách biệt logic sinh graph
    /// khỏi việc instantiate GameObject thật trong scene.
    /// </summary>
    public class RoomNode
    {
        public Vector2Int GridPosition;
        public RoomType Type = RoomType.Normal;
        public DoorDirection Doors = DoorDirection.None;
        public int DistanceFromStart;

        public RoomNode(Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
        }
    }
}

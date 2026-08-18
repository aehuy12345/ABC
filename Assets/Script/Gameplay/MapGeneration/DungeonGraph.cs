using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.MapGeneration
{
    /// <summary>
    /// Class thuần C# (không MonoBehaviour) chứa toàn bộ thuật toán sinh graph.
    /// Tách khỏi MapGenerator (MonoBehaviour) để dễ unit test và tái sử dụng.
    /// </summary>
    public class DungeonGraph
    {
        public Dictionary<Vector2Int, RoomNode> Rooms { get; } = new Dictionary<Vector2Int, RoomNode>();

        private static readonly (Vector2Int offset, DoorDirection dir)[] Neighbors =
        {
            (Vector2Int.up,    DoorDirection.North),
            (Vector2Int.down,  DoorDirection.South),
            (Vector2Int.right, DoorDirection.East),
            (Vector2Int.left,  DoorDirection.West),
        };

        /// <summary>
        /// Bước 1: Random walk trên lưới, có branching, đến khi đủ roomCount phòng.
        /// </summary>
        public void GenerateLayout(int roomCount, float branchChance, System.Random rng)
        {
            var current = Vector2Int.zero;
            Rooms[current] = new RoomNode(current) { Type = RoomType.Start };

            var frontier = new List<Vector2Int> { current }; // các ô có thể "quay lại" để đi nhánh khác

            while (Rooms.Count < roomCount && frontier.Count > 0)
            {
                // Có xác suất branchChance thì nhảy về 1 ô cũ ngẫu nhiên thay vì đi tiếp từ ô hiện tại
                if (rng.NextDouble() < branchChance)
                {
                    current = frontier[rng.Next(frontier.Count)];
                }

                var dirOptions = Neighbors.OrderBy(_ => rng.Next()).ToList();
                bool moved = false;

                foreach (var (offset, _) in dirOptions)
                {
                    var next = current + offset;
                    if (Rooms.ContainsKey(next)) continue;

                    Rooms[next] = new RoomNode(next);
                    frontier.Add(next);
                    current = next;
                    moved = true;
                    break;
                }

                if (!moved)
                {
                    // Ô hiện tại bị vây kín 4 hướng -> loại khỏi frontier, thử ô khác
                    frontier.Remove(current);
                    if (frontier.Count > 0) current = frontier[rng.Next(frontier.Count)];
                }
            }
        }

        /// <summary>
        /// Bước 2: BFS từ Start để tính khoảng cách, gán Boss = ô xa nhất,
        /// Shop/Treasure = một vài ô cụt (chỉ 1 cửa), còn lại Normal.
        /// </summary>
        public void AssignRoomTypes(System.Random rng, int shopCount = 1, int treasureCount = 1)
        {
            var start = Vector2Int.zero;
            var visited = new HashSet<Vector2Int> { start };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            Rooms[start].DistanceFromStart = 0;

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                int dist = Rooms[pos].DistanceFromStart;

                foreach (var (offset, _) in Neighbors)
                {
                    var next = pos + offset;
                    if (!Rooms.ContainsKey(next) || visited.Contains(next)) continue;

                    visited.Add(next);
                    Rooms[next].DistanceFromStart = dist + 1;
                    queue.Enqueue(next);
                }
            }

            // Boss = phòng xa Start nhất
            var bossRoom = Rooms.Values.Where(r => r.Type != RoomType.Start)
                                        .OrderByDescending(r => r.DistanceFromStart)
                                        .FirstOrDefault();
            if (bossRoom != null) bossRoom.Type = RoomType.Boss;

            // Dead-end = phòng chỉ có đúng 1 hướng lân cận tồn tại, chưa phải Start/Boss
            var deadEnds = Rooms.Values
                .Where(r => r.Type == RoomType.Normal)
                .Where(r => CountNeighbors(r.GridPosition) == 1)
                .OrderBy(_ => rng.Next())
                .ToList();

            foreach (var room in deadEnds.Take(shopCount)) room.Type = RoomType.Shop;
            foreach (var room in deadEnds.Skip(shopCount).Take(treasureCount)) room.Type = RoomType.Treasure;
        }

        /// <summary>
        /// Bước 3: Với mỗi phòng, kiểm tra 4 lân cận để tính bitmask cửa cần mở.
        /// </summary>
        public void ComputeDoors()
        {
            foreach (var room in Rooms.Values)
            {
                DoorDirection doors = DoorDirection.None;
                foreach (var (offset, dir) in Neighbors)
                {
                    if (Rooms.ContainsKey(room.GridPosition + offset))
                        doors |= dir;
                }
                room.Doors = doors;
            }
        }

        private int CountNeighbors(Vector2Int pos)
        {
            int count = 0;
            foreach (var (offset, _) in Neighbors)
                if (Rooms.ContainsKey(pos + offset)) count++;
            return count;
        }
    }
}
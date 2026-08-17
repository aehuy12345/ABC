using UnityEngine;

namespace Game.Gameplay.MapGeneration
{
    /// <summary>
    /// Gắn lên mỗi room prefab (room_normal, room_shop, room_boss...).
    /// supportedDoors khai báo prefab này có sẵn những cửa nào (thường để full 4 cửa
    /// cho đơn giản, cửa nào không dùng thì MapGenerator sẽ tự đóng qua doorObjects).
    /// </summary>
    public class RoomInstance : MonoBehaviour
    {
        public RoomType roomType;

        [Header("Door references (kéo-thả GameObject cửa/tường tương ứng)")]
        public GameObject northDoor;
        public GameObject southDoor;
        public GameObject eastDoor;
        public GameObject westDoor;

        [Header("Điểm spawn quái trong phòng, dùng bởi Object Pool")]
        public Transform[] enemySpawnPoints;

        /// <summary>
        /// Bật cửa cần dùng, tắt (đóng bằng tường) cửa không cần theo bitmask từ RoomNode.
        /// </summary>
        public void ApplyDoors(DoorDirection doors)
        {
            SetDoor(northDoor, doors.HasFlag(DoorDirection.North));
            SetDoor(southDoor, doors.HasFlag(DoorDirection.South));
            SetDoor(eastDoor, doors.HasFlag(DoorDirection.East));
            SetDoor(westDoor, doors.HasFlag(DoorDirection.West));
        }

        private void SetDoor(GameObject door, bool open)
        {
            if (door == null) return;
            // "Mở cửa" ở đây = active cửa (đi qua được); nếu không mở thì cửa tắt đi,
            // để lộ tường phía sau (tường nên luôn có sẵn, không cần script riêng).
            door.SetActive(open);
        }
    }
}

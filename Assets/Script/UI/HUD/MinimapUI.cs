using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay.MapGeneration;

namespace Game.UI.HUD
{
    /// <summary>
    /// Gọi Refresh() (public) mỗi khi map vừa generate xong — nối qua GameEventListener
    /// lắng nghe onMapGeneratedEvent của MapGenerator (Observer pattern, không cần reference cứng).
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private Transform player;
        [SerializeField] private RectTransform mapContainer;
        [SerializeField] private GameObject roomIconPrefab; // prefab UI Image nhỏ (hình vuông/tròn)
        [SerializeField] private float iconSpacing = 24f;

        [Header("Màu theo loại phòng")]
        [SerializeField] private Color startColor = Color.cyan;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color shopColor = Color.yellow;
        [SerializeField] private Color treasureColor = Color.magenta;
        [SerializeField] private Color bossColor = Color.red;
        [SerializeField] private Color currentRoomHighlight = Color.green;

        private readonly Dictionary<Vector2Int, Image> _icons = new Dictionary<Vector2Int, Image>();
        private readonly Dictionary<Vector2Int, RoomType> _roomTypes = new Dictionary<Vector2Int, RoomType>();
        private Vector2Int _lastHighlighted;

        /// <summary>Gọi lại mỗi khi map mới được sinh (đầu game, hoặc sau khi qua tầng tiếp theo).</summary>
        public void Refresh()
        {
            foreach (var icon in _icons.Values) Destroy(icon.gameObject);
            _icons.Clear();
            _roomTypes.Clear();

            if (mapGenerator.Graph == null) return;

            foreach (var node in mapGenerator.Graph.Rooms.Values)
            {
                var iconObj = Instantiate(roomIconPrefab, mapContainer);
                var rect = iconObj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(node.GridPosition.x, node.GridPosition.y) * iconSpacing;

                var image = iconObj.GetComponent<Image>();
                image.color = ColorForType(node.Type);

                _icons[node.GridPosition] = image;
                _roomTypes[node.GridPosition] = node.Type;
            }
        }

        private void Update()
        {
            if (player == null || _icons.Count == 0) return;

            Vector2Int currentGrid = WorldToGrid(player.position);
            if (currentGrid == _lastHighlighted) return; // chỉ update màu khi đổi phòng, đỡ tốn hiệu năng

            // Trả lại màu gốc cho phòng cũ
            if (_icons.TryGetValue(_lastHighlighted, out var previousIcon) && _roomTypes.TryGetValue(_lastHighlighted, out var previousType))
            {
                previousIcon.color = ColorForType(previousType);
            }

            if (_icons.TryGetValue(currentGrid, out var currentIcon))
            {
                currentIcon.color = currentRoomHighlight;
                _lastHighlighted = currentGrid;
            }
        }

        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector2 roomSize = mapGenerator.RoomWorldSize;
            int gx = Mathf.RoundToInt(worldPos.x / roomSize.x);
            int gy = Mathf.RoundToInt(worldPos.y / roomSize.y);
            return new Vector2Int(gx, gy);
        }

        private Color ColorForType(RoomType type) => type switch
        {
            RoomType.Start => startColor,
            RoomType.Shop => shopColor,
            RoomType.Treasure => treasureColor,
            RoomType.Boss => bossColor,
            _ => normalColor,
        };
    }
}

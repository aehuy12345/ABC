using System;
using System.Collections.Generic;

namespace Game.Data.Json
{
    /// <summary>
    /// DTO thuần (không kế thừa MonoBehaviour/ScriptableObject) để JsonUtility serialize được.
    /// Đây là nơi JSON được dùng: lưu tiến trình người chơi, KHÔNG dùng để cấu hình
    /// stat class/weapon (phần đó nằm ở ScriptableObject cho dễ chỉnh trong Editor).
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<string> unlockedCharacterIds = new List<string>();
        public List<string> unlockedWeaponIds = new List<string>();
        public string lastSelectedCharacterId;
        public int bestFloorReached;
        public int totalGold;
    }
}

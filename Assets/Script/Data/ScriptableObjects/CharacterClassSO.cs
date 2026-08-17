using UnityEngine;

namespace Game.Data
{
    public enum CharacterRole
    {
        Archer,   // Xạ thủ
        Warrior,  // Đấu sĩ
        Tank,     // Tank
        Assassin  // Sát thủ
    }

    [CreateAssetMenu(fileName = "NewCharacterClass", menuName = "Game/Character/Character Class")]
    public class CharacterClassSO : ScriptableObject
    {
        [Header("Identity")]
        public string classId;              // dùng để lưu vào JSON save data (khớp key)
        public string displayName;
        public CharacterRole role;
        [TextArea] public string description;
        public Sprite portrait;             // dùng cho UI chọn nhân vật
        public GameObject worldPrefab;       // prefab spawn trong map

        [Header("Base Stats")]
        public float baseHP = 100f;
        public float baseATK = 10f;
        public float baseDEF = 0f;
        public float moveSpeed = 5f;

        [Header("Default Loadout")]
        public WeaponSO defaultWeapon;

        [Header("Character Select Scene")]
        public Transform selectSpawnPointOverride; // optional, nếu cần vị trí riêng trên bệ chọn nhân vật
        public string cinemachineCameraId;         // id của vcam tương ứng nhân vật này trong scene CharacterSelect
    }
}

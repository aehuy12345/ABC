using UnityEngine;

namespace Game.Data
{
    public enum WeaponType
    {
        Melee,
        Ranged
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon/Weapon")]
    public class WeaponSO : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId;
        public string displayName;
        public Sprite icon;
        public WeaponType weaponType;

        [Header("Stats")]
        public float damage = 10f;
        public float fireRate = 0.3f;      // giây giữa 2 lần đánh/bắn
        public float range = 5f;           // tầm với melee, tầm bay tối đa với ranged
        public float projectileSpeed = 12f; // chỉ dùng khi weaponType = Ranged

        [Header("References")]
        public GameObject projectilePrefab; // null nếu là melee
        public AudioClip attackSfx;

        [Header("Ammo (chỉ áp dụng cho Ranged)")]
        [Tooltip("0 = đạn vô hạn, không cần reload (VD dùng cho melee hoặc vũ khí đặc biệt)")]
        public int magazineSize = 6;
        public float reloadTime = 1.2f;

        [Header("Pooling")]
        public int poolPrewarmCount = 20;   // số lượng projectile prewarm trong Object Pool
    }
}

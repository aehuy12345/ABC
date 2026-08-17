using UnityEngine;
using Game.Data;
using Game.Data.Traits; // IDamageable
using Game.Gameplay.Player;
using Game.Patterns.Pooling;

namespace Game.Gameplay.Weapon
{
    /// <summary>
    /// Gắn trên Player (implement IWeaponHandler mà PlayerController đã gọi sẵn qua TryAttack).
    /// Cũng có thể gắn trên Enemy để tái sử dụng logic bắn/đánh, chỉ cần đổi targetTag.
    /// </summary>
    public class WeaponHandler : MonoBehaviour, IWeaponHandler
    {
        [SerializeField] private WeaponSO currentWeapon;
        [SerializeField] private Transform firePoint; // điểm xuất phát đạn / tâm vùng đánh melee

        [Tooltip("Tag của đối tượng vũ khí này có thể gây damage. Player dùng \"Enemy\", Enemy dùng \"Player\".")]
        [SerializeField] private string targetTag = "Enemy";

        private float _cooldownTimer;
        private int _currentAmmo;
        private float _reloadTimer;
        private bool _isReloading;

        /// <summary>Vũ khí hiện tại, để HUD lấy icon/tên hiển thị.</summary>
        public WeaponSO CurrentWeapon => currentWeapon;

        /// <summary>0 = vừa bắn xong (cooldown đầy), 1 = sẵn sàng bắn tiếp — HUD dùng để vẽ vòng cooldown.</summary>
        public float CooldownRatio01 => currentWeapon == null || currentWeapon.fireRate <= 0f
            ? 1f : 1f - Mathf.Clamp01(_cooldownTimer / currentWeapon.fireRate);

        public int CurrentAmmo => _currentAmmo;
        public int MagazineSize => currentWeapon != null ? currentWeapon.magazineSize : 0;
        public bool HasUnlimitedAmmo => currentWeapon != null && currentWeapon.magazineSize <= 0;
        public bool IsReloading => _isReloading;
        public float ReloadRatio01 => _isReloading && currentWeapon != null && currentWeapon.reloadTime > 0f
            ? 1f - Mathf.Clamp01(_reloadTimer / currentWeapon.reloadTime) : 0f;

        private bool _isPlayerControlled; // chỉ Player mới đọc phím R để reload, Enemy tự động reload khi hết đạn

        private void Awake()
        {
            if (currentWeapon != null && currentWeapon.weaponType == WeaponType.Ranged && currentWeapon.projectilePrefab != null)
            {
                PoolManager.Instance.RegisterPool(currentWeapon.projectilePrefab, currentWeapon.poolPrewarmCount);
            }

            if (currentWeapon != null) _currentAmmo = currentWeapon.magazineSize;
            _isPlayerControlled = GetComponent<Game.Gameplay.Player.PlayerController>() != null;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            if (_isReloading)
            {
                _reloadTimer -= Time.deltaTime;
                if (_reloadTimer <= 0f) FinishReload();
            }

            // Reload thủ công bằng phím R, chỉ áp dụng cho Player và vũ khí có magazine giới hạn
            if (_isPlayerControlled && Input.GetKeyDown(KeyCode.R) && !_isReloading && !HasUnlimitedAmmo && _currentAmmo < MagazineSize)
            {
                StartReload();
            }
        }

        public void TryAttack(Vector2 aimDirection)
        {
            if (currentWeapon == null) return;
            if (_cooldownTimer > 0f || _isReloading) return;

            if (currentWeapon.weaponType == WeaponType.Ranged && !HasUnlimitedAmmo)
            {
                if (_currentAmmo <= 0)
                {
                    StartReload(); // hết đạn -> tự động reload
                    return;
                }
                _currentAmmo--;
            }

            _cooldownTimer = currentWeapon.fireRate;

            if (currentWeapon.weaponType == WeaponType.Ranged)
                FireProjectile(aimDirection);
            else
                MeleeAttack(aimDirection);
        }

        private void StartReload()
        {
            _isReloading = true;
            _reloadTimer = currentWeapon.reloadTime;
        }

        private void FinishReload()
        {
            _isReloading = false;
            _currentAmmo = currentWeapon.magazineSize;
        }

        private void FireProjectile(Vector2 aimDirection)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

            GameObject instance = PoolManager.Instance.Spawn(
                currentWeapon.projectilePrefab, spawnPos, Quaternion.identity);

            var projectile = instance.GetComponent<Projectile>();
            projectile.Init(
                direction: aimDirection,
                damage: currentWeapon.damage,
                speed: currentWeapon.projectileSpeed,
                maxRange: currentWeapon.range,
                targetTag: targetTag);
        }

        private void MeleeAttack(Vector2 aimDirection)
        {
            Vector2 origin = (firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position)
                              + aimDirection.normalized * (currentWeapon.range * 0.5f);

            // Vùng đánh xấp xỉ hình tròn quanh điểm giữa hướng đánh — đơn giản, dễ tinh chỉnh bằng range.
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, currentWeapon.range * 0.5f);

            foreach (var hit in hits)
            {
                if (!hit.CompareTag(targetTag)) continue;

                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(currentWeapon.damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Hỗ trợ debug vùng đánh melee ngay trong Scene view
            if (currentWeapon == null || currentWeapon.weaponType != WeaponType.Melee) return;

            Vector2 origin = (firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position)
                              + Vector2.right * (currentWeapon.range * 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin, currentWeapon.range * 0.5f);
        }
    }
}

using UnityEngine;
using Game.Data.Traits; // IDamageable
using Game.Patterns.Pooling;

namespace Game.Gameplay.Weapon
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        private Vector2 _direction;
        private float _speed;
        private float _damage;
        private string _targetTag;
        private float _remainingLifetime;

        /// <summary>
        /// Gọi ngay sau khi lấy projectile ra từ Object Pool để "reset" trạng thái cho lần bắn mới.
        /// </summary>
        public void Init(Vector2 direction, float damage, float speed, float maxRange, string targetTag)
        {
            _direction = direction.normalized;
            _damage = damage;
            _speed = speed;
            _targetTag = targetTag;
            _remainingLifetime = speed > 0f ? maxRange / speed : 1f; // bay hết range thì tự despawn

            transform.right = _direction; // xoay sprite theo hướng bay (giả định sprite hướng phải mặc định)
        }

        private void Update()
        {
            transform.position += (Vector3)(_direction * _speed * Time.deltaTime);

            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(_targetTag)) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_damage);
            }

            Despawn();
        }

        private void Despawn()
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(gameObject);
            else
                Destroy(gameObject); // fallback nếu test scene chưa có PoolManager
        }
    }
}
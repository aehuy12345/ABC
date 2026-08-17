using System.Collections.Generic;
using UnityEngine;

namespace Game.Patterns.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private readonly Dictionary<GameObject, ObjectPool> _pools = new Dictionary<GameObject, ObjectPool>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Đăng ký trước 1 pool cho prefab, kèm prewarm. Gọi ở lúc load level / spawn weapon.
        /// </summary>
        public void RegisterPool(GameObject prefab, int prewarmCount)
        {
            if (_pools.ContainsKey(prefab)) return;

            var poolParent = new GameObject($"Pool_{prefab.name}").transform;
            poolParent.SetParent(transform);

            _pools[prefab] = new ObjectPool(prefab, poolParent, prewarmCount);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(prefab))
            {
                RegisterPool(prefab, 0); // fallback: tạo pool rỗng nếu chưa đăng ký trước
            }

            return _pools[prefab].Get(position, rotation);
        }

        public void Despawn(GameObject instance)
        {
            if (instance.TryGetComponent<PooledObject>(out var pooled))
            {
                pooled.ReturnToPool();
            }
            else
            {
                Destroy(instance); // object không thuộc pool nào, hủy bình thường
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Game.Patterns.Pooling
{
    /// <summary>
    /// Object Pool đơn giản cho 1 loại prefab cụ thể.
    /// PoolManager sẽ quản lý nhiều ObjectPool (mỗi prefab 1 pool riêng).
    /// </summary>
    public class ObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();

        public ObjectPool(GameObject prefab, Transform parent, int prewarmCount = 0)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < prewarmCount; i++)
            {
                var obj = CreateNew();
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        private GameObject CreateNew()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            var handle = obj.AddComponent<PooledObject>();
            handle.SourcePool = this;
            return obj;
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public void Release(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_parent);
            _pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Gắn vào mỗi object được pool để nó tự biết trả về pool nào khi despawn.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool SourcePool;

        public void ReturnToPool()
        {
            SourcePool?.Release(gameObject);
        }
    }
}

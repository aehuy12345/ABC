using UnityEngine;
using Game.Patterns.Pooling;

namespace Game.Data.Traits
{
    [CreateAssetMenu(fileName = "SplitOnDeathTrait", menuName = "Game/Enemy Trait/Split On Death")]
    public class SplitOnDeathTrait : EnemyTraitSO
    {
        public GameObject childEnemyPrefab; // enemy nhỏ hơn, lấy từ Object Pool
        public int splitCount = 2;
        public float spreadRadius = 0.5f;

        private void OnEnable()
        {
            trigger = TraitTrigger.OnDeath;
        }

        public override void Execute(EnemyContext ctx)
        {
            if (childEnemyPrefab == null) return;

            for (int i = 0; i < splitCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spreadRadius;
                Vector3 spawnPos = ctx.Self.transform.position + (Vector3)offset;

                // Lấy object từ pool thay vì Instantiate trực tiếp
                PoolManager.Instance.Spawn(childEnemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}

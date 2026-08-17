using UnityEngine;

namespace Game.Data.Traits
{
    [CreateAssetMenu(fileName = "ContactDamageTrait", menuName = "Game/Enemy Trait/Contact Damage")]
    public class ContactDamageTrait : EnemyTraitSO
    {
        public float contactDamage = 5f;

        private void OnEnable()
        {
            trigger = TraitTrigger.OnContact;
        }

        public override void Execute(EnemyContext ctx)
        {
            if (ctx.Target == null) return;

            // Giả định Player có component IDamageable
            if (ctx.Target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(contactDamage);
            }
        }
    }

    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}

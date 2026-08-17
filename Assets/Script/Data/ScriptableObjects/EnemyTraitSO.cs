using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// Context truyền vào mỗi trait khi Execute, để trait thao tác lên enemy hiện tại
    /// mà không cần biết chi tiết implementation của EnemyController.
    /// </summary>
    public class EnemyContext
    {
        public GameObject Self;
        public Transform Target;      // thường là Player
        public float CurrentHP;
        public float MaxHP;

        public EnemyContext(GameObject self, Transform target, float currentHP, float maxHP)
        {
            Self = self;
            Target = target;
            CurrentHP = currentHP;
            MaxHP = maxHP;
        }
    }

    public enum TraitTrigger
    {
        OnUpdate,      // chạy mỗi frame/tick
        OnContact,     // khi va chạm với player
        OnDeath,       // khi enemy chết
        OnSpawn        // khi enemy được spawn ra
    }

    /// <summary>
    /// Base ScriptableObject cho mọi đặc thù kẻ địch (VD: Slime gây sát thương khi chạm,
    /// tách đôi khi chết, bắn xa, v.v). Mỗi EnemySO giữ 1 List các trait này,
    /// EnemyController sẽ gọi Execute() đúng thời điểm dựa theo Trigger.
    /// </summary>
    public abstract class EnemyTraitSO : ScriptableObject
    {
        public TraitTrigger trigger;

        public abstract void Execute(EnemyContext ctx);
    }
}

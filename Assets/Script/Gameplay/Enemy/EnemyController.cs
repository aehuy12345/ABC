using UnityEngine;
using Game.Data;
using Game.Data.Traits; // IDamageable
using Game.Gameplay.Player; // IWeaponHandler (tái sử dụng interface, enemy bắn cũng gọi qua đây)
using Game.Patterns.Pooling;

namespace Game.Gameplay.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyDataSO data;
        [SerializeField] private string playerTag = "Player";

        public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

        private Rigidbody2D _rb;
        private IWeaponHandler _weaponHandler; // null nếu enemy này không có vũ khí tầm xa (VD Slime chỉ contact damage)
        private Transform _player;
        private Vector3 _spawnPosition;
        private Vector3 _patrolTarget;

        private float _currentHP;
        private bool _isDead;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _weaponHandler = GetComponent<IWeaponHandler>();
        }

        private void Start()
        {
            _currentHP = data.maxHP;
            _spawnPosition = transform.position;
            PickNewPatrolTarget();

            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null) _player = playerObj.transform;

            ExecuteTraits(TraitTrigger.OnSpawn);
        }

        private void Update()
        {
            if (_isDead) return;

            ExecuteTraits(TraitTrigger.OnUpdate);

            switch (CurrentState)
            {
                case EnemyState.Patrol: UpdatePatrol(); break;
                case EnemyState.Chase: UpdateChase(); break;
                case EnemyState.Attack: UpdateAttack(); break;
            }
        }

        // ---------------- STATE: PATROL ----------------
        private void UpdatePatrol()
        {
            MoveTowards(_patrolTarget);

            if (Vector3.Distance(transform.position, _patrolTarget) < 0.2f)
                PickNewPatrolTarget();

            if (IsPlayerWithin(data.detectionRange))
                CurrentState = EnemyState.Chase;
        }

        private void PickNewPatrolTarget()
        {
            Vector2 randomOffset = Random.insideUnitCircle * data.patrolRadius;
            _patrolTarget = _spawnPosition + (Vector3)randomOffset;
        }

        // ---------------- STATE: CHASE ----------------
        private void UpdateChase()
        {
            if (_player == null) { CurrentState = EnemyState.Patrol; return; }

            MoveTowards(_player.position);

            if (IsPlayerWithin(data.attackRange))
                CurrentState = EnemyState.Attack;
            else if (!IsPlayerWithin(data.loseTargetRange))
                CurrentState = EnemyState.Patrol;
        }

        // ---------------- STATE: ATTACK ----------------
        private void UpdateAttack()
        {
            _rb.linearVelocity = Vector2.zero; // đứng yên khi trong tầm đánh

            if (_player == null) { CurrentState = EnemyState.Patrol; return; }

            // Nếu enemy có vũ khí tầm xa (WeaponHandler với targetTag="Player") thì bắn.
            // Enemy dạng contact-damage thuần (Slime) không có component này -> chỉ dựa vào ContactDamageTrait qua va chạm.
            Vector2 aimDir = (_player.position - transform.position).normalized;
            _weaponHandler?.TryAttack(aimDir);

            if (!IsPlayerWithin(data.attackRange))
                CurrentState = EnemyState.Chase;
        }

        // ---------------- MOVEMENT HELPER ----------------
        private void MoveTowards(Vector3 target)
        {
            Vector2 dir = ((Vector2)target - _rb.position).normalized;
            _rb.linearVelocity = dir * data.moveSpeed;
        }

        private bool IsPlayerWithin(float range)
        {
            if (_player == null) return false;
            return Vector3.Distance(transform.position, _player.position) <= range;
        }

        // ---------------- DAMAGE / DEATH ----------------
        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            _currentHP -= amount;
            if (_currentHP <= 0f) Die();
        }

        private void Die()
        {
            _isDead = true;
            CurrentState = EnemyState.Dead;
            _rb.linearVelocity = Vector2.zero;

            ExecuteTraits(TraitTrigger.OnDeath); // VD: SplitOnDeathTrait tách quái con ở đây

            PoolManager.Instance?.Despawn(gameObject);
        }

        // Gọi khi player va chạm vật lý với enemy (VD Slime gây damage khi chạm)
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_isDead) return;
            if (!collision.collider.CompareTag(playerTag)) return;

            ExecuteTraits(TraitTrigger.OnContact, collision.transform);
        }

        // ---------------- TRAIT EXECUTION ----------------
        private void ExecuteTraits(TraitTrigger trigger, Transform targetOverride = null)
        {
            if (data.traits == null) return;

            var ctx = new EnemyContext(
                self: gameObject,
                target: targetOverride != null ? targetOverride : _player,
                currentHP: _currentHP,
                maxHP: data.maxHP);

            foreach (var trait in data.traits)
            {
                if (trait != null && trait.trigger == trigger)
                    trait.Execute(ctx);
            }
        }
    }
}

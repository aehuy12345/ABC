using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f; // sẽ bị ghi đè bởi CharacterClassSO nếu có

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 0.8f;
        [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

        [Header("Attack")]
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
        public bool IsInvincible { get; private set; } // dùng bởi PlayerStats để miễn sát thương lúc dash

        private Rigidbody2D _rb;
        private IWeaponHandler _weaponHandler; // implement thật ở bước Weapon System
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.down;

        private bool _isDashing;
        private float _dashCooldownTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _weaponHandler = GetComponent<IWeaponHandler>(); // null nếu chưa có component weapon nào implement

            var selected = GameSession.Instance != null ? GameSession.Instance.SelectedCharacter : null;
            if (selected != null) moveSpeed = selected.moveSpeed;
        }

        private void Update()
        {
            if (_isDashing) return; // trong lúc dash, bỏ qua input thường (đang chạy coroutine Dash)

            ReadMovementInput();
            HandleDashInput();
            HandleAttackInput();

            if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;

            UpdateState();
        }

        private void FixedUpdate()
        {
            if (_isDashing) return; // di chuyển lúc dash do coroutine DoDash() tự set velocity

            _rb.linearVelocity = _moveInput * moveSpeed;
        }

        private void ReadMovementInput()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            _moveInput = new Vector2(x, y).normalized;

            if (_moveInput.sqrMagnitude > 0.01f)
                _lastMoveDirection = _moveInput;
        }

        private void HandleDashInput()
        {
            if (Input.GetKeyDown(dashKey) && _dashCooldownTimer <= 0f)
            {
                StartCoroutine(DoDash());
            }
        }

        private void HandleAttackInput()
        {
            if (Input.GetKey(attackKey))
            {
                // Hướng đánh: ưu tiên hướng đang di chuyển, nếu đứng yên thì dùng hướng nhìn gần nhất
                _weaponHandler?.TryAttack(_lastMoveDirection);
            }
        }

        private IEnumerator DoDash()
        {
            _isDashing = true;
            IsInvincible = true;
            _dashCooldownTimer = dashCooldown;

            Vector2 dashDirection = _moveInput.sqrMagnitude > 0.01f ? _moveInput : _lastMoveDirection;
            _rb.linearVelocity = dashDirection * dashSpeed;

            yield return new WaitForSeconds(dashDuration);

            _isDashing = false;
            IsInvincible = false;
            _rb.linearVelocity = Vector2.zero;
        }

        private void UpdateState()
        {
            if (_isDashing) { CurrentState = PlayerState.Dash; return; }
            if (Input.GetKey(attackKey)) { CurrentState = PlayerState.Attack; return; }
            CurrentState = _moveInput.sqrMagnitude > 0.01f ? PlayerState.Move : PlayerState.Idle;
        }
    }
}
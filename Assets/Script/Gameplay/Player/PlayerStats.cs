using UnityEngine;
using Game.Core;
using Game.Data.Traits; // IDamageable
using Game.Patterns.Observer;
using Game.Audio;

namespace Game.Gameplay.Player
{
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [Header("Fallback stats (dùng nếu không có CharacterClassSO, VD lúc test riêng scene)")]
        [SerializeField] private float fallbackMaxHP = 100f;

        [Header("Events")]
        [SerializeField] private GameEventSO onPlayerDeathEvent;

        [Header("Audio")]
        [SerializeField] private AudioClip hurtSfx;
        [SerializeField] private AudioClip deathSfx;

        public float MaxHP { get; private set; }
        public float CurrentHP { get; private set; }
        public bool IsDead { get; private set; }

        private PlayerController _controller;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();

            var selected = GameSession.Instance != null ? GameSession.Instance.SelectedCharacter : null;
            MaxHP = selected != null ? selected.baseHP : fallbackMaxHP;
            CurrentHP = MaxHP;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            if (_controller != null && _controller.IsInvincible) return; // đang dash -> miễn sát thương

            CurrentHP -= amount;
            AudioManager.Instance?.PlaySFX(hurtSfx);

            if (CurrentHP <= 0f)
            {
                CurrentHP = 0f;
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        }

        private void Die()
        {
            IsDead = true;
            AudioManager.Instance?.PlaySFX(deathSfx);
            onPlayerDeathEvent?.Raise();
        }
    }
}

using UnityEngine;
using Game.Data;

namespace Game.Gameplay.Enemy
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId;
        public string displayName;

        [Header("Stats")]
        public float maxHP = 30f;
        public float moveSpeed = 2.5f;

        [Header("AI Ranges")]
        public float patrolRadius = 3f;      // bán kính lượn quanh điểm spawn khi Patrol
        public float detectionRange = 5f;    // player vào tầm này -> chuyển Chase
        public float attackRange = 1.2f;     // player vào tầm này -> chuyển Attack
        public float loseTargetRange = 7f;   // player ra khỏi tầm này -> quay lại Patrol

        [Header("Traits (composition - kéo nhiều trait vào đây)")]
        public EnemyTraitSO[] traits;

        [Header("Audio")]
        public AudioClip hitSfx;
        public AudioClip deathSfx;
    }
}

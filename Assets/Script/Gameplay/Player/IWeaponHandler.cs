using UnityEngine;

namespace Game.Gameplay.Player
{
    /// <summary>
    /// PlayerController chỉ biết gọi TryAttack() qua interface này, không cần biết
    /// vũ khí hiện tại là melee hay ranged, đạn bay thế nào... (tách rời theo weapon system).
    /// Component thật implement interface này sẽ được viết ở bước Weapon System.
    /// </summary>
    public interface IWeaponHandler
    {
        void TryAttack(Vector2 aimDirection);
    }
}
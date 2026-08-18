using UnityEngine;
using Unity.Cinemachine;
using Game.Data;
using Game.Audio;

namespace Game.UI.CharacterSelect
{
    /// <summary>
    /// Gắn lên từng nhân vật đứng sẵn trong scene CharacterSelect (world-space, không phải UI).
    /// Cần có Collider2D (isTrigger = false, hoặc dùng OnMouseDown nên không cần trigger)
    /// để nhận click chuột trực tiếp lên nhân vật.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CharacterSelectSlot : MonoBehaviour
    {
        [SerializeField] private CharacterClassSO characterData;
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CharacterSelectController controller; // kéo-thả trong Inspector, thay vì Find lúc runtime
        [SerializeField] private AudioClip selectSfx;

        public CharacterClassSO CharacterData => characterData;
        public CinemachineCamera VirtualCamera => virtualCamera;

        // OnMouseDown yêu cầu Collider2D + Camera có Physics2DRaycaster (UI) hoặc chỉ cần Collider2D thường
        private void OnMouseDown()
        {
            AudioManager.Instance?.PlaySFX(selectSfx);
            controller?.SelectCharacter(this);
        }
    }
}

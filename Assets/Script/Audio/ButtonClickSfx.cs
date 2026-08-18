using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

namespace Game.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSfx : MonoBehaviour
    {
        [SerializeField] private AudioClip clickClip;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlayClickSfx);
        }

        private void PlayClickSfx()
        {
            AudioManager.Instance?.PlaySFX(clickClip);
        }
    }
}

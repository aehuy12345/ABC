using UnityEngine;
using Game.Audio;

namespace Game.Gameplay.World
{
    public class BGMTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private bool loop = true;

        private void Start()
        {
            AudioManager.Instance?.PlayBGM(bgmClip, loop);
        }
    }
}

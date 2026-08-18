using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer (tạo qua Assets > Create > Audio Mixer)")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerGroup bgmGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        [Header("Tên tham số Exposed Parameter trong Mixer (phải khớp chính xác)")]
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [SerializeField] private string bgmVolumeParam = "BGMVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";

        [Header("BGM crossfade")]
        [SerializeField] private float bgmFadeDuration = 0.6f;

        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        private Coroutine _fadeRoutine;

        private const string PrefMaster = "audio_master_volume";
        private const string PrefBGM = "audio_bgm_volume";
        private const string PrefSFX = "audio_sfx_volume";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.outputAudioMixerGroup = bgmGroup;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.outputAudioMixerGroup = sfxGroup;

            LoadSavedVolumes();
        }

        // ---------------- SFX ----------------
        /// <summary>
        /// Dùng PlayOneShot trên 1 AudioSource dùng chung — Unity tự mix chồng lấp nhiều SFX
        /// phát cùng lúc mà không cần tự quản lý pool AudioSource riêng.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volumeScale);
        }

        // ---------------- BGM ----------------
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (clip == null || _bgmSource.clip == clip) return; // đang phát đúng bài này rồi thì thôi

            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(CrossfadeBGM(clip, loop));
        }

        public void StopBGM()
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _bgmSource.Stop();
        }

        private System.Collections.IEnumerator CrossfadeBGM(AudioClip newClip, bool loop)
        {
            float startVolume = _bgmSource.volume;

            // Fade out bài cũ
            float t = 0f;
            while (t < bgmFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                _bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / bgmFadeDuration);
                yield return null;
            }

            _bgmSource.clip = newClip;
            _bgmSource.loop = loop;
            _bgmSource.Play();

            // Fade in bài mới
            t = 0f;
            while (t < bgmFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                _bgmSource.volume = Mathf.Lerp(0f, startVolume, t / bgmFadeDuration);
                yield return null;
            }
            _bgmSource.volume = startVolume;
        }

        // ---------------- VOLUME SETTINGS ----------------
        public void SetMasterVolume(float linear01) => SetVolume(masterVolumeParam, PrefMaster, linear01);
        public void SetBGMVolume(float linear01) => SetVolume(bgmVolumeParam, PrefBGM, linear01);
        public void SetSFXVolume(float linear01) => SetVolume(sfxVolumeParam, PrefSFX, linear01);

        public float GetMasterVolume() => PlayerPrefs.GetFloat(PrefMaster, 1f);
        public float GetBGMVolume() => PlayerPrefs.GetFloat(PrefBGM, 1f);
        public float GetSFXVolume() => PlayerPrefs.GetFloat(PrefSFX, 1f);

        private void SetVolume(string mixerParam, string prefKey, float linear01)
        {
            linear01 = Mathf.Clamp(linear01, 0.0001f, 1f); // tránh log10(0) = âm vô cực
            float db = Mathf.Log10(linear01) * 20f;
            mixer.SetFloat(mixerParam, db);
            PlayerPrefs.SetFloat(prefKey, linear01);
        }

        private void LoadSavedVolumes()
        {
            SetMasterVolume(GetMasterVolume());
            SetBGMVolume(GetBGMVolume());
            SetSFXVolume(GetSFXVolume());
        }
    }
}

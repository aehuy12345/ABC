using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

namespace Game.UI.Settings
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void OnEnable()
        {
            if (AudioManager.Instance == null) return;

            // Gán giá trị hiện tại trước, rồi mới add listener -> tránh trigger callback không cần thiết lúc mở panel
            masterSlider.SetValueWithoutNotify(AudioManager.Instance.GetMasterVolume());
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.GetBGMVolume());
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());

            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
            bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        private void OnDisable()
        {
            if (AudioManager.Instance == null) return;

            masterSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMasterVolume);
            bgmSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetBGMVolume);
            sfxSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);
        }
    }
}

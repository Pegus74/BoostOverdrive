using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MusicSettingsUI : MonoBehaviour
{
    [FormerlySerializedAs("volumeSlider")] public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public Toggle musicToggle;

    private void Start()
    {
        if (RMusicManager.Instance != null)
        {
            // Устанавливаем значения UI из PlayerPrefs
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            soundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
            
            musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        }

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        
    }

    private void OnMusicVolumeChanged(float value)
    {
        RMusicManager.Instance.SetMusicVolume(value);
    }

    private void OnSoundVolumeChanged(float value)
    {
        RMusicManager.Instance.SetSoundsVolume(value);
    }

    private void OnMusicToggled(bool isOn)
    {
        RMusicManager.Instance.ToggleMusic(isOn);
    }
}
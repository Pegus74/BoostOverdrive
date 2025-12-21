using UnityEngine;
using UnityEngine.UI;

public class MusicSettingsUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;

    private void Start()
    {
        if (RMusicManager.Instance != null)
        {
            // Устанавливаем значения UI из PlayerPrefs
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        }

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
    }

    private void OnVolumeChanged(float value)
    {
        RMusicManager.Instance.SetVolume(value);
    }

    private void OnMusicToggled(bool isOn)
    {
        RMusicManager.Instance.ToggleMusic(isOn);
    }
}
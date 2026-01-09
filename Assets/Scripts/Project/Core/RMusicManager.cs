using UnityEngine;

public class RMusicManager : MonoBehaviour
{
    public static RMusicManager Instance; 

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private AudioSource audioSource;

    private const string VolumeKey = "MusicVolume";
    private const string SoundVolumeKey = "SoundVolume";
    private const string MusicOnKey = "MusicOn";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        // Загружаем настройки
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);
        bool savedMusicOn = PlayerPrefs.GetInt(MusicOnKey, 1) == 1;

        audioSource.volume = savedVolume;

        if (savedMusicOn)
            audioSource.Play();
        else
            audioSource.Stop();
    }

    private void OnEnable()
    {
        GameEvents.OnGameMusicStart.AddListener(PlayGameMusic);
        GameEvents.OnMenuMusicStart.AddListener(PlayMenuMusic);
    }

    private void OnDisable()
    {
        GameEvents.OnGameMusicStart.RemoveListener(PlayGameMusic);
        GameEvents.OnMenuMusicStart.RemoveListener(PlayMenuMusic);
    }

    // set - с сохранением настройки в PlayerPrefs
    // change - просто смена
    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            PlayerPrefs.SetFloat(VolumeKey, volume);
        }
    }

    public void ChangeMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    public void SetSoundsVolume(float volume)
    {
        if (audioSource != null)
        {
            PlayerPrefs.SetFloat(SoundVolumeKey, volume);
        }
    }

    public void ChangeSoundVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    public void ChangePitch(float pitch)
    {
        if (audioSource != null)
        {
            audioSource.pitch = pitch;
        }
    }

    public void ToggleMusic(bool isOn)
    {
        if (audioSource != null)
        {
            if (isOn)
                audioSource.Play();
            else
                audioSource.Stop();

            PlayerPrefs.SetInt(MusicOnKey, isOn ? 1 : 0); // сохраняем состояние музыки
        }
    }

    private void PlayMenuMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = menuMusic;
            if (PlayerPrefs.GetInt(MusicOnKey, 1) == 1)
                audioSource.Play();
        }
        Debug.Log("[MusicManager] Play MenuMusic");
    }

    private void PlayGameMusic()
    {
        if (audioSource.clip != gameMusic)
        {
            audioSource.Stop();
            audioSource.clip = gameMusic;
            if (PlayerPrefs.GetInt(MusicOnKey, 1) == 1)
                audioSource.Play();
        }
        Debug.Log("[MusicManager] Play GameMusic");
    }
}

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

public static class ExtendedGameEvents
{
    public static IntEvent OnLevelChanged = new IntEvent();
}

public class RMusicManager : MonoBehaviour
{
    public static RMusicManager Instance;
    public MusicSettings musicSettings;

    private AudioSource audioSource;
    private int currentLevel = 1;
    private AudioClip currentGameMusicClip;

    private const string VolumeKey = "MusicVolume";
    private const string SoundVolumeKey = "SoundVolume";
    private const string MusicOnKey = "MusicOn";
    private const string CurrentLevelKey = "CurrentLevel";

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
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.7f);
        bool savedMusicOn = PlayerPrefs.GetInt(MusicOnKey, 1) == 1;
        audioSource.volume = savedVolume;

        if (musicSettings == null)
        {
            musicSettings = Resources.Load<MusicSettings>("MusicSettings");
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGameMusicStart.AddListener(PlayGameMusic);
        GameEvents.OnMenuMusicStart.AddListener(PlayMenuMusic);
        ExtendedGameEvents.OnLevelChanged.AddListener(OnLevelChanged);
    }

    private void OnDisable()
    {
        GameEvents.OnGameMusicStart.RemoveListener(PlayGameMusic);
        GameEvents.OnMenuMusicStart.RemoveListener(PlayMenuMusic);
        ExtendedGameEvents.OnLevelChanged.RemoveListener(OnLevelChanged);
    }

    public void PlayMenuMusic()
    {
        if (audioSource == null || musicSettings == null || musicSettings.menuMusic == null)
            return;

        audioSource.Stop();
        audioSource.clip = musicSettings.menuMusic;
        currentGameMusicClip = null;

        if (PlayerPrefs.GetInt(MusicOnKey, 1) == 1)
            audioSource.Play();
    }

    public void PlayGameMusic()
    {
        PlayMusicForLevel(currentLevel);
    }

    public void PlayMusicForLevel(int level)
    {
        if (audioSource == null || musicSettings == null)
            return;

        AudioClip clipToPlay = GetMusicForLevel(level);

        if (clipToPlay == null) return;

        if (clipToPlay != currentGameMusicClip)
        {
            audioSource.Stop();
            audioSource.clip = clipToPlay;
            currentGameMusicClip = clipToPlay;

            if (PlayerPrefs.GetInt(MusicOnKey, 1) == 1)
                audioSource.Play();
        }
        else if (!audioSource.isPlaying && PlayerPrefs.GetInt(MusicOnKey, 1) == 1)
        {
            audioSource.Play();
        }
    }

    private AudioClip GetMusicForLevel(int level)
    {
        if (musicSettings == null) return null;

        foreach (var range in musicSettings.levelMusicRanges)
        {
            if (level >= range.startLevel && level <= range.endLevel)
                return range.musicClip;
        }

        return musicSettings.defaultMusic;
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Max(1, level);
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);

        if (audioSource.clip == currentGameMusicClip && audioSource.isPlaying)
            PlayMusicForLevel(currentLevel);
    }

    private void OnLevelChanged(int newLevel)
    {
        SetLevel(newLevel);
    }

    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(VolumeKey, volume);
        }
    }

    public void ChangeMusicVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSoundsVolume(float volume)
    {
        PlayerPrefs.SetFloat(SoundVolumeKey, Mathf.Clamp01(volume));
    }

    public void ChangePitch(float pitch)
    {
        if (audioSource != null)
            audioSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
    }

    public void ToggleMusic(bool isOn)
    {
        if (audioSource != null)
        {
            if (isOn && !audioSource.isPlaying)
                audioSource.Play();
            else if (!isOn && audioSource.isPlaying)
                audioSource.Stop();

            PlayerPrefs.SetInt(MusicOnKey, isOn ? 1 : 0);
        }
    }
}
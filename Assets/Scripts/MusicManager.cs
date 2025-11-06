using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private AudioSource audioSource;

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
        
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    public void SetPitch(float pitch) 
    { 
        if (audioSource != null) 
            audioSource.pitch = pitch;
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void PlayMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Play();
    }

    public void PlayMenuMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.resource = menuMusic;
            audioSource.Play();
        }
    }
    
    public void PlayGameMusic()
    {
        if (audioSource.resource != gameMusic)
        {
            audioSource.Stop();
            audioSource.resource = gameMusic;
            audioSource.Play();
        }
    }
    
}
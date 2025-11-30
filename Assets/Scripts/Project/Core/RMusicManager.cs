using System;
using UnityEngine;

public class RMusicManager : MonoBehaviour
{
    public static RMusicManager Instance; 

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

    private void PlayMenuMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.resource = menuMusic;
            audioSource.Play();
        }
        Debug.Log("[MusicManager] Play MenuMusic");
    }
    
    private void PlayGameMusic()
    {
        if (audioSource.resource != gameMusic)
        {
            audioSource.Stop();
            audioSource.resource = gameMusic;
            audioSource.Play();
        }
        Debug.Log("[MusicManager] Play GameMusic");
    }
    
}
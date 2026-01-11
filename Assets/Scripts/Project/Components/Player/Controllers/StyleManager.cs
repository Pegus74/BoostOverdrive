using UnityEngine;

public class StyleManager : MonoBehaviour
{
    public PlayerStateModel playerStateModel;

    [SerializeField] private AudioClip switchSound;

    private AudioSource audioSource;

    private void OnEnable() => InputEvents.ToggleStyleAttemptEvent.AddListener(SwitchStyle);
    private void OnDisable() => InputEvents.ToggleStyleAttemptEvent.RemoveListener(SwitchStyle);

    private void Awake()
    {
        if (playerStateModel == null || playerStateModel.settings?.styleDataAssets == null) enabled = false;
        else playerStateModel.ApplyStyleToModel(0);
        audioSource = GetComponent<AudioSource>();
    }

    private void SwitchStyle()
    {
        if (NewGameManager.Instance.GetCurrentState() != GameState.Playing) return;
        
        PlayerEvents.OnStyleChangedEvent.Invoke();

        audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
	    audioSource.PlayOneShot(switchSound);

        int newIndex = (playerStateModel.CurrentStyleIndex + 1) % playerStateModel.settings.styleDataAssets.Length;
        playerStateModel.ApplyStyleToModel(newIndex);
    }
}

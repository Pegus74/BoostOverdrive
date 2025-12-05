using UnityEngine;

public class StyleManager : MonoBehaviour
{
    public PlayerStateModel playerStateModel;

    private void OnEnable() => InputEvents.ToggleStyleAttemptEvent.AddListener(SwitchStyle);
    private void OnDisable() => InputEvents.ToggleStyleAttemptEvent.RemoveListener(SwitchStyle);

    private void Awake()
    {
        if (playerStateModel == null || playerStateModel.settings?.styleDataAssets == null) enabled = false;
        else playerStateModel.ApplyStyleToModel(0);
    }

    private void SwitchStyle()
    {
        int newIndex = (playerStateModel.CurrentStyleIndex + 1) % playerStateModel.settings.styleDataAssets.Length;
        playerStateModel.ApplyStyleToModel(newIndex);
    }
}
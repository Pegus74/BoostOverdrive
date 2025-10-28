using UnityEngine;

/// <summary>
/// Слушает FloatEvent и обновляет MovementSpeedModifier в PlayerStateModel.
/// </summary>
public class SpeedModifierController : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;
    
    [Header("Event Listener")]
    public FloatEvent OnPlayerSpeedModifierChange;

    void OnEnable()
    {
        OnPlayerSpeedModifierChange?.RegisterListener(UpdateSpeedModifier);
    }

    void OnDisable()
    {
        OnPlayerSpeedModifierChange?.UnregisterListener(UpdateSpeedModifier);
    }

    /// <summary>
    /// Вызывается при срабатывании FloatEvent.
    /// </summary>
    private void UpdateSpeedModifier(float newModifier)
    {
        if (playerStateModel != null)
        {
            playerStateModel.SetMovementSpeedModifier(newModifier);
        }
    }
}
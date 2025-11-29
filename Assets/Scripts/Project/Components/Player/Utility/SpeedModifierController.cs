using UnityEngine;

/// <summary>
/// Слушает FloatEvent и обновляет MovementSpeedModifier в PlayerStateModel.
/// </summary>
public class SpeedModifierController : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerSpeedModifierChange.AddListener(UpdateSpeedModifier);
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerSpeedModifierChange.RemoveListener(UpdateSpeedModifier);
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
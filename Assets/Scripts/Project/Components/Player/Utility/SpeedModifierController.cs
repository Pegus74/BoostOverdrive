using UnityEngine;

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

 
    private void UpdateSpeedModifier(float newModifier)
    {
        if (playerStateModel != null)
        {
            Debug.Log("new modifier: " + newModifier);
            playerStateModel.SetMovementSpeedModifier(newModifier);
        }
    }
}
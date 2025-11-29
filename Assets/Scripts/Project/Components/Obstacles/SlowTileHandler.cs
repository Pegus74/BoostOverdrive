using UnityEngine;

/// <summary>
/// Обрабатывает замедление игрока, издавая событие для изменения модификатора скорости.
/// </summary>
public class SlowTileHandler : MonoBehaviour
{
    [Header("Settings")]
    public ObstaclesSettingsData obstaclesSettingsData;
    
    private const float NormalSpeed = 1.0f;
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            ApplySlowdown();
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            RemoveSlowdown();
            isPlayerInside = false;
        }
    }
    
    private void ApplySlowdown()
    {
        if (obstaclesSettingsData != null)
        {
            float slowdown = obstaclesSettingsData.slowTileSlowdownMultiplier;
            PlayerEvents.OnPlayerSpeedModifierChange.Invoke(slowdown);
            Debug.Log($"[Slow Tile] Speed Modifier Change Event: {slowdown}");
        }
    }
    
    private void RemoveSlowdown()
    {
        PlayerEvents.OnPlayerSpeedModifierChange.Invoke(NormalSpeed);
        Debug.Log($"[Slow Tile] Speed Modifier Change Event: {NormalSpeed}");
    }
}
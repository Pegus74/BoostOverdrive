using UnityEngine;

/// <summary>
/// Обрабатывает замедление игрока, издавая событие для изменения модификатора скорости.
/// </summary>
public class SlowTileHandler : MonoBehaviour
{
    [Header("Settings")]
    public ObstaclesSettingsData obstaclesSettingsData;

    [Header("Event to Raise")]
    // Ссылка на ScriptableObject FloatEvent
    public FloatEvent OnPlayerSpeedModifierChange;

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

    /// <summary>
    /// Вызывает событие с множителем замедления.
    /// </summary>
    private void ApplySlowdown()
    {
        if (obstaclesSettingsData != null)
        {
            float slowdown = obstaclesSettingsData.slowTileSlowdownMultiplier;
            OnPlayerSpeedModifierChange.Raise(slowdown);
            Debug.Log($"[Slow Tile] Speed Modifier Change Event: {slowdown}");
        }
    }

    /// <summary>
    /// Вызывает событие для восстановления нормальной скорости.
    /// </summary>
    private void RemoveSlowdown()
    {
        OnPlayerSpeedModifierChange.Raise(NormalSpeed);
        Debug.Log($"[Slow Tile] Speed Modifier Change Event: {NormalSpeed}");
    }
}
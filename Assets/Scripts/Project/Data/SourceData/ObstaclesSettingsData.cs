using UnityEngine;

// Данные, которые не меняются во время игры для препятсвий
[CreateAssetMenu(fileName = "NewObstaclesettings", menuName = "GameData/Obstacles Settings Data")]
public class ObstaclesSettingsData : ScriptableObject
{
    [Header("Настройки Slow Tile")]
    public float slowTileSlowdownMultiplier = 0.7f;
}
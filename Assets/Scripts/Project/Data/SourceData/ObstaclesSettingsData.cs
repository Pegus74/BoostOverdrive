using UnityEngine;

[CreateAssetMenu(fileName = "NewObstaclesettings", menuName = "GameData/Obstacles Settings Data")]
public class ObstaclesSettingsData : ScriptableObject
{
    [Header("Настройки Slow Tile")]
    public float slowTileSlowdownMultiplier = 0.7f;
    
    [Header("Настройки Boost Tile")]
    public float boostTileMultiplier = 1.3f;
    public float boostTileGroundLingerTime = 0.5f;
    public float boostTileJumpLingerTime = 0.2f;
    
    [Header("Настройки Destructible Wall")]
    public float explosionForce = 10f;
    public float explosionRadius = 5f;
    
    [Header("Настройки SpringWall")]
    public float reboundForceHands = 15f;
    public float extraAccelerationHands = 10f;
    
    public float horizontalForceLegs = 10f;
    public float verticalForceLegs = 15f;
    
    [Header("Настройки CrackedPlatform&DryStip")]
    public float CrackPlatDestroyDelay = 1.0f;
    
    [Header("Настройки DryStip")]
    public float DryStripDestroyDelay = 1.0f;
}
using UnityEngine;

// Данные, которые не меняются во время игры для игрока
[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "GameData/Player Settings Data")]
public class PlayerSettingsData : ScriptableObject
{
    [Header("Настройки Камеры")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;
    public bool invertCamera = false;
    public bool lockCursor = true;
    public bool cameraCanMove = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;
    
    [Header("Настройки Движения")]
    public float maxVelocityChange = 10f;
    public bool enableAirJump = false;
    public bool playerCanMove = true;
    public bool enableJump = true;
    
    [Header("Настройки Dash")]
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.0f;
    
    [Header("Настройки CrawlSlide")]
    public float slideBaseImpulse = 10f;
    public float slideDuration = 1.5f;
    public float squatHeightScale = 0.5f;
    public float squatTransitionDuration = 0.3f;
    public float slideCooldown = 0.5f;
    
    [Header("Настройки Slam")]
    public float slamDuration = 0.15f;
    public float slamCooldown = 1.0f;
    public float slamSlowdownDuration = 0.5f;
    public float destructibleWallReboundMultiplier = 100.0f;
    
    [Header("Настройки Взаимодействия со Стенами")]
    public float raycastCheckDistance = 1.0f;
    public float horizontalForceHands = 10f;
    public float verticalForceLegs = 15f;
}

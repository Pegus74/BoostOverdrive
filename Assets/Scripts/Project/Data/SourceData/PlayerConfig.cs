using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "GameData/Player Config Data")]
public class PlayerConfig : ScriptableObject
{
    public PlayerStyleData[] styleDataAssets;

    [Header("Настройки Камеры")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;
    public float bobSpeed = 5f;
    public bool invertCamera = false;
    public bool lockCursor = true;
    public bool cameraCanMove = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;
    
    [Header("Настройки Движения")]
    public float maxVelocityChange = 10f;
    public float acceleration = 20f;
    public bool playerCanMove = true;
    public bool enableJump = true;
    public bool enableAirJump = false;

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
    public float slamCooldown = 1.0f;
    public float slamSlowdownDuration = 0.5f;
    public float destructibleWallReboundMultiplier = 100.0f;
    
    [Header("Настройки Взаимодействия со Стенами")]
    public float raycastCheckDistance = 1.0f;
    public float horizontalForceHands = 10f;
    public float verticalForceLegs = 15f;

    [Header("Настройки заряженного прыжка")]
    public bool enableChargedJump = true;
    public float chargeSpeed = 2f;
    public float minJumpPowerPercent = 0.35f;

    [Header("Настройки прилипания к склону")]
    public float groundStickForce = 8f;       
    public float groundStickMaxSlope = 75f;

    [Header("Настройки буфера прыжка")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;
 
   


}

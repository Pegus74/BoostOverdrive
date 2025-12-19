using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "GameData/Player Config Data")]
public class PlayerConfig : ScriptableObject
{
    public PlayerStyleData[] styleDataAssets;

    [Header("Настройки Камеры")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 60f;
    public float bobSpeed = 5f;
    public bool invertCamera = false;
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;
    
    [Header("Настройки Движения")]
    public float maxVelocityChange = 10f;
    public float acceleration = 50f;
    public bool enableJump = true;

    [Header("Настройки Dash")]
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.0f;
    
    [Header("Настройки CrawlSlide")]
    public float slideDuration = 2f;
    [FormerlySerializedAs("initialSpeedMultiplier")] public float SlideSpeedMultiplier = 1.2f;
    public float FinalSlideSpeedMultiplier = 1f;
    public float squatHeightScale = 0.5f;
    public float squatTransitionDuration = 0.5f;
    
    [Header("Настройки Slam")]
    public float slamCooldown = 1.0f;
    public float slamSlowdownDuration = 0.5f;
    public float destructibleWallReboundMultiplier = 10.0f;
    
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

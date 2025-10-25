using UnityEngine;

// Данные, которые не меняются во время игры
[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "GameData/Player Settings Data")]
public class PlayerSettingsData : ScriptableObject
{
    [Header("Настройки Камеры")]
    [Tooltip("Чувствительность мыши")]
    public float mouseSensitivity = 2f;
    
    [Tooltip("Максимальный угол поворота камеры по вертикали (Pitch)")]
    public float maxLookAngle = 50f;
    
    [Header("Настройки Движения")]
    [Tooltip("Максимальное изменение скорости за один кадр физики (maxVelocityChange)")]
    public float maxVelocityChange = 10f;
    
    public bool enableAirJump = false;
    public bool enableSlam = true;
    public bool playerCanMove = true;
    public bool enableJump = true;
    
    [Header("Настройки Cпособностей")]
    [Tooltip("Длительность рывка")]
    public float dashDuration = 0.15f;
    [Tooltip("Время перезарядки рывка (cooldown)")]
    public float dashCooldown = 1.0f;
    
    [Header("Настройки Crawl/Slide (Слайд)")]
    [Tooltip("Базовый импульс, прикладываемый в начале слайда.")]
    public float slideBaseImpulse = 10f;
    [Tooltip("Длительность фазы активного скольжения.")]
    public float slideDuration = 1.5f;
    [Tooltip("Масштаб игрока по Y при сжатии")]
    public float squatHeightScale = 0.5f;
    [Tooltip("Продолжительность анимации сжатия/разжимания")]
    public float squatTransitionDuration = 0.3f;
    [Tooltip("Время перезарядки слайда")]
    public float slideCooldown = 0.5f;
    
    [Header("Настройки Взаимодействия со Стенами")]
    [Tooltip("Расстояние для проверки столкновения со стеной (raycastCheckDistance)")]
    public float raycastCheckDistance = 1.0f;
    
    [Tooltip("Сила горизонтального отталкивания при взаимодействии со стеной (Hands style)")]
    public float horizontalForceHands = 10f;
    
    [Tooltip("Сила вертикального отталкивания при взаимодействии со стеной (Legs style)")]
    public float verticalForceLegs = 15f;
}

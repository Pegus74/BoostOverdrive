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

    [Tooltip("Продолжительность, за которую камера должна повернуться (для SmoothlyRotateCameraYaw)")]
    public float cameraRotationDuration = 0.5f;

    [Header("Настройки Движения")]
    [Tooltip("Максимальное изменение скорости за один кадр физики (maxVelocityChange)")]
    public float maxVelocityChange = 10f;
    
    [Tooltip("Длительность рывка")]
    public float dashDuration = 0.15f;
    
    [Tooltip("Время перезарядки рывка (cooldown)")]
    public float dashCooldown = 1.0f;
    
    [Tooltip("Коэффициент скорости для первого рывка/скольжения")]
    public float initialSpeedMultiplier = 2.0f;
    
    [Header("Настройки Взаимодействия со Стенами")]
    [Tooltip("Расстояние для проверки столкновения со стеной (raycastCheckDistance)")]
    public float raycastCheckDistance = 1.0f;
    
    [Tooltip("Сила горизонтального отталкивания при взаимодействии со стеной (Hands style)")]
    public float horizontalForceHands = 10f;
    
    [Tooltip("Сила вертикального отталкивания при взаимодействии со стеной (Legs style)")]
    public float verticalForceLegs = 15f;
}
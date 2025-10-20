using UnityEngine;

/// <summary>
/// Хранит изменяемые параметры игрока. Все контроллеры пишут и читают из него
/// </summary>
[CreateAssetMenu(fileName = "PlayerStateModel", menuName = "GameData/Player State Model")]
public class PlayerStateModel : ScriptableObject
{
    // Приватные поля для хранения данных (НЕ МЕНЯЮТСЯ напрямую извне)
    private float _currentWalkSpeed = 5f;
    private float _currentJumpPower = 10f;
    private bool _isGrounded = false;
    private int _currentStyleIndex = 0;
    
    // Геттеры для чтения состояния извне
    [Header("Рабочие Параметры (GET-Only)")]
    public float CurrentWalkSpeed => _currentWalkSpeed;
    public float CurrentJumpPower => _currentJumpPower;

    [Header("Текущее Состояние (GET-Only)")]
    public bool IsGrounded => _isGrounded;
    public int CurrentStyleIndex => _currentStyleIndex;

    // Ссылки на ассеты событий
    [Header("Уведомления об Изменении Состояния")]
    public IntEvent OnStyleChangedEvent; 
    public BoolEvent OnGroundedStateChangedEvent; 
    // public FloatEventSO OnSpeedChangedEvent;

    /// <summary>
    /// Устанавливает новую скорость ходьбы и оповещает слушателей, если значение изменилось.
    /// </summary>
    public void SetWalkSpeed(float newSpeed)
    {
        if (_currentWalkSpeed != newSpeed)
        {
            _currentWalkSpeed = newSpeed;
            // OnSpeedChangedEvent.Raise(newSpeed);
            Debug.Log($"[Model] Walk Speed updated to: {newSpeed}");
        }
    }

    /// <summary>
    /// Устанавливает новую силу прыжка и оповещает слушателей
    /// </summary>
    public void SetJumpPower(float newPower)
    {
        _currentJumpPower = newPower;
    }
    
    /// <summary>
    /// Устанавливает новое состояние нахождения на земле и оповещает слушателей
    /// </summary>
    public void SetIsGrounded(bool isGrounded)
    {
        if (_isGrounded != isGrounded)
        {
            _isGrounded = isGrounded;
            OnGroundedStateChangedEvent.Raise(isGrounded);
            Debug.Log($"[Model] IsGrounded updated to: {isGrounded}");
        }
    }

    /// <summary>
    /// Устанавливает новый индекс стиля и оповещает слушателей
    /// </summary>
    public void SetStyleIndex(int newIndex)
    {
        if (_currentStyleIndex != newIndex)
        {
            _currentStyleIndex = newIndex;
            OnStyleChangedEvent.Raise(newIndex);
            Debug.Log($"[Model] Style Index updated to: {newIndex}");
        }
    }
    
    /// <summary>
    /// При инициализации устанавливаем базовые значения из PlayerStyleData
    /// </summary>
    public void ResetState(PlayerStyleData initialStyle)
    {
        SetWalkSpeed(initialStyle.walkSpeed);
        SetJumpPower(initialStyle.jumpPower);
        SetStyleIndex(0);
        SetIsGrounded(true);
    }
}
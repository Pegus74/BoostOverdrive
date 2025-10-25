using UnityEngine;

/// <summary>
/// Хранит изменяемые параметры игрока. Все контроллеры пишут и читают из него
/// </summary>
[CreateAssetMenu(fileName = "PlayerStateModel", menuName = "GameData/Player State Model")]
public class PlayerStateModel : ScriptableObject
{
    // Приватные поля для хранения данных (НЕ МЕНЯЮТСЯ напрямую извне)
    private float _currentWalkSpeed;
    private float _currentJumpPower;
    private float _currentDashPower;
    private bool _isGrounded = false;
    private bool _isDashing = false;
    private int _currentStyleIndex = 0;
    
    // Геттеры для чтения состояния извне
    [Header("Рабочие Параметры (GET-Only)")]
    public float CurrentWalkSpeed => _currentWalkSpeed;
    public float CurrentJumpPower => _currentJumpPower;
    public float CurrentDashPower => _currentDashPower;

    [Header("Текущее Состояние (GET-Only)")]
    public bool IsGrounded => _isGrounded;
    public bool IsDashing => _isDashing;

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
    
    public void SetDashPower(float newPower)
    {
        if (_currentDashPower != newPower)
        {
            _currentDashPower = newPower;
            // Здесь можно вызвать отдельное событие, если другие системы
            // должны реагировать на изменение только множителя дэша
            // Например: OnDashMultiplierChangedEvent.Raise(newMultiplier);
        
            Debug.Log($"[Model] Dash Multiplier updated to: {newPower}");
        }
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
    /// Устанавливает новое состояние дэша и оповещает слушателей
    /// </summary>
    public void SetIsDashing(bool isDashing)
    {
        if (_isDashing != isDashing)
        {
            _isDashing = isDashing;
            OnGroundedStateChangedEvent.Raise(isDashing);
            Debug.Log($"[Model] isDashing updated to: {isDashing}");
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
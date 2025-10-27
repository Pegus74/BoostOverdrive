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
    private float _currentSlamPower;
    private float _movementSpeedModifier;
    
    private bool _isGrounded;
    private bool _isDashing;
    private bool _isSliding;
    private bool _isSlamming;
    
    private int _currentStyleIndex = 0;
    
    // Геттеры для чтения состояния извне
    public float CurrentWalkSpeed => _currentWalkSpeed;
    public float CurrentJumpPower => _currentJumpPower;
    public float CurrentDashPower => _currentDashPower;
    public float CurrentSlamPower => _currentSlamPower;
    public float MovementSpeedModifier => _movementSpeedModifier;
    
    public bool IsGrounded => _isGrounded;
    public bool IsDashing => _isDashing;
    public bool IsSliding => _isSliding;
    public bool IsSlamming => _isSlamming;

    public int CurrentStyleIndex => _currentStyleIndex;
    
    [Header("Уведомления об Изменении Состояния")]
    public IntEvent OnStyleChangedEvent; 
    public BoolEvent OnGroundedStateChangedEvent; 
    
    
    public void SetWalkSpeed(float newSpeed)
    {
        if (_currentWalkSpeed != newSpeed)
        {
            _currentWalkSpeed = newSpeed;
            // OnSpeedChangedEvent.Raise(newSpeed);
            Debug.Log($"[Model] Walk Speed updated to: {newSpeed}");
        }
    }
    
    public void SetJumpPower(float newPower)
    {
        _currentJumpPower = newPower;
        Debug.Log($"[Model] Jump Speed updated to: {newPower}");
    }
    
    public void SetDashPower(float newPower)
    {
        if (_currentDashPower != newPower)
        {
            _currentDashPower = newPower;
            Debug.Log($"[Model] Dash Multiplier updated to: {newPower}");
        }
    }

    public void SetSlamPower(float newPower)
    {
        if (_currentSlamPower != newPower)
        {
            _currentSlamPower = newPower;
            Debug.Log($"[Model] Slam Multiplier updated to: {newPower}");
        }
    }
    
    public void SetIsGrounded(bool isGrounded)
    {
        if (_isGrounded != isGrounded)
        {
            _isGrounded = isGrounded;
            OnGroundedStateChangedEvent.Raise(isGrounded);
            Debug.Log($"[Model] IsGrounded updated to: {isGrounded}");
        }
    }
    
    public void SetIsDashing(bool isDashing)
    {
        if (_isDashing != isDashing)
        {
            _isDashing = isDashing;
            OnGroundedStateChangedEvent.Raise(isDashing);
            Debug.Log($"[Model] isDashing updated to: {isDashing}");
        }
    }

    public void SetIsSliding(bool isSliding)
    {
        if (_isSliding != isSliding)
        {
            _isSliding = isSliding;
            OnGroundedStateChangedEvent.Raise(isSliding);
            Debug.Log($"[Model] isSliding updated to: {isSliding}");
        }
    }

    public void SetIsSlamming(bool isSlamming)
    {
        if (_isSlamming != isSlamming)
        {
            _isSlamming = isSlamming;
            Debug.Log($"[Model] isSlamming updated to: {isSlamming}");
        }
    }
    
    public void SetStyleIndex(int newIndex)
    {
        if (_currentStyleIndex != newIndex)
        {
            _currentStyleIndex = newIndex;
            OnStyleChangedEvent.Raise(newIndex);
            Debug.Log($"[Model] Style Index updated to: {newIndex}");
        }
    }

    // public void SetMovementSpeedModifier(float newSpeedModifier)
    // {
    //     if (_movementSpeedModifier != newSpeedModifier)
    //     {
    //         _movementSpeedModifier = newSpeedModifier;
    //         Debug.Log($"[Model] Movement Speed modifier updated to: {newSpeedModifier}");
    //     }
    // }
    
    public void ResetState(PlayerStyleData initialStyle)
    {
        SetWalkSpeed(initialStyle.walkSpeed);
        SetJumpPower(initialStyle.jumpPower);
        SetDashPower(initialStyle.dashPower);
        SetSlamPower(initialStyle.slamPower);
        SetStyleIndex(0);
        SetIsGrounded(false);
        SetIsDashing(false);
        SetIsSliding(false);
    }
}
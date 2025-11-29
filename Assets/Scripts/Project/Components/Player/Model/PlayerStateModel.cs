using System;
using UnityEngine;

public class PlayerStateModel : MonoBehaviour
{
    private float _currentWalkSpeed;
    private float _currentJumpPower;
    private float _currentDashPower;
    private float _currentSlamPower;
    private float _movementSpeedModifier = 1f;
    
    private bool _isGrounded;
    private bool _isDashing;
    private bool _isSliding;
    private bool _isSlamming;
    private bool _isWalking;
    private bool isChargingJump;

    private int _currentStyleIndex = 0;
    
    private Component _lastWallJumpedFrom;

    private Vector3 groundNormal;
    
    public float CurrentWalkSpeed => _currentWalkSpeed;
    public float CurrentJumpPower => _currentJumpPower;
    public float CurrentDashPower => _currentDashPower;
    public float CurrentSlamPower => _currentSlamPower;
    public float MovementSpeedModifier => _movementSpeedModifier;
    
    public bool IsGrounded => _isGrounded;
    public bool IsDashing => _isDashing;
    public bool IsSliding => _isSliding;
    public bool IsSlamming => _isSlamming;
    public bool IsChargingJump => isChargingJump;

    public bool IsWalking()
    {
        if (_currentWalkSpeed > 0.01f)
            _isWalking = true;
        else 
            _isWalking = false;
        return _isWalking;
    }

    public int CurrentStyleIndex => _currentStyleIndex;
    
    public Component LastWallJumpedFrom => _lastWallJumpedFrom;
    public Vector3 GroundNormal => groundNormal;
    
    // [Header("Уведомления об Изменении Состояния")]
    // // public IntEvent OnStyleChangedEvent; 
    // // public BoolEvent OnGroundedStateChangedEvent;


    private void OnEnable()
    {
        _lastWallJumpedFrom = null;
    }

    public void SetWalkSpeed(float newSpeed)
    {
        if (_currentWalkSpeed != newSpeed)
        {
            _currentWalkSpeed = newSpeed;
            Debug.Log($"[Model] Walk Speed updated to: {newSpeed}");
        }
    }
    
    public void SetJumpPower(float newPower)
    {
        _currentJumpPower = newPower;
        Debug.Log($"[Model] Jump Speed updated to: {newPower}");
    }

    public void SetIsChargingJump(bool charging)
    {
        isChargingJump = charging;
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

    public void SetMovementSpeedModifier(float newSpeedModifier)
    {
        _movementSpeedModifier = newSpeedModifier;
    }
    
    public void SetIsGrounded(bool isGrounded)
    {
        if (_isGrounded != isGrounded)
        {
            _isGrounded = isGrounded;
            Debug.Log($"[Model] IsGrounded updated to: {isGrounded}");
        }
    }
    
    public void SetIsDashing(bool isDashing)
    {
        if (_isDashing != isDashing)
        {
            _isDashing = isDashing;
            Debug.Log($"[Model] isDashing updated to: {isDashing}");
        }
    }

    public void SetIsSliding(bool isSliding)
    {
        if (_isSliding != isSliding)
        {
            _isSliding = isSliding;
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
            Debug.Log($"[Model] Style Index updated to: {newIndex}");
        }
    }

    public void SetLastWallJumpedFrom(Component newWall)
    {
        if (_lastWallJumpedFrom != newWall)
        {
            _lastWallJumpedFrom = newWall;
            Debug.Log($"[Model] LastWallJumpedFrom updated to: {newWall}");
        }
    }
    
    public void SetGroundNormal(Vector3 normal)
    {
        groundNormal = normal;
    }
    
}
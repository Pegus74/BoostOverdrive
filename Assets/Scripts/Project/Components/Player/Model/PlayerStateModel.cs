using System;
using UnityEngine;

public class PlayerStateModel : MonoBehaviour
{
    [Header("Settings")]
    public PlayerConfig settings;

    private float _walkSpeed;
    private float _jumpPower;
    private float _dashPower;
    private float _slamPower;
    private float _movementSpeedModifier = 1f;

    private int _currentStyleIndex = 0;
    private bool _isGrounded;
    private bool _isDashing;
    private bool _isSliding;
    private bool _isSlamming;
    private bool _isChargingJump;

    public void ClearLastWallJumpedFrom() => _lastWallJumpedFrom = null;

    public bool playerCanMove = true;

    private Vector3 _groundNormal = Vector3.up;
    private float _coyoteCounter;
    private float _jumpBufferCounter;

    private Component _lastWallJumpedFrom;

    private Rigidbody _rb;
    
    private void Start()
    {
        if (settings != null)
        {
            ApplyStyleToModel(0);
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }


    public float CurrentWalkSpeed => _walkSpeed * _movementSpeedModifier;
    public float CurrentJumpPower => _jumpPower;
    public float CurrentDashPower => _dashPower;
    public float CurrentSlamPower => _slamPower;
    public float MovementSpeedModifier => _movementSpeedModifier;

    public int CurrentStyleIndex => _currentStyleIndex;
    public bool IsGrounded => _isGrounded;
    public bool IsDashing => _isDashing;
    public bool IsSliding => _isSliding;
    public bool IsSlamming => _isSlamming;
    public bool IsChargingJump => _isChargingJump;
    
    public Vector3 GroundNormal => _groundNormal;
    public float CoyoteCounter => _coyoteCounter;
    public float JumpBufferCounter => _jumpBufferCounter;
    public Component LastWallJumpedFrom => _lastWallJumpedFrom;

    public void SetWalkSpeed(float value) => _walkSpeed = value;
    public void SetJumpPower(float value) => _jumpPower = value;
    public void SetDashPower(float value) => _dashPower = value;
    public void SetSlamPower(float value) => _slamPower = value;
    public void SetMovementSpeedModifier(float value) => _movementSpeedModifier = value;
    public void SetStyleIndex(int value) => _currentStyleIndex = value;

    public void SetIsGrounded(bool value) => _isGrounded = value;
    public void SetIsDashing(bool value) => _isDashing = value;
    public void SetIsSliding(bool value) => _isSliding = value;
    public void SetIsSlamming(bool value) => _isSlamming = value;
    public void SetIsChargingJump(bool value) => _isChargingJump = value;

    public void SetGroundNormal(Vector3 normal) => _groundNormal = normal;
    public void SetLastWallJumpedFrom(Component wall) => _lastWallJumpedFrom = wall;

    public void UpdateCoyoteTime(float deltaTime)
    {
        if (_isGrounded) _coyoteCounter = settings.coyoteTime;
        else if (_coyoteCounter > 0f) _coyoteCounter -= deltaTime;
    }

    public void UpdateJumpBuffer(float deltaTime)
    {
        if (_jumpBufferCounter > 0f) _jumpBufferCounter -= deltaTime;
    }

    public void BufferJump() => _jumpBufferCounter = settings.jumpBufferTime;
    public void ResetJumpBuffer() => _jumpBufferCounter = 0f;
    public void ResetCoyoteTime() => _coyoteCounter = 0f;
    
    public Vector3 GetForwardVector() => transform.forward;
    
    public Vector3 GetCurrentHorizontalVelocity()
    {
        Vector3 vel = _rb.linearVelocity;
        vel.y = 0;
        return vel;
    }

    public void ApplyStyleToModel(int index)
    {
        if (settings == null || settings.styleDataAssets == null || index >= settings.styleDataAssets.Length) return;
        var style = settings.styleDataAssets[index];
        SetWalkSpeed(style.walkSpeed);
        SetJumpPower(style.jumpPower);
        SetDashPower(style.dashPower);
        SetSlamPower(style.slamPower);
        SetStyleIndex(index);
    }
}
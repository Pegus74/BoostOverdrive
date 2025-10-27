using UnityEngine;
using System.Collections;

public class DashSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listener")]
    public GameEvent DashAttemptEvent;
    
    private Rigidbody _rb;
    private bool _isDashAvailable = true;
    private Coroutine _currentDashCoroutine;
    
    private readonly bool allowHorizontalDashOnly = true; 

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            enabled = false;
        }
        playerStateModel.SetIsDashing(false);
    }

    void OnEnable()
    {
        DashAttemptEvent?.RegisterListener(InitiateDash);
    }

    void OnDisable()
    {
        DashAttemptEvent?.UnregisterListener(InitiateDash);
        
        if (_currentDashCoroutine != null)
        {
            StopCoroutine(_currentDashCoroutine);
            // Сброс состояния, если отключаемся во время дэша
            playerStateModel.SetIsDashing(false); 
            _isDashAvailable = true;
        }
    }

    /// <summary>
    /// Вызывается при срабатывании DashAttemptEvent.
    /// </summary>
    private void InitiateDash()
    {
        if (_rb == null || !_isDashAvailable || playerStateModel.IsDashing)
        {
            return;
        }
        
        if (_currentDashCoroutine != null)
        {
            StopCoroutine(_currentDashCoroutine);
        }
        
        _currentDashCoroutine = StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        _isDashAvailable = false;
        
        // 1. НАЧАЛО ДЭША
        playerStateModel.SetIsDashing(true); 

        // Определение направления
        Vector3 dashDirection = transform.forward; 

        if (allowHorizontalDashOnly)
        {
            dashDirection.y = 0f;
        }
        dashDirection.Normalize();

        // чтение параметров 
        float finalDashPower = playerStateModel.CurrentDashPower;
        float dashDuration = playerSettingsData.dashDuration;
        float dashCooldown = playerSettingsData.dashCooldown;
        
        _rb.AddForce(dashDirection * finalDashPower, ForceMode.Impulse);
        
        float dashTimer = dashDuration;
        
        while (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            
            float accelerationMagnitude = (finalDashPower * 0.5f) * (dashTimer / dashDuration);
            _rb.AddForce(dashDirection * accelerationMagnitude, ForceMode.Acceleration);
            
            yield return null;
        }
        
        playerStateModel.SetIsDashing(false); 
        
        float dashCooldownTimer = dashCooldown;
        while (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        _isDashAvailable = true;
        Debug.Log("Dash is now available.");
    }
}
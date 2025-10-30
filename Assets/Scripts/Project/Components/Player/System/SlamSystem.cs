using UnityEngine;
using System.Collections;

/// <summary>
/// Реализует способность "Slam": быстрое ускорение игрока строго вниз.
/// Управляет доступностью способности, ее движением и реакцией на коллизии (через события).
/// </summary>
public class SlamSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listener")]
    public GameEvent SlamAttemptEvent;
    
    [Header("Collision Events")] 
    public Vector3Event SlamDestructibleHitEvent; 
    public GameEvent SlamSolidHitEvent;           

    private Rigidbody _rb;
    private bool _isSlamAvailable = true;
    private Coroutine _currentSlamCoroutine;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            enabled = false;
        }
        playerStateModel.SetIsSlamming(false);
    }

    void OnEnable()
    {
        SlamAttemptEvent?.RegisterListener(InitiateSlam);
        
        SlamDestructibleHitEvent?.RegisterListener(StopSlamOnDestructibleHit);
        SlamSolidHitEvent?.RegisterListener(StopSlamOnSolidHit);
    }

    void OnDisable()
    {
        SlamAttemptEvent?.UnregisterListener(InitiateSlam);
        
        SlamDestructibleHitEvent?.UnregisterListener(StopSlamOnDestructibleHit);
        SlamSolidHitEvent?.UnregisterListener(StopSlamOnSolidHit);
        
        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
            playerStateModel.SetIsSlamming(false); 
            _isSlamAvailable = true;
        }
    }

    private void InitiateSlam()
    {
        if (_rb == null || !_isSlamAvailable || playerStateModel.IsGrounded || playerStateModel.IsDashing || playerStateModel.IsSlamming || playerStateModel.IsSliding)
        {
            return;
        }
        
        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
        }
        
        _currentSlamCoroutine = StartCoroutine(SlamCoroutine());
    }

    private IEnumerator SlamCoroutine()
    {
        _isSlamAvailable = false;
        playerStateModel.SetIsSlamming(true); 

        Vector3 slamDirection = Vector3.down; 
        float finalSlamPower = playerStateModel.CurrentSlamPower;
        float slamDuration = playerSettingsData.slamDuration;
        
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(slamDirection * finalSlamPower, ForceMode.Impulse);
        
        float slamTimer = slamDuration;
        
        while (slamTimer > 0 && playerStateModel.IsSlamming) 
        {
            slamTimer -= Time.deltaTime;
            
            float accelerationMagnitude = (finalSlamPower * 0.5f) * (slamTimer / slamDuration);
            _rb.AddForce(slamDirection * accelerationMagnitude, ForceMode.Acceleration);
            
            yield return null;
        }
        
        if (playerStateModel.IsSlamming)
        {
            playerStateModel.SetIsSlamming(false); 
        }
        
        if (!playerStateModel.IsSlamming) 
        {
            _currentSlamCoroutine = StartCoroutine(SlamCooldownRoutine());
        }
    }
    
    /// <summary>
    /// Вызывается при SlamDestructibleHitEvent
    /// </summary>
    private void StopSlamOnDestructibleHit(Vector3 impactPoint)
    {
        if (!playerStateModel.IsSlamming) return;
        
        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
        }
        playerStateModel.SetIsSlamming(false);

        // Отброс игрока 
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z); 
        
        Vector3 reboundForce = Vector3.up * playerSettingsData.destructibleWallReboundMultiplier;
        _rb.AddForce(reboundForce, ForceMode.Impulse); 
        
        _currentSlamCoroutine = StartCoroutine(SlamCooldownRoutine());
    }
    
    /// <summary>
    /// Вызывается при SlamSolidHitEvent
    /// </summary>
    private void StopSlamOnSolidHit()
    {
        if (!playerStateModel.IsSlamming) return;
        
        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
        }
        playerStateModel.SetIsSlamming(false);
        
        StartCoroutine(SlamSlowdownRoutine());
    }
    
    private IEnumerator SlamSlowdownRoutine()
    {
        float slowdownDuration = playerSettingsData.slamSlowdownDuration;
        float timer = 0f;
        
        Vector3 initialVelocity = _rb.velocity;
        
        while (timer < slowdownDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slowdownDuration;
            
            // Плавно гасим скорость
            _rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, t * 0.9f); 
            
            yield return null;
        }
        
        _currentSlamCoroutine = StartCoroutine(SlamCooldownRoutine());
    }
    
    private IEnumerator SlamCooldownRoutine()
    {
        float slamCooldownTimer = playerSettingsData.slamCooldown;
        while (slamCooldownTimer > 0)
        {
            slamCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        _isSlamAvailable = true;
        Debug.Log("Slam is now available.");
    }
}
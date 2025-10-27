using UnityEngine;
using System.Collections;

/// <summary>
/// Управляет способностью CrawlSlide
/// </summary>
public class CrawlSlideSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel; 
    public PlayerSettingsData playerSettingsData; 
    
    public Transform visualModelTransform; 
    public Camera playerCamera; 
    
    [Header("Input Listener")]
    public GameEvent SlideAttemptEvent; 
    
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;
    private bool _isSlideAvailable = true;
    private Coroutine _currentSlideCoroutine;
    
    private float _originalColliderHeight;
    private float _originalColliderCenterY;
    private Vector3 _originalModelScale;
    private float _originalCameraLocalY;
    private float _originalVisualModelLocalY; 
    
    private float _rootPositionAdjustmentY = 0f; 

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (_rb == null || _capsuleCollider == null)
        {
            enabled = false;
        }
        
        // Сохранение исходных параметров
        if (_capsuleCollider != null)
        {
            _originalColliderHeight = _capsuleCollider.height;
            _originalColliderCenterY = _capsuleCollider.center.y;
        }
        if (visualModelTransform != null)
        {
            _originalModelScale = visualModelTransform.localScale;
            _originalVisualModelLocalY = visualModelTransform.localPosition.y; 
        }
        if (playerCamera != null)
        {
            _originalCameraLocalY = playerCamera.transform.localPosition.y;
        }

        playerStateModel.SetIsSliding(false);
    }

    void OnEnable()
    {
        SlideAttemptEvent?.RegisterListener(InitiateCrawlSlide);
    }

    void OnDisable()
    {
        SlideAttemptEvent?.UnregisterListener(InitiateCrawlSlide);
        
        if (_currentSlideCoroutine != null)
        {
            StopCoroutine(_currentSlideCoroutine);
            ResetPlayerToOriginalState(); 
            playerStateModel.SetIsSliding(false); 
            _isSlideAvailable = true;
        }
    }

    private void InitiateCrawlSlide()
    {
        if (!_isSlideAvailable || playerStateModel.IsSliding || !playerStateModel.IsGrounded)
        {
            return;
        }
        
        _currentSlideCoroutine = StartCoroutine(CrawlSlideCoroutine());
    }
    
    // --- Coroutine для пошагового выполнения способности ---
    private IEnumerator CrawlSlideCoroutine()
    {
        _isSlideAvailable = false;
        playerStateModel.SetIsSliding(true); 

        // === РАСЧЕТ ЦЕЛЕВЫХ ПАРАМЕТРОВ НА ОСНОВЕ СТИЛЯ ===
        int styleIndex = playerStateModel.CurrentStyleIndex;
        float transitionDuration = playerSettingsData.squatTransitionDuration;
        
        float targetScaleY = playerSettingsData.squatHeightScale;
        float targetHeight = _originalColliderHeight * targetScaleY;
        
        float verticalShiftMagnitude = (_originalColliderHeight - targetHeight) / 2f; 
        float shiftDirection = 0f;
        
        if (styleIndex == 0) 
        {
            shiftDirection = -1f;
            _rootPositionAdjustmentY = 0f; 
            Debug.Log("[CrawlSlide] Squatting to the Bottom (Legs Style)");
        }
        else 
        {
            shiftDirection = 1f;
            _rootPositionAdjustmentY = -verticalShiftMagnitude; 
            Debug.Log("[CrawlSlide] Squatting to the Top (Hands Style) with Root Adjustment");
        }

        float verticalShift = verticalShiftMagnitude * shiftDirection;
        
        float targetCenterY = _originalColliderCenterY + verticalShift; 
        
        float targetVisualModelLocalY = _originalVisualModelLocalY + verticalShift; 
        
        if (styleIndex != 0)
        {
            targetVisualModelLocalY += verticalShiftMagnitude; 
        }

        float targetCameraY = _originalCameraLocalY + verticalShift; 
        
        Vector3 initialRootPosition = transform.position;
        float targetRootY = initialRootPosition.y + _rootPositionAdjustmentY;
        
        // 1. ФАЗА СЖАТИЯ (SQUAT DOWN)
        _rb.isKinematic = true; 
        
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            
            UpdateTransformAndCollider(t, targetScaleY, targetHeight, targetCenterY, targetVisualModelLocalY, targetCameraY, targetRootY, initialRootPosition.y);

            yield return null; 
        }

        UpdateTransformAndCollider(1f, targetScaleY, targetHeight, targetCenterY, targetVisualModelLocalY, targetCameraY, targetRootY, initialRootPosition.y);
        
        // 2. ФАЗА СКОЛЬЖЕНИЯ (SLIDE)
        
        _rb.isKinematic = false;
        
        Vector3 slideDirection = transform.forward;
        slideDirection.y = 0f; 
        slideDirection.Normalize();
        _rb.AddForce(slideDirection * playerSettingsData.slideBaseImpulse, ForceMode.Impulse);
        
        yield return new WaitForSeconds(playerSettingsData.slideDuration); 
        
        // 3. ФАЗА ВОССТАНОВЛЕНИЯ (UNSQUAT UP)
        
        playerStateModel.SetIsSliding(false);
        _rb.isKinematic = true; 
        
        // Целевые значения - исходные
        targetScaleY = _originalModelScale.y;
        targetHeight = _originalColliderHeight;
        targetCenterY = _originalColliderCenterY;
        targetVisualModelLocalY = _originalVisualModelLocalY;
        targetCameraY = _originalCameraLocalY;
        targetRootY = initialRootPosition.y; 
        
        // Сохраняем начальные значения для восстановления
        float startRestoreHeight = _capsuleCollider.height;
        float startRestoreCenterY = _capsuleCollider.center.y;
        float startRestoreScaleY = visualModelTransform.localScale.y;
        float startRestoreVisualModelY = visualModelTransform.localPosition.y;
        float startRestoreCameraY = playerCamera.transform.localPosition.y;
        float startRestoreRootY = transform.position.y; 

        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            
            UpdateTransformAndCollider(t, targetScaleY, targetHeight, targetCenterY, targetVisualModelLocalY, targetCameraY, targetRootY, startRestoreRootY,
                startRestoreScaleY, startRestoreHeight, startRestoreCenterY, startRestoreVisualModelY, startRestoreCameraY);

            yield return null;
        }
        
        ResetPlayerToOriginalState(targetRootY); 
        
        yield return null; 

        _rb.isKinematic = false;
        _rootPositionAdjustmentY = 0f; 
        
        // 4. ФАЗА ПЕРЕЗАРЯДКИ (COOLDOWN)
        yield return new WaitForSeconds(playerSettingsData.slideCooldown);

        _isSlideAvailable = true;
        Debug.Log("Crawl/Slide is now available.");
        _currentSlideCoroutine = null;
    }
    
    /// <summary>
    /// Вспомогательная функция для плавного изменения размеров/позиции
    /// </summary>
    private void UpdateTransformAndCollider(float t, float targetScaleY, float targetHeight, float targetCenterY, 
        float targetVisualModelLocalY, float targetCameraY, float targetRootY, float startRootY,
        float startScaleY = -1f, float startHeight = -1f, float startCenterY = -1f, 
        float startVisualModelLocalY = -1f, float startCameraY = -1f)
    {
        if (startScaleY == -1f) startScaleY = _originalModelScale.y;
        if (startHeight == -1f) startHeight = _originalColliderHeight;
        if (startCenterY == -1f) startCenterY = _originalColliderCenterY;
        if (startVisualModelLocalY == -1f) startVisualModelLocalY = _originalVisualModelLocalY; 
        if (startCameraY == -1f) startCameraY = _originalCameraLocalY;
        
        if (_rootPositionAdjustmentY != 0f) 
        {
            float currentRootY = Mathf.Lerp(startRootY, targetRootY, t);
            transform.position = new Vector3(transform.position.x, currentRootY, transform.position.z);
        }
        
        if (_capsuleCollider != null)
        {
            _capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, Mathf.Lerp(startCenterY, targetCenterY, t), _capsuleCollider.center.z);
        }
        
        if (visualModelTransform != null)
        {
            float currentYScale = Mathf.Lerp(startScaleY, targetScaleY, t);
            visualModelTransform.localScale = new Vector3(_originalModelScale.x, currentYScale, _originalModelScale.z);
            
            float currentYPos = Mathf.Lerp(startVisualModelLocalY, targetVisualModelLocalY, t);
            visualModelTransform.localPosition = new Vector3(visualModelTransform.localPosition.x, currentYPos, visualModelTransform.localPosition.z);
        }
        
        if (playerCamera != null)
        {
            float currentCameraY = Mathf.Lerp(startCameraY, targetCameraY, t);
            Vector3 localPos = playerCamera.transform.localPosition;
            playerCamera.transform.localPosition = new Vector3(localPos.x, currentCameraY, localPos.z);
        }
    }
    
    /// <summary>
    /// Сбрасывает состояние игрока к исходному
    /// </summary>
    private void ResetPlayerToOriginalState(float finalRootY = -1f)
    {
        if (finalRootY != -1f && _rootPositionAdjustmentY != 0f) 
        {
            transform.position = new Vector3(transform.position.x, finalRootY, transform.position.z);
        }
        
        if (_capsuleCollider != null)
        {
            _capsuleCollider.height = _originalColliderHeight;
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, _originalColliderCenterY, _capsuleCollider.center.z); 
        }
        if (visualModelTransform != null)
        {
            visualModelTransform.localScale = _originalModelScale;
            visualModelTransform.localPosition = new Vector3(visualModelTransform.localPosition.x, _originalVisualModelLocalY, visualModelTransform.localPosition.z);
        }
        if (playerCamera != null)
        {
            Vector3 localPos = playerCamera.transform.localPosition;
            playerCamera.transform.localPosition = new Vector3(localPos.x, _originalCameraLocalY, localPos.z);
        }
    }
}
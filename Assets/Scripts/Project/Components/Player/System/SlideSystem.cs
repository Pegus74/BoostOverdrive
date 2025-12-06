using UnityEngine;
using System.Collections;


public class CrawlSlideSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;

    public PlayerConfig pc;
    public Camera playerCamera; 
    
    public Rigidbody _rb;
    private bool _isSlideAvailable = true;
    private Coroutine _currentSlideCoroutine;
    
    public Transform cap;
    private Vector3 capsuleDefaultPos;
    private Vector3 capsuleDefaultScale;
    private Vector3 originalScale;
    
    private float _rootPositionAdjustmentY = 0f;

    private float timer = 0;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        
        capsuleDefaultPos = cap.localPosition;
        capsuleDefaultScale = cap.localScale;
        
        playerStateModel.SetIsSliding(false);
    }

    void OnEnable()
    { 
        InputEvents.SlideAttemptEvent.AddListener(InitiateCrawlSlide);
        InputEvents.SlideCanceledEvent.AddListener(StopCrawlSlide);
    }

    void OnDisable()
    { 
        InputEvents.SlideAttemptEvent.RemoveListener(InitiateCrawlSlide);
        InputEvents.SlideCanceledEvent.RemoveListener(StopCrawlSlide);
    }

    private void InitiateCrawlSlide()
    {
        if (!playerStateModel.IsSliding && !playerStateModel.IsDashing)
        {
            if (_currentSlideCoroutine != null) StopCoroutine(_currentSlideCoroutine);
            _currentSlideCoroutine = StartCoroutine(CrawlSlideCoroutine());
        }
    }

    private void StopCrawlSlide()
    {
        if (playerStateModel.IsSliding)
        {
            if (_currentSlideCoroutine != null) StopCoroutine(_currentSlideCoroutine);
            _currentSlideCoroutine = StartCoroutine(StopCrawlSlideCoroutine());
        }
    }
    
    
    private IEnumerator CrawlSlideCoroutine()
    {
        playerStateModel.SetIsSliding(true);

        Vector3 originalScale = cap.localScale;
        Vector3 targetScale = originalScale;
        targetScale.y *= pc.squatHeightScale;

        Vector3 originalPos = cap.localPosition;
        Vector3 targetPos = originalPos;

        float delta = originalScale.y - targetScale.y;

        int currentStyleIndex = playerStateModel.CurrentStyleIndex;
        
        if (currentStyleIndex == 1)
        {
            targetPos.y = originalPos.y - delta * 0.5f;
        }
        else
        {
            targetPos.y = originalPos.y + delta * 0.5f;
        }
        
        if (currentStyleIndex == 0 && playerStateModel.IsGrounded)
            _rb.useGravity = false;
        

        targetPos.y = Mathf.Max(targetPos.y, capsuleDefaultPos.y);

        float t = 0f;

        while (t < pc.squatTransitionDuration)
        {
            t += Time.deltaTime;
            float progress = t / pc.squatTransitionDuration;

            cap.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            cap.localPosition = Vector3.Lerp(originalPos, targetPos, progress);

            yield return null;
        }

        cap.localScale = targetScale;
        cap.localPosition = targetPos;

        if (currentStyleIndex == 0)
        {
            _rb.useGravity = true;
        }

        float initialSpeed = playerStateModel.CurrentWalkSpeed * playerStateModel.MovementSpeedModifier;
        Vector3 slideDirection = playerStateModel.GetForwardVector();
        slideDirection.y = 0;
        slideDirection.Normalize();

        Vector3 currentHorizontalVelocity = playerStateModel.GetCurrentHorizontalVelocity();
        float requiredForce = initialSpeed - currentHorizontalVelocity.magnitude;

        if (requiredForce > 0)
        {
            PlayerEvents.OnPlayerSpeedModifierChange.Invoke(pc.SlideSpeedMultiplier);
            _rb.AddForce(slideDirection * requiredForce, ForceMode.VelocityChange);
        }

        
        timer = 0f;
        while (playerStateModel.IsSliding)
        {
            timer += Time.deltaTime;

            float currentSpeedLerp = Mathf.Lerp(pc.SlideSpeedMultiplier, 1.0f, timer / (pc.slideDuration));

            if (timer < pc.slideDuration)
            {
                PlayerEvents.OnPlayerSpeedModifierChange.Invoke(currentSpeedLerp);
            }
            else
            {
                PlayerEvents.OnPlayerSpeedModifierChange.Invoke(pc.FinalSlideSpeedMultiplier);
            }

            yield return null;
        }
    }
    
    private IEnumerator StopCrawlSlideCoroutine()
    {
        playerStateModel.SetIsSliding(false);
        Vector3 currentScale = cap.localScale;

        Vector3 targetScale = originalScale;
        Vector3 targetPos = capsuleDefaultPos;

        float delta = originalScale.y - currentScale.y;

        int currentStyleIndex = playerStateModel.CurrentStyleIndex;
        

        if (currentStyleIndex == 1)
        {
            targetPos.y = capsuleDefaultPos.y + delta * 0.5f;
        }
        else
        {
            targetPos.y = capsuleDefaultPos.y - delta * 0.5f;
        }
        

        targetPos.y = Mathf.Max(targetPos.y, capsuleDefaultPos.y);

        float t = 0f;

        while (t < pc.squatTransitionDuration)
        {
            t += Time.deltaTime;
            float progress = t / pc.squatTransitionDuration;

            cap.localScale = Vector3.Lerp(currentScale, targetScale, progress);

            yield return null;
        }

        cap.localScale = capsuleDefaultScale;

        PlayerEvents.OnPlayerSpeedModifierChange.Invoke(1.0f);
    }
    
    
}
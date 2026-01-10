using UnityEngine;
using System.Collections;

public class SlamSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;

    [Header("Slam Indicator")]
    [SerializeField] private GameObject slamIndicatorPrefab;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float indicatorOffset = 0.02f;

    private Rigidbody _rb;
    private bool _isSlamAvailable = true;
    private Coroutine _currentSlamCoroutine;

    private GameObject currentIndicator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            enabled = false;
        }
        playerStateModel.SetIsSlamming(false);

        if (slamIndicatorPrefab != null)
        {
            currentIndicator = Instantiate(slamIndicatorPrefab);
            currentIndicator.SetActive(false);
        }
    }

    void OnEnable()
    {
        InputEvents.SlamAttemptEvent.AddListener(InitiateSlam);

        AbilityEvents.SlamDestructibleHitEvent.AddListener(StopSlamOnDestructibleHit);
        AbilityEvents.SlamSolidHitEvent.AddListener(StopSlamOnSolidHit);
    }

    void OnDisable()
    {
        InputEvents.SlamAttemptEvent.RemoveListener(InitiateSlam);

        AbilityEvents.SlamDestructibleHitEvent.RemoveListener(StopSlamOnDestructibleHit);
        AbilityEvents.SlamSolidHitEvent.RemoveListener(StopSlamOnSolidHit);

        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
            playerStateModel.SetIsSlamming(false);
            _isSlamAvailable = true;
        }

        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (currentIndicator == null) return;

        bool canSlam = _isSlamAvailable &&
                       !playerStateModel.IsGrounded &&
                       !playerStateModel.IsDashing &&
                       !playerStateModel.IsSlamming &&
                       !playerStateModel.IsSliding;

        if (canSlam)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
            {
                currentIndicator.SetActive(true);
                currentIndicator.transform.position = hit.point + hit.normal * indicatorOffset;
                currentIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
            else
            {
                currentIndicator.SetActive(false);
            }
        }
        else
        {
            currentIndicator.SetActive(false);
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
        AbilityEvents.OnAbilityStarted.Invoke();

        Vector3 slamDirection = Vector3.down;
        float finalSlamPower = playerStateModel.CurrentSlamPower;

        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(slamDirection * finalSlamPower, ForceMode.Impulse);


        while (playerStateModel.IsSlamming && !playerStateModel.IsGrounded)
        {
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

    private void StopSlamOnDestructibleHit(Vector3 impactPoint)
    {
        if (!playerStateModel.IsSlamming) return;

        if (_currentSlamCoroutine != null)
        {
            StopCoroutine(_currentSlamCoroutine);
        }
        playerStateModel.SetIsSlamming(false);

        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        Vector3 reboundForce = Vector3.up * playerStateModel.settings.destructibleWallReboundMultiplier;
        _rb.AddForce(reboundForce, ForceMode.Impulse);

        _currentSlamCoroutine = StartCoroutine(SlamCooldownRoutine());
    }

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
        float slowdownDuration = playerStateModel.settings.slamSlowdownDuration;
        float timer = 0f;

        Vector3 initialVelocity = _rb.linearVelocity;

        while (timer < slowdownDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slowdownDuration;

            _rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, t * 0.9f);

            yield return null;
        }

        _currentSlamCoroutine = StartCoroutine(SlamCooldownRoutine());
    }

    private IEnumerator SlamCooldownRoutine()
    {
        float slamCooldownTimer = playerStateModel.settings.slamCooldown;
        while (slamCooldownTimer > 0)
        {
            slamCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        _isSlamAvailable = true;
    }
}
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class TutorialSign : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Transform player;
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private TutorialType tutorialType = TutorialType.Standard;

    [Header("Визуал")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject arrowIndicator;


    [Header("Collider Settings")]
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private bool hideOnTouch = true;

    public bool IsActive { get; private set; }
    public bool WasShown { get; private set; }
    public float CurrentDistanceToPlayer { get; private set; }

    private Coroutine activationCoroutine;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isHiddenByTouch = false;

    public enum TutorialType
    {
        Standard,
        OneTime,
        Proximity,
        Interactive
    }

    void Start()
    {
        InitializeSign();
        SaveInitialState();
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
            if (triggerCollider == null)
            {
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                triggerCollider = boxCollider;
            }
        }
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    void Update()
    {
        if (player == null)
            TryFindPlayer();

        if (player != null)
        {
            HandleRotation();
            if (!isHiddenByTouch) 
            {
                HandleActivation();
            }
        }
    }

    private void SaveInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        WasShown = true;
        StartActivationAnimation(true);
    }


    public void ResetSign()
    {
        isHiddenByTouch = false;
        WasShown = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        SetSignActive(true, true);
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hideOnTouch) return;

        if (other.CompareTag("Player"))
        {
            HideOnTouch();
        }
    }

    private void HideOnTouch()
    {
        if (isHiddenByTouch) return;

        isHiddenByTouch = true;
        Deactivate();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }


    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        StartActivationAnimation(false);
    }
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    public void SetActivationDistance(float newDistance)
    {
        activationDistance = Mathf.Max(0.1f, newDistance);
    }

    private void InitializeSign()
    {
        SetSignActive(false, true);
    }

    private void HandleRotation()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleActivation()
    {
        CurrentDistanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (tutorialType == TutorialType.OneTime && WasShown)
            return;

        if (CurrentDistanceToPlayer <= activationDistance && !IsActive)
        {
            Activate();
        }
        else if (CurrentDistanceToPlayer > activationDistance && IsActive)
        {
            Deactivate();
        }
    }

    private void StartActivationAnimation(bool activate)
    {
        if (activationCoroutine != null)
            StopCoroutine(activationCoroutine);

        activationCoroutine = StartCoroutine(AnimateSign(activate));
    }

    private IEnumerator AnimateSign(bool activate)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 0f;
        float targetAlpha = activate ? 1f : 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = activate ? Vector3.one : Vector3.one * 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

            yield return null;
        }
        SetSignActive(activate, false);
    }

    private void SetSignActive(bool active, bool immediate = false)
    {
        if (canvasGroup != null)
        {
            if (immediate)
            {
                canvasGroup.alpha = active ? 1f : 0f;
            }
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }

        if (arrowIndicator != null)
            arrowIndicator.SetActive(active);
    }

   
    private void TryFindPlayer()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"Player found automatically for {gameObject.name}");
        }
    }

    


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsActive ? Color.green : new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, activationDistance);
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

}
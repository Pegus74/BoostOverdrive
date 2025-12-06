using UnityEngine;
using System.Collections;

public class TutorialSign : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private TutorialType tutorialType = TutorialType.Standard;

    [SerializeField] private CanvasGroup visualsCanvasGroup;
    [SerializeField] private CanvasGroup textCanvasGroup;

    [SerializeField] private bool textNoFadeOut = true;

    [SerializeField] private Collider triggerCollider;
    [SerializeField] private bool hideOnTouch = true;

    [SerializeField] private bool enableFadeOut = false;
    [SerializeField] private float fadeOutStartDistance = 1.5f;
    [SerializeField] private float fadeOutEndDistance = 0.5f;

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

        if (visualsCanvasGroup == null)
        {
            visualsCanvasGroup = GetComponent<CanvasGroup>();
        }

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
                HandleFadeOut();
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

    private void HandleFadeOut()
    {
        if (!enableFadeOut || !IsActive) return;

        CurrentDistanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (CurrentDistanceToPlayer <= fadeOutStartDistance)
        {
            float fadeProgress = 0f;

            if (CurrentDistanceToPlayer <= fadeOutEndDistance)
            {
                fadeProgress = 1f;
            }
            else
            {
                fadeProgress = 1f - ((CurrentDistanceToPlayer - fadeOutEndDistance) /
                                   (fadeOutStartDistance - fadeOutEndDistance));
                fadeProgress = Mathf.Clamp01(fadeProgress);
            }

            if (visualsCanvasGroup != null)
            {
                visualsCanvasGroup.alpha = 1f - fadeProgress;
            }

            if (textCanvasGroup != null && textNoFadeOut)
            {
                textCanvasGroup.alpha = 1f;
            }
            else if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = 1f - fadeProgress;
            }
        }
        else
        {
            if (visualsCanvasGroup != null)
            {
                visualsCanvasGroup.alpha = 1f;
            }
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = 1f;
            }
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
        float visualsStartAlpha = visualsCanvasGroup != null ? visualsCanvasGroup.alpha : 0f;
        float textStartAlpha = textCanvasGroup != null ? textCanvasGroup.alpha : 0f;
        float targetAlpha = activate ? 1f : 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = activate ? Vector3.one : Vector3.one * 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (visualsCanvasGroup != null)
                visualsCanvasGroup.alpha = Mathf.Lerp(visualsStartAlpha, targetAlpha, smoothT);

            if (textCanvasGroup != null)
                textCanvasGroup.alpha = Mathf.Lerp(textStartAlpha, targetAlpha, smoothT);

            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

            yield return null;
        }

        SetSignActive(activate, false);
    }

    private void SetSignActive(bool active, bool immediate = false)
    {
        if (visualsCanvasGroup != null)
        {
            if (immediate)
            {
                visualsCanvasGroup.alpha = active ? 1f : 0f;
            }
            visualsCanvasGroup.interactable = active;
            visualsCanvasGroup.blocksRaycasts = active;
        }

        if (textCanvasGroup != null)
        {
            if (immediate)
            {
                textCanvasGroup.alpha = active ? 1f : 0f;
            }
            textCanvasGroup.interactable = active;
            textCanvasGroup.blocksRaycasts = active;
        }
    }

    private void TryFindPlayer()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsActive ? Color.green : new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, activationDistance);

        if (enableFadeOut)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, fadeOutStartDistance);

            Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
            Gizmos.DrawWireSphere(transform.position, fadeOutEndDistance);
        }

        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
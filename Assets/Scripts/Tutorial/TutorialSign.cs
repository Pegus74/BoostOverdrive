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

    public bool IsActive { get; private set; }
    public bool WasShown { get; private set; }
    public float CurrentDistanceToPlayer { get; private set; }

    private Coroutine activationCoroutine;

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
    }

    void Update()
    {
        if (player == null)
            TryFindPlayer();

        if (player != null)
        {
            HandleRotation();
            HandleActivation();
        }
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        WasShown = true;
        StartActivationAnimation(true);
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
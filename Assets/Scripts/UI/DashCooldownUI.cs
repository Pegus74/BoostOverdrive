using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DashCooldownUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fillRing;          
    public Image dashIcon;         

    [Header("Visual Settings")]
    public Color readyIconColor = Color.white;
    public Color cooldownIconColor = new Color(1f, 1f, 1f, 0.4f);
    public float fadeInSpeed = 6f;
    public float fadeOutSpeed = 4f;

    private CanvasGroup canvasGroup;
    private DashSystem dashSystem;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        dashSystem = FindObjectOfType<DashSystem>();
        if (dashSystem == null)
        {
            Debug.LogError("[DashCooldownUI] DashSystem не найден в сцене!");
            enabled = false;
            return;
        }

        // Изначально скрываем UI
        canvasGroup.alpha = 0f;
        if (fillRing != null) fillRing.fillAmount = 1f;
        if (dashIcon != null) dashIcon.color = readyIconColor;
    }

    private void OnEnable()
    {
        dashSystem.OnDashPerformed.AddListener(OnDashUsed);
        dashSystem.OnDashReady.AddListener(OnDashReady);
    }

    private void OnDisable()
    {
        if (dashSystem != null)
        {
            dashSystem.OnDashPerformed.RemoveListener(OnDashUsed);
            dashSystem.OnDashReady.RemoveListener(OnDashReady);
        }
    }

    private void OnDashUsed()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(1f, fadeInSpeed));

        if (dashIcon != null)
            dashIcon.color = cooldownIconColor;

        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(CooldownFillCoroutine());
    }

    private void OnDashReady()
    {

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        if (fillRing != null)
            fillRing.fillAmount = 1f;

        if (dashIcon != null)
            dashIcon.color = readyIconColor;

        StartCoroutine(FadeCanvasGroup(0f, fadeOutSpeed));
    }

    private IEnumerator CooldownFillCoroutine()
    {
        float cooldown = dashSystem.GetCooldownDuration(); 
        float timer = 0f;

        while (timer < cooldown)
        {
            timer += Time.unscaledDeltaTime; 
            fillRing.fillAmount = 1f - (timer / cooldown);
            yield return null;
        }

        fillRing.fillAmount = 1f;
        cooldownCoroutine = null;
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float speed)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed * Time.unscaledDeltaTime);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
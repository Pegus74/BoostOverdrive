using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WindEffectUI : MonoBehaviour
{
    public Sprite[] windSprites;
    public float frameRate = 10f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 0.7f;

    private Image windImage;
    private float animationTimer = 0f;
    private int currentFrame = 0;
    private bool isAnimating = false;

    private void Awake()
    {
        windImage = GetComponent<Image>();
        if (windImage == null)
        {
            windImage = gameObject.AddComponent<Image>();
        }

        windImage.color = new Color(1, 1, 1, 0);
        gameObject.SetActive(false);
    }

    public void StartWindEffect(float intensity)
    {
        if (windSprites == null || windSprites.Length == 0) return;

        if (!isAnimating)
        {
            isAnimating = true;
            gameObject.SetActive(true);
            StartCoroutine(AnimateWind(intensity));
        }
    }

    public void StopWindEffect()
    {
        isAnimating = false;
        StopAllCoroutines();
        StartCoroutine(FadeOutWind());
    }

    public void UpdateWindIntensity(float intensity)
    {
        if (windImage != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, intensity);
            Color color = windImage.color;
            color.a = alpha;
            windImage.color = color;

            float shakeAmount = intensity * 2f;
            transform.localPosition = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0
            );
        }
    }

    private IEnumerator AnimateWind(float intensity)
    {
        float frameDelay = 1f / frameRate;

        while (isAnimating)
        {
            if (windSprites != null && windSprites.Length > 0)
            {
                windImage.sprite = windSprites[currentFrame];
                currentFrame = (currentFrame + 1) % windSprites.Length;
            }

            UpdateWindIntensity(intensity);

            yield return new WaitForSeconds(frameDelay);
        }
    }

    private IEnumerator FadeOutWind()
    {
        float fadeDuration = 0.5f;
        float elapsed = 0f;
        Color startColor = windImage.color;
        Color targetColor = new Color(1, 1, 1, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            windImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
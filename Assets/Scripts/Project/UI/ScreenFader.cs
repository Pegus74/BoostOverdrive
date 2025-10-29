using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    
    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; 
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }
    
    public IEnumerator FadeIn(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
}
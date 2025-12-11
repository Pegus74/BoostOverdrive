using UnityEngine;
using UnityEngine.UI;

public class StyleHUDView : MonoBehaviour
{
    [Header("UI Elements")]
    public Image LegstyleUI;
    public Image HandsStyleUI;

    private Image activeImage;
    private Image prevImage;
    
    private void OnEnable() => PlayerEvents.OnStyleChangedEvent.AddListener(UpdateStyleUI);
    private void OnDisable() => PlayerEvents.OnStyleChangedEvent.RemoveListener(UpdateStyleUI);

    private void Awake()
    {
        if (LegstyleUI == null || HandsStyleUI == null)
        {
            Debug.LogError("[StyleHUDView] Images not found");
        }
        
        activeImage = LegstyleUI;
        prevImage =  HandsStyleUI;
        activeImage.gameObject.SetActive(true);
        prevImage.gameObject.SetActive(false);
    }
    
    private void UpdateStyleUI()
    {
        (activeImage, prevImage) = (prevImage, activeImage);
        
        if (activeImage != null)
            activeImage.gameObject.SetActive(true);
    
        if (prevImage != null)
            prevImage.gameObject.SetActive(false);

        
        Debug.Log($"[StyleHUDView] updated. Active Index:");
    }
}
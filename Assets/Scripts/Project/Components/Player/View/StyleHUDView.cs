using UnityEngine;
using UnityEngine.UI;

public class StyleHUDView : MonoBehaviour
{
    [Header("UI Elements")]
    public Image LegstyleUI;
    public Image HandsStyleUI;

    private Image activeImage;
    private Image prevImage;

    private void OnEnable()
    {
        PlayerEvents.OnStyleChangedEvent.AddListener(UpdateStyleUI);
        GameEvents.OnPause.AddListener(ShowHud);
    }

    private void OnDisable()
    {
        PlayerEvents.OnStyleChangedEvent.RemoveListener(UpdateStyleUI);
        GameEvents.OnPause.RemoveListener(ShowHud);
    } 

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
        if (NewGameManager.Instance.GetCurrentState() != GameState.Playing)
            return;
        (activeImage, prevImage) = (prevImage, activeImage);
        
        if (activeImage != null)
            activeImage.gameObject.SetActive(true);
    
        if (prevImage != null)
            prevImage.gameObject.SetActive(false);

        
        Debug.Log($"[StyleHUDView] updated. Active Index:");
    }

    public void ShowHud(bool show)
    {
        activeImage.gameObject.SetActive(show);
    }
}
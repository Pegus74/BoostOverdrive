using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// View: Слушает OnStyleChangedEvent из PlayerStateModel и обновляет UI
/// </summary>
public class StyleHUDView : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel; 

    [Header("UI Elements")]
    public Image LegstyleUI;
    public Image HandsStyleUI;

    private void OnEnable()
    {
        playerStateModel?.OnStyleChangedEvent.RegisterListener(UpdateStyleUI);
        UpdateStyleUI(playerStateModel.CurrentStyleIndex);
    }

    private void OnDisable()
    {
        playerStateModel?.OnStyleChangedEvent.UnregisterListener(UpdateStyleUI);
    }
    
    private void UpdateStyleUI(int newStyleIndex)
    {
        if (LegstyleUI != null && HandsStyleUI != null)
        {
            LegstyleUI.gameObject.SetActive(newStyleIndex == 0);
            HandsStyleUI.gameObject.SetActive(newStyleIndex == 1);
        }
        
        Debug.Log($"StyleHUDView updated. Active Index: {newStyleIndex}");
    }
}
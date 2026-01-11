using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Listener, активирующий/деактивирующий GameObject в зависимости от состояния игры
/// </summary>
public class UIGameStateListener : MonoBehaviour
{
    [Header("Настройки UI")]
    [Tooltip("Состояние(я) игры, при котором этот объект должен быть АКТИВЕН.")]
    [SerializeField] private GameState targetGameState;
    
    [SerializeField] CanvasGroup targetCanvasGroup;
    
    private void Start()
    {
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChange);

        if (NewGameManager.Instance != null)
        {
            HandleGameStateChange(NewGameManager.Instance.GetCurrentState());
        }
        
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChanged.RemoveListener(HandleGameStateChange);
    }

    private void HandleGameStateChange(GameState newState)
    {
        bool shouldBeActive = (targetGameState == newState);

        if (targetCanvasGroup != null)
        {
            targetCanvasGroup.alpha = shouldBeActive ? 1f : 0f;
            targetCanvasGroup.interactable = shouldBeActive;
            targetCanvasGroup.blocksRaycasts = shouldBeActive;
            if (shouldBeActive)
            {
                TextMeshProUGUI[] tmpTexts = targetCanvasGroup.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI tmp in tmpTexts)
                {
                    tmp.ForceMeshUpdate();
                }

                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
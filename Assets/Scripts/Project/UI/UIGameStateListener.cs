using UnityEngine;

/// <summary>
/// Listener, активирующий/деактивирующий GameObject в зависимости от состояния игры
/// </summary>
public class UIGameStateListener : MonoBehaviour
{
    [SerializeField] private GameStateEvent GameStateChangedEvent;

    [Header("Настройки UI")]
    [Tooltip("Состояние(я) игры, при котором этот объект должен быть АКТИВЕН.")]
    [SerializeField] private GameState targetGameState;
    
    [SerializeField] CanvasGroup targetCanvasGroup;

    private void OnEnable()
    {
        if (GameStateChangedEvent != null)
            GameStateChangedEvent.RegisterListener(HandleGameStateChange);
    }

    private void OnDisable()
    {
        if (GameStateChangedEvent != null)
            GameStateChangedEvent.UnregisterListener(HandleGameStateChange);
    }

    private void HandleGameStateChange(GameState newState)
    {
        bool shouldBeActive = (targetGameState == newState);
        
        if (targetCanvasGroup != null)
        {
            targetCanvasGroup.alpha = shouldBeActive ? 1f : 0f;
            targetCanvasGroup.interactable = shouldBeActive;
            targetCanvasGroup.blocksRaycasts = shouldBeActive;
        }
    }
}
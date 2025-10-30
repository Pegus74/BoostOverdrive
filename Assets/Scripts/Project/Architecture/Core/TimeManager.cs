using UnityEngine;

/// <summary>
/// Listener, реагирующий на изменение состояния игры, чтобы управлять Time.timeScale и курсором
/// </summary>
public class TimeManager : MonoBehaviour
{
    [SerializeField] private GameStateEvent GameStateChangedEvent; 

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
        bool isPlaying = (newState == GameState.Playing);
        
        Time.timeScale = isPlaying ? 1f : 0f;
        
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;

        Debug.Log($"TimeManager Time.timeScale set to: {Time.timeScale}. Cursor is visible: {Cursor.visible}");
    }
}
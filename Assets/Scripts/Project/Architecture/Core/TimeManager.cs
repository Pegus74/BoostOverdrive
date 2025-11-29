using UnityEngine;

/// <summary>
/// Listener, реагирующий на изменение состояния игры, чтобы управлять Time.timeScale и курсором
/// </summary>
public class TimeManager : MonoBehaviour
{
    public NewGameManager gameManager;
    
    private void Awake()
    {
        gameManager = NewGameManager.Instance;
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChange);
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChanged.RemoveListener(HandleGameStateChange);
    }

    private void HandleGameStateChange(GameState newState)
    {
        bool isPlaying = (newState == GameState.Playing);
        
        Time.timeScale = isPlaying ? 1f : 0f;
        
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;

        Debug.Log($"[TimeManager] Time set to: {Time.timeScale}. Cursor is visible: {Cursor.visible}");
    }
}
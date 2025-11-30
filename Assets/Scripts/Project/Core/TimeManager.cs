using UnityEngine;

public class TimeManager : MonoBehaviour
{

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
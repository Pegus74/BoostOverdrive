using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private GameStateEvent OnGameStateChanged; 
    [SerializeField] private GameEvent OnPauseAttemptEvent;

    [SerializeField] private GameEvent SoftResetRequestEvent;
    [SerializeField] private GameEvent SoftResetCompletedEvent;
    
    public static NewGameManager Instance;
    [SerializeField] private GameState currentState = GameState.Playing;
    
    [SerializeField] private LevelRestarter levelRestarter;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            UpdateGameState();
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnEnable()
    {
        if (OnPauseAttemptEvent != null)
            OnPauseAttemptEvent.RegisterListener(TogglePause);
        
        if (SoftResetCompletedEvent != null)
            SoftResetCompletedEvent.RegisterListener(FinishRestart);
    }

    private void OnDisable()
    {
        if (OnPauseAttemptEvent != null)
            OnPauseAttemptEvent.UnregisterListener(TogglePause);
        
        if (SoftResetCompletedEvent != null)
            SoftResetCompletedEvent.UnregisterListener(FinishRestart);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    /// <summary>
    /// Центральный метод, который вызывает событие при изменении состояния
    /// </summary>
    private void UpdateGameState()
    {
        if (OnGameStateChanged != null)
        {
            OnGameStateChanged.Raise(currentState);
            Debug.Log($"GameManager gameState Changed to: {currentState}");
        }
    }

    #region PublicMethods
    
    public void PlayerWin()
    {
        currentState = GameState.GameWon;
        UpdateGameState();
    }

    public void PlayerDied()
    {
        currentState = GameState.GameOver;
        UpdateGameState();
    }

    public void RestartLevel()
    {
        if (SoftResetRequestEvent == null)
        {
            Debug.LogWarning("SoftResetRequestEvent не задан. Перезапуск сцены.");
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
            return;
        }
        
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            UpdateGameState(); 
        }
        
        SoftResetRequestEvent.Raise(); 
        
        Debug.Log("[GameManager]: Soft Reset Requested.");
    }
    
    public void FinishRestart()
    {
        currentState = GameState.Playing;
        UpdateGameState();
        Debug.Log("[GameManager]: Soft Reset Completed.");
    }
    

    public void TogglePause()
    {
        if (currentState != GameState.Playing && currentState != GameState.Paused && currentState != GameState.GameWon)
            return;

        bool isPlaying = (currentState == GameState.Playing);
        currentState = isPlaying ? GameState.Paused : GameState.Playing;
        
        UpdateGameState(); 
    }

    public void ContinueGame()
    {
        if (currentState == GameState.Paused)
            TogglePause();
    }

    public void BackToMenu()
    {
        // TODO: вынести
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("MainMenu");
    }

    public GameState GetCurrentState() => currentState;
    
    #endregion
}
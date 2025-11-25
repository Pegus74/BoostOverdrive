using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    public GameStateEvent OnGameStateChanged = new GameStateEvent(); 
    
    public static NewGameManager Instance;
    [SerializeField] private GameState currentState = GameState.Playing;
    
    private PlayerInputController playerInputController;
    
    private void Awake()
    {
        playerInputController = FindObjectOfType<PlayerInputController>();
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
        playerInputController.InputEvents.OnPauseAttemptEvent.AddListener(TogglePause);
    }

    private void OnDisable()
    {
        playerInputController.InputEvents.OnPauseAttemptEvent.RemoveListener(TogglePause);
        
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
            OnGameStateChanged.Invoke(currentState);
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
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            UpdateGameState(); 
        }
        
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private GameStateEvent OnGameStateChanged; 
    [SerializeField] private GameEvent OnPauseAttemptEvent;
    
    public static NewGameManager Instance;

    [SerializeField] private GameState currentState = GameState.Playing;
    
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
    }

    private void OnDisable()
    {
        if (OnPauseAttemptEvent != null)
            OnPauseAttemptEvent.UnregisterListener(TogglePause);
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
        // currentState = GameState.Playing;
        // UpdateGameState();
        

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
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
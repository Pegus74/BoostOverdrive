using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    public static NewGameManager Instance;
    
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private GameMode currentMode = GameMode.Classic;
    
    private string currentLevelName;
    
    private void Awake()
    {
        Debug.Log("GM Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
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
        InputEvents.OnPauseAttemptEvent.AddListener(TogglePause); 
        InputEvents.OnRestartAttemptEvent.AddListener(RestartLevel);
    }

    private void OnDisable()
    { 
        InputEvents.OnPauseAttemptEvent.RemoveListener(TogglePause); 
        InputEvents.OnRestartAttemptEvent.RemoveListener(RestartLevel);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevelName = scene.name;
        Time.timeScale = 1f;
        if (currentLevelName == "MainMenuRefactor")
            currentState = GameState.Menu;
        else
            currentState = GameState.Playing;
        UpdateGameState();
    }
    
    private void UpdateGameState()
    {
        if (GameEvents.OnGameStateChanged != null)
        {
            Debug.Log($"[GameManager] GameState Changed to: {currentState}");
            GameEvents.OnGameStateChanged.Invoke(currentState);
        }
    }

    #region PublicMethods
    
    public void PlayerWin()
    {
        currentState = GameState.GameWon;
        if  (currentMode == GameMode.Classic)
            TimerController.Instance.StopTimer();
        UpdateGameState();
    }

    public void PlayerDied()
    {
        currentState = GameState.GameOver;
        UpdateGameState();
    }

    public void RestartLevel()
    {
        currentState = GameState.Playing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        Time.timeScale = 1f;
        currentState = GameState.Menu;
        
        SceneManager.LoadScene("MainMenuRefactor");
    }
    
    public void ChangeGameMode()
    {
        bool isClassic = (currentMode == GameMode.Classic);
        currentMode = isClassic ? GameMode.Hard : GameMode.Classic;
        Debug.Log("[GameManager] GameMode Changed to: " + currentMode);
    }

    public GameState GetCurrentState() => currentState;
    
    public GameMode GetCurrentGameMode() => currentMode;
    
    public string GetCurrentLevelName() => currentLevelName;
    
    #endregion
    
}
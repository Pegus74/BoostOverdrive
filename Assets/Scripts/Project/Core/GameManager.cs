using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    public static NewGameManager Instance;
    
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private GameMode currentMode = GameMode.Classic;
    
    [SerializeField] private int maxGlobalCoins = 10;

    [SerializeField] private Canvas settingsCanvas;
    
    private string currentLevelName;
    private int nextLevel;
    
    private void Awake()
    {
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
        
        RMusicManager.Instance.ChangeVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        RMusicManager.Instance.ChangePitch(1f);
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
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        PlayerPrefs.SetInt("ContinueLevel", nextLevel);
        PlayerPrefs.Save();
        
        currentState = GameState.GameWon;
        if  (currentMode == GameMode.Classic)
            TimerController.Instance.StopTimer();
        UpdateGameState();
    }

    public void PlayerDied()
    {
        RMusicManager.Instance.ChangeVolume(PlayerPrefs.GetFloat("MusicVolume", 1f) * 0.2f);
        RMusicManager.Instance.ChangePitch(0.7f);
        currentState = GameState.GameOver;
        UpdateGameState();
    }

    public void RestartLevel()
    {
        currentState = GameState.Playing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Debug.Log("Prefs");
        SceneManager.LoadScene(nextLevel);
    }
    
    public void TogglePause()
    {
        if (currentState != GameState.Playing && currentState != GameState.Paused && currentState != GameState.GameWon)
            return;

        bool isPlaying = (currentState == GameState.Playing);
        currentState = isPlaying ? GameState.Paused : GameState.Playing;
        
        TimerController.Instance.ShowTimer(!isPlaying);
        GameEvents.OnPause.Invoke(!isPlaying);
        
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

    public void ShowSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
        currentState = GameState.InGameSettings;
        
    }

    public void HideSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
        currentState = GameState.Paused;
    }

    public GameState GetCurrentState() => currentState;
    
    public GameMode GetCurrentGameMode() => currentMode;
    
    public string GetCurrentLevelName() => currentLevelName;
    
    #endregion
    
}
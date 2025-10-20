using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    [SerializeField] private GameStateEvent OnGameStateChanged; 
    
    public static NewGameManager Instance;

    private GameState currentState = GameState.Playing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            UpdateGameState();
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         Debug.Log("F PRESSED");
    //         TogglePause();
    //     }
    //     if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         Debug.Log("B PRESSED");
    //         PlayerDied();
    //     }
    // }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ...
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
        currentState = GameState.Playing;
        UpdateGameState();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void TogglePause()
    {
        if (currentState != GameState.Playing && currentState != GameState.Paused && currentState != GameState.GameWon)
            return;

        bool isPlaying = (currentState == GameState.Playing);
        currentState = isPlaying ? GameState.Paused : GameState.Playing;
        
        UpdateGameState(); }

    public void BackToMenu()
    {
        // TODO: вынести
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("MainMenu");
    }

    public GameState GetCurrentState() => currentState;
    
    #endregion
}
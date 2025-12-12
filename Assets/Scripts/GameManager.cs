using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Canvas gameWinCanvas;
    [SerializeField] private Canvas pauseCanvas;
    private string tempScene;
    public string lastSceneName;

    [SerializeField] private MusicManager musicManager;
    private string currentLevelName;
    public List<string> levelNames = new List<string>();

    [SerializeField] private CollectableSystem collectableSystem;
    [SerializeField] private CollectableUI collectableUI;
    private Dictionary<string, int> levelCoinTotals = new Dictionary<string, int>();
    public static GameManager Instance;
    
    private int nextLevel;

    public enum State
    {
        Playing,
        Paused,
        GameOver,
        GameWin,
        Menu
    }
    public State currentState = State.Playing;
    
    public enum GameMode
    {
        Classic,
        Hard
    }
    
    [SerializeField] private GameMode currentGameMode = GameMode.Classic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (gameOverCanvas != null) DontDestroyOnLoad(gameOverCanvas.gameObject);
            if (gameWinCanvas != null) DontDestroyOnLoad(gameWinCanvas.gameObject);
            if (pauseCanvas != null) DontDestroyOnLoad(pauseCanvas.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            FindMusicManager();
            InitializeUI();

            if (musicManager != null)
                musicManager.PlayMenuMusic();
            

            UpdateGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FindMusicManager()
    {
        if (musicManager != null)
        {
            Debug.Log("MusicManager assigned ");
            return;
        }
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager != null)
        {
            Debug.Log("MusicManager found automatically ");
        }
        else
        {

            Debug.LogWarning("MusicManager not found, creating new one");
            
        }
    }

    private void InitializeUI()
    {
        if (gameOverCanvas != null) gameOverCanvas.gameObject.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.gameObject.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);

        //Debug.Log($" GameOver: {gameOverCanvas != null}, GameWin: {gameWinCanvas != null}, Pause: {pauseCanvas != null}");
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void UpdateGameState()
    {

        bool shouldTimeStop = (currentState != State.Playing);
        bool allowCursor = (currentState != State.Playing);
        bool changeMusicToGameOver = (currentState == State.GameOver);
        Time.timeScale = shouldTimeStop ? 0f : 1f;

        if (musicManager != null)
        {
            musicManager.SetVolume(changeMusicToGameOver ? 0.1f : 0.25f);
            musicManager.SetPitch(changeMusicToGameOver ? 0.7f : 1.0f);
        }

        Cursor.lockState = allowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = allowCursor;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        tempScene = SceneManager.GetActiveScene().name;
        StartCoroutine(DelayedSceneSetup(scene));
    }


    private IEnumerator DelayedSceneSetup(Scene scene)
    {
        yield return new WaitForEndOfFrame();

        if (musicManager == null)
        {
            FindMusicManager();
        }
        SetupCollectablesUI();
        if (collectableUI != null)
        {
            collectableUI.UpdateAllUI();
        }
        if (scene.name != "MainMenu" && !levelNames.Contains(scene.name))
        {
            levelNames.Add(scene.name);
        }
        if (scene.name == "NewMenu")
        {
            currentLevelName = null;
            currentState = State.Menu;
            InitializeUI();
            StopAllTimers();
            HideTimerUI();
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (musicManager != null)
                musicManager.PlayMenuMusic();
        }
        else
        {
            currentLevelName = scene.name;
            Debug.Log(tempScene);
            InitializeUI();
            if (musicManager != null)
                musicManager.PlayMenuMusic();
            InitializeTimer();
            StartTimerOnSceneLoad();
            ShowTimerUI();
            tempScene = scene.name;
        }

    }
    private void InitializeTimer()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.InitializeTimer();
        }
        else
        {
            Debug.LogError("TimerManager not found in scene!");
        }

    }
    public string GetCurrentLevelName()
    {
        return currentLevelName;
    }

    #region Collectables

    public void SetupCollectablesUI()
    {
        if (collectableUI != null)
        {
            collectableUI.UpdateAllUI();
        }
    }

    public void ResetAllCollectables()
    {
        if (collectableSystem != null)
        {
            collectableSystem.ResetAllCoins();
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void ResetCoinsProgress()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetAllCollectables();
        }
    }

    public CollectableSystem GetCollectableSystem()
    {
        return collectableSystem;
    }

    #endregion

    #region Timer

    private void StartTimerOnSceneLoad()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.StartTimer();
        }
    }

    private void StopAllTimers()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.StopTimerWithoutSaving(); 
        }
    }

    private void HideTimerUI()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.HideTimer();
        }
    }

    private void ShowTimerUI()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.ShowTimer();
        }
    }
    public void ResetAllBestTimes()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        foreach (string levelName in levelNames)
        {
            if (!string.IsNullOrEmpty(levelName))
            {
                string bestTimeKey = "BestTime_" + levelName;
                if (PlayerPrefs.HasKey(bestTimeKey))
                {
                    PlayerPrefs.DeleteKey(bestTimeKey);
                }
            }
        }

        PlayerPrefs.Save();

        if (currentSceneName != "MainMenu")
        {
            TimerManager timerManager = FindObjectOfType<TimerManager>();
            if (timerManager != null) timerManager.ResetBestTime();
        }
    }


    public void AddLevelToResetList(string levelName)
    {
        if (!string.IsNullOrEmpty(levelName) && !levelNames.Contains(levelName))
        {
            levelNames.Add(levelName);
        }
    }

    public void RemoveLevelFromResetList(string levelName)
    {
        if (levelNames.Contains(levelName))
        {
            levelNames.Remove(levelName);
        }
    }

    public static string GetBestTimeKeyForCurrentLevel()
    {
        if (Instance.currentLevelName != null)
        {
            return "BestTime_" + Instance.currentLevelName;
        }
        return "BestTime_Unknown";
    }

    public static string GetBestTimeKeyForLevel(string levelName)
    {
        return "BestTime_" + levelName;
    }

    #endregion
    
    #region Win
    public void PlayerWin(string levelName = null)
    {
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        PlayerPrefs.SetInt("ContinueLevel", nextLevel);
        PlayerPrefs.Save();
        
        currentState = State.GameWin;
        if (levelName != null)
        {
            this.currentLevelName = levelName;
        }

        if (gameWinCanvas != null) gameWinCanvas.gameObject.SetActive(true);
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null) timer.StopTimerAndSaveBestTime();
    
        UpdateGameState();
    }

    public void LoadNextLevel()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResetAllSigns();
        }
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = currentBuildIndex + 1;
        if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            currentState = State.Playing;
            InitializeUI();
            StopAllTimers();
            HideTimerUI();
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextBuildIndex);
        }
        else
        {
            BackToMenu();
        }
    }
    #endregion

    #region GameOver

    public void PlayerDied()
    {
        currentState = State.GameOver;
        if (gameOverCanvas != null) gameOverCanvas.gameObject.SetActive(true);
        if (currentGameMode == GameMode.Classic)
        {
            TimerManager timer = FindObjectOfType<TimerManager>();
            if (timer != null) timer.StopTimerWithoutSaving();
        }
        UpdateGameState();
    }

    public void RestartLevel()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResetAllSigns();
        }

        if (gameOverCanvas != null) gameOverCanvas.gameObject.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.gameObject.SetActive(false);
        currentState = State.Playing;
        Debug.Log(currentState);
        UpdateGameState();
        Debug.Log(currentState);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Paused

    public void TogglePause()
    {
        if (currentState != State.Playing && currentState != State.Paused) return;
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        bool isPausing = (currentState == State.Playing);
        currentState = isPausing ? State.Paused : State.Playing;

        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(isPausing);
        UpdateGameState();
    }

    public void BackToMenu()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ResetAllSigns();
        }
        currentState = State.Playing;
        InitializeUI();
        StopAllTimers();
        HideTimerUI();
        Time.timeScale = 1f;
        SceneManager.LoadScene("NewMenu");
        SceneManager.sceneLoaded += OnReturnToMenuUpdateCoins;
    }
    private void OnReturnToMenuUpdateCoins(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "NewMenu" || scene.name.Contains("Menu"))
        {
            FindObjectOfType<CollectableUI>()?.UpdateAllUI();

            SceneManager.sceneLoaded -= OnReturnToMenuUpdateCoins;
        }
    }

    #endregion

        #region GameMode

    public void ChangeGameMode()
    {
        bool isClassic = (currentGameMode == GameMode.Classic);
        currentGameMode = isClassic ? GameMode.Hard : GameMode.Classic;
        RestartLevel();
    }
    
    #endregion
    
    public GameMode GetCurrentGameMode() => currentGameMode;
    public State GetCurrentState() => currentState;
    public MusicManager MusicManager => musicManager;
}

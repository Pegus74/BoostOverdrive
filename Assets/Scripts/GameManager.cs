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


    public MusicManager musicManager;
    private string currentLevelName;
    public List<string> levelNames = new List<string>();

    public static GameManager Instance;

    public enum State
    {
        Playing,
        Paused,
        GameOver,
        GameWin
    }
    private State currentState = State.Playing;

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

            InitializeUI();

            if (musicManager != null)
                musicManager.PlayMusic();

            UpdateGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUI()
    {
        if (gameOverCanvas != null) gameOverCanvas.gameObject.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.gameObject.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);

        Debug.Log($" GameOver: {gameOverCanvas != null}, GameWin: {gameWinCanvas != null}, Pause: {pauseCanvas != null}");
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }


    private void UpdateGameState()
    {

        bool shouldTimeStop = (currentState != State.Playing);
        bool allowCursor = (currentState != State.Playing);
        bool changeMusicToGameOver = (currentState == State.GameOver);

        Time.timeScale = shouldTimeStop ? 0f : 1f;

        if (musicManager != null)
        {
            musicManager.SetVolume(changeMusicToGameOver ? 0.25f : 0.5f);
            musicManager.SetPitch(changeMusicToGameOver ? 0.7f : 1.0f);
        }

        Cursor.lockState = allowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = allowCursor;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedSceneSetup(scene));
    }

    private IEnumerator DelayedSceneSetup(Scene scene)
    {
        yield return new WaitForEndOfFrame();

        if (scene.name != "MainMenu" && !levelNames.Contains(scene.name))
        {
            levelNames.Add(scene.name);
        }

        if (scene.name == "MainMenu")
        {
            currentLevelName = null;
            currentState = State.Playing;
            InitializeUI();
            StopAllTimers();
            HideTimerUI();
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            currentLevelName = scene.name;
            InitializeUI();

            InitializeTimer();
            StartTimerOnSceneLoad();
            ShowTimerUI();
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
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null) timer.StopTimerWithoutSaving();
        UpdateGameState();
    }

    public void RestartLevel()
    {
        if (gameOverCanvas != null) gameOverCanvas.gameObject.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.gameObject.SetActive(false);
        currentState = State.Playing;
        UpdateGameState();
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
        currentState = State.Playing;
        InitializeUI();
        StopAllTimers();
        HideTimerUI();
        Time.timeScale = 1f;
        if (musicManager != null) musicManager.StopMusic();
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    public State GetCurrentState() => currentState;
}

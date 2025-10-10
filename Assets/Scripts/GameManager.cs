using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject gameWinCanvas;
    public GameObject pauseCanvas;

    public MusicManager musicManager;

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

            SceneManager.sceneLoaded += OnSceneLoaded;

            if (pauseCanvas != null)
                pauseCanvas.SetActive(false);
            if (gameOverCanvas != null)
                gameOverCanvas.SetActive(false);
            if (gameWinCanvas != null)
                gameWinCanvas.SetActive(false);
            if (musicManager != null)
                musicManager.PlayMusic();

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

    private void UpdateGameState()
    {
        bool shouldTimeStop = (currentState != State.Playing);
        bool allowCursor = (currentState != State.Playing);
        bool changeMusicToGameOver = (currentState == State.GameOver);

        Time.timeScale = shouldTimeStop ? 0f : 1f;

        musicManager.SetVolume(changeMusicToGameOver ? 0.25f : 0.5f);
        musicManager.SetPitch(changeMusicToGameOver ? 0.7f : 1.0f);

        Cursor.lockState = allowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = allowCursor;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            currentState = State.Playing;

            if (pauseCanvas != null)
                pauseCanvas.SetActive(false);
            if (gameOverCanvas != null)
                gameOverCanvas.SetActive(false);

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #region Win
    public void PlayerWin()
    {
        currentState = State.GameWin;
        gameWinCanvas.SetActive(true);
        UpdateGameState();
    }

    #endregion

    #region GameOver

    public void PlayerDied()
    {
        currentState = State.GameOver;
        gameOverCanvas.SetActive(true);
        UpdateGameState();
    }

    public void RestartLevel()
    {
        gameOverCanvas.SetActive(false);
        gameWinCanvas.SetActive(false);
        currentState = State.Playing;

        UpdateGameState();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    #endregion

    #region Paused

    public void TogglePause()
    {
        if (currentState != State.Playing && currentState != State.Paused && currentState != State.GameWin)
            return;

        bool isPlaying = (currentState == State.Playing);

        currentState = isPlaying ? State.Paused : State.Playing;

        pauseCanvas.SetActive(isPlaying);
        UpdateGameState();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        musicManager.StopMusic();

        if (pauseCanvas != null && pauseCanvas.activeInHierarchy)
            pauseCanvas.SetActive(false);
        if (gameOverCanvas != null && gameOverCanvas.activeInHierarchy)
            gameOverCanvas.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    public State GetCurrentState() => currentState;
}

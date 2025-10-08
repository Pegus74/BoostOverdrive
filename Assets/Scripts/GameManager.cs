using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public MusicManager musicManager;
    public GameObject pauseCanvas;

    public static GameManager Instance;

    public enum State
    {
        Playing,
        Paused,
        GameOver
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
        bool shouldTimeStop = (currentState == State.Paused);
        bool allowCursor = (currentState == State.Paused || currentState == State.GameOver);
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
        currentState = State.Playing;

        UpdateGameState();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    #endregion

    #region Paused

    public void TogglePause()
    {
        if (currentState != State.Playing && currentState != State.Paused)
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

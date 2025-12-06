using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [Header("References (автопоиск если не назначено)")]
    public TimerManager timerManager;
    public EnergyBar energyBar;
    public Canvas timerCanvas;

    [Header("Настройки поиска")]
    public string timerCanvasName = "Timer";
    public bool searchInChildren = true;

    void Awake()
    {
        // Поиск TimerManager
        if (timerManager == null)
        {
            timerManager = FindObjectOfType<TimerManager>();
            if (timerManager == null)
            {
                Debug.LogWarning("LevelStarter: TimerManager не найден на сцене!");
            }
        }

        // Поиск EnergyBar
        if (energyBar == null)
        {
            energyBar = FindObjectOfType<EnergyBar>();
            if (energyBar == null)
            {
                Debug.LogWarning("LevelStarter: EnergyBar не найден на сцене!");
            }
        }

        // Поиск Timer Canvas
        if (timerCanvas == null)
        {
            GameObject timerObj = GameObject.Find(timerCanvasName);
            if (timerObj != null)
            {
                timerCanvas = timerObj.GetComponent<Canvas>();
                if (timerCanvas == null && searchInChildren)
                {
                    timerCanvas = timerObj.GetComponentInChildren<Canvas>();
                }
            }

            if (timerCanvas == null)
            {
                Debug.LogWarning($"LevelStarter: Canvas с именем '{timerCanvasName}' не найден!");
            }
        }
    }

    void Start()
    {
        // Проверяем GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("LevelStarter: GameManager.Instance не найден!");
            return;
        }

        // Устанавливаем состояние игры
        GameManager.Instance.currentState = GameManager.State.Playing;
        GameManager.Instance.UpdateGameState();

        // Настраиваем режим игры
        if (GameManager.Instance.GetCurrentGameMode() == GameManager.GameMode.Classic)
        {
            SetupClassicMode();
        }
        else if (GameManager.Instance.GetCurrentGameMode() == GameManager.GameMode.Hard)
        {
            SetupHardMode();
        }
    }

    private void SetupClassicMode()
    {
        Debug.Log("LevelStarter: Настройка Classic режима");

        // Timer
        if (timerManager != null)
        {
            timerManager.enabled = true;
            timerManager.ShowTimer();
            timerManager.StartTimer();
        }
        else
        {
            Debug.LogWarning("Classic режим: TimerManager не найден!");
        }

        // Timer Canvas
        if (timerCanvas != null)
        {
            timerCanvas.enabled = true;
        }
        else
        {
            Debug.LogWarning("Classic режим: Timer Canvas не найден!");
        }

        // Energy Bar
        if (energyBar != null)
        {
            energyBar.Hide();
            energyBar.enabled = false;
        }
    }

    private void SetupHardMode()
    {
        Debug.Log("LevelStarter: Настройка Hard режима");

        // Energy Bar
        if (energyBar != null)
        {
            energyBar.enabled = true;
            energyBar.Show();
        }
        else
        {
            Debug.LogWarning("Hard режим: EnergyBar не найден!");
        }

        // Timer Canvas
        if (timerCanvas != null)
        {
            timerCanvas.enabled = false;
        }

        // Timer Manager
        if (timerManager != null)
        {
            timerManager.enabled = false;
        }
    }

    // Метод для ручного поиска компонентов (можно вызвать из инспектора)
    public void FindComponentsManually()
    {
        timerManager = FindObjectOfType<TimerManager>();
        energyBar = FindObjectOfType<EnergyBar>();

        GameObject timerObj = GameObject.Find(timerCanvasName);
        if (timerObj != null)
        {
            timerCanvas = timerObj.GetComponent<Canvas>();
        }

        Debug.Log($"LevelStarter: TimerManager найден: {timerManager != null}");
        Debug.Log($"LevelStarter: EnergyBar найден: {energyBar != null}");
        Debug.Log($"LevelStarter: TimerCanvas найден: {timerCanvas != null}");
    }
}
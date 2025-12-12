using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [Header("References (��������� ���� �� ���������)")]
    public TimerManager timerManager;
    public EnergyBar energyBar;
    public Canvas timerCanvas;

    [Header("��������� ������")]
    public string timerCanvasName = "Timer";
    public bool searchInChildren = true;

    void Awake()
    {
        // ����� TimerManager
        if (timerManager == null)
        {
            timerManager = FindObjectOfType<TimerManager>();
            if (timerManager == null)
            {
                Debug.LogWarning("LevelStarter: TimerManager �� ������ �� �����!");
            }
        }

        // ����� EnergyBar
        if (energyBar == null)
        {
            energyBar = FindObjectOfType<EnergyBar>();
            if (energyBar == null)
            {
                Debug.LogWarning("LevelStarter: EnergyBar �� ������ �� �����!");
            }
        }

        // ����� Timer Canvas
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
                Debug.LogWarning($"LevelStarter: Canvas � ������ '{timerCanvasName}' �� ������!");
            }
        }
    }

    void Start()
    {
        // ��������� GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("LevelStarter: GameManager.Instance �� ������!");
            return;
        }

        // ������������� ��������� ����
        GameManager.Instance.currentState = GameManager.State.Playing;
        GameManager.Instance.UpdateGameState();

        // ����������� ����� ����
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
        Debug.Log("LevelStarter: ��������� Classic ������");

        // Timer
        if (timerManager != null)
        {
            timerManager.enabled = true;
            timerManager.ShowTimer();
            timerManager.StartTimer();
        }
        else
        {
            Debug.LogWarning("Classic �����: TimerManager �� ������!");
        }

        // Timer Canvas
        if (timerCanvas != null)
        {
            timerCanvas.enabled = true;
        }
        else
        {
            Debug.LogWarning("Classic �����: Timer Canvas �� ������!");
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
        Debug.Log("LevelStarter: ��������� Hard ������");

        // Energy Bar
        if (energyBar != null)
        {
            energyBar.enabled = true;
            energyBar.Show();
        }
        else
        {
            Debug.LogWarning("Hard �����: EnergyBar �� ������!");
        }

        // Timer Canvas
        if (timerCanvas != null)
        {
            timerCanvas.enabled = false;
            Debug.Log("A123");
        }

        // Timer Manager
        if (timerManager != null)
        {
            timerManager.enabled = false;
            Debug.Log("B123");
        }
    }

    // ����� ��� ������� ������ ����������� (����� ������� �� ����������)
    public void FindComponentsManually()
    {
        timerManager = FindObjectOfType<TimerManager>();
        energyBar = FindObjectOfType<EnergyBar>();

        GameObject timerObj = GameObject.Find(timerCanvasName);
        if (timerObj != null)
        {
            timerCanvas = timerObj.GetComponent<Canvas>();
        }

        Debug.Log($"LevelStarter: TimerManager ������: {timerManager != null}");
        Debug.Log($"LevelStarter: EnergyBar ������: {energyBar != null}");
        Debug.Log($"LevelStarter: TimerCanvas ������: {timerCanvas != null}");
    }
}
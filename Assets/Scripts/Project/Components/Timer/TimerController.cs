using UnityEngine;

public class TimerController : MonoBehaviour
{
    public static TimerController Instance;

    private TimerModel model;
    [SerializeField] private TimerView view;

    private bool isRunning;
    private string pendingLevelName = null;
    private bool startCalled = false;

    private void Awake()
    {
        Instance = this;
        
        startCalled = true;
        
        if (pendingLevelName != null)
        {
            Debug.Log("Start");
            InitForLevel_Internal(pendingLevelName);
            pendingLevelName = null;
        }
    }

    public void InitForLevel(string levelName)
    {
        Debug.Log("InitForLevelExternal");
        if (!startCalled)
        {
            pendingLevelName = levelName;
            return;
        }

        InitForLevel_Internal(levelName);
    }

    private void InitForLevel_Internal(string levelName)
    {
        Debug.Log("InitForLevel_Internal");
        model = new TimerModel(levelName);

        if (view == null)
        {
            Debug.LogError("View is null");
            return;
        }
        
        view.UpdateBest(model.BestTime);
        view.UpdateTime(0f);
    }

    private void Update()
    {
        if (!isRunning) return;

        model.AddTime(Time.deltaTime);
        view.UpdateTime(model.CurrentTime);
    }

    public void StartTimer()
    {
        if (model == null)
        {
            return;
        }

        model.ResetTime();
        isRunning = true;
        Debug.Log("Starting timer");
    }

    public void StopTimer()
    {
        if (model == null)
            return;

        isRunning = false;
        model.TrySaveBest();
        view.UpdateBest(model.BestTime);
        Debug.Log("Stopping timer");
    }
    
    public void StopTimerWithoutSave()
    {
        if (model == null)
            return;

        isRunning = false;
    }

    public void ShowTimer(bool show)
    {
        view.SetVisible(show);
    }
}
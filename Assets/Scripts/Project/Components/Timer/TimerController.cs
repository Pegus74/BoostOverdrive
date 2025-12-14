using UnityEngine;

public class TimerController : MonoBehaviour
{
    public static TimerController Instance;

    private TimerModel model;
    [SerializeField] private TimerView view;

    private bool isRunning;

    private void Awake()
    {
        Instance = this;
    }

    public void InitForLevel(string levelName)
    {
        model = new TimerModel(levelName);
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
        model.ResetTime();
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
        model.TrySaveBest();
        view.UpdateBest(model.BestTime);
    }
}
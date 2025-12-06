using UnityEngine;

public class TimerModel
{
    private string levelKey;

    public float CurrentTime { get; private set; }
    public float BestTime { get; private set; }

    public TimerModel(string levelName)
    {
        levelKey = $"BestTime_{levelName}";

        CurrentTime = 0f;
        BestTime = PlayerPrefs.GetFloat(levelKey, Mathf.Infinity);
    }

    public void ResetTime()
    {
        CurrentTime = 0f;
    }

    public void AddTime(float delta)
    {
        CurrentTime += delta;
    }

    public void TrySaveBest()
    {
        if (CurrentTime < BestTime)
        {
            BestTime = CurrentTime;
            PlayerPrefs.SetFloat(levelKey, BestTime);
            PlayerPrefs.Save();
        }
    }
}
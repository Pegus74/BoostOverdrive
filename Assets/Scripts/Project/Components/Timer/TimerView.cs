using UnityEngine;
using TMPro;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bestText;

    public void UpdateTime(float time)
    {
        timeText.text = Format(time);
    }

    public void UpdateBest(float best)
    {
        if (float.IsInfinity(best))
            bestText.text = "--:--";
        else
            bestText.text = Format(best);
    }

    private string Format(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        int ms = (int)((t * 100) % 100);

        return $"{minutes:00}:{seconds:00}:{ms:00}";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI timerText;  
    [SerializeField] private TextMeshProUGUI bestTimeText; 
    private float currentTime = 0f;  
    private bool isTimerRunning = false;  
    private float bestTime = Mathf.Infinity; 
    private void Start()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
            UpdateBestTimeUI();
        }
        StartTimer();
    }
    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }
    public void StartTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
    }
    public void StopTimer()
    {
        isTimerRunning = false;
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();
        }
    }
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = (int)(currentTime / 60);
            int seconds = (int)(currentTime % 60);
            int milliseconds = (int)((currentTime % 1) * 1000);
            timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
    }
    private void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
        {
            if (bestTime == Mathf.Infinity)
            {
                bestTimeText.text = "Best: --:--.---";
            }
            else
            {
                int minutes = (int)(bestTime / 60);
                int seconds = (int)(bestTime % 60);
                int milliseconds = (int)((bestTime % 1) * 1000);
                bestTimeText.text = string.Format("Best: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
            }
        }
    }
}

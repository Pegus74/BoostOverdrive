using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject timerContainer;
    private float currentTime = 0f;
    private bool isTimerRunning = false;
    private float bestTime = Mathf.Infinity;
    private string currentLevelKey;

    public void InitializeTimer()
    {
        currentLevelKey = GameManager.GetBestTimeKeyForCurrentLevel();
        Debug.Log($"TimerManager initialized for level key: {currentLevelKey}");

        if (PlayerPrefs.HasKey(currentLevelKey))
        {
            bestTime = PlayerPrefs.GetFloat(currentLevelKey);
            UpdateBestTimeUI();
        }
        else
        {
            bestTime = Mathf.Infinity;
            UpdateBestTimeUI();
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerUI();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
            ResetCurrentTimer();
    }
    public void StartTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
    }
    public void StopTimer()
    {
        StopTimerAndSaveBestTime();
    }
    public void StopTimerAndSaveBestTime()
    {
        isTimerRunning = false;
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat(currentLevelKey, bestTime);
            PlayerPrefs.Save();
            Debug.Log($"Saved new best time: {bestTime} for key: {currentLevelKey}");
            UpdateBestTimeUI();
        }
        else
        {
            Debug.Log($"Current time {currentTime} is not better than best time {bestTime} for key: {currentLevelKey}");
        }
    }
    public void StopTimerWithoutSaving()
    {
        isTimerRunning = false;
    }

    public void ResetBestTime()
    {
        bestTime = Mathf.Infinity;
        PlayerPrefs.DeleteKey(currentLevelKey);
        PlayerPrefs.Save();
        UpdateBestTimeUI();
    }

    public void ResetBestTimeForLevel(string levelName)
    {

        string levelKey = GameManager.GetBestTimeKeyForLevel(levelName);
        PlayerPrefs.DeleteKey(levelKey);
        PlayerPrefs.Save();
        if (levelName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            bestTime = Mathf.Infinity;
            UpdateBestTimeUI();
        }
    }

    private void ResetCurrentTimer()
    {
        currentTime = 0f;
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
    public void ShowTimer()
    {
        if (timerContainer != null)
        {
            timerContainer.SetActive(true);
        }
        else
        {
            if (timerText != null) timerText.gameObject.SetActive(true);
            if (bestTimeText != null) bestTimeText.gameObject.SetActive(true);
        }
    }

    public void HideTimer()
    {
        if (timerContainer != null)
        {
            timerContainer.SetActive(false);
        }
        else
        {
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (bestTimeText != null) bestTimeText.gameObject.SetActive(false);
        }
    }

    public float GetBestTime()
    {
        return bestTime;
    }

    public static float GetBestTimeForLevel(string levelName)
    {
        string levelKey = GameManager.GetBestTimeKeyForLevel(levelName);
        if (PlayerPrefs.HasKey(levelKey))
        {
            return PlayerPrefs.GetFloat(levelKey);
        }
        return Mathf.Infinity;
    }
}

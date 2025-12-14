using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CollectableUI : MonoBehaviour
{
    [Header("References")]
    public CollectableSystem collectableSystem;

    [Header("UI Elements")]
    public TextMeshProUGUI pauseMenuCollectablesText;
    public Button resetLevelCoinsButton;
    public TextMeshProUGUI mainMenuCollectablesText;

    private string currentLevelName;
    private int totalCoinsOnLevel = 0;
    private readonly Dictionary<string, int> levelCoinCache = new();

    private void Start()
    {
        UpdateLevelName();
        CountCoinsOnLevel();
        UpdateAllUI();
        SetupResetButton();
    }

    private void OnEnable()
    {
        if (collectableSystem != null)
            collectableSystem.OnCoinCollected.AddListener(OnCoinsUpdated);

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateAllUI();
    }

    private void OnDisable()
    {
        if (collectableSystem != null)
            collectableSystem.OnCoinCollected.RemoveListener(OnCoinsUpdated);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelCoinCache.Clear();
        UpdateLevelName();
        CountCoinsOnLevel();
        UpdateAllUI();
        UpdateResetButtonVisibility();
    }

    private void UpdateLevelName()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.GetCurrentLevelName()))
        {
            currentLevelName = GameManager.Instance.GetCurrentLevelName();
        }
        else
        {
            currentLevelName = SceneManager.GetActiveScene().name;
        }

        if (string.IsNullOrEmpty(currentLevelName) || currentLevelName == "MainMenu" || currentLevelName.Contains("Menu"))
            currentLevelName = null; 
    }

    private void CountCoinsOnLevel()
    {
        if (string.IsNullOrEmpty(currentLevelName))
        {
            totalCoinsOnLevel = 0;
            return;
        }

        if (levelCoinCache.TryGetValue(currentLevelName, out int cached))
        {
            totalCoinsOnLevel = cached;
            return;
        }

        Collectable[] coins = FindObjectsOfType<Collectable>(true);
        totalCoinsOnLevel = coins.Length;
        levelCoinCache[currentLevelName] = totalCoinsOnLevel;
    }

    private void OnCoinsUpdated(int current, int max)
    {
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        UpdatePauseMenuUI();
        UpdateMainMenuUI();
    }

    private void UpdatePauseMenuUI()
    {
        if (pauseMenuCollectablesText == null) return;

        if (string.IsNullOrEmpty(currentLevelName) || collectableSystem == null)
        {
            pauseMenuCollectablesText.text = "Монеты: 0/0";
            return;
        }

        int collectedOnLevel = collectableSystem.GetLevelCollectedCount(currentLevelName);
        pauseMenuCollectablesText.text = $"Монеты: {collectedOnLevel}/{totalCoinsOnLevel}";
    }

    private void UpdateMainMenuUI()
    {
        if (mainMenuCollectablesText == null) return;

        if (collectableSystem == null)
        {
            mainMenuCollectablesText.text = "Всего: 0/0";
            return;
        }

        int totalCollected = collectableSystem.GetCollectedCoins();
        int maxGlobal = collectableSystem.maxGlobalCoins;

        mainMenuCollectablesText.text = $"Всего собрано: {totalCollected}/{maxGlobal}";

        // PROCENT
        // float percent = collectableSystem.GetCompletionPercentage();
        // mainMenuCollectablesText.text = $"Прогресс: {totalCollected}/{maxGlobal} ({percent:F1}%)";
    }

    public void SetupResetButton()
    {
        if (resetLevelCoinsButton != null)
        {
            resetLevelCoinsButton.onClick.RemoveAllListeners();
            resetLevelCoinsButton.onClick.AddListener(ResetLevelCoins);
            UpdateResetButtonVisibility();
        }
    }

    private void UpdateResetButtonVisibility()
    {
        if (resetLevelCoinsButton == null) return;

        bool inGameLevel = !string.IsNullOrEmpty(currentLevelName);
        resetLevelCoinsButton.gameObject.SetActive(inGameLevel);
    }

    private void ResetLevelCoins()
    {
        if (collectableSystem == null || string.IsNullOrEmpty(currentLevelName))
            return;

        collectableSystem.ResetLevelCoins(currentLevelName);
        levelCoinCache.Remove(currentLevelName);

        SceneManager.LoadScene(currentLevelName);
    }
}
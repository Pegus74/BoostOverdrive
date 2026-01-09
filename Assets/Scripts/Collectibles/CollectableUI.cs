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
    private Dictionary<string, int> levelCoinCache = new Dictionary<string, int>();

    private void Start()
    {
        UpdateLevelName();
        CountCoinsOnLevel();
        UpdateAllUI();

        if (resetLevelCoinsButton != null)
        {
            resetLevelCoinsButton.onClick.AddListener(ResetLevelCoins);
            UpdateResetButtonVisibility();
        }
    }

    private void OnEnable()
    {
        if (collectableSystem != null)
        {
            collectableSystem.OnCoinCollected.AddListener(OnCoinsUpdated);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateAllUI();
    }

    private void OnDisable()
    {
        if (collectableSystem != null)
        {
            collectableSystem.OnCoinCollected.RemoveListener(OnCoinsUpdated);
        }
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
        if (GameManager.Instance != null)
        {
            currentLevelName = GameManager.Instance.GetCurrentLevelName();
        }
     
        if (string.IsNullOrEmpty(currentLevelName))
        {
            currentLevelName = SceneManager.GetActiveScene().name;
        }
        if (string.IsNullOrEmpty(currentLevelName))
        {
            currentLevelName = "UnknownLevel";
        }
    }

    private void CountCoinsOnLevel()
    {
        UpdateLevelName();
        if (string.IsNullOrEmpty(currentLevelName))
        {
            Debug.LogWarning("CountCoinsOnLevel: currentLevelName is null or empty!");
            totalCoinsOnLevel = 0;
            return;
        }

        if (levelCoinCache.ContainsKey(currentLevelName))
        {
            totalCoinsOnLevel = levelCoinCache[currentLevelName];
            return;
        }
        Collectable[] allCoins = GameObject.FindObjectsOfType<Collectable>(true);
        totalCoinsOnLevel = allCoins.Length;

        levelCoinCache[currentLevelName] = totalCoinsOnLevel;

        Debug.Log($"Кэшировано монет для уровня '{currentLevelName}': {totalCoinsOnLevel}");
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

    public void UpdatePauseMenuUI()
    {
        if (pauseMenuCollectablesText != null)
        {
            UpdateLevelName();
            CountCoinsOnLevel();

            if (string.IsNullOrEmpty(currentLevelName) || collectableSystem == null)
            {
                pauseMenuCollectablesText.text = "Собрано: 0/0";
                return;
            }

            int collectedOnLevel = collectableSystem.GetLevelCollectedCount(currentLevelName);

            Debug.Log($"Пауза UI - Уровень: {currentLevelName}, Собрано: {collectedOnLevel}/{totalCoinsOnLevel}");

            pauseMenuCollectablesText.text = $"Собрано: {collectedOnLevel}/{totalCoinsOnLevel}";
        }
    }

    public void UpdateMainMenuUI()
    {
        if (mainMenuCollectablesText != null)
        {
            if (collectableSystem == null)
            {
                mainMenuCollectablesText.text = "Всего собрано: 0/0";
                return;
            }

            int collected = collectableSystem.GetCollectedCoins();
            int max = collectableSystem.maxGlobalCoins;
            mainMenuCollectablesText.text = $"Всего собрано: {collected}/{max}";
        }
    }

    private void UpdateResetButtonVisibility()
    {
        if (resetLevelCoinsButton != null)
        {
            bool isMainMenu = string.IsNullOrEmpty(currentLevelName) || currentLevelName == "MainMenu";
            resetLevelCoinsButton.gameObject.SetActive(!isMainMenu);
        }
    }

    private void ResetLevelCoins()
    {
        if (collectableSystem == null || string.IsNullOrEmpty(currentLevelName) || currentLevelName == "MainMenu")
        {
            Debug.LogWarning($"Cannot reset coins: collectableSystem={collectableSystem}, level={currentLevelName}");
            return;
        }

        Debug.Log($"Сброс монет для уровня: {currentLevelName}");

        collectableSystem.ResetLevelCoins(currentLevelName);

        if (levelCoinCache.ContainsKey(currentLevelName))
        {
            levelCoinCache.Remove(currentLevelName);
        }
        SceneManager.LoadScene(currentLevelName);
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectableUI : MonoBehaviour
{
    [Header("References")]
    public CollectableSystem collectableSystem;

    [Header("Pause Menu UI")]
    public TextMeshProUGUI pauseMenuCollectablesText;
    public Button resetLevelCoinsButton;

    [Header("Main Menu UI")]
    public TextMeshProUGUI mainMenuCollectablesText;

    private string currentLevelName;
    private int totalCoinsOnLevel = 0;

    private void Start()
    {
        currentLevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        CountCoinsOnLevel();
        UpdateAllUI();
        if (resetLevelCoinsButton != null)
        {
            resetLevelCoinsButton.onClick.AddListener(ResetLevelCoins);
            if (currentLevelName == "MainMenu")
            {
                resetLevelCoinsButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        if (collectableSystem != null)
        {
            collectableSystem.OnCoinCollected.AddListener(OnCoinsUpdated);
        }
        UpdateAllUI();
    }

    private void OnDisable()
    {
        if (collectableSystem != null)
        {
            collectableSystem.OnCoinCollected.RemoveListener(OnCoinsUpdated);
        }
    }

    private void CountCoinsOnLevel()
    {
        totalCoinsOnLevel = Collectable.CountAllCoinsOnLevel();
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
            int collectedOnLevel = collectableSystem.GetLevelCollectedCount(currentLevelName);
            int totalGlobal = collectableSystem.GetCollectedCoins();
            int maxGlobal = collectableSystem.maxGlobalCoins;

            pauseMenuCollectablesText.text = $"Собрано: {collectedOnLevel}/{totalCoinsOnLevel}";
        }
    }

    public void UpdateMainMenuUI()
    {
        if (mainMenuCollectablesText != null)
        {
            int collected = collectableSystem.GetCollectedCoins();
            int max = collectableSystem.maxGlobalCoins;
            mainMenuCollectablesText.text = $"Всего собрано: {collected}/{max}";
        }
    }

    private void ResetLevelCoins()
    {
        if (collectableSystem != null && currentLevelName != "MainMenu")
        {
            collectableSystem.ResetLevelCoins(currentLevelName);

            UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelName);
        }
    }
}
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Collectable Settings")]
    public CollectableSystem collectableSystem;

    private bool isCollected = false;
    private string uniqueId;
    private string levelName;
    public string CoinId => uniqueId;

    private void Awake()
    {
        levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        uniqueId = $"{gameObject.name}_{transform.position.x:F2}_{transform.position.y:F2}_{transform.position.z:F2}";

        if (IsAlreadyCollected())
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private bool IsAlreadyCollected()
    {
        string key = $"Coin_{levelName}_{uniqueId}";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void Collect()
    {
        if (isCollected) return;

        isCollected = true;
        collectableSystem.CollectCoin(levelName, uniqueId);

        CollectableUI ui = Object.FindObjectOfType<CollectableUI>();
        if (ui != null)
        {
            ui.UpdateAllUI();
        }

        gameObject.SetActive(false);
    }

    public string GetCoinId()
    {
        return uniqueId;
    }

    public static int CountAllCoinsOnLevel()
    {
        Collectable[] allCoins = FindObjectsOfType<Collectable>(true);
        return allCoins.Length;
    }
}
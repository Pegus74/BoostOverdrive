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
            Debug.Log($"Monetka {uniqueId} uje sobrana, skryvayu");
        }
        else
        {
            Debug.Log($"Monetka {uniqueId} gotova k sboru");
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
        bool collected = PlayerPrefs.GetInt(key, 0) == 1;

        if (collected)
        {
            Debug.Log($"Monetka {uniqueId} uje sobrana (key: {key})");
        }

        return collected;
    }

    private void Collect()
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"Sobiraem monetku: {uniqueId} na urovne: {levelName}");

        collectableSystem.CollectCoin(levelName, uniqueId);
        gameObject.SetActive(false);
    }
    public string GetCoinId()
    {
        return uniqueId;
    }

    public static int CountAllCoinsOnLevel()
    {
        Collectable[] allCoins = FindObjectsOfType<Collectable>(true);
        Debug.Log($"Vsego monetok na urovne: {allCoins.Length}");
        return allCoins.Length;
    }
}
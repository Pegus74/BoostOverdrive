using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Collectables/Collectable System")]
public class CollectableSystem : ScriptableObject
{

    public string playerPrefsKey = "CollectedCoins";
    public int maxGlobalCoins = 100;

    [System.Serializable]
    public class CoinEvent : UnityEvent<int, int> { }

    public CoinEvent OnCoinCollected;

    public void CollectCoin(string levelName, string coinId)
    {
        PlayerPrefs.SetInt($"Coin_{levelName}_{coinId}", 1);
        int currentCoins = GetCollectedCoins();
        if (currentCoins >= maxGlobalCoins) return;
        currentCoins++;
        PlayerPrefs.SetInt(playerPrefsKey, currentCoins);
        PlayerPrefs.Save();
        OnCoinCollected?.Invoke(currentCoins, maxGlobalCoins);
    }

    public int GetCollectedCoins()
    {
        return PlayerPrefs.GetInt(playerPrefsKey, 0);
    }

    public void ResetAllCoins()
    {
        PlayerPrefs.DeleteKey(playerPrefsKey);

        DeleteAllCoinKeys();

        PlayerPrefs.Save();
        OnCoinCollected?.Invoke(0, maxGlobalCoins);
    }

    public void ResetLevelCoins(string levelName)
    {

        List<string> coinIds = FindAllCoinIdsOnLevel(levelName);
        int coinsRemoved = coinIds.Count;
        foreach (string coinId in coinIds)
        {
            PlayerPrefs.DeleteKey($"Coin_{levelName}_{coinId}");
        }
        int currentCoins = GetCollectedCoins();
        currentCoins = Mathf.Max(0, currentCoins - coinsRemoved);
        PlayerPrefs.SetInt(playerPrefsKey, currentCoins);
        PlayerPrefs.Save();

        OnCoinCollected?.Invoke(currentCoins, maxGlobalCoins);
    }

    public int GetLevelCollectedCount(string levelName)
    {
        List<string> coinIds = FindAllCoinIdsOnLevel(levelName);
        return coinIds.Count;
    }
    private List<string> FindAllCoinIdsOnLevel(string levelName)
    {
        List<string> coinIds = new List<string>();
        Collectable[] allCoins = GameObject.FindObjectsOfType<Collectable>(true);

        foreach (Collectable coin in allCoins)
        {
            string coinId = coin.GetCoinId();
            if (!string.IsNullOrEmpty(coinId))
            {
                string key = $"Coin_{levelName}_{coinId}";
                if (PlayerPrefs.HasKey(key))
                {
                    coinIds.Add(coinId);
                }
            }
        }

        return coinIds;
    }

    private void DeleteAllCoinKeys()
    {
        for (int i = 0; i < 10000; i++)
        {
            string key = $"Coin_Level_{i}";
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }

    public float GetCompletionPercentage()
    {
        int collected = GetCollectedCoins();
        return (float)collected / maxGlobalCoins * 100f;
    }
}
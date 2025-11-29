using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Collectables/Collectable Data")]
public class CollectableData : ScriptableObject
{
    public CollectableType type;
    public string playerPrefsKey = "Collectables";

    [System.Serializable]
    public class CollectableEvent : UnityEvent<CollectableType> { }

    public CollectableEvent OnCollectableAdded;

    public void AddCollectable()
    {
        var current = GetCollected();
        if ((current & type) == type) return;

        var newCollection = current | type;
        PlayerPrefs.SetInt(playerPrefsKey, (int)newCollection);
        PlayerPrefs.Save();

        OnCollectableAdded?.Invoke(type);
    }

    public CollectableType GetCollected()
    {
        return (CollectableType)PlayerPrefs.GetInt(playerPrefsKey, 0);
    }

    public bool IsCollected(CollectableType checkType)
    {
        var current = GetCollected();
        return (current & checkType) == checkType;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey(playerPrefsKey);
        PlayerPrefs.Save();
    }
}
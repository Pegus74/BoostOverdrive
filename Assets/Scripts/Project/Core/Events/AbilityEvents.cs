using UnityEngine.Events;

[System.Serializable]
public static class AbilityEvents
{
    public static UnityEvent OnAbilityStarted = new UnityEvent();
    
    public static Vector3Event SlamDestructibleHitEvent = new Vector3Event(); 
    public static UnityEvent SlamSolidHitEvent = new UnityEvent();
}
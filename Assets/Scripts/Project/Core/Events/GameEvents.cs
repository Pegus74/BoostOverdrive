using UnityEngine.Events;

[System.Serializable]
public static class GameEvents
{
    public static GameStateEvent OnGameStateChanged = new GameStateEvent(); 
    
    public static UnityEvent OnClassicModeStart = new UnityEvent();
    public static UnityEvent OnHardModeStart = new UnityEvent();
    
    public static UnityEvent OnMenuMusicStart = new UnityEvent();
    public static UnityEvent OnGameMusicStart = new UnityEvent();
}
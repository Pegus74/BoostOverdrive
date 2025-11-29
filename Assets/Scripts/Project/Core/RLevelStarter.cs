using UnityEngine;
using UnityEngine.Events;

public class RLevelStarter : MonoBehaviour
{
    void Awake()
    {
        
    }
    
    void Start()
    {
        if (NewGameManager.Instance.GetCurrentLevelName() == "MainMenuRefactor")
        {
            GameEvents.OnMenuMusicStart.Invoke();
        }
        else
        {
            GameEvents.OnGameMusicStart.Invoke();
        }
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Classic)
        {
            GameEvents.OnClassicModeStart.Invoke();
            // timer on
        }
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Hard)
        {
            GameEvents.OnHardModeStart.Invoke();
            // timer off
        }
    }
}
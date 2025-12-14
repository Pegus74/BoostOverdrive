using UnityEngine;
using UnityEngine.Events;

public class RLevelStarter : MonoBehaviour
{
    void Awake()
    {
        
    }
    
    void Start()
    {
        string levelName = NewGameManager.Instance.GetCurrentLevelName();
        
        TimerController.Instance.InitForLevel(levelName);
        
        if (levelName == "MainMenuRefactor")
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
            TimerController.Instance.StartTimer();
        }
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Hard)
        {
            GameEvents.OnHardModeStart.Invoke();
            TimerController.Instance.StopTimer();
        }
    }
}
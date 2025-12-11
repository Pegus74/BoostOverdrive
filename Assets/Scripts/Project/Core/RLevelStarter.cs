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
        
        if (levelName == "MainMenuRefactor")
        {
            GameEvents.OnMenuMusicStart.Invoke();
            return;
        }

        GameEvents.OnGameMusicStart.Invoke();
        
        TimerController.Instance.InitForLevel(levelName);
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Classic)
        {
            GameEvents.OnClassicModeStart.Invoke();
            TimerController.Instance.ShowTimer(true);
            TimerController.Instance.StartTimer();
        }
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Hard)
        {
            GameEvents.OnHardModeStart.Invoke();
            TimerController.Instance.ShowTimer(false);
            TimerController.Instance.StopTimerWithoutSave();
        }
    }
}
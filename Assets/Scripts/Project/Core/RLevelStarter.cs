using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RLevelStarter : MonoBehaviour
{
    public InputActionAsset actions;
    
    void Start()
    {
        
        string levelName = NewGameManager.Instance.GetCurrentLevelName();
        Debug.Log(levelName);
        
        // if (!PlayerPrefs.HasKey("InputBindings"))
        //     return;
        //
        // string json = PlayerPrefs.GetString("InputBindings");
        // actions.LoadBindingOverridesFromJson(json);
        //
        // Debug.Log("Bindings loaded");
        
        if (levelName == "MainMenuRefactor")
        {
            GameEvents.OnMenuMusicStart.Invoke();
            return;
        }

        if (levelName == "HUB")
        {
            GameEvents.OnGameMusicStart.Invoke();
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
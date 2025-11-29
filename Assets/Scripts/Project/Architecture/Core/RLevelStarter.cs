using UnityEngine;
using UnityEngine.Events;


public class RLevelStarter : MonoBehaviour
{
    // private TimerManager timerManager;
    private EnergyBar energyBar;
    // private Canvas timerCanvas;
    

    void Awake()
    {
        // timerManager = FindObjectOfType<TimerManager>();
        energyBar = FindObjectOfType<EnergyBar>();
        // timerCanvas = GameObject.Find("Timer").GetComponent<Canvas>();
    }
    
    void Start()
    {
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Classic)
        {
            // timerManager.enabled = true;
            // timerCanvas.enabled = true;
            GameEvents.OnClassicModeStart.Invoke();
            
            // timerManager.ShowTimer();
            // timerManager.StartTimer();
        }
        
        if (NewGameManager.Instance.GetCurrentGameMode() == GameMode.Hard)
        {
            GameEvents.OnHardModeStart.Invoke();
            
            // timerCanvas.enabled = false;
            // timerManager.enabled = false;
        }
    }
}
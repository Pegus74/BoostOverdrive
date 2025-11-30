using Unity.VisualScripting;
using UnityEngine;


public class LevelStarter : MonoBehaviour
{
    private TimerManager timerManager;
    private EnergyBar energyBar;
    private Canvas timerCanvas;

    void Awake()
    {
        timerManager = FindObjectOfType<TimerManager>();
        energyBar = FindObjectOfType<EnergyBar>();
        timerCanvas = GameObject.Find("Timer").GetComponent<Canvas>();
    }
    
    void Start()
    {
        GameManager.Instance.currentState = GameManager.State.Playing;
        GameManager.Instance.UpdateGameState();
        
        
        if (GameManager.Instance.GetCurrentGameMode() == GameManager.GameMode.Classic)
        {
            timerManager.enabled = true;
            timerCanvas.enabled = true;
            energyBar.Hide();
            energyBar.enabled = false;
            timerManager.ShowTimer();
            timerManager.StartTimer();
        }
        
        if (GameManager.Instance.GetCurrentGameMode() == GameManager.GameMode.Hard)
        {

            energyBar.enabled = true;
            timerCanvas.enabled = false;
            timerManager.enabled = false;
            energyBar.Show();
        }
    }
}
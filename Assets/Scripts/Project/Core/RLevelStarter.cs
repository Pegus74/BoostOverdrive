using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RLevelStarter : MonoBehaviour
{
    public InputActionAsset actions;

    void Start()
    {
        string levelName = NewGameManager.Instance.GetCurrentLevelName();
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log($"Загрузка: {levelName} (индекс: {buildIndex})");

        if (levelName != "MainMenuRefactor" && levelName != "HUB")
        {
            int levelNumber = GetLevelNumberFromBuildIndex(buildIndex);

            ExtendedGameEvents.OnLevelChanged.Invoke(levelNumber);

            PlayerPrefs.SetInt("CurrentLevelNumber", levelNumber);
            PlayerPrefs.Save();

            Debug.Log($"Установлен уровень {levelNumber} для музыки");
        }

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

    private int GetLevelNumberFromBuildIndex(int buildIndex)
    {


        if (buildIndex == 1)
        {
            return 1; 
        }
        else if (buildIndex >= 3)
        {
            return buildIndex - 1; 
        }

        return 1; 
    }
}
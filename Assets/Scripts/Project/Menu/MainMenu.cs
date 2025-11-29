using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas selectLevelCanvas;
    [SerializeField] private Canvas settingsCanvas;

    public void OnQuitClicked() => Application.Quit();
    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    private void Awake()
    {
        settingsCanvas.gameObject.SetActive(false);
        selectLevelCanvas.gameObject.SetActive(false);
    }

    public void ShowSettings()
    {
        mainCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }

    public void ShowLevelMenu()
    {
        mainCanvas.gameObject.SetActive(false);
        selectLevelCanvas.gameObject.SetActive(true);
    }

    public void HideLevelMenu()
    {
        selectLevelCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }
}
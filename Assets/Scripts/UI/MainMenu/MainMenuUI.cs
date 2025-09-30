using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject[] mainButtons;   
    [SerializeField] private GameObject[] settingsButtons; 

    public void OnQuitClicked() => Application.Quit();
    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    private void Awake()
    {
        foreach (var button in settingsButtons)
            button.SetActive(false);
    }

    public void ShowSettings()
    {
        foreach (GameObject button in mainButtons)
            button.SetActive(false);
        foreach (GameObject button in settingsButtons)
            button.SetActive(true);
    }

    public void HideSettings()
    {
        foreach (GameObject button in settingsButtons)
            button.SetActive(false);
        foreach (GameObject button in mainButtons)
            button.SetActive(true);
    }
}

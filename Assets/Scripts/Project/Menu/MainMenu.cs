using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas")] 
    [SerializeField] private Canvas firstEnterCanvas;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Canvas collectablesCanvas;

    public void OnQuitClicked() => Application.Quit();
    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("ContinueLevel"))
        {
            mainCanvas.gameObject.SetActive(true);
            firstEnterCanvas.gameObject.SetActive(false);
        }
        else
        {
            mainCanvas.gameObject.SetActive(false);
            firstEnterCanvas.gameObject.SetActive(true);
        }
            
        settingsCanvas.gameObject.SetActive(false);
        collectablesCanvas.gameObject.SetActive(false);
    }

    public void ContinueGame()
    {
        int level = PlayerPrefs.GetInt("ContinueLevel");
        SceneManager.LoadScene(level);
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        LoadScene("Demo(Refactor)");
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

    public void ShowCollectables()
    {
        mainCanvas.gameObject.SetActive(false);
        collectablesCanvas.gameObject.SetActive(true); // 
    }

    public void HideCollectables()
    {
        collectablesCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }
}
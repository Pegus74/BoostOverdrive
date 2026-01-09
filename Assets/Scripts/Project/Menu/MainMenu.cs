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
    [SerializeField] private Canvas keybindsCanvas;
    
    public ControlsMenu controls;

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
        // int level = PlayerPrefs.GetInt("ContinueLevel");
        // SceneManager.LoadScene(level);
        
        SceneManager.LoadScene("HUB");
    }

    public void StartNewGame()
    {
        float temp = PlayerPrefs.GetFloat("MusicVolume");
        float temp2 = PlayerPrefs.GetFloat("SoundVolume");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("MusicVolume", temp);
        PlayerPrefs.SetFloat("SoundVolume", temp2);
        LoadScene("1-1");
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

    public void ShowBindings()
    {
        settingsCanvas.gameObject.SetActive(false);
        controls.LoadBindings();
        keybindsCanvas.gameObject.SetActive(true);
    }
    
    public void HideBindings()
    {
        settingsCanvas.gameObject.SetActive(true);
        keybindsCanvas.gameObject.SetActive(false);
    }

    public void SaveBindings()
    {
        controls.SaveBindings();
    }

    public void ShowCollectables()
    {
        mainCanvas.gameObject.SetActive(false);
        collectablesCanvas.gameObject.SetActive(true);
    }

    public void HideCollectables()
    {
        collectablesCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }
}
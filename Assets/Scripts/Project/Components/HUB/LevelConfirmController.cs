using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConfirmController : MonoBehaviour
{
    public static LevelConfirmController Instance;

    [Header("UI")]
    public TMP_Text levelText;
    public TMP_Text bestTimeText;

    private string currentLevelIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);
    }

    public void Show(string levelIndex)
    {
        NewGameManager.Instance.ShowConfirm();
        currentLevelIndex = levelIndex;

        levelText.text = $"{levelIndex}";

        string key = $"BestTime_{levelIndex}";

        if (PlayerPrefs.HasKey(key))
        {
            float time = PlayerPrefs.GetFloat(key);
            bestTimeText.text = $"{time:F4} сек";
        }
        else
        {
            bestTimeText.text = "—";
        }

        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        NewGameManager.Instance.HideConfirm();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentLevelIndex);
    }
}
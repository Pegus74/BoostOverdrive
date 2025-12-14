using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CollectableUI))]
public class CollectableUIAutoSetup : MonoBehaviour
{
    public Canvas pauseCanvas;
    public TextMeshProUGUI existingPauseText;
    public Button existingResetButton;

    private CollectableUI collectableUI;

    private void Awake()
    {
        collectableUI = GetComponent<CollectableUI>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu") || scene.name.Contains("menu") || scene.name == "MainMenu")
            return;

        StartCoroutine(SetupLevelUI());
    }

    private System.Collections.IEnumerator SetupLevelUI()
    {
        yield return null;

        pauseCanvas ??= GameObject.Find("PauseCanvas")?.GetComponent<Canvas>()
                     ?? GameObject.Find("Pause Canvas")?.GetComponent<Canvas>()
                     ?? FindObjectOfType<Canvas>();

        if (pauseCanvas == null)
            yield break;

        GameObject container = new GameObject("Auto_CoinsContainer");
        container.transform.SetParent(pauseCanvas.transform, false);

        var containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0, 0);
        containerRT.anchorMax = new Vector2(0, 0);
        containerRT.pivot = new Vector2(0, 0);
        containerRT.anchoredPosition = new Vector2(501, 378);

        if (collectableUI.pauseMenuCollectablesText == null && existingPauseText == null)
        {
            GameObject textObj = new GameObject("Auto_CoinText_Pause");
            textObj.transform.SetParent(container.transform, false);

            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Монеты: 0/0";
            text.fontSize = 32;
            text.color = new Color(1f, 0.92f, 0.016f);
            text.alignment = TextAlignmentOptions.Left;

            var rt = textObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 50);
            rt.anchoredPosition = new Vector2(0, 30);

            collectableUI.pauseMenuCollectablesText = text;
        }
        else if (existingPauseText != null)
        {
            collectableUI.pauseMenuCollectablesText = existingPauseText;
        }

        if (collectableUI.resetLevelCoinsButton == null && existingResetButton == null)
        {
            GameObject btnObj = new GameObject("Auto_ResetLevelCoinsButton");
            btnObj.transform.SetParent(container.transform, false);

            var img = btnObj.AddComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(btnObj.transform, false);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Сбросить монеты уровня";
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var rt = btnObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 50);
            rt.anchoredPosition = new Vector2(0, -30);

            collectableUI.resetLevelCoinsButton = btn;
        }
        else if (existingResetButton != null)
        {
            collectableUI.resetLevelCoinsButton = existingResetButton;
        }

        collectableUI.SetupResetButton();
        collectableUI.UpdateAllUI();
    }
}
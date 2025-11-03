using UnityEngine;

public class TutorialSignAutoRegister : MonoBehaviour
{
    private TutorialSign tutorialSign;

    private void Awake()
    {
        tutorialSign = GetComponent<TutorialSign>();
    }

    private void Start()
    {
        if (TutorialManager.Instance != null && tutorialSign != null)
        {
            TutorialManager.Instance.RegisterSign(tutorialSign);
        }
    }

    private void OnDestroy()
    {
        if (TutorialManager.Instance != null && tutorialSign != null)
        {
            TutorialManager.Instance.UnregisterSign(tutorialSign);
        }
    }
}
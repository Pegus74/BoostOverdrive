using UnityEngine;
using UnityEngine.UI;

public class ResetBestTimesButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ResetAllBestTimes);
    }

    public void ResetAllBestTimes()
    {
        GameManager.Instance.ResetAllBestTimes();
        Debug.Log(" времена сброшены");
    }
}
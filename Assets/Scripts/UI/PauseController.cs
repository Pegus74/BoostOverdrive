using UnityEngine;

public class PauseController : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePause();
        }
    }
}
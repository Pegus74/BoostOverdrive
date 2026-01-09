using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleporter: MonoBehaviour
{
    [SerializeField] private string levelName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelConfirmController.Instance.Show(levelName);
        }
    }
}
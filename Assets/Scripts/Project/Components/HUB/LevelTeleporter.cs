using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleporter: MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private float borderTime;
    [SerializeField] private string biomeName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelConfirmController.Instance.Show(levelName,borderTime,biomeName);
        }
    }
}
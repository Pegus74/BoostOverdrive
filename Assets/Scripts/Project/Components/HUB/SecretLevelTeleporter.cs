using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretLevelTeleporter: MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private int colNumber;
    [SerializeField] private string biomeName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SecretLevelConfirmController.Instance.Show(levelName,colNumber,biomeName);
        }
    }
}
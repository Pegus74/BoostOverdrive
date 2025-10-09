using UnityEngine;

public class VoidScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
        }
    }
}
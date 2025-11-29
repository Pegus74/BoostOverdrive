using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectableData collectableData;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        collectableData.AddCollectable();
        gameObject.SetActive(false);
    }
}
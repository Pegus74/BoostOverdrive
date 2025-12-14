using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DryPlatformNode : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public DryStripController stripController;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        stripController.OnPlatformStepped(index);
    }
}
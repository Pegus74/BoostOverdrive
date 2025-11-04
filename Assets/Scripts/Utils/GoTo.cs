using UnityEngine;
using UnityEngine.SceneManagement;
public class GoTo : MonoBehaviour
{
    public Transform targetPoint; 
    public KeyCode key = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(key))
            Teleport();
    }

    void Teleport()
    {
        transform.position = targetPoint.position;
        if (TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = Vector3.zero;
    }
}
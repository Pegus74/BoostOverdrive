using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TMP_Text speedLabel;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 velH = rb.velocity;
        velH.y = 0f;
        float units = velH.magnitude;
        speedLabel.text = $"Speed: {units:F1}";
    }
}
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text xyzLabel;
    [SerializeField] private TMP_Text isGroundedLabel;
    [SerializeField] private TMP_Text jumpLabel;
    private Rigidbody rb;
    [SerializeField] private FirstPersonController fpsController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGroundedLabel.text = "IsGrounded: " + fpsController.isGrounded;
        jumpLabel.text = "IsJumping" + fpsController.isJumping;
        Vector3 velH = rb.velocity;
        velH.y = 0f;
        float units = velH.magnitude;
        speedLabel.text = $"Speed: {units:F1}";
    }
}
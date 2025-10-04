using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text styleLabel;
    private Rigidbody rb;
    StyleController controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<StyleController>();
    }

    private void Update()
    {
        var styleIndex = controller.GetCurrentStyle();
        
        Vector3 velH = rb.velocity;
        velH.y = 0f;
        float units = velH.magnitude;
        speedLabel.text = $"Speed: {units:F1}";
        styleLabel.text = $"Style: {styleIndex.styleName}";
    }
}
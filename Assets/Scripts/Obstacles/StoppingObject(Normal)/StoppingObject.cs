using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoppingObject : MonoBehaviour
{
    private FirstPersonController controller;

    public float slowdownModifier = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = FindObjectOfType<FirstPersonController>();
            controller.SetSpeedModifier(slowdownModifier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.ResetSpeedModifier();
        }
    }
}

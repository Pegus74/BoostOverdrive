using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTile : MonoBehaviour
{
    public float slowMultiplier = 0.7f;  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.SetSpeedModifier(slowMultiplier);  
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.SetSpeedModifier(slowMultiplier);  
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.ResetSpeedModifier(); 
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTile : MonoBehaviour
{
    public float accelMultiplier = 1.3f;  
    public float lingerTimeWalk = 0.5f;   // Время длительности буста после ходьбы 
    public float lingerTimeJump = 0.2f;   // Время длительности буста после прыжка 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController controller = other.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.SetSpeedModifier(accelMultiplier);  
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
                controller.SetSpeedModifier(accelMultiplier);  
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
                float lingerTime = controller.isGrounded ? lingerTimeWalk : lingerTimeJump;  
                controller.StartLingerSpeedModifier(lingerTime);  
            }
        }
    }
}

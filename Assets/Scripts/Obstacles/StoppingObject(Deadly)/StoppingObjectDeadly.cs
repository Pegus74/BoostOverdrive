using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoppingObjectDeadly : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
        }
    }
}

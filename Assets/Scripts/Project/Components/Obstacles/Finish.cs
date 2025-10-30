using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFinish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (NewGameManager.Instance != null)
            {
                NewGameManager.Instance.PlayerWin(); 
            }
        }
    }
}
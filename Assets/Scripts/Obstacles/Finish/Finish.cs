using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private string levelName;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerWin(levelName);
            }
        }
    }
}

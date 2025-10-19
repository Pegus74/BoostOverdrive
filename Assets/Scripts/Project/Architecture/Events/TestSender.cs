using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSender : MonoBehaviour
{
    [SerializeField] private GameEvent OnPlayerJumpAttempt;
    
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerJumpAttempt.Raise();
            Debug.Log("sender: event raised");
        }
    }
}

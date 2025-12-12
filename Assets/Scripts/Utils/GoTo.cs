using UnityEngine;
using UnityEngine.SceneManagement;
public class GoTo : MonoBehaviour
{

    public KeyCode key = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(key))
            Restart();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
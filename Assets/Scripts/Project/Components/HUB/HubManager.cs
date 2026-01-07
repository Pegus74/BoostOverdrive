using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager: MonoBehaviour
{
    public GameObject lock1;
    public GameObject lock2;
    // public GameObject lock3;


    private void Start()
    {
        if (PlayerPrefs.GetFloat("AllTime", 0) < 91f && PlayerPrefs.GetInt("ContinueLevel", 0) >= 6)
        {
            lock1.SetActive(false);
        }

        if (PlayerPrefs.GetFloat("AllTime", 0) < 53.5f && PlayerPrefs.GetInt("ContinueLevel", 0) >= 9)
        {
            lock2.SetActive(false);
        }

        // if (PlayerPrefs.GetFloat("AllTime", 0) < 50f)
        // {
        //     lock3.SetActive(false);
        // }
    }

}
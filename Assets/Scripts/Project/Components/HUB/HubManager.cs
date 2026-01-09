using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager: MonoBehaviour
{
    public GameObject lock1;
    public GameObject lock2;
    
    public GameObject loc1;
    public GameObject loc2;
    public GameObject loc3;
    
    [SerializeField] private Transform _playerTransform;
    
    private void Awake()
    {
        Vector3 playerPos = _playerTransform.position;
        Quaternion playerRot = _playerTransform.rotation;
        
        if (PlayerPrefs.GetFloat("AllTime", 0) < 91f && PlayerPrefs.GetInt("ContinueLevel", 0) >= 8)
        {
            lock1.SetActive(false);
            playerPos = loc2.transform.position;
            playerRot = loc2.transform.rotation;
            InputEvents.OnSecondBiomeTeleportEvent.AddListener(TpTo2);
            
        }

        if (PlayerPrefs.GetFloat("AllTime", 0) < 144.5f && PlayerPrefs.GetInt("ContinueLevel", 0) >= 11)
        {
            lock2.SetActive(false);
            playerPos = loc3.transform.position;
            playerRot = loc3.transform.rotation;
            InputEvents.OnThirdBiomeTeleportEvent.AddListener(TpTo3);
        }
        
        _playerTransform.position = playerPos;
        _playerTransform.rotation = playerRot;
    }

    
    void OnEnable()
    { 
        InputEvents.OnFirstBiomeTeleportEvent.AddListener(TpTo1);
    }

    void OnDisable()
    {
        InputEvents.OnFirstBiomeTeleportEvent.RemoveListener(TpTo1);
        InputEvents.OnSecondBiomeTeleportEvent.RemoveListener(TpTo2);
        InputEvents.OnThirdBiomeTeleportEvent.RemoveListener(TpTo3);
    }

    private void TpTo1()
    {
        _playerTransform.position = loc1.transform.position;
    }

    private void TpTo2()
    {
        _playerTransform.position = loc2.transform.position;
    }

    private void TpTo3()
    {
        _playerTransform.position = loc3.transform.position;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamIndicatorSystem : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;

    [Header("Indicator Prefab")]
    public GameObject slamIndicatorPrefab;

    [Header("Layer Settings")]
    public LayerMask groundLayerMask = 1; 

    private GameObject slamIndicatorInstance;
    private bool wasSlamming = false;

    void Start()
    {
        InitializeIndicator();
    }

    void Update()
    {
        UpdateSlamIndicator();
    }

    private void InitializeIndicator()
    {
        if (slamIndicatorPrefab != null)
        {
            slamIndicatorInstance = Instantiate(slamIndicatorPrefab);
            slamIndicatorInstance.SetActive(false);
        }
    }

    private void UpdateSlamIndicator()
    {
        bool isSlamming = playerStateModel.IsSlamming;
        bool isGrounded = playerStateModel.IsGrounded;


        bool shouldShowIndicator = !isGrounded && !isSlamming && playerSettingsData.enableSlam;

        if (shouldShowIndicator)
        {
            ShowIndicator();
        }
        else
        {
            HideIndicator();
        }

        wasSlamming = isSlamming;
    }

    private void ShowIndicator()
    {
        if (slamIndicatorInstance == null) return;

        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMask))
        {
            slamIndicatorInstance.transform.position = hit.point + Vector3.up * 0.01f;
            slamIndicatorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            slamIndicatorInstance.SetActive(true);
        }
        else
        {
            slamIndicatorInstance.SetActive(false);
        }
    }

    private void HideIndicator()
    {
        if (slamIndicatorInstance != null)
        {
            slamIndicatorInstance.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (slamIndicatorInstance != null)
        {
            Destroy(slamIndicatorInstance);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DryStripController : MonoBehaviour
{
    [Header("Platforms")]
    [SerializeField] private List<DryPlatformView> platforms;
    
    public ObstaclesSettingsData obstaclesSettingsData;

    private DryStripModel model;
    private bool[] destroyed;

    private void Awake()
    {
        model = new DryStripModel();
        destroyed = new bool[platforms.Count];

        for (int i = 0; i < platforms.Count; i++)
        {
            var node = platforms[i].GetComponent<DryPlatformNode>();
            node.index = i;
            node.stripController = this;
        }
    }

    public void OnPlatformStepped(int startIndex)
    {
        if (model.IsTriggered) return;

        model.Trigger();
        StartCoroutine(DestroyChain(startIndex));
    }

    private IEnumerator DestroyChain(int startIndex)
    {
        for (int i = startIndex; i < platforms.Count; i++)
        {
            if (platforms[i] == null || destroyed[i])
                continue;

            destroyed[i] = true;

            platforms[i].PlayCrack();
            platforms[i].ShowCracked();

            yield return new WaitForSeconds(obstaclesSettingsData.DryStripDestroyDelay);

            if (platforms[i] != null)
                platforms[i].DestroyPlatform();
        }
    }
}
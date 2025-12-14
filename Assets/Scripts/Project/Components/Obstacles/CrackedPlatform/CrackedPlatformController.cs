using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CrackedPlatformController : MonoBehaviour
{
    [Header("Settings")]
    
    public ObstaclesSettingsData obstaclesSettingsData;

    private CrackedPlatformModel model;
    private CrackedPlatformView view;

    private void Awake()
    {
        model = new CrackedPlatformModel(obstaclesSettingsData.CrackPlatDestroyDelay);
        view = GetComponent<CrackedPlatformView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (model.IsTriggered) return;

        model.Trigger();

        view.ShowCracked();
        view.PlayCrackSound();

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(model.destroyDelay);

        model.Destroy();
        view.DestroyPlatform();
    }
}
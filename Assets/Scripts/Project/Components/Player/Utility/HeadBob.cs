using UnityEngine;


public class PlayerHeadBob : MonoBehaviour
{
    public PlayerSettingsData playerSettingsData;
    public PlayerStateModel playerStateModel;
    public Transform joint;

    private Vector3 jointOriginalPos;
    private float timer = 0;

    private void Update()
    {
        HeadBob();
    }
    
    private void HeadBob()
    {
        if (playerSettingsData.enableHeadBob && playerStateModel.IsWalking)
        {
            timer += Time.deltaTime * playerSettingsData.bobSpeed;
            joint.localPosition = jointOriginalPos + new Vector3(
                Mathf.Sin(timer) * playerSettingsData.bobAmount.x,
                Mathf.Sin(timer) * playerSettingsData.bobAmount.y,
                Mathf.Sin(timer) * playerSettingsData.bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * playerSettingsData.bobSpeed);
        }
    }
}

using UnityEngine;

public class StyleModelTester : MonoBehaviour
{
    [SerializeField] private PlayerStateModel playerStateModel;
    [SerializeField] private KeyCode testKey = KeyCode.T;
    private int currentTestStyle = 0;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            currentTestStyle = (currentTestStyle + 1) % 2;
            playerStateModel.SetStyleIndex(currentTestStyle);
            playerStateModel.OnStyleChangedEvent.Raise(currentTestStyle);

            Debug.Log($"MODEL style set to: {currentTestStyle}");
        }
    }
}
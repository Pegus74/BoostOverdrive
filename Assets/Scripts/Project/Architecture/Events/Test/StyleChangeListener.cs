using UnityEngine;

public class StyleChangeListener : MonoBehaviour
{
    [SerializeField] private IntEvent styleChangedEvent;

    private void OnEnable()
    {
        if (styleChangedEvent != null)
        {
            styleChangedEvent.RegisterListener(HandleStyleChange);
        }
    }

    private void OnDisable()
    {
        if (styleChangedEvent != null)
        {
            styleChangedEvent.UnregisterListener(HandleStyleChange);
        }
    }

    private void HandleStyleChange(int newStyleIndex)
    {
        Debug.Log($"LISTENER received new style index: {newStyleIndex}");
    }
}
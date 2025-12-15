using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "StickyPlatformSettings", menuName = "Settings/Sticky Platform Settings")]
public class StickyPlatformSettings : ScriptableObject
{
    [Header("Sink Settings")]
    [Range(0f, 1f)]
    public float sinkPercent = 0.5f;
    public float sinkDuration = 0.3f;
    public float unSinkDuration = 0.2f;

    [Header("Sticky Effect")]
    [Range(0f, 1f)]
    public float stickySpeedModifier = 0.4f;

    [Header("Double Jump Escape")]
    public float doubleTapWindow = 0.4f;
}
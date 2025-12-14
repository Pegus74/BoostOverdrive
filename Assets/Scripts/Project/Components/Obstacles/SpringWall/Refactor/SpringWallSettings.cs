using UnityEngine;

[CreateAssetMenu(fileName = "SpringWallSettings", menuName = "Settings/SpringWall Settings")]
public class SpringWallSettings : ScriptableObject
{
    [Header("Hands Style Settings")]
    public float horizontalForceHands = 10f;
    public float verticalForceHands = 12f;
    public float handsSpeedModifier = 0.7f;
    public float handsSpeedModifierDuration = 1f;

    [Header("Legs Style Settings")]
    public float horizontalForceLegs = 8f;
    public float verticalForceLegs = 15f;
}
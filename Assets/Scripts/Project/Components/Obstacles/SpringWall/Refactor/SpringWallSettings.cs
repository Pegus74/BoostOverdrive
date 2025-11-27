using UnityEngine;

[CreateAssetMenu(fileName = "SpringWallSettings", menuName = "Settings/SpringWall Settings")]
public class SpringWallSettings : ScriptableObject
{
    [Header("Detection Settings")]
    public float activationDistance = 1.0f;
    public float raycastCheckDistance = 1.0f;
    public LayerMask playerLayer;

    [Header("Hands Style Settings")]
    public float reboundForceHands = 10f;
    public float extraAccelerationHands = 5f;
    public float handsSpeedModifier = 0.7f;
    public float handsSpeedModifierDuration = 1f;

    [Header("Legs Style Settings")]
    public float horizontalForceLegs = 8f;
    public float verticalForceLegs = 12f;
}
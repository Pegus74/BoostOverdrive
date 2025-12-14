using UnityEngine;

[CreateAssetMenu(fileName = "SpringWallSettings", menuName = "Settings/SpringWall Settings")]
public class SpringWallSettings : ScriptableObject
{
    [Header("Detection Settings")]
    public float activationDistance = 8.0f;
    public float raycastCheckDistance = 10.0f;
    public LayerMask playerLayer;

    [Header("Hands Style Settings")]
    public float horizontalForceHands = 10f;    
    public float verticalForceHands = 5f;     
    public float handsSpeedModifier = 0.7f;     
    public float handsSpeedModifierDuration = 1f; 

    [Header("Legs Style Settings")]
    public float horizontalForceLegs = 3f;      
    public float verticalForceLegs = 10f;       
}

using UnityEngine;

[CreateAssetMenu(fileName = "Metrics", menuName = "Data/Metrics")]
public class MetricsMaster: ScriptableObject
{
    
    [Header("Стили")]
    public PlayerStyle[] Styles;
    [Header("Dash")]
    public float dashPower = 35f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [Header("Slam")]
    public float slamPower = 10f;
    public float slamCooldown = 1;
    
    [Header("Настройки отталкивания")]
    [Header("Стиль ног")]
    public float horizontalForceLegs = 14f;
    public float verticalForceLegs = 10f;
    [Header("Cтиль рук")]
    public float reboundForceHands = 20f;
    public float extraAccelerationHands = 5f;
    
    
}
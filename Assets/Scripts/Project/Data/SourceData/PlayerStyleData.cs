using UnityEngine;

// Данные, которые меняются в зависимости от стиля
[CreateAssetMenu(fileName = "NewPlayerStyle", menuName = "GameData/Player Style Data")]
public class PlayerStyleData : ScriptableObject
{
    [Header("Параметры Движения")]
    public float walkSpeed = 5f;
    public float jumpPower = 5f;

    [Header("Параметры Способностей")]
    public float dashPower = 15f;
    public bool canBreakWallsWithDash = false;
    
    public float slamPower = 15f;
    public bool canBreakWallsWithSlam = false;
    
    public float wallReboundForce = 15f;
    public bool requireVerticalOnWallJump = true;
}
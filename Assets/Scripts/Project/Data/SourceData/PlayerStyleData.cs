using UnityEngine;

// Данные, которые меняются в зависимости от стиля
[CreateAssetMenu(fileName = "NewPlayerStyle", menuName = "GameData/Player Style Data")]
public class PlayerStyleData : ScriptableObject
{
    [Header("Параметры Движения")]
    [Tooltip("Скорость ходьбы, применимая, когда игрок находится на земле")]
    public float walkSpeed = 5f;

    [Tooltip("Сила прыжка")]
    public float jumpPower = 5f;

    [Tooltip("Дополнительный множитель скорости во время рывка")]
    public float dashPower = 15f;
    
    [Header("Способности")]
    [Tooltip("Может ли этот стиль разбивать стены с помощью рывка")]
    public bool canBreakWallsWithDash = false;
    [Tooltip("Сила отталкивания при взаимодействии со стеной")]
    public float wallReboundForce = 15f;
    [Tooltip("Требуется ли вертикальное движение при отталкивании от стены (например, только прыжок)")]
    public bool requireVerticalOnWallJump = true;
}
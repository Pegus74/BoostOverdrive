using UnityEngine;

/// <summary>
/// отвечает только за физическое движение игрока
/// </summary>
public class PlayerMovementController : MonoBehaviour, IRestartable
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;
    public ObstaclesSettingsData obstaclesSettings;
    
    [Header("Input Listeners")] 
    public Vector2Event MoveInputEvent;
    public GameEvent JumpAttemptEvent;
    public WallJumpEvent OnWallJumpDetectedEvent;
    
    [Header("Soft Reset Event")]
    [SerializeField] private GameEvent OnLevelResetEvent;
    
    [HideInInspector] private Rigidbody rb;
    
    private Vector2 currentMoveInput = Vector2.zero; // Текущий ввод для FixedUpdate
    
    private Vector3 externalImpulse = Vector3.zero;
    private const int LEGS_STYLE_INDEX = 0;
    private const int HANDS_STYLE_INDEX = 1;
    // private Coroutine lingerCoroutine;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
            return;
        }
        
        rb.freezeRotation = true;
        
        playerStateModel.SetLastWallJumpedFrom(null);
    }

    void OnEnable()
    {
        MoveInputEvent?.RegisterListener(OnMoveInput);
        JumpAttemptEvent?.RegisterListener(InitiateJumpLogic);
        OnLevelResetEvent.RegisterListener(SoftReset);
        OnWallJumpDetectedEvent?.RegisterListener(HandleWallJump);
    }

    void OnDisable()
    {
        MoveInputEvent?.UnregisterListener(OnMoveInput);
        JumpAttemptEvent?.UnregisterListener(InitiateJumpLogic);
        OnLevelResetEvent.UnregisterListener(SoftReset);
        OnWallJumpDetectedEvent?.UnregisterListener(HandleWallJump);
    }
    
    /// <summary>
    /// Сохраняет ввод движения для FixedUpdate
    /// </summary>
    public void OnMoveInput(Vector2 input)
    {
        currentMoveInput = input;
    }

    /// <summary>
    /// Вызывается при попытке прыжка (через JumpAttemptEvent)
    /// </summary>
    public void InitiateJumpLogic()
    {
        if (playerStateModel.IsGrounded && !playerStateModel.IsSliding && !playerStateModel.IsSlamming)
        {
            Jump();
            Debug.Log("Jump Attempted!");
        }
    }

    // --- ФИЗИЧЕСКАЯ ЛОГИКА ---

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (playerSettingsData.playerCanMove && 
            !playerStateModel.IsDashing &&
            !playerStateModel.IsSliding &&
            !playerStateModel.IsSlamming)
        {
            ApplyMovementForce(currentMoveInput);
        }
    }

    /// <summary>
    /// Применяет силу движения, основываясь на Vector2 ввода.
    /// </summary>
    private void ApplyMovementForce(Vector2 input)
    {
        Vector3 targetDirection = new Vector3(input.x, 0, input.y);

        if (targetDirection.sqrMagnitude > 1f) targetDirection.Normalize();

        float playerSpeed = playerStateModel.CurrentWalkSpeed * playerStateModel.MovementSpeedModifier;

        // Преобразование Vector2 в Vector3 относительно направления игрока
        Vector3 targetVelocity = transform.TransformDirection(targetDirection) * playerSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity); 
        
        velocityChange.x = Mathf.Clamp(velocityChange.x, -playerSettingsData.maxVelocityChange,
            playerSettingsData.maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -playerSettingsData.maxVelocityChange,
            playerSettingsData.maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    
    private void Jump()
    {
        if (!playerSettingsData.enableJump) return;
        
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * playerStateModel.CurrentJumpPower, ForceMode.Impulse);
    }

    /// <summary>
    /// Вызывается при обнаружении прыжка от стены (принимает WallJumpData).
    /// </summary>
    public void HandleWallJump(WallJumpData data)
    {
        Vector3 normal = data.surfaceNormal;
        Component wall = data.wallComponent;
        
        if (playerStateModel.LastWallJumpedFrom == wall) 
            return;

        playerStateModel.SetLastWallJumpedFrom(wall);

        int currentStyleIndex = playerStateModel.CurrentStyleIndex;
        Vector3 approachVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        bool specialVerticalCaseTriggered = false;
        
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        #region HANDS (Стиль Рук)
        if (currentStyleIndex == HANDS_STYLE_INDEX)
        {
            Vector3 V_approach_norm = approachVector.normalized;

            float angle = Vector3.Angle(V_approach_norm, -normal);
            
            // 90-градусов
            bool isSpecialCase = (angle <= 15f || angle >= 165f || (angle >= 75f && angle <= 105f));
            
            Vector3 reboundVector = Vector3.zero;

            if (isSpecialCase)
            {
                reboundVector = normal;
                reboundVector.y = 0; 
                reboundVector.Normalize();
                specialVerticalCaseTriggered = true; 
            }
            else
            {
                reboundVector = Vector3.Reflect(V_approach_norm, normal);
                reboundVector.y = 0;
                reboundVector.Normalize();
            }
            
            Vector3 impulse = reboundVector * obstaclesSettings.reboundForceHands;
            impulse += reboundVector * obstaclesSettings.extraAccelerationHands;
            
            rb.AddForce(impulse, ForceMode.VelocityChange); 
            
        }
        #endregion

        #region LEGS (Стиль Ног)
        else if (currentStyleIndex == LEGS_STYLE_INDEX)
        {
            Vector3 jumpDirection = transform.forward;
            jumpDirection.y = 0;
            jumpDirection.Normalize();
            
            Vector3 finalImpulse = jumpDirection * obstaclesSettings.horizontalForceLegs + Vector3.up * obstaclesSettings.verticalForceLegs;
            
            rb.AddForce(finalImpulse, ForceMode.Impulse);
        }
        #endregion
        
        if (specialVerticalCaseTriggered && currentStyleIndex == HANDS_STYLE_INDEX)
        {
            rb.AddForce(Vector3.up * playerStateModel.CurrentJumpPower * 0.5f, ForceMode.Impulse);
        }
    }
    
    public void SoftReset()
    {
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.isKinematic = false;
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// отвечает только за физическое движение игрока
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;
    public ObstaclesSettingsData obstaclesSettings;
    
    [Header("Input Listeners")] 
    private PlayerInputController playerInputController;


    [Header("Wall Jump")]
    public WallJumpEvent OnWallJumpDetectedEvent;

    [Header("Wall Jump Settings")]
    public SpringWallSettings wallJumpSettings;

    // public WallJumpEvent OnWallJumpDetectedEvent;

    private Rigidbody rb;
    private Vector2 currentMoveInput = Vector2.zero;
    private Vector3 externalImpulse = Vector3.zero;
    private Vector3 smoothedTarget = Vector3.zero;
    private Vector3 smoothedLocalVelocity = Vector3.zero;
    private const int LEGS_STYLE_INDEX = 1;
    private const int HANDS_STYLE_INDEX = 0;

    private bool isWalking = false;
    public bool IsWalking => isWalking;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
            return;
        }
        
        playerInputController = GetComponent<PlayerInputController>();
        
        rb.freezeRotation = true;
        
        playerStateModel.SetLastWallJumpedFrom(null);
    }

    void OnEnable()
    {
        playerInputController.InputEvents.MoveInputEvent.AddListener(OnMoveInput);
        playerInputController.InputEvents.JumpAttemptEvent.AddListener(InitiateJumpLogic);
        // OnWallJumpDetectedEvent?.RegisterListener(HandleWallJump);
        playerInputController.InputEvents.JumpAttemptEvent.AddListener(CheckForWallJumps);
        OnWallJumpDetectedEvent?.AddListener(HandleWallJump);
    }

    void OnDisable()
    {
        playerInputController.InputEvents.MoveInputEvent.RemoveListener(OnMoveInput);
        playerInputController.InputEvents.JumpAttemptEvent.RemoveListener(InitiateJumpLogic);
        // OnWallJumpDetectedEvent?.UnregisterListener(HandleWallJump);
        playerInputController.InputEvents.JumpAttemptEvent.RemoveListener(CheckForWallJumps);
        OnWallJumpDetectedEvent?.RemoveListener(HandleWallJump);
    }
    
    /// <summary>
    /// Сохраняет ввод движения для FixedUpdate
    /// </summary>
    public void OnMoveInput(Vector2 input)
    {
        currentMoveInput = input;
    }
    
    private void InitiateJumpLogic()
    {
        if (playerStateModel.IsGrounded && !playerStateModel.IsSliding && !playerStateModel.IsSlamming)
        {
            Jump();
            Debug.Log("Jump Attempted!");
        }
    }
    
    private void SetExternalImpulse(Vector3 impulse)
    {
        externalImpulse = impulse;
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
        
        Vector3 inputLocal = new Vector3(input.x, 0, input.y);
        if (inputLocal.sqrMagnitude > 1f)
            inputLocal.Normalize();
        
        Vector3 rawLocalTarget = inputLocal * playerSpeed;
        
        smoothedLocalVelocity = Vector3.MoveTowards(
            smoothedLocalVelocity,
            rawLocalTarget,
            playerSettingsData.acceleration * Time.fixedDeltaTime
        );
        
        Vector3 targetVelocity = transform.TransformDirection(smoothedLocalVelocity) + externalImpulse;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity); 
        
        velocityChange.x = Mathf.Clamp(velocityChange.x, -playerSettingsData.maxVelocityChange,
            playerSettingsData.maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -playerSettingsData.maxVelocityChange,
            playerSettingsData.maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        bool hasInput = smoothedLocalVelocity.sqrMagnitude > 0.01f;
        bool hasHorizontalSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).sqrMagnitude > 0.01f;
        isWalking = playerStateModel.IsGrounded && hasInput && hasHorizontalSpeed;
        
        externalImpulse = Vector3.Lerp(externalImpulse, Vector3.zero, 5f * Time.fixedDeltaTime);
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
    /// 
    private void HandleWallJump(WallJumpData data)
    {
        if (playerStateModel.LastWallJumpedFrom == data.wallComponent)
            return;

        playerStateModel.SetLastWallJumpedFrom(data.wallComponent);

        Vector3 normal = data.surfaceNormal;
        SpringWall wall = data.wallComponent as SpringWall;

        Vector3 approachVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        bool specialVerticalCaseTriggered = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        #region HANDS (Стиль Рук)
        if (data.styleIndex == HANDS_STYLE_INDEX)
        {
            Vector3 V_approach_norm = approachVector.normalized;

            float angle = Vector3.Angle(V_approach_norm, -normal);

            bool isSpecialCase = (angle <= 15f || angle >= 165f || (angle >= 75f && angle <= 105f));

            Vector3 reboundVector = Vector3.zero;
            float reboundForce = wall.settings.reboundForceHands;

            if (isSpecialCase)
            {
                reboundVector = normal + Vector3.up;
                reboundVector.Normalize();
                specialVerticalCaseTriggered = true;
            }
            else
            {
                reboundVector = Vector3.Reflect(V_approach_norm, normal);
                reboundVector.y = 0;
                reboundVector.Normalize();
            }

            Vector3 impulse = reboundVector * reboundForce;
            impulse += reboundVector * wall.settings.extraAccelerationHands;

            SetExternalImpulse(impulse);

            if (wall != null)
            {
                StartCoroutine(wall.ApplySpeedModifierCoroutine(playerStateModel));
            }
        }
        #endregion

        #region LEGS (Стиль Ног)
        else if (data.styleIndex == LEGS_STYLE_INDEX)
        {
            Vector3 jumpDirection = transform.forward;
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            Vector3 finalImpulse = jumpDirection * wall.settings.horizontalForceLegs +
                                 Vector3.up * wall.settings.verticalForceLegs;

            rb.AddForce(finalImpulse, ForceMode.Impulse);
            SetExternalImpulse(Vector3.zero);
        }
        #endregion

        if (specialVerticalCaseTriggered && data.styleIndex == HANDS_STYLE_INDEX)
        {
            rb.AddForce(Vector3.up * playerStateModel.CurrentJumpPower * 0.75f, ForceMode.Impulse);
        }
    }
}
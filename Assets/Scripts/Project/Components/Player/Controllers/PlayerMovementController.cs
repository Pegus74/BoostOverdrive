using UnityEngine;

/// <summary>
/// отвечает только за физическое движение игрока
/// </summary>
public class PlayerMovementController : MonoBehaviour, IRestartable
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listeners")] 
    public Vector2Event MoveInputEvent;
    public GameEvent JumpAttemptEvent;
    
    [Header("Soft Reset Event")]
    [SerializeField] private GameEvent OnLevelResetEvent;
    
    [HideInInspector] public Rigidbody rb;

    // Внутреннее состояние контроллера движения
    private Vector2 currentMoveInput = Vector2.zero; // Текущий ввод для FixedUpdate
    private bool canAirJump;

    // // old FPC
    // private Vector3 externalImpulse;
    // private Coroutine lingerCoroutine;
    // private Component LastWallJumpedFrom = null;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
            return;
        }
        
        rb.freezeRotation = true;
    }

    void OnEnable()
    {
        // Подписка на события ввода
        MoveInputEvent?.RegisterListener(OnMoveInput);
        JumpAttemptEvent?.RegisterListener(InitiateJumpLogic);
        OnLevelResetEvent.RegisterListener(SoftReset);
    }

    void OnDisable()
    {
        // Отписка от событий
        MoveInputEvent?.UnregisterListener(OnMoveInput);
        JumpAttemptEvent?.UnregisterListener(InitiateJumpLogic);
        OnLevelResetEvent.UnregisterListener(SoftReset);
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
        else if (playerSettingsData.enableAirJump && canAirJump)
        {
            Jump();
            canAirJump = false;
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

    public void SoftReset()
    {
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.isKinematic = false;
        }
    }
}
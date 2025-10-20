using UnityEngine;

/// <summary>
/// отвечает только за ФИЗИЧЕСКОЕ ДВИЖЕНИЯ игрока
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    [Header("Model & Settings")] [Tooltip("Рабочие параметры состояния игрока")]
    public PlayerStateModel playerStateModel;

    [Tooltip("Неизменяемые настройки игрока")]
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listeners (Events)")] public Vector2Event MoveInputEvent;
    public GameEvent JumpAttemptEvent;
    public GameEvent DashAttemptEvent;
    public GameEvent SlamAttemptEvent;
    // public VoidEventSO ToggleStyleAttemptEvent; 
    
    [HideInInspector] public Rigidbody rb;
    private float speedModifier = 1f; // Для Dash/Slide

    // Внутреннее состояние контроллера движения
    private Vector2 currentMoveInput = Vector2.zero; // Текущий ввод для FixedUpdate
    private bool isDashing = false;
    private bool isSlamming = false;
    private bool canAirJump = true;

    // old FPC
    private Vector3 externalImpulse;
    private Coroutine lingerCoroutine;
    private Component LastWallJumpedFrom = null;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on PlayerMovementController!");
            enabled = false;
            return;
        }

        // Замораживаем вращение, чтобы не мешал RigidBody
        rb.freezeRotation = true;
    }

    void OnEnable()
    {
        // Подписка на события ввода
        MoveInputEvent?.RegisterListener(OnMoveInput);
        JumpAttemptEvent?.RegisterListener(InitiateJumpLogic);
        DashAttemptEvent?.RegisterListener(InitiateDashLogic);
        SlamAttemptEvent?.RegisterListener(InitiateSlamLogic);

        // Подписка на изменение состояния, если нужно для логики движения
        playerStateModel?.OnGroundedStateChangedEvent.RegisterListener(OnGroundedStateChanged);
    }

    void OnDisable()
    {
        // Отписка от событий
        MoveInputEvent?.UnregisterListener(OnMoveInput);
        JumpAttemptEvent?.UnregisterListener(InitiateJumpLogic);
        DashAttemptEvent?.UnregisterListener(InitiateDashLogic);
        SlamAttemptEvent?.UnregisterListener(InitiateSlamLogic);

        playerStateModel?.OnGroundedStateChangedEvent.UnregisterListener(OnGroundedStateChanged);
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
        if (playerStateModel.IsGrounded)
        {
            Jump();
        }
        else if (playerSettingsData.enableAirJump && canAirJump)
        {
            Jump();
            canAirJump = false;
        }
    }

    /// <summary>
    /// Вызывается при попытке дэша (через DashAttemptEvent)
    /// </summary>
    public void InitiateDashLogic()
    {
        if (playerSettingsData.enableDash && !isDashing)
        {
            // Здесь должна быть проверка кулдауна (если он еще не в отдельной системе)
            // StartCoroutine(DashCoroutine()); 
            Debug.Log("Dash Attempted! (TODO: refactoring to DashSystem)");
        }
    }

    /// <summary>
    /// Вызывается при попытке слэма (через SlamAttemptEvent)
    /// </summary>
    public void InitiateSlamLogic()
    {
        if (playerSettingsData.enableSlam && !isSlamming && !playerStateModel.IsGrounded)
        {
            // Здесь должна быть проверка кулдауна
            // StartCoroutine(SlamCoroutine());
            Debug.Log("Slam Attempted! (TODO: refactoring to SlamSystem)");
        }
    }

    // --- ФИЗИЧЕСКАЯ ЛОГИКА ---

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (playerSettingsData.playerCanMove && !isDashing && !isSlamming)
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

        // Преобразование Vector2 в Vector3 относительно направления игрока
        Vector3 targetVelocity = transform.TransformDirection(targetDirection) * playerStateModel.CurrentWalkSpeed *
                                 speedModifier;

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
    /// Обновление внутреннего состояния, когда модель сообщает о смене IsGrounded.
    /// </summary>
    private void OnGroundedStateChanged(bool isGrounded)
    {
        if (isGrounded)
        {
            canAirJump = true;
            // Логика сброса состояния (например, slam/dash)
            isSlamming = false;

            // Если была внешняя импульсная сила, прекращаем ее действие
            if (lingerCoroutine != null) StopCoroutine(lingerCoroutine);

            // Сброс externalImpulse (если она не обнуляется в FixedUpdate)
            externalImpulse = Vector3.zero;
        }
    }
}
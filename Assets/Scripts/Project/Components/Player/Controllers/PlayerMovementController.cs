using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
    public PlayerStateModel playerStateModel;
    public WallJumpEvent OnWallJumpDetectedEvent;

    private Rigidbody rb;
    private Vector2 currentMoveInput = Vector2.zero;
    private Vector3 externalImpulse = Vector3.zero;
    private Vector3 smoothedLocalVelocity = Vector3.zero;

    private const int LEGS_STYLE_INDEX = 1;
    private const int HANDS_STYLE_INDEX = 0;

    private bool isWalking = false;
    public bool IsWalking => isWalking;

    [SerializeField] private AudioClip jumpSound;

    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) enabled = false;
        else rb.freezeRotation = true;
	audioSource = GetComponent<AudioSource>();
        playerStateModel.SetLastWallJumpedFrom(null);
    }

    void OnEnable()
    {
        InputEvents.MoveInputEvent.AddListener(OnMoveInput);
        InputEvents.JumpAttemptEvent.AddListener(InitiateJumpLogic);
        InputEvents.JumpAttemptEvent.AddListener(CheckForWallJumps);
        OnWallJumpDetectedEvent?.AddListener(HandleWallJump);
    }

    void OnDisable()
    {
        InputEvents.MoveInputEvent.RemoveListener(OnMoveInput);
        InputEvents.JumpAttemptEvent.RemoveListener(InitiateJumpLogic);
        InputEvents.JumpAttemptEvent.RemoveListener(CheckForWallJumps);
        OnWallJumpDetectedEvent?.RemoveListener(HandleWallJump);
    }

    public void OnMoveInput(Vector2 input) => currentMoveInput = input;

    private void InitiateJumpLogic() => playerStateModel.BufferJump();

    private void SetExternalImpulse(Vector3 impulse) => externalImpulse = impulse;

    private void Update()
    {
        TryJump();  
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        playerStateModel.UpdateCoyoteTime(Time.fixedDeltaTime);
        playerStateModel.UpdateJumpBuffer(Time.fixedDeltaTime);
        
        if (playerStateModel.IsGrounded && rb.linearVelocity.y <= 0f)
        {
            playerStateModel.ClearLastWallJumpedFrom();
        }

        if (playerStateModel.playerCanMove && !playerStateModel.IsDashing && !playerStateModel.IsSliding && !playerStateModel.IsSlamming)
            ApplyMovementForce(currentMoveInput);

        if (playerStateModel.IsGrounded && rb.linearVelocity.y <= 0f && !playerStateModel.IsDashing)
        {
            ProjectVelocityToGround();
            ApplyGroundStickForce();
        }
        if (playerStateModel.IsGrounded)
        {
            Debug.Log($"Velocity Y: {rb.linearVelocity.y:F3} | Horizontal Speed: {new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude:F2} | Slope Angle: {Vector3.Angle(playerStateModel.GroundNormal, Vector3.up):F1}");
        }
    }

    private void TryJump()
    {
        bool canJump = playerStateModel.IsGrounded || playerStateModel.CoyoteCounter > 0f;
        bool wantsJump = playerStateModel.JumpBufferCounter > 0f;

        if (canJump && wantsJump && playerStateModel.settings.enableJump)
        {
            Jump(); 
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);  
        rb.AddForce(Vector3.up * playerStateModel.CurrentJumpPower, ForceMode.Impulse);
        playerStateModel.ResetJumpBuffer();
        playerStateModel.ResetCoyoteTime();
    }

    private void ApplyMovementForce(Vector2 input)
    {
        Vector3 inputLocal = new Vector3(input.x, 0, input.y);
        if (inputLocal.sqrMagnitude > 1f) inputLocal.Normalize();

        Vector3 rawLocalTarget = inputLocal * playerStateModel.CurrentWalkSpeed;
        smoothedLocalVelocity = Vector3.MoveTowards(smoothedLocalVelocity, rawLocalTarget,
            playerStateModel.settings.acceleration * Time.fixedDeltaTime);

        Vector3 targetVelocity = transform.TransformDirection(smoothedLocalVelocity) + externalImpulse;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -playerStateModel.settings.maxVelocityChange,
            playerStateModel.settings.maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -playerStateModel.settings.maxVelocityChange,
            playerStateModel.settings.maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        bool hasInput = smoothedLocalVelocity.sqrMagnitude > 0.01f;
        bool hasHorizontalSpeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).sqrMagnitude > 0.01f;
        isWalking = playerStateModel.IsGrounded && hasInput && hasHorizontalSpeed;

        externalImpulse = Vector3.Lerp(externalImpulse, Vector3.zero, 5f * Time.fixedDeltaTime);
    }

    private void ProjectVelocityToGround()
    {
        Vector3 normal = playerStateModel.GroundNormal;
        if (normal == Vector3.zero) normal = Vector3.up;

        rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, normal);

        if (playerStateModel.IsGrounded && Mathf.Abs(rb.linearVelocity.y) < 1f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
    }

    private void ApplyGroundStickForce()
    {
        Vector3 normal = playerStateModel.GroundNormal;
        if (normal == Vector3.zero) normal = Vector3.up;

        Vector3 parallelGravity = Vector3.ProjectOnPlane(Physics.gravity, normal);
        rb.AddForce(-parallelGravity * 1.05f, ForceMode.Acceleration);

        float slopeAngle = Vector3.Angle(normal, Vector3.up);
        if (slopeAngle <= 10f)
        {
            rb.AddForce(-Vector3.up * playerStateModel.settings.flatGroundStickForce, ForceMode.Acceleration);
        }
    }

    private void CheckForWallJumps()
    {
        if (playerStateModel.IsGrounded) return;
        foreach (SpringWall wall in FindObjectsOfType<SpringWall>())
        {
            if (wall.TryDetectWallJump(out WallJumpData data))
            {
                OnWallJumpDetectedEvent?.Invoke(data);
                return;
            }
        }
    }

    public void HandleWallJump(WallJumpData data)
    {
        SpringWall wall = data.wallComponent as SpringWall;

        if (playerStateModel.LastWallJumpedFrom == wall || playerStateModel.IsGrounded)
            return;

        float horizontalForce;
        float verticalForce;

	audioSource.PlayOneShot(jumpSound);

        if (data.styleIndex == HANDS_STYLE_INDEX)
        {
            horizontalForce = wall.settings.horizontalForceHands;
            verticalForce = wall.settings.verticalForceHands;

            if (wall != null)
                StartCoroutine(wall.ApplySpeedModifierCoroutine(playerStateModel));
        }
        else
        {
            horizontalForce = wall.settings.horizontalForceLegs;
            verticalForce = wall.settings.verticalForceLegs;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 desiredHorizontal = forward * horizontalForce;

        Vector3 newVelocity = new Vector3(desiredHorizontal.x, verticalForce, desiredHorizontal.z);

        Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float currentHorizontalSpeed = currentHorizontal.magnitude;

        rb.linearVelocity = newVelocity;

        externalImpulse = Vector3.zero;

        playerStateModel.SetLastWallJumpedFrom(wall);
    }
}

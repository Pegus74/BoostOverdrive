using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [Header("Input Events")]
    [Tooltip("Move Input (Vector2)")]
    public Vector2Event MoveInputEvent;
    [Tooltip("Look Input (Vector2)")]
    public Vector2Event LookInputEvent;
    [Tooltip("Jump Attempt (Void)")]
    public GameEvent JumpAttemptEvent;
    [Tooltip("Dash Attempt (Void)")]
    public GameEvent DashAttemptEvent;
    [Tooltip("Slam Attempt (Void)")]
    public GameEvent SlamAttemptEvent;
    [Tooltip("Toggle Style Attempt (Void)")]
    public GameEvent ToggleStyleAttemptEvent;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        
        playerControls.Gameplay.Move.performed += OnMove;
        playerControls.Gameplay.Move.canceled += OnMove;

        playerControls.Gameplay.Look.performed += OnLook;
        playerControls.Gameplay.Look.canceled += OnLook;

        playerControls.Gameplay.Jump.performed += OnJump;
        playerControls.Gameplay.Dash.performed += OnDash;
        playerControls.Gameplay.Slam.performed += OnSlam;
        playerControls.Gameplay.ToggleStyle.performed += OnToggleStyle;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnDestroy()
    {
        playerControls.Gameplay.Move.performed -= OnMove;
        playerControls.Gameplay.Move.canceled -= OnMove;
        
        playerControls.Gameplay.Look.performed -= OnLook;
        playerControls.Gameplay.Look.canceled -= OnLook;
        
        playerControls.Gameplay.Jump.performed -= OnJump;
        playerControls.Gameplay.Dash.performed -= OnDash;
        playerControls.Gameplay.Slam.performed -= OnSlam;
        playerControls.Gameplay.ToggleStyle.performed -= OnToggleStyle;
        
        playerControls.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        MoveInputEvent?.Raise(value); // OnMove(value) -> MoveInputEvent.Raise(Vector2 value)
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        LookInputEvent?.Raise(value);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpAttemptEvent?.Raise(); // OnJump(performed) -> JumpAttemptEvent.Raise()
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        DashAttemptEvent?.Raise(); // OnDash(performed) -> DashAttemptEvent.Raise()
    }

    private void OnSlam(InputAction.CallbackContext context)
    {
        SlamAttemptEvent?.Raise();
    }

    private void OnToggleStyle(InputAction.CallbackContext context)
    {
        ToggleStyleAttemptEvent?.Raise();
    }
}
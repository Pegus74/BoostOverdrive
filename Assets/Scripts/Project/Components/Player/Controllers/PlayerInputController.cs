using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [Header("Input Events")]
    public Vector2Event moveInputEvent;
    public Vector2Event LookInputEvent;
    public GameEvent JumpAttemptEvent;
    public GameEvent DashAttemptEvent;
    public GameEvent SlamAttemptEvent;
    public GameEvent SlideAttemptEvent;
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
        playerControls.Gameplay.Slide.performed += OnSlide;
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
        playerControls.Gameplay.Slide.performed -= OnSlide;
        playerControls.Gameplay.Slam.performed -= OnSlam;
        playerControls.Gameplay.ToggleStyle.performed -= OnToggleStyle;
        
        playerControls.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        moveInputEvent?.Raise(value);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        LookInputEvent?.Raise(value);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpAttemptEvent?.Raise();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        DashAttemptEvent?.Raise();
    }

    private void OnSlam(InputAction.CallbackContext context)
    {
        SlamAttemptEvent?.Raise();
    }
    
    private void OnSlide(InputAction.CallbackContext context)
    {
        SlideAttemptEvent?.Raise();
    }

    private void OnToggleStyle(InputAction.CallbackContext context)
    {
        ToggleStyleAttemptEvent?.Raise();
    }
}
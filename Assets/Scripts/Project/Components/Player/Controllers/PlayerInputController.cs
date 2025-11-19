using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [Header("Input Events")]
    public Vector2Event MoveInputEvent = new Vector2Event();
    public Vector2Event LookInputEvent = new Vector2Event();
    [HideInInspector]
    public UnityEvent JumpAttemptEvent = new UnityEvent(); // через атрибут убрать 
    public UnityEvent DashAttemptEvent = new UnityEvent();
    public UnityEvent SlamAttemptEvent = new UnityEvent();
    public UnityEvent SlideAttemptEvent = new UnityEvent();
    public UnityEvent ToggleStyleAttemptEvent = new UnityEvent();
    public UnityEvent OnPauseAttemptEvent =  new UnityEvent();
    
    [Header("GameState")]
    [SerializeField] private GameStateEvent GameStateChangedEvent;

    private PlayerControls playerControls; // Pattern - Provider

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
        
        playerControls.Gameplay.Pause.performed += OnPause;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        
        if (GameStateChangedEvent != null)
            GameStateChangedEvent.AddListener(HandleGameStateChange);
    }

    private void OnDisable()
    {
        playerControls.Disable();
        
        if (GameStateChangedEvent != null)
            GameStateChangedEvent.RemoveListener(HandleGameStateChange);
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
        
        playerControls.Gameplay.Pause.performed -= OnPause;
        
        playerControls.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        MoveInputEvent.Invoke(value);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        LookInputEvent.Invoke(value);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpAttemptEvent.Invoke();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        DashAttemptEvent.Invoke();
    }

    private void OnSlam(InputAction.CallbackContext context)
    {
        SlamAttemptEvent.Invoke();
    }
    
    private void OnSlide(InputAction.CallbackContext context)
    {
        SlideAttemptEvent.Invoke();
    }

    private void OnToggleStyle(InputAction.CallbackContext context)
    {
        ToggleStyleAttemptEvent.Invoke();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        OnPauseAttemptEvent.Invoke();
    }
    
    private void HandleGameStateChange(GameState newState)
    {
        // Отключаем весь ввод кроме кнопки паузы, если игра не в состоянии Playing
        bool isPlaying = (newState == GameState.Playing);
        if (isPlaying)
        {
            playerControls.Gameplay.Enable();
        }
        else
        {
            playerControls.Gameplay.Disable();
            playerControls.Gameplay.Pause.Enable();
        }
    }
}
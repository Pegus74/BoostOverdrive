using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [HideInInspector]
    public InputEvents InputEvents = new InputEvents();
    
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
        
        playerControls.Gameplay.Restart.performed += OnRestart;
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
        
        playerControls.Gameplay.Restart.performed -= OnRestart;
        
        playerControls.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        InputEvents.MoveInputEvent.Invoke(value);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        InputEvents.LookInputEvent.Invoke(value);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        InputEvents.JumpAttemptEvent.Invoke();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        InputEvents.DashAttemptEvent.Invoke();
    }

    private void OnSlam(InputAction.CallbackContext context)
    {
        InputEvents.SlamAttemptEvent.Invoke();
    }
    
    private void OnSlide(InputAction.CallbackContext context)
    {
        InputEvents.SlideAttemptEvent.Invoke();
    }

    private void OnToggleStyle(InputAction.CallbackContext context)
    {
        InputEvents.ToggleStyleAttemptEvent.Invoke();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        InputEvents.OnPauseAttemptEvent.Invoke();
    }

    private void OnRestart(InputAction.CallbackContext context)
    {
        InputEvents.OnRestartAttemptEvent.Invoke();
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
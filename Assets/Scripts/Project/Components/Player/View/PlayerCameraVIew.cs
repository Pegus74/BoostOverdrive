using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerCameraView : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerMovementController playerMovementController; 
    public PlayerSettingsData playerSettingsData;
    
    [Header("Camera Components")]
    public Camera playerCamera;

    public Transform joint;
    private PlayerInputController playerInputController;
    
    // Углы вращения
    private float yaw = 0f;
    private float pitch = 0f;
    
    // Ввод для обработки в Update()
    private Vector2 currentLookInput = Vector2.zero;
    private Image crosshairObject;
    private Coroutine currentCameraRotationCoroutine;

    private float timer = 0f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
    private Vector3 jointOriginalPos;
    

    private void Awake()
    {
        playerInputController = FindObjectOfType<PlayerInputController>();
        if (playerSettingsData.crosshair)
        {
            SetupCrosshair();
        }
        
        if (playerSettingsData.lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        yaw = transform.localEulerAngles.y;
        pitch = playerCamera.transform.localEulerAngles.x;
        
        jointOriginalPos = joint.localPosition;
    }

    private void OnEnable()
    { 
        InputEvents.LookInputEvent.AddListener(OnLookInput);
    }

    private void OnDisable()
    { 
        InputEvents.LookInputEvent.RemoveListener(OnLookInput);
    }

    /// <summary>
    /// Получает Vector2 ввода для обзора от PlayerInputController.
    /// </summary>
    public void OnLookInput(Vector2 input)
    {
        currentLookInput = input;
    }

    private void LateUpdate()
    {
        if (NewGameManager.Instance.GetCurrentState() == GameState.Playing && currentLookInput != Vector2.zero)
        {
            ApplyLookRotation(currentLookInput);
        }
        
        HeadBob();
    }

    /// <summary>
    /// Применяет вращение камеры на основе Vector2 ввода.
    /// </summary>
    private void ApplyLookRotation(Vector2 input)
    {
        float sensitivity = playerSettingsData.mouseSensitivity;
        float maxAngle = playerSettingsData.maxLookAngle;
        
        yaw += input.x * sensitivity; 
        pitch += (playerSettingsData.invertCamera ? 1 : -1) * input.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -maxAngle, maxAngle);
        
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
    
    private void HeadBob()
    {
        if (playerMovementController.IsWalking)
        {
            timer += Time.deltaTime * playerSettingsData.bobSpeed;
            joint.localPosition = jointOriginalPos + new Vector3(
                Mathf.Sin(timer) * bobAmount.x,
                Mathf.Sin(timer) * bobAmount.y,
                Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * playerSettingsData.bobSpeed);
        }
    }
    
    /// <summary>
    /// Настройка crosshair
    /// </summary>
    private void SetupCrosshair()
    {
        if (!playerSettingsData.crosshairImage) return;

        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(transform.root.GetComponentInChildren<Canvas>().transform, false);
        crosshairObject = crosshairGO.AddComponent<Image>();
        crosshairObject.sprite = playerSettingsData.crosshairImage;
        crosshairObject.color = playerSettingsData.crosshairColor;
        
        crosshairObject.rectTransform.sizeDelta = new Vector2(2, 4); 
    }
}
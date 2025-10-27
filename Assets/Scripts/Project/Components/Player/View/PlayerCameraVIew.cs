using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraView : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel; 
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listeners (Events)")]
    public Vector2Event LookInputEvent; 
    
    [Header("Camera Components")]
    public Camera playerCamera;
    
    [Header("View Settings")]
    public bool invertCamera = false;
    public bool lockCursor = true;
    public bool cameraCanMove = true;
    
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;
    
    // Углы вращения
    private float yaw = 0f;
    private float pitch = 0f;
    
    // Ввод для обработки в Update()
    private Vector2 currentLookInput = Vector2.zero;
    private Image crosshairObject;
    private Coroutine currentCameraRotationCoroutine; 
    

    private void Awake()
    {
        if (crosshair)
        {
            SetupCrosshair();
        }
        
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        yaw = transform.localEulerAngles.y;
        pitch = playerCamera.transform.localEulerAngles.x;
        
        if (playerCamera != null && playerStateModel != null)
        {
            
        }
    }

    private void OnEnable()
    {
        LookInputEvent?.RegisterListener(OnLookInput);
    }

    private void OnDisable()
    {
        LookInputEvent?.UnregisterListener(OnLookInput);
    }

    /// <summary>
    /// Получает Vector2 ввода для обзора от PlayerInputController.
    /// </summary>
    public void OnLookInput(Vector2 input)
    {
        currentLookInput = input;
    }

    private void Update()
    {
        // Применяем вращение только если камера может двигаться
        if (cameraCanMove && currentLookInput != Vector2.zero)
        {
            ApplyLookRotation(currentLookInput);
        }
    }

    /// <summary>
    /// Применяет вращение камеры на основе Vector2 ввода.
    /// </summary>
    private void ApplyLookRotation(Vector2 input)
    {
        float sensitivity = playerSettingsData.mouseSensitivity;
        float maxAngle = playerSettingsData.maxLookAngle;
        
        yaw += input.x * sensitivity; 
        pitch += (invertCamera ? 1 : -1) * input.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -maxAngle, maxAngle);
        
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
    
    /// <summary>
    /// Настройка crosshair
    /// </summary>
    private void SetupCrosshair()
    {
        if (!crosshairImage) return;

        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(transform.root.GetComponentInChildren<Canvas>().transform, false);
        crosshairObject = crosshairGO.AddComponent<Image>();
        crosshairObject.sprite = crosshairImage;
        crosshairObject.color = crosshairColor;
        
        crosshairObject.rectTransform.sizeDelta = new Vector2(2, 4); 
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraView : MonoBehaviour
{
    [Header("Model & Settings")]
    [Tooltip("Рабочие параметры состояния игрока")]
    public PlayerStateModel playerStateModel; 
    [Tooltip("Неизменяемые настройки игрока")]
    public PlayerSettingsData playerSettingsData;
    
    [Header("Input Listeners (Events)")]
    [Tooltip("Look Input (Vector2) из PlayerInputController")]
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
        playerStateModel?.OnStyleChangedEvent.RegisterListener(OnStyleChanged);
    }

    private void OnDisable()
    {
        LookInputEvent?.UnregisterListener(OnLookInput);
        playerStateModel?.OnStyleChangedEvent.UnregisterListener(OnStyleChanged);
    }

    /// <summary>
    /// Получает Vector2 ввода для обзора от PlayerInputController.
    /// </summary>
    public void OnLookInput(Vector2 input)
    {
        currentLookInput = input;
    }

    /// <summary>
    /// Вызывается при смене стиля, чтобы обновить FOV или другие визуальные эффекты.
    /// </summary>
    public void OnStyleChanged(int newStyleIndex)
    {
        float targetFov = (newStyleIndex == 0) ? 60f : 80f;
        
        if (playerCamera.fieldOfView != targetFov)
        {
            playerCamera.fieldOfView = targetFov;
        }
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
    
    // public void SmoothlyRotateCameraYaw(float targetYaw, float duration)
    // {
    //     if (currentCameraRotationCoroutine != null) StopCoroutine(currentCameraRotationCoroutine);
    //     currentCameraRotationCoroutine = StartCoroutine(RotateCameraCoroutine(targetYaw, duration));
    // }
    
    // private IEnumerator RotateCameraCoroutine(float targetYaw, float duration)
    // {
    //     float startYaw = yaw;
    //     float timer = 0f;
    //     
    //     while (timer < duration)
    //     {
    //         timer += Time.deltaTime;
    //         float progress = timer / duration;
    //         yaw = Mathf.LerpAngle(startYaw, targetYaw, progress);
    //         
    //         // Применяем сразу
    //         transform.localEulerAngles = new Vector3(0, yaw, 0); 
    //         
    //         yield return null;
    //     }
    //     yaw = targetYaw;
    //     transform.localEulerAngles = new Vector3(0, yaw, 0);
    //     currentCameraRotationCoroutine = null;
    // }
    
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
        
        // Базовая настройка размера (может потребоваться более сложная логика)
        crosshairObject.rectTransform.sizeDelta = new Vector2(32, 32); 
    }
}
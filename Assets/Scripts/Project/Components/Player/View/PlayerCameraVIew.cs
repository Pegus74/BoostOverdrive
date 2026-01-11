using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerCameraView : MonoBehaviour
{
    public PlayerMovementController playerMovementController;
    public PlayerConfig playerSettingsData;
    public Camera playerCamera;
    public Transform joint;
    public Vector3 normalBobAmount = new Vector3(.15f, .05f, 0f);
    public Vector3 fastBobAmount = new Vector3(.20f, .08f, 0f);
    public float bobSpeed = 5f;
    public Image windEffectImage;
    public Sprite[] windSprites;
    public float windFrameRate = 10f;

    private PlayerInputController playerInputController;
    private float yaw = 0f;
    private float pitch = 0f;
    private Vector2 currentLookInput = Vector2.zero;
    private Image crosshairObject;
    private float timer = 0f;
    private Vector3 jointOriginalPos;
    private float currentSpeed = 0f;
    private Rigidbody playerRigidbody;
    private int currentWindFrame = 0;
    private float windFrameTimer = 0f;
    private bool windActive = false;

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

        if (playerMovementController != null)
        {
            playerRigidbody = playerMovementController.GetComponent<Rigidbody>();
        }

        if (windEffectImage != null)
        {
            windEffectImage.gameObject.SetActive(false);
            if (windSprites != null && windSprites.Length > 0)
            {
                windEffectImage.sprite = windSprites[0];
            }
        }
    }

    private void OnEnable()
    {
        InputEvents.LookInputEvent.AddListener(OnLookInput);
    }

    private void OnDisable()
    {
        InputEvents.LookInputEvent.RemoveListener(OnLookInput);
    }

    public void OnLookInput(Vector2 input)
    {
        currentLookInput = input;
    }

    private void Update()
    {
        UpdateCurrentSpeed();
        UpdateWindEffect();
    }

    private void LateUpdate()
    {
        if (NewGameManager.Instance.GetCurrentState() == GameState.Playing && currentLookInput != Vector2.zero)
        {
            ApplyLookRotation(currentLookInput);
        }
        HeadBob();
    }

    private void UpdateCurrentSpeed()
    {
        if (playerRigidbody != null)
        {
            Vector3 horizontalVelocity = playerRigidbody.linearVelocity;
            horizontalVelocity.y = 0;
            currentSpeed = horizontalVelocity.magnitude;
        }
    }

    private void UpdateWindEffect()
    {
        if (windEffectImage == null || windSprites == null || windSprites.Length == 0)
            return;

        bool shouldShowWind = currentSpeed >= 13f;

        if (shouldShowWind != windActive)
        {
            windActive = shouldShowWind;
            windEffectImage.gameObject.SetActive(windActive);
        }

        if (windActive)
        {
            windFrameTimer += Time.deltaTime;
            float frameTime = 1f / windFrameRate;

            if (windFrameTimer >= frameTime)
            {
                windFrameTimer = 0f;
                currentWindFrame = (currentWindFrame + 1) % windSprites.Length;
                windEffectImage.sprite = windSprites[currentWindFrame];
            }

            float windIntensity = Mathf.Clamp01((currentSpeed - 13f) / 10f);
            Color color = windEffectImage.color;
            color.a = 0.3f + windIntensity * 0.4f;
            windEffectImage.color = color;
        }
    }

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
            timer += Time.deltaTime * bobSpeed;

            Vector3 currentBobAmount = normalBobAmount;
            float speedFactor = 1f;

            if (currentSpeed >= 13f)
            {
                currentBobAmount = fastBobAmount;
                speedFactor = 1.5f;
            }
            else if (currentSpeed >= 10f)
            {
                currentBobAmount = fastBobAmount;
                speedFactor = 1.2f;
            }

            joint.localPosition = jointOriginalPos + new Vector3(
                Mathf.Sin(timer) * currentBobAmount.x * speedFactor,
                Mathf.Sin(timer * 2f) * currentBobAmount.y * speedFactor,
                0f);
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed * 2f);
        }
    }

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
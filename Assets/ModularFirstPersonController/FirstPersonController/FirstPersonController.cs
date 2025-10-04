using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    #region Camera Movement Variables
    [Header("Camera Movement Variables")]
    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal
    private float yaw = 0f;
    private float pitch = 0f;
    private Image crosshairObject;
    private Coroutine currentCameraRotationCoroutine;
    #endregion

    #region Movement Variables
    [Header("Movement Variables")]
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private bool isWalking = false;
    private Vector3 externalImpulse;

    #region Jump
    [Header("Jump")]
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public bool enableAirJump = true;

    private bool isGrounded = false;
    private bool canAirJump = true;
    #endregion

    #region Dash
    [Header("Dash")]
    public bool enableDash = true;
    public KeyCode dashKey = KeyCode.F;
    public float dashPower = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public bool useDashCircle = true;
    public bool hideCircleWhenReady = true;
    public Image dashCircleBG;
    public Image dashCircle;
    public float dashCircleWidthPercent = .1f;
    public float dashCircleHeightPercent = .1f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private CanvasGroup dashCircleCG;
    #endregion
    #endregion

    #region Head Bob
    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;
    private float timer = 0;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = fov;
        jointOriginalPos = joint.localPosition;
    }

    private void Start()
    {
        if (lockCursor) Cursor.lockState = CursorLockMode.Locked;

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else crosshairObject.gameObject.SetActive(false);

        #region Dash Circle
        dashCircleCG = dashCircle?.GetComponentInParent<CanvasGroup>();

        if (useDashCircle && dashCircle && dashCircleBG)
        {
            dashCircleBG.gameObject.SetActive(true);
            dashCircle.gameObject.SetActive(true);

            float w = Screen.width * dashCircleWidthPercent;
            float h = Screen.height * dashCircleHeightPercent;
            dashCircleBG.rectTransform.sizeDelta = new Vector3(w, h, 0);
            dashCircle.rectTransform.sizeDelta = new Vector3(w - 2, h - 2, 0);

            dashCircle.type = Image.Type.Filled;
            dashCircle.fillMethod = Image.FillMethod.Radial360;
            dashCircle.fillOrigin = (int)Image.Origin360.Top;

            if (hideCircleWhenReady) dashCircleCG.alpha = 0;
        }
        else
        {
            if (dashCircleBG) dashCircleBG.gameObject.SetActive(false);
            if (dashCircle) dashCircle.gameObject.SetActive(false);
        }
        #endregion
    }

    private void Update()
    {
        if (PauseController.isPaused) return;

        #region Camera Look
        if (cameraCanMove)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += (invertCamera ? 1 : -1) * Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
        #endregion

        #region Jump
        if (enableJump && Input.GetKeyDown(jumpKey))
        {
            if (isGrounded) { Jump(); }
            else if (enableAirJump && canAirJump) { Jump(); canAirJump = false; }
        }
        #endregion

        #region Dash
        if (enableDash)
        {
            if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

            if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0 && !isDashing)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                dashDirection = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
                    ? transform.TransformDirection(new Vector3(h, 0, v)).normalized
                    : transform.forward;

                isDashing = true;
                dashTimer = dashDuration;
                rb.AddForce(dashDirection * dashPower, ForceMode.Impulse);
            }

            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                rb.AddForce(dashDirection * (dashPower * 0.5f) * (dashTimer / dashDuration), ForceMode.Acceleration);
                if (dashTimer <= 0) { isDashing = false; dashCooldownTimer = dashCooldown; }
            }

            if (useDashCircle && dashCircle)
            {
                dashCircle.fillAmount = 1f - (dashCooldownTimer / dashCooldown);
                if (hideCircleWhenReady && dashCircleCG)
                    dashCircleCG.alpha = dashCooldownTimer > 0 ? 1f : 0f;
            }
        }
        #endregion

        CheckGround();
        if (isGrounded) canAirJump = true;

        if (enableHeadBob) HeadBob();
    }

    private void FixedUpdate()
    {
        #region Movement
        if (playerCanMove && !isDashing)
        {
            Vector3 target = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            isWalking = (target.x != 0 || target.z != 0) && isGrounded;

            target = transform.TransformDirection(target) * walkSpeed + externalImpulse;

            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = target - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        #endregion

        externalImpulse = Vector3.Lerp(externalImpulse, Vector3.zero, 5f * Time.fixedDeltaTime);
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.down * (transform.localScale.y * .5f);
        isGrounded = Physics.Raycast(origin, Vector3.down, .75f);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isGrounded = false;
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            timer += Time.deltaTime * bobSpeed;
            joint.localPosition = jointOriginalPos + new Vector3(
                Mathf.Sin(timer) * bobAmount.x,
                Mathf.Sin(timer) * bobAmount.y,
                Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed);
        }
    }

    public void SetExternalImpulse(Vector3 impulse) => externalImpulse = impulse;

    public void SmoothlyRotateCameraYaw(float targetYaw, float duration)
    {
        if (currentCameraRotationCoroutine != null) StopCoroutine(currentCameraRotationCoroutine);
        currentCameraRotationCoroutine = StartCoroutine(RotateCameraCoroutine(targetYaw, duration));
    }

    private IEnumerator RotateCameraCoroutine(float targetYaw, float duration)
    {
        cameraCanMove = false;
        float startYaw = transform.localEulerAngles.y;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float y = Mathf.LerpAngle(startYaw, targetYaw, t / duration);
            transform.localEulerAngles = new Vector3(0, y, 0);
            yaw = y;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, targetYaw, 0);
        yaw = targetYaw;
        cameraCanMove = true;
        currentCameraRotationCoroutine = null;
    }
}
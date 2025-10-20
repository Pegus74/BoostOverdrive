using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    private StyleController styleManager;
    private float speedModifier = 1f;
    private Coroutine lingerCoroutine;

    [HideInInspector]
    public Component LastWallJumpedFrom { get; private set; } = null;

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
    #endregion
    #region Jump
    [Header("Jump")]
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public bool enableAirJump = true;

    public bool isGrounded = false;
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

    private bool dashInCameraDirection = true; 
    private bool allowHorizontalDashOnly = true;
    #endregion

    #region Slam 
    [Header("Slam")]
    [SerializeField] private bool enableSlam = true;
    [SerializeField] private KeyCode slamKey = KeyCode.F;
    [SerializeField] private float slamPower = 15f;
    [SerializeField] private float slamCooldown = 1f;
    [SerializeField] private float reboundMultiplier = 0.5f;
    [SerializeField] private GameObject slamIndicatorPrefab;
    private GameObject slamIndicatorInstance;
    private bool isSlamming = false;
    private float slamCooldownTimer = 0f;
    private bool slamImpactOccurred = false;
    private bool isReboundingFromSlam = false;
    #endregion

    #region SpringWallVariables
    [Header("Настройки отталкивания")]
    [Header("Ноги")]
    public float horizontalForceLegs = 15f;
    public float verticalForceLegs = 12f;

    [Header("Руки")]
    public float reboundForceHands = 20f;
    public float extraAccelerationHands = 5f;
    public float cameraRotationDuration = 0.3f;
    public KeyCode cameraRotateKey = KeyCode.Q;
    #endregion

    #region CrawlSlide
    [Header("Crawl Slide")]
    public bool enableCrawlSlide = true;
    public KeyCode crawlSlideKey = KeyCode.C;
    public float slideDuration = 2f;
    public float initialSpeedMultiplier = 1.2f;
    public float finalSpeedMultiplier = 1f;
    public float squatHeightScale = 0.5f;
    public float squatTransitionDuration = 0.5f;
    public float slideFrictionMultiplier = 0.95f;
    private bool isSliding = false;
    private Vector3 originalScale;
    private Coroutine slideCoroutine;
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
        styleManager = GetComponent<StyleController>();
        crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = fov;
        jointOriginalPos = joint.localPosition;

        originalScale = transform.localScale;
        if (slamIndicatorPrefab != null)
        {
            slamIndicatorInstance = Instantiate(slamIndicatorPrefab);
            slamIndicatorInstance.SetActive(false);  
        }
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
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GetCurrentState() != GameManager.State.Playing) return;

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
                dashDirection = playerCamera.transform.forward;

                if (allowHorizontalDashOnly)
                {
                    dashDirection.y = 0f;
                }

                dashDirection.Normalize();

                isDashing = true;
                dashTimer = dashDuration;
                Vector3 currentVelocity = rb.velocity;
                Vector3 dashVelocity = dashDirection * dashPower;
                dashVelocity.y = currentVelocity.y;
                rb.velocity = dashVelocity;
            }
            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                // Убирано дополнительное ускорение во время дэша чтобы не накапливалась скорость от предыдущих импульсов
                if (dashTimer <= 0)
                {
                    isDashing = false;
                    dashCooldownTimer = dashCooldown;
                }
            }
            if (useDashCircle && dashCircle)
            {
                dashCircle.fillAmount = 1f - (dashCooldownTimer / dashCooldown);
                if (hideCircleWhenReady && dashCircleCG)
                    dashCircleCG.alpha = dashCooldownTimer > 0 ? 1f : 0f;
            }
        }
        #endregion

        #region Slam
        if (enableSlam && !isGrounded && !isSlamming && slamCooldownTimer <= 0)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
            {
                if (slamIndicatorInstance != null)
                {
                    slamIndicatorInstance.transform.position = hit.point + Vector3.up * 0.01f;
                    slamIndicatorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    slamIndicatorInstance.SetActive(true);
                }
            }
        }
        else
        {
            if (slamIndicatorInstance != null)
            {
                slamIndicatorInstance.SetActive(false);
            }
        }

        if (enableSlam)
        {
            if (slamCooldownTimer > 0) slamCooldownTimer -= Time.deltaTime;
            if (Input.GetKeyDown(slamKey) && slamCooldownTimer <= 0 && !isSlamming && !isGrounded)
            {
                StartSlam();
            }

            if (isSlamming && !slamImpactOccurred && !isReboundingFromSlam)
            {
                Vector3 currentVelocity = rb.velocity;
                currentVelocity.x = 0f;
                currentVelocity.z = 0f;
                rb.velocity = currentVelocity;

                rb.AddForce(Vector3.down * (slamPower * 0.8f), ForceMode.Acceleration);
            }
        }
        #endregion

        #region Crawlslide
        if (enableCrawlSlide && Input.GetKeyDown(crawlSlideKey) && !isSliding && !isDashing)
        {
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(CrawlSlideCoroutine());
        }
        #endregion

        CheckGround();
        if (isGrounded && isSlamming)
        {
            EndSlam();
        }
        if (isGrounded) canAirJump = true;

        if (enableHeadBob) HeadBob();
    }

    private void FixedUpdate()
    {
        #region Crawl Slide Movement
        if (isSliding)
        {
            float verticalVelocity = rb.velocity.y;
            float currentTargetSpeed = walkSpeed * speedModifier;

            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (horizontalVelocity.magnitude > currentTargetSpeed)
            {
                Vector3 newHorizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.fixedDeltaTime * (1.0f - slideFrictionMultiplier));

                if (newHorizontalVelocity.magnitude < currentTargetSpeed)
                {
                    newHorizontalVelocity = newHorizontalVelocity.normalized * currentTargetSpeed;
                }

                rb.velocity = newHorizontalVelocity + Vector3.up * verticalVelocity;
            }
            else if (horizontalVelocity.magnitude < currentTargetSpeed && horizontalVelocity.magnitude > 0.1f)
            {
                Vector3 newHorizontalVelocity = Vector3.MoveTowards(horizontalVelocity, horizontalVelocity.normalized * currentTargetSpeed, Time.fixedDeltaTime * 2f);
                rb.velocity = newHorizontalVelocity + Vector3.up * verticalVelocity;
            }

            return;
        }
        #endregion

        #region Movement
        if (playerCanMove && !isDashing && (!isSlamming || isReboundingFromSlam))
        {
            Vector3 target = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (target.sqrMagnitude > 1f) target.Normalize();

            isWalking = (target.x != 0 || target.z != 0) && isGrounded;

            target = transform.TransformDirection(target) * walkSpeed * speedModifier + externalImpulse;

            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = target - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        #endregion

        externalImpulse = Vector3.Lerp(externalImpulse, Vector3.zero, 5f * Time.fixedDeltaTime);
        if (isSlamming && !slamImpactOccurred && !isReboundingFromSlam)
        {
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            rb.velocity = currentVelocity;

            rb.AddForce(Vector3.down * slamPower * 0.5f, ForceMode.Acceleration);
        }
    }

    private void StartSlam()
    {
        isSlamming = true;
        slamImpactOccurred = false;
        isReboundingFromSlam = false; 
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.x = 0f;
        currentVelocity.z = 0f;
        rb.velocity = currentVelocity;
        rb.AddForce(Vector3.down * slamPower, ForceMode.Impulse);
    }

    private void EndSlam()
    {
        isSlamming = false;
        isReboundingFromSlam = false; 
        slamCooldownTimer = slamCooldown;
        slamImpactOccurred = false;
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = Mathf.Min(currentVelocity.y, 0f);
        rb.velocity = currentVelocity;
    }

    public bool IsSlamming() => isSlamming;


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

    public Vector3 GetForwardVector() => transform.forward;

    public Vector3 GetCurrentHorizontalVelocity()
    {
        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    public KeyCode GetJumpKey() => jumpKey;

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

    public bool IsGrounded() => isGrounded;

    public void InitiateJumpLogic()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (enableAirJump && canAirJump)
        {
            Jump();
            canAirJump = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && styleManager != null && styleManager.CurrentStyle.canBreakWallsWithDash)
        {
            DestructibleWall wall = collision.gameObject.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                wall.DestroyWall();
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
                rb.AddForce(dashDirection * dashPower * 0.5f, ForceMode.Impulse);
            }
        }
        if (isSlamming && styleManager != null && styleManager.CurrentStyle.canBreakWallsWithSlam && !slamImpactOccurred)
        {
            DestructibleWall wall = collision.gameObject.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                slamImpactOccurred = true;
                isReboundingFromSlam = true; 
                Vector3 impactPoint = collision.contacts[0].point;
                wall.DestroyWallFromSlam(impactPoint);
                rb.AddForce(Vector3.up * slamPower * reboundMultiplier, ForceMode.Impulse);
            }
        }
    }

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = modifier;
        if (lingerCoroutine != null)
        {
            StopCoroutine(lingerCoroutine);  
        }
    }

    public void ResetSpeedModifier()
    {
        speedModifier = 1f;
    }

    public void StartLingerSpeedModifier(float lingerTime)
    {
        if (lingerCoroutine != null)
        {
            StopCoroutine(lingerCoroutine);
        }
        lingerCoroutine = StartCoroutine(LingerSpeedModifierCoroutine(lingerTime));
    }

    private IEnumerator LingerSpeedModifierCoroutine(float lingerTime)
    {
        yield return new WaitForSeconds(lingerTime);
        ResetSpeedModifier();
    }

   
    #region LastWallJumpedFrom
    public void SetLastWallJumpedFrom(Component wallComponent)
    {
        LastWallJumpedFrom = wallComponent;
    }

    public void ClearLastWallJumpedFrom()
    {
        LastWallJumpedFrom = null;
    }
    #endregion

    private IEnumerator CrawlSlideCoroutine()
    {
        isSliding = true;
        float timer = 0f;
        Vector3 targetScale = originalScale;
        targetScale.y *= squatHeightScale;

        int currentStyleIndex = styleManager?.GetCurrentStyleIndex() ?? 0;

        Vector3 originalPos = transform.localPosition;
        Vector3 targetPos = originalPos;

        if (currentStyleIndex == 0)
        {
            targetPos.y = originalPos.y - (originalScale.y - targetScale.y) * 0.5f;
        }
        else
        {
            targetPos.y = originalPos.y + (originalScale.y - targetScale.y) * 0.5f;
        }

        if (currentStyleIndex == 1 && isGrounded)
            rb.useGravity = false;

        float t = 0f;
        while (t < squatTransitionDuration)
        {
            t += Time.deltaTime;
            float progress = t / squatTransitionDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            transform.localPosition = Vector3.Lerp(originalPos, targetPos, progress);
            yield return null;
        }
        transform.localScale = targetScale;
        transform.localPosition = targetPos;

        if (currentStyleIndex == 1)
        {
            rb.useGravity = true;
        }

        float initialSpeed = walkSpeed * initialSpeedMultiplier;
        Vector3 slideDirection = GetForwardVector();
        slideDirection.y = 0;
        slideDirection.Normalize();

        Vector3 currentHorizontalVelocity = GetCurrentHorizontalVelocity();
        float requiredForce = initialSpeed - currentHorizontalVelocity.magnitude;

        if (requiredForce > 0)
        {
            rb.AddForce(slideDirection * requiredForce, ForceMode.VelocityChange);
        }
        else
        {
            SetSpeedModifier(initialSpeedMultiplier);
        }

        float finalSpeed = walkSpeed * finalSpeedMultiplier;

        timer = 0f;
        while (timer < slideDuration)
        {
            timer += Time.deltaTime;

            float currentSpeedLerp = Mathf.Lerp(initialSpeedMultiplier, 1.0f, timer / (slideDuration * 0.25f));
            float finalSpeedLerp = Mathf.Lerp(1.0f, finalSpeedMultiplier, (timer - slideDuration * 0.25f) / (slideDuration * 0.75f));

            if (timer < slideDuration * 0.25f)
            {
                SetSpeedModifier(currentSpeedLerp);
            }
            else
            {
                SetSpeedModifier(finalSpeedLerp);
            }

            yield return null;
        }

        t = 0f;
        originalPos = transform.localPosition;
        targetPos = originalPos;

        if (currentStyleIndex == 0)
        {
            targetPos.y = originalPos.y + (originalScale.y - targetScale.y) * 0.5f;
        }
        else
        {
            targetPos.y = originalPos.y - (originalScale.y - targetScale.y) * 0.5f;
        }

        while (t < squatTransitionDuration)
        {
            t += Time.deltaTime;
            float progress = t / squatTransitionDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            transform.localPosition = Vector3.Lerp(originalPos, originalPos, progress);
            yield return null;
        }
        transform.localScale = originalScale;
        transform.localPosition = originalPos;

        ResetSpeedModifier();
        isSliding = false;
    }
}
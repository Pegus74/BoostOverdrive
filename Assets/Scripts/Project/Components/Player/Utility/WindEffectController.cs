
using UnityEngine;
using UnityEngine.UI;

public class WindEffectController : MonoBehaviour
{
    [Header("Настройки эффектов")]
    public Image windEffectImage;
    public Sprite[] windSprites; 
    public float minWindSpeed = 13f;
    public float maxWindSpeed = 30f;
    public float spriteChangeSpeed = 0.1f;

    [Header("Ссылки")]
    public PlayerMovementController playerMovementController;
    public PlayerCameraView playerCameraView;

    private Rigidbody playerRigidbody;
    private float currentWindTimer = 0f;
    private int currentSpriteIndex = 0;
    private bool isWindActive = false;

    private void Awake()
    {
        playerRigidbody = playerMovementController.GetComponent<Rigidbody>();
        if (windEffectImage != null)
        {
            windEffectImage.gameObject.SetActive(false);
            windEffectImage.color = new Color(1, 1, 1, 0);
        }
    }

    private void Update()
    {
        if (playerRigidbody == null || playerMovementController == null) return;
        Vector3 horizontalVelocity = playerRigidbody.linearVelocity;
        horizontalVelocity.y = 0;
        float currentSpeed = horizontalVelocity.magnitude;
        if (currentSpeed >= 13f)
        {
            if (!isWindActive)
            {
                ActivateWindEffect(true);
            }

            UpdateWindEffect(currentSpeed);

            ModifyCameraBob(1.5f);
        }
        else if (currentSpeed >= 10f)
        {
            if (isWindActive)
            {
                ActivateWindEffect(false);
            }
            ModifyCameraBob(1.2f);
        }
        else
        {
            if (isWindActive)
            {
                ActivateWindEffect(false);
            }
            ResetCameraBob();
        }
    }

    private void ActivateWindEffect(bool activate)
    {
        isWindActive = activate;

        if (windEffectImage != null)
        {
            windEffectImage.gameObject.SetActive(activate);

            if (activate && windSprites != null && windSprites.Length > 0)
            {
                windEffectImage.sprite = windSprites[0];
                currentSpriteIndex = 0;
                currentWindTimer = 0f;
            }
        }
    }

    private void UpdateWindEffect(float currentSpeed)
    {
        if (!isWindActive || windEffectImage == null || windSprites == null || windSprites.Length == 0)
            return;
        float windIntensity = Mathf.Clamp01((currentSpeed - minWindSpeed) / (maxWindSpeed - minWindSpeed));

        currentWindTimer += Time.deltaTime;
        if (currentWindTimer >= spriteChangeSpeed)
        {
            currentWindTimer = 0f;
            currentSpriteIndex = (currentSpriteIndex + 1) % windSprites.Length;
            windEffectImage.sprite = windSprites[currentSpriteIndex];
        }
        Color color = windEffectImage.color;
        color.a = Mathf.Lerp(0.3f, 0.7f, windIntensity);
        windEffectImage.color = color;

        float shakeAmount = windIntensity * 0.5f;
        windEffectImage.rectTransform.localPosition = new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount),
            0
        );
    }

    private void ModifyCameraBob(float intensityMultiplier)
    {
        if (playerCameraView != null)
        {

        }
    }

    private void ResetCameraBob()
    {
        if (playerCameraView != null)
        {
        }
    }
}

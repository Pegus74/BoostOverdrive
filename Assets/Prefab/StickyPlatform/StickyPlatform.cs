using UnityEngine;
using System.Collections;

public class StickyPlatform : MonoBehaviour
{
    [Header("Settings")]
    public StickyPlatformSettings settings;

    private Collider platformCollider;

    private PlayerStateModel stuckModel;
    private Rigidbody playerRb;
    private CapsuleCollider playerCapsule;
    private Transform playerTransform;

    private float originalHeight;
    private float originalCenterY;
    private float originalDrag;
    private float originalAngularDrag;

    private float sinkDepth;

    private bool isStuck = false;
    private float lastJumpTime = 0f;
    private bool waitingForSecondJump = false;
    private Coroutine sinkCoroutine = null;
    private Coroutine unsinkCoroutine = null;

    private void Awake()
    {
        platformCollider = GetComponent<Collider>();
        if (platformCollider == null)
        {
            enabled = false;
            return;
        }
        platformCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;
        TryStick(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isStuck) return;
        TryStick(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isStuck)
        {
            EscapeSticky();
        }
    }

    private void TryStick(GameObject playerObj)
    {
        if (!enabled || settings == null) return;
        if (!playerObj.CompareTag("Player")) return;

        PlayerStateModel model = playerObj.GetComponent<PlayerStateModel>();
        if (model == null || model.CurrentStyleIndex != 1) return;

        CapsuleCollider capsule = playerObj.GetComponent<CapsuleCollider>();
        if (capsule == null) return;

        Rigidbody rb = playerObj.GetComponent<Rigidbody>();
        if (rb == null) return;

        ActivateSticky(model, rb, capsule, playerObj.transform);
    }

    private void ActivateSticky(PlayerStateModel model, Rigidbody rb, CapsuleCollider capsule, Transform playerTrans)
    {
        stuckModel = model;
        playerRb = rb;
        playerCapsule = capsule;
        playerTransform = playerTrans;

        originalHeight = capsule.height;
        originalCenterY = capsule.center.y;
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;

        model.SetMovementSpeedModifier(settings.stickySpeedModifier);
        rb.linearDamping = 8f;
        rb.angularDamping = 10f;

        sinkDepth = originalHeight * settings.sinkPercent;

        if (sinkCoroutine != null) StopCoroutine(sinkCoroutine);
        sinkCoroutine = StartCoroutine(SinkCoroutine());

        isStuck = true;

        InputEvents.JumpAttemptEvent.AddListener(OnJumpAttempt);
        InputEvents.ToggleStyleAttemptEvent.AddListener(OnStyleToggle);
    }

    private IEnumerator SinkCoroutine()
    {
        if (playerCapsule == null || playerTransform == null)
        {
            sinkCoroutine = null;
            yield break;
        }

        float targetHeight = originalHeight * (1f - settings.sinkPercent);
        Vector3 startCenter = playerCapsule.center;
        Vector3 targetCenter = new Vector3(0f, originalCenterY - sinkDepth * 0.5f, 0f);

        float halfDuration = settings.sinkDuration * 0.5f;
        float timer = 0f;
        while (timer < halfDuration)
        {
            if (playerCapsule == null)
            {
                sinkCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / halfDuration);

            playerCapsule.height = Mathf.Lerp(originalHeight, targetHeight, t);
            playerCapsule.center = Vector3.Lerp(startCenter, targetCenter, t);

            yield return null;
        }

        if (playerCapsule != null)
        {
            playerCapsule.height = targetHeight;
            playerCapsule.center = targetCenter;
        }

        Physics.IgnoreCollision(playerCapsule, platformCollider, true);

        if (playerTransform == null)
        {
            sinkCoroutine = null;
            yield break;
        }

        Vector3 startPos = playerTransform.position;
        Vector3 targetPos = startPos + Vector3.down * sinkDepth;

        timer = 0f;
        while (timer < halfDuration)
        {
            if (playerTransform == null)
            {
                sinkCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / halfDuration);

            playerTransform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        if (playerTransform != null)
        {
            playerTransform.position = targetPos;
        }

        sinkCoroutine = null;
    }

    private void OnJumpAttempt()
    {
        if (!isStuck) return;

        float currentTime = Time.time;
        if (!waitingForSecondJump || (currentTime - lastJumpTime > settings.doubleTapWindow))
        {
            waitingForSecondJump = true;
            lastJumpTime = currentTime;
            return;
        }

        EscapeSticky();
    }

    private void OnStyleToggle()
    {
        if (!isStuck || stuckModel == null) return;
        if (stuckModel.CurrentStyleIndex == 0)
        {
            EscapeSticky();
        }
    }

    private void EscapeSticky()
    {
        if (!isStuck) return;

        isStuck = false;
        waitingForSecondJump = false;

        InputEvents.JumpAttemptEvent.RemoveListener(OnJumpAttempt);
        InputEvents.ToggleStyleAttemptEvent.RemoveListener(OnStyleToggle);

        if (stuckModel != null)
        {
            stuckModel.SetMovementSpeedModifier(1f);
        }

        if (playerRb != null)
        {
            playerRb.linearDamping = originalDrag;
            playerRb.angularDamping = originalAngularDrag;
        }

        if (sinkCoroutine != null)
        {
            StopCoroutine(sinkCoroutine);
            sinkCoroutine = null;
        }

        if (playerCapsule != null && playerTransform != null)
        {
            if (unsinkCoroutine != null)
                StopCoroutine(unsinkCoroutine);

            unsinkCoroutine = StartCoroutine(UnSinkCoroutine());
        }
        else
        {
            CleanupReferences();
        }
    }

    private IEnumerator UnSinkCoroutine()
    {
        if (playerTransform == null || playerCapsule == null)
        {
            CleanupReferences();
            unsinkCoroutine = null;
            yield break;
        }

        Vector3 startPos = playerTransform.position;
        Vector3 targetPos = startPos + Vector3.up * sinkDepth;

        float halfDuration = settings.unSinkDuration * 0.5f;
        float timer = 0f;
        while (timer < halfDuration)
        {
            if (playerTransform == null)
            {
                CleanupReferences();
                unsinkCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / halfDuration);

            playerTransform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        if (playerTransform != null)
        {
            playerTransform.position = targetPos;
        }

        Physics.IgnoreCollision(playerCapsule, platformCollider, false);

        Vector3 startCenter = playerCapsule.center;
        float startHeight = playerCapsule.height;

        Vector3 targetCenter = new Vector3(0f, originalCenterY, 0f);
        float targetHeight = originalHeight;

        timer = 0f;
        while (timer < halfDuration)
        {
            if (playerCapsule == null)
            {
                CleanupReferences();
                unsinkCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / halfDuration);

            playerCapsule.height = Mathf.Lerp(startHeight, targetHeight, t);
            playerCapsule.center = Vector3.Lerp(startCenter, targetCenter, t);

            yield return null;
        }

        if (playerCapsule != null)
        {
            playerCapsule.height = targetHeight;
            playerCapsule.center = targetCenter;
        }

        CleanupReferences();
        unsinkCoroutine = null;
    }

    private void CleanupReferences()
    {
        stuckModel = null;
        playerRb = null;
        playerCapsule = null;
        playerTransform = null;
    }
}
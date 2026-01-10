using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DashSystem : MonoBehaviour
{
    public PlayerStateModel playerStateModel;
    public UnityEvent OnDashPerformed;
    public UnityEvent OnDashReady;
    private Rigidbody rb;
    private bool isDashAvailable = true;
    private Coroutine currentDashCoroutine;

    public PlayerConfig Pc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) enabled = false;
        playerStateModel.SetIsDashing(false);
    }

    void OnEnable() => InputEvents.DashAttemptEvent.AddListener(InitiateDash);
    void OnDisable() => InputEvents.DashAttemptEvent.RemoveListener(InitiateDash);

    private void InitiateDash()
    {
        if (rb == null || !isDashAvailable || playerStateModel.IsDashing || playerStateModel.IsSliding || playerStateModel.IsSlamming) return;
        if (currentDashCoroutine != null) StopCoroutine(currentDashCoroutine);
        currentDashCoroutine = StartCoroutine(DashCoroutine());
        OnDashPerformed?.Invoke();
    }
    private IEnumerator DashCoroutine()
    {
        isDashAvailable = false;
        playerStateModel.SetIsDashing(true);
        AbilityEvents.OnAbilityStarted.Invoke();

        Vector3 baseDirection = transform.forward;
        baseDirection.y = 0f;
        if (baseDirection.sqrMagnitude > 0.01f)
        {
            baseDirection.Normalize();
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVelocity.sqrMagnitude > 0.01f)
            {
                baseDirection = horizontalVelocity.normalized;
            }
            else
            {
                baseDirection = Vector3.forward;
            }
        }

        float power = playerStateModel.CurrentDashPower;
        float duration = playerStateModel.settings.dashDuration;
        float cooldown = playerStateModel.settings.dashCooldown;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(baseDirection * power, ForceMode.Impulse);

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            Vector3 currentDir = baseDirection;
            currentDir.y = 0f;

            float accel = power * 0.6f * (timer / duration);
            rb.AddForce(currentDir * accel, ForceMode.Acceleration);

            if (Mathf.Abs(rb.linearVelocity.y) > 2f)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    Mathf.Sign(rb.linearVelocity.y) * 2f,
                    rb.linearVelocity.z
                );
            }

            TryDestroyWallDuringDash(baseDirection);

            yield return null;
        }

        playerStateModel.SetIsDashing(false);

        float cdTimer = cooldown;
        while (cdTimer > 0f)
        {
            cdTimer -= Time.deltaTime;
            yield return null;
        }
        OnDashReady?.Invoke();
        isDashAvailable = true;
        currentDashCoroutine = null;
    }

    private void TryDestroyWallDuringDash(Vector3 dashDirection)
    {
        if (playerStateModel.CurrentStyleIndex != 1)
            return;

        Vector3[] origins = new Vector3[]
        {
        transform.position + Vector3.up * 0.5f,
        transform.position + Vector3.up * 1.2f,
        transform.position + Vector3.up * 0.1f
        };

        float checkDistance = 1.2f;

        foreach (Vector3 origin in origins)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, dashDirection, out hit, checkDistance))
            {
                RDestructibleWall destructible = hit.collider.GetComponentInParent<RDestructibleWall>();
                if (destructible != null)
                {
                    destructible.DestroyWall(hit.point);
                    break;
                }
            }
        }
    }

    public float GetCooldownDuration()
    {
        return playerStateModel.settings.dashCooldown;
    }
}
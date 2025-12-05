using UnityEngine;
using System.Collections;

public class DashSystem : MonoBehaviour
{
    public PlayerStateModel playerStateModel;

    private Rigidbody rb;
    private bool isDashAvailable = true;
    private Coroutine currentDashCoroutine;

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
    }

    private IEnumerator DashCoroutine()
    {
        isDashAvailable = false;
        playerStateModel.SetIsDashing(true);
        AbilityEvents.OnAbilityStarted.Invoke();

        // Получаем направление взгляда игрока
        Vector3 baseDirection = transform.forward;

        // ОБНОВЛЕНИЕ: Ограничиваем вертикальную составляющую
        // Сохраняем только горизонтальное направление (убираем вертикальную составляющую)
        baseDirection.y = 0f;

        // Нормализуем только если есть достаточная длина
        if (baseDirection.sqrMagnitude > 0.01f)
        {
            baseDirection.Normalize();
        }
        else
        {
            // Если смотрим прямо вверх/вниз, используем текущее горизонтальное направление движения
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

        // Очищаем вертикальную скорость при начале рывка
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(baseDirection * power, ForceMode.Impulse);

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // В течение рывка продолжаем применять силу только в горизонтальной плоскости
            Vector3 currentDir = baseDirection;
            currentDir.y = 0f; // Обеспечиваем, что направление остается горизонтальным

            float accel = power * 0.6f * (timer / duration);
            rb.AddForce(currentDir * accel, ForceMode.Acceleration);

            // ДОПОЛНИТЕЛЬНО: Ограничиваем вертикальную скорость в течение всего рывка
            if (Mathf.Abs(rb.linearVelocity.y) > 2f) // Макс вертикальная скорость 2 м/с
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    Mathf.Sign(rb.linearVelocity.y) * 2f,
                    rb.linearVelocity.z
                );
            }

            yield return null;
        }

        playerStateModel.SetIsDashing(false);

        float cdTimer = cooldown;
        while (cdTimer > 0f)
        {
            cdTimer -= Time.deltaTime;
            yield return null;
        }

        isDashAvailable = true;
        currentDashCoroutine = null;
    }
}
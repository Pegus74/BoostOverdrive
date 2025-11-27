using UnityEngine;
using UnityEngine.Events;

public class ChargedJumpController : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel;
    public PlayerSettingsData playerSettingsData;

    [Header("Input Events")]
    public UnityEvent JumpAttemptEvent;
    public UnityEvent JumpCanceledEvent;

    [Header("Charged Jump Events")]
    public UnityEvent OnChargeStart;
    public UnityEvent<float> OnChargeUpdate;
    public UnityEvent OnChargeCancel;
    public UnityEvent<float> OnChargedJumpExecute;

    private bool isCharging = false;
    private float currentCharge = 0f;
    private float chargeStartTime = 0f;

    private void OnEnable()
    {
        JumpAttemptEvent?.AddListener(HandleJumpInput);
        JumpCanceledEvent?.AddListener(HandleJumpCancel);
    }

    private void OnDisable()
    {
        // ÈÑÏÐÀÂËÅÍÎ: èñïîëüçóåì RemoveListener âìåñòî UnregisterListener
        JumpAttemptEvent?.RemoveListener(HandleJumpInput);
        JumpCanceledEvent?.RemoveListener(HandleJumpCancel);

        if (isCharging)
        {
            CancelCharging();
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            UpdateCharging();
        }
    }

    private void HandleJumpInput()
    {
        if (!playerSettingsData.enableChargedJump || !playerStateModel.IsGrounded || isCharging)
            return;

        StartCharging();
    }

    private void HandleJumpCancel()
    {
        if (isCharging)
        {
            ExecuteChargedJump();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        currentCharge = 0f;
        chargeStartTime = Time.time;

        OnChargeStart?.Invoke();
        playerStateModel.SetIsChargingJump(true);
    }

    private void UpdateCharging()
    {
        if (!playerStateModel.IsGrounded)
        {
            CancelCharging();
            return;
        }

        currentCharge += playerSettingsData.chargeSpeed * Time.deltaTime;
        currentCharge = Mathf.Clamp01(currentCharge);

        ChargedJumpUI ui = FindObjectOfType<ChargedJumpUI>();
        if (ui != null)
        {
            ui.OnChargeUpdate(currentCharge);
        }

        OnChargeUpdate?.Invoke(currentCharge);

        if (currentCharge >= 1f)
        {
            ExecuteChargedJump();
        }
    }

    private void ExecuteChargedJump()
    {
        if (!isCharging) return;
        float minPower = GetMinJumpPower();
        float maxPower = GetMaxJumpPower();
        float jumpPower = Mathf.Lerp(minPower, maxPower, currentCharge);
        OnChargedJumpExecute?.Invoke(jumpPower);
        CancelCharging();
    }

    private void CancelCharging()
    {
        if (!isCharging) return;

        isCharging = false;
        currentCharge = 0f;
        OnChargeCancel?.Invoke();
        playerStateModel.SetIsChargingJump(false);
    }

    private float GetMinJumpPower()
    {
        return playerStateModel.CurrentJumpPower * playerSettingsData.minJumpPowerPercent;
    }

    private float GetMaxJumpPower()
    {
        return playerStateModel.CurrentJumpPower;
    }
}
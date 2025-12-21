using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset actions;
    public string actionName;
    public int bindingIndex;

    [Header("UI")]
    public TMP_Text bindingText;
    public TMP_Text actionLabel;

    private InputAction action;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Awake()
    {
        action = actions.FindAction(actionName);
        UpdateUI();
    }

    public void StartRebind()
    {
        if (action == null)
            return;

        action.Disable();

        bindingText.text = "Press key...";

        rebindingOperation = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse") // по желанию
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();
                UpdateUI();
            })
            .Start();
    }

    public void UpdateUI()
    {
        if (action == null)
            return;

        bindingText.text = action.GetBindingDisplayString(bindingIndex);
    }
}
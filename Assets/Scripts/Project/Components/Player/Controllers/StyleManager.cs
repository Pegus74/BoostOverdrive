using UnityEngine;

/// <summary>
/// Слушает ввод ToggleStyleAttemptEvent, 
/// переключает текущий стиль, читает данные из PlayerStyleData 
/// и записывает рабочие параметры в PlayerStateModel.
/// </summary>
public class StyleManager : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;
    
    [Tooltip("Массив неизменяемых данных о стилях")]
    public PlayerStyleData[] styleDataAssets; 
    
    [Header("Input Listener")]
    public GameEvent ToggleStyleAttemptEvent; 

    
    private int currentStyleIndex = 0;

    void Awake()
    {
        if (playerStateModel == null || styleDataAssets == null || styleDataAssets.Length == 0)
        {
            enabled = false;
        }
    }

    void OnEnable()
    {
        // Подписываемся на событие ввода
        ToggleStyleAttemptEvent?.RegisterListener(SwitchStyle);
        // Устанавливаем начальное состояние при старте
        ApplyStyleToModel(currentStyleIndex);
    }

    void OnDisable()
    {
        ToggleStyleAttemptEvent?.UnregisterListener(SwitchStyle);
    }
    
    /// <summary>
    /// Вызывается по событию ввода ToggleStyleAttemptEvent и обновляет стиль
    /// </summary>
    private void SwitchStyle()
    {
        int newStyleIndex = (playerStateModel.CurrentStyleIndex + 1) % styleDataAssets.Length;
        ApplyStyleToModel(newStyleIndex);
    }

    /// <summary>
    /// Применяет данные стиля к PlayerStateModel.
    /// </summary>
    private void ApplyStyleToModel(int index)
    {
        if (index < 0 || index >= styleDataAssets.Length) return;

        PlayerStyleData style = styleDataAssets[index];

        // 1. Обновляем рабочие параметры в Модели
        playerStateModel.SetWalkSpeed(style.walkSpeed);
        playerStateModel.SetJumpPower(style.jumpPower);
        playerStateModel.SetDashPower(style.dashPower);
        
        // SetStyleIndex также автоматически вызывает OnStyleChangedEvent(index)
        playerStateModel.SetStyleIndex(index);
    }
}
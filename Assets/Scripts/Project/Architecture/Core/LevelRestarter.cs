using UnityEngine;
using System.Collections;
using System.Linq; 

/// <summary>
/// Управляет логикой мягкого перезапуска уровня.
/// </summary>
public class LevelRestarter : MonoBehaviour
{
    [Header("Soft Reset Events")]
    [SerializeField] private GameEvent SoftResetRequestEvent;    // Слушаем, чтобы начать 
    [SerializeField] private GameEvent SoftResetCompletedEvent;  // Вызываем после завершения
    [SerializeField] private GameEvent OnLevelResetEvent;        // Вызываем, чтобы сбросить объекты
    
    [Header("UI Components")]
    [SerializeField] private ScreenFader screenFader; 
    [SerializeField] private float fadeDuration = 0.5f; 

    [Header("Gameplay Components")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private Transform restartPosition; 

    private void OnEnable()
    {
        if (SoftResetRequestEvent != null)
            SoftResetRequestEvent.RegisterListener(StartSoftReset);
    }

    private void OnDisable()
    {
        if (SoftResetRequestEvent != null)
            SoftResetRequestEvent.UnregisterListener(StartSoftReset);
    }
    
    /// <summary>
    /// Вызывается событием SoftResetRequestEvent.
    /// </summary>
    public void StartSoftReset()
    {
        if (screenFader == null || playerTransform == null || restartPosition == null || SoftResetCompletedEvent == null)
        {
            SoftResetCompletedEvent.Raise(); 
            return;
        }
        
        StartCoroutine(SoftResetLevelCoroutine());
    }

    private IEnumerator SoftResetLevelCoroutine()
    {
        // Затухание экрана
        yield return StartCoroutine(screenFader.FadeOut(fadeDuration)); 
        
        // Вызываем событие сброса 
        OnLevelResetEvent.Raise(); 
        
        // Телепорт игрока
        TeleportPlayer();
        
        // Экран появляется
        yield return StartCoroutine(screenFader.FadeIn(fadeDuration)); 
        
        SoftResetCompletedEvent.Raise();
    }
    
    private void TeleportPlayer()
    {
        if (playerTransform != null && restartPosition != null)
        {
            playerTransform.position = restartPosition.position;
            playerTransform.rotation = restartPosition.rotation;
        }
    }
}
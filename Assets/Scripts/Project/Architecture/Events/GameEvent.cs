using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Базовый класс для всех ScriptableObject событий без аргументов
/// </summary>
[CreateAssetMenu(menuName = "Architecture/Events/Void Event")]
public class GameEvent : ScriptableObject
{
    // Список действий, которые будут вызваны при Raise()
    private readonly List<UnityAction> listeners = new List<UnityAction>();

    /// <summary>
    /// Вызывает событие, уведомляя всех подписчиков
    /// </summary>
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke();
        }
    }

    /// <summary>
    /// Регистрирует новый метод-слушатель
    /// </summary>
    public void RegisterListener(UnityAction listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    /// <summary>
    /// Отписывает метод-слушатель
    /// </summary>
    public void UnregisterListener(UnityAction listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
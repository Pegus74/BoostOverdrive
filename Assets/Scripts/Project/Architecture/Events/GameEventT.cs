using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Базовый класс для ScriptableObject событий с одним аргументом типа T.
/// </summary>
public abstract class GameEvent<T> : ScriptableObject
{
    // Используем дженерик UnityEvent
    private readonly List<UnityAction<T>> listeners = new List<UnityAction<T>>();

    /// <summary>
    /// Вызывает событие, уведомляя всех подписчиков и передавая аргумент.
    /// </summary>
    public void Raise(T value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke(value);
        }
    }

    /// <summary>
    /// Регистрирует новый метод-слушатель.
    /// </summary>
    public void RegisterListener(UnityAction<T> listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    /// <summary>
    /// Отписывает метод-слушатель.
    /// </summary>
    public void UnregisterListener(UnityAction<T> listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}
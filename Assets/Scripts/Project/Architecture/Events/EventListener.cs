using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Компонент, который слушает GameEventSO и вызывает UnityEvent при срабатывании.
/// </summary>
public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent Event;
    [SerializeField] private UnityEvent Response;

    private void OnEnable()
    {
        // При включении компонента (в начале сцены или при активации) регистрируем Response
        if (Event != null)
        {
            Event.RegisterListener(OnEventRaised);
        }
    }

    private void OnDisable()
    {
        // При выключении компонента (или при Destroy) отписываемся
        if (Event != null)
        {
            Event.UnregisterListener(OnEventRaised);
        }
    }

    /// <summary>
    /// Вызывается системой событий при срабатывании GameEvent.
    /// </summary>
    private void OnEventRaised()
    {
        // Вызываем UnityEvent, который можно настроить в Инспекторе
        Response.Invoke();
    }

    public void LogEvent(string message)
    {
        Debug.Log(message);
    }
}
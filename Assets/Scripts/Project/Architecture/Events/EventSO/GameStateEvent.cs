using UnityEngine;

/// <summary>
/// Событие SO для оповещения об изменении состояния игры
/// </summary>
[CreateAssetMenu(menuName = "Architecture/Events/Game State Event")]
public class GameStateEvent : GameEvent<GameState> { }
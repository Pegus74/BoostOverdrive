using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InputEvents
{
    public Vector2Event MoveInputEvent = new Vector2Event();
    public Vector2Event LookInputEvent = new Vector2Event();
    public UnityEvent JumpAttemptEvent = new UnityEvent();
    public UnityEvent JumpCanceledEvent = new UnityEvent();
    public UnityEvent DashAttemptEvent = new UnityEvent();
    public UnityEvent SlamAttemptEvent = new UnityEvent();
    public UnityEvent SlideAttemptEvent = new UnityEvent();
    public UnityEvent ToggleStyleAttemptEvent = new UnityEvent();
    public UnityEvent OnPauseAttemptEvent = new UnityEvent();
}
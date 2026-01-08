using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public static class InputEvents
{
    public static Vector2Event MoveInputEvent = new Vector2Event();
    public static Vector2Event LookInputEvent = new Vector2Event();
    
    public static UnityEvent JumpAttemptEvent = new UnityEvent();
    public static UnityEvent JumpCanceledEvent = new UnityEvent();
    
    
    public static UnityEvent DashAttemptEvent = new UnityEvent(); 
    public static UnityEvent SlamAttemptEvent = new UnityEvent();
    
    public static UnityEvent SlideAttemptEvent = new UnityEvent();
    public static UnityEvent SlideCanceledEvent = new UnityEvent();
    
    public static UnityEvent ToggleStyleAttemptEvent = new UnityEvent();
    
    public static UnityEvent OnPauseAttemptEvent = new UnityEvent();
    public static UnityEvent OnRestartAttemptEvent = new UnityEvent();
    
    public static UnityEvent OnFirstBiomeTeleportEvent = new UnityEvent();
    public static UnityEvent OnSecondBiomeTeleportEvent = new UnityEvent();
    public static UnityEvent OnThirdBiomeTeleportEvent = new UnityEvent();
}
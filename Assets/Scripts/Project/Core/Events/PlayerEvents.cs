using UnityEngine.Events;

[System.Serializable]
public static class PlayerEvents
{
    public static FloatEvent OnPlayerSpeedModifierChange = new FloatEvent();
    
    public static BoolEvent OnStyleChangedEvent = new BoolEvent();
}
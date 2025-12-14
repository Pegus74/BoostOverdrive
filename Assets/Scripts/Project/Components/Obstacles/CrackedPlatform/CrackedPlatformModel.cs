using UnityEngine;

public class CrackedPlatformModel
{
    public bool IsTriggered { get; private set; }
    public bool IsDestroyed { get; private set; }

    public float destroyDelay;

    public CrackedPlatformModel(float destroyDelay)
    {
        this.destroyDelay = destroyDelay;
    }

    public void Trigger()
    {
        if (IsTriggered) return;
        IsTriggered = true;
    }

    public void Destroy()
    {
        IsDestroyed = true;
    }
}
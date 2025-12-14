using System.Collections.Generic;

public class DryStripModel
{
    public bool IsTriggered { get; private set; }

    public void Trigger()
    {
        IsTriggered = true;
    }
}
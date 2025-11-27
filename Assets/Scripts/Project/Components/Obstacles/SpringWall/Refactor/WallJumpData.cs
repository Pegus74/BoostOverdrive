using UnityEngine;

[System.Serializable]
public struct WallJumpData
{
    public Vector3 surfaceNormal;
    public Component wallComponent;
    public int styleIndex;
}

[System.Serializable]
public class WallJumpEvent : UnityEvent<WallJumpData> { }
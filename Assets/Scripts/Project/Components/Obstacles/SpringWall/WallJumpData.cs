using UnityEngine;

[System.Serializable]
public class WallJumpData
{
    public Vector3 surfaceNormal;
    public Component wallComponent;

    public WallJumpData(Vector3 _surfaceNormal, Component _wallComponent)
    {
        this.surfaceNormal = _surfaceNormal;
        this.wallComponent = _wallComponent;
    }
}
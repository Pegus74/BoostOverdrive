using UnityEngine;

public static class SurfaceUtils
{
    public static bool IsSurfaceSloped(Vector3 surfaceNormal, float threshold = 5f)
    {
        return Vector3.Angle(surfaceNormal, Vector3.up) > threshold;
    }

    public static float GetSurfaceSlopeAngle(Vector3 surfaceNormal)
    {
        return Vector3.Angle(surfaceNormal, Vector3.up);
    }

    public static Vector3 ProjectOnSurface(Vector3 vector, Vector3 surfaceNormal)
    {
        return vector - Vector3.Dot(vector, surfaceNormal) * surfaceNormal;
    }
}
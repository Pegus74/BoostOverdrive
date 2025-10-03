using UnityEngine;

public class RedWallReflector : MonoBehaviour
{
    [Header("Коэффициент скорости после отскока")]
    public float reflectionSpeedMultiplier = 1.2f;

    [Header("Время для плавного поворота камеры в секундах")]
    public float cameraRotationTime = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        FirstPersonController fpc = collision.gameObject.GetComponent<FirstPersonController>();
        if (fpc == null)
            return;

        Vector3 incomingVelocity = collision.relativeVelocity;
        // отскока не будет если скорость слишком низка
        if (incomingVelocity.magnitude < 5.0f) 
            return;

        Vector3 surfaceNormal = collision.contacts[0].normal;

        Vector3 reflectedVector = Vector3.Reflect(incomingVelocity.normalized, surfaceNormal);

        float speedMagnitude = incomingVelocity.magnitude;
        float newSpeed = speedMagnitude * reflectionSpeedMultiplier;
        Vector3 impulseVector = reflectedVector * newSpeed; 

        fpc.SetExternalImpulse(impulseVector);

        // поворот камеры
        Quaternion targetRotation = Quaternion.LookRotation(reflectedVector);
        float targetYaw = targetRotation.eulerAngles.y;
        fpc.SmoothlyRotateCameraYaw(targetYaw, cameraRotationTime);
    }
}
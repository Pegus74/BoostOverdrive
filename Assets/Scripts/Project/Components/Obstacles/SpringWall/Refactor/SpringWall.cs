using UnityEngine;
using System.Collections;

public class SpringWall : MonoBehaviour
{
    [Header("Settings")]
    public SpringWallSettings settings;

    [Header("Events")]
    public WallJumpEvent OnWallJumpDetectedEvent;

    private Collider wallCollider;
    private PlayerStateModel playerStateModel;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        wallCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & settings.playerLayer) != 0)
        {
            playerStateModel = other.GetComponent<PlayerStateModel>();
            if (playerStateModel != null)
            {
                isPlayerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & settings.playerLayer) != 0)
        {
            isPlayerInRange = false;
            playerStateModel = null;
        }
    }

    private void Update()
    {
        if (!isPlayerInRange || playerStateModel == null) return;

        // Проверяем возможность прыжка от стены при нажатии прыжка
        if (playerStateModel.IsGrounded)
        {
            playerStateModel.SetLastWallJumpedFrom(null);
        }

        // Проверяем ввод прыжка (это будет обрабатываться через события)
        // Логика обнаружения стены перенесена в отдельный метод
    }

    // Публичный метод для проверки возможности прыжка от стены
    public bool TryDetectWallJump(out WallJumpData jumpData)
    {
        jumpData = new WallJumpData();

        if (!isPlayerInRange || playerStateModel == null || playerStateModel.IsGrounded)
            return false;

        if (playerStateModel.LastWallJumpedFrom == this)
            return false;

        Vector3 surfaceNormal = DetectWallSurfaceNormal();
        if (surfaceNormal == Vector3.zero)
            return false;

        jumpData = new WallJumpData
        {
            surfaceNormal = surfaceNormal,
            wallComponent = this,
            styleIndex = playerStateModel.CurrentStyleIndex
        };

        return true;
    }

    private Vector3 DetectWallSurfaceNormal()
    {
        Vector3 playerPosition = playerStateModel.transform.position;

        Vector3[] checkDirections = {
            playerStateModel.transform.forward,
            -playerStateModel.transform.forward,
            playerStateModel.transform.right,
            -playerStateModel.transform.right
        };

        foreach (Vector3 direction in checkDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerPosition, direction, out hit,
                settings.raycastCheckDistance, settings.playerLayer))
            {
                if (hit.collider == wallCollider && hit.distance <= settings.activationDistance)
                {
                    return hit.normal;
                }
            }
        }
        Vector3 closestPoint = wallCollider.ClosestPoint(playerPosition);
        if (Vector3.Distance(playerPosition, closestPoint) <= settings.activationDistance)
        {
            return (playerPosition - closestPoint).normalized;
        }

        return Vector3.zero;
    }
    public IEnumerator ApplySpeedModifierCoroutine(PlayerStateModel model)
    {
        model.SetMovementSpeedModifier(settings.handsSpeedModifier);
        float timer = 0f;

        while (timer < settings.handsSpeedModifierDuration && !model.IsGrounded)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        model.SetMovementSpeedModifier(1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.activationDistance);
    }
}
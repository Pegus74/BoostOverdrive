using UnityEngine;

public class DashCollisionHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerStateModel playerStateModel;

    private const int HandsStyleIndex = 1;

    private Rigidbody playerRb;
    private Vector3 preservedDashVelocity;
    private float preserveEndTime = -1f;

    private void Awake()
    {
        if (playerStateModel == null)
        {
            playerStateModel = GetComponentInParent<PlayerStateModel>();
        }

        playerRb = playerStateModel.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerStateModel.IsDashing || playerStateModel.CurrentStyleIndex != HandsStyleIndex)
            return;

        RDestructibleWall wall = other.GetComponent<RDestructibleWall>();
        if (wall == null)
            return;

        Collider wallMainCollider = wall.GetComponent<Collider>();
        if (wallMainCollider != null)
        {
            Collider playerCollider = GetComponentInParent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, wallMainCollider, true);
            }
        }

        preservedDashVelocity = playerRb.linearVelocity;

        float dashDuration = playerStateModel.settings.dashDuration;
        preserveEndTime = Time.time + dashDuration;

        wall.DestroyWall(transform.position);
    }

    private void FixedUpdate()
    {
        if (Time.time < preserveEndTime && playerStateModel.IsDashing)
        {
            playerRb.linearVelocity = preservedDashVelocity;
        }
    }
}
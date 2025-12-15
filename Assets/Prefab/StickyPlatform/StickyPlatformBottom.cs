using UnityEngine;

public class StickyPlatformBottom : MonoBehaviour
{
    [Header("Links")]
    public StickyPlatformTop topScript;

    private PlayerStateModel stuckModel;
    private Rigidbody playerRb;
    private Transform playerTransform;
    private CapsuleCollider playerCapsule;
    private GameObject playerObj;

    private bool isStuck = false;

    public void ActivateSticky(GameObject player)
    {
        if (isStuck) return;

        playerObj = player;
        stuckModel = player.GetComponent<PlayerStateModel>();
        playerRb = player.GetComponent<Rigidbody>();
        playerTransform = player.transform;
        playerCapsule = player.GetComponent<CapsuleCollider>();

        if (stuckModel != null)
        {
            stuckModel.playerCanMove = false;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        isStuck = true;

        InputEvents.ToggleStyleAttemptEvent.AddListener(OnStyleToggle);
    }

    public void DeactivateSticky()
    {
        if (!isStuck) return;
        EscapeSticky();
    }

    private void FixedUpdate()
    {
        if (isStuck && playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
    }

    private void OnStyleToggle()
    {
        if (!isStuck || stuckModel == null) return;

        if (stuckModel.CurrentStyleIndex == 0) // Hands
        {
            EscapeSticky();
        }
    }

    private void EscapeSticky()
    {
        if (!isStuck) return;

        isStuck = false;

        InputEvents.ToggleStyleAttemptEvent.RemoveListener(OnStyleToggle);

        if (stuckModel != null)
        {
            stuckModel.playerCanMove = true;
        }

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        // Телепорт наверх и восстановление коллизии
        if (playerTransform != null && topScript != null)
        {
            Vector3 exitPos = playerTransform.position + Vector3.up * 1.5f; // Подгони под толщину
            playerTransform.position = exitPos;
            topScript.RestoreCollision(playerCapsule);
        }

        playerObj = null;
        stuckModel = null;
        playerRb = null;
        playerTransform = null;
        playerCapsule = null;
    }
}
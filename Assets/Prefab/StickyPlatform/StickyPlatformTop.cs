using UnityEngine;

public class StickyPlatformTop : MonoBehaviour
{
    [Header("Links")]
    public Collider topCollider;
    public StickyPlatformBottom bottomScript;

    private void Awake()
    {
        if (topCollider == null) topCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleStyle(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryHandleStyle(collision.gameObject);
    }

    private void TryHandleStyle(GameObject playerObj)
    {
        if (!playerObj.CompareTag("Player")) return;

        PlayerStateModel model = playerObj.GetComponent<PlayerStateModel>();
        CapsuleCollider playerCapsule = playerObj.GetComponent<CapsuleCollider>();
        if (model == null || playerCapsule == null || topCollider == null) return;

        if (model.CurrentStyleIndex == 1) // Legs Ч проваливаемс€
        {
            Physics.IgnoreCollision(playerCapsule, topCollider, true);
            bottomScript?.ActivateSticky(playerObj);
        }
        else // Hands Ч обычна€ платформа
        {
            Physics.IgnoreCollision(playerCapsule, topCollider, false);
            bottomScript?.DeactivateSticky();
        }
    }

    public void RestoreCollision(CapsuleCollider playerCapsule)
    {
        if (playerCapsule != null && topCollider != null)
        {
            Physics.IgnoreCollision(playerCapsule, topCollider, false);
        }
    }
}
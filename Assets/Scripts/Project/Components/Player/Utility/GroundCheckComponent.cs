using UnityEngine;

/// <summary>
/// Отвечает исключительно за физическую проверку нахождения на земле
/// и обновление IsGrounded в PlayerStateModel.
/// </summary>
public class GroundCheckComponent : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;

    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundLayer;
    [Range(0f, 90f)]
    [SerializeField] float maxSlopeAngle = 45f; // Максимальный угол наклона поверхности, чтобы считаться землей
    private Vector3 currentGroundNormal;
    
    private Rigidbody rb;
    private bool isGrounded = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        CheckForGroundContact(collision, true);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckForGroundContact(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        currentGroundNormal = Vector3.up;
        playerStateModel.SetGroundNormal(Vector3.up);
    }
    
    private void FixedUpdate()
    {
        playerStateModel?.SetIsGrounded(isGrounded);
    }
    
    /// <summary>
    /// Проверяет, является ли точка контакта землей, и обновляет локальное состояние.
    /// </summary>
    private void CheckForGroundContact(Collision collision, bool isEntering)
    {
        if (playerStateModel == null) return;

        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        bool contactFound = false;
        Vector3 bestNormal = Vector3.up;

        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle < maxSlopeAngle)
            {
                contactFound = true;
                bestNormal = contact.normal;
                playerStateModel.SetLastWallJumpedFrom(null);
                break;
            }
        }

        if (contactFound)
        {
            isGrounded = true;
            currentGroundNormal = bestNormal;
            playerStateModel.SetGroundNormal(bestNormal);
        }
        else if (isEntering)
        {
            isGrounded = false;
            currentGroundNormal = Vector3.up;
            playerStateModel.SetGroundNormal(Vector3.up);
        }
    }
}

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

        // Проверка LayerMask
        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
        {
            return; // Не является слоем земли
        }

        bool contactFound = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            // Проверка угла наклона поверхности
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle < maxSlopeAngle)
            {
                contactFound = true;
                playerStateModel.SetLastWallJumpedFrom(null);
                break;
            }
        }
        
        if (isEntering)
        {
            isGrounded = contactFound;
        }
        else if (!contactFound)
        {
            isGrounded = false;
        }
    }
}
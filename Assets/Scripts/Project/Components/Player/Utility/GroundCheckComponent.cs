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
    public LayerMask groundLayer;
    [Range(0f, 90f)]
    public float maxSlopeAngle = 45f; // Максимальный угол наклона поверхности, чтобы считаться землей
    
    
    private Rigidbody rb;
    private bool isGrounded = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            enabled = false;
        }
    }
    
    // Используем OnCollisionEnter/Exit для физической проверки
    private void OnCollisionEnter(Collision collision)
    {
        CheckForGroundContact(collision, true);
    }

    private void OnCollisionStay(Collision collision)
    {
        // OnCollisionStay нужен для поддержания состояния isGrounded, 
        // пока мы стоим на склоне или движемся по земле.
        CheckForGroundContact(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Немедленно выходим из состояния "на земле" при отрыве, 
        // но с задержкой, чтобы избежать флуктуаций (см. FixedUpdate)
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
                break;
            }
        }
        
        // Обновляем локальное состояние, которое FixedUpdate запишет в модель
        if (isEntering)
        {
            isGrounded = contactFound;
        }
        else if (!contactFound)
        {
            // Если мы вышли из контакта, но не нашли другую точку контакта, то мы в воздухе
            isGrounded = false;
        }
    }
}
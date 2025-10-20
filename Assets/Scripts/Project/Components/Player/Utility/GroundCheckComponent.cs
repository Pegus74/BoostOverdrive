using UnityEngine;

/// <summary>
/// Отвечает исключительно за физическую проверку нахождения на земле
/// и обновление IsGrounded в PlayerStateModel.
/// </summary>
public class GroundCheckComponent : MonoBehaviour
{
    [Header("Model Link")]
    [Tooltip("Модель состояния игрока для записи IsGrounded")]
    public PlayerStateModel playerStateModel;

    [Header("Ground Check Settings")]
    [Tooltip("Маска слоев, которые считаются землей")]
    public LayerMask groundLayer;
    
    [Tooltip("Максимальный угол наклона поверхности, чтобы считаться землей")]
    [Range(0f, 90f)]
    public float maxSlopeAngle = 45f;
    
    
    private Rigidbody rb;
    private bool isGrounded = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("GroundCheckComponent требует Rigidbody на том же объекте!");
            enabled = false;
        }
        if (playerStateModel == null)
        {
            Debug.LogError("PlayerStateModel не прикреплена к GroundCheckComponent!");
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
        // ВНИМАНИЕ: Для более надежного определения лучше использовать Raycast в FixedUpdate.
        // Здесь мы просто временно обновляем локальное состояние.
        isGrounded = false; 
    }
    
    private void FixedUpdate()
    {
        // Это важный шаг: Мы используем локальное состояние 'isGrounded' (из Collision)
        // и записываем его в модель. Это гарантирует, что SetIsGrounded вызывается
        // только при фактическом изменении состояния.
        playerStateModel?.SetIsGrounded(isGrounded);

        // Если вы используете Raycast для более точного контроля:
        // isGrounded = PerformRaycastGroundCheck();
        // playerStateModel?.SetIsGrounded(isGrounded);
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
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

        // Автоматически находим игрока
        FindPlayer();

        Debug.Log($"SpringWall initialized. Activation distance: {settings.activationDistance}");
    }

    private void FindPlayer()
    {
        // Сначала ищем по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStateModel = player.GetComponent<PlayerStateModel>();
        }

        // Если не нашли по тегу, ищем в сцене
        if (playerStateModel == null)
        {
            playerStateModel = FindObjectOfType<PlayerStateModel>();
        }

        if (playerStateModel != null)
        {
            Debug.Log($"Player found: {playerStateModel.gameObject.name}");
        }
        else
        {
            Debug.LogError("Player not found! Make sure player has PlayerStateModel component.");
        }
    }

    private void Update()
    {
        if (playerStateModel == null)
        {
            // Периодически пытаемся найти игрока, если потеряли
            if (Time.frameCount % 60 == 0) // Каждую секунду
            {
                FindPlayer();
            }
            return;
        }

        // Проверяем расстояние до игрока
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (playerStateModel == null) return;

        float distance = Vector3.Distance(transform.position, playerStateModel.transform.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = (distance <= settings.activationDistance);

        // Логируем изменение состояния
        if (isPlayerInRange && !wasInRange)
        {
            Debug.Log($"Player entered range. Distance: {distance}");
        }
        else if (!isPlayerInRange && wasInRange)
        {
            Debug.Log($"Player left range. Distance: {distance}");
        }
    }

    // Основной метод для обнаружения прыжка от стены
    public bool TryDetectWallJump(out WallJumpData jumpData)
    {
        jumpData = new WallJumpData();

        if (!isPlayerInRange)
        {
            Debug.Log($"Player not in range. Distance: {GetPlayerDistance()}");
            return false;
        }

        if (playerStateModel == null)
        {
            Debug.Log("PlayerStateModel is null");
            return false;
        }

        if (playerStateModel.IsGrounded)
        {
            Debug.Log("Player is grounded - no wall jump");
            return false;
        }

        if (playerStateModel.LastWallJumpedFrom == this)
        {
            Debug.Log("Already jumped from this wall");
            return false;
        }

        // Используем OverlapSphere для обнаружения игрока рядом со стеной
        if (!IsPlayerNearWall())
        {
            Debug.Log("Player not near wall surface");
            return false;
        }

        Vector3 surfaceNormal = CalculateWallNormal();
        if (surfaceNormal == Vector3.zero)
        {
            Debug.Log("Could not determine wall normal");
            return false;
        }

        jumpData = new WallJumpData
        {
            surfaceNormal = surfaceNormal,
            wallComponent = this,
            styleIndex = playerStateModel.CurrentStyleIndex
        };

        Debug.Log($"Wall jump detected! Style: {playerStateModel.CurrentStyleIndex}");
        return true;
    }

    private bool IsPlayerNearWall()
    {
        if (playerStateModel == null) return false;

        // Используем OverlapSphere для проверки наличия игрока в зоне стены
        Collider[] hitColliders = Physics.OverlapSphere(
            playerStateModel.transform.position,
            settings.activationDistance * 0.5f, // Более маленький радиус для точности
            settings.playerLayer
        );

        foreach (var collider in hitColliders)
        {
            if (collider == wallCollider)
            {
                return true;
            }
        }

        // Дополнительная проверка - расстояние до ближайшей точки коллайдера
        Vector3 closestPoint = wallCollider.ClosestPoint(playerStateModel.transform.position);
        float distanceToWall = Vector3.Distance(playerStateModel.transform.position, closestPoint);

        return distanceToWall <= settings.activationDistance;
    }

    private Vector3 CalculateWallNormal()
    {
        if (playerStateModel == null || wallCollider == null)
            return Vector3.zero;

        Vector3 playerPosition = playerStateModel.transform.position;
        Vector3 closestPoint = wallCollider.ClosestPoint(playerPosition);

        // Вычисляем нормаль от ближайшей точки к игроку
        Vector3 normal = (playerPosition - closestPoint).normalized;

        // Для плоскостей можно использовать более точное определение
        if (wallCollider is BoxCollider || wallCollider is MeshCollider)
        {
            // Пытаемся получить нормаль через Raycast
            RaycastHit hit;
            Vector3 rayDirection = (closestPoint - playerPosition).normalized;

            if (Physics.Raycast(playerPosition, rayDirection, out hit, settings.activationDistance * 2f))
            {
                if (hit.collider == wallCollider)
                {
                    Debug.Log($"Using raycast normal: {hit.normal}");
                    return hit.normal;
                }
            }
        }

        Debug.Log($"Using calculated normal: {normal}");
        return normal;
    }

    private float GetPlayerDistance()
    {
        if (playerStateModel == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerStateModel.transform.position);
    }

    // Корутина для модификатора скорости
    public IEnumerator ApplySpeedModifierCoroutine(PlayerStateModel model)
    {
        if (model == null) yield break;

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

        // Зона активации
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.activationDistance);

        // Зона обнаружения стены (меньшая)
        if (playerStateModel != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerStateModel.transform.position, settings.activationDistance * 0.5f);
        }

        // Визуализация нормали
        if (playerStateModel != null && wallCollider != null)
        {
            Vector3 closestPoint = wallCollider.ClosestPoint(playerStateModel.transform.position);
            Vector3 normal = CalculateWallNormal();

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(closestPoint, closestPoint + normal * 2f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerStateModel.transform.position, closestPoint);
        }
    }

    private void OnDrawGizmos()
    {
        if (settings == null || playerStateModel == null) return;

        // Всегда показываем связь со игроком когда в зоне
        float distance = GetPlayerDistance();
        if (distance <= settings.activationDistance)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, playerStateModel.transform.position);
        }
    }
}
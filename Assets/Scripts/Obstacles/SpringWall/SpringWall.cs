using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: проверка на ближайший

public class SpringWall : MonoBehaviour
{
    [Header("Настройки Стены")]
    public float activationDistance = 1.0f;
    public LayerMask playerLayer;
    public float raycastCheckDistance = 1.0f;

    [Header("Настройки отталкивания")]
    public float reboundForceLegs = 15f;
    public float reboundForceHands = 20f;
    public float extraAccelerationHands = 5f;
    public float verticalBounceOn90Degree = 2.0f;
    public float cameraRotationDuration = 0.3f;
    public float fixedVerticalReboundLegs = 12f;


    private const int LEGS_STYLE_INDEX = 0;
    private const int HANDS_STYLE_INDEX = 1;

    private FirstPersonController playerController;
    private StyleController styleController;
    private KeyCode jumpKey;
    private Collider wallCollider;

    private void Awake()
    {
        wallCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<FirstPersonController>();
        styleController = playerController?.GetComponent<StyleController>();
        jumpKey = playerController.GetJumpKey();
    }

    private void Update()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            Debug.Log("-----------------------------------------------------");

            Vector3 surfaceNormal;
            int currentStyleIndex = styleController.GetCurrentStyleIndex();

            Vector3[] checkDirections;
            if (currentStyleIndex == HANDS_STYLE_INDEX)
            {
                // Стиль РУК: Raycast вперед + Raycast по скорости для Wall Climb
                Vector3 forward = playerController.GetForwardVector();
                forward.y = 0; forward.Normalize();

                // Добавляем Raycast по вектору скорости, если игрок движется достаточно быстро
                Vector3 velocityDirection = playerController.GetCurrentHorizontalVelocity().normalized;
                float currentSpeed = playerController.GetCurrentHorizontalVelocity().magnitude;

                // Проверка, движется ли игрок быстрее половины скорости ходьбы
                if (currentSpeed > playerController.walkSpeed * 0.5f) // Предполагаем walkSpeed доступен
                {
                    // Если игрок движется, проверяем и по направлению взгляда, и по направлению движения
                    checkDirections = new Vector3[] { forward, velocityDirection };
                    Debug.Log($"Стиль РУК: Используется Raycast вперед и по направлению скорости ({velocityDirection}) для Wall Climb.");
                }
                else
                {
                    // Иначе, только вперед
                    checkDirections = new Vector3[] { forward };
                    Debug.Log("Стиль РУК: Используется одиночный Raycast вперед.");
                }
            }
            else // LEGS_STYLE_INDEX
            {
                // Стиль НОГ: Множественные Raycast'ы для гибкого обнаружения с боков.
                Vector3 forward = playerController.GetForwardVector();
                forward.y = 0; forward.Normalize();
                Vector3 right = playerController.transform.right;
                right.y = 0; right.Normalize();

                checkDirections = new Vector3[]
                {
                    forward,
                    -forward,
                    right,
                    -right
                };
                Debug.Log("Стиль НОГ: Используется 4 Raycast'а для гибкости.");
            }

            if (TryCheckWallContact(checkDirections, out surfaceNormal))
            {
                Debug.Log($"КОНТАКТ");
                HandleRebound(surfaceNormal);
            }
            else
            {
                Debug.Log("НЕТ КОНТАКТА");
            }
        }
    }

    private bool TryCheckWallContact(Vector3[] directions, out Vector3 normal)
    {
        normal = Vector3.up;
        Vector3 rayStart = playerController.transform.position;

        // Проверка Raycast'ами по заданным направлениям
        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            // Raycast от позиции игрока в одном из заданных направлений
            if (Physics.Raycast(rayStart, direction, out hit, raycastCheckDistance))
            {
                if (hit.collider == wallCollider)
                {
                    normal = hit.normal;
                    if (hit.distance <= activationDistance)
                    {
                        return true;
                    }
                }
            }
        }

        // Проверка по ближайшей точке
        Vector3 closestPointToWall = wallCollider.ClosestPoint(rayStart);
        float distanceToWall = Vector3.Distance(rayStart, closestPointToWall);

        if (distanceToWall <= activationDistance + 0.05f)
        {
            // Используем нормаль от ближайшей точки
            normal = (rayStart - closestPointToWall).normalized;
            normal.y = 0;
            normal.Normalize();
            return true;
        }
        return false;
    }

    private void HandleRebound(Vector3 surfaceNormal)
    {
        Vector3 approachVector = playerController.GetCurrentHorizontalVelocity();

        if (playerController.IsGrounded())
        {
            Debug.Log("Игрок стоит на месте. Выполняется обычный прыжок.");
            return;
        }

        int currentStyleIndex = styleController.GetCurrentStyleIndex();

        Vector3 reboundVector = Vector3.zero;
        float reboundForce = 0f;
        bool specialVerticalCaseTriggered = false;

        #region HANDS
        if (currentStyleIndex == HANDS_STYLE_INDEX)
        {
            // Вектор приближения (скорость игрока)
            Vector3 V_approach_norm = approachVector;
            V_approach_norm.y = 0;
            V_approach_norm.Normalize();

            // Расчет угла приближения: угол между V_пад и вектором от стены к игроку (-surfaceNormal).
            float angle = Vector3.Angle(V_approach_norm, -surfaceNormal);
            Debug.Log($"Угол между V_пад и -N (приближение): {angle:F2} градусов");

            bool isSpecialCase = (angle <= 15f || angle >= 165f || (angle >= 75f && angle <= 105f));

            if (isSpecialCase)
            {
                // отскок строго назад + подброс
                reboundVector = surfaceNormal;
                reboundForce = reboundForceHands;

                reboundVector += Vector3.up * verticalBounceOn90Degree;
                reboundVector.Normalize();
                specialVerticalCaseTriggered = true;
                Debug.Log($"Срабатывание '90-градусного отскока");
            }
            else
            {
                reboundVector = Vector3.Reflect(V_approach_norm, surfaceNormal);
                reboundForce = reboundForceHands;

                reboundVector.y = 0;
                reboundVector.Normalize();
                Debug.Log($"Срабатывание стандартного отскока");
            }

            // 2. Применяем импульс и ускорение
            Vector3 impulse = reboundVector * reboundForce;
            impulse += reboundVector * extraAccelerationHands; // Дополнительное ускорение

            playerController.SetExternalImpulse(impulse);
            Debug.Log($"Финальный импульс (РУКИ): {impulse}");

            // 3. Поворот камеры
            float targetYaw = Quaternion.LookRotation(reboundVector).eulerAngles.y;
            playerController.SmoothlyRotateCameraYaw(targetYaw, cameraRotationDuration);
            Debug.Log($"Поворот камеры: {targetYaw:F2} градусов.");
        }
        #endregion

        #region LEGS
        else if (currentStyleIndex == LEGS_STYLE_INDEX)
        {
            // 1. Определяем желаемое направление (по вектору камеры/взгляда)
            Vector3 jumpDirection = playerController.GetForwardVector();
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            // 2. Рассчитываем полный вектор импульса: Горизонтальный толчок + Вертикальный прыжок
            float horizontalForce = reboundForceLegs;
            float verticalForce = fixedVerticalReboundLegs;

            Vector3 finalImpulse = jumpDirection * horizontalForce + Vector3.up * verticalForce;

            // 3. ПРИМЕНЯЕМ ЕДИНЫЙ ИМПУЛЬС (ForceMode.Impulse)
            playerController.rb.AddForce(finalImpulse, ForceMode.Impulse);

            // Если игрок в воздухе, инициируем логику прыжка
            if (!(playerController.IsGrounded()))
            {
                playerController.InitiateJumpLogic();
            }

            playerController.SetExternalImpulse(Vector3.zero);

            Debug.Log($"Финальный импульс (НОГИ): {finalImpulse}");
            Debug.Log($"Горизонтальная сила: {horizontalForce}, Вертикальная сила: {verticalForce}");
        }
        #endregion

        if (specialVerticalCaseTriggered)
        {
            playerController.rb.AddForce(Vector3.up * playerController.jumpPower * 0.5f, ForceMode.Impulse);
            Debug.Log($"Добавлен дополнительный вертикальный импульс: {playerController.jumpPower * 0.5f}");
        }

        Debug.Log("-----------------------------------------------------");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (playerController != null)
        {
            Vector3 rayStart = playerController.transform.position;
            Vector3 forward = playerController.GetForwardVector();
            forward.y = 0; forward.Normalize();
            Vector3 right = playerController.transform.right;
            right.y = 0; right.Normalize();

            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayStart, forward * raycastCheckDistance);
            Gizmos.DrawRay(rayStart, -forward * raycastCheckDistance);
            Gizmos.DrawRay(rayStart, right * raycastCheckDistance);
            Gizmos.DrawRay(rayStart, -right * raycastCheckDistance);

            Vector3 closestPoint = wallCollider.ClosestPoint(rayStart);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(closestPoint, activationDistance);
        }
    }
}
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
        if (playerController.IsGrounded())
        {
            playerController.ClearLastWallJumpedFrom();
        }

        if (Input.GetKeyDown(jumpKey))
        {
            //Debug.Log("-----------------------------------------------------");

            if (playerController.LastWallJumpedFrom == this)
                return;

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
                    // Debug.Log($"Стиль РУК: Используется Raycast вперед и по направлению скорости ({velocityDirection}) для Wall Climb.");
                }
                else
                {
                    // Иначе, только вперед
                    checkDirections = new Vector3[] { forward };
                    // Debug.Log("Стиль РУК: Используется одиночный Raycast вперед.");
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
                //Debug.Log("Стиль НОГ: Используется 4 Raycast'а для гибкости.");
            }

            if (TryCheckWallContact(checkDirections, out surfaceNormal))
            {
                HandleRebound(surfaceNormal);
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
            //Debug.Log("Игрок стоит на месте. Выполняется обычный прыжок.");
            return;
        }

        playerController.SetLastWallJumpedFrom(this);

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
            //Debug.Log($"Угол между V_пад и -N (приближение): {angle:F2} градусов");

            bool isSpecialCase = (angle <= 15f || angle >= 165f || (angle >= 75f && angle <= 105f));

            if (isSpecialCase)
            {
                // отскок строго назад + подброс
                reboundVector = surfaceNormal;
                reboundForce = playerController.reboundForceHands;

                reboundVector += Vector3.up;
                reboundVector.Normalize();
                specialVerticalCaseTriggered = true;
                // Debug.Log($"Срабатывание '90-градусного отскока");
            }
            else
            {
                reboundVector = Vector3.Reflect(V_approach_norm, surfaceNormal);
                reboundForce = playerController.reboundForceHands;

                reboundVector.y = 0;
                reboundVector.Normalize();
                // Debug.Log($"Срабатывание стандартного отскока");
            }

            // 2. Применяем импульс и ускорение
            Vector3 impulse = reboundVector * reboundForce;
            impulse += reboundVector * playerController.extraAccelerationHands; // Дополнительное ускорение

            playerController.SetExternalImpulse(impulse);
            // Debug.Log($"Финальный импульс (РУКИ): {impulse}");

            if (Input.GetKey(playerController.cameraRotateKey))
            {
                // 3. Поворот камеры
                float targetYaw = Quaternion.LookRotation(reboundVector).eulerAngles.y;
                playerController.SmoothlyRotateCameraYaw(targetYaw, playerController.cameraRotationDuration);
                // Debug.Log($"Поворот камеры: {targetYaw:F2} градусов.");
            }
        }
        #endregion

        #region LEGS
        else if (currentStyleIndex == LEGS_STYLE_INDEX)
        {
            Vector3 jumpDirection = playerController.GetForwardVector();
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            float horizontalForce = playerController.horizontalForceLegs;
            float verticalForce = playerController.verticalForceLegs;

            Vector3 finalImpulse = jumpDirection * horizontalForce + Vector3.up * verticalForce;

            playerController.rb.AddForce(finalImpulse, ForceMode.Impulse);

            playerController.SetExternalImpulse(Vector3.zero);
        }
        #endregion

        if (specialVerticalCaseTriggered)
        {
            playerController.rb.AddForce(Vector3.up * playerController.jumpPower * 0.5f, ForceMode.Impulse);
        }

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
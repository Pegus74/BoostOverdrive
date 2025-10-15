using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: �������� �� ���������

public class SpringWall : MonoBehaviour
{
    [Header("��������� �����")]
    public float activationDistance = 1.0f;
    public LayerMask playerLayer;
    public float raycastCheckDistance = 1.0f;

    [Header("��������� ������������")]
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
                // ����� ���: Raycast ������ + Raycast �� �������� ��� Wall Climb
                Vector3 forward = playerController.GetForwardVector();
                forward.y = 0; forward.Normalize();

                // ��������� Raycast �� ������� ��������, ���� ����� �������� ���������� ������
                Vector3 velocityDirection = playerController.GetCurrentHorizontalVelocity().normalized;
                float currentSpeed = playerController.GetCurrentHorizontalVelocity().magnitude;

                // ��������, �������� �� ����� ������� �������� �������� ������
                if (currentSpeed > playerController.walkSpeed * 0.5f) // ������������ walkSpeed ��������
                {
                    // ���� ����� ��������, ��������� � �� ����������� �������, � �� ����������� ��������
                    checkDirections = new Vector3[] { forward, velocityDirection };
                    Debug.Log($"����� ���: ������������ Raycast ������ � �� ����������� �������� ({velocityDirection}) ��� Wall Climb.");
                }
                else
                {
                    // �����, ������ ������
                    checkDirections = new Vector3[] { forward };
                    Debug.Log("����� ���: ������������ ��������� Raycast ������.");
                }
            }
            else // LEGS_STYLE_INDEX
            {
                // ����� ���: ������������� Raycast'� ��� ������� ����������� � �����.
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
                Debug.Log("����� ���: ������������ 4 Raycast'� ��� ��������.");
            }

            if (TryCheckWallContact(checkDirections, out surfaceNormal))
            {
                Debug.Log($"�������");
                HandleRebound(surfaceNormal);
            }
            else
            {
                Debug.Log("��� ��������");
            }
        }
    }

    private bool TryCheckWallContact(Vector3[] directions, out Vector3 normal)
    {
        normal = Vector3.up;
        Vector3 rayStart = playerController.transform.position;

        // �������� Raycast'��� �� �������� ������������
        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            // Raycast �� ������� ������ � ����� �� �������� �����������
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

        // �������� �� ��������� �����
        Vector3 closestPointToWall = wallCollider.ClosestPoint(rayStart);
        float distanceToWall = Vector3.Distance(rayStart, closestPointToWall);

        if (distanceToWall <= activationDistance + 0.05f)
        {
            // ���������� ������� �� ��������� �����
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
            Debug.Log("����� ����� �� �����. ����������� ������� ������.");
            return;
        }

        int currentStyleIndex = styleController.GetCurrentStyleIndex();

        Vector3 reboundVector = Vector3.zero;
        float reboundForce = 0f;
        bool specialVerticalCaseTriggered = false;

        #region HANDS
        if (currentStyleIndex == HANDS_STYLE_INDEX)
        {
            // ������ ����������� (�������� ������)
            Vector3 V_approach_norm = approachVector;
            V_approach_norm.y = 0;
            V_approach_norm.Normalize();

            // ������ ���� �����������: ���� ����� V_��� � �������� �� ����� � ������ (-surfaceNormal).
            float angle = Vector3.Angle(V_approach_norm, -surfaceNormal);
            Debug.Log($"���� ����� V_��� � -N (�����������): {angle:F2} ��������");

            bool isSpecialCase = (angle <= 15f || angle >= 165f || (angle >= 75f && angle <= 105f));

            if (isSpecialCase)
            {
                // ������ ������ ����� + �������
                reboundVector = surfaceNormal;
                reboundForce = reboundForceHands;

                reboundVector += Vector3.up * verticalBounceOn90Degree;
                reboundVector.Normalize();
                specialVerticalCaseTriggered = true;
                Debug.Log($"������������ '90-���������� �������");
            }
            else
            {
                reboundVector = Vector3.Reflect(V_approach_norm, surfaceNormal);
                reboundForce = reboundForceHands;

                reboundVector.y = 0;
                reboundVector.Normalize();
                Debug.Log($"������������ ������������ �������");
            }

            // 2. ��������� ������� � ���������
            Vector3 impulse = reboundVector * reboundForce;
            impulse += reboundVector * extraAccelerationHands; // �������������� ���������

            playerController.SetExternalImpulse(impulse);
            Debug.Log($"��������� ������� (����): {impulse}");

            // 3. ������� ������
            float targetYaw = Quaternion.LookRotation(reboundVector).eulerAngles.y;
            playerController.SmoothlyRotateCameraYaw(targetYaw, cameraRotationDuration);
            Debug.Log($"������� ������: {targetYaw:F2} ��������.");
        }
        #endregion

        #region LEGS
        else if (currentStyleIndex == LEGS_STYLE_INDEX)
        {
            // 1. ���������� �������� ����������� (�� ������� ������/�������)
            Vector3 jumpDirection = playerController.GetForwardVector();
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            // 2. ������������ ������ ������ ��������: �������������� ������ + ������������ ������
            float horizontalForce = reboundForceLegs;
            float verticalForce = fixedVerticalReboundLegs;

            Vector3 finalImpulse = jumpDirection * horizontalForce + Vector3.up * verticalForce;

            // 3. ��������� ������ ������� (ForceMode.Impulse)
            playerController.rb.AddForce(finalImpulse, ForceMode.Impulse);

            // ���� ����� � �������, ���������� ������ ������
            if (!(playerController.IsGrounded()))
            {
                playerController.InitiateJumpLogic();
            }

            playerController.SetExternalImpulse(Vector3.zero);

            Debug.Log($"��������� ������� (����): {finalImpulse}");
            Debug.Log($"�������������� ����: {horizontalForce}, ������������ ����: {verticalForce}");
        }
        #endregion

        if (specialVerticalCaseTriggered)
        {
            playerController.rb.AddForce(Vector3.up * playerController.jumpPower * 0.5f, ForceMode.Impulse);
            Debug.Log($"�������� �������������� ������������ �������: {playerController.jumpPower * 0.5f}");
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
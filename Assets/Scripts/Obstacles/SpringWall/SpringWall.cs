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

    private const int LEGS_STYLE_INDEX = 0;
    private const int HANDS_STYLE_INDEX = 1;

    private FirstPersonController playerController;
    public MetricsMaster mm;
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
            //Debug.Log("����� ���: ������������ 4 Raycast'� ��� ��������.");

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
            //Debug.Log("����� ����� �� �����. ����������� ������� ������.");
            return;
        }

        playerController.SetLastWallJumpedFrom(this);

        int currentStyleIndex = styleController.GetCurrentStyleIndex();
        
        float reboundForce = 0f;
        bool specialVerticalCaseTriggered = false;

        #region HANDS
        if (currentStyleIndex == HANDS_STYLE_INDEX)
        {
            Vector3 jumpDirection = playerController.GetForwardVector();
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            float horizontalForce = mm.horizontalForceHands;
            float verticalForce = mm.verticalForceHands;

            Vector3 finalImpulse = jumpDirection * horizontalForce + Vector3.up * verticalForce;

            playerController.rb.AddForce(finalImpulse, ForceMode.Impulse);

            playerController.SetExternalImpulse(Vector3.zero);
        }
        #endregion

        #region LEGS
        else if (currentStyleIndex == LEGS_STYLE_INDEX)
        {
            Vector3 jumpDirection = playerController.GetForwardVector();
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            float horizontalForce = mm.horizontalForceLegs;
            float verticalForce = mm.verticalForceLegs;

            Vector3 finalImpulse = jumpDirection * horizontalForce + Vector3.up * verticalForce;

            playerController.rb.AddForce(finalImpulse, ForceMode.Impulse);

            playerController.SetExternalImpulse(Vector3.zero);
        }
        #endregion
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
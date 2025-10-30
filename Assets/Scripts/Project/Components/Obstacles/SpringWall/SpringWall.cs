using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RSpringWall : MonoBehaviour
{
    [Header("Model & Settings")]
    public PlayerStateModel playerStateModel; 
    
    [Header("Events")]
    public WallJumpEvent OnWallJumpDetectedEvent;
    public GameEvent JumpAttemptEvent;
    
    [Header("Настройки Стены")]
    private float activationDistance = 2.0f;
    private float raycastCheckDistance = 1.0f;

    private const int LEGS_STYLE_INDEX = 0;
    private const int HANDS_STYLE_INDEX = 1;

    private Collider wallCollider;
    private Transform playerTransform;
    private LayerMask playerLayer;

    private void Awake()
    {
        wallCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        PlayerMovementController controller = FindAnyObjectByType<PlayerMovementController>();
        playerTransform = controller?.transform;
        
        if (playerStateModel == null || playerTransform == null || OnWallJumpDetectedEvent == null || JumpAttemptEvent == null)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        JumpAttemptEvent?.RegisterListener(CheckForWallJump);
    }

    private void OnDisable()
    {
        JumpAttemptEvent?.UnregisterListener(CheckForWallJump);
    }

    /// <summary>
    /// Вызывается при каждой попытке прыжка игроком.
    /// </summary>
    private void CheckForWallJump()
    {
        if (playerStateModel.IsGrounded)
            return;

        Vector3 surfaceNormal;
        int currentStyleIndex = playerStateModel.CurrentStyleIndex;

        Vector3[] checkDirections = GetCheckDirections(currentStyleIndex);
        
        if (TryCheckWallContact(checkDirections, out surfaceNormal))
        {
            WallJumpData data = new WallJumpData(surfaceNormal, this);
            OnWallJumpDetectedEvent.Raise(data);
        }
    }
    
    private Vector3[] GetCheckDirections(int styleIndex)
    {
        Vector3 forward = playerTransform.forward;
        forward.y = 0; forward.Normalize();
        Vector3 right = playerTransform.right;
        right.y = 0; right.Normalize();

        if (styleIndex == HANDS_STYLE_INDEX)
        {
            return new Vector3[] { forward };
        }
        else // LEGS_STYLE_INDEX
        {
            return new Vector3[]
            {
                forward,
                -forward,
                right,
                -right
            };
        }
    }
    
    private bool TryCheckWallContact(Vector3[] directions, out Vector3 normal)
    {
        normal = Vector3.up;
        Vector3 rayStart = playerTransform.position;

        // Проверка Raycast'ами
        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayStart, direction, out hit, raycastCheckDistance, playerLayer))
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
            normal = (rayStart - closestPointToWall).normalized;
            normal.y = 0;
            normal.Normalize();
            return true;
        }
        return false;
    }
}
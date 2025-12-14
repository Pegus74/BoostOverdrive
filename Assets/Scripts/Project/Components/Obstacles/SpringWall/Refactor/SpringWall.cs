using UnityEngine;           
using System.Collections;    
using UnityEngine.Events;

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
        FindPlayer();
        Debug.Log($"SpringWall initialized. Activation distance: {settings.activationDistance}");
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStateModel = player.GetComponent<PlayerStateModel>();
        }

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
            if (Time.frameCount % 60 == 0) 
            {
                FindPlayer();
            }
            return;
        }
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        if (playerStateModel == null) return;

        float distance = Vector3.Distance(transform.position, playerStateModel.transform.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = (distance <= settings.activationDistance);

        if (isPlayerInRange && !wasInRange)
        {
            Debug.Log($"Player entered range. Distance: {distance}");
        }
        else if (!isPlayerInRange && wasInRange)
        {
            Debug.Log($"Player left range. Distance: {distance}");
        }
    }

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
            Debug.Log("Already jumped from this wall (need to touch ground first)");
            return false;
        }

 
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

        return (playerPosition - closestPoint).normalized;
    }

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

    private float GetPlayerDistance()
    {
        if (playerStateModel == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerStateModel.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.activationDistance);
    }
}

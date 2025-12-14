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
    private Rigidbody playerRb;

    private void Awake()
    {
        wallCollider = GetComponent<Collider>();
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStateModel = player.GetComponent<PlayerStateModel>();
            playerRb = player.GetComponent<Rigidbody>();
        }

        if (playerStateModel == null)
        {
            playerStateModel = FindObjectOfType<PlayerStateModel>();
            if (playerStateModel != null)
            {
                playerRb = playerStateModel.GetComponent<Rigidbody>();
            }
        }
    }


    public bool TryDetectWallJump(out WallJumpData jumpData)
    {
        jumpData = new WallJumpData();

        if (playerStateModel == null || playerRb == null)
        {
            FindPlayer();
            return false;
        }

   
        if (playerStateModel.IsGrounded)
            return false;

        if (playerStateModel.LastWallJumpedFrom == this)
            return false;

        if (!IsPlayerTouchingThisWall())
            return false;

        Vector3 surfaceNormal = CalculateWallNormal();

        jumpData = new WallJumpData
        {
            surfaceNormal = surfaceNormal,
            wallComponent = this,
            styleIndex = playerStateModel.CurrentStyleIndex
        };

        return true;
    }

  
    private bool IsPlayerTouchingThisWall()
    {
        if (playerStateModel == null || wallCollider == null)
            return false;

        Vector3 playerPos = playerStateModel.transform.position;

      
        Vector3 closestPoint = wallCollider.ClosestPoint(playerPos);
        float distanceToWall = Vector3.Distance(playerPos, closestPoint);

        
        if (distanceToWall <= 1f)
            return true;

        
        Vector3[] checkDirections = GetCheckDirections();
        foreach (Vector3 direction in checkDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerPos, direction, out hit, 0.2f))
            {
                if (hit.collider == wallCollider)
                    return true;
            }
        }

        return false;
    }

 
    private Vector3[] GetCheckDirections()
    {
        if (playerStateModel == null)
            return new Vector3[0];

        Vector3 forward = playerStateModel.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerStateModel.transform.right;
        right.y = 0;
        right.Normalize();

        return new Vector3[]
        {
            forward,
            -forward,
            right,
            -right
        };
    }

    private Vector3 CalculateWallNormal()
    {
        if (playerStateModel == null || wallCollider == null)
            return Vector3.zero;

        Vector3 playerPos = playerStateModel.transform.position;
        Vector3 closestPoint = wallCollider.ClosestPoint(playerPos);

        return (playerPos - closestPoint).normalized;
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

   
    private void OnDrawGizmosSelected()
    {
        if (wallCollider == null) return;

        Gizmos.color = Color.yellow;
        if (wallCollider is BoxCollider box)
        {
            Gizmos.DrawWireCube(transform.position + box.center, box.size);
        }

     
        if (playerStateModel != null)
        {
            Gizmos.color = Color.red;
            Vector3[] directions = GetCheckDirections();
            foreach (Vector3 dir in directions)
            {
                Gizmos.DrawRay(playerStateModel.transform.position, dir * 0.2f);
            }
        }
    }
}
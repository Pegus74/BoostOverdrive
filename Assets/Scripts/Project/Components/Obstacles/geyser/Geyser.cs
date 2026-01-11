using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Geyser : MonoBehaviour
{
    [Header("Settings")]
    public GeyserSettings settings;

    [Header("Activation")]
    public RDestructibleWall destructibleWall;
   
    private bool isActive = false;

    private PlayerStateModel playerStateModel;
    private Rigidbody playerRb;

    private void Awake()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (!isActive && destructibleWall != null && destructibleWall.IsDestroyed)
        {
            isActive = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive || playerStateModel == null || playerRb == null)
        {
            if (playerStateModel == null || playerRb == null)
                FindPlayer();
            return;
        }

        if (!IsPlayerInGeyserZone())
            return;
        Vector3 toPlayer = playerStateModel.transform.position - transform.position;
        float distanceAlongAxis = Vector3.Dot(toPlayer, transform.forward);
        float pushMultiplier = Mathf.Clamp01(1f - (distanceAlongAxis / settings.pyramidHeight));

        Vector3 pushDirection = transform.forward.normalized * settings.pushForce * pushMultiplier;
        playerRb.AddForce(pushDirection, ForceMode.Acceleration);
    }

    private bool IsPlayerInGeyserZone()
    {
        Vector3 toPlayer = playerStateModel.transform.position - transform.position;
        float distanceAlongAxis = Vector3.Dot(toPlayer, transform.forward);
        if (distanceAlongAxis <= 0f || distanceAlongAxis >= settings.pyramidHeight)
            return false;
        float maxRadius = (distanceAlongAxis / settings.pyramidHeight) * settings.baseRadius;
        Vector3 perpendicularOffset = toPlayer - (distanceAlongAxis * transform.forward.normalized);
        float perpDistance = perpendicularOffset.magnitude;

        return perpDistance <= maxRadius;
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

    private void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        Gizmos.color = isActive ? Color.cyan : Color.gray;
        DrawPyramidGizmo(transform.position, transform.forward, settings.pyramidHeight, settings.baseRadius);


        Gizmos.color = Color.white;
        Vector3 endPoint = transform.position + transform.forward * settings.pyramidHeight;
        Gizmos.DrawLine(transform.position, endPoint);
    }

    private void DrawPyramidGizmo(Vector3 apex, Vector3 axisDir, float height, float radius)
    {
        axisDir = axisDir.normalized;
        Vector3 baseCenter = apex + axisDir * height;

        int sides = 16;
        Vector3[] basePoints = new Vector3[sides];

        Quaternion baseRot = Quaternion.LookRotation(axisDir);
        for (int i = 0; i < sides; i++)
        {
            float angle = i * 2f * Mathf.PI / sides;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            basePoints[i] = baseCenter + baseRot * offset;
        }
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            Gizmos.DrawLine(basePoints[i], basePoints[next]);
        }


        foreach (Vector3 point in basePoints)
        {
            Gizmos.DrawLine(apex, point);
        }
    }
}
using UnityEngine;

public class GroundCheckComponent : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;

    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundLayer;
    [Range(0f, 90f)]
    [SerializeField] float maxSlopeAngle = 45f;

    private bool isGrounded = false;
    private Vector3 lastValidNormal = Vector3.up;
    
    private bool hasValidGroundContactThisStep;

    private void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
            enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateGroundContact(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateGroundContact(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            TryResetGroundedState();
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = hasValidGroundContactThisStep;

        playerStateModel.SetIsGrounded(isGrounded);
        playerStateModel.SetGroundNormal(isGrounded ? lastValidNormal : Vector3.up);

        if (isGrounded && !wasGrounded)
        {
            playerStateModel.UpdateCoyoteTime(0.15f);
        }

        hasValidGroundContactThisStep = false;
    }


    private void EvaluateGroundContact(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= maxSlopeAngle)
            {
                hasValidGroundContactThisStep = true;
                lastValidNormal = contact.normal;
                playerStateModel.SetLastWallJumpedFrom(null);
                return;
            }
        }
    }


    private void TryResetGroundedState()
    {
        isGrounded = false;
        lastValidNormal = Vector3.up;
    }
}
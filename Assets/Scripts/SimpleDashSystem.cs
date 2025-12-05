using UnityEngine;
using System.Collections;

public class SimpleDashSystem : MonoBehaviour
{
    [Header("Basic Settings")]
    public float dashPower = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Surface Settings")]
    [SerializeField] private float maxVerticalForce = 2f;
    [SerializeField] private float slopeVerticalFactor = 0.03f;

    private Rigidbody rb;
    private bool canDash = true;
    private Coroutine dashCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        dashCoroutine = StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        canDash = false;

        Debug.Log("=== SIMPLE DASH START ===");
        Vector3 startVelocity = rb.linearVelocity;
        Debug.Log($"Start velocity: {startVelocity}, Y={startVelocity.y:F2}");

        // �������� �����������
        Vector3 surfaceNormal = GetSurfaceNormal();
        float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);
        Debug.Log($"Surface: Normal={surfaceNormal}, Angle={slopeAngle:F1}�");

        // ������� ����������� - ��������������
        Vector3 dashDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        // ������������ ���� (�����������)
        float verticalForce = 0f;
        if (slopeAngle > 5f && slopeAngle < 45f)
        {
            // ����������, � ���� ��� ��� ����
            float dot = Vector3.Dot(dashDirection, surfaceNormal);
            if (dot < -0.1f) // � ���� (������ �������)
            {
                verticalForce = Mathf.Min(slopeAngle * slopeVerticalFactor, maxVerticalForce);
                Debug.Log($"Uphill: verticalForce={verticalForce:F2}");
            }
            else if (dot > 0.1f) // ��� ���� (�� �������)
            {
                verticalForce = -Mathf.Min(slopeAngle * slopeVerticalFactor * 0.5f, maxVerticalForce * 0.5f);
                Debug.Log($"Downhill: verticalForce={verticalForce:F2}");
            }
        }

        // ��������� ���� ����
        Vector3 finalDashForce = (dashDirection * dashPower) + (Vector3.up * verticalForce);

        Debug.Log($"Dash force: Horizontal={dashPower:F2}, Vertical={verticalForce:F2}");

        // ��������� �������
        rb.AddForce(finalDashForce, ForceMode.Impulse);

        // ���� ������������ ����
        float timer = dashDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // �������� ��������
            if (Time.frameCount % 5 == 0)
            {
                Debug.Log($"Dash progress: Velocity={rb.linearVelocity}, Y={rb.linearVelocity.y:F2}");
            }

            yield return null;
        }

        Debug.Log($"=== SIMPLE DASH END ===");
        Debug.Log($"Final velocity: Y={rb.linearVelocity.y:F2}");

        // �������
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    Vector3 GetSurfaceNormal()
    {
        // ������� Raycast ����
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f))
        {
            return hit.normal;
        }
        return Vector3.up;
    }
}

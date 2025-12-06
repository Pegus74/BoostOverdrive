using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public List<GameObject> wallParts = new List<GameObject>();
    private List<WallPartData> initPartData = new List<WallPartData>();
    public Collider wallCollider;
    public float explosionForce = 10f;
    public float explosionRadius = 5f;
    public float upwardModifier = 1f;
    public bool isDestroyed = false;
    public float debrisLifetime = 5f;
    public bool useGravity = true;
    public float enableColliderDelay = 0.5f;

    [System.Serializable]
    private class WallPartData
    {
        public GameObject part;
        public Vector3 initPosition;
        public Quaternion rotation;
        public Vector3 initScale;
        public Collider partCollider;
    }

    void Start()
    {
        if (wallCollider == null)
        {
            wallCollider = GetComponent<BoxCollider>();
            if (wallCollider == null)
            {
                wallCollider = GetComponent<Collider>();
            }
        }

        if (wallParts.Count == 0)
        {
            foreach (Transform child in transform)
            {
                wallParts.Add(child.gameObject);
            }
        }

        if (wallParts.Count > 0 && initPartData.Count == 0)
        {
            foreach (GameObject part in wallParts)
            {
                WallPartData data = new WallPartData
                {
                    part = part,
                    initPosition = part.transform.localPosition,
                    rotation = part.transform.localRotation,
                    initScale = part.transform.localScale,
                    partCollider = part.GetComponent<Collider>()
                };
                initPartData.Add(data);
            }
        }

        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }
        ResetWall();
    }

    void Update()
    {
        if (isDestroyed && Input.GetKeyDown(KeyCode.R))
        {
            ResetWall();
        }
    }

    private void ResetWall()
    {
        isDestroyed = false;
        StopAllCoroutines();

        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }

        foreach (WallPartData data in initPartData)
        {
            GameObject part = data.part;
            if (part != null)
            {
                if (!part.activeSelf) part.SetActive(true);

                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                if (data.partCollider != null)
                    data.partCollider.enabled = true;

                part.transform.localPosition = data.initPosition;
                part.transform.localRotation = data.rotation;
                part.transform.localScale = data.initScale;
                part.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void DestroyWall()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        if (wallCollider != null) wallCollider.enabled = false;

        StartCoroutine(DestroyWallCoroutine(transform.position));
    }

    public void DestroyWallFromSlam(Vector3 impactPoint)
    {
        if (isDestroyed) return;
        isDestroyed = true;
        if (wallCollider != null) wallCollider.enabled = false;

        StartCoroutine(DestroyWallCoroutine(impactPoint, 1.2f, 2f));
    }

    private IEnumerator DestroyWallCoroutine(Vector3 explosionPoint, float forceMultiplier = 1f, float upwardMod = 1f)
    {
        int cubeLayer = LayerMask.NameToLayer("IgnorePlayer");
        List<Rigidbody> rigidbodies = new List<Rigidbody>();
        List<Collider> colliders = new List<Collider>();

        foreach (GameObject part in wallParts)
        {
            if (part != null && part.activeSelf)
            {
                part.layer = cubeLayer;

                Collider partCollider = part.GetComponent<Collider>();
                if (partCollider != null)
                {
                    colliders.Add(partCollider);
                    partCollider.enabled = false;
                }

                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb == null) rb = part.AddComponent<Rigidbody>();

                rb.isKinematic = false;
                rb.useGravity = useGravity;
                rb.mass = 1f;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 0.5f;

                Vector3 randomTorque = new Vector3(
                    Random.Range(-100f, 100f),
                    Random.Range(-100f, 100f),
                    Random.Range(-100f, 100f)
                );
                rb.AddTorque(randomTorque, ForceMode.Impulse);

                rb.AddExplosionForce(
                    explosionForce * forceMultiplier,
                    explosionPoint,
                    explosionRadius,
                    upwardMod,
                    ForceMode.Impulse
                );

                rigidbodies.Add(rb);
            }
        }

        yield return new WaitForSeconds(enableColliderDelay);

        foreach (Collider col in colliders)
        {
            if (col != null) col.enabled = true;
        }

        yield return new WaitForSeconds(debrisLifetime - enableColliderDelay);

        foreach (GameObject part in wallParts)
        {
            if (part != null) part.SetActive(false);
        }
    }
}
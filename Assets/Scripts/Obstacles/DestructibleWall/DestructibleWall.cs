using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DestructibleWall : MonoBehaviour
{
    public List<GameObject> wallParts = new List<GameObject>();
    private List<WallPartData> initPartData = new List<WallPartData>();
    public Collider wallCollider;  
    public float explosionForce = 10f;  
    public float explosionRadius = 5f;  
    public bool isDestroyed = false;

    void Start()
    {
        if (wallParts.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf) 
                {
                    wallParts.Add(child.gameObject);
                }
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
                    rotation = part.transform.localRotation
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
        if (isDestroyed)
            if (Input.GetKeyDown(KeyCode.R))
                ResetWall();
    }

    private void ResetWall()
    {
        isDestroyed = false;
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }

        foreach (WallPartData data in initPartData)
        {
            GameObject part = data.part;
            if (part != null)
            {
                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                
                part.transform.localPosition = data.initPosition;
                part.transform.localRotation = data.rotation;

                part.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void DestroyWall()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (wallCollider != null)
        {
            wallCollider.enabled = false;
        }
        int cubeLayer = LayerMask.NameToLayer("IgnorePlayer");
        foreach (GameObject part in wallParts)
        {
            if (part != null)
            {
                part.layer = cubeLayer;
                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = part.AddComponent<Rigidbody>();
                }
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                
            }
        }
    }

    public void DestroyWallFromSlam(Vector3 impactPoint)
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (wallCollider != null)
        {
            wallCollider.enabled = false;
        }
        int cubeLayer = LayerMask.NameToLayer("IgnorePlayer");
        foreach (GameObject part in wallParts)
        {
            if (part != null)
            {
                part.layer = cubeLayer;
                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = part.AddComponent<Rigidbody>();
                }
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce * 1.2f, impactPoint, explosionRadius, 2f, ForceMode.Impulse);
            }
        }
    }


   

}

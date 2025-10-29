using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RDestructibleWall : MonoBehaviour
{
    public List<GameObject> wallParts = new List<GameObject>(); 
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
                if (child.gameObject.activeSelf && child.GetComponent<Collider>() != wallCollider) 
                {
                    wallParts.Add(child.gameObject);
                }
            }
        }
        
        ResetWall();
    }
    
    private void ResetWall()
    {
        isDestroyed = false;
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }
        foreach (GameObject part in wallParts)
        {
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;                                 
            }

            part.layer = LayerMask.NameToLayer("Default");
            part.SetActive(true);
        }
    }
    
    public void DestroyWall(Vector3 impactPoint)
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
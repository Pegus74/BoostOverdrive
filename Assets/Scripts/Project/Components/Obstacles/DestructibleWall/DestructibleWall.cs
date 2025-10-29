using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RDestructibleWall : MonoBehaviour, IRestartable
{
    public List<GameObject> wallParts = new List<GameObject>(); 
    public Collider wallCollider;  
    public float explosionForce = 10f;  
    public float explosionRadius = 5f;  
    public bool isDestroyed = false;
    
    private List<InitialPartData> initialWallPartsData = new List<InitialPartData>();
    [SerializeField] private GameEvent OnLevelResetEvent;
    
    void Awake()
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
        
        // Сохраняем локальные координаты
        foreach (GameObject part in wallParts)
        {
            initialWallPartsData.Add(new InitialPartData
            {
                partObject = part,
                initialPosition = part.transform.localPosition,
                initialRotation = part.transform.localRotation
            });
        }
        
        ResetWall();
    }
    
    private void OnEnable()
    {
        if (OnLevelResetEvent != null)
            OnLevelResetEvent.RegisterListener(SoftReset);
    }
    
    private void OnDisable()
    {
        if (OnLevelResetEvent != null)
            OnLevelResetEvent.UnregisterListener(SoftReset);
    }
    
    private void ResetWall()
    {
        isDestroyed = false;
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }
        foreach (InitialPartData data in initialWallPartsData)
        {
            Rigidbody rb = data.partObject.GetComponent<Rigidbody>();
            
            
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;                                 
            }

            data.partObject.transform.localPosition = data.initialPosition;
            data.partObject.transform.localRotation = data.initialRotation;
            
            data.partObject.layer = LayerMask.NameToLayer("Default");
            data.partObject.SetActive(true);
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

    public void SoftReset()
    {
        if (!isDestroyed) return;
        ResetWall();
    }
}
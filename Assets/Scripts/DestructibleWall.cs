using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DestructibleWall : MonoBehaviour
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
                if (child.gameObject.activeSelf) 
                {
                    wallParts.Add(child.gameObject);
                }
            }
        }
        if (wallCollider != null)
        {
            wallCollider.enabled = true;
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
            part.transform.localRotation = Quaternion.identity;
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

        foreach (GameObject part in wallParts)
        {
            if (part != null)
            {
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


    [ContextMenu("Test")]  
    private void TestDestroyWall()
    {
        DestroyWall();
    }

    [CustomEditor(typeof(DestructibleWall))]
    public class DestructibleWallEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DestructibleWall wall = (DestructibleWall)target;

            if (GUILayout.Button("test"))
            {
                wall.DestroyWall();
            }
        }
    }

}

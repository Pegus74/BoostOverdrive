using UnityEngine;

public class SlamCollisionHandler : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;
    
    [Header("Output Events")]
    public Vector3Event SlamDestructibleHitEvent;
    public GameEvent SlamSolidHitEvent;

    void Awake()
    {
        if (playerStateModel == null)
        {
            enabled = false;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!playerStateModel.IsSlamming)
        {
            return;
        }

        RDestructibleWall wall = other.GetComponent<RDestructibleWall>();
        
        if (wall != null && playerStateModel.CurrentStyleIndex == 0)
        {
            Vector3 impactPoint = other.ClosestPoint(transform.position);
            
            wall.DestroyWall(impactPoint);
            Debug.Log("DestructibleWall destroyed by Slam (via Event)");
            
            SlamDestructibleHitEvent?.Raise(impactPoint); 
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Untagged")) 
        {
            SlamSolidHitEvent?.Raise(); 
        }
    }
}
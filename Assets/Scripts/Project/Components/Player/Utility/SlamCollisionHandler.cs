using UnityEngine;

public class SlamCollisionHandler : MonoBehaviour
{
    [Header("Model")]
    public PlayerStateModel playerStateModel;
    
    private void Awake()
    {
        if (playerStateModel == null)
        {
            enabled = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
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
            Debug.Log("[DestructibleWall] Destroyed by Slam"); 
            
            AbilityEvents.SlamDestructibleHitEvent.Invoke(impactPoint); 
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Untagged")) 
        {
            AbilityEvents.SlamSolidHitEvent.Invoke(); 
        }
    }
}
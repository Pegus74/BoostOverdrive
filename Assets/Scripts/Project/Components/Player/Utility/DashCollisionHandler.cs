using UnityEngine;

/// <summary>
/// Обрабатывает коллизии во время дэша и вызывает разрушение стены при активном стиле
/// </summary>
public class DashCollisionHandler : MonoBehaviour
{
    public PlayerStateModel playerStateModel; 
    
    private const int HandStyleIndex = 0;

    private void Awake()
    {
        if (playerStateModel == null)
        {
            enabled = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (playerStateModel.IsDashing)
        {
            if (playerStateModel.CurrentStyleIndex == HandStyleIndex)
            {
                RDestructibleWall wall = other.GetComponent<RDestructibleWall>();
                if (wall != null)
                {
                    wall.DestroyWall(transform.position);
                    Debug.Log("DestructibleWall destroyed by Dash");
                }
            }
        }
    }
}
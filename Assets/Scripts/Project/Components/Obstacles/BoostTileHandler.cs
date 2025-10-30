using UnityEngine;
using System.Collections;

public class BoostTileHandler : MonoBehaviour
{
    [Header("Settings")]
    public ObstaclesSettingsData obstaclesSettingsData;
    
    [Header("Event to Raise")]
    public FloatEvent OnPlayerSpeedModifierChange;

    private const float NormalSpeed = 1.0f;
    private bool isPlayerInside = false;
    private Coroutine currentLingerCoroutine = null;
    private Rigidbody playerRb; 

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerRb == null)
            {
                playerRb = other.GetComponent<Rigidbody>();
            }
            
            if (currentLingerCoroutine != null)
            {
                StopCoroutine(currentLingerCoroutine);
                currentLingerCoroutine = null;
            }
            
            if (!isPlayerInside)
            {
                ApplyBoost();
                isPlayerInside = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;
            currentLingerCoroutine = StartCoroutine(InitiateCooldown());
        }
    }
    
    private void ApplyBoost()
    {
        if (obstaclesSettingsData != null)
        {
            float boost = obstaclesSettingsData.boostTileMultiplier;
            OnPlayerSpeedModifierChange.Raise(boost);
            Debug.Log($"[Boost Tile] Speed Modifier Change Event: {boost}");
        }
    }
    
    private IEnumerator InitiateCooldown()
    {
        float lingerTime = obstaclesSettingsData.boostTileGroundLingerTime;
        
        if (playerRb != null && playerRb.velocity.y > 0.1f)
        {
            lingerTime = obstaclesSettingsData.boostTileJumpLingerTime;
            Debug.Log($"[Boost Tile] Jump detected. Linger time: {lingerTime}s");
        }
        else
        {
            Debug.Log($"[Boost Tile] Player left tile. Linger time: {lingerTime}s");
        }

        yield return new WaitForSeconds(lingerTime);
        
        RemoveBoost(); 
        currentLingerCoroutine = null;
    }
    
    private void RemoveBoost()
    {
        OnPlayerSpeedModifierChange.Raise(NormalSpeed); 
        Debug.Log($"[Boost Tile] Speed Modifier Change Event: {NormalSpeed} (Cooldown ended)");
    }
}

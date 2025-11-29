using UnityEngine;
using UnityEngine.UI;

public class NewEnergyBar : MonoBehaviour
{
    public float playerEnergy = 1f;
    public Image energyBar;

    void Update()
    {
        energyBar.transform.localScale = new Vector3(playerEnergy, 1, 1);
        
        playerEnergy -= Time.deltaTime / 100;
        
        if (playerEnergy <= 0.001f && NewGameManager.Instance.GetCurrentState() == GameState.Playing)
            NewGameManager.Instance.PlayerDied();
    }

    private void OnEnable()
    {
        GameEvents.OnClassicModeStart.AddListener(HandleClassicModeStart);
        GameEvents.OnHardModeStart.AddListener(HandleHardModeStart);
        
        AbilityEvents.OnAbilityStarted.AddListener(RemoveEnergyOnAbility);
    }

    private void OnDisable()
    {
        GameEvents.OnClassicModeStart.RemoveListener(HandleClassicModeStart);
        GameEvents.OnHardModeStart.RemoveListener(HandleHardModeStart);
        
        AbilityEvents.OnAbilityStarted.RemoveListener(RemoveEnergyOnAbility);
    }

    private void RemoveEnergyOnAbility()
    {
        playerEnergy -= 0.04f;
    }

    private void HandleClassicModeStart()
    {
        gameObject.SetActive(false);
    }

    private void HandleHardModeStart()
    {
        gameObject.SetActive(true);
    }
}
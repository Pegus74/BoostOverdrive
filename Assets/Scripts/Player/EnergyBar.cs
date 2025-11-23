using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public float playerEnergy = 1f;
    public Image energyBar;

    void Update()
    {
        energyBar.fillAmount = playerEnergy;
        
        playerEnergy -= Time.deltaTime / 100;
        
        if (playerEnergy <= 0.01f)
            GameManager.Instance.PlayerDied();
    }

    public void RemoveEnergy()
    {
        playerEnergy -= 0.04f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
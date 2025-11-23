using UnityEngine;
using UnityEngine.UI;

public class ChargedJumpUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image chargedJumpUI;
    public Image chargedJumpBackground;

    [Header("Colors")]
    public Color minChargeColor = Color.green;
    public Color maxChargeColor = Color.red;
    public bool useColorGradient = true;

    private void Start()
    {
        InitializeUI();
    }

   
    private void InitializeUI()
    {
        if (chargedJumpUI != null)
        {
            chargedJumpUI.type = Image.Type.Filled;
            chargedJumpUI.fillMethod = Image.FillMethod.Vertical;
            chargedJumpUI.fillOrigin = (int)Image.OriginVertical.Bottom;
            chargedJumpUI.fillAmount = 0f;
            chargedJumpUI.gameObject.SetActive(false);

            if (useColorGradient)
            {
                chargedJumpUI.color = minChargeColor;
            }
        }

        if (chargedJumpBackground != null)
        {
            chargedJumpBackground.gameObject.SetActive(false);
        }
    }

    public void OnChargeStart()
    {
        if (chargedJumpUI != null)
        {
            chargedJumpUI.gameObject.SetActive(true);
            chargedJumpUI.fillAmount = 0f;
        }

        if (chargedJumpBackground != null)
        {
            chargedJumpBackground.gameObject.SetActive(true);
        }
    }

    public void OnChargeUpdate(float charge)
    {
        if (chargedJumpUI != null)
        {
            chargedJumpUI.fillAmount = charge;

            if (useColorGradient)
            {
                chargedJumpUI.color = Color.Lerp(minChargeColor, maxChargeColor, charge);
            }
        }
    }

    public void OnChargeCancel()
    {
        if (chargedJumpUI != null)
        {
            chargedJumpUI.gameObject.SetActive(false);
            chargedJumpUI.fillAmount = 0f;
        }

        if (chargedJumpBackground != null)
        {
            chargedJumpBackground.gameObject.SetActive(false);
        }
    }
}
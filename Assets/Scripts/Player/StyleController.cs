using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerStyle
{
    public string styleName = "Style";
    public float walkSpeed = 5f;
    public float jumpPower = 5f;
    public float fov = 60f;
    public bool canBreakWallsWithDash = false;
    public bool canBreakWallsWithSlam = false;
}


    public class StyleController : MonoBehaviour
    {
        public PlayerStyle[] styles = new PlayerStyle[2];
    
    public KeyCode switchStyleKey = KeyCode.Tab; 
    private int currentStyleIndex = 0;
    
     public MetricsMaster mm;

    [Header("UI Elements")]
    public Image LegstyleUI;    
    public Image HandsStyleUI;    
    public Canvas styleCanvas;

    private FirstPersonController controller;
    private Camera playerCamera;
    public PlayerStyle CurrentStyle => styles[currentStyleIndex];

    private float airbornWalkSpeed; 
    private bool isInAir = false;  
    private int styleAtJumpStart;

    void Awake()
    {
        controller = GetComponent<FirstPersonController>();
        playerCamera = controller.playerCamera;  

        if (mm != null && mm.Styles != null && mm.Styles.Length > 0)
        {
            styles = mm.Styles;
        }

        ApplyStyle(currentStyleIndex);
        UpdateStyleUI();
        if (controller != null)
        {
      
            var originalJumpMethod = System.Delegate.CreateDelegate(typeof(System.Action), controller, "Jump") as System.Action;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchStyleKey))
        {
            SwitchStyle();
        }
        
        UpdateAirbornState();
        ApplyStyleAirborn(currentStyleIndex);
    }

    private void SwitchStyle()
    {
        int newStyleIndex = (currentStyleIndex + 1) % styles.Length;
        if (isInAir)
        {
            currentStyleIndex = newStyleIndex;
            ApplyStyleAirborn(newStyleIndex);
        }
        else
        {
            currentStyleIndex = newStyleIndex;
            ApplyStyle(currentStyleIndex);
        }
        UpdateStyleUI();
    }

    private void ApplyStyle(int index)
    {
        if (index < 0 || index >= styles.Length) return;

        PlayerStyle style = styles[index];

        controller.walkSpeed = style.walkSpeed;
        controller.jumpPower = style.jumpPower;
        controller.fov = style.fov;
        playerCamera.fieldOfView = style.fov;
        airbornWalkSpeed = style.walkSpeed;

    }

    private void ApplyStyleAirborn(int index)
    {
        if (index < 0 || index >= styles.Length) return;
        PlayerStyle style = styles[index];
        controller.jumpPower = style.jumpPower; 
        controller.fov = style.fov;
        playerCamera.fieldOfView = style.fov;
    }

    private void UpdateAirbornState()
    {
        if (controller == null) return;
        bool wasInAir = isInAir;
        isInAir = !controller.IsGrounded();
        if (wasInAir && !isInAir)
        {
            ApplyStyle(currentStyleIndex);
        }
        if (!wasInAir && isInAir)
        {
            styleAtJumpStart = currentStyleIndex;
            airbornWalkSpeed = styles[styleAtJumpStart].walkSpeed;
            controller.walkSpeed = airbornWalkSpeed;
        }
    }

    private void UpdateStyleUI()
    {
        if (LegstyleUI != null && HandsStyleUI != null)
        {

            LegstyleUI.gameObject.SetActive(currentStyleIndex == 0);
            HandsStyleUI.gameObject.SetActive(currentStyleIndex == 1);
        }
    }

    public PlayerStyle GetCurrentStyle()
    {
        if (currentStyleIndex >= 0 && currentStyleIndex < styles.Length)
        {
            return styles[currentStyleIndex];
        }
        return null; 
    }

    public int GetCurrentStyleIndex() => currentStyleIndex;
    
// #if UNITY_EDITOR
//     void OnValidate()
//     {
//         if (!Application.isPlaying) return;
//
//         if (mm != null && mm.Styles != null && mm.Styles.Length > 0)
//         {
//             styles = mm.Styles;
//             ApplyStyle(currentStyleIndex);
//         }
//     }
// #endif
}
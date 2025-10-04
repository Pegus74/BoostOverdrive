using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStyle
{
    public string styleName = "Style"; 
    public float walkSpeed = 5f;  
    public float sprintSpeed = 7f;  
    public float jumpPower = 5f; 
    public float fov = 60f;
    public bool canBreakWallsWithDash = false;
}

public class StyleController : MonoBehaviour
{
    public PlayerStyle[] styles = new PlayerStyle[2]; 
    public KeyCode switchStyleKey = KeyCode.Tab; 
    private int currentStyleIndex = 0;  

    private FirstPersonController controller;
    private Camera playerCamera;
    public PlayerStyle CurrentStyle => styles[currentStyleIndex];

    void Awake()
    {
        controller = GetComponent<FirstPersonController>();
        playerCamera = controller.playerCamera;  

        if (styles.Length == 0)
        {
            styles = new PlayerStyle[2];
            styles[0] = new PlayerStyle { styleName = "Legs style", walkSpeed = 10f, sprintSpeed = 15f, jumpPower = 10f, fov = 60f, canBreakWallsWithDash = false };
            styles[1] = new PlayerStyle { styleName = "Hands style", walkSpeed = 6f, sprintSpeed = 8f, jumpPower = 6f, fov = 80f, canBreakWallsWithDash = true };
        }

        ApplyStyle(currentStyleIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchStyleKey))
        {
            currentStyleIndex = (currentStyleIndex + 1) % styles.Length;  
            ApplyStyle(currentStyleIndex);
        }
    }

    private void ApplyStyle(int index)
    {
        if (index < 0 || index >= styles.Length) return;

        PlayerStyle style = styles[index];

        controller.walkSpeed = style.walkSpeed;
        controller.sprintSpeed = style.sprintSpeed;
        controller.jumpPower = style.jumpPower;
        controller.fov = style.fov;
        playerCamera.fieldOfView = style.fov;

    }
}
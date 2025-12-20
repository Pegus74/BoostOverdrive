using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsMenu : MonoBehaviour
{
    public InputActionAsset actions;

    private const string SaveKey = "InputBindings";

    public void SaveBindings()
    {
        string json = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("Bindings saved");
    }

    public void LoadBindings()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return;

        string json = PlayerPrefs.GetString(SaveKey);
        actions.LoadBindingOverridesFromJson(json);

        Debug.Log("Bindings loaded");
    }
}
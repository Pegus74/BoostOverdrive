using TMPro;
using UnityEngine;

public class ShowTime : MonoBehaviour
{
    public string key;
    public TMP_Text text;

    private void Start()
    {
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key);
            text.text = value.ToString("F4");
        }
        else
        {
            text.text = "-.---.--";
        }
    }
}
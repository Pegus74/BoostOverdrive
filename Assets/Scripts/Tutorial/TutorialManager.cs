using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance;
    public static TutorialManager Instance => instance;

    private List<TutorialSign> allTutorialSigns = new List<TutorialSign>();
    private TutorialSign currentlyActiveSign;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetAllSigns()
    {
        foreach (var sign in allTutorialSigns)
        {
            if (sign != null)
            {
                sign.ResetSign();
            }
        }
        currentlyActiveSign = null;
    }

    public void RegisterSign(TutorialSign sign)
    {
        if (!allTutorialSigns.Contains(sign))
        {
            allTutorialSigns.Add(sign);
        }
    }
    public void UnregisterSign(TutorialSign sign)
    {
        if (allTutorialSigns.Contains(sign))
        {
            allTutorialSigns.Remove(sign);
        }
    }

    public void DeactivateAllSigns()
    {
        foreach (var sign in allTutorialSigns)
        {
            if (sign != null)
                sign.Deactivate();
        }

        currentlyActiveSign = null;
    }
    public bool ActivateSignByName(string signName)
    {
        foreach (var sign in allTutorialSigns)
        {
            if (sign != null && sign.gameObject.name == signName)
            {
                if (currentlyActiveSign != null && currentlyActiveSign != sign)
                {
                    currentlyActiveSign.Deactivate();
                }

                sign.Activate();
                currentlyActiveSign = sign;
                return true;
            }
        }
        return false;
    }

    public TutorialSign[] GetAllSignsInfo()
    {
        return allTutorialSigns.ToArray();
    }
    public int GetShownSignsCount()
    {
        int count = 0;
        foreach (var sign in allTutorialSigns)
        {
            if (sign != null && sign.WasShown)
                count++;
        }
        return count;
    }
}
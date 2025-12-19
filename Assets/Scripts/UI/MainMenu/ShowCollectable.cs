using UnityEngine;


public class ShowCollectable : MonoBehaviour
{
    public Material lockedMaterial;
    public Material unlockedMaterial;

    public string CollectableName;
    
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        Debug.Log("start");

        if (PlayerPrefs.GetInt(CollectableName) == 1)
        {
            meshRenderer.material = unlockedMaterial;
            return;
        }

        meshRenderer.material = lockedMaterial;
    }
}
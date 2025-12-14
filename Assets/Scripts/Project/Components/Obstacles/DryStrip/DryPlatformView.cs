using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DryPlatformView : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer platformRenderer;
    [SerializeField] private Material crackedMaterial;

    [Header("Audio")]
    [SerializeField] private AudioClip crackSound;

    private Material originalMaterial;
    private AudioSource audioSource;

    private void Awake()
    {
        originalMaterial = platformRenderer.material;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCrack()
    {
        audioSource.PlayOneShot(crackSound);
    }

    public void ShowCracked()
    {
        platformRenderer.material = crackedMaterial;
    }

    public void DestroyPlatform()
    {
        Destroy(gameObject);
    }
}
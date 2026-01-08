using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DashPostProcessEffects : MonoBehaviour
{
    [Header("References")]
    public PlayerStateModel playerStateModel;

    [Header("Effects Settings")]
    public float dashChromaIntensity = 1.5f;
    public float idleChromaIntensity = 0.2f;
    public float dashVignetteIntensity = 0.8f;
    public float idleVignetteIntensity = 0.3f;

    private Volume volume;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    void Awake()
    {
        volume = GetComponent<Volume>();
        if (volume.profile.TryGet<ChromaticAberration>(out chromaticAberration) &&
            volume.profile.TryGet<Vignette>(out vignette))
        {
  
        }
        else
        {
            Debug.LogError("Post Process Profile не содержит Chromatic Aberration или Vignette");
        }

        if (playerStateModel == null)
            playerStateModel = FindObjectOfType<PlayerStateModel>();
    }

    void Update()
    {
        bool isDashing = playerStateModel.IsDashing;

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = isDashing ? dashChromaIntensity : idleChromaIntensity;

        if (vignette != null)
            vignette.intensity.value = isDashing ? dashVignetteIntensity : idleVignetteIntensity;
    }
}
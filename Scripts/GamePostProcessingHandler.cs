using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GamePostProcessingHandler : MonoBehaviour
{
    [SerializeField] VolumeProfile mVolumeProfile;
    [SerializeField] float lSpeed = 0.02f;
    private Vignette mVignette;

    // References
    private HealthHandler healthHandler;
    void Start()
    {
        healthHandler = GetComponent<HealthHandler>();
        // get the vignette effect
        for (int i = 0; i < mVolumeProfile.components.Count; i++)
        {
            if (mVolumeProfile.components[i].name == "Vignette")
            {
                mVignette = (Vignette)mVolumeProfile.components[i];
            }

        }

        mVignette.intensity.value = 0;
    }

    private void Update()
    {
        if (healthHandler.IsDead)
          ChangeVignetteIntensity(0.42f);
    }
    public void ChangeVignetteIntensity(float to)
    {
        mVignette.intensity.value = Mathf.Lerp(mVignette.intensity.value, to, 0.02f);
    }
}

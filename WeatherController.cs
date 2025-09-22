using UnityEngine;
using System.Collections.Generic;
using System;

public class WeatherController : MonoBehaviour
{
    [Header("Toggles")]
    public GameObject rainRig;             // Parent of PS_Rain_Far, PS_Rain_Splashes
    public ParticleSystem Camera;      // PS_Rain_Near
    public AudioSource rainLoop;

    [Header("Wet Materials")]
    public List<Material> wettableMats;    // road, sidewalks, car paint etc.
    [Range(0,1)] public float wetness = 0f;
    public float wetLerpSpeed = 0.5f;      // how quickly wetness ramps

    [Header("Lighting")]
    public Light sunLight;
    public float drySunIntensity = 1.1f;
    public float wetSunIntensity = 0.6f;

    [Header("Fog")]
    public bool useFog = true;
    public float dryFogEnd = 600f;
    public float wetFogEnd = 250f;

    [Header("Car Grip")]
    public WetFrictionController frictionController;

    bool raining;

    public void StartRain()
    {
        raining = true;
        rainRig.SetActive(true);
        if (Camera) Camera.Play();
        if (rainLoop && !rainLoop.isPlaying) rainLoop.Play();
        //if (frictionController) frictionController.SetRaining(true);
    }

    public void StopRain()
    {
        StopRain(frictionController);
    }

    public void StopRain(WetFrictionController frictionController)
    {
        raining = false;
        if (Camera) Camera.Stop();
        if (rainLoop && rainLoop.isPlaying) rainLoop.Stop();
        // leave far rig on briefly for fadeout if you want; here we simply disable:
        rainRig.SetActive(false);
       // if (frictionController) frictionController.SetRaining(false);
    }

    void Update()
    {
        // Lerp wetness & lighting
        float targetWet = raining ? 1f : 0f;
        wetness = Mathf.MoveTowards(wetness, targetWet, Time.deltaTime * wetLerpSpeed);

        foreach (var m in wettableMats)
        {
            if (!m) continue;
            // If using Standard/URP Lit: _Smoothness exists.
            if (m.HasProperty("_Smoothness"))
            {
                float baseSmooth = 0.25f; float extra = 0.6f;
                m.SetFloat("_Smoothness", baseSmooth + wetness * extra);
            }
            // Optional: darken a little
            if (m.HasProperty("_Color"))
            {
                Color c = m.color;
                c *= Mathf.Lerp(1f, 0.9f, wetness);
                m.color = c;
            }
            // If using Shader Graph, expose a _Wetness float instead and set it here.
            if (m.HasProperty("_Wetness"))
                m.SetFloat("_Wetness", wetness);
        }

        if (sunLight)
            sunLight.intensity = Mathf.Lerp(drySunIntensity, wetSunIntensity, wetness);

        if (useFog)
        {
            RenderSettings.fog = true;
            float end = Mathf.Lerp(dryFogEnd, wetFogEnd, wetness);
            // If using RenderSettings linear fog:
            RenderSettings.fogEndDistance = end;
        }
    }
}

public class WetFrictionController
{
    internal void SetRaining(bool v)
    {
        throw new NotImplementedException();
    }
}
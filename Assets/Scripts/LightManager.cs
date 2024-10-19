using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Add light that follows player

public class LightManager : MonoBehaviour
{
    // Setup Variables
    [SerializeField] List<Light2D> m_lights;
    [Range(0, 2.5f)][SerializeField] float Intensity;

    private void Awake()
    {
        m_lights = new List<Light2D>();
    }

    public void RegisterLight(Light2D light)
    {
        if (m_lights.Contains(light)) 
        {
            m_lights.Add(light);
        }
        
    }    

    public void UnRegisterLight(Light2D light)
    {
        if (m_lights.Contains(light))
        {
            m_lights.Remove(light);
        }
    }

    public void SetLightActive(Light2D light, bool isActive)
    {
        if (m_lights.Contains(light))
        {
            light.enabled = isActive;
        }
    }

    public void SetLightIntensity(Light2D light, float intensity)
    {
        if (m_lights.Contains(light))
        {
            light.intensity = intensity;
        }
    }

    public void SetAllLightsActive(bool isActive)
    {
        foreach (var light in m_lights)
        {
            light.enabled = isActive;
        }
    }

    public void SetAllLightsIntensity(float intensity)
    {
        foreach (var light in m_lights)
        {
            light.intensity = intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light2D myLight;
    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;
    [SerializeField] private float flickerSpeed;

    private float timer;

    private void Start()
    {
        if (myLight == null)
        {
            myLight = GetComponent<Light2D>();
        }
        else
        {
            maxLightIntensity = myLight.intensity;
        }
    }

    private void Update()
    {
        timer += flickerSpeed * Time.deltaTime;
        float intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, Mathf.PingPong(timer, 1));
        myLight.intensity = intensity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenSystem : MonoBehaviour
{
    // Maximum oxygen num
    [SerializeField] private float maxOxygen = 100f;
    // Current value of oxygen
    private float currentOxygen;
    // How much oxygen decreases / increases in water
    [SerializeField] private float oxygenDrainRate = 1f;
    [SerializeField] private float oxygenRegenRate = 2f;

    public void Start()
    {
        currentOxygen = maxOxygen;
    }

    public void RefillOxygen()
    {
        currentOxygen = Mathf.Min(currentOxygen + (oxygenRegenRate * Time.deltaTime), maxOxygen);
    }

    public void DrainOxygen()
    {
        currentOxygen = Mathf.Max(currentOxygen - (oxygenDrainRate * Time.deltaTime), 0f);
    }

    public float GetCurrentOxygen() 
    {
        return currentOxygen;
    }
    public float GetOxygenDrainRate()
    {
        return oxygenDrainRate;
    }
    public float GetMaxOxygen()
    {
        return maxOxygen;
    }
}

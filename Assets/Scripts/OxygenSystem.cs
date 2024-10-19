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
        currentOxygen = Mathf.Min(currentOxygen + oxygenRegenRate, maxOxygen);
    }

    public void DrainOxygen(float oxygenChangeNum)
    {
        if (oxygenChangeNum < 0)
        {
            currentOxygen = Mathf.Max(currentOxygen + oxygenDrainRate, 0);
        }
    }

    public float GetCurrentOxygen() 
    {
        return currentOxygen;
    }
}

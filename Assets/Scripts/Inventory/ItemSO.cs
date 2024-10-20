using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChange;

    public void useItem()
    {
        switch (statToChange) 
        {
            case StatToChange.none:
                break;
            case StatToChange.lightSource:
                break;
            case StatToChange.key:
                DoorUnlockAttempt();
                break;
        }
    }

    protected void DoorUnlockAttempt()
    {

    }

    // for any potential healing items etc
    public enum StatToChange
    {
        none,
        lightSource,
        key
    };
}

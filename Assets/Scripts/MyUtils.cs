using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtils
{
    public static InventoryManager FindInventoryManagerInChildrenIncludingInactive(GameObject go)
    {
        // Check the current GameObject for the InventoryManager component
        InventoryManager manager = go.GetComponent<InventoryManager>();
        if (manager != null)
        {
            return manager;
        }

        // Iterate through all child GameObjects
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;

            // Recursively search in the child
            InventoryManager found = FindInventoryManagerInChildrenIncludingInactive(child);
            if (found != null)
            {
                return found;
            }
        }

        return null; // Couldn't find an InventoryManager
    }
}

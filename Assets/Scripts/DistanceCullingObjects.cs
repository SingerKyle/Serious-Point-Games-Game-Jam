using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCullingObjects : MonoBehaviour
{ 
    public Transform player;  // The player object reference
    public float cullingDistance = 15f;  // The distance at which objects should be culled

    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;

    private void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (player != null)
        {
            // Check the distance between this object and the player
            float distance = Vector2.Distance(transform.position, player.position);

            // Cull the object if it is too far
            if (distance > cullingDistance)
            {
                CullObject();  // Deactivate or hide the object
            }
            else
            {
                RestoreObject();  // Reactivate or show the object
            }
        }
    }

    private void CullObject()
    {
        // You can choose to disable rendering or the entire GameObject
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (objectCollider != null) objectCollider.enabled = false;
    }

    private void RestoreObject()
    {
        // Restore visibility and collision
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (objectCollider != null) objectCollider.enabled = true;
    }
}
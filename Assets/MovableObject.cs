using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour

{
    public float pushStrength = 5f;  // Adjust this to control how much force is applied to the crate

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is colliding with the crate
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = collision.transform.position - transform.position;
            rb.AddForce(-direction.normalized * pushStrength);
        }
    }
}
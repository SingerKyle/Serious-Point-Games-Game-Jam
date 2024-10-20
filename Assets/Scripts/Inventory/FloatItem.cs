using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatItem : MonoBehaviour
{
    [SerializeField] public float floatHeight = 0.25f;
    [SerializeField] public float floatSpeed = 1.5f;

    private Vector3 startPosition;

    private void Start()
    {
        // store start pos
        startPosition = transform.position;
    }

    private void Update()
    {
        // calculate new y position with a sine wave
        float sinY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, sinY, transform.position.z);
    }
}

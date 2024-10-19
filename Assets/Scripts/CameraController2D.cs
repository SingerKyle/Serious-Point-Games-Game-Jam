using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    // Designer Values
    [SerializeField] public float m_followSpeed;
    [Range(0, 5)][SerializeField] public float m_yOffset;
    public Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (m_target != null) 
        {
            Vector3 newPos = new Vector3(m_target.position.x, m_target.position.y + m_yOffset, transform.position.z);

            transform.position = Vector3.Slerp(transform.position, newPos, m_followSpeed*Time.deltaTime);

        }
    }
}

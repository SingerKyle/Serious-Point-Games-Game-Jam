using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    // Designer Values
    [SerializeField] public float m_followSpeed;
    [Range(0, 5)][SerializeField] public float m_yOffset;
    public Transform m_target;

    private float s_screenWidth;

    // Start is called before the first frame update
    void Start()
    {
        s_screenWidth = transform.position.x;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        /*if (m_target != null) 
        {
            Vector3 newPos = new Vector3(m_target.position.x, m_target.position.y + m_yOffset, transform.position.z);

            transform.position = Vector3.Slerp(transform.position, newPos, m_followSpeed*Time.deltaTime);

        }*/

        if (m_target != null)
        {
            if (m_target.position.x > s_screenWidth)
            {
                Vector3 newPos = new Vector3(m_target.position.x, m_target.position.y + m_yOffset, transform.position.z);

                transform.position = Vector3.Slerp(transform.position, newPos, m_followSpeed * Time.deltaTime);
                //transform.position = new Vector3(m_target.position.x, transform.position.y, transform.position.z);
            }
            else
            {
                Vector3 newPos = new Vector3(s_screenWidth, m_target.position.y + m_yOffset, transform.position.z);
                transform.position = Vector3.Slerp(transform.position, newPos, m_followSpeed * Time.deltaTime);
            }
        }
    }
}

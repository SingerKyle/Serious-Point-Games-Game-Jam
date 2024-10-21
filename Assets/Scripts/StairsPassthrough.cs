using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
   public bool isUp;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp)
        {
            if (Input.GetKeyDown(KeyCode.W) && player.GetComponent<PlayerController>().GetGrounded())
            {
                transform.parent.GetComponent<Collider2D>().enabled = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S) && player.GetComponent<PlayerController>().GetGrounded() || Input.GetKeyDown(KeyCode.DownArrow) && player.GetComponent<PlayerController>().GetGrounded())
            {
                transform.parent.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            transform.parent.GetComponent<Collider2D>().enabled = isUp;
        }
    }
}

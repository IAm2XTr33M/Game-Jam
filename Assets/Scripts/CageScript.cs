using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageScript : MonoBehaviour
{
    Transform player;

    Rigidbody rb;

    bool drop = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            rb.useGravity = true;
            drop = true;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (drop)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
            if (transform.position.y < player.position.y + 0.1f)
            {
                drop = false;
            }
        }

    }


}

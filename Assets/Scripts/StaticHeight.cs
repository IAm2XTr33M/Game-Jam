using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHeight : MonoBehaviour
{
    public float height;

    void Update()
    {
        if(transform.parent.position.y > -0.5f)
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(0,0,0);
            transform.localScale = new Vector3(100,100,100);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform followObject;
    [SerializeField] Vector3 offset;

    void Update()
    {
        transform.position = followObject.position + offset;
    }
}

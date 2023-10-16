using System.Collections.Generic;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    public float speed;
    List<GameObject> moveObjects = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (!moveObjects.Contains(other.gameObject) && other.gameObject.name == "tile")
        {
            moveObjects.Add(other.gameObject);
        }
    }

    private void Update()
    {
        List<GameObject> objs2remove = new();
        foreach(GameObject obj in moveObjects)
        {
            if(obj.transform.position.y < -1f)
            {
                obj.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
            }
            else
            {
                obj.transform.position = new Vector3(obj.transform.position.x,-1, obj.transform.position.z);
                objs2remove.Add(obj);
            }
        }
        foreach(GameObject obj in objs2remove)
        {
            if (moveObjects.Contains(obj))
            {
                moveObjects.Remove(obj);
            }
        }
    }
}

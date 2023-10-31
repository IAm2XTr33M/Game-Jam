using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUP : MonoBehaviour
{
    [SerializeField] string text;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.WriteText(text);
        Destroy(gameObject);
    }
}

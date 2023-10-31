using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndTriggerScript : MonoBehaviour
{
    [SerializeField] RawImage blackImage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            StartCoroutine(fadeOut());
            IEnumerator fadeOut()
            {
                for (float i = 0; i < 100; i++)
                {
                    yield return new WaitForSeconds(0.005f);
                    blackImage.color = new Color(0, 0, 0, i*1.2f / 100);
                }
                SceneManager.LoadScene("HomeScreen");
            }
        }
    }
}

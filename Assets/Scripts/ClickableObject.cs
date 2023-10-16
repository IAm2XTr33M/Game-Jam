using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    int timesClicked = 0;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Middle;
    public void HandleClick()
    {
        timesClicked++;

        if (timesClicked < 4)
        {
            Top.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Top.transform.eulerAngles += new Vector3(Random.Range(-5, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            Top.transform.localScale += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Bottom.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Bottom.transform.eulerAngles += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            Bottom.transform.localScale += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Middle.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Middle.transform.eulerAngles += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            Middle.transform.localScale += new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
        }
        else if(timesClicked == 4)
        {
            Destroy(gameObject);
        }
    }
}
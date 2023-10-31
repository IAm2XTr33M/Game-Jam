using UnityEngine;

public class CoinScript : MonoBehaviour
{

    bool touched = false;

    float speed = 1;

    private void Start()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
    }
    void Update()
    {
        transform.eulerAngles += new Vector3(0, Time.deltaTime * 180 * speed, 0);
        if (touched)
        {
            transform.localScale -= new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !touched)
        {
            speed = 3;
            GetComponentInChildren<ParticleSystem>().Play();
            touched = true;
            PlayerController.instance.coins += 1 * PlayerController.instance.coinMultiplier;
            if (GetComponent<BoxCollider>())
            {
                GetComponent<BoxCollider>().isTrigger = true;
            }
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3, ForceMode.Impulse);
            Destroy(gameObject, 0.3f);
        }
    }
}

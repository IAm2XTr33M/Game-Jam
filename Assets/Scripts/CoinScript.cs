using UnityEngine;

public class CoinScript : MonoBehaviour
{

    bool touched = false;
    void Update()
    {
        transform.eulerAngles += new Vector3(0, Time.deltaTime * 180, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && !touched)
        {
            touched = true;
            PlayerController.instance.coins += 1 * PlayerController.instance.coinMultiplier;
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3, ForceMode.Impulse);
            Destroy(gameObject, 0.2f);
        }
    }
}

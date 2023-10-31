using System.Collections;
using TMPro;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    public enum SpecialEffects{Random,Damage,Health,Jump,Speed,Slow,Coin,Stamina}
    public SpecialEffects tileEffect;

    public bool infinite;

    PlayerController player;



    bool activated = false;

    void Start()
    {
        player = PlayerController.instance;

        if(tileEffect == null || tileEffect == SpecialEffects.Random)
        {
            int randomEffect = Random.Range(0, 7);
            switch (randomEffect)
            {
                case 0: tileEffect = SpecialEffects.Damage; break;//
                case 1: tileEffect = SpecialEffects.Health; break;//
                case 2: tileEffect = SpecialEffects.Jump; break;//
                case 3: tileEffect = SpecialEffects.Speed; break;//
                case 4: tileEffect = SpecialEffects.Slow; break;//
                case 5: tileEffect = SpecialEffects.Coin; break;//
                case 6: tileEffect = SpecialEffects.Stamina; break;//
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && !activated)
        {
            if (!infinite)
            {
                activated = true;

                StartCoroutine(shrink());

                IEnumerator shrink()
                {
                    Vector3 size = transform.localScale;
                    while(transform.localScale.x +transform.localScale.z > 0.1f)
                    {
                        gameObject.transform.localScale -= new Vector3(size.x/50, 0, size.z/50);
                        yield return new WaitForSeconds(1 / 50);
                    }
                    transform.position += new Vector3(0, 100, 0);
                }
            }
            if (tileEffect == SpecialEffects.Damage)
            {
                int randomDamage = Random.Range(1, 4);
                randomDamage *= 10;
                player.WriteText("You took " + randomDamage.ToString() + " Damage!");
                player.health -= randomDamage;
            }
            else if (tileEffect == SpecialEffects.Health)
            {
                int randomHealth = Random.Range(1, 4) * 10;
                float temp = randomHealth;
                if(100 - player.health < temp)
                {
                    temp = 100 - player.health;
                }
                if(player.health == 100)
                {
                    player.WriteText("You gained 0 Health!");
                }
                else
                {
                    player.WriteText("You gained "+temp.ToString()+" Health!");
                }
                player.health += temp;
            }
            else if (tileEffect == SpecialEffects.Jump)
            {
                player.WriteText("You gained a 1.5x jump boost for 15 seconds!");

                StartCoroutine(player.JumpBoost());
            }
            else if (tileEffect == SpecialEffects.Speed)
            {
                player.WriteText("You gained a 1.5x Speed boost for 15 seconds!");

                StartCoroutine(player.SpeedBoost());
            }
            else if (tileEffect == SpecialEffects.Slow)
            {
                player.WriteText("You received 1.5x slowness for 15 seconds!");

                StartCoroutine(player.Slowness());
            }
            else if (tileEffect == SpecialEffects.Coin)
            {
                player.WriteText("You received 2x coin boost for 30 seconds!");

                StartCoroutine(player.CoinMult());

            }
            else if (tileEffect == SpecialEffects.Stamina)
            {
                player.WriteText("Your stamina has recharged!");
                player.currentStamina = 100;
                player.staminaEmpty = false;
            }
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    enum SpecialEffects{Damage,Health,Jump,Speed,Slow,Coin,Stamina}
    SpecialEffects tileEffect;

    PlayerController player;



    bool activated = false;

    void Start()
    {
        player = PlayerController.instance;

        int randomEffect = Random.Range(0, 7);
        switch (randomEffect)
        {
            case 0:tileEffect = SpecialEffects.Damage; break;//
            case 1:tileEffect = SpecialEffects.Health; break;//
            case 2:tileEffect = SpecialEffects.Jump; break;//
            case 3:tileEffect = SpecialEffects.Speed; break;//
            case 4:tileEffect = SpecialEffects.Slow; break;//
            case 5:tileEffect = SpecialEffects.Coin; break;//
            case 6:tileEffect = SpecialEffects.Stamina; break;//
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && !activated)
        {
            activated = true;
            if (tileEffect == SpecialEffects.Damage)
            {
                int randomDamage = Random.Range(1, 4);
                randomDamage *= 10;
                WriteText("You took " + randomDamage.ToString() + " Damage!");
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
                    WriteText("You gained 0 Health!");
                }
                else
                {
                    WriteText("You gained "+temp.ToString()+" Health!");
                }
                player.health += temp;
            }
            else if (tileEffect == SpecialEffects.Jump)
            {
                WriteText("You gained a 1.5x jump boost for 15 seconds!");

                StartCoroutine(player.JumpBoost());
            }
            else if (tileEffect == SpecialEffects.Speed)
            {
                WriteText("You gained a 1.5x Speed boost for 15 seconds!");

                StartCoroutine(player.SpeedBoost());
            }
            else if (tileEffect == SpecialEffects.Slow)
            {
                WriteText("You received 1.5x slowness for 15 seconds!");

                StartCoroutine(player.Slowness());
            }
            else if (tileEffect == SpecialEffects.Coin)
            {
                WriteText("You received 2x coin boost for 30 seconds!");

                StartCoroutine(player.CoinMult());

            }
            else if (tileEffect == SpecialEffects.Stamina)
            {
                WriteText("Your stamina has recharged!");
                player.currentStamina = 100;
                player.staminaEmpty = false;
            }
        }
    }



    void WriteText(string text)
    {
        TextMeshProUGUI bar = player.textBar.GetComponent<TextMeshProUGUI>();
        bar.fontSize = 40;
        bar.text = text;

        StartCoroutine(ShrinkText());

        IEnumerator ShrinkText()
        {
            yield return new WaitForSeconds(3);
            for (int i = 0; i < 40; i++)
            {
                bar.fontSize = 40 - i;
                yield return new WaitForSeconds(1 / 40);
            }

            bar.text = "";
        }
    }
}

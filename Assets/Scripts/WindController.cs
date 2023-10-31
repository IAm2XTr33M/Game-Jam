using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public ParticleSystem windParticlesHorizontal; 
    public ParticleSystem windParticlesVertical;
    public Transform windParticlesContainerHorizontal; 
    public Transform windParticlesContainerVertical;
    public float windForce = 5.0f;
    public float windChangeInterval = 5.0f;
    public float windDuration = 3.0f;

    public CharacterController characterController;
    private Vector3[] possibleWindDirections = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    private int currentWindDirectionIndex = 0;

    void Start()
    {
        StartCoroutine(RandomLimitedWindLoop());
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);        
    }

    IEnumerator RandomLimitedWindLoop()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            ParticleSystem windParticles;
            Transform windParticlesContainer;
            float rot;

            int randomDirectionIndex = Random.Range(0, possibleWindDirections.Length);
            if (randomDirectionIndex == currentWindDirectionIndex)
            {
                randomDirectionIndex = (randomDirectionIndex + 1) % possibleWindDirections.Length;
            }
            currentWindDirectionIndex = randomDirectionIndex;

            if(currentWindDirectionIndex <= 1)
            {
                windParticles = windParticlesHorizontal;
                windParticlesContainer = windParticlesContainerHorizontal;
                rot = currentWindDirectionIndex == 0 ? -90 : 90;
                windParticlesContainerHorizontal.gameObject.SetActive(true);
                windParticlesContainerVertical.gameObject.SetActive(false);
            }
            else
            {
                windParticles = windParticlesVertical;
                windParticlesContainer = windParticlesContainerVertical;
                rot = currentWindDirectionIndex == 2 ? 0 : 180;
                windParticlesContainerHorizontal.gameObject.SetActive(false);
                windParticlesContainerVertical.gameObject.SetActive(true);
            }

            windParticlesContainer.eulerAngles = new Vector3(windParticlesContainer.eulerAngles.x, rot, windParticlesContainer.eulerAngles.z);

            var mainModule = windParticles.main;
            mainModule.startSpeed = windForce;

            windParticles.Play();

            float elapsedTime = 0f;

            float duration = Random.Range(windDuration / 2, windDuration * 2);

            while (elapsedTime < duration)
            {
                // Apply wind force in the x and z directions, without affecting the y velocity.
                var windForceVector = possibleWindDirections[currentWindDirectionIndex] * windForce;

                // Apply wind force to the character controller without changing the y velocity.
                characterController.Move(windForceVector * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            characterController.SimpleMove(Vector3.zero);
            windParticles.Stop();

            yield return new WaitForSeconds(Random.Range(windChangeInterval/2 , windChangeInterval * 2f));
        }
    }
}
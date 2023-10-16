using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource musicSource;
    public AudioClip musicClip;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();

        musicSource.clip = musicClip;

        musicSource.loop = true;

        musicSource.Play();
    }
}
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float speed = 0.1f;
    public float startTime = 8;

    public float maxFogDensity = 0.02f; 
    public float fogLerpSpeed = 0.02f;

    public float fogStartHour = 19.0f; 
    public float fogEndHour = 6.0f;    

    private float currentTime = 0f;
    private bool isNight = false;

    public bool isLooping = true;

    private float currentFogDensity = 0f;

    private void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        float timeOfDay = Mathf.Repeat(currentTime, 24f) / 24f;

        currentTime += Time.deltaTime * speed;

        if (isLooping)
        {
            if (currentTime >= 24f)
            {
                currentTime = 0f;
            }
        }

        isNight = timeOfDay >= (fogStartHour / 24.0f) || timeOfDay <= (fogEndHour / 24.0f);

        if (isNight)
        {
            currentFogDensity = Mathf.Lerp(currentFogDensity, maxFogDensity, fogLerpSpeed * Time.deltaTime);
        }
        else
        {
            currentFogDensity = Mathf.Lerp(currentFogDensity, 0f, fogLerpSpeed * Time.deltaTime);
        }

        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = currentFogDensity;
    }
}

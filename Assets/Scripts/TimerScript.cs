using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{

    TextMeshProUGUI timer;
    float currentTime;

    private void Start()
    {
        timer = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        timer.text = currentTime.ToString("F2");
    }
}

/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


// This script was originally created by (Seta - Level Design), open sourced from Youtube and modified to fit project: https://www.youtube.com/watch?v=is-OijFIC9o
public class DayandNightScript : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currenttime;
    public float timespeed = 1f;
    
    [Header("currentTime")]
    public string currentTimeString;

    [Header("Light Settings")]
    public Light sunLight;
    public float sunposition = 1f;
    void Start()
    {
        updateTimeText();
    }

    void Update()
    {
        currenttime += Time.deltaTime + timespeed;
        if (currenttime >= 24f)
        {
            currenttime = 0f;
        }
        updateTimeText();
        upddateLight();
    }

    private void OnValidate()
    {
        upddateLight();
    }
    void updateTimeText()
    {
        currentTimeString = Mathf.Floor(currenttime).ToString("00") + ":" + ((currenttime % 1) * 60 ).ToString("00");
    }

    void upddateLight()
    {
        float sunrotation = (currenttime / 24f) * 360f;
        sunlight.transform.rotation = Quaternion.Euler(sunrotation - 90f, sunposition, 0f);
    }

}
*/
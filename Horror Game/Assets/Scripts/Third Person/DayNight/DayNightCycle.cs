using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float time;
    [SerializeField] private float fullDayLength = 30;
    [SerializeField] private float startTime = 0.38f; //approx 9 am - (1.0f/24) * 9
    [SerializeField] private Vector3 noon; //location of the sun directly above the player

    private float timeRate;

    [Header("Sun")]
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColour;
    [SerializeField] private AnimationCurve sunIntensity; //used to interpolate sun positions for intensity

    [Header("Moon")]
    [SerializeField] private Light moon;
    [SerializeField] private Gradient moonColour;
    [SerializeField] private AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    [SerializeField] private AnimationCurve lightingIntensityMultiplyer;
    [SerializeField] private AnimationCurve reflectionsIntensityMultiplyer;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        //increment time
        time += timeRate * Time.deltaTime;

        //if one cycle of a day has finished loop again
        if (time >= 1.0f)
        {
            time = 0.0f;
        }

        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        //light intensity
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        //change colours
        sun.color = sunColour.Evaluate(time);
        moon.color = moonColour.Evaluate(time);

        //enable / disable sun
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }

        //enable / disable moon
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(false);
        }
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
        }

        //lighting and reflection intensity
        RenderSettings.ambientIntensity = lightingIntensityMultiplyer.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionsIntensityMultiplyer.Evaluate(time);
    }
}


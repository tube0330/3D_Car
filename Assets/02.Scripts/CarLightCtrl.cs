using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLightCtrl : MonoBehaviour
{
    public Transform headLights;
    public Transform backLights;
    public static bool headLightsOn = false;
    public static bool backLightsOn = false;

    void Start()
    {
        backLights.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            headLightsOn = !headLightsOn;
        HeadLightOn();
        BackLightOn();
    }

    public void HeadLightOn()
    {
        headLights.gameObject.SetActive(headLightsOn);
    }
    public void BackLightOn()
    {
        if (backLightsOn)
        {
            Light[] lights = backLights.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
                light.intensity = 1f;
        }
        else
        {
            Light[] lights = backLights.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
                light.intensity = 0.5f;
        }
    }
}

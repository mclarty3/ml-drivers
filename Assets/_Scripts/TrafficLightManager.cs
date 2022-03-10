using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrafficLightGroup
{
    [SerializeField]
    public TrafficLight[] trafficLights;
    public int lightColour = -1;
}

public class TrafficLightManager : MonoBehaviour
{
    [SerializeField]
    public TrafficLightGroup[] trafficLightGroups;
    public float redGreenLightTime = 10f;
    public float yellowLightTime = 3f;

    public int activeLightGroup = -1;
    private bool _activeLightGroupYellow = false;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        activeLightGroup = Random.Range(0, trafficLightGroups.Length);
        ActivateLightGroup(activeLightGroup);
        _timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_activeLightGroupYellow)
        {
            if (Time.time - _timer > redGreenLightTime)
            {
                _activeLightGroupYellow = true;
                SetGroupLightColour(activeLightGroup, 1);
                _timer = Time.time;
            }
        }
        else
        {
            if (Time.time - _timer > yellowLightTime)
            {
                _activeLightGroupYellow = false;
                activeLightGroup = (activeLightGroup + 1) % trafficLightGroups.Length;
                ActivateLightGroup(activeLightGroup);
                _timer = Time.time;
            }
        }
    }

    public void ActivateLightGroup(int lightGroup)
    {
        if (lightGroup < 0 || lightGroup >= trafficLightGroups.Length)
        {
            return;
        }

        for (int i = 0; i < trafficLightGroups.Length; i++)
        // if (_activeLightGroup != -1)
        {
            SetGroupLightColour(i, 0);
        }

        activeLightGroup = lightGroup;
        SetGroupLightColour(activeLightGroup , 2);
    }

    public void SetGroupLightColour(int groupIndex, int colourIndex)
    {
        foreach (TrafficLight light in trafficLightGroups[groupIndex].trafficLights)
        {
            light.ActivateLight(colourIndex);
            trafficLightGroups[groupIndex].lightColour = colourIndex;
        }
    }
}

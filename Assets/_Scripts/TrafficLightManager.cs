using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    [System.Serializable]
    public struct TrafficLightGroup
    {
        [SerializeField]
        public TrafficLight[] trafficLights;
    }
    [SerializeField]
    public TrafficLightGroup[] trafficLightGroups;
    public float redGreenLightTime = 10f;
    public float yellowLightTime = 3f;
    
    public int _activeLightGroup = -1;
    private bool _activeLightGroupYellow = false;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _activeLightGroup = Random.Range(0, trafficLightGroups.Length);
        ActivateLightGroup(_activeLightGroup);
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
                SetGroupLightColour(_activeLightGroup, 1);
                _timer = Time.time;
            }
        }
        else
        {
            if (Time.time - _timer > yellowLightTime)
            {
                _activeLightGroupYellow = false;
                _activeLightGroup = (_activeLightGroup + 1) % trafficLightGroups.Length;
                ActivateLightGroup(_activeLightGroup);
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

        _activeLightGroup = lightGroup;
        SetGroupLightColour(_activeLightGroup , 2);
    }

    public void SetGroupLightColour(int groupIndex, int colourIndex)
    {
        foreach (TrafficLight light in trafficLightGroups[groupIndex].trafficLights)
        {
            light.ActivateLight(colourIndex);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrafficLight : MonoBehaviour
{
    public Light redLight;
    public Light yellowLight;
    public Light greenLight;

    public int activeLight = -1;
    private Light _activeLight = null;

    // Start is called before the first frame update
    void Start()
    {
        if (activeLight == -1)
        {
            activeLight = 0;
        }
        ActivateLight(activeLight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateLight(int lightIndex)
    {
        switch (lightIndex)
        {
            case 0:
                _activeLight = redLight;
                activeLight = 0;
                redLight.enabled = true;
                yellowLight.enabled = false;
                greenLight.enabled = false;
                break;
            case 1:
                _activeLight = yellowLight;
                activeLight = 1;
                redLight.enabled = false;
                yellowLight.enabled = true;
                greenLight.enabled = false;
                break;
            case 2:
                _activeLight = greenLight;
                activeLight = 2;
                redLight.enabled = false;
                yellowLight.enabled = false;
                greenLight.enabled = true;
                break;
            default:
                break;
        }
    }
}

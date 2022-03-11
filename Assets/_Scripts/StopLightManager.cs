using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLightManager : TrafficSignalManager
{
    void Awake()
    {
        trafficSignalGroups = new TrafficSignalGroup[1] { new TrafficSignalGroup() };
        trafficSignalGroups[0].signalType = 3;
    }
}

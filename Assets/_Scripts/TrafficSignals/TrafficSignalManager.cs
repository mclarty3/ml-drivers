using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
// public class TrafficSignalGroup : MonoBehaviour
// {
//     public int signalType = -1;
// }

public class TrafficSignalManager : MonoBehaviour
{
    [SerializeField]
    public TrafficSignalGroup[] trafficSignalGroups;
}

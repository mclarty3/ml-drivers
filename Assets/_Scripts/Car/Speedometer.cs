using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    CarController carController;
    [SerializeField]
    Text speedText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (carController != null && speedText != null)
        {
            speedText.text = carController.velocity.ToString("F2");
        }
    }
}

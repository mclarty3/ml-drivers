using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public bool mouseOverUIElement = false;
    [SerializeField]
    private Canvas canvas;
    private Button simulateButton;

    CarSpawner carSpawner;

    private static InterfaceManager instance = null;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        simulateButton = canvas.transform.Find("ManageMapPanel")
                                         .Find("SimulateButton").GetComponent<Button>();
        carSpawner = GetComponent<CarSpawner>();
        ToggleSimulationButton(carSpawner.simulationActive);
    }

    public static InterfaceManager GetInstance()
    {
        return instance;
    }

    public void MouseEnter()
    {
        mouseOverUIElement = true;
    }

    public void MouseExit()
    {
        mouseOverUIElement = false;
    }

    public void ToggleSimulationButton(bool simulationOn)
    {
        if (!simulationOn)
        {
            simulateButton.GetComponentInChildren<Text>().text = "Simulate";
            simulateButton.onClick.RemoveAllListeners();
            simulateButton.onClick.AddListener(() => carSpawner.BeginSimulation());
        }
        else
        {
            simulateButton.GetComponentInChildren<Text>().text = "Stop";
            simulateButton.onClick.RemoveAllListeners();
            simulateButton.onClick.AddListener(() => carSpawner.EndSimulation());
        }
    }
}

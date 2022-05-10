using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public bool mouseOverUIElement = false;
    public Slider numCarsSlider;
    public Text numCarsText;
    public Slider maxSpeedSlider;
    public Text maxSpeedText;
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

    void SetNumCars(float numCars)
    {
        carSpawner.SetNumCars((int)numCars);
        numCarsText.text = numCars.ToString();
    }

    void SetMaxSpeed(float maxSpeed)
    {
        carSpawner.SetMaxSpeed((int)maxSpeed);
        maxSpeedText.text = maxSpeed.ToString();
    }

    private void OnEnable() {
        carSpawner = GetComponent<CarSpawner>();

        numCarsSlider.onValueChanged.AddListener(SetNumCars);
        SetNumCars(numCarsSlider.value);

        maxSpeedSlider.onValueChanged.AddListener(SetMaxSpeed);
        SetMaxSpeed(maxSpeedSlider.value);
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

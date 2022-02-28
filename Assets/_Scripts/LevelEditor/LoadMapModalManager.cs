using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class LoadMapModalManager : MonoBehaviour
{
    [SerializeField]
    private GameObject modalPanel;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenModal(bool open=true)
    {
        SetDropdownOptions(SaveLoadManager.GetSavedLevels());
        modalPanel.SetActive(open);
    }

    public void SetDropdownOptions(List<string> options)
    {
        Dropdown dropdown = modalPanel.transform.Find("Dropdown").GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

}

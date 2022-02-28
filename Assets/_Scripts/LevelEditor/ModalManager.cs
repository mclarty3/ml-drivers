using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalManager : MonoBehaviour
{
    public bool isOpen {
        get {
            return modalPanel.activeSelf;
        }
    }

    [SerializeField]
    private GameObject modalPanel;
    private Text headerText;
    private Text bodyText;
    private Button confirmButton;
    private Button cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        headerText = modalPanel.transform.Find("HeaderText").GetComponent<Text>();
        bodyText = modalPanel.transform.Find("BodyText").GetComponent<Text>();
        confirmButton = modalPanel.transform.Find("ConfirmButton").GetComponent<Button>();
        cancelButton = modalPanel.transform.Find("CancelButton").GetComponent<Button>();

        OpenModal(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenModal(bool open=true)
    {
        modalPanel.SetActive(open);
    }

    public void SetHeaderText(string text)
    {
        headerText.text = text;
    }

    public void SetBodyText(string text)
    {
        bodyText.text = text;
    }

    public void SetModalConfirmButtonOnClick(UnityAction action)
    {
        confirmButton.onClick.AddListener(action);
    }

    public void SetModalCancelButtonOnClick(UnityAction action)
    {
        cancelButton.onClick.AddListener(action);
    }

    public void ClearModalConfirmButtonOnClick()
    {
        confirmButton.onClick.RemoveAllListeners();
    }

    public void ClearModalCancelButtonOnClick()
    {
        cancelButton.onClick.RemoveAllListeners();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public bool mouseOverUIElement = false;

    private static InterfaceManager instance = null;

    void Awake()
    {
        instance = this;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    private static DebugLogger _instance;
    public static DebugLogger GetInstance()
    {
        return _instance;
    }

    LevelManager lm;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Log(string message)
    {
        if (lm.printDebugMessages)
        {
            Debug.Log(message);
        }
    }

    void Start()
    {
        lm = LevelManager.GetInstance();
    }
}

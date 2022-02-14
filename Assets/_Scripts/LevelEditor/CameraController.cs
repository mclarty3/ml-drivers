using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 0.1f;
    public float cameraRotSpeed = 1f;
    Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Horizontal"))
        {
            cam.transform.Translate(new Vector3(Input.GetAxis("Horizontal") * cameraSpeed, 0, 0));
        }
        if (Input.GetButton("Vertical"))
        {
            cam.transform.Translate(new Vector3(0, 0, Input.GetAxis("Vertical") * cameraSpeed));
        }

        if (Input.GetMouseButton(1))
        {
            float sideMovement = Input.GetAxis("Mouse X") * cameraRotSpeed;
            float forwardMovement = -Input.GetAxis("Mouse Y") * cameraRotSpeed;
            cam.transform.Rotate(new Vector3(forwardMovement, sideMovement, 0));
        }
    }
}

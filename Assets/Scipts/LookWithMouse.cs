using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWithMouse : MonoBehaviour
{
    const float k_MouseSensitivityMultiplier = 0.01f;

    //float mouseSensitivity = 60f;

    public Transform playerBody;

    float xRotation = 0f;

    float minSpeed = 20f;

    public float maxSpeed;

    float Timer = 0;
 
    public float ControllerSensitivity;

    float addSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
     
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ControllerSensitivity = GameObject.Find("Canvas").GetComponent<GameSpeedSystem>().maxCameraSpeed;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        bool unlockPressed = false, lockPressed = false;

      //  float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * k_MouseSensitivityMultiplier;
     //   float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * k_MouseSensitivityMultiplier;

        float controllerX = Input.GetAxis("ControllerX") * ControllerSensitivity * k_MouseSensitivityMultiplier*10f;
        float controllerY = -Input.GetAxis("ControllerY") * ControllerSensitivity * k_MouseSensitivityMultiplier*10f;


        controllerX = float.Parse(controllerX.ToString("f2"));  
        controllerY = float.Parse(controllerY.ToString("f2"));

        unlockPressed = Input.GetKeyDown(KeyCode.Escape);
        lockPressed = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);

      
        if (unlockPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (lockPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //  ControllerSensitivity = Mathf.Clamp(ControllerSensitivity,minSpeed, maxSpeed);

        //if (controllerX != 0 || controllerY != 0)
        //{
        //    xRotation -= controllerY;
        //    Timer += Time.deltaTime;

        //    if (Timer>1f) {
        //        ControllerSensitivity += addSpeed*Time.deltaTime;
        //    }

        //}
        //else {

        //    Timer = 0;
        //    ControllerSensitivity = minSpeed;
        //}
        //Debug.Log("Sensitivity: "+ ControllerSensitivity);
        //  Debug.Log("Controller: "+ Input.GetAxis("ControllerX")+"   "+ Input.GetAxis("ControllerY"));
        xRotation -= controllerY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
         

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            //playerBody.Rotate(Vector3.up * mouseX);
            playerBody.Rotate(Vector3.up * controllerX);
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Step : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject controllerMap;
    public float menuSpeed = 5.0f;
    public float menuTargetY = 3.0f;
    public float menuStartPosition = 1000.0f;

    bool isDown = false;
    float targetPos;

        private void Start()
    {
        targetPos = menuTargetY + 300;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.JoystickButton6))
        {
            // move pause menu towards target position
          
            float newY = Mathf.Lerp(pauseMenuPanel.transform.position.y, menuTargetY + 300, Time.deltaTime * menuSpeed);
            pauseMenuPanel.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY, pauseMenuPanel.transform.position.z);

            if (MathF.Abs(newY - targetPos)<5f) {
                isDown = true; 
            
            }
          
            
        }
        else
        {
            // move pause menu back to start position
            float newY = Mathf.Lerp(pauseMenuPanel.transform.position.y, menuStartPosition, Time.deltaTime * menuSpeed);
            pauseMenuPanel.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY, pauseMenuPanel.transform.position.z);

            isDown = false;
        }



        if (Input.GetKey(KeyCode.JoystickButton7))
        {
            // move pause menu towards target position

            float newY = Mathf.Lerp(controllerMap.transform.position.y, menuTargetY + 300, Time.deltaTime * menuSpeed);
            controllerMap.transform.position = new Vector3(controllerMap.transform.position.x, newY, controllerMap.transform.position.z);

            if (MathF.Abs(newY - targetPos) < 5f)
            {
                isDown = true;

            }


        }
        else
        {
            // move pause menu back to start position
            float newY = Mathf.Lerp(controllerMap.transform.position.y, menuStartPosition, Time.deltaTime * menuSpeed);
            controllerMap.transform.position = new Vector3(controllerMap.transform.position.x, newY, controllerMap.transform.position.z);

            isDown = false;
        }



        if (!isDown)
        {
            Time.timeScale = 1;
        }
        else {
            Time.timeScale = 0;       
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSpeedSystem : MonoBehaviour
{
    // Start is called before the first frame update
    bool isFastMode = true;
    bool changedColor = true;
    public GameObject ModeInfor;
    float timer = 0;

    GameObject hand;
    GameObject Character;
    Camera Camera;
    float colorAlpha = 255f;


    public float maxCameraSpeed = 55f;
    public float maxCharcterSpeed = 15f;
    public float maxHandRotSpeed = 500f;

    private void Start()
    {
        hand = GameObject.Find("WhiteHand");
        Character = GameObject.Find("Player");
        Camera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ControllerB"))
        {
            ChangeMode();

        }

    }

    void ChangeMode() {
        Debug.Log("ChangeMode!");
        if (isFastMode)
        {
            isFastMode = false;
            changedColor = false;
            ModeInfor.GetComponent<TextMeshProUGUI>().text = "Slow Mode";
            hand.GetComponent<HandController>().handRotSpeed = 200f;
            Character.GetComponent<PlayerMovement>().speed = 7f;
            Camera.GetComponent<LookWithMouse>().ControllerSensitivity = 27f;


        }
        else {

            isFastMode = true;
            changedColor = false;
            ModeInfor.GetComponent<TextMeshProUGUI>().text = "Fast Mode";
            hand.GetComponent<HandController>().handRotSpeed = maxHandRotSpeed;
            Character.GetComponent<PlayerMovement>().speed = maxCharcterSpeed;
            Camera.GetComponent<LookWithMouse>().ControllerSensitivity = maxCameraSpeed;

        }
    }

    void changeColor() {
        
        if (!changedColor) {
            Color c = ModeInfor.GetComponent<TextMeshProUGUI>().color;
            colorAlpha -= Time.deltaTime*50f;
            ModeInfor.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b,colorAlpha);
            timer += Time.deltaTime;

            if (timer > 5f)
            {
                timer = 0;
                colorAlpha = 255;
                changedColor = true;

            }
            Debug.Log("Changing color: "+colorAlpha);
        }
       
    
    }


   
}

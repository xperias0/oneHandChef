using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class HandController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject one;

    public GameObject two;

    public GameObject three;

    public GameObject four;

    public GameObject five;


    public float speed = 2f;


    [HideInInspector]
    public float handRotSpeed = 600f;


   // public float handspeed;

    public float angle = 280f;

    [HideInInspector]
    public bool isLeftGrab = false;

    [HideInInspector]
    public bool isGrab = false;

    float totalLeftAngle = 0;

    float totalRightAngle = 0;

    bool actOne = false;

    public float handGrabSpeed;

    Vector3 handDefaultPos;

     float handMovespeed = 1.5f;

    Vector3 handMoveTargetPos;

    float moveValue = 0;


    float minRotSpeed =80f;

    [HideInInspector]
    public float maxRotSpeed = 550f;

    float curRotSpeed = 80f;

  

    [HideInInspector]

    private void Start()
    {
        handRotSpeed = GameObject.Find("Canvas").GetComponent<GameSpeedSystem>().maxHandRotSpeed;
       
    }

    //void FixedUpdate()
    //{
    //    handController();

    //    handRot();

    //    handMove();

    //}

    private void Update()
    {
        handController();

        handRot();

        handMove();
    }



    void handMove() {
        float UpandDownAxis = Input.GetAxis("UpDownAxis");

        handDefaultPos = GameObject.Find("h").transform.position;
       

        handMoveTargetPos= handDefaultPos + transform.forward* -2.5f;

        moveValue = Mathf.Clamp(moveValue,0,1f);
       // Debug.Log("HandmoveValue: "+moveValue);

        if (UpandDownAxis == 0.1f) {
            moveValue += Time.deltaTime * handMovespeed;

            transform.position = Vector3.Lerp(handDefaultPos,handMoveTargetPos, moveValue);
     
        }
        if (UpandDownAxis == -0.1f) {
            moveValue -= Time.deltaTime * handMovespeed;
 
            transform.position = Vector3.Lerp(handDefaultPos, handMoveTargetPos, moveValue);
        
        }
        
    }
    void handController() {

        float leftJoy = Input.GetAxis("ControllerHorizontal");
        float rightjoy = Input.GetAxis("ControllerVertical");

        if (leftJoy == 1 && !isGrab)
        {


            if (totalLeftAngle < angle)
            {
                rotOne(one);
                rot(two,1f);

                totalLeftAngle += handGrabSpeed * Time.deltaTime;
                // Debug.Log("Angle: " + totalLeftAngle);
            }
            else
            {
                totalLeftAngle = angle;
            }
        }
        if (leftJoy == 0)
        {


            if (totalLeftAngle > 0)
            {
                InverseRotOne(one);
                InverseRot(two,1f);

                totalLeftAngle -= handGrabSpeed * Time.deltaTime;
            }
            else
            {
                totalLeftAngle = 0;
            }
        }





        if (rightjoy == 1 && !isGrab)
        {
            if (totalRightAngle < angle)
            {
                rot(three,1f);
                rot(four,1f);
                rot(five, 1f);
                totalRightAngle += handGrabSpeed * Time.deltaTime;
            }
            else
            {
                totalRightAngle = angle;
            }
        }

        if (rightjoy == 0)
        {
            if (totalRightAngle > 0)
            {
                InverseRot(three,1f);
                InverseRot(four,1f);
                InverseRot(five,1f);
                totalRightAngle -= handGrabSpeed * Time.deltaTime;

            }
            else
            {
                totalRightAngle = 0;
            }
        }

        if (leftJoy == 1 && rightjoy == 1 && totalLeftAngle == angle && totalRightAngle == angle)
        {
            isGrab = true;
            isLeftGrab = false;
        }
        else
        {
            isGrab = false;
        }

        if (leftJoy == 1 && totalLeftAngle == angle)
        {
            isLeftGrab = true;
        }
        else { 
            isLeftGrab= false;
        }
    }
    void handRot() {
        curRotSpeed = Mathf.Clamp(curRotSpeed,minRotSpeed,maxRotSpeed);
     
        if (Input.GetButton("LeftBumper"))
        {                   
            transform.Rotate(Vector3.forward, -handRotSpeed * Time.deltaTime);
        }
       

        if (Input.GetButton("RightBumper"))
        {
     
            transform.Rotate(Vector3.forward, handRotSpeed * Time.deltaTime);

        } 

    }
 
    void rot(GameObject b,float s)
    {
    
         
            b.transform.Rotate(Vector3.right,-angle * Time.deltaTime * speed *s);
            b.transform.GetChild(0).Rotate(Vector3.right, -angle * Time.deltaTime * speed * s);
            b.transform.GetChild(0).GetChild(0).Rotate(Vector3.right, -angle * Time.deltaTime * speed * 2 * s );
     
      
    }

    void InverseRot(GameObject b,float s)
    {
      
            b.transform.Rotate(Vector3.right, angle * Time.deltaTime * speed * s);
            b.transform.GetChild(0).Rotate(Vector3.right, angle * Time.deltaTime * speed * s);
            b.transform.GetChild(0).GetChild(0).Rotate(Vector3.right, angle * Time.deltaTime * speed * 2 * s);

      
       
    }
    void rotOne(GameObject o)
    {
       
            o.transform.Rotate(Vector3.right, -angle * Time.deltaTime * 2 );
            o.transform.GetChild(0).Rotate(Vector3.right, -angle*Time.deltaTime * 3 );
    }
    void InverseRotOne(GameObject o)
    {
        o.transform.Rotate(Vector3.right, angle * Time.deltaTime * 2);
        o.transform.GetChild(0).Rotate(Vector3.right, angle * Time.deltaTime * 3);
    }


    void actionOne() {
        //if (!actOne)
        //{
        //    rotOne(one);
        //    rot(two, 1f);
        //    rot(four, 1f);
        //    rot(five, 1f);
        //}
        //else { 
        //    InverseRotOne(one);
        //    InverseRot(two,2f);
        //    InverseRot(four,1f);
        //    InverseRot(five,1f);      
        //}

      
    }
}

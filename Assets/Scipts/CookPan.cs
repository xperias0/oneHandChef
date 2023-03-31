using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CookPan : MonoBehaviour
{
    // Start is called before the first frame update

    bool isOpened = false;
    Vector3 lastPos;
    float time = 0;


    private void Start()
    {
        lastPos = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "cookable")
        {
            other.transform.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log(other.name+" enter");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        Vector3 vel = (transform.position - lastPos).normalized;
        lastPos = transform.position;

        isOpened = GameObject.Find("fireButton").GetComponent<ButtonEnableParticle>().isOpen;
        if (other.gameObject.tag == "cookable" && isOpened)
        {
            Cookable b = other.gameObject.GetComponent<Cookable>();
            b.cookTime += Time.fixedDeltaTime;
           // Debug.Log(other.gameObject.name + " on Pan: ");
        }

        if (isOpened &&other.tag=="Grable" ) {

            time += Time.deltaTime;

            if (time>3f) {

                // other.GetComponent<MeshRenderer>().material = GameObject.Find("ckmat").GetComponent<Renderer>().material;
                other.GetComponent<MeshRenderer>().material.color = new Color(255,211,0,20);
                other.GetComponent<Item>().isCooked = true;
                time = 0;
            }
        
            
        }


        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            Debug.Log("vel: "+ vel);
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;

            float r = 0.25f;
            float xRange = Mathf.Clamp(x,x-r,x+r);
            float zRange = Mathf.Clamp(z,z-r,z+r);
            float yRange = Mathf.Clamp(y,y-0.1f,y+r);
            rb.mass = 3f;
          //  other.transform.position = new Vector3(xRange, yRange, zRange);


           // rb.AddForce(vel,ForceMode.Acceleration);
        
        }

    }

   
}

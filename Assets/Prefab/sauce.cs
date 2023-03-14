using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sauce : MonoBehaviour
{
    public ParticleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Abs(transform.rotation.eulerAngles.z);
        float anglex = Mathf.Abs(transform.rotation.eulerAngles.x);
        // Debug.Log(angle);
        if (angle >= 100.0f && angle <=270.0f || anglex >= 100.0f && anglex <= 250.0f)
        {
            Debug.Log("Drop");
            system.Play(true);
            
           // Debug.Log(gameObject.transform.rotation.z);
        }
        else
        {
            Debug.Log("stand");
            system.Stop();

            
          //  Debug.Log(gameObject.transform.rotation.z);
        }
    }


    public void addSteak() {

        system.collision.AddPlane(GameObject.Find("Beef Steak").transform);
    }
}

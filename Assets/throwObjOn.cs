using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwObjOn : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 pos;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Grable") {
            //collision.transform.parent = transform;
            //collision.transform.position = transform.position;
            //transform.GetComponent<BoxCollider>().isTrigger= true;
            //pos = transform.position;
            //collision.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //transform.GetComponent<Rigidbody>().freezeRotation = true;
            //Debug.Log("Thorw on");

            collision.transform.GetComponent<Rigidbody>().isKinematic = true;
            collision.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag =="Grable") {
            //other.transform.position = transform.position;

            //transform.position = pos;
        }
    }
}

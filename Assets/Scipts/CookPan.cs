using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookPan : MonoBehaviour
{
    // Start is called before the first frame update

    bool isOpened = false;
    
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
        isOpened = GameObject.Find("fireButton").GetComponent<ButtonEnableParticle>().isOpen;
        if (other.gameObject.tag == "cookable" && isOpened)
        {
            Cookable b = other.gameObject.GetComponent<Cookable>();
            b.cookTime += Time.fixedDeltaTime;
            Debug.Log(other.gameObject.name + " on Pan: ");
        }
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FinishButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && GameObject.Find("WhiteHand").GetComponent<HandController>().isLeftGrab) {
            GameObject.Find("Time").GetComponent<text>().cd = true ;
                
        }
    }
}

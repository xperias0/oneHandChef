using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashDector : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("Sink.005").GetComponent<ButtonEnableParticle>().isOpen) {

            if (other.tag =="Grable" && !other.GetComponent<Item>().isWashed) {

                GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(10);
                other.GetComponent<Item>().isWashed = true;
            }
        
        }
    }
}

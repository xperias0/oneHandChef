using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WashDector : MonoBehaviour
{
    // Start is called before the first frame update

    float time = 0;
    private void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("Sink.005").GetComponent<ButtonEnableParticle>().isOpen) {

            if (other.tag =="Grable" && !other.GetComponent<Item>().isWashed) {

                time += Time.deltaTime;

                if (time>4f) {
                    GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(5);
                    other.GetComponent<Item>().isWashed = true;
                    GameObject.Find("WashText").GetComponent<TextMeshProUGUI>().text = other.name +" is washed !";
                    time = 0;

                    listBoard.Instance.setToggleTrue(2);
                }
            }
        
        }
    }
}

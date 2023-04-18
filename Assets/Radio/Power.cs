using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.Find("radio").GetComponent<Radio>().Power();
            Debug.Log("Power");
        }
        //Debug.Log("Power2");

    }




    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("radio").GetComponent<Radio>().buttonPower.transform.localPosition = new Vector3(0, 0.016f, 0);
        }
    }
}

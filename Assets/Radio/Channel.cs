using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Channel : MonoBehaviour
{
    public int num = -1;



    private void OnTriggerStay(Collider other)
    {
        if (num >= 4)
        {
            num = num - 4;
            //Debug.Log("Current Channel is: " + num);
        }
        if (other.CompareTag("Player"))
        {
            GameObject.Find("radio").GetComponent<Radio>().changeChannel(num++);
            Debug.Log("Current Channel is: " + num);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("radio").GetComponent<Radio>().buttonChannel.transform.localPosition = new Vector3(0, 0.016f, 0);
            
        }
    }
}


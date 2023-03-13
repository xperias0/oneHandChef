using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class text : MonoBehaviour
{
    // Start is called before the first frame update

    public float Curtime = 0;
    float timer = 0;
    // Update is called once per frame

 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
           timer= Time.time;
        }
        Curtime = Time.time - timer;

        //Debug.Log("Timer: " + timer + "   CurtIme: " + Time.time);

        GetComponent<TextMeshProUGUI>().text = "Time :"+Curtime.ToString("f2");
    }
}

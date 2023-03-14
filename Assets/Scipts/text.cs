using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class text : MonoBehaviour
{
    // Start is called before the first frame update

    public float Curtime = 600;
    public float mins;
    public float seconds  = 60f;
    // Update is called once per frame

 
    void Update()
    {
        countDown();
    }


    void countDown() {
        if (mins > 0 && seconds > 0)
        {

            seconds -= Time.deltaTime;

            if (seconds <= 0)
            {
                seconds = 60.0f;
                mins -= 1;
            }
            if (mins == 0 && seconds <= 30.0f)
            {
                GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            GetComponent<TextMeshProUGUI>().text = "Time: " + mins + " : " + seconds.ToString("f2");
        }
        else { 
        
        
        }
    
    }
}

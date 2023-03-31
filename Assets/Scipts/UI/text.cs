using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class text : MonoBehaviour
{
    // Start is called before the first frame update

  
    public float mins = 5f ;
    public float seconds  = 60f;

    GameObject canvas;
    // Update is called once per frame
    [HideInInspector]
    public bool count = true;

    [HideInInspector]
    public bool cd = false;


    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    void Update()
    {
        if (count) {
            countDown();
        }
        

        if (cd) {
            countAndAddScore();
        }
        if (mins == 0 && seconds == 0 ) {
            
        
        }
    }


    void countDown() {
        if (mins > 0 || seconds > 0)
        {

            seconds -= Time.deltaTime;

            if (seconds <= 0 && mins >= 0)
            {
                seconds = 60.0f;
                mins--;
            }
            if (mins == 0 && seconds <= 30.0f)
            {
                GetComponent<TextMeshProUGUI>().color = Color.red;
                GetComponent<TextMeshProUGUI>().fontSize = 50f;
            }

            GetComponent<TextMeshProUGUI>().text = "Time: " + mins + "." + seconds.ToString("f2");
        }
        else {
          //  Debug.Log("0000");
            seconds = 0;
            mins = 0;
            GetComponent<TextMeshProUGUI>().text = "Time: " + mins + "." + seconds.ToString("f2");
            canvas.GetComponent<ScoreManager>().FInalCodition();
            count = false;
        }

        if (mins<0) {
            seconds = 0;
            mins = 0;
            GetComponent<TextMeshProUGUI>().text = "Time: " + mins + "." + seconds.ToString("f2");
            canvas.GetComponent<ScoreManager>().FInalCodition();
            count = false;


        }
        
    
    }


    public void countAndAddScore() {

        seconds--;
        canvas.GetComponent<ScoreManager>().addScore(1);
    }
}

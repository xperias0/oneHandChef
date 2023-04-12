using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getScore : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject scoreManager = null;
    void Start()
    {
        scoreManager = GameObject.Find("PassScore");

        if (scoreManager)
        {
            float nu = scoreManager.GetComponent<ScorePasser>().score;
            if (nu < 150)
            {
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(1).GetComponent<AudioSource>().Play();
                transform.GetChild(3).gameObject.SetActive(true);


            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetComponent<AudioSource>().Play();
                transform.GetChild(2).gameObject.SetActive(true);
            }

        }
    }

    // Update is called once per frame

}
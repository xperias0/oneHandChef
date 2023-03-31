using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public int FinalScore = 0;

    public int maxSaltScore = 30;

    public int saltScore = 0;

    int maxFinalScore = 300;

    float finishedTime;


    public GameObject FinalPanel;
    // Update is called once per frame

    private void Update()
    {
        //if (Input.GetButtonDown("Finish")) {
        //    Debug.Log("Done! ");
        //    FInalCodition();
        //}

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(1);
        }
    }
    public void addScore(int addNum ) {
       
             FinalScore+= addNum;
        if (FinalScore > maxFinalScore)
        {
            FinalScore = maxFinalScore;
        }
        GameObject.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: "+FinalScore;
    }

    public void addSalt() {
        if (saltScore<maxSaltScore) {
            saltScore++;
            FinalScore+=1;
            GameObject.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: " + FinalScore;

        }
    }

    public void FInalCodition() {
       
        FinalPanel.SetActive(true);

       // finalTime();
        if (FinalScore >= 180)
        {
            GameObject.Find("Condition").GetComponent<TextMeshProUGUI>().text = "Excellent Chef!";
        }
        else {

            GameObject.Find("Condition").GetComponent<TextMeshProUGUI>().text = "Try next time :(";

        }
      //  UnityEditor.EditorApplication.isPaused = true;
    }



    public void finalTime() {
        finishedTime = Time.time;
        GameObject.Find("FinishedTime").GetComponent<TextMeshProUGUI>().text ="Finished time: "+ finishedTime.ToString("f2")+" s";
    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WashDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject WashText;
    ScoreManager score;
    bool isAdded;
    // Update is called once per frame
    private void Start()
    {
        score = GameObject.Find("Canvas").GetComponent<ScoreManager>();
  
    }
    void Update()
    {
     //   Ray ray;
        RaycastHit hit;

        if (Physics.Raycast(transform.position,-transform.up,out hit,50f,1<<LayerMask.NameToLayer("Washable"))) {
            GameObject b = hit.collider.gameObject;

            WashText.GetComponent<TextMeshProUGUI>().text = b.name + " is Washed!";

            isAdded = b.GetComponent<Item>().isWashed;
            if (!isAdded) {
                score.addScore(10);
                b.GetComponent<Item>().isWashed = true;
            }
        }

    }
}

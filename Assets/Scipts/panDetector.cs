using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panDetector : MonoBehaviour
{
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,-transform.up,out hit,10f)) {
            if (hit.collider.tag == "Finish")
            {
                GameObject.Find("FryingPan1").GetComponent<CookPan>().Onfire = true;
                

            }
            else {
                GameObject.Find("FryingPan1").GetComponent<CookPan>().Onfire = false;
                
            }
            
        }
        Debug.DrawLine(transform.position,transform.position+(-transform.up*10f), Color.red);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutFridge : MonoBehaviour
{
    bool isSteakOut = false;
    bool isfruitOut = false;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        

        if (other.tag == "cookable") {
            isSteakOut = true;
        }

        if (other.tag == "Grable") {
            isfruitOut = true;
        }

        if (isSteakOut && isSteakOut) {
            listBoard.Instance.setToggleTrue(1);
        }
    }
}

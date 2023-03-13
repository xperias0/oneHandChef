using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjects : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    Vector3 defalutPos;
    private void Start()
    {
        defalutPos = GameObject.Find("CentralTable").transform.position;
    }

    void Update()
    {
        if (transform.position.y<-10f) {

            transform.position = defalutPos + new Vector3(0,7f,0); 
            transform.rotation = Quaternion.identity;
            Debug.Log("Drop");
        }
    }
}

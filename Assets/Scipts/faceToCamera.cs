using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceToCamera : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

          Vector3 newPos = -(Camera.main.transform.position - transform.position).normalized * Vector3.Distance(transform.position,Camera.main.transform.position) +transform.position;
        newPos.y = transform.position.y;
        //Vector3 newPos = GameObject.Find("Player").transform.position;
        transform.LookAt(newPos);

    }
}

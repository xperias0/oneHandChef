using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjects : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    Vector3 defalutPos;

    HandController h;

    LineRenderer lr;

    public float step = 0.1f;

    float dis = 0.8f;
    Transform b;
    private void Start()
    {
        defalutPos = GameObject.Find("CentralTable").transform.position;
        h = GameObject.Find("WhiteHand").GetComponent<HandController>();
        lr =  transform.GetComponent<LineRenderer>();
        b = transform.GetChild(1);
    }

    void Update()
    {
        if (transform.position.y<-10f) {

            transform.position = defalutPos + new Vector3(0,7f,0); 
            transform.rotation = Quaternion.identity;
            Debug.Log("Drop");
        }
        RaycastHit hit;
        if (transform.parent == GameObject.Find("OverlapSphere").transform && h.isLeftGrab || h.isGrab)
        {
            lr.enabled = true;
            if (Physics.Raycast(b.position, -Vector3.up, out hit))
            {

                
                Vector3 direction = transform.right;
                
                float angle = Vector3.Angle(direction,transform.right);
                Vector3 firstPos = hit.point + direction * dis;
                
                for (int i = 0; i < lr.positionCount; i++)
                {
                    Vector3 curPos = firstPos - direction * ((2 * dis) / lr.positionCount) * i;
                    Vector3 pos = curPos * Mathf.Cos(angle);

                    lr.SetPosition(i, pos);

                }

            }
            

        }
        else {
            lr.enabled = false;
        }
    }
}

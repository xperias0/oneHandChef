
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Start is called before the first frame update

    private static Grab m_instance = null;

    Vector3 lastHandPos;

    GameObject hand;

    public float min = 0.01f;

    public float max = 2f;

    float target = 0;

    public float turnSpeed;

    public float sphereRadius;

    bool isMatAdded = false;

    Dictionary<GameObject, List<Material>> objAndMats;

    bool isRecovered = false;

    GameObject touchedObj = null;

    Collider[] colliders;

    Vector3 sphereColPos;

    List<GameObject> touchedSphereObjs;

    HandController hc;

    bool leftGrabed = false;
    [HideInInspector]
    public GameObject grabObject = null;

    Material mat;

    GameObject firstObj;

    Vector3 RotPos = Vector3.zero;

    bool isGrabRot = false;

    float throwTime = 0;

    public float throwSpeed;
    public static Grab Instance
    {
        get
        {
            return m_instance;
        }
    }
    void Start()
    {
        hand = GameObject.Find("WhiteHand");

        mat = GameObject.Find("mat").GetComponent<Renderer>().material;

        firstObj = null;

        target = max;

        lastHandPos = hand.transform.position;

        objAndMats = new Dictionary<GameObject, List<Material>>();

        touchedSphereObjs = new List<GameObject>();

        hc = GameObject.Find("WhiteHand").GetComponent<HandController>();
    }
    private void Update()
    {
        sphereColPos = GameObject.Find("OverlapSphere").transform.position;


        colliders = Physics.OverlapSphere(sphereColPos, sphereRadius, 1 << 7);




        //   Debug.Log("Collider: "+colliders.Length);
        overlapDetector();
    }
    // Update is called once per frame

    void overlapDetector()
    {
        // Debug.Log("dic: " + objAndMats.Count);
        //   Debug.Log("touchedSphereObjs: "+ touchedSphereObjs.Count);
        if (colliders.Length != 0)
        {
            firstObj = colliders[0].gameObject;


          


            foreach (Collider c in colliders)
            {
                addMaterials(c.gameObject);
                if (!touchedSphereObjs.Contains(c.gameObject))
                {
                    touchedSphereObjs.Add(c.gameObject);
                }

                if (Input.GetButton("ControllerA"))
                {
                    throwTime += Time.deltaTime * throwSpeed;
                    Vector3 throwDirection = Camera.main.transform.forward * throwTime;
                    throwLine(firstObj, throwDirection);

                }
                bool isThrow = false;
                if (Input.GetButtonUp("ControllerA"))
                {
                    Vector3 throwDirection = Camera.main.transform.forward * throwTime;
                    GetComponent<LineRenderer>().enabled = false;
                    c.GetComponent<Rigidbody>().AddForce(throwDirection, ForceMode.Impulse);
                    throwTime = 0;
                    isThrow= true;
                }

                if (isThrow) {
                    // c.transform.rotation *= Quaternion.AngleAxis(50 * Time.deltaTime, c.transform.forward);
                    c.transform.Rotate(c.transform.forward,130f);
                }

            }


            // Debug.Log(firstObj.name.Substring(0, 6));
            matGlow();
            if (hc.isLeftGrab)
            {

                if (firstObj && firstObj.tag != "RotObj")
                {
                    firstObj.transform.parent = transform;
                    if (firstObj.gameObject.GetComponent<Rigidbody>())
                    {
                        firstObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        firstObj.GetComponent<Rigidbody>().freezeRotation = true;

                        if (firstObj.tag == "kinematic")
                        {
                            //   firstObj.gameObject.GetComponent<Rigidbody>().isKinematic= true;
                        }
                        resetMats();
                    }

                }
                else
                {
                    // Debug.Log("RotGrab: "+firstObj.name);

                    if (!isGrabRot)
                    {
                        RotPos = transform.position;
                        isGrabRot = true;
                    }

                    Vector3 direction = (transform.position - RotPos).normalized;
                    float angleOfrot = direction.z + direction.x;

                    Debug.Log("Angle: " + angleOfrot);
                    angleOfrot = Mathf.Clamp(angleOfrot, -20f, 180f);

                    firstObj.transform.RotateAround(firstObj.transform.position, firstObj.transform.up, angleOfrot);
                    resetMats();
                }

            }


            if (hc.isGrab)
            {
                foreach (Collider c in colliders)
                {


                    GameObject cur = c.gameObject;
                    if (cur.tag == "RotObj")
                    {

                        if (!isGrabRot)
                        {
                            RotPos = transform.position;
                            isGrabRot = true;
                        }


                        Vector3 direction = (transform.position - RotPos).normalized;
                        float angleOfrot =direction.z+direction.x;

                        Debug.Log("Angle: " + angleOfrot);
                        angleOfrot = Mathf.Clamp(angleOfrot, -20f, 180f);

                        cur.transform.RotateAround(cur.transform.position, cur.transform.up, angleOfrot);
                        resetMats();
                    }
                    else {
                        cur.gameObject.transform.parent = transform;
                        cur.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        cur.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
                    }
                  

                }
                resetMats();
            }

            if (!hc.isGrab && !hc.isLeftGrab)
            {
                foreach (GameObject cur in touchedSphereObjs)
                {

                    if (cur.GetComponent<Rigidbody>() && cur.tag != "RotObj")
                    {
                        cur.gameObject.transform.parent = null;
                        cur.gameObject.GetComponent<Rigidbody>().useGravity = true;
                        cur.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
                        if (cur.tag == "kinematic")
                        {
                            cur.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        }
                    }

                }
                // resetMats();
                isGrabRot = false;
            }



        }

        if (colliders.Length == 0 && objAndMats.Count != 0)
        {
            resetMats();
            //if (touchedSphereObjs.Count!=0) {
            //    foreach (GameObject cur in touchedSphereObjs)
            //    {
            //        if (cur.tag!="RotObj") {
            //            cur.gameObject.transform.parent = null;
            //            cur.gameObject.GetComponent<Rigidbody>().useGravity = true;
            //            cur.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            //        }


            //    }
            //}

            touchedSphereObjs.Clear();

            //Debug.Log("Leave");
        }


        if (colliders.Length == 0 && touchedSphereObjs.Count != 0)
        {
            foreach (GameObject cur in touchedSphereObjs)
            {
                cur.gameObject.transform.parent = null;
                cur.gameObject.GetComponent<Rigidbody>().useGravity = true;
                cur.gameObject.GetComponent<Rigidbody>().freezeRotation = false;

            }
            Debug.Log("ObjLeaves");
            touchedSphereObjs.Clear();
        }

    }


    void addMat(GameObject obj)
    {



        if (obj.transform.childCount == 0 && !objAndMats.ContainsKey(obj))
        {

            List<Material> list = new List<Material>();

            if (!list.Contains(mat))
            {
                list.Add(mat);
            }
            Material[] mats = obj.GetComponent<Renderer>().materials;

            foreach (Material m in mats)
            {
                string s = m.name.Substring(0, 4);
                if (!s.Equals("glow"))
                {
                    list.Add(m);
                }

            }
            objAndMats.Add(obj, list);

            obj.GetComponent<Renderer>().materials = objAndMats[obj].ToArray();
        }

        if (obj.transform.childCount > 0)
        {
            if (obj.GetComponent<Renderer>())
            {
                List<Material> list = new List<Material>();
                list.Add(mat);
                foreach (Material m in obj.GetComponent<Renderer>().materials)
                {
                    string s = m.name.Substring(0, 4);
                    if (!s.Equals("glow"))
                    {
                        list.Add(m);
                    }
                    obj.GetComponent<Renderer>().materials = list.ToArray();
                }
            }
            else
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject t = obj.transform.GetChild(i).gameObject;
                    List<Material> list = new List<Material>();
                    if (t.tag == "childPart" && !objAndMats.ContainsKey(t))
                    {
                        if (!list.Contains(mat))
                        {
                            list.Add(mat);
                        }
                        foreach (Material m in t.GetComponent<Renderer>().materials)
                        {
                            string s = m.name.Substring(0, 4);
                            if (!s.Equals("glow"))
                            {
                                list.Add(m);
                            }
                        }

                        objAndMats.Add(t, list);
                        t.GetComponent<Renderer>().materials = list.ToArray();
                    }
                }
            }




        }


        isMatAdded = true;

        Debug.Log("Added: " + obj.name);




    }


    void addMaterials(GameObject obj) {
        
        rebuildMaterialsArray(obj);
        for (int i=0;i<obj.transform.childCount;i++) {
            GameObject cur = obj.transform.GetChild(i).gameObject;
            rebuildMaterialsArray(cur);
        }
        isMatAdded = true;
    }

    void rebuildMaterialsArray(GameObject obj) {
        if (obj.GetComponent<Renderer>() && !objAndMats.ContainsKey(obj)) {
            List<Material> list = new List<Material>();
            list.Add(mat);
            foreach (Material m in obj.GetComponent<Renderer>().materials)
            {
                list.Add(m);
            }
            obj.GetComponent<Renderer>().materials = list.ToArray();
            objAndMats.Add(obj, list);
        }

        
    }

    void matGlow()
    {
        if (isMatAdded)
        {
            foreach (GameObject b in objAndMats.Keys)
            {

                Material curMat = b.GetComponent<Renderer>().material;

                string s = curMat.name.Substring(0, 4);
                if (s == "glow")
                {
                    float indes = Mathf.Lerp(curMat.GetFloat("_Emiss"), target, Time.deltaTime * turnSpeed);
                    if (Mathf.Abs(target - curMat.GetFloat("_Emiss")) < 0.02f)
                    {
                        target = target == max ? min : max;
                    }
                    curMat.SetFloat("_Emiss", indes);
                }
            }


        }


    }
    void resetMat(GameObject obj)
    {
        if (isMatAdded)
        {
            if (obj.transform.childCount == 0)
            {
                foreach (GameObject b in objAndMats.Keys)
                {
                    List<Material> curList = objAndMats[b];
                    if (curList.Count > 1)
                    {
                        objAndMats[b].RemoveAt(0);
                    }

                }

                obj.GetComponent<Renderer>().materials = objAndMats[obj].ToArray();
            }

            else
            {
                foreach (GameObject b in objAndMats.Keys)
                {
                    List<Material> curList = objAndMats[b];
                    if (curList.Count > 1)
                    {
                        objAndMats[b].RemoveAt(0);
                    }
                    b.GetComponent<Renderer>().materials = objAndMats[b].ToArray();
                }
            }

            //   objAndMats.Clear();
            isRecovered = true;
            isMatAdded = false;
            // Debug.Log("Clear");
        }


    }



    void resetMats()
    {
        foreach (GameObject b in objAndMats.Keys)
        {
            objAndMats[b].RemoveAt(0);
            b.GetComponent<Renderer>().materials = objAndMats[b].ToArray();
        }
        isRecovered = false;
        isMatAdded = false;
        isGrabRot = true;
        // touchedSphereObjs.Clear();
        objAndMats.Clear();

    }

    public void throwLine(GameObject b, Vector3 direction)
    {
        GetComponent<LineRenderer>().enabled = true;
        float timer = 0.1f;
        LineRenderer lr = GetComponent<LineRenderer>();

        for (int i = 0; i < lr.positionCount; i++)
        {
            float curTime = i * timer;
            Vector3 pos = transform.position + direction * curTime + 0.5f * -9.8f * Vector3.up * (curTime * curTime);
            lr.SetPosition(i, pos);

        }
        //   b.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

    }
}
